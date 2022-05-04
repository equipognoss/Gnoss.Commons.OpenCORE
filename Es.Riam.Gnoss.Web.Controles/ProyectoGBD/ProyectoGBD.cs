using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.Web.Controles.ProyectoGBD
{
    public class ProyectoGBD
    {
        private EntityContext mEntityContext;

        public ProyectoGBD(EntityContext entityContext)
        {
            mEntityContext = entityContext;
        }

        public List<AdministradorGrupoProyecto> CargaAdministradorGrupoProyecto
        {
            get
            {
                return mEntityContext.AdministradorGrupoProyecto.ToList();
            }
        }

        public List<AdministradorProyecto> CargaAdministradorProyecto
        {
            get
            {
                return mEntityContext.AdministradorProyecto.ToList();
            }
        }

        public List<NivelCertificacion> CargaNivelCertificacion
        {
            get
            {
                return mEntityContext.NivelCertificacion.ToList();
            }
        }

        public List<PresentacionListadoSemantico> CargaPresentacionListadoSemantico
        {
            get
            {
                return mEntityContext.PresentacionListadoSemantico.ToList();
            }
        }

        public List<PresentacionMapaSemantico> CargaPresentacionMapaSemantico
        {
            get
            {
                return mEntityContext.PresentacionMapaSemantico.ToList();
            }
        }

        public List<PresentacionMosaicoSemantico> CargaPresentacionMosaicoSemantico
        {
            get
            {
                return mEntityContext.PresentacionMosaicoSemantico.ToList();
            }
        }

        public List<PresentacionPestanyaListadoSemantico> CargaPresentacionPestanyaListadoSemantico
        {
            get
            {
                return mEntityContext.PresentacionPestanyaListadoSemantico.ToList();
            }
        }

        public List<PresentacionPestanyaMapaSemantico> CargaPresentacionPestanyaMapaSemantico
        {
            get
            {
                return mEntityContext.PresentacionPestanyaMapaSemantico.ToList();
            }
        }

        public List<PresentacionPestanyaMosaicoSemantico> CargaPresentacionPestanyaMosaicoSemantico
        {
            get
            {
                return mEntityContext.PresentacionPestanyaMosaicoSemantico.ToList();
            }
        }

        public List<Proyecto> CargaProyecto
        {
            get
            {
                return mEntityContext.Proyecto.ToList();
            }
        }

        public List<ProyectoAgCatTesauro> CargaProyectoAgCatTesauro
        {
            get
            {
                return mEntityContext.ProyectoAgCatTesauro.ToList();
            }
        }

        public List<ProyectoCerradoTmp> CargaProyectoCerradoTmp
        {
            get
            {
                return mEntityContext.ProyectoCerradoTmp.ToList();
            }
        }

        public List<ProyectoCerrandose> CargaProyectoCerrandose
        {
            get
            {
                return mEntityContext.ProyectoCerrandose.ToList();
            }
        }

        public List<ProyectoConfigExtraSem> CargaProyectoConfigExtraSem
        {
            get
            {
                return mEntityContext.ProyectoConfigExtraSem.ToList();
            }
        }

        public List<ProyectoGadget> CargaProyectoGadget
        {
            get
            {
                return mEntityContext.ProyectoGadget.ToList();
            }
        }

        public List<ProyectoGadgetContexto> CargaProyectoGadgetContexto
        {
            get
            {
                return mEntityContext.ProyectoGadgetContexto.ToList();
            }
        }

        public List<ProyectoGadgetContextoHTMLplano> CargaProyectoGadgetContextoHTMLplano
        {
            get
            {
                return mEntityContext.ProyectoGadgetContextoHTMLplano.ToList();
            }
        }

        public List<ProyectoGadgetIdioma> CargaProyectoGadgetIdioma
        {
            get
            {
                return mEntityContext.ProyectoGadgetIdioma.ToList();
            }
        }

        public List<ProyectoGrafoFichaRec> CargaProyectoGrafoFichaRec
        {
            get
            {
                return mEntityContext.ProyectoGrafoFichaRec.ToList();
            }
        }

        public List<ProyectoLoginConfiguracion> CargaProyectoLoginConfiguracion
        {
            get
            {
                return mEntityContext.ProyectoLoginConfiguracion.ToList();
            }
        }

        public List<ProyectoPaginaHtml> CargaProyectoPaginaHtml
        {
            get
            {
                return mEntityContext.ProyectoPaginaHtml.ToList();
            }
        }

        public List<ProyectoPasoRegistro> CargaProyectoPasoRegistro
        {
            get
            {
                return mEntityContext.ProyectoPasoRegistro.ToList();
            }
        }

        public List<ProyectoPerfilNumElem> CargaProyectoPerfilNumElem
        {
            get
            {
                return mEntityContext.ProyectoPerfilNumElem.ToList();
            }
        }

        public List<ProyectoPestanya> CargaProyectoPestanya
        {
            get
            {
                return mEntityContext.ProyectoPestanya.ToList();
            }
        }

        public List<ProyectoPestanyaBusqueda> CargaProyectoPestanyaBusqueda
        {
            get
            {
                return mEntityContext.ProyectoPestanyaBusqueda.ToList();
            }
        }

        public List<ProyectoPestanyaBusquedaExportacion> CargaProyectoPestanyaBusquedaExportacion
        {
            get
            {
                return mEntityContext.ProyectoPestanyaBusquedaExportacion.ToList();
            }
        }

        public List<ProyectoPestanyaBusquedaExportacionExterna> CargaProyectoPestanyaBusquedaExportacionExterna
        {
            get
            {
                return mEntityContext.ProyectoPestanyaBusquedaExportacionExterna.ToList();
            }
        }

        public List<ProyectoPestanyaBusquedaExportacionPropiedad> CargaProyectoPestanyaBusquedaExportacionPropiedad
        {
            get
            {
                return mEntityContext.ProyectoPestanyaBusquedaExportacionPropiedad.ToList();
            }
        }

        public List<ProyectoPestanyaCMS> CargaProyectoPestanyaCMS
        {
            get
            {
                return mEntityContext.ProyectoPestanyaCMS.ToList();
            }
        }

        public List<ProyectoPestanyaExportacionBusqueda> CargaProyectoPestanyaExportacionBusqueda
        {
            get
            {
                return mEntityContext.ProyectoPestanyaExportacionBusqueda.ToList();
            }
        }

        public List<ProyectoPestanyaFiltroOrdenRecursos> CargaProyectoPestanyaFiltroOrdenRecursos
        {
            get
            {
                return mEntityContext.ProyectoPestanyaFiltroOrdenRecursos.ToList();
            }
        }

        public List<ProyectoPestanyaMenu> CargaProyectoPestanyaMenu
        {
            get
            {
                return mEntityContext.ProyectoPestanyaMenu.ToList();
            }
        }

        public List<ProyectoPestanyaMenuRolGrupoIdentidades> CargaProyectoPestanyaMenuRolGrupoIdentidades
        {
            get
            {
                return mEntityContext.ProyectoPestanyaMenuRolGrupoIdentidades.ToList();
            }
        }

        public List<ProyectoPestanyaMenuRolIdentidad> CargaProyectoPestanyaMenuRolIdentidad
        {
            get
            {
                return mEntityContext.ProyectoPestanyaMenuRolIdentidad.ToList();
            }
        }

        public List<ProyectoPestanyaRolGrupoIdentidades> CargaProyectoPestanyaRolGrupoIdentidades
        {
            get
            {
                return mEntityContext.ProyectoPestanyaRolGrupoIdentidades.ToList();
            }
        }

        public List<ProyectoPestanyaRolIdentidad> CargaProyectoPestanyaRolIdentidad
        {
            get
            {
                return mEntityContext.ProyectoPestanyaRolIdentidad.ToList();
            }
        }

        public List<ProyectoRelacionado> CargaProyectoRelacionado
        {
            get
            {
                return mEntityContext.ProyectoRelacionado.ToList();
            }
        }

        public List<ProyectoSearchPersonalizado> CargaProyectoSearchPersonalizado
        {
            get
            {
                return mEntityContext.ProyectoSearchPersonalizado.ToList();
            }
        }

        public List<ProyectoServicioExterno> CargaProyectoServicioExterno
        {
            get
            {
                return mEntityContext.ProyectoServicioExterno.ToList();
            }
        }

        public List<ProyectosMasActivos> CargaProyectosMasActivos
        {
            get
            {
                return mEntityContext.ProyectosMasActivos.ToList();
            }
        }

        public List<ProyTipoRecNoActivReciente> CargaProyTipoRecNoActivReciente
        {
            get
            {
                return mEntityContext.ProyTipoRecNoActivReciente.ToList();
            }
        }

        public List<ProyectoLoginConfiguracion> ObtenerProyectoLoginConfiguracion(Guid clave)
        {
            return mEntityContext.ProyectoLoginConfiguracion.Where(proyLoginConf => proyLoginConf.ProyectoID.Equals(clave)).ToList();
        }

        public void DeleteAdministradorProyecto(AdministradorProyecto admin)
        {
            mEntityContext.EliminarElemento(admin);
            
        }

        public void GuardarCambios()
        {

            mEntityContext.SaveChanges();

        }

        public void AddProyectoPestanyaMenuRow(Guid pestanyaId, Guid organizacionID, Guid proyectoID, ProyectoPestanyaMenu p1, short tipoPestanya, string nombre, string ruta, short orden, bool nuevaPestanya, bool visible, short privacidad, string htmlAlternativo, string idiomasDisponibles, string titulo, string nombreCortoPestanya, bool visibleSinAcceso, string cssBodyClass, string metaDescription, bool activa)
        {
            ProyectoPestanyaMenu proyectoPestanyaMenu = new ProyectoPestanyaMenu(pestanyaId, organizacionID, proyectoID, p1, tipoPestanya, nombre, ruta, orden, nuevaPestanya, visible, privacidad, htmlAlternativo, idiomasDisponibles, titulo, nombreCortoPestanya, visibleSinAcceso, cssBodyClass, metaDescription, activa);
            mEntityContext.ProyectoPestanyaMenu.Add(proyectoPestanyaMenu);
            
        }

        public void AddProyectoPestanyaMenu(ProyectoPestanyaMenu proyectoPestanyaMenu)
        {
            mEntityContext.ProyectoPestanyaMenu.Add(proyectoPestanyaMenu);
            
        }

        internal void AddAdministradorProyecto(AdministradorProyecto pAdminProy)
        {
            mEntityContext.AdministradorProyecto.Add(pAdminProy);
        }

        internal void AddProyectoMasActivo(ProyectosMasActivos pFilaProyectoMasActivo)
        {
            mEntityContext.ProyectosMasActivos.Add(pFilaProyectoMasActivo);
        }

        public void AddProyectoGadget(Guid organizacionID, Guid proyectoID, Guid gadgetID, string titulo, string contenido, int orden, short tipo, string ubicacion, string clases, short tipoUbicacion, bool visible, bool multiIdioma, Guid personalizacionComponenteID, bool cargarPorAjax, string comunidadDestinoFiltros, string nombreCorto)
        {
            ProyectoGadget proyectoGadget = new ProyectoGadget(organizacionID, proyectoID, gadgetID, titulo, contenido, orden, tipo, ubicacion, clases, tipoUbicacion, visible, multiIdioma, personalizacionComponenteID, cargarPorAjax, comunidadDestinoFiltros, nombreCorto);
            proyectoGadget.PersonalizacionComponenteID = null;
            //if (string.IsNullOrEmpty(nombreCorto))
            //{
            //    proyectoGadget.NombreCorto = gadgetID.ToString();
            //}
            mEntityContext.ProyectoGadget.Add(proyectoGadget);
        }

        public DataWrapperProyecto ObtenerProyectoPorID(Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();
            dataWrapperProyecto.ListaProyecto = mEntityContext.Proyecto.Where(proy => proy.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyecto.ListaAdministradorProyecto = mEntityContext.AdministradorProyecto.Where(adminProy => adminProy.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyecto.ListaAdministradorGrupoProyecto = mEntityContext.AdministradorGrupoProyecto.Where(adminGroupProy=>adminGroupProy.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyecto.ListaProyectoAgCatTesauro = mEntityContext.ProyectoAgCatTesauro.Where(proyectoAgCatTesauro=>proyectoAgCatTesauro.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyecto.ListaProyectoCerradoTmp = mEntityContext.ProyectoCerradoTmp.Where(proyectoCerradoTmp=>proyectoCerradoTmp.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyecto.ListaProyectoCerrandose = mEntityContext.ProyectoCerrandose.Where(proyectoCerrandose=>proyectoCerrandose.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyecto.ListaDatoExtraProyecto = mEntityContext.DatoExtraProyecto.Where(datoExtra => datoExtra.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyecto.ListaDatoExtraProyectoOpcion = mEntityContext.DatoExtraProyectoOpcion.Where(datoExtra => datoExtra.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyecto.ListaDatoExtraProyectoVirtuoso = mEntityContext.DatoExtraProyectoVirtuoso.Where(datoExtra => datoExtra.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyecto.ListaProyectoPasoRegistro = mEntityContext.ProyectoPasoRegistro.Where(proyectoPasoRegistro => proyectoPasoRegistro.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyecto.ListaCamposRegistroProyectoGenericos = mEntityContext.CamposRegistroProyectoGenericos.Where(campos => campos.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyecto.ListaProyectoPestanyaMenu = mEntityContext.ProyectoPestanyaMenu.Where(proyectoPestanyaMenu => proyectoPestanyaMenu.ProyectoID.Equals(pProyectoID)).OrderBy(proyecto => proyecto.Orden).ToList();
            dataWrapperProyecto.ListaProyectoPestanyaCMS = mEntityContext.ProyectoPestanyaCMS.Join(mEntityContext.ProyectoPestanyaMenu, proyPestanyaCMS => proyPestanyaCMS.PestanyaID, proyPestanyaMenu => proyPestanyaMenu.PestanyaID, (proyPestanyaCMS, proyPestanyaMenu) => new
            {
                ProyectoPestanyaCMS = proyPestanyaCMS,
                ProyectoID = proyPestanyaMenu.ProyectoID
            }).Where(proyPestanyaCMS => proyPestanyaCMS.ProyectoID.Equals(pProyectoID)).Select(proyPestanyaCMS=> proyPestanyaCMS.ProyectoPestanyaCMS).ToList();

            dataWrapperProyecto.ListaProyectoPestanyaBusqueda = mEntityContext.ProyectoPestanyaBusqueda.Join(mEntityContext.ProyectoPestanyaMenu, proyPestBusqueda => proyPestBusqueda.PestanyaID, proyPestMenu => proyPestMenu.PestanyaID, (proyPestBusqueda, proyPestMenu) => new
            {
                ProyectPestanyaBusqueda = proyPestBusqueda,
                ProyectoID = proyPestMenu.ProyectoID
            }).Where(proyecto => proyecto.ProyectoID.Equals(pProyectoID)).Select(proyecto => proyecto.ProyectPestanyaBusqueda).ToList();

            dataWrapperProyecto.ListaProyectoPestanyaMenuRolGrupoIdentidades = mEntityContext.ProyectoPestanyaMenuRolGrupoIdentidades.Join(mEntityContext.ProyectoPestanyaMenu, proyPestMenuRogrupolId => proyPestMenuRogrupolId.PestanyaID, proyPestMenu => proyPestMenu.PestanyaID, (proyPestMenuRogrupolId, proyPestMenu) => new
            {
                proyPestMenuRogrupolId = proyPestMenuRogrupolId,
                ProyectoID = proyPestMenu.ProyectoID
            }).Where(proy => proy.ProyectoID.Equals(pProyectoID)).Select(proy => proy.proyPestMenuRogrupolId).ToList();

            dataWrapperProyecto.ListaProyectoPestanyaMenuRolIdentidad = mEntityContext.ProyectoPestanyaMenuRolIdentidad.Join(mEntityContext.ProyectoPestanyaMenu, proyPestRolId => proyPestRolId.PestanyaID, proyPestMenu => proyPestMenu.PestanyaID, (proyPestRolId, proyPestMenu) => new
            {
                proyectPestanyaRolIdentidad = proyPestRolId,
                ProyectoID = proyPestMenu.ProyectoID
            }).Where(proy => proy.ProyectoID.Equals(pProyectoID)).Select(proy => proy.proyectPestanyaRolIdentidad).ToList();

            dataWrapperProyecto.ListaProyectoEventoAccion = mEntityContext.ProyectoEventoAccion.Where(proyectoEvento => proyectoEvento.ProyectoID.Equals(pProyectoID)).ToList();

            dataWrapperProyecto.ListaAccionesExternasProyecto = mEntityContext.AccionesExternasProyecto.Where(acciones => acciones.ProyectoID.Equals(pProyectoID)).ToList();

            dataWrapperProyecto.ListaProyectoSearchPersonalizado = mEntityContext.ProyectoSearchPersonalizado.Where(proyectoSearchPersonalizado => proyectoSearchPersonalizado.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyecto.ListaOntologiaProyecto = mEntityContext.OntologiaProyecto.Where(ontologiaProy => ontologiaProy.ProyectoID.Equals(pProyectoID)).ToList();

            dataWrapperProyecto.ListaVistaVirtualProyecto = mEntityContext.VistaVirtualProyecto.Where(vistaVirtualProy => vistaVirtualProy.ProyectoID.Equals(pProyectoID)).ToList();

            dataWrapperProyecto.ListaConfigAutocompletarProy = mEntityContext.ConfigAutocompletarProy.Where(config => config.ProyectoID.Equals(pProyectoID)).ToList();
          
            return dataWrapperProyecto;
        }

        internal void DeleteProyectoPestanyaMenu(ProyectoPestanyaMenu pest)
        {
            mEntityContext.EliminarElemento(pest);
        }

        internal void DeleteProyectoPestanyaFiltroOrdenRecursos(ProyectoPestanyaFiltroOrdenRecursos pFiltro)
        {
            mEntityContext.EliminarElemento(pFiltro);
        }

        internal void DeletePresentacionPestanyaListadoSemantico(PresentacionPestanyaListadoSemantico pPlsr)
        {
            mEntityContext.EliminarElemento(pPlsr);
        }

        internal void DeletePresentacionPestanyaMosaicoSemantico(PresentacionPestanyaMosaicoSemantico pPlsr)
        {
            mEntityContext.EliminarElemento(pPlsr);
        }

        internal void DeletePresentacionPestanyaMapaSemantico(PresentacionPestanyaMapaSemantico pPlsr)
        {
            mEntityContext.EliminarElemento(pPlsr);
        }

        internal void DeletePresentacionMosaicoSemantico(PresentacionMosaicoSemantico plsr)
        {
            mEntityContext.EliminarElemento(plsr);
        }

        internal void DeletePresentacionMapaSemantico(PresentacionMapaSemantico plsr)
        {
            mEntityContext.EliminarElemento(plsr);
        }

        internal void DeletePresentacionListadoSemantico(PresentacionListadoSemantico pls)
        {
            mEntityContext.EliminarElemento(pls);
        }

        internal void DeleteRecursosRelacionadosPresentacion(RecursosRelacionadosPresentacion rrp)
        {
            mEntityContext.EliminarElemento(rrp);
        }

        internal void DeleteProyectoGadget(ProyectoGadget filaPG)
        {
            mEntityContext.EliminarElemento(filaPG);
        }

        internal void DeleteProyectoGadgetContexto(ProyectoGadgetContexto filaPGC)
        {
            mEntityContext.EliminarElemento(filaPGC);
        }

        internal void DeleteProyectoGadgetIdioma(ProyectoGadgetIdioma filaPGI)
        {
            mEntityContext.EliminarElemento(filaPGI);
        }

        public void DeleteProyectoConfigExtraSem(ProyectoConfigExtraSem fila)
        {
            mEntityContext.EliminarElemento(fila);
        }

        internal void AddProyecto(Proyecto filaProyectoNuevo)
        {
            mEntityContext.Proyecto.Add(filaProyectoNuevo);
        }
    }
}
