using Es.Riam.Gnoss.AD.EntityModel.Models.OrganizacionDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.PersonaDS;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Util;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.Elementos.ServiciosGenerales
{
    /// <summary>
    /// Persona
    /// </summary>
    public class Persona : ElementoGnoss, IDisposable
    {
        #region Miembros

        /// <summary>
        /// Lista de organizaciones a las que esta vinculada la persona
        /// </summary>
        private SortedList<Guid, Organizacion> mListaOrganizaciones = null;

        /// <summary>
        /// Lista de vínculos de la persona con la organización
        /// </summary>
        SortedList<Guid, DatosTrabajoPersonaOrganizacion> mListaDatosTrabajoPersonaOrganizacion;

        /// <summary>
        /// Indica si se ha cargado la foto de la persona desde el DataLoader
        /// </summary>
        private bool mFotoCargada = false;

        /// <summary>
        /// Indica si se han cargado los tags de la persona desde el DataLoader
        /// </summary>
        private bool mTagsCargados = false;

        /// <summary>
        /// Indica si se han cargado los estados laborales de la persona desde el DataLoader
        /// </summary>
        private bool mHistoricoEstadosLaboralesCargados = false;

        /// <summary>
        /// Indica si se han cargado los estados perfiles de la persona desde el DataLoader
        /// </summary>
        private bool mPerfilesCargados = false;

        /// <summary>
        /// Indica si se ha cargado el usuario de la persona desde el DataLoader
        /// </summary>
        private bool mUsuarioCargado = false;

        /// <summary>
        /// Indica si se han cargado los amigos de la persona
        /// </summary>
        private bool mAmigosCargados = false;

        /// <summary>
        /// Datos de trabajo de persona libre
        /// </summary>
        private DatosTrabajoPersonaLibre mDatosTrabajoPersonaLibre;

        private AD.EntityModel.Models.PersonaDS.Persona mFilaPersona;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor vacío
        /// </summary>
        public Persona()
            : base()
        {
        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pGestorPersonas">Gestor de personas</param>
        /// <param name="pPersona">row de la persona</param>
        public Persona(AD.EntityModel.Models.PersonaDS.Persona pPersona, GestionPersonas pGestorPersonas)
            : base(pPersona, pGestorPersonas)
        {
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Indica si se ha cargado la foto de la persona desde el DataLoader
        /// </summary>
        public bool FotoCargada
        {
            get
            {
                return mFotoCargada;
            }
            set
            {
                mFotoCargada = value;
            }
        }

        /// <summary>
        /// Indica si se ha cargado el usuario de la persona desde el DataLoader
        /// </summary>
        public bool UsuarioCargado
        {
            get
            {
                return mUsuarioCargado;
            }
            set
            {
                mUsuarioCargado = value;
            }
        }

        /// <summary>
        /// Indica si se han cargado los tags de la persona desde el DataLoader
        /// </summary>
        public bool TagsCargados
        {
            get
            {
                return mTagsCargados;
            }
            set
            {
                mTagsCargados = value;
            }
        }

        /// <summary>
        /// Indica si se han cargado los historicos estados laborales  de la persona desde el DataLoader
        /// </summary>
        public bool HistoricoEstadosLaboralesCargados
        {
            get
            {
                return mHistoricoEstadosLaboralesCargados;
            }
            set
            {
                mHistoricoEstadosLaboralesCargados = value;
            }
        }

        /// <summary>
        /// Indica si se han cargado los perfiles de la persona desde el DataLoader
        /// </summary>
        public bool PerfilesCargados
        {
            get
            {
                return mPerfilesCargados;
            }
            set
            {
                mPerfilesCargados = value;
            }
        }

        /// <summary>
        /// Indica si se han cargado los amigos de la persona desde el DataLoader
        /// </summary>
        public bool AmigosCargados
        {
            get
            {
                return mAmigosCargados;
            }
            set
            {
                mAmigosCargados = value;
            }
        }

        /// <summary>
        /// Usuario que representa a la persona
        /// </summary>
        public UsuarioGnoss Usuario
        {
            get
            {
                if (GestorPersonas.GestorUsuarios != null)
                {
                    if (GestorPersonas.GestorUsuarios.ListaUsuarios.ContainsKey(UsuarioID))
                        return GestorPersonas.GestorUsuarios.ListaUsuarios[UsuarioID];
                }
                return null;
            }
        }

        /// <summary>
        /// Devuelve la lista de las organizaciones a las que esta vinculada una persona
        /// </summary>
        public SortedList<Guid, Organizacion> ListaOrganizacionesVinculadasConLaPersona
        {
            get
            {
                if (mListaOrganizaciones == null)
                {
                    mListaOrganizaciones = new SortedList<Guid, Organizacion>();
                    if (GestorPersonas.GestorOrganizaciones != null)
                        CargarOrganizacionesDePersona();
                }
                return mListaOrganizaciones;
            }
        }

        /// <summary>
        /// Devuelve la lista de vínculos de la persona con organizaciones
        /// </summary>
        public SortedList<Guid, DatosTrabajoPersonaOrganizacion> ListaDatosTrabajoPersonaOrganizacion
        {
            get
            {
                if (mListaDatosTrabajoPersonaOrganizacion == null)
                {
                    mListaDatosTrabajoPersonaOrganizacion = new SortedList<Guid, DatosTrabajoPersonaOrganizacion>();
                    if (GestorPersonas.GestorOrganizaciones != null)
                        CargarOrganizacionesDePersona();
                }
                return mListaDatosTrabajoPersonaOrganizacion;
            }
        }

        /// <summary>
        /// Devuelve una lista de PersonaVinculoOrganizacionRow con las organizaciones en las que está vinculada la persona
        /// </summary>
        public List<PersonaVinculoOrganizacion> ListaFilasOrganizacionesVinculadas
        {
            get
            {
                List<PersonaVinculoOrganizacion> lista = new List<PersonaVinculoOrganizacion>();

                foreach (DatosTrabajoPersonaOrganizacion perfil in ListaDatosTrabajoPersonaOrganizacion.Values)
                {
                    lista.Add(perfil.FilaVinculo);
                }
                return lista;
            }
        }

        /// <summary>
        /// Fila de la persona
        /// </summary>
        public AD.EntityModel.Models.PersonaDS.Persona FilaPersona
        {
            get
            {
                if (mFilaPersona == null)
                {
                    return (AD.EntityModel.Models.PersonaDS.Persona)FilaElementoEntity;
                }
                else
                {
                    return mFilaPersona;
                }
            }
            set { mFilaPersona = value; }
        }

        /// <summary>
        /// Perfil libre de la persona
        /// </summary>
        public DatosTrabajoPersonaLibre DatosTrabajoPersonaLibre
        {
            get
            {
                if (mDatosTrabajoPersonaLibre == null && FilaPersona.DatosTrabajoPersonaLibre != null)
                    mDatosTrabajoPersonaLibre = FilaPersona.DatosTrabajoPersonaLibre;
                return mDatosTrabajoPersonaLibre;
            }
            set
            {
                mDatosTrabajoPersonaLibre = value;
            }
        }

        /// <summary>
        /// Obtiene el gestor de personas
        /// </summary>
        public GestionPersonas GestorPersonas
        {
            get
            {
                return (GestionPersonas)GestorGnoss;
            }
        }

        /// <summary>
        /// Clave de la persona
        /// </summary>
        public override Guid Clave
        {
            get
            {
                return FilaPersona.PersonaID;
            }
        }

        /// <summary>
        /// Email de trabajo
        /// </summary>
        public string Email
        {
            get
            {
                if (!string.IsNullOrEmpty(FilaPersona.Email))
                {
                    return NombreConApellidos + " <" + FilaPersona.Email.ToLower() + ">";
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Devuelve el nombre de la persona junto con los apellidos
        /// </summary>
        public string NombreConApellidos
        {
            get
            {
                if (!FilaPersona.Apellidos.Equals(string.Empty))
                {
                    return FilaPersona.Nombre + " " + FilaPersona.Apellidos;
                }
                else
                {
                    return FilaPersona.Nombre;
                }   
            }
        }

        #region Campos de FilaPersona

        /// <summary>
        /// Devuelve o establece el nombre de la persona
        /// </summary>
        public override string Nombre
        {
            get
            {
                return FilaPersona.Nombre;
            }
            set
            {
                if (!FilaPersona.Nombre.Equals(value))
                {
                    FilaPersona.Nombre = value;
                    base.Nombre = value;
                }
            }
        }

        /// <summary>
        /// Devuelve o establece los apellidos de la persona
        /// </summary>
        public string Apellidos
        {
            get
            {
                return FilaPersona.Apellidos;
            }
            set
            {
                if (!FilaPersona.Apellidos.Equals(value) && value.Equals(string.Empty))
                    FilaPersona.Apellidos = null;
                else if (!FilaPersona.Apellidos.Equals(value))
                    FilaPersona.Apellidos = value;
            }
        }

        /// <summary>
        /// Devuelve o establece el TipoDocumentoAcreditativo de la persona
        /// </summary>
        public short TipoDocAcreditativo
        {
            get
            {
                if (!FilaPersona.TipoDocumentoAcreditativo.HasValue)
                    return (short)TipoDocumentoAcreditativo.NULL;

                return FilaPersona.TipoDocumentoAcreditativo.Value;
            }
            set
            {
                if (value == (short)TipoDocumentoAcreditativo.NULL && FilaPersona.TipoDocumentoAcreditativo.HasValue)
                    FilaPersona.TipoDocumentoAcreditativo = null;
                else if (!FilaPersona.TipoDocumentoAcreditativo.HasValue && value != (short)TipoDocumentoAcreditativo.NULL)
                    FilaPersona.TipoDocumentoAcreditativo = value;
                else if (FilaPersona.TipoDocumentoAcreditativo.HasValue && !FilaPersona.TipoDocumentoAcreditativo.Equals(value))
                    FilaPersona.TipoDocumentoAcreditativo = value;
            }
        }

        /// <summary>
        /// Devuelve o establece el ValorDocumentoAcreditativo de la persona
        /// </summary>
        public string ValorDocumentoAcreditativo
        {
            get
            {
                return FilaPersona.ValorDocumentoAcreditativo;
            }
            set
            {
                if (!FilaPersona.ValorDocumentoAcreditativo.Equals(value) && value.Equals(string.Empty))
                    FilaPersona.ValorDocumentoAcreditativo = null;
                else if (!FilaPersona.ValorDocumentoAcreditativo.Equals(value))
                    FilaPersona.ValorDocumentoAcreditativo = value;
            }
        }

        /// <summary>
        /// Foto de la persona
        /// </summary>
        public byte[] Foto
        {
            get
            {
                if (FilaPersona.Foto == null)
                    return new byte[0];

                return FilaPersona.Foto;
            }
            set
            {
                if (value.Length.Equals(0) && FilaPersona != null)
                    FilaPersona.Foto = null;
                else if (FilaPersona.Foto == null && !value.Length.Equals(0))
                    FilaPersona.Foto = value;
                else if (FilaPersona != null && !UtilGeneral.ArraysIguales(FilaPersona.Foto, value))
                    FilaPersona.Foto = value;
            }
        }

        /// <summary>
        /// Devuelve o establece el sexo de la persona
        /// </summary>
        public string Sexo
        {
            get
            {
                if (FilaPersona.Sexo == null)
                {
                    return string.Empty;
                }
                else
                {
                    return FilaPersona.Sexo;
                }

            }
            set
            {
                if (!FilaPersona.Sexo.Equals(value))
                    FilaPersona.Sexo = value;
            }
        }

        /// <summary>
        /// Devuelve o establece la fecha de nacimiento de la persona
        /// </summary>
        public override DateTime Fecha
        {
            get
            {
                if (!FilaPersona.FechaNacimiento.HasValue)
                    return DateTime.MinValue;

                return FilaPersona.FechaNacimiento.Value;
            }
            set
            {
                if (value == DateTime.MinValue && FilaPersona.FechaNacimiento.HasValue)
                    FilaPersona.FechaNacimiento = null;
                else if (!FilaPersona.FechaNacimiento.HasValue && value != DateTime.MinValue)
                    FilaPersona.FechaNacimiento = value;
                else if (FilaPersona.FechaNacimiento.HasValue && !FilaPersona.FechaNacimiento.Equals(value))
                    FilaPersona.FechaNacimiento = value;
            }
        }

        /// <summary>
        /// Devuelve TRUE si la persona es menor de edad teniendo solo en cuenta el año, es decir, que si este año cumple 18 es mayor.
        /// </summary>
        public bool EsMenorEdad
        {
            get
            {
                return (Fecha.AddYears(18).CompareTo(DateTime.Now) > 0);
            }
        }

        /// <summary>
        /// Devuelve o establece el GUID del identificador de pais
        /// </summary>
        public Guid PaisID
        {
            get
            {
                if (!FilaPersona.PaisPersonalID.HasValue)
                    return Guid.Empty;

                return FilaPersona.PaisPersonalID.Value;
            }
            set
            {
                if (value.Equals(Guid.Empty) && FilaPersona.PaisPersonalID.HasValue)
                    FilaPersona.PaisPersonalID = null;
                else if (!FilaPersona.PaisPersonalID.HasValue && value != Guid.Empty)
                    FilaPersona.PaisPersonalID = value;
                else if (FilaPersona.PaisPersonalID.HasValue && !FilaPersona.PaisPersonalID.Equals(value))
                    FilaPersona.PaisPersonalID = value;
            }
        }

        /// <summary>
        /// Devuelve o establece el GUID del identificador de la provincia
        /// </summary>
        public Guid ProvinciaID
        {
            get
            {
                if (!FilaPersona.ProvinciaPersonalID.HasValue)
                    return Guid.Empty;

                return FilaPersona.ProvinciaPersonalID.Value;
            }
            set
            {
                if (value.Equals(Guid.Empty) && FilaPersona.ProvinciaPersonalID.HasValue)
                    FilaPersona.ProvinciaPersonalID = null;
                else if (!FilaPersona.ProvinciaPersonalID.HasValue && value != Guid.Empty)
                    FilaPersona.ProvinciaPersonalID = value;
                else if (FilaPersona.ProvinciaPersonalID.HasValue && !FilaPersona.ProvinciaPersonalID.Equals(value))
                    FilaPersona.ProvinciaPersonalID = value;
            }
        }

        /// <summary>
        /// Devuelve o establece la provincia
        /// </summary>
        public string Provincia
        {
            get
            {
                return FilaPersona.ProvinciaPersonal;
            }
            set
            {
                if (!FilaPersona.ProvinciaPersonal.Equals(value) && string.IsNullOrEmpty(value))
                    FilaPersona.ProvinciaPersonal = null;
                else if (!FilaPersona.ProvinciaPersonal.Equals(value))
                    FilaPersona.ProvinciaPersonal = value;
            }
        }

        /// <summary>
        /// Devuelve o establece la localidad
        /// </summary>
        public string Localidad
        {
            get
            {
                return FilaPersona.LocalidadPersonal;
            }
            set
            {
                if (FilaPersona.LocalidadPersonal != null && string.IsNullOrEmpty(value))
                    FilaPersona.LocalidadPersonal = null;
                else if (FilaPersona.LocalidadPersonal != null && !FilaPersona.LocalidadPersonal.Equals(value))
                    FilaPersona.LocalidadPersonal = value;
                else if (FilaPersona.LocalidadPersonal == null)
                {
                    FilaPersona.LocalidadPersonal = value;
                }
            }
        }

        /// <summary>
        /// Devuelve o establece el codigo postal
        /// </summary>
        public string CodPostal
        {
            get
            {
                return FilaPersona.CPPersonal;
            }
            set
            {
                if (!FilaPersona.CPPersonal.Equals(value) && string.IsNullOrEmpty(value))
                    FilaPersona.CPPersonal = null;
                else if (!FilaPersona.CPPersonal.Equals(value))
                    FilaPersona.CPPersonal = value;
            }
        }

        /// <summary>
        /// Devuelve o establece la dirección
        /// </summary>
        public string Direccion
        {
            get
            {
                return FilaPersona.DireccionPersonal;
            }
            set
            {
                if (!FilaPersona.DireccionPersonal.Equals(value) && string.IsNullOrEmpty(value))
                    FilaPersona.DireccionPersonal = null;
                else if (!FilaPersona.DireccionPersonal.Equals(value))
                    FilaPersona.DireccionPersonal = value;
            }
        }

        /// <summary>
        /// Devuelve o establece el telefono
        /// </summary>
        public string Telefono
        {
            get
            {
                return FilaPersona.TelefonoPersonal;
            }
            set
            {
                if (!FilaPersona.TelefonoPersonal.Equals(value) && string.IsNullOrEmpty(value))
                    FilaPersona.TelefonoPersonal = null;
                else if (!FilaPersona.TelefonoPersonal.Equals(value))
                    FilaPersona.TelefonoPersonal = value;
            }
        }

        /// <summary>
        /// Devuelve o establece el mail
        /// </summary>
        public string Mail
        {
            get
            {
                return FilaPersona.Email;
            }
            set
            {
                if (!FilaPersona.Email.Equals(value) && string.IsNullOrEmpty(value))
                    FilaPersona.Email = null;
                else if (!FilaPersona.Email.Equals(value))
                    FilaPersona.Email = value.ToLower();
            }
        }

        /// <summary>
        /// Devuelve o establece el GUID del identificador de estado civil
        /// </summary>
        public Guid EstadoCivilID
        {
            get
            {
                if (!FilaPersona.EstadoCivilID.HasValue)
                    return Guid.Empty;

                return FilaPersona.EstadoCivilID.Value;
            }
            set
            {
                if (value.Equals(Guid.Empty) && FilaPersona.EstadoCivilID.HasValue)
                    FilaPersona.EstadoCivilID = null;
                else if (!FilaPersona.EstadoCivilID.HasValue && value != Guid.Empty)
                    FilaPersona.EstadoCivilID = value;
                else if (FilaPersona.EstadoCivilID.HasValue && !FilaPersona.EstadoCivilID.Equals(value))
                    FilaPersona.EstadoCivilID = value;
            }
        }

        /// <summary>
        /// Devuelve o establece el GUID del identificador de la titulacion
        /// </summary>
        public Guid TitulacionID
        {
            get
            {
                if (!FilaPersona.TitulacionID.HasValue)
                    return Guid.Empty;

                return FilaPersona.TitulacionID.Value;
            }
            set
            {
                if (value.Equals(Guid.Empty) && FilaPersona.TitulacionID.HasValue)
                    FilaPersona.TitulacionID = null;
                else if (!FilaPersona.TitulacionID.HasValue && value != Guid.Empty)
                    FilaPersona.TitulacionID = value;
                else if (FilaPersona.TitulacionID.HasValue && !FilaPersona.TitulacionID.Equals(value))
                    FilaPersona.TitulacionID = value;
            }
        }

        /// <summary>
        /// Devuelve o establece los hijos de la persona
        /// </summary>
        public short NumHijos
        {
            get
            {
                if (!FilaPersona.Hijos.HasValue)
                    return short.MinValue;

                return FilaPersona.Hijos.Value;
            }
            set
            {
                if (value == short.MinValue && FilaPersona.Hijos.HasValue)
                    FilaPersona.Hijos = null;
                else if (!FilaPersona.Hijos.HasValue && value != short.MinValue)
                    FilaPersona.Hijos = value;
                else if (FilaPersona.Hijos.HasValue && !FilaPersona.Hijos.Equals(value))
                    FilaPersona.Hijos = value;
            }
        }

        /// <summary>
        /// Devuelve o establece si la persona aparecerá en la lista pública de personas
        /// </summary>
        public bool AparecerEnListaPublica
        {
            get
            {
                return FilaPersona.EsBuscable;
            }
            set
            {
                if (!FilaPersona.EsBuscable.Equals(value))
                    FilaPersona.EsBuscable = value;
            }
        }

        /// <summary>
        /// Devuelve o establece el GUID del identificador de usuario
        /// </summary>
        public Guid UsuarioID
        {
            get
            {
                if (!FilaPersona.UsuarioID.HasValue)
                    return Guid.Empty;

                return FilaPersona.UsuarioID.Value;
            }
            set
            {
                if (value.Equals(Guid.Empty) && FilaPersona.UsuarioID.HasValue)
                    FilaPersona.UsuarioID = null;
                else if (!FilaPersona.UsuarioID.HasValue && value != Guid.Empty)
                    FilaPersona.UsuarioID = value;
                else if (FilaPersona.UsuarioID.HasValue && !FilaPersona.UsuarioID.Equals(value))
                    FilaPersona.UsuarioID = value;
            }
        }

        #endregion

        #endregion

        #region Métodos privados

        /// <summary>
        /// Carga las listas de organizaciones vinculadas a las personas
        /// </summary>
        private void CargarOrganizacionesDePersona()
        {
            mListaDatosTrabajoPersonaOrganizacion = new SortedList<Guid, DatosTrabajoPersonaOrganizacion>();
            mListaOrganizaciones = new SortedList<Guid, Organizacion>();

            if (GestorPersonas.GestorOrganizaciones != null)
            {
                foreach (PersonaVinculoOrganizacion filaPersonaVinculoOrganizacion in GestorPersonas.GestorOrganizaciones.OrganizacionDW.ListaPersonaVinculoOrganizacion.Where(item => item.PersonaID.Equals(Clave)))
                {
                    GestorPersonas.GestorOrganizaciones.CargarOrganizaciones();
                    Guid organizacionID = filaPersonaVinculoOrganizacion.OrganizacionID;
                    if (GestorPersonas.GestorOrganizaciones.ListaOrganizaciones.ContainsKey(organizacionID))
                    {
                        mListaOrganizaciones.Add(organizacionID, GestorPersonas.GestorOrganizaciones.ListaOrganizaciones[organizacionID]);
                        mListaDatosTrabajoPersonaOrganizacion.Add(organizacionID, new DatosTrabajoPersonaOrganizacion(filaPersonaVinculoOrganizacion, GestorPersonas.GestorOrganizaciones));
                    }
                }
            }
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Determina si está disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~Persona()
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
                    //Libero todos los recursos nativos sin administrar que he añadido a esta clase
                    if (mListaOrganizaciones != null)
                        mListaOrganizaciones = null;
                }
                finally
                {
                    mDatosTrabajoPersonaLibre = null;

                    // Llamo al dispose de la clase base
                    base.Dispose(disposing);
                }
            }
        }

        #endregion
    }
}
