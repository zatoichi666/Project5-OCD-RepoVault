using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ComponentModel;

namespace LatestNews
{
   [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession,
                    UseSynchronizationContext = true)]
   public class NewsUpdater : INewsUpdate
   {
      public static event EventHandler<UpdateEventArgs> Updated;

      public NewsUpdater()
      {
         Console.WriteLine("Service instance created....");
      }
      #region INewsUpdate Members

      public void Update(Scoop scoop)
      {
         if (Updated != null)
         {
            Updated(this, new UpdateEventArgs(scoop));
         }
      }

      #endregion
   }

   public class UpdateEventArgs : EventArgs
   {
      public UpdateEventArgs(Scoop update)
      {
         this.Update = update;
      }

      public Scoop Update { get; private set; }
   }
}
