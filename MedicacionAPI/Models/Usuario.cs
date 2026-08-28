using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicacionAPI.Models
{
    [Table("Usuario")]
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdUsuario { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Apellido { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Correo { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string ContrasenaHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Rol { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        // Navegación
        public ICollection<Medicamento> Medicamentos { get; set; } = new List<Medicamento>();
        public ContactoFamiliar? ContactoFamiliar { get; set; }
        public ICollection<Confirmacion> Confirmaciones { get; set; } = new List<Confirmacion>();
        public ICollection<Alerta> Alertas { get; set; } = new List<Alerta>();
    }
}