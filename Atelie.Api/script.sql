CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;

CREATE TABLE "MovimentacoesFinanceiro" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_MovimentacoesFinanceiro" PRIMARY KEY AUTOINCREMENT,
    "Descricao" TEXT NOT NULL,
    "Valor" TEXT NOT NULL,
    "Contexto" INTEGER NOT NULL,
    "MeioPagamento" INTEGER NOT NULL,
    "Data" TEXT NOT NULL
);

CREATE TABLE "Produtos" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Produtos" PRIMARY KEY AUTOINCREMENT,
    "Nome" TEXT NOT NULL,
    "Descricao" TEXT NULL,
    "Valor" TEXT NOT NULL,
    "DataCriacao" TEXT NOT NULL
);

CREATE TABLE "MovimentacoesEstoque" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_MovimentacoesEstoque" PRIMARY KEY AUTOINCREMENT,
    "ProdutoId" INTEGER NOT NULL,
    "Quantidade" INTEGER NOT NULL,
    "Data" TEXT NOT NULL,
    "Observacao" TEXT NULL,
    CONSTRAINT "FK_MovimentacoesEstoque_Produtos_ProdutoId" FOREIGN KEY ("ProdutoId") REFERENCES "Produtos" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_MovimentacoesEstoque_ProdutoId" ON "MovimentacoesEstoque" ("ProdutoId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260112183615_InitialCreate', '8.0.0');

COMMIT;

BEGIN TRANSACTION;

ALTER TABLE "MovimentacoesEstoque" ADD "Tipo" INTEGER NOT NULL DEFAULT 0;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260113001912_EstoqueInicial', '8.0.0');

COMMIT;

BEGIN TRANSACTION;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260114141936_AtualizaMetodoPagamento', '8.0.0');

COMMIT;

BEGIN TRANSACTION;

ALTER TABLE "MovimentacoesEstoque" ADD "MaterialId" INTEGER NOT NULL DEFAULT 0;

CREATE TABLE "ListasTarefa" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ListasTarefa" PRIMARY KEY AUTOINCREMENT,
    "Titulo" TEXT NOT NULL,
    "DataCriacao" TEXT NOT NULL
);

CREATE TABLE "Materiais" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Materiais" PRIMARY KEY AUTOINCREMENT,
    "Nome" TEXT NOT NULL,
    "Categoria" INTEGER NOT NULL,
    "Tamanho" TEXT NULL,
    "Quantidade" INTEGER NOT NULL,
    "Valor" TEXT NOT NULL,
    "Status" INTEGER NOT NULL,
    "DataEntrada" TEXT NOT NULL,
    "DataSaida" TEXT NULL
);

CREATE TABLE "Tarefas" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Tarefas" PRIMARY KEY AUTOINCREMENT,
    "ListaTarefaId" INTEGER NOT NULL,
    "Descricao" TEXT NOT NULL,
    "Concluido" INTEGER NOT NULL,
    "DataConclusao" TEXT NULL,
    "DataCriacao" TEXT NOT NULL,
    CONSTRAINT "FK_Tarefas_ListasTarefa_ListaTarefaId" FOREIGN KEY ("ListaTarefaId") REFERENCES "ListasTarefa" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Encomendas" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Encomendas" PRIMARY KEY AUTOINCREMENT,
    "Descricao" TEXT NOT NULL,
    "MaterialId" INTEGER NOT NULL,
    "Status" INTEGER NOT NULL,
    "ValorOrcado" TEXT NOT NULL,
    "Data" TEXT NOT NULL,
    "DataFinalizacao" TEXT NULL,
    "Cliente" TEXT NULL,
    "Observacao" TEXT NULL,
    CONSTRAINT "FK_Encomendas_Materiais_MaterialId" FOREIGN KEY ("MaterialId") REFERENCES "Materiais" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Vendas" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Vendas" PRIMARY KEY AUTOINCREMENT,
    "MaterialId" INTEGER NOT NULL,
    "Quantidade" INTEGER NOT NULL,
    "ValorVenda" TEXT NOT NULL,
    "Cliente" TEXT NULL,
    "Data" TEXT NOT NULL,
    "Observacao" TEXT NULL,
    CONSTRAINT "FK_Vendas_Materiais_MaterialId" FOREIGN KEY ("MaterialId") REFERENCES "Materiais" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_MovimentacoesEstoque_MaterialId" ON "MovimentacoesEstoque" ("MaterialId");

