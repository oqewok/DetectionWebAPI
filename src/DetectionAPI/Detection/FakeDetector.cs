using DetectionAPI.Detection.DetectionResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;

namespace DetectionAPI.Detection
{
    public class FakeDetector : IDetector
    {
        private Coord[] Coordinates { get; set; }
        public Guid Name { get; set; }

        public FakeDetector()
        {
            //Name = Guid.NewGuid().ToString();
        }

        public Coord[] Detect()
        {
            Random rnd = new Random();

            int x1 = rnd.Next(1, 1920);
            int y1 = rnd.Next(1, 1080);
            int x2 = rnd.Next(x1, x1 + 70);
            int y2 = rnd.Next(y1, y1 + 30);

            Coord[] dr = new Coord[]
            {
            new Coord { Name = "x1", Value = x1 },
            new Coord { Name = "y1", Value = y1 },
            new Coord { Name = "x2", Value = x2 },
            new Coord { Name = "y2", Value = y2 }
            };

            return dr;
        }

    }
}
