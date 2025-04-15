using System.Collections.Generic;
using System.Windows;
using RekenApplicatie.Models;

namespace RekenApplicatie
{
    public partial class HighscoreWindow : Window
    {
        private HighscoreManager highscoreManager;

        public HighscoreWindow(HighscoreManager highscoreManager)
        {
            InitializeComponent();
            this.highscoreManager = highscoreManager;

            // Load highscores
            LoadHighscores();
        }

        // Load and display highscores
        private void LoadHighscores()
        {
            List<(string Name, int Score)> highscores = highscoreManager.GetHighscoresList();

            // Create a list of highscore items for the ListView
            List<HighscoreItem> highscoreItems = new List<HighscoreItem>();

            for (int i = 0; i < highscores.Count; i++)
            {
                highscoreItems.Add(new HighscoreItem
                {
                    Rank = i + 1,
                    Name = highscores[i].Name,
                    Score = highscores[i].Score
                });
            }

            // Set the ListView's ItemsSource
            HighscoreListView.ItemsSource = highscoreItems;
        }

        // Close button click handler
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    // Class to represent a highscore item in the ListView
    public class HighscoreItem
    {
        public int Rank { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }
    }
}