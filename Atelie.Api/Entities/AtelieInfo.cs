using System.ComponentModel.DataAnnotations;

public class AtelieInfo
{
    public int Id { get; set; }

    [Required]
    public Guid UserId { get; set; } // FK auth.users

    [MaxLength(150)]
    public string NomeAtelie { get; set; } = string.Empty;

    [MaxLength(150)]
    public string NomeDono { get; set; } = string.Empty;

    public string Status { get; set; } = "ativa"; // ativa | vencida | trial | cancelada

    public string Plano { get; set; } = "free"; // free | mensal | pro

    public DateTime? DataVencimento { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

     // ID da cobrança gerada na AbacatePay
    public string? BillingId { get; set; }

    // Link para o cliente pagar o PIX
    public string? BillingUrl { get; set; }
}
