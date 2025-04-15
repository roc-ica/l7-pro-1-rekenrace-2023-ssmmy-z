using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RekenApplicatie.Models
{
    // Highscore class to manage highscores
    public class HighscoreManager
    {
        private string highscoreFilePath;
        private Dictionary<string, int> highscores;

        public HighscoreManager(string filePath = "highscore.txt")
        {
            highscoreFilePath = filePath;
            highscores = new Dictionary<string, int>();
            LoadHighscores();
        }

        // Load highscores from file
        private void LoadHighscores()
        {
            try
            {
                if (File.Exists(highscoreFilePath))
                {
                    var lines = File.ReadAllLines(highscoreFilePath);
                    highscores.Clear();

                    foreach (var line in lines)
                    {
                        var parts = line.Split(',');
                        if (parts.Length == 2 && int.TryParse(parts[0], out int score))
                        {
                            highscores[parts[1]] = score;
                        }
                    }
                }
                else
                {
                    File.WriteAllText(highscoreFilePath, "0");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading highscores: {ex.Message}");
            }
        }

        // Save highscores to file
        private void SaveHighscores()
        {
            try
            {
                var sortedEntries = highscores.OrderByDescending(e => e.Value).ToList();
                File.WriteAllLines(highscoreFilePath, sortedEntries.ConvertAll(e => $"{e.Value},{e.Key}"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving highscores: {ex.Message}");
            }
        }

        // Add or update a player's highscore
        public bool UpdateHighscore(Player player)
        {
            bool isNewHighscore = false;
            string playerName = player.Name.Trim();

            if (string.IsNullOrEmpty(playerName))
                return false;

            // Update the score if it's higher than the existing one
            if (highscores.ContainsKey(playerName))
            {
                if (player.Score > highscores[playerName])
                {
                    highscores[playerName] = player.Score;
                    isNewHighscore = true;
                }
            }
            else
            {
                highscores[playerName] = player.Score;
                isNewHighscore = true;
            }

            if (isNewHighscore)
                SaveHighscores();

            return isNewHighscore;
        }

        // Get all highscores as a formatted string
        public string GetHighscoresAsString()
        {
            if (highscores.Count == 0)
                return "Geen highscores gevonden.";

            var sortedScores = highscores.OrderByDescending(h => h.Value).ToList();
            var result = new List<string>();

            for (int i = 0; i < sortedScores.Count; i++)
            {
                result.Add($"{i + 1}. {sortedScores[i].Key}: {sortedScores[i].Value}");
            }

            return string.Join("\n", result);
        }

        // Get highscores as a list of tuples (for displaying in a ListView)
        public List<(string Name, int Score)> GetHighscoresList()
        {
            return highscores.OrderByDescending(h => h.Value)
                             .Select(h => (h.Key, h.Value))
                             .ToList();
        }
    }
}