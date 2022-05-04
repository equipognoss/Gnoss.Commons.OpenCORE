using Es.Riam.Gnoss.AD;
using Es.Riam.Gnoss.OAuthAD.OAuth;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data.Common;

namespace Es.Riam.Gnoss.OAuthAD
{
    public class EntityContextOauth : DbContext
    {

        public static bool ProxyCreationEnabled { get; set; } = true;
        private LoggingService mLoggingService;
        private ConfigService mConfigService;
        private string mDefaultSchema;
        /// <summary>
        /// Constructor internal, para obtener un objeto EntityContext, llamar al método ObtenerEntityContext del BaseAD
        /// </summary>
        public EntityContextOauth(LoggingService loggingService, DbContextOptions<EntityContextOauth> dbContextOptions, ConfigService configService)
            : base(dbContextOptions)
        {
            mLoggingService = loggingService;
            mConfigService = configService;
        }

        /// <summary>
        /// Constructor internal, para obtener un objeto EntityContext, llamar al método ObtenerEntityContext del BaseAD
        /// </summary>
        public EntityContextOauth(LoggingService loggingService, DbContextOptions<EntityContextOauth> dbContextOptions, ConfigService configService, string pDefaultSchema = null)
           : base(dbContextOptions)
        {
            mDefaultSchema = pDefaultSchema;
            mLoggingService = loggingService;
            mConfigService = configService;
        }

        private string GetDafaultSchema(DbConnection pConexionMaster)
        {
            string schemaDefecto = null;

            if (BaseAD.ListaDefaultSchemaPorConexion == null)
            {
                BaseAD.ListaDefaultSchemaPorConexion = new System.Collections.Concurrent.ConcurrentDictionary<string, string>();
            }

            if (BaseAD.ListaDefaultSchemaPorConexion.ContainsKey(pConexionMaster.ConnectionString))
            {
                schemaDefecto = BaseAD.ListaDefaultSchemaPorConexion[pConexionMaster.ConnectionString];
            }
            else if (pConexionMaster is SqlConnection)
            {
                try
                {
                    DbCommand dbCommand = new SqlCommand("select SCHEMA_NAME()", (SqlConnection)pConexionMaster);

                    schemaDefecto = (string)dbCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    mLoggingService.GuardarLogError(ex, "Error al obtener el contexto por defecto de la base de datos: " + pConexionMaster.ConnectionString);
                }
                BaseAD.ListaDefaultSchemaPorConexion.TryAdd(pConexionMaster.ConnectionString, schemaDefecto);
            }
            else
            {
                try
                {
                    DbCommand dbCommand = new OracleCommand("SELECT SYS_CONTEXT('USERENV','CURRENT_SCHEMA') FROM DUAL", (OracleConnection)pConexionMaster);
                    schemaDefecto = (string)dbCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    mLoggingService.GuardarLogError(ex, "Error al obtener el contexto por defecto de la base de datos: " + pConexionMaster.ConnectionString);
                }
                BaseAD.ListaDefaultSchemaPorConexion.TryAdd(pConexionMaster.ConnectionString, schemaDefecto);

            }

            return schemaDefecto;
        }

        /// <summary>
        /// Obtiene la conexión a la base de datos
        /// </summary>
        protected DbConnection ObtenerConexion()
        {

            DbConnection conexion = this.Database.GetDbConnection();

            if (conexion == null)
            {
                string tipoBD = mConfigService.ObtenerTipoBD();

                Microsoft.Practices.EnterpriseLibrary.Data.Database database = null;
                if (!string.IsNullOrEmpty(tipoBD) && tipoBD.Equals("1"))
                {
                    string connectionString = mConfigService.ObtenerOauthConnectionString();
                    string[] partesConexion = connectionString.Split(new string[] { "$$$" }, StringSplitOptions.RemoveEmptyEntries);
                    if (partesConexion.Length > 1 && partesConexion[1].StartsWith("TimeOut="))
                    {
                        connectionString = partesConexion[0];
                    }
                    conexion = new OracleConnection(connectionString);
                    conexion.Open();
                }
                else
                {
                    string connectionString = mConfigService.ObtenerOauthConnectionString();
                    string[] partesConexion = connectionString.Split(new string[] { "$$$" }, StringSplitOptions.RemoveEmptyEntries);
                    if (partesConexion.Length > 1 && partesConexion[1].StartsWith("TimeOut="))
                    {
                        connectionString = partesConexion[0];
                    }
                    database = new SqlDatabase(connectionString);
                    conexion = database.CreateConnection();
                    conexion.Open();
                }
            }
            return conexion;
        }

