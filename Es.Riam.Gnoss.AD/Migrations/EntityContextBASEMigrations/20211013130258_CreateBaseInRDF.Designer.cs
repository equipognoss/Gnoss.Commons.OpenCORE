﻿// <auto-generated />
using System;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextBASEMigrations
{
    [DbContext(typeof(EntityContextBASE))]
    [Migration("20211013130258_CreateBaseInRDF")]
    partial class CreateBaseInRDF
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.10")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Es.Riam.Gnoss.AD.EntityModelBASE.Models.ColaCorreo", b =>
                {
                    b.Property<int>("CorreoID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Asunto")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DireccionRespuesta")
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<bool>("EnviadoRabbit")
                        .HasColumnType("bit");

                    b.Property<bool>("EsHtml")
                        .HasColumnType("bit");

                    b.Property<bool>("EsSeguro")
                        .HasColumnType("bit");

                    b.Property<DateTime>("FechaPuestaEnCola")
                        .HasColumnType("datetime2");

                    b.Property<string>("HtmlTexto")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MascaraDireccionRespuesta")
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("MascaraRemitente")
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<short>("Prioridad")
                        .HasColumnType("smallint");

                    b.Property<int>("Puerto")
                        .HasColumnType("int");

                    b.Property<string>("Remitente")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("SMTP")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("Usuario")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("tipo")
                        .IsRequired()
                        .HasMaxLength(10)
                        .IsUnicode(false)
                        .HasColumnType("varchar(10)");

                    b.HasKey("CorreoID");

                    b.ToTable("ColaCorreo");
                });

            modelBuilder.Entity("Es.Riam.Gnoss.AD.EntityModelBASE.Models.ColaCorreoDestinatario", b =>
                {
                    b.Property<int>("CorreoID")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .HasMaxLength(400)
                        .HasColumnType("nvarchar(400)");

                    b.Property<short>("Estado")
                        .HasColumnType("smallint");

                    b.Property<DateTime?>("FechaProcesado")
                        .HasColumnType("datetime2");

                    b.Property<string>("MascaraDestinatario")
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.HasKey("CorreoID", "Email");

                    b.ToTable("ColaCorreoDestinatario");
                });

            modelBuilder.Entity("Es.Riam.Gnoss.AD.EntityModelBASE.Models.ColaCorreoDestinatario", b =>
                {
                    b.HasOne("Es.Riam.Gnoss.AD.EntityModelBASE.Models.ColaCorreo", "ColaCorreo")
                        .WithMany("ColaCorreoDestinatario")
                        .HasForeignKey("CorreoID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("ColaCorreo");
                });

            modelBuilder.Entity("Es.Riam.Gnoss.AD.EntityModelBASE.Models.ColaCorreo", b =>
                {
                    b.Navigation("ColaCorreoDestinatario");
                });
#pragma warning restore 612, 618
        }
    }
}
