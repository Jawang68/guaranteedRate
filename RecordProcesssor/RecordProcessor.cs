using RecordProcesssor.Model;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.Abstractions;
using System.Linq.Dynamic;

namespace RecordProcesssor
{
    public class RecordProcessor
    {
        readonly IFileSystem fileSystem;

        readonly string fileName;
        private List<PersonRecord> _records;
        private SeparatorType? _separator;
        
        public RecordProcessor(IFileSystem fileSystem, string fileName)
        {
            this.fileSystem = fileSystem;
            this.fileName = fileName;
            _separator = null;
            _records = null;
        }

        public RecordProcessor(string fileName): this(fileSystem: new FileSystem(), fileName)
        {
        }

        
        public List<PersonRecord> Records
        {
            get
            {
               return _records??GetRecords();
            }
            private set { }
        }


        public SeparatorType? Separator
        {
            get
            {
                return _separator??GetSeparator();
            }
            private set { }
        }


        protected SeparatorType? GetSeparator()
        {
            if(fileName == null)
                return null;
            try
            {

                string line1 = fileSystem.File.ReadLines(fileName).First();

                if(line1.IndexOf(',') != -1)
                {
                    return SeparatorType.Comma;
                }

                if (line1.IndexOf(' ') != -1)
                {
                    return SeparatorType.Space;
                }

                if (line1.IndexOf('|') != -1)
                {
                    return SeparatorType.Pipe;
                }

                return null;
            }
            catch
            {
                throw new InvalidDataException($"Input file {fileName} is corrupted");
            }
        }

        protected List<PersonRecord> GetRecords()
        {
            return new List<PersonRecord>();
        }

        public List<PersonRecord> ProcessRecords()
        {
            char[] fieldSeparator = null;
            switch(Separator)
            {
                case SeparatorType.Comma:
                    fieldSeparator = new char[] {','};
                    break;
                case SeparatorType.Pipe:
                    fieldSeparator = new char[] { '|' };
                    break;
                case SeparatorType.Space:
                    fieldSeparator = new char[] { ' ' };
                    break;
                default:
                    throw new InvalidDataException("Invalid separators");
            }

            _records = new List<PersonRecord>();
            foreach ( var lines in fileSystem.File.ReadAllLines(fileName))
            {
                string[] fields = null;
                if(Separator == SeparatorType.Space)
                {
                    fields = lines.Split(fieldSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    fields = lines.Split(fieldSeparator);
                }
                _records.Add( new PersonRecord
                {
                    LastName = fields.Length > 0? fields[0] : string.Empty,
                    FirstName = fields.Length > 1 ? fields[1] : string.Empty,
                    Email = fields.Length > 2 ? fields[2] : string.Empty,
                    FavoriteColor = fields.Length > 3 ? fields[3] : string.Empty,
                    DateOfBirth = fields.Length > 4 ? System.DateTime.Parse(fields[4]) : default(System.DateTime),
                });;;
            }
            return _records;
        }

        public List<PersonRecord> DisplayRecords(string orderBy)
        {
            return _records.AsQueryable()
                           .OrderBy(orderBy).ToList();
        }
    }
}
