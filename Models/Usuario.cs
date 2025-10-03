using System.ComponentModel.DataAnnotations;
using ProyectoInmobiliaria.Models;

namespace ProyectoInmobiliaria.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Apellido { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string ClaveHash { get; set; } 

        [Required]
        public string Rol { get; set; } // "Administrador" y "Empleado"

        public string? Avatar { get; set; }
    }
}