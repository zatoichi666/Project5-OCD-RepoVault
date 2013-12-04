using System;
using System.Configuration;

namespace PgmTransport.Configuration
{
   [AttributeUsage(AttributeTargets.Property)]
   public class IpValidatorAttribute : ConfigurationValidatorAttribute
   {
      string m_regex;
      ConfigurationValidatorBase m_instance = null;

      public IpValidatorAttribute(string regex)
      {
         m_regex = regex;
      }

      public override ConfigurationValidatorBase ValidatorInstance
      {
         get
         {
            if (m_instance == null)
            {
               m_instance = new IpValidator(m_regex);
            }
            return m_instance;
         }
      }
   }
}
