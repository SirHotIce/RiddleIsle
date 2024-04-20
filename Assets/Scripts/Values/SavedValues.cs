
namespace Values
{
    public static class SavedValues
    {
        private static int score;
        private static int level;

        public static int Score
        {
            get => score;
            set => score = value;
        }

        public static int Level
        {
            get => level;
            set => level = value;
        }
    }
}
