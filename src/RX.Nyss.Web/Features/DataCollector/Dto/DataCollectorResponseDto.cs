using RX.Nyss.Data.Concepts;

namespace RX.Nyss.Web.Features.DataCollector
{
    public class DataCollectorResponseDto
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public Sex Sex { get; set; }
        public string PhoneNumber { get; set; }
        public string AdditionalPhoneNumber { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
