namespace EntradasApi.Models
{
    public class Seat
    {
        public int Id { get; set; }

        public int EventId { get; set; }

        public int SectorId { get; set; }

        public int Number { get; set; }

        public DateTime? ReservationTime { get; set; }
        
        public string Status { get; set; } = "Disponible";
        
    }
}