using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace LatestNews
{
   [ServiceContract(SessionMode = SessionMode.Required)]
   public interface INewsUpdate
   {
      [OperationContract(IsOneWay = true)]
      void Update(Scoop scoop);
   }

   [DataContract]
   public class Scoop
   {
      [DataMember]
      public DateTime Originated;

      [DataMember]
      public string Source;

      [DataMember]
      public string Description;
   }
}
