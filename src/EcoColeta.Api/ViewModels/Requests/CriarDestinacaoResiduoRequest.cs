using System.ComponentModel.DataAnnotations;
using EcoColeta.Api.Models;

namespace EcoColeta.Api.ViewModels.Requests;

public class CriarDestinacaoResiduoRequest
{
    [Required(ErrorMessage = "O tipo de resíduo é obrigatório.")]
    [EnumDataType(typeof(TipoResiduo), ErrorMessage = "Tipo de resíduo inválido.")]
    public TipoResiduo TipoResiduo { get; set; }

    [Required(ErrorMessage = "A descrição é obrigatória.")]
    [StringLength(250, MinimumLength = 5, ErrorMessage = "A descrição deve ter entre 5 e 250 caracteres.")]
    public string Descricao { get; set; } = string.Empty;

    [Required(ErrorMessage = "As instruções de descarte são obrigatórias.")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "As instruções devem ter entre 10 e 1000 caracteres.")]
    public string InstrucoesDescarte { get; set; } = string.Empty;

    public bool Reciclavel { get; set; }

    [Required(ErrorMessage = "O risco ambiental é obrigatório.")]
    [StringLength(250, MinimumLength = 3, ErrorMessage = "O risco ambiental deve ter entre 3 e 250 caracteres.")]
    public string RiscoAmbiental { get; set; } = string.Empty;
}
