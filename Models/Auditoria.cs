namespace EntradasApi.Models
{
    public class Auditoria
    {
        public int Id { get; set; }

        public string Accion { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        public DateTime Fecha { get; set; }
            = DateTime.Now;
        
        public string Usuario { get; set; } = "";
        
    }
}