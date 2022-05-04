using Es.Riam.Gnoss.AD.CMS;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.Live;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Es.Riam.Gnoss.Elementos.CMS
{
    public static class UtilComponentes
    {
        #region Miembros y constructores estáticos

        /// <summary>
        /// Lista de propiedades disponibles por tipo de componente
        /// </summary>
        public static Dictionary<TipoComponenteCMS, Dictionary<TipoPropiedadCMS, bool>> PropiedadesDisponiblesPorTipoComponente = new Dictionary<TipoComponenteCMS, Dictionary<TipoPropiedadCMS, bool>>();

        /// <summary>
        /// Lista de componenetes que se cargan por AJAX
        /// </summary>
        public static List<TipoComponenteCMS> ListaComponentesAjax = new List<TipoComponenteCMS>();

        /// <summary>
        /// Lista de caducidades disponibles por tipo de componente
        /// </summary>
        public static Dictionary<TipoComponenteCMS, List<TipoCaducidadComponenteCMS>> CaducidadesDisponiblesPorTipoComponente = new Dictionary<TipoComponenteCMS, List<TipoCaducidadComponenteCMS>>();

        /// <summary>
        /// Lista de propiedades multiidioma (individuales)
        /// </summary>
        public static List<TipoPropiedadCMS> ListaPropiedadesMultiIdioma = new List<TipoPropiedadCMS>();

        /// <summary>
        /// Lista de componentes publicos
        /// </summary>
        public static List<TipoComponenteCMS> ListaComponentesPublicos = new List<TipoComponenteCMS>();

        /// <summary>
        /// Lista de componentes privados
        /// </summary>
        public static List<TipoComponenteCMS> ListaComponentesPrivados = new List<TipoComponenteCMS>();

        /// <summary>
        /// Lista de datos posibles del usuario actual en las busquedas
        /// </summary>
        public static List<string> ListaDatosUsuarioActual = new List<string>();

        /// <summary>
        /// Constructor estático
        /// </summary>
        static UtilComponentes()
        {
            //ListaDatosUsuarioActual
            ListaDatosUsuarioActual.Add("<USUARIOID>");
            ListaDatosUsuarioActual.Add("<IDENTIDADID>");
            ListaDatosUsuarioActual.Add("<PROYECTOID>");
            ListaDatosUsuarioActual.Add("<PROYECTOID_MINUS>");
            ListaDatosUsuarioActual.Add("<NOMBRECORTO_PROYECTO>");

            #region Configuracion de cada componente

            #region HTML
            Dictionary<TipoPropiedadCMS, bool> listaPropiedadesHTML = new Dictionary<TipoPropiedadCMS, bool>();
            listaPropiedadesHTML.Add(TipoPropiedadCMS.Titulo, false);
            listaPropiedadesHTML.Add(TipoPropiedadCMS.HTML, true);
            PropiedadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.HTML, listaPropiedadesHTML);

            List<TipoCaducidadComponenteCMS> listaCaducidadesHTML = new List<TipoCaducidadComponenteCMS>();
            listaCaducidadesHTML.Add(TipoCaducidadComponenteCMS.NoCache);
            CaducidadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.HTML, listaCaducidadesHTML);

            ListaComponentesPublicos.Add(TipoComponenteCMS.HTML);
            #endregion

            #region Destacado
            Dictionary<TipoPropiedadCMS, bool> listaPropiedadesDestacado = new Dictionary<TipoPropiedadCMS, bool>();
            listaPropiedadesDestacado.Add(TipoPropiedadCMS.Titulo, true);
            listaPropiedadesDestacado.Add(TipoPropiedadCMS.Subtitulo, false);
            listaPropiedadesDestacado.Add(TipoPropiedadCMS.Imagen, false);
            listaPropiedadesDestacado.Add(TipoPropiedadCMS.HTML, false);
            listaPropiedadesDestacado.Add(TipoPropiedadCMS.Enlace, false);
            PropiedadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.Destacado, listaPropiedadesDestacado);

            List<TipoCaducidadComponenteCMS> listaCaducidadesDestacado = new List<TipoCaducidadComponenteCMS>();
            listaCaducidadesDestacado.Add(TipoCaducidadComponenteCMS.NoCache);
            CaducidadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.Destacado, listaCaducidadesDestacado);

            ListaComponentesPublicos.Add(TipoComponenteCMS.Destacado);
            #endregion

            #region ListadoPorParametros
            Dictionary<TipoPropiedadCMS, bool> listaPropiedadesListadoPorParametros = new Dictionary<TipoPropiedadCMS, bool>();
            listaPropiedadesListadoPorParametros.Add(TipoPropiedadCMS.Titulo, false);
            listaPropiedadesListadoPorParametros.Add(TipoPropiedadCMS.TipoPresentacionRecurso, true);
            listaPropiedadesListadoPorParametros.Add(TipoPropiedadCMS.TipoPresentacionListadoRecursos, true);
            PropiedadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.ListadoPorParametros, listaPropiedadesListadoPorParametros);

            List<TipoCaducidadComponenteCMS> listaCaducidadesListadoPorParametros = new List<TipoCaducidadComponenteCMS>();
            listaCaducidadesListadoPorParametros.Add(TipoCaducidadComponenteCMS.NoCache);
            CaducidadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.ListadoPorParametros, listaCaducidadesListadoPorParametros);

            ListaComponentesPublicos.Add(TipoComponenteCMS.ListadoPorParametros);
            #endregion

            #region ListadoEstatico
            Dictionary<TipoPropiedadCMS, bool> listaPropiedadesListadoEstatico = new Dictionary<TipoPropiedadCMS, bool>();
            listaPropiedadesListadoEstatico.Add(TipoPropiedadCMS.Titulo, false);
            listaPropiedadesListadoEstatico.Add(TipoPropiedadCMS.ListaIDs, true);
            //listaPropiedadesListadoEstatico.Add(TipoPropiedadCMS.NumItemsMostrar, false);
            listaPropiedadesListadoEstatico.Add(TipoPropiedadCMS.TipoPresentacionRecurso, true);
            listaPropiedadesListadoEstatico.Add(TipoPropiedadCMS.TipoPresentacionListadoRecursos, true);
            //listaPropiedadesListadoEstatico.Add(TipoPropiedadCMS.URLVerMas, false);
            PropiedadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.ListadoEstatico, listaPropiedadesListadoEstatico);

            List<TipoCaducidadComponenteCMS> listaCaducidadesListadoEstatico = new List<TipoCaducidadComponenteCMS>();
            listaCaducidadesListadoEstatico.Add(TipoCaducidadComponenteCMS.NoCaducidad);
            listaCaducidadesListadoEstatico.Add(TipoCaducidadComponenteCMS.NoCache);
            listaCaducidadesListadoEstatico.Add(TipoCaducidadComponenteCMS.Recurso);
            listaCaducidadesListadoEstatico.Add(TipoCaducidadComponenteCMS.Hora);
            listaCaducidadesListadoEstatico.Add(TipoCaducidadComponenteCMS.Dia);
            listaCaducidadesListadoEstatico.Add(TipoCaducidadComponenteCMS.Semana);
            CaducidadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.ListadoEstatico, listaCaducidadesListadoEstatico);

            ListaComponentesPublicos.Add(TipoComponenteCMS.ListadoEstatico);
            #endregion

            #region ListadoDinamico
            Dictionary<TipoPropiedadCMS, bool> listaPropiedadesListadoDinamico = new Dictionary<TipoPropiedadCMS, bool>();
            listaPropiedadesListadoDinamico.Add(TipoPropiedadCMS.Titulo, false);
            listaPropiedadesListadoDinamico.Add(TipoPropiedadCMS.URLBusqueda, true);
            listaPropiedadesListadoDinamico.Add(TipoPropiedadCMS.NumItems, true);
            listaPropiedadesListadoDinamico.Add(TipoPropiedadCMS.TipoPresentacionRecurso, true);
            listaPropiedadesListadoDinamico.Add(TipoPropiedadCMS.TipoPresentacionListadoRecursos, true);
            listaPropiedadesListadoDinamico.Add(TipoPropiedadCMS.URLVerMas, false);
            PropiedadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.ListadoDinamico, listaPropiedadesListadoDinamico);

            List<TipoCaducidadComponenteCMS> listaCaducidadesListadoDinamico = new List<TipoCaducidadComponenteCMS>();
            listaCaducidadesListadoDinamico.Add(TipoCaducidadComponenteCMS.NoCaducidad);
            listaCaducidadesListadoDinamico.Add(TipoCaducidadComponenteCMS.Recurso);
            listaCaducidadesListadoDinamico.Add(TipoCaducidadComponenteCMS.Hora);
            listaCaducidadesListadoDinamico.Add(TipoCaducidadComponenteCMS.Dia);
            listaCaducidadesListadoDinamico.Add(TipoCaducidadComponenteCMS.Semana);
            listaCaducidadesListadoDinamico.Add(TipoCaducidadComponenteCMS.NoCache);
            CaducidadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.ListadoDinamico, listaCaducidadesListadoDinamico);

            ListaComponentesPublicos.Add(TipoComponenteCMS.ListadoDinamico);
            #endregion

            #region ActividadReciente
            Dictionary<TipoPropiedadCMS, bool> listaPropiedadesActividadReciente = new Dictionary<TipoPropiedadCMS, bool>();
            listaPropiedadesActividadReciente.Add(TipoPropiedadCMS.Titulo, true);
            listaPropiedadesActividadReciente.Add(TipoPropiedadCMS.NumItems, true);
            listaPropiedadesActividadReciente.Add(TipoPropiedadCMS.TipoActividadRecienteCMS, true);
            PropiedadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.ActividadReciente, listaPropiedadesActividadReciente);

            List<TipoCaducidadComponenteCMS> listaCaducidadesActividadReciente = new List<TipoCaducidadComponenteCMS>();
            listaCaducidadesActividadReciente.Add(TipoCaducidadComponenteCMS.NoCache);
            CaducidadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.ActividadReciente, listaCaducidadesActividadReciente);

            ListaComponentesPublicos.Add(TipoComponenteCMS.ActividadReciente);
            #endregion

            #region GrupoComponentes
            Dictionary<TipoPropiedadCMS, bool> listaPropiedadesGrupoComponentes = new Dictionary<TipoPropiedadCMS, bool>();
            listaPropiedadesGrupoComponentes.Add(TipoPropiedadCMS.Titulo, false);
            listaPropiedadesGrupoComponentes.Add(TipoPropiedadCMS.ListaIDs, true);
            listaPropiedadesGrupoComponentes.Add(TipoPropiedadCMS.TipoPresentacionGrupoComponentes, true);
            PropiedadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.GrupoComponentes, listaPropiedadesGrupoComponentes);

            List<TipoCaducidadComponenteCMS> listaCaducidadesGrupoComponentes = new List<TipoCaducidadComponenteCMS>();
            listaCaducidadesGrupoComponentes.Add(TipoCaducidadComponenteCMS.NoCache);
            CaducidadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.GrupoComponentes, listaCaducidadesGrupoComponentes);

            ListaComponentesPublicos.Add(TipoComponenteCMS.GrupoComponentes);
            #endregion

            #region Tesauro
            Dictionary<TipoPropiedadCMS, bool> listaPropiedadesTesauro = new Dictionary<TipoPropiedadCMS, bool>();
            listaPropiedadesTesauro.Add(TipoPropiedadCMS.Titulo, true);
            listaPropiedadesTesauro.Add(TipoPropiedadCMS.ElementoID, true);
            listaPropiedadesTesauro.Add(TipoPropiedadCMS.TieneImagen, true);
            listaPropiedadesTesauro.Add(TipoPropiedadCMS.NumItemsMostrar, false);
            PropiedadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.Tesauro, listaPropiedadesTesauro);

            List<TipoCaducidadComponenteCMS> listaCaducidadesTesauro = new List<TipoCaducidadComponenteCMS>();
            listaCaducidadesTesauro.Add(TipoCaducidadComponenteCMS.NoCaducidad);
            listaCaducidadesTesauro.Add(TipoCaducidadComponenteCMS.Recurso);
            listaCaducidadesTesauro.Add(TipoCaducidadComponenteCMS.Hora);
            listaCaducidadesTesauro.Add(TipoCaducidadComponenteCMS.Dia);
            listaCaducidadesTesauro.Add(TipoCaducidadComponenteCMS.Semana);
            CaducidadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.Tesauro, listaCaducidadesTesauro);

            ListaComponentesPublicos.Add(TipoComponenteCMS.Tesauro);
            #endregion

            #region RecursoDestacado
            //Dictionary<TipoPropiedadCMS, bool> listaPropiedadesRecursoDestacado = new Dictionary<TipoPropiedadCMS, bool>();
            //listaPropiedadesRecursoDestacado.Add(TipoPropiedadCMS.Titulo, false);
            //listaPropiedadesRecursoDestacado.Add(TipoPropiedadCMS.ElementoID, true);
            //listaPropiedadesRecursoDestacado.Add(TipoPropiedadCMS.TipoPresentacionRecurso, true);
            //PropiedadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.RecursoDestacado, listaPropiedadesRecursoDestacado);

            //List<TipoCaducidadComponenteCMS> listaCaducidadesRecursoDestacado = new List<TipoCaducidadComponenteCMS>();
            //listaCaducidadesRecursoDestacado.Add(TipoCaducidadComponenteCMS.NoCaducidad);
            //listaCaducidadesRecursoDestacado.Add(TipoCaducidadComponenteCMS.Recurso);
            //listaCaducidadesRecursoDestacado.Add(TipoCaducidadComponenteCMS.Hora);
            //listaCaducidadesRecursoDestacado.Add(TipoCaducidadComponenteCMS.Dia);
            //listaCaducidadesRecursoDestacado.Add(TipoCaducidadComponenteCMS.Semana);
            //CaducidadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.RecursoDestacado, listaCaducidadesRecursoDestacado);

            //ListaComponentesPublicos.Add(TipoComponenteCMS.RecursoDestacado);
            #endregion

            #region DatosComunidad
            Dictionary<TipoPropiedadCMS, bool> listaPropiedadesDatosComunidad = new Dictionary<TipoPropiedadCMS, bool>();
            listaPropiedadesDatosComunidad.Add(TipoPropiedadCMS.Titulo, true);
            listaPropiedadesDatosComunidad.Add(TipoPropiedadCMS.ContarPersonasNoVisibles, true);
            PropiedadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.DatosComunidad, listaPropiedadesDatosComunidad);

            List<TipoCaducidadComponenteCMS> listaCaducidadesDatosComunidad = new List<TipoCaducidadComponenteCMS>();
            listaCaducidadesDatosComunidad.Add(TipoCaducidadComponenteCMS.NoCaducidad);
            listaCaducidadesDatosComunidad.Add(TipoCaducidadComponenteCMS.Recurso);
            listaCaducidadesDatosComunidad.Add(TipoCaducidadComponenteCMS.Hora);
            listaCaducidadesDatosComunidad.Add(TipoCaducidadComponenteCMS.Dia);
            listaCaducidadesDatosComunidad.Add(TipoCaducidadComponenteCMS.Semana);
            CaducidadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.DatosComunidad, listaCaducidadesDatosComunidad);

            ListaComponentesPublicos.Add(TipoComponenteCMS.DatosComunidad);
            #endregion

            #region UsuariosRecomendados
            Dictionary<TipoPropiedadCMS, bool> listaPropiedadesUsuariosRecomendados = new Dictionary<TipoPropiedadCMS, bool>();
            listaPropiedadesUsuariosRecomendados.Add(TipoPropiedadCMS.Titulo, false);
            listaPropiedadesUsuariosRecomendados.Add(TipoPropiedadCMS.NumItems, true);
            PropiedadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.UsuariosRecomendados, listaPropiedadesUsuariosRecomendados);

            List<TipoCaducidadComponenteCMS> listaCaducidadesUsuariosRecomendados = new List<TipoCaducidadComponenteCMS>();
            listaCaducidadesUsuariosRecomendados.Add(TipoCaducidadComponenteCMS.NoCache);
            CaducidadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.UsuariosRecomendados, listaCaducidadesUsuariosRecomendados);

            ListaComponentesPublicos.Add(TipoComponenteCMS.UsuariosRecomendados);
            #endregion

            #region CajaBuscador
            Dictionary<TipoPropiedadCMS, bool> listaPropiedadesCajaBuscador = new Dictionary<TipoPropiedadCMS, bool>();
            listaPropiedadesCajaBuscador.Add(TipoPropiedadCMS.Titulo, false);
            listaPropiedadesCajaBuscador.Add(TipoPropiedadCMS.URLBusqueda, true);
            listaPropiedadesCajaBuscador.Add(TipoPropiedadCMS.TextoDefecto, false);
            PropiedadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.CajaBuscador, listaPropiedadesCajaBuscador);

            List<TipoCaducidadComponenteCMS> listaCaducidadesCajaBuscador = new List<TipoCaducidadComponenteCMS>();
            listaCaducidadesCajaBuscador.Add(TipoCaducidadComponenteCMS.NoCache);
            CaducidadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.CajaBuscador, listaCaducidadesCajaBuscador);

            ListaComponentesPublicos.Add(TipoComponenteCMS.CajaBuscador);
            #endregion

            #region Faceta
            Dictionary<TipoPropiedadCMS, bool> listaPropiedadesFaceta = new Dictionary<TipoPropiedadCMS, bool>();
            listaPropiedadesFaceta.Add(TipoPropiedadCMS.Titulo, false);
            listaPropiedadesFaceta.Add(TipoPropiedadCMS.Faceta, true);
            listaPropiedadesFaceta.Add(TipoPropiedadCMS.URLBusqueda, false);
            listaPropiedadesFaceta.Add(TipoPropiedadCMS.TipoPresentacionFaceta, true);
            PropiedadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.Faceta, listaPropiedadesFaceta);

            List<TipoCaducidadComponenteCMS> listaCaducidadesFaceta = new List<TipoCaducidadComponenteCMS>();
            listaCaducidadesFaceta.Add(TipoCaducidadComponenteCMS.Recurso);
            listaCaducidadesFaceta.Add(TipoCaducidadComponenteCMS.Persona);
            listaCaducidadesFaceta.Add(TipoCaducidadComponenteCMS.Hora);
            listaCaducidadesFaceta.Add(TipoCaducidadComponenteCMS.Dia);
            listaCaducidadesFaceta.Add(TipoCaducidadComponenteCMS.NoCache);
            CaducidadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.Faceta, listaCaducidadesFaceta);

            ListaComponentesPublicos.Add(TipoComponenteCMS.Faceta);
            #endregion

            #region ListadoUsuarios
            Dictionary<TipoPropiedadCMS, bool> listaPropiedadeslistadoUsuarios = new Dictionary<TipoPropiedadCMS, bool>();
            listaPropiedadeslistadoUsuarios.Add(TipoPropiedadCMS.Titulo, false);
            listaPropiedadeslistadoUsuarios.Add(TipoPropiedadCMS.TipoPresentacionListadoUsuarios, true);
            listaPropiedadeslistadoUsuarios.Add(TipoPropiedadCMS.TipoListadoUsuarios, true);
            listaPropiedadeslistadoUsuarios.Add(TipoPropiedadCMS.NumItems, true);
            PropiedadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.ListadoUsuarios, listaPropiedadeslistadoUsuarios);

            List<TipoCaducidadComponenteCMS> listaCaducidadesListadoUsuarios = new List<TipoCaducidadComponenteCMS>();
            listaCaducidadesListadoUsuarios.Add(TipoCaducidadComponenteCMS.Persona);
            listaCaducidadesListadoUsuarios.Add(TipoCaducidadComponenteCMS.Hora);
            listaCaducidadesListadoUsuarios.Add(TipoCaducidadComponenteCMS.Dia);
            listaCaducidadesListadoUsuarios.Add(TipoCaducidadComponenteCMS.Semana);
            CaducidadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.ListadoUsuarios, listaCaducidadesListadoUsuarios);

            ListaComponentesPublicos.Add(TipoComponenteCMS.ListadoUsuarios);
            #endregion

            #region ListadoProyectos
            Dictionary<TipoPropiedadCMS, bool> listaPropiedadesProyectosRecomendados = new Dictionary<TipoPropiedadCMS, bool>();
            listaPropiedadesProyectosRecomendados.Add(TipoPropiedadCMS.Titulo, false);
            listaPropiedadesProyectosRecomendados.Add(TipoPropiedadCMS.TipoListadoProyectos, true);
            listaPropiedadesProyectosRecomendados.Add(TipoPropiedadCMS.NumItems, false);
            listaPropiedadesProyectosRecomendados.Add(TipoPropiedadCMS.ListaIDs, false);
            PropiedadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.ListadoProyectos, listaPropiedadesProyectosRecomendados);

            List<TipoCaducidadComponenteCMS> listaCaducidadesProyectosRecomendados = new List<TipoCaducidadComponenteCMS>();
            listaCaducidadesProyectosRecomendados.Add(TipoCaducidadComponenteCMS.NoCache);
            CaducidadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.ListadoProyectos, listaCaducidadesProyectosRecomendados);

            ListaComponentesPublicos.Add(TipoComponenteCMS.ListadoProyectos);
            #endregion

            #region ResumenPerfil
            Dictionary<TipoPropiedadCMS, bool> listaPropiedadesResumenPerfil = new Dictionary<TipoPropiedadCMS, bool>();
            listaPropiedadesResumenPerfil.Add(TipoPropiedadCMS.Titulo, true);
            PropiedadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.ResumenPerfil, listaPropiedadesResumenPerfil);

            List<TipoCaducidadComponenteCMS> listaCaducidadesResumenPerfil = new List<TipoCaducidadComponenteCMS>();
            listaCaducidadesResumenPerfil.Add(TipoCaducidadComponenteCMS.NoCache);
            CaducidadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.ResumenPerfil, listaCaducidadesResumenPerfil);

            ListaComponentesPublicos.Add(TipoComponenteCMS.ResumenPerfil);
            #endregion

            #region MasVistosEnXDias
            Dictionary<TipoPropiedadCMS, bool> listaPropiedadesMasVistosEnXDias = new Dictionary<TipoPropiedadCMS, bool>();
            listaPropiedadesMasVistosEnXDias.Add(TipoPropiedadCMS.Titulo, true);
            listaPropiedadesMasVistosEnXDias.Add(TipoPropiedadCMS.NumItems, true);
            listaPropiedadesMasVistosEnXDias.Add(TipoPropiedadCMS.NumDias, true);
            listaPropiedadesMasVistosEnXDias.Add(TipoPropiedadCMS.TipoPresentacionRecurso, true);
            listaPropiedadesMasVistosEnXDias.Add(TipoPropiedadCMS.TipoPresentacionListadoRecursos, true);
            PropiedadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.MasVistosEnXDias, listaPropiedadesMasVistosEnXDias);

            List<TipoCaducidadComponenteCMS> listaCaducidadesMasVistosEnXDias = new List<TipoCaducidadComponenteCMS>();
            listaCaducidadesMasVistosEnXDias.Add(TipoCaducidadComponenteCMS.Dia);
            CaducidadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.MasVistosEnXDias, listaCaducidadesMasVistosEnXDias);

            ListaComponentesPublicos.Add(TipoComponenteCMS.MasVistos);
            #endregion

            #region MasVistos
            Dictionary<TipoPropiedadCMS, bool> listaPropiedadesMasVistos = new Dictionary<TipoPropiedadCMS, bool>();
            listaPropiedadesMasVistos.Add(TipoPropiedadCMS.Titulo, true);
            listaPropiedadesMasVistos.Add(TipoPropiedadCMS.NumItems, true);
            listaPropiedadesMasVistos.Add(TipoPropiedadCMS.TipoPresentacionRecurso, true);
            PropiedadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.MasVistos, listaPropiedadesMasVistos);

            List<TipoCaducidadComponenteCMS> listaCaducidadesMasVistos = new List<TipoCaducidadComponenteCMS>();
            listaCaducidadesMasVistos.Add(TipoCaducidadComponenteCMS.Dia);
            CaducidadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.MasVistos, listaCaducidadesMasVistos);

            ListaComponentesPublicos.Add(TipoComponenteCMS.MasVistos);
            #endregion

            #region EnvioCorreo
            Dictionary<TipoPropiedadCMS, bool> listaPropiedadesEnvioCorreo = new Dictionary<TipoPropiedadCMS, bool>();
            listaPropiedadesEnvioCorreo.Add(TipoPropiedadCMS.Titulo, true);
            listaPropiedadesEnvioCorreo.Add(TipoPropiedadCMS.ListaCamposEnvioCorreo, true);
            listaPropiedadesEnvioCorreo.Add(TipoPropiedadCMS.TextoBoton, true);
            listaPropiedadesEnvioCorreo.Add(TipoPropiedadCMS.DestinatarioCorreo, true);
            listaPropiedadesEnvioCorreo.Add(TipoPropiedadCMS.TextoMensajeOK, true);
            PropiedadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.EnvioCorreo, listaPropiedadesEnvioCorreo);

            List<TipoCaducidadComponenteCMS> listaCaducidadesEnvioCorreo = new List<TipoCaducidadComponenteCMS>();
            listaCaducidadesEnvioCorreo.Add(TipoCaducidadComponenteCMS.NoCache);
            CaducidadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.EnvioCorreo, listaCaducidadesEnvioCorreo);

            ListaComponentesPublicos.Add(TipoComponenteCMS.EnvioCorreo);
            #endregion

            #region PreguntaTIC
            Dictionary<TipoPropiedadCMS, bool> listaPropiedadesPreguntaTIC = new Dictionary<TipoPropiedadCMS, bool>();
            listaPropiedadesPreguntaTIC.Add(TipoPropiedadCMS.Titulo, true);
            listaPropiedadesPreguntaTIC.Add(TipoPropiedadCMS.ElementoID, true);
            PropiedadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.PreguntaTIC, listaPropiedadesPreguntaTIC);

            List<TipoCaducidadComponenteCMS> listaCaducidadesPreguntaTIC = new List<TipoCaducidadComponenteCMS>();
            listaCaducidadesPreguntaTIC.Add(TipoCaducidadComponenteCMS.NoCache);
            CaducidadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.PreguntaTIC, listaCaducidadesPreguntaTIC);

            ListaComponentesPrivados.Add(TipoComponenteCMS.PreguntaTIC);
            #endregion

            #region Menu
            Dictionary<TipoPropiedadCMS, bool> listaPropiedadesMenu = new Dictionary<TipoPropiedadCMS, bool>();
            listaPropiedadesMenu.Add(TipoPropiedadCMS.Titulo, true);
            listaPropiedadesMenu.Add(TipoPropiedadCMS.ListaOpcionesMenu, true);
            PropiedadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.Menu, listaPropiedadesMenu);

            List<TipoCaducidadComponenteCMS> listaCaducidadesMenu = new List<TipoCaducidadComponenteCMS>();
            listaCaducidadesMenu.Add(TipoCaducidadComponenteCMS.NoCache);
            CaducidadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.Menu, listaCaducidadesMenu);

            ListaComponentesPublicos.Add(TipoComponenteCMS.Menu);
            #endregion

            #region Buscador
            Dictionary<TipoPropiedadCMS, bool> listaPropiedadesBuscador = new Dictionary<TipoPropiedadCMS, bool>();
            listaPropiedadesBuscador.Add(TipoPropiedadCMS.Titulo, false);
            listaPropiedadesBuscador.Add(TipoPropiedadCMS.URLBusqueda, true);
            listaPropiedadesBuscador.Add(TipoPropiedadCMS.TituloAtributoDeBusqueda, true);
            listaPropiedadesBuscador.Add(TipoPropiedadCMS.AtributoDeBusqueda, true);
            listaPropiedadesBuscador.Add(TipoPropiedadCMS.NumItems, true);
            listaPropiedadesBuscador.Add(TipoPropiedadCMS.TipoPresentacionRecurso, true);
            PropiedadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.Buscador, listaPropiedadesBuscador);

            List<TipoCaducidadComponenteCMS> listaCaducidadesBuscador = new List<TipoCaducidadComponenteCMS>();
            listaCaducidadesBuscador.Add(TipoCaducidadComponenteCMS.NoCaducidad);
            listaCaducidadesBuscador.Add(TipoCaducidadComponenteCMS.Recurso);
            listaCaducidadesBuscador.Add(TipoCaducidadComponenteCMS.Hora);
            listaCaducidadesBuscador.Add(TipoCaducidadComponenteCMS.Dia);
            listaCaducidadesBuscador.Add(TipoCaducidadComponenteCMS.Semana);
            listaCaducidadesBuscador.Add(TipoCaducidadComponenteCMS.NoCache);
            CaducidadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.Buscador, listaCaducidadesBuscador);

            ListaComponentesPublicos.Add(TipoComponenteCMS.Buscador);
            #endregion

            #region BuscadorSPARQL
            Dictionary<TipoPropiedadCMS, bool> listaPropiedadesBuscadorSPARQL = new Dictionary<TipoPropiedadCMS, bool>();
            listaPropiedadesBuscadorSPARQL.Add(TipoPropiedadCMS.Titulo, true);
            listaPropiedadesBuscadorSPARQL.Add(TipoPropiedadCMS.QuerySPARQL, true);
            listaPropiedadesBuscadorSPARQL.Add(TipoPropiedadCMS.NumItems, true);
            listaPropiedadesBuscadorSPARQL.Add(TipoPropiedadCMS.TipoPresentacionRecurso, true);
            PropiedadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.BuscadorSPARQL, listaPropiedadesBuscadorSPARQL);

            List<TipoCaducidadComponenteCMS> listaCaducidadesBuscadorSPARQL = new List<TipoCaducidadComponenteCMS>();
            listaCaducidadesBuscadorSPARQL.Add(TipoCaducidadComponenteCMS.NoCaducidad);
            listaCaducidadesBuscadorSPARQL.Add(TipoCaducidadComponenteCMS.Recurso);
            listaCaducidadesBuscadorSPARQL.Add(TipoCaducidadComponenteCMS.Hora);
            listaCaducidadesBuscadorSPARQL.Add(TipoCaducidadComponenteCMS.Dia);
            listaCaducidadesBuscadorSPARQL.Add(TipoCaducidadComponenteCMS.Semana);
            listaCaducidadesBuscadorSPARQL.Add(TipoCaducidadComponenteCMS.NoCache);
            CaducidadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.BuscadorSPARQL, listaCaducidadesBuscadorSPARQL);

            ListaComponentesPrivados.Add(TipoComponenteCMS.BuscadorSPARQL);
            #endregion   

            #region UltimosRecursosVisitados
            Dictionary<TipoPropiedadCMS, bool> listaPropiedadesUltimosRecursosVisitados = new Dictionary<TipoPropiedadCMS, bool>();
            listaPropiedadesUltimosRecursosVisitados.Add(TipoPropiedadCMS.Titulo, true);
            listaPropiedadesUltimosRecursosVisitados.Add(TipoPropiedadCMS.NumItems, true);
            listaPropiedadesUltimosRecursosVisitados.Add(TipoPropiedadCMS.TipoPresentacionRecurso, true);
            listaPropiedadesUltimosRecursosVisitados.Add(TipoPropiedadCMS.TipoPresentacionListadoRecursos, true);
            PropiedadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.UltimosRecursosVisitados, listaPropiedadesUltimosRecursosVisitados);

            List<TipoCaducidadComponenteCMS> listaCaducidadesUltimosRecursosVisitados = new List<TipoCaducidadComponenteCMS>();
            listaCaducidadesUltimosRecursosVisitados.Add(TipoCaducidadComponenteCMS.NoCache);
            CaducidadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.UltimosRecursosVisitados, listaCaducidadesUltimosRecursosVisitados);

            ListaComponentesPublicos.Add(TipoComponenteCMS.UltimosRecursosVisitados);
            #endregion

            #region FichaDescripcionDocumento
            Dictionary<TipoPropiedadCMS, bool> listaPropiedadesFichaDescripcionDocumento = new Dictionary<TipoPropiedadCMS, bool>();
            listaPropiedadesFichaDescripcionDocumento.Add(TipoPropiedadCMS.Titulo, false);
            listaPropiedadesFichaDescripcionDocumento.Add(TipoPropiedadCMS.ElementoID, true);
            PropiedadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.FichaDescripcionDocumento, listaPropiedadesFichaDescripcionDocumento);

            List<TipoCaducidadComponenteCMS> listaCaducidadesFichaDescripcionDocumento = new List<TipoCaducidadComponenteCMS>();
            listaCaducidadesFichaDescripcionDocumento.Add(TipoCaducidadComponenteCMS.NoCache);
            CaducidadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.FichaDescripcionDocumento, listaCaducidadesFichaDescripcionDocumento);

            ListaComponentesPrivados.Add(TipoComponenteCMS.FichaDescripcionDocumento);
            #endregion

            #region ConsultaSPARQL
            Dictionary<TipoPropiedadCMS, bool> listaPropiedadesConsultaSPARQL = new Dictionary<TipoPropiedadCMS, bool>();
            listaPropiedadesConsultaSPARQL.Add(TipoPropiedadCMS.Titulo, false);
            listaPropiedadesConsultaSPARQL.Add(TipoPropiedadCMS.QuerySPARQL, true);
            PropiedadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.ConsultaSPARQL, listaPropiedadesConsultaSPARQL);

            List<TipoCaducidadComponenteCMS> listaCaducidadesConsultaSPARQL = new List<TipoCaducidadComponenteCMS>();
            listaCaducidadesConsultaSPARQL.Add(TipoCaducidadComponenteCMS.NoCaducidad);
            listaCaducidadesConsultaSPARQL.Add(TipoCaducidadComponenteCMS.Recurso);
            listaCaducidadesConsultaSPARQL.Add(TipoCaducidadComponenteCMS.Hora);
            listaCaducidadesConsultaSPARQL.Add(TipoCaducidadComponenteCMS.Dia);
            listaCaducidadesConsultaSPARQL.Add(TipoCaducidadComponenteCMS.Semana);
            listaCaducidadesConsultaSPARQL.Add(TipoCaducidadComponenteCMS.NoCache);
            CaducidadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.ConsultaSPARQL, listaCaducidadesConsultaSPARQL);

            ListaComponentesPrivados.Add(TipoComponenteCMS.ConsultaSPARQL);
            #endregion            

            #region ConsultaSQLSERVER
            Dictionary<TipoPropiedadCMS, bool> listaPropiedadesConsultaSQLSERVER = new Dictionary<TipoPropiedadCMS, bool>();
            listaPropiedadesConsultaSQLSERVER.Add(TipoPropiedadCMS.Titulo, false);
            listaPropiedadesConsultaSQLSERVER.Add(TipoPropiedadCMS.QuerySQLSERVER, true);
            PropiedadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.ConsultaSQLSERVER, listaPropiedadesConsultaSQLSERVER);

            List<TipoCaducidadComponenteCMS> listaCaducidadesConsultaSQLSERVER = new List<TipoCaducidadComponenteCMS>();
            listaCaducidadesConsultaSQLSERVER.Add(TipoCaducidadComponenteCMS.NoCaducidad);
            listaCaducidadesConsultaSQLSERVER.Add(TipoCaducidadComponenteCMS.Recurso);
            listaCaducidadesConsultaSQLSERVER.Add(TipoCaducidadComponenteCMS.Hora);
            listaCaducidadesConsultaSQLSERVER.Add(TipoCaducidadComponenteCMS.Dia);
            listaCaducidadesConsultaSQLSERVER.Add(TipoCaducidadComponenteCMS.Semana);
            listaCaducidadesConsultaSQLSERVER.Add(TipoCaducidadComponenteCMS.NoCache);
            CaducidadesDisponiblesPorTipoComponente.Add(TipoComponenteCMS.ConsultaSQLSERVER, listaCaducidadesConsultaSQLSERVER);

            ListaComponentesPrivados.Add(TipoComponenteCMS.ConsultaSQLSERVER);
            #endregion

            #endregion


            //AJAX
            ListaComponentesAjax = new List<TipoComponenteCMS>();
            ListaComponentesAjax.Add(TipoComponenteCMS.UsuariosRecomendados);

            //Propiedades multiidioma
            ListaPropiedadesMultiIdioma = new List<TipoPropiedadCMS>();
            ListaPropiedadesMultiIdioma.Add(TipoPropiedadCMS.Titulo);
            ListaPropiedadesMultiIdioma.Add(TipoPropiedadCMS.TextoBoton);
            ListaPropiedadesMultiIdioma.Add(TipoPropiedadCMS.ListaCamposEnvioCorreo);
            ListaPropiedadesMultiIdioma.Add(TipoPropiedadCMS.ListaOpcionesMenu);
            ListaPropiedadesMultiIdioma.Add(TipoPropiedadCMS.TextoMensajeOK);
            ListaPropiedadesMultiIdioma.Add(TipoPropiedadCMS.TituloAtributoDeBusqueda);
            ListaPropiedadesMultiIdioma.Add(TipoPropiedadCMS.TextoDefecto);

            ListaPropiedadesMultiIdioma.Add(TipoPropiedadCMS.Subtitulo);
            ListaPropiedadesMultiIdioma.Add(TipoPropiedadCMS.Imagen);
            ListaPropiedadesMultiIdioma.Add(TipoPropiedadCMS.HTML);
            ListaPropiedadesMultiIdioma.Add(TipoPropiedadCMS.Enlace);
        }

        #endregion
    }

    /// <summary>
    /// CMSComponente
    /// </summary>
    public abstract class CMSComponente : ElementoGnoss
    {
        #region Miembros

        private List<AD.EntityModel.Models.CMS.CMSPropiedadComponente> mFilasPropiedadesComponente = null;

        private Dictionary<TipoPropiedadCMS, string> mPropiedadesComponente = null;

        /// <summary>
        /// Lista de Grupos
        /// </summary>
        private Dictionary<Guid, CMSComponenteRolGrupoIdentidades> mListaRolGrupoIdentidades;

        /// <summary>
        /// Lista de Perfiles
        /// </summary>
        private Dictionary<Guid, CMSComponenteRolIdentidad> mListaRolIdentidad;

        private LoggingService mLoggingService;

        private EntityContext mEntityContext;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor a partir de una fila de componente de CMS y del gestor de CMS pasados por parámetro
        /// </summary>
        /// <param name="pFilaComponente">Fila de componente</param>
        /// <param name="pGestorCMS">Gestor de CMS</param>
        protected CMSComponente(AD.EntityModel.Models.CMS.CMSComponente pFilaComponente, List<AD.EntityModel.Models.CMS.CMSPropiedadComponente> pListaPropiedades, GestionCMS pGestorCMS, LoggingService loggingService, EntityContext entityContext)
            : base(pFilaComponente, pGestorCMS, loggingService)
        {
            mFilasPropiedadesComponente = pListaPropiedades;
            mLoggingService = loggingService;
            mEntityContext = entityContext;
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Obtiene el valor de una propiedad de un componente (si no existe devuelve null)
        /// </summary>
        /// <param name="pTipoPropiedad"></param>
        /// <returns></returns>
        protected string ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS pTipoPropiedad)
        {
            List<AD.EntityModel.Models.CMS.CMSPropiedadComponente> filas = GestorCMS.CMSDW.ListaCMSPropiedadComponente.Where(item => item.ComponenteID.Equals(this.Clave) && item.TipoPropiedadComponente.Equals((short)pTipoPropiedad)).ToList();//"ComponenteID='" + this.Clave + "' AND TipoPropiedadComponente='" + (short)pTipoPropiedad + "'");
            if (filas.Count > 1)
            {
                throw new Exception("Un componente no puede tener la misma propiedad repetida");
            }
            else if (filas.Count == 0)
            {
                return null;
            }
            else
            {
                return (filas.FirstOrDefault()).ValorPropiedad;
            }
        }

        /// <summary>
        /// Asigna el valor de una propiedad de un componente
        /// </summary>
        /// <param name="pTipoPropiedad"></param>
        /// <returns></returns>
        protected void AsignarValorDePropiedadDeComponente(TipoPropiedadCMS pTipoPropiedad, string pValor)
        {
            List<AD.EntityModel.Models.CMS.CMSPropiedadComponente> filas = GestorCMS.CMSDW.ListaCMSPropiedadComponente.Where(item => item.ComponenteID.Equals(this.Clave) && item.TipoPropiedadComponente.Equals((short)pTipoPropiedad)).ToList();
            if (filas.Count > 1)
            {
                throw new Exception("Un componente no puede tener la misma propiedad repetida");
            }
            else if (filas.Count == 0)
            {
                if (!string.IsNullOrEmpty(pValor))
                {
                    AD.EntityModel.Models.CMS.CMSPropiedadComponente propiedadComponente = new AD.EntityModel.Models.CMS.CMSPropiedadComponente();
                    propiedadComponente.CMSComponente = this.FilaComponente;
                    propiedadComponente.TipoPropiedadComponente = (short)pTipoPropiedad;
                    propiedadComponente.ValorPropiedad = pValor;
                    propiedadComponente.ComponenteID = this.FilaComponente.ComponenteID;
                    GestorCMS.CMSDW.ListaCMSPropiedadComponente.Add(propiedadComponente);

                    mEntityContext.CMSPropiedadComponente.Add(propiedadComponente);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(pValor))
                {
                    filas.FirstOrDefault().ValorPropiedad = pValor;
                }
                else
                {
                    GestorCMS.CMSDW.ListaCMSPropiedadComponente.Remove(filas.FirstOrDefault());
                    mEntityContext.EliminarElemento(filas.FirstOrDefault());
                }
            }
        }

        /// <summary>
        /// Carga los grupos lecores
        /// </summary>
        public void CargarRolGrupoIdentidades()
        {
            mListaRolGrupoIdentidades = new Dictionary<Guid, CMSComponenteRolGrupoIdentidades>();

            foreach (AD.EntityModel.Models.CMS.CMSComponenteRolGrupoIdentidades filaRolGrupoIdentidades in GestorCMS.CMSDW.ListaCMSComponenteRolGrupoIdentidades)
            {
                if (filaRolGrupoIdentidades.ComponenteID == Clave)
                {
                    if (!mListaRolGrupoIdentidades.ContainsKey(filaRolGrupoIdentidades.GrupoID))
                    {
                        mListaRolGrupoIdentidades.Add(filaRolGrupoIdentidades.GrupoID, new CMSComponenteRolGrupoIdentidades(filaRolGrupoIdentidades, GestorCMS, mLoggingService));
                    }
                }
            }
        }

        /// <summary>
        /// Carga los perfiles lecores
        /// </summary>
        public void CargarRolIdentidad()
        {
            mListaRolIdentidad = new Dictionary<Guid, CMSComponenteRolIdentidad>();

            foreach (AD.EntityModel.Models.CMS.CMSComponenteRolIdentidad filaRolIdentidad in GestorCMS.CMSDW.ListaCMSComponenteRolIdentidad)
            {
                if (filaRolIdentidad.ComponenteID == Clave)
                {
                    if (!mListaRolIdentidad.ContainsKey(filaRolIdentidad.PerfilID))
                    {
                        mListaRolIdentidad.Add(filaRolIdentidad.PerfilID, new CMSComponenteRolIdentidad(filaRolIdentidad, GestorCMS, mLoggingService));
                    }
                }
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene el tipo de componente
        /// </summary>
        public TipoComponenteCMS TipoComponenteCMS
        {
            get
            {
                return (TipoComponenteCMS)FilaComponente.TipoComponente;
            }
        }

        /// <summary>
        /// Obtiene el tipo de caducidad
        /// </summary>
        public TipoCaducidadComponenteCMS TipoCaducidadComponenteCMS
        {
            get
            {
                return (TipoCaducidadComponenteCMS)FilaComponente.TipoCaducidadComponente;
            }
            set
            {
                FilaComponente.TipoCaducidadComponente = (short)value;
            }
        }

        /// <summary>
        /// Obtiene o establece el nombre del componente
        /// </summary>
        public override string Nombre
        {
            get
            {
                return FilaComponente.Nombre;
            }
            set
            {
                FilaComponente.Nombre = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el nombre corto del componente
        /// </summary>
        public string NombreCortoComponente
        {
            get
            {
                return FilaComponente.NombreCortoComponente;
            }
            set
            {
                FilaComponente.NombreCortoComponente = value;
            }
        }

        /// <summary>
        /// Obtiene o establece los estilos del componente
        /// </summary>
        public string Estilos
        {
            get
            {
                return FilaComponente.Estilos;
            }
            set
            {
                FilaComponente.Estilos = value;
            }
        }

        /// <summary>
        /// Obtiene o establece si el componente esta activo
        /// </summary>
        public bool Activo
        {
            get
            {
                return FilaComponente.Activo;
            }
            set
            {
                FilaComponente.Activo = value;
            }
        }

        /// <summary>
        /// Obtiene o establece si el componente es de acceso público en comunidades privadas o restringidas
        /// </summary>
        public bool AccesoPublico
        {
            get
            {
                return FilaComponente.AccesoPublico;
            }
            set
            {
                FilaComponente.AccesoPublico = value;
            }
        }

        /// <summary>
        /// Obtiene el id del componente
        /// </summary>
        public override Guid Clave
        {
            get
            {
                return FilaComponente.ComponenteID;
            }
        }

        /// <summary>
        /// Obtiene el id del proyecto al que pertenece
        /// </summary>
        public Guid ProyectoID
        {
            get
            {
                return FilaComponente.ProyectoID;
            }
        }

        /// <summary>
        /// Devuelve el gestor de CMS que contiene al componente
        /// </summary>
        public GestionCMS GestorCMS
        {
            get
            {
                return (GestionCMS)this.GestorGnoss;
            }
        }

        /// <summary>
        /// Obtiene la fila del componente
        /// </summary>
        public AD.EntityModel.Models.CMS.CMSComponente FilaComponente
        {
            get
            {
                return (AD.EntityModel.Models.CMS.CMSComponente)FilaElementoEntity;
            }
        }


        /// <summary>
        /// Obtiene o establece una personalización
        /// </summary>
        public Guid? Personalizacion
        {
            get
            {
                string pers = ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.Personalizacion);
                Guid? persGuid = null;
                if (!string.IsNullOrEmpty(pers))
                {
                    persGuid = Guid.Parse(pers);
                }

                return persGuid;
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.Personalizacion, value.ToString());
            }
        }

        /// <summary>
        /// Obtiene las propiedades del componente
        /// </summary>
        public Dictionary<TipoPropiedadCMS, string> PropiedadesComponente
        {
            get
            {
                if (mPropiedadesComponente == null)
                {
                    mPropiedadesComponente = new Dictionary<TipoPropiedadCMS, string>();
                    foreach (AD.EntityModel.Models.CMS.CMSPropiedadComponente fila in GestorCMS.CMSDW.ListaCMSPropiedadComponente.Where(item => item.ComponenteID.Equals(this.Clave)).ToList())
                    {
                        AD.EntityModel.Models.CMS.CMSPropiedadComponente filaPropiedad = fila;
                        mPropiedadesComponente.Add((TipoPropiedadCMS)filaPropiedad.TipoPropiedadComponente, filaPropiedad.ValorPropiedad);
                    }
                }
                return mPropiedadesComponente;
            }
        }

        /// <summary>
        /// Obtiene los idiomas disponibles
        /// </summary>
        public List<string> ListaIdiomasDisponibles(List<string> pListaIdiomas)
        {
            List<string> listaIdiomasDisponibles = new List<string>();
            foreach (string idioma in pListaIdiomas)
            {
                if (string.IsNullOrEmpty(FilaComponente.IdiomasDisponibles) || UtilCadenas.ObtenerTextoDeIdioma(FilaComponente.IdiomasDisponibles, idioma, null, true) == "true")
                {
                    listaIdiomasDisponibles.Add(idioma);
                }
            }
            return listaIdiomasDisponibles;
        }

        /// <summary>
        /// Obtiene la lista de grupos lectores
        /// </summary>
        public Dictionary<Guid, CMSComponenteRolGrupoIdentidades> ListaRolGrupoIdentidades
        {
            get
            {
                if (mListaRolGrupoIdentidades == null)
                {
                    CargarRolGrupoIdentidades();
                }
                return mListaRolGrupoIdentidades;
            }
        }

        /// <summary>
        /// Obtiene la lista de perfiles lectores
        /// </summary>
        public Dictionary<Guid, CMSComponenteRolIdentidad> ListaRolIdentidad
        {
            get
            {
                if (mListaRolIdentidad == null)
                {
                    CargarRolIdentidad();
                }
                return mListaRolIdentidad;
            }
        }

        /// <summary>
        /// Obtiene si el componente es privado
        /// </summary>
        public bool Privado
        {
            get
            {
                return ListaRolGrupoIdentidades.Count > 0 || ListaRolIdentidad.Count > 0;
            }
        }

        #endregion

        #region Agregar Editores/grupos

        /// <summary>
        /// Agrega un nuevo grupo de editores
        /// </summary>
        ///<param name="pGrupoID"></param>
        /// <returns></returns>
        public CMSComponenteRolGrupoIdentidades AgregarGrupoEditorAPagina(Guid pGrupoID)
        {
            AD.EntityModel.Models.CMS.CMSComponenteRolGrupoIdentidades filaGrupoIdentidades = new AD.EntityModel.Models.CMS.CMSComponenteRolGrupoIdentidades();
            filaGrupoIdentidades.ComponenteID = Clave;
            filaGrupoIdentidades.GrupoID = pGrupoID;

            GestorCMS.CMSDW.ListaCMSComponenteRolGrupoIdentidades.Add(filaGrupoIdentidades);
            mEntityContext.CMSComponenteRolGrupoIdentidades.Add(filaGrupoIdentidades);

            CMSComponenteRolGrupoIdentidades RolGrupoIdentidades = new CMSComponenteRolGrupoIdentidades(filaGrupoIdentidades, GestorCMS, mLoggingService);

            ListaRolGrupoIdentidades.Add(pGrupoID, RolGrupoIdentidades);

            return RolGrupoIdentidades;
        }

        /// <summary>
        /// Agrega un nuevo editor
        /// </summary>
        ///<param name="pGrupoID"></param>
        /// <returns></returns>
        public CMSComponenteRolIdentidad AgregarEditorAPagina(Guid pPerfilID)
        {
            AD.EntityModel.Models.CMS.CMSComponenteRolIdentidad filaIdentidad = new AD.EntityModel.Models.CMS.CMSComponenteRolIdentidad();
            filaIdentidad.ComponenteID = Clave;
            filaIdentidad.PerfilID = pPerfilID;

            GestorCMS.CMSDW.ListaCMSComponenteRolIdentidad.Add(filaIdentidad);
            mEntityContext.CMSComponenteRolIdentidad.Add(filaIdentidad);

            CMSComponenteRolIdentidad RolIdentidad = new CMSComponenteRolIdentidad(filaIdentidad, GestorCMS, mLoggingService);

            ListaRolIdentidad.Add(pPerfilID, RolIdentidad);
            return RolIdentidad;
        }

        public void EliminarLectores()
        {
            foreach (CMSComponenteRolIdentidad fila in ListaRolIdentidad.Values)
            {
                mEntityContext.EliminarElemento(fila.FilaElementoEntity);
                GestorCMS.CMSDW.ListaCMSComponenteRolIdentidad.Remove((AD.EntityModel.Models.CMS.CMSComponenteRolIdentidad)fila.FilaElementoEntity);
            }
            foreach (CMSComponenteRolGrupoIdentidades fila in ListaRolGrupoIdentidades.Values)
            {
                //EntityContext.Entry(fila.FilaElementoEntity).State = EntityState.Deleted;
                mEntityContext.EliminarElemento(fila.FilaElementoEntity);
                GestorCMS.CMSDW.ListaCMSComponenteRolGrupoIdentidades.Remove((AD.EntityModel.Models.CMS.CMSComponenteRolGrupoIdentidades)fila.FilaElementoEntity);
                //fila.FilaElemento.Delete();
            }
            CargarRolIdentidad();
            CargarRolGrupoIdentidades();
        }

        #endregion
    }

    /// <summary>
    /// CMSComponenteHTML
    /// </summary>
    public class CMSComponenteHTML : CMSComponente
    {
        #region Constructor

        /// <summary>
        /// Constructor a partir de una fila de componente de CMS y del gestor de CMS pasados por parámetro
        /// </summary>
        /// <param name="pFilaComponente">Fila de componente</param>
        /// <param name="pGestorCMS">Gestor de CMS</param>
        public CMSComponenteHTML(AD.EntityModel.Models.CMS.CMSComponente pFilaComponente, List<AD.EntityModel.Models.CMS.CMSPropiedadComponente> pListaPropiedades, GestionCMS pGestorCMS, LoggingService loggingService, EntityContext entityContext)
            : base(pFilaComponente, pListaPropiedades, pGestorCMS, loggingService, entityContext)
        {
            if (pFilaComponente.TipoComponente != (short)TipoComponenteCMS.HTML)
            {
                throw new Exception("El componente debe ser de tipo HTMLLibre");
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece el Titulo
        /// </summary>
        public string Titulo
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo, value);
            }
        }

        /// <summary>
        /// Obtiene o establece el HTML
        /// </summary>
        public string HTML
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.HTML);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.HTML, value);
            }
        }
        #endregion
    }

    /// <summary>
    /// CMSComponenteDestacado
    /// </summary>
    public class CMSComponenteDestacado : CMSComponente
    {
        #region Constructor

        /// <summary>
        /// Constructor a partir de una fila de componente de CMS y del gestor de CMS pasados por parámetro
        /// </summary>
        /// <param name="pFilaComponente">Fila de componente</param>
        /// <param name="pGestorCMS">Gestor de CMS</param>
        public CMSComponenteDestacado(AD.EntityModel.Models.CMS.CMSComponente pFilaComponente, List<AD.EntityModel.Models.CMS.CMSPropiedadComponente> pListaPropiedades, GestionCMS pGestorCMS, LoggingService loggingService, EntityContext entityContext)
            : base(pFilaComponente, pListaPropiedades, pGestorCMS, loggingService, entityContext)
        {
            if (pFilaComponente.TipoComponente != (short)TipoComponenteCMS.Destacado)
            {
                throw new Exception("El componente debe ser de tipo Destacado");
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece el Titulo
        /// </summary>
        public string Titulo
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo, value);
            }
        }

        /// <summary>
        /// Obtiene o establece la Imagen
        /// </summary>
        public string Imagen
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.Imagen);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.Imagen, value);
            }
        }

        /// <summary>
        /// Obtiene o establece el Subtítulo
        /// </summary>
        public string Subtitulo
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.Subtitulo);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.Subtitulo, value);
            }
        }

        /// <summary>
        /// Obtiene o establece el HTML
        /// </summary>
        public string HTML
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.HTML);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.HTML, value);
            }
        }

        /// <summary>
        /// Obtiene o establece el Enlace
        /// </summary>
        public string Enlace
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.Enlace);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.Enlace, value);
            }
        }
        #endregion
    }

    /// <summary>
    /// CMSComponenteListadoPorParametros
    /// </summary>
    public class CMSComponenteListadoPorParametros : CMSComponente
    {
        #region Constructor

        /// <summary>
        /// Constructor a partir de una fila de componente de CMS y del gestor de CMS pasados por parámetro
        /// </summary>
        /// <param name="pFilaComponente">Fila de componente</param>
        /// <param name="pGestorCMS">Gestor de CMS</param>
        public CMSComponenteListadoPorParametros(AD.EntityModel.Models.CMS.CMSComponente pFilaComponente, List<AD.EntityModel.Models.CMS.CMSPropiedadComponente> pListaPropiedades, GestionCMS pGestorCMS, LoggingService loggingService, EntityContext entityContext)
            : base(pFilaComponente, pListaPropiedades, pGestorCMS, loggingService, entityContext)
        {
            if (pFilaComponente.TipoComponente != (short)TipoComponenteCMS.ListadoPorParametros /*&& pFilaComponente.TipoComponente != (short)TipoComponenteCMS.RecursosDestacadosEstatico*/)
            {
                throw new Exception("El componente debe ser de tipo ListadoEstatico");
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece el Titulo
        /// </summary>
        public string Titulo
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo, value);
            }
        }

        /// <summary>
        /// Obtiene o establece el tipo de presentacion de cada recurso
        /// </summary>
        public string TipoPresentacionRecurso
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.TipoPresentacionRecurso);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.TipoPresentacionRecurso, value);
            }
        }

        /// <summary>
        /// Obtiene o establece el tipo de presentacion del listado de recursos
        /// </summary>
        public string TipoPresentacionListadoRecursos
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.TipoPresentacionListadoRecursos);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.TipoPresentacionListadoRecursos, value);
            }
        }
        #endregion
    }

    /// <summary>
    /// CMSComponenteListadoEstatico
    /// </summary>
    public class CMSComponenteListadoEstatico : CMSComponente
    {
        #region Constructor

        /// <summary>
        /// Constructor a partir de una fila de componente de CMS y del gestor de CMS pasados por parámetro
        /// </summary>
        /// <param name="pFilaComponente">Fila de componente</param>
        /// <param name="pGestorCMS">Gestor de CMS</param>
        public CMSComponenteListadoEstatico(AD.EntityModel.Models.CMS.CMSComponente pFilaComponente, List<AD.EntityModel.Models.CMS.CMSPropiedadComponente> pListaPropiedades, GestionCMS pGestorCMS, LoggingService loggingService, EntityContext entityContext)
            : base(pFilaComponente, pListaPropiedades, pGestorCMS, loggingService, entityContext)
        {
            if (pFilaComponente.TipoComponente != (short)TipoComponenteCMS.ListadoEstatico /*&& pFilaComponente.TipoComponente != (short)TipoComponenteCMS.RecursosDestacadosEstatico*/)
            {
                throw new Exception("El componente debe ser de tipo ListadoEstatico");
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece el Titulo
        /// </summary>
        public string Titulo
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo, value);
            }
        }

        /// <summary>
        /// Obtiene o establece la lista de IDs
        /// </summary>
        public List<Guid> ListaGuids
        {
            get
            {
                string textoIDs = ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.ListaIDs);
                string[] listaIDs = textoIDs.Split(',');
                List<Guid> listaGUIDs = new List<Guid>();
                foreach (string id in listaIDs)
                {
                    Guid idGuid;
                    if (id != "" && Guid.TryParse(id, out idGuid))
                    {
                        listaGUIDs.Add(idGuid);
                    }
                }
                return listaGUIDs;
            }
            set
            {
                string textoIDs = "";
                foreach (Guid id in value)
                {
                    textoIDs += id.ToString() + ",";
                }
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.ListaIDs, textoIDs);
            }
        }

        ///// <summary>
        ///// Obtiene o establece la URL de ver mas
        ///// </summary>
        //public string URLVerMas
        //{
        //    get
        //    {
        //        return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.URLVerMas);
        //    }
        //    set
        //    {
        //        AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.URLVerMas, value);
        //    }
        //}

        ///// <summary>
        ///// Obtiene o establece el HTML
        ///// </summary>
        //public short NumeroItemsMostrar
        //{
        //    get
        //    {
        //        return short.Parse(ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.NumItemsMostrar));
        //    }
        //    set
        //    {
        //        AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.NumItemsMostrar, value.ToString());
        //    }
        //}

        /// <summary>
        /// Obtiene o establece el tipo de presentacion de cada recurso
        /// </summary>
        public string TipoPresentacionRecurso
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.TipoPresentacionRecurso);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.TipoPresentacionRecurso, value);
            }
        }

        /// <summary>
        /// Obtiene o establece el tipo de presentacion del listado de recursos
        /// </summary>
        public string TipoPresentacionListadoRecursos
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.TipoPresentacionListadoRecursos);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.TipoPresentacionListadoRecursos, value);
            }
        }
        #endregion
    }

    /// <summary>
    /// CMSComponenteListadoDinamico
    /// </summary>
    public class CMSComponenteListadoDinamico : CMSComponente
    {
        #region Constructor

        /// <summary>
        /// Constructor a partir de una fila de componente de CMS y del gestor de CMS pasados por parámetro
        /// </summary>
        /// <param name="pFilaComponente">Fila de componente</param>
        /// <param name="pGestorCMS">Gestor de CMS</param>
        public CMSComponenteListadoDinamico(AD.EntityModel.Models.CMS.CMSComponente pFilaComponente, List<AD.EntityModel.Models.CMS.CMSPropiedadComponente> pListaPropiedades, GestionCMS pGestorCMS, LoggingService loggingService, EntityContext entityContext)
            : base(pFilaComponente, pListaPropiedades, pGestorCMS, loggingService, entityContext)
        {
            if (pFilaComponente.TipoComponente != (short)TipoComponenteCMS.ListadoDinamico /*&& pFilaComponente.TipoComponente != (short)TipoComponenteCMS.RecursosDestacados*/)
            {
                throw new Exception("El componente debe ser de tipo ListadoDinamico");
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece el Titulo
        /// </summary>
        public string Titulo
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo, value);
            }
        }

        /// <summary>
        /// Obtiene o establece la URL de busqueda
        /// </summary>
        public string URLBusqueda
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.URLBusqueda);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.URLBusqueda, value);
            }
        }

        /// <summary>
        /// Obtiene o establece la URL de ver mas
        /// </summary>
        public string URLVerMas
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.URLVerMas);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.URLVerMas, value);
            }
        }

        /// <summary>
        /// Obtiene o establece numero de items que pinta
        /// </summary>
        public short NumeroItems
        {
            get
            {
                return short.Parse(ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.NumItems));
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.NumItems, value.ToString());
            }
        }

        /// <summary>
        /// Obtiene o establece numero de items que pinta (los demas estan ocultos para paginar)
        /// </summary>
        public short NumeroItemsMostrar
        {
            get
            {
                return short.Parse(ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.NumItemsMostrar));
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.NumItemsMostrar, value.ToString());
            }
        }

        /// <summary>
        /// Obtiene o establece el tipo de presentacion de cada recurso
        /// </summary>
        public string TipoPresentacionRecurso
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.TipoPresentacionRecurso);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.TipoPresentacionRecurso, value);
            }
        }

        /// <summary>
        /// Obtiene o establece el tipo de presentacion del listado de recursos
        /// </summary>
        public string TipoPresentacionListadoRecursos
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.TipoPresentacionListadoRecursos);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.TipoPresentacionListadoRecursos, value);
            }
        }
        #endregion
    }

    /// <summary>
    /// CMSComponenteActividadReciente
    /// </summary>
    public class CMSComponenteActividadReciente : CMSComponente
    {
        #region Constructor

        /// <summary>
        /// Constructor a partir de una fila de componente de CMS y del gestor de CMS pasados por parámetro
        /// </summary>
        /// <param name="pFilaComponente">Fila de componente</param>
        /// <param name="pGestorCMS">Gestor de CMS</param>
        public CMSComponenteActividadReciente(AD.EntityModel.Models.CMS.CMSComponente pFilaComponente, List<AD.EntityModel.Models.CMS.CMSPropiedadComponente> pListaPropiedades, GestionCMS pGestorCMS, LoggingService loggingService, EntityContext entityContext)
            : base(pFilaComponente, pListaPropiedades, pGestorCMS, loggingService, entityContext)
        {
            if (pFilaComponente.TipoComponente != (short)TipoComponenteCMS.ActividadReciente)
            {
                throw new Exception("El componente debe ser de tipo Actividad reciente");
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece el Titulo
        /// </summary>
        public string Titulo
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo, value);
            }
        }

        /// <summary>
        /// Obtiene o establece el numero de items que se muestran
        /// </summary>
        public short NumeroItems
        {
            get
            {
                return short.Parse(ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.NumItems));
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.NumItems, value.ToString());
            }
        }


        /// <summary>
        /// Obtiene o establece el numero de items que se muestran
        /// </summary>
        public TipoActividadReciente TipoActividadReciente
        {
            get
            {
                return (TipoActividadReciente)short.Parse(ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.TipoActividadRecienteCMS));
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.TipoActividadRecienteCMS, ((short)value).ToString());
            }
        }

        #endregion
    }

    /// <summary>
    /// CMSComponenteGrupodeComponentes
    /// </summary>
    public class CMSComponenteGrupoComponentes : CMSComponente
    {
        #region Miembros

        /// <summary>
        /// Obtiene ls lita de componentes
        /// </summary>
        public List<CMSComponente> mComponentes = null;


        #endregion

        #region Constructor

        /// <summary>
        /// Constructor a partir de una fila de componente de CMS y del gestor de CMS pasados por parámetro
        /// </summary>
        /// <param name="pFilaComponente">Fila de componente</param>
        /// <param name="pGestorCMS">Gestor de CMS</param>
        public CMSComponenteGrupoComponentes(AD.EntityModel.Models.CMS.CMSComponente pFilaComponente, List<AD.EntityModel.Models.CMS.CMSPropiedadComponente> pListaPropiedades, GestionCMS pGestorCMS, LoggingService loggingService, EntityContext entityContext)
            : base(pFilaComponente, pListaPropiedades, pGestorCMS, loggingService, entityContext)
        {
            if (pFilaComponente.TipoComponente != (short)TipoComponenteCMS.GrupoComponentes)
            {
                throw new Exception("El componente debe ser de tipo GrupoComponentes");
            }
        }

        #endregion

        /// <summary>
        /// Carga los Componentes
        /// </summary>
        public void CargarComponentes()
        {
            mComponentes = new List<CMSComponente>();

            foreach (Guid idComponente in ListaGuids)
            {
                if (this.GestorCMS.ListaComponentes.ContainsKey(idComponente))
                {
                    CMSComponente componente = this.GestorCMS.ListaComponentes[idComponente];
                    if (!mComponentes.Contains(componente))
                    {
                        mComponentes.Add(componente);
                    }
                }
            }
        }

        #region Propiedades

        /// <summary>
        /// Obtiene o establece el Titulo
        /// </summary>
        public string Titulo
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo, value);
            }
        }

        /// <summary>
        /// Obtiene o establece la lista de IDs
        /// </summary>
        public List<Guid> ListaGuids
        {
            get
            {
                string textoIDs = ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.ListaIDs);
                string[] listaIDs = textoIDs.Split(',');
                List<Guid> listaGUIDs = new List<Guid>();
                foreach (string id in listaIDs)
                {
                    Guid idGuid;
                    if (id != "" && Guid.TryParse(id, out idGuid))
                    {
                        listaGUIDs.Add(idGuid);
                    }
                }
                return listaGUIDs;
            }
            set
            {
                string textoIDs = "";
                foreach (Guid id in value)
                {
                    textoIDs += id.ToString() + ",";
                }
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.ListaIDs, textoIDs);
            }
        }

        /// <summary>
        /// Obtiene o establece el tipo de presentacion
        /// </summary>
        public string TipoPresentacionGrupoComponentes
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.TipoPresentacionGrupoComponentes);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.TipoPresentacionGrupoComponentes, value);
            }
        }

        /// <summary>
        /// Obtiene o establece el componente que contiene
        /// </summary>
        public List<CMSComponente> Componentes
        {
            get
            {
                if (mComponentes == null)
                {
                    CargarComponentes();
                }
                return mComponentes;
            }
        }
        #endregion
    }

    /// <summary>
    /// CMSComponenteTesauro
    /// </summary>
    public class CMSComponenteTesauro : CMSComponente
    {
        #region Constructor

        /// <summary>
        /// Constructor a partir de una fila de componente de CMS y del gestor de CMS pasados por parámetro
        /// </summary>
        /// <param name="pFilaComponente">Fila de componente</param>
        /// <param name="pGestorCMS">Gestor de CMS</param>
        public CMSComponenteTesauro(AD.EntityModel.Models.CMS.CMSComponente pFilaComponente, List<AD.EntityModel.Models.CMS.CMSPropiedadComponente> pListaPropiedades, GestionCMS pGestorCMS, LoggingService loggingService, EntityContext entityContext)
            : base(pFilaComponente, pListaPropiedades, pGestorCMS, loggingService, entityContext)
        {
            if (pFilaComponente.TipoComponente != (short)TipoComponenteCMS.Tesauro)
            {
                throw new Exception("El componente debe ser de tipo Tesauro");
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece la lista de IDs
        /// </summary>
        public Guid ElementoID
        {
            get
            {
                string id = ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.ElementoID);

                Guid idGuid = Guid.Empty;

                Guid.TryParse(id, out idGuid);

                return idGuid;
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.ElementoID, value.ToString());
            }
        }

        /// <summary>
        /// Obtiene o establece el Titulo
        /// </summary>
        public string Titulo
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo, value);
            }
        }

        /// <summary>
        /// Obtiene o establece si tiene imagen
        /// </summary>
        public bool TieneImagen
        {
            get
            {
                return bool.Parse(ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.TieneImagen));
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.TieneImagen, value.ToString());
            }
        }



        /// <summary>
        /// Obtiene o establece numero de items que pinta
        /// </summary>
        public short? NumeroItemsMostrar
        {
            get
            {
                string num = ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.NumItemsMostrar);
                if (string.IsNullOrEmpty(num))
                {
                    return null;
                }
                else
                {
                    return short.Parse(num);
                }
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.NumItemsMostrar, value.ToString());
            }
        }

        #endregion
    }
    /// <summary>
    /// CMSComponenteDatosComunidad
    /// </summary>
    public class CMSComponenteDatosComunidad : CMSComponente
    {
        #region Constructor

        /// <summary>
        /// Constructor a partir de una fila de componente de CMS y del gestor de CMS pasados por parámetro
        /// </summary>
        /// <param name="pFilaComponente">Fila de componente</param>
        /// <param name="pGestorCMS">Gestor de CMS</param>
        public CMSComponenteDatosComunidad(AD.EntityModel.Models.CMS.CMSComponente pFilaComponente, List<AD.EntityModel.Models.CMS.CMSPropiedadComponente> pListaPropiedades, GestionCMS pGestorCMS, LoggingService loggingService, EntityContext entityContext)
            : base(pFilaComponente, pListaPropiedades, pGestorCMS, loggingService, entityContext)
        {
            if (pFilaComponente.TipoComponente != (short)TipoComponenteCMS.DatosComunidad)
            {
                throw new Exception("El componente debe ser de tipo Datos comunidad");
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece el Titulo
        /// </summary>
        public string Titulo
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo, value);
            }
        }

        /// <summary>
        /// Muestra el numero de todas las personas (incluidas las no visibles)
        /// </summary>
        public bool ContarPersonasNoVisibles
        {
            get
            {
                if (ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.ContarPersonasNoVisibles) != null)
                {
                    return bool.Parse(ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.ContarPersonasNoVisibles));
                }
                else
                {
                    return false;
                }
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.ContarPersonasNoVisibles, value.ToString());
            }
        }

        #endregion
    }

    /// <summary>
    /// CMSComponenteUsuariosRecomendados
    /// </summary>
    public class CMSComponenteUsuariosRecomendados : CMSComponente
    {
        #region Constructor

        /// <summary>
        /// Constructor a partir de una fila de componente de CMS y del gestor de CMS pasados por parámetro
        /// </summary>
        /// <param name="pFilaComponente">Fila de componente</param>
        /// <param name="pGestorCMS">Gestor de CMS</param>
        public CMSComponenteUsuariosRecomendados(AD.EntityModel.Models.CMS.CMSComponente pFilaComponente, List<AD.EntityModel.Models.CMS.CMSPropiedadComponente> pListaPropiedades, GestionCMS pGestorCMS, LoggingService loggingService, EntityContext entityContext)
            : base(pFilaComponente, pListaPropiedades, pGestorCMS, loggingService, entityContext)
        {
            if (pFilaComponente.TipoComponente != (short)TipoComponenteCMS.UsuariosRecomendados)
            {
                throw new Exception("El componente debe ser de tipo Datos comunidad");
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece el Titulo
        /// </summary>
        public string Titulo
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo, value);
            }
        }

        /// <summary>
        /// Obtiene o establece el HTML
        /// </summary>
        public short NumeroItems
        {
            get
            {
                return short.Parse(ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.NumItems));
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.NumItems, value.ToString());
            }
        }

        #endregion
    }

    /// <summary>
    /// CMSComponenteBuscador
    /// </summary>
    public class CMSComponenteCajaBuscador : CMSComponente
    {
        #region Constructor

        /// <summary>
        /// Constructor a partir de una fila de componente de CMS y del gestor de CMS pasados por parámetro
        /// </summary>
        /// <param name="pFilaComponente">Fila de componente</param>
        /// <param name="pGestorCMS">Gestor de CMS</param>
        public CMSComponenteCajaBuscador(AD.EntityModel.Models.CMS.CMSComponente pFilaComponente, List<AD.EntityModel.Models.CMS.CMSPropiedadComponente> pListaPropiedades, GestionCMS pGestorCMS, LoggingService loggingService, EntityContext entityContext)
            : base(pFilaComponente, pListaPropiedades, pGestorCMS, loggingService, entityContext)
        {
            if (pFilaComponente.TipoComponente != (short)TipoComponenteCMS.CajaBuscador)
            {
                throw new Exception("El componente debe ser de tipo Buscador");
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece el Titulo
        /// </summary>
        public string Titulo
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo, value);
            }
        }

        /// <summary>
        /// Obtiene o establece el texto
        /// </summary>
        public string TextoDefecto
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.TextoDefecto);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.TextoDefecto, value);
            }
        }

        /// <summary>
        /// Obtiene o establece la URL de busqueda
        /// </summary>
        public string URLBusqueda
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.URLBusqueda);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.URLBusqueda, value);
            }
        }
        #endregion
    }

    /// <summary>
    /// CMSComponenteFaceta
    /// </summary>
    public class CMSComponenteFaceta : CMSComponente
    {
        #region Constructor

        /// <summary>
        /// Constructor a partir de una fila de componente de CMS y del gestor de CMS pasados por parámetro
        /// </summary>
        /// <param name="pFilaComponente">Fila de componente</param>
        /// <param name="pGestorCMS">Gestor de CMS</param>
        public CMSComponenteFaceta(AD.EntityModel.Models.CMS.CMSComponente pFilaComponente, List<AD.EntityModel.Models.CMS.CMSPropiedadComponente> pListaPropiedades, GestionCMS pGestorCMS, LoggingService loggingService, EntityContext entityContext)
            : base(pFilaComponente, pListaPropiedades, pGestorCMS, loggingService, entityContext)
        {
            if (pFilaComponente.TipoComponente != (short)TipoComponenteCMS.Faceta)
            {
                throw new Exception("El componente debe ser de tipo Faceta");
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece el Titulo
        /// </summary>
        public string Titulo
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo, value);
            }
        }

        /// <summary>
        /// Obtiene o establece la faceta
        /// </summary>
        public string Faceta
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.Faceta);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.Faceta, value);
            }
        }

        /// <summary>
        /// Obtiene o establece la URL de busqueda
        /// </summary>
        public string URLBusqueda
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.URLBusqueda);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.URLBusqueda, value);
            }
        }

        /// <summary>
        /// Obtiene o establece el Tipo de presentación de las facetas
        /// </summary>
        public TipoPresentacionFacetas TipoPresentacionFaceta
        {
            get
            {
                return (TipoPresentacionFacetas)short.Parse(ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.TipoPresentacionFaceta));
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.TipoPresentacionFaceta, ((short)value).ToString());
            }
        }


        #endregion
    }

    /// <summary>
    /// CMSComponenteListadoUsuarios
    /// </summary>
    public class CMSComponenteListadoUsuarios : CMSComponente
    {
        #region Constructor

        /// <summary>
        /// Constructor a partir de una fila de componente de CMS y del gestor de CMS pasados por parámetro
        /// </summary>
        /// <param name="pFilaComponente">Fila de componente</param>
        /// <param name="pGestorCMS">Gestor de CMS</param>
        public CMSComponenteListadoUsuarios(AD.EntityModel.Models.CMS.CMSComponente pFilaComponente, List<AD.EntityModel.Models.CMS.CMSPropiedadComponente> pListaPropiedades, GestionCMS pGestorCMS, LoggingService loggingService, EntityContext entityContext)
            : base(pFilaComponente, pListaPropiedades, pGestorCMS, loggingService, entityContext)
        {
            if (pFilaComponente.TipoComponente != (short)TipoComponenteCMS.ListadoUsuarios)
            {
                throw new Exception("El componente debe ser de tipo ListadoUsuarios");
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece el Titulo
        /// </summary>
        public string Titulo
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo, value);
            }
        }

        /// <summary>
        /// Obtiene o establece el numero
        /// </summary>
        public short NumeroItems
        {
            get
            {
                return short.Parse(ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.NumItems));
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.NumItems, value.ToString());
            }
        }

        /// <summary>
        /// Obtiene o establece el Tipo de presentación del listado
        /// </summary>
        public TipoPresentacionListadoUsuariosCMS TipoPresentacionListadoUsuarios
        {
            get
            {
                return (TipoPresentacionListadoUsuariosCMS)short.Parse(ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.TipoPresentacionListadoUsuarios));
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.TipoPresentacionListadoUsuarios, ((short)value).ToString());
            }
        }

        /// <summary>
        /// Obtiene o establece el Tipo de listado
        /// </summary>
        public TipoListadoUsuariosCMS TipoListadoUsuarios
        {
            get
            {
                return (TipoListadoUsuariosCMS)short.Parse(ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.TipoListadoUsuarios));
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.TipoListadoUsuarios, ((short)value).ToString());
            }
        }

        #endregion
    }

    /// <summary>
    /// CMSComponenteProyectosRecomendados
    /// </summary>
    public class CMSComponenteListadoProyectos : CMSComponente
    {
        #region Constructor

        /// <summary>
        /// Constructor a partir de una fila de componente de CMS y del gestor de CMS pasados por parámetro
        /// </summary>
        /// <param name="pFilaComponente">Fila de componente</param>
        /// <param name="pGestorCMS">Gestor de CMS</param>
        public CMSComponenteListadoProyectos(AD.EntityModel.Models.CMS.CMSComponente pFilaComponente, List<AD.EntityModel.Models.CMS.CMSPropiedadComponente> pListaPropiedades, GestionCMS pGestorCMS, LoggingService loggingService, EntityContext entityContext)
            : base(pFilaComponente, pListaPropiedades, pGestorCMS, loggingService, entityContext)
        {
            if (pFilaComponente.TipoComponente != (short)TipoComponenteCMS.ListadoProyectos)
            {
                throw new Exception("El componente debe ser de tipo ListadoProyectos");
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece el Titulo
        /// </summary>
        public string Titulo
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo, value);
            }
        }


        /// <summary>
        /// Obtiene o establece el numero
        /// </summary>
        public short NumeroItems
        {
            get
            {
                return short.Parse(ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.NumItems));
            }
            set
            {
                short valor = value;
                if (valor == 0)
                {
                    AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.NumItems, "");
                }
                else
                {
                    AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.NumItems, value.ToString());
                }

            }
        }

        /// <summary>
        /// Obtiene o establece el Tipo de listado
        /// </summary>
        public TipoListadoProyectosCMS TipoListadoProyectos
        {
            get
            {
                return (TipoListadoProyectosCMS)short.Parse(ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.TipoListadoProyectos));
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.TipoListadoProyectos, ((short)value).ToString());
            }
        }

        /// <summary>
        /// Obtiene o establece la lista de IDs
        /// </summary>
        public List<Guid> ListaGuids
        {
            get
            {
                string textoIDs = ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.ListaIDs);
                string[] listaIDs = textoIDs.Split(',');
                List<Guid> listaGUIDs = new List<Guid>();
                foreach (string id in listaIDs)
                {
                    Guid idGuid;
                    if (id != "" && Guid.TryParse(id, out idGuid))
                    {
                        listaGUIDs.Add(idGuid);
                    }
                }
                return listaGUIDs;
            }
            set
            {
                string textoIDs = "";
                foreach (Guid id in value)
                {
                    textoIDs += id.ToString() + ",";
                }
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.ListaIDs, textoIDs);
            }
        }

        #endregion
    }

    /// <summary>
    /// CMSComponenteResumenPerfil
    /// </summary>
    public class CMSComponenteResumenPerfil : CMSComponente
    {
        #region Constructor

        /// <summary>
        /// Constructor a partir de una fila de componente de CMS y del gestor de CMS pasados por parámetro
        /// </summary>
        /// <param name="pFilaComponente">Fila de componente</param>
        /// <param name="pGestorCMS">Gestor de CMS</param>
        public CMSComponenteResumenPerfil(AD.EntityModel.Models.CMS.CMSComponente pFilaComponente, List<AD.EntityModel.Models.CMS.CMSPropiedadComponente> pListaPropiedades, GestionCMS pGestorCMS, LoggingService loggingService, EntityContext entityContext)
            : base(pFilaComponente, pListaPropiedades, pGestorCMS, loggingService, entityContext)
        {
            if (pFilaComponente.TipoComponente != (short)TipoComponenteCMS.ResumenPerfil)
            {
                throw new Exception("El componente debe ser de tipo ResumenPerfil");
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece el Titulo
        /// </summary>
        public string Titulo
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo, value);
            }
        }

        #endregion
    }

    /// <summary>
    /// CMSComponenteMasVistos
    /// </summary>
    public class CMSComponenteMasVistosEnXDias : CMSComponente
    {
        #region Constructor

        /// <summary>
        /// Constructor a partir de una fila de componente de CMS y del gestor de CMS pasados por parámetro
        /// </summary>
        /// <param name="pFilaComponente">Fila de componente</param>
        /// <param name="pGestorCMS">Gestor de CMS</param>
        public CMSComponenteMasVistosEnXDias(AD.EntityModel.Models.CMS.CMSComponente pFilaComponente, List<AD.EntityModel.Models.CMS.CMSPropiedadComponente> pListaPropiedades, GestionCMS pGestorCMS, LoggingService loggingService, EntityContext entityContext)
            : base(pFilaComponente, pListaPropiedades, pGestorCMS, loggingService, entityContext)
        {
            if (pFilaComponente.TipoComponente != (short)TipoComponenteCMS.MasVistosEnXDias)
            {
                throw new Exception("El componente debe ser de tipo MasVistosEnXDias");
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece el Titulo
        /// </summary>
        public string Titulo
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo, value);
            }
        }

        /// <summary>
        /// Obtiene o establece el numero
        /// </summary>
        public short NumeroItems
        {
            get
            {
                return short.Parse(ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.NumItems));
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.NumItems, value.ToString());
            }
        }

        /// <summary>
        /// Obtiene o establece el numero
        /// </summary>
        public short NumeroDias
        {
            get
            {
                return short.Parse(ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.NumDias));
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.NumDias, value.ToString());
            }
        }

        /// <summary>
        /// Obtiene o establece el tipo de presentacion de cada recurso
        /// </summary>
        public string TipoPresentacionRecurso
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.TipoPresentacionRecurso);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.TipoPresentacionRecurso, value);
            }
        }

        /// <summary>
        /// Obtiene o establece el tipo de presentacion del listado de recursos
        /// </summary>
        public string TipoPresentacionListadoRecursos
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.TipoPresentacionListadoRecursos);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.TipoPresentacionListadoRecursos, value);
            }
        }

        #endregion
    }


    /// <summary>
    /// CMSComponenteMasVistos
    /// </summary>
    public class CMSComponenteMasVistos : CMSComponente
    {
        #region Constructor

        /// <summary>
        /// Constructor a partir de una fila de componente de CMS y del gestor de CMS pasados por parámetro
        /// </summary>
        /// <param name="pFilaComponente">Fila de componente</param>
        /// <param name="pGestorCMS">Gestor de CMS</param>
        public CMSComponenteMasVistos(AD.EntityModel.Models.CMS.CMSComponente pFilaComponente, List<AD.EntityModel.Models.CMS.CMSPropiedadComponente> pListaPropiedades, GestionCMS pGestorCMS, LoggingService loggingService, EntityContext entityContext)
            : base(pFilaComponente, pListaPropiedades, pGestorCMS, loggingService, entityContext)
        {
            if (pFilaComponente.TipoComponente != (short)TipoComponenteCMS.MasVistos)
            {
                throw new Exception("El componente debe ser de tipo MasVistos");
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece el Titulo
        /// </summary>
        public string Titulo
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo, value);
            }
        }

        /// <summary>
        /// Obtiene o establece el numero
        /// </summary>
        public short NumeroItems
        {
            get
            {
                return short.Parse(ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.NumItems));
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.NumItems, value.ToString());
            }
        }

        /// <summary>
        /// Obtiene o establece el tipo de presentacion de cada recurso
        /// </summary>
        public string TipoPresentacionRecurso
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.TipoPresentacionRecurso);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.TipoPresentacionRecurso, value);
            }
        }

        #endregion
    }


    /// <summary>
    /// CMSComponenteEnvioCorreo
    /// </summary>
    public class CMSComponenteEnvioCorreo : CMSComponente
    {
        #region Constructor

        /// <summary>
        /// Constructor a partir de una fila de componente de CMS y del gestor de CMS pasados por parámetro
        /// </summary>
        /// <param name="pFilaComponente">Fila de componente</param>
        /// <param name="pGestorCMS">Gestor de CMS</param>
        public CMSComponenteEnvioCorreo(AD.EntityModel.Models.CMS.CMSComponente pFilaComponente, List<AD.EntityModel.Models.CMS.CMSPropiedadComponente> pListaPropiedades, GestionCMS pGestorCMS, LoggingService loggingService, EntityContext entityContext)
            : base(pFilaComponente, pListaPropiedades, pGestorCMS, loggingService, entityContext)
        {
            if (pFilaComponente.TipoComponente != (short)TipoComponenteCMS.EnvioCorreo)
            {
                throw new Exception("El componente debe ser de tipo EnvioCorreo");
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece el Titulo
        /// </summary>
        public string Titulo
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo, value);
            }
        }


        /// <summary>
        /// Obtiene o establece la lista de Campos de Envio deCorreo
        /// </summary>
        public Dictionary<short, Dictionary<TipoPropiedadEnvioCorreo, string>> ListaCamposEnvioCorreo
        {
            get
            {
                //Nombre
                //Obligatorio
                //Tipo
                string textoListaCamposEnvioCorreo = ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.ListaCamposEnvioCorreo);
                string[] campos = textoListaCamposEnvioCorreo.Split(new string[] { "###" }, StringSplitOptions.RemoveEmptyEntries);

                Dictionary<short, Dictionary<TipoPropiedadEnvioCorreo, string>> listaCamposEnvioCorreo = new Dictionary<short, Dictionary<TipoPropiedadEnvioCorreo, string>>();
                short i = 0;
                foreach (string campo in campos)
                {
                    //porque tenemos propiedades vacías
                    string[] propiedades = campo.Split(new string[] { "&&&" }, StringSplitOptions.None);
                    Dictionary<TipoPropiedadEnvioCorreo, string> diccionarioPropiedades = new Dictionary<TipoPropiedadEnvioCorreo, string>();
                    diccionarioPropiedades.Add(TipoPropiedadEnvioCorreo.Nombre, propiedades[0]);
                    diccionarioPropiedades.Add(TipoPropiedadEnvioCorreo.Obligatorio, propiedades[1]);
                    diccionarioPropiedades.Add(TipoPropiedadEnvioCorreo.TipoCampo, propiedades[2]);
                    listaCamposEnvioCorreo.Add(i, diccionarioPropiedades);
                    i++;
                }
                return listaCamposEnvioCorreo;
            }
            set
            {
                string textoListaCamposEnvioCorreo = "";
                string separador = "";
                foreach (short orden in value.Keys)
                {
                    textoListaCamposEnvioCorreo += separador + value[orden][TipoPropiedadEnvioCorreo.Nombre] + "&&&" + value[orden][TipoPropiedadEnvioCorreo.Obligatorio] + "&&&" + value[orden][TipoPropiedadEnvioCorreo.TipoCampo];
                    separador = "###";
                }
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.ListaCamposEnvioCorreo, textoListaCamposEnvioCorreo);
            }
        }


        /// <summary>
        /// Obtiene o establece el Texto del Boton
        /// </summary>
        public string TextoBoton
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.TextoBoton);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.TextoBoton, value);
            }
        }

        /// <summary>
        /// Obtiene o establece el Texto para notificar que el proceso del componente ha sido correcto
        /// </summary>
        public string TextoMensajeOK
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.TextoMensajeOK);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.TextoMensajeOK, value);
            }
        }

        /// <summary>
        /// Obtiene o establece el Destinatario del Correo
        /// </summary>
        public string DestinatarioCorreo
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.DestinatarioCorreo);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.DestinatarioCorreo, value);
            }
        }

        #endregion
    }

    /// <summary>
    /// CMSComponentePreguntaTIC
    /// </summary>
    public class CMSComponentePreguntaTIC : CMSComponente
    {
        #region Constructor

        /// <summary>
        /// Constructor a partir de una fila de componente de CMS y del gestor de CMS pasados por parámetro
        /// </summary>
        /// <param name="pFilaComponente">Fila de componente</param>
        /// <param name="pGestorCMS">Gestor de CMS</param>
        public CMSComponentePreguntaTIC(AD.EntityModel.Models.CMS.CMSComponente pFilaComponente, List<AD.EntityModel.Models.CMS.CMSPropiedadComponente> pListaPropiedades, GestionCMS pGestorCMS, LoggingService loggingService, EntityContext entityContext)
            : base(pFilaComponente, pListaPropiedades, pGestorCMS, loggingService, entityContext)
        {
            if (pFilaComponente.TipoComponente != (short)TipoComponenteCMS.PreguntaTIC)
            {
                throw new Exception("El componente debe ser de tipo PreguntaTIC");
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece el Titulo
        /// </summary>
        public string Titulo
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo, value);
            }
        }

        /// <summary>
        /// Obtiene o establece la ontologia
        /// </summary>
        public Guid OntologiaID
        {
            get
            {
                string id = ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.ElementoID);
                Guid idGuid = Guid.Empty;
                Guid.TryParse(id, out idGuid);
                return idGuid;
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.ElementoID, value.ToString());
            }
        }


        #endregion
    }

    /// <summary>
    /// CMSComponenteMenu
    /// </summary>
    public class CMSComponenteMenu : CMSComponente
    {
        #region Constructor

        /// <summary>
        /// Constructor a partir de una fila de componente de CMS y del gestor de CMS pasados por parámetro
        /// </summary>
        /// <param name="pFilaComponente">Fila de componente</param>
        /// <param name="pGestorCMS">Gestor de CMS</param>
        public CMSComponenteMenu(AD.EntityModel.Models.CMS.CMSComponente pFilaComponente, List<AD.EntityModel.Models.CMS.CMSPropiedadComponente> pListaPropiedades, GestionCMS pGestorCMS, LoggingService loggingService, EntityContext entityContext)
            : base(pFilaComponente, pListaPropiedades, pGestorCMS, loggingService, entityContext)
        {
            if (pFilaComponente.TipoComponente != (short)TipoComponenteCMS.Menu)
            {
                throw new Exception("El componente debe ser de tipo Menu");
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece el Titulo
        /// </summary>
        public string Titulo
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo, value);
            }
        }

        /// <summary>
        /// Obtiene o establece la lista de Opciones del menú
        /// </summary>
        public Dictionary<short, KeyValuePair<short, Dictionary<TipoPropiedadMenu, string>>> ListaOpcionesMenu
        {
            get
            {
                //Nombre
                //Enlace
                string textoListaOpcionesMenu = ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.ListaOpcionesMenu);
                string[] campos = textoListaOpcionesMenu.Split(new string[] { "###" }, StringSplitOptions.RemoveEmptyEntries);

                Dictionary<short, KeyValuePair<short, Dictionary<TipoPropiedadMenu, string>>> listaOpcionesMenu = new Dictionary<short, KeyValuePair<short, Dictionary<TipoPropiedadMenu, string>>>();
                short i = 0;
                foreach (string campo in campos)
                {
                    //porque tenemos propiedades vacías
                    string[] propiedades = campo.Split(new string[] { "&&&" }, StringSplitOptions.None);
                    if (propiedades.Length == 2)
                    {
                        Dictionary<TipoPropiedadMenu, string> diccionarioPropiedades = new Dictionary<TipoPropiedadMenu, string>();
                        diccionarioPropiedades.Add(TipoPropiedadMenu.Nombre, propiedades[0]);
                        diccionarioPropiedades.Add(TipoPropiedadMenu.Enlace, propiedades[1]);
                        listaOpcionesMenu.Add(i, new KeyValuePair<short, Dictionary<TipoPropiedadMenu, string>>(0, diccionarioPropiedades));
                    }
                    else if (propiedades.Length == 3)
                    {
                        Dictionary<TipoPropiedadMenu, string> diccionarioPropiedades = new Dictionary<TipoPropiedadMenu, string>();
                        diccionarioPropiedades.Add(TipoPropiedadMenu.Nombre, propiedades[1]);
                        diccionarioPropiedades.Add(TipoPropiedadMenu.Enlace, propiedades[2]);
                        listaOpcionesMenu.Add(i, new KeyValuePair<short, Dictionary<TipoPropiedadMenu, string>>(short.Parse(propiedades[0]), diccionarioPropiedades));
                    }
                    i++;
                }
                return listaOpcionesMenu;
            }
            set
            {
                string textoListaOpcionesMenu = "";
                string separador = "";
                foreach (short orden in value.Keys)
                {
                    textoListaOpcionesMenu += separador + value[orden].Key + "&&&" + value[orden].Value[TipoPropiedadMenu.Nombre] + "&&&" + value[orden].Value[TipoPropiedadMenu.Enlace];
                    separador = "###";
                }
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.ListaOpcionesMenu, textoListaOpcionesMenu);
            }
        }


        /// <summary>
        /// Obtiene o establece el Texto del Boton
        /// </summary>
        public string TextoBoton
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.TextoBoton);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.TextoBoton, value);
            }
        }

        /// <summary>
        /// Obtiene o establece el Texto para notificar que el proceso del componente ha sido correcto
        /// </summary>
        public string TextoMensajeOK
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.TextoMensajeOK);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.TextoMensajeOK, value);
            }
        }

        /// <summary>
        /// Obtiene o establece el Destinatario del Correo
        /// </summary>
        public string DestinatarioCorreo
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.DestinatarioCorreo);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.DestinatarioCorreo, value);
            }
        }

        #endregion
    }

    /// <summary>
    /// CMSComponenteBuscador
    /// </summary>
    public class CMSComponenteBuscador : CMSComponente
    {
        #region Constructor

        /// <summary>
        /// Constructor a partir de una fila de componente de CMS y del gestor de CMS pasados por parámetro
        /// </summary>
        /// <param name="pFilaComponente">Fila de componente</param>
        /// <param name="pGestorCMS">Gestor de CMS</param>
        public CMSComponenteBuscador(AD.EntityModel.Models.CMS.CMSComponente pFilaComponente, List<AD.EntityModel.Models.CMS.CMSPropiedadComponente> pListaPropiedades, GestionCMS pGestorCMS, LoggingService loggingService, EntityContext entityContext)
            : base(pFilaComponente, pListaPropiedades, pGestorCMS, loggingService, entityContext)
        {
            if (pFilaComponente.TipoComponente != (short)TipoComponenteCMS.Buscador)
            {
                throw new Exception("El componente debe ser de tipo Buscador");
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece el Titulo
        /// </summary>
        public string Titulo
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo, value);
            }
        }

        /// <summary>
        /// Obtiene o establece la URL de busqueda
        /// </summary>
        public string URLBusqueda
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.URLBusqueda);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.URLBusqueda, value);
            }
        }

        /// <summary>
        /// Obtiene o establece el TituloAtributoDeBusqueda
        /// </summary>
        public string TituloAtributoDeBusqueda
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.TituloAtributoDeBusqueda);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.TituloAtributoDeBusqueda, value);
            }
        }

        /// <summary>
        /// Obtiene o establece el AtributoDeBusqueda
        /// </summary>
        public string AtributoDeBusqueda
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.AtributoDeBusqueda);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.AtributoDeBusqueda, value);
            }
        }

        /// <summary>
        /// Obtiene o establece numero de items que pinta
        /// </summary>
        public short NumeroItems
        {
            get
            {
                return short.Parse(ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.NumItems));
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.NumItems, value.ToString());
            }
        }

        /// <summary>
        /// Obtiene o establece el tipo de presentacion de cada recurso
        /// </summary>
        public string TipoPresentacionRecurso
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.TipoPresentacionRecurso);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.TipoPresentacionRecurso, value.ToString());
            }
        }
        #endregion
    }

    /// <summary>
    /// CMSComponenteBuscadorSPARQL
    /// </summary>
    public class CMSComponenteBuscadorSPARQL : CMSComponente
    {
        #region Constructor

        /// <summary>
        /// Constructor a partir de una fila de componente de CMS y del gestor de CMS pasados por parámetro
        /// </summary>
        /// <param name="pFilaComponente">Fila de componente</param>
        /// <param name="pGestorCMS">Gestor de CMS</param>
        public CMSComponenteBuscadorSPARQL(AD.EntityModel.Models.CMS.CMSComponente pFilaComponente, List<AD.EntityModel.Models.CMS.CMSPropiedadComponente> pListaPropiedades, GestionCMS pGestorCMS, LoggingService loggingService, EntityContext entityContext)
            : base(pFilaComponente, pListaPropiedades, pGestorCMS, loggingService, entityContext)
        {
            if (pFilaComponente.TipoComponente != (short)TipoComponenteCMS.BuscadorSPARQL)
            {
                throw new Exception("El componente debe ser de tipo BuscadorSPARQL");
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece el Titulo
        /// </summary>
        public string Titulo
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo, value);
            }
        }


        /// <summary>
        /// Obtiene o establece la URL de ver mas
        /// </summary>
        public string QuerySPARQL
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.QuerySPARQL);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.QuerySPARQL, value);
            }
        }

        /// <summary>
        /// Obtiene o establece numero de items que pinta
        /// </summary>
        public short NumeroItems
        {
            get
            {
                return short.Parse(ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.NumItems));
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.NumItems, value.ToString());
            }
        }

        /// <summary>
        /// Obtiene o establece el tipo de presentacion de cada recurso
        /// </summary>
        public string TipoPresentacionRecurso
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.TipoPresentacionRecurso);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.TipoPresentacionRecurso, value);
            }
        }
        #endregion
    }

    /// <summary>
    /// CMSComponenteFichaDescripcionDocumento
    /// </summary>
    public class CMSComponenteFichaDescripcionDocumento : CMSComponente
    {
        #region Constructor

        /// <summary>
        /// Constructor a partir de una fila de componente de CMS y del gestor de CMS pasados por parámetro
        /// </summary>
        /// <param name="pFilaComponente">Fila de componente</param>
        /// <param name="pGestorCMS">Gestor de CMS</param>
        public CMSComponenteFichaDescripcionDocumento(AD.EntityModel.Models.CMS.CMSComponente pFilaComponente, List<AD.EntityModel.Models.CMS.CMSPropiedadComponente> pListaPropiedades, GestionCMS pGestorCMS, LoggingService loggingService, EntityContext entityContext)
            : base(pFilaComponente, pListaPropiedades, pGestorCMS, loggingService, entityContext)
        {
            if (pFilaComponente.TipoComponente != (short)TipoComponenteCMS.FichaDescripcionDocumento)
            {
                throw new Exception("El componente debe ser de tipo FichaDescripcionDocumento");
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece el Titulo
        /// </summary>
        public string Titulo
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo, value);
            }
        }

        /// <summary>
        /// Obtiene o establece el id del recurso
        /// </summary>
        public Guid DocumentoID
        {
            get
            {
                string id = ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.ElementoID);

                Guid idGuid = Guid.Empty;

                Guid.TryParse(id, out idGuid);

                return idGuid;
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.ElementoID, value.ToString());
            }
        }
        #endregion
    }

    /// <summary>
    /// CMSComponenteUltimosRecursosVisitados
    /// </summary>
    public class CMSComponenteUltimosRecursosVisitados : CMSComponente
    {
        #region Constructor

        /// <summary>
        /// Constructor a partir de una fila de componente de CMS y del gestor de CMS pasados por parámetro
        /// </summary>
        /// <param name="pFilaComponente">Fila de componente</param>
        /// <param name="pGestorCMS">Gestor de CMS</param>
        public CMSComponenteUltimosRecursosVisitados(AD.EntityModel.Models.CMS.CMSComponente pFilaComponente, List<AD.EntityModel.Models.CMS.CMSPropiedadComponente> pListaPropiedades, GestionCMS pGestorCMS, LoggingService loggingService, EntityContext entityContext)
            : base(pFilaComponente, pListaPropiedades, pGestorCMS, loggingService, entityContext)
        {
            if (pFilaComponente.TipoComponente != (short)TipoComponenteCMS.UltimosRecursosVisitados)
            {
                throw new Exception("El componente debe ser de tipo UltimosRecursosVisitados");
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece el Titulo
        /// </summary>
        public string Titulo
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo, value);
            }
        }

        /// <summary>
        /// Obtiene o establece numero de items que pinta
        /// </summary>
        public short NumeroItems
        {
            get
            {
                return short.Parse(ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.NumItems));
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.NumItems, value.ToString());
            }
        }

        /// <summary>
        /// Obtiene o establece el tipo de presentacion de cada recurso
        /// </summary>
        public string TipoPresentacionRecurso
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.TipoPresentacionRecurso);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.TipoPresentacionRecurso, value);
            }
        }

        /// <summary>
        /// Obtiene o establece el tipo de presentacion del listado de recursos
        /// </summary>
        public string TipoPresentacionListadoRecursos
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.TipoPresentacionListadoRecursos);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.TipoPresentacionListadoRecursos, value);
            }
        }
        #endregion
    }


    /// <summary>
    /// CMSComponenteConsultaSPARQL
    /// </summary>
    public class CMSComponenteConsultaSPARQL : CMSComponente
    {
        #region Constructor

        /// <summary>
        /// Constructor a partir de una fila de componente de CMS y del gestor de CMS pasados por parámetro
        /// </summary>
        /// <param name="pFilaComponente">Fila de componente</param>
        /// <param name="pGestorCMS">Gestor de CMS</param>
        public CMSComponenteConsultaSPARQL(AD.EntityModel.Models.CMS.CMSComponente pFilaComponente, List<AD.EntityModel.Models.CMS.CMSPropiedadComponente> pListaPropiedades, GestionCMS pGestorCMS, LoggingService loggingService, EntityContext entityContext)
            : base(pFilaComponente, pListaPropiedades, pGestorCMS, loggingService, entityContext)
        {
            if (pFilaComponente.TipoComponente != (short)TipoComponenteCMS.ConsultaSPARQL)
            {
                throw new Exception("El componente debe ser de tipo ConsultaSPARQL");
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece el Titulo
        /// </summary>
        public string Titulo
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo, value);
            }
        }


        /// <summary>
        /// Obtiene o establece la URL de ver mas
        /// </summary>
        public string QuerySPARQL
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.QuerySPARQL);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.QuerySPARQL, value);
            }
        }

        #endregion
    }

    /// <summary>
    /// CMSComponenteConsultaSQLSERVER
    /// </summary>
    public class CMSComponenteConsultaSQLSERVER : CMSComponente
    {
        #region Constructor

        /// <summary>
        /// Constructor a partir de una fila de componente de CMS y del gestor de CMS pasados por parámetro
        /// </summary>
        /// <param name="pFilaComponente">Fila de componente</param>
        /// <param name="pGestorCMS">Gestor de CMS</param>
        public CMSComponenteConsultaSQLSERVER(AD.EntityModel.Models.CMS.CMSComponente pFilaComponente, List<AD.EntityModel.Models.CMS.CMSPropiedadComponente> pListaPropiedades, GestionCMS pGestorCMS, LoggingService loggingService, EntityContext entityContext)
            : base(pFilaComponente, pListaPropiedades, pGestorCMS, loggingService, entityContext)
        {
            if (pFilaComponente.TipoComponente != (short)TipoComponenteCMS.ConsultaSQLSERVER)
            {
                throw new Exception("El componente debe ser de tipo ConsultaSQLSERVER");
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece el Titulo
        /// </summary>
        public string Titulo
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.Titulo, value);
            }
        }


        /// <summary>
        /// Obtiene o establece la URL de ver mas
        /// </summary>
        public string QuerySQLSERVER
        {
            get
            {
                return ObtenerValorDePropiedadDeComponente(TipoPropiedadCMS.QuerySQLSERVER);
            }
            set
            {
                AsignarValorDePropiedadDeComponente(TipoPropiedadCMS.QuerySQLSERVER, value);
            }
        }

        #endregion
    }


    /// <summary>
    /// CMSComponenteRolGrupoIdentidades
    /// </summary>
    public class CMSComponenteRolGrupoIdentidades : ElementoGnoss
    {
        #region Constructor

        /// <summary>
        /// Constructor a partir de una fila de CMSComponenteRolGrupoIdentidades y del gestor de CMS pasados por parámetro
        /// </summary>
        /// <param name="pFilaCMSRolGrupoIdentidades">Fila de CMSRolGrupoIdentidades</param>
        /// <param name="pGestorCMS">Gestor de CMS</param>
        public CMSComponenteRolGrupoIdentidades(AD.EntityModel.Models.CMS.CMSComponenteRolGrupoIdentidades pFilaCMSComponenteRolGrupoIdentidades, GestionCMS pGestorCMS, LoggingService loggingService)
            : base(pFilaCMSComponenteRolGrupoIdentidades, pGestorCMS, loggingService)
        {
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establedce el grupo
        /// </summary>
        public Guid GrupoID
        {
            get
            {
                return FilCMSComponenteRolGrupoIdentidades.GrupoID;
            }
            set
            {
                FilCMSComponenteRolGrupoIdentidades.GrupoID = value;
            }
        }

        /// <summary>
        /// Devuelve el gestor de CMS que contiene al componente
        /// </summary>
        public GestionCMS GestorCMS
        {
            get
            {
                return (GestionCMS)this.GestorGnoss;
            }
        }

        /// <summary>
        /// Obtiene la fila de la página
        /// </summary>
        public AD.EntityModel.Models.CMS.CMSComponenteRolGrupoIdentidades FilCMSComponenteRolGrupoIdentidades
        {
            get
            {
                return (AD.EntityModel.Models.CMS.CMSComponenteRolGrupoIdentidades)FilaElementoEntity;
            }
        }

        #endregion
    }

    /// <summary>
    /// CMSComponenteRolIdentidad
    /// </summary>
    public class CMSComponenteRolIdentidad : ElementoGnoss
    {
        #region Constructor

        /// <summary>
        /// Constructor a partir de una fila deCMSComponenteRolIdentidad y del gestor de CMS pasados por parámetro
        /// </summary>
        /// <param name="pFilaCMSComponente">Fila de CMSComponente</param>
        /// <param name="pGestorCMS">Gestor de CMS</param>
        public CMSComponenteRolIdentidad(AD.EntityModel.Models.CMS.CMSComponenteRolIdentidad pFilaCMSComponenteRolIdentidad, GestionCMS pGestorCMS, LoggingService loggingService)
            : base(pFilaCMSComponenteRolIdentidad, pGestorCMS, loggingService)
        {
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece el perfil
        /// </summary>
        public Guid PerfilID
        {
            get
            {
                return FilaCMSComponenteRolIdentidad.PerfilID;
            }
            set
            {
                FilaCMSComponenteRolIdentidad.PerfilID = value;
            }
        }

        /// <summary>
        /// Devuelve el gestor de CMS
        /// </summary>
        public GestionCMS GestorCMS
        {
            get
            {
                return (GestionCMS)this.GestorGnoss;
            }
        }

        /// <summary>
        /// Obtiene la fila de la página
        /// </summary>
        public AD.EntityModel.Models.CMS.CMSComponenteRolIdentidad FilaCMSComponenteRolIdentidad
        {
            get
            {
                return (AD.EntityModel.Models.CMS.CMSComponenteRolIdentidad)FilaElementoEntity;
            }
        }

        #endregion
    }

    public class JsonListado
    {
        public List<JsonDocumento> ListadoDocumentos { get; set; }
    }

    public class JsonDocumento
    {
        public string Titulo { get; set; }
        public string Enlace { get; set; }
        public List<JsonEtiqueta> Etiquetas { get; set; }
        public List<JsonCategoria> Categorias { get; set; }
        public string Imagen { get; set; }
        public string NombreImagen { get; set; }
        public string NombreAutor { get; set; }
        public string EnlaceAutor { get; set; }
        public string ImagenAutor { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; }
        public string Accion { get; set; }
        public Guid Clave { get; set; }
    }

    public class JsonCategoria
    {
        public Guid Clave { get; set; }
        public string Nombre { get; set; }
        public string Url { get; set; }
    }

    public class JsonEtiqueta
    {
        public string Nombre { get; set; }
        public string Url { get; set; }
    }

}
