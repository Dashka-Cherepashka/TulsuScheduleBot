﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleBot.Migrations {
    /// <inheritdoc />
    public partial class addDispatch_Mode : Migration {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.InsertData(
                table: "Modes",
                columns: new[] { "ID", "Name" },
                values: new object[] { (byte)19, "Dispatch" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DeleteData(
                table: "Modes",
                keyColumn: "ID",
                keyValue: (byte)19);
        }
    }
}