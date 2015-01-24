using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AO3_Formatter
{
    public static class CellPhones
    {
        private static List<CellPhone> s_phones = new List<CellPhone>()
        {
            new CellPhone()
            {
                Name = "Hiccup",
                PhoneType = PhoneType.iPhone,
                Initials = "H"
            },
            new CellPhone()
            {
                Name = "Jackson",
                PhoneType = PhoneType.Android,
                Initials = "J"
            },
            new CellPhone()
            {
                Name = "Emma",
                PhoneType = PhoneType.iPhone,
                Initials = "E"
            },
            new CellPhone()
            {
                Name = "Merida",
                PhoneType = PhoneType.iPhone,
                Initials = "M"
            }
        };

        public static CellPhone Retrieve(string initials)
        {
            return s_phones.FirstOrDefault(phone => phone.Initials.Equals(initials, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
