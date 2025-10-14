using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.Suscripcion;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.Notificacion;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Tesauro;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Es.Riam.Gnoss.Elementos.Suscripcion
{
    /// <summary>
    /// Gestor de suscripciones
    /// </summary>
    [Serializable]
    public class GestionSuscripcion : GestionGnoss
    {
        #region Miembros

        /// <summary>
        /// Lista de suscripciones
        /// </summary>
        private Dictionary<Guid, Suscripcion> mListaSuscripciones;

        /// <summary>
        /// Gestor de identidades
        /// </summary>
        private GestionIdentidades mGestionIdentidades;

        /// <summary>
        /// Gestor de proyectos
        /// </summary>
        private GestionProyecto mGestionProyecto;

        /// <summary>
        /// Gestor de notificaciones
        /// </summary>
        private GestionNotificaciones mGestionNotificaciones;

        private EntityContext mEntityContext;

        private LoggingService mLoggingService;
        #endregion

        #region Constructores

        public GestionSuscripcion() { }

        /// <summary>
        /// Crea el gestor de suscripciones
        /// </summary>
        /// <param name="pSuscripcionDW">Dataset de suscripciones</param>
        public GestionSuscripcion(DataWrapperSuscripcion pSuscripcionDW, LoggingService loggingService, EntityContext entityContext)
            : base(pSuscripcionDW)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
        }

        /// <summary>
        /// Crea el gestor de suscripciones
        /// </summary>
        /// <param name="pSuscripcionDW">Dataset de suscripciones</param>
        /// <param name="pGestionIdentidades">Gestor de identidades</param>
        public GestionSuscripcion(DataWrapperSuscripcion pSuscripcionDW, GestionIdentidades pGestionIdentidades, LoggingService loggingService, EntityContext entityContext)
            : this(pSuscripcionDW, loggingService, entityContext)
        {
            mGestionIdentidades = pGestionIdentidades;;
        }

        /// <summary>
        /// Crea el gestor de suscripciones
        /// </summary>
        /// <param name="pSuscripcionDW">Dataset de suscripciones</param>
        /// <param name="pGestionProyecto">Gestor de proyectos</param>
        public GestionSuscripcion(DataWrapperSuscripcion pSuscripcionDW, GestionProyecto pGestionProyecto, LoggingService loggingService, EntityContext entityContext)
            : this(pSuscripcionDW, loggingService, entityContext)
        {
            mGestionProyecto = pGestionProyecto;
        }

        /// <summary>
        /// Constructor para la deseralización
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serialización</param>
        protected GestionSuscripcion(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            //mLoggingService = loggingService;
            //mEntityContext = entityContext;

            mGestionNotificaciones = (GestionNotificaciones)info.GetValue("GestionNotificaciones", typeof(GestionNotificaciones));
            mGestionProyecto = (GestionProyecto)info.GetValue("GestionProyecto", typeof(GestionProyecto));
            mGestionIdentidades = (GestionIdentidades)info.GetValue("GestionIdentidades", typeof(GestionIdentidades));
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene el dataset de suscripciones
        /// </summary>
        public DataWrapperSuscripcion SuscripcionDW
        {
            get
            {
                return (DataWrapperSuscripcion)DataWrapper;
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

        /// <summary>
        /// Obtiene o establece el gestor de identidades
        /// </summary>
        public GestionIdentidades GestorIdentidades
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
        /// Obtiene o establece el gestor de proyectos
        /// </summary>
        public GestionProyecto GestorProyecto
        {
            get
            {
                return mGestionProyecto;
            }
            set
            {
                mGestionProyecto = value;
            }
        }

        /// <summary>
        /// Obtiene la lista de suscripciones (Identificador, Suscripción)
        /// </summary>
        public Dictionary<Guid, Suscripcion> ListaSuscripciones
        {
            get
            {
                if (mListaSuscripciones == null)
                {
                    mListaSuscripciones = new Dictionary<Guid, Suscripcion>();
                    List<AD.EntityModel.Models.Suscripcion.Suscripcion> filasSuscripcion = SuscripcionDW.ListaSuscripcion.ToList();
                    foreach (AD.EntityModel.Models.Suscripcion.Suscripcion filaSuscripcion in filasSuscripcion)
                    {
                        Suscripcion suscripcion = new Suscripcion(filaSuscripcion, this);
                        mListaSuscripciones.Add(suscripcion.FilaSuscripcion.SuscripcionID, suscripcion);
                    }
                }
                return mListaSuscripciones;
            }
        }       

        #endregion

        #region Métodos

        /// <summary>
        /// Elimina las suscripciones al usuario pasado por parámetro
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        public void EliminarSuscripcionAUsuario(Guid pUsuarioID)
        {
            List<AD.EntityModel.Models.Suscripcion.SuscripcionTesauroUsuario> filasSuscUsuario = SuscripcionDW.ListaSuscripcionTesauroUsuario.Where(item => item.UsuarioID.Equals(pUsuarioID)).ToList();

            foreach (AD.EntityModel.Models.Suscripcion.SuscripcionTesauroUsuario filaSuscUsu in filasSuscUsuario)
            {
                Guid idSusc = filaSuscUsu.SuscripcionID;
                SuscripcionDW.ListaSuscripcionTesauroUsuario.Remove(filaSuscUsu);
                mEntityContext.EliminarElemento(filaSuscUsu);

                if (ListaSuscripciones[idSusc].FilaRelacion == null)
                {
                    SuscripcionDW.ListaSuscripcion.Remove(ListaSuscripciones[idSusc].FilaSuscripcion);
                    mEntityContext.EliminarElemento(ListaSuscripciones[idSusc].FilaSuscripcion);
                    ListaSuscripciones.Remove(idSusc);
                }
            }            
        }

        /// <summary>
        /// Crea una nueva suscripción
        /// </summary>
        /// <param name="pIdentidad">Identidad</param>
        /// <returns>Nueva suscripción</returns>
        public Suscripcion CrearSuscripcion(Identidad.Identidad pIdentidad)
        {
            return CrearSuscripcion(pIdentidad.Clave);
        }

        /// <summary>
        /// Crea una nueva suscripción
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <returns>Nueva suscripción</returns>
        public Suscripcion CrearSuscripcion(Guid pIdentidadID)
        {
            AD.EntityModel.Models.Suscripcion.Suscripcion filaSuscripcion = new AD.EntityModel.Models.Suscripcion.Suscripcion();
            filaSuscripcion.SuscripcionID = Guid.NewGuid();
            filaSuscripcion.IdentidadID = pIdentidadID;
            filaSuscripcion.Bloqueada = false;
            filaSuscripcion.Periodicidad = (int)PeriodicidadSuscripcion.Diaria;
            filaSuscripcion.UltimoEnvio = DateTime.Now;
            filaSuscripcion.FechaSuscripcion = DateTime.Now;
            SuscripcionDW.ListaSuscripcion.Add(filaSuscripcion);
            mEntityContext.Suscripcion.Add(filaSuscripcion);

            Suscripcion suscripcion = new Suscripcion(filaSuscripcion, this);

            if (!ListaSuscripciones.ContainsKey(suscripcion.FilaSuscripcion.SuscripcionID))
            {
                ListaSuscripciones.Add(suscripcion.FilaSuscripcion.SuscripcionID, suscripcion);
            }
            return suscripcion;
        }

        public void CrearSuscripcionTesauroProyecto(Guid pSuscripcionID, Guid pOrganizacionID, Guid pProyectoID, Guid pTesauroID)
        {
            AD.EntityModel.Models.Suscripcion.SuscripcionTesauroProyecto filaTesauro = new AD.EntityModel.Models.Suscripcion.SuscripcionTesauroProyecto();
            filaTesauro.SuscripcionID = pSuscripcionID;
            filaTesauro.ProyectoID = pProyectoID;
            filaTesauro.OrganizacionID = pOrganizacionID;
            filaTesauro.TesauroID = pTesauroID;
            SuscripcionDW.ListaSuscripcionTesauroProyecto.Add(filaTesauro);
            mEntityContext.SuscripcionTesauroProyecto.Add(filaTesauro);
        }

        /// <summary>
        /// Elimina una suscripción entera
        /// </summary>
        /// <param name="pSuscripcionID">Identificador de suscripción</param>
        public void EliminarSuscripcion(Guid pSuscripcionID)
        {
            if (!ListaSuscripciones.ContainsKey(pSuscripcionID)) { return; }

            Suscripcion susc = ListaSuscripciones[pSuscripcionID];

            switch (susc.Tipo)
            {
                case TipoSuscripciones.Comunidades:
                    if (susc.FilasCategoriasVinculadas != null)
                    {
                        foreach (AD.EntityModel.Models.Suscripcion.CategoriaTesVinSuscrip fila in susc.FilasCategoriasVinculadas.ToList())
                        {
                            susc.FilasCategoriasVinculadas.Remove(fila);
                            mEntityContext.EliminarElemento(fila);
                        }
                    }
                    mEntityContext.EliminarElemento(susc.FilaRelacion);
                    susc.FilaRelacion = null;

                    mEntityContext.EliminarElemento(susc.FilaSuscripcion);
                    susc.FilaSuscripcion = null;
                    ListaSuscripciones.Remove(pSuscripcionID);
                    break;
                case TipoSuscripciones.Personas:
                    mEntityContext.EliminarElemento(susc.FilaRelacion);
                    susc.FilaRelacion = null;
                    
                    foreach (AD.EntityModel.Models.Suscripcion.SuscripcionIdentidadProyecto filaSuscIden in susc.FilaSuscripcionIdentidadProyecto)
                    {
                        mEntityContext.EliminarElemento(filaSuscIden);
                        susc.FilaSuscripcionIdentidadProyecto = null;
                    }

                    mEntityContext.EliminarElemento(susc.FilaSuscripcion);
                    susc.FilaSuscripcion = null;

                    ListaSuscripciones.Remove(pSuscripcionID);
                    break;
            }
        }

        /// <summary>
        /// Elimina una suscripción entera
        /// </summary>
        /// <param name="pSuscripcionID">Identificador de suscripción</param>
        public void EliminarSuscripcionAIdentidadEnProyecto(Guid pSuscripcionID,Guid pProyectoID)
        {
            if (!ListaSuscripciones.ContainsKey(pSuscripcionID)) { return; }

            Suscripcion susc = ListaSuscripciones[pSuscripcionID];

            if (susc.FilaSuscripcionIdentidadProyecto != null)
            {
                foreach (AD.EntityModel.Models.Suscripcion.SuscripcionIdentidadProyecto fila in susc.FilaSuscripcionIdentidadProyecto)
                {
                    if (fila.ProyectoID == pProyectoID)
                    {
                        mEntityContext.EliminarElemento(fila);
                        susc.FilaSuscripcionIdentidadProyecto = null;
                    }
                }
            }

            if (susc.FilaSuscripcionIdentidadProyecto == null && susc.FilaRelacion == null)
            {
                mEntityContext.EliminarElemento(susc.FilaSuscripcion);
                susc.FilaSuscripcion = null;
                ListaSuscripciones.Remove(pSuscripcionID);
            }
        }

        /// <summary>
        /// Crea una nueva suscripción a una categoría de tesauro
        /// </summary>
        /// <param name="pSuscripcion">Suscripción a una categoría de tesauro</param>
        /// <param name="pCategoria">Categoría de tesauro</param>
        public void VincularCategoria(Suscripcion pSuscripcion, CategoriaTesauro pCategoria)
        {
            AD.EntityModel.Models.Suscripcion.CategoriaTesVinSuscrip filaCategoriaVinculada = new AD.EntityModel.Models.Suscripcion.CategoriaTesVinSuscrip();
            filaCategoriaVinculada.SuscripcionID = pSuscripcion.Clave;
            filaCategoriaVinculada.CategoriaTesauroID = pCategoria.Clave;
            filaCategoriaVinculada.TesauroID = pCategoria.FilaCategoria.TesauroID;

            if (SuscripcionDW.ListaCategoriaTesVinSuscrip.Where(item => item.SuscripcionID.Equals(pSuscripcion.Clave) && item.TesauroID.Equals(pCategoria.FilaCategoria.TesauroID) && item.CategoriaTesauroID.Equals(pCategoria.Clave)).FirstOrDefault() == null)
            {
                SuscripcionDW.ListaCategoriaTesVinSuscrip.Add(filaCategoriaVinculada);
                mEntityContext.CategoriaTesVinSuscrip.Add(filaCategoriaVinculada);
            }
        }

        /// <summary>
        /// Elimina una suscripción a una categoría de tesauro
        /// </summary>
        /// <param name="pSuscripcion">Suscripción a una categoría de tesauro</param>
        /// <param name="pCategoria">Categoría de tesauro</param>
        public void DesvincularCategoria(Suscripcion pSuscripcion, CategoriaTesauro pCategoria)
        {
            List<AD.EntityModel.Models.Suscripcion.CategoriaTesVinSuscrip> filasCategoriaVinculada = SuscripcionDW.ListaCategoriaTesVinSuscrip.Where(item => item.CategoriaTesauroID.Equals(pCategoria.Clave) && item.SuscripcionID.Equals(pSuscripcion.FilaSuscripcion.SuscripcionID)).ToList();

            if (filasCategoriaVinculada.Count > 0)
            {
                SuscripcionDW.ListaCategoriaTesVinSuscrip.Remove(filasCategoriaVinculada[0]);
                mEntityContext.EliminarElemento(filasCategoriaVinculada[0]);
            }
        }


        /// <summary>
        /// Crea una nueva suscripción
        /// </summary>
        /// <param name="pIdentidadOrigen"></param>
        /// <param name="pPeriodicidad"></param>
        /// <returns></returns>
        public Guid AgregarNuevaSuscripcion(Identidad.Identidad pIdentidadOrigen, int pPeriodicidad)
        {
            AD.EntityModel.Models.Suscripcion.Suscripcion filaSuscripcion = new AD.EntityModel.Models.Suscripcion.Suscripcion();
            filaSuscripcion.SuscripcionID = Guid.NewGuid();
            filaSuscripcion.IdentidadID = pIdentidadOrigen.Clave;
            filaSuscripcion.Periodicidad = pPeriodicidad;
            filaSuscripcion.UltimoEnvio = DateTime.Now;
            filaSuscripcion.FechaSuscripcion = DateTime.Now;
            filaSuscripcion.Bloqueada = false;
            SuscripcionDW.ListaSuscripcion.Add(filaSuscripcion);
            mEntityContext.Suscripcion.Add(filaSuscripcion);

            Suscripcion suscripcion = new Suscripcion(filaSuscripcion, this);

            if (!ListaSuscripciones.ContainsKey(suscripcion.FilaSuscripcion.SuscripcionID))
            {
                ListaSuscripciones.Add(suscripcion.FilaSuscripcion.SuscripcionID, suscripcion);
            }
            return filaSuscripcion.SuscripcionID;
        }

        /// <summary>
        /// Crea una nueva suscripción al tesauro de un usuario
        /// </summary>
        /// <param name="pUsuarioID">Identifiocador del usuario</param>
        /// <param name="pTesauroID">Identificador del tesauro</param>
        /// <param name="pPeriodicidad">Periodicidad de la suscripción</param>
        /// <param name="pSuscripcionID">Suscripcion a la que se agrega</param>
        public void AgregarSuscripcionAUsuario(Guid pUsuarioID, Guid pTesauroID, int pPeriodicidad,Guid pSuscripcionID)
        {         
            AD.EntityModel.Models.Suscripcion.SuscripcionTesauroUsuario filaSuscUsuario = new AD.EntityModel.Models.Suscripcion.SuscripcionTesauroUsuario();
            filaSuscUsuario.SuscripcionID = pSuscripcionID;
            filaSuscUsuario.UsuarioID = pUsuarioID;
            filaSuscUsuario.TesauroID = pTesauroID;
            SuscripcionDW.ListaSuscripcionTesauroUsuario.Add(filaSuscUsuario);
            mEntityContext.SuscripcionTesauroUsuario.Add(filaSuscUsuario);
        }

        /// <summary>
        /// Crea una nueva suscripción al tesauro de una organizacion
        /// </summary>
        /// <param name="pUsuarioID">Identifiocador de la organizacion</param>
        /// <param name="pTesauroID">Identificador del tesauro</param>
        /// <param name="pPeriodicidad">Periodicidad de la suscripción</param>
        /// <param name="pSuscripcionID">Suscripcion a la que se agrega</param>
        public void AgregarSuscripcionAOrganizacion(Guid pOrganizacionID, Guid pTesauroID, int pPeriodicidad, Guid pSuscripcionID)
        {
            AD.EntityModel.Models.Suscripcion.SuscripcionTesauroOrganizacion filaSuscOrganizacion = new AD.EntityModel.Models.Suscripcion.SuscripcionTesauroOrganizacion();
            filaSuscOrganizacion.SuscripcionID = pSuscripcionID;
            filaSuscOrganizacion.OrganizacionID = pOrganizacionID;
            filaSuscOrganizacion.TesauroID = pTesauroID;
            SuscripcionDW.ListaSuscripcionTesauroOrganizacion.Add(filaSuscOrganizacion);
            mEntityContext.SuscripcionTesauroOrganizacion.Add(filaSuscOrganizacion);
        }

        /// <summary>
        /// Crea una nueva suscripción a una identidad en una comunidad
        /// </summary>
        /// <param name="pIdentidadDestinoID">Identidad destino</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pOrganizacionID">Identificador de la oreganizacion del proyecto</param>
        /// <param name="pPeriodicidad">Periodicidad de la suscripción</param>
        /// <param name="pSuscripcionID">Suscripcion a la que se agrega</param>
        public void AgregarSuscripcionAUsuarioEnProyecto(Guid pIdentidadDestinoID, Guid pProyectoID, Guid pOrganizacionID, int pPeriodicidad, Guid pSuscripcionID)
        {
            AD.EntityModel.Models.Suscripcion.SuscripcionIdentidadProyecto filaSuscIdenProy = new AD.EntityModel.Models.Suscripcion.SuscripcionIdentidadProyecto();
            filaSuscIdenProy.SuscripcionID = pSuscripcionID;
            filaSuscIdenProy.IdentidadID = pIdentidadDestinoID;
            filaSuscIdenProy.ProyectoID = pProyectoID;
            filaSuscIdenProy.OrganizacionID = pOrganizacionID;
            SuscripcionDW.ListaSuscripcionIdentidadProyecto.Add(filaSuscIdenProy);
            mEntityContext.SuscripcionIdentidadProyecto.Add(filaSuscIdenProy);

        }

        /// <summary>
        /// Elimina TODAS las suscripciones de cualquier tipo que pueda tener una Identidad. Nota: cargar el gestor de notificaciones para que se pueda eliminar de "NotificacionSuscripcion"
        /// </summary>
        /// <param name="pIdentidadID">Identificador de identidad</param>
        public void EliminarSuscripciones(Guid pIdentidadID)
        {
            foreach (AD.EntityModel.Models.Suscripcion.Suscripcion filaSuscripcion in SuscripcionDW.ListaSuscripcion.Where(item => item.IdentidadID.Equals(pIdentidadID)).ToList())
            {
                //Elimino si hay de "SuscripcionTesauroUsuario"
                foreach (AD.EntityModel.Models.Suscripcion.SuscripcionTesauroUsuario filaSuscripcionTesauroUsuario in SuscripcionDW.ListaSuscripcionTesauroUsuario.Where(item => item.SuscripcionID.Equals(filaSuscripcion.SuscripcionID)).ToList())
                {
                    mEntityContext.EliminarElemento(filaSuscripcionTesauroUsuario);
                    SuscripcionDW.ListaSuscripcionTesauroUsuario.Remove(filaSuscripcionTesauroUsuario);
                }

                //Elimino si hay de "CategoriaTesVinSuscrip"
                foreach (AD.EntityModel.Models.Suscripcion.CategoriaTesVinSuscrip filaCategoriaTesVinSuscrip in SuscripcionDW.ListaCategoriaTesVinSuscrip.Where(item => item.SuscripcionID.Equals(filaSuscripcion.SuscripcionID)).ToList())
                {
                    mEntityContext.EliminarElemento(filaCategoriaTesVinSuscrip);
                    SuscripcionDW.ListaCategoriaTesVinSuscrip.Remove(filaCategoriaTesVinSuscrip);
                }

                //Elimino si hay de "SuscripcionTesauroProyecto"
                foreach (AD.EntityModel.Models.Suscripcion.SuscripcionTesauroProyecto filaSuscripcionTesauroProyecto in SuscripcionDW.ListaSuscripcionTesauroProyecto.Where(item => item.SuscripcionID.Equals(filaSuscripcion.SuscripcionID)).ToList())
                {
                    mEntityContext.EliminarElemento(filaSuscripcionTesauroProyecto);
                    SuscripcionDW.ListaSuscripcionTesauroProyecto.Remove(filaSuscripcionTesauroProyecto);
                }

                //Elimino si hay de "SuscripcionIdentidadProyecto"
                foreach (AD.EntityModel.Models.Suscripcion.SuscripcionIdentidadProyecto filaSuscripcionIdentidadProyecto in SuscripcionDW.ListaSuscripcionIdentidadProyecto.Where(item => item.SuscripcionID.Equals(filaSuscripcion.SuscripcionID)).ToList())
                {
                    mEntityContext.EliminarElemento(filaSuscripcionIdentidadProyecto);
                    SuscripcionDW.ListaSuscripcionIdentidadProyecto.Remove(filaSuscripcionIdentidadProyecto);
                }

                List<Guid> listaNotificaciones = new List<Guid>();

                //Elimino todo las notificaciones relacionadas con las suscruipciones 
                foreach (AD.EntityModel.Models.Notificacion.NotificacionSuscripcion filaNotificacionSuscripcion in mGestionNotificaciones.NotificacionDW.ListaNotificacionSuscripcion.Where(item => item.SuscripcionID.Equals(filaSuscripcion.SuscripcionID)))
                {
                    Guid notificacionID = filaNotificacionSuscripcion.NotificacionID;

                    if (!listaNotificaciones.Contains(notificacionID))
                    {
                        listaNotificaciones.Add(notificacionID);
                    }
                    //Elimino si hay de "NotificacionParametro"
                    foreach (AD.EntityModel.Models.Notificacion.NotificacionParametro filaNotificacionParametro in mGestionNotificaciones.NotificacionDW.ListaNotificacionParametro.Where(item => item.NotificacionID.Equals(notificacionID)))
                    {
                        mGestionNotificaciones.NotificacionDW.ListaNotificacionParametro.Remove(filaNotificacionParametro);
                        mEntityContext.EliminarElemento(filaNotificacionParametro);
                    }
                    //Elimino si hay de "NotificacionParametroPersona"
                    foreach (AD.EntityModel.Models.Notificacion.NotificacionParametroPersona filaNotificacionParametroPersona in mGestionNotificaciones.NotificacionDW.ListaNotificacionParametroPersona.Where(item => item.NotificacionID.Equals(notificacionID)))
                    {
                        mGestionNotificaciones.NotificacionDW.ListaNotificacionParametroPersona.Remove(filaNotificacionParametroPersona);
                        mEntityContext.EliminarElemento(filaNotificacionParametroPersona);
                    }
                    //Elimino si hay de "NotificacionCorreoPersona"
                    foreach (AD.EntityModel.Models.Notificacion.NotificacionCorreoPersona filaNotificacionCorreoPersona in mGestionNotificaciones.NotificacionDW.ListaNotificacionCorreoPersona.Where(item => item.NotificacionID.Equals(notificacionID)))
                    {
                        mGestionNotificaciones.NotificacionDW.ListaNotificacionCorreoPersona.Remove(filaNotificacionCorreoPersona);
                        mEntityContext.EliminarElemento(filaNotificacionCorreoPersona);
                    }
                    //Elimino si hay de "NotificacionAlertaPersona"
                    foreach (AD.EntityModel.Models.Notificacion.NotificacionAlertaPersona filaNotificacionAlertaPersona in mGestionNotificaciones.NotificacionDW.ListaNotificacionAlertaPersona.Where(item => item.NotificacionID.Equals(notificacionID)))
                    {
                        mGestionNotificaciones.NotificacionDW.ListaNotificacionAlertaPersona.Remove(filaNotificacionAlertaPersona);
                        mEntityContext.EliminarElemento(filaNotificacionAlertaPersona);
                    }
                }
                //Elimino todas "NotificacionSuscripcion"
                foreach (AD.EntityModel.Models.Notificacion.NotificacionSuscripcion filaNotificacionSuscripcion in mGestionNotificaciones.NotificacionDW.ListaNotificacionSuscripcion.Where(item => item.SuscripcionID.Equals(filaSuscripcion.SuscripcionID)))
                {
                    mGestionNotificaciones.NotificacionDW.ListaNotificacionSuscripcion.Remove(filaNotificacionSuscripcion);
                    mEntityContext.EliminarElemento(filaNotificacionSuscripcion);
                }

                foreach (Guid notificacionID in listaNotificaciones)
                {
                    if (mGestionNotificaciones.NotificacionDW.ListaNotificacion.Any(item => item.NotificacionID.Equals(notificacionID)))
                    {
                        if (!mEntityContext.Entry(mGestionNotificaciones.NotificacionDW.ListaNotificacion.Where(item => item.NotificacionID.Equals(notificacionID)).FirstOrDefault()).State.Equals(EntityState.Deleted))
                        {
                            AD.EntityModel.Models.Notificacion.Notificacion notificacion = mGestionNotificaciones.NotificacionDW.ListaNotificacion.Where(item => item.NotificacionID.Equals(notificacionID)).FirstOrDefault();

                            mGestionNotificaciones.NotificacionDW.ListaNotificacion.Remove(notificacion);
                            mEntityContext.EliminarElemento(notificacion);
                        }
                    }
                }

                //Elimino "Suscripcion"
                mEntityContext.EliminarElemento(filaSuscripcion);
                SuscripcionDW.ListaSuscripcion.Remove(filaSuscripcion);
            }
        }

        /// <summary>
        /// Elimina TODAS las suscripciones a identidades que pueda tener una Identidad en un proyecto. Nota: cargar el gestor de notificaciones para que se pueda eliminar de "NotificacionSuscripcion"
        /// </summary>
        /// <param name="pIdentidadID">Identificador de identidad</param>
        ///<param name="pProyectoID">Identificador del proyecto</param>
        public void EliminarSuscripcionesDeIdentidadEnProyecto(Guid pIdentidadID,Guid pProyectoID)
        {
            foreach (AD.EntityModel.Models.Suscripcion.Suscripcion filaSuscripcion in SuscripcionDW.ListaSuscripcion.Where(item => item.IdentidadID.Equals(pIdentidadID)).ToList())
            {
                bool eliminar = false;
                foreach (AD.EntityModel.Models.Suscripcion.SuscripcionIdentidadProyecto filaSuscripcionIdentidadProyecto in SuscripcionDW.ListaSuscripcionIdentidadProyecto.Where(item => item.SuscripcionID.Equals(filaSuscripcion.SuscripcionID) && item.ProyectoID.Equals(pProyectoID)).ToList())
                {   
                    List<Guid> listaNotificaciones = new List<Guid>();

                    //Elimino todo las notificaciones relacionadas con las suscruipciones 
                    foreach (AD.EntityModel.Models.Notificacion.NotificacionSuscripcion filaNotificacionSuscripcion in mGestionNotificaciones.NotificacionDW.ListaNotificacionSuscripcion.Where(item => item.SuscripcionID.Equals(filaSuscripcionIdentidadProyecto.SuscripcionID)))
                    {
                        Guid notificacionID = filaNotificacionSuscripcion.NotificacionID;

                        if (!listaNotificaciones.Contains(notificacionID))
                        {
                            listaNotificaciones.Add(notificacionID);
                        }
                        //Elimino si hay de "NotificacionParametro"
                        foreach (AD.EntityModel.Models.Notificacion.NotificacionParametro filaNotificacionParametro in mGestionNotificaciones.NotificacionDW.ListaNotificacionParametro.Where(item => item.NotificacionID.Equals(notificacionID)))
                        {
                            mGestionNotificaciones.NotificacionDW.ListaNotificacionParametro.Remove(filaNotificacionParametro);
                            mEntityContext.EliminarElemento(filaNotificacionParametro);
                        }
                        //Elimino si hay de "NotificacionParametroPersona"
                        foreach (AD.EntityModel.Models.Notificacion.NotificacionParametroPersona filaNotificacionParametroPersona in mGestionNotificaciones.NotificacionDW.ListaNotificacionParametroPersona.Where(item => item.NotificacionID.Equals(notificacionID)))
                        {
                            mGestionNotificaciones.NotificacionDW.ListaNotificacionParametroPersona.Remove(filaNotificacionParametroPersona);
                            mEntityContext.EliminarElemento(filaNotificacionParametroPersona);
                        }
                        //Elimino si hay de "NotificacionCorreoPersona"
                        foreach (AD.EntityModel.Models.Notificacion.NotificacionCorreoPersona filaNotificacionCorreoPersona in mGestionNotificaciones.NotificacionDW.ListaNotificacionCorreoPersona.Where(item => item.NotificacionID.Equals(notificacionID)))
                        {
                            mGestionNotificaciones.NotificacionDW.ListaNotificacionCorreoPersona.Remove(filaNotificacionCorreoPersona);
                            mEntityContext.EliminarElemento(filaNotificacionCorreoPersona);
                        }
                        //Elimino si hay de "NotificacionAlertaPersona"
                        foreach (AD.EntityModel.Models.Notificacion.NotificacionAlertaPersona filaNotificacionAlertaPersona in mGestionNotificaciones.NotificacionDW.ListaNotificacionAlertaPersona.Where(item => item.NotificacionID.Equals(notificacionID)))
                        {
                            mGestionNotificaciones.NotificacionDW.ListaNotificacionAlertaPersona.Remove(filaNotificacionAlertaPersona);
                            mEntityContext.EliminarElemento(filaNotificacionAlertaPersona);
                        }
                    }
                    //Elimino todas "NotificacionSuscripcion"
                    foreach (AD.EntityModel.Models.Notificacion.NotificacionSuscripcion filaNotificacionSuscripcion in mGestionNotificaciones.NotificacionDW.ListaNotificacionSuscripcion.Where(item => item.SuscripcionID.Equals(filaSuscripcionIdentidadProyecto.SuscripcionID)))
                    {
                        mGestionNotificaciones.NotificacionDW.ListaNotificacionSuscripcion.Remove(filaNotificacionSuscripcion);
                        mEntityContext.EliminarElemento(filaNotificacionSuscripcion);
                    }

                    foreach (Guid notificacionID in listaNotificaciones)
                    {
                        if (mGestionNotificaciones.NotificacionDW.ListaNotificacion.Any(item => item.NotificacionID.Equals(notificacionID)))
                        {
                            if (!mEntityContext.Entry(mGestionNotificaciones.NotificacionDW.ListaNotificacion.Where(item => item.NotificacionID.Equals(notificacionID))).State.Equals(EntityState.Deleted))
                            {
                                AD.EntityModel.Models.Notificacion.Notificacion notificacion = mGestionNotificaciones.NotificacionDW.ListaNotificacion.Where(item => item.NotificacionID.Equals(notificacionID)).FirstOrDefault();
                                mGestionNotificaciones.NotificacionDW.ListaNotificacion.Remove(notificacion);
                                mEntityContext.EliminarElemento(notificacion);
                            }
                        }
                    }

                    Guid suscripcionID=filaSuscripcionIdentidadProyecto.SuscripcionID;

                    //Elimino de "SuscripcionIdentidadProyecto"
                    mEntityContext.EliminarElemento(filaSuscripcionIdentidadProyecto);
                    SuscripcionDW.ListaSuscripcionIdentidadProyecto.Remove(filaSuscripcionIdentidadProyecto);
                    eliminar = true;
                }
                if (eliminar)
                {
                    //Elimino "Suscripcion"
                    mEntityContext.EliminarElemento(filaSuscripcion);
                    SuscripcionDW.ListaSuscripcion.Remove(filaSuscripcion);
                }
            }
        }

        /// <summary>
        /// Elimina TODAS las suscripciones de cualquier tipo que pueda tener cada una de las IdentidadID que se le pasa en la lista. 
        /// Nota: cargar el gestor de notificaciones para que se pueda eliminar de "NotificacionSuscripcion"
        /// </summary>
        /// <param name="pListaIdentidades">Lista con los identificadores de las identidades</param>
        public void EliminarSuscripciones(List<Guid> pListaIdentidades)
        {
            foreach (Guid IdentidadID in pListaIdentidades)
            {
                EliminarSuscripciones(IdentidadID);
            }
        }

        /// <summary>
        /// Elimina TODAS las suscripciones a identidades que pueda tener una Identidad en un proyecto. Nota: cargar el gestor de notificaciones para que se pueda eliminar de "NotificacionSuscripcion"
        /// </summary>
        /// <param name="pListaIdentidades">Lista ed identificadores de identidad</param>
        ///<param name="pProyectoID">Identificador del proyecto</param>
        public void EliminarSuscripcionesDeIdentidadEnProyecto(List<Guid> pListaIdentidades, Guid pProyectoID)
        {
            foreach (Guid IdentidadID in pListaIdentidades)
            {
                EliminarSuscripcionesDeIdentidadEnProyecto(IdentidadID, pProyectoID);
            }
        }

        /// <summary>
        /// Obtiene la suscripción a un proyecto dado de entre la lista de suscripciones cargadas
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto para el que queremos obtener su suscripción</param>
        /// <returns>Suscripción al proyecto</returns>
        public Suscripcion ObtenerSuscripcionAProyecto(Guid pProyectoID)
        {
            foreach (Suscripcion susc in ListaSuscripciones.Values)
            {
                if (susc.Tipo != TipoSuscripciones.Comunidades)
                {
                    continue;
                }
                if (((AD.EntityModel.Models.Suscripcion.SuscripcionTesauroProyecto)susc.FilaRelacion).ProyectoID == pProyectoID)
                {
                    return susc;
                }
            }
            return null;
        }

        

        #endregion

        #region Miembros de ISerializable

        /// <summary>
        /// Metodo para serializar el objeto
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serialización</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            
            info.AddValue("GestionNotificaciones", GestorNotificaciones);
            info.AddValue("GestionProyecto", GestorProyecto);
            info.AddValue("GestionIdentidades", GestorIdentidades);
        }

        #endregion
    }
}
