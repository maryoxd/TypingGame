using System;
using System.Collections.Generic;
using System.IO;

namespace TypingGame
{
    /// <summary>
    /// Trieda TextTask predstavuje úlohu, ktorú hráč musí napísať.
    /// </summary>
    public class TextTask
    {
        private readonly List<string> _sentences;
        private readonly List<string> _words;
        private readonly Random _random;
        private readonly string _difficulty;
        private readonly bool _useRandomizer;

        /// <summary>
        /// Konštruktor triedy TextTask.
        /// </summary>
        /// <param name="difficulty"></param> - Slúži na určenie obtiažnosti úloh.
        /// <param name="useRandomizer"></param> - Slúži na určenie, či sa majú generovať náhodné úlohy.
        public TextTask(string difficulty, bool useRandomizer)
        {
            _random = new Random();
            _difficulty = difficulty;
            _useRandomizer = useRandomizer;
            _sentences = LoadSentences(difficulty);
            _words = LoadWords(difficulty);
        }

        /// <summary>
        /// Metóda LoadSentences slúži na načítanie viet z príslušného súboru.
        /// </summary>
        /// <param name="difficulty"></param> - Slúži na určenie obtiažnosti úloh.
        /// <returns></returns>
        private static List<string> LoadSentences(string difficulty)
        {
            var sentences = new List<string>();
            string filePath = Path.Combine("Content", $"{difficulty}.txt");

            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                sentences.AddRange(lines);
            }
            else
            {
                sentences.Add("No sentences available for this difficulty.");
            }

            return sentences;
        }

        /// <summary>
        /// Metóda LoadWords slúži na načítanie slov z príslušného súboru.
        /// </summary>
        /// <param name="difficulty"></param> - Slúži na určenie obtiažnosti úloh.
        /// <returns></returns>
        private static List<string> LoadWords(string difficulty)
        {
            var words = new List<string>();
            string filePath = Path.Combine("Content", $"{difficulty}randomizer.txt");

            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                words.AddRange(lines);
            }
            else
            {
                words.Add("No words available for this difficulty.");
            }

            return words;
        }

        /// <summary>
        /// Metóda GetRandomSentence slúži na získanie náhodnej vety.
        /// </summary>
        /// <returns></returns>
        public string GetRandomSentence()
        {
            if (_useRandomizer)
            {
                return GenerateRandomSentence();
            }

            if (_sentences.Count == 0)
                return "No sentences available.";

            int index = _random.Next(_sentences.Count);
            return _sentences[index];
        }

        /// <summary>
        /// Metóda GenerateRandomSentence slúži na generovanie náhodnej vety.
        /// </summary>
        /// <returns></returns>
        private string GenerateRandomSentence()
        {
            int wordCount = _difficulty.ToLower() switch
            {
                "easy" => 6,
                "medium" => 8,
                "hard" => 10,
                _ => 6
            };

            var selectedWords = new List<string>();
            for (int i = 0; i < wordCount; i++)
            {
                int index = _random.Next(_words.Count);
                selectedWords.Add(_words[index]);
            }

            return string.Join(" ", selectedWords) + ".";
        }
    }
}