        public virtual DbSet<ConsumerData> ConsumerData { get; set; }
        public virtual DbSet<Nonce> Nonce { get; set; }
        public virtual DbSet<OAuthConsumer> OAuthConsumer { get; set; }
        public virtual DbSet<OAuthToken> OAuthToken { get; set; }
        public virtual DbSet<OAuthTokenExterno> OAuthTokenExterno { get; set; }
        public virtual DbSet<PinToken> PinToken { get; set; }
        public virtual DbSet<Usuario> Usuario { get; set; }
        public virtual DbSet<UsuarioConsumer> UsuarioConsumer { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ConsumerData>(entity =>
            {
                entity.Property(e => e.ConsumerId).ValueGeneratedNever();

                entity.HasOne(d => d.OAuthConsumer)
                    .WithOne(p => p.ConsumerData)
                    .HasForeignKey<ConsumerData>(d => d.ConsumerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ConsumerData_OAuthConsumer");
            });

            modelBuilder.Entity<Nonce>(entity =>
            {
                entity.HasKey(e => new { e.Context, e.Code, e.Timestamp });
            });

            modelBuilder.Entity<OAuthConsumer>(entity =>
            {
                entity.HasKey(e => e.ConsumerId)
                    .HasName("PK_dbo.OAuthConsumer");
            });

            modelBuilder.Entity<OAuthToken>(entity =>
            {
                entity.HasKey(e => e.TokenId)
                    .HasName("PK_dbo.OAuthToken");

                entity.HasOne(d => d.OAuthConsumer)
                    .WithMany(p => p.OAuthToken)
                    .HasForeignKey(d => d.ConsumerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("OAuthConsumer_OAuthToken");

                entity.HasOne(d => d.Usuario)
                    .WithMany(p => p.OAuthToken)
                    .HasForeignKey(d => d.UsuarioID)
                    .HasConstraintName("FK_OAuthToken_Usuario");
            });

            modelBuilder.Entity<OAuthTokenExterno>(entity =>
            {
                entity.Property(e => e.TokenId).ValueGeneratedOnAdd();

                entity.HasOne(d => d.OAuthToken)
                    .WithOne(p => p.OAuthTokenExterno)
                    .HasForeignKey<OAuthTokenExterno>(d => d.TokenId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OAuthTokenExterno_OAuthToken");

                entity.HasOne(d => d.Usuario)
                    .WithMany(p => p.OAuthTokenExterno)
                    .HasForeignKey(d => d.UsuarioID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OAuthTokenExterno_Usuario");
            });

            modelBuilder.Entity<PinToken>(entity =>
            {
                entity.Property(e => e.TokenId).ValueGeneratedNever();

                entity.HasOne(d => d.OAuthToken)
                    .WithOne(p => p.PinToken)
                    .HasForeignKey<PinToken>(d => d.TokenId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PinToken_OAuthToken");

                entity.HasOne(d => d.Usuario)
                    .WithMany(p => p.PinToken)
                    .HasForeignKey(d => d.UsuarioID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PinToken_Usuario");
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.Property(e => e.UsuarioID).ValueGeneratedNever();
            });

            modelBuilder.Entity<UsuarioConsumer>(entity =>
            {
                entity.HasKey(e => e.ConsumerId)
                    .HasName("PK_Usuario_Consumer");

                entity.Property(e => e.ConsumerId).ValueGeneratedNever();

                entity.HasOne(d => d.OAuthConsumer)
                    .WithOne(p => p.UsuarioConsumer)
                    .HasForeignKey<UsuarioConsumer>(d => d.ConsumerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Usuario_Consumer_OAuthConsumer");

                entity.HasOne(d => d.Usuario)
                    .WithMany(p => p.UsuarioConsumer)
                    .HasForeignKey(d => d.UsuarioID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Usuario_Consumer_Usuario");
            });


            if (mDefaultSchema != null && !mDefaultSchema.Equals("dbo"))
            {
                modelBuilder.HasDefaultSchema(mDefaultSchema);
            }
        }

        public void EliminarItem(object tokenViejo)
        {
            Entry(tokenViejo).State = EntityState.Deleted;
        }
    }
}
