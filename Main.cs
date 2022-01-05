using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GTA;
using GTA.Math;
using GTA.Native;
using Control = GTA.Control;

namespace Rdr2CinematicCamera
{
    public class Main : Script
    {
        // Variables
        private bool _firstTime = true;
        private bool _isActive = false;

        private const string ModName = "RDR2 Cinematic Camera";
        private const string Developer = "Hermes";
        private const string Version = "1.0a";

        private readonly Config _config;
        private readonly Menu _menu;
        private readonly CinematicBars _cinematicBars;

        private Vector3 _currentDestination;
        private readonly Stopwatch _holdStopwatch = new Stopwatch();
        private bool _alreadyClear = false, _forceCinCam = false, _forceCinCam2 = false, _sameHold = false;

        public Main()
        {
            _cinematicBars = new CinematicBars();

            _config = new Config();
            _menu = new Menu(_config);

            Tick += OnTick;
            KeyDown += OnKeyDown;
            Interval = 0;
        }

        private void OnTick(object sender, EventArgs e)
        {
            if (_config.Enabled)
            {
                if (_forceCinCam)
                    Function.Call(Hash.SET_CINEMATIC_MODE_ACTIVE, true);

                if (_isActive && Game.IsControlPressed(2, Control.NextCamera))
                    _forceCinCam = false;

                if (Game.IsControlPressed(2, Control.VehicleCinCam))
                {
                    if (!_isActive)
                        _forceCinCam2 = false;

                    if (!_holdStopwatch.IsRunning)
                        _holdStopwatch.Start();

                    if (_holdStopwatch.ElapsedMilliseconds > 1000)
                    {
                        _forceCinCam2 = true;

                        CinematicDriveToWaypoint();

                        _holdStopwatch.Stop();
                        _holdStopwatch.Reset();
                    }

                    if (_holdStopwatch.ElapsedMilliseconds < 1000 && _sameHold)
                        if (_isActive) _cinematicBars.DecreaseY(2);
                        else if (!_isActive) _cinematicBars.IncreaseY(2);


                    _sameHold = true;
                }

                if (Game.IsControlJustReleased(2, Control.VehicleCinCam))
                {
                    if (_holdStopwatch.ElapsedMilliseconds < 1000)
                        _cinematicBars.Setup(_isActive ? 1 : 0);

                    _holdStopwatch.Stop();
                    _holdStopwatch.Reset();

                    _forceCinCam = _isActive;
                    _sameHold = false;
                }


                if (Game.IsControlJustReleased(2, Control.VehicleHandbrake) &&
                    Game.IsControlJustReleased(2, Control.VehicleDuck))
                    _menu.Toggle();


                if (_firstTime)
                {
                    UI.Notify(ModName + " " + Version + " by " + Developer + " Loaded");
                    _firstTime = false;
                }

                if (_isActive)
                {
                    _alreadyClear = false;
                    _isActive = Game.IsWaypointActive;
                    Function.Call(Hash.DISPLAY_RADAR, false);
                }

                else
                {
                    if (!_alreadyClear)
                    {
                        Game.Player.Character.Task.ClearAll();
                        _alreadyClear = true;
                    }

                    if (_forceCinCam2)
                        Function.Call(Hash.SET_CINEMATIC_MODE_ACTIVE, false);

                    Function.Call(Hash.DISPLAY_RADAR, true);
                }

                _menu.ProcessMenus();

                if(_config.CinematicBars)
                    _cinematicBars.Draw();
            }
        }

        private void CinematicDriveToWaypoint()
        {
            if (Game.Player.Character.CurrentVehicle == null) return;
            
           if (!_isActive && Game.IsWaypointActive)
            {
                 UI.Notify("Driving started!");
                 _currentDestination = World.GetWaypointPosition();


                Game.Player.Character.Task.DriveTo
                (
                    Game.Player.Character.CurrentVehicle,
                    _currentDestination, 
                    25.0f, 
                    _config.Speed, 
                    (int) _config.DrivingStyle
                );
            }

            else
            {
                UI.Notify("Driving stopped!");
                Game.Player.Character.Task.ClearAll();
            }

            _isActive = !_isActive;
        }


        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F10)
                _menu.Toggle();
        }
    }
}