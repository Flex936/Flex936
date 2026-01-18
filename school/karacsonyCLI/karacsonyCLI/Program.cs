using System.IO;
namespace karacsonyCLI
{
    class DailyWork
    {
        public int day { get; private set; }
        public int BellsMade { get; private set; }
        public int BellsSold { get; private set; }
        public int AngelsMade { get; private set; }
        public int AngelsSold { get; private set; }
        public int TreesMade { get; private set; }
        public int TreesSold { get; private set; }
        public static int AllMade { get; private set; }

        public DailyWork(string line)
        {
            string[] parts = line.Split(';');

            day = int.Parse(parts[0]);

            BellsMade = int.Parse(parts[1]);
            BellsSold = int.Parse(parts[2].Trim('-'));

            AngelsMade = int.Parse(parts[3]);
            AngelsSold = int.Parse(parts[4].Trim('-'));

            TreesMade = int.Parse(parts[5]);
            TreesSold = int.Parse(parts[6].Trim('-'));

            AllMade = BellsMade + AngelsMade + TreesMade;
        }

        public int BellsLeft()
        {
            return BellsMade - BellsSold;
        }
        public int AngelsLeft()
        {
            return AngelsMade - AngelsSold;
        }
        public int TreesLeft()
        {
            return TreesMade - TreesSold;
        }
        public int DailyMade()
        {
            return BellsMade + AngelsMade + TreesMade;
        }
        public int DailyIncome()
        {
            return BellsSold * 1000 + AngelsSold * 1350 + TreesSold * 1500;
        }
    }

    internal class Program
    {
        public static void InStorageSum(
            DailyWork[] dailyWorks,
            int daysToCount,
            ref int bellsInStorage,
            ref int angelsInStorage,
            ref int treesInStorage
            )
        {
            bellsInStorage = 0;
            angelsInStorage = 0;
            treesInStorage = 0;
            for (int i = 0; i < daysToCount; i++)
            {
                bellsInStorage += dailyWorks[i].BellsLeft();
                angelsInStorage += dailyWorks[i].AngelsLeft();
                treesInStorage += dailyWorks[i].TreesLeft();
            }
        }

        static void Main()
        {
            // Preperations
            string[] file = File.ReadAllLines("diszek.txt");
            DailyWork[] dailyWorks = new DailyWork[file.Length];
            for (byte i = 0; i < file.Length; i++)
                dailyWorks[i] = new DailyWork(file[i]);

            // Task 4
            Console.Write("4. feladat: ");
            Console.WriteLine($"Összesen {DailyWork.AllMade} darab dísz készült.");

            // Task 5
            int index = 0;
            bool foundMadeNone = false;
            while (index < dailyWorks.Length && !foundMadeNone)
            {
                if (dailyWorks[index].DailyMade() == 0)
                    foundMadeNone = true;
                else
                    index++;
            }
            Console.Write("5. feladat: ");
            if (foundMadeNone)
                Console.WriteLine("Volt olyan nap, amikor egyetlen dísz sem készült");
            else
                Console.WriteLine("Nem volt olyan nap, amikor egyetlen dísz sem készült");

            // Task 6
            Console.WriteLine("6. feladat: ");
            int inputNum = 0;
            while (inputNum < 1 || inputNum > 40)
            {
                try
                {
                    Console.Write("Adja meg a keresett napot [1 ... 40]: ");
                    inputNum = int.Parse(Console.ReadLine()!);
                }
                catch (Exception ex) { }
            }
            int bellsInStorage = 0;
            int angelsInStorage = 0;
            int treesInStorage = 0;
            InStorageSum(dailyWorks, inputNum, ref bellsInStorage, ref angelsInStorage, ref treesInStorage);
            Console.WriteLine($"\tA(z) {inputNum}. nap végén {bellsInStorage} harang, {angelsInStorage} angyalka és {treesInStorage} fenyőfa maradt készleten.");

            // Task 7 
            Dictionary<string, int> soldSum = new Dictionary<string, int>
            {
                { "harang", 0 },
                { "angyalka", 0 },
                { "fenyőfa", 0 }
            };

            foreach (DailyWork dailyWork in dailyWorks)
            {
                soldSum["harang"] += dailyWork.BellsSold;
                soldSum["angyalka"] += dailyWork.AngelsSold;
                soldSum["fenyőfa"] += dailyWork.TreesSold;
            }

            int maxSold = soldSum.Values.Max();
            var maxDecorations = soldSum.Where(d => d.Value == maxSold).Select(d => d.Key);

            Console.WriteLine($"7. feladat: Legtöbbet eladott dísz: {maxSold} darab");
            foreach (var maxDecor in maxDecorations)
                Console.WriteLine($"\t{maxDecor}");

            // Task 8
            using (StreamWriter writer = new StreamWriter("bevetel.txt"))
            {
                foreach (DailyWork daily in dailyWorks)
                {
                    int income = daily.DailyIncome();
                    if (income > 10000)
                    {
                        writer.WriteLine($"{daily.day}. nap:\t{income} Ft");
                    }
                }
            }

            Console.WriteLine("A fájl elkészült: bevetel.txt");



            // End of code
            Console.ReadLine();
        }
    }
}
