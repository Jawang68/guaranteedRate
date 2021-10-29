using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using RecordProcesssor.Model;
using System.IO;
using System;

namespace PersonRecordService
{
    public class RecordRepositoryBase
    {
        protected static List<PersonRecord> _records;
        protected static Dictionary<SeparatorType, char[]> SeparatorMap { get; } = new Dictionary<SeparatorType, char[]>() {
            {SeparatorType.Comma,  new char[] { ',' } },
            {SeparatorType.Space, new char[]{' '} },
            {SeparatorType.Pipe, new char[] {'|'} }
        };

        protected SeparatorType GetSeparatorFromLine(string line)
        {
            if (line.IndexOf(',') != -1)
            {
                return SeparatorType.Comma;
            }

            if (line.IndexOf(' ') != -1)
            {
                return SeparatorType.Space;
            }

            if (line.IndexOf('|') != -1)
            {
                return SeparatorType.Pipe;
            }

            throw new ArgumentException($"{line} does not contain valid separator");
        }

        public List<PersonRecord> GetRecords(string orderBy)
        {
            if(string.IsNullOrEmpty(orderBy))
                return _records;

            return _records.AsQueryable()
                         .OrderBy(orderBy).ToList();
        }

        public virtual void SaveRecord(string line)
        {
            string[] fields = null;
            var separator = GetSeparatorFromLine(line);
            var fieldSeparator = SeparatorMap[separator];

            if (separator == SeparatorType.Space)
            {
                fields = line.Split(fieldSeparator, System.StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                fields = line.Split(fieldSeparator);
            }
            _records.Add(new PersonRecord
            {
                LastName = fields.Length > 0 ? fields[0] : string.Empty,
                FirstName = fields.Length > 1 ? fields[1] : string.Empty,
                Email = fields.Length > 2 ? fields[2] : string.Empty,
                FavoriteColor = fields.Length > 3 ? fields[3] : string.Empty,
                DateOfBirth = fields.Length > 4 ? DateTime.Parse(fields[4]) : default,
            });
        }
    }
}
