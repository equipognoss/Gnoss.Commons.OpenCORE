using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Util;
using System;
using System.Data;

namespace Es.Riam.Gnoss.Elementos.Facetado
{
    public class Faceta : ElementoGnoss
    {

        #region Constructor

        /// <summary>
        /// Constructor a partir de los datos pasados como parámetros
        /// </summary>
        /// <param name="pFilaFaceta">Fila de la faceta de la tabla faceta, FacetaObjetoConociemto o FacetaObjetoConociemtoProyecto</param>
        public Faceta(object pFilaFaceta, GestionFacetas pGestor, LoggingService loggingService)
            : base(pFilaFaceta, pGestor, loggingService)
        {
            if (!(pFilaFaceta is FacetaObjetoConocimiento)
            && !(pFilaFaceta is FacetaObjetoConocimientoProyecto)
            && !(pFilaFaceta is FacetaFiltroProyecto)
            && !(pFilaFaceta is FacetaHome)
            && !(pFilaFaceta is FacetaObjetoConocimiento)
            && !(pFilaFaceta is FacetaObjetoConocimientoProyecto)
            && !(pFilaFaceta is FacetaFiltroProyecto)
            && !(pFilaFaceta is FacetaFiltroHome))

            {
                throw new Exception("La fila introducida no es una fila de faceta");
            }
        }
        #endregion

        #region Propiedades


        public string ClaveFaceta
        {
            get
            {
                if (FilaElemento == null)
                {
                    return ClaveFacetaEntity;
                }
                return (string)FilaElemento["Faceta"];
            }
            set
            {
                if (FilaElemento == null)
                {
                    ClaveFacetaEntity = value;
                }
                else
                {
                    FilaElemento["Faceta"] = value;
                }

            }
        }

        public string ClaveFacetaEntity
        {
            get
            {
                return (string)UtilReflection.GetValueReflection(FilaElementoEntity, "Faceta");
            }
            set
            {
                UtilReflection.SetValueReflection(FilaElementoEntity, "Faceta", value);
            }
        }

        public override string Nombre
        {
            get
            {
                DataRow fila = FilaElemento;
                if (FilaElementoEntity is FacetaFiltroProyecto)
                {
                    return ((FacetaFiltroProyecto)FilaElementoEntity).FacetaObjetoConocimientoProyecto.NombreFaceta;
                }
                else if(FilaElementoEntity is FacetaObjetoConocimiento)
                {
                    return ((FacetaObjetoConocimiento)FilaElementoEntity).NombreFaceta;
                }
                else if (FilaElementoEntity is FacetaObjetoConocimientoProyecto)
                {
                    return ((FacetaObjetoConocimientoProyecto)FilaElementoEntity).NombreFaceta;
                }
                else if (FilaElementoEntity is FacetaFiltroHome)
                {
                    return ((FacetaFiltroHome)FilaElementoEntity).FacetaHome.FacetaObjetoConocimientoProyecto.NombreFaceta;
                }
                else if (FilaElementoEntity is FacetaHome)
                {
                    return ((FacetaHome)FilaElementoEntity).FacetaObjetoConocimientoProyecto.NombreFaceta;
                }

                return "";
            }
            set
            {
                DataRow fila = FilaElemento;
                if (FilaElementoEntity is FacetaFiltroProyecto)
                {
                    ((FacetaFiltroProyecto)FilaElementoEntity).FacetaObjetoConocimientoProyecto.NombreFaceta = value;
                }
                else if (FilaElementoEntity is FacetaObjetoConocimiento)
                {
                    ((FacetaObjetoConocimiento)FilaElementoEntity).NombreFaceta = value;
                }
                else if (FilaElementoEntity is FacetaObjetoConocimientoProyecto)
                {
                    ((FacetaObjetoConocimientoProyecto)FilaElementoEntity).NombreFaceta = value;
                }
                else if (FilaElementoEntity is FacetaFiltroHome)
                {
                    ((FacetaFiltroHome)FilaElementoEntity).FacetaHome.FacetaObjetoConocimientoProyecto.NombreFaceta = value;
                }
                else if (FilaElementoEntity is FacetaHome)
                {
                    ((FacetaHome)FilaElementoEntity).FacetaObjetoConocimientoProyecto.NombreFaceta = value;
                }
            }
        }

