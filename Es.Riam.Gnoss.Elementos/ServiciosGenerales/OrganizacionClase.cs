using Es.Riam.Gnoss.Util.General;

namespace Es.Riam.Gnoss.Elementos.ServiciosGenerales
{
    /// <summary>
    /// Organización clase
    /// </summary>
    public class OrganizacionClase : Organizacion
    {
        #region Miembros

        private OrganizacionClase mOrganizacionClase;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public OrganizacionClase(LoggingService loggingService)
            : base(loggingService)
        {
        }


        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene la fila de la organización clase
        /// </summary>
        public OrganizacionClase FilaOrganizacionClase
        {
            get
            {
                return mOrganizacionClase;
            }
        }

        /// <summary>
        /// Obtiene o establece la universidad
        /// </summary>
        public string Centro
        {
            get
            {
                return FilaOrganizacionClase.Centro;
            }
            set
            {
                if (!FilaOrganizacionClase.Centro.Equals(value))
                {
                    FilaOrganizacionClase.Centro = value;
                }
            }
        }

        /// <summary>
        /// Obtiene o establece el nombre corto del centro
        /// </summary>
        public string NombreCortoCentro
        {
            get
            {
                return FilaOrganizacionClase.NombreCortoCentro;
            }
            set
            {
                if (!FilaOrganizacionClase.NombreCortoCentro.Equals(value))
                {
                    FilaOrganizacionClase.NombreCortoCentro = value;
                }
            }
        }

        /// <summary>
        /// Obtiene o establece los estudios
        /// </summary>
        public string Asignatura
        {
            get
            {
                return FilaOrganizacionClase.Asignatura;
            }
            set
            {
                if (!FilaOrganizacionClase.Asignatura.Equals(value))
                {
                    FilaOrganizacionClase.Asignatura = value;
                }
            }
        }

        /// <summary>
        /// Obtiene o establece el curso
        /// </summary>
        public string Curso
        {
            get
            {
                return FilaOrganizacionClase.Curso;
            }
            set
            {
                if (!FilaOrganizacionClase.Curso.Equals(value))
                {
                    FilaOrganizacionClase.Curso = value;
                }
            }
        }

        /// <summary>
        /// Obtiene o establece el grupo
        /// </summary>
        public string Grupo
        {
            get
            {
                return FilaOrganizacionClase.Grupo;
            }
            set
            {
                if (!FilaOrganizacionClase.Grupo.Equals(value))
                {
                    FilaOrganizacionClase.Grupo = value;
                }
            }
        }

        /// <summary>
        /// Obtiene o establece el curso académico
        /// </summary>
        public string CursoAcademico
        {
            get
            {
                return FilaOrganizacionClase.CursoAcademico;
            }
            set
            {
                if (!FilaOrganizacionClase.CursoAcademico.Equals(value))
                {
                    FilaOrganizacionClase.CursoAcademico = value;
                }
            }
        }

        #endregion
    }
}
