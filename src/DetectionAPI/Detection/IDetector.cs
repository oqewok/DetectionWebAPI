﻿using DetectionAPI.Detection.DetectionResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DetectionAPI.Detection
{
    public interface IDetector
    {
        Coord[] Detect();
        string GetName();
    }
}
