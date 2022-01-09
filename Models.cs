using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;
using GTA.Math;
using GTA.UI;

namespace RealisticParamedics.Models
{
    public class ParamedicsTeam
    {
        public List<Paramedic> Paramedics { get; set; } = new List<Paramedic>(2);
        public Vehicle Ambulance { get; set; }
        public int RevivedPeds { get; set; } = 0;
        public List<Ped> RevivedPedsList { get; set; } = new List<Ped>(2);
        public Vector3 CurrentDest { get; set; }
        private bool InVehFlag { get; set; } = false;
        private bool IsDrivingAway { get; set; } = false;

        private void DriveAway()
        {
            if (IsDrivingAway) return;

            Notification.Show("DriveAway");
            Ped driver = null;

            foreach (var medic in Paramedics.Where(medic => medic.IsDriver))
                driver = medic.Ped;

            Ambulance.IsSirenSilent = false;
            Ambulance.IsSirenActive = true;
            driver?.Task.CruiseWithVehicle(Ambulance, 100f, DrivingStyle.IgnoreLights);
            IsDrivingAway = true;
        }

        public void DriveAwayWithRevivedPeds()
        {
            if (Paramedics.Count <= 0 || Ambulance == null) return;
            
            foreach (var medic in Paramedics)
                if(medic.Ped != null)
                    if(!medic.Ped.IsInVehicle(Ambulance))
                        return;

            if (RevivedPedsList.Count <= 0) return;

            if (RevivedPedsList.Count == 1)
                if (RevivedPedsList[0].IsInVehicle(Ambulance))
                    DriveAway();

            if (RevivedPedsList.Count != 2) return;
            if (RevivedPedsList[0].IsInVehicle(Ambulance) && RevivedPedsList[1].IsInVehicle(Ambulance))
                DriveAway();
        }
    }

    public class Paramedic
    {
        public Ped Ped { get; set; }
        public ParamedicsTeam Team { get; set; }
        public bool IsRunningToRevive { get; set; } = false;
        public Ped PendingPed { get; set; }
        public Ped RevivedPed { get; set; }
        public bool IsDriver { get; set; }
        public bool IsDrivingToPed { get; set; } = false;
        private bool AnimFlag { get; set; }
        private bool AnimFlag2 { get; set; }
        public bool Flag { get; set; } = false;
        public bool InVehFlag { get; set; } = true;
        private bool tempFlag = false;

        private Stopwatch AnimStopwatch { get; } = new Stopwatch();
        private Stopwatch AnimStopwatch2 { get; } = new Stopwatch();

        public void DrawDebug()
        {
            var pos = new PointF(0, IsDriver ? 0 : 100);
            new TextElement($"InVehFlag: {InVehFlag}\n Flag: {Flag}", pos, 0.5f, Color.White).Draw();
        }

