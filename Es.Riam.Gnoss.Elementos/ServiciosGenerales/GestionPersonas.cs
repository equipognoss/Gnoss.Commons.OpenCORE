using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.PersonaDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Suscripcion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Es.Riam.Gnoss.Elementos.ServiciosGenerales
{
    /// <summary>
    /// Gestor de personas
    /// </summary>
    [Serializable]
    public class GestionPersonas : GestionGnoss, ISerializable, IDisposable
    {
        #region Miembros

        /// <summary>
        /// Gestor de organizaciones
        /// </summary>
        private GestionOrganizaciones mGestionOrganizaciones;

        /// <summary>
        /// Gestor de usuarios
        /// </summary>
        private GestionUsuarios mGestionUsuarios;


        /// <summary>
        /// DataSet de país
        /// </summary>
        private DataWrapperPais mPaisDW;


        /// <summary>
        /// Lista de personas
        /// </summary>
        private SortedList<Guid, Persona> mListaPersonas;

        private LoggingService mLoggingService;
        private EntityContext mEntityContext;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor a partir del dataset de personas pasado por parámetro
        /// </summary>
        /// <param name="pDataWrapperPersona">Dataset de persona</param>
        public GestionPersonas(DataWrapperPersona pDataWrapperPersona, LoggingService loggingService, EntityContext entityContext)
            : base(pDataWrapperPersona, loggingService)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;

            CargarPersonas();
        }

        /// <summary>
        /// Constructor para la deseralización
        /// </summary>
        /// <param name="pInfo">Datos serializados</param>
        /// <param name="pContext">Contexto de serialización</param>
        protected GestionPersonas(SerializationInfo pInfo, StreamingContext pContext)
            : base(pInfo, pContext)
        {
            //mEntityContext = entityContext;
            //mLoggingService = loggingService;

            mPaisDW = (DataWrapperPais)pInfo.GetValue("PaisDW", typeof(DataWrapperPais));
            mGestionOrganizaciones = (GestionOrganizaciones)pInfo.GetValue("GestorOrganizaciones", typeof(GestionOrganizaciones));
            mGestionUsuarios = (GestionUsuarios)pInfo.GetValue("GestorUsuarios", typeof(GestionUsuarios));

            CargarPersonas();
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece el dataset de paises
        /// </summary>
        public DataWrapperPais PaisDW
        {
            get
            {
                return mPaisDW;
            }
            set
            {
                mPaisDW = value;
            }
        }

        /// <summary>
        /// Obtiene la lista de personas (identificador, objeto)
        /// </summary>
        public SortedList<Guid, Persona> ListaPersonas
        {
            get
            {
                if (mListaPersonas == null)
                    CargarPersonas();
                return mListaPersonas;
            }
        }

        /// <summary>
        /// Obtiene o establece el gestor de organizaciones
        /// </summary>
        public GestionOrganizaciones GestorOrganizaciones
        {
            get
            {
                return mGestionOrganizaciones;
            }
            set
            {
                mGestionOrganizaciones = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el gestor de usuarios
        /// </summary>
        public GestionUsuarios GestorUsuarios
        {
            get
            {
                return mGestionUsuarios;
            }
            set
            {
                mGestionUsuarios = value;
            }
        }

        public DataWrapperPersona DataWrapperPersonas
        {
            get
            {
                return (DataWrapperPersona)DataWrapper;
            }
            set
            {
                DataWrapper = value;
            }
        }

        #endregion

        #region Métodos generales

        /// <summary>
        /// Carga las personas
        /// </summary>
        private void CargarPersonas()
        {
            mListaPersonas = new SortedList<Guid, Persona>();

            if (DataWrapperPersonas != null)
            {
                foreach (AD.EntityModel.Models.PersonaDS.Persona filaPersona in DataWrapperPersonas.ListaPersona)
                {
                    Persona persona = new Persona(filaPersona, this, mLoggingService);
                    if (!mListaPersonas.ContainsKey(persona.Clave))
                    {
                        mListaPersonas.Add(persona.Clave, persona);
                    }

                }
            }
        }

        /// <summary>
        /// Recarga la lista de personas
        /// </summary>
        public void RecargarPersonas()
        {
            CargarPersonas();
        }

        /// <summary>
        /// Agrega una nueva persona a la base de datos
        /// </summary>
        public Persona AgregarPersona()
        {
            Persona persona;

            AD.EntityModel.Models.PersonaDS.Persona filaPersona = new AD.EntityModel.Models.PersonaDS.Persona();
            filaPersona.PersonaID = Guid.NewGuid();
            filaPersona.Nombre = string.Empty;
            filaPersona.Sexo = "H";
            filaPersona.EsBuscable = true;
            filaPersona.EsBuscableExternos = false;
            filaPersona.Eliminado = false;
            filaPersona.Idioma = "es";
            filaPersona.EstadoCorreccion = (short)EstadoCorreccion.NoCorreccion;
            DataWrapperPersonas.ListaPersona.Add(filaPersona);

            persona = new Persona(filaPersona, this, mLoggingService);
            mListaPersonas.Add(persona.Clave, persona);

            return persona;
        }

        /// <summary>
        /// Agrega una fila a la tabla "ConfiguracionGnossPersona" con la configuración por defecto de la persona pasada por parámetro
        /// </summary>
        /// <param name="pPersonaID">Identificador de persona</param>
        /// <returns>Fila de configuración GNOSS para persona</returns>
        public AD.EntityModel.Models.PersonaDS.ConfiguracionGnossPersona AgregarConfiguracionGnossPersona(Guid pPersonaID)
        {
            AD.EntityModel.Models.PersonaDS.ConfiguracionGnossPersona filaConfiguracionGnossPersona = new AD.EntityModel.Models.PersonaDS.ConfiguracionGnossPersona();
            filaConfiguracionGnossPersona.PersonaID = pPersonaID;
            filaConfiguracionGnossPersona.BoletinSuscripcion = (short)PeriodicidadSuscripcion.Diaria;
            filaConfiguracionGnossPersona.ComentariosRecursos = true;
            filaConfiguracionGnossPersona.InvitacionComunidad = true;
            filaConfiguracionGnossPersona.InvitacionOrganizacion = true;
            filaConfiguracionGnossPersona.SolicitudesContacto = true;
            filaConfiguracionGnossPersona.MensajesGnoss = true;
            filaConfiguracionGnossPersona.VerAmigos = false;
            filaConfiguracionGnossPersona.VerAmigosExterno = false;
            filaConfiguracionGnossPersona.VerRecursos = false;
            filaConfiguracionGnossPersona.VerRecursosExterno = false;
            filaConfiguracionGnossPersona.Persona = DataWrapperPersonas.ListaPersona.Where(persona => persona.PersonaID.Equals(pPersonaID)).ToList().FirstOrDefault();

            DataWrapperPersonas.ListaConfigGnossPersona.Add(filaConfiguracionGnossPersona);
            mEntityContext.ConfiguracionGnossPersona.Add(filaConfiguracionGnossPersona);
            return filaConfiguracionGnossPersona;
        }

        /// <summary>
        /// Crea los datos de persona libre para una persona pasada como parámetro
        /// </summary>
        /// <param name="pPersona">Persona</param>
        /// <returns>Nuevos datos de persona libre</returns>
        public DatosTrabajoPersonaLibre CrearDatosTrabajoPersonaLibre(Persona pPersona)
        {
            if (pPersona.DatosTrabajoPersonaLibre == null)
            {
                DatosTrabajoPersonaLibre nuevoPerfil = new DatosTrabajoPersonaLibre();
                nuevoPerfil.PersonaID = pPersona.Clave;
                nuevoPerfil.Persona = pPersona.FilaPersona;
                DataWrapperPersonas.ListaDatosTrabajoPersonaLibre.Add(nuevoPerfil);
                mEntityContext.DatosTrabajoPersonaLibre.Add(nuevoPerfil);
                pPersona.DatosTrabajoPersonaLibre = nuevoPerfil;
            }
            return pPersona.DatosTrabajoPersonaLibre;
        }

        /// <summary>
        /// Borra los datos de persona libre de una persona pasada como parámetro
        /// </summary>
        /// <param name="pPersona">Persona</param>
        public void BorrarDatosTrabajoPersonaLibre(Persona pPersona)
        {
            if (pPersona.DatosTrabajoPersonaLibre != null)
            {
                if (mEntityContext.Entry(pPersona.DatosTrabajoPersonaLibre).State.Equals(EntityState.Detached))
                {
                    DatosTrabajoPersonaLibre datosTrabajoPersonaLibreEliminar = mEntityContext.DatosTrabajoPersonaLibre.FirstOrDefault(persona => persona.PersonaID.Equals(pPersona.DatosTrabajoPersonaLibre.PersonaID));
                    if (datosTrabajoPersonaLibreEliminar != null)
                    {
                        mEntityContext.EliminarElemento(datosTrabajoPersonaLibreEliminar);
                    }

                }
                else
                {
                    mEntityContext.EliminarElemento(pPersona.DatosTrabajoPersonaLibre);
                }

                pPersona.DatosTrabajoPersonaLibre = null;
            }
        }

        /// <summary>
        /// Realiza el eliminado FÍSICO de una persona en el sistema (lo elimina de memoria)
        /// </summary>
        /// <param name="pElemento">Elemento para eliminar</param>
        public override void EliminarElemento(ElementoGnoss pElemento)
        {
            Persona personaBorrar = (Persona)pElemento;

            // Borrar los datos de trabajo de persona libre
            BorrarDatosTrabajoPersonaLibre(personaBorrar);

            // Borrar todos los vínculos con organizaciones
            List<Organizacion> listaOrganizacionesVinculadas = new List<Organizacion>();
            listaOrganizacionesVinculadas.AddRange(personaBorrar.ListaOrganizacionesVinculadasConLaPersona.Values);

            foreach (Organizacion organizacion in listaOrganizacionesVinculadas)
            {
                GestorOrganizaciones.DesvincularPersonaOrganizacion(organizacion, personaBorrar);
            }
            // Borrar los estados laborales relacionados con la persona

            if (personaBorrar.Usuario != null)
            {
                //Elimino ConfiguracionGnossPersona
                AD.EntityModel.Models.PersonaDS.ConfiguracionGnossPersona confPersona = DataWrapperPersonas.ListaConfigGnossPersona.FirstOrDefault(conf => conf.PersonaID.Equals(personaBorrar.Clave));
                if (confPersona != null)
                {
                    mEntityContext.EliminarElemento(confPersona);
                    DataWrapperPersonas.ListaConfigGnossPersona.Remove(confPersona);
                }

                // Eliminar los registros de AdministradorOrganizacion que cuelgan de ese usuario
                List<AdministradorOrganizacion> filasBorrar = GestorOrganizaciones.OrganizacionDW.ListaAdministradorOrganizacion.Where(item => item.UsuarioID.Equals(personaBorrar.Usuario.Clave)).ToList();
                foreach (AdministradorOrganizacion fila in filasBorrar)
                {
                    mEntityContext.EliminarElemento(fila);
                    GestorOrganizaciones.OrganizacionDW.ListaAdministradorOrganizacion.Remove(fila);
                }

                // Eliminar los registros de AdministradorProyecto que cuelgan de ese usuario
                if (GestorOrganizaciones.GestorProyectos != null)
                {
                    List<AdministradorProyecto> listaBorrar = GestorOrganizaciones.GestorProyectos.DataWrapperProyectos.ListaAdministradorProyecto.Where(adminProy => adminProy.UsuarioID.Equals(personaBorrar.Usuario.Clave)).ToList();
                    foreach (AdministradorProyecto fila in listaBorrar)
                    {
                        GestorOrganizaciones.GestorProyectos.DataWrapperProyectos.ListaAdministradorProyecto.Remove(fila);
                        mEntityContext.EliminarElemento(fila);
                    }
                }
                GestorUsuarios.GestorSuscripciones.EliminarSuscripcionAUsuario(personaBorrar.UsuarioID);
                //GestorUsuarios.GestorPeticiones.EliminarPeticionesPorUsuarioID(personaBorrar.UsuarioID);
                GestorUsuarios.GestorIdentidades.EliminarTodasIdentidades(personaBorrar);
                GestorUsuarios.GestorIdentidades.EliminarTodosPerfiles(personaBorrar);
                GestorUsuarios.EliminarElemento(personaBorrar.Usuario);
            }
            // Limpiar la persona de la lista
            ListaPersonas.Remove(personaBorrar.Clave);

            // Borrar la fila de persona
            // David: Se quita la operación conjunta para que se actualice correctamente el grid de la pantalla al borrar la fila
            OperacionConjunta = false;
            mEntityContext.Entry(personaBorrar.FilaElemento).State = EntityState.Deleted;
            OperacionConjunta = true;

            base.EliminarElemento(pElemento);
        }

        /// <summary>
        /// Realiza una eliminación lógica de una persona
        /// </summary>
        /// <param name="pPersona">Persona a eliminar lógicamente</param>
        /// <param name="pProyectosParticipaPersona">Lista de proyectos en los que participa la persona</param>
        public void EliminarLogicoElemento(Persona pPersona)
        {
            // Borrar los datos de trabajo de persona libre
            BorrarDatosTrabajoPersonaLibre(pPersona);

            // Borrar todos los vínculos con organizaciones
            List<Organizacion> listaOrganizacionesVinculadas = new List<Organizacion>();
            listaOrganizacionesVinculadas.AddRange(pPersona.ListaOrganizacionesVinculadasConLaPersona.Values);

            foreach (Organizacion organizacion in listaOrganizacionesVinculadas)
            {
                GestorOrganizaciones.DesvincularPersonaOrganizacion(organizacion, pPersona);
            }

            if (pPersona.Usuario != null)
            {
                // Eliminar los registros de AdministradorOrganizacion que cuelgan de ese usuario
                List<AdministradorOrganizacion> filasBorrar = GestorOrganizaciones.OrganizacionDW.ListaAdministradorOrganizacion.Where(item => item.UsuarioID.Equals(pPersona.Usuario.Clave)).ToList();
                foreach (AdministradorOrganizacion fila in filasBorrar)
                {
                    mEntityContext.EliminarElemento(fila);
                    GestorOrganizaciones.OrganizacionDW.ListaAdministradorOrganizacion.Remove(fila);
                }

                // Eliminar los registros de AdministradorProyecto que cuelgan de ese usuario
                if (GestorOrganizaciones.GestorProyectos != null)
                {
                    List<AdministradorProyecto> listaBorrar = GestorOrganizaciones.GestorProyectos.DataWrapperProyectos.ListaAdministradorProyecto.Where(adminProy => adminProy.UsuarioID.Equals(pPersona.Usuario.Clave)).ToList();
                    foreach (AdministradorProyecto fila in listaBorrar)//.Select("UsuarioID = '" + pPersona.Usuario.Clave + "'"))
                    {
                        GestorOrganizaciones.GestorProyectos.DataWrapperProyectos.ListaAdministradorProyecto.Remove(fila);
                        mEntityContext.EliminarElemento(fila);
                    }
                }

                //Eliminar los registros de peticiones del usuario
                if (GestorUsuarios.GestorPeticiones != null)
                {
                    GestorUsuarios.GestorPeticiones.EliminarPeticionesPorUsuarioID(pPersona.Usuario.Clave);
                }
                GestorUsuarios.GestorSuscripciones.EliminarSuscripcionAUsuario(pPersona.UsuarioID);
                GestorUsuarios.GestorIdentidades.GestorAmigos.EliminarAmigosSolicitudesGruposDePersona(pPersona);
                GestorUsuarios.GestorIdentidades.EliminarTodasIdentidades(pPersona);
                GestorUsuarios.GestorIdentidades.EliminarTodosPerfiles(pPersona);
                GestorUsuarios.EliminarElemento(pPersona.Usuario);
            }
            string apellidos, dni;
            short tipoDoc;
            apellidos = pPersona.Apellidos;
            tipoDoc = pPersona.TipoDocAcreditativo;
            dni = pPersona.ValorDocumentoAcreditativo;


            pPersona.Apellidos = apellidos;
            pPersona.TipoDocAcreditativo = tipoDoc;
            pPersona.ValorDocumentoAcreditativo = dni;

            // Marcar la fila como eliminada lógica
            OperacionConjunta = false; // David: Se quita la operación conjunta para que se actualice correctamente el grid de la pantalla
            pPersona.FilaPersona.Eliminado = true;
            mEntityContext.EliminarElemento(pPersona.FilaElemento);
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
        ~GestionPersonas()
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
                mDisposed = true;

                try
                {
                    if (pDisposing)
                    {
                        //Liberamos todos los recursos administrados que hemos añadido a esta clase
                        if (mListaPersonas != null)
                        {
                            foreach (Persona per in mListaPersonas.Values)
                            {
                                per.Dispose();
                            }
                        }
                    }
                }
                finally
                {
                    mListaPersonas = null;
                    mPaisDW = null;

                    // Llamamos al dispose de la clase base
                    base.Dispose(pDisposing);
                }
            }
        }

        #endregion

        #region Miembros de ISerializable

        /// <summary>
        /// Metodo para serializar el objeto
        /// </summary>
        /// <param name="pInfo">Datos serializados</param>
        /// <param name="pContext">Contexto de serialización</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo pInfo, StreamingContext pContext)
        {
            base.GetObjectData(pInfo, pContext);

            pInfo.AddValue("PaisDW", PaisDW);
            pInfo.AddValue("GestorOrganizaciones", GestorOrganizaciones);
            pInfo.AddValue("GestorUsuarios", GestorUsuarios);
        }

        #endregion
    }
}
