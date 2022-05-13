using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.CMS;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.CMS;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.CMS;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Es.Riam.Gnoss.Web.Controles.Administracion
{
    public class ControladorPaginasCMS
    {
        private Proyecto ProyectoSeleccionado = null;
        private GestionCMS mGestorCMS = null;
        private short mTipoUbicacionCMSPaginaActual;
        private LoggingService mLoggingService;
        private VirtuosoAD mVirtuosoAD;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private RedisCacheWrapper mRedisCacheWrapper;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        public ControladorPaginasCMS(Proyecto pProyecto, LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, VirtuosoAD virtuosoAD, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
        {
            mVirtuosoAD = virtuosoAD;
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mRedisCacheWrapper = redisCacheWrapper;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
            ProyectoSeleccionado = pProyecto;
        }

        #endregion

        #region Métodos de carga

        public AdministrarPaginasCMSViewModel CargarPaginaPublicada(short pTipoUbicacionCMSPaginaActual)
        {
            mTipoUbicacionCMSPaginaActual = pTipoUbicacionCMSPaginaActual;
            mGestorCMS = null;

            if (GestorCMSPaginaActual.ListaPaginasProyectos.ContainsKey(ProyectoSeleccionado.Clave) && GestorCMSPaginaActual.ListaPaginasProyectos[ProyectoSeleccionado.Clave].ContainsKey(pTipoUbicacionCMSPaginaActual))
            {
                AdministrarPaginasCMSViewModel paginaCMSModel = new AdministrarPaginasCMSViewModel();

                paginaCMSModel.Type = pTipoUbicacionCMSPaginaActual;

                paginaCMSModel.Rows = CargarFilasPagina();

                paginaCMSModel.MostrarSoloCuerpo = GestorCMSPaginaActual.ListaPaginasProyectos[ProyectoSeleccionado.Clave][pTipoUbicacionCMSPaginaActual].MostrarSoloCuerpo;

                return paginaCMSModel;
            }
            return null;
        }

        public AdministrarPaginasCMSViewModel CargarPagina(short pTipoUbicacionCMSPaginaActual, GestionCMS pGestorCMS)
        {
            mTipoUbicacionCMSPaginaActual = pTipoUbicacionCMSPaginaActual;
            mGestorCMS = pGestorCMS;

            if (GestorCMSPaginaActual.ListaPaginasProyectos.ContainsKey(ProyectoSeleccionado.Clave) && GestorCMSPaginaActual.ListaPaginasProyectos[ProyectoSeleccionado.Clave].ContainsKey(pTipoUbicacionCMSPaginaActual))
            {
                AdministrarPaginasCMSViewModel paginaCMSModel = new AdministrarPaginasCMSViewModel();

                paginaCMSModel.Type = pTipoUbicacionCMSPaginaActual;

                paginaCMSModel.Rows = CargarFilasPagina();

                paginaCMSModel.MostrarSoloCuerpo = GestorCMSPaginaActual.ListaPaginasProyectos[ProyectoSeleccionado.Clave][pTipoUbicacionCMSPaginaActual].MostrarSoloCuerpo;

                return paginaCMSModel;
            }
            return null;
        }


        private List<AdministrarPaginasCMSViewModel.RowCMSModel> CargarFilasPagina()
        {
            List<AdministrarPaginasCMSViewModel.RowCMSModel> filasPagina = new List<AdministrarPaginasCMSViewModel.RowCMSModel>();

            if (GestorCMSPaginaActual.ListaBloques.Count > 0)
            {
                bool cargarborrador = true;

                if(GestorCMSPaginaActual.ListaBloques.Count(bloque2 => bloque2.Value.Borrador) == 0)
                {
                    cargarborrador = false;
                }

                foreach (var bloque in GestorCMSPaginaActual.ListaBloques.Where(bloque2 => bloque2.Value.Borrador == cargarborrador && !bloque2.Value.BloquePadreID.HasValue).OrderBy(bloque2 => bloque2.Value.Orden))
                {
                    AdministrarPaginasCMSViewModel.RowCMSModel fila = new AdministrarPaginasCMSViewModel.RowCMSModel();
                    fila.Attributes = bloque.Value.FilaBloque.Estilos;
                    fila.Key = bloque.Key;

                    fila.Cols = CargarColumnasFila(bloque.Value);

                    filasPagina.Add(fila);
                }
            }
            return filasPagina;
        }

        private List<AdministrarPaginasCMSViewModel.RowCMSModel.ColCMSModel> CargarColumnasFila(CMSBloque pFilaPagina)
        {
            List<AdministrarPaginasCMSViewModel.RowCMSModel.ColCMSModel> columnasFila = new List<AdministrarPaginasCMSViewModel.RowCMSModel.ColCMSModel>();

            bool cargarborrador = true;

            if (GestorCMSPaginaActual.ListaBloques.Count(bloque2 => bloque2.Value.Borrador) == 0)
            {
                cargarborrador = false;
            }

            foreach (var bloque in GestorCMSPaginaActual.ListaBloques.Where(bloque2 => bloque2.Value.Borrador == cargarborrador && bloque2.Value.BloquePadreID.HasValue && bloque2.Value.BloquePadreID == pFilaPagina.Clave).OrderBy(bloque2 => bloque2.Value.Orden))
            {
                AdministrarPaginasCMSViewModel.RowCMSModel.ColCMSModel columna = new AdministrarPaginasCMSViewModel.RowCMSModel.ColCMSModel();
                columna.Key = bloque.Key;
                columna.Class = bloque.Value.FilaBloque.Estilos;

                columna.Components = CargarComponentesColumna(bloque.Value);

                columnasFila.Add(columna);
            }

            return columnasFila;
        }

        private List<AdministrarPaginasCMSViewModel.RowCMSModel.ColCMSModel.ComponentCMSModel> CargarComponentesColumna(CMSBloque pColumnaPagina)
        {
            List<AdministrarPaginasCMSViewModel.RowCMSModel.ColCMSModel.ComponentCMSModel> componentesColumna = new List<AdministrarPaginasCMSViewModel.RowCMSModel.ColCMSModel.ComponentCMSModel>();

            foreach (var componente in pColumnaPagina.Componentes.Values)
            {
                AdministrarPaginasCMSViewModel.RowCMSModel.ColCMSModel.ComponentCMSModel componenteColumna = new AdministrarPaginasCMSViewModel.RowCMSModel.ColCMSModel.ComponentCMSModel();
                componenteColumna.Key = componente.Clave;
                componenteColumna.Name = componente.Nombre;
                componenteColumna.Type = componente.TipoComponenteCMS.ToString();

                if (componente.TipoComponenteCMS.Equals(TipoComponenteCMS.Menu))
                {
                    string valorSeleccionado = "";
                    if (pColumnaPagina.PropiedadesComponentesBloque.ContainsKey(componente.Clave) && pColumnaPagina.PropiedadesComponentesBloque[componente.Clave].ContainsKey(TipoPropiedadCMS.ValorSeleccionado))
                    {
                        valorSeleccionado = pColumnaPagina.PropiedadesComponentesBloque[componente.Clave][TipoPropiedadCMS.ValorSeleccionado];
                    }

                    componenteColumna.Options = new Dictionary<short, KeyValuePair<string, bool>>();

                    foreach (short opcion in ((CMSComponenteMenu)componente).ListaOpcionesMenu.Keys)
                    {
                        string nombreOpcion = ((CMSComponenteMenu)componente).ListaOpcionesMenu[opcion].Value[TipoPropiedadMenu.Nombre];
                        componenteColumna.Options.Add(opcion, new KeyValuePair<string, bool>(nombreOpcion, opcion.ToString() == valorSeleccionado));
                    }
                }

                componentesColumna.Add(componenteColumna);
            }

            return componentesColumna;
        }

        #endregion

        #region Métodos de Guardado

        public void GuardarListaPaginas(Dictionary<Guid, AdministrarPaginasCMSViewModel> pListaPaginasCMS, AdministrarPaginasCMSViewModel pPaginaHomeMiembros, AdministrarPaginasCMSViewModel pPaginaHomeNoMiembros, AdministrarPaginasCMSViewModel pPaginaHomeTodos)
        {
            CMSCN cmsCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            mGestorCMS = new GestionCMS(cmsCN.ObtenerCMSDeProyecto(ProyectoSeleccionado.Clave), mLoggingService, mEntityContext);

            if (pPaginaHomeMiembros != null)
            {
                Guardar(0, pPaginaHomeMiembros);
            }
            if (pPaginaHomeNoMiembros != null)
            {
                Guardar(1, pPaginaHomeNoMiembros);
            }
            if (pPaginaHomeTodos != null)
            {
                Guardar(2, pPaginaHomeTodos);
            }

            foreach (Guid pestanyaID in pListaPaginasCMS.Keys)
            {
                try
                {
                    Guardar(pestanyaID, pListaPaginasCMS[pestanyaID]);
                }
                catch(Exception ex)
                {
                }
            }

            //cmsCN.ActualizarCMSEliminandoBloquesDePaginaDeProyecto(GestorCMSPaginaActual.CMSDS, ProyectoSeleccionado.Clave, mTipoUbicacionCMSPaginaActual, pBorrador);

            cmsCN.ActualizarCMS(GestorCMSPaginaActual.CMSDW);
            cmsCN.Dispose();
        }
        private void Guardar(Guid pestanyaID, AdministrarPaginasCMSViewModel pPaginaCMSModel)
        {
            var filaPestanya = ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoPestanyaCMS.FirstOrDefault(pest => pest.PestanyaID == pestanyaID);
            if(filaPestanya != null)
            {
                mTipoUbicacionCMSPaginaActual = filaPestanya.Ubicacion;
            }
            Guardar(mTipoUbicacionCMSPaginaActual, pPaginaCMSModel);
        }
        public void ActualizarCMS()
        {
            using (CMSCN cmsCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication))
            {
                cmsCN.ActualizarCMS(GestorCMSPaginaActual.CMSDW);
            }
        }
        public void Guardar(short pUbicacion, AdministrarPaginasCMSViewModel pPaginaCMSModel)
        { 
            mTipoUbicacionCMSPaginaActual = pUbicacion;
            
            List<Guid> listaBloques = new List<Guid>();
            //Se hace para insertar tanto los que estan con borrador 0 como los que estan con borrador 1, en este caso es para las que estan con borrador 1
            Guid guidBorradorTrueFila;
            Guid guidBorradorTrueColumna;

            short ordenFila = 0;
            foreach (AdministrarPaginasCMSViewModel.RowCMSModel fila in pPaginaCMSModel.Rows)
            {
                CMSBloque bloqueFila = null;
                listaBloques.Add(fila.Key);
                guidBorradorTrueFila = Guid.NewGuid();
                listaBloques.Add(guidBorradorTrueFila);

                if (GestorCMSPaginaActual.ListaBloques.ContainsKey(fila.Key))
                {
                    bloqueFila = GestorCMSPaginaActual.ListaBloques[fila.Key];
                    bloqueFila.FilaBloque.Ubicacion = mTipoUbicacionCMSPaginaActual;
                    bloqueFila.FilaBloque.BloquePadreID = null;

                    bloqueFila.FilaBloque.Orden = ordenFila;
                    bloqueFila.FilaBloque.Estilos = fila.Attributes;
                    bloqueFila.FilaBloque.Borrador = false;
                }
                else
                {
                    bloqueFila = GestorCMSPaginaActual.AgregarNuevoBloque(fila.Key, GestorCMSPaginaActual.ListaPaginasProyectos[ProyectoSeleccionado.Clave][(short)mTipoUbicacionCMSPaginaActual], null, ordenFila, fila.Attributes, false);
                }

                //Para insertar los borradores
                GestorCMSPaginaActual.AgregarNuevoBloque(guidBorradorTrueFila, GestorCMSPaginaActual.ListaPaginasProyectos[ProyectoSeleccionado.Clave][(short)mTipoUbicacionCMSPaginaActual], null, ordenFila, fila.Attributes, true);

                ordenFila++;

                short ordenColumna = 0;
                foreach (AdministrarPaginasCMSViewModel.RowCMSModel.ColCMSModel columna in fila.Cols)
                {
                    CMSBloque nuevoBloque = null;

                    listaBloques.Add(columna.Key);
                    guidBorradorTrueColumna = Guid.NewGuid();
                    listaBloques.Add(guidBorradorTrueColumna);

                    if (GestorCMSPaginaActual.ListaBloques.ContainsKey(columna.Key))
                    {
                        nuevoBloque = GestorCMSPaginaActual.ListaBloques[columna.Key];
                        nuevoBloque.FilaBloque.Ubicacion = mTipoUbicacionCMSPaginaActual;
                        nuevoBloque.FilaBloque.BloquePadreID = fila.Key;

                        nuevoBloque.FilaBloque.Orden = ordenColumna;
                        nuevoBloque.FilaBloque.Estilos = columna.Class;
                        nuevoBloque.FilaBloque.Borrador = false;

                    }
                    else
                    {
                        nuevoBloque = GestorCMSPaginaActual.AgregarNuevoBloque(columna.Key, GestorCMSPaginaActual.ListaPaginasProyectos[ProyectoSeleccionado.Clave][(short)mTipoUbicacionCMSPaginaActual], fila.Key, ordenColumna, columna.Class, false);                        
                    }

                    //Insertar los borradores
                    GestorCMSPaginaActual.AgregarNuevoBloque(guidBorradorTrueColumna, GestorCMSPaginaActual.ListaPaginasProyectos[ProyectoSeleccionado.Clave][(short)mTipoUbicacionCMSPaginaActual], guidBorradorTrueFila, ordenColumna, columna.Class, true);

                    ordenColumna++;

                    List<Guid> listaComponentes = new List<Guid>();

                    foreach (AdministrarPaginasCMSViewModel.RowCMSModel.ColCMSModel.ComponentCMSModel componente in columna.Components)
                    {
                        listaComponentes.Add(componente.Key);

                        CMSComponente nuevoComponente = nuevoBloque.Componentes.Values.FirstOrDefault(comp => comp.Clave == componente.Key);

                        if(nuevoComponente == null)
                        {
                            AD.EntityModel.Models.CMS.CMSBloque cmsBloqueBD = GestorCMSPaginaActual.CMSDW.ListaCMSBloque.Where(x => x.BloqueID.Equals(columna.Key)).FirstOrDefault();
                            GestorCMSPaginaActual.AgregarComponenteABloque(ProyectoSeleccionado, columna.Key, componente.Key, cmsBloqueBD);
                        }
                        else
                        {
                            List<AD.EntityModel.Models.CMS.CMSBloqueComponentePropiedadComponente> listaAuxiliar = GestorCMSPaginaActual.CMSDW.ListaCMSBloqueComponentePropiedadComponente.Where(item => item.ComponenteID.Equals(componente.Key) && item.BloqueID.Equals(columna.Key)).ToList();
                            foreach (AD.EntityModel.Models.CMS.CMSBloqueComponentePropiedadComponente filaPropiedad in listaAuxiliar)
                            {
                                GestorCMSPaginaActual.CMSDW.ListaCMSBloqueComponentePropiedadComponente.Remove(filaPropiedad);
                                mEntityContext.EliminarElemento(filaPropiedad);
                            }
                            var bloqueComponenteNoBorrador = nuevoComponente.FilaComponente.CMSBloqueComponente.FirstOrDefault(item => item.CMSBloque != null && item.CMSBloque.Borrador == false);
                            if (bloqueComponenteNoBorrador != null)
                            {
                                bloqueComponenteNoBorrador.Orden = (short)(listaComponentes.Count - 1);
                            }

                        }

                        if (componente.Options != null)
                        {
                            foreach (short key in componente.Options.Keys)
                            {
                                if (componente.Options[key].Value)
                                {
                                    AD.EntityModel.Models.CMS.CMSBloqueComponentePropiedadComponente bloqueComponente = new AD.EntityModel.Models.CMS.CMSBloqueComponentePropiedadComponente();
                                    bloqueComponente.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                                    bloqueComponente.ProyectoID = ProyectoSeleccionado.FilaProyecto.ProyectoID;
                                    bloqueComponente.TipoPropiedadComponente = (short)TipoPropiedadCMS.ValorSeleccionado;
                                    bloqueComponente.ValorPropiedad = key.ToString();
                                    bloqueComponente.ComponenteID = componente.Key;
                                    bloqueComponente.BloqueID = columna.Key;

                                    GestorCMSPaginaActual.CMSDW.ListaCMSBloqueComponentePropiedadComponente.Add(bloqueComponente);
                                    mEntityContext.CMSBloqueComponentePropiedadComponente.Add(bloqueComponente);

                                    AD.EntityModel.Models.CMS.CMSBloqueComponentePropiedadComponente bloqueComponenteBorrador = new AD.EntityModel.Models.CMS.CMSBloqueComponentePropiedadComponente();
                                    bloqueComponenteBorrador.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                                    bloqueComponenteBorrador.ProyectoID = ProyectoSeleccionado.FilaProyecto.ProyectoID;
                                    bloqueComponenteBorrador.TipoPropiedadComponente = (short)TipoPropiedadCMS.ValorSeleccionado;
                                    bloqueComponenteBorrador.ValorPropiedad = key.ToString();
                                    bloqueComponenteBorrador.ComponenteID = componente.Key;
                                    bloqueComponenteBorrador.BloqueID = guidBorradorTrueColumna;

                                    GestorCMSPaginaActual.CMSDW.ListaCMSBloqueComponentePropiedadComponente.Add(bloqueComponenteBorrador);
                                    mEntityContext.CMSBloqueComponentePropiedadComponente.Add(bloqueComponenteBorrador);
                                }
                            }
                        }
                        AD.EntityModel.Models.CMS.CMSBloque cmsBloqueBDBorrador = GestorCMSPaginaActual.CMSDW.ListaCMSBloque.Where(x => x.BloqueID.Equals(guidBorradorTrueColumna)).FirstOrDefault();
                        GestorCMSPaginaActual.AgregarComponenteABloque(ProyectoSeleccionado, guidBorradorTrueColumna, componente.Key, cmsBloqueBDBorrador);
                    }

                    foreach (CMSComponente componente in nuevoBloque.Componentes.Values)
                    {
                        //Borrar los componentes que ya no esten
                        if (!listaComponentes.Contains(componente.Clave))
                        {
                            GestorCMSPaginaActual.EliminarComponente(componente.Clave);
                        }
                    }
                }
            }

            foreach (CMSBloque bloqueID in GestorCMSPaginaActual.ListaBloques.Values.Where(bloque => /*!EntityContext.Entry(bloque.FilaBloque).State.Equals(DataRowState.Deleted) && */bloque.TipoUbicacion == mTipoUbicacionCMSPaginaActual).ToList())
            {                
                //Borrar los bloques que ya no esten
                if (!listaBloques.Contains(bloqueID.Clave))
                {
                    GestorCMSPaginaActual.EliminarBloque(bloqueID.Clave);
                }
            }
            

            GestorCMSPaginaActual.CargarBloques();
            //Descartar();

            CMSPagina pagina = GestorCMSPaginaActual.ListaPaginasProyectos[ProyectoSeleccionado.Clave][mTipoUbicacionCMSPaginaActual];
            pagina.MostrarSoloCuerpo = pPaginaCMSModel.MostrarSoloCuerpo;
        }

        #endregion

        #region Métodos de Guardado Web

        public void Descartar()
        {
            GestionCMS gestorCMSDescartar = new GestionCMS(new DataWrapperCMS(), mLoggingService, mEntityContext);

            CMSCN cmsCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            Dictionary<Guid, Guid> listaGuidViejoGuidNuevo = new Dictionary<Guid, Guid>();
            foreach (CMSBloque bloqueCMS in GestorCMSPaginaActual.ListaBloques.Values)
            {
                if (!bloqueCMS.Borrador)
                {
                    listaGuidViejoGuidNuevo.Add(bloqueCMS.Clave, Guid.NewGuid());
                }
            }

            foreach (CMSBloque bloqueCMS in GestorCMSPaginaActual.ListaBloques.Values)
            {
                if (!bloqueCMS.Borrador)
                {
                    Guid? bloquePadreID = null;
                    if (bloqueCMS.BloquePadreID.HasValue)
                    {
                        bloquePadreID = listaGuidViejoGuidNuevo[bloqueCMS.BloquePadreID.Value];
                    }
                    gestorCMSDescartar.AgregarNuevoBloque(listaGuidViejoGuidNuevo[bloqueCMS.Clave], GestorCMSPaginaActual.ListaPaginasProyectos[ProyectoSeleccionado.Clave][(short)mTipoUbicacionCMSPaginaActual], bloquePadreID, bloqueCMS.Orden, bloqueCMS.FilaBloque.Estilos, true);

                    foreach (CMSComponente componente in bloqueCMS.Componentes.Values)
                    {
                        gestorCMSDescartar.AgregarComponenteABloque(ProyectoSeleccionado, listaGuidViejoGuidNuevo[bloqueCMS.Clave], componente.Clave);
                    }
                }
            }

            cmsCN.ActualizarCMSEliminandoBloquesDePaginaDeProyecto(gestorCMSDescartar.CMSDW, ProyectoSeleccionado.Clave, mTipoUbicacionCMSPaginaActual, true);
            cmsCN.Dispose();

            CMSCL cmsCL = new CMSCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            cmsCL.InvalidarCacheConfiguracionCMSPorProyecto(ProyectoSeleccionado.Clave);
            cmsCL.InvalidarCacheCMSDeUbicacionDeProyecto(mTipoUbicacionCMSPaginaActual, ProyectoSeleccionado.Clave);
            cmsCL.Dispose();

        }

        public void GuardarWeb(short pTipoUbicacionCMSPaginaActual, string estructura, string propiedadComponente, bool MostrarSoloCuerpo, bool borrador)
        {
            mTipoUbicacionCMSPaginaActual = pTipoUbicacionCMSPaginaActual;
            mGestorCMS = null;
            
            List<Bloque> listaBloquesPadres = new List<Bloque>();

            // Leemos el string y creamos los bloques padres con sus hijos
            string[] bloquesPadre = estructura.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string bloquePadre in bloquesPadre)
            {
                listaBloquesPadres.Add(new Bloque(bloquePadre));
            }

            short i = 0;

            GestionCMS gestorCMS = new GestionCMS(new DataWrapperCMS(), mLoggingService, mEntityContext);

            List<Tuple<Guid, Guid, short, string>> listaBloqueComponentePropiedadValor = new List<Tuple<Guid, Guid, short, string>>();
            if (!string.IsNullOrEmpty(propiedadComponente))
            {
                string[] propiedades = propiedadComponente.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string propiedad in propiedades)
                {
                    string[] valor = propiedad.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    listaBloqueComponentePropiedadValor.Add(new Tuple<Guid, Guid, short, string>(new Guid(valor[0]), new Guid(valor[1]), short.Parse(valor[2]), valor[3]));
                }
            }

            foreach (Bloque bloque in listaBloquesPadres)
            {
                AgregarBloqueAGestorCMS(bloque, gestorCMS, i, null, true, false, listaBloqueComponentePropiedadValor);
                i++;
            }

            if (!borrador)
            {
                foreach (Bloque bloque in listaBloquesPadres)
                {
                    AgregarBloqueAGestorCMS(bloque, gestorCMS, i, null, false, true, listaBloqueComponentePropiedadValor);
                    i++;
                }
            }

            CMSCN cmsCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            cmsCN.ActualizarCMSEliminandoBloquesDePaginaDeProyecto(gestorCMS.CMSDW, ProyectoSeleccionado.Clave, mTipoUbicacionCMSPaginaActual, borrador);

            CMSPagina pagina = GestorCMSPaginaActual.ListaPaginasProyectos[ProyectoSeleccionado.Clave][mTipoUbicacionCMSPaginaActual];
            pagina.MostrarSoloCuerpo = MostrarSoloCuerpo;
            cmsCN.ActualizarCMS(GestorCMSPaginaActual.CMSDW);

            cmsCN.Dispose();
        }

        private void AgregarBloqueAGestorCMS(Bloque pBloque, GestionCMS pGestorCMS, short pOrden, Guid? pBloquePadreID, bool pBorrador, bool pGenerarNuevosID, List<Tuple<Guid, Guid, short, string>> pListaBloqueComponentePropiedadValor)
        {
            Guid id = pBloque.BloqueID;

            if (pGenerarNuevosID)
            {
                id = Guid.NewGuid();
            }

            CMSBloque nuevoBloque;
            nuevoBloque = pGestorCMS.AgregarNuevoBloque(id, GestorCMSPaginaActual.ListaPaginasProyectos[ProyectoSeleccionado.Clave][(short)mTipoUbicacionCMSPaginaActual], pBloquePadreID, pOrden, pBloque.Clases, pBorrador);

            foreach (Guid componenteID in pBloque.ComponentesID)
            {
                pGestorCMS.AgregarComponenteABloque(ProyectoSeleccionado, id, componenteID, nuevoBloque.FilaBloque);
                foreach (Tuple<Guid, Guid, short, string> bloqueComponentePropiedadValor in pListaBloqueComponentePropiedadValor)
                {
                    if (bloqueComponentePropiedadValor.Item1 == pBloque.BloqueID && bloqueComponentePropiedadValor.Item2 == componenteID)
                    {
                        AD.EntityModel.Models.CMS.CMSBloqueComponentePropiedadComponente bloqueComponentePropiedadComponente = new AD.EntityModel.Models.CMS.CMSBloqueComponentePropiedadComponente();
                        bloqueComponentePropiedadComponente.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                        bloqueComponentePropiedadComponente.ProyectoID = ProyectoSeleccionado.FilaProyecto.ProyectoID;
                        bloqueComponentePropiedadComponente.BloqueID = id;
                        bloqueComponentePropiedadComponente.ComponenteID = bloqueComponentePropiedadValor.Item2;
                        bloqueComponentePropiedadComponente.TipoPropiedadComponente = (short)bloqueComponentePropiedadValor.Item3;
                        bloqueComponentePropiedadComponente.ValorPropiedad = bloqueComponentePropiedadValor.Item4;
                        bloqueComponentePropiedadComponente.CMSBloqueComponente = pGestorCMS.CMSDW.ListaCMSBloqueComponente.First(cmsBloqueComp => cmsBloqueComp.ComponenteID.Equals(bloqueComponentePropiedadComponente.ComponenteID) && cmsBloqueComp.BloqueID.Equals(id));
                        pGestorCMS.CMSDW.ListaCMSBloqueComponentePropiedadComponente.Add(bloqueComponentePropiedadComponente);
                        mEntityContext.CMSBloqueComponentePropiedadComponente.Add(bloqueComponentePropiedadComponente);
                    }
                }
            }

            short i = 0;
            foreach (Bloque bloque in pBloque.Hijos)
            {
                AgregarBloqueAGestorCMS(bloque, pGestorCMS, i, nuevoBloque.Clave, pBorrador, pGenerarNuevosID, pListaBloqueComponentePropiedadValor);
                i++;
            }
        }

        #endregion

        public void InvalidarCache(short pTipoUbicacionCMSPaginaActual, bool borrador)
        {
            CMSCL cmsCL = new CMSCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            cmsCL.InvalidarCacheConfiguracionCMSPorProyecto(ProyectoSeleccionado.Clave);
            cmsCL.InvalidarCacheCMSDeUbicacionDeProyecto(pTipoUbicacionCMSPaginaActual, ProyectoSeleccionado.Clave);
            cmsCL.Dispose();

            if (!borrador)
            {
                ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                proyCL.InvalidarFilaProyecto(ProyectoSeleccionado.Clave);
                proyCL.InvalidarPestanyasProyecto(ProyectoSeleccionado.Clave);
            }
        }

        #region Propiedades

        /// <summary>
        /// 
        /// </summary>
        private GestionCMS GestorCMSPaginaActual
        {
            get
            {
                if (mGestorCMS == null)
                {
                    CMSCN cmsCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    mGestorCMS = new GestionCMS(cmsCN.ObtenerCMSDeUbicacionDeProyecto(mTipoUbicacionCMSPaginaActual, ProyectoSeleccionado.Clave, 2, false), mLoggingService, mEntityContext);
                    cmsCN.Dispose();
                }
                return mGestorCMS;
            }
        }

        #endregion

        #region Clases auxiliares
        public class Bloque
        {
            private Guid mBloqueID;
            private List<Bloque> mHijos;
            private List<Guid> mComponentesID = new List<Guid>();
            private string mClases;

            #region Constructor

            public Bloque(string pEstructura)
            {
                string[] infoBloque = pEstructura.Substring(0, pEstructura.IndexOf(":[")).Split('_');
                mClases = infoBloque[1].Replace("@#*", "_");
                mBloqueID = new Guid(infoBloque[2]);


                List<string> hijos = obtenerHijos(pEstructura);


                mHijos = new List<Bloque>();
                foreach (string hijo in hijos)
                {
                    if (hijo.StartsWith("bloque"))
                    {
                        mHijos.Add(new Bloque(hijo));
                    }
                    else
                    {
                        string componenteID = hijo.Replace("componente_", "").Replace(":[]", "");
                        if (componenteID != "undefined")
                        {
                            mComponentesID.Add(new Guid(componenteID));
                        }
                    }
                }
            }

            #endregion

            public List<string> obtenerHijos(string pTexto)
            {
                List<string> listado = new List<string>();

                string aux = "";
                int nivel = 0;
                for (int i = 0; i < pTexto.Length; i++)
                {
                    string letra = pTexto.Substring(i, 1);

                    if (nivel == 1 && (letra == "," || (pTexto.Length > 1 && i == pTexto.Length - 1)))
                    {
                        if (!string.IsNullOrEmpty(aux))
                        {
                            listado.Add(aux);
                            aux = "";
                        }
                    }

                    if (nivel > 0)
                    {
                        if (!string.IsNullOrEmpty(aux) || letra != ",")
                        {
                            aux += letra;
                        }
                    }

                    if (letra == "[")
                    {
                        nivel++;
                    }

                    if (letra == "]")
                    {
                        nivel--;
                    }
                }
                return listado;
            }

            #region Propiedades

            public Guid BloqueID
            {
                get
                {
                    return mBloqueID;
                }
            }

            public List<Bloque> Hijos
            {
                get
                {
                    return mHijos;
                }
            }

            public List<Guid> ComponentesID
            {
                get
                {
                    return mComponentesID;
                }
            }

            public string Clases
            {
                get
                {
                    return mClases;
                }
            }
            #endregion
        }

        public void BorrarPaginaCMS(short tipoUbicacion)
        {
            using (CMSCN cmscn = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication))
            {
                cmscn.BorrarPaginaCMS(ProyectoAD.MyGnoss, ProyectoSeleccionado.Clave, tipoUbicacion);
            }
        }
        #endregion
    }
}
