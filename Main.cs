using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GTA;
using GTA.Math;
using GTA.Native;
using GTA.UI;
using Control = GTA.Control;

namespace Rdr2CinematicCamera
{
    public class Main : Script
    {
        // Variables
        private bool _firstTime = true;

        private const string ModName = "RDR2 Cinematic Camera";
        private const string Developer = "Hermes";
        private const string Version = "1.1";

        private readonly Menu _menu;
        private readonly CinematicBars _cinematicBars;
        private readonly Stopwatch _holdStopwatch = new Stopwatch();
        private int _pressedCounter;
        private bool _forceCinCam = false, _forceCinCam2 = false, _sameHold = false;
        private bool _alreadyClear;
        private Vector3 _currentDestination;

        public Main()
        {
            _cinematicBars = new CinematicBars();
            Shared.Config = new Config();
            _menu = new Menu(Shared.Config);

            Tick += OnTick;
            KeyDown += OnKeyDown;
            Interval = 0;
        }


        private void OnTick(object sender, EventArgs e)
        {
            new TextElement(_pressedCounter.ToString(), Point.Empty, 1.0f).Draw();

            if (!Shared.Config.Enabled) return;

            if (_forceCinCam)
                Function.Call(Hash.SET_CINEMATIC_MODE_ACTIVE, true);

            if (Shared.IsActive && Game.IsControlPressed(Control.NextCamera))
                _forceCinCam = false;

            if (Game.IsControlJustPressed(Control.VehicleCinCam))
                _pressedCounter++;

            if (Game.IsControlPressed(Control.VehicleCinCam))
            {
                if (!Shared.IsActive)
                    _forceCinCam2 = false;

                if (!_holdStopwatch.IsRunning)
                    _holdStopwatch.Start();

                if (_holdStopwatch.ElapsedMilliseconds > 1000 && _pressedCounter == 1)
                {
                    _forceCinCam2 = true;
                    CinematicDriveToWaypoint();
                    _holdStopwatch.Stop();
                    _holdStopwatch.Reset();
                    _pressedCounter = 0;
                }

                if (_holdStopwatch.ElapsedMilliseconds < 1000 && _sameHold && _pressedCounter == 1)
                    if (Shared.IsActive) _cinematicBars.DecreaseY(2);
                    else _cinematicBars.IncreaseY(2);

                _sameHold = true;
            }

            if (Game.IsControlJustReleased(Control.VehicleCinCam))
            {
                if (_holdStopwatch.ElapsedMilliseconds < 1000)
                    if (Shared.IsActive)
                        _cinematicBars.Setup(1);
                    else
                        _cinematicBars.DecreaseY(2);

                _holdStopwatch.Stop();
                _holdStopwatch.Reset();

                _forceCinCam = Shared.IsActive;
                _sameHold = false;
                _pressedCounter = 0;
            }


            if (Game.IsControlJustReleased(Control.VehicleHandbrake) &&
                Game.IsControlJustReleased(Control.VehicleDuck))
                _menu.Toggle();


            if (_firstTime)
            {
                Notification.Show(ModName + " " + Version + " by " + Developer + " Loaded");
                _firstTime = false;
            }

            if (Shared.IsActive)
            {
                Function.Call(Hash.DISPLAY_RADAR, false); 
                _alreadyClear = false;
                Shared.IsActive = Game.IsWaypointActive;
            }

            else
            {
                if (!_alreadyClear)
                {
                    Game.Player.Character.Task.ClearAll();
                    _alreadyClear = true;
                }

                if (!_sameHold)
                    _cinematicBars.DecreaseY(2);
                    
                if (_forceCinCam2)
                    Function.Call(Hash.SET_CINEMATIC_MODE_ACTIVE, false);

                Function.Call(Hash.DISPLAY_RADAR, true);
            }

            _menu.ProcessMenus();

            if (Shared.Config.CinematicBars && Game.Player.Character.CurrentVehicle != null && Game.IsWaypointActive)
                _cinematicBars.Draw();
        }
        public void CinematicDriveToWaypoint()
        {
            if (Game.Player.Character.CurrentVehicle == null) return;

            if (!Shared.IsActive && Game.IsWaypointActive)
            {
                //Notification.Show("Driving started!");
                _currentDestination = World.WaypointPosition;

                Game.Player.Character.Task.DriveTo
                (
                    Game.Player.Character.CurrentVehicle,
                    _currentDestination,
                    25.0f,
                    Shared.Config.Speed,
                    Shared.Config.DrivingStyle
                );
            }

            else
            {
                //Notification.Show("Driving stopped!");
                Game.Player.Character.Task.ClearAll();
            }

            Shared.IsActive = !Shared.IsActive;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F10)
                _menu.Toggle();
        }
    }
}