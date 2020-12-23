using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Features
{
    class Program
    {
        static void Main(string[] args)
        {
            Func<int, int> square = x => x*x;
            Func<int, int, int> add = (x, y) => x + y;
            Console.WriteLine(square(add(3,5)));

            Action<int> write = x => Console.WriteLine(x);

            IEnumerable<Employee> developers = new Employee[]
            {
                new Employee { id = 1, Name = "Scott"},
                new Employee { id = 2, Name = "Chris"}
            };

            var sales = new List<Employee>()
            {
                new Employee { id = 3, Name = "Alex"}
            };

            //foreach (var person in developers)
            //{
            //    Console.WriteLine(person.Name);

            //}
            //foreach (var person in sales)
            //{
            //    Console.WriteLine(person.Name);

            //}

            //Console.WriteLine(developers.Count());
            //IEnumerator<Employee> enumerator = sales.GetEnumerator();
            //while (enumerator.MoveNext())
            //{
            //    Console.WriteLine(enumerator.Current.Name);
            //}


            //var query = developers.Where(e => e.Name.Length == 5)
            //    .OrderBy(e => e.Name).Select(e => e);
            
            var query = developers.Where(e => e.Name.Length == 5)
                .OrderByDescending(e => e.Name);

            var query2 = from developer in developers
                         where developer.Name.Length == 5
                         orderby developer.Name descending
                         select developer;

            foreach (var employee in query2)
            {
                Console.WriteLine(employee.Name);
            }
            Console.ReadLine();
        }

    }
}
