using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.CL.Identidad
{
    public class IdentidadCL : BaseCL, IDisposable
    {

        #region Miembros

        /// <summary>
        /// Clase de negocio
        /// </summary>
        private IdentidadCN mIdentidadCN = null;

        /// <summary>
        /// Clave MAESTRA de la caché
        /// </summary>
        private readonly string[] mMasterCacheKeyArray = { "Identidad" };

        private const double DURACION_CACHE_GESTOR_IDENTIDADES = 86400; // 60 * 60 * 24 = 1 día

        private ConfigService mConfigService;
        private EntityContext mEntityContext;
        private LoggingService mLoggingService;
        private RedisCacheWrapper mRedisCacheWrapper;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor para IdentidadCL
        /// </summary>
        public IdentidadCL(EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication)
        {
            mConfigService = configService;
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mRedisCacheWrapper = redisCacheWrapper;
        }

        /// <summary>
        /// Constructor para IdentidadCL
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public IdentidadCL(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pFicheroConfiguracionBD, entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication)
        {
            mConfigService = configService;
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mRedisCacheWrapper = redisCacheWrapper;
        }

        #endregion

        #region Métodos

        public Dictionary<Guid, GroupCardModel> ObtenerFichasGruposMVC(List<Guid> pListaGruposID)
        {
            Dictionary<Guid, GroupCardModel> listaGrupos = new Dictionary<Guid, GroupCardModel>();

            if (pListaGruposID.Count > 0)
            {
                Dictionary<Guid, string> keysGrupos = new Dictionary<Guid, string>();
                string[] listaClaves = new string[pListaGruposID.Count];
                int i = 0;
                foreach (Guid idGrupo in pListaGruposID)
                {
                    string clave = ObtenerClaveCache("FichaGrupoMVC_" + idGrupo).ToLower();
                    keysGrupos.Add(idGrupo, clave);
                    listaClaves[i] = clave;
                    i++;
                }
                Dictionary<string, object> objetosCache = ObtenerListaObjetosCache(listaClaves, typeof(GroupCardModel));
                foreach (Guid idGrupo in keysGrupos.Keys)
                {
                    string clave = keysGrupos[idGrupo];
                    GroupCardModel grupo = null;
                    if (objetosCache != null && objetosCache.ContainsKey(clave))
                    {
                        grupo = (GroupCardModel)objetosCache[clave];
                    }

                    listaGrupos.Add(idGrupo, grupo);
                }
            }
            return listaGrupos;
        }

        public void GuardarFichasGruposMVC(Dictionary<Guid, GroupCardModel> pListaModelosGrupos)
        {
            string[] listaClavesAgregar = new string[pListaModelosGrupos.Count];
            object[] listaObjetosAgregar = new object[pListaModelosGrupos.Count];
            int j = 0;
            foreach (Guid idGrupo in pListaModelosGrupos.Keys)
            {
                listaClavesAgregar[j] = string.Concat("FichaGrupoMVC_" + idGrupo);
                listaObjetosAgregar[j] = pListaModelosGrupos[idGrupo];
                j++;
            }
            AgregarListaObjetosCache(listaClavesAgregar, listaObjetosAgregar);
        }


        public ProfileModel ObtenerFichaIdentiadMVC(Guid pIdentidadID)
        {
            List<Guid> listaIdentidades = new List<Guid>();
            listaIdentidades.Add(pIdentidadID);
            return ObtenerFichasIdentiadesMVC(listaIdentidades)[pIdentidadID];
        }

        public Dictionary<Guid, ProfileModel> ObtenerFichasIdentiadesMVC(List<Guid> pListaIdentidadesID)
        {
            Dictionary<Guid, ProfileModel> listaIdentidades = new Dictionary<Guid, ProfileModel>();

            if (pListaIdentidadesID.Count > 0)
            {
                Dictionary<Guid, string> keysIdentidades = new Dictionary<Guid, string>();
                string[] listaClaves = new string[pListaIdentidadesID.Distinct().Count()];
                int i = 0;
                foreach (Guid idIdentidad in pListaIdentidadesID)
                {
                    if (!keysIdentidades.ContainsKey(idIdentidad))
                    {
                        string clave = ObtenerClaveCache(NombresCL.FichaIdentidadMVC + "_" + idIdentidad).ToLower();
                        keysIdentidades.Add(idIdentidad, clave);
                        listaClaves[i] = clave;
                        i++;
                    }
                }
                Dictionary<string, object> objetosCache = ObtenerListaObjetosCache(listaClaves, typeof(ProfileModel));
                foreach (Guid idIdentidad in keysIdentidades.Keys)
                {
                    string clave = keysIdentidades[idIdentidad];
                    if (objetosCache.ContainsKey(clave))
                    {
                        listaIdentidades.Add(idIdentidad, (ProfileModel)objetosCache[clave]);
                    }
                    else
                    {
                        listaIdentidades.Add(idIdentidad, null);
                    }
                }
            }
            return listaIdentidades;
        }

        public void GuardarFichaIdentidadMVC(Guid pIdentidadID, ProfileModel pIdentidadModel)
        {
            Dictionary<Guid, ProfileModel> listaModelosIdentidades = new Dictionary<Guid, ProfileModel>();
            listaModelosIdentidades.Add(pIdentidadID, pIdentidadModel);
            GuardarFichasIdentidadesMVC(listaModelosIdentidades);
        }

        public void GuardarFichasIdentidadesMVC(Dictionary<Guid, ProfileModel> pListaModelosIdentidades)
        {
            string[] listaClavesAgregar = new string[pListaModelosIdentidades.Count];
            object[] listaObjetosAgregar = new object[pListaModelosIdentidades.Count];
            int j = 0;
            foreach (Guid idIdentidad in pListaModelosIdentidades.Keys)
            {
                listaClavesAgregar[j] = string.Concat(NombresCL.FichaIdentidadMVC + "_" + idIdentidad);
                listaObjetosAgregar[j] = pListaModelosIdentidades[idIdentidad];
                j++;
            }
            AgregarListaObjetosCache(listaClavesAgregar, listaObjetosAgregar);
        }

        public void InvalidarFichaIdentidadMVC(Guid pIdentidadID)
        {
            List<Guid> listaIdentidades = new List<Guid>();
            listaIdentidades.Add(pIdentidadID);
            InvalidarFichasIdentidadesMVC(listaIdentidades);
        }

        public void InvalidarFichasIdentidadesMVC(List<Guid> pListaIdentidades)
        {

            string prefijoClave = "";
            if (!string.IsNullOrEmpty(this.Dominio))
            {
                prefijoClave = this.Dominio;
            }
            else
            {
                prefijoClave = IdentidadCL.DominioEstatico;
            }

            prefijoClave = prefijoClave + "_" + this.ClaveCache[0] + "_4.0.0.0_";
            prefijoClave = prefijoClave.ToLower();

            List<string> listaClavesInvalidar = new List<string>();
            foreach (Guid identidadID in pListaIdentidades)
            {
                string rawKey1 = NombresCL.FichaIdentidadMVC + "_" + identidadID;
                string rawKey2 = NombresCL.InfoExtraFichaIdentidadMVC + "_" + identidadID;

                string rawKey1Cache = ObtenerClaveCache(NombresCL.FichaIdentidadMVC + "_" + identidadID);
                string rawKey2Cache = ObtenerClaveCache(NombresCL.InfoExtraFichaIdentidadMVC + "_" + identidadID);
                listaClavesInvalidar.Add(rawKey1Cache.ToLower());
                listaClavesInvalidar.Add(rawKey2Cache.ToLower());
            }
            InvalidarCachesMultiples(listaClavesInvalidar);
        }

        public ProfileModel.ExtraInfoProfileModel ObtenerInfoExtraFichasIdentiadesMVC(Guid pIdentidadID)
        {
            List<Guid> listaIdentidades = new List<Guid>();
            listaIdentidades.Add(pIdentidadID);
            return ObtenerInfoExtraFichasIdentiadesMVC(listaIdentidades)[pIdentidadID];
        }

        public Dictionary<Guid, ProfileModel.ExtraInfoProfileModel> ObtenerInfoExtraFichasIdentiadesMVC(List<Guid> pListaIdentidadesID)
        {
            Dictionary<Guid, ProfileModel.ExtraInfoProfileModel> listaInfoExtraIdentidades = new Dictionary<Guid, ProfileModel.ExtraInfoProfileModel>();

            if (pListaIdentidadesID.Count > 0)
            {
                Dictionary<Guid, string> keysIdentidades = new Dictionary<Guid, string>();
                string[] listaClaves = new string[pListaIdentidadesID.Count];
                int i = 0;
                foreach (Guid idIdentidad in pListaIdentidadesID)
                {
                    string clave = ObtenerClaveCache(NombresCL.InfoExtraFichaIdentidadMVC + "_" + idIdentidad).ToLower();
                    keysIdentidades.Add(idIdentidad, clave);
                    listaClaves[i] = clave;
                    i++;
                }
                Dictionary<string, object> objetosCache = ObtenerListaObjetosCache(listaClaves, typeof(ProfileModel.ExtraInfoProfileModel));
                foreach (Guid idIdentidad in keysIdentidades.Keys)
                {
                    string clave = keysIdentidades[idIdentidad];
                    listaInfoExtraIdentidades.Add(idIdentidad, (ProfileModel.ExtraInfoProfileModel)(objetosCache[clave]));
                }
            }
            return listaInfoExtraIdentidades;
        }

        public void GuardarInfoExtraFichaIdentidadMVC(Guid pIdentidadID, ProfileModel.ExtraInfoProfileModel pIdentidadModel)
        {
            Dictionary<Guid, ProfileModel.ExtraInfoProfileModel> listaModelosIdentidades = new Dictionary<Guid, ProfileModel.ExtraInfoProfileModel>();
            listaModelosIdentidades.Add(pIdentidadID, pIdentidadModel);
            GuardarInfoExtraFichasIdentidadesMVC(listaModelosIdentidades);
        }

        public void GuardarInfoExtraFichasIdentidadesMVC(Dictionary<Guid, ProfileModel.ExtraInfoProfileModel> pListaModelosIdentidades)
        {
            string[] listaClavesAgregar = new string[pListaModelosIdentidades.Count];
            object[] listaObjetosAgregar = new object[pListaModelosIdentidades.Count];
            int j = 0;
            foreach (Guid idIdentidad in pListaModelosIdentidades.Keys)
            {
                listaClavesAgregar[j] = string.Concat(NombresCL.InfoExtraFichaIdentidadMVC + "_" + idIdentidad);
                listaObjetosAgregar[j] = pListaModelosIdentidades[idIdentidad];
                j++;
            }
            AgregarListaObjetosCache(listaClavesAgregar, listaObjetosAgregar);
        }

        public void InvalidarInfoExtraFichaIdentidadMVC(Guid pIdentidadID)
        {
            List<Guid> listaIdentidades = new List<Guid>();
            InvalidarInfoExtraFichasIdentidadesMVC(listaIdentidades);
        }

        public void InvalidarInfoExtraFichasIdentidadesMVC(List<Guid> pListaIdentidades)
        {
            string prefijoClave = "";
            if (!string.IsNullOrEmpty(this.Dominio))
            {
                prefijoClave = this.Dominio;
            }
            else
            {
                prefijoClave = IdentidadCL.DominioEstatico;
            }

            prefijoClave = prefijoClave + "_" + this.ClaveCache[0] + "_4.0.0.0_";
            prefijoClave = prefijoClave.ToLower();

            List<string> listaClavesInvalidar = new List<string>();
            foreach (Guid identidadID in pListaIdentidades)
            {
                string rawKeyCache = ObtenerClaveCache(NombresCL.InfoExtraFichaIdentidadMVC + "_" + identidadID);
                string rawKey = NombresCL.InfoExtraFichaIdentidadMVC + "_" + identidadID;
                listaClavesInvalidar.Add(rawKeyCache.ToLower());
            }
            InvalidarCachesMultiples(listaClavesInvalidar);
        }

        public UserProfileModel ObtenerPerfilMVC(Guid pPerfilID)
        {
            string rawKey = "PerfilMVC_" + pPerfilID;

            UserProfileModel ficha = (UserProfileModel)ObtenerObjetoDeCache(rawKey);

            if (ficha == null || ficha.CacheVersion < UserProfileModel.LastCacheVersion)
            {
                return null;
            }

            return ficha;
        }

        public void AgregarPerfilMVC(Guid pPerfilID, UserProfileModel pPerfilMVC)
        {
            string rawKey = "PerfilMVC_" + pPerfilID;

            pPerfilMVC.CacheVersion = UserProfileModel.LastCacheVersion;

            AgregarObjetoCache(rawKey, pPerfilMVC);
        }

        public void EliminarPerfilMVC(Guid pPerfilID)
        {
            string rawKey = "PerfilMVC_" + pPerfilID;

            InvalidarCache(rawKey);
        }

        public void EliminarPerfilesMVC(List<Guid> pPerfilesID)
        {
            if (pPerfilesID.Count > 0)
            {
                List<string> listaClavesInvalidar = new List<string>();
                foreach (Guid perfilID in pPerfilesID)
                {
                    listaClavesInvalidar.Add(ObtenerClaveCache("PerfilMVC_" + perfilID).ToLower());
                }

                InvalidarCache(listaClavesInvalidar);
            }
        }

        /// <summary>
        /// Agrega los permisos del usuario pasado como parámetro en la caché con expiración en un día.
        /// </summary>
        /// <param name="pUserID">UsuarioID del que se van a guardar los permisos</param>
        /// <param name="pOrgID">OrganizacionID a la que pertenece el usuario.</param>
        /// <param name="pRolEnOrg">Rol que tiene el usuario en la organizacionID</param>
        public void AgregarPermisosUsuarioEnOrg(Guid pUserID, Guid pOrgID, int pRolEnOrg)
        {
            AgregarObjetoCache("PermisoUsuarioEnOrg_" + pUserID + "_" + pOrgID, pRolEnOrg, 86400);
        }

        /// <summary>
        /// Eliminamos las claves de caché correspondientes a los roles de los usuarios en una organización.
        /// </summary>
        /// <param name="pUserID">UsuarioID del que hay que borrar la clave.</param>
        /// <param name="pOrgID">Organización a la que pertenece el usuario.</param>
        public void EliminarPermisosUsuarioEnOrg(Guid pUserID, Guid pOrgID)
        {
            InvalidarCache("PermisoUsuarioEnOrg_" + pUserID + "_" + pOrgID);
        }

        /// <summary>
        /// Devuelve los permisos que tiene el usuario en la Organizacion
        /// </summary>
        /// <param name="pUserID">UsuarioID del que hay que borrar la clave.</param>
        /// <param name="pOrgID">Organización a la que pertenece el usuario.</param>
        /// <returns>Permiso que tiene el usuario en la organización.</returns>
        public int ObtenerPermisosUsuarioEnOrg(Guid pUserID, Guid pOrgID)
        {
            int rol = -1;
            try
            {
                object rolDesdeCache = ObtenerObjetoDeCache("PermisoUsuarioEnOrg_" + pUserID + "_" + pOrgID, true);
                if (rolDesdeCache != null)
                {
                    rol = int.Parse(rolDesdeCache.ToString());
                }
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex);
            }
            return rol;
        }

        /// <summary>
        /// Elimina de la caché la identidad actual con la que está conectado el usuario
        /// </summary>
        public void EliminarCacheGestorIdentidadActual(Guid pUsuarioID, Guid pPersonaID, Guid pPerfilID)
        {
            if (!pUsuarioID.Equals(UsuarioAD.Invitado))
            {
                EliminarCacheGestorIdentidad(pPersonaID, pPerfilID);
            }
        }

        /// <summary>
        /// Elimina de la caché la identidad actual con la que está conectado el usuario
        /// </summary>
        public void EliminarCacheGestorTodasIdentidadesUsuario(Guid pUsuarioID, Guid pPersonaID)
        {
            if (!pUsuarioID.Equals(UsuarioAD.Invitado))
            {
                IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                List<Guid> perfilesUsu = identCN.ObtenerListaPerfilesDeUsuario(pUsuarioID);
                identCN.Dispose();

                foreach (Guid perfilID in perfilesUsu)
                {
                    EliminarCacheGestorIdentidad(pPersonaID, perfilID);
                }
            }
        }

        /// <summary>
        /// Elimina de la caché la identidad actual con la que está conectado el usuario
        /// </summary>
        public void EliminarCacheGestorTodasIdentidadesListaUsuarios(Dictionary<Guid, Guid> pDicUsuarioIDPersonaID, List<Guid> pListaUsuarioIDs)
        {
            if (pDicUsuarioIDPersonaID != null && pDicUsuarioIDPersonaID.Keys.Count > 0)
            {
                IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                Dictionary<Guid, List<Guid>> dicUsuarioPerfiles = identCN.ObtenerListaPerfilesDeListaUsuarios(pListaUsuarioIDs);

                foreach (Guid usuarioID in pDicUsuarioIDPersonaID.Keys)
                {
                    if (!usuarioID.Equals(UsuarioAD.Invitado))
                    {
                        foreach (Guid perfilID in dicUsuarioPerfiles[usuarioID])
                        {
                            EliminarCacheGestorIdentidad(pDicUsuarioIDPersonaID[usuarioID], perfilID);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Elimina de la caché la identidad actual con la que está conectado el usuario
        /// </summary>
        public void EliminarCacheGestorIdentidad(Guid pPersonaID, Guid pPerfilID)
        {
            string rawKey = string.Concat("IdentidadActual_", pPersonaID, "_", pPerfilID);
            InvalidarCache(rawKey);
        }

        /// <summary>
        /// Almacena en la caché el gestor con la identidad actual con la que está conectado el usuario
        /// </summary>
        public GestionIdentidades ObtenerCacheGestorIdentidadActual(Guid pPersonaID, Guid pPerfilID, Guid pOrganizacionID)
        {
            string rawKey = string.Concat("IdentidadActual_", pPersonaID, "_", pPerfilID);

            // Compruebo si está en la caché
            GestionIdentidades gestorIdentidades = ObtenerObjetoDeCache(rawKey) as GestionIdentidades;
            //GestionIdentidades gestorIdentidades = null;
            //byte[] arrayComprimido = ObtenerObjetoDeCache(rawKey) as byte[];
            //if (arrayComprimido == null)
            if (gestorIdentidades == null)
            {
                // Si no está, lo cargo y lo almaceno en la caché
                gestorIdentidades = ObtenerGestorIdentidadActual(pOrganizacionID, pPersonaID);

                //arrayComprimido = UtilZip.Zip(gestorIdentidades);
                //AgregarObjetoCache(rawKey, arrayComprimido);
                AgregarObjetoCache(rawKey, gestorIdentidades, DURACION_CACHE_GESTOR_IDENTIDADES);
            }

            //if (gestorIdentidades == null)
            //{
            //    object objeto = UtilZip.UnZip(arrayComprimido);

            //    if (objeto != null)
            //    {
            //        gestorIdentidades = (GestionIdentidades)objeto;

            //DataWrapperIdentidad idenAuxDW = new DataWrapperIdentidad();
            //foreach (DataTable tabla in idenAuxDW.Tables)
            //{
            //    if (!gestorIdentidades.IdentidadesDS.Tables.Contains(tabla.TableName))
            //    {
            //        EliminarCacheGestorIdentidad(mUsuario.UsuarioActual.PersonaID.UsuarioActual.PerfilID);
            //        gestorIdentidades = ObtenerGestorIdentidadActual();
            //        AgregarObjetoCache(rawKey, gestorIdentidades);
            //        break;
            //    }
            //}

            //foreach (DataColumn columna in idenAuxDW.Identidad.Columns)
            //{
            //    if (!gestorIdentidades.IdentidadesDS.Identidad.Columns.Contains(columna.ColumnName))
            //    {
            //        EliminarCacheGestorIdentidad(mUsuario.UsuarioActual.PersonaID.UsuarioActual.PerfilID);
            //        gestorIdentidades = ObtenerGestorIdentidadActual();
            //        AgregarObjetoCache(rawKey, gestorIdentidades);
            //        break;
            //    }
            //}

            //    }
            //    else
            //    {
            //        gestorIdentidades = ObtenerGestorIdentidadActual();
            //    }
            //}

            return gestorIdentidades;
        }

        /// <summary>
        /// Obtiene el gestor de la identidad acutal.
        /// </summary>
        /// <returns>Gestor de la identidad acutal</returns>
        private GestionIdentidades ObtenerGestorIdentidadActual(Guid pOrganizacionID, Guid pPersonaID)
        {
            mEntityContext.UsarEntityCache = true;
            PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperPersona dataWrapperPersona = personaCN.ObtenerPersonaPorID(pPersonaID);
            personaCN.Dispose();

            DataWrapperOrganizacion organizacionDS = new DataWrapperOrganizacion();
            if (pOrganizacionID != UsuarioAD.Invitado)
            {
                OrganizacionCN organizacionCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

                // Cargar todas las organizaciones de la persona porque si se comparte en una comunidad en la que se participa con otra organización diferente a la de la identidad actual y participa con perfil coporativo, falla la compartición.
                organizacionDS = organizacionCN.ObtenerOrganizacionesDePersona(pPersonaID);
                organizacionDS.LlenarEntidadesCache();
                organizacionCN.Dispose();
            }
            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            dataWrapperPersona.CargaRelacionesPerezosasCache();
            GestionIdentidades gestorIdentidades = new GestionIdentidades(identidadCN.ObtenerPerfilesDePersona(pPersonaID, true), new GestionPersonas(dataWrapperPersona, mLoggingService, mEntityContext), new GestionOrganizaciones(organizacionDS, mLoggingService, mEntityContext), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            identidadCN.Dispose();
            mEntityContext.UsarEntityCache = false;
            return gestorIdentidades;
        }

        /// <summary>
        /// Obtiene un array con 2 valores, el primero es el ID del perfil actual del usuario y el segundo es el ID de la identidad actual del usuario
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pRequest">Request de la petición actual</param>
        /// <returns>Array de GUID de dos valores o null si se acaba de loguear</returns>
        public Guid[] ObtenerIdentidadActualUsuario(Guid pUsuarioID, Guid pPersonaID)
        {
            return ObtenerIdentidadActualUsuario(pUsuarioID, pPersonaID, true);
        }

        /// <summary>
        /// Obtiene un array con 2 valores, el primero es el ID del perfil actual del usuario y el segundo es el ID de la identidad actual del usuario
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pRequest">Request de la petición actual</param>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <param name="pObtenerIdentidadMyGnoss">Verdad si queremos que obtenga la identidad personal en MyGnoss en caso de que la caché esté vacía</param>
        /// <returns>Array de GUID de dos valores o null si se acaba de loguear</returns>
        public Guid[] ObtenerIdentidadActualUsuario(Guid pUsuarioID, Guid pPersonaID, bool pObtenerIdentidadMyGnoss)
        {
            string rawKey = ObtenerKeyIdentidadUsuarioActual(pUsuarioID);
            Guid[] identidadActual = null;

            if (pUsuarioID.Equals(UsuarioAD.Invitado))
            {
                identidadActual = new Guid[2];
                identidadActual[0] = UsuarioAD.Invitado;
                identidadActual[1] = UsuarioAD.Invitado;
            }
            else
            {
                // Compruebo si está en la caché
                identidadActual = ObtenerObjetoDeCache(rawKey) as Guid[];

                if (identidadActual == null && pObtenerIdentidadMyGnoss)
                {
                    Guid[] identMyGnossCambiadas = IdentidadCN.ObtenerIdentidadIDDePersonaEnProyecto(ProyectoAD.MetaProyecto, pPersonaID);
                    if (identMyGnossCambiadas != null)
                    {
                        //Esta función obtiene primero la identidadid y luego el perfilid, le cambio el orden
                        identidadActual = new Guid[2];
                        identidadActual[0] = identMyGnossCambiadas[1];
                        identidadActual[1] = identMyGnossCambiadas[0];
                        AgregarCacheIdentidadActualUsuario(pUsuarioID, identidadActual[0], identidadActual[1]);
                    }
                }
            }
            return identidadActual;
        }

        /// <summary>
        /// Almacena en la caché la identidad actual con la que está conectado el usuario
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <param name="pPerfilID">Identificador del perfil</param>
        /// <param name="pRequest">Request de la petición actual</param>
        public void AgregarCacheIdentidadActualUsuario(Guid pUsuarioID, Guid pPerfilID, Guid pIdentidadID)
        {
            if ((!pPerfilID.Equals(Guid.Empty)) && (!pIdentidadID.Equals(Guid.Empty)) && (!pUsuarioID.Equals(UsuarioAD.Invitado)))
            {
                string rawKey = ObtenerKeyIdentidadUsuarioActual(pUsuarioID);

                Guid[] identidadActual = { pPerfilID, pIdentidadID };

                AgregarObjetoCache(rawKey, identidadActual);
            }
        }

        /// <summary>
        /// Almacena en la caché la identidad actual con la que está conectado el usuario
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pRequest">Request de la petición actual</param>
        public void EliminarCacheIdentidadActualUsuario(Guid pUsuarioID)
        {
            if (!pUsuarioID.Equals(UsuarioAD.Invitado))
            {
                string rawKey = ObtenerKeyIdentidadUsuarioActual(pUsuarioID);
                InvalidarCache(rawKey);
            }
        }

        /// <summary>
        /// Obtiene la clave de cache para obtener la identidad actual de un usuario
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pRequest">Request de la petición actual</param>
        /// <returns></returns>
        private string ObtenerKeyIdentidadUsuarioActual(Guid pUsuarioID)
        {
            return "IdentidadActualUsuario_" + pUsuarioID;
        }

        /// <summary>
        /// Verdad si una organización es una clase
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizacion</param>
        /// <returns></returns>
        public DataWrapperIdentidad ObtenerPerfilDeOrganizacion(Guid pOrganizacionID)
        {
            mEntityContext.UsarEntityCache = true;
            string rawKey = string.Concat("PerfilDeOrganizacion_", pOrganizacionID);

            // Compruebo si está en la caché
            DataWrapperIdentidad identDW = ObtenerObjetoDeCache(rawKey) as DataWrapperIdentidad;
            if (identDW == null)
            {
                // Si no está, lo cargo y lo almaceno en la caché
                identDW = IdentidadCN.ObtenerPerfilDeOrganizacion(pOrganizacionID);
                if (identDW != null)
                {
                    identDW.CargaRelacionesPerezosasCache();
                }
                AgregarObjetoCache(rawKey, identDW);

                mIdentidadCN.Dispose();
                mIdentidadCN = null;
            }
            mEntityContext.UsarEntityCache = false;
            return identDW;
        }

        public string ObtenerNombredePerfilID(Guid pPerfilID)
        {
            string rawKey = "NombrePerfil_" + pPerfilID;

            // Compruebo si está en la caché
            string nombre = ObtenerObjetoDeCache(rawKey) as string;

            if (nombre == null)
            {
                // Si no está, lo cargo y lo almaceno en la caché
                nombre = IdentidadCN.ObtenerNombredePerfilID(pPerfilID);

                AgregarObjetoCache(rawKey, nombre);
            }

            return nombre;
        }

        public DataWrapperIdentidad ObtenerMiembrosComunidad(Guid pProyectoID)
        {
            mEntityContext.UsarEntityCache = true;
            string rawKey = string.Concat("MiembrosComunidad_", pProyectoID.ToString());

            // Compruebo si está en la caché
            DataWrapperIdentidad identDW = ObtenerObjetoDeCache(rawKey) as DataWrapperIdentidad;
            if (identDW == null)
            {
                // Si no está, lo cargo y lo almaceno en la caché
                identDW = IdentidadCN.ObtenerMiembrosComunidad(pProyectoID);
                //Comentado por EF
                //if (!identDW.Perfil.Columns.Contains("NombreBusqueda"))
                //{
                //    identDW.Perfil.Columns.Add("NombreBusqueda");
                //}
                //foreach (AD.EntityModel.Models.IdentidadDS.Perfil filaPerfil in identDW.ListaPerfil)
                //{
                //    filaPerfil["NombreBusqueda"] = UtilCadenas.RemoveAccentsWithRegEx(filaPerfil.NombrePerfil);
                //}

                //if (!identDW.GrupoIdentidades.Columns.Contains("NombreBusqueda"))
                //{
                //    identDW.GrupoIdentidades.Columns.Add("NombreBusqueda");
                //}
                //foreach (AD.EntityModel.Models.IdentidadDS.GrupoIdentidades filaGrupoIdentidades in identDW.ListaGrupoIdentidades)
                //{
                //    filaGrupoIdentidades["NombreBusqueda"] = UtilCadenas.RemoveAccentsWithRegEx(filaGrupoIdentidades.Nombre);
                //}
                identDW.CargaRelacionesPerezosasCache();
                AgregarObjetoCache(rawKey, identDW);

                mIdentidadCN.Dispose();
                mIdentidadCN = null;
            }
            mEntityContext.UsarEntityCache = false;
            return identDW;
        }

        public void InvalidarCacheMiembrosComunidad(Guid pProyectoID)
        {
            string rawKey = string.Concat("MiembrosComunidad_", pProyectoID.ToString());

            InvalidarCache(rawKey);
        }

        public DataWrapperIdentidad ObtenerMiembrosOrganizacionParaFiltroGrupos(Guid pOrganizacionID)
        {
            mEntityContext.UsarEntityCache = true;
            string rawKey = string.Concat("MiembrosOrganizacion_", pOrganizacionID.ToString());

            // Compruebo si está en la caché
            DataWrapperIdentidad identDW = ObtenerObjetoDeCache(rawKey) as DataWrapperIdentidad;
            if (identDW == null)
            {
                // Si no está, lo cargo y lo almaceno en la caché
                identDW = IdentidadCN.ObtenerMiembrosOrganizacionParaFiltroGrupos(pOrganizacionID);
                //Comentado por EF
                //if (!identDW.Perfil.Columns.Contains("NombreBusqueda"))
                //{
                //    identDW.Perfil.Columns.Add("NombreBusqueda");
                //}
                //foreach (IdentidadDS.PerfilRow filaPerfil in identDW.Perfil.Rows)
                //{
                //    filaPerfil["NombreBusqueda"] = UtilCadenas.RemoveAccentsWithRegEx(filaPerfil.NombrePerfil);
                //}

                //if (!identDW.GrupoIdentidades.Columns.Contains("NombreBusqueda"))
                //{
                //    identDW.GrupoIdentidades.Columns.Add("NombreBusqueda");
                //}
                //foreach (AD.EntityModel.Models.IdentidadDS.GrupoIdentidades filaGrupoIdentidades in identDW.ListaGrupoIdentidades)
                //{
                //    filaGrupoIdentidades["NombreBusqueda"] = UtilCadenas.RemoveAccentsWithRegEx(filaGrupoIdentidades.Nombre);
                //}
                if (identDW != null)
                {
                    identDW.CargaRelacionesPerezosasCache();
                }
                AgregarObjetoCache(rawKey, identDW);

                mIdentidadCN.Dispose();
                mIdentidadCN = null;
            }
            mEntityContext.UsarEntityCache = false;
            return identDW;
        }

        public void InvalidarCacheMiembrosOrganizacionParaFiltroGrupos(Guid pOrganizacionID)
        {
            string rawKey = string.Concat("MiembrosOrganizacion_", pOrganizacionID.ToString());

            InvalidarCache(rawKey);
        }

        public DataWrapperIdentidad ObtenerMiembrosGnossVisibles()
        {
            string rawKey = "MiembrosGnossVisibles";
            mEntityContext.UsarEntityCache = true;
            // Compruebo si está en la caché
            DataWrapperIdentidad identDW = ObtenerObjetoDeCache(rawKey) as DataWrapperIdentidad;
            if (identDW == null)
            {
                // Si no está, lo cargo y lo almaceno en la caché
                identDW = IdentidadCN.ObtenerMiembrosGnossVisibles();
                //Comentado por EF
                //if (!identDW.Perfil.Columns.Contains("NombreBusqueda"))
                //{
                //    identDW.Perfil.Columns.Add("NombreBusqueda");
                //}
                //foreach (AD.EntityModel.Models.IdentidadDS.Perfil filaPerfil in identDW.ListaPerfil)
                //{
                //    filaPerfil["NombreBusqueda"] = UtilCadenas.RemoveAccentsWithRegEx(filaPerfil.NombrePerfil);
                //}

                AgregarObjetoCache(rawKey, identDW);
                if (identDW != null)
                {
                    identDW.CargaRelacionesPerezosasCache();
                }
                mIdentidadCN.Dispose();
                mIdentidadCN = null;
            }
            mEntityContext.UsarEntityCache = false;
            return identDW;
        }

        /// <summary>
        /// Obtiene un grupo por el nombre corto y el proyecto
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto del grupo</param>
        /// <param name="pProyecto">Proyecto del Grupo</param>
        /// <returns>DataSet con el grupo</returns>
        public DataWrapperIdentidad ObtenerGrupoPorNombreCortoYProyecto(string pNombreCorto, Guid pProyectoID)
        {
            mEntityContext.UsarEntityCache = true;
            string rawKey = "Grupo_" + pNombreCorto + "_" + pProyectoID;

            // Compruebo si está en la caché
            DataWrapperIdentidad identDW = ObtenerObjetoDeCache(rawKey) as DataWrapperIdentidad;
            if (identDW == null)
            {
                // Si no está, lo cargo y lo almaceno en la caché
                identDW = IdentidadCN.ObtenerGrupoPorNombreCortoYProyecto(pNombreCorto, pProyectoID);

                AgregarObjetoCache(rawKey, identDW);
                if (identDW != null)
                {
                    identDW.CargaRelacionesPerezosasCache();
                }
                mIdentidadCN.Dispose();
                mIdentidadCN = null;
            }
            mEntityContext.UsarEntityCache = false;
            return identDW;
        }

        public void InvalidarCacheGrupoPorNombreCortoYProyecto(string pNombreCorto, Guid pProyectoID)
        {
            string rawKey = "Grupo_" + pNombreCorto + "_" + pProyectoID;

            InvalidarCache(rawKey);
        }

        /// <summary>
        /// Obtiene un grupo por el nombre corto y el proyecto
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto del grupo</param>
        /// <param name="pProyecto">Proyecto del Grupo</param>
        /// <returns>DataSet con el grupo</returns>
        public DataWrapperIdentidad ObtenerGrupoPorNombreCortoYOrganizacion(string pNombreCorto, Guid pOrganizacionID)
        {
            string rawKey = "Grupo_" + pNombreCorto + "_" + pOrganizacionID;
            mEntityContext.UsarEntityCache = true;
            // Compruebo si está en la caché
            DataWrapperIdentidad identDW = ObtenerObjetoDeCache(rawKey) as DataWrapperIdentidad;
            if (identDW == null)
            {
                // Si no está, lo cargo y lo almaceno en la caché
                identDW = IdentidadCN.ObtenerGrupoPorNombreCortoYOrganizacion(pNombreCorto, pOrganizacionID);

                AgregarObjetoCache(rawKey, identDW);
                if (identDW != null)
                {
                    identDW.CargaRelacionesPerezosasCache();
                }
                mIdentidadCN.Dispose();
                mIdentidadCN = null;
            }
            mEntityContext.UsarEntityCache = false;
            return identDW;
        }

        public void InvalidarCacheGrupoPorNombreCortoYOrganizacion(string pNombreCorto, Guid pOrganizacionID)
        {
            string rawKey = "Grupo_" + pNombreCorto + "_" + pOrganizacionID;

            InvalidarCache(rawKey);
        }

        #region Chat

        /// <summary>
        /// Agrega los permisos del usuario pasado como parámetro en la caché con expiración en un día.
        /// </summary>
        /// <param name="pUserID">UsuarioID del que se van a guardar los permisos</param>
        /// <param name="pOrgID">OrganizacionID a la que pertenece el usuario.</param>
        /// <param name="pRolEnOrg">Rol que tiene el usuario en la organizacionID</param>
        public void AgregarPerfilAChat(Guid pPerfilID, string pChatID)
        {
            AgregarObjetoCache("PerfilChat_" + pPerfilID, pChatID);
        }

        /// <summary>
        /// Eliminamos las claves de caché correspondientes a los roles de los usuarios en una organización.
        /// </summary>
        /// <param name="pUserID">UsuarioID del que hay que borrar la clave.</param>
        /// <param name="pOrgID">Organización a la que pertenece el usuario.</param>
        public void EliminarPerfilDeChat(Guid pPerfilID)
        {
            InvalidarCache("PerfilChat_" + pPerfilID);
        }

        /// <summary>
        /// Devuelve los permisos que tiene el usuario en la Organizacion
        /// </summary>
        /// <param name="pUserID">UsuarioID del que hay que borrar la clave.</param>
        /// <param name="pOrgID">Organización a la que pertenece el usuario.</param>
        /// <returns>Permiso que tiene el usuario en la organización.</returns>
        public string ObtenerChatIDDePerfil(Guid pPerfilID)
        {
            string chatID = ObtenerObjetoDeCache("PerfilChat_" + pPerfilID, true) as string;
            return chatID;
        }

        #endregion

        #endregion

        #region Propiedades

        /// <summary>
        /// Clase de negocio
        /// </summary>
        protected IdentidadCN IdentidadCN
        {
            get
            {
                if (mIdentidadCN == null)
                {
                    //if (string.IsNullOrEmpty(mFicheroConfiguracionBD))
                    //{
                    //    mIdentidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService);
                    //}
                    //else
                    //{
                        mIdentidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    //}
                }

                return mIdentidadCN;
            }
        }

        /// <summary>
        /// Clave para la caché
        /// </summary>
        public override string[] ClaveCache
        {
            get
            {
                return mMasterCacheKeyArray;
            }
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Determina si está disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~IdentidadCL()
        {
            //Libero los recursos
            Dispose(false);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        public override void Dispose()
        {
            Dispose(true);

            //impido que se finalice dos veces este objeto
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        /// <param name="disposing">Determina si se está llamando desde el Dispose()</param>
        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                this.disposed = true;

                try
                {
                    if (disposing)
                    {
                    }
                }
                catch (Exception e)
                {
                    mLoggingService.GuardarLogError(e);
                }
            }
        }

        #endregion
    }
}