        public void RevivePed()
        {
            if (PendingPed != null && Ped.Position.DistanceTo(PendingPed.Position) < 1f)
            {
                if (!AnimStopwatch.IsRunning)
                {
                    if (!AnimFlag)
                    {
                        Ped.Task.PlayAnimation("mini@cpr@char_a@cpr_str", "cpr_pumpchest", 8f, -8f, 4000,
                            AnimationFlags.Loop, 0f);
                        AnimStopwatch.Start();
                    }
                }

                else
                {
                    if (AnimStopwatch.ElapsedMilliseconds > 4000)
                    {
                        Ped.Task.PlayAnimation("amb@medic@standing@kneel@exit", "exit", 8f, -8f, 4000,
                            AnimationFlags.None, 0f);

                        PendingPed.Resurrect();

                        //PendingPed.Task.PerformSequence(PendingPedSequence);
                        PendingPed.Task.PlayAnimation("get_up@directional@transition@prone_to_knees@injured",
                            "back_armsdown", 8f, -8f, 4000, AnimationFlags.None, 0f);

                        AnimFlag = true;
                        AnimStopwatch.Reset();
                        AnimStopwatch.Stop();

                        AnimStopwatch2.Start();
                    }
                }
            }

            if (AnimStopwatch2.IsRunning && AnimFlag)
            {
                if (AnimStopwatch2.ElapsedMilliseconds > 2000 && AnimStopwatch2.ElapsedMilliseconds < 4000 &&!AnimFlag2)
                {
                    PendingPed.Task.PlayAnimation("get_up@directional@movement@from_knees@injured", "getup_r_180", 8f, -8f,4000, AnimationFlags.None, 0f);
                    AnimFlag2 = true;
                }

                if (AnimStopwatch2.ElapsedMilliseconds > 4000)
                {
                    Ped.Task.EnterVehicle(Team.Ambulance, IsDriver ? VehicleSeat.Driver : VehicleSeat.RightFront,
                        speed: 10f);

                    IsRunningToRevive = false;
                    RevivedPed = PendingPed;
                    PendingPed = null;

                    RevivedPed.AlwaysKeepTask = true;
                    RevivedPed.Task.EnterVehicle(Team.Ambulance, IsDriver ? VehicleSeat.LeftRear : VehicleSeat.RightRear);

                    Team.RevivedPedsList.Add(RevivedPed);

                    AnimFlag = false;
                    Flag = true;

                    AnimStopwatch2.Reset();
                    AnimStopwatch2.Stop();
                }
            }
            ///////////////////////
            if (PendingPed != null && Ped.Position.DistanceTo(PendingPed.Position) < 1f)
            {
                if (!AnimStopwatch.IsRunning)
                {
                    if (!AnimFlag)
                    {
                        Ped.Task.PlayAnimation("mini@cpr@char_a@cpr_str", "cpr_pumpchest", 8f, -8f, 4000, AnimationFlags.Loop, 0f);
                        AnimStopwatch.Start();
                    }
                }

                else
                {
                    if (AnimStopwatch.ElapsedMilliseconds > 4000)
                    {
                        Ped.Task.PlayAnimation("amb@medic@standing@kneel@exit", "exit", 8f, -8f, 4000, AnimationFlags.None, 0f);

                        PendingPed.Resurrect();

                        PendingPed.Task.PlayAnimation("get_up@directional@transition@prone_to_knees@injured","back_armsdown", 8f, -8f, 4000, AnimationFlags.None, 0f);

                        AnimFlag = true;
                        AnimStopwatch.Reset();
                        AnimStopwatch.Stop();

                        AnimStopwatch2.Start();
                    }
                }
            }

            if (AnimStopwatch2.IsRunning && AnimFlag)
            {
                if (AnimStopwatch2.ElapsedMilliseconds > 2000 && AnimStopwatch2.ElapsedMilliseconds < 4000 &&
                    !AnimFlag2)
                {
                    PendingPed.Task.PlayAnimation("get_up@directional@movement@from_knees@injured", "getup_r_180", 8f,-8f, 4000, AnimationFlags.None, 0f);
                    AnimFlag2 = true;
                }


                if (AnimStopwatch2.ElapsedMilliseconds > 4000)
                {
                    Ped.Task.EnterVehicle(Team.Ambulance, IsDriver ? VehicleSeat.Driver : VehicleSeat.RightFront, speed: 10f);

                    IsRunningToRevive = false;
                    RevivedPed = PendingPed;
                    PendingPed = null;

                    RevivedPed.AlwaysKeepTask = true;
                    RevivedPed.Task.EnterVehicle(Team.Ambulance, IsDriver ? VehicleSeat.LeftRear : VehicleSeat.RightRear);

                    Team.RevivedPedsList.Add(RevivedPed);

                    AnimFlag = false;
                    Flag = true;

                    AnimStopwatch2.Reset();
                    AnimStopwatch2.Stop();
                }
            }
            ///////////////////////
            if (Flag)
            {
                foreach (var revivedPed in Team.RevivedPedsList)
                    if (!revivedPed.IsInVehicle(Team.Ambulance))
                        InVehFlag = false;


                if (InVehFlag)
                {
                    Ped driver = null;

                    foreach (var medic in Team.Paramedics)
                        if (medic.IsDriver)
                            driver = medic.Ped;

                    Team.Ambulance.IsSirenActive = true;
                    driver?.Task.CruiseWithVehicle(Team.Ambulance, 100f, DrivingStyle.IgnoreLights);
                }
            }
        }
    }
}