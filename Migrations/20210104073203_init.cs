using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H4WebApi.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Driver",
                columns: table => new
                {
                    PK = table.Column<Guid>(nullable: false),
                    Company = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    CreateUser = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Driver", x => x.PK);
                });

            migrationBuilder.CreateTable(
                name: "OilStation",
                columns: table => new
                {
                    PK = table.Column<Guid>(nullable: false),
                    Branch = table.Column<string>(nullable: true),
                    District = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    PLCIP = table.Column<string>(nullable: true),
                    HMIIP = table.Column<string>(nullable: true),
                    VolumnPer1cm = table.Column<float>(nullable: false),
                    LevelCalcFactor = table.Column<float>(nullable: false),
                    LevelCalcOffset = table.Column<float>(nullable: false),
                    PumpRatedFlow = table.Column<float>(nullable: false),
                    PumpCalcFactor = table.Column<float>(nullable: false),
                    PumpCalcOffset = table.Column<float>(nullable: false),
                    Longitude = table.Column<string>(nullable: true),
                    Latitude = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    CreateUser = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OilStation", x => x.PK);
                });

            migrationBuilder.CreateTable(
                name: "Truck",
                columns: table => new
                {
                    PK = table.Column<Guid>(nullable: false),
                    Company = table.Column<string>(nullable: true),
                    Number = table.Column<string>(nullable: true),
                    Volumn = table.Column<float>(nullable: false),
                    LeadSealNumber = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    CreateUser = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Truck", x => x.PK);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    Branch = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    Avatar = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Mobile = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    CurrentAuthority = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    CreateUser = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "WorkTicket",
                columns: table => new
                {
                    PK = table.Column<Guid>(nullable: false),
                    LoadStation = table.Column<string>(nullable: true),
                    OilPot = table.Column<string>(nullable: true),
                    UnloadStation = table.Column<string>(nullable: true),
                    SerialNumber = table.Column<string>(nullable: true),
                    OilLoadedMax = table.Column<float>(nullable: false),
                    SubSerialNumber = table.Column<string>(nullable: true),
                    CarNumber = table.Column<string>(nullable: true),
                    LoadingBeginTime = table.Column<DateTime>(nullable: true),
                    LoadingEndTime = table.Column<DateTime>(nullable: true),
                    LoadingActualBeginTime = table.Column<DateTime>(nullable: true),
                    LoadingActualEndTime = table.Column<DateTime>(nullable: true),
                    OilLoaded = table.Column<float>(nullable: false),
                    Driver = table.Column<string>(nullable: true),
                    DriverPhone = table.Column<string>(nullable: true),
                    OilLoader = table.Column<string>(nullable: true),
                    UnloadingBeginTime = table.Column<DateTime>(nullable: true),
                    UnloadingEndTime = table.Column<DateTime>(nullable: true),
                    OilUnloaded = table.Column<float>(nullable: false),
                    OilError = table.Column<float>(nullable: false),
                    OilUnloader = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Reviewer = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    LevelBeginLoad = table.Column<float>(nullable: false),
                    LevelAfterLoad = table.Column<float>(nullable: false),
                    TaskDuration = table.Column<float>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    CreateUser = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkTicket", x => x.PK);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Driver");

            migrationBuilder.DropTable(
                name: "OilStation");

            migrationBuilder.DropTable(
                name: "Truck");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "WorkTicket");
        }
    }
}
