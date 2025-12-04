namespace WareHouse_Service.Domain.Entity
{
    public class Instrument
    {
        public Guid Id { get; set; }
        public string InstrumentName { get; set; } = string.Empty;
        public string[] SupportTestType { get; set; } = new string[0];
        public string Status { get; set; } = "Pending";
        public int Load { get; set; }
    }
}
