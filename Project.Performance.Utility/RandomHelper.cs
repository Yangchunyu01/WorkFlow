using System;
using System.Linq;

namespace Project.Performance.Utility
{
    public class RandomHelper
    {
        #region Variables
        private static int seed = DateTime.Now.Millisecond;
        private static Random rnd = new Random(seed);
        #endregion

        #region Methods
        public static uint Random(uint min, uint max)
        {
            double i = RandomHelper.rnd.NextDouble();
            return min + (uint)Math.Round((max - min) * i);
        }

        public static int Random(int min, int max)
        {
            // Can equal to the max value
            return RandomHelper.rnd.Next(min, max + 1);
        }

        public static int[] GetRandomIndex(int count, int maxValue)
        {
            if (count - 1 > maxValue)
            {
                throw new ArgumentException(string.Format("The max value {0} of the array should be equal or greater than the array count {1} - 1.", maxValue, count));
            }

            int[] items = new int[count];
            int item;
            for (int i = 0; i < count; i++)
            {
                item = Random(0, maxValue);
                while (items.Contains(item))
                {
                    item = (item + 1) % (maxValue + 1);
                }
                items[i] = item;
            }
            return items;
        }
        #endregion
    }
}