        public string FiltroProyectoID
        {
            get
            {
                if (FilaElemento == null)
                {
                    return (string)UtilReflection.GetValueReflection(FilaElementoEntity, "Filtro");
                }
                else if (FilaElemento.Table.Columns.Contains("Filtro"))
                {
                    return (string)FilaElemento["Filtro"];
                }
                else
                {
                    return "";
                }
            }
        }

        public int Orden
        {
            get
            {
                if (FilaElementoEntity is FacetaHome)
                {
                    return ((FacetaHome)FilaElementoEntity).Orden;
                }
                else if (FilaElementoEntity is FacetaFiltroHome)
                {
                    return ((FacetaFiltroHome)FilaElementoEntity).Orden;
                }
                else if (FilaElementoEntity is FacetaConfigProyChart)
                {
                    return ((FacetaConfigProyChart)FilaElementoEntity).Orden;
                }
                else if (FilaElementoEntity is FacetaFiltroProyecto)
                {
                    return ((FacetaFiltroProyecto)FilaElementoEntity).Orden;
                }
                else if (FilaElementoEntity is FacetaObjetoConocimiento)
                {
                    return ((FacetaObjetoConocimiento)FilaElementoEntity).Orden;
                }
                else if (FilaElementoEntity is FacetaObjetoConocimientoProyecto)
                {
                    return ((FacetaObjetoConocimientoProyecto)FilaElementoEntity).Orden;
                }

                return 0;
            }
        }

        public TipoDisenio TipoDisenio
        {
            get
            {
                if (FilaElementoEntity is FacetaFiltroProyecto)
                {
                    return (TipoDisenio)((FacetaFiltroProyecto)FilaElementoEntity).FacetaObjetoConocimientoProyecto.TipoDisenio;
                }
                else if (FilaElementoEntity is FacetaObjetoConocimientoProyecto)
                {
                    return (TipoDisenio)((FacetaObjetoConocimientoProyecto)FilaElementoEntity).TipoDisenio;
                }
                else if (FilaElementoEntity is FacetaObjetoConocimiento)
                {
                    return (TipoDisenio)((FacetaObjetoConocimiento)FilaElementoEntity).TipoDisenio;
                }
                else if (FilaElementoEntity is FacetaFiltroHome)
                {
                    return (TipoDisenio)((FacetaFiltroHome)FilaElementoEntity).FacetaHome.FacetaObjetoConocimientoProyecto.TipoDisenio;
                }
                else if (FilaElementoEntity is FacetaHome)
                {
                    return (TipoDisenio)((FacetaHome)FilaElementoEntity).FacetaObjetoConocimientoProyecto.TipoDisenio;
                }

                return TipoDisenio.ListaMayorAMenor;
            }
            set
            {
                DataRow fila = FilaElemento;
                if (FilaElementoEntity is FacetaFiltroProyecto)
                {
                    ((FacetaFiltroProyecto)FilaElementoEntity).FacetaObjetoConocimientoProyecto.TipoDisenio = (short)value;
                }
                if (FilaElementoEntity is FacetaObjetoConocimientoProyecto)
                {
                    ((FacetaObjetoConocimientoProyecto)FilaElementoEntity).TipoDisenio = (short)value;
                }
                if (FilaElementoEntity is FacetaObjetoConocimiento)
                {
                    ((FacetaObjetoConocimiento)FilaElementoEntity).TipoDisenio = (short)value;
                }
                else if (FilaElementoEntity is FacetaFiltroHome)
                {
                    ((FacetaFiltroHome)FilaElementoEntity).FacetaHome.FacetaObjetoConocimientoProyecto.TipoDisenio = (short)value;
                }
                else if (FilaElementoEntity is FacetaHome)
                {
                    ((FacetaHome)FilaElementoEntity).FacetaObjetoConocimientoProyecto.TipoDisenio = (short)value;
                }
            }
        }

