using System.Runtime.Serialization;

namespace Logic.DTO
{
    [DataContract(Namespace = "")]
    public class Tag
    {
        [DataMember]
        public string Name { get; set; }
    }
}