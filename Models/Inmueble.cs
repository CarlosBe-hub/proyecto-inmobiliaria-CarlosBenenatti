using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ProyectoInmobiliaria.Models
{
    public class Inmueble
    {
        [Key]
        public int IdInmueble { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria")]
        [StringLength(200, ErrorMessage = "La dirección no puede tener más de 200 caracteres")]
        public string? Direccion { get; set; }

        [Required(ErrorMessage = "El tipo de inmueble es obligatorio")]
        [StringLength(50, ErrorMessage = "El tipo no puede superar los 50 caracteres")]
        [Display(Name = "Tipo de Inmueble")]
        public string? TipoInmueble { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        [StringLength(20)]
        public string? Estado { get; set; }

        [Range(1, 10, ErrorMessage = "Los ambientes deben estar entre 1 y 10")]
        public int? Ambientes { get; set; }

        [Range(1, 10000, ErrorMessage = "La superficie debe estar entre 1 y 20,000 m²")]
        public int? Superficie { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser un mayor a 0(cero)")]
        [DataType(DataType.Currency)]
        public decimal? Precio { get; set; }

        [Display(Name = "Propietario")]
        [Required(ErrorMessage = "Debe seleccionar un propietario")]
        public int? PropietarioId { get; set; }

        [ForeignKey(nameof(PropietarioId))]
        [BindNever]
        public Propietario? Propietario { get; set; }
    }
}