        public TipoPropiedadFaceta TipoPropiedad
        {
            get
            {
                if (FilaElementoEntity is FacetaFiltroProyecto)
                {
                    return (TipoPropiedadFaceta)((FacetaFiltroProyecto)FilaElementoEntity).FacetaObjetoConocimientoProyecto.TipoPropiedad;
                }
                if (FilaElementoEntity is FacetaObjetoConocimientoProyecto)
                {
                    return (TipoPropiedadFaceta)((FacetaObjetoConocimientoProyecto)FilaElementoEntity).TipoPropiedad;
                }
                if (FilaElementoEntity is FacetaObjetoConocimiento)
                {
                    return (TipoPropiedadFaceta)((FacetaObjetoConocimiento)FilaElementoEntity).TipoPropiedad;
                }
                else if (FilaElementoEntity is FacetaFiltroHome)
                {
                    return (TipoPropiedadFaceta)((FacetaFiltroHome)FilaElementoEntity).FacetaHome.FacetaObjetoConocimientoProyecto.TipoPropiedad;
                }
                else if (FilaElementoEntity is FacetaHome)
                {
                    return (TipoPropiedadFaceta)((FacetaHome)FilaElementoEntity).FacetaObjetoConocimientoProyecto.TipoPropiedad;
                }

                return TipoPropiedadFaceta.NULL;
            }
        }

        public short ElementosVisibles
        {
            get
            {
                if (FilaElementoEntity is FacetaFiltroProyecto)
                {
                    return ((FacetaFiltroProyecto)FilaElementoEntity).FacetaObjetoConocimientoProyecto.ElementosVisibles;
                }
                else if (FilaElementoEntity is FacetaObjetoConocimientoProyecto)
                {
                    return ((FacetaObjetoConocimientoProyecto)FilaElementoEntity).ElementosVisibles;
                }
                else if (FilaElementoEntity is FacetaObjetoConocimiento)
                {
                    return ((FacetaObjetoConocimiento)FilaElementoEntity).ElementosVisibles;
                }
                else if (FilaElementoEntity is FacetaFiltroHome)
                {
                    return ((FacetaFiltroHome)FilaElementoEntity).FacetaHome.FacetaObjetoConocimientoProyecto.ElementosVisibles;
                }
                else if (FilaElementoEntity is FacetaHome)
                {
                    return ((FacetaHome)FilaElementoEntity).FacetaObjetoConocimientoProyecto.ElementosVisibles;
                }

                return 0;
            }
            set
            {
                DataRow fila = FilaElemento;
                if (FilaElementoEntity is FacetaFiltroProyecto)
                {
                    ((FacetaFiltroProyecto)FilaElementoEntity).FacetaObjetoConocimientoProyecto.ElementosVisibles = value;
                }
                if (FilaElementoEntity is FacetaObjetoConocimientoProyecto)
                {
                    ((FacetaObjetoConocimientoProyecto)FilaElementoEntity).ElementosVisibles = value;
                }
                if (FilaElementoEntity is FacetaObjetoConocimiento)
                {
                    ((FacetaObjetoConocimiento)FilaElementoEntity).ElementosVisibles = value;
                }
                else if (FilaElementoEntity is FacetaFiltroHome)
                {
                    ((FacetaFiltroHome)FilaElementoEntity).FacetaHome.FacetaObjetoConocimientoProyecto.ElementosVisibles = value;
                }
                else if (FilaElementoEntity is FacetaHome)
                {
                    ((FacetaHome)FilaElementoEntity).FacetaObjetoConocimientoProyecto.ElementosVisibles = value;
                }
            }
        }

