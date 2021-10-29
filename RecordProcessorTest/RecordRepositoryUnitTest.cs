using NUnit.Framework;
using System.IO.Abstractions.TestingHelpers;
using System.Collections.Generic;
using System.IO;
using System;
using RecordProcesssor.Model;
using System.Linq;
using PersonRecordService;
using RecordProcessorTest;

namespace FileRecordRepositoryTest 
{
    public class RecordRepositoryTests : RecordRepositoryBase
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
            var repo = new FileRecordRepository(fileSystem);
            var separator = repo.GetSeparator(fileName);

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
            var repo = new FileRecordRepository(fileSystem);
            var separator = repo.GetSeparator(fileName);

            // Verify
            Assert.AreEqual(SeparatorType.Comma, separator);
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
            var repo = new FileRecordRepository(fileSystem);
            var separator = repo.GetSeparator(fileName);

            // Verify
            Assert.AreEqual(SeparatorType.Space, separator);
        }

        [Test]
        public void  GetSeparator_FileNameIsInvalid_ThrowInvalidDataException()
        {
            // Setup
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\DifferentFile.txt", new MockFileData("Wang\tJames\tJawang68@gmail.com\tBlue\t7/6/1968")},
            });

            var expectedMessage = $"Input file {fileName} is corrupted";
            var repo = new FileRecordRepository(fileSystem);

            // Verify
            var ex = Assert.Throws<InvalidDataException>(() => {var separator = repo.GetSeparator(fileName);});
            Assert.AreEqual(expectedMessage, ex.Message);
        }

        #endregion

        #region SaveRecord tests
        [Test]
        public void SaveRecord_CommaSeparated_GetAllFields()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { fileName, new MockFileData("Wang,James,Jawang68@gmail.com,Blue,7/6/1968")},
            });

            var expectedRecords = TestData.OneRecord;

            //Test
            var repo = new FileRecordRepository(fileSystem);
            repo.SaveRecord(fileName);
            var actualRecords = repo.GetRecords(null);

            //Verify
            Assert.IsTrue(Enumerable.SequenceEqual(expectedRecords, actualRecords, new PersonRecordEquityComparer()));
        }

        [Test]
        public void SaveRecord_SeparatedWithMultipleSpaces_GetAllFields()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            { 
                { fileName, new MockFileData("Wang  James Jawang68@gmail.com  Blue   7/6/1968")},
            });

            var expectedRecords = TestData.OneRecord;

            //Test
            var repo = new FileRecordRepository(fileSystem);
            repo.SaveRecord(fileName);
            var actualRecords = repo.GetRecords(null);

            //Verify
            Assert.IsTrue(Enumerable.SequenceEqual(expectedRecords, actualRecords, new PersonRecordEquityComparer()));
        }

        [Test]
        public void SaveRecord_SeparatedWithPipe_GetAllFields()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { fileName, new MockFileData("Wang|James|Jawang68@gmail.com|Blue|7/6/1968")},
            });

            var expectedRecords = TestData.OneRecord;

            //Test
            var repo = new FileRecordRepository(fileSystem);
            repo.SaveRecord(fileName);
            var actualRecords = repo.GetRecords(null);

            //Verify
            Assert.IsTrue(Enumerable.SequenceEqual(expectedRecords, actualRecords, new PersonRecordEquityComparer()));
        }

        [Test]
        public void SaveRecord_WithMissingFields_SetMissingFieldsToDefault()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { fileName, new MockFileData("|James|Jawang68@gmail.com|Blue|7/6/1968\n" +
                                            "Wang|James|Jawang68@gmail.com||7/6/1968\n" +
                                            "Wang|James|Jawang68@gmail.com|Blue") }
            });

            //Test
            var repo = new FileRecordRepository(fileSystem);
            repo.SaveRecord(fileName);
            var actualRecords = repo.GetRecords(null);

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
        public void SaveRecord_WithDifferentDateFormat_SetDateOfBirthField()
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
            var repo = new FileRecordRepository(fileSystem);
            repo.SaveRecord(fileName);
            var actualRecords = repo.GetRecords(null);

            //Verify
            foreach (var record in actualRecords)
            {
                Assert.AreEqual(DateTime.Parse("7/6/1968"), record.DateOfBirth);
            }
        }

        [Test]
        public void SaveRecord_WithEmptyRecord_SetAllFieldsToDefault()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { fileName, new MockFileData("|")}
            });

            //Test
            var repo = new FileRecordRepository(fileSystem);
            repo.SaveRecord(fileName);
            var actualRecord = repo.GetRecords(null).First();

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
        public void SaveRecord_WithMultipleRecords_SetMultipleRecords()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {fileName, new MockFileData("Wang|James|Jawang68@gmail.com|Blue|7/6/1968\n" +
                                            "Lee|Jane|janeLee@yahoo.com|Pink|1988/9/16")
                }
            });
            var expectedRecords = TestData.TwoRecords;

            //Test
            var repo = new FileRecordRepository(fileSystem);
            repo.SaveRecord(fileName);
            var actualRecords = repo.GetRecords(null);

            //Verify
            Assert.IsTrue(Enumerable.SequenceEqual(expectedRecords, actualRecords, new PersonRecordEquityComparer()));
        }

        [Test]
        public void SaveRecord_WithInvalidDoB_ThrowFormatException()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { fileName, new MockFileData("Wang|James|Jawang68@gmail.com|Blue|7/6/19x8")}
            });
            
            var expectedMessage = $"Input file {fileName} is corrupted";
            var repo = new FileRecordRepository(fileSystem);

            // Verify
            var ex = Assert.Throws<FormatException>(() => { repo.SaveRecord(fileName); });
        }
        #endregion

        #region GetRecords tests
        [Test]
        public void GetRecords_OrderByDoBAscending_DisplayRecordsInOrder()
        {
            //Setup 
            var repo = new RecordRepositoryBase();
            RecordRepositoryBase._records = TestData.TwoRecords;

            //Test    
            var results = repo.GetRecords("DateOfBirth");

            //Verify
            Assert.AreEqual(DateTime.Parse("7/6/1968"), results.First().DateOfBirth);
        }

        [Test]
        public void GetRecords_OrderByColorThenLastName_DisplayRecordsInOrder()
        {
            //Setup 
            var repo = new RecordRepositoryBase();
            RecordRepositoryBase._records = TestData.ThreeRecords;

            var expected = new List<PersonRecord> 
            {
                 new PersonRecord
                {
                    LastName = "Gates",
                    FirstName = "Bill",
                    Email = "billGates@microsoft.com",
                    FavoriteColor = "Blue",
                    DateOfBirth = DateTime.Parse("1965/7/13")
                },
                new PersonRecord
                {
                    LastName = "Wang",
                    FirstName = "James",
                    Email = "jawang68@gmail.com",
                    FavoriteColor = "Blue",
                    DateOfBirth = DateTime.Parse("1968/7/6")
                },
                new PersonRecord
                {
                    LastName = "Lee",
                    FirstName = "Jane",
                    Email = "janeLee@yahoo.com",
                    FavoriteColor = "Pink",
                    DateOfBirth = DateTime.Parse("1988/9/16")
                }
            };

            //Test
            var results = repo.GetRecords("FavoriteColor, LastName" );

            //Verify
            Assert.IsTrue(Enumerable.SequenceEqual(expected, results, new PersonRecordEquityComparer()));
        }

        [Test]
        public void GetRecords_OrderByLastNameDescending_DisplayRecordsInOrder()
        {
            //Setup 
            var repo = new RecordRepositoryBase();
            RecordRepositoryBase._records = TestData.TwoRecords;

            //Test
            var results = repo.GetRecords("LastName descending");

            //Verify
            Assert.AreEqual("Wang", results.First().LastName);
        }

        #endregion
    }
}