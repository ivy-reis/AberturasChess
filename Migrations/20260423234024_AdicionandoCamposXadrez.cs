using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LivroAberturasAPI.Migrations
{
    /// <inheritdoc />
    public partial class AdicionandoCamposXadrez : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Adversario",
                table: "Partidas");

            migrationBuilder.DropColumn(
                name: "DataDaPartida",
                table: "Partidas");

            migrationBuilder.RenameColumn(
                name: "LancesChave",
                table: "Variantes",
                newName: "Lances");

            migrationBuilder.RenameColumn(
                name: "ErrosGraves",
                table: "Precisoes",
                newName: "Capivara");

            migrationBuilder.RenameColumn(
                name: "CorDasPecas",
                table: "Partidas",
                newName: "LinkPartida");

            migrationBuilder.RenameColumn(
                name: "FamiliaDeLances",
                table: "Aberturas",
                newName: "Cor");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Lances",
                table: "Variantes",
                newName: "LancesChave");

            migrationBuilder.RenameColumn(
                name: "Capivara",
                table: "Precisoes",
                newName: "ErrosGraves");

            migrationBuilder.RenameColumn(
                name: "LinkPartida",
                table: "Partidas",
                newName: "CorDasPecas");

            migrationBuilder.RenameColumn(
                name: "Cor",
                table: "Aberturas",
                newName: "FamiliaDeLances");

            migrationBuilder.AddColumn<string>(
                name: "Adversario",
                table: "Partidas",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataDaPartida",
                table: "Partidas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
