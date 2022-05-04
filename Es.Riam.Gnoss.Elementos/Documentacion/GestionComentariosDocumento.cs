using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.Elementos.Comentario;
using Es.Riam.Gnoss.Elementos.Notificacion;
using Es.Riam.Gnoss.Logica.Notificacion;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Es.Riam.Gnoss.Elementos.Documentacion
{
    /// <summary>
    /// Gestor de comentarios para los documentos.
    /// </summary>
    [Serializable]
    public class GestionComentariosDocumento : GestionComentarios, ISerializable
    {
        #region Miembros

        /// <summary>
        /// Gestor de documentos.
        /// </summary>
        private GestorDocumental mGestorDocumental;

        private LoggingService mLoggingService;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pComentarioDW">DataSet de comentarios</param>
        /// <param name="pGestorDocumental">Gestor de documentos</param>
        public GestionComentariosDocumento(DataWrapperComentario pComentarioDW, GestorDocumental pGestorDocumental,  LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pComentarioDW, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;

            mGestorDocumental = pGestorDocumental;
        }

        /// <summary>
        /// Constructor para la deseralización
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serialización</param>
        protected GestionComentariosDocumento(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            //mLoggingService = loggingService;
            //mEntityContext = entityContext;
            //mConfigService = configService;

            mGestorDocumental = (GestorDocumental)info.GetValue("GestorDocumental", typeof(GestorDocumental));
        }

        #endregion

        #region Métodos generales

        /// <summary>
        /// Agrega un comentario a un documento.
        /// </summary>
        /// <param name="pTexto">Texto del comentario</param>
        /// <param name="pIdentidad">Identidad de la persona que hace el comentario</param>
        /// <param name="pDocumento">Documento comentado</param>
        /// <param name="pProyectoID">Identificador del proyecto donde se hace el comentario</param>
        /// <param name="pEnlace">Enlace del comentario</param>
        /// <param name="pUrlIntragnoss">Enlace del comentario</param>
        /// <returns>Comentario realizado</returns>
        public Comentario.Comentario AgregarComentarioDocumento(string pTexto, Identidad.Identidad pIdentidad, Documento pDocumento, Guid pProyectoID, string pEnlace, string pUrlIntragnoss, ServiciosGenerales.Proyecto pProyecto,string pLanguageCode, bool pEsEcosistemaSinMetaProyecto = false)
        {
            Comentario.Comentario nuevoComentario = base.AgregarComentario(pTexto, pIdentidad.Clave);
            AD.EntityModel.Models.Documentacion.DocumentoComentario filaComentarioDoc = new AD.EntityModel.Models.Documentacion.DocumentoComentario();
            filaComentarioDoc.ComentarioID = nuevoComentario.Clave;
            filaComentarioDoc.DocumentoID = pDocumento.Clave;
            filaComentarioDoc.ProyectoID = pProyectoID;
            filaComentarioDoc.Comentario = nuevoComentario.FilaComentario;

            if (!pDocumento.TipoDocumentacion.Equals(TiposDocumentacion.Wiki))
            {
                NotificacionCN notificacionCN = new NotificacionCN( mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                DataWrapperNotificacion notificacionDW = new DataWrapperNotificacion();
                GestionNotificaciones gestorNotificaciones = new GestionNotificaciones(notificacionDW,  mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                gestorNotificaciones.AgregarNotificacionComentarioDocumento(pDocumento, pIdentidad, pUrlIntragnoss, pEnlace, pProyecto, pLanguageCode, pEsEcosistemaSinMetaProyecto);
                notificacionCN.ActualizarNotificacion();
                gestorNotificaciones.Dispose();
                notificacionCN.Dispose();
            }
            GestorDocumental.DataWrapperDocumentacion.ListaDocumentoComentario.Add(filaComentarioDoc);
            mEntityContext.DocumentoComentario.Add(filaComentarioDoc);
            return nuevoComentario;
        }

        /// <summary>
        /// Agrega un comentario a un documento pero no envía la notificacion al autor del recurso.
        /// </summary>
        /// <param name="pTexto">Texto del comentario</param>
        /// <param name="pIdentidad">Identidad de la persona que hace el comentario</param>
        /// <param name="pDocumento">Documento comentado</param>
        /// <param name="pProyectoID">Identificador del proyecto donde se hace el comentario</param>
        /// <returns>Comentario realizado</returns>
        public Comentario.Comentario AgregarComentarioDocumentoSinNotificacion(string pTexto, Identidad.Identidad pIdentidad, Documento pDocumento, Guid pProyectoID)
        {
            Comentario.Comentario nuevoComentario = base.AgregarComentario(pTexto, pIdentidad.Clave);
            AD.EntityModel.Models.Documentacion.DocumentoComentario filaComentarioDoc = new AD.EntityModel.Models.Documentacion.DocumentoComentario();
            filaComentarioDoc.ComentarioID = nuevoComentario.Clave;
            filaComentarioDoc.DocumentoID = pDocumento.Clave;
            filaComentarioDoc.ProyectoID = pProyectoID;
            
            GestorDocumental.DataWrapperDocumentacion.ListaDocumentoComentario.Add(filaComentarioDoc);
            mEntityContext.DocumentoComentario.Add(filaComentarioDoc);
            return nuevoComentario;
        }

        /// <summary>
        /// Elimina un comentario
        /// </summary>
        /// <param name="pComentario">Comentario a eliminar</param>
        /// <param name="pDocumento">Documento al que pertenecía el comentario</param>
        public void EliminarComentario(Es.Riam.Gnoss.Elementos.Comentario.Comentario pComentario, Documento pDocumento)
        {
            pDocumento.Comentarios.Remove(pComentario);
            ListaComentarios.Remove(pComentario.Clave);
            pComentario.FilaComentario.Eliminado = true;
        }

        /// <summary>
        /// Recarga la lista de comentarios del gestor
        /// </summary>
        public override void RecargarComentarios()
        {
            mListaComentarios = new Dictionary<Guid, Comentario.Comentario>();

            foreach (AD.EntityModel.Models.Comentario.Comentario filaComentario in ComentarioDW.ListaComentario.OrderByDescending(item => item.Fecha))
            {
                if (!mListaComentarios.ContainsKey(filaComentario.ComentarioID))
                {
                    Comentario.Comentario comentario = new Comentario.Comentario(filaComentario, this,  mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                    mListaComentarios.Add(comentario.Clave, comentario);
                }
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Gestor de Documentos.
        /// </summary>
        public GestorDocumental GestorDocumental
        {
            get
            {
                return mGestorDocumental;
            }
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
            info.AddValue("GestorDocumental", GestorDocumental);
        }

        #endregion
    }
}
