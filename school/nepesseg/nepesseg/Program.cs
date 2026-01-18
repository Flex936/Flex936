namespace nepesseg
{
    internal class Program
    {
        class Country
        {
            public string Name { get; private set; }
            public int Area { get; private set; }
            public int Population { get; private set; }
            public int PopulationDensity { get; private set; }
            public string Capital { get; private set; }
            public int CapitalPopulation { get; private set; }

            public Country(string line)
            {
                string[] data = line.Split(';');

                Name = data[0];
                Area = int.Parse(data[1]);
                if (data[2].Contains('g'))
                    Population = int.Parse(data[2].Replace("g", "0000"));
                else Population = int.Parse(data[2]);
                PopulationDensity = Population / Area;
                Capital = data[3];
                CapitalPopulation = int.Parse(data[4]) * 1000;
            }

            public bool isCapitalPopulous()
            {
                if (CapitalPopulation >= (Population * 0.3))
                    return true;
                return false;
            }

            public override string ToString()
            {
                return $"{Name} ({Capital})";
            }
        }

        public static void PrintTask(int taskNumber, string text)
        {
            Console.WriteLine($"{taskNumber}. feladat");
            Console.WriteLine($"{text}\n");
        }

        static void Main()
        {

            string[] file = File.ReadAllLines("adatok-utf8.txt");
            List<Country> countries = new List<Country>();
            foreach (string line in file.Skip(1))
                countries.Add(new Country(line));

            // Task 3
            PrintTask(3, $"A beolvasott országok száma {countries.Count}.");

            // Task 4
            Country China = countries.Find(country => country.Name == "Kína")!;
            PrintTask(4, $"Kína népsűrűsége: {China.PopulationDensity} fő/km^2.");

            // Task 5
            Country India = countries.Find(country => country.Name == "India")!;
            int populationDifference = China.Population - India.Population;
            PrintTask(5, $"Kínában a lakosság {populationDifference} fővel volt több.");

            // Task 6
            Country _3dMostPopulous = countries.OrderByDescending(country => country.Population).Skip(2).First();
            PrintTask(6, $"A 3. legnépesebb ország: {_3dMostPopulous.Name}, a lakosság {_3dMostPopulous.Population} fő.");

            // Task 7
            var populousCapitalCountries = countries.Where(country => country.isCapitalPopulous()).ToList();
            Console.WriteLine(populousCapitalCountries.Count);
            Console.WriteLine("7. feladat - A következő országok lakosságának több mint 30%-a a fővárosban lakik:");
            foreach (var country in populousCapitalCountries)
                Console.WriteLine($"\t{country.ToString()}");

            // End of code
            Console.ReadLine();
        }
    }
}
