using RecordProcesssor.Model;
using System;
using System.Collections.Generic;

namespace RecordRepositoryTests
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

        public static List<PersonRecord> ThreeRecords => new List<PersonRecord>()
        {
                new PersonRecord  {
                    LastName = "Wang",
                    FirstName = "James",
                    Email = "jawang68@gmail.com",
                    FavoriteColor = "Blue",
                    DateOfBirth = DateTime.Parse("7/6/1968")
                },
                new PersonRecord  {
                    LastName = "Lee",
                    FirstName = "Jane",
                    Email = "janeLee@yahoo.com",
                    FavoriteColor = "Pink",
                    DateOfBirth = DateTime.Parse("9/16/1988")
                },
                new PersonRecord  {
                    LastName = "Gates",
                    FirstName = "Bill",
                    Email = "billGates@microsoft.com",
                    FavoriteColor = "Blue",
                    DateOfBirth = DateTime.Parse("7/13/1965")
                },
        };
    }
}
