using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EHM_Survey_App.Migrations
{
    public partial class AddSurveyFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Stores tablosu oluşturuluyor
            migrationBuilder.CreateTable(
                name: "Stores",
                columns: table => new
                {
                    StoreId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StoreName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stores", x => x.StoreId);
                });

            // SurveyResponses tablosu oluşturuluyor
            migrationBuilder.CreateTable(
                name: "SurveyResponses",
                columns: table => new
                {
                    ResponseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SurveyId = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    Response = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    VerificationCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubmissonDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Abandoned = table.Column<bool>(type: "bit", nullable: false),
                    NpsScore = table.Column<int>(type: "int", nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompletionTime = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyResponses", x => x.ResponseId);
                    table.ForeignKey(
                        name: "FK_SurveyResponses_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Cascade);
                });

            // Surveys tablosu oluşturuluyor
            migrationBuilder.CreateTable(
                name: "Surveys",
                columns: table => new
                {
                    SurveyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuestionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuestionOptions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false, defaultValue: 0), // Yeni eklenen kolon
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true) // Yeni eklenen kolon
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Surveys", x => x.SurveyId);
                    table.ForeignKey(
                        name: "FK_Surveys_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Cascade);
                });

            // Indeksler oluşturuluyor
            migrationBuilder.CreateIndex(
                name: "IX_SurveyResponses_StoreId",
                table: "SurveyResponses",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Surveys_StoreId",
                table: "Surveys",
                column: "StoreId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Tablo ve kolonlar geri alınırken siliniyor
            migrationBuilder.DropTable(
                name: "SurveyResponses");

            migrationBuilder.DropTable(
                name: "Surveys");

            migrationBuilder.DropTable(
                name: "Stores");
        }
    }
}
