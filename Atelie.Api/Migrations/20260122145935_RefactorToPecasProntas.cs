using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atelie.Api.Migrations
{
    /// <inheritdoc />
    public partial class RefactorToPecasProntas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_Produtos_ProdutoId",
                table: "Vendas");

            migrationBuilder.DropTable(
                name: "ProdutoMateriais");

            migrationBuilder.DropTable(
                name: "Produtos");

            migrationBuilder.RenameColumn(
                name: "ProdutoId",
                table: "Vendas",
                newName: "PecaProntaId");

            migrationBuilder.RenameIndex(
                name: "IX_Vendas_ProdutoId",
                table: "Vendas",
                newName: "IX_Vendas_PecaProntaId");

            migrationBuilder.CreateTable(
                name: "PecasProntas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Titulo = table.Column<string>(type: "TEXT", nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", nullable: true),
                    Valor = table.Column<decimal>(type: "TEXT", nullable: false),
                    FotoUrl = table.Column<string>(type: "TEXT", nullable: true),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    Vendida = table.Column<bool>(type: "INTEGER", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PecasProntas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PecaProntaMateriais",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PecaProntaId = table.Column<int>(type: "INTEGER", nullable: false),
                    MaterialId = table.Column<int>(type: "INTEGER", nullable: false),
                    QuantidadeUsada = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PecaProntaMateriais", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PecaProntaMateriais_Materiais_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materiais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PecaProntaMateriais_PecasProntas_PecaProntaId",
                        column: x => x.PecaProntaId,
                        principalTable: "PecasProntas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PecaProntaMateriais_MaterialId",
                table: "PecaProntaMateriais",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_PecaProntaMateriais_PecaProntaId",
                table: "PecaProntaMateriais",
                column: "PecaProntaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendas_PecasProntas_PecaProntaId",
                table: "Vendas",
                column: "PecaProntaId",
                principalTable: "PecasProntas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_PecasProntas_PecaProntaId",
                table: "Vendas");

            migrationBuilder.DropTable(
                name: "PecaProntaMateriais");

            migrationBuilder.DropTable(
                name: "PecasProntas");

            migrationBuilder.RenameColumn(
                name: "PecaProntaId",
                table: "Vendas",
                newName: "ProdutoId");

            migrationBuilder.RenameIndex(
                name: "IX_Vendas_PecaProntaId",
                table: "Vendas",
                newName: "IX_Vendas_ProdutoId");

            migrationBuilder.CreateTable(
                name: "Produtos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EncomendaId = table.Column<int>(type: "INTEGER", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataVenda = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Descricao = table.Column<string>(type: "TEXT", nullable: true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    Quantidade = table.Column<int>(type: "INTEGER", nullable: false),
                    Valor = table.Column<decimal>(type: "TEXT", nullable: false),
                    Vendido = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produtos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Produtos_Encomendas_EncomendaId",
                        column: x => x.EncomendaId,
                        principalTable: "Encomendas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ProdutoMateriais",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MaterialId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProdutoId = table.Column<int>(type: "INTEGER", nullable: false),
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
                name: "IX_ProdutoMateriais_MaterialId",
                table: "ProdutoMateriais",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_ProdutoMateriais_ProdutoId",
                table: "ProdutoMateriais",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_EncomendaId",
                table: "Produtos",
                column: "EncomendaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendas_Produtos_ProdutoId",
                table: "Vendas",
                column: "ProdutoId",
                principalTable: "Produtos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
