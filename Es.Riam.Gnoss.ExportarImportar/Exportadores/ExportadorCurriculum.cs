using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Elementos;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.ExportarImportar;
using Es.Riam.Gnoss.ExportarImportar.ElementosOntologia;
using Es.Riam.Gnoss.ExportarImportar.Exportadores;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Interfaces;
using Es.Riam.Semantica.OWL;
using Es.Riam.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Es.Riam.Metagnoss.ExportarImportar.Exportadores
{
    /// <summary>
    /// 
    /// </summary>
    public class ExportadorCurriculum : ExportadorElementoGnoss, IDisposable
    {

        #region Miembros

        /// <summary>
        /// Ruta de la ontología del CV.
        /// </summary>
        public static string RUTA_URL_ONTOLOGIA_CV = "";

        /// <summary>
        /// URL de la ontología del CV.
        /// </summary>
        public static string URL_ONTOLOGIA_CV = "";

        /// <summary>
        /// URL de intragnoss.
        /// </summary>
        public static string URLINTRAGNOSS = "";

        private GestionProyecto mGestorProyectos;
        private LoggingService mLoggingService;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private RedisCacheWrapper mRedisCacheWrapper;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #endregion

        #region Constructores

        /// <summary>
        /// Crea un nuevo exportador de Curriculum
        /// </summary>
        /// <param name="pOntologia">Ontología</param>
        /// <param name="pGestor">Gestor de libro</param>
        public ExportadorCurriculum(Ontologia pOntologia, GestionGnoss pGestor, string pIdiomaUsuario, LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, UtilSemCms utilSemCms, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, VirtuosoAD virtuosoAd, ILogger<ExportadorCurriculum> logger, ILoggerFactory loggerFactory)
            : base(pOntologia, pIdiomaUsuario, loggingService, entityContext, configService, redisCacheWrapper, utilSemCms, servicesUtilVirtuosoAndReplication, virtuosoAd, logger, loggerFactory)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mlogger = logger;
            mloggerFactory = loggerFactory;
            mRedisCacheWrapper = redisCacheWrapper;
        }

        #endregion

        #region Métodos generales

        #region Métodos protegidos

        /// <summary>
        /// Obtiene los atributos de la entidad.
        /// </summary>
        /// <param name="pEntidadBuscada">Entidad de la que hay que obtener sus atribtos.</param>
        /// <param name="pElementoGnoss">Elemento que representa la entidad.</param>
        public override void ObtenerAtributosEntidad(ElementoOntologia pEntidadBuscada, IElementoGnoss pElementoGnoss)
        {
            if (pElementoGnoss is Identidad)
                if (pEntidadBuscada.TipoEntidad == TipoElementoGnoss.PerfilPersonaFoaf)
                {
                    #region Obtener Datos de persona

                    string nombrePersona = "";
                    string apellidosPers = "";
                    string sexoPer = "";

                    if (((Identidad)pElementoGnoss).Persona != null)
                    {
                        nombrePersona = ((Identidad)pElementoGnoss).Persona.Nombre;
                        apellidosPers = ((Identidad)pElementoGnoss).Persona.Apellidos;
                        sexoPer = ((Identidad)pElementoGnoss).Persona.Sexo;
                    }
                    else
                    {
                        PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mloggerFactory.CreateLogger<PersonaCN>(), mloggerFactory);
                        Gnoss.AD.EntityModel.Models.PersonaDS.Persona persona = personaCN.ObtenerFilaPersonaPorID(((Identidad)pElementoGnoss).PerfilUsuario.PersonaID.Value);

                        nombrePersona = persona.Nombre;
                        apellidosPers = persona.Apellidos;
                        sexoPer = persona.Sexo;
                    }

                    #endregion

                    #region Propiedades Perfil personal

                    Propiedad firstName = UtilImportarExportar.ObtenerPropiedadDeNombre(UtilImportarExportar.PROPIEDAD_FOAF_FIRSTNAME, pEntidadBuscada.Propiedades);
                    firstName.ListaValores.Add(nombrePersona, null);

                    Propiedad familyName = UtilImportarExportar.ObtenerPropiedadDeNombre(UtilImportarExportar.PROPIEDAD_FOAF_FAMILYNAME, pEntidadBuscada.Propiedades);
                    familyName.ListaValores.Add(apellidosPers, null);

                    #endregion

                    if (((Identidad)pElementoGnoss).FilaIdentidad.Tipo == (short)TiposIdentidad.ProfesionalPersonal)
                    {
                        //Es profesional-personal

                        #region Propiedades Perfil profesional-personal

                        Propiedad name = UtilImportarExportar.ObtenerPropiedadDeNombre(UtilImportarExportar.PROPIEDAD_FOAF_NAME, pEntidadBuscada.Propiedades);
                        name.ListaValores.Add(((Identidad)pElementoGnoss).PerfilUsuario.FilaPerfil.NombrePerfil, null);

                        Identidad identPers = (Identidad)pElementoGnoss;
                        Identidad identOrg = identPers.IdentidadOrganizacion;

                        if (identOrg == null)
                        {
                            IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mloggerFactory.CreateLogger<IdentidadCN>(), mloggerFactory);
                            DataWrapperIdentidad identidadesOrgDW = identCN.ObtenerPerfilDeOrganizacion(identPers.OrganizacionID.Value);
                            identCN.Dispose();
                            //"OrganizacionID = '" + identPers.OrganizacionID + "' AND PersonaID is null"
                            List<Gnoss.AD.EntityModel.Models.IdentidadDS.Perfil> filasPerfil = identidadesOrgDW.ListaPerfil.Where(perfil => perfil.OrganizacionID.Equals(identPers.OrganizacionID) && !perfil.PersonaID.HasValue).ToList();

                            GestionIdentidades gestorIdentidades = new GestionIdentidades(identidadesOrgDW, identPers.GestorIdentidades.GestorPersonas, identPers.GestorIdentidades.GestorOrganizaciones, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

                            if (gestorIdentidades.GestorOrganizaciones == null)
                            {
                                gestorIdentidades.GestorOrganizaciones = new GestionOrganizaciones(new DataWrapperOrganizacion(), mLoggingService, mEntityContext);
                            }

                            if (!gestorIdentidades.GestorOrganizaciones.ListaOrganizaciones.ContainsKey(identPers.OrganizacionID.Value))
                            {
                                OrganizacionCN orgCn = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mloggerFactory.CreateLogger<OrganizacionCN>(), mloggerFactory);
                                gestorIdentidades.GestorOrganizaciones.OrganizacionDW.Merge(orgCn.ObtenerOrganizacionPorID(identPers.OrganizacionID.Value));
                                orgCn.Dispose();
                            }
                            //if (filasPerfil[0].Identidad != null )
                            //{
                                identOrg = gestorIdentidades.ListaIdentidades[filasPerfil[0].Identidad.First().IdentidadID];
                            //}
                            //else
                            //{
                            //    Perfil perfilOrg = new Perfil(filasPerfil[0], gestorIdentidades, mLoggingService);
                            //    identOrg = new Identidad(identCN.ObtenerIdentidadesDePerfil(filasPerfil[0].PerfilID).ListaIdentidad[0], perfilOrg, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                            //}
                            
                        }

                        if (identOrg != null)
                        {

                            //Agrego la organización a la que pertence:
                            ElementoOntologia groupOrg = new ElementoOntologiaGnoss(this.Ontologia.GetEntidadTipo(TipoElementoGnoss.PerfilOrganizacionFoaf));
                            UtilImportarExportar.ObtenerID(groupOrg, identOrg.FilaIdentidad, identOrg);
                            ObtenerEntidad(groupOrg, identOrg, true, identOrg.GestorIdentidades);

                            groupOrg.ObtenerPropiedad(UtilImportarExportar.PROPIEDAD_FOAF_MEMBER).ListaValores.Add(pEntidadBuscada.ID, pEntidadBuscada);
                            //Habría que hacerlo alrevés, pero esta es la única forma de que aparezca siendo la identidad la entidad principal.
                            pEntidadBuscada.EntidadesRelacionadas.Add(groupOrg);
                        }

                        #endregion
                    }

                    Propiedad gender = UtilImportarExportar.ObtenerPropiedadDeNombre(UtilImportarExportar.PROPIEDAD_FOAF_GENDER, pEntidadBuscada.Propiedades);
                    if (sexoPer == "M")
                    {
                        gender.UnicoValor = new KeyValuePair<string, ElementoOntologia>("female", null);
                    }
                    else
                    {
                        gender.UnicoValor = new KeyValuePair<string, ElementoOntologia>("male", null);
                    }
                }
                else if (pEntidadBuscada.TipoEntidad == TipoElementoGnoss.PerfilOrganizacionFoaf)
                {
                    #region Obtener Datos de organización

                    string nombreOrg = "";

                    if (((Identidad)pElementoGnoss).OrganizacionPerfil != null)
                    {
                        nombreOrg = ((Identidad)pElementoGnoss).OrganizacionPerfil.Nombre;
                    }
                    else
                    {
                        OrganizacionCN organizacionCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mloggerFactory.CreateLogger<OrganizacionCN>(), mloggerFactory);
                        Gnoss.AD.EntityModel.Models.OrganizacionDS.Organizacion organizacion = organizacionCN.ObtenerNombreOrganizacionPorID(((Identidad)pElementoGnoss).PerfilUsuario.OrganizacionID.Value);
                        organizacionCN.Dispose();

                        nombreOrg = organizacion.Nombre;
                    }

                    #endregion

                    Propiedad name = UtilImportarExportar.ObtenerPropiedadDeNombre(UtilImportarExportar.PROPIEDAD_FOAF_NAME, pEntidadBuscada.Propiedades);
                    name.ListaValores.Add(nombreOrg, null);

                    pEntidadBuscada.Descripcion = nombreOrg;
                }
                else
                {
                    UtilImportarExportar.ObtenerAtributosEntidad(pEntidadBuscada, ((Identidad)pElementoGnoss).PerfilUsuario.FilaPerfil);
                }
            else if (pElementoGnoss is Gnoss.Elementos.ServiciosGenerales.Proyecto)
                UtilImportarExportar.ObtenerAtributosEntidad(pEntidadBuscada, ((Gnoss.Elementos.ServiciosGenerales.Proyecto)pElementoGnoss).FilaProyecto);
            else if (pElementoGnoss is Persona)
                UtilImportarExportar.ObtenerAtributosEntidad(pEntidadBuscada, ((Persona)pElementoGnoss).FilaPersona);
            else
                base.ObtenerAtributosEntidad(pEntidadBuscada, pElementoGnoss);
        }

        /// <summary>
        /// Especializa un elemento de estructura si es posible.
        /// </summary>
        /// <param name="pEntidad">Entidad a especializar</param>
        /// <param name="pElementoGnoss">Elemento que representa la entidad</param>
        /// <param name="pGestor">Gestor de estructura.</param>
        /// <param name="pModoExportacion">Modo en el que se exporta la entidad.</param>
        /// <returns></returns>
        protected override bool ComprobarEspecializacion(Es.Riam.Semantica.OWL.ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, GestionGnoss pGestor, EModosExportacion pModoExportacion)
        {
            bool resultado = false;

            switch (pEntidad.TipoEntidad)
            {
                case TipoElementoGnoss.PerfilPersona:
                    if (!((Identidad)pElementoGnoss).FilaIdentidad.Tipo.Equals((short)TiposIdentidad.Personal))
                    {
                        EspecializarEntidad(pEntidad, pElementoGnoss, pGestor, TipoElementoGnoss.PerfilPersonaOrg, EModosExportacion.AtributosYPropiedades);
                        resultado = true;
                    }

                    break;
            }
            return resultado;
        }

        /// <summary>
        /// Generaliza una entidad para obtener los atributos del padre.
        /// </summary>
        /// <param name="pEntidad">Entidad a generalizar</param>
        /// <param name="pElementoGnoss">Elemento que representa la entidad.</param>
        /// <param name="pFilaElemento">Fila del elemento</param>
        /// <param name="pGestor"></param>
        protected override void GeneralizarEntidad(ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, object pFilaElemento, GestionGnoss pGestor)
        {
            switch (pEntidad.TipoEntidad)
            {
                case TipoElementoGnoss.PerfilPersonaOrg:
                case TipoElementoGnoss.PerfilPersonaFoaf:
                case TipoElementoGnoss.PerfilOrganizacionFoaf:
                    base.GeneralizarEntidad(pEntidad, pElementoGnoss, pGestor, ((Identidad)pElementoGnoss).PerfilUsuario.FilaPerfil);
                    break;
            }
        }

        /// <summary>
        /// Trata los casos especiales de competencias.
        /// </summary>
        /// <param name="pEntidad">Entidad que posee las propiedades</param>
        /// <param name="pElementoGnoss">Elemento que representa la entidad.</param>
        /// <param name="pPropiedad">Propiedad ha tratar.</param>
        /// <param name="pGestor">Gestor de competencias.</param>
        /// <returns></returns>
        protected override bool TratarCasoEspecial(ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, Propiedad pPropiedad, GestionGnoss pGestor)
        {
            bool resultado = false;
            switch (pPropiedad.Nombre)
            {
                case UtilImportarExportar.PROPIEDAD_EVENTOS:
                    //ObtenerListasEventos(pEntidad, pElementoGnoss, pPropiedad, pGestor);
                    resultado = true;
                    break;
                case UtilImportarExportar.PROPIEDAD_PERSONALIZACIOINES:
                    //ObtenerPersonalizacion(pEntidad, pElementoGnoss, pPropiedad, pGestor);
                    resultado = true;
                    break;
                case UtilImportarExportar.PROPIEDAD_AUTOR_PUBLICACION:
                    //ObtenerAutorPublicacion(pEntidad, pElementoGnoss, pPropiedad, pGestor);
                    resultado = true;
                    break;
                case UtilImportarExportar.PROPIEDAD_CV_LIBRO:
                    //ObtenerCVLibro(pEntidad, pElementoGnoss, pPropiedad, pGestor);
                    resultado = true;
                    break;
                case UtilImportarExportar.PROPIEDAD_PUESTO_EXP_LAB:
                    //ObtenerPuestoExpLab(pEntidad, pElementoGnoss, pPropiedad, pGestor);
                    resultado = true;
                    break;
                case UtilImportarExportar.PROPIEDAD_DOCENCIA:
                    //ObtenerDocencia(pEntidad, pElementoGnoss, pPropiedad, pGestor);
                    resultado = true;
                    break;
                case UtilImportarExportar.PROPIEDAD_CVPERFILORGANIZACION:
                    //ObtenerCVOrganizacion(pEntidad, pElementoGnoss, pPropiedad, pGestor);
                    resultado = true;
                    break;
                case UtilImportarExportar.PROPIEDAD_PERSONASORGANIZACION:
                    ObtenerPerfilPersonasOrganizacion(pEntidad, pElementoGnoss, pPropiedad, pGestor);
                    resultado = true;
                    break;
                case UtilImportarExportar.PROPIEDAD_COMUNIDADES:
                case UtilImportarExportar.PROPIEDAD_CUENTA_USUARIO:
                    //TODO bug8029: no se quiere que aparezca la propiedad cuenta al descargar el RDF de un recurso
                    //ObtenerComunidadesPerfil(pEntidad, pElementoGnoss, pPropiedad, pGestor);
                    resultado = true;
                    break;
                case UtilImportarExportar.PROPIEDAD_PADRE:
                    if (pEntidad.TipoEntidad.Equals(TipoElementoGnoss.Comunidad))
                    {
                        resultado = true;
                    }
                    break;
                default:
                    break;
            }
            return resultado;
        }

        private void ObtenerPerfilPersonasOrganizacion(ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, Propiedad pPropiedad, GestionGnoss pGestor)
        {
            Organizacion org = ((Identidad)pElementoGnoss).OrganizacionPerfil;
            if ((org != null) && org.FilaOrganizacion.ModoPersonal)
            {
                GestionIdentidades gestorIdentidades = ((Identidad)pElementoGnoss).GestorIdentidades;

                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mloggerFactory.CreateLogger<IdentidadCN>(), mloggerFactory);
                gestorIdentidades.DataWrapperIdentidad.Merge(identidadCN.ObtenerIdentidadesDeOrganizacionYEmpleados(org.Clave));
                identidadCN.Dispose();

                gestorIdentidades.RecargarHijos();
                //"OrganizacionID = '" + org.Clave + "' AND Tipo = " + ((short)TiposIdentidad.ProfesionalPersonal).ToString())
                foreach (Gnoss.AD.EntityModel.Models.IdentidadDS.Identidad identidad in gestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(identidad => identidad.OrganizacionID.Equals(org.Clave) && identidad.Tipo.Equals((short)TiposIdentidad.ProfesionalPersonal)).ToList())
                {
                    ElementoOntologia elemento = new ElementoOntologiaGnoss(Ontologia.GetEntidadTipo(TipoElementoGnoss.PerfilPersonaOrg));
                    UtilImportarExportar.ObtenerID(elemento, identidad, gestorIdentidades.ListaIdentidades[identidad.IdentidadID]);
                    AgregarYObtenerEntidadRelacionada(pEntidad, pPropiedad, elemento, gestorIdentidades.ListaIdentidades[identidad.IdentidadID], identidad, true, gestorIdentidades);
                }
            }
        }

        private void ObtenerComunidadesPerfil(ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, Propiedad pPropiedad, GestionGnoss pGestor, Guid pUsuarioID)
        {
            Identidad identidad = (Identidad)pElementoGnoss;
            mGestorProyectos = GestorProyectos(identidad.FilaIdentidad.PerfilID, pUsuarioID);

            List<Gnoss.AD.EntityModel.Models.IdentidadDS.Identidad> identidades = identidad.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(ident => ident.PerfilID.Equals(identidad.FilaIdentidad.PerfilID)).ToList();

            if (identidades.Count < 2)
            {
                IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mloggerFactory.CreateLogger<IdentidadCN>(), mloggerFactory);
                identidad.GestorIdentidades.DataWrapperIdentidad.Merge(identCN.ObtenerIdentidadesDePerfil(identidad.PerfilUsuario.Clave));
                identCN.Dispose();

                identidad.GestorIdentidades.RecargarHijos();

                identidades = identidad.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(ident => ident.PerfilID.Equals(identidad.FilaIdentidad.PerfilID)).ToList();
            }

            ElementoOntologia userAccount = null;

            if (pPropiedad.Nombre == UtilImportarExportar.PROPIEDAD_CUENTA_USUARIO)
            {
                //Creo el UserAccount:
                userAccount = new ElementoOntologiaGnoss(this.Ontologia.GetEntidadTipo(TipoElementoGnoss.USER_ACCOUNT));
                UtilImportarExportar.ObtenerID(userAccount, identidad.FilaIdentidad, identidad);

                AgregarYObtenerEntidadRelacionada(pEntidad, pPropiedad, userAccount, identidad, identidad.FilaIdentidad, false, pGestor);
            }

            foreach (Gnoss.AD.EntityModel.Models.IdentidadDS.Identidad identProy in identidades)
            {
                if (!identProy.ProyectoID.Equals(ProyectoAD.MetaProyecto))
                {
                    if (mGestorProyectos.ListaProyectos.ContainsKey(identProy.ProyectoID))
                    {
                        Proyecto proyecto = mGestorProyectos.ListaProyectos[identProy.ProyectoID];

                        if (pPropiedad.Nombre != UtilImportarExportar.PROPIEDAD_CUENTA_USUARIO)
                        {
                            ElementoOntologia elemento = new ElementoOntologiaGnoss(this.Ontologia.GetEntidadTipo(TipoElementoGnoss.Comunidad));
                            UtilImportarExportar.ObtenerID(elemento, proyecto.FilaProyecto, proyecto);
                            AgregarYObtenerEntidadRelacionadaProyecto(pEntidad, pPropiedad, elemento, proyecto, proyecto.FilaProyecto, false, pGestor);
                        }
                        else
                        {
                            ElementoOntologia userGroup = new ElementoOntologiaGnoss(this.Ontologia.GetEntidadTipo(TipoElementoGnoss.USER_GROUP));
                            UtilImportarExportar.ObtenerID(userGroup, proyecto.FilaProyecto, proyecto);
                            Propiedad memberOf = userAccount.ObtenerPropiedad(UtilImportarExportar.PROPIEDAD_GRUPO_DE_USUARIO);
                            if (!memberOf.ListaValores.ContainsKey(userGroup.ID))
                            {
                                memberOf.ListaValores.Add(userGroup.ID, userGroup);
                                userAccount.EntidadesRelacionadas.Add(userGroup);

                                ElementoOntologia elemento = new ElementoOntologiaGnoss(this.Ontologia.GetEntidadTipo(TipoElementoGnoss.ComunidadSioc));
                                UtilImportarExportar.ObtenerID(elemento, proyecto.FilaProyecto, proyecto);
                                AgregarYObtenerEntidadRelacionadaProyecto(userGroup, userGroup.ObtenerPropiedad(UtilImportarExportar.PROPIEDAD_COMUNIDAD_DE_GRUPO_USUARIOS), elemento, proyecto, proyecto.FilaProyecto, false, pGestor);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Métodos públicos

        /// <summary>
        /// Obtiene la entidad pEntidad y todas sus relaciones.
        /// </summary>
        /// <param name="pEntidad">Entidad que se va a obtener.</param>
        /// <param name="pElementoGnoss">Elemento gnoss que representa la entidad</param>
        /// <param name="pEspecializacion">Indica si la entidad será especilización de otra.</param>
        /// <param name="pGestor">Gestor de entidades</param>
        public override void ObtenerEntidad(ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, bool pEspecializacion, GestionGnoss pGestor)
        {
            if (pElementoGnoss is Identidad)
            {
                Identidad identidad = (Identidad)pElementoGnoss;
                GestionIdentidades gestorIdentidades = identidad.GestorIdentidades;

                if (identidad.FilaIdentidad.Tipo.Equals((short)TiposIdentidad.ProfesionalCorporativo) && (pEntidad.TipoEntidad == TipoElementoGnoss.PerfilPersona || pEntidad.TipoEntidad == TipoElementoGnoss.PerfilPersonaOrg))
                {
                    List<Gnoss.AD.EntityModel.Models.IdentidadDS.Perfil> filasPerfil = identidad.GestorIdentidades.DataWrapperIdentidad.ListaPerfil.Where(perfil => perfil.OrganizacionID.Equals(identidad.OrganizacionID) && !perfil.PersonaID.HasValue).ToList();
                    if ((filasPerfil.Count == 0) || (filasPerfil.First().Identidad.Count == 0))
                    {
                        IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mloggerFactory.CreateLogger<IdentidadCN>(), mloggerFactory);
                        DataWrapperIdentidad identidadesOrgDW = identCN.ObtenerPerfilDeOrganizacion(identidad.OrganizacionID.Value);
                        identCN.Dispose();
                        //"OrganizacionID = '" + identidad.OrganizacionID + "' AND PersonaID is null"
                        filasPerfil = identidadesOrgDW.ListaPerfil.Where(perfil => perfil.OrganizacionID.Equals(identidad.OrganizacionID) && !perfil.PersonaID.HasValue).ToList();

                        gestorIdentidades = new GestionIdentidades(identidadesOrgDW, gestorIdentidades.GestorPersonas, gestorIdentidades.GestorOrganizaciones, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

                        if (gestorIdentidades.GestorOrganizaciones == null)
                        {
                            gestorIdentidades.GestorOrganizaciones = new GestionOrganizaciones(new DataWrapperOrganizacion(), mLoggingService, mEntityContext);
                        }

                        if (!gestorIdentidades.GestorOrganizaciones.ListaOrganizaciones.ContainsKey(identidad.OrganizacionID.Value))
                        {
                            OrganizacionCN orgCn = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mloggerFactory.CreateLogger<OrganizacionCN>(), mloggerFactory);
                            gestorIdentidades.GestorOrganizaciones.OrganizacionDW.Merge(orgCn.ObtenerOrganizacionPorID(identidad.OrganizacionID.Value));
                            orgCn.Dispose();
                        }
                    }

                    if ((filasPerfil.Count > 0) && (filasPerfil.First().Identidad.Count > 0))
                    {
                        pElementoGnoss = gestorIdentidades.ListaIdentidades[filasPerfil.First().Identidad.First().IdentidadID];
                        Identidad ident = (Identidad)pElementoGnoss;

                        ElementoOntologia entidadCreador = new ElementoOntologiaGnoss(this.Ontologia.GetEntidadTipo(TipoElementoGnoss.PerfilOrganizacion));

                        entidadCreador.Descripcion = ident.NombreOrganizacion;
                        Gnoss.AD.EntityModel.Models.IdentidadDS.Identidad creadorRow = ident.FilaIdentidad;

                        UtilImportarExportar.ObtenerID(entidadCreador, creadorRow, ident);

                        pEntidad.ClonarEntidad(entidadCreador);
                    }
                }

                base.ObtenerEntidad(pEntidad, pElementoGnoss, ((ElementoGnoss)pElementoGnoss).FilaElemento, pEspecializacion, pGestor);
            }
            else if (pElementoGnoss is ElementoGnoss)
            {
                base.ObtenerEntidad(pEntidad, pElementoGnoss, ((ElementoGnoss)pElementoGnoss).FilaElemento, pEspecializacion, pGestor);
            }
        }

        /// <summary>
        /// Obtiene el gestor de proyectos con los proyectos en los que participa el perfil cuyo identificador se pasa por parámetro
        /// </summary>
        /// <param name="pPerfilID">Identificador de perfil</param>
        /// <returns></returns>
        public GestionProyecto GestorProyectos(Guid pPerfilID, Guid pUsuarioID)
        {
            if (mGestorProyectos == null)
            {
                mGestorProyectos = new GestionProyecto(new DataWrapperProyecto(), mLoggingService, mEntityContext,mloggerFactory.CreateLogger<GestionProyecto>(), mloggerFactory);
            }

            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mloggerFactory.CreateLogger<ProyectoCN>(), mloggerFactory);
            mGestorProyectos.DataWrapperProyectos.Merge(proyCN.ObtenerProyectosParticipaPerfilUsuario(pPerfilID, true, pUsuarioID));
            proyCN.Dispose();

            mGestorProyectos.RecargarProyectos();

            return mGestorProyectos;
        }

        #endregion

        #endregion

        #region Dispose

        /// <summary>
        /// Destructor
        /// </summary>
        ~ExportadorCurriculum()
        {
            //Libero los recursos
            Dispose(false);
        }

        #endregion
    }
}