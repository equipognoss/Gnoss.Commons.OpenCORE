using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Peticion;
using Es.Riam.Gnoss.AD.Peticion;
using Es.Riam.Gnoss.Elementos.Notificacion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Es.Riam.Gnoss.Elementos.Peticiones
{
    /// <summary>
    /// Gestor de peticiones
    /// </summary>
    [Serializable]
    public class GestionPeticiones : GestionGnoss, ISerializable, IDisposable
    {
        #region Miembros

        /// <summary>
        /// Gestor de notificaciones
        /// </summary>
        private GestionNotificaciones mGestionNotificaciones;

        /// <summary>
        /// Lista de peticiones
        /// </summary>
        private Dictionary<Guid, Peticion> mListaPeticiones;

        private EntityContext mEntityContext;
        private LoggingService mLoggingService;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor a partir de un dataset de peticiones pasado como parámetro
        /// </summary>
        /// <param name="pPeticionDW">Dataset de peticiones</param>
        public GestionPeticiones(DataWrapperPeticion pPeticionDW, LoggingService loggingService, EntityContext entityContext) 
            : base(pPeticionDW, loggingService)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
        }

        /// <summary>
        /// Constructor para la deseralización
        /// </summary>
        /// <param name="pInfo">Datos serializados</param>
        /// <param name="pContext">Contexto de serialización</param>
        protected GestionPeticiones(SerializationInfo pInfo, StreamingContext pContext) 
            : base(pInfo, pContext)
        {
            //mLoggingService = loggingService;
            //mEntityContext = entityContext;
            mGestionNotificaciones = (GestionNotificaciones)pInfo.GetValue("GestionNotificaciones", typeof(GestionNotificaciones));
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene el dataset de peticiones
        /// </summary>
        public DataWrapperPeticion PeticionDW
        {
            get
            {
                return (DataWrapperPeticion)DataWrapper;
            }
        }

        /// <summary>
        /// Obtiene la lista de todas las peticiones
        /// </summary>
        public Dictionary<Guid, Peticion> ListaPeticiones
        {
            get
            {
                if (mListaPeticiones == null)
                {
                    CargarPeticiones();
                }
                return mListaPeticiones;
            }
        }

        /// <summary>
        /// Obtiene o establece el gestor de notificaciones
        /// </summary>
        public GestionNotificaciones GestorNotificaciones
        {
            get
            {
                return mGestionNotificaciones;
            }
            set
            {
                mGestionNotificaciones = value;
            }
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Crea una petición de acceso a una organización
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización para la que se quiere dar acceso a la persona</param>
        /// <returns>Petición generada</returns>
        public Peticion AgregarPeticionDeAccesoOrg(Guid pOrganizacionID)
        {
            AD.EntityModel.Models.Peticion.Peticion filaPeticion = new AD.EntityModel.Models.Peticion.Peticion();
            filaPeticion.PeticionID = Guid.NewGuid();
            filaPeticion.FechaPeticion = DateTime.Now;
            filaPeticion.Tipo = (short)TipoPeticion.AccesoAOrganizacion;
            filaPeticion.Estado = (short)EstadoPeticion.Pendiente;
            mEntityContext.Peticion.Add(filaPeticion);
            PeticionDW.ListaPeticion.Add(filaPeticion);

            PeticionOrgInvitaPers filaPeticionOrg = new PeticionOrgInvitaPers();
            filaPeticionOrg.OrganizacionID = pOrganizacionID;
            filaPeticionOrg.Peticion = filaPeticion;
            mEntityContext.PeticionOrgInvitaPers.Add(filaPeticionOrg);
            PeticionDW.ListaPeticionOrgInvitaPers.Add(filaPeticionOrg);

            Peticion peticion = new PeticionInvOrganizacion(filaPeticionOrg, filaPeticion, this, mLoggingService);

            if (!ListaPeticiones.ContainsKey(peticion.Clave))
            {
                this.ListaPeticiones.Add(peticion.Clave, peticion);
                this.Hijos.Add(peticion);
            }
            return peticion;
        }

        /// <summary>
        /// Crea una petición de acceso a una comunidad
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización del proyecto al que se quiere dar acceso a un usuario</param>
        /// <param name="pProyectoID">Identificador del proyecto al que se quiere dar acceso a un usuario</param>
        /// <returns>Petición generada</returns>
        public Peticion AgregarPeticionDeAccesoComunidad(Guid pOrganizacionID, Guid pProyectoID)
        {
            AD.EntityModel.Models.Peticion.Peticion filaPeticion = new AD.EntityModel.Models.Peticion.Peticion();
            filaPeticion.PeticionID = Guid.NewGuid();
            filaPeticion.FechaPeticion = DateTime.Now;
            filaPeticion.Tipo = (short)TipoPeticion.AccesoAProyecto;
            filaPeticion.Estado = (short)EstadoPeticion.Pendiente;
            PeticionDW.ListaPeticion.Add(filaPeticion);
            mEntityContext.Peticion.Add(filaPeticion);

            PeticionInvitacionComunidad filaPeticionCom = new PeticionInvitacionComunidad();
            filaPeticionCom.OrganizacionID = pOrganizacionID;
            filaPeticionCom.ProyectoID = pProyectoID;
            filaPeticionCom.PeticionID = filaPeticion.PeticionID;
            filaPeticionCom.Peticion = filaPeticion;
            PeticionDW.ListaPeticionInvitacionComunidad.Add(filaPeticionCom);
            mEntityContext.PeticionInvitacionComunidad.Add(filaPeticionCom);

            Peticion peticion = new PeticionInvComunidad(filaPeticionCom, filaPeticion, this, mLoggingService);

            if (!ListaPeticiones.ContainsKey(peticion.Clave))
            {
                this.ListaPeticiones.Add(peticion.Clave, peticion);
                this.Hijos.Add(peticion);
            }
            return peticion;
        }

        /// <summary>
        /// Crea una petición de acceso a una comunidad
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización del proyecto al que se quiere dar acceso a un usuario</param>
        /// <param name="pProyectoID">Identificador del proyecto al que se quiere dar acceso a un usuario</param>
        /// <returns>Petición generada</returns>
        public Peticion AgregarPeticionDeAccesoComunidadYGrupo(List<Guid> pListaGrupos,Guid pOrganizacionID, Guid pProyectoID)
        {
            string grupos = "";
            foreach (Guid GrupoID in pListaGrupos)
            {
                grupos += grupos + "," + GrupoID.ToString();
            }

            AD.EntityModel.Models.Peticion.Peticion filaPeticion = new AD.EntityModel.Models.Peticion.Peticion();
            filaPeticion.PeticionID = Guid.NewGuid();
            filaPeticion.FechaPeticion = DateTime.Now;
            filaPeticion.Tipo = (short)TipoPeticion.AccesoAProyecto;
            filaPeticion.Estado = (short)EstadoPeticion.Pendiente;
            PeticionDW.ListaPeticion.Add(filaPeticion);
            mEntityContext.Peticion.Add(filaPeticion);

            PeticionInvitacionGrupo filaPeticionGrupo = new PeticionInvitacionGrupo();
            filaPeticionGrupo.PeticionID = filaPeticion.PeticionID;
            filaPeticionGrupo.OrganizacionID = pOrganizacionID;
            filaPeticionGrupo.ProyectoID = pProyectoID;
            filaPeticionGrupo.GruposID = grupos;
            PeticionDW.ListaPeticionInvitacionGrupo.Add(filaPeticionGrupo);
            mEntityContext.PeticionInvitacionGrupo.Add(filaPeticionGrupo);

            PeticionInvitacionComunidad filaPeticionCom = new PeticionInvitacionComunidad();
            filaPeticionCom.OrganizacionID = pOrganizacionID;
            filaPeticionCom.ProyectoID = pProyectoID;
            filaPeticionCom.PeticionID = filaPeticion.PeticionID;
            filaPeticionCom.Peticion = filaPeticion;
            PeticionDW.ListaPeticionInvitacionComunidad.Add(filaPeticionCom);
            mEntityContext.PeticionInvitacionComunidad.Add(filaPeticionCom);

            Peticion peticion = new PeticionInvComunidad(filaPeticionCom, filaPeticion, this, mLoggingService);

            if (!ListaPeticiones.ContainsKey(peticion.Clave))
            {
                this.ListaPeticiones.Add(peticion.Clave, peticion);
                this.Hijos.Add(peticion);
            }
            return peticion;
        }

        /// <summary>
        /// Crea una petición de acceso a una comunidad proveniente de Ning
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización del proyecto al que se quiere dar acceso a un usuario</param>
        /// <param name="pProyectoID">Identificador del proyecto al que se quiere dar acceso a un usuario</param>
        /// <param name="pNingID">Usuario de Ning al que se le realiza la petición</param>
        /// <returns>Petición generada</returns>
        public Peticion AgregarPeticionDeAccesoComunidadNing(Guid pOrganizacionID, Guid pProyectoID, string pNingID)
        {
            AD.EntityModel.Models.Peticion.Peticion filaPeticion = new AD.EntityModel.Models.Peticion.Peticion();
            filaPeticion.PeticionID = Guid.NewGuid();
            filaPeticion.FechaPeticion = DateTime.Now;
            filaPeticion.Tipo = (short)TipoPeticion.AccesoAProyecto;
            filaPeticion.Estado = (short)EstadoPeticion.Pendiente;
            PeticionDW.ListaPeticion.Add(filaPeticion);
            mEntityContext.Peticion.Add(filaPeticion);

            PeticionInvitacionComunidad filaPeticionCom = new PeticionInvitacionComunidad();
            filaPeticionCom.OrganizacionID = pOrganizacionID;
            filaPeticionCom.ProyectoID = pProyectoID;
            filaPeticionCom.PeticionID = filaPeticion.PeticionID;
            filaPeticionCom.NingID = pNingID;
            filaPeticionCom.Peticion = filaPeticion;
            PeticionDW.ListaPeticionInvitacionComunidad.Add(filaPeticionCom);

            Peticion peticion = new PeticionInvComunidad(filaPeticionCom, filaPeticion, this, mLoggingService);

            if (!ListaPeticiones.ContainsKey(peticion.Clave))
            {
                this.ListaPeticiones.Add(peticion.Clave, peticion);
                this.Hijos.Add(peticion);
            }
            return peticion;
        }

        /// <summary>
        /// Crea una petición de acceso a gnoss de un contacto importado
        /// </summary>
        /// <param name="pIdentidadID">Identidad del usuario que esta enviando la petición</param>  
        /// <param name="pOrganizacionID">Identificador de la organización del proyecto al que se quiere dar acceso a un usuario</param>
        /// <param name="pProyectoID">Identificador del proyecto al que se quiere dar acceso a un usuario</param>
        /// <returns>Petición generada</returns>
        public Peticion AgregarPeticionDeAccesoContacto(Guid pIdentidadID, Guid pOrganizacionID, Guid pProyectoID)
        {
            AD.EntityModel.Models.Peticion.Peticion filaPeticion = new AD.EntityModel.Models.Peticion.Peticion();
            filaPeticion.PeticionID = Guid.NewGuid();
            filaPeticion.FechaPeticion = DateTime.Now;
            filaPeticion.Tipo = (short)TipoPeticion.AccesoGnossYContacto;
            filaPeticion.Estado = (short)EstadoPeticion.Pendiente;
            PeticionDW.ListaPeticion.Add(filaPeticion);
            mEntityContext.Peticion.Add(filaPeticion);

            PeticionInvitaContacto filaPeticionContacto = new PeticionInvitaContacto();
            filaPeticionContacto.PeticionID = filaPeticion.PeticionID;
            filaPeticionContacto.IdentidadID = pIdentidadID;
            filaPeticionContacto.Peticion = filaPeticion;
            PeticionDW.ListaPeticionInvitacionContacto.Add(filaPeticionContacto);
            mEntityContext.PeticionInvitaContacto.Add(filaPeticionContacto);

            PeticionInvitacionComunidad filaPeticionComunidad = new PeticionInvitacionComunidad();
            filaPeticionComunidad.PeticionID = filaPeticion.PeticionID;
            filaPeticionComunidad.OrganizacionID = pOrganizacionID;
            filaPeticionComunidad.ProyectoID = pProyectoID;
            filaPeticionComunidad.Peticion = filaPeticion;
            PeticionDW.ListaPeticionInvitacionComunidad.Add(filaPeticionComunidad);
            mEntityContext.PeticionInvitacionComunidad.Add(filaPeticionComunidad);

            Peticion peticion = new PeticionInvContacto(filaPeticionContacto,filaPeticionComunidad, filaPeticion, this, mLoggingService);

            if (!ListaPeticiones.ContainsKey(peticion.Clave))
            {
                this.ListaPeticiones.Add(peticion.Clave, peticion);
                this.Hijos.Add(peticion);
            }
            return peticion;
        }
                                
        /// <summary>
        /// Crea una petición de acceso a grupos y a una comunidad para alguien
        /// </summary>
        /// <param name="pGruposID">Ids de los grupos a los que se va a dar acceso</param>
        /// <param name="pProyectoID">Proyecto al que pertenece el Dafo, y del que se va a hacer miembro el destinatario</param>
        /// <param name="pOrganizacionID">Organizacion del proyecto anterior</param>
        /// <returns>Petición generada</returns>
        public Peticion AgregarPeticionDeAccesoGrupoYComUsuario(List<Guid> pGruposID, Guid pProyectoID, Guid pOrganizacionID)
        {
            string grupos = "";
            foreach (Guid GrupoID in pGruposID)
            {
                grupos += grupos +","+GrupoID.ToString();
            }

            AD.EntityModel.Models.Peticion.Peticion filaPeticion = new AD.EntityModel.Models.Peticion.Peticion();
            filaPeticion.PeticionID = Guid.NewGuid();
            filaPeticion.FechaPeticion = DateTime.Now;
            filaPeticion.Tipo = (short)TipoPeticion.AccesoAGruposYComunidad;
            filaPeticion.Estado = (short)EstadoPeticion.Pendiente;
            PeticionDW.ListaPeticion.Add(filaPeticion);
            mEntityContext.Peticion.Add(filaPeticion);

            PeticionInvitacionGrupo filaPeticionGrupo = new PeticionInvitacionGrupo();
            filaPeticionGrupo.PeticionID = filaPeticion.PeticionID;
            filaPeticionGrupo.OrganizacionID = pOrganizacionID;
            filaPeticionGrupo.ProyectoID = pProyectoID;
            filaPeticionGrupo.GruposID = grupos;
            PeticionDW.ListaPeticionInvitacionGrupo.Add(filaPeticionGrupo);
            mEntityContext.PeticionInvitacionGrupo.Add(filaPeticionGrupo);

            PeticionInvitacionComunidad filaPeticionCom = new PeticionInvitacionComunidad();
            filaPeticionCom.ProyectoID = pProyectoID;
            filaPeticionCom.OrganizacionID = pOrganizacionID;
            filaPeticionCom.PeticionID = filaPeticion.PeticionID;
            PeticionDW.ListaPeticionInvitacionComunidad.Add(filaPeticionCom);
            mEntityContext.PeticionInvitacionComunidad.Add(filaPeticionCom);

            Peticion peticion = new PeticionInvGrupoYComunidad(filaPeticionGrupo, filaPeticionCom, filaPeticion, this, mLoggingService);

            if (!ListaPeticiones.ContainsKey(peticion.Clave))
            {
                this.ListaPeticiones.Add(peticion.Clave, peticion);
                this.Hijos.Add(peticion);
            }
            return peticion;
        }
                
        /// <summary>
        /// Crea una petición de creación de una comunidad reservada
        /// </summary>
        /// <param name="pNombre">Nombre de la comunidad</param>
        /// <param name="pNombreCorto">Nombre corto de la comunidad</param>
        /// <param name="pDescripcion">Descripcion de la comunidad</param>
        /// <param name="pTipo">Tipo de la comunidad</param>
        /// <param name="pUsuarioID">GUID del usuario que solicita la comunidad</param>
        /// <param name="pComunidadPrivadaPadreID">GUID de la comunidad padre de la comunidad reservada que se solicita</param>
        /// <param name="pPerfilCreadorID">GUID del perfil del que solicita la comunidad</param>
        /// <returns>Petición generada</returns>
        public Peticion AgregarPeticionDeNuevoProyecto(string pNombre, string pNombreCorto, string pDescripcion, Int16 pTipo, Guid pUsuarioID, Guid pComunidadPrivadaPadreID, Guid pPerfilCreadorID)
        {
            AD.EntityModel.Models.Peticion.Peticion filaPeticion = new AD.EntityModel.Models.Peticion.Peticion();
            filaPeticion.PeticionID = Guid.NewGuid();
            filaPeticion.FechaPeticion = DateTime.Now;
            filaPeticion.Tipo = (short)TipoPeticion.NuevoProyecto;
            filaPeticion.Estado = (short)EstadoPeticion.Pendiente;
            filaPeticion.UsuarioID = pUsuarioID;
            PeticionDW.ListaPeticion.Add(filaPeticion);
            mEntityContext.Peticion.Add(filaPeticion);

            
            AD.EntityModel.Models.Peticion.PeticionNuevoProyecto filaPeticionNuevoProy = new AD.EntityModel.Models.Peticion.PeticionNuevoProyecto();
            filaPeticionNuevoProy.PeticionID = filaPeticion.PeticionID;
            filaPeticionNuevoProy.Nombre = pNombre;
            filaPeticionNuevoProy.NombreCorto = pNombreCorto;
            filaPeticionNuevoProy.Descripcion = pDescripcion;
            filaPeticionNuevoProy.Tipo = pTipo;
            filaPeticionNuevoProy.ComunidadPrivadaPadreID = pComunidadPrivadaPadreID;
            filaPeticionNuevoProy.PerfilCreadorID = pPerfilCreadorID;
            filaPeticionNuevoProy.Peticion = filaPeticion;
            PeticionDW.ListaPeticionNuevoProyecto.Add(filaPeticionNuevoProy);
            mEntityContext.PeticionNuevoProyecto.Add(filaPeticionNuevoProy);

            Peticion peticion = new PeticionNuevoProyecto(filaPeticionNuevoProy, filaPeticion, this, mLoggingService);

            if (!ListaPeticiones.ContainsKey(peticion.Clave))
            {
                this.ListaPeticiones.Add(peticion.Clave, peticion);
                this.Hijos.Add(peticion);
            }
            return peticion;
        }

        /// <summary>
        /// Crea una petición de creación de una comunidad diferente de reservada
        /// </summary>
        /// <param name="pNombre">Nombre de la comunidad</param>
        /// <param name="pNombreCorto">Nombre corto de la comunidad</param>
        /// <param name="pDescripcion">Descripcion de la comunidad</param>
        /// <param name="pTipo">Tipo de la comunidad</param>
        /// <param name="pUsuarioID">GUID del usuario que solicita la comunidad</param>
        /// <param name="pPerfilCreadorID">GUID del perfil que solicita la conunidad</param>
        /// <returns>Petición generada</returns>
        public Peticion AgregarPeticionDeNuevoProyecto(String pNombre, String pNombreCorto, String pDescripcion, Int16 pTipo, Guid pUsuarioID, Guid pPerfilCreadorID)
        {
            AD.EntityModel.Models.Peticion.Peticion filaPeticion = new AD.EntityModel.Models.Peticion.Peticion();
            filaPeticion.PeticionID = Guid.NewGuid();
            filaPeticion.FechaPeticion = DateTime.Now;
            filaPeticion.Tipo = (short)TipoPeticion.NuevoProyecto;
            filaPeticion.Estado = (short)EstadoPeticion.Pendiente;
            filaPeticion.UsuarioID = pUsuarioID;
            PeticionDW.ListaPeticion.Add(filaPeticion);
            mEntityContext.Peticion.Add(filaPeticion);

            AD.EntityModel.Models.Peticion.PeticionNuevoProyecto filaPeticionNuevoProy = new AD.EntityModel.Models.Peticion.PeticionNuevoProyecto();
            filaPeticionNuevoProy.PeticionID = filaPeticion.PeticionID;
            filaPeticionNuevoProy.Nombre = pNombre;
            filaPeticionNuevoProy.NombreCorto = pNombreCorto;
            filaPeticionNuevoProy.Descripcion = pDescripcion;
            filaPeticionNuevoProy.Tipo = pTipo;
            filaPeticionNuevoProy.PerfilCreadorID = pPerfilCreadorID;
            filaPeticionNuevoProy.Peticion = filaPeticion;
            PeticionDW.ListaPeticionNuevoProyecto.Add(filaPeticionNuevoProy);
            mEntityContext.PeticionNuevoProyecto.Add(filaPeticionNuevoProy);

            Peticion peticion = new PeticionNuevoProyecto(filaPeticionNuevoProy, filaPeticion, this, mLoggingService);

            if (!ListaPeticiones.ContainsKey(peticion.Clave))
            {
                this.ListaPeticiones.Add(peticion.Clave, peticion);
                this.Hijos.Add(peticion);
            }
            return peticion;
        }

        /// <summary>
        /// Carga las peticiones desde el dataSet
        /// </summary>
        public void CargarPeticiones()
        {
            mListaPeticiones = new Dictionary<Guid, Peticion>();
            mHijos = new List<IElementoGnoss>();

            foreach (PeticionOrgInvitaPers peticionOrg in PeticionDW.ListaPeticionOrgInvitaPers.ToList())
            {
                Peticion peticion = new PeticionInvOrganizacion(peticionOrg, peticionOrg.Peticion, this, mLoggingService);

                mListaPeticiones.Add(peticion.Clave, peticion);
                mHijos.Add(peticion);
            }

            foreach (PeticionInvitacionComunidad peticionCom in PeticionDW.ListaPeticionInvitacionComunidad.ToList())
            {
                Peticion peticion = new PeticionInvComunidad(peticionCom, peticionCom.Peticion, this, mLoggingService);

                mListaPeticiones.Add(peticion.Clave, peticion);
                mHijos.Add(peticion);
            }

            foreach (AD.EntityModel.Models.Peticion.PeticionNuevoProyecto peticionNuevoProy in PeticionDW.ListaPeticionNuevoProyecto.ToList())
            {
                Peticion peticion = new PeticionNuevoProyecto(peticionNuevoProy, peticionNuevoProy.Peticion, this, mLoggingService);

                mListaPeticiones.Add(peticion.Clave, peticion);
                mHijos.Add(peticion);
            }

            foreach (AD.EntityModel.Models.Peticion.Peticion filaPet in PeticionDW.ListaPeticion.ToList())
            {
                if (!mListaPeticiones.ContainsKey(filaPet.PeticionID))
                {
                    Peticion peticion = new Peticion(filaPet, this, mLoggingService);

                    mListaPeticiones.Add(peticion.Clave, peticion);
                    mHijos.Add(peticion);
                }
            }
        }

        /// <summary>
        /// Elimina los registros de peticiones del usuario pasado como parámetro
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        public void EliminarPeticionesPorUsuarioID(Guid pUsuarioID)
        {
            foreach (AD.EntityModel.Models.Peticion.Peticion fila in PeticionDW.ListaPeticion.Where(item => item.UsuarioID.Equals(pUsuarioID)).ToList())
            {
                //Si es una petición de invitación a comunidad, la eliminamos
                PeticionInvitacionComunidad filaInvitaComunidad = PeticionDW.ListaPeticionInvitacionComunidad.Where(item => item.PeticionID.Equals(fila.PeticionID)).FirstOrDefault();

                if (filaInvitaComunidad != null)
                {
                    mEntityContext.EliminarElemento(filaInvitaComunidad);
                    PeticionDW.ListaPeticionInvitacionComunidad.Remove(filaInvitaComunidad);
                }

                //Si es una petición de nueva comunidad, la eliminamos
                AD.EntityModel.Models.Peticion.PeticionNuevoProyecto filaNuevoProy = PeticionDW.ListaPeticionNuevoProyecto.Where(item => item.PeticionID.Equals(fila.PeticionID)).FirstOrDefault();

                if (filaNuevoProy != null)
                {
                    mEntityContext.EliminarElemento(filaNuevoProy);
                    PeticionDW.ListaPeticionNuevoProyecto.Remove(filaNuevoProy);
                }

                //Si es una petición de invitación a organización la eliminamos
                PeticionOrgInvitaPers filaInvitaOrg = PeticionDW.ListaPeticionOrgInvitaPers.Where(item => item.PeticionID.Equals(fila.PeticionID)).FirstOrDefault();

                if (filaInvitaOrg != null)
                {
                    mEntityContext.EliminarElemento(filaInvitaOrg);
                    PeticionDW.ListaPeticionOrgInvitaPers.Remove(filaInvitaOrg);
                }
                //Eliminamos la petición
                mEntityContext.EliminarElemento(fila);
                PeticionDW.ListaPeticion.Remove(fila);
            }
        }

        #endregion

        #region Miembros de ISerializable

        /// <summary>
        /// Método para serializar el objeto
        /// </summary>
        /// <param name="pInfo">Datos serializados</param>
        /// <param name="pContext">Contexto de serialización</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo pInfo, StreamingContext pContext)
        {
            base.GetObjectData(pInfo, pContext);

            pInfo.AddValue("GestionNotificaciones", GestorNotificaciones);
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
        ~GestionPeticiones()
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
                        //Liberamos todos los recursos administrados que hemos añadido a esta clase
                        foreach (Peticion peticion in ListaPeticiones.Values)
                        {
                            peticion.Dispose();
                        }
                    }
                }
                finally
                {
                    mGestionNotificaciones = null;

                    // Llamamos al dispose de la clase base
                    base.Dispose(pDisposing);
                }
            }
        }

        #endregion
    }
}
