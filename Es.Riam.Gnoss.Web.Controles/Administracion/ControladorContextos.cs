using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Es.Riam.Gnoss.Web.Controles.Administracion
{
    public class ControladorContextos : ControladorBase
    {
        private DataWrapperProyecto mDataWrapperProyecto;
        private Elementos.ServiciosGenerales.Proyecto ProyectoSeleccionado = null;
        private Dictionary<string, string> ParametroProyecto = null;
        private bool CrearFilasPropiedadesExportacion = false;
        private Dictionary<string, Guid> mListaProyectosOrigen = null;
        private const bool UTILIZAR_MASCARA = false;

        private LoggingService mLoggingService;
        private VirtuosoAD mVirtuosoAD;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private RedisCacheWrapper mRedisCacheWrapper;

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        public ControladorContextos(Elementos.ServiciosGenerales.Proyecto pProyecto, Dictionary<string, string> pParametroProyecto, LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, VirtuosoAD virtuosoAD, GnossCache gnossCache, IHttpContextAccessor httpContextAccessor, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, bool pCrearFilasPropiedadesExportacion = false)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, servicesUtilVirtuosoAndReplication)
        {
            mVirtuosoAD = virtuosoAD;
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mRedisCacheWrapper = redisCacheWrapper;

            ProyectoSeleccionado = pProyecto;
            ParametroProyecto = pParametroProyecto;

            CrearFilasPropiedadesExportacion = pCrearFilasPropiedadesExportacion;
        }

        #endregion

        #region Metodos de Carga


        public ContextoModel CargarGadget(ProyectoGadget filaGadget)
        {
            ContextoModel gadget = CrearGadget(filaGadget);

            List<IntegracionContinuaPropiedad> propiedadesIntegracionContinua = new List<IntegracionContinuaPropiedad>();

            if (filaGadget.Tipo == (short)TipoGadget.RecursosContextos)
            {
                //filaGadget.GetProyectoGadgetContextoRows();
                ProyectoGadgetContexto filasContexto = filaGadget.ProyectoGadgetContexto;
                if (filasContexto != null)
                {
                    gadget.Contexto = CargarContexto(filasContexto);

                    if (CrearFilasPropiedadesExportacion)
                    {
                        propiedadesIntegracionContinua = ObtenerPropiedadesIntegracionContinua(propiedadesIntegracionContinua, gadget, true);
                    }
                }
            }

            if (filaGadget.Tipo == (short)TipoGadget.HtmlIncrustado) 
            {
                if(mConfigService.ObtenerListaIdiomasDictionary().Count > 1)
                {
                    ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);                    
                    List<ProyectoGadgetIdioma> listaProyectoGadgetIdioma = proyectoCN.ObtenerProyectoGadgetIdiomaDeGadget(filaGadget.GadgetID);
                    if(listaProyectoGadgetIdioma != null && listaProyectoGadgetIdioma.Count > 0)
                    {
                        foreach(ProyectoGadgetIdioma proyectoGadgetIdioma in listaProyectoGadgetIdioma)
                        {
                            gadget.Contenido += $"{proyectoGadgetIdioma.Contenido}@{proyectoGadgetIdioma.Idioma}|||";
                        }
                    }
                }
            }

            if (CrearFilasPropiedadesExportacion && !string.IsNullOrEmpty(filaGadget.ComunidadDestinoFiltros))
            {
                //Crear las filas de las porpiedades de Integracion Continua
                IntegracionContinuaPropiedad propiedadFiltrosDestino = new IntegracionContinuaPropiedad();
                propiedadFiltrosDestino.ProyectoID = ProyectoSeleccionado.Clave;
                propiedadFiltrosDestino.TipoObjeto = (short)TipoObjeto.Gadget;
                propiedadFiltrosDestino.ObjetoPropiedad = filaGadget.NombreCorto;
                propiedadFiltrosDestino.TipoPropiedad = (short)TipoPropiedad.FiltrosDestinoGadget;
                propiedadFiltrosDestino.ValorPropiedad = filaGadget.ComunidadDestinoFiltros;

                IntegracionContinuaPropiedad propiedadEsta = propiedadesIntegracionContinua.Where(propiedad => propiedad.ProyectoID == ProyectoSeleccionado.Clave && propiedad.TipoObjeto == propiedadFiltrosDestino.TipoObjeto && propiedad.ObjetoPropiedad == propiedadFiltrosDestino.ObjetoPropiedad && propiedad.TipoPropiedad == propiedadFiltrosDestino.TipoPropiedad).FirstOrDefault();

                if (propiedadEsta == null)
                {
                    propiedadesIntegracionContinua.Add(propiedadFiltrosDestino);
                }
            }

            if (gadget.TipoGadget == TipoGadget.CMS)
            {
                Guid testGuid = Guid.Empty;

                if (!Guid.TryParse(gadget.Contenido, out testGuid))
                {
                    gadget.Contenido = testGuid.ToString();
                }
            }

            if (CrearFilasPropiedadesExportacion)
            {
                try
                {
                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    proyCN.CrearFilasIntegracionContinuaParametro(propiedadesIntegracionContinua, ProyectoSeleccionado.Clave, TipoObjeto.Gadget, filaGadget.NombreCorto);
                    proyCN.Dispose();
                }
                catch
                { }
            }

            return gadget;
        }

        private static ContextoModel CrearGadget(ProyectoGadget filaGadget)
        {
            ContextoModel gadget = new ContextoModel();
            gadget.Key = filaGadget.GadgetID;
            gadget.TipoGadget = (TipoGadget)filaGadget.Tipo;
            gadget.Name = filaGadget.Titulo;
            gadget.Orden = filaGadget.Orden;
            gadget.Clases = filaGadget.Clases;
            gadget.Visible = filaGadget.Visible;
            gadget.Contenido = filaGadget.Contenido;
            gadget.Ajax = filaGadget.CargarPorAjax;
            gadget.FiltrosDestino = filaGadget.ComunidadDestinoFiltros;
            if (string.IsNullOrEmpty(filaGadget.NombreCorto))
            {
                gadget.ShortName = filaGadget.GadgetID.ToString();
            }
            else
            {
                gadget.ShortName = filaGadget.NombreCorto;
            }

            if (gadget.TipoGadget == TipoGadget.HtmlIncrustado)
            {
                List<ProyectoGadgetIdioma> filasIdioma = filaGadget.ProyectoGadgetIdioma.ToList();

                if (filasIdioma.Count > 0)
                {
                    string contenido = "";
                    foreach (ProyectoGadgetIdioma filaIdioma in filasIdioma)
                    {
                        contenido += filaIdioma.Contenido + "@" + filaIdioma.Idioma + "|||";
                    }
                    gadget.Contenido = contenido;
                }
            }

            return gadget;
        }

        private List<IntegracionContinuaPropiedad> AsignarValoresPropiedadesIntegracionContinua(List<IntegracionContinuaPropiedad> pPropiedadesIntegracionContinua, ContextoModel pGadget, bool pEsCarga)
        {
            if (!string.IsNullOrEmpty(pGadget.Contexto.ComunidadOrigen))
            {
                //Crear las filas de las porpiedades de Integracion Continua
                IntegracionContinuaPropiedad propiedadComunidadOrigen = new IntegracionContinuaPropiedad();
                propiedadComunidadOrigen.ProyectoID = ProyectoSeleccionado.Clave;
                propiedadComunidadOrigen.TipoObjeto = (short)TipoObjeto.Gadget;
                propiedadComunidadOrigen.ObjetoPropiedad = pGadget.ShortName;
                propiedadComunidadOrigen.TipoPropiedad = (short)TipoPropiedad.ComunidadOrigenGadget;
                propiedadComunidadOrigen.ValorPropiedad = pGadget.Contexto.ComunidadOrigen;
                pPropiedadesIntegracionContinua.Add(propiedadComunidadOrigen);
                if (!pEsCarga)
                {
                    //pGadget.Contexto.ComunidadOrigen = UtilIntegracionContinua.ObtenerMascaraPropiedad(propiedadComunidadOrigen);
                }
            }

            if (!string.IsNullOrEmpty(pGadget.Contexto.FiltrosOrigen))
            {
                IntegracionContinuaPropiedad propiedadFiltrosOrigen = new IntegracionContinuaPropiedad();
                propiedadFiltrosOrigen.ProyectoID = ProyectoSeleccionado.Clave;
                propiedadFiltrosOrigen.TipoObjeto = (short)TipoObjeto.Gadget;
                propiedadFiltrosOrigen.ObjetoPropiedad = pGadget.ShortName;
                propiedadFiltrosOrigen.TipoPropiedad = (short)TipoPropiedad.FiltrosOrigenGadget;
                propiedadFiltrosOrigen.ValorPropiedad = pGadget.Contexto.FiltrosOrigen;
                pPropiedadesIntegracionContinua.Add(propiedadFiltrosOrigen);
                if (!pEsCarga)
                {
                    //pGadget.Contexto.FiltrosOrigen = UtilIntegracionContinua.ObtenerMascaraPropiedad(propiedadFiltrosOrigen);
                }

            }

            if (!string.IsNullOrEmpty(pGadget.Contexto.RelacionOrigenDestino))
            {
                IntegracionContinuaPropiedad propiedadRelacionOrigenDestino = new IntegracionContinuaPropiedad();
                propiedadRelacionOrigenDestino.ProyectoID = ProyectoSeleccionado.Clave;
                propiedadRelacionOrigenDestino.TipoObjeto = (short)TipoObjeto.Gadget;
                propiedadRelacionOrigenDestino.ObjetoPropiedad = pGadget.ShortName;
                propiedadRelacionOrigenDestino.TipoPropiedad = (short)TipoPropiedad.RelacionOrigenDestinoGadget;
                propiedadRelacionOrigenDestino.ValorPropiedad = pGadget.Contexto.RelacionOrigenDestino;
                pPropiedadesIntegracionContinua.Add(propiedadRelacionOrigenDestino);
                if (!pEsCarga)
                {
                    //pGadget.Contexto.RelacionOrigenDestino = UtilIntegracionContinua.ObtenerMascaraPropiedad(propiedadRelacionOrigenDestino);
                }
            }
            return pPropiedadesIntegracionContinua;
        }

        private ContextoModel.ContextModel CargarContexto(ProyectoGadgetContexto filaContexto)
        {
            ContextoModel.ContextModel contexto = new ContextoModel.ContextModel();

            contexto.ComunidadOrigen = filaContexto.ComunidadOrigen;
            contexto.FiltrosOrigen = filaContexto.ComunidadOrigenFiltros;
            contexto.RelacionOrigenDestino = filaContexto.FiltrosOrigenDestino;
            contexto.NumResultados = filaContexto.NumRecursos;
            contexto.OrdenResultados = filaContexto.OrdenContexto;
            contexto.Imagen = filaContexto.Imagen;
            contexto.MostrarEnlaceOriginal = filaContexto.MostrarEnlaceOriginal;
            contexto.MostrarVerMas = !filaContexto.OcultarVerMas;
            contexto.AbrirEnPestanyaNueva = filaContexto.NuevaPestanya;
            contexto.NamespacesExtra = filaContexto.NamespacesExtra;
            contexto.ResultadosExcluir = filaContexto.ResultadosEliminar;

            return contexto;
        }

        #endregion

        #region Metodos de Guardado
        public void GuardarGadget(ContextoModel pGadget)
        {
            List<Guid> listaGadgetsNuevos = new List<Guid>();

            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperProyecto proyDS = proyCN.ObtenerDataSetGadget(pGadget.Key, ProyectoSeleccionado.Clave);


            //Añadir los nuevos
            AniadirGadgetNuevo(listaGadgetsNuevos, pGadget, proyDS);

            //Modificar los que tienen cambios
            ModificarCambiosGadgetExistente(listaGadgetsNuevos, pGadget, proyDS);

            //Eliminar los eliminados
            EliminarGadget(listaGadgetsNuevos, pGadget, proyDS);

            //Actualizamos cambios
            proyCN.ActualizarProyectos();

        }

        public void GuardarGadgets(List<ContextoModel> pListaGadgetVista, Dictionary<string, Guid> pListaProyectosOrigen = null)
        {
            if (pListaProyectosOrigen != null && pListaProyectosOrigen.Count > 0)
            {
                ListaProyectosOrigen = pListaProyectosOrigen;
            }

            List<Guid> listaGadgetsNuevos = new List<Guid>();

            //Añadir los nuevos
            foreach (ContextoModel gadgetVista in pListaGadgetVista)
            {
                AniadirGadgetNuevo(listaGadgetsNuevos, gadgetVista, DataWrapperProyecto);
            }

            //Modificar los que tienen cambios
            foreach (ContextoModel gadgetVista in pListaGadgetVista)
            {
                ModificarCambiosGadgetExistente(listaGadgetsNuevos, gadgetVista, DataWrapperProyecto);
            }

            //Eliminar los eliminados
            foreach (ContextoModel gadgetVista in pListaGadgetVista)
            {
                EliminarGadget(listaGadgetsNuevos, gadgetVista, DataWrapperProyecto);
            }

            //Eliminar las que no se encuentran 
            List<ProyectoGadget> listaProyectoGadget = DataWrapperProyecto.ListaProyectoGadget.ToList();
            foreach (ProyectoGadget filaGadget in listaProyectoGadget)
            {
                if (mEntityContext.Entry(filaGadget).State != EntityState.Deleted && !pListaGadgetVista.Any(gadget => gadget.Key == filaGadget.GadgetID))
                {
                    EliminarGadget(filaGadget, DataWrapperProyecto);


                    mEntityContext.Entry(filaGadget).State = EntityState.Deleted;
                    DataWrapperProyecto.ListaProyectoGadget.Remove(filaGadget);
                }
            }

            using (ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication))
            {
                proyCN.ActualizarProyectos();
            }
        }

        private void EliminarGadget(List<Guid> listaGadgetsNuevos, ContextoModel gadget, DataWrapperProyecto pDataWrapperProyecto)
        {
            if (gadget.Deleted && !listaGadgetsNuevos.Contains(gadget.Key))
            {
                List<ProyectoGadget> filasGadgets = pDataWrapperProyecto.ListaProyectoGadget.Where(gadgetProy => gadgetProy.GadgetID.Equals(gadget.Key)).ToList();
                if (filasGadgets.Count > 0)
                {
                    EliminarGadget(filasGadgets.First(), pDataWrapperProyecto);
                    mEntityContext.EliminarElemento(filasGadgets.First());
                    pDataWrapperProyecto.ListaProyectoGadget.Remove(filasGadgets.First());
                    mEntityContext.SaveChanges();
                }
            }
        }

        private void ModificarCambiosGadgetExistente(List<Guid> listaGadgetsNuevos, ContextoModel gadgetVista, DataWrapperProyecto pDataWrapperProyecto)
        {
            if (!gadgetVista.Deleted && !listaGadgetsNuevos.Contains(gadgetVista.Key))
            {
                ProyectoGadget gadgetBD = pDataWrapperProyecto.ListaProyectoGadget.Where(gadgetProy => gadgetProy.GadgetID.Equals(gadgetVista.Key)).FirstOrDefault();
                if (gadgetBD != null)
                {
                    GuardarDatosFilaGadget(gadgetBD, gadgetVista);

                    if (gadgetVista.Contexto != null && gadgetVista.TipoGadget == TipoGadget.RecursosContextos)
                    {
                        ProyectoGadgetContexto filasContextos = pDataWrapperProyecto.ListaProyectoGadgetContexto.Where(gadgetProy => gadgetProy.GadgetID.Equals(gadgetVista.Key)).FirstOrDefault();
                        if (filasContextos != null)
                        {
                            GuardarDatosFilaContexto(filasContextos, gadgetVista.Contexto);
                        }
                    }
                }
            }
        }

        private void AniadirGadgetNuevo(List<Guid> listaGadgetsNuevos, ContextoModel gadget, DataWrapperProyecto pDataWrapperProyecto)
        {
            if (!gadget.Deleted)
            {
                ProyectoGadget proyectoGadget = pDataWrapperProyecto.ListaProyectoGadget.Where(gadgetProy => gadgetProy.GadgetID.Equals(gadget.Key)).FirstOrDefault();
                if (proyectoGadget == null)
                {
                    listaGadgetsNuevos.Add(gadget.Key);
                    AgregarGadgetNuevo(gadget, pDataWrapperProyecto);
                }
            }
        }

        public void CrearFilasPropiedadesIntegracionContinua(ContextoModel pGadget)
        {
            CrearFilasPropiedadesIntegracionContinua(new List<ContextoModel>() { pGadget });
        }


        public void CrearFilasPropiedadesIntegracionContinua(List<ContextoModel> pListaGadgets)
        {
            List<IntegracionContinuaPropiedad> propiedadesIntegracionUnGadget = new List<IntegracionContinuaPropiedad>();
            List<IntegracionContinuaPropiedad> todasLasPropiedadesIntegracionContinua = new List<IntegracionContinuaPropiedad>();
            try
            {
                foreach (ContextoModel gadget in pListaGadgets)
                {

                    if (gadget.TipoGadget == TipoGadget.RecursosContextos)
                    {
                        //Obtenemos las propiedades para este gadget
                        propiedadesIntegracionUnGadget = ObtenerPropiedadesIntegracionContinua(propiedadesIntegracionUnGadget, gadget);
                    }
                    //Agregamos a la lista con todos las propiedades para guardarlas a la vez en BD
                    todasLasPropiedadesIntegracionContinua.AddRange(propiedadesIntegracionUnGadget);

                }
                using (ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication))
                {
                    proyCN.CrearFilasIntegracionContinuaParametro(todasLasPropiedadesIntegracionContinua, ProyectoSeleccionado.Clave, TipoObjeto.Gadget);

                }
            }

            catch
            {
                //Deberia guardar errores en Log?
            }
        }

        public void ModificarFilasIntegracionContinuaEntornoSiguiente(List<ContextoModel> pListaGadgets, string UrlApiDesplieguesEntornoSiguiente, Guid pUsuarioID)
        {
            List<IntegracionContinuaPropiedad> propiedadesIntegracionUnGadget = new List<IntegracionContinuaPropiedad>();
            List<IntegracionContinuaPropiedad> todasLasPropiedadesIntegracionContinua = new List<IntegracionContinuaPropiedad>();
            try
            {
                foreach (ContextoModel gadget in pListaGadgets)
                {

                    if (gadget.TipoGadget == TipoGadget.RecursosContextos)
                    {
                        //Obtenemos las propiedades para este gadget
                        propiedadesIntegracionUnGadget = ObtenerPropiedadesIntegracionContinua(propiedadesIntegracionUnGadget, gadget);
                    }
                    //Agregamos a la lista con todos las propiedades para guardarlas a la vez en BD
                    todasLasPropiedadesIntegracionContinua.AddRange(propiedadesIntegracionUnGadget);

                }
            }
            catch
            {
                //Deberia guardar errores en Log?
            }

            try
            {
                string peticion = $"{UrlApiDesplieguesEntornoSiguiente}/PropiedadesIntegracion?nombreProy={ProyectoSeleccionado.NombreCorto}&UsuarioID={pUsuarioID}";
                string requestParameters = UtilWeb.WebRequestPostWithJsonObject(peticion, todasLasPropiedadesIntegracionContinua, "");
            }
            catch
            {

            }
        }

        private List<IntegracionContinuaPropiedad> ObtenerPropiedadesIntegracionContinua(List<IntegracionContinuaPropiedad> propiedadesIntegracionContinua, ContextoModel pGadget, bool pEsCarga = false)
        {


            if (!pGadget.Deleted)
            {
                if (pGadget.Contexto != null)
                {
                    propiedadesIntegracionContinua = AsignarValoresPropiedadesIntegracionContinua(propiedadesIntegracionContinua, pGadget, pEsCarga);
                }

                if (!string.IsNullOrEmpty(pGadget.FiltrosDestino))
                {
                    IntegracionContinuaPropiedad propiedadFiltrosDestino = new IntegracionContinuaPropiedad();
                    propiedadFiltrosDestino.ProyectoID = ProyectoSeleccionado.Clave;
                    propiedadFiltrosDestino.TipoObjeto = (short)TipoObjeto.Gadget;
                    propiedadFiltrosDestino.ObjetoPropiedad = pGadget.ShortName;
                    propiedadFiltrosDestino.TipoPropiedad = (short)TipoPropiedad.FiltrosDestinoGadget;
                    propiedadFiltrosDestino.ValorPropiedad = pGadget.FiltrosDestino;
                    propiedadesIntegracionContinua.Add(propiedadFiltrosDestino);
                    if (!pEsCarga)
                    {
                        // pGadget.FiltrosDestino = UtilIntegracionContinua.ObtenerMascaraPropiedad(propiedadFiltrosDestino);
                    }
                }
            }
            return propiedadesIntegracionContinua;
        }

        public string ComprobarErrores(ContextoModel pGadget)
        {
            string errores = string.Empty;
            errores = ComprobarErroresGadget(errores, pGadget);
            if (errores == "")
            {
                ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                DataWrapperProyecto dataWrapperProyecto = proyCN.ObtenerDataSetGadget(pGadget.Key, ProyectoSeleccionado.Clave);
                foreach (ProyectoGadgetContexto filaGadget in dataWrapperProyecto.ListaProyectoGadgetContexto)
                {
                    if (!ListaProyectosOrigen.ContainsKey(filaGadget.ComunidadOrigen))
                    {
                        ListaProyectosOrigen.Add(filaGadget.ComunidadOrigen, filaGadget.ProyectoOrigenID);
                    }
                }
                if (pGadget.TipoGadget == TipoGadget.RecursosContextos)
                {
                    errores = AsignarValorComunidadoOrigen(errores, pGadget);
                }
                if (errores == "")
                {
                    //Si han cambiado el nombre
                    if (dataWrapperProyecto.ListaProyectoGadget.Any() && string.Compare(dataWrapperProyecto.ListaProyectoGadget.First().NombreCorto, pGadget.ShortName) != 0)
                    {
                        //Comprobamos que no exista ya en BD
                        if (proyCN.ExisteGadgetContextoPorNombreCorto(pGadget.ShortName))
                        {
                            errores = "NOMBRECORTO REPETIDO|||" + pGadget.Key;
                        }
                    }
                }

            }

            return errores;
        }

        public string ComprobarErrores(List<ContextoModel> pListaGadgets)
        {
            List<string> listaNombresCortos = new List<string>();

            string errores = "";

            //Ponemos los textos nulos como vacios
            foreach (ContextoModel gadget in pListaGadgets)
            {
                errores = ComprobarErroresGadget(errores, gadget);
            }

            if (errores == "")
            {
                ListaProyectosOrigen.Clear();
                foreach (ProyectoGadgetContexto filaGadget in DataWrapperProyecto.ListaProyectoGadgetContexto)
                {
                    if (!ListaProyectosOrigen.ContainsKey(filaGadget.ComunidadOrigen))
                    {
                        ListaProyectosOrigen.Add(filaGadget.ComunidadOrigen, filaGadget.ProyectoOrigenID);
                    }
                }
                foreach (ContextoModel gadget in pListaGadgets)
                {
                    if (gadget.TipoGadget == TipoGadget.RecursosContextos)
                    {
                        errores = AsignarValorComunidadoOrigen(errores, gadget);
                    }
                }
            }
            return errores;
        }

        private string AsignarValorComunidadoOrigen(string errores, ContextoModel gadget)
        {
            ContextoModel.ContextModel contexto = gadget.Contexto;
            {
                if (!ListaProyectosOrigen.ContainsKey(contexto.ComunidadOrigen))
                {
                    try
                    {
                        string nombrecorto = contexto.ComunidadOrigen;
                        ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                        if (Uri.IsWellFormedUriString(contexto.ComunidadOrigen, UriKind.RelativeOrAbsolute) && nombrecorto.IndexOf('/') != -1)
                        {
                            nombrecorto = contexto.ComunidadOrigen.Substring(0, contexto.ComunidadOrigen.LastIndexOf("/"));
                            nombrecorto = nombrecorto.Substring(nombrecorto.LastIndexOf("/") + 1);                            
                        }

                        Guid proyectoOrigenID = proyCL.ObtenerProyectoIDPorNombreCorto(nombrecorto);                        
                        if(proyectoOrigenID.Equals(Guid.Empty))
                        {
                            errores = $"La comunidad origen {nombrecorto} no existe";
                        }
                        else
                        {
                            ListaProyectosOrigen.Add(contexto.ComunidadOrigen, proyectoOrigenID);
                        }                        
                    }
                    catch
                    {
                        errores = "COMUNIDADORIGEN|||" + gadget.Key.ToString();
                    }
                }
            }

            return errores;
        }

        private static string ComprobarErroresGadget(string errores, ContextoModel gadget)
        {
            if (string.IsNullOrEmpty(gadget.FiltrosDestino))
            {
                gadget.FiltrosDestino = string.Empty;
            }
            if (string.IsNullOrEmpty(gadget.Contenido))
            {
                gadget.Contenido = string.Empty;
            }
            if (string.IsNullOrEmpty(gadget.ShortName))
            {
                gadget.ShortName = string.Empty;
            }
            if (gadget.ShortName.Contains("#"))
            {
                errores = $"NOMBRECORTOGADGET CARACTERESINC|||{gadget.Key}";
            }
            if (gadget.ShortName.Contains(" "))
            {
                errores = $"NOMBRECORTOGADGET ESPACIOS|||{gadget.Key}";
            }
            if (gadget.TipoGadget == TipoGadget.RecursosContextos)
            {
                ContextoModel.ContextModel contexto = gadget.Contexto;

                if (string.IsNullOrEmpty(contexto.ComunidadOrigen))
                {
                    errores = $"COMUNIDADORIGEN VACIA|||{gadget.Key}";
                }
                if (string.IsNullOrEmpty(contexto.RelacionOrigenDestino))
                {
                    errores = $"RELACIONFILTROS VACIA|||{gadget.Key}";
                }
                if (string.IsNullOrEmpty(contexto.FiltrosOrigen))
                {
                    contexto.FiltrosOrigen = string.Empty;
                }
                if (string.IsNullOrEmpty(contexto.OrdenResultados))
                {
                    contexto.OrdenResultados = string.Empty;
                }
                if (string.IsNullOrEmpty(contexto.NamespacesExtra))
                {
                    contexto.NamespacesExtra = string.Empty;
                }
                if (string.IsNullOrEmpty(contexto.ResultadosExcluir))
                {
                    contexto.ResultadosExcluir = string.Empty;
                }
            }

            return errores;
        }
        private string ComprobarErrorNombresCortosRepetidos(List<ContextoModel> pListaGadgets)
        {
            List<string> listaNombresCortos = new List<string>();

            foreach (ContextoModel gadget in pListaGadgets)
            {
                if (!gadget.Deleted)
                {
                    string nombreCorto = gadget.ShortName;
                    if (string.IsNullOrEmpty(nombreCorto))
                    {
                        nombreCorto = gadget.Key.ToString();
                    }

                    if (!listaNombresCortos.Contains(nombreCorto))
                    {
                        listaNombresCortos.Add(nombreCorto);
                    }
                    else
                    {
                        return $"NOMBRECORTO REPETIDO|||{gadget.Key}";
                    }
                }
            }

            return string.Empty;
        }

        private void AgregarGadgetNuevo(ContextoModel pGadget, DataWrapperProyecto pDataWrapperProyecto)
        {
            //Agregar fila gadget
            ProyectoGadget filaNuevoGadget = new ProyectoGadget();
            filaNuevoGadget.GadgetID = pGadget.Key;
            filaNuevoGadget.ProyectoID = ProyectoSeleccionado.Clave;
            filaNuevoGadget.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
            filaNuevoGadget.Tipo = (short)pGadget.TipoGadget;
            filaNuevoGadget.Ubicacion = "10";
            filaNuevoGadget.TipoUbicacion = 1;
            filaNuevoGadget.MultiIdioma = false;
            filaNuevoGadget.Contenido = pGadget.Contenido;

            GuardarDatosFilaGadget(filaNuevoGadget, pGadget);

            if (!(mEntityContext.ProyectoGadget.Where(proyectoGadget => proyectoGadget.OrganizacionID.Equals(filaNuevoGadget.OrganizacionID) && proyectoGadget.ProyectoID.Equals(filaNuevoGadget.ProyectoID) && proyectoGadget.GadgetID.Equals(filaNuevoGadget.GadgetID)).ToList().Count > 0))
            {
                pDataWrapperProyecto.ListaProyectoGadget.Add(filaNuevoGadget);
                mEntityContext.ProyectoGadget.Add(filaNuevoGadget);
            }

            if (pGadget.Contexto != null && pGadget.TipoGadget == TipoGadget.RecursosContextos)
            {
                //Agregar fila gadget contexto
                ProyectoGadgetContexto filaNuevoContexto = new ProyectoGadgetContexto();
                filaNuevoContexto.GadgetID = pGadget.Key;
                filaNuevoContexto.ProyectoID = ProyectoSeleccionado.Clave;
                filaNuevoContexto.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                filaNuevoContexto.ServicioResultados = "";
                filaNuevoContexto.ObtenerPrivados = false;

                GuardarDatosFilaContexto(filaNuevoContexto, pGadget.Contexto);

                pDataWrapperProyecto.ListaProyectoGadgetContexto.Add(filaNuevoContexto);
                if (!(mEntityContext.ProyectoGadgetContexto.Where(proyectoGadget => proyectoGadget.OrganizacionID.Equals(filaNuevoGadget.OrganizacionID) && proyectoGadget.ProyectoID.Equals(filaNuevoGadget.ProyectoID) && proyectoGadget.GadgetID.Equals(filaNuevoGadget.GadgetID)).ToList().Count > 0))
                {
                    mEntityContext.ProyectoGadgetContexto.Add(filaNuevoContexto);
                }
            }
        }

        private void GuardarDatosFilaGadget(ProyectoGadget pGadgetBD, ContextoModel pGadgetVista)
        {
            pGadgetBD.Titulo = pGadgetVista.Name;
            pGadgetBD.Clases = pGadgetVista.Clases;
            pGadgetBD.Visible = pGadgetVista.Visible;
            pGadgetBD.Orden = pGadgetVista.Orden;
            pGadgetBD.MultiIdioma = false;
            pGadgetBD.CargarPorAjax = false;
            pGadgetBD.ComunidadDestinoFiltros = pGadgetVista.FiltrosDestino;

            if (string.IsNullOrEmpty(pGadgetVista.ShortName))
            {
                pGadgetBD.NombreCorto = pGadgetBD.GadgetID.ToString();
                pGadgetVista.ShortName = pGadgetBD.NombreCorto;
            }
            else
            {
                pGadgetBD.NombreCorto = pGadgetVista.ShortName;
            }
                        
            if (pGadgetVista.TipoGadget == TipoGadget.CMS)
            {
                pGadgetBD.CargarPorAjax = pGadgetVista.Ajax;
            }
            else if (pGadgetVista.TipoGadget == TipoGadget.RecursosContextos)
            {
                pGadgetBD.CargarPorAjax = true;
            }

            string contenido = string.Empty;
            if (!string.IsNullOrEmpty(pGadgetVista.Contenido) && (pGadgetVista.TipoGadget == TipoGadget.HtmlIncrustado || pGadgetVista.TipoGadget == TipoGadget.Consulta || pGadgetVista.TipoGadget == TipoGadget.CMS))
            {
                pGadgetVista.Contenido = HttpUtility.UrlDecode(pGadgetVista.Contenido);

                Dictionary<string, string> listaContenidoMultiIdioma = UtilCadenas.ObtenerTextoPorIdiomas(pGadgetVista.Contenido);
                if (pGadgetVista.TipoGadget == TipoGadget.Consulta || pGadgetVista.TipoGadget == TipoGadget.CMS || listaContenidoMultiIdioma.Count == 0)
                {
                    contenido = pGadgetVista.Contenido;
                }
                else
                {
                    ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    List<ProyectoGadgetIdioma> listaProyectoGadgetIdioma = proyectoCN.ObtenerProyectoGadgetIdiomaDeGadget(pGadgetBD.GadgetID);

                    if (listaContenidoMultiIdioma.Count == 1)
                    {
                        //Si solo hay un idioma no creo las filas en la tabla ProyectoGadgetIdioma, y elimino si habia alguna
                        contenido = listaContenidoMultiIdioma.First().Value;
                        foreach (ProyectoGadgetIdioma filaIdioma in listaProyectoGadgetIdioma)
                        {
                            pGadgetBD.ProyectoGadgetIdioma.Remove(filaIdioma);
                        }
                    }
                    else
                    {
                        pGadgetBD.MultiIdioma = true;
                        foreach (string idioma in listaContenidoMultiIdioma.Keys)
                        {
                            ProyectoGadgetIdioma proyectoGadgetIdioma = listaProyectoGadgetIdioma.Where(fila => fila.Idioma == idioma).FirstOrDefault();
                            if (proyectoGadgetIdioma == null)
                            {
                                proyectoGadgetIdioma = new ProyectoGadgetIdioma();
                                proyectoGadgetIdioma.GadgetID = pGadgetBD.GadgetID;
                                proyectoGadgetIdioma.OrganizacionID = pGadgetBD.OrganizacionID;
                                proyectoGadgetIdioma.ProyectoID = pGadgetBD.ProyectoID;
                                proyectoGadgetIdioma.Idioma = idioma;
                                proyectoGadgetIdioma.Contenido = listaContenidoMultiIdioma[idioma];
                                mEntityContext.ProyectoGadgetIdioma.Add(proyectoGadgetIdioma);
                                DataWrapperProyecto.ListaProyectoGadgetIdioma.Add(proyectoGadgetIdioma);
                            }
                            else if(proyectoGadgetIdioma != null)
                            {
                                proyectoGadgetIdioma.Contenido = listaContenidoMultiIdioma[idioma];
                            }
                        }
                    }
                }
            }

            if (pGadgetVista.TipoGadget == TipoGadget.CMS)
            {
                Guid testGuid = Guid.Empty;

                if (!Guid.TryParse(contenido, out testGuid))
                {
                    contenido = testGuid.ToString();
                }
            }

            pGadgetBD.Contenido = contenido;
        }

        private void GuardarDatosFilaContexto(ProyectoGadgetContexto pFilaContexto, ContextoModel.ContextModel pContexto)
        {
            pFilaContexto.ComunidadOrigen = pContexto.ComunidadOrigen;
            pFilaContexto.ComunidadOrigenFiltros = pContexto.FiltrosOrigen;
            pFilaContexto.FiltrosOrigenDestino = pContexto.RelacionOrigenDestino;
            pFilaContexto.NumRecursos = pContexto.NumResultados;
            pFilaContexto.OrdenContexto = pContexto.OrdenResultados;
            pFilaContexto.Imagen = pContexto.Imagen;
            pFilaContexto.MostrarEnlaceOriginal = pContexto.MostrarEnlaceOriginal;
            pFilaContexto.OcultarVerMas = !pContexto.MostrarVerMas;
            pFilaContexto.NuevaPestanya = pContexto.AbrirEnPestanyaNueva;
            pFilaContexto.NamespacesExtra = pContexto.NamespacesExtra;
            pFilaContexto.ResultadosEliminar = pContexto.ResultadosExcluir;

            if (ListaProyectosOrigen.ContainsKey(pContexto.ComunidadOrigen))
            {
                pFilaContexto.ProyectoOrigenID = ListaProyectosOrigen[pContexto.ComunidadOrigen];
            }
        }

        private void EliminarGadget(ProyectoGadget pFilaGadget, DataWrapperProyecto pDataWrapperProyecto)
        {
            List<ProyectoGadgetIdioma> filasGadgetIdioma = pFilaGadget.ProyectoGadgetIdioma.ToList();
            if (filasGadgetIdioma.Any())
            {
                filasGadgetIdioma = pDataWrapperProyecto.ListaProyectoGadgetIdioma.Where(x => x.GadgetID.Equals(pFilaGadget.GadgetID)).ToList();
            }

            foreach (ProyectoGadgetIdioma filaGadgetIdioma in filasGadgetIdioma)
            {
                mEntityContext.EliminarElemento(filaGadgetIdioma);
                pDataWrapperProyecto.ListaProyectoGadgetIdioma.Remove(filaGadgetIdioma);
                pDataWrapperProyecto.ListaProyectoGadgetIdioma.Remove(filaGadgetIdioma);
            }
            ProyectoGadgetContexto filaGadgetContexto = pFilaGadget.ProyectoGadgetContexto;

            if (filaGadgetContexto == null)
            {
                filaGadgetContexto = pDataWrapperProyecto.ListaProyectoGadgetContexto.Where(x => x.GadgetID.Equals(pFilaGadget.GadgetID)).FirstOrDefault();
            }

            if (filaGadgetContexto != null)
            {
                mEntityContext.EliminarElemento(filaGadgetContexto);
                pDataWrapperProyecto.ListaProyectoGadgetContexto.Remove(filaGadgetContexto);
                pDataWrapperProyecto.ListaProyectoGadgetContexto.Remove(filaGadgetContexto);
            }
            mEntityContext.SaveChanges();
        }

        #endregion

        #region Invalidar caches

        public void InvalidarCaches()
        {
            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            proyCL.InvalidarGadgetsProyecto(ProyectoSeleccionado.Clave);
            proyCL.Dispose();
        }

        #endregion

        #region Propiedades

        private DataWrapperProyecto DataWrapperProyecto
        {
            get
            {
                if (mDataWrapperProyecto == null)
                {
                    mDataWrapperProyecto = new DataWrapperProyecto();
                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    proyCN.ObtenerGadgetsProyecto(ProyectoSeleccionado.Clave, mDataWrapperProyecto);
                }
                return mDataWrapperProyecto;
            }
        }
        private Dictionary<string, Guid> ListaProyectosOrigen
        {
            get
            {
                if (mListaProyectosOrigen == null)
                {
                    mListaProyectosOrigen = new Dictionary<string, Guid>();
                }
                return mListaProyectosOrigen;
            }
            set
            {
                mListaProyectosOrigen = value;
            }
        }

        #endregion

    }
}
