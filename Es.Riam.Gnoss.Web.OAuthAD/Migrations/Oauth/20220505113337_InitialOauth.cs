using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Es.Riam.Gnoss.Web.OAuthAD.Migrations.Oauth
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OAuthConsumer",
                columns: table => new
                {
                    ConsumerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConsumerKey = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ConsumerSecret = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Callback = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    VerificationCodeFormat = table.Column<int>(type: "int", nullable: false),
                    VerificationCodeLength = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.OAuthConsumer", x => x.ConsumerId);
                });

            //migrationBuilder.CreateTable(
            //    name: "Usuario",
            //    columns: table => new
            //    {
            //        UsuarioID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //        Login = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Usuario", x => x.UsuarioID);
            //    });

            migrationBuilder.CreateTable(
                name: "ConsumerData",
                columns: table => new
                {
                    ConsumerId = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UrlOrigen = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    FechaAlta = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsumerData", x => x.ConsumerId);
                    table.ForeignKey(
                        name: "FK_ConsumerData_OAuthConsumer",
                        column: x => x.ConsumerId,
                        principalTable: "OAuthConsumer",
                        principalColumn: "ConsumerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OAuthToken",
                columns: table => new
                {
                    TokenId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TokenSecret = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConsumerId = table.Column<int>(type: "int", nullable: false),
                    UsuarioID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Scope = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestTokenVerifier = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RequestTokenCallback = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ConsumerVersion = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.OAuthToken", x => x.TokenId);
                    table.ForeignKey(
                        name: "FK_OAuthToken_Usuario",
                        column: x => x.UsuarioID,
                        principalTable: "Usuario",
                        principalColumn: "UsuarioID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "OAuthConsumer_OAuthToken",
                        column: x => x.ConsumerId,
                        principalTable: "OAuthConsumer",
                        principalColumn: "ConsumerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioConsumer",
                columns: table => new
                {
                    ConsumerId = table.Column<int>(type: "int", nullable: false),
                    UsuarioID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario_Consumer", x => x.ConsumerId);
                    table.ForeignKey(
                        name: "FK_Usuario_Consumer_OAuthConsumer",
                        column: x => x.ConsumerId,
                        principalTable: "OAuthConsumer",
                        principalColumn: "ConsumerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Usuario_Consumer_Usuario",
                        column: x => x.UsuarioID,
                        principalTable: "Usuario",
                        principalColumn: "UsuarioID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OAuthTokenExterno",
                columns: table => new
                {
                    TokenId = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TokenSecret = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TokenVinculadoId = table.Column<int>(type: "int", nullable: true),
                    UsuarioID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OAuthTokenExterno", x => x.TokenId);
                    table.ForeignKey(
                        name: "FK_OAuthTokenExterno_OAuthToken",
                        column: x => x.TokenId,
                        principalTable: "OAuthToken",
                        principalColumn: "TokenId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OAuthTokenExterno_Usuario",
                        column: x => x.UsuarioID,
                        principalTable: "Usuario",
                        principalColumn: "UsuarioID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PinToken",
                columns: table => new
                {
                    TokenId = table.Column<int>(type: "int", nullable: false),
                    UsuarioID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Pin = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PinToken", x => x.TokenId);
                    table.ForeignKey(
                        name: "FK_PinToken_OAuthToken",
                        column: x => x.TokenId,
                        principalTable: "OAuthToken",
                        principalColumn: "TokenId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PinToken_Usuario",
                        column: x => x.UsuarioID,
                        principalTable: "Usuario",
                        principalColumn: "UsuarioID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OAuthToken_ConsumerId",
                table: "OAuthToken",
                column: "ConsumerId");

            migrationBuilder.CreateIndex(
                name: "IX_OAuthToken_UsuarioID",
                table: "OAuthToken",
                column: "UsuarioID");

            migrationBuilder.CreateIndex(
                name: "IX_OAuthTokenExterno_UsuarioID",
                table: "OAuthTokenExterno",
                column: "UsuarioID");

            migrationBuilder.CreateIndex(
                name: "IX_PinToken_UsuarioID",
                table: "PinToken",
                column: "UsuarioID");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioConsumer_UsuarioID",
                table: "UsuarioConsumer",
                column: "UsuarioID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConsumerData");

            migrationBuilder.DropTable(
                name: "OAuthTokenExterno");

            migrationBuilder.DropTable(
                name: "PinToken");

            migrationBuilder.DropTable(
                name: "UsuarioConsumer");

            migrationBuilder.DropTable(
                name: "OAuthToken");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "OAuthConsumer");
        }
    }
}
