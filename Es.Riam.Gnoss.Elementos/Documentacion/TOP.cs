using System.Collections.Generic;

namespace Es.Riam.Gnoss.Elementos.Documentacion
{
    public class JsonTOPApi
    {
        public int id { get; set; }
        public string idref { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string mail { get; set; }
        public string user_id { get; set; }
        public string process_info { get; set; }
        public string begin_of_process { get; set; }
        public string end_of_process { get; set; }
        public string base_ { get; set; }
        public bool islive { get; set; }
        public bool isdvr { get; set; }
        public int length { get; set; }
        public bool logo { get; set; }

        // ICM-25 - Al convertir el JSON no le gusta la propiedad tags. ,"tags":[],
        // public string tags { get; set; }
        public bool active { get; set; }
        public string url_thumbnail { get; set; }
        public string url_video_still { get; set; }
        public JsonTOPApiAccount account { get; set; }
        public JsonTOPApiProvider provider { get; set; }
        public JsonTOPApiStatus status { get; set; }
        public List<JsonTOPApiAsset> asset { get; set; }
        public bool sucess { get; set; }
        public string error { get; set; }
    }

    public class JsonTOPApiAccount
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }

    public class JsonTOPApiProvider
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class JsonTOPApiStatus
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class JsonTOPApiAsset
    {
        public int id { get; set; }
        public string src { get; set; }
        public int bitrate { get; set; }
        public string codec { get; set; }
        public string mimetype { get; set; }
        public string container { get; set; }
        public string tag { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int size { get; set; }
        public string created_at { get; set; }
        public List<JsonTOPApiURL> url { get; set; }
    }

    public class JsonTOPApiURL
    {
        public int id { get; set; }
        public JsonTOPApiTypeURL type_url { get; set; }
        public string url { get; set; }
    }

    public class JsonTOPApiTypeURL
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}
