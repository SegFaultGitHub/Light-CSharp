using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArcadeGame
{
    class WTuple<T1, T2>
    {
        private T1 item1_;
        public T1 Item1_
        {
            get { return item1_; }
            set { item1_ = value; }
        }

        private T2 item2_;
        public T2 Item2_
        {
            get { return item2_; }
            set { item2_ = value; }
        }

        public WTuple(T1 item1, T2 item2)
        {
            item1_ = item1;
            item2_ = item2;
        }
    }
}
