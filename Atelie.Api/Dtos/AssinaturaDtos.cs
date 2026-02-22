namespace Atelie.Api.Dtos
{
    public class IniciarAssinaturaDto
    {
        public string Periodicidade { get; set; }
    }

    public class AbacateApiResponse<T>
    {
        public T Data { get; set; }
        public string? Error { get; set; }
    }

    public class BillingData
    {
        public string Id { get; set; }
        public string Url { get; set; }
    }

    public class WebhookPayload
    {
        public string Event { get; set; }
        public WebhookData Data { get; set; }
    }

    public class WebhookData
    {
        public WebhookBilling Billing { get; set; }
    }

    public class WebhookBilling
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public List<WebhookProduct> Products { get; set; }
    }

    public class WebhookProduct
    {
        public string ExternalId { get; set; }
    }
}