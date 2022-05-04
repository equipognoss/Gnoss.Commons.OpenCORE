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
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Email { get; set; }
        public string Pass { get; set; }
        public List<JsonDatosExtraUsuario> DatosExtra { get; set; }
        public List<JsonEventoUsuario> EventosUsuario { get; set; }
        public string DatoAux { get; set; }
        public Guid ProyectoID { get; set; }
        public string NombreCortoComunidad { get; set; }
        public string NombreCortoUsuario { get; set; }
        public Guid UsuarioID { get; set; }
        public string DNI { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public Guid PaisID { get; set; }
        public string Pais { get; set; }
        public Guid ProvinciaID { get; set; }
        public string Provincia { get; set; }
        public string Localidad { get; set; }
        public string Direccion { get; set; }
        public string CodigoPostal { get; set; }
        public DateTime FechaRegistroComunidad { get; set; }
        public string Sexo { get; set; }
        public string TipoAccion { get; set; }
        public Dictionary<Guid, string> Preferencias { get; set; }
        public List<JsonPreferenciaJerarquica> ListaPreferenciasJerarquicas { get; set; }
        public JsonEstado EstadoAccion { get; set; }
        public bool RecibirNewsletter { get; set; }
        public string Idioma { get; set; }
    }

    public class JsonDatosExtraUsuario
    {
        public Guid NombreID { get; set; }
        public string Nombre { get; set; }
        public Guid ValorID { get; set; }
        public string Valor { get; set; }
    }

    public class JsonEventoUsuario
    {
        public Guid IdentificadorEvento { get; set; }
        public string NombreEvento { get; set; }
        public DateTime Fecha { get; set; }
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
