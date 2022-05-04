using Es.Riam.Gnoss.Util.General;
using System;

namespace Es.Riam.Gnoss.Elementos.ServiciosGenerales
{
    /// <summary>
    /// Organización empresarial
    /// </summary>
    public class OrganizacionEmpresa : Organizacion
    {
        #region Miembros

        private Es.Riam.Gnoss.AD.EntityModel.Models.OrganizacionDS.OrganizacionEmpresa mOrganizacionEmpresa;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public OrganizacionEmpresa(LoggingService loggingService) : base(loggingService)
        {
        }

        /// <summary>
        /// Constructor de la organización
        /// </summary>
        /// <param name="pOrganizacionEmpresa">Fila de organización empresa</param>
        /// <param name="pGestorOrganizaciones">Gestor de organizaciones</param>
        public OrganizacionEmpresa(Es.Riam.Gnoss.AD.EntityModel.Models.OrganizacionDS.Organizacion pOrganizacion, Es.Riam.Gnoss.AD.EntityModel.Models.OrganizacionDS.OrganizacionEmpresa pOrganizacionEmpresa, GestionOrganizaciones pGestorOrganizaciones, LoggingService loggingService)
            : base(pOrganizacion, pGestorOrganizaciones, loggingService)
        {
            mOrganizacionEmpresa = pOrganizacionEmpresa;
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene la fila de la organización empresa
        /// </summary>
        public Es.Riam.Gnoss.AD.EntityModel.Models.OrganizacionDS.OrganizacionEmpresa FilaOrganizacionEmpresa
        {
            get
            {
                return mOrganizacionEmpresa;
            }
        }

        /// <summary>
        /// Devuelve o establece CIF/NIF de la organización empresa
        /// </summary>
        public string CIF
        {
            get
            {
                return FilaOrganizacionEmpresa.CIF;
            }
            set
            {
                if (!FilaOrganizacionEmpresa.CIF.Equals(value) && string.IsNullOrEmpty(value))
                {
                    FilaOrganizacionEmpresa.CIF = null;
                }
                else if (!FilaOrganizacionEmpresa.CIF.Equals(value))
                {
                    FilaOrganizacionEmpresa.CIF = value;
                }
            }
        }

        /// <summary>
        /// Devuelve o establece la fecha de creación de la organización empresa
        /// </summary>
        public override DateTime Fecha
        {
            get
            {
                if (!FilaOrganizacionEmpresa.FechaCreacion.HasValue)
                {
                    return DateTime.MinValue;
                }
                return FilaOrganizacionEmpresa.FechaCreacion.Value;
            }
            set
            {
                if (value == DateTime.MinValue && FilaOrganizacionEmpresa.FechaCreacion.HasValue)
                {
                    FilaOrganizacionEmpresa.FechaCreacion = null;
                }
                else if (!FilaOrganizacionEmpresa.FechaCreacion.HasValue && value != DateTime.MinValue)
                {
                    FilaOrganizacionEmpresa.FechaCreacion = value;
                }
                else if (FilaOrganizacionEmpresa.FechaCreacion.HasValue && !FilaOrganizacionEmpresa.FechaCreacion.Value.Equals(value))
                {
                    FilaOrganizacionEmpresa.FechaCreacion = value;
                }
            }
        }

        /// <summary>
        /// Obtiene o establece el número de empleados de la organización empresa
        /// </summary>
        public int NumeroEmpleados
        {
            get
            {
                if (!FilaOrganizacionEmpresa.Empleados.HasValue)
                {
                    return int.MinValue;
                }
                return FilaOrganizacionEmpresa.Empleados.Value;
            }
            set
            {
                if (value == short.MinValue && FilaOrganizacionEmpresa.Empleados.HasValue)
                {
                    FilaOrganizacionEmpresa.Empleados = null;
                }
                else if (!FilaOrganizacionEmpresa.Empleados.HasValue && value != int.MinValue)
                {
                    FilaOrganizacionEmpresa.Empleados = value;
                }
                else if (FilaOrganizacionEmpresa.Empleados.HasValue && !FilaOrganizacionEmpresa.Empleados.Value.Equals(value))
                {
                    FilaOrganizacionEmpresa.Empleados = value;
                }
            }
        }

        /// <summary>
        /// Devuelve o establece el tipo de organización empresa
        /// </summary>
        public short TipoOrg
        {
            get
            {
                return FilaOrganizacionEmpresa.TipoOrganizacion;
            }
            set
            {
                if (!FilaOrganizacionEmpresa.TipoOrganizacion.Equals(value))
                {
                    FilaOrganizacionEmpresa.TipoOrganizacion = value;
                }
            }
        }

        /// <summary>
        /// Devuelve o establece el sector de organización empresa
        /// </summary>
        public short SectorOrg
        {
            get
            {
                return FilaOrganizacionEmpresa.SectorOrganizacion;
            }
            set
            {
                if (!FilaOrganizacionEmpresa.SectorOrganizacion.Equals(value))
                {
                    FilaOrganizacionEmpresa.SectorOrganizacion = value;
                }
            }
        }

        #endregion
    }
}
