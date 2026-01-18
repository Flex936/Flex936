namespace banyato
{
    class CaveLake
    {
        public List<int[]> matrix = new List<int[]>();
        public int Area = 0;
        public float avgDepth = 0;
        public int maxDepth = 0;
        public List<int[]> deepestCoords = new List<int[]>();
        public int coastline = 0;

        public CaveLake(string[] data)
        {
            int totalDepth = 0;
            // Load matrix from file
            foreach (string row in data.Skip(2))
            {
                int[] rowValues = Array.ConvertAll(row.Split(' '), int.Parse);
                matrix.Add(rowValues);

                // Calculate average depth and area
                foreach (int depth in rowValues)
                {
                    if (depth > 0)
                    {
                        Area++;
                        totalDepth += depth;
                    }
                }
            }
            // Calculate average depth
            avgDepth = (totalDepth / (float)Area) / 10;
            // Find deepest points
            maxDepth = matrix.Max(row => row.Max());
            for (int row = 0; row < matrix.Count; row++)
                for (int col = 0; col < matrix[row].Length; col++)
                    if (matrix[row][col] == maxDepth)
                        deepestCoords.Add(new int[] { row + 1, col + 1 });
            // Calculate coastline
            bool isLand = true;
            foreach (int[] row in matrix)
                foreach (int col in row)
                {
                    if (col != 0 && isLand)
                    {
                        coastline++;
                        isLand = false;
                    }
                    else if (col == 0 && !isLand)
                    {
                        coastline++;
                        isLand = true;
                    }
                }
            isLand = true;
            for (int col = 0; col < matrix[0].Length; col++)
                for (int row = 0; row < matrix.Count; row++)
                {
                    if (matrix[row][col] != 0 && isLand)
                    {
                        coastline++;
                        isLand = false;
                    }
                    else if (matrix[row][col] == 0 && !isLand)
                    {
                        coastline++;
                        isLand = true;
                    }
                }
        }
    }
    internal class Program
    {
        public static void Print(string header, ref string text)
        {
            Console.WriteLine($"{header}");
            Console.WriteLine($"{text}");
            Console.WriteLine("\n", 5);
            text = "";
        }
        static void Main()
        {
            string[] file = File.ReadAllLines("melyseg.txt");
            CaveLake caveLake = new CaveLake(file);

            string text = "";

            // Task 2
            Console.WriteLine("2. fealadat:");
            Console.Write("Mérés sora: ");
            int row = int.Parse(Console.ReadLine()!) - 1;
            Console.Write("Mérés oszlopa: ");
            int column = int.Parse(Console.ReadLine()!) - 1;
            Console.WriteLine($"A mérés helyén a tó mélysége {caveLake.matrix[row][column]} dm");
            Console.WriteLine("\n", 5);

            // Task 3
            text += $"A tó felszíne: {caveLake.Area} m2\n";
            text += $"A tó átlagos mélysége: {caveLake.avgDepth:F2} m";
            Print("3. feladat:", ref text);

            // Task 4
            text += $"A tó legnagyobb mélysége: {caveLake.maxDepth} dm\n";
            text += $"A legmélyebb pontok koordinátái:\n";
            foreach (var coord in caveLake.deepestCoords)
                text += $"({coord[0]}; {coord[1]}) ";
            Print("4. feladat:", ref text);

            // Task 5
            text += $"A tó partvonala {caveLake.coastline} m";
            Print("5. feladat:", ref text);

            // Task 6
            Console.Write("6. feladat:\nA vizsgált szelvény oszlopának azonosítója: ");
            int inputColumn = int.Parse(Console.ReadLine()!);
            string[] writeText = new string[caveLake.matrix.Count];
            for (int i = 0; i < caveLake.matrix.Count; i++)
            {
                writeText[i] = $"{i}";
                writeText[i] += new string('*', caveLake.matrix[i][inputColumn - 1]);
            }
            File.WriteAllLines("diagram.txt", writeText);


            Console.WriteLine("\n", 8);
            Console.Write("Press Enter to exit\t");
            Console.ReadLine();
        }
    }
}
