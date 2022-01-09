using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;
using RealisticParamedics.Models;

namespace RealisticParamedics
{
    public static class ExtensionMethods
    {
        public static ParamedicsTeam GetTeam(this Ped paramedic, List<ParamedicsTeam> paramedicsTeams)
        {
            if (paramedic == null || paramedicsTeams == null || paramedicsTeams.Count <= 0) return null;

            foreach (var paramedicsTeam in paramedicsTeams)
                foreach(var _paramedic in paramedicsTeam.Paramedics)
                    if (paramedic == _paramedic.Ped)
                        return paramedicsTeam;

            return null;
        }

        public static Ped GetNearestPed(this List<Ped> peds, Ped ped)
        {
            if(peds != null && ped != null)
                return peds.OrderByDescending(x => x.Bones[Bone.SkelSpine3].Position.DistanceTo(ped.Position)).LastOrDefault();

            return null;
        }
    }
}
