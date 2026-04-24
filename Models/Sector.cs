namespace EntradasApi.Models
{
    public class Sector
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";
        
        public int EventId { get; set; }
    
        public DateTime? ReservationTime { get; set; }
    }
}