using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ProyectoInmobiliaria.Models
{
    public class Contrato : IValidatableObject
    {
        [Key]
        public int IdContrato { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Inicio")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de finalización es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Fin")]
        public DateTime FechaFin { get; set; }

        [Required(ErrorMessage = "El monto mensual es obligatorio")]
        [Range(1, double.MaxValue, ErrorMessage = "El monto debe ser mayor que 0")]
        [DataType(DataType.Currency)]
        [Display(Name = "Monto Mensual")]
        public decimal Monto { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        [StringLength(20, ErrorMessage = "El estado no puede superar los 20 caracteres")]
        [RegularExpression("^(Activo|Inactivo|Finalizado)$", ErrorMessage = "El estado debe ser Activo, Inactivo o Finalizado")]
        public string? Estado { get; set; } = "Activo";

        // Relación con Inmueble
        [Required(ErrorMessage = "Debe seleccionar un inmueble")]
        [Display(Name = "Inmueble")]
        public int InmuebleId { get; set; }

        [ForeignKey(nameof(InmuebleId))]
        [BindNever]
        public Inmueble? Inmueble { get; set; }

        // Relación con Inquilino
        [Required(ErrorMessage = "Debe seleccionar un inquilino")]
        [Display(Name = "Inquilino")]
        public int InquilinoId { get; set; }

        [ForeignKey(nameof(InquilinoId))]
        [BindNever]
        public Inquilino? Inquilino { get; set; }

        // Propiedades para terminar anticipado
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Finalización Anticipada")]
        [Column("FechaFinAnticipada")]
        public DateTime? FechaAnticipada { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Multa")]
        public decimal? Multa { get; set; }

        // Validacion
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FechaInicio >= FechaFin)
            {
                yield return new ValidationResult(
                    "La fecha de inicio debe ser menor que la fecha de finalización.",
                    new[] { nameof(FechaInicio), nameof(FechaFin) });
            }

            if (FechaAnticipada.HasValue && FechaAnticipada < FechaInicio)
            {
                yield return new ValidationResult(
                    "La fecha de finalización anticipada no puede ser anterior a la fecha de inicio.",
                    new[] { nameof(FechaAnticipada) });
            }
        }
    }
}