        public TiposAlgoritmoTransformacion AlgoritmoTransformacion
        {
            get
            {
                if (FilaElementoEntity is FacetaFiltroProyecto)
                {
                    return (TiposAlgoritmoTransformacion)((FacetaFiltroProyecto)FilaElementoEntity).FacetaObjetoConocimientoProyecto.AlgoritmoTransformacion;
                }
                else if (FilaElementoEntity is FacetaObjetoConocimientoProyecto)
                {
                    return (TiposAlgoritmoTransformacion)((FacetaObjetoConocimientoProyecto)FilaElementoEntity).AlgoritmoTransformacion;
                }
                else if (FilaElementoEntity is FacetaObjetoConocimiento)
                {
                    return (TiposAlgoritmoTransformacion)((FacetaObjetoConocimiento)FilaElementoEntity).AlgoritmoTransformacion;
                }
                else if (FilaElementoEntity is FacetaFiltroHome)
                {
                    return (TiposAlgoritmoTransformacion)((FacetaFiltroHome)FilaElementoEntity).FacetaHome.FacetaObjetoConocimientoProyecto.AlgoritmoTransformacion;
                }
                else if (FilaElementoEntity is FacetaHome)
                {
                    return (TiposAlgoritmoTransformacion)((FacetaHome)FilaElementoEntity).FacetaObjetoConocimientoProyecto.AlgoritmoTransformacion;
                }

                return TiposAlgoritmoTransformacion.Ninguno;
            }
            set
            {
                if (FilaElementoEntity is FacetaFiltroProyecto)
                {
                    ((FacetaFiltroProyecto)FilaElementoEntity).FacetaObjetoConocimientoProyecto.AlgoritmoTransformacion = (short)value;
                }
                else if (FilaElementoEntity is FacetaObjetoConocimientoProyecto)
                {
                    ((FacetaObjetoConocimientoProyecto)FilaElementoEntity).AlgoritmoTransformacion = (short)value;
                }
                else if (FilaElementoEntity is FacetaObjetoConocimiento)
                {
                    ((FacetaObjetoConocimiento)FilaElementoEntity).AlgoritmoTransformacion = (short)value;
                }
                else if (FilaElementoEntity is FacetaFiltroHome)
                {
                    ((FacetaFiltroHome)FilaElementoEntity).FacetaHome.FacetaObjetoConocimientoProyecto.AlgoritmoTransformacion = (short)value;
                }
                else if (FilaElementoEntity is FacetaHome)
                {
                    ((FacetaHome)FilaElementoEntity).FacetaObjetoConocimientoProyecto.AlgoritmoTransformacion = (short)value;
                }
            }
        }

        public string NivelSemantico
        {
            get
            {
                DataRow fila = FilaElemento;
                if (FilaElementoEntity is FacetaFiltroProyecto)
                {
                    return ((FacetaFiltroProyecto)FilaElementoEntity).FacetaObjetoConocimientoProyecto.NivelSemantico;
                }
                if (FilaElementoEntity is FacetaObjetoConocimientoProyecto)
                {
                    return ((FacetaObjetoConocimientoProyecto)FilaElementoEntity).NivelSemantico;
                }
                if (FilaElementoEntity is FacetaFiltroHome)
                {
                    return ((FacetaFiltroHome)FilaElementoEntity).FacetaHome.FacetaObjetoConocimientoProyecto.NivelSemantico;
                }
                if (FilaElementoEntity is FacetaHome)
                {
                    return ((FacetaHome)FilaElementoEntity).FacetaObjetoConocimientoProyecto.NivelSemantico;
                }
                return null;
            }
            set
            {
                DataRow fila = FilaElemento;
                if (FilaElementoEntity is FacetaFiltroProyecto)
                {
                    ((FacetaFiltroProyecto)FilaElementoEntity).FacetaObjetoConocimientoProyecto.NivelSemantico = value;
                }
                if (FilaElementoEntity is FacetaObjetoConocimientoProyecto)
                {
                    ((FacetaObjetoConocimientoProyecto)FilaElementoEntity).NivelSemantico = value;
                }
                if (FilaElementoEntity is FacetaFiltroHome)
                {
                    ((FacetaFiltroHome)FilaElementoEntity).FacetaHome.FacetaObjetoConocimientoProyecto.NivelSemantico = value;
                }
                if (FilaElementoEntity is FacetaHome)
                {
                    ((FacetaHome)FilaElementoEntity).FacetaObjetoConocimientoProyecto.NivelSemantico = value;
                }
            }
        }

