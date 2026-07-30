namespace SeniorCrud.Application.DTOs.ViaCep;

public sealed record ViaCepResponseDto(
    string? Cep,
    string? Logradouro,
    string? Complemento,
    string? Bairro,
    string? Localidade,
    string? Uf,
    string? Ibge,
    string? Gia,
    string? Ddd,
    string? Siafi,
    bool? Erro);
