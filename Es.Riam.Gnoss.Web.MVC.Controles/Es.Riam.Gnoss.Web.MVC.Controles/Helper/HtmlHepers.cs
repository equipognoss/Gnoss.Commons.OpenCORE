using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.CMS;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Controles.Controladores;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Util;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Es.Riam.Gnoss.Logica.Cookie;
using Es.Riam.Gnoss.AD.EntityModel.Models.Cookies;
using System.IO;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Primitives;
using static System.Net.Mime.MediaTypeNames;
using AngleSharp.Dom;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace Es.Riam.Gnoss.Web.MVC.Controles.Helper
{
    public static class HtmlHelpers
    {

        /// <summary>
        /// Método para conocer a través del ViewBag si el usuario es o no MetaAdministrador. Usado para necesidades del Front. En DevTools se puede realizar con permisosPaginas.        
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns>
        public static bool GetEsMetaAdministrador(this IHtmlHelper helper)
        {
            return (bool)helper.ViewBag.EsMetaAdministrador;
        }

        /// <summary>
        /// Devolver la clase correspondiente a una columna para la edición de páginas CMS, de tal forma que dependiendo del valor existe en base de datos (Ej: span12 break)
        /// el valor entendible y legible para el Page Builder de CMS.
        /// Devuelve un string con el con el valor correspondiente y un valor entero con el tamaño de la columna
        /// </summary>
        public static Tuple<string, string> GetValuesForGridColumnInCMSPage(this IHtmlHelper helper, String columnClassName)
        {

            // Span o clase para que sea guardada en bd
            string styleName = "span11";
            // Valor entero para establecer en la columna a modo de "Previsualización del width" de la columna
            string widthColumn = "100.0";

            // Es posible que columnClassName venga separado por espacios: Ej: span12
            if (!String.IsNullOrEmpty(columnClassName))
            {
                // Convertirlo a array
                var arrayColumnClassName = columnClassName.Split(" ");

                foreach (var className in arrayColumnClassName)
                {
                    switch (className)
                    {
                        case "span11":
                            // 100%
                            styleName = "span11";
                            widthColumn = "100.0";
                            break;

                        case "span12":
                            // 50%
                            styleName = "span12";
                            widthColumn = "50.0";
                            break;

                        case "span13":
                            // 33%
                            styleName = "span13";
                            widthColumn = "33.33";
                            break;

                        case "span14":
                            // 25%
                            styleName = "span14";
                            widthColumn = "25.0";
                            break;
                        case "span16":
                            // 16,66%
                            styleName = "span14";
                            widthColumn = "16.66";
                            break;
                        case "span23":
                            // 66,66%
                            styleName = "span23";
                            widthColumn = "66.66";
                            break;

                        case "span34":
                            // 75,0%
                            styleName = "span34";
                            widthColumn = "75.0";
                            break;

                        case "span38":
                            // 37,5%
                            styleName = "span38";
                            widthColumn = "37.5";
                            break;

                        case "span58":
                            // 62,5%
                            styleName = "span58";
                            widthColumn = "62.5";
                            break;

                        case "span56":
                            // 83,3%
                            styleName = "span58";
                            widthColumn = "83.3";
                            break;
                    }
                }
            }

            return Tuple.Create(widthColumn, styleName);
        }


        /// <summary>
        /// Obtener un diccionario de los valores almacenados en los diferentes idiomas: Ej: El valor en los idiomas disponibles de una página, url...
        /// Devuelve un diccionario con Key,Value siendo key un string con el idioma en cuestión y Value con el valor en ese idioma.
        /// El parámetro multiLanguageValueString tiene un formato así: NombreDeLaPagina@es|||PageName@en|||
        /// </summary>
        public static Dictionary<string, string> GetDictionaryValuesFromMultiLanguageItem(this IHtmlHelper helper, String multiLanguageValueString)
        {

            Dictionary<string, string> dictionaryWithMultiLanguageValues = new Dictionary<string, string>();

            // Obtener el nombre multiIdioma separado en arrayItems
            if (multiLanguageValueString == null)
            {
                multiLanguageValueString = "";
            }
            string[] arrayValues = multiLanguageValueString.Split("|||");

            // Recorrer cada item según los diferentes idiomas existentes
            foreach (var itemValue in arrayValues)
            {
                // Posición del separador del idioma
                int indexLanguageSeparator = itemValue.LastIndexOf("@");
                if (indexLanguageSeparator != -1)
                {
                    // Obtener el valor en el idioma que corresponda
                    string pageValue = itemValue.Substring(0, indexLanguageSeparator);
                    string pageLanguageKey = itemValue.Substring(indexLanguageSeparator + 1, itemValue.Length - indexLanguageSeparator - 1);
                    // Añadir el valor + key
                    dictionaryWithMultiLanguageValues.Add(pageLanguageKey, pageValue);
                }
            }
            // Devuelve el valor con los Idiomas (Key) y los idiomas (Value)
            return dictionaryWithMultiLanguageValues;
        }



        /// <summary>
        /// Obtener el nombre del tipo de Página de la comunidad para ser mostrado en el listado de páginas de Administración ** DevTools **
        /// </summary>
        public static string GetNombreTipoPaginaDevTools(this IHtmlHelper helper, TabModel pestanya)
        {
            // Nombre del tipo de la pestaña
            string nombreTipoPestanya = "";

            switch (pestanya.Type)
            {
                case TipoPestanyaMenu.Home:
                    nombreTipoPestanya = helper.GetText("COMMON", "HOME");
                    break;
                case TipoPestanyaMenu.Indice:
                    nombreTipoPestanya = helper.GetText("COMMON", "INDICE");
                    break;
                case TipoPestanyaMenu.Recursos:
                    nombreTipoPestanya = helper.GetText("COMMON", "RECURSOS");
                    break;
                case TipoPestanyaMenu.Preguntas:
                    nombreTipoPestanya = helper.GetText("COMMON", "PREGUNTAS");
                    break;
                case TipoPestanyaMenu.Debates:
                    nombreTipoPestanya = helper.GetText("COMMON", "DEBATES");
                    break;
                case TipoPestanyaMenu.Encuestas:
                    nombreTipoPestanya = helper.GetText("COMMON", "ENCUESTAS");
                    break;
                case TipoPestanyaMenu.PersonasYOrganizaciones:
                    nombreTipoPestanya = helper.GetText("COMMON", "PERSONASYORGANIZACIONES");
                    break;
                case TipoPestanyaMenu.AcercaDe:
                    nombreTipoPestanya = helper.GetText("COMMON", "ACERCADE");
                    break;
                case TipoPestanyaMenu.CMS:
                    nombreTipoPestanya = helper.GetText("COMMON", "CMS");
                    break;
                case TipoPestanyaMenu.BusquedaSemantica:
                    nombreTipoPestanya = helper.GetText("COMMON", "BUSQUEDASEMANTICA");
                    break;
                case TipoPestanyaMenu.EnlaceInterno:
                    nombreTipoPestanya = helper.GetText("COMMON", "ENLACEINTERNO");
                    break;
                case TipoPestanyaMenu.EnlaceExterno:
                    nombreTipoPestanya = helper.GetText("COMMON", "ENLACEEXTERNO");
                    break;
                case TipoPestanyaMenu.BusquedaAvanzada:
                    nombreTipoPestanya = helper.GetText("COMMON", "BUSQUEDAAVANZADA");
                    break;
            }

            return nombreTipoPestanya;
        }

        /// <summary>
        /// Obtener el título de la página para las Administración/DevTools. El título se mostrará en el Header o Cabecera del Layout y lo establecerá cada Controller
        /// </summary>
        public static string GetHeaderTitle(this IHtmlHelper helper)
        {
            return (string)helper.ViewBag.HeaderTitle;
        }





        /// <summary>
        /// Método que proporcionará en formato string un logo en formato "SVG" para ser usado en páginas de login o registro
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="socialNetworkName">Nombre de la red social de la que se desea obtener su icono</param>
        /// <param name="tamanyo">Tamaño del que se desesa obtener el icono de la red social. Por defecto, 40px</param>
        /// <returns></returns>
        public static string GetLogoForSocialNetworkName(this IHtmlHelper helper, string socialNetworkName, int tamanyo = 30)
        {
            // Logo que se devolverá. En caso de no encontrar ninguno, se devolverá "vacío".
            string currentLogo = "";

            string logoTwitterSvg = $@"
            <div style=""background-color: #000; display: inline-block;"">
                <svg fill=""#FFFFFF"" xmlns=""http://www.w3.org/2000/svg"" viewBox=""0 0 48 48"" width=""{tamanyo}px"" height=""{tamanyo}px"">
                <path d=""M 6.9199219 6 L 21.136719 26.726562 L 6.2285156 44 L 9.40625 44 L 22.544922 28.777344 L 32.986328 44 L 43 44 L 28.123047 22.3125 L 42.203125 6 L 39.027344 6 L 26.716797 20.261719 L 16.933594 6 L 6.9199219 6 z"" />
                </svg>
            </div>";

            string logoFacebookSvg = $@"
            <div style=""background-color: #1877F2; display: inline-block;"">
                <svg xmlns=""http://www.w3.org/2000/svg"" xmlns:xlink=""http://www.w3.org/1999/xlink"" viewBox=""0,0,256,256"" width=""{tamanyo}px"" height=""{tamanyo}px"">
                <g fill=""#ffffff"" fill-rule=""nonzero"" stroke=""none"" stroke-width=""1"" stroke-linecap=""butt"" stroke-linejoin=""miter"" stroke-miterlimit=""10"" stroke-dasharray="""" stroke-dashoffset=""0"" font-family=""none"" font-weight=""none"" font-size=""none"" text-anchor=""none"" style=""mix-blend-mode: normal"">
                    <g transform=""scale(5.12,5.12)"">
                        <path d=""M32,11h5c0.552,0 1,-0.448 1,-1v-6.737c0,-0.524 -0.403,-0.96 -0.925,-0.997c-1.591,-0.113 -4.699,-0.266 -6.934,-0.266c-6.141,0 -10.141,3.68 -10.141,10.368v6.632h-7c-0.552,0 -1,0.448 -1,1v7c0,0.552 0.448,1 1,1h7v19c0,0.552 0.448,1 1,1h7c0.552,0 1,-0.448 1,-1v-19h7.222c0.51,0 0.938,-0.383 0.994,-0.89l0.778,-7c0.066,-0.592 -0.398,-1.11 -0.994,-1.11h-8v-5c0,-1.657 1.343,-3 3,-3z""></path>
                    </g>
                </g>
                 </svg>
            </div>";

            string logoGoogleSvg = $@"
            <div style=""background-color: #fff; display: inline-block;"">
                <svg xmlns=""http://www.w3.org/2000/svg"" viewBox=""0 0 48 48"" width=""{tamanyo}px"" height=""{tamanyo}px"">
                <path fill=""#FFC107""                d=""M43.611,20.083H42V20H24v8h11.303c-1.649,4.657-6.08,8-11.303,8c-6.627,0-12-5.373-12-12c0-6.627,5.373-12,12-12c3.059,0,5.842,1.154,7.961,3.039l5.657-5.657C34.046,6.053,29.268,4,24,4C12.955,4,4,12.955,4,24c0,11.045,8.955,20,20,20c11.045,0,20-8.955,20-20C44,22.659,43.862,21.35,43.611,20.083z"" />
                <path fill=""#FF3D00""                d=""M6.306,14.691l6.571,4.819C14.655,15.108,18.961,12,24,12c3.059,0,5.842,1.154,7.961,3.039l5.657-5.657C34.046,6.053,29.268,4,24,4C16.318,4,9.656,8.337,6.306,14.691z"" />
                <path fill=""#4CAF50""               d=""M24,44c5.166,0,9.86-1.977,13.409-5.192l-6.19-5.238C29.211,35.091,26.715,36,24,36c-5.202,0-9.619-3.317-11.283-7.946l-6.522,5.025C9.505,39.556,16.227,44,24,44z"" />
                <path fill=""#1976D2""                d=""M43.611,20.083H42V20H24v8h11.303c-0.792,2.237-2.231,4.166-4.087,5.571c0.001-0.001,0.002-0.001,0.003-0.002l6.19,5.238C36.971,39.205,44,34,44,24C44,22.659,43.862,21.35,43.611,20.083z"" />
                </svg>
            </div>";

            string logoAppleSvg = $@"
            <div style=""background-color: #fff; display: inline-block;"">
                <svg xmlns=""http://www.w3.org/2000/svg"" viewBox=""0 0 26 26"" width=""{tamanyo}px"" height=""{tamanyo}px"">
                <path
                    d=""M 23.933594 18.945313 C 23.335938 20.269531 23.050781 20.863281 22.28125 22.03125 C 21.210938 23.667969 19.695313 25.707031 17.820313 25.71875 C 16.15625 25.734375 15.726563 24.632813 13.464844 24.652344 C 11.203125 24.660156 10.730469 25.738281 9.0625 25.722656 C 7.191406 25.707031 5.757813 23.867188 4.683594 22.238281 C 1.679688 17.664063 1.363281 12.300781 3.21875 9.449219 C 4.53125 7.425781 6.609375 6.238281 8.5625 6.238281 C 10.546875 6.238281 11.796875 7.328125 13.441406 7.328125 C 15.035156 7.328125 16.003906 6.234375 18.304688 6.234375 C 20.039063 6.234375 21.878906 7.179688 23.191406 8.816406 C 18.894531 11.167969 19.59375 17.304688 23.933594 18.945313 Z M 16.558594 4.40625 C 17.394531 3.335938 18.027344 1.820313 17.800781 0.277344 C 16.433594 0.371094 14.839844 1.242188 13.90625 2.367188 C 13.0625 3.394531 12.363281 4.921875 12.636719 6.398438 C 14.125 6.445313 15.664063 5.558594 16.558594 4.40625 Z"" />
                </svg>
            </div>";


            if (socialNetworkName.Contains("Facebook", StringComparison.OrdinalIgnoreCase) || socialNetworkName.Contains("Meta", StringComparison.OrdinalIgnoreCase))
            {
                currentLogo = logoFacebookSvg;
            }
            else if (socialNetworkName.Contains("Twitter", StringComparison.OrdinalIgnoreCase) || socialNetworkName.Contains("X", StringComparison.OrdinalIgnoreCase))
            {
                currentLogo = logoTwitterSvg;
            }
            else if (socialNetworkName.Contains("Google", StringComparison.OrdinalIgnoreCase))
            {
                currentLogo = logoGoogleSvg;
            }
            else if (socialNetworkName.Contains("Apple", StringComparison.OrdinalIgnoreCase))
            {
                currentLogo = logoAppleSvg;
            }

            // Si no se encuentra un logo asociable a la red Social, devolver la inicial de la red para que pinte algo
            if (string.IsNullOrEmpty(currentLogo))
            {
                currentLogo = $"<strong>{socialNetworkName.Substring(0, 1)}</strong>";
            }
            return currentLogo;
        }

        /// <summary>
        /// Obtener el padre del titulo o la categoría a la que pertenece el título de la página para las Administración/DevTools. El títutlo "padre" se mostrará en el Header o Cabecera del Layout y lo establecerá cada Controller
        /// </summary>
        public static string GetHeaderParentTitle(this IHtmlHelper helper)
        {
            string headerParentTitle = "";

            if (!string.IsNullOrEmpty((string)helper.ViewBag.HeaderParentTitle))
            {
                headerParentTitle = (string)helper.ViewBag.HeaderParentTitle;
            }
            return headerParentTitle;
        }

        /// <summary>
        /// Obtener el enum correspondiente a la sección Padre para la navegación en Administración/DevTools. Indicará en que sección Padre se encuentra el usuario.
        /// </summary>
        public static AdministracionSeccionesDevTools.SeccionesDevTools GetActiveSeccionDevTools(this IHtmlHelper helper)
        {
            return (AdministracionSeccionesDevTools.SeccionesDevTools)helper.ViewBag.ActiveSection;
        }

        /// <summary>
        /// Obtener el enum correspondiente a la subsección para la navegación en Administración/DevTools. Indicará en que subsección se encuentra el usuario.
        /// </summary>
        public static AdministracionSeccionesDevTools.SubSeccionesDevTools GetActiveSubSeccionDevTools(this IHtmlHelper helper)
        {
            return (AdministracionSeccionesDevTools.SubSeccionesDevTools)helper.ViewBag.ActiveSubSection;
        }

        /// <summary>
        /// Devolver el nombre del usuario dependiendo del modo del perfil del usuario (Personal, ProfessionalPersonal, ProfessionalCorporate)
        /// </summary>
        public static string GetUserNameByProfileMode(this IHtmlHelper helper, UserProfileModel perfil)
        {
            // Nombre de la persona
            string userNameByProfileMode = "";

            switch (perfil.TypeProfile)
            {
                case ProfileType.Personal:
                    userNameByProfileMode = perfil.Name;
                    break;
                case ProfileType.ProfessionalPersonal:
                    if (!String.IsNullOrEmpty(perfil.NameOrg))
                    {
                        userNameByProfileMode = perfil.Name + " @ " + perfil.NameOrg;
                    }
                    else
                    {
                        userNameByProfileMode = perfil.Name;
                    }
                    break;
                case ProfileType.ProfessionalCorporate:
                    userNameByProfileMode = perfil.NameOrg;
                    break;
                case ProfileType.Organization:
                    userNameByProfileMode = perfil.NameOrg;
                    break;
                default:
                    userNameByProfileMode = perfil.Name;
                    break;
            }
            return userNameByProfileMode;
        }


        /// <summary>
        /// Devolver el nombre de la foto/icono anonima dependiendo del modo del perfil del usuario (Personal, ProfessionalPersonal, ProfessionalCorporate)
        /// </summary>
        public static string GetAnonimousIconByProfileMode(this IHtmlHelper helper, UserProfileModel perfil)
        {
            // Nombre del icono a devolver
            string iconName = "";

            switch (perfil.TypeProfile)
            {
                case ProfileType.Personal:
                    iconName = "person";
                    break;
                case ProfileType.ProfessionalPersonal:
                    iconName = "person";
                    break;
                case ProfileType.ProfessionalCorporate:
                    iconName = "corporate_fare";
                    break;
                case ProfileType.Organization:
                    iconName = "corporate_fare";
                    break;
                default:
                    iconName = "person";
                    break;
            }
            return iconName;
        }


        /// <summary>
        /// Convertir la primera letra en maýuscula y dejar las demás en minúscula. Útil para evitar añadir nuevos textos traducidos y aprovechar los ya existentes
        /// </summary>
        public static string FirstLetterToUpper(this IHtmlHelper helper, string texto)
        {
            if (texto == null)
                return null;

            if (texto.Length > 1)
                return char.ToUpper(texto[0]) + texto.Substring(1).ToLower();

            return texto.ToUpper();
        }

        // Método para extraer palabras clave que puedan ser necesarias para construir urls a partir de propiedades (EJ: Construir urls en modales...)
        // Extraerá la cadena de texto que se encuentre entre el characterStart y characterEnd                
        // stringExtractFrom: Donde se realizará la búsqueda
        // characterToStart: String concreto que se encuentra antes de la cadena deseada a encontrar
        // characterToend: Último carácter o string que se encuentra después del deseado
        // 
        public static string ExtractValueInStringFromStartAndEnd(this IHtmlHelper helper, string stringExtractFrom, string characterStart, string characterEnd)
        {
            string extractString = stringExtractFrom;
            if (!string.IsNullOrEmpty(stringExtractFrom) && stringExtractFrom.IndexOf(characterStart) != -1 && stringExtractFrom.IndexOf(characterEnd) != -1)
            {
                var initialPosition = stringExtractFrom.IndexOf(characterStart) + characterStart.Length;
                var finalPosition = stringExtractFrom.IndexOf(characterEnd, initialPosition + 1);

                // Fragmento que deberá eliminarse
                extractString = stringExtractFrom.Substring(initialPosition, finalPosition - initialPosition);
            }
            return extractString;
        }

        private static Dictionary<string, string> ELEMENTOS_HTML_LIMPIAR = new Dictionary<string, string>() { { "<pre ", "</pre>" }, { "<img ", ">" }, { "<iframe ", "</iframe>" }, { "<table", "</table>" }, { "<strong>", "<strong>" }, { "</strong>", "</strong>" }, { "<ul", ">" }, { "</ul>", "</ul>" }, { "<ol", ">" }, { "</ol>", "</ol>" }, { "<a", ">" }, { "</a>", "</a>" }, { "<span", ">" }, { "</span>", "</span>" }, { "<h1", ">" }, { "</h1>", "</h1>" }, { "<h2", ">" }, { "</h2>", "</h2>" }, { "<h3>", ">" }, { "</h3>", "</h3>" }, { "<h4>", ">" }, { "</h4>", "</h4>" }, { "<figure", ">" }, { "<em", ">" }, { "</em>", "</em>" }, { "<b>", ">" }, { "</b>", "</b>" }, { "<li>", ">" }, { "</li>", "</li>" }, { "<i>", "</i>" }, { "<code>", "</code>" } };

        // Método para limpiar un String que contiene imagenes, videos de youtube pero no se desean ser mostrados (Ej: Ficha Mini Recurso)
        public static string CleanHtmlFromMultimediaItems(this IHtmlHelper helper, string stringHtml)
        {
            if (!string.IsNullOrEmpty(stringHtml))
            {
                foreach (string clave in ELEMENTOS_HTML_LIMPIAR.Keys)
                {
                    stringHtml = LimpiarFragmentoHTML(stringHtml, clave, ELEMENTOS_HTML_LIMPIAR[clave]);
                }
            }
            return stringHtml;

        }


        private static readonly List<string> ELEMENTOS_HTML_MENSAJES_LIMPIAR = new List<string> { "img", "iframe", "table", "strong", "ul", "ol", "a", "span", "h1", "h2", "h3", "h4", "figure", "em", "b", "li", "i", "article" };

        /// <summary>
        /// Método para limpiar las etiquetas de los mensajes con el objetivo de que su visualización sea correcta en la Bandeja de entrada       
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="stringHtml"></param>
        /// <returns></returns>
        public static string CleanHtmlInMessages(this IHtmlHelper helper, string stringHtml)
        {
            if (!string.IsNullOrEmpty(stringHtml))
            {
                foreach (string elemento in ELEMENTOS_HTML_MENSAJES_LIMPIAR)
                {
                    // Eliminar etiquetas de apertura
                    stringHtml = Regex.Replace(stringHtml, $"<{Regex.Escape(elemento)}[^>]*>", " ", RegexOptions.IgnoreCase);
                    // Eliminar etiquetas de cierre correspondientes
                    stringHtml = Regex.Replace(stringHtml, $"</{Regex.Escape(elemento)}>", " ", RegexOptions.IgnoreCase);
                }
            }
            return stringHtml;
        }



        private static string LimpiarFragmentoHTML(string stringHtml, string inicio, string fin)
        {
            int inicioIndex = stringHtml.IndexOf(inicio);
            while (inicioIndex != -1)
            {
                int finIndex = stringHtml.IndexOf(fin, inicioIndex);
                if (finIndex != -1)
                {
                    stringHtml = stringHtml.Remove(inicioIndex, finIndex - inicioIndex + fin.Length);
                }
                else
                {
                    break;
                }
                inicioIndex = stringHtml.IndexOf(inicio, inicioIndex);
            }
            return stringHtml;
        }


        // Método para de parrafo (<p>) un texto para que no se muestre en pantalla para que no genere espacio adicional        
        public static string CleanHtmlParagraphsStringHtml(this IHtmlHelper helper, string stringHtml)
        {
            if (!string.IsNullOrEmpty(stringHtml))
            {
                // Contendrá el stringHtml pero sin las imagenes, vídeos incrustadas                        
                while (stringHtml.Contains("<p>") || stringHtml.Contains("</p>") || stringHtml.Contains("<p "))
                {
                    int initialPosition = 0;
                    int finalPosition = 0;

                    string descriptionToRemove = "";

                    // Controlar si dispone de párrafos
                    if (stringHtml.Contains("<p>"))
                    {
                        initialPosition = stringHtml.IndexOf("<p>");
                        finalPosition = stringHtml.IndexOf(">", initialPosition);
                        // Fragmento que deberá eliminarse
                        descriptionToRemove = stringHtml.Substring(initialPosition, finalPosition - initialPosition + 1);
                    }
                    else if (stringHtml.Contains("</p>"))
                    {
                        initialPosition = stringHtml.IndexOf("</p>");
                        finalPosition = stringHtml.IndexOf(">", initialPosition);
                        // Fragmento que deberá eliminarse
                        descriptionToRemove = stringHtml.Substring(initialPosition, finalPosition - initialPosition + 1);
                    }
                    else if (stringHtml.Contains("<p "))
                    {
                        initialPosition = stringHtml.IndexOf("<p ");
                        finalPosition = stringHtml.IndexOf(">", initialPosition);
                        // Fragmento que deberá eliminarse
                        descriptionToRemove = stringHtml.Substring(initialPosition, finalPosition - initialPosition + 1);
                    }

                    stringHtml = stringHtml.Replace(descriptionToRemove, "");
                }
            }
            return (stringHtml);
        }

        // Método para truncar texto y añadirle "..." al final        
        public static string TruncateString(this IHtmlHelper helper, string value, int pMaxChars)
        {
            return value.Length <= pMaxChars ? value : value.Substring(0, pMaxChars) + "...";
        }

        public static string ObtenerNombreClasePorTipoRecurso(this IHtmlHelper helper, ResourceModel.DocumentType pDocumentType)
        {
            // Devolver el nombre de recurso válido para ciertos casos:
            string nombreClasePorTipoRecurso = "";

            switch (pDocumentType)
            {
                case ResourceModel.DocumentType.FicheroServidor:
                    nombreClasePorTipoRecurso = "file_download";
                    break;

                case ResourceModel.DocumentType.Hipervinculo:
                    nombreClasePorTipoRecurso = "link";
                    break;

                case ResourceModel.DocumentType.Encuesta:
                    nombreClasePorTipoRecurso = "poll";
                    break;

                case ResourceModel.DocumentType.Blog:
                    nombreClasePorTipoRecurso = "note";
                    break;

                case ResourceModel.DocumentType.Debate:
                    nombreClasePorTipoRecurso = "discussion";
                    break;

                case ResourceModel.DocumentType.Pregunta:
                    nombreClasePorTipoRecurso = "question";
                    break;

                case ResourceModel.DocumentType.Imagen:
                    nombreClasePorTipoRecurso = "image";
                    break;

                case ResourceModel.DocumentType.Video:
                    nombreClasePorTipoRecurso = "video";
                    break;

                case ResourceModel.DocumentType.Nota:
                    nombreClasePorTipoRecurso = "note";
                    break;

                default:
                    nombreClasePorTipoRecurso = "article";
                    break;
            }

            return (nombreClasePorTipoRecurso);
        }

        //Obtener el nombre correcto según el tipo de recurso a crear/editar ya que por defecto lo trae según está en en 'enum DocumentType'
        public static string ObtenerNombreRecursoConTildes(this IHtmlHelper helper, ResourceModel.DocumentType pDocumentType)
        {
            // Devolver el nombre de recurso válido para ciertos casos:
            string nombreRecursoConTildes = "";

            switch (pDocumentType)
            {
                case ResourceModel.DocumentType.Hipervinculo:
                    nombreRecursoConTildes = "Hipervínculo";
                    break;
                case ResourceModel.DocumentType.FicheroServidor:
                    nombreRecursoConTildes = "Archivo Adjunto";
                    break;
                case ResourceModel.DocumentType.Semantico:
                    nombreRecursoConTildes = "Semántico";
                    break;
                default:
                    nombreRecursoConTildes = pDocumentType.ToString();
                    break;
            }

            return (nombreRecursoConTildes.ToLower());
        }
        //Obtener fotografía anónima del perfil si es Profesional o Empresa.        
        public static string ObtenerFotoAnonimaDePerfil(this IHtmlHelper helper, ProfileType pTipoPerfil)
        {
            if (pTipoPerfil == ProfileType.ProfessionalCorporate)
            {
                return "/" + Es.Riam.Util.UtilArchivos.ContentImagenes + "/" + Es.Riam.Util.UtilArchivos.ContentImagenesOrganizaciones + "/" + "anonimo_peque.png";
            }
            else
            {
                return "/" + Es.Riam.Util.UtilArchivos.ContentImagenes + "/" + Es.Riam.Util.UtilArchivos.ContentImagenesPersonas + "/" + "anonimo_peque.png";
            }
        }

        // Obtener el nombre de tipo de comunidad (Privada, Pública, Reservada, Restringida)
        public static string getCommunityNameType(this IHtmlHelper helper, CommunityModel pCommunity)
        {
            string communityNameType = "";
            switch ((CommunityModel.TypeAccessProject)pCommunity.AccessType)
            {
                case CommunityModel.TypeAccessProject.Private:
                    communityNameType = helper.GetText("COMUNIDADES", "COMUNIDADPRIVADA");
                    break;
                case CommunityModel.TypeAccessProject.Public:
                    communityNameType = helper.GetText("COMUNIDADES", "COMUNIDADPUBLICA");
                    break;
                case CommunityModel.TypeAccessProject.Reserved:
                    communityNameType = helper.GetText("COMUNIDADES", "COMUNIDADRESERVADA");
                    break;
                case CommunityModel.TypeAccessProject.Restricted:
                    communityNameType = helper.GetText("COMUNIDADES", "COMUNIDADRESTRINGIDA");
                    break;
                default:
                    return communityNameType;
            }
            return communityNameType;
        }

        // Obtener el nombre del icono de material-icons dependiendo del tipo de Comunidad
        public static string getCommunityIconType(this IHtmlHelper helper, CommunityModel pCommunity)
        {

            string communityNameIconType = "";
            switch ((CommunityModel.TypeAccessProject)pCommunity.AccessType)
            {
                case CommunityModel.TypeAccessProject.Private:
                case CommunityModel.TypeAccessProject.Reserved:
                case CommunityModel.TypeAccessProject.Restricted:
                    communityNameIconType = "vpn_key";
                    break;
                case CommunityModel.TypeAccessProject.Public:
                    communityNameIconType = "visibility";
                    break;
                default:
                    return "";
            }
            return communityNameIconType;
        }

        // Obtener el estado de la comunidad (Cerrada, Abierta ...)
        public static string getCommunityStateType(this IHtmlHelper helper, CommunityModel pCommunity)
        {
            String communityState = "";

            switch ((CommunityModel.StateProject)pCommunity.ProjectState)
            {
                case CommunityModel.StateProject.Close:
                    communityState = helper.GetText("COMADMIN", "CERRADO");
                    break;
                case CommunityModel.StateProject.CloseTemporaly:
                    communityState = helper.GetText("COMADMIN", "CERRADOTMP");
                    break;
                case CommunityModel.StateProject.Closing:
                    communityState = helper.GetText("COMADMIN", "CERRANDOSE");
                    break;
                case CommunityModel.StateProject.Definition:
                    communityState = helper.GetText("COMADMIN", "DEFINICION");
                    break;
                default:
                    return communityState;
            }
            return communityState;
        }

        // Método utilizado para saber si la página actual es la de editar un recurso. De ser así, devolverá "false" para que no se muestre el botón de "Añadir recurso"
        public static bool EsPaginaEdicionRecurso(this IHtmlHelper helper)
        {
            if (helper.ViewBag.EsPaginaEdicionRecurso != null)
            {
                return (bool)helper.ViewBag.EsPaginaEdicionRecurso;
            }
            else
            {
                return false;
            }
        }

        public static bool EsPaginaEspacioPersonal(this IHtmlHelper helper)
        {
            if (helper.ViewBag.EsPaginaEspacioPersonal != null)
            {
                return (bool)helper.ViewBag.EsPaginaEspacioPersonal;
            }
            else
            {
                return false;
            }
        }

        public static bool EsPaginaContribuciones(this IHtmlHelper helper)
        {
            if (helper.ViewBag.EsPaginaContribuciones != null)
            {
                return (bool)helper.ViewBag.EsPaginaContribuciones;
            }
            else
            {
                return false;
            }
        }



        public static string GetSessionTimeout(this IHtmlHelper helper)
        {
            return (string)helper.ViewBag.SessionTimeout;
        }

        //public static Dictionary<string, string> GetParametrosAplicacion(this IHtmlHelper helper)
        //{
        //    return helper.ViewBag.ParametrosAplicacion;
        //}

        public static Dictionary<Guid, ProfileModel> GetIdentitiesByUserID(this IHtmlHelper helper, List<Guid> pListUsers, bool pExtraData = false)
        {
            ControladorProyectoMVC controladorProyectoMVC = helper.ViewBag.ControladorProyectoMVC;
            return controladorProyectoMVC.ObtenerIdentidadesPorIDUsuarios(pListUsers, pExtraData);
        }

        public static Dictionary<Guid, ProfileModel> GetIdentitiesByID(this IHtmlHelper helper, List<Guid> pListIdentities, bool pExtraData = false)
        {
            ControladorProyectoMVC controladorProyectoMVC = helper.ViewBag.ControladorProyectoMVC;
            return controladorProyectoMVC.ObtenerIdentidadesPorID(pListIdentities, pExtraData);
        }

        public static string GetText(this IHtmlHelper helper, string pPage, string pText)
        {
            if (helper.ViewBag.UtilIdiomas != null)
            {
                return helper.ViewBag.UtilIdiomas.GetText(pPage, pText);
            }
            return pText;
        }

        public static string GetText(this IHtmlHelper helper, string pPage, string pText, params string[] pParametros)
        {
            if (helper.ViewBag.UtilIdiomas != null)
            {
                return helper.ViewBag.UtilIdiomas.GetText(pPage, pText, pParametros);
            }
            return pText;
        }

        /// <summary>
        /// Obtiene una fecha a través de un string con 14 caracteres de la forma '20150501123000' o un string de la forma 20/08/1984
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="pDate"></param>
        /// <returns></returns>
        public static DateTime? GetDate(this IHtmlHelper helper, string pDate)
        {
            DateTime? fecha = null;
            try
            {
                int anio = 0;
                int mes = 0;
                int dia = 0;
                int hora = 0;
                int minuto = 0;
                int segundo = 0;
                if (pDate.Length == 14)
                {
                    anio = int.Parse(pDate.Substring(0, 4));
                    mes = int.Parse(pDate.Substring(4, 2));
                    dia = int.Parse(pDate.Substring(6, 2));
                    hora = int.Parse(pDate.Substring(8, 2));
                    minuto = int.Parse(pDate.Substring(10, 2));
                    segundo = int.Parse(pDate.Substring(12, 2));
                }
                else if (pDate.Length >= 10 && pDate.Contains("/"))
                {
                    anio = int.Parse(pDate.Substring(6, 4));
                    mes = int.Parse(pDate.Substring(3, 2));
                    dia = int.Parse(pDate.Substring(0, 2));
                }

                fecha = new DateTime(anio, mes, dia, hora, minuto, segundo);
            }
            catch
            {
                //Controlamos que si el texto no es una fecha no falle la aplicación.
            }

            return fecha;
        }

        public static string EliminarCaracteresUrlSem(this IHtmlHelper helper, string pText)
        {
            return UtilCadenas.EliminarCaracteresUrlSem(pText);
        }

        public static string ObtenerTextoDeIdioma(this IHtmlHelper helper, string pText)
        {
            return ObtenerTextoDeIdioma(helper, pText, helper.ViewBag.UtilIdiomas.LanguageCode, "es");
        }

        public static string ObtenerTextoDeIdioma(this IHtmlHelper helper, string pText, string pIdioma, string pIdiomaDefecto, bool pSoloIdiomaIndicado = false)
        {
            return UtilCadenas.ObtenerTextoDeIdioma(pText, pIdioma, pIdiomaDefecto, pSoloIdiomaIndicado);
        }

        public static string ObtenerTextoIdiomaUsuario(this IHtmlHelper helper, string pTexto, bool pTraerPrimeroSiNoEncuentra = false)
        {
            return ObtenerTextoDeIdioma(helper, pTexto, helper.GetUtilIdiomas().LanguageCode, null, !pTraerPrimeroSiNoEncuentra);
        }

        private static bool TienePersonalizacion(this IHtmlHelper htmlHelper)
        {
            CommunityModel Comunidad = htmlHelper.ViewBag.Comunidad;
            bool tienePersonalizacion = false;

            if ((!string.IsNullOrEmpty((string)htmlHelper.ViewBag.Personalizacion) || !string.IsNullOrEmpty((string)htmlHelper.ViewBag.PersonalizacionEcosistema) || !string.IsNullOrEmpty((string)htmlHelper.ViewBag.PersonalizacionDominio)) && Comunidad != null)
            {
                tienePersonalizacion = true;
            }

            return tienePersonalizacion;
        }

        public static IHtmlContent PartialView(this IHtmlHelper htmlHelper, string partialViewName)
        {
            IHtmlContent resultado = null;
            CommunityModel Comunidad = htmlHelper.ViewBag.Comunidad;

            if (TienePersonalizacion(htmlHelper))
            {
                string personalizacion = MultiViewResult.ComprobarPersonalizacion(htmlHelper.ViewBag, Comunidad, partialViewName);
                try
                {
                    resultado = HtmlHelperPartialExtensions.Partial(htmlHelper, partialViewName + personalizacion);
                }
                catch (Exception ex)
                {
                    LoggingService loggingService = new LoggingService(null, null, null);

                    loggingService.GuardarLogError(ex);
                }
            }

            if (resultado == null)
            {
                resultado = HtmlHelperPartialExtensions.Partial(htmlHelper, partialViewName);
            }

            return resultado;
        }

        public static IHtmlContent PartialView(this IHtmlHelper htmlHelper, string partialViewName, Object model)
        {
            IHtmlContent resultado = null;
            CommunityModel Comunidad = htmlHelper.ViewBag.Comunidad;
            DateTime date = DateTime.Now;
            LoggingService loggingService = new LoggingService(null, null, null);
            try
            {
                if (TienePersonalizacion(htmlHelper))
                {
                    string personalizacion = MultiViewResult.ComprobarPersonalizacion(htmlHelper.ViewBag, Comunidad, partialViewName);
                    try
                    {
                        resultado = HtmlHelperPartialExtensions.Partial(htmlHelper, partialViewName + personalizacion, model);
                    }
                    catch (Exception ex)
                    {
                        loggingService.GuardarLogError(ex);
                    }
                }

                if (resultado == null)
                {
                    resultado = HtmlHelperPartialExtensions.Partial(htmlHelper, partialViewName, model);
                }
            }
            catch (Exception ex)
            {
                loggingService.GuardarLogError(ex);
            }

            return resultado;
        }
        public static IHtmlContent PartialView(this IHtmlHelper htmlHelper, string partialViewName, object model, ViewDataDictionary diccionario)
        {
            IHtmlContent resultado = null;
            CommunityModel Comunidad = htmlHelper.ViewBag.Comunidad;

            try
            {
                if (TienePersonalizacion(htmlHelper))
                {
                    LoggingService loggingService = new LoggingService(null, null, null);
                    string personalizacion = MultiViewResult.ComprobarPersonalizacion(htmlHelper.ViewBag, Comunidad, partialViewName);
                    try
                    {
                        resultado = HtmlHelperPartialExtensions.Partial(htmlHelper, partialViewName + personalizacion, model, diccionario);
                    }
                    catch (Exception ex)
                    {
                        loggingService.GuardarLogError(ex);
                    }
                }

                if (resultado == null)
                {
                    resultado = HtmlHelperPartialExtensions.Partial(htmlHelper, partialViewName, model, diccionario);
                }
            }
            catch (Exception ex)
            {
                //TODO Poner LoggingService en viewBag
                //loggingService.GuardarLogError(ex);
            }

            return resultado;
        }

        public static string ObtenerImagenConTamano(this IHtmlHelper helper, string pUrlImg, int pTamaño)
        {
            if (!string.IsNullOrEmpty(pUrlImg))
            {
                return pUrlImg.Substring(0, pUrlImg.LastIndexOf('.')) + "_" + pTamaño + pUrlImg.Substring(pUrlImg.LastIndexOf('.'));
            }
            else
            {
                return "";
            }
        }

        public static string AcortarTexto(this IHtmlHelper helper, string pTexto, int pNumCaracteres)
        {
            return UtilCadenas.AcortarTexto(pTexto, pNumCaracteres);
        }

        public static string RequestParams(this IHtmlHelper helper, string nameParam, HttpContext httpContext)
        {
            var httpRequest = httpContext.Request;
            if (httpRequest.Query.TryGetValue(nameParam, out StringValues parametros))
            {
                return parametros;
            }
            return null;
        }

        private static string EliminarTablas(string pDescripcion)
        {
            return pDescripcion;
        }

        public static string AcortarDescripcionHtml(this IHtmlHelper helper, string pTexto, int pNumCaracteres)
        {
            return UtilCadenas.AcortarDescripcionHtml(pTexto, pNumCaracteres);
        }

        public static string AcortarDescripcionHtmlPorNumeroParrafos(this IHtmlHelper helper, string pTexto, int pNumParrafos)
        {
            return UtilCadenas.AcortarDescripcionHtmlPorNumeroParrafos(pTexto, pNumParrafos);
        }

        public static string ConvertirPrimeraLetraDeFraseAMayusculas(this IHtmlHelper helper, string pTexto)
        {
            return UtilCadenas.ConvertirPrimeraLetraDeFraseAMayúsculas(pTexto);
        }

        public static string ConvertirPrimeraLetraPalabraAMayusculas(this IHtmlHelper helper, string pTexto)
        {
            return UtilCadenas.ConvertirPrimeraLetraPalabraAMayusculas(pTexto);
        }

        public static string ObtenerUrlDeDoc(this IHtmlHelper helper, Guid idDocumento, string pNombreDocumento)
        {
            CommunityModel Comunidad = helper.ViewBag.Comunidad;
            UserProfileModel Perfil = helper.ViewBag.Perfil;
            string urlPerfil = null;
            if (Perfil != null)
            {
                urlPerfil = Perfil.Url;
            }
            GnossUrlsSemanticas gnossUrlsSemanticas = GetGeneradorURLs(helper);

            return gnossUrlsSemanticas.GetURLBaseRecursosFichaConIDs(helper.ViewBag.BaseUrlIdioma, helper.ViewBag.UtilIdiomas, Comunidad.ShortName, urlPerfil, Es.Riam.Util.UtilCadenas.EliminarCaracteresUrlSem(pNombreDocumento), idDocumento, null, false);
        }

        public static string ObtenerNombreCompletoDeFichaIdentidad(this IHtmlHelper helper, ProfileModel pIdentidad)
        {
            string nombreCompleto = "";
            if (pIdentidad.TypeProfile == ProfileType.Personal || pIdentidad.TypeProfile == ProfileType.Teacher)
            {
                nombreCompleto = pIdentidad.NamePerson;
            }
            else if (pIdentidad.TypeProfile == ProfileType.ProfessionalPersonal)
            {
                nombreCompleto = pIdentidad.NamePerson + " " + ConstantesDeSeparacion.SEPARACION_CONCATENADOR + " " + pIdentidad.NameOrganization;
            }
            else if (pIdentidad.TypeProfile == ProfileType.ProfessionalCorporate && !string.IsNullOrEmpty(pIdentidad.NamePerson))
            {
                nombreCompleto = pIdentidad.NameOrganization + " (" + pIdentidad.NamePerson + ")";
            }
            else
            {
                nombreCompleto = pIdentidad.NameOrganization;
            }
            return nombreCompleto;
        }

        public static string ObtenerNombrePerfil(this IHtmlHelper helper, ProfileModel pPerfil)
        {
            string nombrePerfil = "";
            if (pPerfil != null)
            {
                if (!string.IsNullOrEmpty(pPerfil.NamePerson))
                {
                    nombrePerfil += pPerfil.NamePerson;
                }

                if (!string.IsNullOrEmpty(pPerfil.NameOrganization))
                {
                    if (!string.IsNullOrEmpty(pPerfil.NamePerson))
                    {
                        nombrePerfil += " " + ConstantesDeSeparacion.SEPARACION_CONCATENADOR + " ";
                    }
                    nombrePerfil += pPerfil.NameOrganization;
                }
            }
            else
            {
                nombrePerfil = "MyGnoss";
            }
            return nombrePerfil;
        }

        public static string ObtenerUrlPerfil(this IHtmlHelper helper, ProfileModel pPerfil)
        {
            string urlPerfil = "";
            if (pPerfil != null)
            {
                if (!string.IsNullOrEmpty(pPerfil.UrlPerson))
                {
                    urlPerfil += pPerfil.UrlPerson;
                }
                else if (!string.IsNullOrEmpty(pPerfil.UrlOrganization))
                {
                    urlPerfil += pPerfil.UrlOrganization;
                }
            }
            else
            {
                urlPerfil = "#";
            }
            return urlPerfil;
        }

        /// <summary>
        /// Genera un parámetro con valor correctamente. Ejemplo: class="active".
        /// </summary>
        /// <param name="pParametro">Parámetro</param>
        /// <param name="pValor">Valor del parámetro</param>
        /// <returns>Parámetro con valor correctamente. Ejemplo: class="active"</returns>
        public static IHtmlContent GetParam(this IHtmlHelper helper, string pParametro, string pValor)
        {
            if (string.IsNullOrEmpty(pValor))
            {
                return helper.Raw("");
            }
            else
            {
                return helper.Raw(pParametro + "=\"" + pValor + "\"");
            }
        }

        /// <summary>
        /// Obtiene el aviso de aceptar cookies.
        /// </summary>
        public static string GetCookiesWarning(this IHtmlHelper helper)
        {
            return (string)helper.ViewBag.CookiesWarning;
        }

        public static string GetCookiesText(this IHtmlHelper helper)
        {
            return (string)helper.ViewBag.TextoPoliticaCookies;
        }

        /// <summary>
        /// Traduce el texto asociado al Texto ID al idioma de la comunidad.
        /// </summary>
        /// <param name="pTextoID">Identificador del texto</param>
        /// <param name="pIdioma">El Idioma del texto</param>
        /// <returns>El texto traducido</returns>
        public static string Translate(this IHtmlHelper helper, string pTextoID, params string[] pParams)
        {
            string resultado = helper.ViewBag.UtilIdiomas.GetTextoPersonalizado(pTextoID, pParams);

            if (string.IsNullOrEmpty(resultado))
            {
                resultado = pTextoID;
            }

            return resultado;
        }

        /// <summary>
        /// Elimina el primer parrafo de un texto si lo tiene.
        /// </summary>
        /// <param name="helper">Helper</param>
        /// <param name="pText">Texto</param>
        /// <returns>Texto sin el primer parrafo de un texto si lo tiene</returns>
        public static string DeleteFirstParagraph(this IHtmlHelper helper, string pText)
        {
            string texto = string.Empty;

            if (!string.IsNullOrEmpty(pText) && pText.StartsWith("<p>") && pText.EndsWith("</p>"))
            {
                texto = pText.Substring(3).Replace(pText.Substring(pText.LastIndexOf("</p>")), string.Empty);
            }
            else
            {
                texto = pText;
            }

            return texto;
        }

        /// <summary>
        /// Obtiene la url para obtener los contextos de un recurso
        /// </summary>
        /// <param name="helper">Helper</param>
        /// <param name="pResourceUrl">Url completa del recurso</param>
        /// <returns>Url a la que hay que realizar una petición POST que obtendría todos los contextos del recurso</returns>
        public static string GetUrlContext(this IHtmlHelper helper, string pResourceUrl, Guid pResourceID, string pCommunityShortName)
        {
            string urlContexto = ControladorProyectoMVC.ObtenerUrlRecursoParaPeticionServicioContextos(helper.ViewBag.UrlServicioContextos, pResourceUrl, pResourceID, pCommunityShortName);

            if (!string.IsNullOrEmpty(urlContexto) && !urlContexto.Equals(pResourceUrl))
            {
                //Si es distinta, significa que hay un servicio de contextos
                urlContexto += "/context";

                if (helper.GetUtilIdiomas() != null)
                {
                    urlContexto += "/" + helper.GetUtilIdiomas().LanguageCode;
                }

            }

            return urlContexto;
        }

        /// <summary>
        /// Obtiene la url principal del ecosistema
        /// </summary>
        /// <param name="helper">Helper</param>
        /// <param name="pResourceUrl">Url completa del recurso</param>
        /// <returns>Url a la que hay que realizar una petición POST que obtendría todos los contextos del recurso</returns>
        public static string GetUrlComunidadPrincipal(this IHtmlHelper helper)
        {
            return helper.ViewBag.UrlComunidadPrincipal;
        }

        public static string GetUrlLogout(this IHtmlHelper helper)
        {
            CommunityModel comunidad = helper.GetComunidad();
            string linkDesconectar = string.Concat(comunidad.Url, "/", helper.GetUtilIdiomas().GetText("URLSEM", "DESCONECTAR"));
            if (!comunidad.Key.Equals(ProyectoAD.MetaProyecto))
            {
                linkDesconectar += "/redirect/" + helper.GetUtilIdiomas().GetText("URLSEM", "COMUNIDAD") + "/" + comunidad.ShortName;
            }

            return linkDesconectar;
        }

        /// <summary>
        /// Permite obtener una SemanticProperty en el idioma de navegación
        /// </summary>
        /// <param name="helper">Helper</param>
        /// <param name="propiedad">Lista con los diferentes idiomas de la propiedad</param>
        /// <returns></returns>
        public static string ObtenerPropiedadTraducida(this IHtmlHelper helper, List<SemanticPropertieModel> propiedad)
        {
            string resultado = string.Empty;
            // Buscamos en el idioma del usuario
            var propiedades = propiedad.Where(x => x.Url.EndsWith("@" + helper.GetUtilIdiomas().LanguageCode));
            if (propiedades.Any())
            {
                resultado = propiedades.First().Name;
            }

            // Si el idioma es alguno de España y no estaba buscamos en español
            if (string.IsNullOrEmpty(resultado) && (helper.GetUtilIdiomas().LanguageCode.Equals("gl") || helper.GetUtilIdiomas().LanguageCode.Equals("ca") || helper.GetUtilIdiomas().LanguageCode.Equals("eu")))
            {
                propiedades = propiedad.Where(x => x.Url.EndsWith("@es"));
                if (propiedades.Any())
                {
                    resultado = propiedades.First().Name;
                }
            }

            // Ci no está en el idioma del usuario buscamos en ingles
            if (string.IsNullOrEmpty(resultado))
            {
                propiedades = propiedad.Where(x => x.Url.EndsWith("@en"));
                if (propiedades.Any())
                {
                    resultado = propiedades.First().Name;
                }
            }

            // Si tampoco está en ingles buscamos en español
            if (string.IsNullOrEmpty(resultado))
            {
                propiedades = propiedad.Where(x => x.Url.EndsWith("@es"));
                if (propiedades.Any())
                {
                    resultado = propiedades.First().Name;
                }
            }

            // si no encontramos ninguno añadimos el primero 
            if (string.IsNullOrEmpty(resultado))
            {
                resultado = propiedad.First().Name;
            }
            return resultado;
        }

        #region ViewBag Getters

        public static CommunityModel GetComunidad(this IHtmlHelper helper)
        {
            return (CommunityModel)helper.ViewBag.Comunidad;
        }

        public static UserProfileModel GetPerfil(this IHtmlHelper helper)
        {
            return (UserProfileModel)helper.ViewBag.Perfil;
        }

        public static string GetBodyClass(this IHtmlHelper helper)
        {
            string bodyClass = (string)helper.ViewBag.BodyClass;

            if (string.IsNullOrEmpty(bodyClass))
            {
                return "";
            }

            return (string)helper.ViewBag.BodyClass;
        }

        public static string GetBodyClassPestanya(this IHtmlHelper helper)
        {
            string BodyClassPestanya = (string)helper.ViewBag.BodyClassPestanya;

            if (string.IsNullOrEmpty(BodyClassPestanya))
            {
                return "";
            }

            return (string)helper.ViewBag.BodyClassPestanya;

        }

        public static string GetMetaDescriptionPestanya(this IHtmlHelper helper)
        {
            return (string)helper.ViewBag.MetaDescriptionPestanya;
        }



        public static string GetBaseUrlIdioma(this IHtmlHelper helper)
        {
            return (string)helper.ViewBag.BaseUrlIdioma;
        }

        public static bool GetOcultarPersonalizacion(this IHtmlHelper helper)
        {
            if (helper.ViewBag.OcultarPersonalizacion != null)
            {
                return (bool)helper.ViewBag.OcultarPersonalizacion;
            }

            return false;
        }

        public static string GetPersonalizacion(this IHtmlHelper helper)
        {
            return (string)helper.ViewBag.Personalizacion;
        }

        public static string GetPersonalizacionLayout(this IHtmlHelper helper)
        {
            return (string)helper.ViewBag.PersonalizacionLayout;
        }

        public static string GetTextoSubida(this IHtmlHelper helper)
        {
            return (string)helper.ViewBag.TextoSubida;
        }

        public static ResultadoModel GetResultado(this IHtmlHelper helper)
        {
            return (ResultadoModel)helper.ViewBag.Resultado;
        }

        public static List<FacetModel> GetFacetas(this IHtmlHelper helper)
        {
            return (List<FacetModel>)helper.ViewBag.Facetas;
        }

        public static List<FacetItemModel> GetFiltros(this IHtmlHelper helper)
        {
            return (List<FacetItemModel>)helper.ViewBag.Filtros;
        }

        public static SortedDictionary<string, List<CMSComponente>> GetListaComponentesItem(this IHtmlHelper helper)
        {
            return (SortedDictionary<string, List<CMSComponente>>)helper.ViewBag.ListaComponentesItem;
        }

        public static SortedDictionary<string, List<short>> GetListaPaginasItem(this IHtmlHelper helper)
        {
            return (SortedDictionary<string, List<short>>)helper.ViewBag.ListaPaginasItem;
        }

        public static string GetTituloPagina(this IHtmlHelper helper)
        {
            return (string)helper.ViewBag.TituloPagina;
        }

        public static string GetUrlPagina(this IHtmlHelper helper)
        {
            return (string)helper.ViewBag.UrlPagina;
        }

        public static string GetURLRSS(this IHtmlHelper helper)
        {
            return (string)helper.ViewBag.URLRSS;
        }

        public static string GetURLRDF(this IHtmlHelper helper)
        {
            return (string)helper.ViewBag.URLRDF;
        }

        public static List<KeyValuePair<string, string>> GetListaMetas(this IHtmlHelper helper)
        {
            if (helper.ViewBag.ListaMetas != null)
            {
                return (List<KeyValuePair<string, string>>)helper.ViewBag.ListaMetas;
            }
            return new List<KeyValuePair<string, string>>();
        }

        public static List<string> GetListaJS(this IHtmlHelper helper)
        {
            if (helper.ViewBag.ListaJS != null)
            {
                return (List<string>)helper.ViewBag.ListaJS;
            }
            return new List<string>();
        }

        public static string GetJSGraficos(this IHtmlHelper helper)
        {
            return (string)helper.ViewBag.JSGraficos;
        }

        public static string GetJSMapa(this IHtmlHelper helper)
        {
            return (string)helper.ViewBag.JSMapa;
        }

        public static UtilIdiomas GetUtilIdiomas(this IHtmlHelper helper)
        {
            return (UtilIdiomas)helper.ViewBag.UtilIdiomas;
        }

        public static string GetUrlLoadResourceActions(this IHtmlHelper helper)
        {
            return (string)helper.ViewBag.UrlLoadResourceActions;
        }

        public static string GetBaseUrl(this IHtmlHelper helper)
        {
            return (string)helper.ViewBag.BaseUrl;
        }

        public static string GetBaseUrlStatic(this IHtmlHelper helper)
        {
            return (string)helper.ViewBag.BaseUrlStatic;
        }

        public static string GetMetaEtiquetasXMLOntologias(this IHtmlHelper helper)
        {
            return (string)helper.ViewBag.MetaEtiquetasXMLOntologias;
        }

        public static string GetBaseUrlContent(this IHtmlHelper helper)
        {
            return (string)helper.ViewBag.BaseUrlContent;
        }

        public static GnossUrlsSemanticas GetGeneradorURLs(this IHtmlHelper helper)
        {
            return (GnossUrlsSemanticas)helper.ViewBag.GeneradorURLs;
        }

        public static Guid? GetProyectoConexion(this IHtmlHelper helper)
        {
            return (Guid?)helper.ViewBag.ProyectoConexion;
        }

        public static LoggingService GetLoggingService(this IHtmlHelper helper)
        {
            return (LoggingService)helper.ViewBag.LoggingService;
        }

        public static string GetTokenLoginUsuario(this IHtmlHelper helper)
        {
            return (string)helper.ViewBag.TokenLoginUsuario;
        }

        public static string GetUrlServicioLogin(this IHtmlHelper helper)
        {
            return (string)helper.ViewBag.UrlServicioLogin;
        }

        //TODO Juan 
        public static string GetStyleVersion(this IHtmlHelper helper/*, IHttpContextAccessor httpContextAccessor*/)
        {
            return string.Empty;

            //string styleVersion = string.Empty;

            //if (httpContextAccessor.HttpContext.Session.Keys.Contains("VersionEstilos"))
            //{
            //    styleVersion = string.Format("/versiones/{0}", httpContextAccessor.HttpContext.Session.Get("VersionEstilos"));
            //}

            //return styleVersion;
        }

        public static string GetBaseUrlPersonalizacionEcosistema(this IHtmlHelper helper)
        {
            return (string)helper.ViewBag.BaseUrlPersonalizacionEcosistema;
        }

        public static string GetBaseUrlPersonalizacion(this IHtmlHelper helper)
        {
            return (string)helper.ViewBag.BaseUrlPersonalizacion;
        }

        public static bool GetEsEcosistemaSinMetaProyecto(this IHtmlHelper helper)
        {
            if (helper.ViewBag.EsEcosistemaSinMetaProyecto != null)
            {
                return (bool)helper.ViewBag.EsEcosistemaSinMetaProyecto;
            }

            return false;
        }

        public static string GetBaseUrlMetaProyecto(this IHtmlHelper helper)
        {
            return (string)helper.ViewBag.BaseUrlMetaProyecto;
        }

        public static UserIdentityModel GetIdentidadActual(this IHtmlHelper helper)
        {
            return (UserIdentityModel)helper.ViewBag.IdentidadActual;
        }

        public static string GetMainCommunityUrl(this IHtmlHelper helper)
        {
            return (string)helper.ViewBag.UrlComunidadPrincipal;
        }

        public static List<CommunityModel.TabModel> GetMainCommunityTabs(this IHtmlHelper helper)
        {
            return (List<CommunityModel.TabModel>)helper.ViewBag.MainCommunityTabs;
        }

        public static string GetNombreProyectoEcosistema(this IHtmlHelper helper)
        {
            return (string)helper.ViewBag.NombreProyectoEcosistema;
        }

        public static string GetNombreEspacioPersonal(this IHtmlHelper helper)
        {
            return (string)helper.ViewBag.NombreEspacioPersonal;
        }

        public static HeaderModel GetCabecera(this IHtmlHelper helper)
        {
            return (HeaderModel)helper.ViewBag.Cabecera;
        }

        public static string GetUrlCanonical(this IHtmlHelper helper)
        {
            return (string)helper.ViewBag.UrlCanonical;
        }

        public static string GetControllerName(this IHtmlHelper helper)
        {
            return (string)helper.ViewBag.ControllerName;
        }

        public static string GetJSExtra(this IHtmlHelper helper)
        {
            return (string)helper.ViewBag.JSExtra;
        }

        public static List<Dictionary<string, string>> GetListaMetasComplejas(this IHtmlHelper helper)
        {
            if (helper.ViewBag.ListaMetasComplejas != null)
            {
                return (List<Dictionary<string, string>>)helper.ViewBag.ListaMetasComplejas;
            }
            return new List<Dictionary<string, string>>();
        }

        public static string GetVersion(this IHtmlHelper helper)
        {
            return (string)helper.ViewBag.Version;
        }

        public static bool? GetJSYCSSunificado(this IHtmlHelper helper)
        {
            if (helper.ViewBag.JSYCSSunificado != null)
            {
                return (bool)helper.ViewBag.JSYCSSunificado;
            }
            return null;
        }

        public static string GetGoogleAnalytics(this IHtmlHelper helper)
        {
            return (string)helper.ViewBag.GoogleAnalytics;
        }

        public static string GetGoogleAnalyticsCode(this IHtmlHelper helper)
        {
            return (string)helper.ViewBag.GoogleAnalyticsCode;
        }

        public static List<string> GetListaCSS(this IHtmlHelper helper)
        {
            if (helper.ViewBag.ListaCSS != null)
            {
                return (List<string>)helper.ViewBag.ListaCSS;
            }
            return new List<string>();
        }

        public static List<string> GetBusquedasXml(this IHtmlHelper helper)
        {
            if (helper.ViewBag.BusquedasXml != null)
            {
                return (List<string>)helper.ViewBag.BusquedasXml;
            }
            return new List<string>();
        }

        public static List<KeyValuePair<string, string>> GetListaInputHidden(this IHtmlHelper helper)
        {
            if (helper.ViewBag.ListaInputHidden != null)
            {
                return (List<KeyValuePair<string, string>>)helper.ViewBag.ListaInputHidden;
            }
            return new List<KeyValuePair<string, string>>();

        }

        public static string GetUrlActionLogin(this IHtmlHelper helper)
        {
            return (string)helper.ViewBag.UrlActionLogin;
        }
        
        public static string GetUrlActionTwoFactorAuthentication(this IHtmlHelper helper)
        {
            return (string)helper.ViewBag.UrlActionTwoFactorAuthentication;
        }

        public static int GetEdadMinimaRegistro(this IHtmlHelper helper)
        {
            if (helper.ViewBag.EdadMinimaRegistro != null)
            {
                return (int)helper.ViewBag.EdadMinimaRegistro;
            }
            return 0;
        }

        public static bool ComprobarPersonaEsMayorAnios(this IHtmlHelper helper, DateTime fecha, int edad)
        {
            DateTime hoy = DateTime.Now;
            int anios = hoy.Year - fecha.Year;
            int meses = hoy.Month - fecha.Month;

            if (meses < 0 || (meses == 0 && hoy.Day < fecha.Day))
            {
                anios--;
            }

            bool esMayor = (anios >= edad);

            return esMayor;
        }

        public static Guid GetIdentidadMensajeID(this IHtmlHelper helper)
        {
            return (Guid)helper.ViewBag.IdentidadMensajeID;
        }

        public static string GetNombreUrlPestanya(this IHtmlHelper helper)
        {
            return (string)helper.ViewBag.NombreUrlPestanya;
        }

        public static List<ProfileModel> GetPerfilesAceptarinvitacion(this IHtmlHelper helper)
        {
            if (helper.ViewBag.PerfilesAceptarinvitacion != null)
            {
                return (List<ProfileModel>)helper.ViewBag.PerfilesAceptarinvitacion;
            }
            return new List<ProfileModel>();
        }

        public static bool? GetSoloIdentidadPersonal(this IHtmlHelper helper)
        {
            if (helper.ViewBag.SoloIdentidadPersonal != null)
            {
                return (bool)helper.ViewBag.SoloIdentidadPersonal;
            }
            return null;
        }

        public static string GetUrlOrigen(this IHtmlHelper helper)
        {
            return (string)helper.ViewBag.UrlOrigen;
        }

        public static string GetDownloadUrlFile(this IHtmlHelper helper, string pFileName, Guid resourceID, Guid pOntologyID, IHttpContextAccessor httpContextAccessor)
        {
            // URL seems like: 
            //https://www.gnoss.com/download-file?doc=74694007-7d13-1256-98b2-b41bc2bf8947&ext=.pdf&archivoAdjuntoSem=test_ebca7c22-8eab-3abe-699a-f6abfe2671f3&ontologiaAdjuntoSem=98f11bc6-f4f5-415d-aa8b-a4284fedeab6&ID=90407afa-9ace-4262-b1c0-15e257f07528&proy=f4506454-e16f-422a-981c-db6d3def37a8

            System.IO.FileInfo fileInfo = new System.IO.FileInfo(pFileName);
            string fileWithoutExtension = pFileName.Substring(0, pFileName.Length - fileInfo.Extension.Length);
            Guid identidadActual = helper.GetIdentidadActual().KeyIdentity;
            Guid proyID = helper.GetComunidad().Key;

            string url = $"//{httpContextAccessor.HttpContext.Request.Host}/download-file?doc={resourceID}&ext={fileInfo.Extension}&archivoAdjuntoSem={HttpUtility.UrlEncode(fileWithoutExtension)}&ontologiaAdjuntoSem={pOntologyID}&ID={identidadActual}&proy={proyID}";

            return url;
        }

        public static string GetTitle(this IHtmlHelper helper)
        {
            return (string)helper.ViewBag.Title;
        }

        public static string GetGrafoID(this IHtmlHelper helper)
        {
            return (string)helper.ViewBag.GrafoID;
        }

        public static string GetParametros(this IHtmlHelper helper)
        {
            return (string)helper.ViewBag.Parametros;
        }

        public static string GetFiltroContextoWhere(this IHtmlHelper helper)
        {
            return (string)helper.ViewBag.FiltroContextoWhere;
        }

        public static bool? GetPintarH1(this IHtmlHelper helper)
        {
            if (helper.ViewBag.PintarH1 != null)
            {
                return (bool)helper.ViewBag.PintarH1;
            }
            return null;

        }

        public static bool? GetOcultarMenusComunidad(this IHtmlHelper helper)
        {
            if (helper.ViewBag.OcultarMenusComunidad != null)
            {
                return (bool)helper.ViewBag.OcultarMenusComunidad;
            }
            return null;
        }

        public static string GetFavicon(this IHtmlHelper helper)
        {
            return (string)helper.ViewBag.Favicon;
        }

        public static string GetNombrePestanya(this IHtmlHelper helper)
        {
            return (string)helper.ViewBag.NombrePestanya;
        }

        public static List<CategoryCookieModel> GetCookiesAndCategories(this HtmlHelper helper)
        {
            return (List<CategoryCookieModel>)helper.ViewBag.ListCategoryCookieModel;
        }

        public static CommunityModel.TabModel BuscarPestanyaPorID(this IHtmlHelper helper, Guid pPestanyaID, List<CommunityModel.TabModel> listaPestanyas)
        {
            CommunityModel.TabModel pestanya = listaPestanyas.FirstOrDefault(tab => tab.Key.Equals(pPestanyaID));
            if (pestanya == null)
            {
                foreach (CommunityModel.TabModel tab in listaPestanyas.Where(t => t.SubTab != null && t.SubTab.Count > 0))
                {
                    pestanya = BuscarPestanyaPorID(helper, pPestanyaID, tab.SubTab);
                    if (pestanya != null) { break; }
                }
            }
            return pestanya;
        }

        #endregion

        #region ViewBag Setters

        public static void AddBodyClass(this IHtmlHelper helper, string pBodyClass)
        {
            helper.ViewBag.BodyClass = $"{helper.ViewBag.BodyClass} {pBodyClass}";
        }

        public static void SetBodyClass(this IHtmlHelper helper, string pBodyClass)
        {
            helper.ViewBag.BodyClass = pBodyClass;
        }

        public static void SetBodyClassPestanya(this IHtmlHelper helper, string pBodyClassPestanya)
        {
            helper.ViewBag.BodyClassPestanya = pBodyClassPestanya;
        }

        public static void SetTitle(this IHtmlHelper helper, string pTitle)
        {
            helper.ViewBag.Title = pTitle;
        }

        public static void SetPintarH1(this IHtmlHelper helper, Boolean pPintarH1)
        {
            helper.ViewBag.PintarH1 = pPintarH1;
        }

        public static void SetOcultarMenusComunidad(this IHtmlHelper helper, Boolean pOcultarMenusComunidad)
        {
            helper.ViewBag.OcultarMenusComunidad = pOcultarMenusComunidad;
        }
        #endregion
    }
}