using ModelLayer.Models.Base;
using System.Text;

namespace ModelLayer.Models
{
    public class Address : BaseEntity
    {
        public string City { get; set; }
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public string Zipcode { get; set; }
        public string Country { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Land: " + Country);
            sb.AppendLine();
            sb.Append("PLZ: " + Zipcode);
            sb.AppendLine();
            sb.Append("Stadt: " + City);
            sb.AppendLine();
            sb.Append("Strasse: " + Street + " " + StreetNumber);
            return sb.ToString();
        }
    }
}