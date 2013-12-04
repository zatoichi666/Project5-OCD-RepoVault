using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;

namespace PgmTransport.Diagnostics
{
   class DiagnosticsManager : IDisposable
   {
      static readonly DiagnosticsManager m_instance = new DiagnosticsManager();
      Timer m_timer = new Timer();
      SynchronizedCollection<DiagnosticAppender> m_writers = new SynchronizedCollection<DiagnosticAppender>();
      string m_location;
      bool m_configured = false;

      private DiagnosticsManager()
      {
         m_timer.Elapsed += new ElapsedEventHandler(m_timer_Elapsed);
      }

      public static DiagnosticsManager Instance
      {
         get
         {
            return m_instance;
         }
      }

      public void Configure(DiagnosticsConfiguration config)
      {
         if (!config.Enabled)
         {
            return;
         }

         m_timer.Interval = config.Interval.TotalMilliseconds;
         m_location = config.OutputDirectory;

         if (!Directory.Exists(m_location))
         {
            Directory.CreateDirectory(m_location);
         }
         m_configured = true;

         Start();
      }

      public void AddWriter(IDiagnose writer)
      {
         if (!m_configured)
         {
            return;
         }

         lock (m_writers.SyncRoot)
         {
            DiagnosticAppender appender = new DiagnosticAppender(writer, m_location);
            m_writers.Add(appender); 
         }
      }

      void m_timer_Elapsed(object sender, ElapsedEventArgs e)
      {
         lock (m_writers.SyncRoot)
         {
            foreach (var item in m_writers)
            {
               item.Append();
            } 
         }
      }

      public void Start()
      {
         m_timer.Start();
      }

      public void Stop()
      {
         m_timer.Stop();
      }

      #region IDisposable Members

      public void Dispose()
      {
         m_timer.Dispose();

         lock (m_writers.SyncRoot)
         {
            foreach (var item in m_writers)
            {
               item.Dispose();
            } 
         }
      }

      #endregion      
   }
}
