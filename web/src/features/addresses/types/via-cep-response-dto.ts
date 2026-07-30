export interface ViaCepResponseDto {
  cep?: string | null
  logradouro?: string | null
  complemento?: string | null
  bairro?: string | null
  localidade?: string | null
  uf?: string | null
  ibge?: string | null
  gia?: string | null
  ddd?: string | null
  siafi?: string | null
  erro?: boolean | null
}
