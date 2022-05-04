using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    /// <summary>
    /// Modelo de datos de la consulta de la carga masiva
    /// </summary>
    public class ConsultaCargaMasivaViewModel
    {
        /// <summary>
        /// Nombre d ela carga masiva
        /// </summary>
        public List<string> NombreCargas { get; set; }
        /// <summary>
        /// Estado de la carga masiva
        /// </summary>
        public List<string> EstadoCargas { get; set; }
        /// <summary>
        /// Id de la carga a la que pertenece el paquete
        /// </summary>
        public List<Guid> IdPaqueteCargas { get; set; }
        /// <summary>
        /// Fechas de alta de los paquetes
        /// </summary>
        public List<DateTime> FechasAlta { get; set; }
        /// <summary>
        /// Diccionario con el estado del paquete y su posible error
        /// </summary>
        public Dictionary<string, string> EstadosPaquetes { get; set; }

        public List<InformacionMostrar> ListaInformacion { get; set; }
    }
    public class InformacionMostrar
    {
        public string NombreCarga { get; set; }
        public string FechaCarga { get; set; }
        public string EstadoCarga { get; set; }
        public int NumPaquetes { get; set; }
        public int NumParquetesProcesados { get; set; }
        public int NumPaquetesConError { get; set; }
        public string ErrorCarga { get; set; }
        public string IDCarga { get; set; }
    }
}
