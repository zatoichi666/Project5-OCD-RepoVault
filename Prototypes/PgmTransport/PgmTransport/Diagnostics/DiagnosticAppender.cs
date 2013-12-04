using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace PgmTransport.Diagnostics
{
   class DiagnosticAppender : IDisposable
   {
      StreamWriter m_writer;
      IDiagnose m_source;

      public DiagnosticAppender(IDiagnose source, string path)
      {
         string fullPath = Path.Combine(path, source.ID + ".log");
         m_source = source;
         m_writer = new StreamWriter(fullPath, true);
         m_writer.AutoFlush = true;
      }

      public void Append()
      {
         object stats = m_source.GetStatistics();
         if (stats != null)
         {
            m_writer.WriteLine(StructFormatter.GetFormattedObject(stats));
         }
      }

      #region IDisposable Members

      public void Dispose()
      {
         m_writer.Dispose();
      }

      #endregion

      private class StructFormatter
      {
         private static Dictionary<Type, FieldInfo[]> mapper = new Dictionary<Type, FieldInfo[]>();

         public static string GetFormattedObject(object obj)
         {
            Type type = obj.GetType();

            if (!mapper.ContainsKey(type))
            {
               mapper.Add(type, obj.GetType().GetFields());
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(DateTime.Now.ToString());
            FieldInfo[] fields = mapper[type];

            foreach (var field in fields)
            {
               sb.AppendFormat("{0} = {1}", field.Name, field.GetValue(obj));
               sb.AppendLine();
            }

            sb.AppendLine();
            return sb.ToString();
         }
      }
   }
}
