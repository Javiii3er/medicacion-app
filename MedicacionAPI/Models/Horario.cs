using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicacionAPI.Models
{
    [Table("Horario")]
    public class Horario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdHorario { get; set; }

        [Required]
        public int IdMedicamento { get; set; }

        [Required]
        public TimeOnly HoraAdministracion { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Navegación
        [ForeignKey("IdMedicamento")]
        public Medicamento? Medicamento { get; set; }
        public ICollection<Confirmacion> Confirmaciones { get; set; } = new List<Confirmacion>();
        public ICollection<Alerta> Alertas { get; set; } = new List<Alerta>();
    }
}