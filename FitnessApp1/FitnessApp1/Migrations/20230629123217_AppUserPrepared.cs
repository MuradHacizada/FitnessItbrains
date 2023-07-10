using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessApp1.Migrations
{
    public partial class AppUserPrepared : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trainers_Positions_PositionId",
                table: "Trainers");

            migrationBuilder.AlterColumn<int>(
                name: "PositionId",
                table: "Trainers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Trainers_Positions_PositionId",
                table: "Trainers",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trainers_Positions_PositionId",
                table: "Trainers");

            migrationBuilder.AlterColumn<int>(
                name: "PositionId",
                table: "Trainers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Trainers_Positions_PositionId",
                table: "Trainers",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
