using System;
using System.Collections.Generic;

public static class FisherYatesShuffle
{
    public static void Shuffle<T>(List<T> list)
    {
        Random random = new Random();
        int n = list.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}
