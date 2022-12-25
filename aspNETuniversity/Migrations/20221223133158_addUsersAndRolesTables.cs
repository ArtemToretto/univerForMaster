using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace aspNETuniversity.Migrations
{
    public partial class addUsersAndRolesTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "facultys",
                columns: table => new
                {
                    faculty_code = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    dean_name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    faculty_name = table.Column<string>(type: "varchar(25)", unicode: false, maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_facultys", x => x.faculty_code);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "universityAudit",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    eventName = table.Column<string>(type: "varchar(1000)", unicode: false, maxLength: 1000, nullable: true),
                    EditBy = table.Column<string>(type: "varchar(1000)", unicode: false, maxLength: 1000, nullable: false, defaultValueSql: "(original_login())"),
                    eventDateTime = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "specializations",
                columns: table => new
                {
                    spec_code = table.Column<int>(type: "int", nullable: false),
                    cvalification = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    faculty_code = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_specializations", x => x.spec_code);
                    table.ForeignKey(
                        name: "FK_specializations_facultys",
                        column: x => x.faculty_code,
                        principalTable: "facultys",
                        principalColumn: "faculty_code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Login = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "stud_groups",
                columns: table => new
                {
                    stud_group_code = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    year = table.Column<short>(type: "smallint", nullable: false),
                    specialization_code = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stud_groups", x => x.stud_group_code);
                    table.ForeignKey(
                        name: "FK_stud_groups_specializations",
                        column: x => x.specialization_code,
                        principalTable: "specializations",
                        principalColumn: "spec_code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "students",
                columns: table => new
                {
                    zachetka = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    salary_father = table.Column<int>(type: "int", nullable: true, defaultValueSql: "((13000))"),
                    salary_mother = table.Column<int>(type: "int", nullable: true, defaultValueSql: "((13000))"),
                    family_kol = table.Column<byte>(type: "tinyint", nullable: true),
                    stud_group_code = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_students", x => x.zachetka);
                    table.ForeignKey(
                        name: "FK_students_stud_groups",
                        column: x => x.stud_group_code,
                        principalTable: "stud_groups",
                        principalColumn: "stud_group_code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "indx_name",
                table: "facultys",
                column: "faculty_name");

            migrationBuilder.CreateIndex(
                name: "UC_faculty_name",
                table: "facultys",
                column: "faculty_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "indx_faculty_code",
                table: "specializations",
                column: "faculty_code");

            migrationBuilder.CreateIndex(
                name: "UC_spec_name",
                table: "specializations",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "indx_spec",
                table: "stud_groups",
                column: "specialization_code");

            migrationBuilder.CreateIndex(
                name: "indx_group",
                table: "students",
                column: "stud_group_code");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "students");

            migrationBuilder.DropTable(
                name: "universityAudit");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "stud_groups");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "specializations");

            migrationBuilder.DropTable(
                name: "facultys");
        }
    }
}
