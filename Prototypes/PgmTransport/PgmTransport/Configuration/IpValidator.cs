using System.Configuration;

namespace PgmTransport.Configuration
{
   public class IpValidator : RegexStringValidator
   {
      public IpValidator(string regex)
         : base(regex)
      {
      }

      public override void Validate(object value)
      {
         string str = value.ToString();

         if (string.IsNullOrEmpty(str))
         {
            return;
         }

         base.Validate(value);
      }
   }
}
