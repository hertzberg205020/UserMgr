using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserMgr.Infrastructure.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_user",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhoneNumber_RegionNumber = table.Column<string>(type: "varchar(5)", unicode: false, maxLength: 5, nullable: false),
                    PhoneNumber_Number = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    _passwordHash = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_user", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "t_userLoginHistory",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PhoneNumber_RegionNumber = table.Column<string>(type: "varchar(5)", unicode: false, maxLength: 5, nullable: false),
                    PhoneNumber_Number = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_userLoginHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "t_userAccessFail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LockOutEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false),
                    _isLockedOut = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_userAccessFail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_t_userAccessFail_t_user_UserId",
                        column: x => x.UserId,
                        principalTable: "t_user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_t_userAccessFail_UserId",
                table: "t_userAccessFail",
                column: "UserId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_userAccessFail");

            migrationBuilder.DropTable(
                name: "t_userLoginHistory");

            migrationBuilder.DropTable(
                name: "t_user");
        }
    }
}
