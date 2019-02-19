using System;
using System.Collections.Generic;
using System.Text;

public static class RandomNumbers
{
    private static int[] numbers;
    private static int currentIndex;

    public static void Initialize(int begin, int end, int count)
    {
        numbers = new int[end - begin + 1];

        for (int n = begin, i = 0; n <= end; n++, i++)
            numbers[i] = n;

        var rnd = new Random();
        for(int resultPos = 0; resultPos < count; resultPos++)
        {
            int nextResultPos = rnd.Next(resultPos, numbers.Length);

            int temp = numbers[resultPos];
            numbers[resultPos] = numbers[nextResultPos];
            numbers[nextResultPos] = temp;
        }
    }

    public static int NonDuplicateNumber()
    {
        var result = numbers[currentIndex];
        currentIndex++;
        if (currentIndex > numbers.Length - 1)
            currentIndex = 0;

        return result;
    }
}
