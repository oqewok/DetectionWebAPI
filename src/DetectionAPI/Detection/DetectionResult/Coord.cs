using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DetectionAPI.Detection.DetectionResult
{
 
    public class Coord
    {
        
        public string Name { get; set; }

        [JsonProperty(PropertyName = "CoordValue")]
        public int Value { get; set; }
    }
}
