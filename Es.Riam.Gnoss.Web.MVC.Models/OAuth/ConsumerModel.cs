using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.Web.MVC.Models.OAuth
{
    /// <summary>
    /// Modelo de un consumer para una aplicación
    /// </summary>
    [Serializable]
    [NotMapped]
    public class ConsumerModel
    {
        public ConsumerModel()
        {

        }

        /// <summary>
        /// Constructor
        /// <param name="pName">Nombre de la aplicación</param>
        /// <param name="pDescription">Descripción de la aplicación</param>
        /// </summary>
        public ConsumerModel(int pConsumerId, string pName, string pDescription, string pKey, string pSecret, string pCallback)
        {
            ConsumerId = pConsumerId;
            Name = pName;
            Description = pDescription;
            Key = pKey;
            Secret = pSecret;
            AplicationCallback = pCallback;
        }
        /// <summary>
        /// ConsumerId de la aplicación
        /// </summary>
        public int ConsumerId { get; set; }
        /// <summary>
        /// Nombre de la aplicación
        /// </summary>
        [StringLength(2000, ErrorMessage = "Máximo de caracteres excedido."), Required(ErrorMessage = "El nombre de la aplicación es obligatorio.")]
        public string Name { get; set; }
        /// <summary>
        /// Descrpción de la aplicación
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// ConsumerKey de la aplicación
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// ConsumerSecret de la aplicación
        /// </summary>
        public string Secret { get; set; }
        /// <summary>
        /// Descrpción de la aplicación
        /// </summary>
        [StringLength(4000, ErrorMessage = "Máximo de caracteres excedido.")]
        public string AplicationCallback { get; set; }
        /// <summary>
        /// Token para la aplicación
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// Secret Token para la aplicación
        /// </summary>
        public string SecretToken { get; set; }
    }
}