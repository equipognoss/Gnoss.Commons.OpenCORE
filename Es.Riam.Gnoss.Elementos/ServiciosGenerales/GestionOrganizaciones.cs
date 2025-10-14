using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.OrganizacionDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.Tesauro;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Interfaces;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Es.Riam.Gnoss.Elementos.ServiciosGenerales
{
    /// <summary>
    /// Gestor de organizaciones
    /// </summary>
    [Serializable]
    public class GestionOrganizaciones : GestionGnoss, ISerializable, IDisposable
    {
        #region Miembros

        /// <summary>
        /// Gestor documental
        /// </summary>
        private GestorDocumental mGestorDocumental;

        /// <summary>
        /// Gestor del tesauro
        /// </summary>
        private GestionTesauro mGestorTesauro;

        /// <summary>
        /// Gestor de personas
        /// </summary>
        private GestionPersonas mGestorPersonas;

        /// <summary>
        /// Gestor de proyectos
        /// </summary>
        private GestionProyecto mGestorProyectos;

        /// <summary>
        /// Gestor de identidades
        /// </summary>
        private GestionIdentidades mGestorIdentidades;

        /// <summary>
        /// DataSet de país
        /// </summary>
        private DataWrapperPais mPaisDW;

        /// <summary>
        /// Lista de organizaciones
        /// </summary>
        private SortedList<Guid, Organizacion> mListaOrganizaciones = null;

        private LoggingService mLoggingService;

        private EntityContext mEntityContext;
        #endregion

        #region Constructor

        public GestionOrganizaciones() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pOrganizacionDW">Dataset de organización</param>
        public GestionOrganizaciones(DataWrapperOrganizacion pOrganizacionDW, LoggingService loggingService, EntityContext entityContext)
            : base(pOrganizacionDW)
        {
            mEntityContext = entityContext;
            mLoggingService = loggingService;
        }

        

        /// <summary>
        /// Constructor para la deseralización
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serialización</param>
        protected GestionOrganizaciones(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            mPaisDW = (DataWrapperPais)info.GetValue("PaisDW", typeof(DataWrapperPais));
            mGestorDocumental = (GestorDocumental)info.GetValue("GestorDocumental", typeof(GestorDocumental));
            mGestorIdentidades = (GestionIdentidades)info.GetValue("GestorIdentidades", typeof(GestionIdentidades));
            mGestorPersonas = (GestionPersonas)info.GetValue("GestorPersonas", typeof(GestionPersonas));
            mGestorProyectos = (GestionProyecto)info.GetValue("GestorProyectos", typeof(GestionProyecto));
            mGestorTesauro = (GestionTesauro)info.GetValue("GestorTesauro", typeof(GestionTesauro));
        }



        #endregion

        #region Propiedades

        /// <summary>
        /// Elementos hijos (sus organizaciones)
        /// </summary>
        public override List<IElementoGnoss> Hijos
        {
            get 
            {
                List<IElementoGnoss> listaHijos = new List<IElementoGnoss>();
               
                foreach (Organizacion organizacion in ListaOrganizaciones.Values)
                {
                    listaHijos.Add((IElementoGnoss)organizacion);
                }
                return listaHijos; 
            }
        }

        /// <summary>
        /// Obtiene o establece el data set de paises
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
        /// Obtiene la lista de organizaciones
        /// </summary>
        public SortedList<Guid, Organizacion> ListaOrganizaciones
        {
            get
            {
                if (mListaOrganizaciones == null)
                {
                    CargarOrganizaciones();
                }
                return mListaOrganizaciones;
            }
        }
        
        /// <summary>
        /// Obtiene o establece el dataset de organizaciones
        /// </summary>
        public DataWrapperOrganizacion OrganizacionDW
        {
            get
            {
                return (DataWrapperOrganizacion) DataWrapper;
            }
           set { DataWrapper = value; }
        }

        /// <summary>
        /// Obtiene o establece el dataset de personas
        /// </summary>
        public GestionPersonas GestorPersonas
        {
            get
            {
                return mGestorPersonas;
            }
            set
            {
                mGestorPersonas = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el gestor de identidades
        /// </summary>
        public GestionIdentidades GestorIdentidades
        {
            get
            {
                return mGestorIdentidades;
            }
            set
            {
                mGestorIdentidades = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el gestor de proyectos
        /// </summary>
        public GestionProyecto GestorProyectos
        {
            get
            {
                return mGestorProyectos;
            }
            set
            {
                mGestorProyectos = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el gestor documental
        /// </summary>
        public GestorDocumental GestorDocumental
        {
            get
            {
                return mGestorDocumental;
            }
            set
            {
                this.mGestorDocumental = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el gestor de tesauro
        /// </summary>
        public GestionTesauro GestorTesauro
        {
            get
            {
                return mGestorTesauro;
            }
            set
            {
                this.mGestorTesauro = value;
            }
        }

        #endregion

        #region Métodos generales

        /// <summary>
        /// Carga las organizaciones
        /// </summary>
        public void CargarOrganizaciones()
        {
            mListaOrganizaciones = new SortedList<Guid, Organizacion>();
           
            foreach (AD.EntityModel.Models.OrganizacionDS.Organizacion filaOrganizacion in OrganizacionDW.ListaOrganizacion)
            {
                Organizacion organizacion = null;

                if (filaOrganizacion.OrganizacionGnoss != null)
                {
                    organizacion = new OrganizacionGnoss(filaOrganizacion, filaOrganizacion.OrganizacionGnoss, this, mLoggingService, mEntityContext);
                    
                }
                else
                {
                    AD.EntityModel.Models.OrganizacionDS.OrganizacionEmpresa filaEmpresa = filaOrganizacion.OrganizacionEmpresa;
                    if(filaEmpresa != null)
                    {
                        organizacion = new OrganizacionEmpresa(filaOrganizacion, filaOrganizacion.OrganizacionEmpresa, this);
                    }
                    else
                    {
                        organizacion = new Organizacion(filaOrganizacion, this);
                    }
                }
                if(organizacion != null)
                {
                    if (!mListaOrganizaciones.ContainsKey(organizacion.Clave))
                    {
                        mListaOrganizaciones.Add(organizacion.Clave, organizacion);
                    }
                }
                
            }
        }

        /// <summary>
        /// Recarga la lista de personas
        /// </summary>
        public void RecargarOrganizciones()
        {
            CargarOrganizaciones();
        }

        ///// <summary>
        ///// Quita las organizaciones que aparezcan en el "pOrganizacionesAQuitar" del "pTodasOrganizacionDS"
        ///// </summary>
        ///// <param name="pTodasOrganizacionDS">Dataset con todas las organizaciones</param>
        ///// <param name="pOrganizacionesAQuitar">Dataset con las organizaciones que hay que quitar</param>
        ///// <returns>OrganizacionDS </returns>
        //public OrganizacionDS QuitarRepetidas(OrganizacionDS pTodasOrganizacionDS, OrganizacionDS pOrganizacionesAQuitar)
        //{
        //    if (pOrganizacionesAQuitar.OrganizacionGnoss.Count > 0)
        //    {
        //        foreach (OrganizacionGnoss filaOrganizacion in pOrganizacionesAQuitar.OrganizacionGnoss.Rows)
        //        {
        //            if (filaOrganizacion.RowState != DataRowState.Deleted && filaOrganizacion.RowState != DataRowState.Detached)
        //            {
        //                OrganizacionDS.OrganizacionRow org = pTodasOrganizacionDS.Organizacion.FindByOrganizacionID(filaOrganizacion.OrganizacionID);
        //                if (org != null)
        //                {
        //                    pTodasOrganizacionDS.Organizacion.RemoveOrganizacionRow(org);
        //                }
        //            }
        //        }
        //    }
        //    return pTodasOrganizacionDS;
        //}

        /// <summary>
        /// Crea un vínculo entre una organización y una persona pasados como parámetros
        /// </summary>
        /// <param name="pOrganizacion">Organización</param>
        /// <param name="pPersona">Persona</param>
        /// <returns>DatosTrabajoPersonaOrganizacion</returns>
        public DatosTrabajoPersonaOrganizacion VincularPersonaOrganizacion(Organizacion pOrganizacion, Persona pPersona)
        {
            DatosTrabajoPersonaOrganizacion nuevoVinculo = VincularPersonaOrganizacion(pOrganizacion, pPersona.Clave);

            // Actualizar las listas de vinculaciones entre los objetos
            if (!pPersona.ListaOrganizacionesVinculadasConLaPersona.ContainsKey(pOrganizacion.Clave))
            {
                pPersona.ListaOrganizacionesVinculadasConLaPersona.Add(pOrganizacion.Clave, pOrganizacion);
            }

            if (!pPersona.ListaDatosTrabajoPersonaOrganizacion.ContainsKey(pOrganizacion.Clave))
            {
                pPersona.ListaDatosTrabajoPersonaOrganizacion.Add(pOrganizacion.Clave, nuevoVinculo);
            }

            if (!pOrganizacion.ListaPersonasVinculadasConLaOrganizacion.ContainsKey(pPersona.Clave))
            {
                pOrganizacion.ListaPersonasVinculadasConLaOrganizacion.Add(pPersona.Clave, pPersona);
            }
            return nuevoVinculo;
        }

        /// <summary>
        /// Crea un vínculo entre una organización y una persona pasados como parámetros
        /// </summary>
        /// <param name="pOrganizacion">Organización</param>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <returns>DatosTrabajoPersonaOrganizacion</returns>
        public DatosTrabajoPersonaOrganizacion VincularPersonaOrganizacion(Organizacion pOrganizacion, Guid pPersonaID)
        {
            PersonaVinculoOrganizacion filaOrgPersona = new PersonaVinculoOrganizacion();

            // Crear las filas de datos y copiar algunos datos de la información de la organizacion
            if (!OrganizacionDW.ListaPersonaVinculoOrganizacion.Any(item => item.OrganizacionID.Equals(pOrganizacion.FilaOrganizacion.OrganizacionID) && item.PersonaID.Equals(pPersonaID)))
            {
                filaOrgPersona.OrganizacionID = pOrganizacion.FilaOrganizacion.OrganizacionID;
                filaOrgPersona.PersonaID = pPersonaID;
                filaOrgPersona.DireccionTrabajo = pOrganizacion.FilaOrganizacion.Direccion;
                filaOrgPersona.LocalidadTrabajo = pOrganizacion.FilaOrganizacion.Localidad;
                filaOrgPersona.CPTrabajo = pOrganizacion.FilaOrganizacion.CP;

                if (pOrganizacion.FilaOrganizacion.ProvinciaID.HasValue)
                {
                    filaOrgPersona.ProvinciaTrabajoID = pOrganizacion.FilaOrganizacion.ProvinciaID;
                }

                if (pOrganizacion.FilaOrganizacion.PaisID.HasValue)
                {
                    filaOrgPersona.PaisTrabajoID = pOrganizacion.FilaOrganizacion.PaisID;
                }
                filaOrgPersona.ProvinciaTrabajo = pOrganizacion.FilaOrganizacion.Provincia;
                filaOrgPersona.TelefonoTrabajo = pOrganizacion.FilaOrganizacion.Telefono;
                filaOrgPersona.FechaVinculacion = DateTime.Now;
                filaOrgPersona.CoordenadasFoto = "";
                filaOrgPersona.UsarFotoPersonal = false;

                OrganizacionDW.ListaPersonaVinculoOrganizacion.Add(filaOrgPersona);
                mEntityContext.PersonaVinculoOrganizacion.Add(filaOrgPersona);
                
            }
            else
            {
                filaOrgPersona = OrganizacionDW.ListaPersonaVinculoOrganizacion.Where(item => item.OrganizacionID.Equals(pOrganizacion.FilaOrganizacion.OrganizacionID) && item.PersonaID.Equals(pPersonaID)).FirstOrDefault();
            }
            DatosTrabajoPersonaOrganizacion nuevoVinculo = new DatosTrabajoPersonaOrganizacion(filaOrgPersona, pOrganizacion.GestorOrganizaciones);

            return nuevoVinculo;
        }

      

        /// <summary>
        /// Borra los vínculos entre una persona y una organización
        /// </summary>
        /// <param name="pOrganizacion">Organización</param>
        /// <param name="pPersona">Persona</param>
        public void DesvincularPersonaOrganizacion(Organizacion pOrganizacion, Persona pPersona)
        {
            //Eliminar la fila del vínculo
            if (pPersona.ListaDatosTrabajoPersonaOrganizacion.ContainsKey(pOrganizacion.Clave))
            {
                var item  = pPersona.ListaDatosTrabajoPersonaOrganizacion[pOrganizacion.Clave].FilaVinculo;
                mEntityContext.EliminarElemento(item);
            }

            //Actualizar las listas
            if (pPersona.ListaOrganizacionesVinculadasConLaPersona.ContainsKey(pOrganizacion.Clave))
            {
                pPersona.ListaOrganizacionesVinculadasConLaPersona.Remove(pOrganizacion.Clave);
            }

            if (pPersona.ListaDatosTrabajoPersonaOrganizacion.ContainsKey(pOrganizacion.Clave))
            {
                pPersona.ListaDatosTrabajoPersonaOrganizacion.Remove(pOrganizacion.Clave);
            }

            if (pOrganizacion.ListaPersonasVinculadasConLaOrganizacion.ContainsKey(pOrganizacion.Clave))
            {
                pOrganizacion.ListaPersonasVinculadasConLaOrganizacion.Remove(pOrganizacion.Clave);
            }
        }

        /// <summary>
        /// Agrega a una organización como participante de un proyecto
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización que va a participar en el proyecto</param>
        /// <param name="pOrganizacionProyID">Identificador de la organización creadora del proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto en el que va a participar la organización</param>
        /// <param name="pIdentidadID">Identidad con la que participará la organización en el proyecto</param>
        /// <returns></returns>
        public OrganizacionParticipaProy AgregarOrganizacionAProyecto(Guid pOrganizacionID, Guid pOrganizacionProyID, Guid pProyectoID, Guid pIdentidadID)
        {
            OrganizacionParticipaProy filaProy = new OrganizacionParticipaProy();
            filaProy.EstaBloqueada = false;
            filaProy.FechaInicio = DateTime.Now;
            filaProy.IdentidadID = pIdentidadID;
            filaProy.OrganizacionID = pOrganizacionID;
            filaProy.OrganizacionProyectoID = pOrganizacionProyID;
            filaProy.ProyectoID = pProyectoID;
            filaProy.RegistroAutomatico = 0;

            OrganizacionDW.ListaOrganizacionParticipaProy.Add(filaProy);
            mEntityContext.OrganizacionParticipaProy.Add(filaProy);

            return filaProy;
        }
        
        /// <summary>
        /// Agrega un nueva organizacion a la lista de organizaciones del gestor y a las tablas del OrganizacionDS
        /// </summary>
        /// <returns>Objeto organización nuevo</returns>
        public Organizacion AgregarOrganizacion()
        {
            Es.Riam.Gnoss.AD.EntityModel.Models.OrganizacionDS.Organizacion filaOrganizacion = new Es.Riam.Gnoss.AD.EntityModel.Models.OrganizacionDS.Organizacion();
            
            filaOrganizacion.OrganizacionID = Guid.NewGuid();
            filaOrganizacion.Nombre = string.Empty;
            filaOrganizacion.NombreCorto = string.Empty;
            //BUG 3818
            //filaOrganizacion.TipoOrganizacion = (short)TiposOrganizacion.Otros;
            filaOrganizacion.EsBuscable = false;
            filaOrganizacion.EsBuscableExternos = false;
            filaOrganizacion.ModoPersonal = true;
            filaOrganizacion.Eliminada = false;
            //BUG 3818
            //filaOrganizacion.FechaCreacion = DateTime.Now;
            //filaOrganizacion.SectorOrganizacion = (short)SectoresOrganizacion.Otros;
            
            OrganizacionDW.ListaOrganizacion.Add(filaOrganizacion);
            mEntityContext.Organizacion.Add(filaOrganizacion);
            Organizacion nueva = new Organizacion(filaOrganizacion, this);

            if (!ListaOrganizaciones.ContainsKey(nueva.Clave))
            {
                ListaOrganizaciones.Add(nueva.Clave, nueva);
            }
            nueva.Padre = null;

            return nueva;
        }


        /// <summary>
        /// Añade un administrador a una organización
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        /// <param name="pGestorUsuarios">Gestor de usuarios</param>
        public void AgregarAdministradorDeOrganizacion(Guid pUsuarioID, Guid pOrganizacionID, GestionUsuarios pGestorUsuarios)
        {
            if (OrganizacionDW.ListaAdministradorOrganizacion.Where(item => item.UsuarioID.Equals(pUsuarioID) && item.OrganizacionID.Equals(pOrganizacionID) && item.Tipo.Equals((short) TipoAdministradoresOrganizacion.Administrador)).ToList().FirstOrDefault() == null)
            {
                //Añado fila "AdministradorOrganizacion" tipo Administrador
                AdministradorOrganizacion filaAdminOrg = new AdministradorOrganizacion();
                filaAdminOrg.UsuarioID = pUsuarioID;
                filaAdminOrg.OrganizacionID = pOrganizacionID;
                filaAdminOrg.Tipo = (short)TipoAdministradoresOrganizacion.Administrador;
                this.OrganizacionDW.ListaAdministradorOrganizacion.Add(filaAdminOrg);
                mEntityContext.AdministradorOrganizacion.Add(filaAdminOrg);
                if (pGestorUsuarios.DataWrapperUsuario.ListaOrganizacionRolUsuario.FirstOrDefault(orgRolUs=>orgRolUs.UsuarioID.Equals(pUsuarioID) && orgRolUs.OrganizacionID.Equals(pOrganizacionID)) != null)
                {
                    //Existe la fila de permisos del usurio-organizacion, le actualizo los permisos para que sea admnistrador
                    AD.EntityModel.Models.UsuarioDS.OrganizacionRolUsuario filaOrganizacionRolUsuario = pGestorUsuarios.DataWrapperUsuario.ListaOrganizacionRolUsuario.FirstOrDefault(orgRolUs => orgRolUs.UsuarioID.Equals(pUsuarioID) && orgRolUs.OrganizacionID.Equals(pOrganizacionID));

                    //Le doy todos los permisos
                    filaOrganizacionRolUsuario.RolPermitido = UsuarioAD.FilaPermisosAdministrador;
                    //No le deniego ninguno
                    filaOrganizacionRolUsuario.RolDenegado = UsuarioAD.FilaPermisosSinDefinir;
                }
                else
                {
                    //No Existe la fila de permisos del usurio-organizacion, la agrego y le pongo los permisos de administrador
                    AD.EntityModel.Models.UsuarioDS.OrganizacionRolUsuario filaOrganizacionRolUsuario = new AD.EntityModel.Models.UsuarioDS.OrganizacionRolUsuario();
                    filaOrganizacionRolUsuario.UsuarioID = pUsuarioID;
                    filaOrganizacionRolUsuario.OrganizacionID = pOrganizacionID;
                    //Le doy todos los permisos
                    filaOrganizacionRolUsuario.RolPermitido = UsuarioAD.FilaPermisosAdministrador;
                    //No le deniego ninguno
                    filaOrganizacionRolUsuario.RolDenegado = UsuarioAD.FilaPermisosSinDefinir;

                    pGestorUsuarios.DataWrapperUsuario.ListaOrganizacionRolUsuario.Add(filaOrganizacionRolUsuario);
                    mEntityContext.OrganizacionRolUsuario.Add(filaOrganizacionRolUsuario);
                }
            }
        }

        /// <summary>
        /// Añade un editor para una organización
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        /// <param name="pGestorUsuarios">Gestor de usuarios</param>
        public void AgregarEditorDeOrganizacion(Guid pUsuarioID, Guid pOrganizacionID, GestionUsuarios pGestorUsuarios)
        {
            if (OrganizacionDW.ListaAdministradorOrganizacion.Where(item => item.UsuarioID.Equals(pUsuarioID) && item.OrganizacionID.Equals(pOrganizacionID) && item.Tipo.Equals((short)TipoAdministradoresOrganizacion.Editor)).ToList().FirstOrDefault() == null)
            {
                //Añado fila "AdministradorOrganizacion" tipo Editor
                AdministradorOrganizacion filaAdminOrg = new AdministradorOrganizacion();
                filaAdminOrg.UsuarioID = pUsuarioID;
                filaAdminOrg.OrganizacionID = pOrganizacionID;
                filaAdminOrg.Tipo = (short)TipoAdministradoresOrganizacion.Editor;
                this.OrganizacionDW.ListaAdministradorOrganizacion.Add(filaAdminOrg);
                mEntityContext.AdministradorOrganizacion.Add(filaAdminOrg);
                if (pGestorUsuarios.DataWrapperUsuario.ListaOrganizacionRolUsuario.Where(item => item.UsuarioID.Equals(pUsuarioID) && item.OrganizacionID.Equals(pOrganizacionID)) != null)
                {
                    //Existe la fila de permisos del usurio-organizacion, le actualizo los permisos para que sea administrador
                    OrganizacionRolUsuario filaOrganizacionRolUsuario = pGestorUsuarios.DataWrapperUsuario.ListaOrganizacionRolUsuario.Where(item => item.UsuarioID.Equals(pUsuarioID) && item.OrganizacionID.Equals(pOrganizacionID)).FirstOrDefault();

                    string RolPermitido = filaOrganizacionRolUsuario.RolPermitido;
                    string RolDenegado;

                    if (filaOrganizacionRolUsuario.RolDenegado == null)
                    {
                        RolDenegado = UsuarioAD.FilaPermisosSinDefinir;
                    }
                    else
                    {
                        RolDenegado = filaOrganizacionRolUsuario.RolDenegado;
                    }

                    //Si no tiene el permiso de Editor permitido
                    if ((System.Convert.ToUInt64(RolPermitido, 16) & (ulong)Capacidad.Organizacion.CapacidadesPropiedades.EditarOrganizacion) == 0)
                    {
                        RolPermitido = ((System.Convert.ToUInt64(RolPermitido, 16) | (ulong)Capacidad.Organizacion.CapacidadesPropiedades.EditarOrganizacion)).ToString("X16");
                    }

                    //Si tiene el permiso de Editor denegado
                    if ((System.Convert.ToUInt64(RolDenegado, 16) & (ulong)Capacidad.Organizacion.CapacidadesPropiedades.EditarOrganizacion) != 0)
                    {
                        RolDenegado = ((System.Convert.ToUInt64(RolDenegado, 16) ^ (ulong)Capacidad.Organizacion.CapacidadesPropiedades.EditarOrganizacion)).ToString("X16");
                    }

                    if (RolPermitido != filaOrganizacionRolUsuario.RolPermitido)
                    {
                        filaOrganizacionRolUsuario.RolPermitido = RolPermitido;
                    }

                    if (filaOrganizacionRolUsuario.RolDenegado == null)
                    {
                        filaOrganizacionRolUsuario.RolDenegado = RolDenegado;
                    }
                    else
                    {
                        filaOrganizacionRolUsuario.RolDenegado = RolDenegado;
                    }
                }
                else
                {
                    //No Existe la fila de permisos del usurio-organizacion, la agrego y le pongo los permisos de administrador
                    OrganizacionRolUsuario filaOrganizacionRolUsuario = new OrganizacionRolUsuario();
                    filaOrganizacionRolUsuario.UsuarioID = pUsuarioID;
                    filaOrganizacionRolUsuario.OrganizacionID = pOrganizacionID;
                    filaOrganizacionRolUsuario.RolPermitido = UsuarioAD.FilaPermisosSinDefinir;
                    filaOrganizacionRolUsuario.RolDenegado = UsuarioAD.FilaPermisosSinDefinir;
                    string RolPermitido = filaOrganizacionRolUsuario.RolPermitido;
                    string RolDenegado = filaOrganizacionRolUsuario.RolDenegado;
                    
                    //Si no tiene el permiso de Editor permitido
                    if ((Convert.ToUInt64(RolPermitido, 16) & (ulong)Capacidad.Organizacion.CapacidadesPropiedades.EditarOrganizacion) == 0)
                    {
                        RolPermitido = ((Convert.ToUInt64(RolPermitido, 16) | (ulong)Capacidad.Organizacion.CapacidadesPropiedades.EditarOrganizacion)).ToString("X16");
                    }

                    //Si tiene el permiso de Editor denegado
                    if ((Convert.ToUInt64(RolDenegado, 16) & (ulong)Capacidad.Organizacion.CapacidadesPropiedades.EditarOrganizacion) != 0)
                    {
                        RolDenegado = ((Convert.ToUInt64(RolDenegado, 16) ^ (ulong)Capacidad.Organizacion.CapacidadesPropiedades.EditarOrganizacion)).ToString("X16");
                    }

                    if (RolPermitido != filaOrganizacionRolUsuario.RolPermitido)
                    {
                        filaOrganizacionRolUsuario.RolPermitido = RolPermitido;
                    }

                    if (filaOrganizacionRolUsuario.RolDenegado == null)
                    {
                        filaOrganizacionRolUsuario.RolDenegado = RolDenegado;
                    }
                    else
                    {
                        filaOrganizacionRolUsuario.RolDenegado = RolDenegado;
                    }

                    mEntityContext.OrganizacionRolUsuario.Add(filaOrganizacionRolUsuario);
                    pGestorUsuarios.DataWrapperUsuario.ListaOrganizacionRolUsuario.Add(filaOrganizacionRolUsuario);
                }
            }
        }

        /// <summary>
        /// Añade un comentarista para una organización
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        /// <param name="pGestorUsuarios">Gestor de usuarios</param>
        public void AgregarComentaristaDeOrganizacion(Guid pUsuarioID, Guid pOrganizacionID, GestionUsuarios pGestorUsuarios)
        {
            if (OrganizacionDW.ListaAdministradorOrganizacion.Where(item => item.UsuarioID.Equals(pUsuarioID) && item.OrganizacionID.Equals(pOrganizacionID) && item.Tipo.Equals((short)TipoAdministradoresOrganizacion.Comentarista)).ToList().FirstOrDefault() == null)
            {
                //Añado fila "AdministradorOrganizacion" tipo Comentarista
                AdministradorOrganizacion filaAdminOrg = new AdministradorOrganizacion();
                filaAdminOrg.UsuarioID = pUsuarioID;
                filaAdminOrg.OrganizacionID = pOrganizacionID;
                filaAdminOrg.Tipo = (short)TipoAdministradoresOrganizacion.Comentarista;
                this.OrganizacionDW.ListaAdministradorOrganizacion.Add(filaAdminOrg);
                mEntityContext.AdministradorOrganizacion.Add(filaAdminOrg);
                if (pGestorUsuarios.DataWrapperUsuario.ListaOrganizacionRolUsuario.Where(item => item.UsuarioID.Equals(pUsuarioID) && item.OrganizacionID.Equals(pOrganizacionID)).FirstOrDefault() != null)
                {
                    //Existe la fila de permisos del usurio-organizacion, le actualizo los permisos para que sea administrador
                    OrganizacionRolUsuario filaOrganizacionRolUsuario = pGestorUsuarios.DataWrapperUsuario.ListaOrganizacionRolUsuario.Where(item => item.UsuarioID.Equals(pUsuarioID) && item.OrganizacionID.Equals(pOrganizacionID)).FirstOrDefault();

                    string RolPermitido = filaOrganizacionRolUsuario.RolPermitido;
                    string RolDenegado;

                    if (filaOrganizacionRolUsuario.RolDenegado == null)
                    {
                        RolDenegado = UsuarioAD.FilaPermisosSinDefinir;
                    }
                    else
                    {
                        RolDenegado = filaOrganizacionRolUsuario.RolDenegado;
                    }

                    //Si no tiene el permiso de Comentarista permitido
                    if ((System.Convert.ToUInt64(RolPermitido, 16) & (ulong)Capacidad.Organizacion.CapacidadesComentarios.EditarComentarios) == 0)
                        RolPermitido = ((System.Convert.ToUInt64(RolPermitido, 16) | (ulong)Capacidad.Organizacion.CapacidadesComentarios.EditarComentarios)).ToString("X16");

                    //Si tiene el permiso de Editor denegado
                    if ((System.Convert.ToUInt64(RolDenegado, 16) & (ulong)Capacidad.Organizacion.CapacidadesComentarios.EditarComentarios) != 0)
                    {
                        RolDenegado = ((System.Convert.ToUInt64(RolDenegado, 16) ^ (ulong)Capacidad.Organizacion.CapacidadesComentarios.EditarComentarios)).ToString("X16");
                    }

                    if (RolPermitido != filaOrganizacionRolUsuario.RolPermitido)
                    {
                        filaOrganizacionRolUsuario.RolPermitido = RolPermitido;
                    }

                    if (filaOrganizacionRolUsuario.RolDenegado == null)
                    {
                        filaOrganizacionRolUsuario.RolDenegado = RolDenegado;
                    }
                    else
                    {
                        filaOrganizacionRolUsuario.RolDenegado = RolDenegado;
                    }
                }
                else
                {
                    //No Existe la fila de permisos del usurio-organizacion, la agrego y le pongo los permisos de comentarista
                    OrganizacionRolUsuario filaOrganizacionRolUsuario = new OrganizacionRolUsuario();
                    filaOrganizacionRolUsuario.UsuarioID = pUsuarioID;
                    filaOrganizacionRolUsuario.OrganizacionID = pOrganizacionID;
                    filaOrganizacionRolUsuario.RolPermitido = UsuarioAD.FilaPermisosSinDefinir;
                    filaOrganizacionRolUsuario.RolDenegado = UsuarioAD.FilaPermisosSinDefinir;
                    string RolPermitido = filaOrganizacionRolUsuario.RolPermitido;
                    string RolDenegado = filaOrganizacionRolUsuario.RolDenegado;
                   
                    //Si no tiene el permiso de comentarista permitido
                    if ((System.Convert.ToUInt64(RolPermitido, 16) & (ulong)Capacidad.Organizacion.CapacidadesComentarios.EditarComentarios) == 0)
                    {
                        RolPermitido = ((System.Convert.ToUInt64(RolPermitido, 16) | (ulong)Capacidad.Organizacion.CapacidadesComentarios.EditarComentarios)).ToString("X16");
                    }

                    //Si tiene el permiso de comentarista denegado
                    if ((System.Convert.ToUInt64(RolDenegado, 16) & (ulong)Capacidad.Organizacion.CapacidadesComentarios.EditarComentarios) != 0)
                    {
                        RolDenegado = ((System.Convert.ToUInt64(RolDenegado, 16) ^ (ulong)Capacidad.Organizacion.CapacidadesComentarios.EditarComentarios)).ToString("X16");
                    }

                    if (RolPermitido != filaOrganizacionRolUsuario.RolPermitido)
                    {
                        filaOrganizacionRolUsuario.RolPermitido = RolPermitido;
                    }

                    if (filaOrganizacionRolUsuario.RolDenegado == null)
                    {
                        filaOrganizacionRolUsuario.RolDenegado = RolDenegado;
                    }
                    else
                    {
                        filaOrganizacionRolUsuario.RolDenegado = RolDenegado;
                    }
                    mEntityContext.OrganizacionRolUsuario.Add(filaOrganizacionRolUsuario);
                    pGestorUsuarios.DataWrapperUsuario.ListaOrganizacionRolUsuario.Add(filaOrganizacionRolUsuario);
                }
            }
        }

        /// <summary>
        /// Establece a una persona de la organizacion como visible cuando se vea el perfil de la organizacion
        /// </summary>
        /// <param name="pPersonaID">Identificador de usuario</param>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        /// <param name="pOrden">Orden de aparicion del nuevo</param>
        public void AgregarPersonaVisible(Guid pPersonaID, Guid pOrganizacionID, int pOrden)
        {
            if (OrganizacionDW.ListaPersonaVisibleEnOrg.Where(item => item.PersonaID.Equals(pPersonaID) && item.OrganizacionID.Equals(pOrganizacionID)).ToList().FirstOrDefault() == null)
            {
                //Añado fila "PersonaVisibleEnOrg" 
                PersonaVisibleEnOrg filaPersonaVisibleEnOrg = new PersonaVisibleEnOrg();
                filaPersonaVisibleEnOrg.PersonaID = pPersonaID;
                filaPersonaVisibleEnOrg.OrganizacionID = pOrganizacionID;
                filaPersonaVisibleEnOrg.Orden = pOrden;
                this.OrganizacionDW.ListaPersonaVisibleEnOrg.Add(filaPersonaVisibleEnOrg);
            }
        }

        /// <summary>
        /// Elimina a una persona de la organizacion como visible cuando se vea el perfil de la organizacion
        /// </summary>
        /// <param name="pPersonaID">Identificador de usuario</param>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        /// <param name="pOrden">Orden de aparicion de la persona a eliminar su visibilidad</param>
        public void EliminarPersonaVisible(Guid pPersonaID, Guid pOrganizacionID, int pOrden)
        {
            if (OrganizacionDW.ListaPersonaVisibleEnOrg.Where(item => item.PersonaID.Equals(pPersonaID) && item.OrganizacionID.Equals(pOrganizacionID)).ToList().FirstOrDefault() == null)
            {
                //Elimino fila "PersonaVisibleEnOrg" 
               List<PersonaVisibleEnOrg> res =  OrganizacionDW.ListaPersonaVisibleEnOrg.Where(item => item.PersonaID.Equals(pPersonaID) && item.OrganizacionID.Equals(pOrganizacionID)).ToList();
                foreach (var item in res)
                {
                    mEntityContext.EliminarElemento(item);
                    OrganizacionDW.ListaPersonaVisibleEnOrg.Remove(item);
                }
                
                //Recalculo el orden de los superiores
                foreach (PersonaVisibleEnOrg filaPersonaVisibleEnOrg in OrganizacionDW.ListaPersonaVisibleEnOrg)
                {
                    if (filaPersonaVisibleEnOrg.Orden > pOrden)
                    {
                        filaPersonaVisibleEnOrg.Orden = filaPersonaVisibleEnOrg.Orden - 1;
                    }
                }
            }
        }

        /// <summary>
        /// Sube el orden a una persona de la organizacion como visible cuando se vea el perfil de la organizacion
        /// </summary>
        /// <param name="pPersonaID">Identificador de usuario</param>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        /// <param name="pOrden">Orden de aparicion de la persona a eliminar su visibilidad</param>
        public void SubirPersonaVisible(Guid pPersonaID, Guid pOrganizacionID, int pOrden)
        {
            if (OrganizacionDW.ListaPersonaVisibleEnOrg.Where(item => item.PersonaID.Equals(pPersonaID) && item.OrganizacionID.Equals(pOrganizacionID)).ToList().FirstOrDefault() != null)
            {
                //Se lo bajo a la que era mi superior
                PersonaVisibleEnOrg filaSuperior = (PersonaVisibleEnOrg)this.OrganizacionDW.ListaPersonaVisibleEnOrg.Where(item => item.Orden.Equals(pOrden + 1)).FirstOrDefault();
                filaSuperior.Orden = filaSuperior.Orden - 1;

                //Me subo el orden a mi
                PersonaVisibleEnOrg filaMia = this.OrganizacionDW.ListaPersonaVisibleEnOrg.Where(item => item.PersonaID.Equals(pPersonaID) && item.OrganizacionID.Equals(pOrganizacionID)).ToList().FirstOrDefault();
                filaMia.Orden = filaMia.Orden + 1;
            }
        }

        /// <summary>
        /// Baja el orden a una persona de la organizacion como visible cuando se vea el perfil de la organizacion
        /// </summary>
        /// <param name="pPersonaID">Identificador de usuario</param>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        /// <param name="pOrden">Orden de aparicion de la persona a eliminar su visibilidad</param>
        public void BajarPersonaVisible(Guid pPersonaID, Guid pOrganizacionID, int pOrden)
        {
            if (this.OrganizacionDW.ListaPersonaVisibleEnOrg.Where(item => item.PersonaID.Equals(pPersonaID) && item.OrganizacionID.Equals(pOrganizacionID)).ToList().FirstOrDefault() != null)
            {
                //Se lo subo a la que era mi inferior
                PersonaVisibleEnOrg filaInferior = (PersonaVisibleEnOrg)this.OrganizacionDW.ListaPersonaVisibleEnOrg.Where(item => item.Orden.Equals(pOrden - 1)).FirstOrDefault();
                filaInferior.Orden = filaInferior.Orden + 1;

                //Me bajo el orden a mi
                PersonaVisibleEnOrg filaMia = this.OrganizacionDW.ListaPersonaVisibleEnOrg.Where(item => item.PersonaID.Equals(pPersonaID) && item.OrganizacionID.Equals(pOrganizacionID)).ToList().FirstOrDefault();
                filaMia.Orden = filaMia.Orden - 1;
            }
        }

        /// <summary>
        /// Elimina a un usuario como administrador de una organización
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        /// <param name="pGestorUsuarios">Gestor de usuarios</param>
        public void EliminarAdministradorDeOrganizacion(Guid pUsuarioID, Guid pOrganizacionID, GestionUsuarios pGestorUsuarios)
        {
            if (OrganizacionDW.ListaAdministradorOrganizacion.Where(item => item.UsuarioID.Equals(pUsuarioID) && item.OrganizacionID.Equals(pOrganizacionID) && item.Tipo.Equals((short) TipoAdministradoresOrganizacion.Administrador)).ToList().FirstOrDefault() != null)
            {
                //Elimino fila "AdministradorOrganizacion" tipo Administrador
                List<AdministradorOrganizacion> resultado = OrganizacionDW.ListaAdministradorOrganizacion.Where(item => item.UsuarioID.Equals(pUsuarioID) && item.OrganizacionID.Equals(pOrganizacionID) && item.Tipo.Equals((short)TipoAdministradoresOrganizacion.Administrador)).ToList();

                foreach (var item in resultado)
                {
                    mEntityContext.EliminarElemento(item);
                    OrganizacionDW.ListaAdministradorOrganizacion.Remove(item);
                }
                
                if (pGestorUsuarios.DataWrapperUsuario.ListaOrganizacionRolUsuario.Where(item => item.UsuarioID.Equals(pUsuarioID) && item.OrganizacionID.Equals(pOrganizacionID)).FirstOrDefault() != null)
                {
                    //Existe la fila de permisos del usurio-organizacion, le actualizo los permisos para que no sea admnistrador
                    OrganizacionRolUsuario filaOrganizacionRolUsuario = pGestorUsuarios.DataWrapperUsuario.ListaOrganizacionRolUsuario.Where(item => item.UsuarioID.Equals(pUsuarioID) && item.OrganizacionID.Equals(pOrganizacionID)).FirstOrDefault();

                    //Le quito todos los permisos
                    filaOrganizacionRolUsuario.RolPermitido = UsuarioAD.FilaPermisosSinDefinir;
                    //No le deniego ninguno
                    filaOrganizacionRolUsuario.RolDenegado = UsuarioAD.FilaPermisosSinDefinir;
                }
            }
        }

        /// <summary>
        /// Elimina a un usuario como editor de una organización
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        /// <param name="pGestorUsuarios">Gestor de usuarios</param>
        public void EliminarEditorDeOrganizacion(Guid pUsuarioID, Guid pOrganizacionID, GestionUsuarios pGestorUsuarios)
        {
            if (OrganizacionDW.ListaAdministradorOrganizacion.Where(item => item.UsuarioID.Equals(pUsuarioID) && item.OrganizacionID.Equals(pOrganizacionID) && item.Tipo.Equals((short)TipoAdministradoresOrganizacion.Editor)).ToList().FirstOrDefault() != null)
            {
                //Elimino fila "AdministradorOrganizacion" tipo Administrador
                List<AdministradorOrganizacion> resultado = OrganizacionDW.ListaAdministradorOrganizacion.Where(item => item.UsuarioID.Equals(pUsuarioID) && item.OrganizacionID.Equals(pOrganizacionID) && item.Tipo.Equals((short) TipoAdministradoresOrganizacion.Editor)).ToList();

                foreach (var item in resultado)
                {
                    mEntityContext.EliminarElemento(item);
                    OrganizacionDW.ListaAdministradorOrganizacion.Remove(item);
                }
                
                //Les actualizo los permisos a cada usuario respecto CapacidadOrganizacion.EditarOrganizacion
                OrganizacionRolUsuario filaOrganizacionRolUsuario = pGestorUsuarios.DataWrapperUsuario.ListaOrganizacionRolUsuario.Where(item => item.UsuarioID.Equals(pUsuarioID) && item.OrganizacionID.Equals(pOrganizacionID)).FirstOrDefault();
                string RolPermitido = filaOrganizacionRolUsuario.RolPermitido;
                string RolDenegado;
                
                if (filaOrganizacionRolUsuario.RolDenegado == null)
                {
                    RolDenegado = UsuarioAD.FilaPermisosSinDefinir;
                }
                else
                {
                    RolDenegado = filaOrganizacionRolUsuario.RolDenegado;
                }

                //Si tiene el permiso permitido
                if ((System.Convert.ToUInt64(RolPermitido, 16) & (ulong)Capacidad.Organizacion.CapacidadesPropiedades.EditarOrganizacion) != 0)
                {
                    RolPermitido = ((System.Convert.ToUInt64(RolPermitido, 16) ^ (ulong)Capacidad.Organizacion.CapacidadesPropiedades.EditarOrganizacion)).ToString("X16");
                }

                //Si no tiene el permiso denegado
                if ((System.Convert.ToUInt64(RolDenegado, 16) & (ulong)Capacidad.Organizacion.CapacidadesPropiedades.EditarOrganizacion) == 0)
                {
                    RolDenegado = ((System.Convert.ToUInt64(RolDenegado, 16) | (ulong)Capacidad.Organizacion.CapacidadesPropiedades.EditarOrganizacion)).ToString("X16");
                }

                if (RolPermitido != filaOrganizacionRolUsuario.RolPermitido)
                {
                    filaOrganizacionRolUsuario.RolPermitido = RolPermitido;
                }

                if (filaOrganizacionRolUsuario.RolDenegado == null)
                {
                    filaOrganizacionRolUsuario.RolDenegado = RolDenegado;
                }
                else
                {
                    filaOrganizacionRolUsuario.RolDenegado = RolDenegado;
                }
            }
        }

        /// <summary>
        /// Elimina a un miembro de organizacion como comentarista
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        /// <param name="pGestorUsuarios">Gestor de usuarios</param>
        public void EliminarComentaristaDeOrganizacion(Guid pUsuarioID, Guid pOrganizacionID, GestionUsuarios pGestorUsuarios)
        {
            if (OrganizacionDW.ListaAdministradorOrganizacion.Where(item => item.UsuarioID.Equals(pUsuarioID) && item.OrganizacionID.Equals(pOrganizacionID) && item.Tipo.Equals((short)TipoAdministradoresOrganizacion.Comentarista)).ToList().FirstOrDefault() != null)
            {
                //Elimino fila "AdministradorOrganizacion" tipo Comentarista
                List<AdministradorOrganizacion> resultado = OrganizacionDW.ListaAdministradorOrganizacion.Where(item => item.UsuarioID.Equals(pUsuarioID) && item.OrganizacionID.Equals(pOrganizacionID) && item.Tipo.Equals((short)TipoAdministradoresOrganizacion.Comentarista)).ToList();

                foreach (var item in resultado)
                {
                    mEntityContext.EliminarElemento(item);
                    OrganizacionDW.ListaAdministradorOrganizacion.Remove(item);
                }
                
                //Les actualizo los permisos a cada usuario respecto CapacidadesComentarios.EditarComentarios
                OrganizacionRolUsuario filaOrganizacionRolUsuario = pGestorUsuarios.DataWrapperUsuario.ListaOrganizacionRolUsuario.Where(item => item.UsuarioID.Equals(pUsuarioID) && item.OrganizacionID.Equals(pOrganizacionID)).FirstOrDefault();
                string RolPermitido = filaOrganizacionRolUsuario.RolPermitido;
                string RolDenegado;
               
                if (filaOrganizacionRolUsuario.RolDenegado == null)
                {
                    RolDenegado = UsuarioAD.FilaPermisosSinDefinir;
                }
                else
                {
                    RolDenegado = filaOrganizacionRolUsuario.RolDenegado;
                }

                //Si tiene el permiso permitido
                if ((System.Convert.ToUInt64(RolPermitido, 16) & (ulong)Capacidad.Organizacion.CapacidadesComentarios.EditarComentarios) != 0)
                {
                    RolPermitido = ((System.Convert.ToUInt64(RolPermitido, 16) ^ (ulong)Capacidad.Organizacion.CapacidadesComentarios.EditarComentarios)).ToString("X16");
                }

                //Si no tiene el permiso denegado
                if ((System.Convert.ToUInt64(RolDenegado, 16) & (ulong)Capacidad.Organizacion.CapacidadesComentarios.EditarComentarios) == 0)
                {
                    RolDenegado = ((System.Convert.ToUInt64(RolDenegado, 16) | (ulong)Capacidad.Organizacion.CapacidadesComentarios.EditarComentarios)).ToString("X16");
                }

                if (RolPermitido != filaOrganizacionRolUsuario.RolPermitido)
                {
                    filaOrganizacionRolUsuario.RolPermitido = RolPermitido;
                }

                if (filaOrganizacionRolUsuario.RolDenegado == null)
                {
                    filaOrganizacionRolUsuario.RolDenegado = RolDenegado;
                }
                else
                {
                    filaOrganizacionRolUsuario.RolDenegado = RolDenegado;
                }
            }
        }

        /// <summary>
        /// Elimina si existia un registro del OrganizacionDS de la tabla "OrganizacionParticipaProy"
        /// </summary>
        /// <param name="pOrganizacionAEliminarID">Clave de Organizacion a eliminar del proyecto</param>
        /// <param name="pOrganizacionPadreDelProyecto">Clave Organizacion padre del proyecto</param>
        /// <param name="pProyectoID">Clave del proyecto</param>
        public void EliminarOrganizacionDeProyecto(Guid pOrganizacionAEliminarID, Guid pOrganizacionPadreDelProyecto, Guid pProyectoID)
        {
            if (OrganizacionDW.ListaOrganizacionParticipaProy.Where(item => item.OrganizacionID.Equals(pOrganizacionAEliminarID) && item.OrganizacionProyectoID.Equals(pOrganizacionPadreDelProyecto) && item.ProyectoID.Equals(pProyectoID)).ToList().FirstOrDefault() != null)
            {
                List<OrganizacionParticipaProy> resultado = OrganizacionDW.ListaOrganizacionParticipaProy.Where(item => item.OrganizacionID.Equals(pOrganizacionAEliminarID) && item.OrganizacionProyectoID.Equals(pOrganizacionPadreDelProyecto) && item.ProyectoID.Equals(pProyectoID)).ToList();

                foreach (var item in resultado)
                {
                    mEntityContext.EliminarElemento(item);
                    OrganizacionDW.ListaOrganizacionParticipaProy.Remove(item);
                }
            }
        }

        /// <summary>
        /// Modifica los IDs superiores de un elemento para relacionarlo con uno superior
        /// </summary>
        /// <param name="pPegado">Elemento pegado</param>
        /// <param name="pPadre">Padre</param>
        protected override void ModificarIdCortar(ElementoGnoss pPegado, ElementoGnoss pPadre)
        {
            // David: Si se pega un proyecto sobre una organización, el proyecto superior vale NULO
            if (pPegado is Proyecto && pPadre is OrganizacionGnoss)
            {
                //((Proyecto)pPegado).FilaProyecto.SetProyectoSuperiorIDNull();
                ((Proyecto)pPegado).FilaProyecto.ProyectoSuperiorID = null;
            }

            // Actualizar las colecciones de hijos del padre
            if (pPegado.Padre is Proyecto)
            {
                ((Proyecto)pPegado.Padre).ListaSubProyectos.Remove((Proyecto)pPegado);
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
        ~GestionOrganizaciones()
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
                disposed = true;
                try
                {
                    if (disposing)
                    {
                        //Libero todos los recursos administrados que he añadido a esta clase
                        if (mListaOrganizaciones != null)
                        {
                            foreach (Organizacion org in mListaOrganizaciones.Values)
                            {
                                org.Dispose();
                            }
                        }
                    }
                    //Libero todos los recursos nativos sin administrar que he añadido a esta clase

                    //Libero las variables grandes
                    mListaOrganizaciones = null;
                    mPaisDW = null;
                    mGestorPersonas = null;
                    mGestorProyectos = null;
                    mGestorIdentidades = null;
                }
                finally
                {
                    // Llamo al dispose de la clase base
                    base.Dispose(disposing);
                }
            }
        }

        #endregion

        #region Miembros de ISerializable

        /// <summary>
        /// Metodo para serializar el objeto
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serialización</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("PaisDW", PaisDW);
            info.AddValue("GestorDocumental", GestorDocumental);
            info.AddValue("GestorIdentidades", GestorIdentidades);
            info.AddValue("GestorPersonas", GestorPersonas);
            info.AddValue("GestorProyectos", GestorProyectos);
            info.AddValue("GestorTesauro", GestorTesauro);
        }

        #endregion
    }
}
