using Es.Riam.Gnoss.AD.EntityModel.Models.OrganizacionDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Interfaces;
using Es.Riam.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Es.Riam.Gnoss.Elementos.ServiciosGenerales
{
    /// <summary>
    /// Organización
    /// </summary>
    public class Organizacion : ElementoGnoss, IDisposable
    {
        #region Miembros

        /// <summary>
        /// Lista de las personas que administran la organización
        /// </summary>
        private List<Persona> mListaAdministradores;

        /// <summary>
        /// Lista de personas que tiene relacion con la organizacion
        /// </summary>
        protected SortedList<Guid, Persona> mListaPersonas = null;

        /// <summary>
        /// Indica si se ha cargado el logotipo de la organizacion desde el DataLoader
        /// </summary>
        private bool mLogoCargado = false;

        /// <summary>
        /// Indica si se han cargado los tags de la organizacion desde el DataLoader
        /// </summary>
        private bool mTagsCargados = false;

        /// <summary>
        /// Indica si se han cargado las agregaciones con CNAE desde el DataLoader
        /// </summary>
        private bool mAgregacionesCNAESCargados = false;

        /// <summary>
        /// Indica si se han cargado los administradores desde el DataLoader
        /// </summary>
        private bool mAdministradoresOrgCargados = false;

        /// <summary>
        /// Indica si se han cargado cargado las personas establecidas como visibles para la organización "PersonaVisibleEnOrg"
        /// </summary>
        private bool mPersonasVisiblesEnOrganizacionCargados = false;

        /// <summary>
        /// Indica si se han cargado los usuarios de la organizaciópn y sus permisos a nivel de organización
        /// </summary>
        private bool mUsuariosCargados = false;

        /// <summary>
        /// Indica si se han cargado las identidades y perfiles de los usuarios de la organización (las personas vinculadas que tienen usuarios)
        /// </summary>
        private bool mIdentidadesUsuariosCargados = false;

        /// <summary>
        /// Indica si se han cargado los historicos laborales para esa organización
        /// </summary>
        private bool mHistoricosLaboralesCargados = false;

        /// <summary>
        /// Indica si se ha cargado el tesauro de la organizacion
        /// </summary>
        private bool mTesauroCargado;

        /// <summary>
        /// Indica si se ha cargado la base de recursos de la organización
        /// </summary>
        private bool mBaseRecursosCargada;

        /// <summary>
        /// Indica si se ha cargado el perfil y sus identidades asociades de la organización
        /// </summary>
        private bool mPerfilCargado;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public Organizacion()
            : base()
        {

        }

        /// <summary>
        /// Constructor de la organización
        /// </summary>
        /// <param name="pOrganizacion">Fila de organización</param>
        /// <param name="pGestorOrganizaciones">Gestor de organizaciones</param>
        public Organizacion(AD.EntityModel.Models.OrganizacionDS.Organizacion pOrganizacion, GestionOrganizaciones pGestorOrganizaciones)
            : base(pOrganizacion, pGestorOrganizaciones)
        {
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Indica si se han cargado el perfil de la organizacion desde el DataLoader
        /// </summary>
        public bool PerfilCargado
        {
            get
            {
                return mPerfilCargado;
            }
            set
            {
                mPerfilCargado = value;
            }
        }

        /// <summary>
        /// Devuelve si la organización es raiz (No depende de ninguna otra)
        /// </summary>
        public bool EsRaiz
        {
            get
            {
                return !FilaOrganizacion.OrganizacionPadreID.HasValue;
            }
        }


        /// <summary>
        /// Devuelve TRUE si el usuario actual está vinculado con la organización en concreto
        /// </summary>
        public bool UsuarioVinculado(Guid pPersonaID)
        {
            return GestorOrganizaciones.OrganizacionDW.ListaPersonaVinculoOrganizacion.Where(item => item.OrganizacionID.Equals(Clave) && item.PersonaID.Equals(pPersonaID)).Count() > 0;
        }

        /// <summary>
        /// Obtiene o establece si se han cargado los administradores de la organización desde el DataLoader
        /// </summary>
        public bool AdministradoresOrgCargados
        {
            get
            {
                return mAdministradoresOrgCargados;
            }
            set
            {
                mAdministradoresOrgCargados = value;
            }
        }

        /// <summary>
        /// Obtiene o establece si se han cargado las personas establecidas como visibles para la organización "PersonaVisibleEnOrg" desde el DataLoader
        /// </summary>
        public bool PersonasVisiblesEnOrganizacionCargados
        {
            get
            {
                return mPersonasVisiblesEnOrganizacionCargados;
            }
            set
            {
                mPersonasVisiblesEnOrganizacionCargados = value;
            }
        }

        /// <summary>
        /// Obtiene o establece si se han cargado las agregaciones con CNAE desde el DataLoader
        /// </summary>
        public bool AgregacionesCNAESCargados
        {
            get
            {
                return mAgregacionesCNAESCargados;
            }
            set
            {
                mAgregacionesCNAESCargados = value;
            }
        }

        /// <summary>
        /// Obtiene o establece si se ha cargado el logotipo de la organización desde el DataLoader
        /// </summary>
        public bool LogoCargado
        {
            get
            {
                return mLogoCargado;
            }
            set
            {
                mLogoCargado = value;
            }
        }

        /// <summary>
        /// Obtiene o establece si se han cargado los tags de la organización desde el DataLoader
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
        /// Obtiene o establece si se han cargado el Tesauro de la organización 
        /// </summary>
        public bool TesauroCargado
        {
            get
            {
                return mTesauroCargado;
            }
            set
            {
                mTesauroCargado = value;
            }
        }

        /// <summary>
        /// Obtiene o establece si se ha cargado la base de recursos de la organización
        /// </summary>
        public bool BaseRecursosCargada
        {
            get
            {
                return mBaseRecursosCargada;
            }
            set
            {
                mBaseRecursosCargada = value;
            }
        }

        /// <summary>
        /// Obtiene o establece si se han cargado los usuarios de la organización (las personas vinculadas que tienen usuarios)
        /// </summary>
        public bool UsuariosCargados
        {
            get
            {
                return mUsuariosCargados;
            }
            set
            {
                mUsuariosCargados = value;
            }
        }

        /// <summary>
        /// Obtiene o establece si se han cargado las identidades y perfiles de los usuarios de la organización (las personas vinculadas que tienen usuarios)
        /// </summary>
        public bool IdentidadesUsuariosCargados
        {
            get
            {
                return mIdentidadesUsuariosCargados;
            }
            set
            {
                mIdentidadesUsuariosCargados = value;
            }
        }

        /// <summary>
        /// Obtiene o establece si se han cargado los históricos laborales para esa organización
        /// </summary>
        public bool HistoricosLaboralesCargados
        {
            get
            {
                return mHistoricosLaboralesCargados;
            }
            set
            {
                mHistoricosLaboralesCargados = value;
            }
        }

        /// <summary>
        /// Obtiene la lista de administradores del proyecto
        /// </summary>
        public List<Persona> ListaAdministradores
        {
            get
            {
                if (mListaAdministradores == null)
                {
                    mListaAdministradores = new List<Persona>();

                    foreach (AdministradorOrganizacion filaAdmin in GestorOrganizaciones.OrganizacionDW.ListaAdministradorOrganizacion)
                    {
                        Guid usuarioID = filaAdmin.UsuarioID;
                        AD.EntityModel.Models.PersonaDS.Persona personaRes = GestorOrganizaciones.GestorPersonas.DataWrapperPersonas.ListaPersona.FirstOrDefault(persona => persona.UsuarioID.Equals(usuarioID));
                        if (personaRes != null)
                        {
                            mListaAdministradores.Add(GestorOrganizaciones.GestorPersonas.ListaPersonas[personaRes.PersonaID]);
                        }
                    }
                }
                return mListaAdministradores;
            }
        }

        /// <summary>
        /// Devuelve la lista de las personas que estan vinculadas con la organización
        /// </summary>
        public SortedList<Guid, Persona> ListaPersonasVinculadasConLaOrganizacion
        {
            get
            {
                if (mListaPersonas == null)
                {
                    mListaPersonas = new SortedList<Guid, Persona>();

                    if (GestorOrganizaciones.GestorPersonas != null)
                    {
                        CargarPersonasDeOrganizacion();
                    }
                }
                return mListaPersonas;
            }
        }

        /// <summary>
        /// Obtiene la lista de usuarios vinculados a la organización
        /// </summary>
        public SortedList<Guid, UsuarioGnoss> ListaUsuariosVinculados
        {
            get
            {
                SortedList<Guid, UsuarioGnoss> mListaUsuarios = new SortedList<Guid, UsuarioGnoss>();

                if (GestorOrganizaciones.GestorPersonas != null && GestorOrganizaciones.GestorPersonas.GestorUsuarios != null)
                {
                    foreach (Persona persona in ListaPersonasVinculadasConLaOrganizacion.Values)
                    {
                        if (persona.Usuario != null)
                        {
                            mListaUsuarios.Add(persona.UsuarioID, persona.Usuario);
                        }
                    }
                }
                return mListaUsuarios;
            }
        }

        /// <summary>
        /// Obtiene la lista de elementos hijos
        /// </summary>
        public override List<IElementoGnoss> Hijos
        {
            get
            {
                if (mHijos == null)
                {
                    mHijos = new List<IElementoGnoss>();

                    foreach (AD.EntityModel.Models.OrganizacionDS.Organizacion organizacionRow in FilaOrganizacion.Organizacion1)
                    {
                        Organizacion org;

                        if (GestorOrganizaciones.ListaOrganizaciones.ContainsKey(organizacionRow.OrganizacionID))
                        {
                            org = GestorOrganizaciones.ListaOrganizaciones[organizacionRow.OrganizacionID];
                        }
                        else
                        {
                            org = new Organizacion(organizacionRow, GestorOrganizaciones);

                            GestorOrganizaciones.ListaOrganizaciones.Add(organizacionRow.OrganizacionID, org);
                        }
                        org.Padre = this;
                        mHijos.Add(org);
                    }
                }
                return mHijos;
            }
        }

        /// <summary>
        /// Obtiene la fila de la organización
        /// </summary>
        public AD.EntityModel.Models.OrganizacionDS.Organizacion FilaOrganizacion
        {
            get
            {
                return (AD.EntityModel.Models.OrganizacionDS.Organizacion)FilaElementoEntity;
            }
        }

        /// <summary>
        /// Obtiene el gestor de organizaciones
        /// </summary>
        public GestionOrganizaciones GestorOrganizaciones
        {
            get
            {
                return (GestionOrganizaciones)GestorGnoss;
            }
        }

        /// <summary>
        /// Obtiene la clave de la organización
        /// </summary>
        public override Guid Clave
        {
            get
            {
                return FilaOrganizacion.OrganizacionID;
            }
        }

        #region Campos de tablas

        /// <summary>
        /// Devuelve o establece el nombre de la organización
        /// </summary>
        public override string Nombre
        {
            get
            {
                return FilaOrganizacion.Nombre;
            }
            set
            {
                if (!FilaOrganizacion.Nombre.Equals(value))
                {
                    FilaOrganizacion.Nombre = value;
                    base.Nombre = value;
                }
            }
        }

        /// <summary>
        /// Devuelve o establece el nombreCorto de la organización
        /// </summary>
        public string NombreCorto
        {
            get
            {
                return FilaOrganizacion.NombreCorto;
            }
            set
            {
                FilaOrganizacion.NombreCorto = value;
            }
        }

        /// <summary>
        /// Devuelve o establece el teléfono de la organización
        /// </summary>
        public string Telefono
        {
            get
            {
                return FilaOrganizacion.Telefono;
            }
            set
            {
                if (!FilaOrganizacion.Telefono.Equals(value) && value.Equals(string.Empty))
                {
                    FilaOrganizacion.Telefono = null;
                }
                else if (!FilaOrganizacion.Telefono.Equals(value))
                {
                    FilaOrganizacion.Telefono = value;
                }
            }
        }

        /// <summary>
        /// Devuelve o establece el mail de la organización
        /// </summary>
        public string Mail
        {
            get
            {
                return FilaOrganizacion.Email;
            }
            set
            {
                if (!FilaOrganizacion.Email.Equals(value) && value.Equals(string.Empty))
                {
                    FilaOrganizacion.Email = null;
                }
                else if (!FilaOrganizacion.Email.Equals(value))
                {
                    FilaOrganizacion.Email = value;
                }
            }
        }

        /// <summary>
        /// Devuelve o establece el Fax de la organización
        /// </summary>
        public string Fax
        {
            get
            {
                return FilaOrganizacion.Fax;
            }
            set
            {
                if (!FilaOrganizacion.Fax.Equals(value) && value.Equals(string.Empty))
                {
                    FilaOrganizacion.Fax = null;
                }
                else if (!FilaOrganizacion.Fax.Equals(value))
                {
                    FilaOrganizacion.Fax = value;
                }
            }
        }

        /// <summary>
        /// Devuelve o establece la web de la organización
        /// </summary>
        public string Web
        {
            get
            {
                return FilaOrganizacion.Web;
            }
            set
            {
                if (!FilaOrganizacion.Web.Equals(value) && value.Equals(string.Empty))
                {
                    FilaOrganizacion.Web = null;
                }
                else if (!FilaOrganizacion.Web.Equals(value))
                {
                    FilaOrganizacion.Web = value;
                }
            }
        }

        /// <summary>
        /// Obtiene o establece el logotipo de la organización
        /// </summary>
        public byte[] Logotipo
        {
            get
            {
                if (FilaOrganizacion.Logotipo == null)
                {
                    return new byte[0];
                }
                return FilaOrganizacion.Logotipo;
            }
            set
            {
                if (value.Length.Equals(0) && FilaOrganizacion.Logotipo != null)
                {
                    FilaOrganizacion.Logotipo = null;
                }
                else if (FilaOrganizacion.Logotipo == null && !value.Length.Equals(0))
                {
                    FilaOrganizacion.Logotipo = value;
                }
                else if (FilaOrganizacion.Logotipo != null && !UtilGeneral.ArraysIguales(FilaOrganizacion.Logotipo, value))
                {
                    FilaOrganizacion.Logotipo = value;
                }
            }
        }

        /// <summary>
        /// Devuelve o establece el identificador de pais de la organización
        /// </summary>
        public Guid PaisID
        {
            get
            {
                if (!FilaOrganizacion.PaisID.HasValue)
                {
                    return Guid.Empty;
                }
                return FilaOrganizacion.PaisID.Value;
            }
            set
            {
                if (value.Equals(Guid.Empty) && FilaOrganizacion.PaisID.HasValue)
                {
                    FilaOrganizacion.PaisID = null;
                }
                else if (!FilaOrganizacion.PaisID.HasValue && value != Guid.Empty)
                {
                    FilaOrganizacion.PaisID = value;
                }
                else if (FilaOrganizacion.PaisID.HasValue && !FilaOrganizacion.PaisID.Equals(value))
                {
                    FilaOrganizacion.PaisID = value;
                }
            }
        }

        /// <summary>
        /// Devuelve o establece el identificador de la provincia de la organización
        /// </summary>
        public Guid ProvinciaID
        {
            get
            {
                if (!FilaOrganizacion.ProvinciaID.HasValue)
                {
                    return Guid.Empty;
                }
                return FilaOrganizacion.ProvinciaID.Value;
            }
            set
            {
                if (value.Equals(Guid.Empty) && FilaOrganizacion.ProvinciaID.HasValue)
                {
                    FilaOrganizacion.ProvinciaID = null;
                }
                else if (!FilaOrganizacion.ProvinciaID.HasValue && value != Guid.Empty)
                {
                    FilaOrganizacion.ProvinciaID = value;
                }
                else if (FilaOrganizacion.ProvinciaID.HasValue && !FilaOrganizacion.ProvinciaID.Equals(value))
                {
                    FilaOrganizacion.ProvinciaID = value;
                }
            }
        }

        /// <summary>
        /// Devuelve o establece la provincia de la organización
        /// </summary>
        public string Provincia
        {
            get
            {
                return FilaOrganizacion.Provincia;
            }
            set
            {
                if (!FilaOrganizacion.Provincia.Equals(value) && value.Equals(string.Empty))
                {
                    FilaOrganizacion.Provincia = null;
                }
                else if (!FilaOrganizacion.Provincia.Equals(value))
                {
                    FilaOrganizacion.Provincia = value;
                }
            }
        }

        /// <summary>
        /// Devuelve o establece la localidad de la organización
        /// </summary>
        public string Localidad
        {
            get
            {
                return FilaOrganizacion.Localidad;
            }
            set
            {
                if (!FilaOrganizacion.Localidad.Equals(value) && value.Equals(string.Empty))
                {
                    FilaOrganizacion.Localidad = null;
                }
                else if (!FilaOrganizacion.Localidad.Equals(value))
                {
                    FilaOrganizacion.Localidad = value;
                }
            }
        }

        /// <summary>
        /// Devuelve o establece el código postal de la organización
        /// </summary>
        public string CodPostal
        {
            get
            {
                return FilaOrganizacion.CP;
            }
            set
            {
                if (!FilaOrganizacion.CP.Equals(value) && value.Equals(string.Empty))
                {
                    FilaOrganizacion.CP = null;
                }
                else if (!FilaOrganizacion.CP.Equals(value))
                {
                    FilaOrganizacion.CP = value;
                }
            }
        }

        /// <summary>
        /// Devuelve o establece la dirección de la organización
        /// </summary>
        public string Direccion
        {
            get
            {
                return FilaOrganizacion.Direccion;
            }
            set
            {
                if (!FilaOrganizacion.Direccion.Equals(value) && value.Equals(string.Empty))
                {
                    FilaOrganizacion.Direccion = null;
                }
                else if (!FilaOrganizacion.Direccion.Equals(value))
                {
                    FilaOrganizacion.Direccion = value;
                }
            }
        }

        /// <summary>
        /// Devuelve o establece si la organización aparecerá en la lista pública de organizaciones
        /// </summary>
        public bool AparecerEnListaPublica
        {
            get
            {
                return FilaOrganizacion.EsBuscable;
            }
            set
            {
                if (!FilaOrganizacion.EsBuscable.Equals(value))
                {
                    FilaOrganizacion.EsBuscable = value;
                }
            }
        }

        /// <summary>
        /// Devuelve o establece si la organización trabaja en modo personal
        /// </summary>
        public bool ModoPersonal
        {
            get
            {
                return FilaOrganizacion.ModoPersonal;
            }
            set
            {
                if (!FilaOrganizacion.ModoPersonal.Equals(value))
                {
                    FilaOrganizacion.ModoPersonal = value;
                }
            }
        }

        #endregion

        #endregion

        #region Métodos protegidos

        /// <summary>
        /// Carga las personas vinculadas a la organización
        /// </summary>
        protected void CargarPersonasDeOrganizacion()
        {
            if (GestorOrganizaciones.GestorPersonas != null)
            {
                foreach (PersonaVinculoOrganizacion filaPersonaVinculoOrganizacion in FilaOrganizacion.PersonaVinculoOrganizacion)
                {
                    Guid personaID = filaPersonaVinculoOrganizacion.PersonaID;

                    if (!mListaPersonas.ContainsKey(personaID) && GestorOrganizaciones.GestorPersonas.ListaPersonas.ContainsKey(personaID))
                    {
                        mListaPersonas.Add(personaID, GestorOrganizaciones.GestorPersonas.ListaPersonas[personaID]);
                    }
                }
            }
        }

        #endregion

        #region Métodos públicos

        /// <summary>
        /// Recarga la lista de personas vinculadas
        /// </summary>
        public void RecargarPersonasVinculadas()
        {
            CargarPersonasDeOrganizacion();
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Determina si está disposed
        /// </summary>
        private bool mDisposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~Organizacion()
        {
            //Libero los recursos
            Dispose(false);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        /// <param name="pDisposing">Determina si se está llamando desde el Dispose()</param>
        protected override void Dispose(bool pDisposing)
        {
            if (!this.mDisposed)
            {
                this.mDisposed = true;
                try
                {
                    if (pDisposing)
                    {
                    }
                    //Libero todos los recursos nativos sin administrar que he añadido a esta clase
                }
                finally
                {
                    mListaPersonas = null;
                    mListaAdministradores = null;
                    // Llamo al dispose de la clase base
                    base.Dispose(pDisposing);
                }
            }
        }

        #endregion
    }
}
