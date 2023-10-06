using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Elementos.ParametroGeneralDSEspacio;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.Solicitud;
using Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;

namespace Es.Riam.Gnoss.Web.Controles.Administracion
{
	public class ControladorDatosExtra
	{
		private DataWrapperDocumentacion mDataWrapperDocumentacion = null;
		private DataWrapperProyecto mDataWrapperProyecto = null;
		private Dictionary<string, string> mParametroProyecto;

		private Elementos.ServiciosGenerales.Proyecto ProyectoSeleccionado = null;

		private GestorParametroGeneral mParametrosGeneralesDS;
		private ParametroGeneral mFilaParametrosGenerales = null;

		private LoggingService mLoggingService;
		private VirtuosoAD mVirtuosoAD;
		private EntityContext mEntityContext;
		private IHttpContextAccessor mHttpContextAccessor;
		private ConfigService mConfigService;
		private RedisCacheWrapper mRedisCacheWrapper;
		private GnossCache mGnossCache;
		private EntityContextBASE mEntityContextBASE;
		private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;

		/// <summary>
		/// 
		/// </summary>
		public ControladorDatosExtra(Elementos.ServiciosGenerales.Proyecto pProyecto, LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, VirtuosoAD virtuosoAD, EntityContextBASE entityContextBASE, GnossCache gnossCache, IHttpContextAccessor httpContextAccessor, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
		{
			mVirtuosoAD = virtuosoAD;
			mLoggingService = loggingService;
			mEntityContext = entityContext;
			mConfigService = configService;
			mHttpContextAccessor = httpContextAccessor;
			mRedisCacheWrapper = redisCacheWrapper;
			mEntityContextBASE = entityContextBASE;
			mGnossCache = gnossCache;
			mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
			ProyectoSeleccionado = pProyecto;
		}

		public void GuardarNuevoDatoExtraVirtuosoEcosistema(Guid pDatoExtraID, string pNombre, bool pObligatorio, int pOrden, string pPredicadoRDF)
		{
			DatoExtraEcosistemaVirtuoso datoVirtuoso = new DatoExtraEcosistemaVirtuoso();

			datoVirtuoso.DatoExtraID = pDatoExtraID;
			datoVirtuoso.Orden = pOrden;
			datoVirtuoso.Titulo = pNombre;
			datoVirtuoso.InputID = pNombre;
			datoVirtuoso.InputsSuperiores = "";
			datoVirtuoso.QueryVirtuoso = "";
			datoVirtuoso.ConexionBD = "";
			datoVirtuoso.Obligatorio = pObligatorio;
			datoVirtuoso.Paso1Registro = false;
			datoVirtuoso.VisibilidadFichaPerfil = true;
			datoVirtuoso.PredicadoRDF = pPredicadoRDF;
			datoVirtuoso.NombreCampo = pNombre;
			datoVirtuoso.EstructuraHTMLFicha = "";

			mEntityContext.DatoExtraEcosistemaVirtuoso.Add(datoVirtuoso);
		}

		public void GuardarNuevoDatoExtraEcosistema(Guid pDatoExtraID, string pNombre, bool pObligatorio, string pOpciones, int pOrden, string pPredicadoRDF)
		{
			DatoExtraEcosistema datoExtra = new DatoExtraEcosistema();

			datoExtra.DatoExtraID = pDatoExtraID;
			datoExtra.Orden = pOrden;
			datoExtra.Titulo = pNombre;
			datoExtra.PredicadoRDF = pPredicadoRDF;
			datoExtra.Obligatorio = pObligatorio;
			datoExtra.Paso1Registro = false;

			mEntityContext.DatoExtraEcosistema.Add(datoExtra);
			GuardarCambios();

			string[] opciones = pOpciones.Split(",");
			int orden = 0;

			foreach (string opcion in opciones)
			{
				if (!string.IsNullOrEmpty(opcion))
				{
					DatoExtraEcosistemaOpcion opcionBD = new DatoExtraEcosistemaOpcion();

					opcionBD.DatoExtraID = pDatoExtraID;
					opcionBD.Orden = orden;
					opcionBD.OpcionID = Guid.NewGuid();
					opcionBD.Opcion = opcion;
					orden++;

					mEntityContext.DatoExtraEcosistemaOpcion.Add(opcionBD);					
				}
			}
		}

        public void ModificarDatoExtraEcosistema(Guid pDatoExtraID, string pNombre, bool pObligatorio, string pOpciones, int pOrden, string pPredicadoRDF)
        {
            DatoExtraEcosistema datoExtra = mEntityContext.DatoExtraEcosistema.Where(x => x.DatoExtraID.Equals(pDatoExtraID)).FirstOrDefault();

            datoExtra.Orden = pOrden;
            datoExtra.Titulo = pNombre;
            datoExtra.Obligatorio = pObligatorio;
			datoExtra.PredicadoRDF = pPredicadoRDF;

			mEntityContext.DatoExtraEcosistema.Update(datoExtra);
            GuardarCambios();

            string[] opciones = pOpciones.Split(",");
            int orden = 0;

            //eliminar opciones
            List<DatoExtraEcosistemaOpcion> opcionesEnBd = mEntityContext.DatoExtraEcosistemaOpcion.Where(x => x.DatoExtraID.Equals(pDatoExtraID)).ToList();
			foreach (DatoExtraEcosistemaOpcion opcion in opcionesEnBd)
			{
				if (!opciones.Contains(opcion.Opcion))
				{
					List<DatoExtraEcosistemaOpcionSolicitud> opcionesBorrarSolicitud = mEntityContext.DatoExtraEcosistemaOpcionSolicitud.Where(x => x.OpcionID.Equals(opcion.OpcionID)).ToList();
					foreach (var opt in opcionesBorrarSolicitud)
					{
						mEntityContext.DatoExtraEcosistemaOpcionSolicitud.Remove(opt);
					}
                    List<DatoExtraEcosistemaOpcionPerfil> opcionesBorrarPerfil = mEntityContext.DatoExtraEcosistemaOpcionPerfil.Where(x => x.OpcionID.Equals(opcion.OpcionID)).ToList();
                    foreach (var opt in opcionesBorrarPerfil)
                    {
                        mEntityContext.DatoExtraEcosistemaOpcionPerfil.Remove(opt);
                    }
					GuardarCambios();

					mEntityContext.DatoExtraEcosistemaOpcion.Remove(opcion);
                }
			}

            foreach (string opcion in opciones)
			{
				DatoExtraEcosistemaOpcion opcionBD = mEntityContext.DatoExtraEcosistemaOpcion.Where(x => x.DatoExtraID.Equals(pDatoExtraID) && x.Opcion.Equals(opcion)).FirstOrDefault();
				//si la opcion existe, no se añade
				if (opcionBD == null)
				{
                    if (!string.IsNullOrEmpty(opcion))
                    {
                        DatoExtraEcosistemaOpcion opcionNueva = new DatoExtraEcosistemaOpcion();

                        opcionNueva.DatoExtraID = pDatoExtraID;
                        opcionNueva.Orden = orden;
                        opcionNueva.OpcionID = Guid.NewGuid();
                        opcionNueva.Opcion = opcion;
                        orden++;

                        mEntityContext.DatoExtraEcosistemaOpcion.Add(opcionNueva);
                    }
                }
			}
        }

        public void ModificarDatoExtraVirtuosoEcosistema(Guid pDatoExtraID, string pNombre, bool pObligatorio, int pOrden, string pPredicadoRDF)
		{
			DatoExtraEcosistemaVirtuoso datoExtra = mEntityContext.DatoExtraEcosistemaVirtuoso.Where(x => x.DatoExtraID.Equals(pDatoExtraID)).FirstOrDefault();

			if (datoExtra != null)
			{
				datoExtra.Titulo = pNombre;
				datoExtra.Orden = pOrden;
				datoExtra.Obligatorio = pObligatorio;
				datoExtra.InputID = pNombre;
				datoExtra.NombreCampo = pNombre;
				datoExtra.PredicadoRDF = pPredicadoRDF;

				mEntityContext.DatoExtraEcosistemaVirtuoso.Update(datoExtra);
            }		
		}

		public List<DatoExtraOpcionModel> ObtenerOpciones(Guid pDatoExtraID, bool pEcosistema)
		{
			List<DatoExtraOpcionModel> listaOpciones = new List<DatoExtraOpcionModel>();

            if (pEcosistema)
			{
				List<DatoExtraEcosistemaOpcion> opciones = mEntityContext.DatoExtraEcosistemaOpcion.Where(x => x.DatoExtraID.Equals(pDatoExtraID)).ToList();

				foreach (DatoExtraEcosistemaOpcion opcion in opciones)
				{
					DatoExtraOpcionModel opcionModel = new DatoExtraOpcionModel();

					opcionModel.OpcionID = opcion.OpcionID;
					opcionModel.Orden = opcion.Orden;
					opcionModel.ProyectoID = ProyectoAD.MetaProyecto;
					opcionModel.OrganizacionID = ProyectoAD.MetaOrganizacion;
					opcionModel.Nombre = opcion.Opcion;
					opcionModel.DatoExtraID = opcion.DatoExtraID;

					listaOpciones.Add(opcionModel);
				}
			}
			else
			{
                List<DatoExtraProyectoOpcion> opciones = mEntityContext.DatoExtraProyectoOpcion.Where(x => x.DatoExtraID.Equals(pDatoExtraID)).ToList();

				foreach (DatoExtraProyectoOpcion opcion in opciones)
				{
					DatoExtraOpcionModel opcionModel = new DatoExtraOpcionModel();

					opcionModel.OpcionID = opcion.OpcionID;
					opcionModel.Orden = opcion.Orden;
					opcionModel.ProyectoID = opcion.ProyectoID;
					opcionModel.OrganizacionID = opcion.OrganizacionID;
					opcionModel.Nombre = opcion.Opcion;
					opcionModel.DatoExtraID = opcion.DatoExtraID;

					listaOpciones.Add(opcionModel);
				}
            }

			return listaOpciones;
		}

		public void GuardarNuevoDatoExtraProyecto(Guid pProyectoID, Guid pOrganizacion, Guid pDatoExtraID, string pNombre, bool pObligatorio, string pOpciones, int pOrden, string pPredicadoRDF)
		{
			DatoExtraProyecto datoExtra = new DatoExtraProyecto();

			datoExtra.DatoExtraID = pDatoExtraID;
			datoExtra.ProyectoID = pProyectoID;
			datoExtra.OrganizacionID = pOrganizacion;
			datoExtra.Orden = pOrden;
			datoExtra.Titulo = pNombre;
			datoExtra.PredicadoRDF = pPredicadoRDF;
			datoExtra.Obligatorio = pObligatorio;
			datoExtra.Paso1Registro = false;

			mEntityContext.DatoExtraProyecto.Add(datoExtra);
			GuardarCambios();

			string[] opciones = pOpciones.Split(",");
			int orden = 0;

			foreach (string opcion in opciones)
			{
				if (!string.IsNullOrEmpty(opcion))
				{
					DatoExtraProyectoOpcion opcionBD = new DatoExtraProyectoOpcion();
					opcionBD.OrganizacionID = pOrganizacion;
					opcionBD.ProyectoID = pProyectoID;
					opcionBD.DatoExtraID = pDatoExtraID;
					opcionBD.Orden = orden;
					opcionBD.OpcionID = Guid.NewGuid();
					opcionBD.Opcion = opcion;
					orden++;

					mEntityContext.DatoExtraProyectoOpcion.Add(opcionBD);
				}
			}
		}

		public void ModificarDatoExtraProyecto(Guid pProyectoID, Guid pOrganizacion, Guid pDatoExtraID, string pNombre, bool pObligatorio, string pOpciones, int pOrden, string pPredicadoRDF)
		{
            DatoExtraProyecto datoExtra = mEntityContext.DatoExtraProyecto.Where(x => x.DatoExtraID.Equals(pDatoExtraID)).FirstOrDefault();

            datoExtra.Orden = pOrden;
            datoExtra.Titulo = pNombre;
            datoExtra.Obligatorio = pObligatorio;
			datoExtra.PredicadoRDF = pPredicadoRDF;

			mEntityContext.DatoExtraProyecto.Update(datoExtra);
            GuardarCambios();

            string[] opciones = pOpciones.Split(",");
            int orden = 0;

            //eliminar opciones
            List<DatoExtraProyectoOpcion> opcionesEnBd = mEntityContext.DatoExtraProyectoOpcion.Where(x => x.DatoExtraID.Equals(pDatoExtraID)).ToList();
            foreach (DatoExtraProyectoOpcion opcion in opcionesEnBd)
            {
                if (!opciones.Contains(opcion.Opcion))
                {
                    List<DatoExtraProyectoOpcionSolicitud> opcionesBorrarSolicitud = mEntityContext.DatoExtraProyectoOpcionSolicitud.Where(x => x.OpcionID.Equals(opcion.OpcionID)).ToList();
                    foreach (var opt in opcionesBorrarSolicitud)
                    {
                        mEntityContext.DatoExtraProyectoOpcionSolicitud.Remove(opt);
                    }
                    List<DatoExtraProyectoOpcionIdentidad> opcionesBorrarIdentidad = mEntityContext.DatoExtraProyectoOpcionIdentidad.Where(x => x.OpcionID.Equals(opcion.OpcionID)).ToList();
                    foreach (var opt in opcionesBorrarIdentidad)
                    {
                        mEntityContext.DatoExtraProyectoOpcionIdentidad.Remove(opt);
                    }
                    GuardarCambios();

                    mEntityContext.DatoExtraProyectoOpcion.Remove(opcion);
                }
            }

            foreach (string opcion in opciones)
            {
                DatoExtraProyectoOpcion opcionBD = mEntityContext.DatoExtraProyectoOpcion.Where(x => x.DatoExtraID.Equals(pDatoExtraID) && x.Opcion.Equals(opcion)).FirstOrDefault();
                //si la opcion existe, no se añade
                if (opcionBD == null)
                {
                    if (!string.IsNullOrEmpty(opcion))
                    {
                        DatoExtraProyectoOpcion opcionNueva = new DatoExtraProyectoOpcion();

                        opcionNueva.DatoExtraID = pDatoExtraID;
                        opcionNueva.Orden = orden;
                        opcionNueva.OpcionID = Guid.NewGuid();
                        opcionNueva.Opcion = opcion;
						opcionNueva.ProyectoID = pProyectoID;
						opcionNueva.OrganizacionID = pOrganizacion;
                        orden++;

                        mEntityContext.DatoExtraProyectoOpcion.Add(opcionNueva);
                    }
                }
            }
        }

        public void GuardarNuevoDatoExtraVirtuosoProyecto(Guid pProyectoID, Guid pOrganizacion, Guid pDatoExtraID, string pNombre, bool pObligatorio, int pOrden, string pPredicadoRDF)
		{
			DatoExtraProyectoVirtuoso datoVirtuoso = new DatoExtraProyectoVirtuoso();

			datoVirtuoso.OrganizacionID = pOrganizacion;
			datoVirtuoso.ProyectoID = pProyectoID;
			datoVirtuoso.DatoExtraID = pDatoExtraID;
			datoVirtuoso.Orden = pOrden;
			datoVirtuoso.Titulo = pNombre;
			datoVirtuoso.InputID = pNombre;
			datoVirtuoso.InputsSuperiores = "";
			datoVirtuoso.QueryVirtuoso = "";
			datoVirtuoso.ConexionBD = "";
			datoVirtuoso.Obligatorio = pObligatorio;
			datoVirtuoso.Paso1Registro = false;
			datoVirtuoso.VisibilidadFichaPerfil = true;
			datoVirtuoso.PredicadoRDF = pPredicadoRDF;
			datoVirtuoso.NombreCampo = pNombre;
			datoVirtuoso.EstructuraHTMLFicha = "";

			mEntityContext.DatoExtraProyectoVirtuoso.Add(datoVirtuoso);
		}

        public void ModificarDatoExtraVirtuosoProyecto(Guid pProyectoID, Guid pOrganizacion, Guid pDatoExtraID, string pNombre, bool pObligatorio, int pOrden, string pPredicadoRDF)
        {
			DatoExtraProyectoVirtuoso datoVirtuoso = mEntityContext.DatoExtraProyectoVirtuoso.Where(x => x.DatoExtraID.Equals(pDatoExtraID)).FirstOrDefault();

            datoVirtuoso.OrganizacionID = pOrganizacion;
            datoVirtuoso.ProyectoID = pProyectoID;
            datoVirtuoso.DatoExtraID = pDatoExtraID;
            datoVirtuoso.Orden = pOrden;
            datoVirtuoso.Titulo = pNombre;
            datoVirtuoso.InputID = pNombre;
            datoVirtuoso.Obligatorio = pObligatorio;
            datoVirtuoso.NombreCampo = pNombre;
			datoVirtuoso.PredicadoRDF = pPredicadoRDF;

			mEntityContext.DatoExtraProyectoVirtuoso.Update(datoVirtuoso);
        }

        public void EliminarDatoExtra(Guid pDatoExtraID, bool pEcosistema)
		{
			if (pEcosistema)
			{
				//Eliminar de ecosistema
				DatoExtraEcosistema datoExtraEcosistema = mEntityContext.DatoExtraEcosistema.Where(x => x.DatoExtraID.Equals(pDatoExtraID)).FirstOrDefault();

				//tipo opcion
				if (datoExtraEcosistema != null)
				{
                    List<DatoExtraEcosistemaOpcionSolicitud> ecosOpcSol = mEntityContext.DatoExtraEcosistemaOpcionSolicitud.Where(x => x.DatoExtraID.Equals(pDatoExtraID)).ToList();
                    foreach (var solicitud in ecosOpcSol)
                    {
                        mEntityContext.DatoExtraEcosistemaOpcionSolicitud.Remove(solicitud);
                    }
                    List<DatoExtraEcosistemaOpcionPerfil> proyOpcPerfil = mEntityContext.DatoExtraEcosistemaOpcionPerfil.Where(x => x.DatoExtraID.Equals(pDatoExtraID)).ToList();
                    foreach (var perfil in proyOpcPerfil)
                    {
                        mEntityContext.DatoExtraEcosistemaOpcionPerfil.Remove(perfil);
                    }
					GuardarCambios();
                    List<DatoExtraEcosistemaOpcion> ecosOpc = mEntityContext.DatoExtraEcosistemaOpcion.Where(x => x.DatoExtraID.Equals(pDatoExtraID)).ToList();
                    foreach (var opcion in ecosOpc)
                    {
                        mEntityContext.DatoExtraEcosistemaOpcion.Remove(opcion);
                    }
                    GuardarCambios();

					mEntityContext.DatoExtraEcosistema.Remove(datoExtraEcosistema);
				}
				else
				{
                    DatoExtraEcosistemaVirtuoso datoExtraVirtuoso = mEntityContext.DatoExtraEcosistemaVirtuoso.Where(x => x.DatoExtraID.Equals(pDatoExtraID)).FirstOrDefault();
					if (datoExtraVirtuoso != null)
					{
                        List<DatoExtraEcosistemaVirtuosoSolicitud> ecosSolicitud = mEntityContext.DatoExtraEcosistemaVirtuosoSolicitud.Where(x => x.DatoExtraID.Equals(pDatoExtraID)).ToList();
                        foreach (var solicitud in ecosSolicitud)
                        {
                            mEntityContext.DatoExtraEcosistemaVirtuosoSolicitud.Remove(solicitud);
                        }
                        List<DatoExtraEcosistemaVirtuosoPerfil> ecosPerfil = mEntityContext.DatoExtraEcosistemaVirtuosoPerfil.Where(x => x.DatoExtraID.Equals(pDatoExtraID)).ToList();
                        foreach (var perfil in ecosPerfil)
                        {
                            mEntityContext.DatoExtraEcosistemaVirtuosoPerfil.Remove(perfil);
                        }
						GuardarCambios();

						mEntityContext.DatoExtraEcosistemaVirtuoso.Remove(datoExtraVirtuoso);

                    }                   
                }
			}
			else
			{
				// Eliminar del proyecto
				DatoExtraProyecto datoExtraProyecto = mEntityContext.DatoExtraProyecto.Where(x => x.DatoExtraID.Equals(pDatoExtraID)).FirstOrDefault();
				// tipo opcion
				if (datoExtraProyecto != null)
				{
					List<DatoExtraProyectoOpcionSolicitud> proyOpcSol = mEntityContext.DatoExtraProyectoOpcionSolicitud.Where(x => x.DatoExtraID.Equals(pDatoExtraID)).ToList();
					foreach (var solicitud in proyOpcSol)
					{
						mEntityContext.DatoExtraProyectoOpcionSolicitud.Remove(solicitud);
					}
					List<DatoExtraProyectoOpcionIdentidad> proyOpcIden = mEntityContext.DatoExtraProyectoOpcionIdentidad.Where(x => x.DatoExtraID.Equals(pDatoExtraID)).ToList();
					foreach (var opcIden in proyOpcIden)
					{
						mEntityContext.DatoExtraProyectoOpcionIdentidad.Remove(opcIden);
					}
					GuardarCambios();
					List<DatoExtraProyectoOpcion> proyOpc = mEntityContext.DatoExtraProyectoOpcion.Where(x => x.DatoExtraID.Equals(pDatoExtraID)).ToList();
					foreach (var opc in proyOpc)
					{
						mEntityContext.DatoExtraProyectoOpcion.Remove(opc);
					}
					GuardarCambios();

					mEntityContext.DatoExtraProyecto.Remove(datoExtraProyecto);
				}
				else
				{
					//tipo texto libre
					DatoExtraProyectoVirtuoso datoExtra = mEntityContext.DatoExtraProyectoVirtuoso.Where(x => x.DatoExtraID.Equals(pDatoExtraID)).FirstOrDefault();
					
					if (datoExtra != null)
					{
						List<DatoExtraProyectoVirtuosoSolicitud> virSol = mEntityContext.DatoExtraProyectoVirtuosoSolicitud.Where(x => x.DatoExtraID.Equals(pDatoExtraID)).ToList();
						foreach (var sol in virSol)
						{
							mEntityContext.DatoExtraProyectoVirtuosoSolicitud.Remove(sol);
						}
						List<DatoExtraProyectoVirtuosoIdentidad> virIden = mEntityContext.DatoExtraProyectoVirtuosoIdentidad.Where(x => x.DatoExtraID.Equals(pDatoExtraID)).ToList();
						foreach (var iden in virIden)
						{
							mEntityContext.DatoExtraProyectoVirtuosoIdentidad.Remove(iden);
						}
						GuardarCambios();

						mEntityContext.DatoExtraProyectoVirtuoso.Remove(datoExtra);

					}
				}
			}		
		}

		private string ObtenerStringOpciones(List<DatoExtraOpcionModel> pListaOpciones)
		{
			string opciones = string.Empty;

			if (pListaOpciones != null && pListaOpciones.Count > 0)
			{
				foreach (DatoExtraOpcionModel opt in pListaOpciones)
				{
					opciones += $"{opt.Nombre},";
				}
			}

			return opciones;
		}

		public void GuardarDatosExtra(List<DatoExtraModel> pLista, bool pEcosistema, Guid pProyecto, Guid pOrganizacion)
		{
			List<DatoExtraEcosistema> extraEcosistema = mEntityContext.DatoExtraEcosistema.ToList();
			List<DatoExtraEcosistemaVirtuoso> extraEcosistemaVirtuoso = mEntityContext.DatoExtraEcosistemaVirtuoso.ToList();
			List<DatoExtraProyecto> extraProyecto = mEntityContext.DatoExtraProyecto.Where(x => x.ProyectoID.Equals(pProyecto)).ToList();
			List<DatoExtraProyectoVirtuoso> extraProyectoVirtuoso = mEntityContext.DatoExtraProyectoVirtuoso.Where(x => x.ProyectoID.Equals(pProyecto)).ToList();

			if (pEcosistema)
			{
				foreach (var dato in extraEcosistema)
				{
					if (!pLista.Exists(x => x.DatoExtraID.Equals(dato.DatoExtraID) && x.Tipo.Equals(TipoDatoExtra.Opcion)))
					{
						EliminarDatoExtra(dato.DatoExtraID, true);
					}
				}

				foreach (var dato in extraEcosistemaVirtuoso)
				{
					if (!pLista.Exists(x => x.DatoExtraID.Equals(dato.DatoExtraID) && x.Tipo.Equals(TipoDatoExtra.TextoLibre)))
					{
						EliminarDatoExtra(dato.DatoExtraID, true);
					}
				}
			}
			else
			{
				foreach (var dato in extraProyecto)
				{
					if (!pLista.Exists(x => x.DatoExtraID.Equals(dato.DatoExtraID) && x.Tipo.Equals(TipoDatoExtra.Opcion)))
					{
						EliminarDatoExtra(dato.DatoExtraID, false);
					}
				}

				foreach (var dato in extraProyectoVirtuoso)
				{
					if (!pLista.Exists(x => x.DatoExtraID.Equals(dato.DatoExtraID) && x.Tipo.Equals(TipoDatoExtra.TextoLibre)))
					{
						EliminarDatoExtra(dato.DatoExtraID, false);
					}
				}
			}

			foreach (DatoExtraModel datoExtra in pLista)
			{
				if (datoExtra.Deleted)
				{
					EliminarDatoExtra(datoExtra.DatoExtraID, pEcosistema);
				}
				else
				{
					string opciones = ObtenerStringOpciones(datoExtra.Opciones);

					if (pEcosistema)
					{
						if (datoExtra.Tipo.Equals(TipoDatoExtra.Opcion))
						{
                            DatoExtraEcosistema datoExtraEcosistema = mEntityContext.DatoExtraEcosistema.Where(x => x.DatoExtraID.Equals(datoExtra.DatoExtraID)).FirstOrDefault();
							
							if (datoExtraEcosistema != null)
							{
								ModificarDatoExtraEcosistema(datoExtraEcosistema.DatoExtraID, datoExtra.Nombre, datoExtra.Obligatorio, opciones, datoExtra.Orden, datoExtra.PredicadoRDF);
							}
							else
							{
								GuardarNuevoDatoExtraEcosistema(datoExtra.DatoExtraID, datoExtra.Nombre, datoExtra.Obligatorio, opciones, datoExtra.Orden, datoExtra.PredicadoRDF);
							}							
						}
						else
						{
							DatoExtraEcosistemaVirtuoso datoExtraEcosistemaVirtuoso = mEntityContext.DatoExtraEcosistemaVirtuoso.Where(x => x.DatoExtraID.Equals(datoExtra.DatoExtraID)).FirstOrDefault();
							
							if (datoExtraEcosistemaVirtuoso != null)
							{
								ModificarDatoExtraVirtuosoEcosistema(datoExtra.DatoExtraID, datoExtra.Nombre, datoExtra.Obligatorio, datoExtra.Orden, datoExtra.PredicadoRDF);
							}
							else
							{
								GuardarNuevoDatoExtraVirtuosoEcosistema(datoExtra.DatoExtraID, datoExtra.Nombre, datoExtra.Obligatorio, datoExtra.Orden, datoExtra.PredicadoRDF);
							}							
						}
					}
					else
					{
						if (datoExtra.Tipo.Equals(TipoDatoExtra.Opcion))
						{
							DatoExtraProyecto datoExtraProyecto = mEntityContext.DatoExtraProyecto.Where(x => x.DatoExtraID.Equals(datoExtra.DatoExtraID)).FirstOrDefault();

							if (datoExtraProyecto != null)
							{
								ModificarDatoExtraProyecto(datoExtraProyecto.ProyectoID, datoExtraProyecto.OrganizacionID, datoExtra.DatoExtraID, datoExtra.Nombre, datoExtra.Obligatorio, opciones, datoExtra.Orden, datoExtra.PredicadoRDF);
							}
							else
							{
								GuardarNuevoDatoExtraProyecto(pProyecto, pOrganizacion, datoExtra.DatoExtraID, datoExtra.Nombre, datoExtra.Obligatorio, opciones, datoExtra.Orden, datoExtra.PredicadoRDF);
							}							
						}
						else
						{
							DatoExtraProyectoVirtuoso datoExtraProyectoVirtuoso = mEntityContext.DatoExtraProyectoVirtuoso.Where(x => x.DatoExtraID.Equals(datoExtra.DatoExtraID)).FirstOrDefault();

							if (datoExtraProyectoVirtuoso != null)
							{
								ModificarDatoExtraVirtuosoProyecto(datoExtraProyectoVirtuoso.ProyectoID, datoExtraProyectoVirtuoso.OrganizacionID, datoExtra.DatoExtraID, datoExtra.Nombre, datoExtra.Obligatorio, datoExtra.Orden, datoExtra.PredicadoRDF);
							}
							else
							{
								GuardarNuevoDatoExtraVirtuosoProyecto(pProyecto, pOrganizacion, datoExtra.DatoExtraID, datoExtra.Nombre, datoExtra.Obligatorio, datoExtra.Orden, datoExtra.PredicadoRDF);
							}	
						}
					}
				}
			}

			GuardarCambios();
		}

		public void GuardarCambios()
		{
			mEntityContext.SaveChanges();
		}

	}
}