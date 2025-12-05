using System.Security.Cryptography;

namespace Application.HelperFunctions
{
    public static class RandomizeList
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = RandomNumberGenerator.GetInt32(0, n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }
    }
}
