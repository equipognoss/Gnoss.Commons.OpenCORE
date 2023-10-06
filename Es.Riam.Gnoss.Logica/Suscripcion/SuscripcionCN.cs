using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.Suscripcion;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Logica.Suscripcion
{
    /// <summary>
    /// Clase que maneja la lógica de las suscripciones
    /// </summary>
    public class SuscripcionCN : BaseCN, IDisposable
    {

        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public SuscripcionCN(EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication)
        {
            SuscripcionAD = new SuscripcionAD(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication);
        }

        /// <summary>
        /// Constructor a partir del fichero de configuración de la conexión a la base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de la conexión a la base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public SuscripcionCN(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication)
        {
            SuscripcionAD = new SuscripcionAD(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication);
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Actualiza las suscripciones en la base de datos
        /// </summary>
        /// <param name="pSuscripcionDS">Dataset de suscripciones</param>
        public void ActualizarSuscripcion()
        {
            SuscripcionAD.ActualizarSuscripcion();
        }

        /// <summary>
        /// Obtiene una lista con las identidades que son seguidas por la identidad
        /// </summary>
        /// <param name="pPerfilID">Perfil del seguidor</param>
        /// <returns>Lista con los ids de las identidades seguidas por la identidad</returns>
        public List<Guid> ObtenerListaIdentidadesSuscritasPerfil(Guid pPerfilID)
        {
            return SuscripcionAD.ObtenerListaIdentidadesSuscritasPerfil(pPerfilID);
        }

        /// <summary>
        /// Obtiene una lista con las identidades que son seguidas por la identidad de la lista de claves
        /// </summary>
        /// <param name="pPerfilID">Perfil del seguidor</param>
        /// <returns>Lista con los ids de las identidades seguidas por la identidad</returns>
        public List<Guid> ComprobarListaIdentidadesSuscritasPerfil(Guid pPerfilID, List<Guid> pListaIdentidades)
        {
            return SuscripcionAD.ComprobarListaIdentidadesSuscritasPerfil(pPerfilID, pListaIdentidades);
        }

        /// <summary>
        /// Obtiene las suscripciones de las identidades (las suscripciones, no sus elementos)
        /// </summary>
        /// <param name="pListaIdentidadesID">Lista de IdentidadID</param>
        /// <param name="pObtenerBloqueadas">True si queremos obtener las suscripciones bloquedas ademas de las no bloqueadas</param>
        /// <returns>SuscripcionDS cargado con las suscripciones (Suscripcion,SuscripcionTesauroUsuario,SuscripcionTesauroProyecto,
        /// CategoriaTesVinSuscrip)</returns>
        public DataWrapperSuscripcion ObtenerSuscripcionesDeListaIdentidades(List<Guid> pListaIdentidadesID, bool pObtenerBloqueadas)
        {
            return SuscripcionAD.ObtenerSuscripcionesDeListaIdentidades(pListaIdentidadesID, pObtenerBloqueadas);
        }

        /// <summary>
        /// Obtiene las suscripciones de una identidad (las suscripciones, no sus elementos)
        /// </summary>
        /// <param name="pIdentidadID">IdentidadID</param>
        /// <param name="pObtenerBloqueadas">True si queremos obtener las suscripciones bloquedas ademas de las no bloqueadas</param>
        /// <returns>SuscripcionDS cargado con las suscripciones (Suscripcion,SuscripcionTesauroUsuario,SuscripcionTesauroProyecto,
        /// CategoriaTesVinSuscrip)</returns>
        public DataWrapperSuscripcion ObtenerSuscripcionesDeIdentidad(Guid pIdentidadID, bool pObtenerBloqueadas)
        {
            return SuscripcionAD.ObtenerSuscripcionesDeIdentidad(pIdentidadID, pObtenerBloqueadas);
        }

        /// <summary>
        /// Obtiene un diccionario con clave las identidades para comprobar y un booleano que nos dice si le seguiomos en la comunidad o no
        /// </summary>
        /// <param name="pIdentidadID">IdentidadID</param>
        /// <param name="pListaIdentidadesComprobar">Lista de identidades para comprobar</param>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns></returns>
        public List<Guid> ObtenerListaIdentidadesSuscritasPorProyecto(Guid pIdentidadID, List<Guid> pListaIdentidadesComprobar, Guid pProyectoID)
        {
            return SuscripcionAD.ObtenerListaIdentidadesSuscritasPorProyecto(pIdentidadID, pListaIdentidadesComprobar, pProyectoID);
        }

        /// <summary>
        /// Obtiene las suscripciones de un perfil (las suscripciones, no sus elementos)
        /// </summary>
        /// <param name="pPerfilID">PerfilID</param>
        /// <param name="pObtenerCamposDesnormalizados">True si se quieren obtener campos desnormalizados como el nombre de 
        /// las categorias suscritas, de los blogss etc...</param>
        /// <param name="pObtenerBloqueadas">True si queremos obtener las suscripciones bloquedas ademas de las no bloqueadas</param>
        /// <returns>SuscripcionDS cargado con las suscripciones (Suscripcion,SuscripcionTesauroUsuario,
        /// SuscripcionTesauroProyecto,CategoriaTesVinSuscrip)</returns>
        public DataWrapperSuscripcion ObtenerSuscripcionesDePerfil(Guid pPerfilID, bool pObtenerBloqueadas)
        {
            return SuscripcionAD.ObtenerSuscripcionesDePerfil(pPerfilID, pObtenerBloqueadas);
        }

        public DataWrapperSuscripcion ObtenerSuscripcionDePerfilAPerfil(Guid pPerfilOrigenID, Guid pPerfilDestinoID, bool pObtenerBloqueadas, bool pIncluirBRPersonal)
        {
            return SuscripcionAD.ObtenerSuscripcionDePerfilAPerfil(pPerfilOrigenID, pPerfilDestinoID, pObtenerBloqueadas, pIncluirBRPersonal);
        }

        public DataWrapperSuscripcion ObtenerSuscripcionDePerfilAOrganizacion(Guid pPerfilOrigenID, Guid pOrganizacionID, bool pObtenerBloqueadas)
        {
            return SuscripcionAD.ObtenerSuscripcionDePerfilAOrganizacion(pPerfilOrigenID, pOrganizacionID, pObtenerBloqueadas);
        }

        public List<Guid> ListaPerfilesSuscritosAPerfilEnComunidad(Guid pPerfilDestinoID, Guid pProyectoID, bool pObtenerSuscritosMetaProyecto = true)
        {
            List<Guid> perfilesDestinoID = new List<Guid>();
            perfilesDestinoID.Add(pPerfilDestinoID);
            return SuscripcionAD.ListaPerfilesSuscritosAPerfilEnComunidad(perfilesDestinoID, pProyectoID, pObtenerSuscritosMetaProyecto);
        }

        public List<Guid> ListaPerfilesSuscritosAAlgunaCategoriaDeDocumento(Guid pDocumentoID, Guid pProyectoID)
        {
            return SuscripcionAD.ListaPerfilesSuscritosAAlgunaCategoriaDeDocumento(pDocumentoID, pProyectoID);
        }

        /// <summary>
        /// Obtiene SuscripcionDS con todas las suscripciones "SuscripcionTesauroUsuario","SuscripcionTesauroProyecto",
        /// "CategoriaTesVinSuscrip", "TesVinSuscrip","SuscripcionBlog" de todas las identidades del usuario, 
        /// a través de la persona 'pPersonaID' vinculada a él
        /// </summary>
        /// <param name="pPersonaID">Clave de la persona vinculada al usuario</param>
        /// <param name="pObtenerBloqueadas">True si queremos obtener las suscripciones bloquedas ademas de las no bloqueadas</param>
        /// <returns>SuscripcionDS</returns>
        public DataWrapperSuscripcion ObtenerSuscripcionesDeUsuario(Guid pPersonaID, bool pObtenerBloqueadas)
        {
            return SuscripcionAD.ObtenerSuscripcionesDeUsuario(pPersonaID, pObtenerBloqueadas);
        }

        /// <summary>
        /// Obtiene las vinculaciones de las suscripciones que poseen ciertas categorías.
        /// </summary>
        /// <param name="pListaCategorias">Identificadores de las categorías</param>
        /// <param name="pTesauroID">Identificador del tesauro al que pertenecen las categorías</param>
        /// <returns>DataSet con las vinculaciones cargadas</returns>
        public DataWrapperSuscripcion ObtenerVinculacionesSuscripcionesDeCategoriasTesauro(List<Guid> pListaCategorias, Guid pTesauroID)
        {
            return SuscripcionAD.ObtenerVinculacionesSuscripcionesDeCategoriasTesauro(pListaCategorias, pTesauroID);
        }

        /// <summary>
        /// Devuelve si se han enviado resultados de la suscripción pasada como parámetro después de la fecha dada
        /// </summary>
        /// <param name="pSuscripcionID">Identificador de la suscripción</param>
        /// <param name="pFecha">Fecha a partir de la cual se comprueba si tiene boletin</param>
        /// <returns>TRUE si existe boletin posterior a la fecha, FALSE en caso contrario</returns>
        public bool TienePerfilBoletinPosteriorAFecha(Guid pSuscripcionID, DateTime pFecha)
        {
            return SuscripcionAD.TienePerfilBoletinPosteriorAFecha(pSuscripcionID, pFecha);
        }

        /// <summary>
        /// Obtiene la identidad de un seguidor.
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>Clave de la base de recursos del usuario</returns>
        public Guid ObtenerIdentidadSeguidorPorIDSuscripcion(Guid pSuscripcionID)
        {
            return SuscripcionAD.ObtenerIdentidadSeguidorPorIDSuscripcion(pSuscripcionID);
        }

        /// <summary>
        /// Obtiene la identidad de un seguidor en una comunidad.
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>Clave de la base de recursos del usuario</returns>
        public Guid ObtenerIdentidadSeguidorComPorIDSuscripcion(Guid pSuscripcionID)
        {
            return SuscripcionAD.ObtenerIdentidadSeguidorComPorIDSuscripcion(pSuscripcionID);
        }

        /// <summary>
        /// Obtiene la identidad de un seguidor de un blog.
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>Identidad del seguidor de un blog</returns>
        public Guid ObtenerIdentidadSeguidorBlogPorIDSuscripcion(Guid pSuscripcionID)
        {
            return SuscripcionAD.ObtenerIdentidadSeguidorBlogPorIDSuscripcion(pSuscripcionID);
        }

        /// <summary>
        /// Devuelve el ultimo score que se ha enviado de una suscripción 
        /// </summary>
        /// <param name="pSuscripcionID">Identificador de la suscripción</param>
        /// <returns></returns>
        public int UltimoScoreSuscripcionEnviado(Guid pSuscripcionID)
        {
            return SuscripcionAD.UltimoScoreSuscripcionEnviado(pSuscripcionID);
        }

        /// <summary>
        /// Suscribe al tesauro personal del usuario a cada persona que esté suscrita a todo el perfil del usuario.
        /// </summary>
        /// <param name="pUsuarioID">ID del usuario</param>
        public void CrearSuscripcionesAUsuarioSiPersonaSuscriptaATodo(Guid pUsuarioID)
        {
            SuscripcionAD.CrearSuscripcionesAUsuarioSiPersonaSuscriptaATodo(pUsuarioID);
        }

        /// <summary>
        /// Elimina las suscripciones al tesauro personal del usuario a cada persona que NO esté suscrita a todo el perfil del usuario.
        /// </summary>
        /// <param name="pUsuarioID">ID del usuario</param>
        public void EliminarSuscripcionesAUsuarioSiPersonaNoSuscriptaATodo(Guid pUsuarioID)
        {
            SuscripcionAD.EliminarSuscripcionesAUsuarioSiPersonaNoSuscriptaATodo(pUsuarioID);
        }

        public DataWrapperDatoExtra ObtenerCategoriasTesauroDeProyectoSuscritaIdentidad(Guid pProyectoID, Guid pIdentidadID)
        {
            return SuscripcionAD.ObtenerCategoriasTesauroDeProyectoSuscritaIdentidad(pProyectoID, pIdentidadID);
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
        ~SuscripcionCN()
        {
            //Libero los recursos
            Dispose(false);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            //impido que se finalice dos veces este objeto
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        /// <param name="disposing">Determina si se está llamando desde el Dispose()</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;

                if (disposing)
                {
                    //Libero todos los recursos administrados que he añadido a esta clase
                    if (SuscripcionAD != null)
                        SuscripcionAD.Dispose();
                }
                SuscripcionAD = null;
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Clase para interacturar con la BD
        /// </summary>
        private SuscripcionAD SuscripcionAD
        {
            get
            {
                return (SuscripcionAD)AD;
            }
            set
            {
                AD = value;
            }
        }

        #endregion

    }
}
