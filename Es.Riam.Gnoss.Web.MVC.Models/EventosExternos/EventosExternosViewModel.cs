using Newtonsoft.Json;

namespace Es.Riam.Gnoss.Web.MVC.Models.EventosExternos
{
    public class EventosExternosViewModel
    {
        public bool ConfigProyecto {  get; set; } = false;
        public string Usuario {  get; set; } = string.Empty;
        public string Password {  get; set; } = string.Empty ;
        public bool PublicarRecursos { get; set; } = false;
        public bool PublicarComentarios { get; set; } = false;
        public bool PublicarUsuarios { get; set; } = false;
        public string ConnectionString
        {
            get
            {
                return mConnectionString;
            }
        }
        private string mConnectionString {  get; set; }

        /// <summary>
        /// Carga la cadena de conexion de RabbitMQ
        /// </summary>
        /// <param name="conexionRabbit"></param>
        public void CargarConnectionString(string conexionRabbit)
        {
            string vHost = $"{conexionRabbit.Substring(conexionRabbit.LastIndexOf("/") + 1)}_public";

            var longit = conexionRabbit.LastIndexOf("/") - conexionRabbit.LastIndexOf(":") - 1;
            string puerto = conexionRabbit.Substring(conexionRabbit.LastIndexOf(":") + 1, longit);

            var longit2 = conexionRabbit.LastIndexOf(":") - conexionRabbit.LastIndexOf("@") - 1;
            string ip = conexionRabbit.Substring(conexionRabbit.LastIndexOf("@") + 1, longit2);

            mConnectionString = $"amqp://{Usuario}:{Password}@{ip}:{puerto}/{vHost}";
        }
        
    }
}
