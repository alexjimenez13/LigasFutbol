namespace LigasFutbol.Models
{
    public class Clasificacion
    {
        public int EquipoId { get; set; }
        public string NombreEquipo { get; set; }
        public int PartidosJugados { get; set; }
        public int Ganados { get; set; }
        public int Empatados { get; set; }
        public int Perdidos { get; set; }
        public int GolesAFavor { get; set; }
        public int GolesEnContra { get; set; }
        public int DiferenciaGoles { get; set; }
        public int Puntos { get; set; }
    }
}
