using System;

namespace RekenApplicatie.Models
{
    // Base abstract class for all questions
    public abstract class Question
    {
        public string QuestionText { get; protected set; }
        public int CorrectAnswer { get; protected set; }
        public string ImageName { get; protected set; }

        // Method to check if the given answer is correct
        public bool CheckAnswer(int userAnswer)
        {
            return userAnswer == CorrectAnswer;
        }

        // Abstract method that must be implemented by derived classes
        public abstract void Generate(DifficultyLevel difficulty, Random random);
    }

    // Enum for difficulty levels
    public enum DifficultyLevel
    {
        Easy,
        Normal,
        Hard
    }

    // Enum for question types (used for image selection)
    public enum QuestionType
    {
        Addition,
        Subtraction,
        Multiplication,
        Division,
        Ratio,
        Geometry
    }

    // Math question class
    public class MathQuestion : Question
    {
        public int Number1 { get; private set; }
        public int Number2 { get; private set; }
        public string Operation { get; private set; }
        public QuestionType Type { get; private set; }

        public override void Generate(DifficultyLevel difficulty, Random random)
        {
            string[] operations = { "+", "-", "*", "/" };
            Operation = operations[random.Next(operations.Length)];

            // Generate numbers based on difficulty
            switch (difficulty)
            {
                case DifficultyLevel.Easy:
                    // Single digits (1-9) for all operations
                    Number1 = random.Next(1, 10);
                    Number2 = random.Next(1, 10);
                    break;

                case DifficultyLevel.Normal:
                    // Double digits for addition/subtraction, smaller for multiplication/division
                    if (Operation == "+" || Operation == "-")
                    {
                        Number1 = random.Next(10, 100);
                        Number2 = random.Next(10, 100);
                    }
                    else
                    {
                        Number1 = random.Next(10, 50);
                        Number2 = random.Next(10, 50);
                    }
                    break;

                case DifficultyLevel.Hard:
                    // Triple digits for addition/subtraction, larger for multiplication/division
                    if (Operation == "+" || Operation == "-")
                    {
                        Number1 = random.Next(100, 1000);
                        Number2 = random.Next(100, 1000);
                    }
                    else
                    {
                        Number1 = random.Next(50, 100);
                        Number2 = random.Next(10, 100);
                    }
                    break;
            }

            // Calculate the correct answer based on operation
            switch (Operation)
            {
                case "+":
                    CorrectAnswer = Number1 + Number2;
                    Type = QuestionType.Addition;
                    break;

                case "-":
                    // Make sure first number is larger for easier calculation
                    if (Number1 < Number2)
                    {
                        int temp = Number1;
                        Number1 = Number2;
                        Number2 = temp;
                    }
                    CorrectAnswer = Number1 - Number2;
                    Type = QuestionType.Subtraction;
                    break;

                case "*":
                    CorrectAnswer = Number1 * Number2;
                    Type = QuestionType.Multiplication;
                    break;

                case "/":
                    if (Number2 == 0) Number2 = 1; // Avoid division by zero
                    // Make division yield whole numbers
                    Number1 = Number2 * random.Next(1, 10);
                    CorrectAnswer = Number1 / Number2;
                    Type = QuestionType.Division;
                    break;
            }

            // Set the image name based on the question type
            ImageName = Type.ToString().ToLower() + ".png";

            // Set the question text
            QuestionText = $"Wat is {Number1} {Operation} {Number2}?";
        }
    }

    // Ratio/Word problem class
    public class RatioQuestion : Question
    {
        public int ObjectAmount { get; private set; }
        public int ObjectTotal { get; private set; }
        public string ChosenObject { get; private set; }

        public override void Generate(DifficultyLevel difficulty, Random random)
        {
            ObjectAmount = random.Next(1, 10);
            ObjectTotal = random.Next(1, 20);

            string[] objects = { "boek", "pen", "fles water", "stoel", "laptop", "armbandje", "kladblok" };
            ChosenObject = objects[random.Next(objects.Length)];

            string objectName = GetCorrectPluralForm(ChosenObject, ObjectTotal);
            CorrectAnswer = ObjectAmount * ObjectTotal;

            // Set specific image name for this object
            ImageName = ChosenObject.Replace(" ", "_") + ".png";

            // Set the question text
            QuestionText = $"Als 1 {ChosenObject} {ObjectAmount} euro kost, hoeveel kosten {ObjectTotal} {objectName}?";
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
    }

    // Geometry question class
    public class GeometryQuestion : Question
    {
        public int Length { get; private set; }
        public int Width { get; private set; }

        public override void Generate(DifficultyLevel difficulty, Random random)
        {
            // Adjust dimensions based on difficulty
            switch (difficulty)
            {
                case DifficultyLevel.Easy:
                    Length = random.Next(1, 10);
                    Width = random.Next(1, 10);
                    break;

                case DifficultyLevel.Normal:
                    Length = random.Next(5, 15);
                    Width = random.Next(5, 15);
                    break;

                case DifficultyLevel.Hard:
                    Length = random.Next(10, 25);
                    Width = random.Next(10, 25);
                    break;
            }

            CorrectAnswer = Length * Width;
            ImageName = "rectangle.png";
            QuestionText = $"Wat is de oppervlakte van een rechthoek met een lengte van {Length} en een breedte van {Width}?";
        }
    }
}