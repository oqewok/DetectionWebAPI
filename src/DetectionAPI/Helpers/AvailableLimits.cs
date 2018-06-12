using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace DetectionAPI.Helpers
{
    /// <summary>
    /// Описывает структуру сообщения в HTTP-ответе на запрос ограничений использования сервиса
    /// </summary>
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
    }
}
