using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlateDetector.Detection;

namespace DetectionAPI.Misc.DetectionCollector
{
    public class DetectionAlgCollector
    {
        public ConcurrentBag<Detector> DetectorBag { get; set; }

        public DetectionAlgCollector()
        {
            DetectorBag = new ConcurrentBag<Detector>()
            {
                
            };
        }

    }
}
