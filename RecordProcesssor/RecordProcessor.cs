using RecordProcesssor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.IO.Abstractions;

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
                if(_records != null)
                {
                    return null;
                }
                return null;
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

                if (line1.IndexOf(' ') != -1 || line1.IndexOf('\t') != -1)
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

        public void ProcessRecord()
        {

        }

    }
}
