using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;
using IniParser;
using IniParser.Model;

namespace Rdr2CinematicCamera
{
    public class Config
    {
        private const string ConfigPath = "scripts\\Rdr2CinematicCamera\\Rdr2CinematicCamera.ini";
        private readonly FileIniDataParser _iniParser = new FileIniDataParser();

        public readonly List<DrivingStyle> DrivingStyles;
        public DrivingStyle DrivingStyle { get; set; } = DrivingStyle.Normal;
        public int Speed { get; set; } = 50;
        public bool CinematicBars { get; set; } = true;
        public bool Enabled { get; set; } = true;
        
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
                var data = _iniParser.ReadFile(ConfigPath);

                Speed = Convert.ToInt16(data["Global"]["Speed"]);
                DrivingStyle = DrivingStyles[Convert.ToInt16(data["Global"]["DrivingStyle"])];
                CinematicBars = bool.Parse(data["Global"]["CinematicBars"]);
                Enabled = bool.Parse(data["Global"]["Enabled"]);
            }

            else
            {
                if (!Directory.Exists("scripts\\Rdr2CinematicCamera"))
                    Directory.CreateDirectory("scripts\\Rdr2CinematicCamera");

                var firstData = new IniData
                {
                    ["Global"] =
                    {
                        ["Speed"] = "50",
                        ["DrivingStyle"] = "3",
                        ["CinematicBars"] = "true",
                        ["Enabled"] = "true"
                    }
                };

                _iniParser.WriteFile(ConfigPath, firstData, Encoding.UTF8);
            }
        }
        private int GetIndexFromEnum(DrivingStyle drivingStyle)
        {
            for (var i = 0; i < DrivingStyles.Count; i++)
                if (DrivingStyles[i] == drivingStyle)
                    return i;
            return 3;
        }

        public void Save()
        {
            var data = _iniParser.ReadFile(ConfigPath);

            data["Global"]["Speed"] = Speed.ToString();
            data["Global"]["DrivingStyle"] = (GetIndexFromEnum(DrivingStyle)).ToString();
            data["Global"]["CinematicBars"] = CinematicBars.ToString();
            data["Global"]["Enabled"] = Enabled.ToString();

            _iniParser.WriteFile(ConfigPath, data, Encoding.UTF8);
        }
    }
}
