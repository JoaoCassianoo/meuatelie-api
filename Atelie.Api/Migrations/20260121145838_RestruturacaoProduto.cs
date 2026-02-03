using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atelie.Api.Migrations
{
    /// <inheritdoc />
    public partial class RestruturacaoProduto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Encomendas_Materiais_MaterialId",
                table: "Encomendas");

            migrationBuilder.DropForeignKey(
                name: "FK_MovimentacoesEstoque_Produtos_ProdutoId",
                table: "MovimentacoesEstoque");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_Materiais_MaterialId",
                table: "Vendas");

            migrationBuilder.DropIndex(
                name: "IX_MovimentacoesEstoque_ProdutoId",
                table: "MovimentacoesEstoque");

            migrationBuilder.DropIndex(
                name: "IX_Encomendas_MaterialId",
                table: "Encomendas");

            migrationBuilder.DropColumn(
                name: "ProdutoId",
                table: "MovimentacoesEstoque");

            migrationBuilder.DropColumn(
                name: "MaterialId",
                table: "Encomendas");

            migrationBuilder.RenameColumn(
                name: "MaterialId",
                table: "Vendas",
                newName: "ProdutoId");

            migrationBuilder.RenameIndex(
                name: "IX_Vendas_MaterialId",
                table: "Vendas",
                newName: "IX_Vendas_ProdutoId");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataVenda",
                table: "Produtos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EncomendaId",
                table: "Produtos",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Quantidade",
                table: "Produtos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Vendido",
                table: "Produtos",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ProdutoMateriais",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProdutoId = table.Column<int>(type: "INTEGER", nullable: false),
                    MaterialId = table.Column<int>(type: "INTEGER", nullable: false),
                    QuantidadeUsada = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProdutoMateriais", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProdutoMateriais_Materiais_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materiais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProdutoMateriais_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_EncomendaId",
                table: "Produtos",
                column: "EncomendaId");

            migrationBuilder.CreateIndex(
                name: "IX_ProdutoMateriais_MaterialId",
                table: "ProdutoMateriais",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_ProdutoMateriais_ProdutoId",
                table: "ProdutoMateriais",
                column: "ProdutoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_Encomendas_EncomendaId",
                table: "Produtos",
                column: "EncomendaId",
                principalTable: "Encomendas",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Vendas_Produtos_ProdutoId",
                table: "Vendas",
                column: "ProdutoId",
                principalTable: "Produtos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_Encomendas_EncomendaId",
                table: "Produtos");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_Produtos_ProdutoId",
                table: "Vendas");

            migrationBuilder.DropTable(
                name: "ProdutoMateriais");

            migrationBuilder.DropIndex(
                name: "IX_Produtos_EncomendaId",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "DataVenda",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "EncomendaId",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "Quantidade",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "Vendido",
                table: "Produtos");

            migrationBuilder.RenameColumn(
                name: "ProdutoId",
                table: "Vendas",
                newName: "MaterialId");

            migrationBuilder.RenameIndex(
                name: "IX_Vendas_ProdutoId",
                table: "Vendas",
                newName: "IX_Vendas_MaterialId");

            migrationBuilder.AddColumn<int>(
                name: "ProdutoId",
                table: "MovimentacoesEstoque",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaterialId",
                table: "Encomendas",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MovimentacoesEstoque_ProdutoId",
                table: "MovimentacoesEstoque",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_Encomendas_MaterialId",
                table: "Encomendas",
                column: "MaterialId");

            migrationBuilder.AddForeignKey(
                name: "FK_Encomendas_Materiais_MaterialId",
                table: "Encomendas",
                column: "MaterialId",
                principalTable: "Materiais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MovimentacoesEstoque_Produtos_ProdutoId",
                table: "MovimentacoesEstoque",
                column: "ProdutoId",
                principalTable: "Produtos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendas_Materiais_MaterialId",
                table: "Vendas",
                column: "MaterialId",
                principalTable: "Materiais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
