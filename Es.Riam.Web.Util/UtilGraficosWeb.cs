namespace Es.Riam.Gnoss.Web.Util
{
    /// <summary>
    /// Descripción breve de Util
    /// </summary>
    public class UtilGraficosWeb
    {
        #region Constantes

        #region Iconos

        private const string ICONO_ESTRUCTURA = "images/iconos/organizacion.ico";
        private const string ICONO_METAESTRUCTURA = "images/iconos/organizacion.ico";
        private const string ICONO_SUBESTRUCTURA = "images/iconos/subestructura16.ico";
        private const string ICONO_FORMA = "images/iconos/forma16.ico";
        private const string ICONO_GRUPO = "images/iconos/grupo16.ico";
        private const string ICONO_FIGURA = "images/iconos/figura16.ico";
        private const string ICONO_OCUPACION = "images/iconos/empleado16.ico";

        private const string ICONO_TIPOPROCESO = "images/iconos/tipoproceso16.ico";
        private const string ICONO_PROCESO = "images/iconos/proceso16.ico";
        private const string ICONO_PERSPECTIVA = "images/iconos/perspectiva16.ico";
        private const string ICONO_OBJETIVO = "images/iconos/objetivo16.ico";

        private const string ICONO_COMPETENCIA = "images/iconos/libro16.ico";

        #endregion

        #endregion

        #region Métodos estáticos

        public static string ObtenerIcono(object pElemento)
        {
            //TODO JUAN: Comento esto para quitar la referencia del AD al proyecto Es.Riam.Web.Util 
            //if (pElemento is Forma)
            {
                //switch (((Forma)pElemento).FilaForma.TipoForma)
                //{
                //    case (short)TiposForma.Forma:
                //        {
                //            return ICONO_FORMA;
                //        }
                //    case (short)TiposForma.Grupo:
                //        {
                //            return ICONO_GRUPO;
                //        }
                //    case (short)TiposForma.Figura:
                //        {
                //            return ICONO_FIGURA;
                //        }
                //    default:
                //        {
                //            return ICONO_FORMA;
                //        }
                //}
                //return ICONO_FORMA;
            }
            return ObtenerIcono(pElemento.GetType().Name);
        }

        public static string ObtenerIcono(string pTipo)
        {
            switch (pTipo)
            {
                case "Estructura":
                    {
                        return ICONO_ESTRUCTURA;
                    }
                case "MetaEstructura":
                    {
                        return ICONO_METAESTRUCTURA;
                    }
                case "SubEstructura":
                    {
                        return ICONO_SUBESTRUCTURA;
                    }
                case "Forma":
                    {
                        return ICONO_FORMA;
                    }
                case "Grupo":
                    {
                        return ICONO_GRUPO;
                    }
                case "Figura":
                    {
                        return ICONO_FIGURA;
                    }
                case "Ocupacion":
                    {
                        return ICONO_OCUPACION;
                    }
                case "TipoProceso":
                    {
                        return ICONO_TIPOPROCESO;
                    }
                case "Proceso":
                    {
                        return ICONO_PROCESO;
                    }
                case "Perspectiva":
                    {
                        return ICONO_PERSPECTIVA;
                    }
                case "Objetivo":
                    {
                        return ICONO_OBJETIVO;
                    }
                case "Competencia":
                    {
                        return ICONO_COMPETENCIA;
                    }
                default:
                    {
                        //TODO: ""
                        return ICONO_ESTRUCTURA;
                    }
            }
        }

        #endregion
    }
}
