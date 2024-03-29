﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frogs
{
    public class Player3 : Frog
    {
        public Player3(FliesCollection flies) : base(flies)
        {
            position = new Point(487, Adjustments.Ground);

            direction = (CustomRandom.GetNumber(0,2)!=0);
            id = 3;
            img1 = Properties.Resources.frog3_1;
            img2 = Properties.Resources.frog3_2;
            imgjump = Properties.Resources.frog3_jump;
            img1F = Properties.Resources.frog3_1;
            img1F.RotateFlip(RotateFlipType.RotateNoneFlipX);
            img2F = Properties.Resources.frog3_2;
            img2F.RotateFlip(RotateFlipType.RotateNoneFlipX);
            imgjumpF = Properties.Resources.frog3_jump;
            imgjumpF.RotateFlip(RotateFlipType.RotateNoneFlipX);

            CreateTongue();

        }


    }
}
