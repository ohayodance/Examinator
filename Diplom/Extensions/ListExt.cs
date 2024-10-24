using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diplom.Extensions
{
    static class ListExt
    {
        private static readonly Random random;

        static ListExt()
        {
            random = new Random();
        }

        public static void Shuffle<T>(this IList<T> lst)
        {
            
            int n = lst.Count;

            for (int i = lst.Count - 1; i > 1; i--)
            {
                int rnd = random.Next(i + 1);

                T value = lst[rnd];
                lst[rnd] = lst[i];
                lst[i] = value;
            }
        }
    }
}
