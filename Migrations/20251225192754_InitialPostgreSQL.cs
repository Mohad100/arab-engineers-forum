using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fourm.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgreSQL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PrivateMessages",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    SenderId = table.Column<string>(type: "text", nullable: false),
                    SenderUsername = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    RecipientId = table.Column<string>(type: "text", nullable: false),
                    RecipientUsername = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Subject = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeletedBySender = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeletedByRecipient = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivateMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Threads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    CategoryId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AuthorUsername = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastReplyAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReplyCount = table.Column<int>(type: "integer", nullable: false),
                    IsPinned = table.Column<bool>(type: "boolean", nullable: false),
                    IsLocked = table.Column<bool>(type: "boolean", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    IsViolation = table.Column<bool>(type: "boolean", nullable: false),
                    ViolationReason = table.Column<string>(type: "text", nullable: true),
                    ViolationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ViolatedByAdmin = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Threads", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsAdmin = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Replies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ThreadId = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    AuthorUsername = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsEdited = table.Column<bool>(type: "boolean", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AttachmentUrl = table.Column<string>(type: "text", nullable: true),
                    AttachmentFileName = table.Column<string>(type: "text", nullable: true),
                    IsViolation = table.Column<bool>(type: "boolean", nullable: false),
                    ViolationReason = table.Column<string>(type: "text", nullable: true),
                    ViolationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ViolatedByAdmin = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Replies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Replies_Threads_ThreadId",
                        column: x => x.ThreadId,
                        principalTable: "Threads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrivateMessages_RecipientUsername",
                table: "PrivateMessages",
                column: "RecipientUsername");

            migrationBuilder.CreateIndex(
                name: "IX_PrivateMessages_RecipientUsername_IsRead",
                table: "PrivateMessages",
                columns: new[] { "RecipientUsername", "IsRead" });

            migrationBuilder.CreateIndex(
                name: "IX_PrivateMessages_SenderUsername",
                table: "PrivateMessages",
                column: "SenderUsername");

            migrationBuilder.CreateIndex(
                name: "IX_Replies_ThreadId",
                table: "Replies",
                column: "ThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrivateMessages");

            migrationBuilder.DropTable(
                name: "Replies");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Threads");
        }
    }
}
