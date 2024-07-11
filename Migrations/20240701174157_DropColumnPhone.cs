using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class DropColumnPhone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "AspNetUsers");
            migrationBuilder.DropColumn(
               name: "PhoneNumberConfirmed",
               table: "AspNetUsers");
        }
        
        protected override void Down(MigrationBuilder migrationBuilder) 
        {
           
        }
    }
}
