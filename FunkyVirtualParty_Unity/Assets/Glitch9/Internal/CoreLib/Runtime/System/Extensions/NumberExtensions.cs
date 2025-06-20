namespace Glitch9
{
    public static class NumberExtensions
    {
        public static bool IsOdd(this int number) => number % 2 != 0;
        public static bool IsEven(this int number) => number % 2 == 0;

        public static int IncrementIndex(this int index, int total)
        {
            int newIndex = index++;
            if (total <= 0) return 0; // Avoid division by zero
            return (newIndex + 1) % total; // Ensure non-negative index
        }

        public static int DecrementIndex(this int index, int total)
        {
            int newIndex = index--;
            if (total <= 0) return 0; // Avoid division by zero
            return (newIndex - 1 + total) % total; // Ensure non-negative index
        }
    }
}