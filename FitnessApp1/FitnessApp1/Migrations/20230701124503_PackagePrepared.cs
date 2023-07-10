using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessApp1.Migrations
{
    public partial class PackagePrepared : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PackageTrainer_Packages_PackageId",
                table: "PackageTrainer");

            migrationBuilder.DropForeignKey(
                name: "FK_PackageTrainer_Trainers_TrainerId",
                table: "PackageTrainer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PackageTrainer",
                table: "PackageTrainer");

            migrationBuilder.RenameTable(
                name: "PackageTrainer",
                newName: "PackageTrainers");

            migrationBuilder.RenameIndex(
                name: "IX_PackageTrainer_TrainerId",
                table: "PackageTrainers",
                newName: "IX_PackageTrainers_TrainerId");

            migrationBuilder.RenameIndex(
                name: "IX_PackageTrainer_PackageId",
                table: "PackageTrainers",
                newName: "IX_PackageTrainers_PackageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PackageTrainers",
                table: "PackageTrainers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PackageTrainers_Packages_PackageId",
                table: "PackageTrainers",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PackageTrainers_Trainers_TrainerId",
                table: "PackageTrainers",
                column: "TrainerId",
                principalTable: "Trainers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PackageTrainers_Packages_PackageId",
                table: "PackageTrainers");

            migrationBuilder.DropForeignKey(
                name: "FK_PackageTrainers_Trainers_TrainerId",
                table: "PackageTrainers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PackageTrainers",
                table: "PackageTrainers");

            migrationBuilder.RenameTable(
                name: "PackageTrainers",
                newName: "PackageTrainer");

            migrationBuilder.RenameIndex(
                name: "IX_PackageTrainers_TrainerId",
                table: "PackageTrainer",
                newName: "IX_PackageTrainer_TrainerId");

            migrationBuilder.RenameIndex(
                name: "IX_PackageTrainers_PackageId",
                table: "PackageTrainer",
                newName: "IX_PackageTrainer_PackageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PackageTrainer",
                table: "PackageTrainer",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PackageTrainer_Packages_PackageId",
                table: "PackageTrainer",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PackageTrainer_Trainers_TrainerId",
                table: "PackageTrainer",
                column: "TrainerId",
                principalTable: "Trainers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
