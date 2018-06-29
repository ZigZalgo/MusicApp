using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Extensions
{
    public static class ExtensionMethods
    {
        public static T[] Slice<T>(this T[] source, int index, int length)
        {
            T[] slice;
            if (index + length > source.Length)
            {
                slice = new T[source.Length - index];
                Array.Copy(source, index, slice, 0, source.Length - index);
            }
            else
            {
                slice = new T[length]; 
                Array.Copy(source, index, slice, 0, length);
            }

            return slice;
        }
    }
}
