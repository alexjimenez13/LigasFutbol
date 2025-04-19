namespace LigasFutbol.Models
{
    public class Entrenamiento
    {
        public int EntrenamientoId { get; set; }
        public int EquipoId { get; set; }
        public DateTime FechaHora { get; set; }
        public string Ubicacion { get; set; }
        public bool Estado { get; set; }
    }
}
