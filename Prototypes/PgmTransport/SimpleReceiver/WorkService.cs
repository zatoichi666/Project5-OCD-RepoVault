using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace SimpleReceiver
{
   [ServiceContract(SessionMode = SessionMode.NotAllowed)]
   public interface IWorkService
   {
      [OperationContract(IsOneWay = true)]
      void DoWork(string message);
   }

   [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
   class WorkService : IWorkService
   {
      private int counter = 0;

      #region IWorkService Members
    
      public void DoWork(string message)
      {
         Console.Write("Recieved " + counter++ + " messages\r");
      }

      #endregion
   }
}
