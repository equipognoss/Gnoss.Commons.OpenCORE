using Es.Riam.Gnoss.AD.EntityModelBASE.Models;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using Npgsql;

namespace Es.Riam.Gnoss.AD.EntityModelBASE
{
    public partial class EntityContextBASE : DbContext
    {
        private bool mCache;
        private string mDefaultSchema;
        DbContextOptions<EntityContextBASE> mDbContextOptions;
        private UtilPeticion mUtilPeticion;
        private LoggingService mLoggingService;
        private ConfigService mConfigService;

        public EntityContextBASE(UtilPeticion utilPeticion, LoggingService loggingService, ConfigService configService, DbContextOptions<EntityContextBASE> dbContextOptions, bool pCache = false)
            : base(dbContextOptions)
        {
            mCache = pCache;
            mUtilPeticion = utilPeticion;
            mLoggingService = loggingService;
            mConfigService = configService;
            mDbContextOptions = dbContextOptions;
        }

        internal EntityContextBASE(UtilPeticion utilPeticion, LoggingService loggingService, ConfigService configService, DbContextOptions<EntityContextBASE> dbContextOptions, string pDefaultSchema = null, bool pCache = false)
           : base(dbContextOptions)
        {
            mDefaultSchema = pDefaultSchema;
            mCache = pCache;
            mUtilPeticion = utilPeticion;
            mLoggingService = loggingService;
            mConfigService = configService;
            mDbContextOptions = dbContextOptions;
        }
        public void Migrate()
        {
            Database.Migrate();
        }
        public virtual DbSet<ColaCorreo> ColaCorreo { get; set; }
        public virtual DbSet<ColaCorreoDestinatario> ColaCorreoDestinatario { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (mConfigService.ObtenerTipoBD().Equals("2"))
            {
                optionsBuilder.UseNpgsql(mConfigService.ObtenerBaseConnectionString());
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ColaCorreoDestinatario>()
               .HasKey(c => new { c.CorreoID, c.Email });
            modelBuilder.Entity<ColaCorreo>()
               .Property(e => e.tipo)
               .IsUnicode(false);

            modelBuilder.Entity<ColaCorreo>()
                .HasMany(e => e.ColaCorreoDestinatario)
                .WithOne(e => e.ColaCorreo)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void InicializarEntityContextBASECache()
        {
            var conexion = ObtenerConexion();
            //EntityContext [System.Runtime.CompilerServices.CallerFilePath] string memberName = ""
            string schemaDefecto = GetDafaultSchema(conexion);

            //UtilPeticion.AgregarObjetoAPeticionActual("EntityContextSinProxy", new EntityContext(conexion, schemaDefecto));
            //UsarEntityCache = true;

            mUtilPeticion.AgregarObjetoAPeticionActual("EntityContextSinProxy", new EntityContextBASE(mUtilPeticion, mLoggingService, mConfigService, mDbContextOptions, schemaDefecto));

        }

        public bool ContextoInicializado
        {
            get
            {
                bool? ini = (bool?)mUtilPeticion.ObtenerObjetoDePeticion("ContextoInicializadoBASE");
                if (!ini.HasValue)
                {
                    ini = false;
                }
                return ini.Value;
            }
            set
            {
                mUtilPeticion.AgregarObjetoAPeticionActual("ContextoInicializadoBASE", value);
            }
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
            else if (pConexionMaster is OracleConnection)
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
            DbConnection conexion = null;
            if (conexion == null)
            {
                conexion = (DbConnection)mUtilPeticion.ObtenerObjetoDePeticion("ConexionBase");

                if (conexion == null)
                {
                    string ficheroConexion = (string)mUtilPeticion.ObtenerObjetoDePeticion("FicheroConexion");
                   
                    string tipoBD = mConfigService.ObtenerTipoBD();

                    Microsoft.Practices.EnterpriseLibrary.Data.Database database = null;
                    if (!string.IsNullOrEmpty(tipoBD) && tipoBD.Equals("1"))
                    {
                        string connectionString = mConfigService.ObtenerBaseConnectionString();
                        string[] partesConexion = connectionString.Split(new string[] { "$$$" }, StringSplitOptions.RemoveEmptyEntries);
                        if (partesConexion.Length > 1 && partesConexion[1].StartsWith("TimeOut="))
                        {
                            connectionString = partesConexion[0];
                        }
                        conexion = new OracleConnection(connectionString);
                        conexion.Open();
                    }
                    else if (!string.IsNullOrEmpty(tipoBD) && tipoBD.Equals("2"))
                    {
                        string connectionString = mConfigService.ObtenerBaseConnectionString();
                        string[] partesConexion = connectionString.Split(new string[] { "$$$" }, StringSplitOptions.RemoveEmptyEntries);
                        if (partesConexion.Length > 1 && partesConexion[1].StartsWith("TimeOut="))
                        {
                            connectionString = partesConexion[0];
                        }
                        conexion = new NpgsqlConnection(connectionString);
                        conexion.Open();
                    }
                    else
                    {
                        string connectionString = mConfigService.ObtenerBaseConnectionString();
                        string[] partesConexion = connectionString.Split(new string[] { "$$$" }, StringSplitOptions.RemoveEmptyEntries);
                        if (partesConexion.Length > 1 && partesConexion[1].StartsWith("TimeOut="))
                        {
                            connectionString = partesConexion[0];
                        }
                        database = new SqlDatabase(connectionString);
                        conexion = database.CreateConnection();
                        conexion.Open();
                    }

                    mUtilPeticion.AgregarObjetoAPeticionActual("ConexionBase", conexion);
                }
            }
            return conexion;
        }

        private void InicializarEntityContextBASE()
        {
            var conexion = ObtenerConexion();
            //EntityContext [System.Runtime.CompilerServices.CallerFilePath] string memberName = ""
            string schemaDefecto = GetDafaultSchema(conexion);

            //UtilPeticion.AgregarObjetoAPeticionActual("EntityContextSinProxy", new EntityContext(conexion, schemaDefecto));
            //UsarEntityCache = true;

            mUtilPeticion.AgregarObjetoAPeticionActual("EntityContextBase", new EntityContextBASE(mUtilPeticion, mLoggingService, mConfigService, mDbContextOptions, schemaDefecto));
            //UsarEntityCache = false;
            ContextoInicializado = true;
        }

        public bool UsarEntityCache
        {
            get
            {
                bool? usarEntityCache = mUtilPeticion.ObtenerObjetoDePeticion("EntityContextCache") as bool?;

                return usarEntityCache.HasValue && usarEntityCache.Value;
            }
            set
            {
                mUtilPeticion.AgregarObjetoAPeticionActual("EntityContextCache", value);
            }
        }
    }
}
