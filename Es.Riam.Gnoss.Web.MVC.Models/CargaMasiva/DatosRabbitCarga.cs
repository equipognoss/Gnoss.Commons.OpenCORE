using System;
using System.Collections.Generic;
namespace Es.Riam.Gnoss.Web.MVC.Models.CargaMasiva
{
    /// <summary>
    /// Datos para meter la carga en rabbit
    /// </summary>
    public class DatosRabbitCarga : IDisposable
    {
        private Guid cargaID;
        private Guid paqueteID;
        private string urlTriplesOntologia;
        private string urlTriplesBusqueda;
        private string urlDatosAcido;
        private string urlImagenesManifest;
        private byte[] bytesTriplesOntologia;
        private byte[] bytesTriplesBusqueda;
        private byte[] bytesDatosAcido;
        private byte[] bytesImagenesManifest;
        /// <summary>
        /// Id de la carga
        /// </summary>
        public Guid CargaID
        {
            get { return cargaID; }
            set { cargaID = value; }
        }
        /// <summary>
        /// Id del paquete de la carga
        /// </summary>
        public Guid PaqueteID
        {
            get { return paqueteID; }
            set { paqueteID = value; }
        }
        /// <summary>
        /// Url del archivo de triples de la ontologia          
        /// </summary>
        public string UrlTriplesOntologia
        {
            get { return urlTriplesOntologia; }
            set { urlTriplesOntologia = value; }
        }
        /// <summary>
        /// Url del archivo de triple del grafo de busqueda
        /// </summary>
        public string UrlTriplesBusqueda
        {
            get { return urlTriplesBusqueda; }
            set { urlTriplesBusqueda = value; }
        }
        /// <summary>
        /// Url del archivo de los datos del acido
        /// </summary>
        public string UrlDatosAcido
        {
            get { return urlDatosAcido; }
            set { urlDatosAcido = value; }
        }

        public byte[] BytesDatosAcido
        {
            get { return bytesDatosAcido; }
            set { bytesDatosAcido = value; }
        }

        public byte[] BytesTriplesOntologia
        {
            get { return bytesTriplesOntologia; }
            set { bytesTriplesOntologia = value; }
        }

        public byte[] BytesTriplesBusqueda
        {
            get { return bytesTriplesBusqueda; }
            set { bytesTriplesBusqueda = value; }
        }

        public string UrlImagenesManifest
        {
            get
            { return urlImagenesManifest; }
            set
            { urlImagenesManifest = value; }
        }
        public byte[] BytesImagenesManifest
        {
            get { return bytesImagenesManifest; }
            set { bytesImagenesManifest = value; }
        }

        public void Dispose()
        {
            bytesDatosAcido = null;
            bytesTriplesOntologia = null;
            bytesTriplesBusqueda = null;
        }

        public bool SonDatosdebug()
        {
            if (bytesDatosAcido != null && bytesTriplesBusqueda != null && bytesTriplesOntologia != null)
            {
                return true;
            }
            return false;
        }

    }
    public class ImageManifestEntry
    {
        /// <summary>
        /// Identificador del documento al que pertenecen las imágenes
        /// </summary>
        public string DocumentoID { get; set; }

        /// <summary>
        /// Lista de nombres de fichero con formato {documentoId}_{nombreOriginal}
        /// </summary>
        public List<string> ImagePaths { get; set; }
    }

    public class DatosRabbitImagenes
    {
        public string RutaManifest { get; set; }
        public string UrlBaseImagenes { get; set; }
        public Guid ProyectoId { get; set; }
        public Guid OntologiaId { get; set; }
        public Guid PaqueteId { get; set; }
        public Guid CargaId { get; set; }
    }

    public class ImageSizeConfig
    {
        public int Ancho { get; set; }
        public int Alto { get; set; }
        public bool EsRecorteCuadrado { get; set; }  // true si Tipo="RecorteCuadrado"
    }
}


