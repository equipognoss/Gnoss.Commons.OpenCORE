using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.Documentacion;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.BASE;
using Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS;
using Es.Riam.Gnoss.AD.Facetado.Model;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Util;
using Es.Riam.Util.AnalisisSintactico;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Es.Riam.Gnoss.AD.Facetado
{
    /// <summary>
    /// 
    /// </summary>
    public class ActualizacionFacetadoAD : BaseAD
    {

        #region Miembros

        /// <summary>
        /// Url de la Intranet
        /// </summary>
        private string mUrlIntranet;


        private VirtuosoAD mVirtuosoAD;

        #endregion

        #region Constructores

        /// <summary>
        /// El por defecto, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        /// <param name="pUrlIntragnoss">URL de intragnoss</param>
        public ActualizacionFacetadoAD(string pUrlIntragnoss, LoggingService loggingService, EntityContext entityContext, ConfigService configService, VirtuosoAD virtuosoAD, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            mUrlIntranet = pUrlIntragnoss;
            mVirtuosoAD = virtuosoAD;
            //this.CargarConsultasYDataAdapters();
        }

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuracion de conexion a la Base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD"></param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        /// <param name="pUrlIntragnoss">URL de intragnoss</param>
        public ActualizacionFacetadoAD(string pFicheroConfiguracionBD, string pUrlIntragnoss, LoggingService loggingService, EntityContext entityContext, ConfigService configService, VirtuosoAD virtuosoAD, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            mUrlIntranet = pUrlIntragnoss;
            mVirtuosoAD = virtuosoAD;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tipoitem"></param>
        /// <param name="idproy"></param>
        public Dictionary<Guid, List<string>> ObtenerTagsLista(List<Guid> listaElementosID, string tipoitem, Guid proyectoID)
        {
            Dictionary<Guid, string> tags = new Dictionary<Guid, string>();
            Dictionary<Guid, List<string>> tagsFinales = new Dictionary<Guid, List<string>>();
            foreach (Guid elementoID in listaElementosID)
            {
                tags.Add(elementoID, "");
            }

            if (tipoitem.Equals("Recurso"))
            {
                DocumentacionAD docAD = new DocumentacionAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                tags = docAD.ObtenerTagsDeDocumentos(listaElementosID);
            }
            else if (tipoitem.Equals("Persona"))
            {
                PersonaAD personaAD = new PersonaAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                tags = personaAD.ObtenerTagsDePersonasEnProyecto(listaElementosID, proyectoID);
            }
            else if (tipoitem.Equals("Organizacion"))
            {
                OrganizacionAD organizacionAD = new OrganizacionAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                tags = organizacionAD.ObtenerTagsDeOrganizacionesEnProyecto(listaElementosID, proyectoID);
            }
            else if (tipoitem.Equals("Grupo"))
            {
                IdentidadAD IdentidadAD = new IdentidadAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                tags = IdentidadAD.ObtenerTagsDeGruposEnProyecto(listaElementosID, proyectoID);
            }
            else if (tipoitem.Equals("Proyecto"))
            {
                ProyectoAD proyectoAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                tags = proyectoAD.ObtenerTagsDeProyectos(listaElementosID);
            }

            else if (tipoitem.Equals("Mensaje"))
            {
                //Usar algoritmo deusto
            }

            foreach (Guid elementoID in listaElementosID)
            {
                List<string> listaTags = UtilCadenas.SepararTexto(tags[elementoID].Replace("\"", "").Replace((char)13, ' ').Replace((char)10, ' ').Replace('\\', ' '));
                tagsFinales.Add(elementoID, listaTags);
            }

            return tagsFinales;
        }

        /// <summary>
        /// Obtiene el título de un blog
        /// </summary>
        /// <param name="pIdBlog"></param>
        /// <returns></returns>
        public string ObtenerTituloBlog(Guid pIdBlog)
        {
            return mEntityContext.Blog.Where(item => item.BlogID.Equals(pIdBlog)).Select(item => item.Titulo).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene el título de un blog
        /// </summary>
        /// <param name="DataSet"></param>
        /// <param name="pIdEntradaBlog"></param>
        /// <returns></returns>
        public string ObtenerTituloEntradaBlog(Guid pIdEntradaBlog)
        {
            return mEntityContext.EntradaBlog.Where(item => item.EntradaBlogID.Equals(pIdEntradaBlog)).Select(item => item.Titulo).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene las identidades del metaproyecto
        /// </summary>
        /// <returns>Lista de indetidades del metaproyecto</returns>
        public List<Guid> ObtenerIdentidadesMyGnoss()
        {
            return mEntityContext.Identidad.Where(item => item.ProyectoID.Equals(new Guid("11111111-1111-1111-1111-111111111111"))).Select(item => item.IdentidadID).ToList();
        }

        /// <summary>
        /// Obtiene el título de un proyecto
        /// </summary>
        /// <param name="pProyectoID"></param>
        /// <returns></returns>
        public string ObtenerTituloProyecto(Guid pProyectoID)
        {
            return mEntityContext.Proyecto.Where(item => item.ProyectoID.Equals(pProyectoID)).Select(item => item.Nombre).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene el destinatario de un correo
        /// </summary>
        /// <param name="pCorreoID">Id del correo a obtener el destinatario</param>
        /// <param name="pCorreoFrom">Identificador de la tabla a consultar</param>
        /// <returns></returns>
        public string ObtenerDestinatarioMensaje(DataSet pDataSet, string pCorreoID, string pCorreoFrom)
        {
            string identificadorTablaCorreo = pCorreoFrom.Substring(0, 2);
            string consulta = $"SELECT \"CorreoInterno_{identificadorTablaCorreo}\".\"DestinatariosNombres\" FROM  \"CorreoInterno_{identificadorTablaCorreo}\" WHERE \"CorreoID\" = '{pCorreoID}'";

            DbCommand commandsql = ObtenerComando(consulta);

            CargarDataSet(commandsql, pDataSet, "idProy2");

            if (pDataSet.Tables.Contains("idProy2") && pDataSet.Tables["idProy2"].Rows.Count > 0)
            {
                return pDataSet.Tables["idProy2"].Rows[0][0].ToString();
            }
            else
            {
                return Guid.Empty.ToString();
            }
        }

        /// <summary>
        /// Devuelve el título de un correo y rellena su descripción
        /// </summary>
        /// <param name="pCorreoFrom">Identificador de la tabla a consultar</param>
        /// <param name="pCorreoID">Id del correo a obtener el titulo</param>
        /// <param name="pDescripcion">Descripcion a devolver</param>
        /// <returns></returns>
        public string ObtenerTituloMensaje(DataSet pDataSet, Guid pCorreoID, string pCorreoFrom, out string pDescripcion)
        {
            string iniciofrom = pCorreoFrom.Substring(0, 2);
            string consulta = $"SELECT \"CorreoInterno_{iniciofrom}\".\"Asunto\", \"CorreoInterno_{iniciofrom}\".\"Cuerpo\" FROM \"CorreoInterno_{iniciofrom}\" WHERE \"CorreoID\" = '{pCorreoID}'";

            pDescripcion = "";
            DbCommand commandsql = ObtenerComando(consulta);

            CargarDataSet(commandsql, pDataSet, "idProy");

            if (pDataSet.Tables.Contains("idProy") && pDataSet.Tables["idProy"].Rows.Count > 0)
            {
                pDescripcion = pDataSet.Tables["idProy"].Rows[0][1].ToString();
                return pDataSet.Tables["idProy"].Rows[0][0].ToString();
            }
            else
            {
                return Guid.Empty.ToString();
            }
        }


        /// <summary>
        /// Obtiene el título de una descripción.
        /// </summary>
        /// <param name="pSuscripcionID">ID de suscripción</param>
        /// <param name="pRecursoID">ID de recurso</param>
        /// <returns>Título de la suscripción</returns>
        public string ObtenerTituloSuscripcion(Guid pSuscripcionID, Guid pRecursoID)
        {
            return mEntityContext.ResultadoSuscripcion.Where(item => item.SuscripcionID.Equals(pSuscripcionID) && item.RecursoID.Equals(pRecursoID)).Select(item => item.Titulo).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene el titulo de un comentario
        /// </summary>
        /// <param name="pComentarioID">Id del comentario</param>
        /// <returns>Devuelve el titulo de un comentario</returns>
        public string ObtenerTituloComentario(Guid pComentarioID)
        {
            return mEntityContext.Comentario.Where(item => item.ComentarioID.Equals(pComentarioID)).Select(item => item.Descripcion).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene el nombre y apellido de un contacto
        /// </summary>
        /// <param name="pIdentidadId">Identidad del contacto a buscar</param>
        /// <returns>Titulo del contacto</returns>
        public string ObtenerTituloContacto(Guid pIdentidadId)
        {
            return mEntityContext.Identidad.JoinPerfil().JoinPersona().Where(item => item.Identidad.IdentidadID.Equals(pIdentidadId)).Select(item => $"{item.Persona.Nombre}{item.Persona.Apellidos}").FirstOrDefault();
        }

        /// <summary>
        /// Obtiene la descripción de los recursos indicados
        /// </summary>        
        /// <param name="pListaIdRecurso">Lista de recursos</param>
        /// <returns>Diccioncario con clave valor Id con descripción</returns>
        public Dictionary<Guid, string> ObtenerDescripcionesRecursos(List<Guid> pListaIdRecurso)
        {
            return mEntityContext.Documento.Where(item => pListaIdRecurso.Contains(item.DocumentoID)).ToDictionary(item => item.DocumentoID, item => item.Descripcion);
        }

        /// <summary>
        /// Obtiene los titulos de los recursos
        /// </summary>
        /// <param name="pListaIdRecurso">Lista de los ids de los recursos</param>
        /// <returns>Diccionario con clave valor Id con titulo</returns>
        public Dictionary<Guid, string> ObtenerTitulosRecursos(List<Guid> pListaIdRecurso)
        {
            return mEntityContext.Documento.Where(item => pListaIdRecurso.Contains(item.DocumentoID)).ToDictionary(item => item.DocumentoID, item => item.Titulo);
        }

        /// <summary>
        /// Obtiene el nombre y apellido de una persona a partir del perfil id
        /// </summary>
        /// <param name="pIdentidadID">Id del perfil</param>
        /// <returns>Nombre y apellido de la persona</returns>
        public string ObtenerTituloPersona(Guid pIdentidadID)
        {
            return mEntityContext.Persona.JoinPerfil().JoinIdentidad().Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID)).Select(item => $"{item.Persona.Nombre} {item.Persona.Apellidos}").FirstOrDefault();
        }

        /// <summary>
        /// Obtiene el título del grupo a partir de id
        /// </summary>
        /// <param name="pGrupoID">Id del grupo</param>
        /// <returns></returns>
        public string ObtenerTituloGrupo(Guid pGrupoID)
        {
            return mEntityContext.GrupoIdentidades.Where(item => item.GrupoID.Equals(pGrupoID)).Select(item => item.Nombre).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene el nombre de una organizacion
        /// </summary>
        /// <param name="pIdentidadID"></param>
        /// <returns></returns>
        public string ObtenerTituloOrganizacion(Guid pIdentidadID)
        {
            return mEntityContext.Organizacion.JoinPerfil().JoinIdentidad().Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID)).Select(item => item.Organizacion.Nombre).FirstOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="proyid"></param>
        /// <param name="pPropiedad"></param>
        /// <param name="pTextoSinSeparar"></param>
        public void AgregarTripletasDescompuestasTituloViejo(string id, string proyid, string pPropiedad, string pTextoSinSeparar)
        {
            FacetadoAD facetadoAD = new FacetadoAD(mUrlIntranet, mLoggingService, mEntityContext, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            pTextoSinSeparar = pTextoSinSeparar.Replace("\"", "").Replace(((char)13).ToString() + ((char)10).ToString() + ((char)10).ToString() + ((char)13).ToString(), " ").Replace((char)10, ' ').Replace("\\", " ").Replace((char)13, ' ');

            int numeroTagsDespreciadosTitulo = 0;

            //Recorro los tags individuales
            List<string> miniTagsTitulo = AnalizadorSintactico.ObtenerTagsFrase(pTextoSinSeparar, out numeroTagsDespreciadosTitulo);
            if (miniTagsTitulo.Count + numeroTagsDespreciadosTitulo > 0)
            {
                foreach (string tagDescompuesto in miniTagsTitulo)
                {

                    if ((!tagDescompuesto.Contains("º")) && (!tagDescompuesto.Contains("ª")))
                    {
                        facetadoAD.InsertaTripleta(proyid, $"<http://gnoss/{id}> ", pPropiedad, $"\"{tagDescompuesto.ToLower()}\"", 0);
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene el Id del base recursos del proyecto
        /// </summary>        
        /// <param name="pProyectoID"></param>
        /// <returns></returns>
        public Guid ObtenerBaseRecursoDesdeId(Guid pProyectoID)
        {
            return mEntityContext.BaseRecursos.JoinBaseRecursosProyecto().Where(item => item.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID)).Select(item => item.BaseRecursosProyecto.BaseRecursosID).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene el nombre del editor a partir del id
        /// </summary>
        /// <param name="pIdentidadID"></param>
        /// <returns>Nombre del editor</returns>
        public string ObtieneNombreEditor(Guid pIdentidadID)
        {
            return mEntityContext.Persona.JoinPerfil().JoinIdentidad().Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID)).Select(item => $"{item.Persona.Nombre} {item.Persona.Apellidos}").FirstOrDefault();
        }

        /// <summary>
        /// Obtiene el id del blog a través del id de la entrada
        /// </summary>
        /// <param name="pArticuloBlogID">Entrada id del artículo</param>
        /// <returns>El id del blog</returns>
        public Guid ObtieneIDBlog(Guid pArticuloBlogID)
        {
            return mEntityContext.EntradaBlog.Where(item => item.EntradaBlogID.Equals(pArticuloBlogID)).Select(item => item.BlogID).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene la identidad id
        /// </summary>
        /// <param name="pProyectoID"></param>
        /// <param name="pPersonaID"></param>
        /// <param name="pTraerEliminados"></param>
        /// <returns></returns>
        public Guid? ObtieneIDIdentidad(Guid pProyectoID, Guid pPersonaID, bool pTraerEliminados)
        {
            var consulta = mEntityContext.Perfil.JoinIdentidad().Where(x => x.Identidad.ProyectoID.Equals(pProyectoID) && x.Perfil.PersonaID.Equals(pPersonaID));

            if (!pTraerEliminados)
            {
                consulta = consulta.Where(x => !x.Identidad.FechaBaja.HasValue && x.Perfil.Eliminado == false && !x.Identidad.FechaExpulsion.HasValue);
            }
            if (pProyectoID.Equals(ProyectoAD.MetaProyecto))
            {
                consulta = consulta.Where(x => x.Identidad.Tipo == 0);
            }

            if (consulta.Any())
            {
                return consulta.Select(x => x.Identidad.IdentidadID).FirstOrDefault();
            }

            return null;

        }

        /// <summary>
        /// Obtiene el id de la organizacion
        /// </summary>
        /// <param name="IDproyecto"></param>
        /// <param name="IDpersona"></param>
        /// <param name="pTraerEliminados"></param>
        /// <returns></returns>
        public Guid? ObtieneIDIdentidadOrg(Guid IDproyecto, Guid IDpersona, bool pTraerEliminados)
        {
            var consulta = mEntityContext.Perfil.JoinIdentidad().Where(x => x.Identidad.ProyectoID.Equals(IDproyecto) && x.Perfil.PersonaID.Equals(IDpersona) && x.Identidad.Tipo == 3);

            if (!pTraerEliminados)
            {
                consulta = consulta.Where(x => !x.Identidad.FechaBaja.HasValue && x.Perfil.Eliminado == false && !x.Identidad.FechaExpulsion.HasValue);
            }
            if (IDproyecto.Equals(ProyectoAD.MetaProyecto))
            {
                consulta = consulta.Where(x => x.Identidad.Tipo == 0);
            }

            if (consulta.Any())
            {
                return consulta.Select(x => x.Identidad.IdentidadID).FirstOrDefault();
            }

            return null;
        }

        /// <summary>
        /// Obtiene triples con la privacidad de las persona
        /// </summary>
        /// <param name="pIdentidadID">Id de la identidad de la persona</param>
        /// <param name="pProyectoID">Id del proyecto</param>
        /// <param name="pParametrosApliacion"><Parametros aplicación/param>
        public List<QueryTriples> ObtieneTripletasPrivacidadPersonas(Guid pIdentidadID, Guid pProyectoID, List<EntityModel.ParametroAplicacion> pParametrosApliacion)
        {
            string hasPrivacidad = string.Empty;
            bool esMetaProyecto = false;
            if (pProyectoID.Equals(ProyectoAD.MetaProyecto))
            {
                hasPrivacidad = "<http://gnoss/hasprivacidadMyGnoss>";
                esMetaProyecto = true;
            }
            else
            {
                hasPrivacidad = "<http://gnoss/hasprivacidadCom>";
            }

            Guid comunidadPrincipalID;

            if (pParametrosApliacion.Where(parametro => parametro.Parametro.Equals("EcosistemaSinMetaProyecto")).Count() > 0 && pParametrosApliacion.Where(parametro => parametro.Parametro.Equals("ComunidadPrincipalID")).Count() > 0)
            {
                string proyectoPrincipalEcosistema = pParametrosApliacion.Where(parametro => parametro.Parametro.Equals("ComunidadPrincipalID")).First().Valor;

                if (Guid.TryParse(proyectoPrincipalEcosistema, out comunidadPrincipalID) && pProyectoID.Equals(comunidadPrincipalID))
                {
                    esMetaProyecto = true;
                }
            }

            List<ProyectoRegistroObligatorio> filasProyRegistroObl = mEntityContext.ProyectoRegistroObligatorio.Where(proyecto => proyecto.ProyectoID.Equals(pProyectoID)).ToList();
            List<QueryTriples> resultadoConsulta;
            if ((filasProyRegistroObl != null && filasProyRegistroObl.Count > 0) || esMetaProyecto)
            {
                bool visibilidadUsuariosActivos = !esMetaProyecto && filasProyRegistroObl[0].VisibilidadUsuariosActivos == 1;

                if (!visibilidadUsuariosActivos)
                {
                    var consulta1 = mEntityContext.Identidad.JoinPerfil().JoinPersona().Where(item => item.Persona.EsBuscableExternos && !item.Persona.Eliminado && item.Identidad.ProyectoID.Equals(pProyectoID) && item.Identidad.IdentidadID.Equals(pIdentidadID) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue);

                    var consulta2 = mEntityContext.Identidad.JoinPerfil().JoinPersona().Where(item => item.Persona.EsBuscable && !item.Persona.EsBuscableExternos && !item.Persona.Eliminado && item.Identidad.ProyectoID.Equals(pProyectoID) && item.Identidad.IdentidadID.Equals(pIdentidadID) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue);

                    var consulta3 = mEntityContext.Identidad.JoinPerfil().JoinPersona().Where(item => (!item.Persona.EsBuscable && !item.Persona.EsBuscableExternos) && !item.Persona.Eliminado && item.Identidad.ProyectoID.Equals(pProyectoID) && item.Identidad.IdentidadID.Equals(pIdentidadID) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue);

                    var consulta4 = mEntityContext.Identidad.JoinPerfil().JoinPersona().Where(item => !item.Persona.Eliminado && item.Identidad.ProyectoID.Equals(pProyectoID) && item.Identidad.IdentidadID.Equals(pIdentidadID) && !item.Identidad.FechaExpulsion.HasValue);

                    if (!pProyectoID.Equals(ProyectoAD.MetaProyecto))
                    {
                        int[] andTiposIdentidad = new int[] { 0, 1, 4 };
                        consulta1 = consulta1.Where(item => andTiposIdentidad.Contains(item.Identidad.Tipo));
                        consulta2 = consulta2.Where(item => andTiposIdentidad.Contains(item.Identidad.Tipo));
                        consulta3 = consulta3.Where(item => andTiposIdentidad.Contains(item.Identidad.Tipo));
                        consulta4 = consulta4.Where(item => andTiposIdentidad.Contains(item.Identidad.Tipo));
                    }
                    else
                    {
                        consulta1 = consulta1.Where(item => item.Identidad.Tipo.Equals(0));
                        consulta2 = consulta2.Where(item => item.Identidad.Tipo.Equals(0));
                        consulta3 = consulta3.Where(item => item.Identidad.Tipo.Equals(0));
                        consulta4 = consulta4.Where(item => item.Identidad.Tipo.Equals(0));
                    }

                    resultadoConsulta = consulta1.ToList().Select(item => new QueryTriples
                    {
                        Sujeto = $"<http://gnoss/{item.Identidad.IdentidadID.ToString().ToUpper()}>",
                        Predicado = hasPrivacidad,
                        Objeto = "\"publico\""
                    }).Distinct().Union(consulta2.ToList().Select(item => new QueryTriples
                    {
                        Sujeto = $"<http://gnoss/{item.Identidad.IdentidadID.ToString().ToUpper()}>",
                        Predicado = hasPrivacidad,
                        Objeto = "\"publicReg\""
                    }).Distinct()).Union(consulta3.ToList().Select(item => new QueryTriples
                    {
                        Sujeto = $"<http://gnoss/{item.Identidad.IdentidadID.ToString().ToUpper()}>",
                        Predicado = hasPrivacidad,
                        Objeto = "\"privado\""
                    }).Distinct()).Union(consulta4.ToList().Select(item => new QueryTriples
                    {
                        Sujeto = $"<http://gnoss/{item.Identidad.IdentidadID.ToString().ToUpper()}>",
                        Predicado = hasPrivacidad,
                        Objeto = "\"privado\""
                    }).Distinct()).ToList();
                }
                else
                {
                    DateTime fechaComparar = new DateTime(2012, 1, 4);
                    var consulta1 = mEntityContext.Identidad.JoinProyecto().JoinPerfil().JoinPersona().Where(item => !item.Persona.Eliminado && !item.Perfil.Eliminado && new int[] { 0, 4 }.Contains(item.Identidad.Tipo) && item.Identidad.ProyectoID.Equals(pProyectoID) && item.Identidad.IdentidadID.Equals(pIdentidadID) && item.Persona.EsBuscableExternos && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue && (item.Identidad.FechaAlta < fechaComparar || item.Persona.EsBuscable || item.Identidad.ActivoEnComunidad)).ToList();

                    //Personas y profesores que están en Didactalia, activos y Existen antes del 1-Abril-2012 o son publicos o son activos o se han registrado directamente
                    //Los insertamos como publicos registrados
                    var consulta2 = mEntityContext.Identidad.JoinProyecto().JoinPerfil().JoinPersona().Where(item => new int[] { 0, 1, 4 }.Contains(item.Identidad.Tipo) && item.Identidad.ProyectoID.Equals(pProyectoID) && item.Identidad.IdentidadID.Equals(pIdentidadID) && item.Identidad.FechaAlta >= fechaComparar && item.Identidad.ActivoEnComunidad && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaBaja.HasValue).ToList();

                    //Personas y profesores que están en Didactalia, existen despues del 1-Abril-2012 , no son publicos ni son activos ni se han registrado directamente
                    var consulta3 = mEntityContext.Identidad.JoinProyecto().JoinPerfil().JoinPersona().Where(item => new int[] { 0, 1, 4 }.Contains(item.Identidad.Tipo) && item.Identidad.ProyectoID.Equals(pProyectoID) && item.Identidad.IdentidadID.Equals(pIdentidadID) && item.Identidad.FechaAlta >= fechaComparar && !item.Identidad.ActivoEnComunidad && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue).ToList();

                    //Identidades de tipo 1                     
                    var consulta4 = mEntityContext.Identidad.JoinProyecto().JoinPerfil().JoinPersona().Where(item => !item.Persona.Eliminado && item.Identidad.Tipo == 1 && !item.Perfil.Eliminado && item.Identidad.ProyectoID.Equals(pProyectoID) && item.Identidad.IdentidadID.Equals(pIdentidadID) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue).ToList();

                    // Usuarios expulsados
                    var consulta5 = mEntityContext.Identidad.JoinProyecto().JoinPerfil().JoinPersona().Where(item => !item.Persona.Eliminado && item.Identidad.ProyectoID.Equals(pProyectoID) && item.Identidad.IdentidadID.Equals(pIdentidadID) && item.Identidad.FechaExpulsion.HasValue).ToList();

                    resultadoConsulta = consulta1.Select(item => new QueryTriples
                    {
                        Sujeto = $"<http://gnoss/{item.Identidad.IdentidadID.ToString().ToUpper()}>",
                        Predicado = hasPrivacidad,
                        Objeto = "\"publico\""
                    }).Union(consulta2.Select(item => new QueryTriples
                    {
                        Sujeto = $"<http://gnoss/{item.Identidad.IdentidadID.ToString().ToUpper()}>",
                        Predicado = hasPrivacidad,
                        Objeto = "\"publicoReg\""
                    })).Union(consulta3.Select(item => new QueryTriples
                    {
                        Sujeto = $"<http://gnoss/{item.Identidad.IdentidadID.ToString().ToUpper()}>",
                        Predicado = hasPrivacidad,
                        Objeto = "\"privado\""
                    })).Union(consulta4.Select(item => new QueryTriples
                    {
                        Sujeto = $"<http://gnoss/{item.Identidad.IdentidadID.ToString().ToUpper()}>",
                        Predicado = hasPrivacidad,
                        Objeto = "\"publico\""
                    })).Union(consulta5.Select(item => new QueryTriples
                    {
                        Sujeto = $"<http://gnoss/{item.Identidad.IdentidadID.ToString().ToUpper()}>",
                        Predicado = hasPrivacidad,
                        Objeto = "\"privado\""
                    })).Distinct().ToList();
                }
            }
            else
            {
                var consulta1 = mEntityContext.Identidad.JoinProyecto().JoinPerfil().JoinPersona().Where(item => !item.Persona.Eliminado && !item.Perfil.Eliminado && new int[] { 0, 1, 4 }.Contains(item.Identidad.Tipo) && !item.Identidad.ProyectoID.Equals(new Guid("11111111-1111-1111-1111-111111111111")) && item.Identidad.IdentidadID.Equals(pIdentidadID) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue).ToList();

                var consulta2 = mEntityContext.Identidad.JoinProyecto().JoinPerfil().JoinPersona().Where(item => !item.Persona.Eliminado && !item.Perfil.Eliminado && new int[] { 0, 1, 4 }.Contains(item.Identidad.Tipo) && item.Identidad.ProyectoID.Equals(new Guid("11111111-1111-1111-1111-111111111111")) && item.Identidad.IdentidadID.Equals(pIdentidadID) && item.Identidad.FechaExpulsion.HasValue).ToList();

                resultadoConsulta = consulta1.Select(item => new QueryTriples
                {
                    Sujeto = $"<http://gnoss/{item.Identidad.IdentidadID.ToString().ToUpper()}>",
                    Predicado = hasPrivacidad,
                    Objeto = "\"publico\""
                }).Union(consulta2.Select(item => new QueryTriples
                {
                    Sujeto = $"<http://gnoss/{item.Identidad.IdentidadID.ToString().ToUpper()}>",
                    Predicado = hasPrivacidad,
                    Objeto = "\"privado\""
                })).Distinct().ToList();
            }

            return resultadoConsulta;
        }

        /// <summary>
        /// Devuelve los triples referentes a la privacidad de las organizaciones
        /// </summary>        
        /// <param name="pIdentidadID">Id de la identidad</param>
        /// <param name="pProyectoID">Id del proyecto</param>
        /// <returns>Lista de triples referentes a la privacidad de las organizaciones</returns>
        public List<QueryTriples> ObtieneTripletasPrivacidadOrganizaciones(Guid pIdentidadID, Guid pProyectoID)
        {
            List<QueryTriples> resultadoConsulta;

            if (!pProyectoID.Equals(ProyectoAD.MetaProyecto))
            {
                var consulta1 = mEntityContext.Organizacion.JoinOrganizacionParticipaProy().JoinProyecto().JoinPerfil().JoinIdentidad().Where(item => !item.Organizacion.Eliminada && item.Identidad.Tipo == 3 && !item.Perfil.Eliminado && !item.Perfil.PersonaID.HasValue && item.Identidad.IdentidadID.Equals(pIdentidadID) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue).ToList();

                var consulta2 = mEntityContext.Organizacion.JoinOrganizacionParticipaProy().JoinProyecto().JoinPerfil().JoinIdentidad().Where(item => !item.Organizacion.Eliminada && item.Identidad.Tipo == 3 && !item.Perfil.Eliminado && !item.Perfil.PersonaID.HasValue && item.Identidad.IdentidadID.Equals(pIdentidadID) && item.Identidad.FechaExpulsion.HasValue).ToList();

                resultadoConsulta = consulta1.Select(item => new QueryTriples
                {
                    Sujeto = $"<http://gnoss/{item.Identidad.IdentidadID.ToString().ToUpper()}>",
                    Predicado = "<http://gnoss/hasprivacidadCom>",
                    Objeto = "\"publico\""
                }).Union(consulta2.Select(item => new QueryTriples
                {
                    Sujeto = $"<http://gnoss/{item.Identidad.IdentidadID.ToString().ToUpper()}>",
                    Predicado = "<http://gnoss/hasprivacidadCom>",
                    Objeto = "\"privado\""
                })).Distinct().ToList();
            }
            else
            {
                var consulta1 = mEntityContext.Organizacion.JoinOrganizacionParticipaProy().JoinProyecto().JoinPerfil().JoinIdentidad().Where(item => item.Organizacion.EsBuscableExternos && !item.Organizacion.Eliminada && item.Identidad.Tipo == 3 && !item.Perfil.Eliminado && !item.Perfil.PersonaID.HasValue && item.Proyecto.ProyectoID.Equals(new Guid("11111111-1111-1111-1111-111111111111")) && item.Identidad.IdentidadID.Equals(pIdentidadID)).ToList();

                var consulta2 = mEntityContext.Organizacion.JoinOrganizacionParticipaProy().JoinProyecto().JoinPerfil().JoinIdentidad().Where(item => !item.Organizacion.EsBuscableExternos && item.Organizacion.EsBuscable && !item.Organizacion.Eliminada && item.Identidad.Tipo == 3 && !item.Perfil.Eliminado && !item.Perfil.PersonaID.HasValue && item.Proyecto.ProyectoID.Equals(new Guid("11111111-1111-1111-1111-111111111111")) && item.Identidad.IdentidadID.Equals(pIdentidadID)).ToList();

                var consulta3 = mEntityContext.Organizacion.JoinOrganizacionParticipaProy().JoinProyecto().JoinPerfil().JoinIdentidad().Where(item => !item.Organizacion.EsBuscable && !item.Organizacion.EsBuscableExternos && !item.Organizacion.Eliminada && item.Identidad.Tipo == 3 && !item.Perfil.Eliminado && !item.Perfil.PersonaID.HasValue && item.Proyecto.ProyectoID.Equals(new Guid("11111111-1111-1111-1111-111111111111")) && item.Identidad.IdentidadID.Equals(pIdentidadID)).ToList();

                resultadoConsulta = consulta1.Select(item => new QueryTriples
                {
                    Sujeto = $"<http://gnoss/{item.Identidad.IdentidadID.ToString().ToUpper()}>",
                    Predicado = "<http://gnoss/hasprivacidadMyGnoss>",
                    Objeto = "\"publico\""
                }).Union(consulta2.Select(item => new QueryTriples
                {
                    Sujeto = $"<http://gnoss/{item.Identidad.IdentidadID.ToString().ToUpper()}>",
                    Predicado = "<http://gnoss/hasprivacidadMyGnoss>",
                    Objeto = "\"publicoReg\""
                })).Union(consulta3.Select(item => new QueryTriples
                {
                    Sujeto = $"<http://gnoss/{item.Identidad.IdentidadID.ToString().ToUpper()}>",
                    Predicado = "<http://gnoss/hasprivacidadMyGnoss>",
                    Objeto = "\"privado\""
                })).Distinct().ToList();
            }
            return resultadoConsulta;
        }

        /// <summary>
        /// Obtiene los triples referentes a la privacidad de los grupos
        /// </summary>
        /// <param name="pGrupoID">Id del grupo</param>
        /// <returns>Lista con los triples referentes a la privacidad de los grupos</returns>
        public List<QueryTriples> ObtieneTripletasPrivacidadGrupos(Guid pGrupoID)
        {
            List<QueryTriples> resultadoConsulta;

            var consulta1 = mEntityContext.GrupoIdentidades.Where(item => item.Publico && item.GrupoID.Equals(pGrupoID)).ToList();
            var consulta2 = mEntityContext.GrupoIdentidades.Where(item => !item.Publico && item.GrupoID.Equals(pGrupoID)).ToList();
            var consulta3 = mEntityContext.GrupoIdentidadesParticipacion.Where(item => item.GrupoID.Equals(pGrupoID)).ToList();
            var consulta4 = mEntityContext.GrupoIdentidadesParticipacion.JoinIdentidad().JoinPerfil().Where(item => item.GrupoIdentidadesParticipacion.GrupoID.Equals(pGrupoID)).ToList();

            resultadoConsulta = consulta1.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.GrupoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasprivacidadCom>",
                Objeto = "\"publico\""
            }).Union(consulta2.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.GrupoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasprivacidadCom>",
                Objeto = "\"privado\""
            })).Union(consulta3.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.GrupoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasparticipanteID>",
                Objeto = $"<http://gnoss/{item.IdentidadID.ToString().ToUpper()}>"
            })).Union(consulta4.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.GrupoIdentidadesParticipacion.GrupoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasparticipante>",
                Objeto = $"\"{RemplazarCaracteresBD(item.Perfil.NombrePerfil)}{RemplazarCaracteresBDSeparadorOrganizacion(item.Perfil.NombreOrganizacion)}\""
            })).Distinct().ToList();

            return resultadoConsulta;
        }

        /// <summary>
        /// Obtiene el nombre de una identidad
        /// </summary>
        /// <param name="pIdentidadID"></param>
        /// <returns></returns>
        public string ObtenerNombrePerfilIdentidad(Guid pIdentidadID)
        {
            return mEntityContext.Identidad.JoinPerfil().Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID)).ToList().Select(item => $"{RemplazarCaracteresBD(item.Perfil.NombrePerfil)}{RemplazarCaracteresBDSeparadorOrganizacion(item.Perfil.NombreOrganizacion)}").FirstOrDefault();
        }

        /// <summary>
        /// Obtiene informacion general de los recursos de una organizació
        /// </summary>        
        /// <param name="pDocumentoID">Identificador del documento</param>
        public List<QueryTriples> ObtieneInformacionGeneralRecursoOrganizacion(Guid pDocumentoID)
        {
            List<QueryTriples> resultadoConsulta;

            var consulta1 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosOrganizacion().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && !item.Documento.Borrador && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta2 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosOrganizacion().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID) && item.DocumentoWebVinBaseRecursos.TipoPublicacion.Equals(0) && item.Documento.ProyectoID.HasValue && item.Documento.ProyectoID.Value.Equals(new Guid("11111111-1111-1111-1111-111111111111"))).ToList();

            var consulta3 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosOrganizacion().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && !item.Documento.Borrador && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta4 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosOrganizacion().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && !item.Documento.Borrador && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta5 = mEntityContext.Identidad.JoinDocumentoRolIdentidad().JoinDocumento().JoinPerfil().JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosOrganizacion().Where(item => item.DocumentoRolIdentidad.Editor && !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && item.Identidad.ProyectoID.Equals(item.Documento.ProyectoID.Value) && (item.Identidad.Tipo == 0 || item.Identidad.Tipo == 4) && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta6 = mEntityContext.Identidad.JoinDocumentoRolIdentidad().JoinDocumento().JoinPerfil().JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosOrganizacion().Where(item => item.DocumentoRolIdentidad.Editor && !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && item.Identidad.ProyectoID.Equals(item.Documento.ProyectoID.Value) && (item.Identidad.Tipo == 2 || item.Identidad.Tipo == 3) && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta7 = mEntityContext.Identidad.JoinDocumentoRolIdentidad().JoinDocumento().JoinPerfil().JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosOrganizacion().Where(item => item.DocumentoRolIdentidad.Editor && !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && item.Identidad.ProyectoID.Equals(item.Documento.ProyectoID.Value) && item.Identidad.Tipo == 1 && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta8 = mEntityContext.Documento.JoinIdentidad().JoinPerfil().JoinDocumentoWebVinBaseRecursos().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && (item.Identidad.Tipo == 0 || item.Identidad.Tipo == 4) && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta9 = mEntityContext.Documento.JoinIdentidad().JoinPerfil().JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosOrganizacion().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && item.Identidad.Tipo == 1 && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta10 = mEntityContext.Documento.JoinIdentidad().JoinPerfil().JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosOrganizacion().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && (item.Identidad.Tipo == 2 || item.Identidad.Tipo == 3) && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta11 = mEntityContext.Documento.JoinIdentidad().JoinPerfil().JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosOrganizacion().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && (item.Identidad.Tipo == 0 && item.Identidad.Tipo == 4) && item.Documento.CreadorEsAutor && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta12 = mEntityContext.Documento.JoinIdentidad().JoinPerfil().JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosOrganizacion().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && item.Identidad.Tipo == 1 && item.Documento.CreadorEsAutor && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta13 = mEntityContext.Documento.JoinIdentidad().JoinPerfil().JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosOrganizacion().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && (item.Identidad.Tipo == 2 || item.Identidad.Tipo == 3) && item.Documento.CreadorEsAutor && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta14 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosOrganizacion().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta15 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosOrganizacion().Where(item => item.Documento.Rank_Tiempo.HasValue && !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && !item.Documento.Borrador && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta16 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosOrganizacion().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta17 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosOrganizacion().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta18 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosOrganizacion().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta19 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosOrganizacion().Where(item => new int[] { 2, 3, 6 }.Contains(item.Documento.Tipo) && !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && !item.Documento.Enlace.Substring(0, 4).Equals("http") && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta20 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosOrganizacion().Where(item => item.Documento.Tipo != 15 && item.Documento.Tipo != 16 && !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && item.Documento.Tipo != 5 && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta21 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosOrganizacion().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta22 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosOrganizacion().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta23 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosOrganizacion().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && !item.Documento.Borrador && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta24 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosOrganizacion().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.Borrador && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta25 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosOrganizacion().Where(item => !item.Documento.Eliminado && item.DocumentoWebVinBaseRecursos.TipoPublicacion == (short)TipoPublicacion.Publicado && !item.DocumentoWebVinBaseRecursos.Eliminado && !item.Documento.Borrador && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta26 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosOrganizacion().Where(item => !item.Documento.Eliminado && item.DocumentoWebVinBaseRecursos.TipoPublicacion != (short)TipoPublicacion.Publicado && !item.DocumentoWebVinBaseRecursos.Eliminado && !item.Documento.Borrador && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            string dateFormat = "yyyyMMddHHmmdd";

            resultadoConsulta = consulta1.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasnumeroVotos>",
                Objeto = $"{item.Documento.Valoracion.Value} ."
            }).Union(consulta2.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://rdfs.org/sioc/ns#has_space>",
                Objeto = $"<http://gnoss/{item.Documento.ProyectoID.ToString().ToUpper()} ."
            })).Union(consulta3.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasnumeroVisitas>",
                Objeto = $"{item.Documento.NumeroTotalConsultas} ."
            })).Union(consulta4.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasnumeroComentarios>",
                Objeto = $"{item.Documento.NumeroComentariosPublicos} ."
            })).Union(consulta5.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.DocumentoRolIdentidad.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/haseditor>",
                Objeto = $"\"{item.Perfil.NombrePerfil}\" ."
            })).Union(consulta6.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.DocumentoRolIdentidad.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/haseditor>",
                Objeto = $"\"{item.Perfil.NombreOrganizacion.ToString().ToUpper()}\" ."
            })).Union(consulta7.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.DocumentoRolIdentidad.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/haseditor>",
                Objeto = $"\"{item.Perfil.NombrePerfil} - {item.Perfil.NombreOrganizacion}\" ."
            })).Union(consulta8.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/haspublicador>",
                Objeto = $"\"{item.Perfil.NombrePerfil}\" ."
            })).Union(consulta9.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/haspublicador>",
                Objeto = $"\"{item.Perfil.NombrePerfil} - {item.Perfil.NombreOrganizacion}\" ."
            })).Union(consulta10.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/haspublicador>",
                Objeto = $"\"{item.Perfil.NombreOrganizacion}\" ."
            })).Union(consulta11.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasautor>",
                Objeto = $"\"{item.Perfil.NombrePerfil}\" ."
            })).Union(consulta12.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasautor>",
                Objeto = $"\"{item.Perfil.NombrePerfil} - {item.Perfil.NombreOrganizacion}\" ."
            })).Union(consulta13.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasautor>",
                Objeto = $"\"{item.Perfil.NombreOrganizacion}\" ."
            })).Union(consulta14.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://xmlns.com/foaf/0.1/firstName>",
                Objeto = $"\"{RemplazarCaracteresBD(item.Documento.Titulo.Replace("\"", ""))}\" ."
            })).Union(consulta15.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasPopularidad>",
                Objeto = $"{item.Documento.Rank_Tiempo.Value} ."
            })).Union(consulta16.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hastipodoc>",
                Objeto = $"\"{item.Documento.Tipo}\" ."
            })).Union(consulta17.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.DocumentoWebVinBaseRecursos.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasfechapublicacion>",
                Objeto = $"{item.DocumentoWebVinBaseRecursos.FechaPublicacion.Value.ToString(dateFormat)} ."
            })).Union(consulta18.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.DocumentoWebVinBaseRecursos.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasfechamodificacion>",
                Objeto = $"{item.Documento.FechaModificacion.Value.ToString(dateFormat)} ."
            })).Union(consulta19.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasextension>",
                Objeto = $"\"{item.Documento.Enlace.Substring(item.Documento.Enlace.LastIndexOf('.'))}\" ."
            })).Union(consulta20.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>",
                Objeto = $"\"RecursoPerfilPersonal\" ."
            })).Union(consulta21.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.DocumentoWebVinBaseRecursos.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasPerfilOrganizacion>",
                Objeto = $"<http://gnoss/{item.BaseRecursosOrganizacion.OrganizacionID.ToString().ToUpper()}> ."
            })).Union(consulta22.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://rdfs.org/sioc/ns#content>",
                Objeto = $"\"{RemplazarCaracteresBD(item.Documento.Descripcion)}\" ."
            })).Union(consulta23.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasEstadoPP>",
                Objeto = $"\"Publicado\" ."
            })).Union(consulta24.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasEstadoPP>",
                Objeto = $"\"Borrador\" ."
            })).Union(consulta25.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasOrigen>",
                Objeto = $"\"Publicado por mí\" ."
            })).Union(consulta26.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasOrigen>",
                Objeto = $"\"Publicado por otro\" ."
            })).Distinct().ToList();

            return resultadoConsulta;
        }

        /// <summary>
        /// Obtiene los triples referentes a la información general de los recusos personales
        /// </summary>
        /// <param name="pDocumentoID">Id del documento</param>
        public List<QueryTriples> ObtieneInformacionGeneralRecursoPersonal(Guid pDocumentoID)
        {
            List<QueryTriples> resultadoConsulta;

            var consulta1 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosUsuario().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && !item.Documento.Borrador && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta2 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID) && item.DocumentoWebVinBaseRecursos.TipoPublicacion == 0 && !item.Documento.ProyectoID.Equals(new Guid("11111111-1111-1111-1111-111111111111"))).ToList();

            var consulta3 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosUsuario().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && !item.Documento.Borrador && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta4 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosUsuario().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && !item.Documento.Borrador && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta5 = mEntityContext.Identidad.JoinDocumentoRolIdentidad().JoinDocumento().JoinPerfil().JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosUsuario().Where(item => item.DocumentoRolIdentidad.Editor && !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && item.Identidad.ProyectoID.Equals(item.Documento.ProyectoID) && (item.Identidad.Tipo == 0 || item.Identidad.Tipo == 4) && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta6 = mEntityContext.Identidad.JoinDocumentoRolIdentidad().JoinDocumento().JoinPerfil().JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosUsuario().Where(item => item.DocumentoRolIdentidad.Editor && !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && item.Identidad.ProyectoID.Equals(item.Documento.ProyectoID) && (item.Identidad.Tipo == 2 || item.Identidad.Tipo == 3) && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta7 = mEntityContext.Identidad.JoinDocumentoRolIdentidad().JoinDocumento().JoinPerfil().JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosUsuario().Where(item => item.DocumentoRolIdentidad.Editor && !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && item.Identidad.ProyectoID.Equals(item.Documento.ProyectoID) && item.Identidad.Tipo == 1 && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta8 = mEntityContext.Documento.JoinIdentidad().JoinPerfil().JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosUsuario().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && (item.Identidad.Tipo == 0 || item.Identidad.Tipo == 4) && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta9 = mEntityContext.Documento.JoinIdentidad().JoinPerfil().JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosUsuario().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && item.Identidad.Tipo == 1 && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta10 = mEntityContext.Documento.JoinIdentidad().JoinPerfil().JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosUsuario().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && (item.Identidad.Tipo == 2 || item.Identidad.Tipo == 3) && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta11 = mEntityContext.Documento.JoinIdentidad().JoinPerfil().JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosUsuario().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && (item.Identidad.Tipo == 0 || item.Identidad.Tipo == 4) && item.Documento.CreadorEsAutor && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta12 = mEntityContext.Documento.JoinIdentidad().JoinPerfil().JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosUsuario().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && item.Identidad.Tipo == 1 && item.Documento.CreadorEsAutor && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta13 = mEntityContext.Documento.JoinIdentidad().JoinPerfil().JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosUsuario().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && (item.Identidad.Tipo == 2 || item.Identidad.Tipo == 3) && item.Documento.CreadorEsAutor && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta14 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosUsuario().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta15 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosUsuario().Where(item => item.Documento.Rank_Tiempo.HasValue && !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && !item.Documento.Borrador && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta16 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosUsuario().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta17 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosUsuario().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta18 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosUsuario().Where(item => new int[] { 2, 3, 6 }.Contains(item.Documento.Tipo) && !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && !item.Documento.Enlace.Substring(0, 4).Equals("http") && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta19 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosUsuario().Where(item => item.Documento.Tipo != 15 && item.Documento.Tipo != 16 && !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta20 = mEntityContext.Persona.JoinPerfil().JoinBaseRecursosUsuario().JoinDocumentoWebVinBaseRecursos().JoinDocumento().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta21 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosUsuario().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta22 = mEntityContext.DocumentoWebVinBaseRecursos.JoinBaseRecursosUsuario().JoinPersona().JoinDocumento().JoinPerfil().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && !item.Documento.Borrador && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta23 = mEntityContext.DocumentoWebVinBaseRecursos.JoinBaseRecursosUsuario().JoinPersona().JoinDocumento().JoinPerfil().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.Borrador && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta24 = mEntityContext.DocumentoWebVinBaseRecursos.JoinBaseRecursosUsuario().JoinPersona().JoinDocumento().JoinPerfil().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && !item.Documento.Borrador && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID) && item.DocumentoWebVinBaseRecursos.TipoPublicacion == (short)TipoPublicacion.Publicado).ToList();

            var consulta25 = mEntityContext.DocumentoWebVinBaseRecursos.JoinBaseRecursosUsuario().JoinPersona().JoinDocumento().JoinPerfil().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && !item.Documento.Borrador && item.DocumentoWebVinBaseRecursos.TipoPublicacion != (short)TipoPublicacion.Publicado && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta26 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosUsuario().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList();

            string dateFormat = "yyyyMMddHHmmdd";

            resultadoConsulta = consulta1.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasnumeroVotos>",
                Objeto = $"{item.Documento.Valoracion} ."
            }).Union(consulta2.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://rdfs.org/sioc/ns#has_space>",
                Objeto = $"<http://gnoss/{item.Documento.ProyectoID.Value.ToString().ToUpper()}> ."
            })).Union(consulta3.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasnumeroVisitas>",
                Objeto = $"{item.Documento.NumeroTotalConsultas} ."
            })).Union(consulta4.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasnumeroComentarios>",
                Objeto = $"{item.Documento.NumeroComentariosPublicos} ."
            })).Union(consulta5.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.DocumentoRolIdentidad.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/haseditor>",
                Objeto = $"\"{item.Perfil.NombrePerfil}\" ."
            })).Union(consulta6.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.DocumentoRolIdentidad.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/haseditor>",
                Objeto = $"\"{item.Perfil.NombreOrganizacion}\" ."
            })).Union(consulta7.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.DocumentoRolIdentidad.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/haseditor>",
                Objeto = $"\"{item.Perfil.NombrePerfil} - {item.Perfil.NombreOrganizacion}\" ."
            })).Union(consulta8.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/haspublicador>",
                Objeto = $"\"{item.Perfil.NombrePerfil}\" ."
            })).Union(consulta9.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/haspublicador>",
                Objeto = $"\"{item.Perfil.NombrePerfil} - {item.Perfil.NombreOrganizacion}\" ."
            })).Union(consulta10.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/haspublicador>",
                Objeto = $"\"{item.Perfil.NombreOrganizacion}\" ."
            })).Union(consulta11.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasautor>",
                Objeto = $"\"{item.Perfil.NombrePerfil}\" ."
            })).Union(consulta12.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasautor>",
                Objeto = $"\"{item.Perfil.NombrePerfil} - {item.Perfil.NombreOrganizacion}\" ."
            })).Union(consulta13.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasautor>",
                Objeto = $"\"{item.Perfil.NombreOrganizacion}\" ."
            })).Union(consulta14.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://xmlns.com/foaf/0.1/firstName>",
                Objeto = $"\"{RemplazarCaracteresBD(item.Documento.Titulo.Replace("\"", ""))}\" ."
            })).Union(consulta15.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasPopularidad>",
                Objeto = $"{item.Documento.Rank_Tiempo.Value} ."
            })).Union(consulta16.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hastipodoc>",
                Objeto = $"\"{item.Documento.Tipo}\" ."
            })).Union(consulta17.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.DocumentoWebVinBaseRecursos.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasfechamodificacion>",
                Objeto = $"{item.Documento.FechaModificacion.Value.ToString(dateFormat)} ."
            })).Union(consulta18.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasextension>",
                Objeto = $"\"{item.Documento.Enlace.Substring(item.Documento.Enlace.LastIndexOf('.'))}\" ."
            })).Union(consulta19.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>",
                Objeto = $"\"RecursoPerfilPersonal\" ."
            })).Union(consulta20.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.DocumentoWebVinBaseRecursos.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasPerfilPersonal>",
                Objeto = $"<http://gnoss/{item.Perfil.PerfilID.ToString().ToUpper()}> ."
            })).Union(consulta21.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://rdfs.org/sioc/ns#content>",
                Objeto = $"\"{RemplazarCaracteresBD(item.Documento.Descripcion)}\" ."
            })).Union(consulta22.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasEstadoPP>",
                Objeto = $"\"Publicado\" ."
            })).Union(consulta23.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasEstadoPP>",
                Objeto = $"\"Borrador\" ."
            })).Union(consulta24.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasOrigen>",
                Objeto = $"\"Publicado por mí\" ."
            })).Union(consulta25.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasOrigen>",
                Objeto = $"\"Publicado por otro\" ."
            })).Union(consulta17.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasfechapublicacion>",
                Objeto = $"{item.DocumentoWebVinBaseRecursos.FechaPublicacion.Value.ToString(dateFormat)} ."
            })).Distinct().ToList();

            return resultadoConsulta;
        }

        /// <summary>
        /// Obtiene información de privacidad de myGnoss
        /// </summary>
        /// <param name="pProyectoID">Id del proyecto</param>
        public List<QueryTriples> ObtieneInformacionPrivacidadMyGnoss(string pProyectoID)
        {
            List<QueryTriples> resultadoQuery;

            var consulta1 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosProyecto().JoinProyecto().Where(item => !item.Documento.Publico && !item.Documento.Eliminado && item.Documento.Eliminado && item.Proyecto.ProyectoID.Equals(pProyectoID)).ToList();

            var consulta2 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinIdentidad().JoinPerfil().JoinConfiguracionGnossPersona().JoinBaseRecursosProyecto().JoinProyecto().Where(item => item.Documento.Publico && !item.Documento.Eliminado && item.Documento.UltimaVersion && item.ConfiguracionGnossPersona.VerRecursosExterno && item.Proyecto.ProyectoID.Equals(pProyectoID)).ToList();

            var consulta3 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosProyecto().JoinProyecto().Where(item => item.Documento.Publico && !item.Documento.Eliminado && item.Documento.UltimaVersion && (item.Proyecto.TipoAcceso == 2 || item.Proyecto.TipoAcceso == 0) && item.Proyecto.ProyectoID.Equals(pProyectoID)).ToList();

            var subconsulta1 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinIdentidad().JoinPerfil().JoinConfiguracionGnossPersona().JoinBaseRecursosProyecto().JoinProyecto().Where(item => item.Documento.Publico && !item.Documento.Eliminado && item.Documento.UltimaVersion && item.ConfiguracionGnossPersona.VerRecursosExterno).Select(item => item.Documento.DocumentoID).ToList();

            var subconsulta2 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosProyecto().JoinProyecto().Where(item => item.Documento.Publico && !item.Documento.Eliminado && item.Documento.UltimaVersion && (item.Proyecto.TipoAcceso == 2 || item.Proyecto.TipoAcceso == 0)).Select(item => item.Documento.DocumentoID).ToList();

            var subconsulta3 = mEntityContext.Documento.Where(item => !item.Publico && !item.Eliminado && item.UltimaVersion).Select(item => item.DocumentoID);


            List<Guid> listaDocumentosIds = subconsulta1.Union(subconsulta2).Union(subconsulta3).ToList();

            var consulta4 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosProyecto().JoinProyecto().Where(item => !item.Documento.Eliminado && item.Documento.UltimaVersion && item.Proyecto.ProyectoID.Equals(pProyectoID) && listaDocumentosIds.Contains(item.Documento.DocumentoID)).ToList();

            var consulta5 = mEntityContext.DocumentoWebVinBaseRecursos.JoinBaseRecursosProyecto().JoinDocumento().JoinProyecto().Where(item => !item.Documento.Eliminado && item.Documento.UltimaVersion && !item.DocumentoWebVinBaseRecursos.Eliminado && !item.Documento.Publico && (item.Proyecto.TipoAcceso == 1 || item.Proyecto.TipoAcceso == 3) && !item.DocumentoWebVinBaseRecursos.PrivadoEditores && item.Proyecto.ProyectoID.Equals(pProyectoID)).ToList();

            resultadoQuery = consulta1.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasprivacidadMyGnoss>",
                Objeto = "\"privado\" ."
            }).Union(consulta2.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasprivacidadMyGnoss>",
                Objeto = "\"publico\" ."
            })).Union(consulta3.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasprivacidadMyGnoss>",
                Objeto = "\"publico\" ."
            })).Union(consulta4.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasprivacidadMyGnoss>",
                Objeto = "\"publicoreg\" ."
            })).Union(consulta5.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasprivacidadMyGnoss>",
                Objeto = $"<http://gnoss/{item.Proyecto.ProyectoID.ToString().ToUpper()}> ."
            })).Distinct().ToList();

            return resultadoQuery;
        }

        /// <summary>
        /// Obtiene la información de privacidad de los recursos de MyGnoss
        /// </summary>
        /// <param name="pListaIdRecursos">Lista de los identificadores de los recursos a cargar</param>
        public void ObtieneInformacionPrivacidadRecursoMyGnoss(List<Guid> pListaIdRecursos)
        {
            List<QueryTriples> listaRecursos;

            if (pListaIdRecursos.Count > 0)
            {
                var consulta1 = mEntityContext.Documento.Where(item => !item.Publico && !item.Eliminado && item.UltimaVersion && pListaIdRecursos.Contains(item.DocumentoID)).ToList();

                var consulta2 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinIdentidad().JoinPerfil().JoinConfiguracionGnossPersona().Where(item => item.Documento.Publico && !item.Documento.Eliminado && item.Documento.UltimaVersion && item.ConfiguracionGnossPersona.VerRecursosExterno && pListaIdRecursos.Contains(item.Documento.DocumentoID)).ToList();

                var consulta3 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosProyecto().JoinProyecto().Where(item => item.Documento.Publico && !item.Documento.Eliminado && item.Documento.UltimaVersion && (item.Proyecto.TipoAcceso == 2 || item.Proyecto.TipoAcceso == 0) && pListaIdRecursos.Contains(item.Documento.DocumentoID)).ToList();

                var subconsulta1 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinIdentidad().JoinPerfil().JoinConfiguracionGnossPersona().Where(item => item.Documento.Publico && !item.Documento.Eliminado && item.Documento.UltimaVersion && item.ConfiguracionGnossPersona.VerRecursosExterno).Select(item => item.Documento.DocumentoID).ToList();

                var subconsulta2 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosProyecto().JoinProyecto().Where(item => item.Documento.Publico && !item.Documento.Eliminado && item.Documento.UltimaVersion && (item.Proyecto.TipoAcceso == 2 || item.Proyecto.TipoAcceso == 0)).Select(item => item.Documento.DocumentoID).ToList();

                var subconsulta3 = mEntityContext.Documento.Where(item => !item.Publico && !item.Eliminado && item.UltimaVersion).Select(item => item.DocumentoID).ToList();

                List<Guid> listaIDSubconsulta = subconsulta1.Union(subconsulta2).Union(subconsulta3).ToList();

                var consulta4 = mEntityContext.Documento.Where(item => !item.Eliminado && item.UltimaVersion && pListaIdRecursos.Contains(item.DocumentoID) && !listaIDSubconsulta.Contains(item.DocumentoID)).ToList();

                var consulta5 = mEntityContext.DocumentoWebVinBaseRecursos.JoinBaseRecursosProyecto().JoinDocumento().JoinProyecto().Where(item => !item.Documento.Eliminado && item.Documento.UltimaVersion && !item.DocumentoWebVinBaseRecursos.Eliminado && !item.Documento.Publico && (item.Proyecto.TipoAcceso == 1 || item.Proyecto.TipoAcceso == 3) && !item.DocumentoWebVinBaseRecursos.PrivadoEditores && pListaIdRecursos.Contains(item.Documento.DocumentoID)).ToList();

                listaRecursos = consulta1.Select(item => new QueryTriples
                {
                    Sujeto = $"<http://gnoss/{item.DocumentoID.ToString().ToUpper()}>",
                    Predicado = "<http://gnoss/hasprivacidadMyGnoss>",
                    Objeto = "\"privado\""
                }).Union(consulta2.Select(item => new QueryTriples
                {
                    Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                    Predicado = "<http://gnoss/hasprivacidadMyGnoss>",
                    Objeto = "\"publico\""
                })).Union(consulta3.Select(item => new QueryTriples
                {
                    Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                    Predicado = "<http://gnoss/hasprivacidadMyGnoss>",
                    Objeto = "\"publico\""
                })).Union(consulta4.Select(item => new QueryTriples
                {
                    Sujeto = $"<http://gnoss/{item.DocumentoID.ToString().ToUpper()}>",
                    Predicado = "<http://gnoss/hasprivacidadMyGnoss>",
                    Objeto = "\"publicoreg\""
                })).Union(consulta5.Select(item => new QueryTriples
                {
                    Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                    Predicado = "<http://gnoss/hasprivacidadMyGnoss>",
                    Objeto = $"<http://gnoss/{item.Proyecto.ProyectoID.ToString().ToUpper()}>"
                })).Distinct().ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DataSet"></param>
        /// <param name="idrec"></param>
        /// <param name="pProyectoID"></param>
        public List<QueryTriples> ObtieneInformacionComunRecurso(List<Guid> pListaRecursoId, Guid pProyectoID)
        {
            List<QueryTriples> resultadoConsulta = new List<QueryTriples>();
            string dateFormat = "yyyyMMddHHmmdd";

            if (pListaRecursoId.Count > 0)
            {
                if (!pProyectoID.Equals(ProyectoAD.MetaProyecto))
                {
                    var consultaDocumentoRolGrupoIdentidades = mEntityContext.Documento.JoinDocumentoDocumentoRolGrupoIdentidades().JoinGrupoIdentidades().Where(item => !item.Documento.Eliminado && item.Documento.UltimaVersion && pListaRecursoId.Contains(item.DocumentoRolGrupoIdentidades.DocumentoID))
                        .Select(x => new { x.Documento.DocumentoID , x.DocumentoRolGrupoIdentidades.Editor , x.DocumentoRolGrupoIdentidades.GrupoID , x.GrupoIdentidades.Nombre}).Distinct();

                    //DocumentoRolGrupoIdentidades
                    foreach (var resultado in consultaDocumentoRolGrupoIdentidades)
                    {
                        if (resultado.Editor)
                        {
                            resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{resultado.DocumentoID.ToString().ToUpper()}>", Predicado = "<http://gnoss/haseditorIdentidadID>", Objeto = $"<http://gnoss/{resultado.GrupoID.ToString().ToUpper()}> ." });
                            resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{resultado.DocumentoID.ToString().ToUpper()}>", Predicado = "<http://gnoss/haseditor>", Objeto = $"<http://gnoss/{resultado.Nombre}> ." });
                            resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{resultado.DocumentoID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasgrupoEditor>", Objeto = $"<http://gnoss/{resultado.Nombre}> ." });
                            resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{resultado.DocumentoID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasgrupoLector>", Objeto = $"<http://gnoss/{resultado.Nombre}> ." });
                        }
                        resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{resultado.DocumentoID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasparticipanteIdentidadID>", Objeto = $"<http://gnoss/{resultado.GrupoID.ToString().ToUpper()}> ." });
                        resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{resultado.DocumentoID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasparticipante>", Objeto = $"<http://gnoss/{resultado.Nombre}> ." });
                    }

                    var consultaDocumentoRolIdentidad = mEntityContext.Identidad.JoinDocumentoRolIdentidad().JoinDocumento().JoinPerfil().JoinProyecto().Where(item => !item.Documento.Eliminado && item.Proyecto.ProyectoID.Equals(pProyectoID) && pListaRecursoId.Contains(item.DocumentoRolIdentidad.DocumentoID) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue)
                        .Select(x => new { x.Documento.DocumentoID, x.DocumentoRolIdentidad.Editor ,x.Identidad.IdentidadID , x.Perfil.NombrePerfil , x.Perfil.NombreOrganizacion , x.Identidad.Tipo }).Distinct();

                    //DocumentoRolIdentidad
                    foreach (var resultado in consultaDocumentoRolIdentidad)
                    {
                        resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{resultado.DocumentoID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasparticipanteIdentidadID>", Objeto = $"<http://gnoss/{resultado.IdentidadID.ToString().ToUpper()}> ." });
                        if (resultado.Editor)
                        {
                            resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{resultado.DocumentoID.ToString().ToUpper()}>", Predicado = "<http://gnoss/haseditorIdentidadID>", Objeto = $"<http://gnoss/{resultado.IdentidadID.ToString().ToUpper()}> ." });
                            if (resultado.Tipo != 2)
                            {
                                resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{resultado.DocumentoID.ToString().ToUpper()}>", Predicado = "<http://gnoss/haseditor>", Objeto = $"\"{RemplazarCaracteresBD(resultado.NombrePerfil)}{RemplazarCaracteresBDSeparadorOrganizacion(resultado.NombreOrganizacion)}\" ." });
                            }
                            else
                            {
                                resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{resultado.DocumentoID.ToString().ToUpper()}>", Predicado = "<http://gnoss/haseditor>", Objeto = $"\"{RemplazarCaracteresBD(resultado.NombreOrganizacion)}\" ." });
                            }
                        }
                        else
                        {
                            if (resultado.Tipo != 2)
                            {
                                resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{resultado.DocumentoID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasparticipante>", Objeto = $"\"{RemplazarCaracteresBD(resultado.NombrePerfil)}{RemplazarCaracteresBDSeparadorOrganizacion(resultado.NombreOrganizacion)}\" ." });
                            }
                        }
                    }

                    var consultaDocumentoWebVinBaseRecursos = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosProyecto().JoinIdentidad().JoinPerfil().JoinProyecto().Where(item => !item.Documento.Eliminado && item.Documento.UltimaVersion && !item.DocumentoWebVinBaseRecursos.Eliminado && pListaRecursoId.Contains(item.DocumentoWebVinBaseRecursos.DocumentoID) && item.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID))
                        .Select(x=> new { x.Documento.DocumentoID, x.DocumentoWebVinBaseRecursos.PrivadoEditores , x.Documento.Visibilidad, x.Proyecto.TipoAcceso, x.Identidad.Tipo , x.DocumentoWebVinBaseRecursos.IdentidadPublicacionID , x.DocumentoWebVinBaseRecursos.FechaPublicacion , x.Documento.FechaModificacion , x.Documento.NumeroTotalConsultas , x.Perfil.NombrePerfil , x.Perfil.NombreOrganizacion , x.Documento.NumeroComentariosPublicos , x.DocumentoWebVinBaseRecursos.NumeroComentarios , x.DocumentoWebVinBaseRecursos.Rank_Tiempo }).Distinct();

                    //DocumentoWebVinBaseRecursos
                    foreach (var resultado in consultaDocumentoWebVinBaseRecursos)
                    {
                        resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{resultado.DocumentoID.ToString().ToUpper()}>", Predicado = "<http://gnoss/haspublicadorIdentidadID>", Objeto = $"<http://gnoss/{resultado.IdentidadPublicacionID.Value.ToString().ToUpper()}> ." });
                        resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{resultado.DocumentoID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasfechapublicacion>", Objeto = $"{resultado.FechaPublicacion.Value.ToString(dateFormat)} ." });
                        resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{resultado.DocumentoID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasfechamodificacion>", Objeto = $"{resultado.FechaModificacion.Value.ToString(dateFormat)} ." });
                        resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{resultado.DocumentoID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasnumeroVisitas>", Objeto = $"{resultado.NumeroTotalConsultas} ." });

                        if (resultado.Tipo != 2)
                        {
                            resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{resultado.DocumentoID.ToString().ToUpper()}>", Predicado = "<http://gnoss/haspublicador>", Objeto = $"\"{RemplazarCaracteresBD(resultado.NombrePerfil)}{RemplazarCaracteresBDSeparadorOrganizacion(resultado.NombreOrganizacion)}\" ." });
                        }
                        else
                        {
                            resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{resultado.DocumentoID.ToString().ToUpper()}>", Predicado = "<http://gnoss/haspublicador>", Objeto = $"\"{RemplazarCaracteresBD(resultado.NombreOrganizacion)}\" ." });
                        }
                        if (resultado.TipoAcceso == 2 || resultado.TipoAcceso == 0)
                        {
                            resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{resultado.DocumentoID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasnumeroComentarios>", Objeto = $"{resultado.NumeroComentariosPublicos} ." });
                        }
                        else
                        {
                            resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{resultado.DocumentoID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasnumeroComentarios>", Objeto = $"{resultado.NumeroComentariosPublicos + resultado.NumeroComentarios} ." });
                        }
                        if (resultado.Rank_Tiempo.HasValue)
                        {
                            resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{resultado.DocumentoID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasPopularidad>", Objeto = $"{resultado.Rank_Tiempo.Value} ." });
                        }
                        if (!resultado.PrivadoEditores)
                        {
                            if (resultado.Visibilidad == 0 || resultado.Visibilidad == 1)
                            {
                                resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{resultado.DocumentoID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasprivacidadCom>", Objeto = $"\"publico\" ." });
                            }
                            else if (resultado.Visibilidad == 2)
                            {
                                resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{resultado.DocumentoID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasprivacidadCom>", Objeto = $"\"publicoreg\" ." });
                            }
                        }
                        else
                        {
                            resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{resultado.DocumentoID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasprivacidadCom>", Objeto = $"\"privado\" ." });
                        }
                    }
                }
                else
                {
                    var consultaDocumento = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosProyecto().JoinIdentidadDocumento().JoinPerfil().Where(item => !item.Documento.Eliminado && item.Documento.UltimaVersion && pListaRecursoId.Contains(item.Documento.DocumentoID))
                        .Select(x => new { x.Documento.DocumentoID, x.Documento.Rank_Tiempo,x.Identidad.Tipo, x.Documento.Valoracion, x.Documento.NumeroTotalConsultas , x.Documento.NumeroComentariosPublicos , x.Documento.FechaCreacion, x.BaseRecursosProyecto.ProyectoID, x.Documento.Titulo, x.Perfil.NombrePerfil, x.Perfil.NombreOrganizacion, x.Documento.FechaModificacion}).Distinct();
                    string consulta = consultaDocumento.ToQueryString();
                    //Documento
                    foreach (var resultado in consultaDocumento)
                    {
                        if (resultado.Rank_Tiempo.HasValue)
                        {
                            resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{resultado.DocumentoID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasPopularidad>", Objeto = $"{resultado.Rank_Tiempo.Value} ." });
                        }
                        resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{resultado.DocumentoID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasnumeroVotos>", Objeto = $"{resultado.Valoracion} ." });
                        resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{resultado.DocumentoID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasnumeroVisitas>", Objeto = $"{resultado.NumeroTotalConsultas} ." });
                        resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{resultado.DocumentoID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasnumeroComentarios>", Objeto = $"{resultado.NumeroComentariosPublicos} ." });
                        resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{resultado.DocumentoID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasfechapublicacion>", Objeto = $"{resultado.FechaCreacion.Value.ToString(dateFormat)} ." });
                        resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{resultado.DocumentoID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasfechamodificacion>", Objeto = $"{resultado.FechaModificacion.Value.ToString(dateFormat)} ." });
                        resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{resultado.DocumentoID.ToString().ToUpper()}>", Predicado = "<http://rdfs.org/sioc/ns#has_space>", Objeto = $"<http://gnoss/{resultado.ProyectoID.ToString().ToUpper()}> ." });
                        resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{resultado.DocumentoID.ToString().ToUpper()}>", Predicado = "<http://xmlns.com/foaf/0.1/firstName>", Objeto = $"\"{RemplazarCaracteresBD(resultado.Titulo)}\"" }); //Quitar . para que procese bien los triples cuando vienen el titulo con "", problema con EDMA
                        if (resultado.Tipo != 2)
                        {
                            resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{resultado.DocumentoID.ToString().ToUpper()}>", Predicado = "<http://gnoss/haspublicador>", Objeto = $"\"{RemplazarCaracteresBD(resultado.NombrePerfil)}{RemplazarCaracteresBDSeparadorOrganizacion(resultado.NombreOrganizacion)}\" ." });
                        }
                        else
                        {
                            resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{resultado.DocumentoID.ToString().ToUpper()}>", Predicado = "<http://gnoss/haspublicador>", Objeto = $"\"{RemplazarCaracteresBD(resultado.NombreOrganizacion)}\"" });
                        }
                    }

                    //Identidad
                    var consultaIdentidad = mEntityContext.Identidad.JoinDocumentoRolIdentidad().JoinDocumento().JoinPerfil().JoinIdentidad().Where(item => !item.Documento.Eliminado && item.Documento.UltimaVersion && pListaRecursoId.Contains(item.Documento.DocumentoID) && item.IdentidadMyGnoss.ProyectoID.Equals(ProyectoAD.MetaProyecto))
                        .Select(x => new { x.Documento.DocumentoID, x.IdentidadMyGnoss.IdentidadID ,x.Perfil.NombrePerfil , x.Perfil.NombreOrganizacion, x.DocumentoRolIdentidad.Editor , x.Identidad.Tipo }).Distinct();
                    string consulta2 = consultaIdentidad.ToQueryString();
                    foreach (var resultado in consultaIdentidad)
                    {
                        resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{resultado.DocumentoID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasparticipanteIdentidadID>", Objeto = $"<http://gnoss/{resultado.IdentidadID.ToString().ToUpper()}> ." });
                        if (resultado.Editor)
                        {
                            resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{resultado.DocumentoID.ToString().ToUpper()}>", Predicado = "<http://gnoss/haseditorIdentidadID>", Objeto = $"<http://gnoss/{resultado.IdentidadID.ToString().ToUpper()}> ." });
                            if (resultado.Tipo != 2)
                            {
                                resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{resultado.DocumentoID.ToString().ToUpper()}>", Predicado = "<http://gnoss/haseditor>", Objeto = $"\"{RemplazarCaracteresBD(resultado.NombrePerfil)}{RemplazarCaracteresBDSeparadorOrganizacion(resultado.NombreOrganizacion)}\"" });
                            }
                            else
                            {
                                resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{resultado.DocumentoID.ToString().ToUpper()}>", Predicado = "<http://gnoss/haseditor>", Objeto = $"\"{RemplazarCaracteresBD(resultado.NombreOrganizacion)}\"" });
                            }
                        }
                    }

                    //Persona
                    var consultaPersona = mEntityContext.Persona.JoinPerfil().JoinBaseRecursosUsuario().JoinDocumentoWebVinBaseRecursos().JoinDocumento().Where(item => !item.Documento.Eliminado && item.Documento.UltimaVersion && pListaRecursoId.Contains(item.Documento.DocumentoID))
                        .Select(x=> new { x.Documento.DocumentoID , x.Perfil.PerfilID }).Distinct();
                    string consulta3 = consultaPersona.ToQueryString();
                    foreach (var resultado in consultaPersona)
                    {
                        resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{resultado.DocumentoID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasPerfilPersonal>", Objeto = $"<http://gnoss/{resultado.PerfilID.ToString().ToUpper()}> ." });
                    }
                }
            }

            return resultadoConsulta;
        }

        /// <summary>
        /// Ontiene la información extra de las preguntas
        /// </summary>
        /// <param name="pDocumentoID">Id del documento</param>
        /// <param name="pProyectoID">Id del proyecto</param>
        public List<QueryTriples> ObtieneInformacionExtraPregunta(Guid pDocumentoID, Guid pProyectoID)
        {
            List<QueryTriples> resultadoConsulta;

            var consulta1 = mEntityContext.DocumentoWebVinBaseRecursos.JoinBaseRecursosProyecto().JoinDocumento().Where(item => item.Documento.Tipo == 15 && item.DocumentoWebVinBaseRecursos.NumeroComentarios == 0 && item.DocumentoWebVinBaseRecursos.PermiteComentarios && item.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID) && item.Documento.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta2 = mEntityContext.DocumentoWebVinBaseRecursos.JoinBaseRecursosProyecto().JoinDocumento().Where(item => item.Documento.Tipo == 15 && item.DocumentoWebVinBaseRecursos.NumeroComentarios > 0 && item.DocumentoWebVinBaseRecursos.PermiteComentarios && item.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID) && item.Documento.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta3 = mEntityContext.DocumentoWebVinBaseRecursos.JoinBaseRecursosProyecto().JoinDocumento().Where(item => item.Documento.Tipo == 15 && item.DocumentoWebVinBaseRecursos.NumeroComentarios > 0 && !item.DocumentoWebVinBaseRecursos.PermiteComentarios && item.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID) && item.Documento.DocumentoID.Equals(pDocumentoID)).ToList();

            resultadoConsulta = consulta1.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasestado>",
                Objeto = "\"Sin respuestas\""
            }).Union(consulta2.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasestado>",
                Objeto = "\"Abierta\""
            })).Union(consulta3.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasestado>",
                Objeto = "\"Cerrada\""
            })).Distinct().ToList();

            return resultadoConsulta;
        }

        /// <summary>
        /// Obtiene la informacion extra de las encuestas
        /// </summary>
        /// <param name="pDocumentoID">Id del documento</param>
        /// <param name="pProyectoID">Id del proyecto</param>
        public List<QueryTriples> ObtieneInformacionExtraEncuesta(Guid pDocumentoID, Guid pProyectoID)
        {
            List<QueryTriples> resultadoConsulta;

            var consulta1 = mEntityContext.DocumentoWebVinBaseRecursos.JoinBaseRecursosProyecto().JoinDocumento().Where(item => item.Documento.Tipo == 18 && item.DocumentoWebVinBaseRecursos.PermiteComentarios && item.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID) && item.Documento.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta2 = mEntityContext.DocumentoWebVinBaseRecursos.JoinBaseRecursosProyecto().JoinDocumento().Where(item => item.Documento.Tipo == 18 && !item.DocumentoWebVinBaseRecursos.PermiteComentarios && item.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID) && item.Documento.DocumentoID.Equals(pDocumentoID)).ToList();

            resultadoConsulta = consulta1.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasestado>",
                Objeto = "\"Abierta\""
            }).Union(consulta2.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasestado>",
                Objeto = "\"Cerrada\""
            })).Distinct().ToList();

            return resultadoConsulta;
        }

        /// <summary>
        /// Obtiene información extra de los recursos referentes a las personas
        /// </summary>
        /// <param name="pDocumentoID">Id del recurso</param>
        /// <param name="pPerfilID">Id del perfil</param>
        public List<QueryTriples> ObtieneInformacionExtraRecursosContribucionesPer(Guid pDocumentoID, Guid pPerfilID)
        {
            List<QueryTriples> resultadosConsulta;

            var consultaDocumento1 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinIdentidad().JoinPerfil().JoinPerfil().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.DocumentoID.Equals(pDocumentoID) && item.PerfilPersona.PerfilID.Equals(pPerfilID) && !item.Identidad.ProyectoID.Equals(new Guid("11111111-1111-1111-1111-111111111111")));

            resultadosConsulta = consultaDocumento1.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://rdfs.org/sioc/ns#has_space>",
                Objeto = $"<http://gnoss/{item.Identidad.ProyectoID.ToString().ToUpper()}> ."
            }).Distinct().ToList();

            var consultaDocumento2_1 = mEntityContext.DocumentoWebVinBaseRecursos.JoinBaseRecursosUsuario().JoinPersona().JoinDocumento().JoinPerfil().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID) && item.Perfil.PerfilID.Equals(pPerfilID) && !item.Documento.ProyectoID.Value.Equals(new Guid("11111111-1111-1111-1111-111111111111"))).ToList();

            var consultaDocumento2_2 = mEntityContext.DocumentoWebVinBaseRecursos.JoinBaseRecursosUsuario().JoinPersona().JoinDocumento().JoinPerfil().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID) && item.Perfil.PerfilID.Equals(pPerfilID) && !item.Documento.ProyectoID.Value.Equals(new Guid("11111111-1111-1111-1111-111111111111")) && !(item.Documento.Tipo.Equals((short)TiposDocumentacion.Semantico) || item.Documento.Tipo.Equals((short)TiposDocumentacion.Pregunta) || item.Documento.Tipo.Equals((short)TiposDocumentacion.Debate)) && item.Documento.UltimaVersion).ToList();

            var consultaDocumento2_3 = mEntityContext.DocumentoWebVinBaseRecursos.JoinBaseRecursosUsuario().JoinPersona().JoinDocumento().JoinPerfil().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID) && item.Perfil.PerfilID.Equals(pPerfilID) && !item.Documento.ProyectoID.Value.Equals(new Guid("11111111-1111-1111-1111-111111111111")) && !(item.Documento.Tipo.Equals((short)TiposDocumentacion.Semantico) || item.Documento.Tipo.Equals((short)TiposDocumentacion.Pregunta) || item.Documento.Tipo.Equals((short)TiposDocumentacion.Debate) || item.Documento.Tipo.Equals((short)TiposDocumentacion.Encuesta)) && item.Documento.UltimaVersion && item.DocumentoWebVinBaseRecursos.TipoPublicacion != 0).ToList();

            resultadosConsulta = resultadosConsulta.Union(consultaDocumento2_1.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://rdfs.org/sioc/ns#has_space>",
                Objeto = $"<http://gnoss/{item.Documento.ProyectoID.Value.ToString().ToUpper()}> ."
            })).Union(consultaDocumento2_2.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>",
                Objeto = "\"RecursoPerfilPersonal\" ."
            })).Union(consultaDocumento2_3.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>",
                Objeto = "\"Recurso Compartido\" ."
            })).Distinct().ToList();

            var consultaDocumento2Bis_1 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosProyecto().JoinIdentidad().Where(item => item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID) && item.Identidad.PerfilID.Equals(pPerfilID) && !(item.Documento.Tipo == (short)TiposDocumentacion.Pregunta || item.Documento.Tipo == (short)TiposDocumentacion.Debate || item.Documento.Tipo == (short)TiposDocumentacion.Encuesta) && item.DocumentoWebVinBaseRecursos.TipoPublicacion == 0).ToList();

            var consultaDocumento2Bis_2 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosProyecto().JoinIdentidad().Where(item => item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID) && item.Identidad.PerfilID.Equals(pPerfilID) && !(item.Documento.Tipo == (short)TiposDocumentacion.Pregunta || item.Documento.Tipo == (short)TiposDocumentacion.Debate || item.Documento.Tipo == (short)TiposDocumentacion.Encuesta) && item.DocumentoWebVinBaseRecursos.TipoPublicacion != 0).ToList();

            var consultaDocumento2Bis_3 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosProyecto().JoinIdentidad().Where(item => item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID) && item.Identidad.PerfilID.Equals(pPerfilID) && item.Documento.Borrador).ToList();

            var consultaDocumento2Bis_4 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosProyecto().JoinIdentidad().Where(item => item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID) && item.Identidad.PerfilID.Equals(pPerfilID) && !item.Documento.Borrador).ToList();

            resultadosConsulta = resultadosConsulta.Union(consultaDocumento2Bis_1.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>",
                Objeto = $"\"Recurso Publicado\" ."
            })).Union(consultaDocumento2Bis_2.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>",
                Objeto = $"\"Recurso Compartido\" ."
            })).Union(consultaDocumento2Bis_3.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasEstado>",
                Objeto = $"\"Borrador\" ."
            })).Union(consultaDocumento2Bis_4.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasEstado>",
                Objeto = $"\"Publicado / Compartido\" ."
            })).Distinct().ToList();

            var consultaDocumento3_1 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosProyecto().JoinIdentidad().JoinPerfil().Where(item => !item.Documento.Eliminado && item.Documento.UltimaVersion && item.Documento.DocumentoID.Equals(pDocumentoID) && item.Perfil.PerfilID.Equals(pPerfilID) && !(item.Documento.Tipo == (short)TiposDocumentacion.Semantico || item.Documento.Tipo == (short)TiposDocumentacion.Pregunta || item.Documento.Tipo == (short)TiposDocumentacion.Debate || item.Documento.Tipo == (short)TiposDocumentacion.Encuesta)).ToList();

            var consultaDocumento3_2 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosProyecto().JoinIdentidad().JoinPerfil().Where(item => !item.Documento.Eliminado && item.Documento.UltimaVersion && item.Documento.DocumentoID.Equals(pDocumentoID) && item.Perfil.PerfilID.Equals(pPerfilID) && item.Documento.Tipo == (short)TiposDocumentacion.Debate).ToList();

            var consultaDocumento3_3 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosProyecto().JoinIdentidad().JoinPerfil().Where(item => !item.Documento.Eliminado && item.Documento.UltimaVersion && item.Documento.DocumentoID.Equals(pDocumentoID) && item.Perfil.PerfilID.Equals(pPerfilID) && item.Documento.Tipo == (short)TiposDocumentacion.Pregunta).ToList();

            var consultaDocumento3_4 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosProyecto().JoinIdentidad().JoinPerfil().Where(item => !item.Documento.Eliminado && item.Documento.UltimaVersion && item.Documento.DocumentoID.Equals(pDocumentoID) && item.Perfil.PerfilID.Equals(pPerfilID) && item.Documento.Tipo == (short)TiposDocumentacion.Encuesta).ToList();


            resultadosConsulta = resultadosConsulta.Union(consultaDocumento3_1.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>",
                Objeto = $"\"Recurso Publicado\" ."
            })).Union(consultaDocumento3_2.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>",
                Objeto = $"\"Contribuciones en Debates\" ."
            })).Union(consultaDocumento3_3.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>",
                Objeto = $"\"Contribuciones en Preguntas\" ."
            })).Union(consultaDocumento3_4.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>",
                Objeto = $"\"Contribuciones en Encuestas\" ."
            })).Distinct().ToList();

            return resultadosConsulta;
        }

        /// <summary>
        /// Obtiene la información extra de las contribuciones de los recursos referentes a organizaciones
        /// </summary>
        /// <param name="pDocumentoID">Id del recurso</param>
        /// <param name="pOrganizacionID">Id de la organización</param>
        public List<QueryTriples> ObtieneInformacionExtraRecursosContribucionesOrg(Guid pDocumentoID, Guid pOrganizacionID)
        {
            List<QueryTriples> resultadoConsulta;
            string dateFormat = "yyyyMMddHHmmdd";

            var consulta1 = mEntityContext.DocumentoWebVinBaseRecursos.JoinBaseRecursosProyecto().JoinDocumento().JoinIdentidad().JoinPerfil().Where(item => !item.Documento.Eliminado && item.Documento.UltimaVersion && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.DocumentoID.Equals(pDocumentoID) && item.Perfil.OrganizacionID.Value.Equals(pOrganizacionID)).ToList();

            var consulta2 = mEntityContext.DocumentoWebVinBaseRecursos.JoinBaseRecursosProyecto().JoinDocumento().JoinIdentidad().JoinPerfil().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID) && item.Perfil.OrganizacionID.Value.Equals(pOrganizacionID)).ToList();

            var consulta3 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosOrganizacion().Where(item => item.Documento.Tipo != 15 && item.Documento.Tipo != 16 && !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && item.Documento.Tipo != 5 && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID) && item.BaseRecursosOrganizacion.OrganizacionID.Equals(pOrganizacionID)).ToList();

            var consulta4 = mEntityContext.DocumentoWebVinBaseRecursos.JoinBaseRecursosProyecto().JoinDocumento().JoinIdentidad().JoinPerfil().Where(item => item.Documento.Tipo != 5 && item.Documento.Tipo != 15 && item.Documento.Tipo != 16 && item.Documento.Tipo != 18 && !item.Documento.Eliminado && item.Documento.UltimaVersion && item.DocumentoWebVinBaseRecursos.TipoPublicacion != (short)TipoPublicacion.Publicado && item.Documento.DocumentoID.Equals(pDocumentoID) && item.Perfil.OrganizacionID.Value.Equals(pOrganizacionID)).ToList();

            var consulta5 = mEntityContext.DocumentoWebVinBaseRecursos.JoinBaseRecursosProyecto().JoinDocumento().JoinIdentidad().JoinPerfil().Where(item => !item.Documento.Eliminado && item.Documento.Tipo != 5 && item.Documento.Tipo != 15 && item.Documento.Tipo != 16 && item.Documento.Tipo != 18 && item.Documento.UltimaVersion && item.Documento.DocumentoID.Equals(pDocumentoID) && item.Perfil.OrganizacionID.Value.Equals(pOrganizacionID)).ToList();

            var consulta6 = mEntityContext.DocumentoWebVinBaseRecursos.JoinBaseRecursosProyecto().JoinDocumento().JoinIdentidad().JoinPerfil().Where(item => !item.Documento.Eliminado && item.Documento.UltimaVersion && item.Documento.Tipo == 16 && item.Documento.DocumentoID.Equals(pDocumentoID) && item.Perfil.OrganizacionID.Value.Equals(pOrganizacionID)).ToList();

            var consulta7 = mEntityContext.DocumentoWebVinBaseRecursos.JoinBaseRecursosProyecto().JoinDocumento().JoinIdentidad().JoinPerfil().Where(item => !item.Documento.Eliminado && item.Documento.UltimaVersion && item.Documento.Tipo == 15 && item.Documento.DocumentoID.Equals(pDocumentoID) && item.Perfil.OrganizacionID.Value.Equals(pOrganizacionID)).ToList();

            var consulta8 = mEntityContext.DocumentoWebVinBaseRecursos.JoinBaseRecursosProyecto().JoinDocumento().JoinIdentidad().JoinPerfil().Where(item => !item.Documento.Eliminado && item.Documento.UltimaVersion && item.Documento.Tipo == 18 && item.Documento.DocumentoID.Equals(pDocumentoID) && item.Perfil.OrganizacionID.Value.Equals(pOrganizacionID)).ToList();

            var consulta9 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosUsuario().JoinIdentidad().JoinPerfil().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID) && item.Perfil.OrganizacionID.Value.Equals(pOrganizacionID)).ToList();

            var consulta10 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosUsuario().JoinIdentidad().JoinPerfil().Where(item => !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID) && item.Perfil.OrganizacionID.Value.Equals(pOrganizacionID)).ToList();

            resultadoConsulta = consulta1.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://rdfs.org/sioc/ns#has_space>",
                Objeto = $"<http://gnoss/{item.BaseRecursosProyecto.ProyectoID.ToString().ToUpper()}> ."
            }).Union(consulta2.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://rdfs.org/sioc/ns#has_space>",
                Objeto = $"<http://gnoss/11111111-1111-1111-1111-111111111111> ."
            })).Union(consulta3.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>",
                Objeto = $"\"RecursoPerfilPersonal\" ."
            })).Union(consulta4.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>",
                Objeto = $"\"Recurso Compartido\" ."
            })).Union(consulta5.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>",
                Objeto = $"\"Recurso Publicado\" ."
            })).Union(consulta6.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>",
                Objeto = $"\"Contribuciones en Debates\" ."
            })).Union(consulta7.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>",
                Objeto = $"\"Contribuciones en Preguntas\" ."
            })).Union(consulta8.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>",
                Objeto = $"\"Contribuciones en Encuestas\" ."
            })).Union(consulta9.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasfechapublicacion>",
                Objeto = $"{item.DocumentoWebVinBaseRecursos.FechaPublicacion.Value.ToString(dateFormat)} ."
            })).Union(consulta10.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasfechamodificacion>",
                Objeto = $"{item.Documento.FechaModificacion.Value.ToString(dateFormat)} ."
            })).Distinct().ToList();

            return resultadoConsulta;
        }

        /// <summary>
        /// Obtiene la información extra de las contribuciones de los recursos
        /// </summary>
        /// <param name="pListaRecursosID">Lista de los ids de los recursos</param>
        /// <param name="pPerfilID">Id del perfil</param>
        /// <param name="pProyectoID">Id del proyecto</param>
        public StringBuilder ObtieneInformacionExtraRecursosContribuciones(List<Guid> pListaRecursosID, Guid pPerfilID, Guid pProyectoID)
        {
            StringBuilder TripletasContribuciones = new StringBuilder();
            Dictionary<Guid, Dictionary<string, List<string>>> listaPropiedadesDocumentos = new Dictionary<Guid, Dictionary<string, List<string>>>();

            //Antes de hacer la query hay que comprobar que el documento no esta eliminado y es ultima version
            DocumentacionAD docAD = new DocumentacionAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperDocumentacion dataWrapperDocumentacion = docAD.ObtenerDocumentosPorID(pListaRecursosID, false);
            docAD.Dispose();

            if (dataWrapperDocumentacion.ListaDocumento.Where(x => x.UltimaVersion == true && x.Eliminado == false).ToList().Count != 0)
            {
                foreach (Documento document in dataWrapperDocumentacion.ListaDocumento.Where(x => x.UltimaVersion == true && x.Eliminado == false).ToList())
                {
                    //Primera parte: DataSetDocumento
                    string sujeto = "<http://gnoss/" + document.DocumentoID.ToString().ToUpper() + ">";
                    if (document.Tipo != (short)TiposDocumentacion.Semantico)
                    {
                        string predicado = "<http://gnoss/hastipodocExt>";
                        string objeto = $"{document.Tipo} .";
                        TripletasContribuciones.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto));
                    }

                    if ((document.Tipo == (short)TiposDocumentacion.Video || document.Tipo == (short)TiposDocumentacion.FicheroServidor || document.Tipo == (short)TiposDocumentacion.Imagen) && document.Enlace.Substring(0, 5).Equals("http"))
                    {
                        string predicado = "<http://gnoss/hasextension>";
                        string objeto = $"\"{document.Enlace.Substring(document.Enlace.LastIndexOf("."))}\" .";
                        TripletasContribuciones.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto));
                    }

                    if (!string.IsNullOrEmpty(document.Autor))
                    {
                        string predicado = "<http://gnoss/hasautor>";
                        string objeto = $"\"{document.Autor}\" .";
                        TripletasContribuciones.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto));
                    }

                    TripletasContribuciones.Append(FacetadoAD.GenerarTripleta(sujeto, "<http://gnoss/hasprivacidadMyGnoss>", "\"publico\" ."));

                    TripletasContribuciones.Append(FacetadoAD.GenerarTripleta(sujeto, "<http://xmlns.com/foaf/0.1/firstName>", $"\"{document.Titulo.ToLowerSearchGraph().Replace("\"", "")}\" ."));

                    if (document.TipoEntidad == 12 || document.TipoEntidad == 13)
                    {
                        string predicado = "<http://gnoss/hasEstado>";
                        string objeto = $"\"Autoguardado\" .";
                        TripletasContribuciones.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto));
                    }

                    if (document.Borrador == true)
                    {
                        string predicado = "<http://gnoss/hasEstado>";
                        string objeto = $"\"Borrador\" .";
                        TripletasContribuciones.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto));
                    }
                    else
                    {
                        string predicado = "<http://gnoss/hasEstado>";
                        string objeto = $"\"Publicado / Compartido\" .";
                        TripletasContribuciones.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto));
                    }

                    if (document.PrivadoEditores == true)
                    {
                        string predicado = "<http://gnoss/hasprivacidadCom>";
                        string objeto = $"\"privado\" .";
                        TripletasContribuciones.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto));
                    }
                    else if (document.Visibilidad == (short)VisibilidadDocumento.PrivadoMiembrosComunidad)
                    {
                        string predicado = "<http://gnoss/hasprivacidadCom>";
                        string objeto = $"\"publicoreg\" .";
                        TripletasContribuciones.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto));
                    }
                    else if (document.Visibilidad == (short)VisibilidadDocumento.Todos || document.Visibilidad == (short)VisibilidadDocumento.MiembrosComunidad)
                    {
                        string predicado = "<http://gnoss/hasprivacidadCom>";
                        string objeto = $"\"publico\" .";
                        TripletasContribuciones.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto));
                    }

                    if (document.Rank_Tiempo.HasValue)
                    {
                        string predicado = "<http://gnoss/hasPopularidad>";
                        string objeto = $"\"publico\" .";
                        TripletasContribuciones.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto));
                    }

                    TripletasContribuciones.Append(FacetadoAD.GenerarTripleta(sujeto, "<http://gnoss/hasfechapublicacion>", $"{document.FechaCreacion.Value.ToString("yyyyyMMddHHmmss")} ."));
                    TripletasContribuciones.Append(FacetadoAD.GenerarTripleta(sujeto, "<http://gnoss/hasfechamodificacion>", $"{document.FechaCreacion.Value.ToString("yyyyyMMddHHmmss")} ."));

                    //Segunda parte: DataSetDocumentoEstado
                    Guid baseRecursos = mEntityContext.BaseRecursosProyecto.Where(x => x.ProyectoID.Equals(pProyectoID)).Select(x => x.BaseRecursosID).FirstOrDefault();
                    var dataSetDocumentoEstado = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().Where(x => pListaRecursosID.Contains(x.Documento.DocumentoID) && x.DocumentoWebVinBaseRecursos.BaseRecursosID.Equals(baseRecursos));
                    if (dataSetDocumentoEstado.Where(x => (x.Documento.Tipo.Equals((short)TiposDocumentacion.Pregunta) || x.Documento.Tipo.Equals((short)TiposDocumentacion.Encuesta)) && x.DocumentoWebVinBaseRecursos.PermiteComentarios == true).Any())
                    {
                        string predicado = "<http://gnoss/hasestado>";
                        string objeto = $"\"Abierta\" .";
                        TripletasContribuciones.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto));
                    }
                    else if (dataSetDocumentoEstado.Where(x => (x.Documento.Tipo == (short)TiposDocumentacion.Pregunta || x.Documento.Tipo == (short)TiposDocumentacion.Encuesta) && x.DocumentoWebVinBaseRecursos.PermiteComentarios == false).Any())
                    {
                        string predicado = "<http://gnoss/hasestado>";
                        string objeto = $"\"Cerrada\" .";
                        TripletasContribuciones.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto));
                    }

                    if (dataSetDocumentoEstado.Where(x => (x.Documento.Tipo == (short)TiposDocumentacion.Pregunta || x.Documento.Tipo == (short)TiposDocumentacion.Encuesta) && x.DocumentoWebVinBaseRecursos.NumeroComentarios == 0).Any())
                    {
                        string predicado = "<http://gnoss/hasestado>";
                        string objeto = $"\"Sin respuestas\" .";
                        TripletasContribuciones.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto));
                    }

                    if (dataSetDocumentoEstado.Where(x => x.Documento.Tipo == (short)TiposDocumentacion.Debate && x.DocumentoWebVinBaseRecursos.PermiteComentarios == true).Any())
                    {
                        string predicado = "<http://gnoss/hasestado>";
                        string objeto = $"\"Abierto\" .";
                        TripletasContribuciones.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto));
                    }
                    else if (dataSetDocumentoEstado.Where(x => x.Documento.Tipo == (short)TiposDocumentacion.Debate && x.DocumentoWebVinBaseRecursos.PermiteComentarios == false).Any())
                    {
                        string predicado = "<http://gnoss/hasestado>";
                        string objeto = $"\"Cerrado\" .";
                        TripletasContribuciones.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto));
                    }

                    if (dataSetDocumentoEstado.Where(x => x.Documento.Tipo == (short)TiposDocumentacion.Debate && x.DocumentoWebVinBaseRecursos.NumeroComentarios == 0).Any())
                    {
                        string predicado = "<http://gnoss/hasestado>";
                        string objeto = $"\"Sin comentarios\" .";
                        TripletasContribuciones.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto));
                    }

                    if (dataSetDocumentoEstado.Where(x => !(x.Documento.Tipo == (short)TiposDocumentacion.Pregunta || x.Documento.Tipo == (short)TiposDocumentacion.Encuesta || x.Documento.Tipo == (short)TiposDocumentacion.Debate) && x.DocumentoWebVinBaseRecursos.TipoPublicacion == 0).Any())
                    {
                        string predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>";
                        string objeto = $"\"Recurso Publicado\" .";
                        TripletasContribuciones.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto));
                    }
                    else if (dataSetDocumentoEstado.Where(x => !(x.Documento.Tipo == (short)TiposDocumentacion.Pregunta || x.Documento.Tipo == (short)TiposDocumentacion.Encuesta || x.Documento.Tipo == (short)TiposDocumentacion.Debate) && x.DocumentoWebVinBaseRecursos.TipoPublicacion == 1).Any())
                    {
                        string predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>";
                        string objeto = $"\"Recurso Compartido\" .";
                        TripletasContribuciones.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto));
                    }

                    if (dataSetDocumentoEstado.Where(x => x.Documento.Tipo == (short)TiposDocumentacion.Pregunta).Any())
                    {
                        string predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>";
                        string objeto = $"\"{FacetadoAD.BUSQUEDA_CONTRIBUCIONES_PREGUNTA}\" .";
                        TripletasContribuciones.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto));
                    }
                    else if (dataSetDocumentoEstado.Where(x => x.Documento.Tipo == (short)TiposDocumentacion.Debate).Any())
                    {
                        string predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>";
                        string objeto = $"\"{FacetadoAD.BUSQUEDA_CONTRIBUCIONES_DEBATE}\" .";
                        TripletasContribuciones.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto));
                    }
                    else if (dataSetDocumentoEstado.Where(x => x.Documento.Tipo == (short)TiposDocumentacion.Encuesta).Any())
                    {
                        string predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>";
                        string objeto = $"\"{FacetadoAD.BUSQUEDA_CONTRIBUCIONES_ENCUESTA}\" .";
                        TripletasContribuciones.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto));
                    }

                    //Tercera parte: DataSetDocumentoPublicadoBRPersonal
                    var DataSetDocumentoPublicadoBRPersonal = mEntityContext.DocumentoWebVinBaseRecursos.JoinDocumentoWebVinBaseRecursosBaseRecursosUsuario().JoinPersona().JoinPerfil().Where(x => pListaRecursosID.Contains(x.DocumentoWebVinBaseRecursos.DocumentoID) && x.Perfil.PerfilID.Equals(pPerfilID));

                    if (dataSetDocumentoEstado.Any() && pProyectoID.Equals(ProyectoAD.MetaProyecto))
                    {
                        string predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>";
                        string objeto = $"\"RecursoPerfilPersonal\" .";
                        TripletasContribuciones.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto));
                    }

                    //Cuarta parte: DataSetDocumentoParticipanteEditor
                    var dataSetDocumentoParticipanteEditor = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosProyecto().JoinIdentidad().JoinPerfil().JoinDocumentoRolIdentidad().JoinIdentidad().JoinPerfil().Where(x => x.IdentidadEditor.ProyectoID.Equals(ProyectoAD.MetaProyecto) && x.DocumentoRolIdentidad.Editor == true && x.Identidad.FechaBaja == null && x.Identidad.FechaExpulsion == null && pListaRecursosID.Contains(x.Documento.DocumentoID) && x.DocumentoWebVinBaseRecursos.Eliminado == false)
                        .Select(x => new  { x.IdentidadEditor.IdentidadID, x.PerfilEditor.NombreOrganizacion, x.PerfilEditor.NombrePerfil, x.Identidad.Tipo }).Distinct();
                    string consulta = dataSetDocumentoParticipanteEditor.ToQueryString();
                    if (dataSetDocumentoParticipanteEditor.Any())
                    {
                        foreach (var fila in dataSetDocumentoParticipanteEditor.ToList())
                        {
                            TripletasContribuciones.Append(FacetadoAD.GenerarTripleta(sujeto, "<http://gnoss/haseditorIdentidadID>", $"<http://gnoss/{fila.IdentidadID.ToString().ToUpper()}>"));

                            if (fila.Tipo == (short)TiposIdentidad.ProfesionalCorporativo)
                            {
                                string objeto = "";
                                string predicado = "<http://gnoss/haseditor>";
                                if (!string.IsNullOrEmpty(fila.NombreOrganizacion))
                                {
                                    objeto = "\"" + fila.NombreOrganizacion.ToLowerSearchGraph() + "\"";
                                    TripletasContribuciones.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto));
                                }
                            }
                            else
                            {
                                string objeto = "";
                                string predicado = "<http://gnoss/haseditor>";
                                if (!string.IsNullOrEmpty(fila.NombrePerfil))
                                {
                                    objeto = fila.NombrePerfil;
                                }
                                if (!string.IsNullOrEmpty(fila.NombreOrganizacion))
                                {
                                    objeto = objeto + $" - {fila.NombreOrganizacion}";
                                }
                                if (!string.IsNullOrEmpty(objeto))
                                {
                                    objeto = "\"" + objeto + "\"";
                                    TripletasContribuciones.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto.ToLowerSearchGraph()));
                                }
                            }

                            TripletasContribuciones.Append(FacetadoAD.GenerarTripleta(sujeto, "<http://gnoss/hasparticipanteIdentidadID>", $"<http://gnoss/{fila.IdentidadID.ToString().ToUpper()}>"));

                            if (fila.Tipo == (short)TiposIdentidad.ProfesionalCorporativo)
                            {
                                string predicado = "<http://gnoss/hasparticipante>";
                                if (!string.IsNullOrEmpty(fila.NombreOrganizacion))
                                {
                                    TripletasContribuciones.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, $"\"{fila.NombreOrganizacion.ToLowerSearchGraph()}\" ."));
                                }
                            }
                            else
                            {
                                string objeto = "";
                                string predicado = "<http://gnoss/hasparticipante>";
                                if (!string.IsNullOrEmpty(fila.NombrePerfil))
                                {
                                    objeto = fila.NombrePerfil;
                                }
                                if (!string.IsNullOrEmpty(fila.NombreOrganizacion))
                                {
                                    objeto = objeto + $" - {fila.NombreOrganizacion}";
                                }
                                if (!string.IsNullOrEmpty(objeto))
                                {
                                    objeto = "\"" + objeto + "\"";
                                    TripletasContribuciones.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto.ToLowerSearchGraph()));
                                }
                            }
                        }
                    }

                    //Quinta parte: DataSetDocumentoPublicadorCompartidor
                    var dataSetDocumentoPublicadorCompartidor = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinIdentidad().JoinPerfil().JoinIdentidad().Where(x => pListaRecursosID.Contains(x.Documento.DocumentoID) && x.IdentidadMyGnoss.ProyectoID.Equals(ProyectoAD.MetaProyecto));

                    if (dataSetDocumentoPublicadorCompartidor.Any())
                    {
                        foreach (JoinDocumentoDocumentoWebVinBaseRecursosIdentidadPerfilIdentidad fila in dataSetDocumentoPublicadorCompartidor)
                        {

                            TripletasContribuciones.Append(FacetadoAD.GenerarTripleta(sujeto, "<http://gnoss/haspublicadorIdentidadID>", $"<http://gnoss/{fila.IdentidadMyGnoss.IdentidadID.ToString().ToUpper()}>"));

                            if (fila.Identidad.Tipo == (short)TiposIdentidad.ProfesionalCorporativo)
                            {
                                string predicado = "<http://gnoss/haspublicador>";
                                if (!string.IsNullOrEmpty(fila.Perfil.NombreOrganizacion))
                                {
                                    TripletasContribuciones.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, $"\"{fila.Perfil.NombreOrganizacion.ToLowerSearchGraph()}\" ."));
                                }
                            }
                            else
                            {
                                string objeto = "";
                                string predicado = "<http://gnoss/haspublicador>";
                                if (!string.IsNullOrEmpty(fila.Perfil.NombrePerfil))
                                {
                                    objeto = fila.Perfil.NombrePerfil;
                                }
                                if (!string.IsNullOrEmpty(fila.Perfil.NombreOrganizacion))
                                {
                                    objeto = objeto + $" - {fila.Perfil.NombreOrganizacion}";
                                }
                                if (!string.IsNullOrEmpty(objeto))
                                {
                                    objeto = "\"" + objeto + "\"";
                                    TripletasContribuciones.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto.ToLowerSearchGraph()));
                                }
                            }

                            if (fila.Identidad.Tipo == (short)TiposIdentidad.ProfesionalPersonal)
                            {
                                string objeto = "";
                                string predicado = "<http://gnoss/haspublicadorMostrarCom>";
                                if (!string.IsNullOrEmpty(fila.Perfil.NombrePerfil))
                                {
                                    objeto = fila.Perfil.NombrePerfil;
                                }
                                if (!string.IsNullOrEmpty(fila.Perfil.NombreOrganizacion))
                                {
                                    objeto = objeto + $" - {fila.Perfil.NombreOrganizacion}";
                                }
                                if (!string.IsNullOrEmpty(objeto))
                                {
                                    objeto = "\"" + objeto + "\"";
                                    TripletasContribuciones.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto.ToLowerSearchGraph()));
                                }
                            }

                            if (fila.Identidad.Tipo == (short)TiposIdentidad.ProfesionalCorporativo)
                            {
                                string predicado = "<http://gnoss/haspublicadorMostrarCom>";
                                if (!string.IsNullOrEmpty(fila.Perfil.NombreOrganizacion))
                                {
                                    TripletasContribuciones.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, $"\"{fila.Perfil.NombreOrganizacion.ToLowerSearchGraph()}\" ."));
                                }
                            }

                            if (fila.DocumentoWebVinBaseRecursos.TipoPublicacion == (short)TipoPublicacion.Publicado && fila.Perfil.OrganizacionID.HasValue)
                            {
                                string predicado = "<http://gnoss/hasSpaceIDPublicador>";
                                string objeto = $"<http://gnoss/{fila.Identidad.ProyectoID.ToString().ToUpper()}> .";
                                TripletasContribuciones.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto));
                            }

                            if (fila.DocumentoWebVinBaseRecursos.TipoPublicacion == (short)TipoPublicacion.Publicado && fila.Perfil.OrganizacionID.HasValue)
                            {
                                string predicado = "<http://gnoss/haspublicadorSpace>";
                                string objeto = "";
                                if (!string.IsNullOrEmpty(fila.Perfil.NombrePerfil))
                                {
                                    objeto = fila.Perfil.NombrePerfil;
                                }
                                if (!string.IsNullOrEmpty(fila.Perfil.NombreOrganizacion))
                                {
                                    objeto = objeto + $" - {fila.Perfil.NombreOrganizacion}";
                                }
                                if (!string.IsNullOrEmpty(objeto))
                                {
                                    objeto = "\"" + objeto + "\"";
                                    TripletasContribuciones.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto.ToLowerSearchGraph()));
                                }
                            }

                            if (fila.DocumentoWebVinBaseRecursos.TipoPublicacion == (short)TipoPublicacion.Publicado)
                            {
                                string predicado = "<http://gnoss/hasSpaceIDCompartidor>";
                                string objeto = $"<http://gnoss/{fila.Identidad.ProyectoID.ToString().ToUpper()}> .";
                                TripletasContribuciones.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto));
                            }

                            if (fila.DocumentoWebVinBaseRecursos.TipoPublicacion == (short)TipoPublicacion.Publicado)
                            {
                                string predicado = "<http://gnoss/haspublicadorSpace>";
                                string objeto = "";
                                if (!string.IsNullOrEmpty(fila.Perfil.NombrePerfil))
                                {
                                    objeto = fila.Perfil.NombrePerfil;
                                }
                                if (!string.IsNullOrEmpty(fila.Perfil.NombreOrganizacion))
                                {
                                    objeto = objeto + $" - {fila.Perfil.NombreOrganizacion}";
                                }
                                if (!string.IsNullOrEmpty(objeto))
                                {
                                    objeto = "\"" + objeto + "\"";
                                    TripletasContribuciones.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto.ToLowerSearchGraph()));
                                }
                            }
                        }
                    }
                }
            }

            return TripletasContribuciones;
        }

        /// <summary>
        /// Obtiene información extra de los comentarios de las contribuciones
        /// </summary>
        /// <param name="pComentarioId">Id del comentario a obtener su información</param>
        public List<QueryTriples> ObtieneInformacionExtraComentariosContribuciones(Guid pComentarioId)
        {
            List<QueryTriples> resultadoConsulta;
            string dateFormat = "yyyyMMddHHmmdd";

            var consulta1 = mEntityContext.Comentario.JoinIdentidad().JoinPerfil().Where(item => item.Comentario.ComentarioID.Equals(pComentarioId)).ToList();

            var consulta2 = mEntityContext.Comentario.JoinIdentidad().JoinDocumentoComentario().JoinDocumento().JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosProyecto().Where(item => !item.DocumentoWebVinBaseRecursos.PrivadoEditores && (item.Documento.Visibilidad == 0 || item.Documento.Visibilidad == 1) && item.Comentario.ComentarioID.Equals(pComentarioId)).ToList();

            var consulta3 = mEntityContext.Comentario.JoinIdentidad().JoinDocumentoComentario().JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosProyecto().Where(item => item.DocumentoWebVinBaseRecursos.PrivadoEditores && item.Comentario.ComentarioID.Equals(pComentarioId)).ToList();

            var consulta4 = mEntityContext.Comentario.JoinIdentidad().JoinDocumentoComentario().JoinDocumento().JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosProyecto().Where(item => !item.DocumentoWebVinBaseRecursos.PrivadoEditores && item.Documento.Visibilidad == 2 && item.Comentario.ComentarioID.Equals(pComentarioId)).ToList();

            var consulta5 = mEntityContext.Comentario.JoinIdentidad().Where(item => item.Comentario.ComentarioID.Equals(pComentarioId)).ToList();

            var consulta6 = mEntityContext.Comentario.JoinIdentidad().JoinPerfil().Where(item => item.Comentario.ComentarioID.Equals(pComentarioId)).ToList();

            var consulta7 = mEntityContext.Comentario.JoinIdentidad().JoinPerfil().JoinVotoComentario().Where(item => item.Comentario.ComentarioID.Equals(pComentarioId)).GroupBy(item => item.VotoComentario.ComentarioID);

            List<Guid> listaVotoComenario = mEntityContext.VotoComentario.Select(item => item.ComentarioID).ToList().ToList();

            var consulta8 = mEntityContext.Comentario.JoinIdentidad().JoinPerfil().Where(item => item.Comentario.ComentarioID.Equals(pComentarioId) && listaVotoComenario.Contains(item.Comentario.ComentarioID)).ToList();

            var consulta9 = mEntityContext.Comentario.JoinIdentidad().JoinPerfil().JoinDocumentoComentario().Where(item => item.Comentario.ComentarioID.Equals(pComentarioId)).ToList();

            var consulta10 = mEntityContext.Comentario.JoinIdentidad().JoinPerfil().JoinDocumentoComentario().JoinDocumentoWebVinBaseRecursos().JoinBaseRercursosProyecto().JoinDocumento().Where(item => !item.Documento.Eliminado && item.Documento.UltimaVersion && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Comentario.ComentarioID.Equals(pComentarioId) && item.Documento.Tipo != 15 && item.Documento.Tipo != 16 && item.Documento.Tipo != 18).ToList();

            var consulta11 = mEntityContext.Comentario.JoinIdentidad().JoinPerfil().JoinDocumentoComentario().JoinDocumentoWebVinBaseRecursos().JoinBaseRercursosProyecto().JoinDocumento().Where(item => !item.Documento.Eliminado && item.Documento.UltimaVersion && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Comentario.ComentarioID.Equals(pComentarioId) && item.Documento.Tipo == 15).ToList();

            var consulta12 = mEntityContext.Comentario.JoinIdentidad().JoinPerfil().JoinDocumentoComentario().JoinDocumentoWebVinBaseRecursos().JoinBaseRercursosProyecto().JoinDocumento().Where(item => !item.Documento.Eliminado && item.Documento.UltimaVersion && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Comentario.ComentarioID.Equals(pComentarioId) && item.Documento.Tipo == 16).ToList();

            var consulta13 = mEntityContext.Comentario.JoinIdentidad().JoinPerfil().JoinDocumentoComentario().JoinDocumentoWebVinBaseRecursos().JoinBaseRercursosProyecto().JoinDocumento().Where(item => !item.Documento.Eliminado && item.Documento.UltimaVersion && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Comentario.ComentarioID.Equals(pComentarioId) && item.Documento.Tipo == 18).ToList();

            var consulta14 = mEntityContext.Comentario.JoinIdentidad().JoinPerfil().JoinVotoComentario().Where(item => item.Comentario.ComentarioID.Equals(pComentarioId)).GroupBy(item => item.VotoComentario.ComentarioID);

            var consulta15 = mEntityContext.Comentario.JoinIdentidad().JoinPerfil().Where(item => item.Comentario.ComentarioID.Equals(pComentarioId)).ToList();

            var consulta16 = mEntityContext.Comentario.JoinIdentidad().JoinPerfil().JoinIdentidadMyGnoss().Where(item => item.Comentario.ComentarioID.Equals(pComentarioId) && item.Perfil.OrganizacionID.HasValue && item.IdentidadMyGnoss.ProyectoID.Equals(new Guid("11111111-1111-1111-1111-111111111111"))).ToList();

            var consulta17 = mEntityContext.Comentario.JoinIdentidad().JoinPerfil().JoinIdentidadMyGnoss().Where(item => item.Comentario.ComentarioID.Equals(pComentarioId) && item.IdentidadMyGnoss.ProyectoID.Equals(new Guid("11111111-1111-1111-1111-111111111111"))).ToList();

            var consulta18 = mEntityContext.Comentario.JoinIdentidad().JoinPerfil().JoinIdentidadMyGnoss().Where(item => item.Comentario.ComentarioID.Equals(pComentarioId) && item.IdentidadMyGnoss.ProyectoID.Equals(new Guid("11111111-1111-1111-1111-111111111111"))).ToList();

            var consulta19 = mEntityContext.Comentario.JoinIdentidad().JoinPerfil().JoinIdentidadMyGnoss().Where(item => item.Comentario.ComentarioID.Equals(pComentarioId) && item.IdentidadMyGnoss.ProyectoID.Equals(new Guid("11111111-1111-1111-1111-111111111111"))).ToList();

            var consulta20 = mEntityContext.Comentario.JoinIdentidad().JoinPerfil().JoinIdentidadMyGnoss().Where(item => item.Comentario.ComentarioID.Equals(pComentarioId) && item.IdentidadMyGnoss.ProyectoID.Equals(new Guid("11111111-1111-1111-1111-111111111111"))).ToList();

            var consulta21 = mEntityContext.Comentario.JoinIdentidad().JoinPerfil().JoinIdentidadMyGnoss().Where(item => item.Comentario.ComentarioID.Equals(pComentarioId) && item.IdentidadMyGnoss.ProyectoID.Equals(new Guid("11111111-1111-1111-1111-111111111111")) && item.Identidad.Tipo == 1).ToList();

            var consulta22 = mEntityContext.Comentario.JoinIdentidad().JoinPerfil().JoinIdentidadMyGnoss().Where(item => item.Comentario.ComentarioID.Equals(pComentarioId) && item.IdentidadMyGnoss.ProyectoID.Equals(new Guid("11111111-1111-1111-1111-111111111111"))).ToList();

            resultadoConsulta = consulta1.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Comentario.ComentarioID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasprivacidadMyGnoss>",
                Objeto = $"\"publico\" ."
            }).Union(consulta2.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Comentario.ComentarioID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasprivacidadCom>",
                Objeto = $"\"publico\" ."
            })).Union(consulta3.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Comentario.ComentarioID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasprivacidadCom>",
                Objeto = $"\"privado\" ."
            })).Union(consulta4.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Comentario.ComentarioID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasprivacidadCom>",
                Objeto = $"\"publicoreg\" ."
            })).Union(consulta5.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Comentario.ComentarioID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/haspublicadorIdentidadID>",
                Objeto = $"<http://gnoss/{item.Identidad.IdentidadID.ToString().ToUpper()}> ."
            })).Union(consulta6.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Comentario.ComentarioID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasfechapublicacion>",
                Objeto = $"{item.Comentario.Fecha.ToString(dateFormat)} ."
            })).Union(consulta7.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Key}>",
                Predicado = "<http://gnoss/hasnumeroVotos>",
                Objeto = $"{item.Count()} ."
            })).Union(consulta8.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Comentario.ComentarioID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasnumeroVotos>",
                Objeto = $" 0 ."
            })).Union(consulta9.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Comentario.ComentarioID.ToString().ToUpper()}>",
                Predicado = "<http://rdfs.org/sioc/ns#has_space>",
                Objeto = $" <http://gnoss/{item.DocumentoComentario.ProyectoID.Value.ToString().ToUpper()}> ."
            })).Union(consulta10.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Comentario.ComentarioID.ToString().ToUpper()}>",
                Predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>",
                Objeto = $" \"Comentarios en recursos\" ."
            })).Union(consulta11.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Comentario.ComentarioID.ToString().ToUpper()}>",
                Predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>",
                Objeto = $" \"Comentarios en preguntas\" ."
            })).Union(consulta12.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Comentario.ComentarioID.ToString().ToUpper()}>",
                Predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>",
                Objeto = $" \"Comentarios en debates\" ."
            })).Union(consulta13.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Comentario.ComentarioID.ToString().ToUpper()}>",
                Predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>",
                Objeto = $" \"Comentarios en encuestas\" ."
            })).Union(consulta14.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Key}>",
                Predicado = "<http://gnoss/hasnumeroVotos>",
                Objeto = $" {item.Count()} ."
            })).Union(consulta15.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Comentario.ComentarioID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasEstado>",
                Objeto = $" \"Publicado / Compartido\" ."
            })).Union(consulta16.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Comentario.ComentarioID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/haspublicadorIdentidadID>",
                Objeto = $" <http://gnoss/{item.IdentidadMyGnoss.IdentidadID.ToString().ToUpper()}> ."
            })).Union(consulta17.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Comentario.ComentarioID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasSpaceIDPublicador>",
                Objeto = $" <http://gnoss/{item.IdentidadMyGnoss.IdentidadID.ToString().ToUpper()}> ."
            })).Union(consulta18.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Comentario.ComentarioID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/haspublicadorSpace>",
                Objeto = $" <http://gnoss/{item.Identidad.ProyectoID.ToString().ToUpper()}> ."
            })).Union(consulta19.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Comentario.ComentarioID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/haspublicador>",
                Objeto = $" \"{RemplazarCaracteresBD(item.Perfil.NombrePerfil).ToLowerSearchGraph()}{RemplazarCaracteresBDSeparadorOrganizacion(item.Perfil.NombreOrganizacion).ToLowerSearchGraph()}\" ."
            })).Union(consulta20.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Comentario.ComentarioID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/haspublicador>",
                Objeto = $" \"{RemplazarCaracteresBD(item.Perfil.NombreOrganizacion).ToLowerSearchGraph()}\" ."
            })).Union(consulta21.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Comentario.ComentarioID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/haspublicadorMostrarCom>",
                Objeto = $" \"{RemplazarCaracteresBD(item.Perfil.NombrePerfil).ToLowerSearchGraph()}{RemplazarCaracteresBDSeparadorOrganizacion(item.Perfil.NombreOrganizacion).ToLowerSearchGraph()}\" ."
            })).Union(consulta22.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Comentario.ComentarioID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/haspublicadorMostrarCom>",
                Objeto = $" \"{RemplazarCaracteresBD(item.Perfil.NombreOrganizacion).ToLowerSearchGraph()}\" ."
            })).Distinct().ToList();

            return resultadoConsulta;
        }

        /// <summary>
        /// Obtiene información extra de una lista de recursos
        /// </summary>
        /// <param name="pListaIdRecursos">Lista con los identificadores de los recursos</param>
        /// <param name="pIdProyecto">Id del proyecto</param>
        public List<QueryTriples> ObtieneInformacionExtraRecurso(List<Guid> pListaIdRecursos, Guid pIdProyecto)
        {
            List<QueryTriples> resultadoTriples = new List<QueryTriples>();

            if (pListaIdRecursos.Count > 0)
            {
                //Comun
                var queryDocumentoComun = mEntityContext.Documento.Where(item => !item.Eliminado && item.UltimaVersion && pListaIdRecursos.Contains(item.DocumentoID));

                foreach (var query in queryDocumentoComun)
                {
                    resultadoTriples.Add(new QueryTriples { Sujeto = $"<http://gnoss/{query.DocumentoID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hastipodoc>", Objeto = $"\"{query.Tipo}\" ." });
                    if (!string.IsNullOrEmpty(query.Autor))
                    {
                        resultadoTriples.Add(new QueryTriples { Sujeto = $"<http://gnoss/{query.DocumentoID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasautor>", Objeto = $"\"{RemplazarCaracteresBD(query.Autor)}\" ." });
                    }
                }
            }

            return resultadoTriples;
        }


        /// <summary>
        /// Obtiene la información extra de los debates
        /// </summary>
        /// <param name="pDocumentoID">Id del documento del debate</param>
        /// <param name="pProyectoID">Id del proyecto</param>
        public List<QueryTriples> ObtieneInformacionExtraDebate(Guid pDocumentoID, Guid pProyectoID)
        {
            List<QueryTriples> resultadoConsulta;

            var consulta1 = mEntityContext.DocumentoWebVinBaseRecursos.JoinBaseRecursosProyecto().JoinDocumento().Where(item => item.Documento.Tipo == 16 && item.DocumentoWebVinBaseRecursos.NumeroComentarios == 0 && item.DocumentoWebVinBaseRecursos.PermiteComentarios && item.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID) && item.Documento.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta2 = mEntityContext.DocumentoWebVinBaseRecursos.JoinBaseRecursosProyecto().JoinDocumento().Where(item => item.Documento.Tipo == 16 && item.DocumentoWebVinBaseRecursos.NumeroComentarios > 0 && item.DocumentoWebVinBaseRecursos.PermiteComentarios && item.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID) && item.Documento.DocumentoID.Equals(pDocumentoID)).ToList();

            var consulta3 = mEntityContext.DocumentoWebVinBaseRecursos.JoinBaseRecursosProyecto().JoinDocumento().Where(item => item.Documento.Tipo == 16 && item.DocumentoWebVinBaseRecursos.NumeroComentarios > 0 && !item.DocumentoWebVinBaseRecursos.PermiteComentarios && item.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID) && item.Documento.DocumentoID.Equals(pDocumentoID)).ToList();

            resultadoConsulta = consulta1.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasestado>",
                Objeto = $"\"Sin comentarios .\""
            }).Union(consulta2.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasestado>",
                Objeto = $"\"Abierto .\""
            })).Union(consulta3.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Documento.DocumentoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasestado>",
                Objeto = $"\"Cerrado .\""
            })).Distinct().ToList();

            return resultadoConsulta;
        }

        /// <summary>
        /// Obtiene la información extra de los contactos
        /// </summary>
        /// <param name="pIdentidadID">Identidad id del usuario</param>
        /// <param name="pIdentidadAmigoID">Identidad id del contacto</param>
        public List<QueryTriples> ObtieneInformacionExtraContactos(string pIdentidadID, string pIdentidadAmigoID)
        {
            List<QueryTriples> resultadoConsulta;

            var consulta1 = mEntityContext.Amigo.Where(item => item.IdentidadID.Equals(pIdentidadID) && item.IdentidadAmigoID.Equals(pIdentidadAmigoID)).ToList();

            var consulta2 = mEntityContext.Amigo.Where(item => item.IdentidadID.Equals(pIdentidadID) && item.IdentidadAmigoID.Equals(pIdentidadAmigoID)).ToList();

            var consulta3 = mEntityContext.Amigo.JoinIdentidad().Where(item => new int[] { 0, 1, 4 }.Contains(item.Identidad.Tipo) && item.Amigo.IdentidadID.Equals(pIdentidadID) && item.Amigo.IdentidadAmigoID.Equals(pIdentidadAmigoID)).ToList();

            var consulta4 = mEntityContext.Amigo.JoinIdentidad().Where(item => new int[] { 0, 1, 4 }.Contains(item.Identidad.Tipo) && item.Amigo.IdentidadID.Equals(pIdentidadID) && item.Amigo.IdentidadAmigoID.Equals(pIdentidadAmigoID)).ToList();

            var consulta5 = mEntityContext.PermisoAmigoOrg.JoinIdentidad().Where(item => new int[] { 0, 1, 4 }.Contains(item.Identidad.Tipo) && item.PermisoAmigoOrg.IdentidadUsuarioID.Equals(pIdentidadID) && item.PermisoAmigoOrg.IdentidadAmigoID.Equals(pIdentidadAmigoID)).ToList();

            var consulta6 = mEntityContext.PermisoAmigoOrg.JoinIdentidad().Where(item => new int[] { 0, 1, 4 }.Contains(item.Identidad.Tipo) && item.PermisoAmigoOrg.IdentidadUsuarioID.Equals(pIdentidadID) && item.PermisoAmigoOrg.IdentidadAmigoID.Equals(pIdentidadAmigoID)).ToList();

            var consulta7 = mEntityContext.Amigo.JoinIdentidad().Where(item => new int[] { 2, 3 }.Contains(item.Identidad.Tipo) && item.Amigo.IdentidadID.Equals(pIdentidadID) && item.Amigo.IdentidadAmigoID.Equals(pIdentidadAmigoID)).ToList();

            var consulta8 = mEntityContext.Amigo.JoinIdentidad().Where(item => new int[] { 2, 3 }.Contains(item.Identidad.Tipo) && item.Amigo.IdentidadID.Equals(pIdentidadID) && item.Amigo.IdentidadAmigoID.Equals(pIdentidadAmigoID)).ToList();

            List<PerfilIdentidad> listaPerfilIdentidad = mEntityContext.Identidad.Where(item => item.IdentidadID.Equals(pIdentidadAmigoID) || item.IdentidadID.Equals(pIdentidadID)).Select(item => new PerfilIdentidad
            {
                PerfilID = item.PerfilID,
                IdentidadID = item.IdentidadID
            }).ToList();

            var consulta9 = mEntityContext.Identidad.Join(listaPerfilIdentidad, identidad => identidad.PerfilID, perfilIdentidad => perfilIdentidad.PerfilID, (identidad, perfilIdentidad) => new { Identidad = identidad, PerfilIdentidad = perfilIdentidad }).Join(mEntityContext.Proyecto, item => item.Identidad.ProyectoID, proyecto => proyecto.ProyectoID, (item, proyecto) => new { Identidad = item.Identidad, Proyecto = proyecto, PerfilIdentidad = item.PerfilIdentidad }).Where(item => new int[] { 0, 2 }.Contains(item.Proyecto.TipoAcceso) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue).ToList();

            var consulta10 = mEntityContext.Persona.JoinPerfil().JoinIdentidad().Where(item => !item.Perfil.Eliminado && !item.Persona.Eliminado && !item.Identidad.FechaBaja.HasValue && item.Identidad.ProyectoID.Equals(new Guid("11111111-1111-1111-1111-111111111111")) && new int[] { 0, 1, 4 }.Contains(item.Identidad.Tipo) && (item.Identidad.IdentidadID.Equals(pIdentidadID) || item.Identidad.IdentidadID.Equals(pIdentidadAmigoID))).ToList();

            var consulta11 = mEntityContext.Organizacion.JoinPerfil().JoinIdentidad().Where(item => !item.Organizacion.Eliminada && !item.Organizacion.OrganizacionID.Equals(new Guid("11111111-1111-1111-1111-111111111111")) && !item.Identidad.FechaBaja.HasValue && item.Identidad.ProyectoID.Equals(new Guid("11111111-1111-1111-1111-111111111111")) && item.Identidad.Tipo == 3 && (item.Identidad.IdentidadID.Equals(pIdentidadID) || item.Identidad.IdentidadID.Equals(pIdentidadAmigoID))).ToList();

            var consulta12 = mEntityContext.Persona.JoinPerfil().JoinIdentidad().Where(item => !item.Perfil.Eliminado && !item.Persona.Eliminado && !item.Identidad.FechaBaja.HasValue && item.Identidad.ProyectoID.Equals(new Guid("11111111-1111-1111-1111-111111111111")) && new int[] { 0, 1, 4 }.Contains(item.Identidad.Tipo) && (item.Identidad.IdentidadID.Equals(pIdentidadID) || item.Identidad.IdentidadID.Equals(pIdentidadAmigoID))).ToList();

            var consulta13 = mEntityContext.Persona.JoinPerfil().JoinIdentidad().Where(item => !item.Perfil.Eliminado && !item.Persona.Eliminado && !item.Identidad.FechaBaja.HasValue && item.Identidad.ProyectoID.Equals(new Guid("11111111-1111-1111-1111-111111111111")) && new int[] { 0, 1, 4 }.Contains(item.Identidad.Tipo) && (item.Identidad.IdentidadID.Equals(pIdentidadID) || item.Identidad.IdentidadID.Equals(pIdentidadAmigoID))).ToList();

            var consulta14 = mEntityContext.Organizacion.JoinPerfil().JoinIdentidad().Where(item => !item.Organizacion.Eliminada && !item.Organizacion.OrganizacionID.Equals(new Guid("11111111-1111-1111-1111-111111111111")) && !item.Identidad.FechaBaja.HasValue && item.Identidad.ProyectoID.Equals(new Guid("11111111-1111-1111-1111-111111111111")) && item.Identidad.Tipo == 3 && (item.Identidad.IdentidadID.Equals(pIdentidadID) || item.Identidad.IdentidadID.Equals(pIdentidadAmigoID))).ToList();

            resultadoConsulta = consulta1.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.IdentidadAmigoID.ToString().ToUpper()}>",
                Predicado = "<http://xmlns.com/foaf/0.1/knows>",
                Objeto = $"<http://gnoss/{item.IdentidadID}> ."
            }).Union(consulta2.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.IdentidadID.ToString().ToUpper()}>",
                Predicado = "<http://xmlns.com/foaf/0.1/knows>",
                Objeto = $"<http://gnoss/{item.IdentidadAmigoID.ToString().ToUpper()}> ."
            })).Union(consulta3.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Amigo.IdentidadAmigoID.ToString().ToUpper()}>",
                Predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>",
                Objeto = $"\"ContactoPer\" ."
            })).Union(consulta4.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Amigo.IdentidadID.ToString().ToUpper()}>",
                Predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>",
                Objeto = $"\"ContactoPer\" ."
            })).Union(consulta5.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.PermisoAmigoOrg.IdentidadAmigoID.ToString().ToUpper()}>",
                Predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>",
                Objeto = $"\"ContactoPer\" ."
            })).Union(consulta6.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.PermisoAmigoOrg.IdentidadUsuarioID.ToString().ToUpper()}>",
                Predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>",
                Objeto = $"\"ContactoPer\" ."
            })).Union(consulta7.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Amigo.IdentidadAmigoID.ToString().ToUpper()}>",
                Predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>",
                Objeto = $"\"ContactoOrg\" ."
            })).Union(consulta8.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Amigo.IdentidadID.ToString().ToUpper()}>",
                Predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>",
                Objeto = $"\"ContactoOrg\" ."
            })).Union(consulta9.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.PerfilIdentidad.IdentidadID.ToString().ToUpper()}>",
                Predicado = "<http://rdfs.org/sioc/ns#has_space>",
                Objeto = $"<http://gnoss/{item.Identidad.ProyectoID.ToString().ToUpper()}> ."
            })).Union(consulta10.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Identidad.IdentidadID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasnombrecompleto>",
                Objeto = $"\"{item.Persona.Nombre.ToLowerSearchGraph()} {item.Persona.Apellidos.ToLowerSearchGraph()}\" ."
            })).Union(consulta11.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Identidad.IdentidadID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/hasnombrecompleto>",
                Objeto = $"\"{item.Organizacion.Nombre.ToLowerSearchGraph()}\" ."
            })).Union(consulta12.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Identidad.IdentidadID.ToString().ToUpper()}>",
                Predicado = "<http://xmlns.com/foaf/0.1/firstName>",
                Objeto = $"\"{item.Persona.Nombre.ToLowerSearchGraph()}\" ."
            })).Union(consulta13.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Identidad.IdentidadID.ToString().ToUpper()}>",
                Predicado = "<http://xmlns.com/foaf/0.1/familyName>",
                Objeto = $"\"{item.Persona.Apellidos.ToLowerSearchGraph()}\" ."
            })).Union(consulta14.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Identidad.IdentidadID.ToString().ToUpper()}>",
                Predicado = "<http://xmlns.com/foaf/0.1/firstName>",
                Objeto = $"\"{item.Organizacion.Nombre.ToLowerSearchGraph()}\" ."
            })).Distinct().ToList();

            return resultadoConsulta;
        }

        public class PerfilIdentidad
        {
            public Guid PerfilID { get; set; }
            public Guid IdentidadID { get; set; }
        }

        public List<QueryTriples> ObtieneInformacionExtraInvitaciones(string pInvitacionID)
        {
            List<QueryTriples> resultadoConsutla;
            string dateFormat = "yyyyMMddHHmmdd";

            var consulta1 = mEntityContext.Invitacion.Where(item => new int[] { 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54 }.Contains(item.TipoInvitacion) && item.InvitacionID.Equals(pInvitacionID)).ToList();

            var consulta2 = mEntityContext.Invitacion.Where(item => new int[] { 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 58 }.Contains(item.TipoInvitacion) && item.InvitacionID.Equals(pInvitacionID)).ToList();

            var consulta3 = mEntityContext.Invitacion.JoinIdentidad().JoinPerfil().Where(item => new int[] { 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 58 }.Contains(item.Invitacion.TipoInvitacion) && item.Identidad.Tipo == 2 && item.Invitacion.InvitacionID.Equals(pInvitacionID)).ToList();

            var consulta4 = mEntityContext.Invitacion.JoinIdentidad().JoinPerfil().Where(item => new int[] { 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 58 }.Contains(item.Invitacion.TipoInvitacion) && item.Identidad.Tipo != 2 && item.Invitacion.InvitacionID.Equals(pInvitacionID)).ToList();

            var consulta5 = mEntityContext.Invitacion.Where(item => new int[] { 41, 42, 52, 53 }.Contains(item.TipoInvitacion) && item.InvitacionID.Equals(pInvitacionID)).ToList();

            var consulta6 = mEntityContext.Invitacion.Where(item => new int[] { 43, 44, 49 }.Contains(item.TipoInvitacion) && item.InvitacionID.Equals(pInvitacionID)).ToList();

            var consulta7 = mEntityContext.Invitacion.Where(item => new int[] { 45, 46, 47, 48, 50, 51, 58 }.Contains(item.TipoInvitacion) && item.InvitacionID.Equals(pInvitacionID)).ToList();

            resultadoConsutla = consulta1.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.InvitacionID.ToString().ToUpper()}>",
                Predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>",
                Objeto = "\"Invitacion\" ."
            }).Union(consulta2.Select(item => new QueryTriples 
            {
                Sujeto = $"<http://gnoss/{item.InvitacionID.ToString().ToUpper()}>",
                Predicado = "<http://purl.org/dc/elements/1.1/date>",
                Objeto = $"{item.FechaInvitacion.ToString(dateFormat)} ."
            })).Union(consulta3.Select(item => new QueryTriples 
            {
                Sujeto = $"<http://gnoss/{item.Invitacion.InvitacionID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/to>",
                Objeto = $"\"{RemplazarCaracteresBD(item.Perfil.NombreOrganizacion)}\" ."
            })).Union(consulta4.Select(item => new QueryTriples 
            {
                Sujeto = $"<http://gnoss/{item.Invitacion.InvitacionID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/to>",
                Objeto = $"\"{RemplazarCaracteresBD(item.Perfil.NombrePerfil)}\" ."
            })).Union(consulta5.Select(item => new QueryTriples 
            {
                Sujeto = $"<http://gnoss/{item.InvitacionID.ToString().ToUpper()}>",
                Predicado = "<http://purl.org/dc/elements/1.1/type>",
                Objeto = $"\"Contacto\" ."
            })).Union(consulta6.Select(item => new QueryTriples 
            {
                Sujeto = $"<http://gnoss/{item.InvitacionID.ToString().ToUpper()}>",
                Predicado = "<http://purl.org/dc/elements/1.1/type>",
                Objeto = $"\"Comunidad\" ."
            })).Union(consulta7.Select(item => new QueryTriples 
            {
                Sujeto = $"<http://gnoss/{item.InvitacionID.ToString().ToUpper()}>",
                Predicado = "<http://purl.org/dc/elements/1.1/type>",
                Objeto = $"\"Organizacion\" ."
            })).Distinct().ToList();

            return resultadoConsutla;
        }

        /// <summary>
        /// Obtiene la información extra que hace falta para las facetas de suscripciones.
        /// </summary>
        /// <param name="pSuscripcionID"></param>
        /// <param name="pRecursoID"></param>
        public List<QueryTriples> ObtieneInformacionExtraSuscripciones(Guid pSuscripcionID, Guid pRecursoID)
        {
            List<QueryTriples> resultadoConsulta;
            string dateFormat = "yyyyMMddHHmmdd";

            var consulta1 = mEntityContext.ResultadoSuscripcion.JoinSuscripcion().Where(item => item.Suscripcion.SuscripcionID.Equals(pSuscripcionID) && item.ResultadoSuscripcion.RecursoID.Equals(pRecursoID)).ToList();

            var consulta2 = mEntityContext.ResultadoSuscripcion.JoinSuscripcion().Where(item => item.ResultadoSuscripcion.TipoResultado == 0 && item.Suscripcion.SuscripcionID.Equals(pSuscripcionID) && item.ResultadoSuscripcion.RecursoID.Equals(pRecursoID)).ToList();

            var consulta3 = mEntityContext.ResultadoSuscripcion.JoinSuscripcion().Where(item => new int[] { 2, 4 }.Contains(item.ResultadoSuscripcion.TipoResultado) && item.Suscripcion.SuscripcionID.Equals(pSuscripcionID) && item.ResultadoSuscripcion.RecursoID.Equals(pRecursoID)).ToList();

            var consulta4 = mEntityContext.ResultadoSuscripcion.JoinSuscripcion().Where(item => item.ResultadoSuscripcion.TipoResultado == 0 && item.Suscripcion.SuscripcionID.Equals(pSuscripcionID) && item.ResultadoSuscripcion.RecursoID.Equals(pRecursoID)).ToList();

            var consulta5 = mEntityContext.ResultadoSuscripcion.JoinSuscripcion().Where(item => new int[] { 2, 4 }.Contains(item.ResultadoSuscripcion.TipoResultado) && item.Suscripcion.SuscripcionID.Equals(pSuscripcionID) && item.ResultadoSuscripcion.RecursoID.Equals(pRecursoID)).ToList();

            var consulta6 = mEntityContext.ResultadoSuscripcion.JoinSuscripcion().Where(item => item.ResultadoSuscripcion.Leido && item.Suscripcion.SuscripcionID.Equals(pSuscripcionID) && item.ResultadoSuscripcion.RecursoID.Equals(pRecursoID)).ToList();

            var consulta7 = mEntityContext.ResultadoSuscripcion.JoinSuscripcion().Where(item => !item.ResultadoSuscripcion.Leido && item.Suscripcion.SuscripcionID.Equals(pSuscripcionID) && item.ResultadoSuscripcion.RecursoID.Equals(pRecursoID)).ToList();

            var consulta8 = mEntityContext.ResultadoSuscripcion.JoinSuscripcion().Where(item => item.ResultadoSuscripcion.Sincaducidad && item.Suscripcion.SuscripcionID.Equals(pSuscripcionID) && item.ResultadoSuscripcion.RecursoID.Equals(pRecursoID)).ToList();

            resultadoConsulta = consulta1.Select(item => new QueryTriples 
            {
                Sujeto = $"<http://gnoss/{item.ResultadoSuscripcion.SuscripcionID.ToString().ToUpper()}_{item.ResultadoSuscripcion.RecursoID.ToString().ToUpper()}>",
                Predicado = "<http://purl.org/dc/elements/1.1/date>",
                Objeto = $"{item.ResultadoSuscripcion.FechaModificacion.ToString(dateFormat)} ."
            }).Union(consulta2.Select(item => new QueryTriples 
            {
                Sujeto = $"<http://gnoss/{item.ResultadoSuscripcion.SuscripcionID.ToString().ToUpper()}_{item.ResultadoSuscripcion.RecursoID.ToString().ToUpper()}>",
                Predicado = "<http://purl.org/dc/elements/1.1/type>",
                Objeto = $"\"Comunidades\" ."
            })).Union(consulta3.Select(item => new QueryTriples 
            {
                Sujeto = $"<http://gnoss/{item.ResultadoSuscripcion.SuscripcionID.ToString().ToUpper()}_{item.ResultadoSuscripcion.RecursoID.ToString().ToUpper()}>",
                Predicado = "<http://purl.org/dc/elements/1.1/type>",
                Objeto = $"\"Personas\" ."
            })).Union(consulta4.Select(item => new QueryTriples 
            {
                Sujeto = $"<http://gnoss/{item.ResultadoSuscripcion.SuscripcionID.ToString().ToUpper()}_{item.ResultadoSuscripcion.RecursoID.ToString().ToUpper()}>",
                Predicado = "<http://rdfs.org/sioc/ns#has_space>",
                Objeto = $"\"{item.ResultadoSuscripcion.OrigenNombre.ToLowerSearchGraph()}\" ."
            })).Union(consulta5.Select(item => new QueryTriples 
            {
                Sujeto = $"<http://gnoss/{item.ResultadoSuscripcion.SuscripcionID.ToString().ToUpper()}_{item.ResultadoSuscripcion.RecursoID.ToString().ToUpper()}>",
                Predicado = "<http://xmlns.com/foaf/0.1/firstName>",
                Objeto = $"\"{item.ResultadoSuscripcion.OrigenNombre.ToLowerSearchGraph()}\" ."
            })).Union(consulta6.Select(item => new QueryTriples 
            {
                Sujeto = $"<http://gnoss/{item.ResultadoSuscripcion.SuscripcionID.ToString().ToUpper()}_{item.ResultadoSuscripcion.RecursoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/Estado>",
                Objeto = $"\"Leidas\" ."
            })).Union(consulta7.Select(item => new QueryTriples 
            {
                Sujeto = $"<http://gnoss/{item.ResultadoSuscripcion.SuscripcionID.ToString().ToUpper()}_{item.ResultadoSuscripcion.RecursoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/Estado>",
                Objeto = $"\"Pendientes de leer\" ."
            })).Union(consulta8.Select(item => new QueryTriples 
            {
                Sujeto = $"<http://gnoss/{item.ResultadoSuscripcion.SuscripcionID.ToString().ToUpper()}_{item.ResultadoSuscripcion.RecursoID.ToString().ToUpper()}>",
                Predicado = "<http://gnoss/Estado>",
                Objeto = $"\"Favoritos\" ."
            })).Distinct().ToList();

            return resultadoConsulta;
        }

        public void ObtieneInformacionMensajesLeidoNoLeido(DataSet pDataSet, string pCorreoID, string pIdentidadID)
        {
            string tabla = $"\"CorreoInterno_{pIdentidadID.Substring(0, 2)}\"";
            string valorTrue = "1";
            string valorFalse = "0";

            if (EsPostgres())
            {
                valorTrue = "true";
                valorFalse = "false";
            }

            string consulta = $"SELECT DISTINCT '<http://gnoss/{pCorreoID}>', '<http://www.semanticdesktop.org/ontologies/2007/03/22/nmo#isRead>', '\"Leidos\" .' AS Expr3 FROM {tabla} WHERE \"Eliminado\" = {valorFalse} AND \"Leido\" = {valorTrue} AND \"Destinatario\" = '{pIdentidadID}'AND  \"CorreoID\" = '{pCorreoID}' UNION SELECT DISTINCT '<http://gnoss/{pCorreoID}>', '<http://www.semanticdesktop.org/ontologies/2007/03/22/nmo#isRead>', '\"Pendientes de leer\" .' AS Expr3 FROM {tabla} WHERE \"Eliminado\" = {valorFalse} AND \"Destinatario\" = '{pIdentidadID}' AND  \"Leido\" = {valorFalse} AND \"CorreoID\" = '{pCorreoID}'";


            DbCommand commandsql = ObtenerComando(consulta);

            CargarDataSet(commandsql, pDataSet, "TripletasMensajesLeido");
        }

        public void ObtieneInformacionExtraMensajesTo(DataSet pDataSet, string pCorreoID, string pIdentidadID)
        {
            string tabla = $"\"CorreoInterno_{pIdentidadID.Substring(0, 2)}\"";
            string valorTrue = "1";
            string valorFalse = "0";
            string simboloConcatenar = "+";

            if (EsPostgres())
            {
                valorTrue = "true";
                valorFalse = "false";
                simboloConcatenar = "||";
            }

            string consulta1 = $"SELECT DISTINCT '<http://gnoss/{pCorreoID}>', '<http://www.semanticdesktop.org/ontologies/2007/03/22/nmo#sentDate>', {FormatearFecha("Fecha")} AS Expr3 FROM {tabla} WHERE \"Eliminado\" = {valorFalse} AND \"CorreoID\" = '{pCorreoID}'";
            string consulta2 = $"SELECT DISTINCT '<http://gnoss/{pCorreoID}>', '<http://www.semanticdesktop.org/ontologies/2007/03/22/nmo#from>', {FormatearRemplazos("NombreOrganizacion")} AS Expr3 FROM {tabla} INNER JOIN \"Identidad\" ON \"Identidad\".\"IdentidadID\" = {tabla}.\"Autor\" INNER JOIN \"Perfil\" ON \"Perfil\".\"PerfilID\" = \"Identidad\".\"PerfilID\" WHERE {tabla}.\"Eliminado\" = {valorFalse} AND \"Identidad\".\"Tipo\" = 2 AND {tabla}.\"CorreoID\" = '{pCorreoID}'";
            string consulta3 = $"SELECT DISTINCT '<http://gnoss/{pCorreoID}>', '<http://www.semanticdesktop.org/ontologies/2007/03/22/nmo#from>', {FormatearRemplazosPerfilOrganizacion("NombrePerfil", "NombreOrganizacion")} AS Expr3 FROM {tabla} INNER JOIN \"Identidad\" ON \"Identidad\".\"IdentidadID\" = {tabla}.\"Autor\" INNER JOIN \"Perfil\" ON \"Perfil\".\"PerfilID\" = \"Identidad\".\"PerfilID\" WHERE {tabla}.\"Eliminado\" = {valorFalse} AND \"Identidad\".\"Tipo\" <> 2 AND {tabla}.\"CorreoID\" = '{pCorreoID}'";
            string consulta4 = $"SELECT DISTINCT '<http://gnoss/{pCorreoID}>', '<http://www.semanticdesktop.org/ontologies/2007/03/22/nmo#from>', '\"GNOSS\" .' AS Expr3 FROM {tabla} WHERE \"Autor\" = '00000000-0000-0000-0000-000000000000' AND {tabla}.\"CorreoID\" = '{pCorreoID}'";
            string consulta5 = $"SELECT DISTINCT '<http://gnoss/{pCorreoID}>', '<http://purl.org/dc/elements/1.1/type>', '\"Eliminados\" .' AS Expr3 FROM {tabla} WHERE \"Eliminado\" = {valorFalse} AND \"EnPapelera\" = {valorTrue} AND {tabla}.\"CorreoID\" = '{pCorreoID}'";
            string consulta6 = $"SELECT DISTINCT '<http://gnoss/{pCorreoID}>', '<http://purl.org/dc/elements/1.1/type>', '\"Entrada\" .' AS Expr3 FROM {tabla} WHERE  \"Eliminado\" = {valorFalse} AND \"EnPapelera\" = {valorFalse} AND \"Destinatario\" != '00000000-0000-0000-0000-000000000000' AND {tabla}.\"CorreoID\" = '{pCorreoID}'";
            string consulta7 = $"SELECT DISTINCT '<http://gnoss/{pCorreoID}>', '<http://gnoss/hasConversacion>', '<http://gnoss/' {simboloConcatenar}CONVERT(nvarchar(1000), ConversacionID){simboloConcatenar} '> .' AS Expr3 FROM {tabla} WHERE \"Eliminado\" = {valorFalse} AND \"EnPapelera\" = {valorFalse} AND \"Destinatario\" != '00000000-0000-0000-0000-000000000000' AND {tabla}.\"CorreoID\" = '{pCorreoID}' AND \"ConversacionID\" IS NOT NULL";
            string union = " UNION ";

            string consultaCompleta = consulta1 + union + consulta2 + union + consulta3 + union + consulta4 + union + consulta5 + union + consulta6 + union + consulta7;

            DbCommand commandsql = ObtenerComando(consultaCompleta);

            CargarDataSet(commandsql, pDataSet, "TripletasMensajesTo");
        }


        public void ObtieneInformacionExtraMensajesFrom(DataSet pDataSet, string pCorreoID, string pIdentidadID)
        {
            string tabla = $"\"CorreoInterno_{pIdentidadID.Substring(0, 2)}\"";
            string valorTrue = "1";
            string valorFalse = "0";
            string simboloConcatenar = "+";

            if (EsPostgres())
            {
                valorTrue = "true";
                valorFalse = "false";
                simboloConcatenar = "||";
            }

            string consulta = $"SELECT DISTINCT '<http://gnoss/{pCorreoID}>', '<http://www.semanticdesktop.org/ontologies/2007/03/22/nmo#sentDate>', {FormatearFecha("Fecha")} AS Expr3 FROM {tabla} WHERE \"Eliminado\" = {valorFalse} AND \"CorreoID\" = '{pCorreoID}'";

            consulta += " UNION ";

            consulta += $"SELECT DISTINCT '<http://gnoss/{pCorreoID}>', '<http://purl.org/dc/elements/1.1/type>', '\"Eliminados\" .' AS Expr3 FROM {tabla} WHERE \"Eliminado\" = {valorFalse} AND \"EnPapelera\" = {valorTrue} AND {tabla}.\"CorreoID\" = '{pCorreoID}'";
            consulta += " UNION ";
            consulta += $"SELECT DISTINCT '<http://gnoss/{pCorreoID}>', '<http://purl.org/dc/elements/1.1/type>', '\"Enviados\" .' AS Expr3 FROM {tabla} WHERE  \"Eliminado\" = {valorFalse} AND \"EnPapelera\" = {valorFalse} AND \"Destinatario\" = '00000000-0000-0000-0000-000000000000' AND {tabla}.\"CorreoID\" = '{pCorreoID}'";
            consulta += " UNION ";
            consulta += $"SELECT DISTINCT '<http://gnoss/{pCorreoID}>', '<http://gnoss/hasConversacion>', '<http://gnoss/' {simboloConcatenar}CONVERT(nvarchar(1000), ConversacionID){simboloConcatenar} '> .' AS Expr3 FROM {tabla} WHERE \"Eliminado\" = {valorFalse} AND \"EnPapelera\" = 0 AND \"Destinatario\" = '00000000-0000-0000-0000-000000000000' AND {tabla}.\"CorreoID\" = '{pCorreoID}' AND \"ConversacionID\" IS NOT NULL";

            DbCommand commandsql = ObtenerComando(consulta);

            CargarDataSet(commandsql, pDataSet, "TripletasMensajesFrom");
        }

        public void ObtieneInformacionExtraMensajesFromObtenerTo(DataSet pDataSet, string pCorreoID, string pIdentidadID)
        {
            string tabla = $"\"CorreoInterno_{pIdentidadID.Substring(0, 2)}\"";
            string valorFalse = "0";
            string simboloConcatenar = "+";

            if (EsPostgres())
            {
                valorFalse = "false";
                simboloConcatenar = "||";
            }

            string consulta = $"SELECT DISTINCT '<http://gnoss/{pCorreoID}>', '<http://www.semanticdesktop.org/ontologies/2007/03/22/nmo#to>', '\"' {simboloConcatenar} \"DestinatariosNombres\" {simboloConcatenar} '\" .' AS Expr3 FROM {tabla} WHERE {tabla}.\"Eliminado\" = {valorFalse} AND {tabla}.\"Destinatario\" = '00000000-0000-0000-0000-000000000000' AND \"CorreoID\" = '{pCorreoID}'";

            DbCommand commandsql = ObtenerComando(consulta);

            CargarDataSet(commandsql, pDataSet, "TripletasMensajesFromObtenerTo");
        }

        public List<QueryTriples> ObtieneInformacionExtraComentarios(Guid pComentarioID)
        {
            List<QueryTriples> resultadoConsulta;
            string dateFormat = "yyyyMMddHHmmdd";

            var consulta1 = mEntityContext.DocumentoComentario.Where(item => item.ComentarioID.Equals(pComentarioID)).ToList();
            var consulta2 = mEntityContext.Comentario.Where(item => item.ComentarioID.Equals(pComentarioID)).ToList();

            resultadoConsulta = consulta1.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.ComentarioID.ToString().ToUpper()}>",
                Predicado = "<http://purl.org/dc/elements/1.1/type>",
                Objeto = "\"Contribuciones\""
            }).Union(consulta2.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.ComentarioID.ToString().ToUpper()}>",
                Predicado = "<http://purl.org/dc/elements/1.1/date>",
                Objeto = $"{item.Fecha.ToString(dateFormat)} ."
            })).Distinct().ToList();

            return resultadoConsulta;
        }

        /// <summary>
        /// Obtiene el ID de usuario a partir de su perfil.
        /// </summary>
        /// <param name="pPerfilID">ID de perfil</param>
        /// <returns>ID de usuario</returns>
        public Guid ObtenerIdUsuarioDesdePerfil(Guid pPerfilID)
        {
            Guid usuarioID = mEntityContext.Persona.JoinPerfil().JoinIdentidad().Where(item => item.Perfil.PerfilID.Equals(pPerfilID) && item.Persona.UsuarioID.HasValue).Select(item => item.Persona.UsuarioID.Value).FirstOrDefault();

            if (usuarioID.Equals(Guid.Empty))
            {
                usuarioID = mEntityContext.Identidad.JoinPerfil().JoinOrganizacion().Where(item => item.Perfil.PerfilID.Equals(pPerfilID)).Select(item => item.Organizacion.OrganizacionID).FirstOrDefault();
            }

            return usuarioID;
        }

        /// <summary>
        /// Onbtiene el UsuarioID a partir del Id de la identidad
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <returns>Identificador del usuario referente a la identidad dada</returns>
        public Guid ObtenerIdUsuarioDesdeIdentidad(Guid pIdentidadID)
        {
            Guid usuarioID = mEntityContext.Persona.JoinPerfil().JoinIdentidad().Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID) && item.Persona.UsuarioID.HasValue).Select(item => item.Persona.UsuarioID.Value).FirstOrDefault();

            if(usuarioID.Equals(Guid.Empty))
            {
                usuarioID = mEntityContext.Identidad.JoinPerfil().JoinOrganizacion().Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID)).Select(item => item.Organizacion.OrganizacionID).FirstOrDefault();
            }

            return usuarioID;
        }

        /// <summary>
        /// Obtiene la informacion extra de las personas
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad de la persona</param>
        /// <param name="pProyectoID">Identificador del proyecto al que pertenece la persona</param>
        public List<QueryTriples> ObtieneInformacionExtraPersona(Guid pIdentidadID, Guid pProyectoID)
        {
            List<QueryTriples> resultadoConsulta = new List<QueryTriples>();
            string dateFormat = "yyyyMMddHHmmdd";

            var consultaSqlName = mEntityContext.Identidad.JoinPerfil().GroupJoin(mEntityContext.Persona, item => item.Perfil.PersonaID.Value, persona => persona.PersonaID, (item, persona) => new 
            {
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                Persona = persona
            }).SelectMany(item => item.Persona.DefaultIfEmpty(), (x, y) => new 
            {
                Identidad = x.Identidad,
                Perfil = x.Perfil,
                Persona = y
            }).GroupJoin(mEntityContext.Organizacion, item => item.Perfil.OrganizacionID, organizacion => organizacion.OrganizacionID, (item, organizacion) => new 
            {
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                Persona = item.Persona,
                Organizacion = organizacion
            }).SelectMany(item => item.Organizacion.DefaultIfEmpty(), (x, y) => new 
            {
                Identidad = x.Identidad,
                Perfil = x.Perfil,
                Persona = x.Persona,
                Organizacion = y
            }).Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID) && !item.Perfil.Eliminado);

            foreach(var query in consultaSqlName)
            {
                if(query.Identidad.Tipo == (short)TiposIdentidad.Organizacion)
                {
                    resultadoConsulta.Add(new QueryTriples() { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://xmlns.com/foaf/0.1/firstName>", Objeto = $"\"{RemplazarCaracteresBD(query.Organizacion.Nombre).ToLowerSearchGraph()}\"" });
                }
                else if(query.Identidad.Tipo == (short)TiposIdentidad.Personal || query.Identidad.Tipo == (short)TiposIdentidad.ProfesionalPersonal || query.Identidad.Tipo == (short)TiposIdentidad.Profesor)
                {
                    resultadoConsulta.Add(new QueryTriples() { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://xmlns.com/foaf/0.1/firstName>", Objeto = $"\"{RemplazarCaracteresBD(query.Persona.Nombre).ToLowerSearchGraph()}\"" });
                }

                if(query.Identidad.Tipo == (short)TiposIdentidad.Personal || query.Identidad.Tipo == (short)TiposIdentidad.ProfesionalPersonal || query.Identidad.Tipo == (short)TiposIdentidad.Profesor)
                {
                    resultadoConsulta.Add(new QueryTriples() { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://xmlns.com/foaf/0.1/familyName>", Objeto = $"\"{RemplazarCaracteresBD(query.Persona.Apellidos)}\" ." });
                }
            }

            var consultaRegionPers = mEntityContext.Identidad.JoinPerfil().JoinPersona().GroupJoin(mEntityContext.Provincia, item => item.Persona.ProvinciaPersonalID, provincia => provincia.ProvinciaID, (item, provincia) => new
            {
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                Persona = item.Persona,
                Provincia = provincia
            }).SelectMany(item => item.Provincia.DefaultIfEmpty(), (x, y) => new
            {
                Identidad = x.Identidad,
                Perfil = x.Perfil,
                Persona = x.Persona,
                Provincia = y
            }).Join(mEntityContext.Pais, item => item.Persona.PaisPersonalID.Value, pais => pais.PaisID, (item, pais) => new
            {
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                Persona = item.Persona,
                Provincia = item.Provincia,
                Pais = pais
            }).Where(item => !item.Persona.Eliminado && !item.Perfil.Eliminado && item.Identidad.IdentidadID.Equals(pIdentidadID) && item.Persona.PaisPersonalID.HasValue);

            foreach(var query in consultaRegionPers)
            {
                if(query.Identidad.Tipo == (short)TiposIdentidad.Personal || query.Identidad.Tipo == (short)TiposIdentidad.ProfesionalPersonal || query.Identidad.Tipo == (short)TiposIdentidad.Profesor)
                {
                    resultadoConsulta.Add(new QueryTriples() { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://www.w3.org/2006/vcard/ns#country-name>", Objeto = $"\"{query.Pais.Nombre}\" ." });
                    resultadoConsulta.Add(new QueryTriples() { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://www.w3.org/2006/vcard/ns#locality>", Objeto = $"\"{query.Persona.LocalidadPersonal}\" ." });
                    if(query.Provincia != null && query.Provincia.Nombre!= null)
                    {
                        resultadoConsulta.Add(new QueryTriples() { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://d.opencalais.com/1/type/er/Geo/ProvinceOrState>", Objeto = $"\"{query.Provincia.Nombre}\" ." });
                    }                    
                    if(query.Identidad.Tipo == (short)TiposIdentidad.Profesor)
                    {
                        resultadoConsulta.Add(new QueryTriples() { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>", Objeto = $"\"Profesor\" ." });
                    }
                }
            }

            var consultaRegionOrg = mEntityContext.Organizacion.JoinOrganizacionParticipaProy().JoinProyecto().JoinPerfil().JoinIdentidad().JoinPais().Where(item => item.Organizacion.PaisID.HasValue && !item.Organizacion.Eliminada && item.Identidad.Tipo == 3 && !item.Perfil.Eliminado && !item.Perfil.PersonaID.HasValue && item.Identidad.IdentidadID.Equals(pIdentidadID));

            foreach (var query in consultaRegionOrg)
            {
                resultadoConsulta.Add(new QueryTriples() { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://www.w3.org/2006/vcard/ns#country-name>", Objeto = $"\"{query.Pais.Nombre}\" ." });
            }

            var consultaRegionAux = mEntityContext.Identidad.JoinPerfil().GroupJoin(mEntityContext.OrganizacionClase, item => item.Perfil.OrganizacionID.Value, organizacionClase => organizacionClase.OrganizacionID, (item, organizacionClase) => new
            {
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                OrganizacionClase = organizacionClase
            }).SelectMany(item => item.OrganizacionClase.DefaultIfEmpty(), (x, y) => new 
            {
                Identidad = x.Identidad,
                Perfil = x.Perfil,
                OrganizacionClase = y
            }).Where(item => !item.Perfil.Eliminado && item.Identidad.IdentidadID.Equals(pIdentidadID) && item.Perfil.OrganizacionID.HasValue);

            foreach(var query in consultaRegionAux)
            {
                resultadoConsulta.Add(new QueryTriples() { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasPerfilProyecto>", Objeto = $"<http://gnoss/{query.Identidad.PerfilID.ToString().ToUpper()}_{query.Identidad.ProyectoID.ToString().ToUpper()}> ." });
                resultadoConsulta.Add(new QueryTriples() { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasPerfil>", Objeto = $"<http://gnoss/{query.Identidad.PerfilID.ToString().ToUpper()}> ." });

                if (!string.IsNullOrEmpty(query.Perfil.NombreCortoUsu))
                {
                    resultadoConsulta.Add(new QueryTriples() { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://gnoss/nombreCortoUsu>", Objeto = $"\"{query.Perfil.NombreCortoUsu}\" ." });
                }
                if (!string.IsNullOrEmpty(query.Perfil.NombreCortoOrg))
                {
                    resultadoConsulta.Add(new QueryTriples() { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://gnoss/nombreCortoOrg>", Objeto = $"\"{query.Perfil.NombreCortoOrg}\" ." });
                }
                if(query.Identidad.Tipo == (short)TiposIdentidad.Personal || query.Identidad.Tipo == (short)TiposIdentidad.Profesor)
                {
                    resultadoConsulta.Add(new QueryTriples() { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasfechaAlta>", Objeto = $"{query.Identidad.FechaAlta.ToString(dateFormat)} ." });
                }
                if(!query.OrganizacionClase.OrganizacionID.Equals(Guid.Empty) && (query.Identidad.Tipo == (short)TiposIdentidad.ProfesionalPersonal || query.Identidad.Tipo == (short)TiposIdentidad.ProfesionalCorporativo))
                {
                    resultadoConsulta.Add(new QueryTriples() { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>", Objeto = $"\"Alumno\" ." });
                }
            }

            var consultaFoto = mEntityContext.Identidad.JoinPerfil().GroupJoin(mEntityContext.Persona, item => item.Perfil.PersonaID, persona => persona.PersonaID, (item, persona) => new
            {
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                Persona = persona
            }).SelectMany(item => item.Persona.DefaultIfEmpty(), (x, y) => new 
            {
                Identidad = x.Identidad,
                Perfil = x.Perfil,
                Persona = y
            }).GroupJoin(mEntityContext.Organizacion, item => item.Perfil.OrganizacionID, organizacion => organizacion.OrganizacionID, (item, organizacion) => new 
            {
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                Persona = item.Persona,
                Organizacion = organizacion
            }).SelectMany(item => item.Organizacion.DefaultIfEmpty(), (x, y) => new 
            {
                Identidad = x.Identidad,
                Perfil = x.Perfil,
                Persona = x.Persona,
                Organizacion = y
            }).GroupJoin(mEntityContext.PerfilPersonaOrg, item => item.Identidad.PerfilID, perfilPersonaOrg => perfilPersonaOrg.PerfilID, (item, perfilPersonaOrg) => new 
            {
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                Persona = item.Persona,
                Organizacion = item.Organizacion,
                PerfilPersonaOrg = perfilPersonaOrg
            }).SelectMany(item => item.PerfilPersonaOrg.DefaultIfEmpty(), (x, y) => new 
            {
                Identidad = x.Identidad,
                Perfil = x.Perfil,
                Persona = x.Persona,
                Organizacion = x.Organizacion,
                PerfilPersonaOrg = y
            }).GroupJoin(mEntityContext.PersonaVinculoOrganizacion, item => new { item.Persona.PersonaID, item.PerfilPersonaOrg.OrganizacionID }, personaVinculoOrganizacion => new { personaVinculoOrganizacion.PersonaID, personaVinculoOrganizacion.OrganizacionID }, (item, personaVinculoOrganizacion) => new 
            {
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                Persona = item.Persona,
                Organizacion = item.Organizacion,
                PerfilPersonaOrg = item.PerfilPersonaOrg,
                PersonaVinculoOrganizacion = personaVinculoOrganizacion
            }).SelectMany(item => item.PersonaVinculoOrganizacion.DefaultIfEmpty(), (x, y) => new 
            {
                Identidad = x.Identidad,
                Perfil = x.Perfil,
                Persona = x.Persona,
                Organizacion = x.Organizacion,
                PerfilPersonaOrg = x.PerfilPersonaOrg,
                PersonaVinculoOrganizacion = y
            })
            .Where(x=> x.Identidad.IdentidadID.Equals(pIdentidadID));

            foreach(var query in consultaFoto)
            {
                if(query.Persona.FechaAnadidaFoto != null)
                {
                    if (query.Organizacion != null && query.Organizacion.VersionLogo.HasValue && (query.Identidad.Tipo == (short)TiposIdentidad.ProfesionalCorporativo || query.Identidad.Tipo == (short)TiposIdentidad.Organizacion) && !string.IsNullOrEmpty(query.Organizacion.CoordenadasLogo))
                    {
                        resultadoConsulta.Add(new QueryTriples() { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasfoto>", Objeto = $"\"/Organizaciones/{query.Organizacion.OrganizacionID}_peque.png?{query.Organizacion.VersionLogo.Value}\" ." });
                    }
                    else if (query.PersonaVinculoOrganizacion != null && query.PersonaVinculoOrganizacion.VersionFoto.HasValue && query.Identidad.Tipo == (short)TiposIdentidad.ProfesionalPersonal && !query.PersonaVinculoOrganizacion.UsarFotoPersonal && !string.IsNullOrEmpty(query.PersonaVinculoOrganizacion.CoordenadasFoto))
                    {
                        resultadoConsulta.Add(new QueryTriples() { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasfoto>", Objeto = $"\"/Persona_Organizacion/{query.PersonaVinculoOrganizacion.PersonaID}_peque.png?{query.PersonaVinculoOrganizacion.VersionFoto.Value}\" ." });
                    }
                    else if (query.Persona.VersionFoto.HasValue && (query.Identidad.Tipo == (short)TiposIdentidad.Personal || query.Identidad.Tipo == (short)TiposIdentidad.Profesor) || (query.Identidad.Tipo == (short)TiposIdentidad.ProfesionalPersonal && query.PersonaVinculoOrganizacion.UsarFotoPersonal) && !string.IsNullOrEmpty(query.Persona.CoordenadasFoto))
                    {
                        resultadoConsulta.Add(new QueryTriples() { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasfoto>", Objeto = $"\"/Personas/{query.Persona.PersonaID}_peque.png?{query.Persona.VersionFoto.Value}\" ." });
                    }
                    else
                    {
                        resultadoConsulta.Add(new QueryTriples() { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasfoto>", Objeto = $"\"sinfoto\" ." });
                    }
                }
                else
                {
                    resultadoConsulta.Add(new QueryTriples() { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasfoto>", Objeto = $"\"sinfoto\" ." });
                }

            }

            var consultaEstadoUsuario = mEntityContext.Identidad.JoinPerfil().JoinPersona().Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID) && item.Identidad.ProyectoID.Equals(pProyectoID) && item.Persona.UsuarioID.HasValue).ToList();

            foreach(var query in consultaEstadoUsuario)
            {
                Guid usuarioID = query.Persona.UsuarioID.Value;

                if (query.Identidad.FechaExpulsion.HasValue)
                {
                    resultadoConsulta.Add(new QueryTriples() { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://gnoss/userstatus>", Objeto = $"{(short)TipoMiembros.Expulsados} ." });
                }
                else
                {
                    ProyectoRolUsuario proyectoRolUsuario = mEntityContext.ProyectoRolUsuario.Where(item => item.UsuarioID.Equals(query.Persona.UsuarioID.Value) && query.Identidad.ProyectoID.Equals(pProyectoID)).FirstOrDefault();
                    if (proyectoRolUsuario.EstaBloqueado)
                    {
                        resultadoConsulta.Add(new QueryTriples() { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://gnoss/userstatus>", Objeto = $"{(short)TipoMiembros.Bloqueados} ." });
                    }
                    else
                    {
                        resultadoConsulta.Add(new QueryTriples() { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://gnoss/userstatus>", Objeto = $"{(short)TipoMiembros.Activos} ." });
                    }
                }
            }
            
            var consultaRolUsuario = mEntityContext.Identidad.JoinPerfil().JoinPersona().Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID) && item.Identidad.ProyectoID.Equals(pProyectoID) && item.Persona.UsuarioID.HasValue).ToList();

            foreach(var query in consultaRolUsuario)
            {
                AdministradorProyecto administradorProyecto = mEntityContext.AdministradorProyecto.Where(item => item.UsuarioID.Equals(query.Persona.UsuarioID.Value) && item.ProyectoID.Equals(query.Identidad.ProyectoID)).FirstOrDefault();

                if(administradorProyecto != null && administradorProyecto.Tipo == (short)TipoRolUsuario.Administrador)
                {
                    resultadoConsulta.Add(new QueryTriples() { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://gnoss/rol>", Objeto = $"{(short)TipoRolUsuario.Administrador} ." });
                }
                else if(administradorProyecto != null && administradorProyecto.Tipo == (short)TipoRolUsuario.Supervisor)
                {
                    resultadoConsulta.Add(new QueryTriples() { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://gnoss/rol>", Objeto = $"{(short)TipoRolUsuario.Supervisor} ." });
                }
                else
                {
                    resultadoConsulta.Add(new QueryTriples() { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://gnoss/rol>", Objeto = $"{(short)TipoRolUsuario.Usuario} ." });
                }
            }

            if (!pProyectoID.Equals(ProyectoAD.MetaProyecto))
            {
                var consultaNomyGnoss = mEntityContext.Identidad.JoinPerfil().GroupJoin(mEntityContext.Persona, item => item.Perfil.PersonaID, persona => persona.PersonaID, (item, persona) => new
                {
                    Identidad = item.Identidad,
                    Perfil = item.Perfil,
                    Persona = persona
                }).SelectMany(item => item.Persona.DefaultIfEmpty(), (x, y) => new
                {
                    Identidad = x.Identidad,
                    Perfil = x.Perfil,
                    Persona = y
                }).Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID) && !item.Perfil.Eliminado && item.Persona.UsuarioID.HasValue).ToList();

                foreach(var query in consultaNomyGnoss)
                {
                    AdministradorProyecto administradorProyecto = mEntityContext.AdministradorProyecto.Where(item => item.ProyectoID.Equals(query.Identidad.ProyectoID) && item.UsuarioID.Equals(query.Persona.UsuarioID.Value)).FirstOrDefault();

                    if(query.Identidad.Tipo == (short)TiposIdentidad.Personal || query.Identidad.Tipo == (short)TiposIdentidad.ProfesionalPersonal || query.Identidad.Tipo == (short)TiposIdentidad.ProfesionalCorporativo || query.Identidad.Tipo == (short)TiposIdentidad.Profesor)
                    {
                        resultadoConsulta.Add(new QueryTriples() { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasPopularidad>", Objeto = $"{query.Identidad.Rank} ." });
                    }
                    if (administradorProyecto != null && administradorProyecto.ProyectoID.Equals(pProyectoID))
                    {
                        Proyecto proyecto = mEntityContext.Proyecto.Where(item => item.ProyectoID.Equals(pProyectoID)).FirstOrDefault();
                        resultadoConsulta.Add(new QueryTriples() { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://gnoss/ComunidadesAdministradas>", Objeto = $"\"{RemplazarCaracteresBD(proyecto.Nombre)}\" ." });
                    }
                    if(query.Identidad.Tipo == (short)TiposIdentidad.Personal || query.Identidad.Tipo == (short)TiposIdentidad.ProfesionalPersonal || query.Identidad.Tipo == (short)TiposIdentidad.Profesor)
                    {
                        resultadoConsulta.Add(new QueryTriples() { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://rdfs.org/sioc/ns#has_space>", Objeto = $"<http://gnoss/{query.Identidad.ProyectoID}> ." });
                    }
                }               
            }
            else
            {
                var consultaMyGnoss = mEntityContext.Identidad.JoinPerfil().JoinPersona().JoinAdministradorProyecto().JoinProyecto().Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID) && !item.Perfil.Eliminado).ToList();

                foreach(var query in consultaMyGnoss)
                {
                    resultadoConsulta.Add(new QueryTriples() { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://gnoss/ComunidadesAdministradas>", Objeto = $"\"{RemplazarCaracteresBD(query.Proyecto.Nombre)}\" ." });
                    resultadoConsulta.Add(new QueryTriples() { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasPopularidad>", Objeto = $"{query.Identidad.Rank} ." });
                    
                    if(query.Identidad.Tipo == (short)TiposIdentidad.Personal || query.Identidad.Tipo == (short)TiposIdentidad.Profesor)
                    {
                        resultadoConsulta.Add(new QueryTriples() { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasEstadoCorreccion>", Objeto = $"{query.Persona.EstadoCorreccion} ." });
                    }
                }
            }

            var consulta1 = mEntityContext.DatoExtraProyectoOpcionIdentidad.Join(mEntityContext.DatoExtraProyectoOpcion, datoExtraProyectoOpcionIdentidad => new { datoExtraProyectoOpcionIdentidad.OrganizacionID, datoExtraProyectoOpcionIdentidad.ProyectoID, datoExtraProyectoOpcionIdentidad.DatoExtraID, datoExtraProyectoOpcionIdentidad.OpcionID }, datoExtraProyectoOpcion => new { datoExtraProyectoOpcion.OrganizacionID, datoExtraProyectoOpcion.ProyectoID, datoExtraProyectoOpcion.DatoExtraID, datoExtraProyectoOpcion.OpcionID}, (datoExtraProyectoOpcionIdentidad, datoExtraProyectoOpcion) => new 
            {
                DatoExtraProyectoOpcionIdentidad = datoExtraProyectoOpcionIdentidad,
                DatoExtraProyectoOpcion = datoExtraProyectoOpcion
            }).Join(mEntityContext.DatoExtraProyecto, item => new { item.DatoExtraProyectoOpcion.OrganizacionID, item.DatoExtraProyectoOpcion.ProyectoID, item.DatoExtraProyectoOpcion.DatoExtraID }, datoExtraProyecto => new { datoExtraProyecto.OrganizacionID, datoExtraProyecto.ProyectoID, datoExtraProyecto.DatoExtraID }, (item, datoExtraProyecto) => new 
            {
                DatoExtraProyectoOpcionIdentidad = item.DatoExtraProyectoOpcionIdentidad,
                DatoExtraProyectoOpcion = item.DatoExtraProyectoOpcion,
                DatoExtraProyecto = datoExtraProyecto
            }).Where(item => item.DatoExtraProyectoOpcionIdentidad.ProyectoID.Equals(pProyectoID) && item.DatoExtraProyectoOpcionIdentidad.IdentidadID.Equals(pIdentidadID) && !string.IsNullOrEmpty(item.DatoExtraProyecto.PredicadoRDF));

            resultadoConsulta.AddRange(consulta1.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.DatoExtraProyectoOpcionIdentidad.IdentidadID.ToString().ToUpper()}>",
                Predicado = $"<{item.DatoExtraProyecto.PredicadoRDF}>",
                Objeto = $"\"{item.DatoExtraProyectoOpcion.Opcion}\" ."
            }).Distinct().ToList());

            var consulta2 = mEntityContext.DatoExtraEcosistemaOpcionPerfil.Join(mEntityContext.Identidad, item => item.PerfilID, identidad => identidad.PerfilID, (item, identidad) => new
            {
                DatoExtraEcosistemaOpcionPerfil = item,
                Identidad = identidad
            }).Join(mEntityContext.DatoExtraEcosistemaOpcion, item => new { item.DatoExtraEcosistemaOpcionPerfil.DatoExtraID, item.DatoExtraEcosistemaOpcionPerfil.OpcionID }, datoExtraEcosistemaOpcion => new { datoExtraEcosistemaOpcion.DatoExtraID, datoExtraEcosistemaOpcion.OpcionID }, (item, datoExtraEcosistemaOpcion) => new
            {
                DatoExtraEcosistemaOpcionPerfil = item.DatoExtraEcosistemaOpcionPerfil,
                Identidad = item.Identidad,
                DatoExtraEcosistemaOpcion = datoExtraEcosistemaOpcion
            }).Join(mEntityContext.DatoExtraEcosistema, item => item.DatoExtraEcosistemaOpcion.DatoExtraID, datoExtraEcosistema => datoExtraEcosistema.DatoExtraID, (item, DatoExtraEcosistema) => new
            {
                DatoExtraEcosistemaOpcionPerfil = item.DatoExtraEcosistemaOpcionPerfil,
                Identidad = item.Identidad,
                DatoExtraEcosistemaOpcion = item.DatoExtraEcosistemaOpcion,
                DatoExtraEcosistema = DatoExtraEcosistema
            }).Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID) && !string.IsNullOrEmpty(item.DatoExtraEcosistema.PredicadoRDF));

            resultadoConsulta.AddRange(consulta2.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Identidad.IdentidadID.ToString().ToUpper()}>",
                Predicado = $"<{item.DatoExtraEcosistema.PredicadoRDF}>",
                Objeto = $"\"{item.DatoExtraEcosistemaOpcion.Opcion}\" ."
            }).Distinct().ToList());

            //DatosExtraVirtuoso 
            var consulta3 = mEntityContext.DatoExtraProyectoVirtuosoIdentidad.Join(mEntityContext.DatoExtraProyectoVirtuoso, datoExtraProyectoVirtuosoIdentidad => new { datoExtraProyectoVirtuosoIdentidad.OrganizacionID, datoExtraProyectoVirtuosoIdentidad.ProyectoID, datoExtraProyectoVirtuosoIdentidad.DatoExtraID }, datoExtraProyectoVirtuoso => new { datoExtraProyectoVirtuoso.OrganizacionID, datoExtraProyectoVirtuoso.ProyectoID, datoExtraProyectoVirtuoso.DatoExtraID }, (datoExtraProyectoVirtuosoIdentidad, datoExtraProyectoVirtuoso) => new
            {
                DatoExtraProyectoVirtuosoIdentidad = datoExtraProyectoVirtuosoIdentidad,
                DatoExtaProyectoVirtuoso = datoExtraProyectoVirtuoso
            }).Where(item => item.DatoExtraProyectoVirtuosoIdentidad.ProyectoID.Equals(pProyectoID) && item.DatoExtraProyectoVirtuosoIdentidad.IdentidadID.Equals(pIdentidadID));

            resultadoConsulta.AddRange(consulta3.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.DatoExtraProyectoVirtuosoIdentidad.IdentidadID.ToString().ToUpper()}>",
                Predicado = $"<{item.DatoExtaProyectoVirtuoso.PredicadoRDF}>",
                Objeto = $"\"{item.DatoExtraProyectoVirtuosoIdentidad.Opcion}\" ."
            }).Distinct().ToList());

            var consulta4 = mEntityContext.DatoExtraEcosistemaVirtuosoPerfil.Join(mEntityContext.Identidad, datoExtraExosistemaVirtuosoPerfil => datoExtraExosistemaVirtuosoPerfil.PerfilID, identidad => identidad.PerfilID, (datoExtraEcosistemaVirtuosoPerfil, identidad) => new
            {
                DatoExtraEcosistemaVirtuosoPerfil = datoExtraEcosistemaVirtuosoPerfil,
                Identidad = identidad
            }).Join(mEntityContext.DatoExtraEcosistemaVirtuoso, item => item.DatoExtraEcosistemaVirtuosoPerfil.DatoExtraID, datoExtraEcosistemaVirtuoso => datoExtraEcosistemaVirtuoso.DatoExtraID, (item, datoExtraEcosistemaVirtuoso) => new
            {
                DatoExtraEcosistemaVirtuosoPerfil = item.DatoExtraEcosistemaVirtuosoPerfil,
                Identidad = item.Identidad,
                DatoExtraEcosistemaVirtuoso = datoExtraEcosistemaVirtuoso
            });

            resultadoConsulta.AddRange(consulta4.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Identidad.IdentidadID.ToString().ToUpper()}>",
                Predicado = $"<{item.DatoExtraEcosistemaVirtuoso.PredicadoRDF}>",
                Objeto = $"\"{item.DatoExtraEcosistemaVirtuosoPerfil.Opcion}\" ."
            }).Distinct().ToList());

            var consultaUsuario = mEntityContext.Usuario.JoinPersona().JoinPerfil().JoinIdentidad().Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID));

            resultadoConsulta.AddRange(consultaUsuario.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Usuario.UsuarioID.ToString().ToUpper()}>",
                Predicado = $"<http://gnoss/hasIdentidadID>",
                Objeto = $"<http://gnoss/{item.Identidad.IdentidadID.ToString().ToUpper()}> ."
            }).Distinct().ToList());

            return resultadoConsulta;
        }

        /// <summary>
        /// Obtiene la información extra de personas y contactos
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad a obtener la información</param>
        /// <param name="pProyectoID">Identificador del proyecto al que pertenece el usuario</param>
        public List<QueryTriples> ObtieneInformacionExtraPersonaContactos(Guid pIdentidadID, Guid pProyectoID)
        {
            List<QueryTriples> resultadoConsulta;

            var consulta1 = mEntityContext.Identidad.JoinProyecto().JoinPerfil().JoinPersona().Where(item => !item.Persona.Eliminado && !item.Perfil.Eliminado && new int[] { 0, 1, 4 }.Contains(item.Identidad.Tipo) && item.Identidad.IdentidadID.Equals(pIdentidadID) && !string.IsNullOrEmpty(item.Persona.LocalidadPersonal)).ToList();

            var consulta2 = mEntityContext.Identidad.JoinPerfil().JoinPersona().JoinPais().Where(item => !item.Persona.Eliminado && item.Persona.PaisPersonalID.HasValue && !item.Perfil.Eliminado && new int[] { 0, 1, 4 }.Contains(item.Identidad.Tipo) && item.Identidad.IdentidadID.Equals(pIdentidadID)).ToList();

            var consulta3 = mEntityContext.Identidad.JoinPerfil().JoinPersona().Where(item => !item.Persona.Eliminado && !item.Perfil.Eliminado && new int[] { 0, 1, 4 }.Contains(item.Identidad.Tipo) && item.Identidad.IdentidadID.Equals(pIdentidadID) && !string.IsNullOrEmpty(item.Persona.LocalidadPersonal)).ToList();

            var consulta4 = mEntityContext.Proyecto.JoinOrganizacionParticipaProy().JoinOrganizacion().JoinPais().JoinPerfil().JoinIdentidad().Where(item => !item.Organizacion.Eliminada && item.Identidad.Tipo == 3 && !item.Perfil.Eliminado && !item.Perfil.PersonaID.HasValue && item.Identidad.IdentidadID.Equals(pIdentidadID) && item.Organizacion.PaisID.HasValue).ToList();

            var consulta5 = mEntityContext.Identidad.JoinPerfil().JoinPersona().JoinProvincia().Where(item => !item.Persona.Eliminado && new int[] { 0, 1, 4}.Contains(item.Identidad.Tipo) && !item.Perfil.Eliminado && item.Identidad.IdentidadID.Equals(pIdentidadID)).ToList();

            var consulta6 = mEntityContext.Identidad.JoinPerfil().JoinPersona().Where(item => !item.Persona.Eliminado && new int[] { 0, 1, 4 }.Contains(item.Identidad.Tipo) && !item.Perfil.Eliminado && item.Identidad.IdentidadID.Equals(pIdentidadID) && !string.IsNullOrEmpty(item.Persona.ProvinciaPersonal)).ToList();

            resultadoConsulta = consulta1.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Identidad.IdentidadID.ToString().ToUpper()}>",
                Predicado = "<http://www.w3.org/2006/vcard/ns#locality>",
                Objeto = $"\"{item.Persona.LocalidadPersonal}\" ."
            }).Union(consulta2.Select(item => new QueryTriples 
            {
                Sujeto = $"<http://gnoss/{item.Identidad.IdentidadID.ToString().ToUpper()}>",
                Predicado = "<http://www.w3.org/2006/vcard/ns#country-name>",
                Objeto = $"\"{item.Pais.Nombre}\" ."
            })).Union(consulta3.Select(item => new QueryTriples 
            {
                Sujeto = $"<http://gnoss/{item.Identidad.IdentidadID.ToString().ToUpper()}>",
                Predicado = "<http://www.w3.org/2006/vcard/ns#locality>",
                Objeto = $"\"{item.Persona.LocalidadPersonal}\" ."
            })).Union(consulta4.Select(item => new QueryTriples 
            {
                Sujeto = $"<http://gnoss/{item.Identidad.IdentidadID.ToString().ToUpper()}>",
                Predicado = "<http://www.w3.org/2006/vcard/ns#country-name>",
                Objeto = $"\"{item.Pais.Nombre}\" ."
            })).Union(consulta5.Select(item => new QueryTriples 
            {
                Sujeto = $"<http://gnoss/{item.Identidad.IdentidadID.ToString().ToUpper()}>",
                Predicado = "<http://d.opencalais.com/1/type/er/Geo/ProvinceOrState>",
                Objeto = $"\"{item.Provincia.Nombre}\" ."
            })).Union(consulta6.Select(item => new QueryTriples 
            {
                Sujeto = $"<http://gnoss/{item.Identidad.IdentidadID.ToString().ToUpper()}>",
                Predicado = "<http://d.opencalais.com/1/type/er/Geo/ProvinceOrState>",
                Objeto = $"\"{item.Persona.ProvinciaPersonalID}\" ."
            })).Distinct().ToList();

            return resultadoConsulta;
        }

        public List<QueryTriples> ObtenerIdentidadesSigueIdentidadDeProyecto(Guid pProyectoID, Guid pPersonaID)
        {
            List<QueryTriples> resultadoConsulta;

            List<Guid> listaIdentidadID = mEntityContext.Identidad.JoinIdentidadPerfil().JoinPerfil().Where(item => item.IdentidadPerfil.ProyectoID.Equals(pProyectoID) && item.Perfil.PersonaID.HasValue && item.Perfil.PersonaID.Value.Equals(pPersonaID) && item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto)).Select(item => item.Identidad.IdentidadID).ToList();

            var query = mEntityContext.Suscripcion.JoinSuscripcionIdentidadProyecto().JoinIdentidad().JoinIdentidadEnProyecto().Where(item => item.SuscripcionIdentidadProyecto.ProyectoID.Equals(pProyectoID) && item.IdentidadEnProyecto.ProyectoID.Equals(pProyectoID) && listaIdentidadID.Contains(item.Suscripcion.IdentidadID));

            resultadoConsulta = query.Select(item => new QueryTriples 
            {
                Sujeto = $"<http://gnoss/{item.IdentidadEnProyecto.IdentidadID.ToString().ToUpper()}>",
                Predicado = "<http://xmlns.com/foaf/0.1/knows>",
                Objeto = $"<http://gnoss/{item.SuscripcionIdentidadProyecto.IdentidadID.ToString().ToUpper()}> ."
            }).Distinct().ToList();

            return resultadoConsulta;
        }

        public List<QueryTriples> ObtenerCategoriasTesauroDeProyectoSuscritaIdentidad(Guid pProyectoID, Guid pIdentidadID)
        {
            List<QueryTriples> resultadoConsulta;

            var query = mEntityContext.Suscripcion.JoinSuscripcionTesauroProyecto().JoinCategoriaTesVinSuscrip().Where(item => item.SuscripcionTesauroProyecto.ProyectoID.Equals(pProyectoID) && item.Suscripcion.IdentidadID.Equals(pIdentidadID));

            resultadoConsulta = query.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Suscripcion.IdentidadID.ToString().ToUpper()}>",
                Predicado = "<http://xmlns.com/foaf/0.1/interest>",
                Objeto = $"<http://gnoss/{item.CategoriaTesVinSuscrip.CategoriaTesauroID}> ."
            }).Distinct().ToList();

            return resultadoConsulta;
        }

        public List<QueryTriples> ObtenerIdentidadDatoExtraRegistroDeProyecto(Guid pProyectoID, Guid pIdentidadID)
        {
            List<QueryTriples> resultadoConsulta;

            var consultaDatoExtraProyectoOpcion = mEntityContext.DatoExtraProyecto.JoinDatoExtraProyectoOpcion().JoinDatoExtraProyectoOpcionIdentidad().Where(item => item.DatoExtraProyecto.ProyectoID.Equals(pProyectoID) && item.DatoExtraProyectoOpcionIdentidad.IdentidadID.Equals(pIdentidadID)).ToList();

            var consultaDatoExtraProyectoVirtuoso = mEntityContext.DatoExtraProyectoVirtuosoIdentidad.JoinDatoExtraProyectoVirtuoso().Where(item => item.DatoExtraProyectoVirtuosoIdentidad.ProyectoID.Equals(pProyectoID) && item.DatoExtraProyectoVirtuosoIdentidad.IdentidadID.Equals(pIdentidadID)).ToList();

            var consultaDatoExtraEcosistemaOpcion = mEntityContext.DatoExtraEcosistema.JoinDatoExtraEcosistemaOpcion().JoinDatoExtraEcosistemaOpcionPerfil().JoinIdentidad().Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID)).ToList();

            var consultaDatoExtraEcosistemaVirtuoso = mEntityContext.DatoExtraEcosistemaVirtuosoPerfil.JoinDatoExtraEcosistemaVirtuoso().JoinIdentidad().Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID)).ToList();

            resultadoConsulta = consultaDatoExtraProyectoOpcion.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.DatoExtraProyectoOpcionIdentidad.IdentidadID.ToString().ToUpper()}>",
                Predicado = $"<{item.DatoExtraProyecto.PredicadoRDF}>",
                Objeto = $"\"{item.DatoExtraProyectoOpcion.Opcion}\" ."
            }).Union(consultaDatoExtraProyectoVirtuoso.Select(item => new QueryTriples 
            {
                Sujeto = $"<http://gnoss/{item.DatoExtraProyectoVirtuosoIdentidad.IdentidadID.ToString().ToUpper()}>",
                Predicado = $"<{item.DatoExtraProyectoVirtuoso.PredicadoRDF}>",
                Objeto = $"\"{item.DatoExtraProyectoVirtuosoIdentidad.Opcion}\" ."
            })).Union(consultaDatoExtraEcosistemaOpcion.Select(item => new QueryTriples 
            {
                Sujeto = $"<http://gnoss/{item.Identidad.IdentidadID.ToString().ToUpper()}>",
                Predicado = $"<{item.DatoExtraEcosistema.PredicadoRDF}>",
                Objeto = $"\"{item.DatoExtraEcosistemaOpcion.Opcion}\" ."
            })).Union(consultaDatoExtraEcosistemaVirtuoso.Select(item => new QueryTriples 
            {
                Sujeto = $"<http://gnoss/{item.Identidad.IdentidadID.ToString().ToUpper()}>",
                Predicado = $"<{item.DatoExtraEcosistemaVirtuoso.PredicadoRDF}>",
                Objeto = $"\"{item.DatoExtraEcosistemaVirtuosoPerfil.Opcion}\" ."
            })).Distinct().ToList();

            return resultadoConsulta;
        }

        /// <summary>
        /// Obtiene la información extra de una organizacion
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        public List<QueryTriples> ObtieneInformacionExtraOrganizacion(Guid pIdentidadID, Guid pProyectoID)
        {
            List<QueryTriples> resultadosConsulta = new List<QueryTriples>();
            string dateFormat = "yyyyMMddHHmmdd";

            var consulta1 = mEntityContext.Identidad.JoinPerfil().JoinPersona().Where(item => item.Persona.Foto != null && (!item.Perfil.OrganizacionID.HasValue || item.Perfil.OrganizacionID.Equals(Guid.Empty)) && item.Identidad.IdentidadID.Equals(pIdentidadID) && item.Perfil.PersonaID.HasValue);

            foreach(var query in consulta1)
            {
                if (query.Persona.VersionFoto.HasValue)
                {
                    resultadosConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasfoto>", Objeto = $"\"/Personas/{query.Perfil.PersonaID.Value}_peque.png?{query.Persona.VersionFoto.Value}\" ." });
                }
                else
                {
                    resultadosConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasfoto>", Objeto = $"\"/Personas/{query.Perfil.PersonaID.Value}_peque.png\" ." });
                }
            }

            var consulta2 = mEntityContext.Identidad.JoinPerfil().JoinOrganizacion().Where(item => item.Identidad.Tipo > 1 && item.Identidad.Tipo < 4 && item.Organizacion.Logotipo != null && item.Identidad.IdentidadID.Equals(pIdentidadID));

            foreach(var query in consulta2)
            {
                if (query.Organizacion.VersionLogo.HasValue)
                {
                    resultadosConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasfoto>", Objeto = $"\"/{UtilArchivos.ContentImagenesOrganizaciones}/{query.Organizacion.OrganizacionID}_peque.png?{query.Organizacion.VersionLogo.Value}\" ." });
                }
                else
                {
                    resultadosConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasfoto>", Objeto = $"\"/{UtilArchivos.ContentImagenesOrganizaciones}/{query.Organizacion.OrganizacionID}_peque.png\" ." });
                }
            }

            var consulta3 = mEntityContext.Identidad.JoinPerfil().JoinPersona().JoinPersonaVinculoOrganizacion().Where(item => item.Persona.Foto != null && item.Identidad.Tipo == 1 && item.PersonaVinculoOrganizacion.UsarFotoPersonal && item.Identidad.IdentidadID.Equals(pIdentidadID) && item.Perfil.PersonaID.HasValue);
            
            foreach(var query in consulta3)
            {
                if (query.Persona.VersionFoto.HasValue)
                {
                    resultadosConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasfoto>", Objeto = $"\"/Personas/{query.Perfil.PersonaID.Value}_peque.png?{query.Persona.VersionFoto}\" ." });
                }
                else
                {
                    resultadosConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasfoto>", Objeto = $"\"/Personas/{query.Perfil.PersonaID.Value}_peque.png\" ." });
                }
            }

            var consulta4 = mEntityContext.Identidad.JoinPerfil().JoinPersona().JoinPersonaVinculoOrganizacion().Where(item => item.Identidad.Tipo == 1 && !item.PersonaVinculoOrganizacion.UsarFotoPersonal && item.PersonaVinculoOrganizacion.Foto != null && item.Identidad.IdentidadID.Equals(pIdentidadID) && item.Perfil.OrganizacionID.HasValue && item.Perfil.PersonaID.HasValue);

            foreach(var query in consulta4)
            {
                if (query.PersonaVinculoOrganizacion.VersionFoto.HasValue)
                {
                    resultadosConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasfoto>", Objeto = $"\"/Persona_Organizacion/{query.Perfil.OrganizacionID.Value}/{query.Perfil.PersonaID.Value}_peque.png?{query.PersonaVinculoOrganizacion.VersionFoto}\" ." });
                }
                else
                {
                    resultadosConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasfoto>", Objeto = $"\"/Persona_Organizacion/{query.Perfil.OrganizacionID.Value}/{query.Perfil.PersonaID.Value}_peque.png\" ." });
                }
            }

            var consulta5 = mEntityContext.Identidad.JoinPerfil().JoinPersona().Where(item => item.Persona.Foto == null && !item.Perfil.OrganizacionID.HasValue && item.Identidad.IdentidadID.Equals(pIdentidadID));

            foreach(var query in consulta5)
            {
                resultadosConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasfoto>", Objeto = "\"sinfoto\" ." });
            }

            var consulta6 = mEntityContext.Organizacion.JoinPerfil().JoinIdentidad().Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID) && item.Identidad.Tipo == 3 && !item.Perfil.Eliminado);

            foreach(var query in consulta6)
            {
                resultadosConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://xmlns.com/foaf/0.1/firstName>", Objeto = $"\"{RemplazarCaracteresBD(query.Organizacion.Nombre)}\"" });
            }

            var consulta7 = mEntityContext.Organizacion.JoinPerfil().JoinIdentidad().Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID) && !item.Identidad.FechaBaja.HasValue && new int[] { 2, 3 }.Contains(item.Identidad.Tipo));

            foreach (var query in consulta7)
            {
                resultadosConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://xmlns.com/foaf/0.1/group>", Objeto = $"\"{RemplazarCaracteresBD(query.Organizacion.Nombre)}\" ." });
            }

            var consulta8 = mEntityContext.Organizacion.JoinPerfil().JoinIdentidad().JoinPais().Where(item => !item.Organizacion.Eliminada && item.Identidad.Tipo == 3 && !item.Perfil.Eliminado && !item.Perfil.PersonaID.HasValue && item.Identidad.IdentidadID.Equals(pIdentidadID));

            foreach (var query in consulta8)
            {
                resultadosConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://www.w3.org/2006/vcard/ns#country-name>", Objeto = $"\"{query.Pais.Nombre}\" ." });
            }

            var consulta9 = mEntityContext.Organizacion.JoinPerfil().JoinIdentidad().JoinProvincia().Where(item => !item.Organizacion.Eliminada && item.Identidad.Tipo == 3 && !item.Perfil.Eliminado && !item.Perfil.PersonaID.HasValue && item.Identidad.IdentidadID.Equals(pIdentidadID));

            foreach(var query in consulta9)
            {
                resultadosConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://d.opencalais.com/1/type/er/Geo/ProvinceOrState>", Objeto = $"\"{query.Provincia.Nombre}\" ." });
            }

            var consulta10 = mEntityContext.Organizacion.JoinPerfil().JoinIdentidad().Where(item => !item.Organizacion.Eliminada && item.Identidad.Tipo == 3 && !item.Perfil.Eliminado && !item.Perfil.PersonaID.HasValue && item.Identidad.IdentidadID.Equals(pIdentidadID));

            foreach (var query in consulta10)
            {
                resultadosConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://www.w3.org/2006/vcard/ns#locality>", Objeto = $"\"{query.Organizacion.Localidad}\" ." });
            }

            var consulta11 = mEntityContext.Organizacion.JoinPerfil().JoinIdentidad().Where(item => item.Identidad.Tipo == 3 && !item.Perfil.Eliminado && item.Identidad.IdentidadID.Equals(pIdentidadID));

            foreach (var query in consulta11)
            {
                resultadosConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://xmlns.com/foaf/0.1/familyName>", Objeto = $"\"{query.Organizacion.Nombre}\" ." });
            }

            var consulta12 = mEntityContext.Identidad.Where(item => item.IdentidadID.Equals(pIdentidadID));

            foreach(var query in consulta12)
            {
                resultadosConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{query.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasPerfilProyecto>", Objeto = $"<http://gnoss/{query.IdentidadID.ToString().ToUpper()}> ." });
                resultadosConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{query.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasPerfil>", Objeto = $"<http://gnoss/{query.PerfilID.ToString().ToUpper()}> ." });
            }

            var consulta13 = mEntityContext.Identidad.JoinPerfil().Where(item => !string.IsNullOrEmpty(item.Perfil.NombreCortoUsu) && item.Identidad.IdentidadID.Equals(pIdentidadID));

            foreach(var query in consulta13)
            {
                resultadosConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://gnoss/nombreCortoUsu>", Objeto = $"\"{query.Perfil.NombreCortoUsu}\" ." });
            }

            var consulta14 = mEntityContext.Identidad.JoinPerfil().Where(item => !string.IsNullOrEmpty(item.Perfil.NombreCortoOrg) && item.Identidad.IdentidadID.Equals(pIdentidadID));

            foreach(var query in consulta14)
            {
                resultadosConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://gnoss/nombreCortoOrg>", Objeto = $"\"{query.Perfil.NombreCortoOrg}\" ." });
            }

            var consulta15 = mEntityContext.Organizacion.JoinPerfil().JoinIdentidad().Where(item => !item.Organizacion.Eliminada && item.Identidad.Tipo == 3 && !item.Perfil.Eliminado && !item.Perfil.PersonaID.HasValue && item.Identidad.IdentidadID.Equals(pIdentidadID));

            foreach(var query in consulta15)
            {
                resultadosConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasfechaAlta>", Objeto = $"{query.Identidad.FechaAlta.ToString(dateFormat)} ." });
            }

            if (!pProyectoID.Equals(ProyectoAD.MetaProyecto))
            {
                List<Guid> listaOrganizacionClaseID = mEntityContext.OrganizacionClase.Select(item => item.OrganizacionID).ToList();

                var consulta16 = mEntityContext.Organizacion.JoinPerfil().JoinIdentidad().Where(item => !item.Organizacion.Eliminada && item.Identidad.Tipo == 3 && !item.Perfil.Eliminado && !item.Perfil.PersonaID.HasValue && !listaOrganizacionClaseID.Contains(item.Organizacion.OrganizacionID) && item.Identidad.IdentidadID.Equals(pIdentidadID));

                foreach(var query in consulta16)
                {
                    resultadosConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>", Objeto = $"\"Organizacion\" ." });
                }

                var consulta17 = mEntityContext.Organizacion.JoinPerfil().JoinIdentidad().JoinOrganizacionClase().Where(item => !item.Organizacion.Eliminada && item.Identidad.Tipo == 3 && !item.Perfil.Eliminado && !item.Perfil.PersonaID.HasValue && item.OrganizacionClase.TipoClase == (short)TipoClase.EducacionPrimaria && item.Identidad.IdentidadID.Equals(pIdentidadID));

                foreach(var query in consulta17)
                {
                    resultadosConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>", Objeto = $"\"clase de primaria\" ." });
                }

                var consulta18 = mEntityContext.Organizacion.JoinPerfil().JoinIdentidad().JoinOrganizacionClase().Where(item => !item.Organizacion.Eliminada && item.Identidad.Tipo == 3 && !item.Perfil.Eliminado && !item.Perfil.PersonaID.HasValue && item.OrganizacionClase.TipoClase == (short)TipoClase.EducacionExpandida && item.Identidad.IdentidadID.Equals(pIdentidadID));

                foreach(var query in consulta18)
                {
                    resultadosConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>", Objeto = $"\"clase de secundaria\" ." });
                }

                var consulta19 = mEntityContext.Organizacion.JoinPerfil().JoinIdentidad().JoinOrganizacionClase().Where(item => !item.Organizacion.Eliminada && item.Identidad.Tipo == 3 && !item.Perfil.Eliminado && !item.Perfil.PersonaID.HasValue && item.OrganizacionClase.TipoClase == (short)TipoClase.Universidad20 && item.Identidad.IdentidadID.Equals(pIdentidadID));

                foreach(var query in consulta19)
                {
                    resultadosConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>", Objeto = $"\"clase de universidad\" ." });
                }

                var consulta20 = mEntityContext.Organizacion.JoinPerfil().JoinIdentidad().Where(item => !item.Organizacion.Eliminada && item.Identidad.Tipo == 3 && !item.Perfil.Eliminado && !item.Perfil.PersonaID.HasValue && item.Identidad.IdentidadID.Equals(pIdentidadID));

                foreach(var query in consulta20)
                {
                    resultadosConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://rdfs.org/sioc/ns#has_space>", Objeto = $"<http://gnoss/{query.Identidad.ProyectoID}> ." });
                }

                var consulta21 = mEntityContext.Identidad.JoinProyecto().JoinPerfil().JoinIndetidadOrgComunidades().Where(item => item.Perfil.OrganizacionID.HasValue && item.Identidad.IdentidadID.Equals(pIdentidadID) && item.Identidad.Rank.HasValue && !item.Identidad.ProyectoID.Equals(new Guid("11111111-1111-1111-1111-111111111111")) && item.IdentidadOrgComunidades.Tipo == 3).GroupBy(item => item.IdentidadOrgComunidades.IdentidadID).Select(item => new { IdentidadID = item.Key, SumRank = item.Sum(item2 => item2.Identidad.Rank)});

                foreach(var query in consulta21)
                {
                    resultadosConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{query.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasPopularidad>", Objeto = $"{query.SumRank} ." });
                }
            }
            else
            {
                List<Guid> listaOrganizacionClaseID = mEntityContext.OrganizacionClase.Select(item => item.OrganizacionID).ToList();

                var consulta16 = mEntityContext.Organizacion.JoinPerfil().JoinIdentidad().Where(item => !item.Organizacion.Eliminada && item.Identidad.Tipo == 3 && !item.Perfil.Eliminado && !item.Perfil.PersonaID.HasValue && !listaOrganizacionClaseID.Contains(item.Organizacion.OrganizacionID) && item.Identidad.ProyectoID.Equals(new Guid("11111111-1111-1111-1111-111111111111")) && item.Identidad.IdentidadID.Equals(pIdentidadID));

                foreach (var query in consulta16)
                {
                    resultadosConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>", Objeto = $"\"Organizacion\" ." });
                }

                var consulta17 = mEntityContext.Organizacion.JoinPerfil().JoinIdentidad().JoinOrganizacionClase().Where(item => !item.Organizacion.Eliminada && item.Identidad.Tipo == 3 && !item.Perfil.Eliminado && item.Identidad.ProyectoID.Equals(new Guid("11111111-1111-1111-1111-111111111111")) && !item.Perfil.PersonaID.HasValue && item.OrganizacionClase.TipoClase == 1 && item.Identidad.IdentidadID.Equals(pIdentidadID));

                foreach(var query in consulta17)
                {
                    resultadosConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>", Objeto = $"\"clase de secundaria\" ." });
                }

                var consulta18 = mEntityContext.Organizacion.JoinPerfil().JoinIdentidad().JoinOrganizacionClase().Where(item => !item.Organizacion.Eliminada && item.Identidad.Tipo == 3 && !item.Perfil.Eliminado && item.Identidad.ProyectoID.Equals(new Guid("11111111-1111-1111-1111-111111111111")) && !item.Perfil.PersonaID.HasValue && item.OrganizacionClase.TipoClase == 0 && item.Identidad.IdentidadID.Equals(pIdentidadID));

                foreach (var query in consulta18)
                {
                    resultadosConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>", Objeto = $"\"clase de universidad\" ." });
                }

                var consulta19 = mEntityContext.Organizacion.JoinPerfil().JoinIdentidadMyGnoss().JoinIdentidad().Where(item => !item.Organizacion.Eliminada && item.IdentidadMyGnoss.Tipo == 3 && !item.Perfil.Eliminado && !item.Perfil.PersonaID.HasValue && item.IdentidadMyGnoss.ProyectoID.Equals(new Guid("11111111-1111-1111-1111-111111111111")) && !item.Identidad.ProyectoID.Equals(new Guid("11111111-1111-1111-1111-111111111111")) && item.Identidad.IdentidadID.Equals(pIdentidadID));

                foreach(var query in consulta19)
                {
                    resultadosConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{query.Identidad.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://rdfs.org/sioc/ns#has_space>", Objeto = $"<http://gnoss/{query.IdentidadMyGnoss.IdentidadID.ToString().ToUpper()}> ." });
                }

                var consulta20 = mEntityContext.Identidad.JoinProyecto().JoinPerfil().JoinIndetidadOrgComunidades().Where(item => item.Perfil.OrganizacionID.HasValue && item.Identidad.IdentidadID.Equals(pIdentidadID) && item.Identidad.Rank.HasValue && item.IdentidadOrgComunidades.ProyectoID.Equals(new Guid("11111111-1111-1111-1111-111111111111")) && item.IdentidadOrgComunidades.Tipo == 3).GroupBy(item => item.IdentidadOrgComunidades.IdentidadID).Select(item => new { IdentidadID = item.Key, SumRank = item.Sum(item2 => item2.Identidad.Rank) });

                foreach (var query in consulta20)
                {
                    resultadosConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{query.IdentidadID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasPopularidad>", Objeto = $"{query.SumRank} ." });
                }
            }
            return resultadosConsulta;
        }

        public List<QueryTriples> ObtieneInformacionExtraOrganizacionContactos(Guid pIdentidadID)
        {
            List<QueryTriples> resultadoConsulta;

            var consulta1 = mEntityContext.Organizacion.JoinPerfil().JoinIdentidad().JoinPais().Where(item => !item.Organizacion.Eliminada && item.Identidad.Tipo == 3 && !item.Perfil.Eliminado && !item.Perfil.PersonaID.HasValue && item.Identidad.IdentidadID.Equals(pIdentidadID) && item.Organizacion.PaisID.HasValue).ToList();

            var consulta2 = mEntityContext.Organizacion.JoinPerfil().JoinIdentidad().JoinProvincia().Where(item => !item.Organizacion.Eliminada && item.Identidad.Tipo == 3 && !item.Perfil.Eliminado && !item.Perfil.PersonaID.HasValue && item.Identidad.IdentidadID.Equals(pIdentidadID)).ToList();

            var consulta3 = mEntityContext.Organizacion.JoinPerfil().JoinIdentidad().Where(item => !item.Organizacion.Eliminada && item.Identidad.Tipo == 3 && !item.Perfil.Eliminado && !item.Perfil.PersonaID.HasValue && item.Identidad.IdentidadID.Equals(pIdentidadID)).ToList();

            resultadoConsulta = consulta1.Select(item => new QueryTriples
            {
                Sujeto = $"<http://gnoss/{item.Identidad.IdentidadID.ToString().ToUpper()}>",
                Predicado = "<http://www.w3.org/2006/vcard/ns#country-name>",
                Objeto = $"\"{item.Pais.Nombre}\" ."
            }).Union(consulta2.Select(item => new QueryTriples 
            {
                Sujeto = $"<http://gnoss/{item.Identidad.IdentidadID.ToString().ToUpper()}>",
                Predicado = "<http://d.opencalais.com/1/type/er/Geo/ProvinceOrState>",
                Objeto = $"\"{item.Provincia.Nombre}\" ."
            })).Union(consulta3.Select(item => new QueryTriples 
            {
                Sujeto = $"<http://gnoss/{item.Identidad.IdentidadID.ToString().ToUpper()}>",
                Predicado = "<http://www.w3.org/2006/vcard/ns#locality>",
                Objeto = $"\"{item.Organizacion.Localidad}\" ."
            })).Distinct().ToList();

            return resultadoConsulta;
        }


        /// <summary>
        /// Obtiene la informacion extra de la comunidad
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto (comunidad)</param>
        public List<QueryTriples> ObtieneInformacionExtraCom(Guid pProyectoID)
        {
            List<QueryTriples> resultadoConsulta = new List<QueryTriples>();
            string dateFormat = "yyyyMMddHHmmdd";

            var consulta1 = mEntityContext.Proyecto.Where(item => item.ProyectoID.Equals(pProyectoID));
            foreach(var query in consulta1)
            {
                resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{query.ProyectoID.ToString().ToUpper()}>", Predicado = "<http://xmlns.com/foaf/0.1/firstName>", Objeto = $"\"{RemplazarCaracteresBD(query.Nombre)}\"" });
            }

            var parametroAplicacion = mEntityContext.ParametroAplicacion.Where(item => item.Parametro.Equals("ComunidadesPrivadasVisibles") && item.Valor.Equals("true")).FirstOrDefault();

            var consulta2 = mEntityContext.Proyecto.Where(item => new int[] { 0, 2 }.Contains(item.TipoAcceso) && parametroAplicacion != null && item.ProyectoID.Equals(pProyectoID));
            foreach(var query in consulta2)
            {
                resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{query.ProyectoID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasprivacidadMyGnoss>", Objeto = $"\"publico\" ." });
            }

            var consulta3 = mEntityContext.Proyecto.JoinAdministradorProyecto().JoinPersona().JoinPerfil().JoinIdentidad().Where(item => !item.Persona.Eliminado && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue && !item.Perfil.Eliminado && new int[] { 0, 4 }.Contains(item.Identidad.Tipo) && item.Proyecto.ProyectoID.Equals(pProyectoID));
            foreach(var query in consulta3)
            {
                resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{query.Proyecto.ProyectoID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasAdministrador>", Objeto = $"\"{RemplazarCaracteresBD(query.Perfil.NombrePerfil)}\" ." });
            }

            var consulta4 = mEntityContext.Proyecto.JoinAdministradorProyecto().JoinPersona().JoinPerfil().JoinIdentidad().Where(item => !item.Persona.Eliminado && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue && !item.Perfil.Eliminado && new int[] { 1, 2 }.Contains(item.Identidad.Tipo) && item.Proyecto.ProyectoID.Equals(pProyectoID));
            foreach(var query in consulta4)
            {
                resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{query.Proyecto.ProyectoID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasAdministrador>", Objeto = $"\"{RemplazarCaracteresBD(query.Perfil.NombreOrganizacion)}\" ." });
            }

            var consulta5 = mEntityContext.Proyecto.Where(item => (item.TipoProyecto == (short)TipoProyecto.Universidad20 || item.TipoProyecto == (short)TipoProyecto.EducacionExpandida || item.TipoProyecto == (short)TipoProyecto.EducacionPrimaria) && item.ProyectoID.Equals(pProyectoID));
            foreach(var query in consulta5)
            {
                resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{query.ProyectoID.ToString().ToUpper()}>", Predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>", Objeto = $"\"Comunidad educativa\" ." });
            }

            var consulta6 = mEntityContext.Proyecto.Where(item => (item.TipoProyecto != (short)TipoProyecto.Universidad20 || item.TipoProyecto != (short)TipoProyecto.EducacionExpandida || item.TipoProyecto != (short)TipoProyecto.EducacionPrimaria) && item.ProyectoID.Equals(pProyectoID));
            foreach (var query in consulta6)
            {
                resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{query.ProyectoID.ToString().ToUpper()}>", Predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>", Objeto = $"\"Comunidad no educativa\" ." });
            }

            var consulta7 = mEntityContext.Proyecto.Where(item => item.ProyectoID.Equals(pProyectoID));
            foreach(var query in consulta7)
            {
                resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{query.ProyectoID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasnumerorecursos>", Objeto = $"{query.NumeroArticulos} ." });
            }

            var consulta8 = mEntityContext.Proyecto.Where(item => item.ProyectoID.Equals(pProyectoID) && item.FechaInicio.HasValue);
            foreach(var query in consulta8)
            {
                resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{query.ProyectoID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasfechaAlta>", Objeto = $"{query.FechaInicio.Value.ToString(dateFormat)} ." });
            }

            var consulta9 = mEntityContext.ProyectosMasActivos.Where(item => item.ProyectoID.Equals(pProyectoID));
            foreach (var query in consulta9)
            {
                resultadoConsulta.Add(new QueryTriples { Sujeto = $"<http://gnoss/{query.ProyectoID.ToString().ToUpper()}>", Predicado = "<http://gnoss/hasPopularidad>", Objeto = $"{query.Peso} ." });
            }

            return resultadoConsulta;
        }

        private string RemplazarCaracteresBD(string pCadena)
        {
            if (string.IsNullOrEmpty(pCadena))
            {
                return "";
            }
            else
            {
                return pCadena.Replace("\r\n", "").Replace("\r", "").Replace("\n", "").Replace("\\", " ");//.Replace("\"", "\\\"");
            }
            
        }

        private string RemplazarCaracteresBDSeparadorOrganizacion(string pCadena)
        {
            if (string.IsNullOrEmpty(pCadena))
            {
                return "";
            }
            else
            {
                return $" {ConstantesDeSeparacion.SEPARACION_CONCATENADOR} {pCadena.Replace("\r\n", "").Replace("\r", "").Replace("\n", "").Replace("\\", " ")}";//.Replace("\"", "\\\"")}";
            }

        }

        private string FormatearRemplazos(string pNombreColumna)
        {
            if (EsPostgres())
            {
                return $"'\"' || REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(\"{pNombreColumna}\", '\"', ''), CHR(13)||CHR(10)||CHR(10)||CHR(13), ' '), CHR(13), ' '), CHR(10), ' '), '\', ' ') || '\" .'";
            }
            else
            {
                return $"'\"'+ CONVERT(nvarchar(1000), REPLACE(replace(REPLACE(replace(replace(ISNULL(\"{pNombreColumna}\", ''),'\"',''), char(13)+char(10)+char(10)+char(13), ' '), CHAR(13), ' '), CHAR(10), ' '), '\', ' ') ) + '\" .'";
            }
        }

        private string FormatearRemplazosPerfilOrganizacion(string pNombrePerfil, string pNombreOrganizacion)
        {
            if (EsPostgres())
            {
                return $"'\"' || LOWER(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(\"{pNombrePerfil}\" + ' @ ' + \"{pNombreOrganizacion}\", '\"', ''), CHR(13)||CHR(10)||CHR(10)||CHR(13), ' '), CHR(13), ' '), CHR(10), ' '), '\', ' ')) || '\" .'";
            }
            else
            {
                return $"'\"'+ lower(CONVERT(nvarchar(1000), REPLACE(replace(REPLACE(replace(replace(ISNULL(\"{pNombrePerfil}\" + ISNULL(' @ ' + \"{pNombreOrganizacion}\", ''), ''),'\"',''), char(13)+char(10)+char(10)+char(13), ' '), CHAR(13), ' '), CHAR(10), ' '), '\', ' '))) + '\" .'";
            }
        }

        private string FormatearFecha(string pNombreColumna)
        {
            if (EsPostgres())
            {
                return $"SELECT EXTRACT(YEAR FROM \"{pNombreColumna}\") + EXTRACT(MONTH FROM \"{pNombreColumna}\") + EXTRACT(DAY FROM \"{pNombreColumna}\") + EXTRACT(HOUR FROM \"{pNombreColumna}\") + EXTRACT(MINUTE FROM \"{pNombreColumna}\") + EXTRACT(SECOND FROM \"{pNombreColumna}\") || ' .'";
            }
            else
            {
                return $"CONVERT(nvarchar(1000), YEAR(\"{pNombreColumna}\"))+ REVERSE(SUBSTRING(REVERSE('0'+CONVERT(nvarchar(1000), CONVERT(nvarchar(1000), MONTH(\"{pNombreColumna}\")))),0,3 )) + REVERSE(SUBSTRING(REVERSE('0'+CONVERT(nvarchar(1000), CONVERT(nvarchar(1000), DAY(\"{pNombreColumna}\"))) ),0,3 ))+ REVERSE(SUBSTRING(REVERSE('0'+ CONVERT(nvarchar(1000), DATEPART(hour, \"{pNombreColumna}\"))) , 0, 3))+REVERSE(SUBSTRING(REVERSE('0'+ CONVERT(nvarchar(1000), DATEPART(minute, \"{pNombreColumna}\")) ),0,3 ))+ REVERSE(SUBSTRING(REVERSE('0'+ CONVERT(nvarchar(1000), DATEPART(second, \"{pNombreColumna}\"))),0,3 )) + ' .'";
            }
        }
    }
}
