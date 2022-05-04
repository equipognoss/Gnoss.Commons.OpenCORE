using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.Elementos.Voto;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Es.Riam.Gnoss.Elementos.Documentacion
{
    /// <summary>
    /// Gestiona los votos de documentos.
    /// </summary>
    [Serializable]
    public class GestionVotosDocumento : GestorVotos, ISerializable
    {
        #region Miembros

        /// <summary>
        /// Gestor documental
        /// </summary>
        private GestorDocumental mGestorDocumental;

        private EntityContext mEntityContext;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor vacío
        /// </summary>
        public GestionVotosDocumento( LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            mEntityContext = entityContext;
        }

        /// <summary>
        /// Constructor con parámtros.
        /// </summary>
        /// <param name="pVotoDS">DataSet de Voto</param>
        /// <param name="pGestorDocumental">Gestor documental</param>
        public GestionVotosDocumento(DataWrapperVoto pVotoDS, GestorDocumental pGestorDocumental,  LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pVotoDS, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            mEntityContext = entityContext;
            mGestorDocumental = pGestorDocumental;
        }

        /// <summary>
        /// Constructor para la deseralización
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serialización</param>
        protected GestionVotosDocumento(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            //mEntityContext = entityContext;
            mGestorDocumental = (GestorDocumental)info.GetValue("GestorDocumental", typeof(GestorDocumental));
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Agrega un voto a un documento.
        /// </summary>
        /// <param name="pVoto">Votación realizada</param>
        /// <param name="pIdentidadID">Identidad de la persona que vota</param>
        /// <param name="pElementoID">Elemento que se vota</param>
        /// <param name="pIdentidadVotadaID">Identidad del que el propietario de lo que se vota</param>
        /// <param name="pDocumento">Documento que se vota</param>
        /// <param name="pProyectoID"></param>
        /// <returns>Voto realizado</returns>
        public AD.EntityModel.Models.Voto.Voto AgregarVoto(double pVoto, Guid pIdentidadID, Guid pElementoID, Guid pIdentidadVotadaID, Documento pDocumento, Guid pProyectoID)
        {
            AD.EntityModel.Models.Voto.Voto voto = base.AgregarVoto(pVoto, pIdentidadID, pElementoID, pIdentidadVotadaID, (short)TiposVotos.Documento);

            AD.EntityModel.Models.Documentacion.VotoDocumento filaVotoDoc = new AD.EntityModel.Models.Documentacion.VotoDocumento();
            filaVotoDoc.VotoID = voto.VotoID;
            filaVotoDoc.DocumentoID = pDocumento.Clave;
            filaVotoDoc.ProyectoID = pProyectoID;

            GestorDocumental.DataWrapperDocumentacion.ListaVotoDocumento.Add(filaVotoDoc);
            mEntityContext.VotoDocumento.Add(filaVotoDoc);

            if (!pDocumento.ListaVotos.ContainsKey(voto.VotoID))
            {
                pDocumento.ListaVotos.Add(voto.VotoID, voto);
                pDocumento.FilaDocumento.Valoracion = (int)ObtenerMedia(pDocumento);
                pDocumento.FilaDocumentoWebVinBR.NumeroVotos = (int)ObtenerMediaProyecto(pDocumento, pProyectoID);
            }
            return voto;
        }

        /// <summary>
        /// Comprueba si una entrada de blog ya ha sido votada por una identidad
        /// </summary>
        /// <param name="pDocumento">Documento</param>
        /// <param name="pIdentidad">Identidad</param>
        /// <returns>TRUE si una entrada de blog ya ha sido votada por una identidad, FALSE en caso contrario</returns>
        public bool EstaVotado(Documento pDocumento, Identidad.Identidad pIdentidad)
        {
            bool estaVotado = false;

            if (pIdentidad.ListaTodosIdentidadesDeIdentidad.Contains(pDocumento.CreadorID))
            {
                return true;
            }
            foreach (AD.EntityModel.Models.Voto.Voto voto in pDocumento.ListaVotosPorComunidad(pIdentidad.FilaIdentidad.ProyectoID).Values)
            {
                if (pIdentidad.Clave == voto.IdentidadID)
                {
                    estaVotado = true;
                    break;
                }
            }
            return estaVotado;
        }

        /// <summary>
        /// Comprueba si un documento ya ha sido votado positivamente por una identidad
        /// </summary>
        /// <param name="pDocumento">Documento</param>
        /// <param name="pIdentidad">Identidad</param>
        /// <returns>TRUE si una entrada de blog ya ha sido votada por una identidad, FALSE en caso contrario</returns>
        public bool EstaVotadoPositivo(Documento pDocumento, Identidad.Identidad pIdentidad)
        {
            bool estaVotado = false;

            if (pIdentidad.ListaTodosIdentidadesDeIdentidad.Contains(pDocumento.CreadorID))
            {
                return true;
            }
            foreach (AD.EntityModel.Models.Voto.Voto voto in pDocumento.ListaVotosPorComunidad(pIdentidad.FilaIdentidad.ProyectoID).Values)
            {
                if (voto.Voto1 > 0)
                {
                    if (pIdentidad.Clave == voto.IdentidadID)
                    {
                        estaVotado = true;
                        break;
                    }
                }
            }
            return estaVotado;
        }

        /// <summary>
        /// Comprueba si un documento ya ha sido votado negativamente por una identidad
        /// </summary>
        /// <param name="pDocumento">Documento</param>
        /// <param name="pIdentidad">Identidad</param>
        /// <returns>TRUE si una entrada de blog ya ha sido votada por una identidad, FALSE en caso contrario</returns>
        public bool EstaVotadoNegativo(Documento pDocumento, Identidad.Identidad pIdentidad)
        {
            bool estaVotado = false;

            if (pIdentidad.ListaTodosIdentidadesDeIdentidad.Contains(pDocumento.CreadorID))
            {
                return true;
            }
            foreach (AD.EntityModel.Models.Voto.Voto voto in pDocumento.ListaVotosPorComunidad(pIdentidad.FilaIdentidad.ProyectoID).Values)
            {
                if (voto.Voto1 < 0)
                {
                    if (pIdentidad.Clave == voto.IdentidadID)
                    {
                        estaVotado = true;
                        break;
                    }
                }
            }
            return estaVotado;
        }

        /// <summary>
        /// Obtiene la media de votos de un documento pasado por parámetro
        /// </summary>
        /// <param name="pDocumento">Documento del que hay que obtener la media</param>
        /// <returns>Media de votos de un documento</returns>
        public double ObtenerMedia(Documento pDocumento)
        {
            double total = 0;

            foreach (AD.EntityModel.Models.Voto.Voto voto in pDocumento.ListaVotos.Values)
            {
                total = total + voto.Voto1;
            }
            return total;
        }

        /// <summary>
        /// Obtiene la media de votos de un documento pasado por parámetro en un determinado proyecto
        /// </summary>
        /// <param name="pDocumento">Documento del que hay que obtener la media</param>
        /// <param name="pProyectoID"></param>
        /// <returns>Media de votos de un documento</returns>
        public double ObtenerMediaProyecto(Documento pDocumento, Guid pProyectoID)
        {
            double total = 0;

            foreach (AD.EntityModel.Models.Voto.Voto voto in pDocumento.ListaVotosPorComunidad(pProyectoID).Values)
            {
                total = total + voto.Voto1;
            }
            return total;
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Devuelve o establece el gestor documental
        /// </summary>
        public GestorDocumental GestorDocumental
        {
            get
            {
                return mGestorDocumental;
            }
            set
            {
                mGestorDocumental = value;
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
