using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compliance.Dashboard.Domain.ValueTypes
{
    public class EmailAddress
    {
        public string Domain { get; private set; }
        public string Username { get; private set; }

        private EmailAddress() { }

        public EmailAddress(string email)
        {
            //TODO: E-mail: Better exception reporting
            try
            {
                var pieces = email.Split('@');

                Username = pieces[0];
                Domain = pieces[1];
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public override string ToString()
        {
            return String.Format("{0}@{1}", Username, Domain);
        }
    }
}
