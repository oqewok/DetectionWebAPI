using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DetectionAPI.Detection.DetectionResult
{
 
    public class Coord
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "value")]
        public int Value { get; set; }




        [DataContract]
        class Response
        {
            [DataMember]
            [JsonProperty(PropertyName = "foundZones")]
            public Bounds[] FoundZones { get; set; }
        }

        [DataContract]
        class Bounds
        {
            [DataMember]
            [JsonProperty(PropertyName = "x")]
            public int X { get; set; }

            [DataMember]
            [JsonProperty(PropertyName = "y")]
            public int Y { get; set; }

            [DataMember]
            [JsonProperty(PropertyName = "width")]
            public int Width { get; set; }

            [DataMember]
            [JsonProperty(PropertyName = "height")]
            public int Height { get; set; }
        }
    }
}
