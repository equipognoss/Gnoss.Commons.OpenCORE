using Es.Riam.Gnoss.Web.MVC.Models.ConfiguracionOCVistas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Es.Riam.Gnoss.Web.MVC.Models.ConfiguracionOC
{
    public class ConfiguracionOCSerializacionXMLModel
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
    [XmlRoot(ElementName = "ImagenDoc")]
    public class ImagenDoc
    {
        [XmlAttribute(AttributeName = "EntidadID")]
        public string EntidadID { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "PropiedadesOntologia")]
    public class PropiedadesOntologia
    {
        [XmlElement(ElementName = "UrlServicioCreacionRecurso")]
        public string UrlServicioCreacionRecurso { get; set; }

        [XmlElement(ElementName = "UrlServicioComplementarioCreacionRecurso")]
        public string UrlServicioComplementarioCreacionRecurso { get; set; }

        [XmlElement(ElementName = "UrlServicioComplementarioCreacionRecursoSincrono")]
        public string UrlServicioComplementarioCreacionRecursoSincrono { get; set; }

        [XmlElement(ElementName = "UrlServicioEliminacionRecurso")]
        public string UrlServicioEliminacionRecurso { get; set; }
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

        [XmlElement (ElementName = "PropiedadesOntologia")]
        public PropiedadesOntologia PropiedadesOntologia { get; set; }

        [XmlElement(ElementName = "ocultarTituloDescpImgDoc")]
        public object OcultarTituloDescpImgDoc { get; set; }

        [XmlElement(ElementName = "TituloDoc")]
        public TituloDoc TituloDoc { get; set; }

        [XmlElement(ElementName = "DescripcionDoc")]
        public DescripcionDoc DescripcionDoc { get; set; }

        [XmlElement(ElementName = "ImagenDoc")]
        public ImagenDoc ImagenDoc { get; set; }    

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

        [XmlAttribute(AttributeName = "lang")]
        public string Lang { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "AtrNombreLectura")]
    public class AtrNombreLectura
    {

        [XmlAttribute(AttributeName = "lang")]
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

        [XmlElement(ElementName = "ValoresCombo")]
        public ValoresCombo valoresCombo { get; set; }

        [XmlElement(ElementName = "ImgMiniVP")]
        public ImgMiniVP ImgMiniVP { get; set; }

        [XmlAttribute(AttributeName = "ID")]
        public string ID { get; set; }

        [XmlAttribute(AttributeName = "EntidadID")]
        public string EntidadID { get; set; }

        [XmlText]
        public string Text { get; set; }

        [XmlElement(ElementName = "SeleccionEntidad")]
        public SeleccionEntidad SeleccionEntidad { get; set; }

        [XmlElement(ElementName = "GuardarFechaComoEntero")]
        public string GuardarFechaComoEntero { get; set; }

        [XmlElement(ElementName = "UsarOpenSeaDragon")]
        public UsarOpenSeaDragon UsarOpenSeaDragon { get; set; }

        [XmlElement(ElementName = "MultiIdioma")]
        public string MultiIdioma { get; set; }
    }

    public class UsarOpenSeaDragon
    {
        [XmlAttribute(AttributeName ="PropiedadAnchoID")]
        public string PropiedadAnchoID { get; set; }

        [XmlAttribute(AttributeName = "PropiedadAltoID")]
        public string PropiedadAltoID { get; set; }
    }

    [XmlRoot(ElementName = "ValoresCombo")]
    public class ValoresCombo
    {
        [XmlAttribute(AttributeName ="valorDefecto")]
        public string valorDefecto { get; set; }

        [XmlElement(ElementName ="ValorCombo")]
        public List<string> valorCombo { get; set; }
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
        [XmlElement(ElementName = "Reciproca")]
        public Reciproca Reciproca { get; set; }

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
    [XmlRoot(ElementName = "Reciproca")]
    public class Reciproca
    {
        [XmlAttribute(AttributeName = "PropRecipID")]
        public string PropRecipID { get; set; }

        [XmlAttribute(AttributeName = "PropOrdenID")]
        public string PropOrdenID { get; set; }

        [XmlAttribute(AttributeName = "TipoOrden")]
        public string TipoOrden { get; set; }

        [XmlAttribute(AttributeName = "PropEdicionRecipID")]
        public string PropEdicionRecipID { get; set; }

        [XmlAttribute(AttributeName = "EntEdicionRecipID")]
        public string EntEdicionRecipID { get; set; }
    }
    [XmlRoot(ElementName = "EspefPropiedad")]
    public class EspefPropiedad
    {

        [XmlElement(ElementName = "Propiedad")]
        public List<PropiedadXML> Propiedad { get; set; }
    }

    [XmlRoot(ElementName = "Grupo")]
    public class Grupo
    {

        [XmlElement(ElementName = "Grupo")]
        public List<Grupo> GrupoInterno { get; set; }

        [XmlAttribute(AttributeName = "classLectura")]
        public string ClassLectura { get; set; }

        [XmlText]
        public string Text { get; set; }

        [XmlElement(ElementName = "NameProp")]
        public List<string> NameProp { get; set; }

        [XmlElement(ElementName = "AtrNombreGrupo")]
        public string AtrNombreGrupo { get; set; }

        [XmlAttribute(AttributeName = "Tipo")]
        public string Tipo { get; set; }

        [XmlAttribute(AttributeName = "class")]
        public string Class { get; set; }
    }

    [XmlRoot(ElementName = "OrdenEntidad")]
    public class OrdenEntidad
    {

        [XmlElement(ElementName = "Grupo")]
        public Grupo Grupo { get; set; }
    }

    [XmlRoot(ElementName = "OrdenEntidadLectura")]
    public class OrdenEntidadLectura
    {

        [XmlElement(ElementName = "Grupo")]
        public List<Grupo> Grupo { get; set; }
    }

    [XmlRoot(ElementName = "Entidad")]
    public class Entidad
    {

        [XmlElement(ElementName = "AtrNombre")]
        public string AtrNombre { get; set; }

        [XmlElement(ElementName = "AtrNombreLectura")]
        public string AtrNombreLectura { get; set; }

        [XmlElement(ElementName = "OrdenEntidad")]
        public OrdenEntidad OrdenEntidad { get; set; }

        [XmlElement(ElementName = "OrdenEntidadLectura")]
        public OrdenEntidadLectura OrdenEntidadLectura { get; set; }

        [XmlAttribute(AttributeName = "ID")]
        public string ID { get; set; }

        [XmlText]
        public string Text { get; set; }

        [XmlElement(ElementName = "CampoOrden")]
        public string CampoOrden { get; set; }
    }

    [XmlRoot(ElementName = "EspefEntidad")]
    public class EspefEntidad
    {

        [XmlElement(ElementName = "Entidad")]
        public List<Entidad> Entidad { get; set; }
    }

    [XmlRoot(ElementName = "config")]
    public class XMLModel
    {

        [XmlElement(ElementName = "ConfiguracionGeneral")]
        public ConfiguracionGeneral ConfiguracionGeneral { get; set; }

        [XmlElement(ElementName = "EspefPropiedad")]
        public EspefPropiedad EspefPropiedad { get; set; }

        [XmlElement(ElementName = "EspefEntidad")]
        public EspefEntidad EspefEntidad { get; set; }
    }
}