CREATE INDEX "IX_Encomendas_MaterialId" ON "Encomendas" ("MaterialId");

CREATE INDEX "IX_Tarefas_ListaTarefaId" ON "Tarefas" ("ListaTarefaId");

CREATE INDEX "IX_Vendas_MaterialId" ON "Vendas" ("MaterialId");

CREATE TABLE "ef_temp_MovimentacoesEstoque" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_MovimentacoesEstoque" PRIMARY KEY AUTOINCREMENT,
    "Data" TEXT NOT NULL,
    "MaterialId" INTEGER NOT NULL,
    "Observacao" TEXT NULL,
    "ProdutoId" INTEGER NULL,
    "Quantidade" INTEGER NOT NULL,
    "Tipo" INTEGER NOT NULL,
    CONSTRAINT "FK_MovimentacoesEstoque_Materiais_MaterialId" FOREIGN KEY ("MaterialId") REFERENCES "Materiais" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_MovimentacoesEstoque_Produtos_ProdutoId" FOREIGN KEY ("ProdutoId") REFERENCES "Produtos" ("Id")
);

INSERT INTO "ef_temp_MovimentacoesEstoque" ("Id", "Data", "MaterialId", "Observacao", "ProdutoId", "Quantidade", "Tipo")
SELECT "Id", "Data", "MaterialId", "Observacao", "ProdutoId", "Quantidade", "Tipo"
FROM "MovimentacoesEstoque";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;

DROP TABLE "MovimentacoesEstoque";

ALTER TABLE "ef_temp_MovimentacoesEstoque" RENAME TO "MovimentacoesEstoque";

COMMIT;

PRAGMA foreign_keys = 1;

BEGIN TRANSACTION;

CREATE INDEX "IX_MovimentacoesEstoque_MaterialId" ON "MovimentacoesEstoque" ("MaterialId");

CREATE INDEX "IX_MovimentacoesEstoque_ProdutoId" ON "MovimentacoesEstoque" ("ProdutoId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260120152311_CreateEstoqueVendaEncomendaTodoList', '8.0.0');

COMMIT;

BEGIN TRANSACTION;

DROP INDEX "IX_MovimentacoesEstoque_ProdutoId";

DROP INDEX "IX_Encomendas_MaterialId";

ALTER TABLE "Vendas" RENAME COLUMN "MaterialId" TO "ProdutoId";

DROP INDEX "IX_Vendas_MaterialId";

CREATE INDEX "IX_Vendas_ProdutoId" ON "Vendas" ("ProdutoId");

ALTER TABLE "Produtos" ADD "DataVenda" TEXT NULL;

ALTER TABLE "Produtos" ADD "EncomendaId" INTEGER NULL;

ALTER TABLE "Produtos" ADD "Quantidade" INTEGER NOT NULL DEFAULT 0;

ALTER TABLE "Produtos" ADD "Vendido" INTEGER NOT NULL DEFAULT 0;

CREATE TABLE "ProdutoMateriais" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ProdutoMateriais" PRIMARY KEY AUTOINCREMENT,
    "ProdutoId" INTEGER NOT NULL,
    "MaterialId" INTEGER NOT NULL,
    "QuantidadeUsada" INTEGER NOT NULL,
    CONSTRAINT "FK_ProdutoMateriais_Materiais_MaterialId" FOREIGN KEY ("MaterialId") REFERENCES "Materiais" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ProdutoMateriais_Produtos_ProdutoId" FOREIGN KEY ("ProdutoId") REFERENCES "Produtos" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_Produtos_EncomendaId" ON "Produtos" ("EncomendaId");

CREATE INDEX "IX_ProdutoMateriais_MaterialId" ON "ProdutoMateriais" ("MaterialId");

CREATE INDEX "IX_ProdutoMateriais_ProdutoId" ON "ProdutoMateriais" ("ProdutoId");

CREATE TABLE "ef_temp_Encomendas" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Encomendas" PRIMARY KEY AUTOINCREMENT,
    "Cliente" TEXT NULL,
    "Data" TEXT NOT NULL,
    "DataFinalizacao" TEXT NULL,
    "Descricao" TEXT NOT NULL,
    "Observacao" TEXT NULL,
    "Status" INTEGER NOT NULL,
    "ValorOrcado" TEXT NOT NULL
);