        public bool EsSemantica
        {
            get
            {
                DataRow fila = FilaElemento;
                if (FilaElementoEntity is FacetaFiltroProyecto)
                {
                    return ((FacetaFiltroProyecto)FilaElementoEntity).FacetaObjetoConocimientoProyecto.EsSemantica;
                }
                if (FilaElementoEntity is FacetaObjetoConocimientoProyecto)
                {
                    return ((FacetaObjetoConocimientoProyecto)FilaElementoEntity).EsSemantica;
                }
                if (FilaElementoEntity is FacetaObjetoConocimiento)
                {
                    return ((FacetaObjetoConocimiento)FilaElementoEntity).EsSemantica;
                }
                if (FilaElementoEntity is FacetaFiltroHome)
                {
                    return ((FacetaFiltroHome)FilaElementoEntity).FacetaHome.FacetaObjetoConocimientoProyecto.EsSemantica;
                }
                if (FilaElementoEntity is FacetaHome)
                {
                    return ((FacetaHome)FilaElementoEntity).FacetaObjetoConocimientoProyecto.EsSemantica;
                }

                return false;
            }
        }

        public FacetaMayuscula Mayusculas
        {
            get
            {
                DataRow fila = FilaElemento;
                if (FilaElementoEntity is FacetaFiltroProyecto)
                {
                    return (FacetaMayuscula)((FacetaFiltroProyecto)FilaElementoEntity).FacetaObjetoConocimientoProyecto.Mayusculas;

                }
                if (FilaElementoEntity is FacetaObjetoConocimiento)
                {
                    return (FacetaMayuscula)((FacetaObjetoConocimiento)FilaElementoEntity).Mayusculas;
                }
                if (FilaElementoEntity is FacetaObjetoConocimientoProyecto)
                {
                    return (FacetaMayuscula)((FacetaObjetoConocimientoProyecto)FilaElementoEntity).Mayusculas;
                }
                if (FilaElementoEntity is FacetaFiltroHome)
                {
                    return (FacetaMayuscula)((FacetaFiltroHome)FilaElementoEntity).FacetaHome.FacetaObjetoConocimientoProyecto.Mayusculas;
                }
                if (FilaElementoEntity is FacetaHome)
                {
                    return (FacetaMayuscula)((FacetaHome)FilaElementoEntity).FacetaObjetoConocimientoProyecto.Mayusculas;
                }


                return 0;
            }
        }

        public string ObjetoConocimiento
        {
            get
            {
                string salida = "";

                FacetaObjetoConocimientoProyecto filaFacetaObjetoConocimientoProyecto = FilaElementoEntity as FacetaObjetoConocimientoProyecto;

                if (filaFacetaObjetoConocimientoProyecto == null && FilaElementoEntity is FacetaFiltroProyecto)
                {
                    filaFacetaObjetoConocimientoProyecto = ((FacetaFiltroProyecto)FilaElementoEntity).FacetaObjetoConocimientoProyecto;
                }
                if (filaFacetaObjetoConocimientoProyecto == null && FilaElementoEntity is FacetaObjetoConocimiento)
                {
                    salida = ((FacetaObjetoConocimiento)FilaElementoEntity).ObjetoConocimiento;
                }

                if (filaFacetaObjetoConocimientoProyecto != null && filaFacetaObjetoConocimientoProyecto.ObjetoConocimiento != null)
                {
                    salida = filaFacetaObjetoConocimientoProyecto.ObjetoConocimiento;
                }

                return salida;
            }
        }

