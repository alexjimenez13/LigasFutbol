namespace LigasFutbol.Models
{
    public class Partido
    {
        public int PartidoId { get; set; }
        public int LigaId { get; set; }
        public int EquipoLocalId { get; set; }
        public int EquipoVisitaId { get; set; }
        public DateTime FechaHora { get; set; }
        public string Estadio { get; set; }
        public bool Estado { get; set; }
    }
}
