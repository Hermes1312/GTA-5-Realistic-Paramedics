using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;

namespace Rdr2CinematicCamera
{
    public class CinematicBars
    {
        private readonly UIRectangle[] _cinematicBars = new UIRectangle[2];

        public CinematicBars()
        {
            var y = Game.ScreenResolution.Height - 360;

            _cinematicBars[0] =
                new UIRectangle(new Point(0, -100), new Size(Game.ScreenResolution.Width, 100), Color.Black);

            _cinematicBars[1] =
                new UIRectangle(new Point(0, y), new Size(Game.ScreenResolution.Width, 100), Color.Black);
        }

        public void Animate(int i)
        {
            if (i == 0)
            {
                if (_cinematicBars[0].Position.Y > -100)
                {
                    const int animSpeed = 2;

                    _cinematicBars[0].Position =
                        new Point(_cinematicBars[0].Position.X, _cinematicBars[0].Position.Y - animSpeed);
                    _cinematicBars[1].Position =
                        new Point(_cinematicBars[1].Position.X, _cinematicBars[1].Position.Y + animSpeed);

                    _cinematicBars[0].Draw();
                    _cinematicBars[1].Draw();
                }
            }

            else
            {
                if (_cinematicBars[0].Position.Y < 0)
                {
                    const int animSpeed = 2;

                    _cinematicBars[0].Position =
                        new Point(_cinematicBars[0].Position.X, _cinematicBars[0].Position.Y + animSpeed);

                    _cinematicBars[1].Position =
                        new Point(_cinematicBars[1].Position.X, _cinematicBars[1].Position.Y - animSpeed);
                }

                _cinematicBars[0].Draw();
                _cinematicBars[1].Draw();
            }
        }
    }
}
