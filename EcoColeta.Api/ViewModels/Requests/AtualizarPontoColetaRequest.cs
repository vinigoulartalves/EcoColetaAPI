using System.ComponentModel.DataAnnotations;
using EcoColeta.Api.Models;

namespace EcoColeta.Api.ViewModels.Requests;

public class AtualizarPontoColetaRequest
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(150, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 150 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O endereço é obrigatório.")]
    [StringLength(250, MinimumLength = 5, ErrorMessage = "O endereço deve ter entre 5 e 250 caracteres.")]
    public string Endereco { get; set; } = string.Empty;

    [Required(ErrorMessage = "O bairro é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O bairro deve ter entre 2 e 100 caracteres.")]
    public string Bairro { get; set; } = string.Empty;

    [Required(ErrorMessage = "A cidade é obrigatória.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "A cidade deve ter entre 2 e 100 caracteres.")]
    public string Cidade { get; set; } = string.Empty;

    [Range(-90, 90, ErrorMessage = "A latitude deve estar entre -90 e 90.")]
    public double Latitude { get; set; }

    [Range(-180, 180, ErrorMessage = "A longitude deve estar entre -180 e 180.")]
    public double Longitude { get; set; }

    [Required(ErrorMessage = "O tipo de resíduo aceito é obrigatório.")]
    [EnumDataType(typeof(TipoResiduo), ErrorMessage = "Tipo de resíduo inválido.")]
    public TipoResiduo TipoResiduoAceito { get; set; }

    [Range(1, 100000, ErrorMessage = "A capacidade máxima deve ser entre 1 e 100.000 kg.")]
    public decimal CapacidadeMaximaKg { get; set; }

    public bool Ativo { get; set; } = true;
}