        public TipoMostrarSoloCaja Comportamiento
        {
            get
            {
                DataRow fila = FilaElemento;
                if (FilaElementoEntity is FacetaFiltroProyecto)
                {
                    return (TipoMostrarSoloCaja)((FacetaFiltroProyecto)FilaElementoEntity).FacetaObjetoConocimientoProyecto.Comportamiento;

                }
                else if (FilaElementoEntity is FacetaObjetoConocimientoProyecto)
                {
                    return (TipoMostrarSoloCaja)((FacetaObjetoConocimientoProyecto)FilaElementoEntity).Comportamiento;
                }
                else if(FilaElementoEntity is FacetaFiltroHome)
                {
                    return (TipoMostrarSoloCaja)((FacetaFiltroHome)FilaElementoEntity).FacetaHome.FacetaObjetoConocimientoProyecto.Comportamiento;
                }
                else if(FilaElementoEntity is FacetaHome)
                {
                    return (TipoMostrarSoloCaja)((FacetaHome)FilaElementoEntity).FacetaObjetoConocimientoProyecto.Comportamiento;
                }

                return 0;
            }
        }

        public bool OcultaEnFacetas
        {
            get
            {
                if (FilaElementoEntity is FacetaFiltroProyecto)
                {
                    return ((FacetaFiltroProyecto)FilaElementoEntity).FacetaObjetoConocimientoProyecto.OcultaEnFacetas;
                }
                else if (FilaElementoEntity is FacetaObjetoConocimientoProyecto)
                {
                    return ((FacetaObjetoConocimientoProyecto)FilaElementoEntity).OcultaEnFacetas;
                }
                else if (FilaElementoEntity is FacetaFiltroHome)
                {
                    return ((FacetaFiltroHome)FilaElementoEntity).FacetaHome.FacetaObjetoConocimientoProyecto.OcultaEnFacetas;
                }
                else if (FilaElementoEntity is FacetaHome)
                {
                    return ((FacetaHome)FilaElementoEntity).FacetaObjetoConocimientoProyecto.OcultaEnFacetas;
                }

                return false;
            }
        }

        public bool OcultaEnFiltros
        {
            get
            {
                DataRow fila = FilaElemento;
                if (FilaElementoEntity is FacetaFiltroProyecto)
                {
                    return ((FacetaFiltroProyecto)FilaElementoEntity).FacetaObjetoConocimientoProyecto.OcultaEnFiltros;
                }
                else if (FilaElementoEntity is FacetaObjetoConocimientoProyecto)
                {
                    return ((FacetaObjetoConocimientoProyecto)FilaElementoEntity).OcultaEnFiltros;
                }
                else if (FilaElementoEntity is FacetaFiltroHome)
                {
                    return ((FacetaFiltroHome)FilaElementoEntity).FacetaHome.FacetaObjetoConocimientoProyecto.OcultaEnFiltros;
                }
                else if (FilaElementoEntity is FacetaHome)
                {
                    return ((FacetaHome)FilaElementoEntity).FacetaObjetoConocimientoProyecto.OcultaEnFiltros;
                }

                return false;
            }
        }

        public bool PriorizarOrdenResultados
        {
            get
            {
                if (FilaElementoEntity is FacetaFiltroProyecto)
                {
                    return ((FacetaFiltroProyecto)FilaElementoEntity).FacetaObjetoConocimientoProyecto.PriorizarOrdenResultados;
                }
                else if (FilaElementoEntity is FacetaObjetoConocimientoProyecto)
                {
                    return ((FacetaObjetoConocimientoProyecto)FilaElementoEntity).PriorizarOrdenResultados;
                }
                else if (FilaElementoEntity is FacetaFiltroHome)
                {
                    return ((FacetaFiltroHome)FilaElementoEntity).FacetaHome.FacetaObjetoConocimientoProyecto.PriorizarOrdenResultados;
                }
                else if (FilaElementoEntity is FacetaHome)
                {
                    return ((FacetaHome)FilaElementoEntity).FacetaObjetoConocimientoProyecto.PriorizarOrdenResultados;
                }

                return false;
            }
        }

