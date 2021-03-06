﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unconventional.Game
{
    class Line
    {
        public bool IsSolid,
            IsStatic;
        public int From,
            To;

        public bool IsValid
        {
            get
            {
                return From < To &&
                    (To - From) > 2;
            }
        }
    }
}
