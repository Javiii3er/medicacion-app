using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicacionAPI.Models
{
    [Table("ContactoFamiliar")]
    public class ContactoFamiliar
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdContacto { get; set; }

        [Required]
        public int IdUsuario { get; set; }

        [Required]
        [MaxLength(150)]
        public string NombreFamiliar { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string CorreoFamiliar { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? TelefonoFamiliar { get; set; }

        public bool Activo { get; set; } = true;

        // Navegación
        [ForeignKey("IdUsuario")]
        public Usuario? Usuario { get; set; }
    }
}