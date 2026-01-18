namespace szakaszsebesseg
{
    class Meres
    {
        public string plateID;
        public TimeOnly enterTime;
        public TimeOnly exitTime;
        public int speed;

        public Meres(string line)
        {
            string[] data = line.Split(' ');
            plateID = data[0];
            enterTime = new TimeOnly(int.Parse(data[1]), int.Parse(data[2]), int.Parse(data[3]), int.Parse(data[4]));
            exitTime = new TimeOnly(int.Parse(data[5]), int.Parse(data[6]), int.Parse(data[7]), int.Parse(data[8]));

            double timeDifferenceHours = (exitTime - enterTime).TotalHours;
            speed = (int)(10 / timeDifferenceHours);
        }
    }
    internal class Program
    {
        static public void Print()
        {
            Console.WriteLine("\n");
            Console.WriteLine("-", 20);
            Console.WriteLine("\n");
        }
        static void Main()
        {
            string[] file = File.ReadAllLines("meresek.txt");
            Meres[] meresek = new Meres[file.Length];
            for (ushort i = 0;  i < file.Length; i++)
                meresek[i] = new Meres(file[i]);

            // Task 2
            Console.WriteLine("2. feladat");
            Console.WriteLine($"A mérés során {meresek.Length} jármű adatait rögzítették.");

            Print();

            // Task 3
            ushort beforeNine = 0;
            foreach (Meres meres in meresek)
                if (meres.exitTime.Hour < 9)
                    beforeNine++;

            Console.WriteLine("3. feladat");
            Console.WriteLine($"9 óra előtt {beforeNine} jármű haladt el a végponti mérőnél.");

            Print();

            // Task 4
            Console.WriteLine("4. feladat");
            Console.WriteLine("Adjon meg egy óra és perc értékét: ");
            Console.Write("Óra: ");
            byte hour;
            byte.TryParse(Console.ReadLine(), out hour);
            Console.Write("Perc: ");
            byte minute;
            byte.TryParse(Console.ReadLine(), out minute);
            // Vehicles passed at enter point
            ushort vehiclesPassed = 0;
            foreach (Meres meres in meresek)
                if (meres.enterTime.Hour == hour && meres.enterTime.Minute ==  minute)
                    vehiclesPassed++;
            Console.WriteLine($"\ta. A kezdeti méréspontnál elhaladt járművek száma: {vehiclesPassed}");
            // Traffic density (not working)
            ushort vehiclesInside = 0;
            foreach (Meres meres in meresek)
                if (meres.enterTime.Hour == hour && meres.enterTime.Minute <= minute && meres.exitTime.Hour == hour && meres.exitTime.Minute !> minute)
                    vehiclesInside++;
            float density = (float)vehiclesInside / 10;
            Console.WriteLine($"\tb. A forgalomsűrűség: {density:F1}");

            Print();

            // Task 5
            int maxIndex = 0;

            for (int i = 1; i < meresek.Length; i++)
                if (meresek[i].speed > meresek[maxIndex].speed)
                    maxIndex = i;

            int leftVehicels = 0;
            foreach (var meres in meresek)
                if (meres.enterTime < meresek[maxIndex].enterTime && meres.exitTime > meresek[maxIndex].enterTime && meres.exitTime < meresek[maxIndex].exitTime)
                    leftVehicels++;
            Console.WriteLine("5. feladat\nA legnagyobb sebességgel haladó jármű");
            Console.WriteLine($"\trendszám: {meresek[maxIndex].plateID}");
            Console.WriteLine($"\tátlagsebessége: {meresek[maxIndex].speed}");
            Console.WriteLine($"\táltal elhagyott járművek: {leftVehicels}\n\n");

            // Task 6
            int fastVehicles = 0;
            foreach (var meres in meresek)
                if (meres.speed > 90)
                    fastVehicles++;
            Console.WriteLine($"6. feladat\nA járművek {((float)fastVehicles/meresek.Length)*100:F2}%-a volt gyorshajtó.\n\n");

            // Task 7
            List<Meres> buntetett = new List<Meres>();
            foreach (var meres in meresek)
                if (meres.speed > 104)
                    buntetett.Add(meres);
            string[] writeText = new string[buntetett.Count];
            for (int i = 0; i < buntetett.Count; i++)
            {
                writeText[i] = $"{buntetett[i].plateID}\t{buntetett[i].speed}";
                if (buntetett[i].speed > 156)
                    writeText[i] += "\t\t200 000 Ft";
                else if (buntetett[i].speed > 136)
                    writeText[i] += "\t\t60 000 Ft";
                else if (buntetett[i].speed > 121)
                    writeText[i] += "\t\t45 000 Ft";
                else
                    writeText[i] += "\t\t30 000 Ft";
            }
            File.WriteAllLines("buntetes.txt", writeText);

            Console.WriteLine("A fájl elkészült");


            Console.WriteLine("\n\nPress Enter to exit");
            Console.ReadLine();
        }
    }
}