INSERT INTO "ef_temp_Encomendas" ("Id", "Cliente", "Data", "DataFinalizacao", "Descricao", "Observacao", "Status", "ValorOrcado")
SELECT "Id", "Cliente", "Data", "DataFinalizacao", "Descricao", "Observacao", "Status", "ValorOrcado"
FROM "Encomendas";

CREATE TABLE "ef_temp_MovimentacoesEstoque" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_MovimentacoesEstoque" PRIMARY KEY AUTOINCREMENT,
    "Data" TEXT NOT NULL,
    "MaterialId" INTEGER NOT NULL,
    "Observacao" TEXT NULL,
    "Quantidade" INTEGER NOT NULL,
    "Tipo" INTEGER NOT NULL,
    CONSTRAINT "FK_MovimentacoesEstoque_Materiais_MaterialId" FOREIGN KEY ("MaterialId") REFERENCES "Materiais" ("Id") ON DELETE CASCADE
);

INSERT INTO "ef_temp_MovimentacoesEstoque" ("Id", "Data", "MaterialId", "Observacao", "Quantidade", "Tipo")
SELECT "Id", "Data", "MaterialId", "Observacao", "Quantidade", "Tipo"
FROM "MovimentacoesEstoque";

CREATE TABLE "ef_temp_Vendas" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Vendas" PRIMARY KEY AUTOINCREMENT,
    "Cliente" TEXT NULL,
    "Data" TEXT NOT NULL,
    "Observacao" TEXT NULL,
    "ProdutoId" INTEGER NOT NULL,
    "Quantidade" INTEGER NOT NULL,
    "ValorVenda" TEXT NOT NULL,
    CONSTRAINT "FK_Vendas_Produtos_ProdutoId" FOREIGN KEY ("ProdutoId") REFERENCES "Produtos" ("Id") ON DELETE CASCADE
);

INSERT INTO "ef_temp_Vendas" ("Id", "Cliente", "Data", "Observacao", "ProdutoId", "Quantidade", "ValorVenda")
SELECT "Id", "Cliente", "Data", "Observacao", "ProdutoId", "Quantidade", "ValorVenda"
FROM "Vendas";

CREATE TABLE "ef_temp_Produtos" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Produtos" PRIMARY KEY AUTOINCREMENT,
    "DataCriacao" TEXT NOT NULL,
    "DataVenda" TEXT NULL,
    "Descricao" TEXT NULL,
    "EncomendaId" INTEGER NULL,
    "Nome" TEXT NOT NULL,
    "Quantidade" INTEGER NOT NULL,
    "Valor" TEXT NOT NULL,
    "Vendido" INTEGER NOT NULL,
    CONSTRAINT "FK_Produtos_Encomendas_EncomendaId" FOREIGN KEY ("EncomendaId") REFERENCES "Encomendas" ("Id") ON DELETE SET NULL
);

INSERT INTO "ef_temp_Produtos" ("Id", "DataCriacao", "DataVenda", "Descricao", "EncomendaId", "Nome", "Quantidade", "Valor", "Vendido")
SELECT "Id", "DataCriacao", "DataVenda", "Descricao", "EncomendaId", "Nome", "Quantidade", "Valor", "Vendido"
FROM "Produtos";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;

DROP TABLE "Encomendas";

ALTER TABLE "ef_temp_Encomendas" RENAME TO "Encomendas";

DROP TABLE "MovimentacoesEstoque";

