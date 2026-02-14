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
