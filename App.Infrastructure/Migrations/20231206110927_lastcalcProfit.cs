using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Infrastructure.Migrations
{
    public partial class lastcalcProfit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "lastTimeProfitCalced",
                table: "InvoiceMaster",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "lastTimeProfitCalced",
                table: "GLJournalEntry",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "lastTimeProfitCalced",
                table: "InvoiceMaster");

            migrationBuilder.DropColumn(
                name: "lastTimeProfitCalced",
                table: "GLJournalEntry");
        }
    }
}
