namespace EntradasApi.Models
{
    public class Event
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public required string Venue { get; set; }
    }
}