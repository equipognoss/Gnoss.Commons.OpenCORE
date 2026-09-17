using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.CMS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.Elementos.ParametroGeneralDSEspacio;
using Es.Riam.Gnoss.Elementos.ParametroGeneralDSName;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.Web.Controles.ParametroGeneralDSName
{

    public class ParametroGeneralGBD
    {
        private EntityContext mEntityContext;

        public ParametroGeneralGBD(EntityContext entityContext)
        {
            mEntityContext = entityContext;
        }

        public List<ParametroGeneral> CargarParametroGeneral()
        {
            return mEntityContext.ParametroGeneral.ToList();
        }

        public void EliminarParametroGeneral(ParametroGeneral pFila)
        {
            mEntityContext.ParametroGeneral.Remove(pFila);
            mEntityContext.SaveChanges();
        }

        public EntityState GetState(object entry)
        {
            return mEntityContext.Entry(entry).State;
        }

        public GestorParametroGeneral ObtenerParametrosGeneralesDeProyecto(GestorParametroGeneral pGestor, Guid pProyectoID)
        {
            pGestor.ListaProyectoMetaRobots = mEntityContext.ProyectoMetaRobots.Where(proyectoMetaRobots => proyectoMetaRobots.ProyectoID.Equals(pProyectoID)).ToList();
            pGestor.ListaConfiguracionAmbitoBusquedaProyecto = mEntityContext.ConfiguracionAmbitoBusquedaProyecto.Where(ambitoBusqueda => ambitoBusqueda.ProyectoID.Equals(pProyectoID)).ToList();
            pGestor.ListaParametroGeneral = mEntityContext.ParametroGeneral.Where(parametroGeneral => parametroGeneral.ProyectoID.Equals(pProyectoID)).ToList();
            return pGestor;
        }


        public GestorParametroGeneral ObtenerFilaParametrosGeneralesDeProyecto(GestorParametroGeneral pGestor, Guid pProyectoID)
        {
            pGestor.ListaParametroGeneral.Clear();
            pGestor.ListaProyectoElementoHtml.Clear();
            pGestor.ListaProyectoMetaRobots.Clear();
            pGestor.ListaParametroGeneral.Add(mEntityContext.ParametroGeneral.Where(parametroG => parametroG.ProyectoID.Equals(pProyectoID)).FirstOrDefault());
            pGestor.ListaProyectoElementoHtml.Add(mEntityContext.ProyectoElementoHtml.Where(parametroG => parametroG.ProyectoID.Equals(pProyectoID)).FirstOrDefault());
            pGestor.ListaProyectoMetaRobots.Add(mEntityContext.ProyectoMetaRobots.Where(parametroG => parametroG.ProyectoID.Equals(pProyectoID)).FirstOrDefault());
            return pGestor;
        }

        public List<TextosPersonalizadosPersonalizacion> ObtenerTextosPersonalizadosPersonalizacion(Guid pProyectoID)
        {
            return mEntityContext.TextosPersonalizadosPersonalizacion.Join(mEntityContext.VistaVirtualProyecto, textoPersonalizacion => textoPersonalizacion.PersonalizacionID, vistaVirtualProyecto => vistaVirtualProyecto.PersonalizacionID, (textoPersonalizacion, textoProyecto) => new
            {
                ProyectoID = textoProyecto.ProyectoID,
                TextosPersonalizadosPersonalizacion = textoPersonalizacion
            }).Where(textoPersonalizacion => textoPersonalizacion.ProyectoID.Equals(pProyectoID)).Select(objeto => objeto.TextosPersonalizadosPersonalizacion).ToList();
        }

        public List<ParametroProyecto> ObtenerParametroProyecto(Guid pProyectoID)
        {
            return mEntityContext.ParametroProyecto.Where(param => param.ProyectoID.Equals(pProyectoID)).ToList();
        }

        public TextosPersonalizadosPersonalizacion GetTextoPersonalizado(Guid pPersonalizacionID, string pTextoID, string pIdioma)
        {
            return mEntityContext.TextosPersonalizadosPersonalizacion.FirstOrDefault(textoPersonalizado => textoPersonalizado.PersonalizacionID.Equals(pPersonalizacionID) && textoPersonalizado.TextoID.Equals(pTextoID) && textoPersonalizado.Language.Equals(pIdioma));
        }

        public GestorParametroGeneral ObtenerParametrosGeneralesDeProyectoConIdiomas(GestorParametroGeneral pGestor, Guid pProyectoID)
        {
            pGestor.ListaParametroGeneral = mEntityContext.ParametroGeneral.Where(parametroGeneral => parametroGeneral.ProyectoID.Equals(pProyectoID)).ToList();
            pGestor.ListaConfiguracionAmbitoBusquedaProyecto = mEntityContext.ConfiguracionAmbitoBusquedaProyecto.Where(confAmbitoBusquedaProy => confAmbitoBusquedaProy.ProyectoID.Equals(pProyectoID)).ToList();
            pGestor.ListaTextosPersonalizadosProyecto = mEntityContext.TextosPersonalizadosProyecto.Where(textosPersonalizadosProy => textosPersonalizadosProy.ProyectoID.Equals(pProyectoID)).ToList();
            pGestor.ListaProyectoMetaRobots = mEntityContext.ProyectoMetaRobots.Where(proyectoMetaRobots => proyectoMetaRobots.ProyectoID.Equals(pProyectoID)).ToList();
            
            return pGestor;
        }

        public ConfiguracionAmbitoBusquedaProyecto ObtenerConfiguracionAmbitoBusqueda(Guid pProyectoID)
        {
            return mEntityContext.ConfiguracionAmbitoBusquedaProyecto.Where(confBusqueda => confBusqueda.ProyectoID.Equals(pProyectoID)).FirstOrDefault();
        }

        public List<CMSComponentePrivadoProyecto> ObtenerCMSComponentePrivadoProyecto(GestorParametroGeneral pGestor, Guid pProyectoID)
        {
            return mEntityContext.CMSComponentePrivadoProyecto.Where(cmsComponente => cmsComponente.ProyectoID.Equals(pProyectoID)).ToList();
        }

        public void NuevoParametroProyecto(ParametroProyecto parametro)
        {
            mEntityContext.ParametroProyecto.Add(parametro);
        }

        /// <summary>
        /// Actualiza el valor de un parámetro de proyecto. Si la fila recibida está desvinculada del contexto
        /// (por ejemplo, porque se ha construido en memoria a partir del diccionario de parámetros del proyecto),
        /// se recupera la fila real de base de datos para que Entity Framework detecte el cambio.
        /// </summary>
        /// <param name="pParametro">Fila del parámetro a actualizar</param>
        /// <param name="pValor">Nuevo valor del parámetro</param>
        public void ActualizarParametroProyecto(ParametroProyecto pParametro, string pValor)
        {
            ParametroProyecto parametroBD = pParametro;

            if (mEntityContext.Entry(pParametro).State == EntityState.Detached)
            {
                parametroBD = mEntityContext.ParametroProyecto.FirstOrDefault(param => param.OrganizacionID.Equals(pParametro.OrganizacionID) && param.ProyectoID.Equals(pParametro.ProyectoID) && param.Parametro.Equals(pParametro.Parametro));
            }

            pParametro.Valor = pValor;

            if (parametroBD == null)
            {
                // La fila no existe en base de datos, la creo con el nuevo valor
                mEntityContext.ParametroProyecto.Add(pParametro);
            }
            else
            {
                parametroBD.Valor = pValor;
            }
        }

        public void EliminarParametroProyecto(ParametroProyecto parametro)
        {
            if (mEntityContext.Entry(parametro).State == EntityState.Detached)
            {
                parametro = mEntityContext.ParametroProyecto.FirstOrDefault(param => param.OrganizacionID.Equals(parametro.OrganizacionID) && param.ProyectoID.Equals(parametro.ProyectoID) && param.Parametro.Equals(parametro.Parametro));
            }
            if (parametro != null)
            {
                mEntityContext.EliminarElemento(parametro);
            }
        }

        public void SaveChanges()
        {
            mEntityContext.SaveChanges();
        }

        public EntityState ObtenerEstado(object objetoBD)
        {
            return mEntityContext.Entry(objetoBD).State;
        }

        public TextosPersonalizadosPersonalizacion ObtenerTextoPersonalizadoPersonalizacion(Guid pPersonalizacionID, string pTextoID, string pIdioma)
        {
            return mEntityContext.TextosPersonalizadosPersonalizacion.FirstOrDefault(textoPersonalizado => textoPersonalizado.PersonalizacionID.Equals(pPersonalizacionID) && textoPersonalizado.TextoID.Equals(pTextoID) && textoPersonalizado.Language.Equals(pIdioma));
        }

        internal void addAmbitoBusqueda(ConfiguracionAmbitoBusquedaProyecto pFilaAmbitoBusqueda)
        {
            mEntityContext.ConfiguracionAmbitoBusquedaProyecto.Add(pFilaAmbitoBusqueda);
        }

        public void AddTextosPersonalizadosPersonalizacion(TextosPersonalizadosPersonalizacion pTextoPersonalizado)
        {
            mEntityContext.TextosPersonalizadosPersonalizacion.Add(pTextoPersonalizado);
        }

        public void DeleteTextoPersonalizadoPersonalizacion(TextosPersonalizadosPersonalizacion pFilaTextoPersonalizado, bool pPeticionIntegracionContinua = false)
        {
            mEntityContext.EliminarElemento(pFilaTextoPersonalizado, pPeticionIntegracionContinua);
        }
    }
}
