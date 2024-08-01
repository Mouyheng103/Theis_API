using System.ComponentModel.DataAnnotations;

namespace API.Data.View
{
    public class Address
    {
        [Key]
        public string VillageCode { get; set; } = string.Empty;
        public string VillageName { get; set; } = string.Empty;
        public string CommuneName { get; set; } = string.Empty;
        public string DistrictName { get; set; } = string.Empty;
        public string ProvinceName { get; set; } = string.Empty;
        public string ProvinceCode { get; set; } = string.Empty;
        public string DistrictCode { get; set; } = string.Empty;
        public string CommuneCode { get; set; } = string.Empty;
    }
}
