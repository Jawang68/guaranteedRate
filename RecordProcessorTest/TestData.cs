using RecordProcesssor.Model;
using System;
using System.Collections.Generic;

namespace RecordProcessorTest
{
    public class TestData
    {
        public static List<PersonRecord> OneRecord => new List<PersonRecord>()
        {
                new PersonRecord  {
                    LastName = "Wang",
                    FirstName = "James",
                    Email = "jawang68@gmail.com",
                    FavoriteColor = "blue",
                    DateOfBirth = DateTime.Parse("7/6/1968")
                }
        };

        public static List<PersonRecord> TwoRecords => new List<PersonRecord>()
        {
                new PersonRecord  {
                    LastName = "Wang",
                    FirstName = "James",
                    Email = "jawang68@gmail.com",
                    FavoriteColor = "blue",
                    DateOfBirth = DateTime.Parse("7/6/1968")
                },
                new PersonRecord  {
                    LastName = "Lee",
                    FirstName = "Jane",
                    Email = "janeLee@yahoo.com",
                    FavoriteColor = "pink",
                    DateOfBirth = DateTime.Parse("9/16/1988")
                },
        };
    }
}
