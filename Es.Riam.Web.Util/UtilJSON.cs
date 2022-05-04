using Newtonsoft.Json;

namespace Es.Riam.Gnoss.Web.Util
{
    /// <summary>
    /// Métodos de utilidad para manejar controles
    /// </summary>
    public class UtilJSON
    {
        public static void WriteElemento(string pID, bool pRender, string pInnerHTML, JsonTextWriter pWriter)
        {
            pWriter.WriteStartObject();
            pWriter.WritePropertyName("id");
            pWriter.WriteValue(pID);
            pWriter.WritePropertyName("render");
            pWriter.WriteValue(pRender);
            pWriter.WritePropertyName("innerHTML");
            pWriter.WriteValue(pInnerHTML);
            pWriter.WriteEndObject();
        }

        public static void WriteElemento(string pID, string pFunction, JsonTextWriter pWriter)
        {
            pWriter.WriteStartObject();
            pWriter.WritePropertyName("id");
            pWriter.WriteValue(pID);
            pWriter.WritePropertyName("render");
            pWriter.WriteValue(false);
            pWriter.WritePropertyName("funcion");
            pWriter.WriteValue(pFunction);
            pWriter.WriteEndObject();
        }

        public static void WriteElemento(string pID, string pFunction, string pInnerHTML, JsonTextWriter pWriter)
        {
            pWriter.WriteStartObject();
            pWriter.WritePropertyName("id");
            pWriter.WriteValue(pID);
            pWriter.WritePropertyName("render");
            pWriter.WriteValue(false);
            pWriter.WritePropertyName("funcion");
            pWriter.WriteValue(pFunction);
            pWriter.WritePropertyName("innerHTML");
            pWriter.WriteValue(pInnerHTML);
            pWriter.WriteEndObject();
        }

        public static void WriteElemento(string pID, string pFunction, string pAtributo, string pValor, JsonTextWriter pWriter)
        {
            pWriter.WriteStartObject();
            pWriter.WritePropertyName("id");
            pWriter.WriteValue(pID);
            pWriter.WritePropertyName("render");
            pWriter.WriteValue(false);
            pWriter.WritePropertyName("funcion");
            pWriter.WriteValue(pFunction);
            pWriter.WritePropertyName("atributo");
            pWriter.WriteValue(pAtributo);
            pWriter.WritePropertyName("valor");
            pWriter.WriteValue(pValor);
            pWriter.WriteEndObject();
        }

        public static void WriteElementoHTML(string pValor, JsonTextWriter pWriter)
        {
            pWriter.WriteStartObject();
            pWriter.WritePropertyName("html");
            pWriter.WriteValue(pValor);
            pWriter.WriteEndObject();
        }

        public static void WriteRemplazarElemento(string pID, string pInnerHTML, JsonTextWriter pWriter)
        {
            pWriter.WriteStartObject();
            pWriter.WritePropertyName("id");
            pWriter.WriteValue(pID);
            pWriter.WritePropertyName("render");
            pWriter.WriteValue(true);
            pWriter.WritePropertyName("reemplazarHtml");
            pWriter.WriteValue(true);
            pWriter.WritePropertyName("innerHTML");
            pWriter.WriteValue(pInnerHTML);
            pWriter.WriteEndObject();
        }

        public static void WriteAgregarHtmlAElemento(string pID, string pInnerHTML, JsonTextWriter pWriter)
        {
            pWriter.WriteStartObject();
            pWriter.WritePropertyName("id");
            pWriter.WriteValue(pID);
            pWriter.WritePropertyName("render");
            pWriter.WriteValue(true);
            pWriter.WritePropertyName("agregarHtml");
            pWriter.WriteValue(true);
            pWriter.WritePropertyName("innerHTML");
            pWriter.WriteValue(pInnerHTML);
            pWriter.WriteEndObject();
        }

        public static void WriteAgregarHtmlAlPrincipioElemento(string pID, string pInnerHTML, JsonTextWriter pWriter)
        {
            pWriter.WriteStartObject();
            pWriter.WritePropertyName("id");
            pWriter.WriteValue(pID);
            pWriter.WritePropertyName("render");
            pWriter.WriteValue(true);
            pWriter.WritePropertyName("agregarHtmlAlPrinc");
            pWriter.WriteValue(true);
            pWriter.WritePropertyName("innerHTML");
            pWriter.WriteValue(pInnerHTML);
            pWriter.WriteEndObject();
        }
    }
}
