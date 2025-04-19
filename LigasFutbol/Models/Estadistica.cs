namespace LigasFutbol.Models
{
    public class Estadistica
    {
        public int EstadisticaId { get; set; }
        public int JugadorId { get; set; }
        public int PartidoId { get; set; }
        public int Goles { get; set; }
        public int Asistencias { get; set; }
        public int TarjetasAmarillas { get; set; }
        public int TarjetasRojas { get; set; }
    }
}
