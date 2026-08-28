using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicacionAPI.Models
{
    [Table("Medicamento")]
    public class Medicamento
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdMedicamento { get; set; }

        [Required]
        public int IdUsuario { get; set; }

        [Required]
        [MaxLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(8,2)")]
        public decimal Dosis { get; set; }

        [Required]
        [MaxLength(30)]
        public string Unidad { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Frecuencia { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? Notas { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Navegación
        [ForeignKey("IdUsuario")]
        public Usuario? Usuario { get; set; }
        public ICollection<Horario> Horarios { get; set; } = new List<Horario>();
    }
}