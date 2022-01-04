using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;

namespace Rdr2CinematicCamera
{
    public class Config
    {
        private const string ConfigPath = "scripts\\Rdr2CinematicCamera\\Rdr2CinematicCamera.cfg";

        public readonly List<DrivingStyle> DrivingStyles;
        public DrivingStyle DrivingStyle { get; set; } = DrivingStyle.Normal;
        public int Speed { get; set; } = 50;

        public Config()
        {
            DrivingStyles = new List<DrivingStyle>()
            {
                DrivingStyle.AvoidTraffic,
                DrivingStyle.AvoidTrafficExtremely,
                DrivingStyle.IgnoreLights,
                DrivingStyle.Normal,
                DrivingStyle.Rushed,
                DrivingStyle.SometimesOvertakeTraffic
            };

            if (File.Exists(ConfigPath))
            {
                var cfgLines = File.ReadAllLines("scripts\\Rdr2CinematicCamera\\Rdr2CinematicCamera.cfg");
                DrivingStyle = DrivingStyles[Convert.ToInt16(cfgLines[0])];
                Speed = Convert.ToInt16(cfgLines[1]);
            }

            else
            {
                if (!Directory.Exists("scripts\\Rdr2CinematicCamera"))
                    Directory.CreateDirectory("scripts\\Rdr2CinematicCamera");

                File.WriteAllLines(ConfigPath, new []
                {
                    "3",
                    "50"
                });
            }
        }
    }
}
