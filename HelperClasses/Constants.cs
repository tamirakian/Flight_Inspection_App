using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flight_Inspection_App.HelperClasses
{
    static class Constants
    {
        public const string HOST_IP = "localhost";
        public const int HOST_PORT = 5400;
        public const int NUM_OF_NODES = 42;
        public const float SIMPLE_DETECTOR_TRESHOLD = (float)0.9;
        public const float HYBRID_DETECTOR_MIN_CIRCLE_TRESHOLD = (float)0.5;
    }
}
