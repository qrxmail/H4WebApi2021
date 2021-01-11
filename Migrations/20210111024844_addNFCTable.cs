using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H4WebApi.Migrations
{
    public partial class addNFCTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Device",
                columns: table => new
                {
                    GId = table.Column<Guid>(nullable: false),
                    DeviceNo = table.Column<string>(nullable: true),
                    Site = table.Column<string>(nullable: true),
                    Region = table.Column<string>(nullable: true),
                    InspectNo = table.Column<string>(nullable: true),
                    DeviceType = table.Column<string>(nullable: true),
                    DeviceName = table.Column<string>(nullable: true),
                    Longitude = table.Column<string>(nullable: true),
                    Latitude = table.Column<string>(nullable: true),
                    BaiduLongitude = table.Column<string>(nullable: true),
                    BaiduLatitude = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    CreateUser = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Device", x => x.GId);
                });

            migrationBuilder.CreateTable(
                name: "DeviceInspectItem",
                columns: table => new
                {
                    GId = table.Column<Guid>(nullable: false),
                    DeviceNo = table.Column<string>(nullable: true),
                    InspectItemNo = table.Column<string>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    CreateUser = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceInspectItem", x => x.GId);
                });

            migrationBuilder.CreateTable(
                name: "Inspect",
                columns: table => new
                {
                    GId = table.Column<Guid>(nullable: false),
                    InspectNo = table.Column<string>(nullable: true),
                    InspectName = table.Column<string>(nullable: true),
                    InspectOrderNo = table.Column<int>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    CreateUser = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inspect", x => x.GId);
                });

            migrationBuilder.CreateTable(
                name: "InspectCycles",
                columns: table => new
                {
                    GId = table.Column<Guid>(nullable: false),
                    CycleName = table.Column<string>(nullable: true),
                    CycleType = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    CreateUser = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectCycles", x => x.GId);
                });

            migrationBuilder.CreateTable(
                name: "InspectData",
                columns: table => new
                {
                    GId = table.Column<Guid>(nullable: false),
                    TaskId = table.Column<Guid>(nullable: false),
                    TaskNo = table.Column<string>(nullable: true),
                    InspectNo = table.Column<string>(nullable: true),
                    DeviceNo = table.Column<string>(nullable: true),
                    InspectItemNo = table.Column<string>(nullable: true),
                    ResultValue = table.Column<double>(nullable: false),
                    IsJumpInspect = table.Column<string>(nullable: true),
                    JumpInspectReason = table.Column<string>(nullable: true),
                    InspectTime = table.Column<DateTime>(nullable: false),
                    InspectUser = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    CreateUser = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectData", x => x.GId);
                });

            migrationBuilder.CreateTable(
                name: "InspectItems",
                columns: table => new
                {
                    GId = table.Column<Guid>(nullable: false),
                    InspectItemNo = table.Column<string>(nullable: true),
                    InspectItemName = table.Column<string>(nullable: true),
                    ValueType = table.Column<string>(nullable: true),
                    Unit = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    CreateUser = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectItems", x => x.GId);
                });

            migrationBuilder.CreateTable(
                name: "InspectLine",
                columns: table => new
                {
                    GId = table.Column<Guid>(nullable: false),
                    LineName = table.Column<string>(nullable: true),
                    DeviceInspectItems = table.Column<string>(nullable: true),
                    DeviceInspectItemsName = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    CreateUser = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectLine", x => x.GId);
                });

            migrationBuilder.CreateTable(
                name: "InspectTask",
                columns: table => new
                {
                    GId = table.Column<Guid>(nullable: false),
                    InspectNo = table.Column<string>(nullable: true),
                    DeviceNo = table.Column<string>(nullable: true),
                    InspectItemNo = table.Column<string>(nullable: true),
                    TaskOrderNo = table.Column<int>(nullable: false),
                    TaskNo = table.Column<string>(nullable: true),
                    TaskName = table.Column<string>(nullable: true),
                    InspectCycles = table.Column<string>(nullable: true),
                    CycleStartTime = table.Column<DateTime>(nullable: false),
                    CycleEndTime = table.Column<DateTime>(nullable: false),
                    LineName = table.Column<string>(nullable: true),
                    InspectTime = table.Column<DateTime>(nullable: false),
                    InspectUser = table.Column<string>(nullable: true),
                    IsComplete = table.Column<string>(nullable: true),
                    InspectCompleteTime = table.Column<DateTime>(nullable: false),
                    InspectCompleteUser = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    CreateUser = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectTask", x => x.GId);
                });

            migrationBuilder.CreateTable(
                name: "NFCCard",
                columns: table => new
                {
                    GId = table.Column<Guid>(nullable: false),
                    NFCCardNo = table.Column<string>(nullable: true),
                    PrintNo = table.Column<string>(nullable: true),
                    DeviceNo = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    CreateUser = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: true),
                    LastInspectTime = table.Column<DateTime>(nullable: false),
                    LastInspectUser = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NFCCard", x => x.GId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Device");

            migrationBuilder.DropTable(
                name: "DeviceInspectItem");

            migrationBuilder.DropTable(
                name: "Inspect");

            migrationBuilder.DropTable(
                name: "InspectCycles");

            migrationBuilder.DropTable(
                name: "InspectData");

            migrationBuilder.DropTable(
                name: "InspectItems");

            migrationBuilder.DropTable(
                name: "InspectLine");

            migrationBuilder.DropTable(
                name: "InspectTask");

            migrationBuilder.DropTable(
                name: "NFCCard");
        }
    }
}
