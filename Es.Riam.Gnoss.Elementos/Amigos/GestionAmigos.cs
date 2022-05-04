using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.Amigos;
using Es.Riam.Gnoss.AD.Amigos.Model;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica;
using Es.Riam.Gnoss.Logica.Amigos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Es.Riam.Gnoss.Elementos.Amigos
{
    /// <summary>
    /// Gestor de amigos
    /// </summary>
    [Serializable()]
    public class GestionAmigos : GestionGnoss, ISerializable
    {
        #region Miembros

        /// <summary>
        /// Lista de contactos (No amigos)
        /// </summary>
        private Dictionary<Guid, Identidad.Identidad> mListaContactos;

        /// <summary>
        ///Lista que contiene las identidades en Mygnoss de todos aquellos perfiles de organizacion que las organizaciones les han permitido ser mi contacto
        /// </summary>
        private List<Guid> mListaPermitidosPorOrg;

        /// <summary>
        /// Lista de amigos de la persona para la presentación.
        /// </summary>
        private Dictionary<Guid, Identidad.Identidad> mListaAmigosPresentacion;

        /// <summary>
        ///Lista de grupos de amigos
        /// </summary>
        private Dictionary<Guid, AD.EntityModel.Models.IdentidadDS.GrupoAmigos> mListaGrupoAmigos;

        /// <summary>
        ///Lista de identificadores de perfiles de amigos
        /// </summary>
        private List<Guid> mListaPerfilesAmigos;

        /// <summary>
        ///Lista de identificadores de perfiles de solicitudes de contacto
        /// </summary>
        private List<Guid> mListaPerfilesSolicitudContacto;

        /// <summary>
        ///Lista de identificadores de perfiles de solicitudes de amigos
        /// </summary>
        private List<Guid> mListaPerfilesSolicitudAmigo;

        //private List<Guid> mListaAmigosDeGrupo;

        /// <summary>
        /// Gestor de identidades
        /// </summary>
        private GestionIdentidades mGestionIdentidades;

        private LoggingService mLoggingService;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor del gestor de amigos a partir del dataset de amigos y del gestor de identidades
        /// </summary>
        /// <param name="pAmigosDS">Dataset de amigos</param>
        /// <param name="pGestionIdentidades">Gestor de identidades</param>
        public GestionAmigos(DataWrapperAmigos pAmigosDS, GestionIdentidades pGestionIdentidades, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pAmigosDS, loggingService)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;

            mGestionIdentidades = pGestionIdentidades;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
        }

        public GestionAmigos(AmigosDS pAmigosDS, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pAmigosDS, loggingService)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
        }

        /// <summary>
        /// Constructor para la deseralización
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serialización</param>
        protected GestionAmigos(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            //mLoggingService = loggingService;
            //mEntityContext = entityContext;
            //mConfigService = configService;

            mGestionIdentidades = (GestionIdentidades)info.GetValue("GestionIdentidades", typeof(GestionIdentidades));
        }

        #endregion

        #region Propiedades


        /// <summary>
        /// Obtiene el dataset de amigos
        /// </summary>
        public DataWrapperAmigos AmigosDW
        {
            get
            {
                return (DataWrapperAmigos)DataWrapper;
            }
        }

        /// <summary>
        /// Obtiene o establece el gestor de identidades
        /// </summary>
        public GestionIdentidades GestionIdentidades
        {
            get
            {
                return mGestionIdentidades;
            }
            set
            {
                mGestionIdentidades = value;
            }
        }

        /// <summary>
        /// Devuelve la lista de amigos de la persona para la presentación.
        /// </summary>
        public Dictionary<Guid, Identidad.Identidad> ListaAmigosPresentacion(Guid pIdentidadID)
        {
            if (mListaAmigosPresentacion == null)
            {
                mListaAmigosPresentacion = new Dictionary<Guid, Identidad.Identidad>();

                foreach (Amigo filaAmigo in AmigosDW.ListaAmigo)
                {
                    Identidad.Identidad identidadAmigo = null;

                    if (mGestionIdentidades.ListaIdentidades.ContainsKey(filaAmigo.IdentidadAmigoID))
                    {
                        if (!mListaAmigosPresentacion.ContainsKey(filaAmigo.IdentidadAmigoID))
                        {
                            identidadAmigo = mGestionIdentidades.ListaIdentidades[filaAmigo.IdentidadAmigoID];

                            if (identidadAmigo.Clave == pIdentidadID)
                            {
                                continue;
                            }
                            mListaAmigosPresentacion.Add(filaAmigo.IdentidadAmigoID, identidadAmigo);
                        }
                    }
                }
            }
            return mListaAmigosPresentacion;
        }

        /// <summary>
        /// TRUE si las identidades son amigos de verdad y no a través de permisos entre contactos de organización
        /// </summary>
        public bool EsAmigoRealDeIdentidad(Identidad.Identidad pIdentidad, Identidad.Identidad pAmigo, bool pEsOrganizacion, bool pEsAdministradorDeOrganizacion)
        {
            Guid identidad;

            if (pIdentidad.IdentidadOrganizacion != null && pEsAdministradorDeOrganizacion && pEsOrganizacion)
            {
                identidad = pIdentidad.IdentidadOrganizacion.IdentidadMyGNOSS.Clave;
            }
            else
            {
                identidad = pIdentidad.IdentidadMyGNOSS.Clave;
            }
            return AmigosDW.ListaAmigo.Any(item => item.IdentidadID.Equals(identidad) && item.IdentidadAmigoID.Equals(pAmigo.IdentidadMyGNOSS.Clave));
        }

        /// <summary>
        /// TRUE si la identidad y el usuario actual son amigos a través de los permisos asignados de amigos de la organización
        /// </summary>
        public bool EsAmigoPermisoAmigosOrganizacion(Identidad.Identidad pIdentidad, Identidad.Identidad pAmigo)
        {
            return AmigosDW.ListaPermisoAmigoOrg.Any(item => item.IdentidadAmigoID.Equals(pAmigo.IdentidadMyGNOSS.Clave) && item.IdentidadUsuarioID.Equals(pIdentidad.IdentidadMyGNOSS.Clave));
        }

        /// <summary>
        /// Devuelve la lista de contactos de la persona (No amigos)
        /// </summary>
        public Dictionary<Guid, Identidad.Identidad> ListaContactos
        {
            get
            {
                if (mListaContactos == null)
                {
                    mListaContactos = new Dictionary<Guid, Identidad.Identidad>();

                    foreach (Amigo filaAmigo in AmigosDW.ListaAmigo)
                    {
                        Identidad.Identidad identidadAmigo = null;

                        if (mGestionIdentidades.ListaIdentidades.ContainsKey(filaAmigo.IdentidadAmigoID))
                        {
                            identidadAmigo = mGestionIdentidades.ListaIdentidades[filaAmigo.IdentidadAmigoID];

                            if (!mListaContactos.ContainsKey(filaAmigo.IdentidadAmigoID))
                            {
                                mListaContactos.Add(filaAmigo.IdentidadAmigoID, identidadAmigo);
                            }
                        }
                    }

                    foreach (PermisoAmigoOrg filaPermisoAmigoOrg in AmigosDW.ListaPermisoAmigoOrg)
                    {
                        Identidad.Identidad identidadAmigo = null;

                        if (mGestionIdentidades.ListaIdentidades.ContainsKey(filaPermisoAmigoOrg.IdentidadUsuarioID))
                        {
                            identidadAmigo = mGestionIdentidades.ListaIdentidades[filaPermisoAmigoOrg.IdentidadUsuarioID];

                            if (!mListaContactos.ContainsKey(filaPermisoAmigoOrg.IdentidadUsuarioID))
                            {
                                mListaContactos.Add(filaPermisoAmigoOrg.IdentidadUsuarioID, identidadAmigo);
                            }
                        }
                    }
                }
                return mListaContactos;
            }
        }

        /// <summary>
        /// Lista que contiene las identidades en Mygnoss de todos aquellos perfiles de organizacion que se les ha permitido ser mi contacto
        /// </summary>
        public List<Guid> ListaPermitidosPorOrg
        {
            get
            {
                if (mListaPermitidosPorOrg == null)
                {
                    mListaPermitidosPorOrg = new List<Guid>();
                }
                return mListaPermitidosPorOrg;
            }
            set
            {
                if (mListaPermitidosPorOrg == null)
                {
                    mListaPermitidosPorOrg = new List<Guid>();
                }
                mListaPermitidosPorOrg = value;
            }
        }

        /// <summary>
        /// Devuelve la lista de identificadores de perfiles de los amigos de la persona
        /// </summary>
        public List<Guid> ListaPerfilesAmigos
        {
            get
            {
                if (mListaPerfilesAmigos == null)
                {
                    mListaPerfilesAmigos = new List<Guid>();

                    foreach (Amigo filaAmigo in AmigosDW.ListaAmigo)
                    {
                        Identidad.Identidad identidadAmigo = null;

                        if (mGestionIdentidades.ListaIdentidades.ContainsKey(filaAmigo.IdentidadAmigoID))
                        {
                            identidadAmigo = mGestionIdentidades.ListaIdentidades[filaAmigo.IdentidadAmigoID];
                            Guid perfil = identidadAmigo.FilaIdentidad.PerfilID;

                            if (!mListaPerfilesAmigos.Contains(perfil))
                            {
                                mListaPerfilesAmigos.Add(perfil);
                            }
                        }
                    }
                }
                return mListaPerfilesAmigos;
            }
        }



        /// <summary>
        /// Devuelve la lista de grupos de amigos de la persona
        /// </summary>
        public Dictionary<Guid, AD.EntityModel.Models.IdentidadDS.GrupoAmigos> ListaGrupoAmigos
        {
            get
            {
                if (mListaGrupoAmigos == null)
                {
                    RecargarListaGrupoAmigos();
                }
                return mListaGrupoAmigos;
            }
        }

        /// <summary>
        /// Recarga la lista de grupos de amigos.
        /// </summary>
        public void RecargarListaGrupoAmigos()
        {
            mListaGrupoAmigos = new Dictionary<Guid, AD.EntityModel.Models.IdentidadDS.GrupoAmigos>();

            foreach (AD.EntityModel.Models.IdentidadDS.GrupoAmigos filaGrupoAmigos in AmigosDW.ListaGrupoAmigos)
            {
                mListaGrupoAmigos.Add(filaGrupoAmigos.GrupoID, filaGrupoAmigos);
            }
        }

        #endregion

        #region Métodos generales
                
        /// <summary>
        /// Crea una fila de contacto entre las identidades pasadas como parámetro
        /// </summary>
        /// <param name="pIdentidad">Identidad que solicita ser contacto (MyGNOSS)</param>
        /// <param name="pIdentidadAmigo">Identidad del amigo (MyGNOSS)</param>
        public void CrearFilaAmigo(Guid pIdentidad, Guid pIdentidadAmigo)
        {
            if (!AmigosDW.ListaAmigo.Any(item => item.IdentidadAmigoID.Equals(pIdentidadAmigo) && item.IdentidadID.Equals(pIdentidad)))
            {
                Amigo amigo = new Amigo();
                amigo.IdentidadID = pIdentidad;
                amigo.IdentidadAmigoID = pIdentidadAmigo;
                amigo.EsFanMutuo = false;
                amigo.Fecha = new GeneralCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication).HoraServidor;

                AmigosDW.ListaAmigo.Add(amigo);
                mEntityContext.Amigo.Add(amigo);
            }
        }

        /// <summary>
        /// Indica si 2 identidades son amigos o no.
        /// </summary>
        /// <param name="pIdentidadID">Identidad 1</param>
        /// <param name="pAmigoID">Identidad 2</param>
        /// <returns>TRUE si son amigos, FALSE en caso contrario</returns>
        private bool SonIdentidadesAmigas(Guid pIdentidadID, Guid pAmigoID)
        {
            //Se que aquí no debería hacerse llamadas al CN, pero puesto que ya se están haciendo (GeneralCN) y creo que es más importante la eficiencia que esta arquitectura sin sentido (no tiene sentido que no se puedan hacer llamadas al CN), pues lo hago:

            AmigosCN amigosCN = new AmigosCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            bool sonAmigas = amigosCN.EsAmigoDeIdentidad(pIdentidadID, pAmigoID);
            amigosCN.Dispose();

            return sonAmigas;
        }

        /// <summary>
        /// Agrega una fila de permisos sobre el contacto de una organización y miembro pasado como parámetros
        /// </summary>
        /// <param name="pIdentidadOrganizacionID">Identidad de la organización en MyGNOSS</param>
        /// <param name="pAmigoID">Identidad del contacto de organización en MyGNOSS</param>
        /// <param name="pIdentidadMiembro">Identidad del miembro de la organización en MyGNOSS</param>
        /// <returns>Fila creada</returns>
        public PermisoAmigoOrg AgregarPermisoContactoOrganizacion(Guid pIdentidadOrganizacionID, Guid pAmigoID, Guid pIdentidadMiembro)
        {
            if (!AmigosDW.ListaPermisoAmigoOrg.Any(item => item.IdentidadOrganizacionID.Equals(pIdentidadOrganizacionID) && item.IdentidadUsuarioID.Equals(pIdentidadMiembro) && item.IdentidadAmigoID.Equals(pAmigoID)))
            {
                PermisoAmigoOrg permisoAmigoOrg = new PermisoAmigoOrg();
                permisoAmigoOrg.IdentidadAmigoID = pAmigoID;
                permisoAmigoOrg.IdentidadOrganizacionID = pIdentidadOrganizacionID;
                permisoAmigoOrg.IdentidadUsuarioID = pIdentidadMiembro;

                AmigosDW.ListaPermisoAmigoOrg.Add(permisoAmigoOrg);
                mEntityContext.PermisoAmigoOrg.Add(permisoAmigoOrg);

                return permisoAmigoOrg;
            }

            return null;
        }

        /// <summary>
        /// Elimina todos los amigos y las solicitudes de amigo o contacto existentes
        /// </summary>
        /// <param name="pIdentidadUsuario">Identidad de usuario</param>
        /// <param name="pIdentidadAmigo">Identidad de amigo</param>
        public void EliminarAmigos(Identidad.Identidad pIdentidadUsuario, Identidad.Identidad pIdentidadAmigo)
        {
            if (pIdentidadUsuario.EsOrganizacion)
            {
                foreach (PermisoAmigoOrg fila in AmigosDW.ListaPermisoAmigoOrg.Where(item => item.IdentidadOrganizacionID.Equals(pIdentidadUsuario.Clave) && item.IdentidadAmigoID.Equals(pIdentidadAmigo.Clave)).ToList())
                {
                    mEntityContext.Remove(fila);
                    AmigosDW.ListaPermisoAmigoOrg.Remove(fila);
                }
            }

            foreach (AmigoAgGrupo filaAmigoAgGrupo in AmigosDW.ListaAmigoAgGrupo.Where(item => (item.IdentidadID.Equals(pIdentidadUsuario.Clave) && item.IdentidadAmigoID.Equals(pIdentidadAmigo.Clave)) || (item.IdentidadID.Equals(pIdentidadAmigo.Clave) && item.IdentidadAmigoID.Equals(pIdentidadUsuario.Clave))).ToList())
            {
                mEntityContext.Remove(filaAmigoAgGrupo);
                AmigosDW.ListaAmigoAgGrupo.Remove(filaAmigoAgGrupo);
            }

            foreach (Amigo filaAmigo in AmigosDW.ListaAmigo.Where(item => (item.IdentidadID.Equals(pIdentidadUsuario.Clave) && item.IdentidadAmigoID.Equals(pIdentidadAmigo.Clave)) || (item.IdentidadID.Equals(pIdentidadAmigo.Clave) && item.IdentidadAmigoID.Equals(pIdentidadAmigo.Clave))).ToList())
            {
                mEntityContext.Remove(filaAmigo);
                AmigosDW.ListaAmigo.Remove(filaAmigo);                
            }
        }

        /// <summary>
        /// Elimina el grupo de amigos pasado por parámetro
        /// </summary>
        /// <param name="pGrupoAmigos">Grupo de amigos</param>
        public void EliminarGrupoAmigos(GrupoAmigos pGrupoAmigos)
        {
            if (ListaGrupoAmigos.ContainsKey(pGrupoAmigos.Clave))
            {
                ListaGrupoAmigos.Remove(pGrupoAmigos.Clave);
            }
            EliminarAmigosDeGrupo(pGrupoAmigos);

            // Eliminar los permisos sobre ese grupo si los tuviera
            foreach (AmigosDS.PermisoGrupoAmigoOrgRow fila in pGrupoAmigos.FilaGrupoAmigos.GetPermisoGrupoAmigoOrgRows())
            {
                fila.Delete();
            }
            pGrupoAmigos.FilaGrupoAmigos.Delete();
        }

        /// <summary>
        /// Agrega un amigo a un grupo de amigos
        /// </summary>
        /// <param name="pGrupoAmigos">Grupo de amigos</param>
        /// <param name="pIdentidadID">Identificador de identidad del grupo</param>
        /// <param name="pAmigoID">Identificador del amigo</param>
        public void AgregarAmigoAGrupo(GrupoAmigos pGrupoAmigos, Guid pIdentidadID, Guid pAmigoID)
        {
            AgregarAmigoAGrupo(pGrupoAmigos, pIdentidadID, pAmigoID, true);
        }

        /// <summary>
        /// Agrega un amigo a un grupo de amigos
        /// </summary>
        /// <param name="pGrupoAmigos">Grupo de amigos</param>
        /// <param name="pIdentidadID">Identificador de identidad del grupo</param>
        /// <param name="pAmigoID">Identificador del amigo</param>
        public void AgregarAmigoAGrupo(GrupoAmigos pGrupoAmigos, Guid pIdentidadID, Guid pAmigoID, bool pCrearFilasAmigos)
        {
            if (!pCrearFilasAmigos && AmigosDW.ListaAmigo.Any(item => item.IdentidadAmigoID.Equals(pAmigoID) && item.IdentidadID.Equals(pIdentidadID)) && !AmigosDW.ListaAmigo.Any(item => item.IdentidadAmigoID.Equals(pIdentidadID) && item.IdentidadID.Equals(pAmigoID)))
            {
                CrearFilaAmigo(pIdentidadID, pAmigoID);
                CrearFilaAmigo(pAmigoID, pIdentidadID);
            }

            AmigoAgGrupo filaAmigoAgGrupo = new AmigoAgGrupo();
            filaAmigoAgGrupo.GrupoID = pGrupoAmigos.Clave;
            filaAmigoAgGrupo.IdentidadID = pIdentidadID;
            filaAmigoAgGrupo.IdentidadAmigoID = pAmigoID;
            filaAmigoAgGrupo.Fecha = DateTime.Now;

            if (!pGrupoAmigos.ListaAmigos.ContainsKey(pAmigoID) && GestionIdentidades != null && GestionIdentidades.ListaIdentidades.ContainsKey(pAmigoID))
            {
                Identidad.Identidad amigo = GestionIdentidades.ListaIdentidades[pAmigoID];
                pGrupoAmigos.ListaAmigos.Add(pAmigoID, amigo);
            }

            if (!AmigosDW.ListaAmigoAgGrupo.Any(item => item.GrupoID.Equals(pGrupoAmigos.Clave) && item.IdentidadID.Equals(pIdentidadID) && item.IdentidadAmigoID.Equals(pAmigoID)))
            {
                AmigosDW.ListaAmigoAgGrupo.Add(filaAmigoAgGrupo);
                mEntityContext.AmigoAgGrupo.Add(filaAmigoAgGrupo);
            }
        }

        /// <summary>
        /// Elimina todos los amigos de un grupo de manera que ya no pertenezcan a él
        /// </summary>
        /// <param name="pGrupoAmigos">Grupo de amigos</param>
        public void EliminarAmigosDeGrupo(GrupoAmigos pGrupoAmigos)
        {
            List<Identidad.Identidad> listaAmigos = new List<Es.Riam.Gnoss.Elementos.Identidad.Identidad>();

            foreach (Identidad.Identidad amigo in pGrupoAmigos.ListaAmigos.Values)
            {
                listaAmigos.Add(amigo);
            }
            foreach (Identidad.Identidad amigo in listaAmigos)
            {
                EliminarAmigoDeGrupo(pGrupoAmigos, amigo.Clave);
            }
        }

        /// <summary>
        /// Cambia todos los amigos/contacto existentes de una identidad original a otra(de tipo(3) organizacion), elimina las solicitudes de la original y los grupos de la original
        /// </summary>
        /// <param name="pIdentidadIDOriginal">Clave de la identidad original</param>
        /// <param name="pIdentidadIDASustituir">Clave de la identidad a sustituir por la original, identidad de tipo 3</param>
        /// <param name="pAmigosOrganizacionDW">DS con los amigos/contactos de la organizacion</param>
        public void CambiarAmigos(Guid pIdentidadIDOriginal, Guid pIdentidadIDASustituir, DataWrapperAmigos pAmigosOrganizacionDW)
        {
            List<AmigoAgGrupo> amigoAgGrupo = AmigosDW.ListaAmigoAgGrupo.Where(item => item.IdentidadID.Equals(pIdentidadIDOriginal) || item.IdentidadAmigoID.Equals(pIdentidadIDOriginal)).ToList();

            List<Amigo> amigos = AmigosDW.ListaAmigo.Where(item => item.IdentidadID.Equals(pIdentidadIDOriginal) || item.IdentidadAmigoID.Equals(pIdentidadIDOriginal)).ToList();

            List<AD.EntityModel.Models.IdentidadDS.GrupoAmigos> grupoAmigos = AmigosDW.ListaGrupoAmigos.Where(item => item.IdentidadID.Equals(pIdentidadIDOriginal)).ToList();

            if (amigoAgGrupo != null && amigoAgGrupo.Count > 0)
            {
                foreach (AmigoAgGrupo filaAmigoAgGrupo in amigoAgGrupo)
                {
                    mEntityContext.Remove(filaAmigoAgGrupo);
                    AmigosDW.ListaAmigoAgGrupo.Remove(filaAmigoAgGrupo);
                }
            }

            if (amigos != null && amigos.Count > 0)
            {
                foreach (Amigo filaAmigo in amigos)
                {
                    if (filaAmigo.IdentidadID == pIdentidadIDOriginal)
                    {
                        if (AmigosDW.ListaAmigo.Any(item => item.IdentidadAmigoID.Equals(filaAmigo.IdentidadAmigoID) && item.IdentidadID.Equals(pIdentidadIDASustituir)))
                        {
                            mEntityContext.Remove(filaAmigo);
                            AmigosDW.ListaAmigo.Remove(filaAmigo);
                        }
                        else
                        {
                            filaAmigo.IdentidadID = pIdentidadIDASustituir;

                            if (pAmigosOrganizacionDW.ListaAmigo.Any(item => item.IdentidadAmigoID.Equals(filaAmigo.IdentidadAmigoID) && item.IdentidadID.Equals(filaAmigo.IdentidadID)) || filaAmigo.IdentidadID == filaAmigo.IdentidadAmigoID)
                            {
                                filaAmigo.IdentidadID = pIdentidadIDOriginal;
                                mEntityContext.Remove(filaAmigo);
                                AmigosDW.ListaAmigo.Remove(filaAmigo);
                            }
                        }
                    }
                    else if (filaAmigo.IdentidadAmigoID == pIdentidadIDOriginal)
                    {
                        if (AmigosDW.ListaAmigo.Any(item => item.IdentidadAmigoID.Equals(pIdentidadIDASustituir) && item.IdentidadID.Equals(filaAmigo.IdentidadID)))
                        {
                            mEntityContext.Remove(filaAmigo);
                            AmigosDW.ListaAmigo.Remove(filaAmigo);
                        }
                        else
                        {
                            filaAmigo.IdentidadAmigoID = pIdentidadIDASustituir;

                            if (pAmigosOrganizacionDW.ListaAmigo.Any(item => item.IdentidadAmigoID.Equals(filaAmigo.IdentidadAmigoID) && item.IdentidadID.Equals(filaAmigo.IdentidadID)) || filaAmigo.IdentidadID == filaAmigo.IdentidadAmigoID)
                            {
                                filaAmigo.IdentidadAmigoID = pIdentidadIDOriginal;
                                mEntityContext.Remove(filaAmigo);
                                AmigosDW.ListaAmigo.Remove(filaAmigo);
                            }
                        }
                    }
                }
            }
            if (grupoAmigos != null && grupoAmigos.Count > 0)
            {
                foreach (AD.EntityModel.Models.IdentidadDS.GrupoAmigos filaGrupoAmigo in grupoAmigos)
                {
                    mEntityContext.Remove(filaGrupoAmigo);
                    AmigosDW.ListaGrupoAmigos.Remove(filaGrupoAmigo);
                }
            }
        }


        public void EliminarContacto(Guid pIdentidadID)
        {
            List<Amigo> amigos = AmigosDW.ListaAmigo.Where(item => item.IdentidadAmigoID.Equals(pIdentidadID)).ToList();
            List<AmigoAgGrupo> amigoAgGrupo = AmigosDW.ListaAmigoAgGrupo.Where(item => item.IdentidadAmigoID.Equals(pIdentidadID)).ToList();

            if (amigoAgGrupo != null && amigoAgGrupo.Count > 0)
            {
                foreach (AmigoAgGrupo filaAmigoAgGrupo in amigoAgGrupo)
                {
                    mEntityContext.Remove(filaAmigoAgGrupo);
                    AmigosDW.ListaAmigoAgGrupo.Remove(filaAmigoAgGrupo);
                }
            }

            if (amigos != null && amigos.Count > 0)
            {
                foreach (Amigo filaAmigo in amigos)
                {
                    mEntityContext.Remove(filaAmigo);
                    AmigosDW.ListaAmigo.Remove(filaAmigo);
                }
            }
        }

        /// <summary>
        /// Elimina todos los amigos/contacto existentes entre una identidad y otras de forma reciproca, elimina las solicitudes y los grupos de la original
        /// </summary>
        /// <param name="pIdentidadID">Clave de la identidad cuyos contactos hay q eliminar</param>
        public void EliminarAmigosReciproco(Guid pIdentidadID)
        {
            List<AmigoAgGrupo> amigoAgGrupo = AmigosDW.ListaAmigoAgGrupo.Where(item => item.IdentidadID.Equals(pIdentidadID) || item.IdentidadAmigoID.Equals(pIdentidadID)).ToList();

            List<Amigo> amigos = AmigosDW.ListaAmigo.Where(item => item.IdentidadID.Equals(pIdentidadID) || item.IdentidadAmigoID.Equals(pIdentidadID)).ToList();

            List<AD.EntityModel.Models.IdentidadDS.GrupoAmigos> grupoAmigos = AmigosDW.ListaGrupoAmigos.Where(item => item.IdentidadID.Equals(pIdentidadID)).ToList();

            if (amigoAgGrupo != null && amigoAgGrupo.Count > 0)
            {
                foreach (AmigoAgGrupo filaAmigoAgGrupo in amigoAgGrupo)
                {
                    mEntityContext.AmigoAgGrupo.Remove(filaAmigoAgGrupo);
                    AmigosDW.ListaAmigoAgGrupo.Remove(filaAmigoAgGrupo);
                }
            }

            if (amigos != null && amigos.Count > 0)
            {
                foreach (Amigo filaAmigo in amigos)
                {
                    mEntityContext.Amigo.Remove(filaAmigo);
                    AmigosDW.ListaAmigo.Remove(filaAmigo);
                }
            }
            if (grupoAmigos != null && grupoAmigos.Count > 0)
            {
                foreach (AD.EntityModel.Models.IdentidadDS.GrupoAmigos filaGrupoAmigo in grupoAmigos)
                {
                    mEntityContext.GrupoAmigos.Remove(filaGrupoAmigo);
                    AmigosDW.ListaGrupoAmigos.Remove(filaGrupoAmigo);
                }
            }
        }

        /// <summary>
        /// Dada una nueva persona "Persona"(con usuario) creará permisos de contacto entre las demas personas de la organización "Organizacion" (los que tengan usuario). Los contactos serían entre la identidad (persona+organización) del metaproyecto de "Persona" y cada una de las identidades (persona+organización) del metaproyecto de los empleados compañeros de la "Organizacion"
        /// </summary>
        /// <param name="pPersona">Persona a crear contactos automáticos entre los compañeros de organización</param>
        /// <param name="pOrganizacion">Organización</param>
        /// <param name="pMetaProyectoID">Identificador del metaproyecto</param>
        public void CrearContactosCompanieros(Persona pPersona, Organizacion pOrganizacion, Guid pMetaProyectoID)
        {
            Guid perfilIDPersona = pPersona.GestorPersonas.GestorUsuarios.GestorIdentidades.DataWrapperIdentidad.ListaPerfilPersonaOrg.Where(perfilPersonaOrg => perfilPersonaOrg.OrganizacionID.Equals(pOrganizacion.Clave) && perfilPersonaOrg.PersonaID.Equals(pPersona.Clave)).Select(perfilPersonaOrg => perfilPersonaOrg.PerfilID).FirstOrDefault();

            Guid idMetaProyectoPersona = pPersona.GestorPersonas.GestorUsuarios.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(identidad => identidad.PerfilID.Equals(perfilIDPersona) && identidad.ProyectoID.Equals(pMetaProyectoID)).Select(identidad => identidad.IdentidadID).FirstOrDefault();

            foreach (PerfilPersonaOrg filaperfil in pOrganizacion.GestorOrganizaciones.GestorIdentidades.DataWrapperIdentidad.ListaPerfilPersonaOrg.Where(perfilPersonaOrg => perfilPersonaOrg.OrganizacionID.Equals(pOrganizacion.Clave)).ToList())
            {
                if (filaperfil.PersonaID != pPersona.Clave)
                {
                    Guid perfilIDOrganizacion = pOrganizacion.GestorOrganizaciones.GestorIdentidades.DataWrapperIdentidad.ListaPerfil.Where(perfil => perfil.OrganizacionID.Equals(pOrganizacion.Clave) && !perfil.PersonaID.HasValue).Select(perfil => perfil.PerfilID).FirstOrDefault();

                    Guid idMetaProyectoOrganizacion = pOrganizacion.GestorOrganizaciones.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(identidad => identidad.PerfilID.Equals(perfilIDOrganizacion) && identidad.ProyectoID.Equals(pMetaProyectoID)).Select(identidad => identidad.IdentidadID).FirstOrDefault();

                    Guid identidadCompiTrabajoMyGnoss = pOrganizacion.GestorOrganizaciones.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(identidad => identidad.PerfilID.Equals(filaperfil.PerfilID) && identidad.ProyectoID.Equals(pMetaProyectoID)).Select(identidad => identidad.IdentidadID).FirstOrDefault();

                    AgregarPermisoContactoOrganizacion(idMetaProyectoOrganizacion, identidadCompiTrabajoMyGnoss, idMetaProyectoPersona);
                }
            }
        }

        /// <summary>
        /// Cambia el contacto pIdentidadAmigo de la identidad pIdentidadUsuario por el contacto pIdentidadNuevoContacto
        /// </summary>
        /// <param name="pIdentidadUsuario">Identidad en MyGNOSS del usuario</param>
        /// <param name="pIdentidadAmigo">Identidad en MyGNOSS del contacto</param>
        /// <param name="pIdentidadNuevoContacto">Identidad en MyGNOSS del nuevo contacto</param>
        public void CambiarContactoDeIdentidad(Guid pIdentidadUsuario, Guid pIdentidadAmigo, Guid pIdentidadNuevoContacto)
        {
            List<AmigoAgGrupo> amigoAgGrupo = AmigosDW.ListaAmigoAgGrupo.Where(item => item.IdentidadID.Equals(pIdentidadUsuario) && item.IdentidadAmigoID.Equals(pIdentidadAmigo)).ToList();

            List<Amigo> amigos = AmigosDW.ListaAmigo.Where(item => item.IdentidadID.Equals(pIdentidadUsuario) && item.IdentidadAmigoID.Equals(pIdentidadUsuario)).ToList();

            foreach (AmigoAgGrupo filaAmigoAgGrupo in amigoAgGrupo)
            {
                if (!AmigosDW.ListaAmigoAgGrupo.Any(item => item.GrupoID.Equals(filaAmigoAgGrupo.GrupoID) && item.IdentidadID.Equals(pIdentidadUsuario) && item.IdentidadAmigoID.Equals(pIdentidadNuevoContacto)))
                {
                    AmigoAgGrupo nuevaFila = new AmigoAgGrupo();
                    nuevaFila.Fecha = filaAmigoAgGrupo.Fecha;
                    nuevaFila.GrupoAmigos = filaAmigoAgGrupo.GrupoAmigos;
                    nuevaFila.GrupoID = filaAmigoAgGrupo.GrupoID;
                    nuevaFila.IdentidadID = filaAmigoAgGrupo.IdentidadID;                    
                    nuevaFila.IdentidadAmigoID = pIdentidadNuevoContacto;
                    
                    AmigosDW.ListaAmigoAgGrupo.Add(nuevaFila);
                    mEntityContext.AmigoAgGrupo.Add(nuevaFila);

                    AmigosDW.ListaAmigoAgGrupo.Remove(filaAmigoAgGrupo);
                    mEntityContext.AmigoAgGrupo.Remove(filaAmigoAgGrupo);
                }
            }

            foreach (Amigo filaAmigo in amigos)
            {
                if (!AmigosDW.ListaAmigo.Any(item => item.IdentidadAmigoID.Equals(pIdentidadNuevoContacto) && item.IdentidadID.Equals(pIdentidadUsuario)))
                {
                    Amigo nuevaFila = new Amigo();
                    nuevaFila.EsFanMutuo = filaAmigo.EsFanMutuo;
                    nuevaFila.Fecha = filaAmigo.Fecha;
                    nuevaFila.IdentidadID = filaAmigo.IdentidadID;
                    nuevaFila.IdentidadAmigoID = pIdentidadNuevoContacto;

                    AmigosDW.ListaAmigo.Add(nuevaFila);
                    mEntityContext.Amigo.Add(nuevaFila);

                    AmigosDW.ListaAmigo.Remove(filaAmigo);
                    mEntityContext.Amigo.Remove(filaAmigo);
                }
            }
        }

        /// <summary>
        /// Obtiene la lista de grupos de contactos sobre los que tiene permiso de edición una identidad
        /// </summary>
        /// <param name="pIdentidad">Identidad de la que se quieren obtener los grupos de contactos</param>
        /// <returns></returns>
        public SortedList<Guid, AD.EntityModel.Models.IdentidadDS.GrupoAmigos> ObtenerListaGruposTienePermisoEdicionIdentidad(Identidad.Identidad pIdentidad)
        {
            SortedList<Guid, AD.EntityModel.Models.IdentidadDS.GrupoAmigos> lista = new SortedList<Guid, AD.EntityModel.Models.IdentidadDS.GrupoAmigos>();

            foreach (Guid id in ListaGrupoAmigos.Keys)
            {
                PermisoGrupoAmigoOrg filas = this.AmigosDW.ListaPermisoGrupoAmigoOrg.Where(item => item.GrupoID.Equals(id) && item.IdentidadOrganizacionID.Equals(pIdentidad.IdentidadOrganizacion.IdentidadPersonalMyGNOSS.Clave) && item.IdentidadUsuarioID.Equals(pIdentidad.IdentidadMyGNOSS.Clave)).FirstOrDefault();

                if (filas != null && filas.PermisoEdicion)
                {
                    lista.Add(id, pIdentidad.GestorAmigos.ListaGrupoAmigos[id]);
                }
            }

            return lista;
        }

        /// <summary>
        /// Comprueba si una identidad tiene permiso de edición sobre algún grupo de contactos de la organización
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <returns>Verdad si la identidad tiene permiso de edición sobre algún grupo de contactos de la organización</returns>
        public bool ComprobarIdentidadOrgTienePermisoEdicionAlgunGrupo(Identidad.Identidad pIdentidad)
        {
            return (ObtenerListaGruposTienePermisoEdicionIdentidad(pIdentidad).Count > 0);
        }

        /// <summary>
        /// Elimina todos los amigos, solicitudes , GrupoAmigos para todas las identidades que tenga la pPersona  (NOTA: Asegurarse de haber cargado los perfiles y las identidades de dicha persona)
        /// </summary>
        /// <param name="pPersona">Objeto Persona</param>
        public void EliminarAmigosSolicitudesGruposDePersona(Persona pPersona)
        {
            List<Guid> listaPerfiles = new List<Guid>();
            PerfilPersona perfilPersona = pPersona.GestorPersonas.GestorUsuarios.GestorIdentidades.DataWrapperIdentidad.ListaPerfilPersona.FirstOrDefault(perfilPers => perfilPers.PersonaID.Equals(pPersona.Clave));
            if (perfilPersona != null)
            {
                listaPerfiles.Add(perfilPersona.PerfilID);
            }
            List<PerfilPersonaOrg> listaPerfilPersonaOrg = pPersona.GestorPersonas.GestorUsuarios.GestorIdentidades.DataWrapperIdentidad.ListaPerfilPersonaOrg.Where(perfilPersonOrg => perfilPersonOrg.PersonaID.Equals(pPersona.Clave)).ToList();
            if (listaPerfilPersonaOrg.Count > 0)
            {
                foreach (PerfilPersonaOrg fila in listaPerfilPersonaOrg)
                {
                    listaPerfiles.Add(fila.PerfilID);
                }
            }

            if (listaPerfiles.Count > 0)
            {
                List<Guid> listaIdentidades = new List<Guid>();

                for (int i = 0; i <= listaPerfiles.Count - 1; i++)
                {
                    Guid PerfilID = listaPerfiles[i];
                    List<AD.EntityModel.Models.IdentidadDS.Identidad> listaIdentidad = pPersona.GestorPersonas.GestorUsuarios.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(identidad => identidad.PerfilID.Equals(PerfilID)).ToList();
                    if (listaIdentidad.Count > 0)
                    {
                        foreach (AD.EntityModel.Models.IdentidadDS.Identidad fila in listaIdentidad)
                        {
                            listaIdentidades.Add(fila.IdentidadID);
                        }
                    }
                }
                if (listaIdentidades.Count > 0)
                {
                    for (int i = 0; i <= listaIdentidades.Count - 1; i++)
                    {
                        foreach (AmigoAgGrupo fila in pPersona.GestorPersonas.GestorUsuarios.GestorIdentidades.GestorAmigos.AmigosDW.ListaAmigoAgGrupo.Where(item => item.IdentidadID.Equals(listaIdentidades[i]) || item.IdentidadAmigoID.Equals(listaIdentidades[i])).ToList())
                        {
                            if (mEntityContext.Entry(fila).State != Microsoft.EntityFrameworkCore.EntityState.Deleted)
                            {
                                mEntityContext.Entry(fila).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                                AmigosDW.ListaAmigoAgGrupo.Remove(fila);
                            }
                        }

                        foreach (Amigo fila in pPersona.GestorPersonas.GestorUsuarios.GestorIdentidades.GestorAmigos.AmigosDW.ListaAmigo.Where(item => item.IdentidadID.Equals(listaIdentidades[i]) || item.IdentidadAmigoID.Equals(listaIdentidades[i])).ToList())
                        {
                            if (mEntityContext.Entry(fila).State != Microsoft.EntityFrameworkCore.EntityState.Deleted)
                            {
                                mEntityContext.Entry(fila).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                                AmigosDW.ListaAmigo.Remove(fila);
                            }
                        }

                        foreach (AD.EntityModel.Models.IdentidadDS.GrupoAmigos fila in pPersona.GestorPersonas.GestorUsuarios.GestorIdentidades.GestorAmigos.AmigosDW.ListaGrupoAmigos.Where(item => item.IdentidadID.Equals(listaIdentidades[i])).ToList())
                        {
                            if (mEntityContext.Entry(fila).State != Microsoft.EntityFrameworkCore.EntityState.Deleted)
                            {
                                mEntityContext.Entry(fila).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                                AmigosDW.ListaGrupoAmigos.Remove(fila);
                            }
                        }
                    }
                }

                if (listaIdentidades != null)
                {
                    listaIdentidades.Clear();
                    listaIdentidades = null;
                }
            }

            if (listaPerfiles != null)
            {
                listaPerfiles.Clear();
                listaPerfiles = null;
            }
        }

        /// <summary>
        /// Hace que el amigo cuyo identificador se pasa por parámetro deje de pertenecer a un grupo de amigos dado
        /// </summary>
        /// <param name="pGrupoAmigos">Grupo de amigos</param>
        /// <param name="pAmigoID">Identificador de amigo</param>
        public void EliminarAmigoDeGrupo(GrupoAmigos pGrupoAmigos, Guid pAmigoID)
        {
            if (pGrupoAmigos.ListaAmigos.ContainsKey(pAmigoID))
            {
                pGrupoAmigos.ListaAmigos.Remove(pAmigoID);
            }
            AmigosDS.AmigoAgGrupoRow[] filasAmigosAgregados = pGrupoAmigos.FilaGrupoAmigos.GetAmigoAgGrupoRows();

            foreach (AmigosDS.AmigoAgGrupoRow filaAmigoAgregado in filasAmigosAgregados)
            {
                if (filaAmigoAgregado.IdentidadAmigoID == pAmigoID)
                {
                    filaAmigoAgregado.Delete();
                }
            }
        }

        /// <summary>
        /// Compara los nombres de dos amigos de manera ascendente
        /// </summary>
        /// <param name="pAmigoX">Amigo x</param>
        /// <param name="pAmigoY">Amigo y</param>
        /// <returns></returns>
        public static int CompararAmigosPorNombreAsc(ElementoGnoss pAmigoX, ElementoGnoss pAmigoY)
        {
            return pAmigoX.Nombre.CompareTo(pAmigoY.Nombre);
        }

        /// <summary>
        /// Compara los nombres de dos amigos de manera descendente
        /// </summary>
        /// <param name="pAmigoX">Amigo x</param>
        /// <param name="pAmigoY">Amigo y</param>
        /// <returns></returns>
        public static int CompararAmigosPorNombreDesc(ElementoGnoss pAmigoX, ElementoGnoss pAmigoY)
        {
            return pAmigoY.Nombre.CompareTo(pAmigoX.Nombre);
        }

        /// <summary>
        /// Compara los tipos de dos amigos de manera ascendente
        /// </summary>
        /// <param name="pAmigoX">Amigo x</param>
        /// <param name="pAmigoY">Amigo y</param>
        /// <returns></returns>
        public int CompararAmigosPorTipoAsc(ElementoGnoss pAmigoX, ElementoGnoss pAmigoY)
        {
            if (pAmigoX is GrupoAmigos)
            {
                if (pAmigoY is GrupoAmigos)
                {
                    return pAmigoX.Nombre.CompareTo(pAmigoY.Nombre);
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                if (pAmigoY is GrupoAmigos)
                {
                    return -1;
                }
                else
                {
                    return pAmigoX.Nombre.CompareTo(pAmigoY.Nombre);
                }
            }
        }

        /// <summary>
        /// Compara los tipos de dos amigos de manera descendente
        /// </summary>
        /// <param name="pAmigoX">Amigo x</param>
        /// <param name="pAmigoY">Amigo y</param>
        /// <returns></returns>
        public int CompararAmigosPorTipoDesc(ElementoGnoss pAmigoX, ElementoGnoss pAmigoY)
        {
            if (pAmigoX is GrupoAmigos)
            {
                if (pAmigoY is GrupoAmigos)
                {
                    return pAmigoX.Nombre.CompareTo(pAmigoY.Nombre);
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                if (pAmigoY is GrupoAmigos)
                {
                    return 1;
                }
                else
                {
                    return pAmigoX.Nombre.CompareTo(pAmigoY.Nombre);
                }
            }
        }

        /// <summary>
        /// Compara los tipos de dos amigos de manera descendente
        /// </summary>
        /// <param name="pAmigoX">Amigo x</param>
        /// <param name="pAmigoY">Amigo y</param>
        /// <returns></returns>
        public int CompararAmigosPorApellidoAsc(ElementoGnoss pAmigoX, ElementoGnoss pAmigoY)
        {
            Identidad.Identidad x = (Identidad.Identidad)pAmigoX;
            Identidad.Identidad y = (Identidad.Identidad)pAmigoY;

            if (x.ModoParticipacion != Es.Riam.Gnoss.AD.Identidad.TiposIdentidad.Personal)
            {
                if (y.ModoParticipacion != Es.Riam.Gnoss.AD.Identidad.TiposIdentidad.Personal)
                {
                    return x.Nombre().CompareTo(y.Nombre());
                }
                else
                {
                    return x.Nombre().CompareTo(y.Persona.Apellidos);
                }
            }
            else
            {
                if (y.ModoParticipacion != Es.Riam.Gnoss.AD.Identidad.TiposIdentidad.Personal)
                {
                    return x.Persona.Apellidos.CompareTo(y.Nombre());
                }
                else
                {
                    return x.Persona.Apellidos.CompareTo(y.Persona.Apellidos);
                }
            }
        }

        /// <summary>
        /// Compara los tipos de dos amigos de manera descendente
        /// </summary>
        /// <param name="pAmigoX">Amigo x</param>
        /// <param name="pAmigoY">Amigo y</param>
        /// <returns></returns>
        public int CompararAmigosPorApellidoDesc(ElementoGnoss pAmigoX, ElementoGnoss pAmigoY)
        {
            Identidad.Identidad x = (Identidad.Identidad)pAmigoX;
            Identidad.Identidad y = (Identidad.Identidad)pAmigoY;

            if (x.ModoParticipacion != Es.Riam.Gnoss.AD.Identidad.TiposIdentidad.Personal)
            {
                if (y.ModoParticipacion != Es.Riam.Gnoss.AD.Identidad.TiposIdentidad.Personal)
                {
                    return y.Nombre().CompareTo(x.Nombre());
                }
                else
                {
                    return y.Persona.Apellidos.CompareTo(x.Nombre());
                }
            }
            else
            {
                if (y.ModoParticipacion != Es.Riam.Gnoss.AD.Identidad.TiposIdentidad.Personal)
                {
                    return y.Nombre().CompareTo(x.Persona.Apellidos);
                }
                else
                {
                    return y.Persona.Apellidos.CompareTo(x.Persona.Apellidos);
                }
            }
        }

        /// <summary>
        /// Método para serializar el objeto
        /// </summary>
        /// <param name="pInfo">Datos serializados</param>
        /// <param name="pContext">Contexto de serialización</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo pInfo, StreamingContext pContext)
        {
            base.GetObjectData(pInfo, pContext);
            pInfo.AddValue("GestionIdentidades", GestionIdentidades);
        }

        #endregion
    }
}
