using PersonRecordService;
using System;

namespace RecordProcesssor
{
    class Program
    {
        static void Main(string[] args)
        {
            var repository = new FileRecordRepository();
            repository.SaveRecord(@"..\..\TestFiles\TestDataWithSpace.txt");

            Console.WriteLine("Order by favoriteColor, lastName ... ");
            Console.WriteLine(new String('-', 80));
            repository.GetRecords("favoriteColor, lastName").ForEach( x=> Console.WriteLine($"{x.FavoriteColor}, {x.LastName}, {x.FirstName}, {x.Email}, {x.DateOfBirthStr}"));
            Console.WriteLine(new String('-', 80));

            Console.WriteLine("Order by date of birth ... ");
            Console.WriteLine(new String('-', 80));
            repository.GetRecords("dateOfBirth").ForEach(x => Console.WriteLine($"{x.DateOfBirthStr}, {x.LastName}, {x.FirstName}, {x.Email} {x.FavoriteColor}"));
            Console.WriteLine(new String('-', 80));

            Console.WriteLine("Order by lastName descending ... ");
            Console.WriteLine(new String('-', 80));
            repository.GetRecords("lastName descending").ForEach(x => Console.WriteLine($"{x.LastName}, {x.FirstName}, {x.Email} {x.FavoriteColor} {x.DateOfBirthStr}"));
        }
    }
}
