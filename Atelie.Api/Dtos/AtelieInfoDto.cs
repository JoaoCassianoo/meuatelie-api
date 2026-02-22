public class AtelieDto
{
    public string NomeAtelie { get; set; } = string.Empty;
    public string NomeDono { get; set; } = string.Empty;
    public string Plano { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? DataVencimento { get; set; }
}

public class UpdateAtelieDto
{
    public string NomeAtelie { get; set; } = string.Empty;
    public string NomeDono { get; set; } = string.Empty;
}

public class RegistroAtelieDto
{
    public string Nome { get; set; } = string.Empty;
    public string AtelieNome { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;   // novo
    public string CpfCnpj { get; set; } = string.Empty;  
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}
