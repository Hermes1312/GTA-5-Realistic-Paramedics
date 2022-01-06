using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rdr2CinematicCamera
{
    public static class Shared
    {
        public static Config Config { get; set; }
        public static bool IsActive { get; set; } = false;
    }
}
