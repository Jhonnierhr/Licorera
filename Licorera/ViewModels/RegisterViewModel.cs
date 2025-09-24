using System.ComponentModel.DataAnnotations;

namespace Licorera.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es requerido")]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El correo electr�nico es requerido")]
        [EmailAddress(ErrorMessage = "El formato del correo electr�nico no es v�lido")]
        [Display(Name = "Correo Electr�nico")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contrase�a es requerida")]
        [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contrase�a")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contrase�a")]
        [Compare("Password", ErrorMessage = "Las contrase�as no coinciden.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "El tel�fono es requerido")]
        [Phone(ErrorMessage = "El formato del tel�fono no es v�lido")]
        [Display(Name = "Tel�fono")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "La direcci�n es requerida")]
        [Display(Name = "Direcci�n")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un rol")]
        [Display(Name = "Tipo de Usuario")]
        public int RolId { get; set; }
    }
}