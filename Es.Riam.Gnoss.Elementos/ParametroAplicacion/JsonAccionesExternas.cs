using Es.Riam.Gnoss.Web.MVC.Models;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Elementos.ParametroAplicacion
{
    public class JsonEstado
    {
        public bool Correcto { get; set; }
        public string InfoExtra { get; set; }
    }

    public class JsonUsuario
    {
        public string name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public List<JsonDatosExtraUsuario> extra_data { get; set; }
        public List<JsonEventoUsuario> user_events { get; set; }
        public string aux_data { get; set; }
        public Guid community_id { get; set; }
        public string community_short_name { get; set; }
        public string user_short_name { get; set; }
        public Guid user_id { get; set; }
        public string id_card { get; set; }
        public DateTime born_date { get; set; }
        public Guid country_id { get; set; }
        public string country { get; set; }
        public Guid province_id { get; set; }
        public string provice { get; set; }
        public string city { get; set; }
        public string address { get; set; }
        public string postal_code { get; set; }
        public DateTime join_community_date { get; set; }
        public string sex { get; set; }      
        public Dictionary<Guid, string> preferences { get; set; }
        public List<JsonPreferenciaJerarquica> ListaPreferenciasJerarquicas { get; set; }        
        public bool receive_newsletter { get; set; }
        public string languaje { get; set; }
    }

    public class JsonDatosExtraUsuario
    {
        public Guid name_id { get; set; }
        public string name { get; set; }
        public Guid value_id { get; set; }
        public string value { get; set; }
    }

    public class JsonEventoUsuario
    {
        public Guid event_id { get; set; }
        public string name { get; set; }
        public DateTime Date { get; set; }
    }

    public class JsonPreferenciaJerarquica
    {
        public Guid CategoriaID { get; set; }
        public string Nombre { get; set; }
        public JsonPreferenciaJerarquica CategoriaPadre { get; set; }
    }

    public class JsonDocumento
    {
        public Guid ProyectoID { get; set; }
        public Guid DocumentoID { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public TiposDocumentacion TipoDocumento { get; set; }
    }
}
