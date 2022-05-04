using Es.Riam.Gnoss.Web.MVC.Models.IntegracionContinua;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public class AdministrarDesplieguesViewModel
    {
        public string VersionActualEntornoActual { get; set; }
        public string NombreEntornoSuperior { get; set; }
        public Dictionary<DateTime, string> VersionesEntornoActual { get; set; }
        public List<string> VersionesOtrosEntorno { get; set; }
        public bool EsDesplegable { get; set; }
        public string Error { get; set; }
        public bool PeticionCreacion { get; set; }
        public bool PeticionDespliegue { get; set; }
        //MODIFICACIONIC: Creación un parametro para que el usuario despliegue desde pruebas, y no permitirle desplegar en pro.
        public bool DesplegarDesdePruebas { get; set; }
        //MODIFICACIONIC: Creación un parametro para que se sepa si hay cambios pendientes y estas en un entorno que no es el de pruebas.
        public bool HayCambiosPendientes { get; set; }
        //MODIFICACIONIC: Creación un parametro para que se sepa que la version es hotfix y saber si se ha desplegado en pre.
        public bool VersionHotfixCreadaPre { get; set; }
        //MODIFICACIONIC: Creación un parametro para que se estabilizar la version si hay cambios en pro
        public bool DebeEstabilizarVersion { get; set; }
        //MODIFICACIONIC: Creación un parametro para comprobar si el entorno de Produccion está configurado En el caso que se compruebe desde pre, en el resto de entornos será true por defecto
        public bool IsEntornoMasterConfigurado { get; set; }
        //MODIFICACIONIC: Creación un parametro para comprobar si es hotfix la version
        public bool EsVersionHotfix { get; set; }
        //MODIFICACIONIC: Tener una variable con los cambios que hay que fusionar.
        public List<ChangesContinuosIntegration> Cambios { get; set; }
        public bool PermitirFusionPRO { get; set; }
        public AdministrarDesplieguesViewModel()
        {
            VersionesEntornoActual = new Dictionary<DateTime, string>();
            VersionesOtrosEntorno = new List<string>();
            Cambios = new List<ChangesContinuosIntegration>();
        }
    }
}
