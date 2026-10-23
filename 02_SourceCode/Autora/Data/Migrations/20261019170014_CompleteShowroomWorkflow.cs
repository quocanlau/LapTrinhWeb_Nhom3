using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Autora.Data.Migrations
{
                      
    public partial class CompleteShowroomWorkflow : Migration
    {
                          
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DepositRequests_CustomerId_CarId",
                table: "DepositRequests");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Request_Status",
                table: "DepositRequests");

            migrationBuilder.CreateTable(
                name: "ContactInquiries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    InterestedModel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactInquiries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShowroomSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Hotline = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    OpeningHours = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Policy = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShowroomSettings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DepositRequests_CustomerId_CarId",
                table: "DepositRequests",
                columns: new[] { "CustomerId", "CarId" },
                unique: true,
                filter: "[Status] IN (0, 3)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Request_Status",
                table: "DepositRequests",
                sql: "[Status] BETWEEN 0 AND 3");
        }

                          
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContactInquiries");

            migrationBuilder.DropTable(
                name: "ShowroomSettings");

            migrationBuilder.DropIndex(
                name: "IX_DepositRequests_CustomerId_CarId",
                table: "DepositRequests");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Request_Status",
                table: "DepositRequests");

            migrationBuilder.CreateIndex(
                name: "IX_DepositRequests_CustomerId_CarId",
                table: "DepositRequests",
                columns: new[] { "CustomerId", "CarId" },
                unique: true,
                filter: "[Status] = 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Request_Status",
                table: "DepositRequests",
                sql: "[Status] BETWEEN 0 AND 2");
        }
    }
}
