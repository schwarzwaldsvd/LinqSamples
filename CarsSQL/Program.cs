using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace CarsSQL
{
    public class Program
    {
        static void Main(string[] args)
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<CarDb>());
            InsertData();
            QueryData();

            Console.ReadLine();
        }

        private static void QueryData()
        {
            var db = new CarDb();
            db.Database.Log = Console.WriteLine;

            var query1a =
                from car in db.Cars
                orderby car.Combined descending, car.Name ascending
                select car;

            var query2a =
                from car in db.Cars
                group car by car.Manufacturer into manufacturer
                select new
                {
                    Name = manufacturer.Key,
                    // Cars = manufacturer.OrderByDescending(c => c.Combined).Take(2) 
                    Cars = (from car in manufacturer
                           orderby car.Combined descending
                           select car).Take(2)
                };

                #region extensions method syntax
            var query1b =
                db.Cars.Where(c => c.Manufacturer=="BMW")
                .OrderByDescending(c => c.Combined)
                .ThenBy(c => c.Name)
                .Take(10);

            var query2b =
                db.Cars.GroupBy(c => c.Manufacturer)
                .Select(g =>
                    new
                    {
                        Name = g.Key,
                        Cars = g.OrderByDescending(c => c.Combined).Take(2)
                    });

            #endregion

            foreach (var group in query2a)
            {
                Console.WriteLine(group.Name);
                foreach (var car in group.Cars)
                {
                    Console.WriteLine($"\t {car.Name} : {car.Combined}");
                }
            }
            
        }

        private static void InsertData()
        {
            var cars = ProcessCars("fuel.csv");
            var db = new CarDb();
            db.Database.Log = Console.WriteLine;
            if (!db.Cars.Any())
            {
                foreach (var car in cars)
                {
                    db.Cars.Add(car);
                }
                db.SaveChanges();
            }
        }

        #region XML
        private static void QueryXML()
        {
            var ns = (XNamespace)"http://pluralsight.com/cars/2016";
            var ex = (XNamespace)"http://pluralsight.com/cars/2016/ex";
            var document = XDocument.Load("fuel.xml");
            var query =
                from element in document.Element(ns + "Cars")?.Elements(ex + "Car")
                                                ?? Enumerable.Empty<XElement>()
                where element.Attribute("Manufacturer")?.Value == "BMW"
                select element.Attribute("Name").Value;
            foreach (var name in query)
            {
                Console.WriteLine(name);
            }

        }

        private static void CreateXML()
        {
            var records = ProcessCars("fuel.csv");
            var ns = (XNamespace)"http://pluralsight.com/cars/2016";
            var ex = (XNamespace)"http://pluralsight.com/cars/2016/ex";
            var document = new XDocument();

            var cars = new XElement(ns + "Cars",
                from record in records
                select new XElement(ex + "Car",
                    new XAttribute("Name", record.Name),
                    new XAttribute("Combined", record.Combined),
                    new XAttribute("Manufacturer", record.Manufacturer),
                    new XAttribute("Year", record.Year))
                );

            cars.Add(new XAttribute(XNamespace.Xmlns + "ex", ex));
            document.Add(cars);
            document.Save("fuel.xml");
        }
        #endregion

        private static List<Car> ProcessCars(string path)
        {

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
}
