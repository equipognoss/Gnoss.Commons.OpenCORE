using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Es.Riam.Util
{
    public class UtilSherlock
    {
        private readonly UtilWeb _utilWeb;
        public UtilSherlock(UtilWeb utilWeb)
        {
            _utilWeb = utilWeb;
        }
        public static string UriNerSherlock { get; } = "http://sherlocknerdproduccion.gnoss.com/api/annotator/PostAnnotation";

        public AnnotationResult AnnotationSherlock(string pTitle, string pDescription, float pCutOffMark, bool pShowAlternatives = false, bool pAugmentedText = false, List<string> pDisambiguationEnhancer = null, string pUri = null, string pClient = null, bool pTopics = false, bool pPreserveHTML = false)
        {
            pTitle = pTitle.Replace("&nbsp;", " ");
            pDescription = pDescription.Replace("&nbsp;", " ");
            pDescription = UtilCadenas.AcuteToAcento(pDescription);
            AnnotationResult respuesta = null;

            AnnotationQuery anotation = new AnnotationQuery();
            if (!string.IsNullOrEmpty(pUri))
            {
                anotation.uri = pUri;
            }
            else
            {
                anotation.title = pTitle;
                anotation.description = pDescription;
            }
            anotation.cutOffMark = pCutOffMark;
            anotation.showAlternatives = pShowAlternatives;
            anotation.augmentedText = pAugmentedText;
            anotation.disambiguationEnhancer = new List<string>();
            anotation.client = pClient;
            anotation.preserveHTML = pPreserveHTML;
            if (pDisambiguationEnhancer != null)
            {
                anotation.disambiguationEnhancer = pDisambiguationEnhancer;
            }
            anotation.topics = pTopics;
            List<SherlockEntityInfoModel> listSherlockTopicInfoModel = new List<SherlockEntityInfoModel>();
            List<SherlockEntityInfoModel> listSherlockEntityInfoModel = new List<SherlockEntityInfoModel>();
            respuesta = JsonConvert.DeserializeObject<AnnotationResult>(_utilWeb.WebRequest(UtilWeb.Metodo.POST, UriNerSherlock, JsonConvert.SerializeObject(anotation), "application/json"));
            respuesta.augmentedTextDescription = UtilCadenas.AcentoToAcute(HttpUtility.UrlDecode(respuesta.augmentedTextDescription));
            foreach (AnnotationResult.Result result in respuesta.entityList)
            {
                if (result.entityList.Count > 0)
                {
                    foreach (AnnotationResult.Result.Entity entity in result.entityList)
                    {
                        SherlockEntityInfoModel sherlockEntitycInfo = new SherlockEntityInfoModel();
                        sherlockEntitycInfo.Label = GetFirstLabelEntity(entity);
                        entity.score = result.relevance;

                        string wikidataUri = entity.uri;
                        string wikipediaUri = GetWikipediaLink(entity, "es");
                        sherlockEntitycInfo.DbpediaUri = GetDbPediaUriEntity(entity);
                        sherlockEntitycInfo.WikipediaUri = wikipediaUri;
                        listSherlockEntityInfoModel.Add(sherlockEntitycInfo);

                        respuesta.augmentedTextDescription = respuesta.augmentedTextDescription.Replace($"\"{wikidataUri}\"", $"\"{wikipediaUri}\"");
                        respuesta.augmentedTextTitle = respuesta.augmentedTextTitle.Replace($"\"{wikidataUri}\"", $"\"{wikipediaUri}\"");

                        wikidataUri = wikidataUri.Replace("http://wikidata.dbpedia.org/resource/", "http://wikidata.org/entity/");
                        wikidataUri = wikidataUri.Replace("https://wikidata.dbpedia.org/resource/", "http://wikidata.org/entity/");
                        entity.sameAs.Add(entity.uri);
                        entity.sameAs.Add(wikidataUri);
                        entity.uri = wikidataUri;
                    }
                }
            }

            foreach (AnnotationResult.Result topic in respuesta.topicList)
            {
                if (topic.entityList.Count > 0)
                {
                    foreach (AnnotationResult.Result.Entity entity in topic.entityList)
                    {
                        SherlockEntityInfoModel sherlockTopicInfo = new SherlockEntityInfoModel();
                        sherlockTopicInfo.Label = GetFirstLabelEntity(entity);
                        entity.score = topic.relevance;

                        string wikidataUri = entity.uri;
                        string wikipediaUri = GetWikipediaLink(entity, "es");
                        sherlockTopicInfo.DbpediaUri = GetDbPediaUriEntity(entity);
                        sherlockTopicInfo.WikipediaUri = wikipediaUri;
                        listSherlockTopicInfoModel.Add(sherlockTopicInfo);
                        respuesta.augmentedTextDescription = respuesta.augmentedTextDescription.Replace($"\"{wikidataUri}\"", $"\"{wikipediaUri}\"");
                        respuesta.augmentedTextTitle = respuesta.augmentedTextTitle.Replace($"\"{wikidataUri}\"", $"\"{wikipediaUri}\"");

                        wikidataUri = wikidataUri.Replace("http://wikidata.dbpedia.org/resource/", "http://wikidata.org/entity/");
                        wikidataUri = wikidataUri.Replace("https://wikidata.dbpedia.org/resource/", "http://wikidata.org/entity/");
                        entity.sameAs.Add(entity.uri);
                        entity.sameAs.Add(wikidataUri);
                        entity.uri = wikidataUri;
                    }
                }
            }
            foreach (AnnotationResult.Result topic in respuesta.topicList)
            {
                foreach (string text in topic.textList)
                {
                    string textAcute = UtilCadenas.AcentoToAcute(text);
                    string topicSpan = $"<span class=\"topic\">{textAcute}</span>";
                    string topicSpanNoAcute = $"<span class=\"topic\">{text}</span>";
                    int first = respuesta.augmentedTextDescription.IndexOf(topicSpan);
                    int last = 0;
                    while (first > -1)
                    {
                        last = respuesta.augmentedTextDescription.IndexOf("</span>", first);
                        last = last + "</span>".Length;
                        string textSpanReplace = respuesta.augmentedTextDescription.Substring(first, last - first);
                        respuesta.augmentedTextDescription = respuesta.augmentedTextDescription.Replace(textSpanReplace, textAcute);
                        first = respuesta.augmentedTextDescription.IndexOf(topicSpan);
                    }

                    first = respuesta.augmentedTextTitle.IndexOf(topicSpanNoAcute, StringComparison.CurrentCultureIgnoreCase);
                    last = 0;
                    while (first > -1)
                    {
                        last = respuesta.augmentedTextTitle.IndexOf("</span>", first, StringComparison.CurrentCultureIgnoreCase);
                        last = last + "</span>".Length;
                        string textSpanReplace = respuesta.augmentedTextTitle.Substring(first, last - first);
                        respuesta.augmentedTextTitle = respuesta.augmentedTextTitle.Replace(textSpanReplace, text);
                        first = respuesta.augmentedTextTitle.IndexOf(topicSpan);
                    }

                }
            }
            respuesta.entitiesInfo = JsonConvert.SerializeObject(listSherlockEntityInfoModel);
            respuesta.topicsInfo = JsonConvert.SerializeObject(listSherlockTopicInfoModel);
            return respuesta;
        }
        private static string GetFirstLabelEntity(AnnotationResult.Result.Entity entity)
        {
            var entityLabel = entity.labels.FirstOrDefault(x => x.language == "es");
            if (entityLabel == null)
            {
                entityLabel = entity.labels.FirstOrDefault(x => x.language == "en");
            }

            if (entityLabel != null)
            {
                return entityLabel.label.ToLower();
            }
            return null;
        }
        private static string GetDbPediaUriEntity(AnnotationResult.Result.Entity entity)
        {
            var entityUri = entity.sameAs.FirstOrDefault(x => x.StartsWith("http://dbpedia.org/resource/"));

            if (!string.IsNullOrEmpty(entityUri))
            {
                return entityUri;
            }
            return null;
        }
        private static string GetWikipediaLink(AnnotationResult.Result.Entity pEntity, string pLanguage)
        {
            string wikipediaUri = pEntity.wikipediaUri.FirstOrDefault(item => item.language.Equals(pLanguage))?.uri;

            if (string.IsNullOrEmpty(wikipediaUri) && !pLanguage.Equals("en"))
            {
                wikipediaUri = GetWikipediaLink(pEntity, "en");
            }
            else if (string.IsNullOrEmpty(wikipediaUri) && pEntity.wikipediaUri.Count > 0)
            {
                wikipediaUri = pEntity.wikipediaUri.First().uri;
            }

            return wikipediaUri;
        }
    }
    public class SherlockEntityInfoModel
    {
        public string WikipediaUri { get; set; }
        public string DbpediaUri { get; set; }
        public string Label { get; set; }
    }
    /// <summary>
    /// Resultado del NERD
    /// </summary>
    public class AnnotationResult
    {
        /// <summary>
        /// Idioma del texto
        /// </summary>
        public string lang { get; set; }
        /// <summary>
        /// Número de entidades
        /// </summary>
        public int entityNumber { get; set; }
        /// <summary>
        /// Número de topics
        /// </summary>
        public int topicNumber { get; set; }
        /// <summary>
        /// Titulo aumentado
        /// </summary>
        public string augmentedTextTitle { get; set; }
        /// <summary>
        /// Descripción aumentada
        /// </summary>
        public string augmentedTextDescription { get; set; }
        public string entitiesInfo { get; set; }
        public string topicsInfo { get; set; }
        /// <summary>
        /// Lista de entidades obtenidos
        /// </summary>
        public List<Result> entityList { get; set; }
        /// <summary>
        /// Lista de entidades obtenidos
        /// </summary>
        public List<Result> topicList { get; set; }
        /// <summary>
        /// Modelo de resultado
        /// </summary>
        public class Result
        {
            /// <summary>
            /// Textos por los que se ha identificado el resultado
            /// </summary>
            public List<string> textList { get; set; }
            /// <summary>
            /// Relevancia del resultado en el texto
            /// </summary>
            public float relevance { get; set; }
            public List<int> parrafos { get; set; }
            /// <summary>
            /// Número de apariciones en el texto
            /// </summary>
            public int count { get; set; }
            /// <summary>
            /// Listado de entidades obtenidas
            /// </summary>
            public List<Entity> entityList { get; set; }
            /// <summary>
            /// Modelo de entidad
            /// </summary>
            public class Entity
            {
                /// <summary>
                /// Identificador único
                /// </summary>
                public string uri { get; set; }
                /// <summary>
                /// Relevancia del resultado en el texto
                /// </summary>
                public float score { get; set; }
                /// <summary>
                /// Tipos de la entidad
                /// </summary>
                public HashSet<string> types { get; set; }
                /// <summary>
                /// Tipo de la entidad
                /// </summary>
                public string type { get; set; }
                /// <summary>
                /// Tipo de la entidad multiidioma
                /// </summary>
                public HashSet<Label> langType { get; set; }
                /// <summary>
                /// Subtipo
                /// </summary>
                public string subType { get; set; }
                /// <summary>
                /// Subtipo multiidioma
                /// </summary>
                public HashSet<Label> langSubType { get; set; }
                /// <summary>
                /// Nombre real de la entidad
                /// </summary>
                public HashSet<Label> labels { get; set; }
                /// <summary>
                /// Descripcion de la entidad
                /// </summary>
                public HashSet<Label> description { get; set; }
                /// <summary>
                /// Identificadores alternativos de la entidad
                /// </summary>
                public HashSet<string> sameAs { get; set; }
                /// <summary>
                /// Uris de dbpedia de la entidad
                /// </summary>
                public HashSet<WikipediaUri> wikipediaUri { get; set; }
                /// <summary>
                /// Modelo de label (nombre con idioma)
                /// </summary>
                public class Label
                {
                    /// <summary>
                    /// Idioma
                    /// </summary>
                    public string language { get; set; }
                    /// <summary>
                    /// Nombre
                    /// </summary>
                    public string label { get; set; }
                }
                /// <summary>
                /// Modelo de la uri de Wikipedia
                /// </summary>
                public class WikipediaUri
                {
                    /// <summary>
                    /// Idioma
                    /// </summary>
                    public string language { get; set; }
                    /// <summary>
                    /// Uri de dbpedia
                    /// </summary>
                    public string uri { get; set; }
                }
            }
        }
    }


    /// <summary>
    /// Modelo para la query de anotación
    /// </summary>
    public class AnnotationQuery
    {
        /// <summary>
        /// Título del texto a anotar
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// Descripción del texto a anotar
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// Uri del texto a anotar
        /// </summary>
        public string uri { get; set; }
        /// <summary>
        /// Nota de 'corte' para la desambiguación
        /// </summary>       
        public float cutOffMark { get; set; }
        /// <summary>
        /// Mostrar alternativas en caso de que existan
        /// </summary>
        public bool showAlternatives { get; set; }
        /// <summary>
        /// Obtener topics
        /// </summary>
        public bool topics { get; set; }
        /// <summary>
        /// Texto aumentado
        /// </summary>
        public bool augmentedText { get; set; }

        public bool preserveHTML { get; set; }
        /// <summary>
        /// Potenciadores de desambiguación
        /// </summary>
        public List<string> disambiguationEnhancer { get; set; }
        /// <summary>
        /// Cliente
        /// </summary>
        public string client { get; set; }
    }
}
