using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordProcesssor.Model
{
    public class PersonRecord
    {
        public String LastName { get; set;}
        public String FirstName { get; set;}
        public String Email { get; set; }
        public String FavoriteColor { get; set;}
        public DateTime  DateOfBirth { get; set;}
    }

    public class PersonRecordEquityComparer : IEqualityComparer<PersonRecord>
    {
        public bool Equals(PersonRecord left, PersonRecord right)
        {
            return string.Compare(left.FirstName, right.FirstName, true) == 0 &&
               string.Compare(left.LastName, right.LastName, true) == 0 &&
               string.Compare(left.Email, right.Email, true) == 0 &&
               string.Compare(left.FavoriteColor, right.FavoriteColor, true) == 0 &&
               left.DateOfBirth == right.DateOfBirth;
        }

        public int GetHashCode(PersonRecord record)
        {
            var hCode = record.LastName.ToLower() + record.FirstName.ToLower() + record.DateOfBirth.ToString();
            return hCode.GetHashCode();
        }
    }

}
