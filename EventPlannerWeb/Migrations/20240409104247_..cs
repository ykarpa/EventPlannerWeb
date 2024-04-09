using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventPlannerWeb.Migrations
{
    /// <inheritdoc />
    public partial class _ : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_User_UserId",
                table: "Event");

            migrationBuilder.DropForeignKey(
                name: "FK_EventGuest_Event_EventId",
                table: "EventGuest");

            migrationBuilder.DropForeignKey(
                name: "FK_EventGuest_Guest_GuestId",
                table: "EventGuest");

            migrationBuilder.DropForeignKey(
                name: "FK_EventRecipe_Event_EventId",
                table: "EventRecipe");

            migrationBuilder.DropForeignKey(
                name: "FK_EventRecipe_Recipe_RecipeId",
                table: "EventRecipe");

            migrationBuilder.DropForeignKey(
                name: "FK_Guest_User_UserId",
                table: "Guest");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_User_UserId",
                table: "Event",
                column: "UserId",
                principalTable: "User",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventGuest_Event_EventId",
                table: "EventGuest",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "event_id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventGuest_Guest_GuestId",
                table: "EventGuest",
                column: "GuestId",
                principalTable: "Guest",
                principalColumn: "guest_id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventRecipe_Event_EventId",
                table: "EventRecipe",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "event_id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventRecipe_Recipe_RecipeId",
                table: "EventRecipe",
                column: "RecipeId",
                principalTable: "Recipe",
                principalColumn: "recipe_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Guest_User_UserId",
                table: "Guest",
                column: "UserId",
                principalTable: "User",
                principalColumn: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_User_UserId",
                table: "Event");

            migrationBuilder.DropForeignKey(
                name: "FK_EventGuest_Event_EventId",
                table: "EventGuest");

            migrationBuilder.DropForeignKey(
                name: "FK_EventGuest_Guest_GuestId",
                table: "EventGuest");

            migrationBuilder.DropForeignKey(
                name: "FK_EventRecipe_Event_EventId",
                table: "EventRecipe");

            migrationBuilder.DropForeignKey(
                name: "FK_EventRecipe_Recipe_RecipeId",
                table: "EventRecipe");

            migrationBuilder.DropForeignKey(
                name: "FK_Guest_User_UserId",
                table: "Guest");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_User_UserId",
                table: "Event",
                column: "UserId",
                principalTable: "User",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventGuest_Event_EventId",
                table: "EventGuest",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "event_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventGuest_Guest_GuestId",
                table: "EventGuest",
                column: "GuestId",
                principalTable: "Guest",
                principalColumn: "guest_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventRecipe_Event_EventId",
                table: "EventRecipe",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "event_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventRecipe_Recipe_RecipeId",
                table: "EventRecipe",
                column: "RecipeId",
                principalTable: "Recipe",
                principalColumn: "recipe_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Guest_User_UserId",
                table: "Guest",
                column: "UserId",
                principalTable: "User",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
