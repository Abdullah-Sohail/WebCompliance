using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Compliance.Dashboard.Domain.ValueTypes
{
    public class Phone
    {
        public long? Value { get; private set; }

        public Phone()
        {
        }

        public Phone(long phone)
        {
            var toTest = phone.ToString();

            if (toTest.Length > 10)
                phone = long.Parse(toTest.Substring(toTest.Length - 10));

            Value = phone;
        }

        public Phone(string phone)
        {
            try
            {
                var cleaned = Regex.Replace(phone, @"[^\d]", "");
                long longPhone = 0;

                long.TryParse(cleaned.Substring(Math.Max(cleaned.Length, 10) - 10), out longPhone);

                Value = longPhone;
            }
            catch (Exception)
            {
            }
            
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public string ToString(bool formatted)
        {
            //TODO: handle 5/7/10 digit phone display
            return Value.ToString();
        }
    }
}
