using MySql.Data.MySqlClient;

namespace hegyek_CLI
{
    class Mountain
    {
        public int ID { get; set; }
        public string Peak { get; set; }
        public string MountainRange { get; set; }
        public int Height { get; set; }

        public Mountain(MySqlDataReader reader)
        {
            ID = reader.GetInt32("azon");
            Peak = reader.GetString("csucs");
            MountainRange = reader.GetString("hegyseg");
            Height = reader.GetInt32("magassag");
        }

        public override string ToString()
        {
            return $"{ID} | {Peak} | {MountainRange} | {Height}";
        }
    }

    internal class Program
    {
        static void Main()
        {
            string connectionString = "server=localhost;user='root';pwd='';database='hegyek'";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            string query = "SELECT * FROM hegy";
            MySqlCommand command = new MySqlCommand(query, connection);
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
                Console.WriteLine($"{reader["azon"]} | {reader["csucs"]} | {reader["hegyseg"]} | {reader["magassag"]}");
            reader.Close();

            Console.WriteLine();
            Console.WriteLine("-", 20);
            Console.WriteLine();

            query = "SELECT COUNT(*) FROM hegy WHERE magassag > 2000";
            command = new MySqlCommand(query, connection);
            reader = command.ExecuteReader();
            if (reader.Read())
                Console.WriteLine($"3000 méternél magasabb hegyek száma: {reader[0]}");
            reader.Close();

            Console.WriteLine();
            Console.WriteLine("-", 20);
            Console.WriteLine();

            query = "SELECT * FROM hegy";
            command = new MySqlCommand(query, connection);
            List<Mountain> mountains = new List<Mountain>();
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                Mountain mountain = new Mountain(reader);
                mountains.Add(mountain);
                Console.WriteLine(mountain.ToString());
            }
            reader.Close();



            connection.Close();
            Console.ReadLine();
        }
    }
}