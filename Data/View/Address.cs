using System.ComponentModel.DataAnnotations;

namespace API.Data.View
{
    public class Address
    {
        [Key]
        public string VillageCode { get; set; }
        public string VillageName { get; set; }
        public string CommuneName { get; set; }
        public string DistrictName { get; set; }
        public string ProvinceName { get; set; }
    }
}
