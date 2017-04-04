using System;

namespace Leayal
{
    public static class ArrayHelper
    {
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            System.Array.Copy(data, index, result, 0, length);
            return result;
        }
    }
}
