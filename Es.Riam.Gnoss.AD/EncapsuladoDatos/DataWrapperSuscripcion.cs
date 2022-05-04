using Es.Riam.Gnoss.AD.EntityModel.Models.Suscripcion;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.AD.EncapsuladoDatos
{
    [Serializable]
    public class DataWrapperSuscripcion : DataWrapperBase
    {
        public List<CategoriaTesVinSuscrip> ListaCategoriaTesVinSuscrip { get; set; }
        public List<EntityModel.Models.Suscripcion.Suscripcion> ListaSuscripcion { get; set; }
        public List<SuscripcionBlog> ListaSuscripcionBlog { get; set; }
        public List<SuscripcionIdentidadProyecto> ListaSuscripcionIdentidadProyecto { get; set; }
        public List<SuscripcionTesauroOrganizacion> ListaSuscripcionTesauroOrganizacion { get; set; }
        public List<SuscripcionTesauroProyecto> ListaSuscripcionTesauroProyecto { get; set; }
        public List<SuscripcionTesauroUsuario> ListaSuscripcionTesauroUsuario { get; set; }
        public List<SuscripcionTesauroProyectoProyecto> ListaSuscripcionTesauroProyectoProyecto { get; set; }

        public DataWrapperSuscripcion()
        {
            ListaCategoriaTesVinSuscrip = new List<CategoriaTesVinSuscrip>();
            ListaSuscripcion = new List<EntityModel.Models.Suscripcion.Suscripcion>();
            ListaSuscripcionBlog = new List<SuscripcionBlog>();
            ListaSuscripcionIdentidadProyecto = new List<SuscripcionIdentidadProyecto>();
            ListaSuscripcionTesauroOrganizacion = new List<SuscripcionTesauroOrganizacion>();
            ListaSuscripcionTesauroProyecto = new List<SuscripcionTesauroProyecto>();
            ListaSuscripcionTesauroUsuario = new List<SuscripcionTesauroUsuario>();
            ListaSuscripcionTesauroProyectoProyecto = new List<SuscripcionTesauroProyectoProyecto>();
        }

        public override void Merge(DataWrapperBase pDataWrapper)
        {
            DataWrapperSuscripcion dataWrapperSuscripcion = (DataWrapperSuscripcion)pDataWrapper;

            ListaCategoriaTesVinSuscrip = ListaCategoriaTesVinSuscrip.Union(dataWrapperSuscripcion.ListaCategoriaTesVinSuscrip).ToList();
            ListaSuscripcion = ListaSuscripcion.Union(dataWrapperSuscripcion.ListaSuscripcion).ToList();
            ListaSuscripcionBlog = ListaSuscripcionBlog.Union(dataWrapperSuscripcion.ListaSuscripcionBlog).ToList();
            ListaSuscripcionIdentidadProyecto = ListaSuscripcionIdentidadProyecto.Union(dataWrapperSuscripcion.ListaSuscripcionIdentidadProyecto).ToList();
            ListaSuscripcionTesauroOrganizacion = ListaSuscripcionTesauroOrganizacion.Union(dataWrapperSuscripcion.ListaSuscripcionTesauroOrganizacion).ToList();
            ListaSuscripcionTesauroProyecto = ListaSuscripcionTesauroProyecto.Union(dataWrapperSuscripcion.ListaSuscripcionTesauroProyecto).ToList();
            ListaSuscripcionTesauroUsuario = ListaSuscripcionTesauroUsuario.Union(dataWrapperSuscripcion.ListaSuscripcionTesauroUsuario).ToList();
            ListaSuscripcionTesauroProyectoProyecto = ListaSuscripcionTesauroProyectoProyecto.Union(dataWrapperSuscripcion.ListaSuscripcionTesauroProyectoProyecto).ToList();
        }
    }
}
