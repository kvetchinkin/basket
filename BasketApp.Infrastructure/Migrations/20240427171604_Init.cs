using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BasketApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "goods",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    price = table.Column<decimal>(type: "numeric", nullable: false),
                    weight_gram = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_goods", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "outbox",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    occuredOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProcessedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    error = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_outbox", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "time_slots",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    start = table.Column<int>(type: "integer", nullable: false),
                    end = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_time_slots", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "baskets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    address_country = table.Column<string>(type: "text", nullable: true),
                    address_city = table.Column<string>(type: "text", nullable: true),
                    address_street = table.Column<string>(type: "text", nullable: true),
                    address_house = table.Column<string>(type: "text", nullable: true),
                    address_apartment = table.Column<string>(type: "text", nullable: true),
                    timeslot_id = table.Column<int>(type: "integer", nullable: true),
                    status = table.Column<string>(type: "text", nullable: true),
                    Total = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_baskets", x => x.id);
                    table.ForeignKey(
                        name: "FK_baskets_time_slots_timeslot_id",
                        column: x => x.timeslot_id,
                        principalTable: "time_slots",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    good_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    price = table.Column<decimal>(type: "numeric", nullable: false),
                    basket_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_items_baskets_basket_id",
                        column: x => x.basket_id,
                        principalTable: "baskets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "goods",
                columns: new[] { "id", "description", "price", "title", "weight_gram" },
                values: new object[,]
                {
                    { new Guid("292dc3c5-2bdd-4e0c-bd75-c5e8b07a8792"), "Описание кофе", 500m, "Кофе", 7 },
                    { new Guid("34b1e64a-6471-44a0-8c4a-e5d21584a76c"), "Описание колбасы", 400m, "Колбаса", 4 },
                    { new Guid("a1d48be9-4c98-4371-97c0-064bde03874e"), "Описание яиц", 300m, "Яйца", 8 },
                    { new Guid("a3fcc8e1-d2a3-4bd6-9421-c82019e21c2d"), "Описание сахара", 600m, "Сахар", 1 },
                    { new Guid("e8cb8a0b-d302-485a-801c-5fb50aced4d5"), "Описание молока", 200m, "Молоко", 9 },
                    { new Guid("ec85ceee-f186-4e9c-a4dd-2929e69e586c"), "Описание хлеба", 100m, "Хлеб", 6 }
                });

            migrationBuilder.InsertData(
                table: "time_slots",
                columns: new[] { "id", "end", "name", "start" },
                values: new object[,]
                {
                    { 1, 12, "morning", 6 },
                    { 2, 17, "midday", 12 },
                    { 3, 24, "evening", 17 },
                    { 4, 6, "night", 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_baskets_timeslot_id",
                table: "baskets",
                column: "timeslot_id");

            migrationBuilder.CreateIndex(
                name: "IX_items_basket_id",
                table: "items",
                column: "basket_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "goods");

            migrationBuilder.DropTable(
                name: "items");

            migrationBuilder.DropTable(
                name: "outbox");

            migrationBuilder.DropTable(
                name: "baskets");

            migrationBuilder.DropTable(
                name: "time_slots");
        }
    }
}
