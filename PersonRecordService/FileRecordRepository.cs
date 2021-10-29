using RecordProcesssor.Model;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.Abstractions;

using PersonRecordService.Interfaces;
using System;

namespace PersonRecordService
{
    public class FileRecordRepository : RecordRepositoryBase, IRecordRepository
    {
        readonly IFileSystem fileSystem;
        private string _fileName;

        public FileRecordRepository(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
            _records = null;
        }

        public FileRecordRepository() : this(fileSystem: new FileSystem())
        {
        }

        public override void SaveRecord(string fileName)
        {
            _fileName = fileName;
            _records = new List<PersonRecord>();

            foreach (var line in fileSystem.File.ReadAllLines(_fileName))
            {
                base.SaveRecord(line);
            }
            return;
        }

        public SeparatorType GetSeparator(string fileName)
        {
            try
            {
                string line1 = fileSystem.File.ReadLines(fileName).First();
                return GetSeparatorFromLine(line1);
            }
            catch(ArgumentException e)
            {
                throw e;
            }
            catch
            {
                throw new InvalidDataException($"Input file {fileName} is corrupted");
            }
        }
    }
}
