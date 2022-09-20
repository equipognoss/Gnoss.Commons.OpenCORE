using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.Amigos;
using Es.Riam.Gnoss.AD.Amigos.Model;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.OrganizacionDS;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.Live.Model;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Amigos;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Suscripcion;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Interfaces;
using Es.Riam.Util;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Es.Riam.Gnoss.Elementos.Identidad
{
    /// <summary>
    /// Gestor de identidades
    /// </summary>
    [Serializable]
    public class GestionIdentidades : GestionGnoss, ISerializable, IDisposable
    {
        #region Miembros

        private GestionPersonas mGestorPersonas;

        private GestionOrganizaciones mGestorOrganizaciones;

        private GestionUsuarios mGestorUsuarios;

        private GestionSuscripcion mGestorSuscripciones;

        private SortedList<Guid, GrupoIdentidades> mListaGrupos;

        private SortedList<Guid, Perfil> mListaPerfiles;

        private SortedList<Guid, Identidad> mListaIdentidades;

        /// <summary>
        /// Contiene una lista con las identidades que son visibles para usuarios no gnoss
        /// </summary>
        private SortedList<Guid, bool> mListaIdentidadesVisiblesExternos;

        /// <summary>
        /// Gestor de amigos
        /// </summary>
        private GestionAmigos mGestorAmigos;

        private LoggingService mLoggingService;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pDataWrapperIdentidad">Dataset de identidades</param>
        public GestionIdentidades(DataWrapperIdentidad pDataWrapperIdentidad, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pDataWrapperIdentidad, loggingService)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pDataWrapperIdentidad">Dataset de identidades</param>
        /// <param name="pGestorOrganizaciones">Gestor de organizaciones</param>
        /// <param name="pGestorPersonas">Gestor de personas</param>
        public GestionIdentidades(DataWrapperIdentidad pDataWrapperIdentidad, GestionPersonas pGestorPersonas, GestionOrganizaciones pGestorOrganizaciones, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pDataWrapperIdentidad, loggingService)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;

            this.mGestorOrganizaciones = pGestorOrganizaciones;
            GestorPersonas = pGestorPersonas;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pDataWrapperIdentidad">Dataset de identidades</param>
        /// <param name="pGestorPersonas">Gestor de personas</param>
        /// <param name="pGestorUsuarios">Gestor de usuarios</param>
        /// <param name="pGestorOrganizaciones">Gestor de organizaciones</param>
        public GestionIdentidades(DataWrapperIdentidad pDataWrapperIdentidad, GestionPersonas pGestorPersonas, GestionUsuarios pGestorUsuarios, GestionOrganizaciones pGestorOrganizaciones, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pDataWrapperIdentidad, loggingService)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;

            this.mGestorOrganizaciones = pGestorOrganizaciones;
            GestorPersonas = pGestorPersonas;
            this.mGestorUsuarios = pGestorUsuarios;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
        }

        /// <summary>
        /// Constructor para la deseralización
        /// </summary>
        /// <param name="pInfo">Datos serializados</param>
        /// <param name="pContext">Contexto de serialización</param>
        protected GestionIdentidades(SerializationInfo pInfo, StreamingContext pContext)
            : base(pInfo, pContext)
        {
            //mLoggingService = loggingService;
            //mEntityContext = entityContext;
            //mConfigService = configService;

            mGestorAmigos = (GestionAmigos)pInfo.GetValue("GestorAmigos", typeof(GestionAmigos));
            mGestorOrganizaciones = (GestionOrganizaciones)pInfo.GetValue("GestorOrganizaciones", typeof(GestionOrganizaciones));
            mGestorPersonas = (GestionPersonas)pInfo.GetValue("GestorPersonas", typeof(GestionPersonas));
            mGestorUsuarios = (GestionUsuarios)pInfo.GetValue("GestorUsuarios", typeof(GestionUsuarios));
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene el dataset de identidades
        /// </summary>
        public DataWrapperIdentidad DataWrapperIdentidad
        {
            get
            {
                return (DataWrapperIdentidad)DataWrapper;
            }
        }

        /// <summary>
        /// Obtiene o establece el gestor de personas
        /// </summary>
        public GestionPersonas GestorPersonas
        {
            get
            {
                return mGestorPersonas;
            }
            set
            {
                mGestorPersonas = value;

                if ((mGestorOrganizaciones != null) && (mGestorOrganizaciones.GestorPersonas == null))
                {
                    this.mGestorOrganizaciones.GestorPersonas = value;
                }
            }
        }

        /// <summary>
        /// Obtiene o establece el gestor de usuarios
        /// </summary>
        public GestionUsuarios GestorUsuarios
        {
            get
            {
                return mGestorUsuarios;
            }
            set
            {
                mGestorUsuarios = value;

                if (GestorPersonas != null)
                {
                    GestorPersonas.GestorUsuarios = mGestorUsuarios;
                }
            }
        }

        /// <summary>
        /// Obtiene o establece el gestor de amigos
        /// </summary>
        public GestionAmigos GestorAmigos
        {
            get
            {
                return mGestorAmigos;
            }
            set
            {
                mGestorAmigos = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el gestor de organizaciones
        /// </summary>
        public GestionOrganizaciones GestorOrganizaciones
        {
            get
            {
                return mGestorOrganizaciones;
            }
            set
            {
                this.mGestorOrganizaciones = value;

                if (mGestorOrganizaciones != null && (mGestorOrganizaciones.GestorPersonas != null) && (mGestorPersonas == null))
                {
                    mGestorPersonas = mGestorOrganizaciones.GestorPersonas;
                }
            }
        }

        /// <summary>
        /// Obtiene o establece el gestor de suscripciones
        /// </summary>
        public GestionSuscripcion GestorSuscripciones
        {
            get
            {
                return mGestorSuscripciones;
            }
            set
            {
                mGestorSuscripciones = value;
            }
        }

        /// <summary>
        /// Obtiene la lista de hijos
        /// </summary>
        public override List<IElementoGnoss> Hijos
        {
            get
            {
                if (this.mHijos == null)
                {
                    CargarHijos();
                }
                return this.mHijos;
            }
        }

        /// <summary>
        /// Obtiene la lista de perfiles
        /// </summary>
        public SortedList<Guid, Perfil> ListaPerfiles
        {
            get
            {
                if (mListaPerfiles == null)
                {
                    CargarHijos();
                }
                return mListaPerfiles;
            }
        }

        /// <summary>
        /// Obtiene la lista de identidades
        /// </summary>
        public SortedList<Guid, Identidad> ListaIdentidades
        {
            get
            {
                if (mListaIdentidades == null)
                {
                    CargarHijos();
                }
                return mListaIdentidades;
            }
        }

        /// <summary>
        /// Obtiene la lista de identidades
        /// </summary>
        public SortedList<Guid, GrupoIdentidades> ListaGrupos
        {
            get
            {
                if (mListaGrupos == null)
                {
                    CargarGrupos();
                }
                return mListaGrupos;
            }
        }

        /// <summary>
        /// Obtiene o establece una lista con las identidades que son visibles para usuarios no gnoss
        /// </summary>
        /// 
        public SortedList<Guid, bool> ListaIdentidadesVisiblesExternos
        {
            get
            {
                if (mListaIdentidadesVisiblesExternos == null)
                {
                    mListaIdentidadesVisiblesExternos = new SortedList<Guid, bool>();
                }
                return mListaIdentidadesVisiblesExternos;
            }
            set
            {
                this.mListaIdentidadesVisiblesExternos = value;
            }
        }

        #endregion

        #region Métodos generales

        #region Agregaciones

        #region Públicos

        /// <summary>
        /// Agrega EL perfil de organización a la organización (UNA organización sólo tiene UN perfil de organización)
        /// </summary>
        /// <param name="pOrganizacion">Organización a la que se le va a crear el perfil</param>
        /// <returns>Perfil</returns>
        public Perfil AgregarPerfilOrganizacion(ServiciosGenerales.Organizacion pOrganizacion, Dictionary<Guid, bool> pRecibirNewsletterDefectoProyectos)
        {
            Perfil perfil = AgregarPerfil(pOrganizacion.Nombre, TiposIdentidad.Organizacion, pOrganizacion.FilaOrganizacion.NombreCorto, pOrganizacion.FilaOrganizacion.Alias, "", true, pOrganizacion.Clave, Guid.Empty, ProyectoAD.MetaOrganizacion, ProyectoAD.MetaProyecto, pRecibirNewsletterDefectoProyectos);

            AD.EntityModel.Models.IdentidadDS.PerfilOrganizacion filaPerfilOrg = new AD.EntityModel.Models.IdentidadDS.PerfilOrganizacion();
            filaPerfilOrg.PerfilID = perfil.Clave;
            filaPerfilOrg.OrganizacionID = pOrganizacion.Clave;
            DataWrapperIdentidad.ListaPerfilOrganizacion.Add(filaPerfilOrg);
            mEntityContext.PerfilOrganizacion.Add(filaPerfilOrg);

            if (GestorAmigos == null)
            {
                GestorAmigos = new GestionAmigos(new DataWrapperAmigos(), new GestionIdentidades(new DataWrapperIdentidad(), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication),  mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            }
            AD.EntityModel.Models.IdentidadDS.GrupoAmigos grupoAmigos = new AD.EntityModel.Models.IdentidadDS.GrupoAmigos();
            grupoAmigos.Automatico = true;
            grupoAmigos.Fecha = DateTime.Now;
            grupoAmigos.GrupoID = Guid.NewGuid();
            grupoAmigos.IdentidadID = perfil.IdentidadMyGNOSS.Clave;
            grupoAmigos.Nombre = $"Miembros de {((AD.EntityModel.Models.OrganizacionDS.Organizacion)pOrganizacion.FilaElementoEntity).Alias}";
            grupoAmigos.Tipo = (short)TipoGrupoAmigos.AutomaticoOrganizacion;
            mEntityContext.SaveChanges();
            mEntityContext.GrupoAmigos.Add(grupoAmigos);
            GestorAmigos.AmigosDW.ListaGrupoAmigos.Add(grupoAmigos);
            mEntityContext.SaveChanges();
            return perfil;
        }

        /// <summary>
        /// Agrega EL perfil de organización a la organización (UNA organización sólo tiene UN perfil de organización)
        /// </summary>
        /// <param name="pOrganizacion">Organización a la que se le va a crear el perfil</param>
        /// <param name="pCrearIdentidadEnMetaProyecto">TRUE si debe crear la identidad en el metaproyecto, FALSE en caso contrario</param>
        /// <param name="pMetaOrganizacionID">Identificador de la metaorganización</param>
        /// <param name="pMetaProyectoID">Identificador del metaproyecto</param>
        /// <returns>Perfil</returns>
        public Perfil AgregarPerfilOrganizacion(ServiciosGenerales.Organizacion pOrganizacion, bool pCrearIdentidadEnMetaProyecto, Guid? pMetaOrganizacionID, Guid? pMetaProyectoID, Dictionary<Guid, bool> pRecibirNewsletterDefectoProyectos)
        {
            Perfil perfil = AgregarPerfil(pOrganizacion.FilaOrganizacion.Alias, TiposIdentidad.Organizacion, pOrganizacion.FilaOrganizacion.NombreCorto, pOrganizacion.FilaOrganizacion.Alias, "", pCrearIdentidadEnMetaProyecto, pOrganizacion.Clave, Guid.Empty, pMetaOrganizacionID, pMetaProyectoID, pRecibirNewsletterDefectoProyectos);

            AD.EntityModel.Models.IdentidadDS.PerfilOrganizacion filaPerfilOrg = new AD.EntityModel.Models.IdentidadDS.PerfilOrganizacion();
            filaPerfilOrg.PerfilID = perfil.Clave;
            filaPerfilOrg.OrganizacionID = pOrganizacion.Clave;
            DataWrapperIdentidad.ListaPerfilOrganizacion.Add(filaPerfilOrg);
            mEntityContext.PerfilOrganizacion.Add(filaPerfilOrg);

            return perfil;
        }

        /// <summary>
        /// Agrega EL perfil personal a la persona (UNA persona solo tiene UN perfil personal)
        /// </summary>
        /// <param name="pPersona">Persona a la que se le va a crear el perfil</param>
        /// <returns>Perfil</returns>
        public Perfil AgregarPerfilPersonal(AD.EntityModel.Models.PersonaDS.Persona pPersona, Dictionary<Guid, bool> pRecibirNewsletterDefectoProyectos)
        {
            return AgregarPerfilPersonal(pPersona, true, ProyectoAD.MetaProyecto, ProyectoAD.MetaProyecto, pRecibirNewsletterDefectoProyectos);
        }

        /// <summary>
        /// Agrega EL perfil personal a la persona (UNA persona solo tiene UN perfil personal)
        /// </summary>
        /// <param name="pPersona">Persona a la que se le va a crear el perfil</param>
        /// <param name="pCrearIdentidadEnMetaProyecto">TRUE si debe crear la identidad para ese perfil en el metaproyecto, FALSE en caso contrario</param>
        /// <param name="pMetaOrganizacionID">Identificador de la metaorganización</param>
        /// <param name="pMetaProyectoID">Identificador del metaproyecto</param>
        /// <param name="pCrearAccesoComunidadDidactalia">TRUE si se le debe dar acceso al nuevo perfil a la comunidad de didactalia, FALSE en caso contrario</param>
        /// <returns>Perfil</returns>
        public Perfil AgregarPerfilPersonal(AD.EntityModel.Models.PersonaDS.Persona pPersona, bool pCrearIdentidadEnMetaProyecto, Guid? pMetaOrganizacionID, Guid? pMetaProyectoID, Dictionary<Guid, bool> pRecibirNewsletterDefectoProyectos)
        {
            Perfil perfil = AgregarPerfil($"{pPersona.Nombre} {pPersona.Apellidos}", TiposIdentidad.Personal, string.Empty, string.Empty, pPersona.Usuario.NombreCorto, pCrearIdentidadEnMetaProyecto, Guid.Empty, pPersona.PersonaID, pMetaOrganizacionID, pMetaProyectoID, pRecibirNewsletterDefectoProyectos);

            AD.EntityModel.Models.IdentidadDS.PerfilPersona filaPerfilPers = new AD.EntityModel.Models.IdentidadDS.PerfilPersona();
            filaPerfilPers.PerfilID = perfil.Clave;
            filaPerfilPers.PersonaID = pPersona.PersonaID;
            DataWrapperIdentidad.ListaPerfilPersona.Add(filaPerfilPers);
            mEntityContext.PerfilPersona.Add(filaPerfilPers);
            return perfil;
        }

        /// <summary>
        /// Agrega una fila en PerfilPersonaOrganizacion
        /// </summary>
        ///<param name="pOrganizacionID"></param>
        /// <param name="pPerfilID"></param>
        /// <param name="pPersonaID"></param>
        /// <returns>Perfil</returns>
        public void AgregarFilaPerfilPersonaOrganizacion(Guid pPersonaID, Guid pOrganizacionID, Guid pPerfilID)
        {
            AD.EntityModel.Models.IdentidadDS.PerfilPersonaOrg filaPerfilPersOrg = new AD.EntityModel.Models.IdentidadDS.PerfilPersonaOrg();
            filaPerfilPersOrg.PerfilID = pPerfilID;
            filaPerfilPersOrg.PersonaID = pPersonaID;
            filaPerfilPersOrg.OrganizacionID = pOrganizacionID;
            mEntityContext.PerfilPersonaOrg.Add(filaPerfilPersOrg);
            DataWrapperIdentidad.ListaPerfilPersonaOrg.Add(filaPerfilPersOrg);
        }

        /// <summary>
        /// Agrega un perfil de organización a una persona o lo retoma si estaba dado de baja(eliminado)
        /// </summary>
        /// <param name="pPersona">Persona a la que se le va a generar un perfil nuevo</param>
        /// <param name="pOrganizacion">Organización en la que participa la persona</param>
        /// <param name="pLiveDS">Dataset para el modelo LIVE</param>
        /// <returns>Perfil</returns>
        public Perfil AgregarPerfilPersonaOrganizacion(Persona pPersona, ServiciosGenerales.Organizacion pOrganizacion, LiveDS pLiveDS, Dictionary<Guid, bool> pRecibirNewsletterDefectoProyectos)
        {
            return AgregarPerfilPersonaOrganizacion(pPersona, pOrganizacion, true, null, null, pRecibirNewsletterDefectoProyectos);
        }

        /// <summary>
        /// Agrega un perfil de organización a una persona o lo retoma si estaba dado de baja(eliminado)
        /// </summary>
        /// <param name="pPersona">Persona a la que se le va a generar un perfil nuevo</param>
        /// <param name="pOrganizacion">Organización en la que participa la persona</param>
        /// <param name="pCrearIdentidadEnMetaProyecto">TRUE si debe crear la identidad para dicho perfil en el metaproyecto, FALSE en caso contrario</param>
        /// <param name="pMetaOrganizacionID">Identificador de la metaorganización</param>
        /// <param name="pMetaProyectoID">Identificador del metaproyecto</param>
        /// <param name="pLiveDS">Dataset para el modelo LIVE</param>
        /// <returns>Perfil</returns>
        public Perfil AgregarPerfilPersonaOrganizacion(Persona pPersona, ServiciosGenerales.Organizacion pOrganizacion, bool pCrearIdentidadEnMetaProyecto, Guid? pMetaOrganizacionID, Guid? pMetaProyectoID, Dictionary<Guid, bool> pRecibirNewsletterDefectoProyectos)
        {
            TiposIdentidad tipoPerfil = TiposIdentidad.ProfesionalPersonal;
            string nombreCorto = pPersona.Nombre;
            string nombreLargo = pPersona.NombreConApellidos;

            if (!pOrganizacion.ModoPersonal)
            {
                tipoPerfil = TiposIdentidad.ProfesionalCorporativo;
                nombreCorto = pOrganizacion.Nombre;
            }
            Perfil perfil = AgregarPerfil(nombreLargo, tipoPerfil, pOrganizacion.FilaOrganizacion.NombreCorto, pOrganizacion.FilaOrganizacion.Alias, pPersona.Usuario.FilaUsuario.NombreCorto, pCrearIdentidadEnMetaProyecto, pOrganizacion.Clave, pPersona.Clave, pMetaOrganizacionID, pMetaProyectoID, pRecibirNewsletterDefectoProyectos);

            AD.EntityModel.Models.IdentidadDS.PerfilPersonaOrg filaPerfilPersOrg = new AD.EntityModel.Models.IdentidadDS.PerfilPersonaOrg();
            filaPerfilPersOrg.PerfilID = perfil.Clave;
            filaPerfilPersOrg.PersonaID = pPersona.Clave;
            filaPerfilPersOrg.OrganizacionID = pOrganizacion.Clave;
            mEntityContext.PerfilPersonaOrg.Add(filaPerfilPersOrg);
            DataWrapperIdentidad.ListaPerfilPersonaOrg.Add(filaPerfilPersOrg);
            List<AD.EntityModel.Models.IdentidadDS.Perfil> filasPerfil = DataWrapperIdentidad.ListaPerfil.Where(perf => perf.OrganizacionID.Equals(pOrganizacion.Clave) && !perf.PersonaID.HasValue).ToList();
            this.RecargarHijos();

            if (filasPerfil.Count > 0 && perfil.IdentidadMyGNOSS != null)
            {
                AD.EntityModel.Models.IdentidadDS.Perfil filaPerfil = filasPerfil[0];
                Perfil perfOrg = this.ListaPerfiles[filaPerfil.PerfilID];
                Identidad idenOrg = perfOrg.IdentidadMyGNOSS;
                List<AD.EntityModel.Models.IdentidadDS.GrupoAmigos> filasGrupoAmigos = GestorAmigos.AmigosDW.ListaGrupoAmigos.Where(item => item.IdentidadID.Equals(idenOrg.Clave) && item.Tipo == (int)TipoGrupoAmigos.AutomaticoOrganizacion && item.Automatico).ToList();

                if (filasGrupoAmigos.Count > 0 && mEntityContext.GrupoAmigos.Local.Any(item => item.GrupoID.Equals(filasGrupoAmigos[0].GrupoID) && item.IdentidadID.Equals(filasGrupoAmigos[0].IdentidadID)))
                {
                    GrupoAmigos elementoGrupoAmigos = new GrupoAmigos(filasGrupoAmigos[0], GestorAmigos, mLoggingService);
                    GestorAmigos.AgregarAmigoAGrupo(elementoGrupoAmigos, filasGrupoAmigos[0].IdentidadID, perfil.IdentidadMyGNOSS.Clave);
                }
            }

            return perfil;
        }

        #region Foto

        /// <summary>
        /// Obtiene la foto de una identidad a partir de su organización y su persona.
        /// </summary>
        /// <param name="pPerfil">Perfil</param>
        /// <returns>Foto de una identidad a partir de su tipo, su organización y su persona</returns>
        public static string ObtenerFotoIdentidad(Perfil pPerfil)
        {
            Persona persona = null;
            ServiciosGenerales.Organizacion organizacion = null;
            TiposIdentidad tipoIdent = TiposIdentidad.Personal;

            if (pPerfil.OrganizacionID.HasValue)
            {
                organizacion = pPerfil.OrganizacionPerfil;

                if (pPerfil.PersonaID.HasValue)
                {
                    persona = pPerfil.PersonaPerfil;

                    if (pPerfil.OrganizacionPerfil.ModoPersonal)
                    {
                        tipoIdent = TiposIdentidad.ProfesionalPersonal;
                    }
                    else
                    {
                        tipoIdent = TiposIdentidad.ProfesionalCorporativo;
                    }
                }
                else
                {
                    tipoIdent = TiposIdentidad.Organizacion;
                }
            }
            else
            {
                persona = pPerfil.PersonaPerfil;

                if ((pPerfil.IdentidadMyGNOSS != null) && (pPerfil.IdentidadMyGNOSS.EsIdentidadProfesor))
                {
                    tipoIdent = TiposIdentidad.Profesor;
                }
                else
                {
                    tipoIdent = TiposIdentidad.Personal;
                }
            }

            return ObtenerFotoIdentidad(persona, organizacion, tipoIdent);
        }

        /// <summary>
        /// Obtiene la foto de una identidad a partir de su organización y su persona.
        /// </summary>
        /// <param name="pPersona">Persona</param>
        /// <param name="pOrganizacion">Organización</param>
        /// <returns>Foto de una identidad a partir de su tipo, su organización y su persona</returns>
        public static string ObtenerFotoIdentidad(Persona pPersona, ServiciosGenerales.Organizacion pOrganizacion)
        {
            TiposIdentidad tipoIdentidad = TiposIdentidad.ProfesionalPersonal;

            if (!pOrganizacion.ModoPersonal)
            {
                tipoIdentidad = TiposIdentidad.ProfesionalCorporativo;
            }

            return ObtenerFotoIdentidad(pPersona, pOrganizacion, tipoIdentidad);
        }

        /// <summary>
        /// Obtiene la foto de una identidad a partir de su tipo, su organización y su persona.
        /// </summary>
        /// <param name="pPersona">Persona</param>
        /// <param name="pOrganizacion">Organización</param>
        /// <param name="pTipoIdentidad">Tipo de identidad</param>
        /// <returns>Foto de una identidad a partir de su tipo, su organización y su persona</returns>
        public static string ObtenerFotoIdentidad(Persona pPersona, ServiciosGenerales.Organizacion pOrganizacion, TiposIdentidad pTipoIdentidad)
        {
            string foto = PersonaAD.SIN_IMAGENES_PERSONA;

            if (pTipoIdentidad == TiposIdentidad.Personal || pTipoIdentidad == TiposIdentidad.Profesor)
            {
                if (pPersona.FilaPersona.VersionFoto.HasValue)
                {
                    foto = "/" + UtilArchivos.ContentImagenesPersonas + "/" + pPersona.Clave + IdentidadAD.SUFIJO_PEQUE + "?" + pPersona.FilaPersona.VersionFoto.Value;
                }
            }
            else if (pTipoIdentidad == TiposIdentidad.Organizacion || pTipoIdentidad == TiposIdentidad.ProfesionalCorporativo)
            {
                if (pOrganizacion.FilaOrganizacion.VersionLogo.HasValue)
                {
                    foto = "/" + UtilArchivos.ContentImagenesOrganizaciones + "/" + pOrganizacion.Clave + IdentidadAD.SUFIJO_PEQUE + "?" + pOrganizacion.FilaOrganizacion.VersionLogo;
                }
            }
            else if (pTipoIdentidad == TiposIdentidad.ProfesionalPersonal)
            {
                List<PersonaVinculoOrganizacion> filasPerVinOrg = pOrganizacion.GestorOrganizaciones.OrganizacionDW.ListaPersonaVinculoOrganizacion.Where(item => item.PersonaID.Equals(pPersona.Clave) && item.OrganizacionID.Equals(pOrganizacion.Clave)).ToList();//.Select("PersonaID='" + pPersona.Clave + "' AND OrganizacionID='" + pOrganizacion.Clave + "'");
                if (filasPerVinOrg.Count > 0)
                {
                    if (((PersonaVinculoOrganizacion)filasPerVinOrg[0]).UsarFotoPersonal)
                    {
                        if (pPersona.FilaPersona.VersionFoto.HasValue)
                        {
                            foto = "/" + UtilArchivos.ContentImagenesPersonas + "/" + pPersona.Clave + IdentidadAD.SUFIJO_PEQUE + "?" + pPersona.FilaPersona.VersionFoto.Value;
                        }
                    }
                    else if (filasPerVinOrg.First().VersionFoto.HasValue)
                    {
                        foto = "/" + UtilArchivos.ContentImagenesPersona_Organizacion + "/" + pOrganizacion.Clave + "/" + pPersona.Clave + IdentidadAD.SUFIJO_PEQUE + "?" + (filasPerVinOrg.First()).VersionFoto.Value;
                    }
                }
                else //Pongo foto a NULL ya que no es posible generarla. Se hará a posteriori.
                {
                    foto = null;
                }
            }

            return foto;
        }

        #endregion

        /// <summary>
        /// Agrega una identidad en un proyecto a un perfil
        /// </summary>
        /// <param name="pPerfil">Perfil al que se le va a crear la nueva identidad</param>
        /// <param name="pOrganizacionID">Identificador de la organización del proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Identidad</returns>
        public Identidad AgregarIdentidadPerfil(Perfil pPerfil, Guid pOrganizacionID, Guid pProyectoID, Dictionary<Guid, bool> pRecibirNewsletterDefectoProyectos)
        {
            AD.EntityModel.Models.IdentidadDS.Identidad filaIdentidad = new AD.EntityModel.Models.IdentidadDS.Identidad();
            filaIdentidad.IdentidadID = Guid.NewGuid();
            filaIdentidad.FechaAlta = DateTime.Now;
            filaIdentidad.FechaBaja = null;
            filaIdentidad.OrganizacionID = pOrganizacionID;
            filaIdentidad.ProyectoID = pProyectoID;
            filaIdentidad.PerfilID = pPerfil.Clave;
            filaIdentidad.NumConnexiones = 0;
            filaIdentidad.RecibirNewsLetter = true;
            if (pRecibirNewsletterDefectoProyectos != null)
            {
                if (pRecibirNewsletterDefectoProyectos.ContainsKey(pProyectoID))
                {
                    //Si se especifica en ese proyecto se coge de ahí
                    filaIdentidad.RecibirNewsLetter = pRecibirNewsletterDefectoProyectos[pProyectoID];
                }
                else if (pRecibirNewsletterDefectoProyectos.ContainsKey(Guid.Empty))
                {
                    //Si no se especifica en el proyecto pero se especifica en el ecosistema lo carga del ecosistema
                    filaIdentidad.RecibirNewsLetter = pRecibirNewsletterDefectoProyectos[Guid.Empty];
                }
            }
            filaIdentidad.MostrarBienvenida = true;
            filaIdentidad.DiasUltActualizacion = 0;
            filaIdentidad.ValorAbsoluto = 1;
            filaIdentidad.Rank = 0;
            filaIdentidad.ActualizaHome = true;
            filaIdentidad.ActivoEnComunidad = true;

            if (pPerfil.OrganizacionID.HasValue)
            {
                if (pPerfil.PersonaID.HasValue)
                {
                    if (pPerfil.OrganizacionPerfil.ModoPersonal)
                    {
                        filaIdentidad.Tipo = (short)TiposIdentidad.ProfesionalPersonal;
                        filaIdentidad.NombreCortoIdentidad = pPerfil.PersonaPerfil.Nombre;
                    }
                    else
                    {
                        filaIdentidad.Tipo = (short)TiposIdentidad.ProfesionalCorporativo;
                        filaIdentidad.NombreCortoIdentidad = pPerfil.NombreOrganizacion;
                    }
                }
                else
                {
                    filaIdentidad.Tipo = (short)TiposIdentidad.Organizacion;
                    filaIdentidad.NombreCortoIdentidad = pPerfil.NombreOrganizacion;
                }
            }
            else
            {
                if ((pPerfil.IdentidadMyGNOSS != null) && (pPerfil.IdentidadMyGNOSS.EsIdentidadProfesor))
                {
                    filaIdentidad.Tipo = (short)TiposIdentidad.Profesor;
                    filaIdentidad.NombreCortoIdentidad = pPerfil.IdentidadMyGNOSS.FilaIdentidad.NombreCortoIdentidad;
                }
                else
                {
                    filaIdentidad.Tipo = (short)TiposIdentidad.Personal;
                    filaIdentidad.NombreCortoIdentidad = pPerfil.PersonaPerfil.Nombre;
                }
            }

            string foto = ObtenerFotoIdentidad(pPerfil);

            if (!string.IsNullOrEmpty(foto))
            {
                filaIdentidad.Foto = foto;
            }
            else
            {
                filaIdentidad.Foto = PersonaAD.SIN_IMAGENES_PERSONA;
            }

            DataWrapperIdentidad.ListaIdentidad.Add(filaIdentidad);
            mEntityContext.Identidad.Add(filaIdentidad);

            AD.EntityModel.Models.IdentidadDS.IdentidadContadores filaContadores = new AD.EntityModel.Models.IdentidadDS.IdentidadContadores();
            filaContadores.IdentidadID = filaIdentidad.IdentidadID;
            filaContadores.NumeroDescargas = 0;
            filaContadores.NumeroVisitas = 0;
            filaContadores.Identidad = filaIdentidad;
            filaIdentidad.IdentidadContadores = filaContadores;
            pPerfil.FilaPerfil.Identidad.Add(filaIdentidad);            
            DataWrapperIdentidad.ListaIdentidadContadores.Add(filaContadores);
            mEntityContext.IdentidadContadores.Add(filaContadores);

            Identidad identidad = new Identidad(filaIdentidad, pPerfil, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

            if (!this.ListaIdentidades.ContainsKey(identidad.Clave))
            {
                this.ListaIdentidades.Add(identidad.Clave, identidad);
            }

            if (!pPerfil.Hijos.Contains(identidad))
            {
                pPerfil.Hijos.Add(identidad);
            }
            return identidad;
        }

        /// <summary>
        /// Agrega una identidad de tipo profesor a un perfil.
        /// </summary>
        /// <param name="pPerfil">Perfil al que se le va a crear la nueva identidad</param>
        /// <returns>Identidad</returns>
        public Identidad AgregarIdentidadPerfilProfesor(Perfil pPerfil, Dictionary<Guid, bool> pRecibirNewsletterDefectoProyectos)
        {
            Identidad identidad = AgregarIdentidadPerfil(pPerfil, ProyectoAD.MetaOrganizacion, ProyectoAD.MetaProyecto, pRecibirNewsletterDefectoProyectos);
            identidad.FilaIdentidad.Tipo = (short)TiposIdentidad.Profesor;
            return identidad;
        }

        #endregion

        #region Privados

        /// <summary>
        /// Agrega un perfil a la Base de datos (de persona o de organización)
        /// </summary>
        /// <param name="pNombreCompletoPerfil">Nombre completo del perfil</param>
        /// <param name="pTipoPerfil">Tipo del perfil</param>
        /// <param name="pNombreCortoOrg">Nombre corto de organizacion</param>
        /// <param name="pNombreOrganizacion">Nombre de organización</param>
        /// <param name="pNombreCortoUsu">Nombre corto de usuario</param>
        /// <param name="pOrganizacionID">ID de organización del perfil</param>
        /// <param name="pPersonaID">ID de la persona del perfil</param>
        /// <returns>Perfil</returns>
        private Perfil AgregarPerfil(string pNombreCompletoPerfil, TiposIdentidad pTipoPerfil, string pNombreCortoOrg, string pNombreOrganizacion, string pNombreCortoUsu, Guid pOrganizacionID, Guid pPersonaID, Dictionary<Guid, bool> pRecibirNewsletterDefectoProyectos)
        {
            return AgregarPerfil(pNombreCompletoPerfil, pTipoPerfil, pNombreCortoOrg, pNombreOrganizacion, pNombreCortoUsu, true, pOrganizacionID, pPersonaID, ProyectoAD.MetaOrganizacion, ProyectoAD.MetaProyecto, pRecibirNewsletterDefectoProyectos);
        }

        /// <summary>
        /// Agrega un perfil a la Base de datos (de persona o de organización)
        /// </summary>
        /// <param name="pNombreCompletoPerfil">Nombre completo del perfil</param>
        /// <param name="pTipoPerfil">Tipo del perfil</param>
        /// <param name="pNombreCortoOrg">Nombre corto de organización</param>
        /// <param name="pNombreOrganizacion">Nombre de organización</param>
        /// <param name="pNombreCortoUsu">Nombre corto de usuario</param>
        /// <param name="pCrearIdentidadEnMetaProyecto">TRUE si queremos crear la identidad para ese perfil en el metaproyecto, FALSE en caso contrario</param>
        /// <param name="pCrearAccesoComunidadFAQ">TRUE si se le debe dar acceso al nuevo perfil a la comunidad de FAQs, FALSE en caso contrario</param>
        /// <param name="pOrganizacionID">Identificador de organización del perfil</param>
        /// <param name="pPersonaID">Identificador de la persona del perfil</param>
        /// <param name="pMetaOrganizacionID">Identificador de la metaorganización</param>
        /// <param name="pMetaProyectoID">Identificador del metaproyecto</param>
        /// <param name="pCrearAccesoComunidadDidactalia">TRUE si se le debe dar acceso al nuevo perfil a la comunidad de didactalia, FALSE en caso contrario</param>
        /// <returns>Perfil</returns>
        private Perfil AgregarPerfil(string pNombreCompletoPerfil, TiposIdentidad pTipoPerfil, string pNombreCortoOrg, string pNombreOrganizacion, string pNombreCortoUsu, bool pCrearIdentidadEnMetaProyecto, Guid pOrganizacionID, Guid pPersonaID, Guid? pMetaOrganizacionID, Guid? pMetaProyectoID, Dictionary<Guid, bool> pRecibirNewsletterDefectoProyectos)
        {
            AD.EntityModel.Models.IdentidadDS.Perfil filaPerfil = new AD.EntityModel.Models.IdentidadDS.Perfil();
            filaPerfil.PerfilID = Guid.NewGuid();
            filaPerfil.NombrePerfil = pNombreCompletoPerfil;
            filaPerfil.Eliminado = false;
            filaPerfil.CaducidadResSusc = 7;

            if (string.IsNullOrEmpty(pNombreCortoOrg))
            {
                filaPerfil.NombreCortoOrg = null;
            }
            else
            {
                filaPerfil.NombreCortoOrg = pNombreCortoOrg;
            }

            if (string.IsNullOrEmpty(pNombreCortoUsu))
            {
                filaPerfil.NombreCortoUsu = null;
            }
            else
            {
                filaPerfil.NombreCortoUsu = pNombreCortoUsu;
            }
            if (string.IsNullOrEmpty(pNombreOrganizacion))
            {
                filaPerfil.NombreOrganizacion = null;
            }
            else
            {
                filaPerfil.NombreOrganizacion = pNombreOrganizacion;
            }

            if (pPersonaID != Guid.Empty)
            {
                filaPerfil.PersonaID = pPersonaID;
            }

            if (pOrganizacionID != Guid.Empty)
            {
                filaPerfil.OrganizacionID = pOrganizacionID;
            }
            filaPerfil.TieneTwitter = false;
   
            DataWrapperIdentidad.ListaPerfil.Add(filaPerfil);
            mEntityContext.Perfil.Add(filaPerfil);
            Perfil nuevoPerfil = new Perfil(filaPerfil, this, mLoggingService);

            if (!ListaPerfiles.ContainsKey(nuevoPerfil.Clave))
            {
                this.ListaPerfiles.Add(nuevoPerfil.Clave, nuevoPerfil);
            }

            if (pCrearIdentidadEnMetaProyecto && pMetaOrganizacionID != null && pMetaProyectoID != null)
            {
                Identidad objetoIdentidad = AgregarIdentidadPerfil(nuevoPerfil, (Guid)pMetaOrganizacionID, (Guid)pMetaProyectoID, pRecibirNewsletterDefectoProyectos);
                objetoIdentidad.Tipo = pTipoPerfil;
            }
            return nuevoPerfil;
        }

        #endregion

        #endregion

        /// <summary>
        /// Obtiene las identidades de un CV
        /// </summary>
        /// <param name="pCvID">Identificador de currículum</param>
        /// <returns></returns>
        public List<Identidad> ObtenerIdentidadesDeCV(Guid pCvID)
        {
            List<Identidad> lista = new List<Identidad>();//"CurriculumID = '" + pCvID.ToString() + "'"
            List<AD.EntityModel.Models.IdentidadDS.Identidad> filasIdentidad = DataWrapperIdentidad.ListaIdentidad.Where(identidad => identidad.CurriculumID.HasValue && identidad.CurriculumID.Value.Equals(pCvID)).ToList();

            foreach (AD.EntityModel.Models.IdentidadDS.Identidad filaIdent in filasIdentidad)
            {
                lista.Add(ListaIdentidades[filaIdent.IdentidadID]);
            }
            return lista;
        }

        /// <summary>
        /// Obtiene el perfil de una persona en una organización
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <returns></returns>
        public Perfil ObtenerPerfilDePersonaEnOrganizacion(Guid pOrganizacionID, Guid pPersonaID)
        {
            if (DataWrapperIdentidad != null)
            {
                AD.EntityModel.Models.IdentidadDS.PerfilPersonaOrg fila = DataWrapperIdentidad.ListaPerfilPersonaOrg.Where(item => item.OrganizacionID.Equals(pOrganizacionID) && item.PersonaID.Equals(pPersonaID)).FirstOrDefault();

                if (fila != null && ListaPerfiles.ContainsKey(fila.PerfilID))
                {
                    return ListaPerfiles[fila.PerfilID];
                }
            }
            return null;
        }

        /// <summary>
        /// Carga los hijos
        /// </summary>
        private void CargarHijos()
        {
            mHijos = new List<IElementoGnoss>();
            mListaPerfiles = new SortedList<Guid, Perfil>();
            mListaIdentidades = new SortedList<Guid, Identidad>();

            foreach (AD.EntityModel.Models.IdentidadDS.Perfil filaPerfil in this.DataWrapperIdentidad.ListaPerfil)
            {
                if (!mListaPerfiles.ContainsKey(filaPerfil.PerfilID))
                {
                    Perfil perfil = new Perfil(filaPerfil, this, mLoggingService);

                    foreach (AD.EntityModel.Models.IdentidadDS.Identidad filaIdentidad in this.DataWrapperIdentidad.ListaIdentidad.Where(identidad => identidad.PerfilID.Equals(filaPerfil.PerfilID)))
                    {
                        if (!mListaIdentidades.ContainsKey(filaIdentidad.IdentidadID))
                        {
                            if (filaIdentidad.Perfil == null)
                            {
                                filaIdentidad.Perfil = perfil.FilaPerfil;
                            }

                            Identidad identidad = new Identidad(filaIdentidad, perfil, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

                            mListaIdentidades.Add(identidad.Clave, identidad);
                            perfil.Hijos.Add(identidad);
                        }
                    }
                    this.mHijos.Add(perfil);
                    this.mListaPerfiles.Add(perfil.Clave, perfil);
                }
            }
        }

        /// <summary>
        /// Carga los hijos
        /// </summary>
        public void CargarGrupos()
        {
            mListaGrupos = new SortedList<Guid, GrupoIdentidades>();

            foreach (AD.EntityModel.Models.IdentidadDS.GrupoIdentidades filaGrupoIdentidades in this.DataWrapperIdentidad.ListaGrupoIdentidades)
            {
                if (!mListaGrupos.ContainsKey(filaGrupoIdentidades.GrupoID))
                {
                    GrupoIdentidades grupo = new GrupoIdentidades(filaGrupoIdentidades, this, mLoggingService);

                    this.mListaGrupos.Add(grupo.Clave, grupo);
                }
            }
        }

        /// <summary>
        /// Recarga los hijos de nuevo
        /// </summary>
        public void RecargarHijos()
        {
            CargarHijos();
        }

        /// <summary>
        /// Verdad si la identidad es visible a externos
        /// </summary>
        /// <param name="pIdentidad">Identidad</param>
        /// <returns></returns>
        public bool EsIdentidadVisibleExternos(Identidad pIdentidad)
        {
            return EsIdentidadVisibleExternos(pIdentidad.Clave);
        }

        /// <summary>
        /// Verdad si la identidad es visible a externos
        /// </summary>
        /// <param name="pIdentidadID">Clave de la identidad</param>
        /// <returns></returns>
        public bool EsIdentidadVisibleExternos(Guid pIdentidadID)
        {
            bool visible = false;

            if (ListaIdentidadesVisiblesExternos.ContainsKey(pIdentidadID))
            {
                visible = ListaIdentidadesVisiblesExternos[pIdentidadID];
            }
            return visible;
        }

        /// <summary>
        /// Obtiene el identificador de identidad de un perfil en un proyecto
        /// </summary>
        /// <param name="pPerfilID">Identificador de perfil</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>Devuelve el guid de la identidad o un guid vacío si no se ha encontrado</returns>
        public Guid ObtenerIdentidadDePerfilEnProyecto(Guid pPerfilID, Guid pProyectoID)
        {//"PerfilID='"+pPerfilID+"' AND ProyectoID='"+pProyectoID+"'"
            AD.EntityModel.Models.IdentidadDS.Identidad filasIdent = DataWrapperIdentidad.ListaIdentidad.FirstOrDefault(identidad => identidad.PerfilID.Equals(pPerfilID) && identidad.ProyectoID.Equals(pProyectoID));

            if (filasIdent != null)
            {
                return filasIdent.IdentidadID;
            }
            else
            {
                return Guid.Empty;
            }
        }

        /// <summary>
        /// Obtiene el identificador de identidad de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <param name="pPersonaID">Identificador de la persona que posee la identidad</param>
        /// <returns>Devuelve el guid de la identidad o un guid vacío si no se ha encontrado</returns>
        public Guid ObtenerIdentidadDeProyecto(Guid pProyectoID, Guid pPersonaID)
        {
            if (pProyectoID == ProyectoAD.MyGnoss)
            {
                foreach (Identidad ident in ListaIdentidades.Values)
                {
                    if (ident.FilaIdentidad.ProyectoID == pProyectoID && !ident.TrabajaConOrganizacion && ident.PerfilUsuario.PersonaID != null && ident.PerfilUsuario.PersonaID.Value.Equals(pPersonaID))
                    {
                        return ident.Clave;
                    }
                }
            }
            else
            {
                List<AD.EntityModel.Models.IdentidadDS.Perfil> perfiles = DataWrapperIdentidad.ListaPerfil.Where(perfil => perfil.PersonaID.Equals(pPersonaID)).ToList();

                if (perfiles != null && perfiles.Count > 0)
                {
                    List<Guid> listaPerfilesID = perfiles.Select(perfi => perfi.PerfilID).ToList();
                    List<AD.EntityModel.Models.IdentidadDS.Identidad> identidades = DataWrapperIdentidad.ListaIdentidad.Where(identidad => identidad.ProyectoID.Equals(pProyectoID) && listaPerfilesID.Contains(identidad.PerfilID)).ToList();

                    if ((identidades != null) && (identidades.Count > 0))
                    {
                        return identidades[0].IdentidadID;
                    }
                }
            }
            return Guid.Empty;
        }

        /// <summary>
        /// Obtiene el identificador de identidad de un proyecto ACTIVA.
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>Devuelve el guid de la identidad o un guid vacío si no se ha encontrado</returns>
        public Guid ObtenerIdentidadDeProyectoActiva(Guid pProyectoID, Guid pPersonaID)
        {
            if (pProyectoID == ProyectoAD.MetaProyecto)
            {
                foreach (Identidad ident in ListaIdentidades.Values)
                {
                    if (ident.FilaIdentidad.ProyectoID == pProyectoID && !ident.TrabajaConOrganizacion && ident.PerfilUsuario.PersonaID != null && ident.PerfilUsuario.PersonaID.Value.Equals(pPersonaID))
                    {
                        if (!ident.FilaIdentidad.FechaBaja.HasValue)
                        {
                            return ident.Clave;
                        }
                    }
                }
            }
            else
            {
                foreach (Identidad ident in ListaIdentidades.Values)
                {
                    if (ident.FilaIdentidad.ProyectoID == pProyectoID && ident.PerfilUsuario.PersonaID != null && ident.PerfilUsuario.PersonaID.Value.Equals(pPersonaID))
                    {
                        if (!ident.FilaIdentidad.FechaBaja.HasValue)
                        {
                            return ident.Clave;
                        }
                    }
                }
            }
            return Guid.Empty;
        }

        /// <summary>
        /// Obtiene el identificador de identidad de un proyecto NO ACTIVA.
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>Devuelve el guid de la identidad o un guid vacío si no se ha encontrado</returns>
        public Guid ObtenerIdentidadDeProyectoNOActiva(Guid pProyectoID, Guid pPersonaID)
        {
            if (pProyectoID == ProyectoAD.MetaProyecto)
            {
                foreach (Identidad ident in ListaIdentidades.Values)
                {
                    if (ident.FilaIdentidad.ProyectoID == pProyectoID && !ident.TrabajaConOrganizacion && ident.PerfilUsuario.PersonaID != null && ident.PerfilUsuario.PersonaID.Value.Equals(pPersonaID))
                    {
                        if (ident.FilaIdentidad.FechaBaja.HasValue)
                        {
                            return ident.Clave;
                        }
                    }
                }
            }
            else
            {
                foreach (Identidad ident in ListaIdentidades.Values)
                {
                    if (ident.FilaIdentidad.ProyectoID == pProyectoID && ident.PerfilUsuario.PersonaID != null && ident.PerfilUsuario.PersonaID.Value.Equals(pPersonaID))
                    {
                        if (ident.FilaIdentidad.FechaBaja.HasValue)
                        {
                            return ident.Clave;
                        }
                    }
                }
            }
            return Guid.Empty;
        }

        /// <summary>
        /// Elimina un PerfilPersonaOrg totalmente de memoria (si era nuevo) o lo marca como eliminado (si era viejo, estaba en BD) 
        /// </summary>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        public void EliminarPerfilPersonaOrganizacion(Guid pPersonaID, Guid pOrganizacionID)
        {
            if (DataWrapperIdentidad.ListaPerfil.Any(item => item.OrganizacionID.Equals(pOrganizacionID) && item.PersonaID.Equals(pPersonaID)))
            {
                AD.EntityModel.Models.IdentidadDS.Perfil filaPerfil = DataWrapperIdentidad.ListaPerfil.Where(item => item.OrganizacionID.Equals(pOrganizacionID) && item.PersonaID.Equals(pPersonaID)).FirstOrDefault();

                AD.EntityModel.Models.IdentidadDS.PerfilPersonaOrg filaPerfilPersonaOrg = DataWrapperIdentidad.ListaPerfilPersonaOrg.Where(item => item.OrganizacionID.Equals(pOrganizacionID) && item.PersonaID.Equals(pPersonaID)).FirstOrDefault();

                //Si estan recien añadidas las borro
                if (filaPerfilPersonaOrg != null && mEntityContext.Entry(filaPerfilPersonaOrg).State.Equals(DataRowState.Added))
                {
                    Guid perfilID = filaPerfilPersonaOrg.PerfilID;

                    //Borro PerfilPersonaOrg
                    mEntityContext.EliminarElemento(filaPerfilPersonaOrg);
                    DataWrapperIdentidad.ListaPerfilPersonaOrg.Remove(filaPerfilPersonaOrg);

                    //Borro Perfil
                    AD.EntityModel.Models.IdentidadDS.Perfil perfilBorrar = DataWrapperIdentidad.ListaPerfil.FirstOrDefault(perf => perf.PerfilID.Equals(perfilID));
                    mEntityContext.EliminarElemento(perfilBorrar);
                    DataWrapperIdentidad.ListaPerfil.Remove(perfilBorrar);
                }
                //Si estaban guardadas en la base de datos no lo borro pero tendre que actualizar eliminado-->1
                else
                {
                    filaPerfil.Eliminado = true;
                }
            }
        }

        /// <summary>
        /// Elimina un PerfilOrganizacion totalmente de memoria (si era nuevo) o lo marca como eliminado (si era viejo, estaba en BD) 
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        public void EliminarPerfilOrganizacion(Guid pOrganizacionID)
        {
            if (DataWrapperIdentidad.ListaPerfilOrganizacion.Any(item => item.OrganizacionID.Equals(pOrganizacionID)))
            {
                AD.EntityModel.Models.IdentidadDS.PerfilOrganizacion fila = DataWrapperIdentidad.ListaPerfilOrganizacion.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).FirstOrDefault();

                //Si estan recien añadidas las borro
                if (mEntityContext.Entry(fila).State.Equals(DataRowState.Added))
                {
                    Guid perfilID = fila.PerfilID;

                    //Borro PerfilOrganizacion
                    AD.EntityModel.Models.IdentidadDS.PerfilOrganizacion perfilOrganizacion = this.DataWrapperIdentidad.ListaPerfilOrganizacion.Where(item => item.PerfilID.Equals(perfilID)).FirstOrDefault();

                    mEntityContext.EliminarElemento(perfilOrganizacion);
                    DataWrapperIdentidad.ListaPerfilOrganizacion.Remove(perfilOrganizacion);

                    //Borro Perfil
                    AD.EntityModel.Models.IdentidadDS.Perfil perfilBorrar = this.DataWrapperIdentidad.ListaPerfil.FirstOrDefault(perfil => perfil.PerfilID.Equals(perfilID));
                    mEntityContext.EliminarElemento(perfilBorrar);
                    DataWrapperIdentidad.ListaPerfil.Remove(perfilBorrar);
                }
                //Si estaban guardadas en la base de datos no lo borro pero tendre que actualizar eliminado-->1
                else
                {
                    Guid perfilID = fila.PerfilID;
                    AD.EntityModel.Models.IdentidadDS.Perfil filaPerfil = this.DataWrapperIdentidad.ListaPerfil.FirstOrDefault(perfil => perfil.PerfilID.Equals(perfilID));
                    filaPerfil.Eliminado = true;
                    //"OrganizacionID = '" + pOrganizacionID + "' AND PersonaID IS NULL"
                    List<AD.EntityModel.Models.IdentidadDS.Perfil> filasPerfil = DataWrapperIdentidad.ListaPerfil.Where(perfil => perfil.OrganizacionID.Equals(pOrganizacionID) && !perfil.PersonaID.HasValue).ToList();

                    if (filasPerfil.Count > 0)
                    {
                        AD.EntityModel.Models.IdentidadDS.Perfil filaPerfilOrg = filasPerfil.First();
                        Perfil perfOrg = this.ListaPerfiles[filaPerfilOrg.PerfilID];
                        Identidad idenOrg = perfOrg.IdentidadMyGNOSS;
                        
                        AD.EntityModel.Models.IdentidadDS.GrupoAmigos grupoAmigo = GestorAmigos.AmigosDW.ListaGrupoAmigos.Where(item => item.IdentidadID.Equals(idenOrg.Clave) && item.Tipo.Equals((int)TipoGrupoAmigos.AutomaticoOrganizacion) && item.Automatico).FirstOrDefault();

                        if (grupoAmigo != null)
                        {
                            GestorAmigos.AmigosDW.ListaGrupoAmigos.Remove(grupoAmigo);
                            mEntityContext.GrupoAmigos.Remove(grupoAmigo);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Elimina un PerfilPersonal totalmente de memoria (si era nuevo) o lo marca como eliminado (si era viejo, estaba en BD) 
        /// </summary>
        /// <param name="pPersonaID">Identificador de persona</param>
        public void EliminarPerfilPersonal(Guid pPersonaID)
        {
            AD.EntityModel.Models.IdentidadDS.PerfilPersona perfilPersona = DataWrapperIdentidad.ListaPerfilPersona.FirstOrDefault(perfilPers => perfilPers.PersonaID.Equals(pPersonaID));
            if (perfilPersona != null)
            {
                //Si estan recien añadidas las borro
                if (mEntityContext.Entry(perfilPersona).State == EntityState.Added)
                {
                    Guid perfilID = perfilPersona.PerfilID;

                    //Borro PerfilPersonal
                    this.DataWrapperIdentidad.ListaPerfilPersona.Remove(perfilPersona);
                    mEntityContext.EliminarElemento(perfilPersona);
                    //Borro Perfil
                    AD.EntityModel.Models.IdentidadDS.Perfil perfil = DataWrapperIdentidad.ListaPerfil.Find(ident => ident.PerfilID.Equals(perfilID));
                    this.DataWrapperIdentidad.ListaPerfil.Remove(perfil);
                    mEntityContext.EliminarElemento(perfil);
                }
                //Si estaban guardadas en la base de datos no lo borro pero tendre q actualizar eliminado-->1
                else
                {
                    Guid perfilID = perfilPersona.PerfilID;
                    AD.EntityModel.Models.IdentidadDS.Perfil filaPerfil = this.DataWrapperIdentidad.ListaPerfil.Find(perfil => perfil.PerfilID.Equals(perfilID));
                    filaPerfil.Eliminado = true;
                }
            }
        }

        /// <summary>
        /// Retoma una identidad actualizando fecha de Alta/Baja 
        /// </summary>
        /// <param name="pIdentidad">Identidad</param>
        public void RetomarIdentidadPerfil(Identidad pIdentidad)
        {
            AD.EntityModel.Models.IdentidadDS.Identidad filaIdentidad = pIdentidad.FilaIdentidad;
            filaIdentidad.FechaAlta = DateTime.Now;
            filaIdentidad.FechaBaja = null;

            if (filaIdentidad.FechaExpulsion.HasValue)
            {
                filaIdentidad.FechaExpulsion = null;
            }//"OrganizacionID = '" + pIdentidad.OrganizacionID + "' AND PersonaID IS NULL"
            List<AD.EntityModel.Models.IdentidadDS.Perfil> filasPerfil = DataWrapperIdentidad.ListaPerfil.Where(perfil => perfil.OrganizacionID.Equals(pIdentidad.OrganizacionID) && !perfil.PersonaID.HasValue).ToList();

            if (filasPerfil.Count > 0)
            {
                AD.EntityModel.Models.IdentidadDS.Perfil filaPerfil = filasPerfil.First();
                Perfil perfOrg = this.ListaPerfiles[filaPerfil.PerfilID];
                Identidad idenOrg = perfOrg.IdentidadMyGNOSS;
                if (GestorAmigos != null)
                {
                    AD.EntityModel.Models.IdentidadDS.GrupoAmigos grupoAmigo = GestorAmigos.AmigosDW.ListaGrupoAmigos.Where(item => item.IdentidadID.Equals(idenOrg.Clave) && item.Tipo == (int)TipoGrupoAmigos.AutomaticoOrganizacion && item.Automatico).FirstOrDefault();

                    if (grupoAmigo != null)
                    {
                        GrupoAmigos grupoAmigos = new GrupoAmigos(grupoAmigo, GestorAmigos, mLoggingService);
                        if(grupoAmigos.FilaGrupoAmigos != null)
                        {
                            GestorAmigos.AgregarAmigoAGrupo(grupoAmigos, grupoAmigos.FilaGrupoAmigos.IdentidadID, pIdentidad.PerfilUsuario.IdentidadMyGNOSS.Clave);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Retoma una identidad actualizando fecha de Alta/Baja 
        /// </summary>
        /// <param name="pIdentidadID">Identificador de identidad</param>
        public void RetomarIdentidadPerfil(Guid pIdentidadID)
        {
            AD.EntityModel.Models.IdentidadDS.Identidad filaIdentidad = this.DataWrapperIdentidad.ListaIdentidad.Find(identidad => identidad.IdentidadID.Equals(pIdentidadID));
            Perfil perfil = ListaPerfiles[filaIdentidad.PerfilID];
            Identidad ident = new Identidad(filaIdentidad, perfil, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

            if (!ListaIdentidades.ContainsKey(ident.Clave))
            {
                ListaIdentidades.Add(ident.Clave, ident);
            }
            RetomarIdentidadPerfil(ident);
        }

        /// <summary>
        /// Elimina de memoria si estaban añadidos (Added) o marca como eliminados en base de datos cada uno de los perfiles de la persona
        /// </summary>
        /// <param name="pPersona">Persona</param>
        public void EliminarTodosPerfiles(Persona pPersona)
        {
            //Elimino cada perfil persona+organizacion
            List<AD.EntityModel.Models.IdentidadDS.PerfilPersonaOrg> listaPerfilPersona = this.DataWrapperIdentidad.ListaPerfilPersonaOrg.Where(item => item.PersonaID.Equals(pPersona.Clave)).ToList();
            foreach (AD.EntityModel.Models.IdentidadDS.PerfilPersonaOrg fila in listaPerfilPersona)
            {
                EliminarPerfilPersonaOrganizacion(pPersona.Clave, fila.OrganizacionID);
            }
            //Elimino el perfil personal
            EliminarPerfilPersonal(pPersona.Clave);
        }

        /// <summary>
        /// Elimina de memoria si estaban añadidos (Added) o establece fecha de baja en base de datos cada uno de las identidades de la persona
        /// </summary>
        /// <param name="pPersona">Persona</param>
        public void EliminarTodasIdentidades(Persona pPersona)
        {
            //Elimino cada perfil persona+organizacion
            List<AD.EntityModel.Models.IdentidadDS.PerfilPersonaOrg> listaPerfilPersona = this.DataWrapperIdentidad.ListaPerfilPersonaOrg.Where(item => item.PersonaID.Equals(pPersona.Clave)).ToList();
            foreach (AD.EntityModel.Models.IdentidadDS.PerfilPersonaOrg fila in listaPerfilPersona)
            {
                EliminarIdentidadesAsociadasAPerfil(fila.PerfilID, true);
            }

            //Elimino el perfil personal
            AD.EntityModel.Models.IdentidadDS.PerfilPersona perfilPersona = DataWrapperIdentidad.ListaPerfilPersona.FirstOrDefault(perfilPers => perfilPers.PersonaID.Equals(pPersona.Clave));
            if (perfilPersona != null)
            {
                EliminarIdentidadesAsociadasAPerfil(perfilPersona.PerfilID, true);
            }
        }

        /// <summary>
        /// Elimina las identidades asociadas al perfil pasado por parámetro (y si queremos los los amigos/contactos que pueda tener con dicho perfi)
        /// </summary>
        /// <param name="pPerfilID">Identificador de perfil</param>
        /// <param name="pEliminarAmigos">True si eliminamos los amigos/contactos que pueda tener con dicho perfil</param>
        public void EliminarIdentidadesAsociadasAPerfil(Guid pPerfilID, bool pEliminarAmigos)
        {//"PerfilID = '" + pPerfilID + "'"
            List<AD.EntityModel.Models.IdentidadDS.Identidad> listaIdentidades = DataWrapperIdentidad.ListaIdentidad.Where(ident => ident.PerfilID.Equals(pPerfilID)).ToList();
            if (listaIdentidades.Count > 0)
            {
                foreach (AD.EntityModel.Models.IdentidadDS.Identidad filaIdentidad in listaIdentidades)
                {
                    if (pEliminarAmigos)
                    {
                        GestorAmigos.EliminarAmigosReciproco(filaIdentidad.IdentidadID);
                    }

                    //Si estan recien añadidas las borro
                    if (mEntityContext.Entry(filaIdentidad).State == EntityState.Added)
                    {
                        Guid identidadID = filaIdentidad.IdentidadID;
                        //Borro Identidad
                        this.DataWrapperIdentidad.ListaIdentidad.Remove(filaIdentidad);
                        mEntityContext.EliminarElemento(filaIdentidad);
                    }
                    //Si estaban guardadas en la base de datos no lo borro pero tendre q actualizar fechabaja
                    else
                    {
                        Guid identidadID = filaIdentidad.IdentidadID;
                        filaIdentidad.FechaBaja = DateTime.Now;
                    }
                }
            }
        }

        /// <summary>
        /// Elimina la identidad (si estaba Added) o le actualiza la fecha de baja
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad a eliminar</param>
        public void EliminarIdentidad(Guid pIdentidadID)
        {
            AD.EntityModel.Models.IdentidadDS.Identidad identidadEliminar = DataWrapperIdentidad.ListaIdentidad.FirstOrDefault(identidad => identidad.IdentidadID.Equals(pIdentidadID));
            if (identidadEliminar != null)
            {
                //Si esta recien añadida la borro
                if (mEntityContext.Entry(identidadEliminar).State == EntityState.Added)
                {
                    //Borro Identidad
                    mEntityContext.EliminarElemento(identidadEliminar);
                    DataWrapperIdentidad.ListaIdentidad.Remove(identidadEliminar);
                }
                //Si estaban guardadas en la base de datos no lo borro pero tendre q actualizar fechabaja
                else
                {
                    identidadEliminar.FechaBaja = DateTime.Now;
                }
            }
        }

        /// <summary>
        /// Cambia el modo de participación de una identidad dentro de una comunidad
        /// </summary>
        /// <param name="pIdentidad"></param>
        /// <param name="pModo"></param>
        /// <returns>TRUE si cambia el modo, FALSE si no</returns>
        public bool CambiarModoParticipacionIdentidad(Identidad pIdentidadProyecto, Identidad pIdentidadMetaProyecto, Identidad pIdentidadOrganizacionMetaProyecto, TiposIdentidad pModo)
        {
            if ((short)pModo != pIdentidadProyecto.FilaIdentidad.Tipo)
            {
                if (pModo == TiposIdentidad.ProfesionalCorporativo)
                {
                    pIdentidadProyecto.FilaIdentidad.NombreCortoIdentidad = pIdentidadProyecto.OrganizacionPerfil.FilaOrganizacion.Alias;
                    pIdentidadProyecto.FilaIdentidad.Foto = pIdentidadOrganizacionMetaProyecto.FilaIdentidad.Foto;
                }
                else
                {
                    pIdentidadProyecto.FilaIdentidad.NombreCortoIdentidad = pIdentidadProyecto.PerfilUsuario.PersonaPerfil.Nombre;
                    pIdentidadProyecto.FilaIdentidad.Foto = pIdentidadMetaProyecto.FilaIdentidad.Foto;
                }
                pIdentidadProyecto.FilaIdentidad.Tipo = (short)pModo;

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Crea un perfil de profesor para un usuario existente
        /// </summary>
        /// <param name="pPerfilNombre">Nombre del perfl</param>
        /// <param name="pIdentidadNombreCorto">Nombre corto de la identidad</param>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <param name="pEmail">Email del profesor</param>
        /// <param name="pCentroEstudios">Centro de estudios del profesor</param>
        /// <param name="pAreaEstudios">Area de estudios del profesor</param>
        /// <returns>Perfil creado</returns>
        public Perfil CrearPerfilProfesor(string pPerfilNombre, string pIdentidadNombreCorto, Guid pPersonaID, string pEmail, string pCentroEstudios, string pAreaEstudios, Dictionary<Guid, bool> pRecibirNewsletterDefectoProyectos)
        {
            Perfil perfil = this.AgregarPerfil(pPerfilNombre, TiposIdentidad.Profesor, string.Empty, string.Empty, pIdentidadNombreCorto, true, Guid.Empty, pPersonaID, ProyectoAD.MetaOrganizacion, ProyectoAD.MetaProyecto, pRecibirNewsletterDefectoProyectos);
            perfil.IdentidadMyGNOSS.FilaIdentidad.Tipo = (short)TiposIdentidad.Profesor;
            perfil.IdentidadMyGNOSS.FilaIdentidad.NombreCortoIdentidad = pPerfilNombre;

            AD.EntityModel.Models.IdentidadDS.Profesor filaProfesor = new AD.EntityModel.Models.IdentidadDS.Profesor();

            filaProfesor.PerfilID = perfil.Clave;
            filaProfesor.Email = pEmail;
            filaProfesor.CentroEstudios = pCentroEstudios;
            filaProfesor.AreaEstudios = pAreaEstudios;
            filaProfesor.ProfesorID = Guid.NewGuid();

            perfil.GestorIdentidades.DataWrapperIdentidad.ListaProfesor.Add(filaProfesor);

            AD.EntityModel.Models.IdentidadDS.PerfilPersona filaPerfilPersona = new AD.EntityModel.Models.IdentidadDS.PerfilPersona();
            filaPerfilPersona.PerfilID = perfil.Clave;
            filaPerfilPersona.PersonaID = pPersonaID;

            perfil.GestorIdentidades.DataWrapperIdentidad.ListaPerfilPersona.Add(filaPerfilPersona);
            mEntityContext.PerfilPersona.Add(filaPerfilPersona);

            GestorUsuarios.AgregarProyectoUsuarioIdentidad(perfil.IdentidadMyGNOSS.Clave, perfil.PersonaPerfil.FilaPersona.UsuarioID.Value, ProyectoAD.MetaProyecto, ProyectoAD.MetaOrganizacion, DateTime.Now);

            return perfil;
        }

        /// <summary>
        /// Agrega a una identidad a un grupo de identidades participación
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <param name="pGrupoID">Identificador del grupo</param>
        public void AgregarIdentidadAGrupoIdentidadesParticipacion(Guid pIdentidadID, Guid pGrupoID)
        {
            AD.EntityModel.Models.IdentidadDS.GrupoIdentidadesParticipacion filaGrupoIdentidadesParticipacion = new AD.EntityModel.Models.IdentidadDS.GrupoIdentidadesParticipacion();

            filaGrupoIdentidadesParticipacion.GrupoID = pGrupoID;
            filaGrupoIdentidadesParticipacion.IdentidadID = pIdentidadID;
            filaGrupoIdentidadesParticipacion.FechaAlta = DateTime.Now;

            DataWrapperIdentidad.ListaGrupoIdentidadesParticipacion.Add(filaGrupoIdentidadesParticipacion);
        }

        #endregion

        #region Miembros de ISerializable

        /// <summary>
        /// Metodo para serializar el objeto
        /// </summary>
        /// <param name="pInfo">Datos serializados</param>
        /// <param name="pContext">Contexto de serialización</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo pInfo, StreamingContext pContext)
        {
            base.GetObjectData(pInfo, pContext);

            pInfo.AddValue("GestorAmigos", GestorAmigos);
            pInfo.AddValue("GestorOrganizaciones", GestorOrganizaciones);
            pInfo.AddValue("GestorPersonas", GestorPersonas);
            pInfo.AddValue("GestorUsuarios", GestorUsuarios);
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Determina si está disposed
        /// </summary>
        private bool mDisposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~GestionIdentidades()
        {
            //Libero los recursos
            Dispose(false);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        /// <param name="pDisposing">Determina si se está llamando desde el Dispose()</param>
        protected override void Dispose(bool pDisposing)
        {
            if (!this.mDisposed)
            {
                mDisposed = true;

                try
                {
                    if (pDisposing)
                    {
                        if (mGestorPersonas != null)
                        {
                            mGestorPersonas.Dispose();
                        }

                        if (mGestorOrganizaciones != null)
                        {
                            mGestorOrganizaciones.Dispose();
                        }

                        if (mGestorUsuarios != null)
                        {
                            mGestorUsuarios.Dispose();
                        }

                        if (mListaPerfiles != null)
                        {
                            foreach (Perfil perfil in mListaPerfiles.Values)
                            {
                                perfil.Dispose();
                            }
                            mListaPerfiles.Clear();
                        }

                        if (mListaIdentidades != null)
                        {
                            foreach (Identidad Identidad in mListaIdentidades.Values)
                            {
                                Identidad.Dispose();
                            }
                            mListaIdentidades.Clear();
                        }
                    }
                }
                finally
                {
                    mGestorPersonas = null;
                    mGestorOrganizaciones = null;
                    mGestorUsuarios = null;
                    mListaPerfiles = null;
                    mListaIdentidades = null;

                    // Llamamos al Dispose de la clase base
                    base.Dispose(pDisposing);
                }
            }
        }

        #endregion
    }
}
