namespace LigasFutbol.Models
{
    public class Clasificacion
    {
        public int ClasificacionId { get; set; }
        public int LigaId { get; set; }
        public int EquipoId { get; set; }
        public int Puntos { get; set; }
        public int PJ { get; set; }  // Partidos Jugados
        public int PG { get; set; }  // Partidos Ganados
        public int PE { get; set; }  // Partidos Empatados
        public int PP { get; set; }  // Partidos Perdidos
        public int GF { get; set; }  // Goles a Favor
        public int GC { get; set; }  // Goles en Contra
    }
}