ALTER TABLE "ef_temp_MovimentacoesEstoque" RENAME TO "MovimentacoesEstoque";

DROP TABLE "Vendas";

ALTER TABLE "ef_temp_Vendas" RENAME TO "Vendas";

DROP TABLE "Produtos";

ALTER TABLE "ef_temp_Produtos" RENAME TO "Produtos";

COMMIT;

PRAGMA foreign_keys = 1;

BEGIN TRANSACTION;

CREATE INDEX "IX_MovimentacoesEstoque_MaterialId" ON "MovimentacoesEstoque" ("MaterialId");

CREATE INDEX "IX_Vendas_ProdutoId" ON "Vendas" ("ProdutoId");

CREATE INDEX "IX_Produtos_EncomendaId" ON "Produtos" ("EncomendaId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260121145838_RestruturacaoProduto', '8.0.0');

COMMIT;

BEGIN TRANSACTION;

DROP TABLE "ProdutoMateriais";

DROP TABLE "Produtos";

ALTER TABLE "Vendas" RENAME COLUMN "ProdutoId" TO "PecaProntaId";

DROP INDEX "IX_Vendas_ProdutoId";

CREATE INDEX "IX_Vendas_PecaProntaId" ON "Vendas" ("PecaProntaId");

CREATE TABLE "PecasProntas" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_PecasProntas" PRIMARY KEY AUTOINCREMENT,
    "Titulo" TEXT NOT NULL,
    "Descricao" TEXT NULL,
    "Valor" TEXT NOT NULL,
    "FotoUrl" TEXT NULL,
    "Tipo" INTEGER NOT NULL,
    "Vendida" INTEGER NOT NULL,
    "DataCriacao" TEXT NOT NULL
);

CREATE TABLE "PecaProntaMateriais" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_PecaProntaMateriais" PRIMARY KEY AUTOINCREMENT,
    "PecaProntaId" INTEGER NOT NULL,
    "MaterialId" INTEGER NOT NULL,
    "QuantidadeUsada" INTEGER NOT NULL,
    CONSTRAINT "FK_PecaProntaMateriais_Materiais_MaterialId" FOREIGN KEY ("MaterialId") REFERENCES "Materiais" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_PecaProntaMateriais_PecasProntas_PecaProntaId" FOREIGN KEY ("PecaProntaId") REFERENCES "PecasProntas" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_PecaProntaMateriais_MaterialId" ON "PecaProntaMateriais" ("MaterialId");

CREATE INDEX "IX_PecaProntaMateriais_PecaProntaId" ON "PecaProntaMateriais" ("PecaProntaId");

CREATE TABLE "ef_temp_Vendas" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Vendas" PRIMARY KEY AUTOINCREMENT,
    "Cliente" TEXT NULL,
    "Data" TEXT NOT NULL,
    "Observacao" TEXT NULL,
    "PecaProntaId" INTEGER NOT NULL,
    "Quantidade" INTEGER NOT NULL,
    "ValorVenda" TEXT NOT NULL,
    CONSTRAINT "FK_Vendas_PecasProntas_PecaProntaId" FOREIGN KEY ("PecaProntaId") REFERENCES "PecasProntas" ("Id") ON DELETE CASCADE
);

INSERT INTO "ef_temp_Vendas" ("Id", "Cliente", "Data", "Observacao", "PecaProntaId", "Quantidade", "ValorVenda")
SELECT "Id", "Cliente", "Data", "Observacao", "PecaProntaId", "Quantidade", "ValorVenda"
FROM "Vendas";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;

DROP TABLE "Vendas";

ALTER TABLE "ef_temp_Vendas" RENAME TO "Vendas";

COMMIT;

PRAGMA foreign_keys = 1;

BEGIN TRANSACTION;

CREATE INDEX "IX_Vendas_PecaProntaId" ON "Vendas" ("PecaProntaId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260122145935_RefactorToPecasProntas', '8.0.0');

COMMIT;

BEGIN TRANSACTION;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260209181735_EnumToString', '8.0.0');

COMMIT;

