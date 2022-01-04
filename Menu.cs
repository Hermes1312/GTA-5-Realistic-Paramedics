using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;
using GTA.Math;
using NativeUI;

namespace Rdr2CinematicCamera
{
    public class Menu
    {
        private readonly Config _config;
        private readonly UIMenu _uiMenu;
        private readonly MenuPool _menuPool;

        public Menu(Config config)
        {
            _config = config;

            _menuPool = new MenuPool();
            _uiMenu = new UIMenu("Cinematic Camera", "Like in Red Dead Redemption 2");

            _menuPool.Add(_uiMenu);
            
            var listOfDrivingStyles = new List<object>
            {
                "Avoid Traffic",
                "Avoid Traffic Extremely",
                "Ignore Lights",
                "Normal",
                "Rushed",
                "Sometimes Overtake Traffic"
            };

            var menuDrivingStyles = new UIMenuListItem("Driving style: ", listOfDrivingStyles, 0);
            _uiMenu.AddItem(menuDrivingStyles);
            menuDrivingStyles.Index = GetIndexFromEnum(config.DrivingStyle);

            var menuSpeed = new UIMenuSliderItem("Speed: ")
            {
                Maximum = 250,
                Value = config.Speed
            };

            _uiMenu.AddItem(menuSpeed);

            var saveButton = new UIMenuItem("Save changes");
            _uiMenu.AddItem(saveButton);

            _uiMenu.OnItemSelect += (sender, item, index) =>
            {
                if (item != saveButton) return;

                File.WriteAllText("scripts\\Rdr2CinematicCamera\\Rdr2CinematicCamera.cfg",
                    $"{menuDrivingStyles.Index}{Environment.NewLine}{menuSpeed.Value}");

                config.DrivingStyle = config.DrivingStyles[menuDrivingStyles.Index];
                config.Speed = menuSpeed.Value;
            };
        }

        public void ProcessMenus()
            => _menuPool?.ProcessMenus();

        public void Toggle()
            => _uiMenu.Visible = !_uiMenu.Visible;

        private int GetIndexFromEnum(DrivingStyle drivingStyle)
        {
            for (var i = 0; i < _config.DrivingStyles.Count; i++)
                if (_config.DrivingStyles[i] == drivingStyle)
                    return i;
            return 3;
        }

    }
}
