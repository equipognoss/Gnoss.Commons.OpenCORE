using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.Amigos.Model;
using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.BASE_BD.Model;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Usuarios.Model;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Amigos;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Amigos;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Amigos;
using Es.Riam.Gnoss.Logica.BASE_BD;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.Live;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Util;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Es.Riam.Gnoss.Web.Controles.Amigos
{
    public class ControladorAmigos : ControladorBase
    {
        private LoggingService mLoggingService;
        private VirtuosoAD mVirtuosoAD;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private RedisCacheWrapper mRedisCacheWrapper;
        private EntityContextBASE mEntityContextBASE;
        private IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication;

        #region Constructor

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public ControladorAmigos(LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, EntityContextBASE entityContextBASE, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, servicesUtilVirtuosoAndReplication)
        {
            mVirtuosoAD = virtuosoAD;
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mRedisCacheWrapper = redisCacheWrapper;
            mEntityContextBASE = entityContextBASE;
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Carga los amigos de una identidad 
        /// </summary>
        /// <param name="pIdentidad">Identidad a cargar los amigos</param>
        /// <param name="pCargarAmigosIdentidadOrganizacion">TRUE si se deben cargar los amigos de la organización o FALSE si se deben cargar los de la persona</param>
        /// <param name="pSoloComprobar">TRUE si solo se debe comprobar que la clave existe</param>
        public void CargarAmigos(Identidad pIdentidad, bool pEsAdministradorDeOrganizacion, bool pCargarAmigosIdentidadOrganizacion = true, bool pSoloComprobar = false)
        {
            GestionIdentidades gestorIdentidadesAmigos = null;

            AmigosCL amigosCL = new AmigosCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);

            DataWrapperIdentidad dataWrapperIdentidad = new DataWrapperIdentidad();
            DataWrapperPersona dataWrapperPersona = new DataWrapperPersona();
            DataWrapperOrganizacion organizacionDW = new DataWrapperOrganizacion();
            DataWrapperAmigos amigosDW = new DataWrapperAmigos();

            Guid identidad;
            if (pIdentidad.TrabajaConOrganizacion && pEsAdministradorDeOrganizacion && pIdentidad.IdentidadOrganizacion != null && pCargarAmigosIdentidadOrganizacion)
            {
                identidad = pIdentidad.IdentidadOrganizacion.IdentidadMyGNOSS.Clave;
            }
            else
            {
                identidad = pIdentidad.IdentidadMyGNOSS.Clave;
                pCargarAmigosIdentidadOrganizacion = false;
            }

            if (!amigosCL.ObtenerAmigos(identidad, dataWrapperIdentidad, dataWrapperPersona, organizacionDW, amigosDW, pCargarAmigosIdentidadOrganizacion, pSoloComprobar))
            {
                gestorIdentidadesAmigos = CargarAmigosDeBBDD(pIdentidad, pCargarAmigosIdentidadOrganizacion, pEsAdministradorDeOrganizacion);

                amigosCL.AgregarAmigos(identidad, gestorIdentidadesAmigos.DataWrapperIdentidad, gestorIdentidadesAmigos.GestorPersonas.DataWrapperPersonas, gestorIdentidadesAmigos.GestorOrganizaciones.OrganizacionDW, gestorIdentidadesAmigos.GestorAmigos.AmigosDW, pCargarAmigosIdentidadOrganizacion);
            }
            else
            {
                gestorIdentidadesAmigos = new GestionIdentidades(dataWrapperIdentidad, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                gestorIdentidadesAmigos.GestorPersonas = new GestionPersonas(dataWrapperPersona, mLoggingService, mEntityContext);
                gestorIdentidadesAmigos.GestorOrganizaciones = new GestionOrganizaciones(organizacionDW, mLoggingService, mEntityContext);
                gestorIdentidadesAmigos.GestorAmigos = new GestionAmigos(amigosDW, gestorIdentidadesAmigos, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            }

            if (!pSoloComprobar)
            {
                //Al hacer los merges les pasamos TRUE como 2º parametro para quedarnos los valores que nos han pasado en pIdentidad
                //en caso de que haya filas que entren en conflicto
                GestionIdentidades gestorIdentidades = pIdentidad.GestorIdentidades;
                gestorIdentidades.DataWrapperIdentidad.Merge(gestorIdentidadesAmigos.DataWrapperIdentidad);
                gestorIdentidades.RecargarHijos();
                gestorIdentidades.GestorPersonas.DataWrapperPersonas.Merge(gestorIdentidadesAmigos.GestorPersonas.DataWrapperPersonas);
                gestorIdentidades.GestorPersonas.RecargarPersonas();
                gestorIdentidades.GestorOrganizaciones.OrganizacionDW.Merge(gestorIdentidadesAmigos.GestorOrganizaciones.OrganizacionDW);
                gestorIdentidades.GestorPersonas.RecargarPersonas();
                gestorIdentidades.GestorOrganizaciones.CargarOrganizaciones();

                if (gestorIdentidades.GestorAmigos == null)
                {
                    gestorIdentidades.GestorAmigos = gestorIdentidadesAmigos.GestorAmigos;
                }
                else
                {
                    gestorIdentidades.GestorAmigos.AmigosDW.Merge(gestorIdentidadesAmigos.GestorAmigos.AmigosDW);
                    gestorIdentidades.GestorAmigos.RecargarListaGrupoAmigos();
                }
            }

            CargarAmigosEIdentidadesEnMisProyectosPrivados(pIdentidad, pCargarAmigosIdentidadOrganizacion, pSoloComprobar);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pIdentidad"></param>
        /// <param name="pCargarAmigosIdentidadOrganizacion"></param>
        /// <returns></returns>
        private GestionIdentidades CargarAmigosDeBBDD(Identidad pIdentidad, bool pCargarAmigosIdentidadOrganizacion, bool pEsAdministradorDeOrganizacion)
        {
            mEntityContext.UsarEntityCache = true;
            GestionIdentidades gestorIdentidadesAmigos = null;
            try
            {
                AmigosCN amigosCN = new AmigosCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

                DataWrapperIdentidad dataWrapperIdentidad = new DataWrapperIdentidad();
                DataWrapperPersona dataWrapperPersona = new DataWrapperPersona();
                DataWrapperOrganizacion organizacionDW = new DataWrapperOrganizacion();
                DataWrapperAmigos amigosDW = new DataWrapperAmigos();

                Guid identidad;
                if (pIdentidad.TrabajaConOrganizacion && pEsAdministradorDeOrganizacion && pCargarAmigosIdentidadOrganizacion)
                {
                    identidad = pIdentidad.IdentidadOrganizacion.IdentidadMyGNOSS.Clave;
                }
                else
                {
                    identidad = pIdentidad.IdentidadMyGNOSS.Clave;
                }

                gestorIdentidadesAmigos = new GestionIdentidades(new DataWrapperIdentidad(), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

                if (pIdentidad.TrabajaConOrganizacion)
                {
                    if (pIdentidad.IdentidadOrganizacion == null && pIdentidad.OrganizacionID.HasValue)
                    {
                        IdentidadCN IdentidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        gestorIdentidadesAmigos.DataWrapperIdentidad.Merge(IdentidadCN.ObtenerPerfilDeOrganizacion(pIdentidad.OrganizacionID.Value));
                        IdentidadCN.Dispose();

                        gestorIdentidadesAmigos.RecargarHijos();
                    }

                    if (pEsAdministradorDeOrganizacion && pCargarAmigosIdentidadOrganizacion)
                    {
                        amigosDW = amigosCN.ObtenerAmigosDeIdentidad(pIdentidad.IdentidadOrganizacion.IdentidadMyGNOSS.Clave);

                        gestorIdentidadesAmigos = new GestionIdentidades(pIdentidad.IdentidadOrganizacion.GestorIdentidades.DataWrapperIdentidad, pIdentidad.IdentidadOrganizacion.GestorIdentidades.GestorPersonas, pIdentidad.IdentidadOrganizacion.GestorIdentidades.GestorOrganizaciones, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

                    }
                    else
                    {
                        if (pIdentidad.IdentidadOrganizacion != null)
                        {
                            amigosDW = amigosCN.ObtenerAmigosOrganizacionConAccesoParaUsuario(pIdentidad.IdentidadOrganizacion.IdentidadMyGNOSS.Clave, pIdentidad.IdentidadMyGNOSS.Clave);

                        }
                        amigosDW.Merge(amigosCN.ObtenerAmigosDeIdentidad(pIdentidad.IdentidadMyGNOSS.Clave));
                    }
                }
                else
                {
                    amigosDW = amigosCN.ObtenerAmigosDeIdentidad(pIdentidad.IdentidadMyGNOSS.Clave);

                    if (pIdentidad.EsIdentidadProfesor)
                    {
                        //Obtengo los alumnos de las clases que administra este usuario
                        amigosDW.Merge(amigosCN.ObtenerAmigosDeOrganizacionesAdministradas(UsuarioActual.UsuarioID, null));
                    }
                }

                amigosCN.Dispose();

                // Obtener todas las nuevas identidades de los amigos
                List<Guid> listaIdentidades = new List<Guid>();
                foreach (AD.EntityModel.Models.IdentidadDS.Amigo filaAmigo in amigosDW.ListaAmigo)
                {
                    if (!filaAmigo.IdentidadAmigoID.Equals(pIdentidad.Clave) && !listaIdentidades.Contains(filaAmigo.IdentidadAmigoID))
                    {
                        listaIdentidades.Add(filaAmigo.IdentidadAmigoID);
                    }
                }
                //foreach (AmigosDS.SolicitudAmigoRow filaSolicitudAmigo in amigosDS.SolicitudAmigo.Rows)
                //{
                //    if (!filaSolicitudAmigo.IdentidadIDAmigo.Equals(pIdentidad.Clave) && !listaIdentidades.Contains(filaSolicitudAmigo.IdentidadIDAmigo))
                //    {
                //        listaIdentidades.Add(filaSolicitudAmigo.IdentidadIDAmigo);
                //    }
                //}
                //foreach (SolicitudContacto filaSolicitudContacto in amigosDW.ListaSolicitudContacto)
                //{
                //    if (!filaSolicitudContacto.IdentidadIDAmigo.Equals(pIdentidad.Clave) && !listaIdentidades.Contains(filaSolicitudContacto.IdentidadIDAmigo))
                //    {
                //        listaIdentidades.Add(filaSolicitudContacto.IdentidadIDAmigo);
                //    }
                //}
                foreach (AD.EntityModel.Models.IdentidadDS.PermisoAmigoOrg filaAmigoOrgPermiso in amigosDW.ListaPermisoAmigoOrg)
                {
                    //Comprobar que identidadID es la identidad de la organizacion...
                    if (!filaAmigoOrgPermiso.IdentidadOrganizacionID.Equals(pIdentidad.IdentidadOrganizacion) && !listaIdentidades.Contains(filaAmigoOrgPermiso.IdentidadUsuarioID))
                    {
                        listaIdentidades.Add(filaAmigoOrgPermiso.IdentidadUsuarioID);
                    }
                }

                //Cargamos las identidades de las clases.
                foreach (AD.EntityModel.Models.IdentidadDS.GrupoAmigos filaGrupoAmigos in amigosDW.ListaGrupoAmigos)
                {
                    if (!filaGrupoAmigos.IdentidadID.Equals(pIdentidad.Clave) && !listaIdentidades.Contains(filaGrupoAmigos.IdentidadID))
                    {
                        listaIdentidades.Add(filaGrupoAmigos.IdentidadID);
                    }
                }


                // Cargar las identidades de los amigos en el gestor
                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                dataWrapperIdentidad = identidadCN.ObtenerIdentidadesPorID(listaIdentidades, true);
                gestorIdentidadesAmigos.DataWrapperIdentidad.Merge(dataWrapperIdentidad);
                identidadCN.Dispose();
                gestorIdentidadesAmigos.RecargarHijos();

                // Cargar las personas y organizaciones
                dataWrapperPersona = new DataWrapperPersona();
                organizacionDW = new DataWrapperOrganizacion();
                UtilServicioResultados utilServicioResultados = new UtilServicioResultados(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                utilServicioResultados.ObtenerPersonasYOrgDeIdentidades(gestorIdentidadesAmigos.DataWrapperIdentidad, dataWrapperPersona, organizacionDW, true);

                //gestorIdentidadesAmigos.GestorPersonas.GestorUsuarios = new GestionUsuarios(usuarioCN.ObtenerUsuariosPorIdentidadesCargaLigera(listaIdentidades));
                gestorIdentidadesAmigos.GestorPersonas = new GestionPersonas(dataWrapperPersona, mLoggingService, mEntityContext);
                gestorIdentidadesAmigos.GestorOrganizaciones = new GestionOrganizaciones(organizacionDW, mLoggingService, mEntityContext);
                gestorIdentidadesAmigos.GestorPersonas.RecargarPersonas();
                gestorIdentidadesAmigos.GestorOrganizaciones.CargarOrganizaciones();
                if (gestorIdentidadesAmigos.GestorAmigos != null)
                {
                    gestorIdentidadesAmigos.GestorAmigos.Dispose();
                }
                gestorIdentidadesAmigos.GestorAmigos = new GestionAmigos(amigosDW, gestorIdentidadesAmigos, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                if (pIdentidad.IdentidadMyGNOSS != null)
                {
                    IdentidadCN idenCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    //GUID Identidad: gestorIdentidades.GestorAmigos.ListaPermitidosPorOrg[0]
                    //IDENTIDAD gestorIdentidades.ListaIdentidades[gestorIdentidades.GestorAmigos.listaPermitidosPorOrg[0]]
                    gestorIdentidadesAmigos.GestorAmigos.ListaPermitidosPorOrg = idenCN.ObtenerListaIdentidadesPermitidasPorOrg(pIdentidad.IdentidadMyGNOSS.Clave);
                    //Debemos recorrer la lista e ir añadiendolos poco a poco a la lista de contactos, para que asi se procesen.
                    foreach (Guid id in gestorIdentidadesAmigos.GestorAmigos.ListaPermitidosPorOrg)
                    {
                        if ((!gestorIdentidadesAmigos.GestorAmigos.ListaContactos.ContainsKey(id)) && (gestorIdentidadesAmigos.ListaIdentidades.ContainsKey(id)))
                        {
                            gestorIdentidadesAmigos.GestorAmigos.ListaContactos.Add(id, gestorIdentidadesAmigos.ListaIdentidades[id]);
                        }
                    }

                    idenCN.Dispose();
                }
                //Comentado por EF
                //if (!gestorIdentidadesAmigos.IdentidadesDS.Perfil.Columns.Contains("NombreBusqueda"))
                //{
                //    gestorIdentidadesAmigos.IdentidadesDS.Perfil.Columns.Add("NombreBusqueda");
                //}
                //foreach (IdentidadDS.PerfilRow filaPerfil in gestorIdentidadesAmigos.IdentidadesDS.Perfil.Rows)
                //{
                //    filaPerfil["NombreBusqueda"] = UtilCadenas.RemoveAccentsWithRegEx(filaPerfil.NombrePerfil);
                //}

                foreach (AD.EntityModel.Models.IdentidadDS.GrupoAmigos filaGrupoAmigos in gestorIdentidadesAmigos.GestorAmigos.AmigosDW.ListaGrupoAmigos)
                {
                    filaGrupoAmigos.NombreBusqueda = UtilCadenas.RemoveAccentsWithRegEx(filaGrupoAmigos.Nombre);
                }
            }
            finally
            {
                mEntityContext.UsarEntityCache = false;
            }

            return gestorIdentidadesAmigos;
        }


        /// <summary>
        /// Carga los amigos de una identidad y los contactos de comunidades privadas.
        /// </summary>
        /// <param name="pIdentidad">Identidad a cargar los amigos</param>
        /// <param name="pCargarAmigosIdentidadOrganizacion">TRUE si se deben cargar los amigos de la organización o FALSE si se deben cargar los de la persona</param>
        public void CargarAmigosEIdentidadesEnMisProyectosPrivados(Identidad pIdentidad, bool pCargarAmigosIdentidadOrganizacion, bool pSoloComprobar = false)
        {
            bool esAdministrador = false;
            if (pIdentidad.TrabajaConOrganizacion)
            {
                esAdministrador = pCargarAmigosIdentidadOrganizacion;
            }

            Guid identidad = pIdentidad.IdentidadMyGNOSS.Clave;
            if (pIdentidad.TrabajaConOrganizacion && pCargarAmigosIdentidadOrganizacion && esAdministrador)
            {
                identidad = pIdentidad.IdentidadOrganizacion.IdentidadMyGNOSS.Clave;
            }

            if (CargarIdentidadesDeProyectosPrivadosComoAmigos)
            {
                DataWrapperIdentidad dataWrapperIdentidad = new DataWrapperIdentidad();
                AmigosCL amigosCL = new AmigosCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                if (!amigosCL.ObtenerAmigosEIdentidadesEnMisProyectosPrivados(identidad, dataWrapperIdentidad, pCargarAmigosIdentidadOrganizacion, pSoloComprobar))
                {
                    IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

                    List<Guid> identEnMisProyPriv = identidadCN.ObtenerIdentidadesIDEnMisProyectosPrivados(identidad);

                    mEntityContext.UsarEntityCache = true;
                    try
                    {
                        // Cargar las identidades de los amigos en el gestor
                        dataWrapperIdentidad = identidadCN.ObtenerIdentidadesPorID(identEnMisProyPriv, true);
                    }
                    finally
                    {
                        mEntityContext.UsarEntityCache = false;
                    }
                    identidadCN.Dispose();

                    amigosCL.AgregarAmigosEIdentidadesEnMisProyectosPrivados(identidad, dataWrapperIdentidad, pCargarAmigosIdentidadOrganizacion);
                }
            }
        }

        /// <summary>
        /// Guarda los datos de los amigos en BD.
        /// </summary>
        /// <param name="pAmigosDS">DataSet de amigos</param>
        /// <param name="pIdentidad">Identidad a cargar los amigos</param>
        /// <param name="pCargarAmigosIdentidadOrganizacion">TRUE si se deben cargar los amigos de la organización o FALSE si se deben cargar</param>
        public void GuardarAmigos(AmigosDS pAmigosDS, Identidad pIdentidad, bool pCargarAmigosIdentidadOrganizacion, bool pEsAdministradorDeOrganizacion)
        {
            mEntityContext.SaveChanges();
            InvalidarAmigosIdentidad(pIdentidad, pCargarAmigosIdentidadOrganizacion, pEsAdministradorDeOrganizacion);
        }

        /// <summary>
        /// Invalida la caché de amigos de una identidad
        /// </summary>
        /// <param name="pIdentidad">Identidad a cargar los amigos</param>
        /// <param name="pCargarAmigosIdentidadOrganizacion">TRUE si se deben cargar los amigos de la organización o FALSE si se deben cargar</param>
        public void InvalidarAmigosIdentidad(Identidad pIdentidad, bool pCargarAmigosIdentidadOrganizacion, bool pEsAdministradorDeOrganizacion)
        {
            Guid identidad;
            if (pIdentidad.TrabajaConOrganizacion && pEsAdministradorDeOrganizacion && pCargarAmigosIdentidadOrganizacion)
            {
                identidad = pIdentidad.IdentidadOrganizacion.IdentidadMyGNOSS.Clave;
            }
            else
            {
                identidad = pIdentidad.IdentidadMyGNOSS.Clave;
            }
            InvalidarAmigosIdentidad(identidad);
        }

        /// <summary>
        /// Invalida la caché de amigos de una identidad
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        public void InvalidarAmigosIdentidad(Guid pIdentidadID)
        {
            AmigosCL amigosCL = new AmigosCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            amigosCL.InvalidarAmigos(pIdentidadID);
        }
        /// <summary>
        /// Incluye en Cache una clave para borrar la cache local de 
        /// </summary>
        /// <param name="newGuid"></param>
        private void InvalidarCacheLocalAutocompletar(Guid pGuid)
        {
            AmigosCL amigosCL = new AmigosCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            amigosCL.AgregarCacheAutocompletarInvalidar(pGuid);
        }

        /// <summary>
        /// Devuelve un gestor de identidades conteniendo un gestor de organizaciones y de personas con los miembros de la organización pasada como parámetro
        /// </summary>
        /// <param name="pOrganizacionID">ID de la organización</param>
        /// <returns></returns>
        public GestionIdentidades ObtenerGestorIdentidadesMiembrosOrganizacion(Guid pOrganizacionID)
        {
            List<Guid> listaIdentidades = new List<Guid>();
            DataWrapperIdentidad idnTermporalDW = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication).ObtenerIdentidadesDeOrganizacionYEmpleados(pOrganizacionID);

            foreach (AD.EntityModel.Models.IdentidadDS.Identidad filaIdentidad in idnTermporalDW.ListaIdentidad)
            {
                listaIdentidades.Add(filaIdentidad.IdentidadID);
            }

            DataWrapperOrganizacion orgTemporalDW = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication).ObtenerOrganizacionesPorIdentidadesCargaLigera(listaIdentidades);
            DataWrapperPersona dataWrapperPersona = new DataWrapperPersona();
            dataWrapperPersona = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication).ObtenerPersonasPorIdentidadesCargaLigera(listaIdentidades);

            return new GestionIdentidades(idnTermporalDW, new GestionPersonas(dataWrapperPersona, mLoggingService, mEntityContext), new GestionOrganizaciones(orgTemporalDW, mLoggingService, mEntityContext), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
        }

        /// <summary>
        /// Proceso para convertir en contactos a los miembros de una organización y a la propia organización
        /// </summary>
        /// <param name="pOrganizacionID">ID de la organización</param>
        /// <param name="pIdentidadID">Identidad con el perfil persona+org del miembro nuevo</param>
        /// <param name="pGestorIdentidades">Gestor de identidades de la página</param>
        public List<string> AgregarContactosOrganizacion(Guid pOrganizacionID, Guid pIdentidadID, GestionIdentidades pGestorIdentidades)
        {
            Guid idIdentidadOrganizacion = Guid.Empty; // Identidad MyGNOSS de la organizacion
            List<string> listaAmigosdeOrganizacion = new List<string>();

            #region Carga del ID de la organización

            // Obtener la identidad MyGNOSS de la organización del gestor pasado como parametro  
            List<AD.EntityModel.Models.IdentidadDS.PerfilOrganizacion> filasPerfilOrg = pGestorIdentidades.DataWrapperIdentidad.ListaPerfilOrganizacion.Where(perfilOrg=>perfilOrg.OrganizacionID.Equals(pOrganizacionID)).ToList();
            if (filasPerfilOrg.Count > 0)
            {//"ProyectoID = '" + ProyectoAD.MetaProyecto + "' AND PerfilID = '" + filasPerfilOrg[0].PerfilID + "'"
                List<AD.EntityModel.Models.IdentidadDS.Identidad> filasIdentidad = pGestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(identidad=>identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && identidad.PerfilID.Equals(filasPerfilOrg.First().PerfilID)).ToList();

                if (filasIdentidad.Count > 0)
                {
                    idIdentidadOrganizacion = filasIdentidad.First().IdentidadID;
                }
            }

            if (idIdentidadOrganizacion.Equals(Guid.Empty))
            {
                // En caso de que no estén, se cargan de la base de datos su identidad y la de sus empleados
                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                pGestorIdentidades.DataWrapperIdentidad.Merge(identidadCN.ObtenerIdentidadesDeOrganizacionYEmpleados(pOrganizacionID));
                identidadCN.Dispose();

                pGestorIdentidades.RecargarHijos();
 
                filasPerfilOrg = pGestorIdentidades.DataWrapperIdentidad.ListaPerfilOrganizacion.Where(perfilOrg => perfilOrg.OrganizacionID.Equals(pOrganizacionID)).ToList();
                if (filasPerfilOrg.Count > 0)
                {
                    List<AD.EntityModel.Models.IdentidadDS.Identidad> filasIdentidad = pGestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(identidad => identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && identidad.PerfilID.Equals(filasPerfilOrg[0].PerfilID)).ToList();

                    if (filasIdentidad.Count > 0)
                    {
                        idIdentidadOrganizacion = filasIdentidad[0].IdentidadID;
                    }
                }
            }

            #endregion

            //Se añaden más abajo, es necesario hacer esto aquí¿?

            // Crear un gestor de amigos donde almacenar todos los cambios
            AmigosCN amigosCN = new AmigosCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperAmigos dataWrapperAmigos = amigosCN.CargarAmigosCompleto(pIdentidadID);
            GestionAmigos gestorAmigos = new GestionAmigos(dataWrapperAmigos, pGestorIdentidades, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

            #region Bug2519

            // Si el usuario ha sido dado de alta como miembro de la organización a partir de un contacto con otra identidad,
            // en este caso se borra el antiguo contacto y se deja solo el del nuevo miembro
            if (pGestorIdentidades.ListaIdentidades.ContainsKey(pIdentidadID))
            {
                Identidad identidadUsuario = pGestorIdentidades.ListaIdentidades[pIdentidadID];
                Identidad identidadOrganizacion = pGestorIdentidades.ListaIdentidades[idIdentidadOrganizacion];

                // Obtener por si no estuvieran cargado, todos los perfiles de la persona
                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                pGestorIdentidades.DataWrapperIdentidad.Merge(identidadCN.ObtenerPerfilesDePersona(identidadUsuario.PerfilUsuario.PersonaID.Value, false, identidadUsuario.Clave));
                identidadCN.Dispose();

                pGestorIdentidades.RecargarHijos();

                // Comprobar si con alguna de las identidades del perfil del usuario (exceptuando la pasada como parámetro)
                // ya era amigo de la organización para en ese caso borrar los registro de contacto

                foreach (AD.EntityModel.Models.IdentidadDS.Perfil filaPerfil in pGestorIdentidades.DataWrapperIdentidad.ListaPerfil.Where(perfil=> perfil.PersonaID.HasValue && perfil.PersonaID.Value.Equals(identidadUsuario.PerfilUsuario.PersonaID.Value)).ToList())
                {//"PersonaID = '" + identidadUsuario.PerfilUsuario.PersonaID.Value + "'"
                    foreach (AD.EntityModel.Models.IdentidadDS.Identidad filaIdentidad in filaPerfil.Identidad)
                    {
                        if (filaIdentidad.IdentidadID != pIdentidadID && gestorAmigos.AmigosDW.ListaAmigo.Where(item => item.IdentidadID.Equals(idIdentidadOrganizacion) && item.IdentidadAmigoID.Equals(filaIdentidad.IdentidadID)).ToList().Count > 0)
                        {
                            gestorAmigos.CambiarContactoDeIdentidad(idIdentidadOrganizacion, filaIdentidad.IdentidadID, pIdentidadID);
                            gestorAmigos.EliminarAmigos(pGestorIdentidades.ListaIdentidades[filaIdentidad.IdentidadID], identidadOrganizacion);
                        }
                    }
                }
            }

            #endregion

            // Darse permiso entre todos los miembros de la organización
            // Si hay más de 1000 usuarios, la Web se queda mucho tiempo haciendo esto y no le contesta al usuario. Hay que hacerlo offline
            if (pGestorIdentidades.DataWrapperIdentidad.ListaPerfilPersonaOrg.Count < 1000)
            {

                foreach (AD.EntityModel.Models.IdentidadDS.PerfilPersonaOrg filaPerfilPersonaOrg in pGestorIdentidades.DataWrapperIdentidad.ListaPerfilPersonaOrg.Where(item => item.OrganizacionID.Equals(pOrganizacionID)))
                {
                    Guid idIdentidadCompaniero = Guid.Empty;

                    List<AD.EntityModel.Models.IdentidadDS.Identidad> filasIdentidad = pGestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(identidad => identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && identidad.PerfilID.Equals(filaPerfilPersonaOrg.PerfilID)).ToList();

                    if (filasIdentidad.Count > 0)
                    {
                        idIdentidadCompaniero = filasIdentidad[0].IdentidadID;
                    }

                    listaAmigosdeOrganizacion.Add(idIdentidadCompaniero.ToString());

                    if (!amigosCN.EsAmigoDeIdentidad(pIdentidadID, idIdentidadCompaniero) && pIdentidadID != idIdentidadCompaniero)
                    {
                        if (pIdentidadID != idIdentidadCompaniero)
                        {
                            gestorAmigos.AgregarPermisoContactoOrganizacion(idIdentidadOrganizacion, idIdentidadCompaniero, pIdentidadID);
                        }
                    }

                    if (!amigosCN.EsAmigoDeIdentidad(idIdentidadCompaniero, pIdentidadID))
                    {
                        if (pIdentidadID != idIdentidadCompaniero)
                        {
                            gestorAmigos.AgregarPermisoContactoOrganizacion(idIdentidadOrganizacion, pIdentidadID, idIdentidadCompaniero);
                        }
                    }

                    //Limpiamos la cache de cada miembro de la organizacion para que se muestre el nuevo miembro de la organizacion.
                    InvalidarAmigosIdentidad(idIdentidadCompaniero);
                }
            }

            //Limpiamos la cache de los contactos para mostrar el nuevo miembro de la organizacion.
            InvalidarAmigosIdentidad(idIdentidadOrganizacion);

            //TODO: Alberto
            //Si es una clase y agregamos la organización, estamos agregando la clase del profesor como contacto.
            //Controlar los permisos que se les dan a los usuarios al agregar al profesor... (Hablar con Alvaro)

            #region Agregamos al profesor y la organizacion como contacos

            //Si es una clase, hay que hacer contactos al profesor y al alumno:
            if (new OrganizacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication).ComprobarOrganizacionEsClase(pOrganizacionID))
            {
                UsuarioCN usuCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                List<UsuarioIdentidadPersona> listaUsuarioIdentidadPersona = usuCN.ObtenerAdministradoresNombreApellidosPorOrganizacion(pOrganizacionID);

                IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                foreach (UsuarioIdentidadPersona filaUsu in listaUsuarioIdentidadPersona)
                {
                    PersonaCN perCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    DataWrapperPersona dataWrapperPersona = perCN.ObtenerPersonaPorUsuario(filaUsu.UsuarioID, false, false);
                    perCN.Dispose();
                    Guid identidadIDProyecto = identCN.ObtenerIdentidadIDDePersonaEnProyecto(ProyectoSeleccionado.Clave, dataWrapperPersona.ListaPersona.First().PersonaID)[0];
                    DataWrapperIdentidad identDW = identCN.ObtenerPerfilesDePersona(dataWrapperPersona.ListaPersona.First().PersonaID, true, identidadIDProyecto);
                    //"Tipo =" + (int)TiposIdentidad.Profesor + " AND ProyectoID = '" + ProyectoAD.MetaProyecto + "'"
                    Guid identidadID = identDW.ListaIdentidad.Where(identidad => identidad.Tipo.Equals((int)TiposIdentidad.Profesor) && identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto)).Select(filaIdent => filaIdent.IdentidadID).FirstOrDefault();
                    if (identidadID.Equals(Guid.Empty))
                    {
                        //Añadimos al profesor a la lista de amigos de la organización
                        listaAmigosdeOrganizacion.Add(identidadID.ToString());

                        //Se le da permisos de contacto organización al alumno y al profesor y también se les añade como amigos.
                        if (!amigosCN.EsAmigoDeIdentidad(pIdentidadID, identidadID) && pIdentidadID != identidadID)
                        {
                            gestorAmigos.CrearFilaAmigo(identidadID, pIdentidadID);
                            gestorAmigos.AgregarPermisoContactoOrganizacion(idIdentidadOrganizacion, identidadID, pIdentidadID);
                        }

                        if (!amigosCN.EsAmigoDeIdentidad(identidadID, pIdentidadID) && pIdentidadID != identidadID)
                        {
                            gestorAmigos.CrearFilaAmigo(pIdentidadID, identidadID);
                            gestorAmigos.AgregarPermisoContactoOrganizacion(idIdentidadOrganizacion, pIdentidadID, identidadID);
                        }

                        //Limpiamos la cache del profesor.
                        InvalidarAmigosIdentidad(identidadID);
                    }
                    perCN.Dispose();
                }

                usuCN.Dispose();
                identCN.Dispose();
            }
            else
            {
                // Agregar como contacto del usuario a la organización darle permisos al usuario para poder verla
                if (!amigosCN.EsAmigoDeIdentidad(pIdentidadID, idIdentidadOrganizacion))
                {
                    gestorAmigos.CrearFilaAmigo(pIdentidadID, idIdentidadOrganizacion);
                    gestorAmigos.AgregarPermisoContactoOrganizacion(idIdentidadOrganizacion, idIdentidadOrganizacion, pIdentidadID);
                    listaAmigosdeOrganizacion.Add(idIdentidadOrganizacion.ToString());
                }

                // Agregar como contacto de la organización al propio usuario
                if (!amigosCN.EsAmigoDeIdentidad(idIdentidadOrganizacion, pIdentidadID))
                {
                    gestorAmigos.CrearFilaAmigo(idIdentidadOrganizacion, pIdentidadID);
                }

                UsuarioCN usuCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                List<UsuarioIdentidadPersona> listaUsuarioIdentidadPersona = usuCN.ObtenerAdministradoresNombreApellidosPorOrganizacion(pOrganizacionID);

                IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                foreach (UsuarioIdentidadPersona filaUsu in listaUsuarioIdentidadPersona)
                {
                    PersonaCN perCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    DataWrapperPersona dataWrapperPersona = perCN.ObtenerPersonaPorUsuario(filaUsu.UsuarioID, false, false);
                    Guid identidadIDProyecto = identCN.ObtenerIdentidadIDDePersonaEnProyecto(ProyectoSeleccionado.Clave, dataWrapperPersona.ListaPersona.First().PersonaID)[0];
                    DataWrapperIdentidad identDW = identCN.ObtenerPerfilesDePersona(dataWrapperPersona.ListaPersona.First().PersonaID, true, identidadIDProyecto);

                    //Dentro de identDS está la tabla con los perfiles de los administradores

                    foreach (AD.EntityModel.Models.IdentidadDS.Perfil filaPerfil in identDW.ListaPerfil.Where(perfil => perfil.OrganizacionID.Equals(pOrganizacionID) && perfil.PersonaID.Equals(dataWrapperPersona.ListaPersona.First().PersonaID)).ToList())
                    {
                        AD.EntityModel.Models.IdentidadDS.Identidad filaIdent = identDW.ListaIdentidad.FirstOrDefault(identidad => identidad.PerfilID.Equals(filaPerfil.PerfilID) && identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto));
                        if(filaIdent != null)
                        {
                            //Añadimos al administrador de la organizacion a la lista de amigos
                            listaAmigosdeOrganizacion.Add(filaIdent.IdentidadID.ToString());


                            if (!amigosCN.EsAmigoDeIdentidad(pIdentidadID, filaIdent.IdentidadID) && pIdentidadID != filaIdent.IdentidadID)
                            {
                                gestorAmigos.AgregarPermisoContactoOrganizacion(idIdentidadOrganizacion, filaIdent.IdentidadID, pIdentidadID);
                            }

                            if (!amigosCN.EsAmigoDeIdentidad(filaIdent.IdentidadID, pIdentidadID) && pIdentidadID != filaIdent.IdentidadID)
                            {
                                gestorAmigos.AgregarPermisoContactoOrganizacion(idIdentidadOrganizacion, pIdentidadID, filaIdent.IdentidadID);
                            }

                            //Limpiamos la cache del profesor.
                            InvalidarAmigosIdentidad(filaIdent.IdentidadID);
                        }
                    }
                    perCN.Dispose();

                }

                usuCN.Dispose();
                usuCN.Dispose();
                identCN.Dispose();
            }

            #endregion

            amigosCN.Dispose();

            //Agregar nuevo cache para borrar las caches locales del servicio autocompletar
            InvalidarCacheLocalAutocompletar(Guid.NewGuid());



            // Pasar los cambios realizados en la función al gestor de amigos de la página
            if (pGestorIdentidades.GestorAmigos == null)
            {
                pGestorIdentidades.GestorAmigos = gestorAmigos;
            }
            else
            {
                pGestorIdentidades.GestorAmigos.AmigosDW.Merge(gestorAmigos.AmigosDW);
            }

            //Quitamos la propia identidad por si se ha colado:
            if (listaAmigosdeOrganizacion.Contains(pIdentidadID.ToString()))
            {
                listaAmigosdeOrganizacion.Remove(pIdentidadID.ToString());
            }

            return listaAmigosdeOrganizacion;
        }

        

        /// <summary>
        /// Notifica al live que determinados usuario tienen invitaciones nuevas.
        /// </summary>
        /// <param name="pPerfilID">Perfil destinatario</param>
        public void AgregarNotificacionInvitacionNuevaAPerfil(Guid pPerfilID)
        {
            List<Guid> listaPerfiles = new List<Guid>();
            listaPerfiles.Add(pPerfilID);
            AgregarNotificacionInvitacionNuevaAPerfiles(listaPerfiles);
        }

        /// <summary>
        /// Notifica al live que determinados usuario tienen invitaciones nuevas.
        /// </summary>
        /// <param name="pPerfiles">Perfiles destinatarios</param>
        public void AgregarNotificacionInvitacionNuevaAPerfiles(List<Guid> pPerfiles)
        {
            LiveCN liveCN = new LiveCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            foreach (Guid perfilID in pPerfiles)
            {
                liveCN.AumentarContadorNuevasInvitaciones(perfilID);
            }
            liveCN.Dispose();
        }

        /// <summary>
        /// Notifica al modelo base que se ha generado una invitación para un usuario.
        /// </summary>
        /// <param name="pInvitacionPorIdentidad">Lista de invitaciones con la identidad de destino</param>
        /// <param name="pPrioridadBase">Prioridad de las filas introducidas en el modelo base</param>
        public void AgregarInvitacionModeloBase(Dictionary<Guid, Guid> pInvitacionPorIdentidad, PrioridadBase pPrioridadBase)
        {
            BaseInvitacionesDS baseInvitacionesDS = new BaseInvitacionesDS();

            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            int id = proyCL.ObtenerTablaBaseProyectoIDProyectoPorID(ProyectoAD.MetaProyecto);
            proyCL.Dispose();

            //if (!string.IsNullOrEmpty(pFicheroConfiguracionBD))
            //{
            //    ProyectoCN proyCN = new ProyectoCN(pFicheroConfiguracionBD, false);
            //    id = proyCN.ObtenerTablaBaseProyectoIDProyectoPorID(pProyectoID);
            //    proyCN.Dispose();
            //}
            //else
            //{
            //    ProyectoCL proyCL = new ProyectoCL();
            //    id = proyCL.ObtenerTablaBaseProyectoIDProyectoPorID(pProyectoID);
            //}

            #region Marcar agregado

            foreach (Guid invitacionID in pInvitacionPorIdentidad.Keys)
            {
                BaseInvitacionesDS.ColaTagsInvitacionesRow filaColaTagsCom = baseInvitacionesDS.ColaTagsInvitaciones.NewColaTagsInvitacionesRow();
                filaColaTagsCom.Estado = (short)EstadosColaTags.EnEspera;
                filaColaTagsCom.FechaPuestaEnCola = DateTime.Now;
                filaColaTagsCom.TablaBaseProyectoID = id;
                filaColaTagsCom.Tags = Constantes.ID_INVITACION + invitacionID.ToString() + Constantes.ID_INVITACION + Constantes.ID_INVITACION_IDDESTINO + pInvitacionPorIdentidad[invitacionID].ToString() + Constantes.ID_INVITACION_IDDESTINO;
                filaColaTagsCom.Tipo = 0;
                filaColaTagsCom.Prioridad = (short)pPrioridadBase;

                baseInvitacionesDS.ColaTagsInvitaciones.AddColaTagsInvitacionesRow(filaColaTagsCom);
            }

            #endregion


            BaseComunidadCN brComCN = new BaseComunidadCN(mEntityContext, mLoggingService, mEntityContextBASE, mConfigService, mServicesUtilVirtuosoAndReplication);//, -1
            brComCN.InsertarFilasEnRabbit("ColaTagsInvitaciones", baseInvitacionesDS);
            brComCN.Dispose();

            baseInvitacionesDS.Dispose();


        }

        /// <summary>
        /// Notifica al live que un perfil a aceptado o rechazado una invitación.
        /// </summary>
        /// <param name="pPerfilID">PerfilID</param>
        public void AgregarNotificacionInvitacionLeidaAPerfil(Guid pPerfilID)
        {
            AgregarNotificacionInvitacionLeidaAPerfil(pPerfilID, 1);
        }

        /// <summary>
        /// Notifica al live que un perfil a aceptado o rechazado una invitación.
        /// </summary>
        /// <param name="pPerfilID">PerfilID</param>
        public void AgregarNotificacionInvitacionLeidaAPerfil(Guid pPerfilID, int pNumDisminucion)
        {
            LiveCN liveCN = new LiveCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            for (int i = 0; i < pNumDisminucion; i++)
            {
                liveCN.DisminuirContadoInvitacionesLeidas(pPerfilID);
            }
            liveCN.Dispose();
        }

        /// <summary>
        /// Resetea el contador de nuevas invitaciones de un perfil.
        /// </summary>
        /// <param name="pPerfilID">ID del perfil</param>
        public void ResetearContadorNuevasInvitaciones(Guid pPerfilID)
        {
            LiveCN liveCN = new LiveCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            liveCN.ResetearContadorNuevasInvitaciones(pPerfilID);
            liveCN.Dispose();
        }

        #endregion
    }
}