        public string PestanyaFaceta
        {
            get
            {
                FacetaHome fila = new FacetaHome();

                if (FilaElementoEntity is FacetaFiltroHome)
                {
                    fila = ((FacetaFiltroHome)(FilaElementoEntity)).FacetaHome;
                }

                string pestanyaFaceta = null;
                if (fila.PestanyaFaceta != null && !string.IsNullOrEmpty(fila.PestanyaFaceta))
                {
                    pestanyaFaceta = fila.PestanyaFaceta;
                }
                return pestanyaFaceta;
            }
        }

        public bool Excluyente
        {
            get
            {
                if (FilaElementoEntity is FacetaFiltroProyecto)
                {
                    return ((FacetaFiltroProyecto)FilaElementoEntity).FacetaObjetoConocimientoProyecto.Excluyente;
                }
                else if (FilaElementoEntity is FacetaObjetoConocimientoProyecto)
                {
                    return ((FacetaObjetoConocimientoProyecto)FilaElementoEntity).Excluyente;
                }
                else if (FilaElementoEntity is FacetaFiltroHome)
                {
                    return ((FacetaFiltroHome)FilaElementoEntity).FacetaHome.FacetaObjetoConocimientoProyecto.Excluyente;
                }
                else if (FilaElementoEntity is FacetaHome)
                {
                    return ((FacetaHome)FilaElementoEntity).FacetaObjetoConocimientoProyecto.Excluyente;
                }

                return false;
            }
        }

        public int Reciproca
        {
            get
            {
                if (FilaElementoEntity is FacetaFiltroProyecto)
                {
                    return ((FacetaFiltroProyecto)FilaElementoEntity).FacetaObjetoConocimientoProyecto.Reciproca;
                }
                else if (FilaElementoEntity is FacetaObjetoConocimientoProyecto)
                {
                    return ((FacetaObjetoConocimientoProyecto)FilaElementoEntity).Reciproca;
                }
                else if (FilaElementoEntity is FacetaFiltroHome)
                {
                    return ((FacetaFiltroHome)FilaElementoEntity).FacetaHome.FacetaObjetoConocimientoProyecto.Reciproca;
                }
                else if (FilaElementoEntity is FacetaHome)
                {
                    return ((FacetaHome)FilaElementoEntity).FacetaObjetoConocimientoProyecto.Reciproca;
                }
                return 0;
            }
        }

        public bool MostrarSoloCaja
        {
            get
            {
                return (Comportamiento.Equals(TipoMostrarSoloCaja.SoloCajaPrimeraPagina) || Comportamiento.Equals(TipoMostrarSoloCaja.SoloCajaSiempre));
            }
        }

        public bool MostrarSoloCajaSiempre
        {
            get
            {
                return Comportamiento.Equals(TipoMostrarSoloCaja.SoloCajaSiempre);
            }
        }

        public bool MostrarVerMas
        {
            get
            {
                if(FilaElementoEntity is FacetaHome)
                {
                    return ((FacetaHome)FilaElementoEntity).MostrarVerMas;
                }

                return false;
            }
            set
            {
                if (FilaElementoEntity is FacetaHome)
                {
                    ((FacetaHome)FilaElementoEntity).MostrarVerMas = value;
                }
            }
        }

