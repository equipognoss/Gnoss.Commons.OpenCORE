using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextOracleMigrations
{
	/// <inheritdoc />
	public partial class Flujos : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "Flujo",
				columns: table => new
				{
					FlujoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
					Nombre = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
					Descripcion = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
					OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
					ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
					Nota = table.Column<bool>(type: "NUMBER(1)", nullable: false),
					Adjunto = table.Column<bool>(type: "NUMBER(1)", nullable: false),
					Video = table.Column<bool>(type: "NUMBER(1)", nullable: false),
					Link = table.Column<bool>(type: "NUMBER(1)", nullable: false),
					Encuesta = table.Column<bool>(type: "NUMBER(1)", nullable: false),
					Debate = table.Column<bool>(type: "NUMBER(1)", nullable: false),
					PaginaCMS = table.Column<bool>(type: "NUMBER(1)", nullable: false),
					ComponenteCMS = table.Column<bool>(type: "NUMBER(1)", nullable: false),
					RecursoSemantico = table.Column<bool>(type: "NUMBER(1)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Flujo", x => x.FlujoID);
					table.ForeignKey(
						name: "FK_Flujo_Proyecto_OrganizacionID_ProyectoID",
						columns: x => new { x.OrganizacionID, x.ProyectoID },
						principalTable: "Proyecto",
						principalColumns: new[] { "OrganizacionID", "ProyectoID" },
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "Estado",
				columns: table => new
				{
					EstadoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
					FlujoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
					Nombre = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
					Publico = table.Column<bool>(type: "NUMBER(1)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Estado", x => x.EstadoID);
					table.ForeignKey(
						name: "FK_Estado_Flujo_FlujoID",
						column: x => x.FlujoID,
						principalTable: "Flujo",
						principalColumn: "FlujoID",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "FlujoObjetoConocimientoProyecto",
				columns: table => new
				{
					FlujoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
					Ontologia = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
					OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
					ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_FlujoObjetoConocimientoProyecto", x => new { x.FlujoID, x.Ontologia, x.OrganizacionID, x.ProyectoID });
					table.ForeignKey(
						name: "FK_FlujoObjetoConocimientoProyecto_Flujo_FlujoID",
						column: x => x.FlujoID,
						principalTable: "Flujo",
						principalColumn: "FlujoID",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_FlujoObjetoConocimientoProyecto_OntologiaProyecto_OrganizacionID_ProyectoID_Ontologia",
						columns: x => new { x.OrganizacionID, x.ProyectoID, x.Ontologia },
						principalTable: "OntologiaProyecto",
						principalColumns: new[] { "OrganizacionID", "ProyectoID", "OntologiaProyecto" },
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "EstadoGrupo",
				columns: table => new
				{
					EstadoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
					GrupoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
					Editor = table.Column<bool>(type: "NUMBER(1)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_EstadoGrupo", x => new { x.EstadoID, x.GrupoID });
					table.ForeignKey(
						name: "FK_EstadoGrupo_Estado_EstadoID",
						column: x => x.EstadoID,
						principalTable: "Estado",
						principalColumn: "EstadoID",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_EstadoGrupo_GrupoIdentidades_GrupoID",
						column: x => x.GrupoID,
						principalTable: "GrupoIdentidades",
						principalColumn: "GrupoID",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "EstadoIdentidad",
				columns: table => new
				{
					EstadoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
					IdentidadID = table.Column<Guid>(type: "RAW(16)", nullable: false),
					Editor = table.Column<bool>(type: "NUMBER(1)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_EstadoIdentidad", x => new { x.EstadoID, x.IdentidadID });
					table.ForeignKey(
						name: "FK_EstadoIdentidad_Estado_EstadoID",
						column: x => x.EstadoID,
						principalTable: "Estado",
						principalColumn: "EstadoID",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_EstadoIdentidad_Identidad_IdentidadID",
						column: x => x.IdentidadID,
						principalTable: "Identidad",
						principalColumn: "IdentidadID",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "Transicion",
				columns: table => new
				{
					TransicionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
					Nombre = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
					EstadoOrigenID = table.Column<Guid>(type: "RAW(16)", nullable: false),
					EstadoDestinoID = table.Column<Guid>(type: "RAW(16)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Transicion", x => x.TransicionID);
					table.ForeignKey(
						name: "FK_Transicion_Estado_EstadoDestinoID",
						column: x => x.EstadoDestinoID,
						principalTable: "Estado",
						principalColumn: "EstadoID",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_Transicion_Estado_EstadoOrigenID",
						column: x => x.EstadoOrigenID,
						principalTable: "Estado",
						principalColumn: "EstadoID",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "HistorialTransicionCMSComponente",
				columns: table => new
				{
					HistorialTransicionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
					ComponenteID = table.Column<Guid>(type: "RAW(16)", nullable: false),
					TransicionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
					IdentidadID = table.Column<Guid>(type: "RAW(16)", nullable: false),
					Fecha = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
					Comentario = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_HistorialTransicionCMSComponente", x => x.HistorialTransicionID);
					table.ForeignKey(
						name: "FK_HistorialTransicionCMSComponente_CMSComponente_ComponenteID",
						column: x => x.ComponenteID,
						principalTable: "CMSComponente",
						principalColumn: "ComponenteID",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_HistorialTransicionCMSComponente_Identidad_IdentidadID",
						column: x => x.IdentidadID,
						principalTable: "Identidad",
						principalColumn: "IdentidadID",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_HistorialTransicionCMSComponente_Transicion_TransicionID",
						column: x => x.TransicionID,
						principalTable: "Transicion",
						principalColumn: "TransicionID",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "HistorialTransicionDocumento",
				columns: table => new
				{
					HistorialTransicionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
					DocumentoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
					TransicionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
					IdentidadID = table.Column<Guid>(type: "RAW(16)", nullable: false),
					Fecha = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
					Comentario = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_HistorialTransicionDocumento", x => x.HistorialTransicionID);
					table.ForeignKey(
						name: "FK_HistorialTransicionDocumento_Documento_DocumentoID",
						column: x => x.DocumentoID,
						principalTable: "Documento",
						principalColumn: "DocumentoID",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_HistorialTransicionDocumento_Identidad_IdentidadID",
						column: x => x.IdentidadID,
						principalTable: "Identidad",
						principalColumn: "IdentidadID",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_HistorialTransicionDocumento_Transicion_TransicionID",
						column: x => x.TransicionID,
						principalTable: "Transicion",
						principalColumn: "TransicionID",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "HistorialTransicionPestanyaCMS",
				columns: table => new
				{
					HistorialTransicionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
					PestanyaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
					Ubicacion = table.Column<short>(type: "NUMBER(5)", nullable: false),
					TransicionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
					IdentidadID = table.Column<Guid>(type: "RAW(16)", nullable: false),
					Fecha = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
					Comentario = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_HistorialTransicionPestanyaCMS", x => x.HistorialTransicionID);
					table.ForeignKey(
						name: "FK_HistorialTransicionPestanyaCMS_Identidad_IdentidadID",
						column: x => x.IdentidadID,
						principalTable: "Identidad",
						principalColumn: "IdentidadID",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_HistorialTransicionPestanyaCMS_ProyectoPestanyaCMS_PestanyaID_Ubicacion",
						columns: x => new { x.PestanyaID, x.Ubicacion },
						principalTable: "ProyectoPestanyaCMS",
						principalColumns: new[] { "PestanyaID", "Ubicacion" },
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_HistorialTransicionPestanyaCMS_Transicion_TransicionID",
						column: x => x.TransicionID,
						principalTable: "Transicion",
						principalColumn: "TransicionID",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "TransicionGrupo",
				columns: table => new
				{
					TransicionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
					GrupoID = table.Column<Guid>(type: "RAW(16)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_TransicionGrupo", x => new { x.TransicionID, x.GrupoID });
					table.ForeignKey(
						name: "FK_TransicionGrupo_GrupoIdentidades_GrupoID",
						column: x => x.GrupoID,
						principalTable: "GrupoIdentidades",
						principalColumn: "GrupoID",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_TransicionGrupo_Transicion_TransicionID",
						column: x => x.TransicionID,
						principalTable: "Transicion",
						principalColumn: "TransicionID",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "TransicionIdentidad",
				columns: table => new
				{
					TransicionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
					IdentidadID = table.Column<Guid>(type: "RAW(16)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_TransicionIdentidad", x => new { x.TransicionID, x.IdentidadID });
					table.ForeignKey(
						name: "FK_TransicionIdentidad_Identidad_IdentidadID",
						column: x => x.IdentidadID,
						principalTable: "Identidad",
						principalColumn: "IdentidadID",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_TransicionIdentidad_Transicion_TransicionID",
						column: x => x.TransicionID,
						principalTable: "Transicion",
						principalColumn: "TransicionID",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "IX_ProyectoPestanyaCMS_EstadoID",
				table: "ProyectoPestanyaCMS",
				column: "EstadoID");

			migrationBuilder.CreateIndex(
				name: "IX_Documento_EstadoID",
				table: "Documento",
				column: "EstadoID");

			migrationBuilder.CreateIndex(
				name: "IX_CMSComponente_EstadoID",
				table: "CMSComponente",
				column: "EstadoID");

			migrationBuilder.CreateIndex(
				name: "IX_Estado_FlujoID",
				table: "Estado",
				column: "FlujoID");

			migrationBuilder.CreateIndex(
				name: "IX_EstadoGrupo_GrupoID",
				table: "EstadoGrupo",
				column: "GrupoID");

			migrationBuilder.CreateIndex(
				name: "IX_EstadoIdentidad_IdentidadID",
				table: "EstadoIdentidad",
				column: "IdentidadID");

			migrationBuilder.CreateIndex(
				name: "IX_Flujo_OrganizacionID_ProyectoID",
				table: "Flujo",
				columns: new[] { "OrganizacionID", "ProyectoID" });

			migrationBuilder.CreateIndex(
				name: "IX_FlujoObjetoConocimientoProyecto_OrganizacionID_ProyectoID_Ontologia",
				table: "FlujoObjetoConocimientoProyecto",
				columns: new[] { "OrganizacionID", "ProyectoID", "Ontologia" },
				unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_HistorialTransicionCMSComponente_ComponenteID",
				table: "HistorialTransicionCMSComponente",
				column: "ComponenteID");

			migrationBuilder.CreateIndex(
				name: "IX_HistorialTransicionCMSComponente_IdentidadID",
				table: "HistorialTransicionCMSComponente",
				column: "IdentidadID");

			migrationBuilder.CreateIndex(
				name: "IX_HistorialTransicionCMSComponente_TransicionID",
				table: "HistorialTransicionCMSComponente",
				column: "TransicionID");

			migrationBuilder.CreateIndex(
				name: "IX_HistorialTransicionDocumento_DocumentoID",
				table: "HistorialTransicionDocumento",
				column: "DocumentoID");

			migrationBuilder.CreateIndex(
				name: "IX_HistorialTransicionDocumento_IdentidadID",
				table: "HistorialTransicionDocumento",
				column: "IdentidadID");

			migrationBuilder.CreateIndex(
				name: "IX_HistorialTransicionDocumento_TransicionID",
				table: "HistorialTransicionDocumento",
				column: "TransicionID");

			migrationBuilder.CreateIndex(
				name: "IX_HistorialTransicionPestanyaCMS_IdentidadID",
				table: "HistorialTransicionPestanyaCMS",
				column: "IdentidadID");

			migrationBuilder.CreateIndex(
				name: "IX_HistorialTransicionPestanyaCMS_PestanyaID_Ubicacion",
				table: "HistorialTransicionPestanyaCMS",
				columns: new[] { "PestanyaID", "Ubicacion" });

			migrationBuilder.CreateIndex(
				name: "IX_HistorialTransicionPestanyaCMS_TransicionID",
				table: "HistorialTransicionPestanyaCMS",
				column: "TransicionID");

			migrationBuilder.CreateIndex(
				name: "IX_Transicion_EstadoDestinoID",
				table: "Transicion",
				column: "EstadoDestinoID");

			migrationBuilder.CreateIndex(
				name: "IX_Transicion_EstadoOrigenID_EstadoDestinoID",
				table: "Transicion",
				columns: new[] { "EstadoOrigenID", "EstadoDestinoID" },
				unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_TransicionGrupo_GrupoID",
				table: "TransicionGrupo",
				column: "GrupoID");

			migrationBuilder.CreateIndex(
				name: "IX_TransicionIdentidad_IdentidadID",
				table: "TransicionIdentidad",
				column: "IdentidadID");

			migrationBuilder.AddForeignKey(
				name: "FK_CMSComponente_Estado_EstadoID",
				table: "CMSComponente",
				column: "EstadoID",
				principalTable: "Estado",
				principalColumn: "EstadoID",
				onDelete: ReferentialAction.Cascade);

			migrationBuilder.AddForeignKey(
				name: "FK_Documento_Estado_EstadoID",
				table: "Documento",
				column: "EstadoID",
				principalTable: "Estado",
				principalColumn: "EstadoID",
				onDelete: ReferentialAction.Cascade);

			migrationBuilder.AddForeignKey(
				name: "FK_ProyectoPestanyaCMS_Estado_EstadoID",
				table: "ProyectoPestanyaCMS",
				column: "EstadoID",
				principalTable: "Estado",
				principalColumn: "EstadoID",
				onDelete: ReferentialAction.Cascade);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_CMSComponente_Estado_EstadoID",
				table: "CMSComponente");

			migrationBuilder.DropForeignKey(
				name: "FK_Documento_Estado_EstadoID",
				table: "Documento");

			migrationBuilder.DropForeignKey(
				name: "FK_ProyectoPestanyaCMS_Estado_EstadoID",
				table: "ProyectoPestanyaCMS");

			migrationBuilder.DropTable(
				name: "EstadoGrupo");

			migrationBuilder.DropTable(
				name: "EstadoIdentidad");

			migrationBuilder.DropTable(
				name: "FlujoObjetoConocimientoProyecto");

			migrationBuilder.DropTable(
				name: "HistorialTransicionCMSComponente");

			migrationBuilder.DropTable(
				name: "HistorialTransicionDocumento");

			migrationBuilder.DropTable(
				name: "HistorialTransicionPestanyaCMS");

			migrationBuilder.DropTable(
				name: "TransicionGrupo");

			migrationBuilder.DropTable(
				name: "TransicionIdentidad");

			migrationBuilder.DropTable(
				name: "Transicion");

			migrationBuilder.DropTable(
				name: "Estado");

			migrationBuilder.DropTable(
				name: "Flujo");

			migrationBuilder.DropIndex(
				name: "IX_ProyectoPestanyaCMS_EstadoID",
				table: "ProyectoPestanyaCMS");

			migrationBuilder.DropIndex(
				name: "IX_Documento_EstadoID",
				table: "Documento");

			migrationBuilder.DropIndex(
				name: "IX_CMSComponente_EstadoID",
				table: "CMSComponente");

			migrationBuilder.DropColumn(
				name: "EstadoID",
				table: "ProyectoPestanyaCMS");

			migrationBuilder.DropColumn(
				name: "EstadoID",
				table: "Documento");

			migrationBuilder.DropColumn(
				name: "EstadoID",
				table: "CMSComponente");
		}
	}
}
