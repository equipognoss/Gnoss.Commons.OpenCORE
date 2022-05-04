using Es.Riam.Gnoss.AD.EntityModel.Models.OrganizacionDS;
using Es.Riam.Gnoss.Util.General;
using System;

namespace Es.Riam.Gnoss.Elementos.ServiciosGenerales
{
    /// <summary>
    /// Datos de la vinculación de una persona a una organización
    /// </summary>
    public class DatosTrabajoPersonaOrganizacion : ElementoGnoss,IDisposable
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pVinculo">Fila de vínculo entre persona y organización</param>
        /// <param name="pGestorOrganizaciones">Gestor de organizaciones</param>
        public DatosTrabajoPersonaOrganizacion(PersonaVinculoOrganizacion pVinculo, GestionOrganizaciones pGestorOrganizaciones, LoggingService loggingService) 
            : base(pVinculo, pGestorOrganizaciones, loggingService)
        {
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Devuelve la persona asociada al perfil
        /// </summary>
        public Persona Persona
        {
            get
            {
                if (GestorPersonas != null && GestorPersonas.ListaPersonas.ContainsKey(FilaVinculo.PersonaID))
                    return GestorPersonas.ListaPersonas[FilaVinculo.PersonaID];
                return null;
            }
        }

        /// <summary>
        /// Devuelve la organización asociada al perfil
        /// </summary>
        public Organizacion Organizacion
        {
            get
            {
                if (GestorOrganizaciones != null && GestorOrganizaciones.ListaOrganizaciones.ContainsKey(FilaVinculo.OrganizacionID))
                    return GestorOrganizaciones.ListaOrganizaciones[FilaVinculo.OrganizacionID];
                return null;
            }
        }

        /// <summary>
        /// Devuelve el gestor de organizaciones
        /// </summary>
        public GestionOrganizaciones GestorOrganizaciones
        {
            get
            {
                return (GestionOrganizaciones)GestorGnoss;
            }
        }

        /// <summary>
        /// Devuelve el gestor de personas
        /// </summary>
        public GestionPersonas GestorPersonas
        {
            get
            {
                if (GestorOrganizaciones == null)
                    return null;
                return GestorOrganizaciones.GestorPersonas;
            }
        }

        /// <summary>
        /// Devuelve la fila del vínculo entre persona y organización
        /// </summary>
        public PersonaVinculoOrganizacion FilaVinculo
        {
            get
            {
                return (PersonaVinculoOrganizacion)FilaElementoEntity;
            }
        }

        #region Campos de la tabla

        /// <summary>
        /// Devuelve o establece el identificador de pais
        /// </summary>
        public Guid PaisID
        {
            get
            {
                if (!FilaVinculo.PaisTrabajoID.HasValue)
                    return Guid.Empty;
                return FilaVinculo.PaisTrabajoID.Value;
            }
            set
            {
                if (value.Equals(Guid.Empty) && FilaVinculo.PaisTrabajoID.HasValue)
                    FilaVinculo.PaisTrabajoID = null;
                else if (!FilaVinculo.PaisTrabajoID.HasValue && value != Guid.Empty)
                    FilaVinculo.PaisTrabajoID = value;
                else if (FilaVinculo.PaisTrabajoID.HasValue && FilaVinculo.PaisTrabajoID.HasValue)
                    FilaVinculo.PaisTrabajoID = value;
            }
        }

        /// <summary>
        /// Devuelve o establece el identificador de la provincia
        /// </summary>
        public Guid ProvinciaID
        {
            get
            {
                if (!FilaVinculo.ProvinciaTrabajoID.HasValue)
                    return Guid.Empty;
                return FilaVinculo.ProvinciaTrabajoID.Value;
            }
            set
            {
                if (value.Equals(Guid.Empty) && FilaVinculo.ProvinciaTrabajoID.HasValue)
                    FilaVinculo.ProvinciaTrabajoID = null;
                else if (!FilaVinculo.ProvinciaTrabajoID.HasValue && value != Guid.Empty)
                    FilaVinculo.ProvinciaTrabajoID = value;
                else if (FilaVinculo.ProvinciaTrabajoID.HasValue && !FilaVinculo.ProvinciaTrabajoID.Value.Equals(value))
                    FilaVinculo.ProvinciaTrabajoID = value;
            }
        }

        /// <summary>
        /// Devuelve o establece la provincia
        /// </summary>
        public string Provincia
        {
            get
            {
                return FilaVinculo.ProvinciaTrabajo;
            }
            set
            {
                if (!FilaVinculo.ProvinciaTrabajo.Equals(value) && value.Equals(string.Empty))
                    FilaVinculo.ProvinciaTrabajoID = null;
                else if (!FilaVinculo.ProvinciaTrabajo.Equals(value))
                    FilaVinculo.ProvinciaTrabajo = value;
            }
        }

        /// <summary>
        /// Devuelve o establece la localidad
        /// </summary>
        public string Localidad
        {
            get
            {
                return FilaVinculo.LocalidadTrabajo;
            }
            set
            {
                if (!FilaVinculo.LocalidadTrabajo.Equals(value) && value.Equals(string.Empty))
                    FilaVinculo.LocalidadTrabajo = null;
                else if (!FilaVinculo.LocalidadTrabajo.Equals(value))
                    FilaVinculo.LocalidadTrabajo = value;
            }
        }

        /// <summary>
        /// Devuelve o establece el codigo postal
        /// </summary>
        public string CodPostal
        {
            get
            {
                return FilaVinculo.CPTrabajo;
            }
            set
            {
                if (!FilaVinculo.CPTrabajo.Equals(value) && value.Equals(string.Empty))
                    FilaVinculo.CPTrabajo = null;
                else if (!FilaVinculo.CPTrabajo.Equals(value))
                    FilaVinculo.CPTrabajo = value;
            }
        }

        /// <summary>
        /// Devuelve o establece la dirección
        /// </summary>
        public string Direccion
        {
            get
            {
                return FilaVinculo.DireccionTrabajo;
            }
            set
            {
                if (!FilaVinculo.DireccionTrabajo.Equals(value) && value.Equals(string.Empty))
                    FilaVinculo.DireccionTrabajo = null;
                else if (!FilaVinculo.DireccionTrabajo.Equals(value))
                    FilaVinculo.DireccionTrabajo = value;
            }
        }

        /// <summary>
        /// Devuelve o establece el teléfono fijo
        /// </summary>
        public string Telefono
        {
            get
            {
                return FilaVinculo.TelefonoTrabajo;
            }
            set
            {
                if (!FilaVinculo.TelefonoTrabajo.Equals(value) && value.Equals(string.Empty))
                    FilaVinculo.TelefonoTrabajo = null;
                else if (!FilaVinculo.TelefonoTrabajo.Equals(value))
                    FilaVinculo.TelefonoTrabajo = value;
            }
        }

        /// <summary>
        /// Devuelve o establece la extensión
        /// </summary>
        public string Extension
        {
            get
            {
                return FilaVinculo.Extension;
            }
            set
            {
                if (!FilaVinculo.Extension.Equals(value) && value.Equals(string.Empty))
                    FilaVinculo.Extension = null;
                else if (!FilaVinculo.Extension.Equals(value))
                    FilaVinculo.Extension = value;
            }
        }

        /// <summary>
        /// Devuelve o establece el teléfono móvil
        /// </summary>
        public string Movil
        {
            get
            {
                return FilaVinculo.TelefonoMovilTrabajo;
            }
            set
            {
                if (!FilaVinculo.TelefonoMovilTrabajo.Equals(value) && value.Equals(string.Empty))
                    FilaVinculo.TelefonoMovilTrabajo = null;
                else if (!FilaVinculo.TelefonoMovilTrabajo.Equals(value))
                    FilaVinculo.TelefonoMovilTrabajo = value;
            }
        }

        /// <summary>
        /// Devuelve o establece el mail
        /// </summary>
        public string Mail
        {
            get
            {
                return FilaVinculo.EmailTrabajo;
            }
            set
            {
                if (!FilaVinculo.EmailTrabajo.Equals(value) && value.Equals(string.Empty))
                    FilaVinculo.EmailTrabajo = null;
                else if (!FilaVinculo.EmailTrabajo.Equals(value))
                    FilaVinculo.EmailTrabajo = value;
            }
        }

        /// <summary>
        /// Devuelve o establece el identificador de la categoria profesional
        /// </summary>
        public Guid CategoriaProfesionalID
        {
            get
            {
                if (!FilaVinculo.CategoriaProfesionalID.HasValue)
                    return Guid.Empty;
                return FilaVinculo.CategoriaProfesionalID.Value;
            }
            set
            {
                if (value.Equals(Guid.Empty) && FilaVinculo.CategoriaProfesionalID.HasValue)
                    FilaVinculo.CategoriaProfesionalID = null;
                else if (!FilaVinculo.CategoriaProfesionalID.HasValue && value != Guid.Empty)
                    FilaVinculo.CategoriaProfesionalID = value;
                else if (FilaVinculo.CategoriaProfesionalID.HasValue && !FilaVinculo.CategoriaProfesionalID.Equals(value))
                    FilaVinculo.CategoriaProfesionalID = value;
            }
        }

        /// <summary>
        /// Devuelve o establece el identificador del tipo de contrato
        /// </summary>
        public Guid TipoContratoID
        {
            get
            {
                if (!FilaVinculo.TipoContratoID.HasValue)
                    return Guid.Empty;
                return FilaVinculo.TipoContratoID.Value;
            }
            set
            {
                if (value.Equals(Guid.Empty) && FilaVinculo.TipoContratoID.HasValue)
                    FilaVinculo.TipoContratoID = null;
                else if (!FilaVinculo.TipoContratoID.HasValue && value != Guid.Empty)
                    FilaVinculo.TipoContratoID = value;
                else if (FilaVinculo.TipoContratoID.HasValue && !FilaVinculo.TipoContratoID.Equals(value))
                    FilaVinculo.TipoContratoID = value;
            }
        }

        /// <summary>
        /// Devuelve o establece la fecha de vinculación
        /// </summary>
        public override DateTime Fecha
        {
            get
            {
                return FilaVinculo.FechaVinculacion;
            }
            set
            {
                if (!FilaVinculo.FechaVinculacion.Equals(value))
                    FilaVinculo.FechaVinculacion = value;
            }
        }

        /// <summary>
        /// Devuelve o establece el cargo del perfil
        /// </summary>
        public string Cargo
        {
            get
            {
                return FilaVinculo.Cargo;
            }
            set
            {
                if (!FilaVinculo.Cargo.Equals(value) && value.Equals(string.Empty))
                    FilaVinculo.Cargo = null;
                else if (!FilaVinculo.Cargo.Equals(value))
                    FilaVinculo.Cargo = value;
            }
        }

        #endregion

        #endregion

        #region Dispose

        /// <summary>
        /// Determina si está disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~DatosTrabajoPersonaOrganizacion()
        {
            //Libero los recursos
            Dispose(false);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        /// <param name="disposing">Determina si se está llamando desde el Dispose()</param>
        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                this.disposed = true;
                try
                {
                    if (disposing)
                    {

                    }
                }
                finally
                {
                    // Llamo al dispose de la clase base
                    base.Dispose(disposing);
                }
            }
        }

        #endregion
    }
}
