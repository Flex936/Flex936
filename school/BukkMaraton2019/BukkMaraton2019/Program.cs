namespace BukkMaraton2019
{
    class Versenytav
    {
        public string Rajtszam;
        public string Tav
        {
            get
            {
                switch (Rajtszam[0])
                {
                    case 'M': return "Mini";
                    case 'R': return "Rövid";
                    case 'K': return "Közép";
                    case 'H': return "Hosszú";
                    case 'E': return "Pedelec";
                }
                return "Hibás rajtszám";
            }
        }
        public Versenytav(string rajtszam)
        {
            Rajtszam = rajtszam;
        }
    }

    class Versenyzo
    {
        public string Egyesulet;
        public string Nev;
        public string Kategoria;
        public bool isFerfi;
        public Versenytav Versenytav;
        public TimeOnly Ido;
        public static Dictionary<string, int> ferfiKateg = new Dictionary<string, int>();

        public Versenyzo(string line)
        {
            string[] data = line.Split(';');

            Versenytav = new Versenytav(data[0]);
            Kategoria = data[1];
            Nev = data[2];
            Egyesulet = data[3];
            Ido = TimeOnly.Parse(data[4]);

            isFerfi = Kategoria.EndsWith('f');

            if (isFerfi)
            {
                if (ferfiKateg.ContainsKey(Kategoria))
                    ferfiKateg[Kategoria]++;
                else
                    ferfiKateg.Add(Kategoria, 1);
            }
        }
    }

    internal class Program
    {
        static void Main()
        {
            string[] file = File.ReadAllLines("bukkm2019.txt");
            List<Versenyzo> versenyzok = new List<Versenyzo>();
            foreach (string line in file.Skip(1))
                versenyzok.Add(new Versenyzo(line));

            // 4. feladat
            float nemTeljesitoArany = 100 - ((float)versenyzok.Count / 691 * 100);
            Console.WriteLine($"4. feladat: Versenytávot nem teljesítők: {nemTeljesitoArany}%");

            // 5. feladat
            ushort noiVersenyzokRovid = 0;
            foreach (Versenyzo versenyzo in versenyzok)
                if (!versenyzo.isFerfi && versenyzo.Versenytav.Tav == "Rövid")
                    noiVersenyzokRovid++;
            Console.WriteLine($"5. feladat: Női versenyzők száma a rövid távú versenyen: {noiVersenyzokRovid} fő");

            // 6. feladat
            int index = 0;
            TimeOnly comparedTime = new TimeOnly(6, 0, 0);
            while (index < versenyzok.Count)
            {
                if (versenyzok[index].Ido > comparedTime) 
                    break;
                index++;
            }

            if (index < versenyzok.Count)
                Console.WriteLine("6. Van ilyen versenyző");
            else
                Console.WriteLine("6. feladat: Nincs ilyen versenyző");

            // 7. feladat
            int felFerfiIndex = versenyzok.FindIndex(v => v.Kategoria == "ff" && v.Versenytav.Tav == "Rövid");
            for (int i = felFerfiIndex; i < versenyzok.Count; i++)
                if (versenyzok[i].Kategoria == "ff" && versenyzok[i].Versenytav.Tav == "Rövid" && versenyzok[i].Ido < versenyzok[felFerfiIndex].Ido)
                    felFerfiIndex = i;

            Console.WriteLine($"7. feladat: A felnőtt férfi (ff) kategória győztese rövidtávon:" +
                $"\nRajtszám: {versenyzok[felFerfiIndex].Versenytav.Rajtszam}" +
                $"\nNév: {versenyzok[felFerfiIndex].Nev}" +
                $"\nEgyesület: {versenyzok[felFerfiIndex].Egyesulet}" +
                $"\nIdő: {versenyzok[felFerfiIndex].Ido.ToString()}");

            // 8. feladat
            Console.WriteLine("8. feladat: Statisztika");
            foreach (var kateg in Versenyzo.ferfiKateg)
                Console.WriteLine($"\t{kateg.Key} - {kateg.Value} fő");


            Console.WriteLine("\n\n");
            Console.ReadLine();
        }
    }
}
