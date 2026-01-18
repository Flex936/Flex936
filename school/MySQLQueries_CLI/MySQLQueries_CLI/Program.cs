using MySql.Data.MySqlClient;

namespace MySQLQueries_CLI
{
    class Country
    {
        public string Name { get; set; }
        public string NamesInOtherLanguages { get; set; }
        public string Capital { get; set; }
        public string CapitalInOtherLanguages { get; set; }
        public string GeoLocation { get; set; }
        public float Area { get; set; }
        public string FormOfGovernment { get; set; }
        public int Population { get; set; }
        public int PopulationCapital { get; set; }
        public double PopulationDensity { get; set; }
        public string Currency { get; set; }
        public string CurrencyCode { get; set; }
        public string ChangeCurrency { get; set; }
        public string CarSign { get; set; }
        public int PhoneCode { get; set; }
        public int GDP { get; set; }
        public int Category { get; set; }

        public Country(MySqlDataReader reader)
        {
            Name = reader.GetString("orszag");
            NamesInOtherLanguages = reader.GetString("country");
            Capital = reader.GetString("fovaros");
            CapitalInOtherLanguages = reader.GetString("capital");
            GeoLocation = reader.GetString("foldr_hely");
            Area = reader.GetFloat("terulet");
            FormOfGovernment = reader.GetString("allamforma");
            Population = reader.GetInt32("nepesseg") * 1000;
            PopulationCapital = reader.GetInt32("nep_fovaros");
            PopulationDensity = Math.Round(Population / Area, 2);
            Currency = reader.GetString("penznem");
            CurrencyCode = reader.GetString("penzjel");
            ChangeCurrency = reader.GetString("valtopenz");
            CarSign = reader.GetString("autojel");
            PhoneCode = reader.GetInt32("telefon");
            GDP = reader.GetInt32("gdp");
            Category = reader.GetInt32("kat");
        }
    }

    internal class Program
    {
        static void Main()
        {
            // Open a connection to the MySQL database
            string connectionString = "server=localhost;user='root';password='';database='orszagok';";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            // Load countries from the database
            List<Country> countries = new List<Country>();
            string query = "SELECT * FROM orszagok";
            MySqlCommand command = new MySqlCommand(query, connection);
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Country country = new Country(reader);
                countries.Add(country);
            }
            connection.Close();

            // Simple queries
            // Madagaskar's capital
            var madagaskarCapital = countries.FirstOrDefault(c => c.Name == "MADAGASZKÁR")?.Capital;
            Console.WriteLine($"Madagascar's capital: {madagaskarCapital}");

            // Country of OUAGADOUGOU
            var ouagadougouCountry = countries.FirstOrDefault(c => c.Capital == "OUAGADOUGOU")?.Name;
            Console.WriteLine($"Country of OUAGADOUGOU: {ouagadougouCountry}");

            // TT car sign's country
            var ttCarSignCountry = countries.FirstOrDefault(c => c.CarSign == "TT")?.Name;
            Console.WriteLine($"Country with car sign 'TT': {ttCarSignCountry}");

            // SGD currency code's country
            var sgdCurrencyCountry = countries.FirstOrDefault(c => c.CurrencyCode == "SGD")?.Name;
            Console.WriteLine($"Country with currency code 'SGD': {sgdCurrencyCountry}");

            // 61 phone code's country
            var phoneCode61Country = countries.FirstOrDefault(c => c.PhoneCode == 61)?.Name;
            Console.WriteLine($"Country with phone code '61': {phoneCode61Country}");

            // Area of Monaco
            var monacoArea = countries.FirstOrDefault(c => c.Name == "MONACO")?.Area;
            Console.WriteLine($"Area of Monaco: {monacoArea} km²");

            // Population of Malta
            var maltaPopulation = countries.FirstOrDefault(c => c.Name == "MÁLTA")?.Population;
            Console.WriteLine($"Population of Malta: {maltaPopulation * 1000}");

            // Japan's Population Density
            var japanPopulationDensity = countries.FirstOrDefault(c => c.Name == "JAPÁN")?.PopulationDensity;
            Console.WriteLine($"Japan's Population Density: {japanPopulationDensity} people/km²");

            // Population density over 500 people/km²
            var highDensityCountries = countries.Where(c => c.PopulationDensity > 500).Select(c => c.Name).ToList();
            Console.WriteLine("Countries with population density over 500 people/km²:");
            foreach (var country in highDensityCountries)
                Console.WriteLine($"\t- {country}");

            // Island countries
            var islandCountries = countries.Where(c => c.GeoLocation.Contains("(szigetország)")).Select(c => c.Name).ToList();
            Console.WriteLine("Island countries:");
            foreach (var country in islandCountries)
                Console.WriteLine($"\t- {country}");

            // Top 6 Area countries
            var top6AreaCountries = countries.OrderByDescending(c => c.Area).Take(6).Select(c => c.Name).ToList();
            Console.WriteLine("Top 6 countries by area:");
            foreach (var country in top6AreaCountries)
                Console.WriteLine($"\t- {country}");

            // Top 3 smallest Area countries
            var smallest3AreaCountries = countries.OrderBy(c => c.Area).Take(3).Select(c => c.Name).ToList();
            Console.WriteLine("3 smallest countries by area:");
            foreach (var country in smallest3AreaCountries)
                Console.WriteLine($"\t- {country}");

            // 40th smallest country by area
            var _40thSmallestCountry = countries.OrderBy(c => c.Area).Skip(39).FirstOrDefault()?.Name;
            Console.WriteLine($"40th smallest country by area: {_40thSmallestCountry}");

            // 15th smallest country by population
            var _15thSmallestPopulationCountry = countries.OrderBy(c => c.Population).Skip(14).FirstOrDefault()?.Name;
            Console.WriteLine($"15th smallest country by population: {_15thSmallestPopulationCountry}");

            // 61th largest country by population density
            var _61thLargestPopulationDensityCountry = countries.OrderByDescending(c => c.PopulationDensity).Skip(60).FirstOrDefault()?.Name;
            Console.WriteLine($"61th largest country by population density: {_61thLargestPopulationDensityCountry}");

            // Summary queries
            // Earth's population
            long earthPopulation = 0;
            foreach (var country in countries)
                earthPopulation += country.Population;
            Console.WriteLine($"Earth's population: {earthPopulation}");

            // Population density of Earth
            long earthArea = 0;
            foreach (var country in countries)
                earthArea += (long)country.Area;
            double earthPopulationDensity = Math.Round((double)earthPopulation / earthArea, 2);
            Console.WriteLine($"Earth's population density: {earthPopulationDensity} people/km²");

            // End of connection setup
            Console.WriteLine("\n\n\n\n\nPress Enter to exit...");
            Console.ReadLine();
        }
    }
}
