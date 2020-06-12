using System;
using System.Collections.Generic;
using System.Text;



namespace ExtensionMethod
{
    public static class ListExtenstions
    {
        public static void AddMany<T>(this List<T> list, params T[] elements)
        {
            list.AddRange(elements);
        }
    }

}
