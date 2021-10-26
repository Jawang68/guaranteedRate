using NUnit.Framework;
using System.IO.Abstractions.TestingHelpers;
using System.Collections.Generic;
using RecordProcesssor;
using System.IO;

namespace RecordProcessorTest
{
    public class Tests
    {
        private const string fileName = @"c:\myfile.txt";

        [SetUp]
        public void Setup()
        {
        }

        #region GetSeparator Tests
        [Test]
        public void GetSeparator_IsSeparatedByPipe_ReturnPipe()
        {
            // Setup
            var fileSystem = new MockFileSystem( new Dictionary<string, MockFileData>
            {
                { fileName, new MockFileData("Wang|James|Jawang68@gmail.com|Blue|7/6/1968")},
            });

            //Test
            var processor = new RecordProcessor(fileSystem, fileName);
            var separator = processor.Separator;

            // Verify
            Assert.AreEqual(SeparatorType.Pipe, separator);
        }

        [Test]
        public void GetSeparator_IsSeparatedByComma_ReturnComma()
        {
            // Setup
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { fileName, new MockFileData("Wang,James,Jawang68@gmail.com,Blue,7/6/1968")},
            });

            //Test
            var processor = new RecordProcessor(fileSystem, fileName);
            
            // Verify
            Assert.AreEqual(SeparatorType.Comma, processor.Separator);
        }

        [Test]
        public void GetSeparator_IsSeparatedBySpace_ReturnSpace()
        {
            // Setup
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { fileName, new MockFileData("Wang  James Jawang68@gmail.com Blue 7/6/1968")},
            });

            //Test
            var processor = new RecordProcessor(fileSystem, fileName);
            
            // Verify
            Assert.AreEqual(SeparatorType.Space, processor.Separator);
        }

        [Test]
        public void GetSeparator_IsSeparatedByTab_ReturnSpace()
        {
            // Setup
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { fileName, new MockFileData("Wang\tJames\tJawang68@gmail.com\tBlue\t7/6/1968")},
            });

            //Test
            var processor = new RecordProcessor(fileSystem, fileName);
            
            // Verify
            Assert.AreEqual(SeparatorType.Space, processor.Separator);
        }
        
        [Test]
        public void GetSeparator_FileNameIsNull_ReturnNull()
        {
            var processor = new RecordProcessor(null);
            
            // Verify
            Assert.IsNull(processor.Separator);
        }

        [Test]
        public void  GetSeparator_FileNameIsInvalid_ThrowInvalidDataException()
        {
            // Setup
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\aDifferentFile.txt", new MockFileData("Wang\tJames\tJawang68@gmail.com\tBlue\t7/6/1968")},
            });

            var expectedMessage = $"Input file {fileName} is corrupted";

            var processor = new RecordProcessor(fileSystem, fileName);


            // Verify
            var ex = Assert.Throws<InvalidDataException>(() => {var separator = processor.Separator;});
            Assert.AreEqual(expectedMessage, ex.Message);
        }

        #endregion
    }
}