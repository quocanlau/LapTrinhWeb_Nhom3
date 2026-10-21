using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
namespace Autora.Data.Migrations;
[DbContext(typeof(AppDbContext))]
[Migration("20261016135000_AddAdminNotificationReads")]
public partial class AddAdminNotificationReads : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(name: "AdminNotificationReads", columns: table => new
        {
            Id = table.Column<long>(type: "bigint", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
            UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
            EventKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            ReadAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
        }, constraints: table =>
        {
            table.PrimaryKey("PK_AdminNotificationReads", x => x.Id);
            table.ForeignKey(name: "FK_AdminNotificationReads_AspNetUsers_UserId", column: x => x.UserId, principalTable: "AspNetUsers", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
        });
        migrationBuilder.CreateIndex(name: "IX_AdminNotificationReads_UserId_EventKey", table: "AdminNotificationReads", columns: new[] { "UserId", "EventKey" }, unique: true);
    }
    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable(name: "AdminNotificationReads");
}
