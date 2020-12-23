using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars
{
    public class Program
    {
        static void Main(string[] args)
        {
            var cars = ProcessCars("fuel.csv");
            var manufacturers = ProcessManufacturers("manufacturers.csv");

            #region  fiddling around
            //var query = cars.OrderByDescending(c => c.Combined)
            //    .ThenBy(c => c.Name);

            //var query =
            //    from car in cars
            //    where car.Manufacturer == "BMW" && car.Year == 2016
            //    orderby car.Combined descending, car.Name ascending
            //    select car;

            //var top =
            //    cars.Where(c => c.Manufacturer == "BMW" && c.Year == 2016)
            //    .OrderByDescending(c => c.Combined)
            //    .ThenBy(c => c.Name)
            //    .Select(c => c)
            //    .First();

            //var query =
            //    from car in cars
            //    where car.Manufacturer == "BMW" && car.Year == 2016
            //    orderby car.Combined descending, car.Name ascending
            //    select new
            //    {
            //        car.Manufacturer,
            //        car.Name,
            //        car.Combined
            //    };

            // var result = cars.Any(c => c.Manufacturer == "Ford");
            // var result = cars.All(c => c.Manufacturer == "Ford");

            //var result = cars.Select(c => c.Name);

            //foreach (var name in result)
            //{
            //    Console.WriteLine(name);
            //}

            //Console.WriteLine(top.Name);

            //foreach (var car in query.Take(10))
            //{
            //    Console.WriteLine($"{car.Name} : {car.Combined}");
            //}

            // --  JOINS on 2 fields
            //var query =
            //    from car in cars
            //    join manufacturer in manufacturers
            //        on new { car.Manufacturer, car.Year }
            //        equals
            //        new { Manufacturer = manufacturer.Name, manufacturer.Year }
            //    orderby car.Combined descending, car.Name ascending
            //    select new
            //    {
            //        manufacturer.Headquarters,
            //        car.Name,
            //        car.Combined
            //    };

            //var query2 =
            //    cars.Join(manufacturers,
            //    c => new { c.Manufacturer, c.Year },
            //    m => new { Manufacturer = m.Name, m.Year },
            //    (c, m) => new
            //    {
            //        m.Headquarters,
            //        c.Name,
            //        c.Combined
            //    })
            //    .OrderByDescending(c => c.Combined)
            //    .ThenBy(c => c.Name);

            //foreach (var car in query.Take(10))
            //{
            //    Console.WriteLine($"{car.Headquarters} {car.Name} : {car.Combined}");
            //}

            //var query =
            //    from car in cars
            //    group car by car.Manufacturer.ToUpper() into manufacturer
            //    orderby manufacturer.Key
            //    select manufacturer;

            //var query2 =
            //    cars.GroupBy(c => c.Manufacturer.ToUpper())
            //    .OrderBy(g => g.Key);

            //foreach (var group in query2)
            //{
            //    // Console.WriteLine($"{ result.Key } has {result.Count()} cars");
            //    Console.WriteLine(group.Key);
            //    foreach (var car in group.OrderByDescending(c => c.Combined).Take(2))
            //    {
            //        Console.WriteLine($"\t{car.Name} : {car.Combined} ");
            //    }
            //}

            // grouping
            //var query =
            //    from manufacturer in manufacturers
            //    join car in cars on manufacturer.Name equals car.Manufacturer
            //        into carGroup
            //    orderby manufacturer.Name
            //    select new
            //    {
            //        Manufacturer = manufacturer,
            //        Cars = carGroup
            //    };

            //var query2 =
            //    manufacturers.GroupJoin(cars, m => m.Name, c => c.Manufacturer,
            //        (m, g) =>
            //        new {
            //            Manufacturer = m,
            //            Cars = g
            //        })
            //    .OrderBy(m => m.Manufacturer.Name);

            //foreach (var group in query2)
            //{
            //    Console.WriteLine($"{group.Manufacturer.Name} - {group.Manufacturer.Headquarters}");
            //    foreach (var car in group.Cars.OrderByDescending(c => c.Combined).Take(2))
            //    {
            //        Console.WriteLine($"\t{car.Name} : {car.Combined} ");
            //    }
            //}

            // group by country

            //var query =
            //    from manufacturer in manufacturers
            //    join car in cars on manufacturer.Name equals car.Manufacturer
            //        into carGroup
            //    select new
            //    {
            //        Manufacturer = manufacturer,
            //        Cars = carGroup
            //    } into result
            //    group result by result.Manufacturer.Headquarters;

            //var query2 =
            //    manufacturers.GroupJoin(cars, m => m.Name, c => c.Manufacturer,
            //        (m, g) =>
            //        new {
            //            Manufacturer = m,
            //            Cars = g
            //        })
            //    .GroupBy(m => m.Manufacturer.Headquarters);

            //foreach (var group in query2)
            //{
            //    Console.WriteLine($"{group.Key}");
            //    foreach (var car in group.SelectMany(g => g.Cars)
            //        .OrderByDescending(c => c.Combined)
            //        .Take(3))
            //    {
            //        Console.WriteLine($"\t{car.Name} : {car.Combined} ");
            //    }
            //}

            #endregion

            var query =
                from car in cars
                group car by car.Manufacturer into carGroup
                select new
                {
                    Name = carGroup.Key,
                    Max = carGroup.Max(c => c.Combined),
                    Min = carGroup.Min(c => c.Combined),
                    Avg = carGroup.Average(c => c.Combined)
                } into result
                orderby result.Max descending
                select result;

            var query2 =
                cars.GroupBy(c => c.Manufacturer)
                .Select(g =>
                {
                    var results = g.Aggregate(new CarStatistics(),
                            (acc, c) => acc.Accumulate(c),
                            acc => acc.Compute());
                    return new
                    {
                        Name = g.Key
                    };
                });


            foreach (var result in query)
            {
                Console.WriteLine($"{result.Name}");
                Console.WriteLine($"\t Max: { result.Max} ");
                Console.WriteLine($"\t Min: { result.Min} ");
                Console.WriteLine($"\t Avg: { result.Avg} ");
            }
            

            Console.ReadLine();
        }



        private static List<Car> ProcessCars(string path)
        {
            //return File.ReadAllLines(path)
            //    .Skip(1)
            //    .Where(line => line.Length > 1)
            //    .Select(Car.ParseFromCsv)
            //    .ToList();

            // the same <-->
            //var query =
            //    from line in File.ReadAllLines(path).Skip(1)
            //    where line.Length > 1
            //    select Car.ParseFromCsv(line);
            //return query.ToList();

            var query =
                File.ReadAllLines(path)
                .Skip(1)
                .Where(l => l.Length > 1)
                .ToCar();
            return query.ToList();
        }

        private static List<Manufacturer> ProcessManufacturers(string path)
        {
            var query =
                File.ReadAllLines(path)
                .Where(l => l.Length > 1)
                .Select(l =>
                {
                    var columns = l.Split(',');
                    return new Manufacturer
                    {
                        Name = columns[0],
                        Headquarters = columns[1],
                        Year = int.Parse(columns[2])
                    };
                });
            return query.ToList();
        }
    }
    public static class CarExtensions
    {
        public static IEnumerable<Car> ToCar(this IEnumerable<string> source)
        {
            foreach (var line in source)
            {
                var columns = line.Split(',');
                yield return new Car
                {
                    Year = int.Parse(columns[0]),
                    Manufacturer = columns[1],
                    Name = columns[2],
                    Displacement = double.Parse(columns[3]),
                    Cylinders = int.Parse(columns[4]),
                    City = int.Parse(columns[5]),
                    Highway = int.Parse(columns[6]),
                    Combined = int.Parse(columns[7])
                };
            }
        }
    }
    public class CarStatistics
    {
        public CarStatistics()
        {
            Max = Int32.MinValue;
            Min = Int32.MaxValue;

        }
        public CarStatistics Accumulate(Car car)
        {
            Count += 1;
            Total += car.Combined;
            Max = Math.Max(Max, car.Combined);
            Min = Math.Min(Min, car.Combined);
            return this;
        }
        public CarStatistics Compute()
        {
            Average = Total / Count;
            return this;
        }
        public int Max { get; set; }
        public int Min { get; set; }
        public int Total { get; set; }
        public int Count { get; set; }
        public double Average { get; set; }

    }
}
