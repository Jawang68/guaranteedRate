using NUnit.Framework;
using System.IO.Abstractions.TestingHelpers;
using System.Collections.Generic;
using RecordProcesssor;
using System.IO;
using System;
using RecordProcesssor.Model;
using Newtonsoft.Json;
using System.Linq;

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

        #region ProcessorRecord tests
        [Test]
        public void ProcessRecord_CommaSeparated_GetAllFields()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { fileName, new MockFileData("Wang,James,Jawang68@gmail.com,Blue,7/6/1968")},
            });

            var expectedRecords = TestData.OneRecord;

            //Test
            var processor = new RecordProcessor(fileSystem, fileName);
            var actualRecords = processor.ProcessRecord();

            //Verify
            Assert.IsTrue(Enumerable.SequenceEqual(expectedRecords, actualRecords, new PersonRecordEquityComparer()));
        }

        [Test]
        public void ProcessRecord_SeparatedWithMultipleSpaces_GetAllFields()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            { 
                { fileName, new MockFileData("Wang  James Jawang68@gmail.com  Blue   7/6/1968")},
            });

            var expectedRecords = TestData.OneRecord;

            //Test
            var processor = new RecordProcessor(fileSystem, fileName);
            var actualRecords = processor.ProcessRecord();

            //Verify
            Assert.IsTrue(Enumerable.SequenceEqual(expectedRecords, actualRecords, new PersonRecordEquityComparer()));
        }

        [Test]
        public void ProcessRecord_SeparatedWithPipe_GetAllFields()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { fileName, new MockFileData("Wang|James|Jawang68@gmail.com|Blue|7/6/1968")},
            });

            var expectedRecords = TestData.OneRecord;

            //Test
            var processor = new RecordProcessor(fileSystem, fileName);
            var actualRecords = processor.ProcessRecord();

            //Verify
            Assert.IsTrue(Enumerable.SequenceEqual(expectedRecords, actualRecords, new PersonRecordEquityComparer()));
        }

        [Test]
        public void ProcessRecord_WithMissingFields_SetMissingFieldsToDefault()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { fileName, new MockFileData("|James|Jawang68@gmail.com|Blue|7/6/1968\n" +
                                            "Wang|James|Jawang68@gmail.com||7/6/1968\n" +
                                            "Wang|James|Jawang68@gmail.com|Blue") }
            });
            //Test
            var processor = new RecordProcessor(fileSystem, fileName);
            var actualRecords = processor.ProcessRecord().ToArray();

            //Verify
            Assert.Multiple( () =>
                {
                    Assert.AreEqual(string.Empty, actualRecords[0].LastName);
                    Assert.AreEqual(string.Empty, actualRecords[1].FavoriteColor);
                    Assert.AreEqual(default(DateTime), actualRecords[2].DateOfBirth);
                }
            );
        }

        [Test]
        public void ProcessRecord_WithDifferentDateFormat_SetDateOfBirthField()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { fileName, new MockFileData("Wang|James|Jawang68@gmail.com|Blue|7/6/1968\n" +
                                            "Wang|James|Jawang68@gmail.com|Blue|1968/7/6\n" +
                                            "Wang|James|Jawang68@gmail.com|Blue|07/06/1968\n" +
                                            "Wang|James|Jawang68@gmail.com|Blue|1968-7-6"
                                            )}
            });
            //Test
            var processor = new RecordProcessor(fileSystem, fileName);
            var actualRecords = processor.ProcessRecord().ToArray();

            //Verify
            foreach(var record in actualRecords)
            {
                Assert.AreEqual(DateTime.Parse("7/6/1968"), record.DateOfBirth);
            }
        }

        [Test]
        public void ProcessRecord_WithEmptyRecord_SetAllFieldsToDefault()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { fileName, new MockFileData("|")}
            });

            //Test
            var processor = new RecordProcessor(fileSystem, fileName);
            var actualRecord = processor.ProcessRecord().First();

            //Verify
            Assert.Multiple(() =>
                {
                    Assert.AreEqual(string.Empty, actualRecord.LastName);
                    Assert.AreEqual(string.Empty, actualRecord.FirstName);
                    Assert.AreEqual(string.Empty, actualRecord.Email);
                    Assert.AreEqual(string.Empty, actualRecord.FavoriteColor);
                    Assert.AreEqual(default(DateTime), actualRecord.DateOfBirth);
                }
            );
        }

        [Test]
        public void ProcessRecord_WithMultipleRecords_SetMultipleRecords()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {fileName, new MockFileData("Wang|James|Jawang68@gmail.com|Blue|7/6/1968\n" +
                                            "Lee|Jane|janeLee@yahoo.com|Pink|1988/9/16")
                }
            });
            var expectedRecords = TestData.TwoRecords;

            //Test
            var processor = new RecordProcessor(fileSystem, fileName);
            var actualRecords = processor.ProcessRecord();

            //Verify
            Assert.IsTrue(Enumerable.SequenceEqual(expectedRecords, actualRecords, new PersonRecordEquityComparer()));
        }

        [Test]
        public void ProcessRecord_WithInvalidDoB_ThrowFormatException()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { fileName, new MockFileData("Wang|James|Jawang68@gmail.com|Blue|7/6/19x8")}
            });
            
            var expectedMessage = $"Input file {fileName} is corrupted";
            var processor = new RecordProcessor(fileSystem, fileName);

            // Verify
            var ex = Assert.Throws<FormatException>(() => { processor.ProcessRecord(); });
        }
        #endregion
    }
}