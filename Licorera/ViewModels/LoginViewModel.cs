using System.ComponentModel.DataAnnotations;

namespace Licorera.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El correo electr�nico es requerido")]
        [EmailAddress(ErrorMessage = "El formato del correo electr�nico no es v�lido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contrase�a es requerida")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}