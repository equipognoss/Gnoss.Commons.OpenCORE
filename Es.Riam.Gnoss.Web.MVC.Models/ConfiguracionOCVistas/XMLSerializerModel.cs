using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Es.Riam.Gnoss.Web.MVC.Models.ConfiguracionOCVistas
{
    public class XMLSerializerModel
    {
    }
        [XmlRoot(ElementName = "idiomasOnto")]
        public class IdiomasOnto
        {
            [XmlElement(ElementName = "idiomaOnto")]
            public List<string> IdiomaOnto { get; set; }
        }

        [XmlRoot(ElementName = "TituloDoc")]
        public class TituloDoc
        {

            [XmlAttribute(AttributeName = "EntidadID")]
            public string EntidadID { get; set; }

            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "DescripcionDoc")]
        public class DescripcionDoc
        {

            [XmlAttribute(AttributeName = "EntidadID")]
            public string EntidadID { get; set; }

            [XmlText]
            public string Text { get; set; }
        }


        public class Meta
        {
            [XmlAttribute(AttributeName = "EntidadID")]
            public string EntidadID { get; set; }

            [XmlAttribute(AttributeName = "name")]
            public string name { get; set; }

            [XmlAttribute(AttributeName = "content")]
            public string content { get; set; }
        }
        [XmlRoot(ElementName = "ConfiguracionGeneral")]
        public class ConfiguracionGeneral
        {

            [XmlElement(ElementName = "namespace")]
            public string Namespace { get; set; }

            [XmlElement(ElementName = "idiomasOnto")]
            public IdiomasOnto IdiomasOnto { get; set; }

            [XmlElement(ElementName = "TituloDoc")]
            public TituloDoc TituloDoc { get; set; }

            [XmlElement(ElementName = "DescripcionDoc")]
            public DescripcionDoc DescripcionDoc { get; set; }

            [XmlElement(ElementName = "HtmlNuevo")]
            public bool HtmlNuevo { get; set; }

            [XmlElement(ElementName = "CategorizacionTesauroGnossObligatoria")]
            public bool CategorizacionTesauroGnossObligatoria { get; set; }

            [XmlElement(ElementName = "IncluirIconoGnoss")]
            public object IncluirIconoGnoss { get; set; }

            [XmlElement(ElementName = "MultiIdioma")]
            public object MultiIdioma { get; set; }

            [XmlElement(ElementName = "MetasPagina")]
            public MetasPagina MetasPagina { get; set; }

            [XmlElement(ElementName = "MenuDocumentoAbajo")]
            public bool MenuDocumentoAbajo { get; set; }

            [XmlElement(ElementName = "ocultarTituloDescpImgDoc")]
            public object OcultarTituloDescpImgDoc { get; set; }

            [XmlElement(ElementName = "ocultarFechaRec")]
            public bool OcultarFechaRec { get; set; }

            [XmlElement(ElementName = "ocultarAutoriaEdicion")]
            public object OcultarAutoriaEdicion { get; set; }

            [XmlElement(ElementName = "OcultarPublicadorDoc")]
            public object OcultarPublicadorDoc { get; set; }

            [XmlElement(ElementName = "OcultarUtilsDoc")]
            public object OcultarUtilsDoc { get; set; }

            [XmlElement(ElementName = "OcultarAccionesDoc")]
            public object OcultarAccionesDoc { get; set; }

            [XmlElement(ElementName = "OcultarCategoriasDoc")]
            public object OcultarCategoriasDoc { get; set; }

            [XmlElement(ElementName = "OcultarEtiquetasDoc")]
            public object OcultarEtiquetasDoc { get; set; }

            [XmlElement(ElementName = "OcultarEditoresDoc")]
            public object OcultarEditoresDoc { get; set; }

            [XmlElement(ElementName = "OcultarAutoresDoc")]
            public object OcultarAutoresDoc { get; set; }

            [XmlElement(ElementName = "OcultarVistasDoc")]
            public object OcultarVisitasDoc { get; set; }

            [XmlElement(ElementName = "OcultarVotosDoc")]
            public object OcultarVotosDoc { get; set; }

            [XmlElement(ElementName = "OcultarCompartidoDoc")]
            public object OcultarCompartidoDoc { get; set; }

            [XmlElement(ElementName = "OcultarCompartidoEnDoc")]
            public object OcultarCompartidoEnDoc { get; set; }

            [XmlElement(ElementName = "OcultarLicenciaDoc")]
            public object OcultarLicenciaDoc { get; set; }

            [XmlElement(ElementName = "OcultarVersionDoc")]
            public object OcultarVersionDoc { get; set; }

            [XmlElement(ElementName = "OcultarBotonEditarDoc")]
            public object OcultarBotonEditarDoc { get; set; }

            [XmlElement(ElementName = "OcultarBotonCrearVersionDoc")]
            public object OcultarBotonCrearVersionDoc { get; set; }

            [XmlElement(ElementName = "OcultarBotonEnviarEnlaceDoc")]
            public object OcultarBotonEnviarEnlaceDoc { get; set; }

            [XmlElement(ElementName = "OcultarBotonVincularDoc")]
            public object OcultarBotonVincularDoc { get; set; }

            [XmlElement(ElementName = "OcultarBotonEliminarDoc")]
            public object OcultarBotonEliminarDoc { get; set; }

            [XmlElement(ElementName = "OcultarBotonRestaurarVersionDoc")]
            public object OcultarBotonRestaurarVersionDoc { get; set; }

            [XmlElement(ElementName = "OcultarBotonAgregarCategoriaDoc")]
            public object OcultarBotonAgregarCategoriaDoc { get; set; }

            [XmlElement(ElementName = "OcultarBotonAgregarEtiquetasDoc")]
            public object OcultarBotonAgregarEtiquetasDoc { get; set; }

            [XmlElement(ElementName = "OcultarBotonHistorialDoc")]
            public object OcultarBotonHistorialDoc { get; set; }

            [XmlElement(ElementName = "OcultarBotonBloquearComentariosDoc")]
            public object OcultarBotonBloquearComentariosDoc { get; set; }

            [XmlElement(ElementName = "OcultarBotonCertificarDoc")]
            public object OcultarBotonCertificarDoc { get; set; }
        }

        [XmlRoot(ElementName = "MetasPagina")]
        public class MetasPagina
        {
            [XmlElement(ElementName = "meta")]
            public List<Meta> Meta { get; set; }
        }

        [XmlRoot(ElementName = "AtrNombre")]
        public class AtrNombre
        {

            [XmlAttribute(AttributeName = "xml:lang")]
            public string Lang { get; set; }

            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "AtrNombreLectura")]
        public class AtrNombreLectura
        {

            [XmlAttribute(AttributeName = "xml:lang")]
            public string Lang { get; set; }

            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "Size")]
        public class Size
        {

            [XmlElement(ElementName = "Alto")]
            public int Alto { get; set; }

            [XmlAttribute(AttributeName = "Tipo")]
            public string Tipo { get; set; }

            [XmlText]
            public int Text { get; set; }

            [XmlElement(ElementName = "Ancho")]
            public int Ancho { get; set; }
        }

        [XmlRoot(ElementName = "ImgMiniVP")]
        public class ImgMiniVP
        {

            [XmlElement(ElementName = "Size")]
            public List<Size> Size { get; set; }
        }

        [XmlRoot(ElementName = "Propiedad")]
        public class PropiedadXML
        {

            [XmlElement(ElementName = "AtrNombre")]
            public AtrNombre AtrNombre { get; set; }

            [XmlElement(ElementName = "AtrNombreLectura")]
            public List<AtrNombreLectura> AtrNombreLectura { get; set; }

            [XmlElement(ElementName = "TipoCampo")]
            public string TipoCampo { get; set; }

            [XmlAttribute(AttributeName = "ID")]
            public string ID { get; set; }

            [XmlAttribute(AttributeName = "EntidadID")]
            public string EntidadID { get; set; }

            [XmlText]
            public string Text { get; set; }

            [XmlElement(ElementName = "SeleccionEntidad")]
            public SeleccionEntidad SeleccionEntidad { get; set; }
        }

        [XmlRoot(ElementName = "LinkARecurso")]
        public class LinkARecurso
        {

            [XmlAttribute(AttributeName = "PropiedadID")]
            public string PropiedadID { get; set; }

            [XmlAttribute(AttributeName = "IrAComunidad")]
            public bool IrAComunidad { get; set; }
        }

        [XmlRoot(ElementName = "PropsEntEdicion")]
        public class PropsEntEdicion
        {

            [XmlElement(ElementName = "NameProp")]
            public string NameProp { get; set; }
        }

        [XmlRoot(ElementName = "PropsEntLectura")]
        public class PropsEntLectura
        {

            [XmlElement(ElementName = "Propiedad")]
            public List<PropiedadXML> Propiedad { get; set; }
        }

        [XmlRoot(ElementName = "SeleccionEntidad")]
        public class SeleccionEntidad
        {
            
            [XmlElement(ElementName = "NuevaPestanya")]
            public bool NuevaPestanya { get; set; }

            [XmlElement(ElementName = "TipoSeleccion")]
            public string TipoSeleccion { get; set; }

            [XmlElement(ElementName = "Grafo")]
            public string Grafo { get; set; }

            [XmlElement(ElementName = "UrlTipoEntSolicitada")]
            public string UrlTipoEntSolicitada { get; set; }

            [XmlElement(ElementName = "LinkARecurso")]
            public LinkARecurso LinkARecurso { get; set; }

            [XmlElement(ElementName = "PropsEntEdicion")]
            public PropsEntEdicion PropsEntEdicion { get; set; }

            [XmlElement(ElementName = "PropsEntLectura")]
            public PropsEntLectura PropsEntLectura { get; set; }
        }

        


        [XmlRoot(ElementName = "EspefPropiedad")]
        public class EspefPropiedad
        {

            [XmlElement(ElementName = "Propiedad")]
            public List<PropiedadXML> Propiedad { get; set; }
        }

        [XmlRoot(ElementName = "AtrNombreGrupo")]
        public class AtrNombreGrupo
        {
            [XmlText]
            public string Valor { get; set; }

            [XmlAttribute(AttributeName = "class")]
            public string Class { get; set; }

            [XmlAttribute(AttributeName = "Tipo")]
            public string Tipo { get; set; }
        }

        [XmlRoot(ElementName = "Literal")]
        public class Literal
        {
            [XmlText]
            public string Valor { get; set; }

            [XmlAttribute(AttributeName = "link")]
            public string Link { get; set; }

            [XmlAttribute(AttributeName = "target")]
            public string Target { get; set; }
        }

        [XmlRoot(ElementName = "NameProp")]
        public class NameProp
        {
            [XmlText]
            public string Valor { get; set; }

            [XmlAttribute(AttributeName = "PropDeEntHija")]
            public string PropDeEntHija { get; set; }

            [XmlAttribute(AttributeName = "SoloPrimerValor")]
            public string SoloPrimerValor { get; set; }

            [XmlAttribute(AttributeName = "SinTitulo")]
            public string SinTitulo { get; set; }

            [XmlAttribute(AttributeName = "Tipo")]
            public string Tipo { get; set; }

            [XmlAttribute(AttributeName = "link")]
            public string Link { get; set; }

            [XmlAttribute(AttributeName = "target")]
            public string Target { get; set; }
        }

        [XmlType(IncludeInSchema = false)]
        public enum ElementosOrden
        {
            NameProp,
            Literal,
            Grupo,
            None
        }

        [XmlRoot(ElementName = "Grupo")]
        public class Grupo
        {
            [XmlAttribute(AttributeName = "classLectura")]
            public string ClassLectura { get; set; }

            [XmlElement(ElementName = "AtrNombreGrupo")]
            public AtrNombreGrupo AtrNombreGrupo { get; set; }

            [XmlAttribute(AttributeName = "id")]
            public string ID { get; set; }

            [XmlAttribute(AttributeName = "Tipo")]
            public string Tipo { get; set; }

            [XmlAttribute(AttributeName = "class")]
            public string Class { get; set; }

            // Contenido contiene la lista de los elementos. Necesitamos esta estructura, porque importa el orden de los tipos.
            [XmlChoiceIdentifier("TipoArray")]
            [XmlElement("Grupo", typeof(Grupo))]
            [XmlElement("Literal", typeof(Literal))]
            [XmlElement("NameProp", typeof(NameProp))]
            public object[] Contenido { get; set; }

            // TipoArray contiene la enumeración que corresponde con los tipos de Contenido, uno por cada elemento del array
            [XmlIgnore]
            public ElementosOrden[] TipoArray { get; set; }
        }

        [XmlRoot(ElementName = "Representante")]
        public class Representante
        {
            [XmlText]
            public string Valor { get; set; }

            [XmlAttribute(AttributeName = "tipo")]
            public string Tipo { get; set; }

            [XmlAttribute(AttributeName = "numCaracteres")]
            public int NumCaracteres { get; set; }
        }

        [XmlRoot(ElementName = "Representantes")]
        public class Representantes
        {
            [XmlElement(ElementName = "Representante")]
            public List<Representante> Representante { get; set; }
        }

        [XmlRoot(ElementName = "ArtNombre")]
        public class ArtNombre
        {
            [XmlText]
            public string Valor { get; set; }

            [XmlAttribute(AttributeName = "xml:lang")]
            public string XmlLang { get; set; }
        }

        [XmlRoot(ElementName = "ArtNombreLectura")]
        public class ArtNombreLectura
        {
            [XmlText]
            public string Valor { get; set; }

            [XmlAttribute(AttributeName = "xml:lang")]
            public string XmlLang { get; set; }
        }

        [XmlRoot(ElementName = "OrdenEntidad")]
        public class OrdenEntidad
        {
            // Contenido contiene la lista de los elementos. Necesitamos esta estructura, porque importa el orden de los tipos.
            [XmlChoiceIdentifier("TipoArray")]
            [XmlElement("Grupo", typeof(Grupo))]
            [XmlElement("Literal", typeof(Literal))]
            [XmlElement("NameProp", typeof(NameProp))]
            public object[] Contenido { get; set; }

            // TipoArray contiene la enumeración que corresponde con los tipos de Contenido, uno por cada elemento del array
            [XmlIgnore]
            public ElementosOrden[] TipoArray { get; set; }
        }

        [XmlRoot(ElementName = "OrdenEntidadLectura")]
        public class OrdenEntidadLectura
        {
            // Contenido contiene la lista de los elementos. Necesitamos esta estructura, porque importa el orden de los tipos.
            [XmlChoiceIdentifier("TipoArray")]
            [XmlElement("Grupo", typeof(Grupo))]
            [XmlElement("Literal", typeof(Literal))]
            [XmlElement("NameProp", typeof(NameProp))]
            public object[] Contenido { get; set; }

            // TipoArray contiene la enumeración que corresponde con los tipos de Contenido, uno por cada elemento del array
            [XmlIgnore]
            public ElementosOrden[] TipoArray { get; set; }
        }

        [XmlRoot(ElementName = "Entidad")]
        public class Entidad
        {
            [XmlElement(ElementName = "AtrNombre")]
            public List<AtrNombre> AtrNombre { get; set; }

            [XmlElement(ElementName = "AtrNombreLectura")]
            public List<AtrNombreLectura> AtrNombreLectura { get; set; }

            [XmlElement(ElementName = "Representantes")]
            public Representantes Representantes { get; set; }

            [XmlElement(ElementName = "ClaseCssPanel")]
            public string ClaseCssPanel { get; set; }

            [XmlElement(ElementName = "ClaseCssTitulo")]
            public string ClaseCssTitulo { get; set; }

            [XmlElement(ElementName = "TagNameTituloEdicion")]
            public string TagNameTituloEdicion { get; set; }

            [XmlElement(ElementName = "TagNameTituloLectura")]
            public string TagNameTituloLectura { get; set; }

            [XmlElement(ElementName = "OrdenEntidad")]
            public OrdenEntidad OrdenEntidad { get; set; }

            [XmlElement(ElementName = "OrdenEntidadLectura")]
            public OrdenEntidadLectura OrdenEntidadLectura { get; set; }

            [XmlAttribute(AttributeName = "ID")]
            public string ID { get; set; }

            [XmlText]
            public string Text { get; set; }

            [XmlElement(ElementName = "Microdatos")]
            public string Microdatos { get; set; }

            [XmlElement(ElementName = "CampoOrden")]
            public string CampoOrden { get; set; }

            [XmlElement(ElementName = "CampoRepresentanteOrden")]
            public string CampoRepresentanteOrden { get; set; }
        }

        [XmlRoot(ElementName = "EspefEntidad")]
        public class EspefEntidad
        {

            [XmlElement(ElementName = "Entidad")]
            public List<Entidad> Entidad { get; set; }
        }

        [XmlRoot(ElementName = "config")]
        public class Config
        {

            [XmlElement(ElementName = "ConfiguracionGeneral")]
            public ConfiguracionGeneral ConfiguracionGeneral { get; set; }

            [XmlElement(ElementName = "EspefPropiedad")]
            public EspefPropiedad EspefPropiedad { get; set; }

            [XmlElement(ElementName = "EspefEntidad")]
            public EspefEntidad EspefEntidad { get; set; }
        }
    
}

