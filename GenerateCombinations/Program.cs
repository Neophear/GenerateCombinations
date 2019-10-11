using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GenerateCombinations
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> dictonary = Properties.Resources.ResourceManager.GetString("da_DK").Split(new[] { '\r', '\n' }).ToList();

            while (true)
            {
                Console.Clear();
                Console.Write("Bogstaver: ");
                Word w = new Word(Console.ReadLine());
                List<Word> words = new List<Word>();
                int length = w.Length;

                Console.Write($"Antal tegn [{length}]: ");
                string lengthInput;

                do
                {
                    lengthInput = Console.ReadLine();
                    if (lengthInput == "" || int.TryParse(lengthInput.Trim(), out length))
                        break;

                    length = w.Length;
                    Console.Write($"Forkert format! Antal tegn [{length}]: ");
                } while (true);

                Console.WriteLine($"Muligheder for [{w}] med en længde på {length}:\n");

                foreach (var c in Math.GetVariationsWithoutDuplicates(w.Letters, length))
                    if (!words.Contains(new Word(String.Concat(c))))
                        words.Add(new Word(String.Concat(c)));

                foreach (Word wo in words)
                    if (dictonary.Contains(wo.ToString()))
                        Console.WriteLine(wo);

                Console.Write("\nTryk på en tast for at prøve igen. Tryk Q for at afslutte.");
                if (Console.ReadKey().Key == ConsoleKey.Q)
                    break;
            }
        }
    }

    class Word
    {
        public List<Letter> Letters { get; set; }
        public int Length { get { return Letters.Count; } }
        public Word(string word)
        {
            Letters = new List<Letter>();
            for (int i = 0; i < word.Length; i++)
                Letters.Add(new Letter { Character = word[i], Index = i });
        }

        public override bool Equals(object obj)
        {
            return this.ToString() == obj.ToString();
        }

        public override string ToString() { return String.Concat(Letters); }
        public class Letter
        {
            public char Character { get; set; }
            public int Index { get; set; }
            public override string ToString() { return Character.ToString(); }
        }
    }

    static class Math
    {
        public static IEnumerable<IEnumerable<T>> GetVariationsWithoutDuplicates<T>(IList<T> items, int length)
        {
            if (length == 0 || !items.Any()) return new List<List<T>> { new List<T>() };
            return from item in items.Distinct()
                   from permutation in GetVariationsWithoutDuplicates(items.Where(i => !EqualityComparer<T>.Default.Equals(i, item)).ToList(), length - 1)
                   select Prepend(item, permutation);
        }

        public static IEnumerable<IEnumerable<T>> GetVariations<T>(IList<T> items, int length)
        {
            if (length == 0 || !items.Any()) return new List<List<T>> { new List<T>() };
            return from item in items
                   from permutation in GetVariations(Remove(item, items).ToList(), length - 1)
                   select Prepend(item, permutation);
        }

        public static IEnumerable<T> Prepend<T>(T first, IEnumerable<T> rest)
        {
            yield return first;
            foreach (var item in rest)
                yield return item;
        }

        public static IEnumerable<T> Remove<T>(T item, IEnumerable<T> from)
        {
            var isRemoved = false;
            foreach (var i in from)
            {
                if (!EqualityComparer<T>.Default.Equals(item, i) || isRemoved) yield return i;
                else isRemoved = true;
            }
        }
    }
}