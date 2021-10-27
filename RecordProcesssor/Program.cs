using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordProcesssor
{
    class Program
    {
        static void Main(string[] args)
        {
            var processor = new RecordProcessor(@"..\..\TestFiles\TestDataWithSpace.txt");
            
            processor.ProcessRecords();
            Console.WriteLine("Order by favoriteColor, lastName ... ");
            Console.WriteLine(new String('-', 80));
            processor.DisplayRecords("favoriteColor, lastName").ForEach( x=> Console.WriteLine($"{x.FavoriteColor}, {x.LastName}, {x.FirstName}, {x.Email}, {x.DateOfBirthStr}"));
            Console.WriteLine(new String('-', 80));

            Console.WriteLine("Order by date of birth ... ");
            Console.WriteLine(new String('-', 80));
            processor.DisplayRecords("dateOfBirth").ForEach(x => Console.WriteLine($"{x.DateOfBirthStr}, {x.LastName}, {x.FirstName}, {x.Email} {x.FavoriteColor}"));
            Console.WriteLine(new String('-', 80));

            Console.WriteLine("Order by lastName descending ... ");
            Console.WriteLine(new String('-', 80));
            processor.DisplayRecords("lastName descending").ForEach(x => Console.WriteLine($"{x.LastName}, {x.FirstName}, {x.Email} {x.FavoriteColor} {x.DateOfBirthStr}"));
        }
    }
}
