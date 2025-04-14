using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RekenApplicatie
{
    public partial class MainWindow : Window
    {
        private int score;
        private int questionCount;
        private int totalQuestions; // Added to track total questions
        private int correctAnswer;
        private Random random = new Random();
        private List<Action> questions;
        private string highscoreFilePath = "highscore.txt";
        private bool isQuizRunning = false;
        private MediaPlayer _mediaPlayer = new MediaPlayer();
        private string imagesFolder;

        // Enum to track question types for image selection
        private enum QuestionType
        {
            Addition,
            Subtraction,
            Multiplication,
            Division,
            Ratio,
            Geometry
        }

        public MainWindow()
        {
            InitializeComponent();

            AnswerBox.Visibility = Visibility.Collapsed;
            CheckAnswerButton.Visibility = Visibility.Collapsed;
            QuestionImage.Visibility = Visibility.Collapsed;
            QuestionCounter.Visibility = Visibility.Collapsed; // Make sure it's hidden at start

            // Set up the images folder path
            imagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Images");

            // Create the images directory if it doesn't exist
            if (!Directory.Exists(imagesFolder))
            {
                Directory.CreateDirectory(imagesFolder);
            }

            if (!File.Exists(highscoreFilePath))
            {
                File.WriteAllText(highscoreFilePath, "0");
            }
        }

        private void SetQuestionImage(QuestionType type, string specificImage = null)
        {
            try
            {
                string imageName;

                if (!string.IsNullOrEmpty(specificImage))
                {
                    // Use specific image if provided
                    imageName = specificImage;
                }
                else
                {
                    // Otherwise use default image based on question type
                    switch (type)
                    {
                        case QuestionType.Addition:
                            imageName = "addition.png";
                            break;
                        case QuestionType.Subtraction:
                            imageName = "subtraction.png";
                            break;
                        case QuestionType.Multiplication:
                            imageName = "multiplication.png";
                            break;
                        case QuestionType.Division:
                            imageName = "division.png";
                            break;
                        case QuestionType.Ratio:
                            imageName = "ratio.png";
                            break;
                        case QuestionType.Geometry:
                            imageName = "geometry.png";
                            break;
                        default:
                            imageName = "default.png";
                            break;
                    }
                }

                string imagePath = Path.Combine(imagesFolder, imageName);

                // If the image exists, display it
                if (File.Exists(imagePath))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();

                    QuestionImage.Source = bitmap;
                    QuestionImage.Visibility = Visibility.Visible;
                }
                else
                {
                    // If no image is found, hide the image control
                    QuestionImage.Visibility = Visibility.Collapsed;

                    // Optionally log for debugging
                    Console.WriteLine($"Image not found: {imagePath}");
                }
            }
            catch (Exception ex)
            {
                // Hide image on error
                QuestionImage.Visibility = Visibility.Collapsed;
                Console.WriteLine($"Error loading image: {ex.Message}");
            }
        }

        private void PlaySound(string soundFileName)
        {
            try
            {
                string soundFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Sounds", soundFileName);
                _mediaPlayer.Open(new Uri(soundFilePath, UriKind.Absolute));
                _mediaPlayer.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij het afspelen van geluid: {ex.Message}");
            }
        }

        private void StartStop_Click(object sender, RoutedEventArgs e)
        {
            if (isQuizRunning)
            {
                var result = MessageBox.Show("Highscore voor deze oefening wordt NIET opgeslagen als je stopt. Wil je doorgaan?",
                                             "Stop Oefening",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    StopOefening();
                }
            }
            else
            {
                StartOefening();
            }
        }

        private void StartOefening()
        {
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

            isQuizRunning = true;
            score = 0;
            questionCount = 0;
            FeedbackText.Text = "";
            ScoreText.Text = "";

            NameLabel.Visibility = Visibility.Collapsed;
            PlayerName.Visibility = Visibility.Collapsed;
            DifficultyLabel.Visibility = Visibility.Collapsed;
            DifficultyLevel.Visibility = Visibility.Collapsed;
            HighscoreButton.Visibility = Visibility.Collapsed;
            StartStopButton.Content = "Stop Oefening";

            WelcomeText.Text = "Veel succes!";

            QuestionText.Visibility = Visibility.Visible;
            FeedbackText.Visibility = Visibility.Visible;
            AnswerBox.Visibility = Visibility.Visible;
            CheckAnswerButton.Visibility = Visibility.Visible;
            ScoreText.Visibility = Visibility.Visible;
            QuestionCounter.Visibility = Visibility.Visible; // Show the question counter

            GenerateQuestions();

            // Set total questions count for the counter display
            totalQuestions = questions.Count;

            NextQuestion();
        }

        private void StopOefening()
        {
            isQuizRunning = false;
            StartStopButton.Content = "Start Oefening";
            NameLabel.Visibility = Visibility.Visible;
            PlayerName.Visibility = Visibility.Visible;
            DifficultyLabel.Visibility = Visibility.Visible;
            DifficultyLevel.Visibility = Visibility.Visible;
            HighscoreButton.Visibility = Visibility.Visible;

            WelcomeText.Text = "Welkom bij de Reken Race!";

            QuestionText.Visibility = Visibility.Collapsed;
            FeedbackText.Visibility = Visibility.Collapsed;
            AnswerBox.Visibility = Visibility.Collapsed;
            CheckAnswerButton.Visibility = Visibility.Collapsed;
            ScoreText.Visibility = Visibility.Collapsed;
            QuestionImage.Visibility = Visibility.Collapsed;
            QuestionCounter.Visibility = Visibility.Collapsed; // Hide the question counter when stopping

            QuestionText.Text = "";
            FeedbackText.Text = "";
            AnswerBox.Clear();
            ScoreText.Text = "";
            QuestionImage.Source = null;
            QuestionCounter.Text = ""; // Clear the counter text
        }

        private void NextQuestion()
        {
            if (questionCount < questions.Count)
            {
                questionCount++;

                // Update the question counter text
                QuestionCounter.Text = $"Vraag {questionCount} van {totalQuestions}";

                questions[questionCount - 1]();  // Roep de volgende vraagfunctie aan
            }
            else
            {
                ScoreText.Text = $"Je hebt {score} van de {questions.Count} vragen goed!";
                QuestionText.Text = "Oefening afgerond!";
                QuestionImage.Visibility = Visibility.Collapsed;
                QuestionCounter.Visibility = Visibility.Collapsed; // Hide the counter at the end
                CheckHighscore();
                StopOefening();  // Reset naar startstatus
            }
        }

        private void GenerateMathQuestion()
        {
            int number1 = 0, number2 = 0;
            string[] operations = { "+", "-", "*", "/" };
            string operation = operations[random.Next(operations.Length)];
            string difficulty = (DifficultyLevel.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Makkelijk";
            QuestionType questionType;

            switch (difficulty)
            {
                case "Makkelijk":
                    // Single digits (0-9) voor optellen en aftrekken
                    if (operation == "+" || operation == "-")
                    {
                        number1 = random.Next(1, 10);
                        number2 = random.Next(1, 10);
                    }
                    else
                    {
                        number1 = random.Next(1, 10);
                        number2 = random.Next(1, 10);
                    }
                    break;

                case "Normaal":
                    // Double digits (10-99) voor optellen en aftrekken
                    if (operation == "+" || operation == "-")
                    {
                        number1 = random.Next(10, 100);
                        number2 = random.Next(10, 100);
                    }
                    else
                    {
                        number1 = random.Next(10, 50);
                        number2 = random.Next(10, 50);
                    }
                    break;

                case "Moeilijk":
                    // Triple digits (100-999) voor optellen en aftrekken
                    if (operation == "+" || operation == "-")
                    {
                        number1 = random.Next(100, 1000);
                        number2 = random.Next(100, 1000);
                    }
                    else
                    {
                        number1 = random.Next(50, 100);
                        number2 = random.Next(10, 100);
                    }
                    break;
            }

            switch (operation)
            {
                case "+":
                    correctAnswer = number1 + number2;
                    questionType = QuestionType.Addition;
                    break;
                case "-":
                    if (number1 < number2)
                    {
                        int temp = number1;
                        number1 = number2;
                        number2 = temp;
                    }
                    correctAnswer = number1 - number2;
                    questionType = QuestionType.Subtraction;
                    break;
                case "*":
                    correctAnswer = number1 * number2;
                    questionType = QuestionType.Multiplication;
                    break;
                case "/":
                    if (number2 == 0) number2 = 1;
                    // Make sure division results in whole numbers for easier calculation
                    number1 = number2 * random.Next(1, 10);
                    correctAnswer = number1 / number2;
                    questionType = QuestionType.Division;
                    break;
                default:
                    questionType = QuestionType.Addition;
                    break;
            }

            // Set question image based on operation type
            SetQuestionImage(questionType);

            // Removed the questionCount from the question text since it's now in the counter
            QuestionText.Text = $"Wat is {number1} {operation} {number2}?";
        }

        private void GenerateRatioOrWordProblem()
        {
            int objectAmount = random.Next(1, 10);
            int objectTotal = random.Next(1, 20);
            string[] objects = { "boek", "pen", "fles water", "stoel", "laptop", "armbandje", "kladblok" };
            string chosenObject = objects[random.Next(objects.Length)];

            string objectName = GetCorrectPluralForm(chosenObject, objectTotal);
            correctAnswer = objectAmount * objectTotal;

            // Set image based on object type
            string specificImage = $"{chosenObject.Replace(" ", "_")}.png";
            SetQuestionImage(QuestionType.Ratio, specificImage);

            // Removed the questionCount from the question text since it's now in the counter
            QuestionText.Text = $"Als 1 {chosenObject} {objectAmount} euro kost, hoeveel kosten {objectTotal} {objectName}?";
        }

        private string GetCorrectPluralForm(string word, int quantity)
        {
            if (quantity > 1)
            {
                if (word == "fles water") return "flessen water";
                if (word.EndsWith("je")) return word + "s";
                return word + "en";
            }
            return word;
        }

        private void GenerateGeometryQuestion()
        {
            int length = random.Next(1, 10);
            int width = random.Next(1, 10);

            correctAnswer = length * width;

            // Set image for geometry question
            SetQuestionImage(QuestionType.Geometry, "rectangle.png");

            // Removed the questionCount from the question text since it's now in the counter
            QuestionText.Text = $"Wat is de oppervlakte van een rechthoek met een lengte van {length} en een breedte van {width}?";
        }

        private void CheckAnswer_Click(object sender, RoutedEventArgs e)
        {
            if (!isQuizRunning) return;

            if (int.TryParse(AnswerBox.Text, out int userAnswer))
            {
                int points = 1;
                string difficulty = (DifficultyLevel.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Makkelijk";

                switch (difficulty)
                {
                    case "Normaal":
                        points = 2;
                        break;
                    case "Moeilijk":
                        points = 3;
                        break;
                }

                if (userAnswer == correctAnswer)
                {
                    FeedbackText.Text = "Correct!";
                    PlaySound("correct.mp3");
                    score += points;
                }
                else
                {
                    FeedbackText.Text = $"Fout! Het juiste antwoord was {correctAnswer}.";
                    PlaySound("wrong.mp3");
                }

                AnswerBox.Clear();
                NextQuestion();
            }
            else
            {
                FeedbackText.Text = "Voer een geldig nummer in.";
            }
        }

        private void CheckHighscore()
        {
            string playerName = PlayerName.Text.Trim();
            if (string.IsNullOrEmpty(playerName))
            {
                MessageBox.Show("Voer je naam in om een highscore op te slaan.");
                return;
            }

            // Lees bestaande highscore gegevens
            var highscoreLines = File.Exists(highscoreFilePath) ? File.ReadAllLines(highscoreFilePath) : new string[0];
            var highscoreEntries = new Dictionary<string, int>();

            foreach (var line in highscoreLines)
            {
                var parts = line.Split(',');
                if (parts.Length == 2 && int.TryParse(parts[0], out int scoreValue))
                {
                    highscoreEntries[parts[1]] = scoreValue;
                }
            }

            // Update de score als deze hoger is
            if (highscoreEntries.ContainsKey(playerName))
            {
                if (score > highscoreEntries[playerName])
                {
                    highscoreEntries[playerName] = score;
                }
            }
            else
            {
                highscoreEntries[playerName] = score;
            }

            // Sorteer de scores op aflopende volgorde en schrijf ze weer naar het bestand
            var sortedEntries = highscoreEntries.OrderByDescending(e => e.Value).ToList();
            File.WriteAllLines(highscoreFilePath, sortedEntries.ConvertAll(e => $"{e.Value},{e.Key}"));

            MessageBox.Show($"Gefeliciteerd {playerName}! Je hebt een nieuwe highscore van {score}!");
        }

        private void BekijkHighscore_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(highscoreFilePath))
            {
                string highscoreText = File.ReadAllText(highscoreFilePath);
                MessageBox.Show($"Highscores:\n{highscoreText}");
            }
            else
            {
                MessageBox.Show("Geen highscore gegevens gevonden.");
            }
        }

        private void GenerateQuestions()
        {
            questions = new List<Action>();
            string difficulty = (DifficultyLevel.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Makkelijk";

            int numberOfQuestions = 0;

            switch (difficulty)
            {
                case "Makkelijk":
                    numberOfQuestions = 10;
                    for (int i = 0; i < numberOfQuestions; i++)
                    {
                        // Voeg alleen wiskundevragen en verhoudingen toe
                        if (i % 2 == 0)
                        {
                            questions.Add(GenerateMathQuestion);
                        }
                        else
                        {
                            questions.Add(GenerateRatioOrWordProblem);
                        }
                    }
                    break;

                case "Normaal":
                    numberOfQuestions = 13;
                    for (int i = 0; i < numberOfQuestions; i++)
                    {
                        if (i % 2 == 0)
                        {
                            questions.Add(GenerateMathQuestion);
                        }
                        else
                        {
                            questions.Add(GenerateRatioOrWordProblem);
                        }
                    }
                    break;

                case "Moeilijk":
                    numberOfQuestions = 15;
                    for (int i = 0; i < numberOfQuestions; i++)
                    {
                        // Voeg een grotere hoeveelheid meetkunde toe samen met andere vraagtypes
                        if (i % 4 == 0)
                        {
                            questions.Add(GenerateGeometryQuestion);
                        }
                        else if (i % 2 == 0)
                        {
                            questions.Add(GenerateMathQuestion);
                        }
                        else
                        {
                            questions.Add(GenerateRatioOrWordProblem);
                        }
                    }
                    break;
            }

            Shuffle(questions); // Shuffle om vragen in willekeurige volgorde te krijgen
        }

        private void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}