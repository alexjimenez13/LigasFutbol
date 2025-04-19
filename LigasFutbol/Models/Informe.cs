namespace LigasFutbol.Models
{
    public class Informe
    {
        public int InformeId { get; set; }
        public string Tipo { get; set; }
        public string Formato { get; set; }  // PDF, EXCEL, etc.
        public DateTime FechaCreacion { get; set; }
        public int? LigaId { get; set; }
    }
}
