<div align="center">

# ✂️ Meu Ateliê — API

**API REST para gestão completa do Ateliê Janainy Fiel**

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com)
[![C#](https://img.shields.io/badge/C%23-99.5%25-239120?style=for-the-badge&logo=csharp)](https://learn.microsoft.com/dotnet/csharp)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-Supabase-4169E1?style=for-the-badge&logo=postgresql)](https://supabase.com)
[![Docker](https://img.shields.io/badge/Docker-Enabled-2496ED?style=for-the-badge&logo=docker)](https://www.docker.com)
[![Render](https://img.shields.io/badge/Deploy-Render-46E3B7?style=for-the-badge&logo=render)](https://render.com)

[🌐 Frontend](https://github.com/JoaoCassianoo/meuatelie-frontend) • [📦 Deploy](https://meuatelie-api.onrender.com)

</div>

---

## 📋 Sobre o projeto

Backend do **Meu Ateliê**, um SaaS completo para gerenciamento de ateliês de costura. A API oferece controle financeiro, estoque de materiais, encomendas, vendas, todo list e sistema de assinaturas com pagamento via PIX integrado à AbacatePay.

---

## 🚀 Funcionalidades

- 🔐 **Autenticação** via Supabase Auth com JWT
- 💰 **Financeiro** — lançamentos de receitas e despesas
- 📦 **Estoque** — materiais e peças prontas
- 📋 **Encomendas** — controle de pedidos com status
- 🛒 **Vendas** — registro e histórico de vendas
- ✅ **Todo List** — tarefas do ateliê
- 💳 **Assinaturas** — planos mensal, trimestral e anual via PIX (AbacatePay)
- 🔒 **Middleware de plano** — bloqueio automático de acesso por inadimplência
- 🔑 **Recuperação de senha** via Supabase

---

## 🛠️ Tecnologias

| Tecnologia | Uso |
|---|---|
| ASP.NET Core 8 | Framework principal |
| Entity Framework Core | ORM com PostgreSQL |
| Supabase | Banco de dados + Auth |
| AbacatePay | Gateway de pagamento PIX |
| Hangfire | Jobs agendados |
| Docker | Containerização |
| Render | Deploy em produção |

---

## ⚙️ Configuração

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com) (opcional)
- Conta no [Supabase](https://supabase.com)
- Conta na [AbacatePay](https://abacatepay.com)

### Variáveis de ambiente

Crie um `appsettings.json` ou configure as variáveis de ambiente:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "sua_connection_string_postgresql"
  },
  "Supabase": {
    "Url": "https://xxxx.supabase.co/",
    "Key": "sua_anon_key"
  },
  "AbacatePay": {
    "ApiKey": "sua_api_key",
    "WebhookSecret": "seu_webhook_secret"
  }
}
```

### Rodando localmente

```bash
# Clone o repositório
git clone https://github.com/JoaoCassianoo/meuatelie-api.git
cd meuatelie-api

# Restaurar dependências
dotnet restore

# Aplicar migrations
dotnet ef database update

# Rodar a aplicação
dotnet run --project Atelie.Api
```

### Rodando com Docker

```bash
docker build -t meuatelie-api .
docker run -p 8080:8080 meuatelie-api
```

---

## 📁 Estrutura do projeto

```
Atelie.Api/
├── Controllers/         # Endpoints da API
│   ├── AtelieController.cs
│   ├── EncomendaController.cs
│   ├── EstoqueController.cs
│   ├── FinanceiroController.cs
│   ├── AssinaturaController.cs
│   └── WebhookController.cs
├── Services/            # Regras de negócio
│   ├── AtelieService.cs
│   ├── EncomendaService.cs
│   ├── AssinaturaService.cs
│   └── AbacatePayService.cs
├── Entities/            # Models do banco
├── Dtos/                # Objetos de transferência
├── Data/                # DbContext
├── Middlewares/         # PlanoAtivoMiddleware
└── Migrations/          # Migrations do EF Core
```

---

## 🔗 Endpoints principais

| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/api/Atelie/registrar` | Cadastro de novo ateliê |
| `GET` | `/api/Atelie` | Dados do ateliê logado |
| `POST` | `/api/assinatura/iniciar` | Gerar cobrança de assinatura |
| `POST` | `/api/webhook/pagamento` | Webhook de confirmação de pagamento |
| `GET` | `/api/Encomenda` | Listar encomendas |
| `GET` | `/api/Financeiro` | Listar lançamentos financeiros |
| `GET` | `/api/Estoque` | Listar materiais em estoque |

---

## 💳 Sistema de assinaturas

O sistema de assinaturas utiliza a **AbacatePay** como gateway de pagamento via PIX. O fluxo funciona da seguinte forma:

```
Usuário seleciona plano
→ API cria cliente + cobrança na AbacatePay
→ Retorna URL de pagamento
→ Usuário paga via PIX
→ AbacatePay dispara webhook
→ API ativa o acesso por 30/90/365 dias
```

### Planos disponíveis

| Plano | Valor | Acesso |
|---|---|---|
| Mensal | R$ 40,00 | 30 dias |
| Trimestral | R$ 108,00 | 90 dias |
| Anual | R$ 360,00 | 365 dias |

---

## 🔒 Autenticação

A API utiliza **JWT Bearer** com tokens emitidos pelo Supabase Auth. Todos os endpoints são protegidos por padrão, exceto:

- `POST /api/Atelie/registrar`
- `POST /api/webhook/pagamento`

---

## 📄 Licença

Este projeto é de uso privado. Todos os direitos reservados.

---

<div align="center">
  Feito com ❤️ por <a href="https://github.com/JoaoCassianoo">João Cassiano</a>
</div>