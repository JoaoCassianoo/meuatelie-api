namespace Atelie.Api.Dtos
{
    public class IniciarAssinaturaDto
    {
        public string Periodicidade { get; set; }
    }

    public class AbacateApiResponse<T>
    {
        public T Data { get; set; } = default!;
        public string? Error { get; set; }
    }

    public class BillingData
    {
        public string Id { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }

    public class WebhookPayload
    {
        public string Event { get; set; } = string.Empty;
        public WebhookData Data { get; set; } = new();
    }

    public class WebhookData
    {
        public WebhookBilling Billing { get; set; } = new();
    }

    public class WebhookBilling
    {
        public string Id { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public List<WebhookProduct> Products { get; set; } = new();
    }

    public class WebhookProduct
    {
        public string ExternalId { get; set; } = string.Empty;
    }
}