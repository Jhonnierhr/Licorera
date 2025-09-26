using System.ComponentModel.DataAnnotations;

namespace Licorera.Models
{
    public class ConfiguracionSistema
    {
        [Key]
        public int ConfiguracionId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Clave { get; set; } = null!;
        
        [Required]
        [StringLength(500)]
        public string Valor { get; set; } = null!;
        
        [StringLength(200)]
        public string? Descripcion { get; set; }
        
        public DateTime? CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
    }
}