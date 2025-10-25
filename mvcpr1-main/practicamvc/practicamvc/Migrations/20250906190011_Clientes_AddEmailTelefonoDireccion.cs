using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace practicamvc.Migrations
{
    /// <inheritdoc />
    public partial class Clientes_AddEmailTelefonoDireccion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Correo",
                table: "Clientes",
                newName: "Email");

            migrationBuilder.AddColumn<string>(
                name: "Telefono",
                table: "Clientes",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Direccion",
                table: "Clientes",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Telefono", table: "Clientes");
            migrationBuilder.DropColumn(name: "Direccion", table: "Clientes");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Clientes",
                newName: "Correo");
        }


    }
}