        public string FacetaPrivadaParaGrupoEditores
        {
            get
            {
                if (FilaElementoEntity is FacetaFiltroProyecto)
                {
                    return ((FacetaFiltroProyecto)FilaElementoEntity).FacetaObjetoConocimientoProyecto.FacetaPrivadaParaGrupoEditores;
                }
                else if (FilaElementoEntity is FacetaObjetoConocimientoProyecto)
                {
                    return ((FacetaObjetoConocimientoProyecto)FilaElementoEntity).FacetaPrivadaParaGrupoEditores;
                }
                else if (FilaElementoEntity is FacetaFiltroHome)
                {
                    return ((FacetaFiltroHome)FilaElementoEntity).FacetaHome.FacetaObjetoConocimientoProyecto.FacetaPrivadaParaGrupoEditores;
                }
                else if (FilaElementoEntity is FacetaHome)
                {
                    return ((FacetaHome)FilaElementoEntity).FacetaObjetoConocimientoProyecto.FacetaPrivadaParaGrupoEditores;
                }

                return "";
            }
            set
            {
                if(FilaElementoEntity is FacetaFiltroProyecto)
                {
                    ((FacetaFiltroProyecto)FilaElementoEntity).FacetaObjetoConocimientoProyecto.FacetaPrivadaParaGrupoEditores = value;
                }
                if (FilaElementoEntity is FacetaObjetoConocimientoProyecto)
                {
                    ((FacetaObjetoConocimientoProyecto)FilaElementoEntity).FacetaPrivadaParaGrupoEditores = value;
                }
                if (FilaElementoEntity is FacetaFiltroHome)
                {
                    ((FacetaFiltroHome)FilaElementoEntity).FacetaHome.FacetaObjetoConocimientoProyecto.FacetaPrivadaParaGrupoEditores = value;
                }
                if (FilaElementoEntity is FacetaHome)
                {
                    ((FacetaHome)FilaElementoEntity).FacetaObjetoConocimientoProyecto.FacetaPrivadaParaGrupoEditores = value;
                }
            }
        }

        public GestionFacetas GestorFacetas
        {
            get
            {
                return (GestionFacetas)GestorGnoss;
            }
        }

        /// <summary>
        /// Indica si la faceta es multiidioma.
        /// </summary>
        public bool MultiIdioma
        {
            get
            {
                return (AlgoritmoTransformacion == TiposAlgoritmoTransformacion.MultiIdioma || ((FilaElementoEntity is FacetaFiltroProyecto || FilaElementoEntity is FacetaFiltroHome) && !string.IsNullOrEmpty(FiltroProyectoID) && FiltroProyectoID.Contains(";") && FiltroProyectoID.Split(';')[0].Contains("[MultiIdioma]")));
            }
        }


        //necesario añadir el atributo inmutable para hacer las facetas.
        public bool Inmutable
        {
            get
            {
                if(FilaElementoEntity is FacetaFiltroProyecto)
                {
                    return ((FacetaFiltroProyecto)FilaElementoEntity).FacetaObjetoConocimientoProyecto.Inmutable;
                }
                else if (FilaElementoEntity is FacetaObjetoConocimientoProyecto)
                {
                    return ((FacetaObjetoConocimientoProyecto)FilaElementoEntity).Inmutable;
                }
                else if (FilaElementoEntity is FacetaFiltroHome)
                {
                    return ((FacetaFiltroHome)FilaElementoEntity).FacetaHome.FacetaObjetoConocimientoProyecto.Inmutable;
                }
                else if (FilaElementoEntity is FacetaHome)
                {
                    return ((FacetaHome)FilaElementoEntity).FacetaObjetoConocimientoProyecto.Inmutable;
                }

                return false;
            }
            set
            {
                if (FilaElementoEntity is FacetaFiltroProyecto)
                {
                    ((FacetaFiltroProyecto)FilaElementoEntity).FacetaObjetoConocimientoProyecto.Inmutable = value;
                }
                else if (FilaElementoEntity is FacetaObjetoConocimientoProyecto)
                {
                   ((FacetaObjetoConocimientoProyecto)FilaElementoEntity).Inmutable = value;
                }
                else if (FilaElementoEntity is FacetaFiltroHome)
                {
                    ((FacetaFiltroHome)FilaElementoEntity).FacetaHome.FacetaObjetoConocimientoProyecto.Inmutable = value;
                }
                else if (FilaElementoEntity is FacetaHome)
                {
                    ((FacetaHome)FilaElementoEntity).FacetaObjetoConocimientoProyecto.Inmutable = value;
                }
            }
        }

        #endregion

    }
}
