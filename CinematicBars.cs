using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GTA;

namespace Rdr2CinematicCamera
{
    public class CinematicBars
    {
        private readonly UIRectangle[] _cinematicBars = new UIRectangle[2];

        public CinematicBars() => Setup(0);

        public void Setup(int i)
        {
            if (i == 0)
            {
                _cinematicBars[0] =
                    new UIRectangle(new Point(0, -100), new Size(1280, 108), Color.Black);

                _cinematicBars[1] =
                    new UIRectangle(new Point(0, 712), new Size(1280, 108), Color.Black);
            }

            else if (i == 1)
            {
                _cinematicBars[0] =
                    new UIRectangle(new Point(0, 0), new Size(1280, 108), Color.Black);

                _cinematicBars[1] =
                    new UIRectangle(new Point(0, 612), new Size(1280, 108), Color.Black);
            }
        }

        public void IncreaseY(int i)
        {
            if (_cinematicBars[0].Position.Y >= 0) return;

            _cinematicBars[0].Position =
                new Point(_cinematicBars[0].Position.X, _cinematicBars[0].Position.Y + i);

            _cinematicBars[1].Position =
                new Point(_cinematicBars[1].Position.X, _cinematicBars[1].Position.Y - i);
        }

        public void DecreaseY(int i)
        {
            if (_cinematicBars[0].Position.Y <= -100) return;

            _cinematicBars[0].Position =
                new Point(_cinematicBars[0].Position.X, _cinematicBars[0].Position.Y - i);

            _cinematicBars[1].Position =
                new Point(_cinematicBars[1].Position.X, _cinematicBars[1].Position.Y + i);
        }

        public void Draw()
        {
            if (_cinematicBars[0].Position.Y <= -100) return;

            _cinematicBars[0].Draw();
            _cinematicBars[1].Draw();
        }
    }
}