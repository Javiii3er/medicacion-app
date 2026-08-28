using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicacionAPI.Models
{
    [Table("Alerta")]
    public class Alerta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdAlerta { get; set; }

        [Required]
        public int IdUsuario { get; set; }

        [Required]
        public int IdMedicamento { get; set; }

        [Required]
        public int IdHorario { get; set; }

        [Required]
        public TimeOnly HoraProgramada { get; set; }

        [Required]
        public DateTime HoraVencimiento { get; set; }

        [Required]
        [MaxLength(20)]
        public string Estado { get; set; } = "pendiente";

        public DateTime? HoraEnvio { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Navegación
        [ForeignKey("IdUsuario")]
        public Usuario? Usuario { get; set; }

        [ForeignKey("IdMedicamento")]
        public Medicamento? Medicamento { get; set; }

        [ForeignKey("IdHorario")]
        public Horario? Horario { get; set; }
    }
}