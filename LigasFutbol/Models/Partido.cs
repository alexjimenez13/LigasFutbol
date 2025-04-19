namespace LigasFutbol.Models
{
    public class Partido
    {
        public int PartidoId { get; set; }
        public DateTime FechaHora { get; set; }
        public bool Estado { get; set; }
        public int LigaId { get; set; }
        public Liga Liga { get; set; }
        public int EquipoLocalId { get; set; }
        public Equipo EquipoLocal { get; set; }

        public int EquipoVisitanteId { get; set; }
        public Equipo EquipoVisitante { get; set; }
        public string Estadio { get; set; }

    }
}
