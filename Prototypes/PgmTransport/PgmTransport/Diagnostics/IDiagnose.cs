
namespace PgmTransport.Diagnostics
{
   interface IDiagnose
   {
      string ID { get; }
      object GetStatistics();
   }
}
