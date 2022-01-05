using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Extract1
{
    public class IntVec2
    {
        int x = 0, y = 0;
        public IntVec2()
        {

        }
        public IntVec2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int X {
            set => x = value;
            get => x;
        }

        public int Y {
            set => y = value;
            get => y;
        }

        public int Width {
            set => x = value;
            get => x;
        }

        public int Height {
            set => y = value;
            get => y;
        }

    }
}