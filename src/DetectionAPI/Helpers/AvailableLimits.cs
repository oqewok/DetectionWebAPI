using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DetectionAPI.Helpers
{
    [DataContract]
    public class AvailableLimits
    {
        [DataMember]
        [JsonProperty(PropertyName = "limitReached")]
        public bool IsLimitReached { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "imagesCount")]
        public long CurrentImagesCount { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "platesCount")]
        public long CurrentPlatesCount { get; set; }


        [DataMember]
        [JsonProperty(PropertyName = "imagesLimit")]
        public long ImagesLimit { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "platesLimit")]
        public long PlatesLimit { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "currentPlan")]
        public long CurrentPlan { get; set; }

        //public override string ToString() => $@"{IsLimitReached} {CurrentImagesCount} {CurrentPlatesCount} {ImagesLimit} {PlatesLimit} {CurrentPlan}";
    }
}
