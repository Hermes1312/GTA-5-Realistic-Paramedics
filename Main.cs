using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using GTA;
using GTA.Math;
using GTA.Native;
using GTA.UI;
using RealisticParamedics.Models;

namespace RealisticParamedics
{
    public class Main : Script
    {
        private readonly List<ParamedicsTeam> _paramedicsTeams = new List<ParamedicsTeam>();
        private readonly List<Ped> _pendingPeds = new List<Ped>(), _deadPeds = new List<Ped>();
        public Main()
        {
            Tick += OnTick;
            KeyDown += OnKeyDown;
            Interval = 1;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.NumPad7)
            {
                var ambulance = World.CreateVehicle(new Model(VehicleHash.Ambulance),
                    Game.Player.Character.Position.Around(50f));

                var med1 = World.CreatePed(new Model(PedHash.Paramedic01SMM), ambulance.Position.Around(5f));
                var med2 = World.CreatePed(new Model(PedHash.Paramedic01SMM), ambulance.Position.Around(5f));
                med1.SetIntoVehicle(ambulance, VehicleSeat.Driver);
                med2.SetIntoVehicle(ambulance, VehicleSeat.RightFront);


                ambulance.PlaceOnNextStreet();
            }

            if (e.KeyCode == Keys.NumPad9)
            {
                foreach (var ambulance in World.GetNearbyVehicles(Game.Player.Character.Position, 300f, new Model(VehicleHash.Ambulance)))
                    ambulance.Delete();

                foreach (var paramedic in World.GetNearbyPeds(Game.Player.Character.Position, 300f, new Model(PedHash.Paramedic01SMM)))
                    paramedic.Delete();
            }
        }

        private int tempInt = -1;
        private void OnTick(object sender, EventArgs e)
        {
            #region MyRegion
            new TextElement($"{tempInt}", PointF.Empty, 0.5f, Color.Azure).Draw();
            #endregion
            
            foreach (var medicPed in World.GetNearbyPeds(Game.Player.Character, 200f))
            {

                if (medicPed.IsAlive && medicPed.Model == new Model(PedHash.Paramedic01SMM))
                {
                    foreach (var ped in World.GetNearbyPeds(medicPed, 60f))
                    {
                        if (!ped.IsDead) continue;
                        if (_deadPeds != null && !_deadPeds.Contains(ped) && !ped.IsInVehicle()) _deadPeds.Add(ped);
                    }

                    if (medicPed.IsInVehicle() && medicPed.CurrentVehicle.Model == new Model(VehicleHash.Ambulance))
                    {
                        //if(_paramedicsTeams.Count <= 0) continue;
                        var thisTeam = _paramedicsTeams.Select(x => x).FirstOrDefault(x => x.Ambulance == medicPed.CurrentVehicle);
                        if (thisTeam == null)
                        {

                            var team = new ParamedicsTeam
                            {
                                Ambulance = medicPed.CurrentVehicle,
                                Paramedics = new List<Paramedic>
                                {
                                    new Paramedic
                                    {
                                        Ped = medicPed.CurrentVehicle.GetPedOnSeat(VehicleSeat.Driver),
                                        IsDriver = true
                                    },
                                    new Paramedic
                                    {
                                        Ped = medicPed.CurrentVehicle.GetPedOnSeat(VehicleSeat.RightFront),
                                        IsDriver = false
                                    }
                                }
                            };

                            foreach (var paramedic in team.Paramedics)
                                paramedic.Team = team;

                            _paramedicsTeams.Add(team);
                        }

                        else
                        {
                            //Notification.Show($"{thisTeam.RevivedPedsList?.Count}");
                            if (thisTeam.RevivedPedsList.Count == 0 && _deadPeds.Count > 0)
                            {
                                if (!thisTeam.Paramedics[0].IsDrivingToPed)
                                {
                                    thisTeam.Ambulance.IsSirenActive = true;

                                    var nearest = _deadPeds.GetNearestPed(thisTeam.Paramedics[0].Ped);
                                    //if(nearest == null) continue;

                                    thisTeam.CurrentDest = nearest.Position;
                                    thisTeam.Paramedics[0].Ped.Task.DriveTo(thisTeam.Ambulance, thisTeam.CurrentDest, 7.5f, 40f, DrivingStyle.IgnoreLights);
                                    thisTeam.Paramedics[0].IsDrivingToPed = true;
                                }
                                else
                                {
                                    if (thisTeam.Paramedics[0].Ped.Position.DistanceTo(thisTeam.CurrentDest) < 35f)
                                        thisTeam.Ambulance.IsSirenSilent = true;

                                    if (!(thisTeam.Paramedics[0].Ped.Position.DistanceTo(thisTeam.CurrentDest) < 10f))
                                        continue;

                                    foreach (var paramedic in thisTeam.Paramedics)
                                        paramedic.Ped.Task.LeaveVehicle();
                                    
                                }
                            }
                            else
                                thisTeam.DriveAwayWithRevivedPeds();
                        }
                    }

                    else
                    {
                        if(_deadPeds.Count <= 0) continue;;
                        // Get nearby peds that are dea
                        var medicTeam = medicPed.GetTeam(_paramedicsTeams);
                        tempInt = medicTeam == null ? 1 : 0;
                        if (medicTeam == null) continue;

                        foreach (var medic in medicTeam.Paramedics)
                        {
                            // If medic is not running and team has less than 2 revived peds
                            if (!medic.IsRunningToRevive && medicTeam.RevivedPeds < 2)
                            {
                                var nearest = _deadPeds.GetNearestPed(medic.Ped);
                                //if (nearest == null) continue;
                                _pendingPeds.Add(nearest);
                                _deadPeds.Remove(nearest);
                                medic.PendingPed = nearest;
                                medicTeam.RevivedPeds++;
                                medic.IsRunningToRevive = true;
                                medic.Ped.AlwaysKeepTask = true;
                                medic.Ped.Task.RunTo(nearest.Position);
                            }

                            else if(medic.IsRunningToRevive)
                            {
                                if (medic.PendingPed != null && medic.Ped.Position.DistanceTo(medic.PendingPed.Position) < 2f)
                                    _pendingPeds.Remove(medic.PendingPed);
                                    
                                medic.RevivePed();
                            }
                        }
                    }
                }
            }
        }
    }
}