using System;

namespace Es.Riam.Gnoss.Web.MVC.Models.CargaMasiva
{
    /// <summary>
    /// Rutas de los archuivos para pasarlos a la cola de carga
    /// </summary>
    public class RutasPaqueteCarga
    {
        private string rutaTriplesOntologia;
        private string rutaTriplesBusqueda;
        private string rutaDocumento;
        private string rutaDocumentoWebVinBase;
        private string rutaDocumentoWebVinBaseExtra;
        private string rutaDocuemntoRolIndentidad;
        private string ontologia;
        private Guid proyectoId;
        private Guid paqueteId;
        private Guid cargaId;

        /// <summary>
        /// Ruta triples del grafo de ontolgia
        /// </summary>
        public string RutaTriplesOntologia
        {
            get { return rutaTriplesOntologia; }
            set { rutaTriplesOntologia = value; }
        }

        /// <summary>
        /// Ruta de los triples del grafo de búsqueda
        /// </summary>
        public string RutaTriplesBusqueda
        {
            get { return rutaTriplesBusqueda; }
            set { rutaTriplesBusqueda = value; }
        }

        /// <summary>
        /// Ruta del archivo de datos sql
        /// </summary>
        public string RutaDocumento
        {
            get { return rutaDocumento; }
            set { rutaDocumento = value; }
        }

        /// <summary>
        /// Ruta del archivo de datos sql
        /// </summary>
        public string RutaDocumentoWebVinBase
        {
            get { return rutaDocumentoWebVinBase; }
            set { rutaDocumentoWebVinBase = value; }
        }

        /// <summary>
        /// Ruta del archivo de datos sql
        /// </summary>
        public string RutaDocumentoWebVinBaseExtra
        {
            get { return rutaDocumentoWebVinBaseExtra; }
            set { rutaDocumentoWebVinBaseExtra = value; }
        }

        /// <summary>
        /// Ruta del archivo de datos sql
        /// </summary>
        public string RutaDocuemntoRolIndentidad
        {
            get { return rutaDocuemntoRolIndentidad; }
            set { rutaDocuemntoRolIndentidad = value; }
        }

        /// <summary>
        /// Ontologia a la que pertenece
        /// </summary>
        public string Ontologia
        {
            get { return ontologia; }
            set { ontologia = value; }
        }

        /// <summary>
        /// Id del proyecto
        /// </summary>
        public Guid ProyectoId
        {
            get { return proyectoId; }
            set { proyectoId = value; }
        }

        /// <summary>
        /// Id del paquete
        /// </summary>
        public Guid PaqueteId
        {
            get { return paqueteId; }
            set { paqueteId = value; }
        }

        /// <summary>
        /// Id de la carga
        /// </summary>
        public Guid CargaId
        {
            get { return cargaId; }
            set { cargaId = value; }
        }
    }
}
