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
using Microsoft.Extensions.Logging;
using Es.Riam.Gnoss.AD.ParametroAplicacion;

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
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public EntityContextBASE(UtilPeticion utilPeticion, LoggingService loggingService, ConfigService configService, DbContextOptions<EntityContextBASE> dbContextOptions, ILogger<EntityContextBASE> logger, ILoggerFactory loggerFactory, bool pCache = false)
            : base(dbContextOptions)
        {
            mCache = pCache;
            mUtilPeticion = utilPeticion;
            mLoggingService = loggingService;
            mConfigService = configService;
            mDbContextOptions = dbContextOptions;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        internal EntityContextBASE(UtilPeticion utilPeticion, LoggingService loggingService, ConfigService configService, DbContextOptions<EntityContextBASE> dbContextOptions, ILogger<EntityContextBASE> logger, ILoggerFactory loggerFactory, string pDefaultSchema = null, bool pCache = false)
           : base(dbContextOptions)
        {
            mDefaultSchema = pDefaultSchema;
            mCache = pCache;
            mUtilPeticion = utilPeticion;
            mLoggingService = loggingService;
            mConfigService = configService;
            mDbContextOptions = dbContextOptions;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }
        public void Migrate()
        {
            Database.Migrate();
        }
        public virtual DbSet<ColaCorreo> ColaCorreo { get; set; }
        public virtual DbSet<ColaCorreoDestinatario> ColaCorreoDestinatario { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string tipoBD = mConfigService.ObtenerTipoBD();
            string acid = mConfigService.ObtenerSqlConnectionString();

            optionsBuilder.LogTo(mLoggingService.AgregarEntradaTrazaEntity);
            switch (tipoBD)
            {
                case "0":
                    optionsBuilder.UseSqlServer(mConfigService.ObtenerSqlConnectionString(), o => o.UseCompatibilityLevel(110));
                    break;

                case "1":
                    optionsBuilder.UseOracle(mConfigService.ObtenerSqlConnectionString(), o => o.UseOracleSQLCompatibility(ObtenerNivelCompatibilidadOracle()));
                    break;

                case "2":
                    optionsBuilder.UseNpgsql(mConfigService.ObtenerSqlConnectionString(), o => o.SetPostgresVersion(new Version(9, 6)));
                    break;
            }

            optionsBuilder.UseLoggerFactory(mLoggerFactory);
        }

        /// <summary>
        /// Obtiene el nivel de compatibilidad de Oracle configurado en las variables de entorno. 
        /// En caso de no haber nada configurado utilizará el nivel de compatibilidad de la última versión.
        /// </summary>        
        /// <returns>Devuelve el valor del nivel de compatibilidad necesario para el provider de oracle</returns>
        private OracleSQLCompatibility ObtenerNivelCompatibilidadOracle()
        {
            OracleSQLCompatibility nivelCompatibilidad;
            string nivelConfigurado = mConfigService.ObtenerNivelCompatibiliadBaseDatos();

            switch (nivelConfigurado)
            {
                case "19":
                    nivelCompatibilidad = OracleSQLCompatibility.DatabaseVersion19;
                    break;
                case "21":
                    nivelCompatibilidad = OracleSQLCompatibility.DatabaseVersion21;
                    break;
                case "23":
                    nivelCompatibilidad = OracleSQLCompatibility.DatabaseVersion23;
                    break;
                default:
                    nivelCompatibilidad = OracleSQLCompatibility.DatabaseVersion23;
                    break;
            }

            return nivelCompatibilidad;
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

            mUtilPeticion.AgregarObjetoAPeticionActual("EntityContextSinProxy", new EntityContextBASE(mUtilPeticion, mLoggingService, mConfigService, mDbContextOptions, mLoggerFactory.CreateLogger<EntityContextBASE>(),mLoggerFactory,schemaDefecto));

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
                    mLoggingService.GuardarLogError(ex, "Error al obtener el contexto por defecto de la base de datos: " + pConexionMaster.ConnectionString,mlogger);
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
                    mLoggingService.GuardarLogError(ex,"Error al obtener el contexto por defecto de la base de datos: " + pConexionMaster.ConnectionString,mlogger);
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

            mUtilPeticion.AgregarObjetoAPeticionActual("EntityContextBase", new EntityContextBASE(mUtilPeticion, mLoggingService, mConfigService, mDbContextOptions,mLoggerFactory.CreateLogger<EntityContextBASE>() ,mLoggerFactory,schemaDefecto));
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
