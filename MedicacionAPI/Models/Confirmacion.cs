using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicacionAPI.Models
{
    [Table("Confirmacion")]
    public class Confirmacion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdConfirmacion { get; set; }

        [Required]
        public int IdUsuario { get; set; }

        [Required]
        public int IdMedicamento { get; set; }

        [Required]
        public int IdHorario { get; set; }

        [Required]
        public DateOnly FechaConfirmacion { get; set; }

        [Required]
        public TimeOnly HoraConfirmacion { get; set; }

        public DateTime TimestampExacto { get; set; } = DateTime.Now;

        // Navegación
        [ForeignKey("IdUsuario")]
        public Usuario? Usuario { get; set; }

        [ForeignKey("IdMedicamento")]
        public Medicamento? Medicamento { get; set; }

        [ForeignKey("IdHorario")]
        public Horario? Horario { get; set; }
    }
}