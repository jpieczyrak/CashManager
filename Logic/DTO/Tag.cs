using System.Runtime.Serialization;

namespace LogicOld.DTO
{
    [DataContract(Namespace = "")]
    public class Tag
    {
        [DataMember]
        public string Name { get; set; }
    }
}