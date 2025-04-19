using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LigasFutbol.Models
{
    [Table("FUT_ENTRENAMIENTOS")]
    public class Entrenamiento
    {
        [Key]
        [Column("ENTRENAMIENTO_ID")]
        public int EntrenamientoId { get; set; }

        [Column("EQUIPO_ID")]
        public int EquipoId { get; set; }

        [Column("FECHA_HORA")]
        public DateTime FechaHora { get; set; }

        [Column("UBICACION")]
        public string Ubicacion { get; set; }

        [Column("ESTADO")]
        public bool Estado { get; set; }

        public Equipo Equipo { get; set; }
    }
}
