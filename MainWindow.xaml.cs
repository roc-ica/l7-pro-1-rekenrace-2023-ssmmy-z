using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using RekenApplicatie.Models;
using RekenApplicatie.Utilities;

namespace RekenApplicatie
{
    public partial class MainWindow : Window
    {
        private QuizManager quizManager;
        private ResourceManager resourceManager;
        private Player player;
        private HighscoreManager highscoreManager;

        public MainWindow()
        {
            InitializeComponent();

            // Initialize managers
            quizManager = new QuizManager();
            resourceManager = new ResourceManager();
            highscoreManager = new HighscoreManager();

            // Hide quiz elements at startup
            HideQuizElements();
        }

        // Hide quiz-related UI elements
        private void HideQuizElements()
        {
            AnswerBox.Visibility = Visibility.Collapsed;
            CheckAnswerButton.Visibility = Visibility.Collapsed;
            QuestionImage.Visibility = Visibility.Collapsed;
            QuestionCounter.Visibility = Visibility.Collapsed;
            FeedbackText.Text = "";
            ScoreText.Text = "";
            QuestionText.Text = "";
        }

        // Show quiz-related UI elements
        private void ShowQuizElements()
        {
            AnswerBox.Visibility = Visibility.Visible;
            CheckAnswerButton.Visibility = Visibility.Visible;
            QuestionCounter.Visibility = Visibility.Visible;
            FeedbackText.Visibility = Visibility.Visible;
            QuestionText.Visibility = Visibility.Visible;
            ScoreText.Visibility = Visibility.Visible;
        }

        // Handle Start/Stop button click
        private void StartStopButton_Click(object sender, RoutedEventArgs e)
        {
            if (quizManager.IsQuizRunning)
            {
                var result = MessageBox.Show("Highscore voor deze oefening wordt NIET opgeslagen als je stopt. Wil je doorgaan?",
                                           "Stop Oefening",
                                           MessageBoxButton.YesNo,
                                           MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    StopQuiz();
                }
            }
            else
            {
                StartQuiz();
            }
        }

        // Start a new quiz
        private void StartQuiz()
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(PlayerName.Text))
            {
                MessageBox.Show("Voer een naam in om te beginnen met de oefening.", "Naam vereist", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DifficultyLevel.SelectedItem == null)
            {
                MessageBox.Show("Selecteer een moeilijkheidsgraad om te beginnen met de oefening.", "Moeilijkheidsgraad vereist", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Create player with selected difficulty
            DifficultyLevel playerDifficulty;
            switch ((DifficultyLevel.SelectedItem as ComboBoxItem)?.Content.ToString())
            {
                case "Makkelijk":
                    playerDifficulty = Models.DifficultyLevel.Easy;
                    break;
                case "Moeilijk":
                    playerDifficulty = Models.DifficultyLevel.Hard;
                    break;
                default:
                    playerDifficulty = Models.DifficultyLevel.Normal;
                    break;
            }

            player = new Player(PlayerName.Text, playerDifficulty);

            // Update UI
            ConfigurationPanel.Visibility = Visibility.Collapsed;
            HighscoreButton.Visibility = Visibility.Collapsed;
            StartStopButton.Content = "Stop Oefening";
            WelcomeText.Text = "Veel succes!";

            // Show quiz elements
            ShowQuizElements();

            // Start the quiz
            quizManager.StartQuiz(player);

            // Show the first question
            DisplayCurrentQuestion();
        }

        // Stop the current quiz
        private void StopQuiz()
        {
            quizManager.StopQuiz();

            // Update UI
            ConfigurationPanel.Visibility = Visibility.Visible;
            HighscoreButton.Visibility = Visibility.Visible;
            StartStopButton.Content = "Start Oefening";
            WelcomeText.Text = "Welkom bij de Reken Race!";

            // Hide quiz elements
            HideQuizElements();
        }

        // Display the current question
        private void DisplayCurrentQuestion()
        {
            var currentQuestion = quizManager.CurrentQuestion;

            if (currentQuestion != null)
            {
                // Update question counter
                QuestionCounter.Text = $"Vraag {quizManager.CurrentQuestionNumber} van {quizManager.TotalQuestions}";

                // Display question text
                QuestionText.Text = currentQuestion.QuestionText;

                // Display question image
                if (!string.IsNullOrEmpty(currentQuestion.ImageName))
                {
                    BitmapImage image = resourceManager.LoadImage(currentQuestion.ImageName);

                    if (image != null)
                    {
                        QuestionImage.Source = image;
                        QuestionImage.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        QuestionImage.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    QuestionImage.Visibility = Visibility.Collapsed;
                }

                // Clear answer box and feedback
                AnswerBox.Clear();
                FeedbackText.Text = "";
            }
        }

        // Handle Check Answer button click
        private void CheckAnswerButton_Click(object sender, RoutedEventArgs e)
        {
            if (!quizManager.IsQuizRunning) return;

            if (int.TryParse(AnswerBox.Text, out int userAnswer))
            {
                bool isCorrect = quizManager.CheckAnswer(userAnswer);

                if (isCorrect)
                {
                    FeedbackText.Text = "Correct!";
                    resourceManager.PlaySound("correct.mp3");
                }
                else
                {
                    FeedbackText.Text = $"Fout! Het juiste antwoord was {quizManager.CurrentQuestion.CorrectAnswer}.";
                    resourceManager.PlaySound("wrong.mp3");
                }

                // Move to next question or end quiz
                bool hasNextQuestion = quizManager.MoveToNextQuestion();

                if (hasNextQuestion)
                {
                    DisplayCurrentQuestion();
                }
                else
                {
                    // Quiz completed
                    ScoreText.Text = $"Je hebt {quizManager.GetPlayerScore()} van de {quizManager.TotalQuestions} vragen goed!";
                    QuestionText.Text = "Oefening afgerond!";
                    QuestionImage.Visibility = Visibility.Collapsed;
                    QuestionCounter.Visibility = Visibility.Collapsed;

                    // Check for highscore
                    if (highscoreManager.UpdateHighscore(player))
                    {
                        MessageBox.Show($"Gefeliciteerd {player.Name}! Je hebt een nieuwe highscore van {player.Score}!");
                    }

                    StopQuiz();
                }
            }
            else
            {
                FeedbackText.Text = "Voer een geldig nummer in.";
            }
        }

        // Handle Highscore button click
        private void HighscoreButton_Click(object sender, RoutedEventArgs e)
        {
            // Open highscore window
            HighscoreWindow highscoreWindow = new HighscoreWindow(highscoreManager);
            highscoreWindow.Owner = this;
            highscoreWindow.ShowDialog();
        }
    }
}