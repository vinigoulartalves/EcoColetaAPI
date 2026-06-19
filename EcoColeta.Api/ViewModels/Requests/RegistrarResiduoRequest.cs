using System.ComponentModel.DataAnnotations;
using EcoColeta.Api.Models;

namespace EcoColeta.Api.ViewModels.Requests;

public class RegistrarResiduoRequest
{
    [Required(ErrorMessage = "O ponto de coleta é obrigatório.")]
    [Range(1, int.MaxValue, ErrorMessage = "O ponto de coleta deve ser válido.")]
    public int PontoColetaId { get; set; }

    [Required(ErrorMessage = "O tipo de resíduo é obrigatório.")]
    [EnumDataType(typeof(TipoResiduo), ErrorMessage = "Tipo de resíduo inválido.")]
    public TipoResiduo TipoResiduo { get; set; }

    [Range(0.01, 10000, ErrorMessage = "O peso deve ser entre 0,01 e 10.000 kg.")]
    public decimal PesoKg { get; set; }

    [Required(ErrorMessage = "A origem é obrigatória.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "A origem deve ter entre 2 e 150 caracteres.")]
    public string Origem { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "A observação deve ter no máximo 500 caracteres.")]
    public string? Observacao { get; set; }
}
