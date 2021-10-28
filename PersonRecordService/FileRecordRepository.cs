using RecordProcesssor.Model;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.Abstractions;
using System.Linq.Dynamic;
using PersonRecordService.Interfaces;

namespace PersonRecordService
{
    public class FileRecordRepository : IRecordRepository
    {
        readonly IFileSystem fileSystem;

        private string _fileName;
        private List<PersonRecord> _records;
        private SeparatorType? _separator;

        public FileRecordRepository(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
            
            _separator = null;
            _records = null;
        }

        public FileRecordRepository() : this(fileSystem: new FileSystem())
        {
        }

        public List<PersonRecord> GetRecords(string orderBy)
        {
            if(string.IsNullOrEmpty(orderBy))
                return _records;

            return _records.AsQueryable()
                         .OrderBy(orderBy).ToList();
        }

        public void SaveRecord(string fileName)
        {
            _fileName = fileName;
            char[] fieldSeparator = null;

            var separator = GetSeparator(fileName);
            switch (separator)
            {
                case SeparatorType.Comma:
                    fieldSeparator = new char[] { ',' };
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
            foreach (var lines in fileSystem.File.ReadAllLines(_fileName))
            {
                string[] fields = null;
                if (separator == SeparatorType.Space)
                {
                    fields = lines.Split(fieldSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    fields = lines.Split(fieldSeparator);
                }
                _records.Add(new PersonRecord
                {
                    LastName = fields.Length > 0 ? fields[0] : string.Empty,
                    FirstName = fields.Length > 1 ? fields[1] : string.Empty,
                    Email = fields.Length > 2 ? fields[2] : string.Empty,
                    FavoriteColor = fields.Length > 3 ? fields[3] : string.Empty,
                    DateOfBirth = fields.Length > 4 ? System.DateTime.Parse(fields[4]) : default,
                }); 
            }
            return;
        }

        public SeparatorType? GetSeparator(string fileName)
        {
            if (fileName == null)
                return null;
            try
            {
                string line1 = fileSystem.File.ReadLines(fileName).First();

                if (line1.IndexOf(',') != -1)
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
    }
}
