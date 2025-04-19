namespace LigasFutbol.Models
{
    public class Jugador
    {
        public int JugadorId { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public int? PosicionId { get; set; }
        public Posicion Posicion { get; set; }
        public int? EquipoId { get; set; }
        public Equipo Equipo { get; set; }

        public bool Estado { get; set; }
    }
}
