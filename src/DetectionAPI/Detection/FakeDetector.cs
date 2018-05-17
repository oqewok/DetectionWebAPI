using DetectionAPI.Detection.DetectionResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using System.IO;

namespace DetectionAPI.Detection
{
    public class FakeDetector : IDetector
    {
        private Coord[] Coordinates { get; set; }
        public Guid Name { get; set; }

        private FakeDetectorInsider FDI;

        public FakeDetector(FakeDetectorInsider fdi)
        {
            FDI = fdi;
            Name = Guid.NewGuid();
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


        public string GetName()
        {
            return Name.ToString();
        }

        public class FakeDetectorInsider
        {
            public Guid InsiderName { get; set; }

            private Insiderinsider II;

            public FakeDetectorInsider(Insiderinsider ii)
            {
                II = ii;
                InsiderName = Guid.NewGuid();
            }
        }

        public class Insiderinsider
        {
            public Guid InsiderInsiderName { get; set; }
            public List<StreamReader> SomeStreamReader { get; set; }

            public Insiderinsider()
            {
                InsiderInsiderName = Guid.NewGuid();
                SomeStreamReader = new List<StreamReader>();
                for(int i=0; i<500; i++)
                {
                    SomeStreamReader.Add(new StreamReader("D:\\images\\car10326495.jpg"));
                }
                
            }

        }

    }
}
