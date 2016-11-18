using System;
using System.Collections.Generic;
using System.Text;

namespace Banlinea.Framework.DatabaseTools.MetaModeler.Exceptions
{
    public class MissingDataFieldException : MissingFieldException
    {
        public MissingDataFieldException(IEnumerable<string> missingFields)
            : base(BuildMessage(missingFields))
        {
            //Default Constructor
        }

        private static string BuildMessage(IEnumerable<string> missingFields)
        {
            var messageSb = new StringBuilder("The values for the following required fields have not been specified: ");
            foreach (var field in missingFields)
            {
                messageSb.AppendLine(field);
            }
            return messageSb.ToString();
        }
    }
}