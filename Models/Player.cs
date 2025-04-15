namespace RekenApplicatie.Models
{
    // Player class to store player information
    public class Player
    {
        public string Name { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public int Score { get; private set; }

        public Player(string name, DifficultyLevel difficulty)
        {
            Name = name;
            Difficulty = difficulty;
            Score = 0;
        }

        public void AddPoints(int points)
        {
            Score += points;
        }

        public void ResetScore()
        {
            Score = 0;
        }

        // Calculate points based on difficulty
        public int CalculatePoints()
        {
            switch (Difficulty)
            {
                case DifficultyLevel.Easy:
                    return 1;
                case DifficultyLevel.Normal:
                    return 2;
                case DifficultyLevel.Hard:
                    return 3;
                default:
                    return 1;
            }
        }
    }
}