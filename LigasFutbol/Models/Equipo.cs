namespace LigasFutbol.Models
{
    public class Equipo
    {
        public int EquipoId { get; set; }
        public string Nombre { get; set; }
        public int LigaId { get; set; }
        public string Entrenador { get; set; }
        public bool Estado { get; set; }
    }
}
