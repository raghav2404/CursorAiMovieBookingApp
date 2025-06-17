using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieBooking.API.Data.Migrations
{
    public partial class SeedInitialData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add Movies
            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "Id", "Title", "Description", "Duration", "Genre", "Language", "ReleaseDate", "IsActive" },
                values: new object[,]
                {
                    { 
                        "1", 
                        "Inception", 
                        "A thief who steals corporate secrets through dream-sharing technology is given the inverse task of planting an idea into the mind of a C.E.O.", 
                        148, 
                        "Sci-Fi", 
                        "English", 
                        "2010-07-16", 
                        true 
                    },
                    { 
                        "2", 
                        "The Dark Knight", 
                        "When the menace known as the Joker wreaks havoc and chaos on the people of Gotham, Batman must accept one of the greatest psychological and physical tests of his ability to fight injustice.", 
                        152, 
                        "Action", 
                        "English", 
                        "2008-07-18", 
                        true 
                    }
                });

            // Add Theaters
            migrationBuilder.InsertData(
                table: "Theaters",
                columns: new[] { "Id", "Name", "Location", "IsActive" },
                values: new object[,]
                {
                    { "1", "Cineplex Downtown", "123 Main St, Downtown", true },
                    { "2", "MovieMax Central", "456 Park Ave, Central", true }
                });

            // Add Screens
            migrationBuilder.InsertData(
                table: "Screens",
                columns: new[] { "Id", "Name", "TheaterId", "Capacity", "IsActive" },
                values: new object[,]
                {
                    { "1", "Screen 1", "1", 100, true },
                    { "2", "Screen 2", "1", 80, true },
                    { "3", "Screen 1", "2", 120, true }
                });

            // Add Seats for Screen 1
            for (int row = 1; row <= 10; row++)
            {
                for (int number = 1; number <= 10; number++)
                {
                    migrationBuilder.InsertData(
                        table: "Seats",
                        columns: new[] { "Id", "ScreenId", "Row", "Number", "Type", "PriceMultiplier", "IsActive" },
                        values: new object[]
                        {
                            $"S1-{row}-{number}",
                            "1",
                            row,
                            number,
                            row <= 2 ? "Premium" : "Standard",
                            row <= 2 ? 1.5m : 1.0m,
                            true
                        });
                }
            }

            // Add ShowTimes
            migrationBuilder.InsertData(
                table: "ShowTimes",
                columns: new[] { "Id", "MovieId", "TheaterId", "ScreenId", "StartTime", "BasePrice", "IsActive" },
                values: new object[,]
                {
                    { 
                        "1", 
                        "1", 
                        "1", 
                        "1", 
                        DateTime.Now.Date.AddDays(1).AddHours(18), 
                        15.99m, 
                        true 
                    },
                    { 
                        "2", 
                        "2", 
                        "1", 
                        "2", 
                        DateTime.Now.Date.AddDays(1).AddHours(20), 
                        15.99m, 
                        true 
                    }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(table: "ShowTimes", keyColumn: "Id", keyValue: "1");
            migrationBuilder.DeleteData(table: "ShowTimes", keyColumn: "Id", keyValue: "2");
            
            // Delete seats for Screen 1
            for (int row = 1; row <= 10; row++)
            {
                for (int number = 1; number <= 10; number++)
                {
                    migrationBuilder.DeleteData(
                        table: "Seats",
                        keyColumn: "Id",
                        keyValue: $"S1-{row}-{number}");
                }
            }

            migrationBuilder.DeleteData(table: "Screens", keyColumn: "Id", keyValue: "1");
            migrationBuilder.DeleteData(table: "Screens", keyColumn: "Id", keyValue: "2");
            migrationBuilder.DeleteData(table: "Screens", keyColumn: "Id", keyValue: "3");
            
            migrationBuilder.DeleteData(table: "Theaters", keyColumn: "Id", keyValue: "1");
            migrationBuilder.DeleteData(table: "Theaters", keyColumn: "Id", keyValue: "2");
            
            migrationBuilder.DeleteData(table: "Movies", keyColumn: "Id", keyValue: "1");
            migrationBuilder.DeleteData(table: "Movies", keyColumn: "Id", keyValue: "2");
        }
    }
} 