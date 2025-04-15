using System;
using System.Collections.Generic;
using RekenApplicatie.Models;

namespace RekenApplicatie.Utilities
{
    // QuizManager class to manage the quiz state and progression
    public class QuizManager
    {
        private Random random = new Random();
        private List<Question> questions;
        private int currentQuestionIndex;
        private Player player;

        public bool IsQuizRunning { get; private set; }
        public int TotalQuestions => questions?.Count ?? 0;
        public int CurrentQuestionNumber => currentQuestionIndex + 1;
        public Question CurrentQuestion => currentQuestionIndex < TotalQuestions ? questions[currentQuestionIndex] : null;

        public QuizManager()
        {
            questions = new List<Question>();
            currentQuestionIndex = 0;
            IsQuizRunning = false;
        }

        // Start a new quiz with the given player
        public void StartQuiz(Player player)
        {
            this.player = player;
            IsQuizRunning = true;
            currentQuestionIndex = 0;

            // Generate questions based on player difficulty
            GenerateQuestions(player.Difficulty);

            // Shuffle questions
            ShuffleQuestions();
        }

        // Stop the current quiz
        public void StopQuiz()
        {
            IsQuizRunning = false;
        }

        // Move to the next question
        public bool MoveToNextQuestion()
        {
            if (!IsQuizRunning || currentQuestionIndex >= questions.Count - 1)
                return false;

            currentQuestionIndex++;
            return true;
        }

        // Check the player's answer and update score
        public bool CheckAnswer(int answer)
        {
            if (!IsQuizRunning || CurrentQuestion == null)
                return false;

            bool isCorrect = CurrentQuestion.CheckAnswer(answer);

            if (isCorrect)
            {
                player.AddPoints(player.CalculatePoints());
            }

            return isCorrect;
        }

        // Generate questions based on difficulty
        private void GenerateQuestions(DifficultyLevel difficulty)
        {
            questions.Clear();
            int numberOfQuestions = 0;

            switch (difficulty)
            {
                case DifficultyLevel.Easy:
                    numberOfQuestions = 10;
                    break;
                case DifficultyLevel.Normal:
                    numberOfQuestions = 13;
                    break;
                case DifficultyLevel.Hard:
                    numberOfQuestions = 15;
                    break;
            }

            // Create questions based on difficulty
            for (int i = 0; i < numberOfQuestions; i++)
            {
                Question question;

                if (difficulty == DifficultyLevel.Hard && i % 4 == 0)
                {
                    // Add more geometry questions for hard difficulty
                    question = new GeometryQuestion();
                }
                else if (i % 2 == 0)
                {
                    // Add math questions
                    question = new MathQuestion();
                }
                else
                {
                    // Add ratio/word problems
                    question = new RatioQuestion();
                }

                // Generate the question with correct difficulty
                question.Generate(difficulty, random);
                questions.Add(question);
            }
        }

        // Shuffle the questions list
        private void ShuffleQuestions()
        {
            int n = questions.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                var temp = questions[k];
                questions[k] = questions[n];
                questions[n] = temp;
            }
        }

        // Get player score
        public int GetPlayerScore()
        {
            return player?.Score ?? 0;
        }
    }
}