using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;

namespace RealisticParamedics
{
    public static class Functions
    {
        public static void EnterNearestAmbulance(Ped ped)
        {
            ped.Task.EnterVehicle(World.GetNearbyVehicles(ped, new Model(VehicleHash.Ambulance))[0], VehicleSeat.Driver);
        }
    }
}
