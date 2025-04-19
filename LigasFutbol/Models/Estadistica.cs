using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LigasFutbol.Models
{
    public class Estadistica
    {
        [Key]
        [Column("ESTADISTICA_ID")]
        public int EstadisticaId { get; set; }

        [Column("JUGADOR_ID")]
        public int JugadorId { get; set; }

        [Column("PARTIDO_ID")]
        public int PartidoId { get; set; }

        [Column("GOLES")]
        public int Goles { get; set; }

        [Column("ASISTENCIAS")]
        public int Asistencias { get; set; }

        [Column("TARJETAS_AMARILLAS")]
        public int TarjetasAmarillas { get; set; }

        [Column("TARJETAS_ROJAS")]
        public int TarjetasRojas { get; set; }

        // Navegación (opcional)
        public Partido Partido { get; set; }
        public Jugador Jugador { get; set; }
    }
}
