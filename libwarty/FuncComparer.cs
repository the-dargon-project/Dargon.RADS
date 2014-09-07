//http://stackoverflow.com/questions/3130922/sortedsett-and-anonymous-icomparert-in-the-constructor-is-not-working
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItzWarty
{
    public class FuncComparer<T> : IComparer<T>
    {
        private readonly Comparison<T> comparison;
        public FuncComparer(Comparison<T> comparison)
        {
            this.comparison = comparison;
        }
        public int Compare(T x, T y)
        {
            return comparison(x, y);
        }
    }

    public class FuncComparer : IComparer
    {
        private readonly Func<object, object, int> comparison;
        public FuncComparer(Func<object, object, int> comparison)
        {
            this.comparison = comparison;
        }
        public int Compare(object x, object y)
        {
            return comparison(x, y);
        }
    }
}
