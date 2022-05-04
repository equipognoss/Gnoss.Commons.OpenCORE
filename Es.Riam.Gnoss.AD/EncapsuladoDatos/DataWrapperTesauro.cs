using Es.Riam.Gnoss.AD.EntityModel.Models.Tesauro;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.AD.EncapsuladoDatos
{
    [Serializable]
    public class DataWrapperTesauro : DataWrapperBase
    {
        public List<EntityModel.Models.Tesauro.Tesauro> ListaTesauro { get; set; }
        public List<TesauroOrganizacion> ListaTesauroOrganizacion { get; set; }
        public List<TesauroProyecto> ListaTesauroProyecto { get; set; }
        public List<TesauroUsuario> ListaTesauroUsuario { get; set; }
        public List<CategoriaTesauro> ListaCategoriaTesauro { get; set; }
        public List<CatTesauroAgCatTesauro> ListaCatTesauroAgCatTesauro { get; set; }
        public List<CategoriaTesauroPropiedades> ListaCategoriaTesauroPropiedades { get; set; }
        public List<CategoriaTesauroSugerencia> ListaCategoriaTesauroSugerencia { get; set; }
        public List<CatTesauroCompartida> ListaCatTesauroCompartida { get; set; }
        public List<CatTesauroPermiteTipoRec> ListaCatTesauroPermiteTipoRec { get; set; }


        public DataWrapperTesauro()
        {
            ListaTesauro = new List<EntityModel.Models.Tesauro.Tesauro>();
            ListaTesauroOrganizacion = new List<TesauroOrganizacion>();
            ListaTesauroProyecto = new List<TesauroProyecto>();
            ListaTesauroUsuario = new List<TesauroUsuario>();
            ListaCategoriaTesauro = new List<CategoriaTesauro>();
            ListaCatTesauroAgCatTesauro = new List<CatTesauroAgCatTesauro>();
            ListaCategoriaTesauroPropiedades = new List<CategoriaTesauroPropiedades>();
            ListaCategoriaTesauroSugerencia = new List<CategoriaTesauroSugerencia>();
            ListaCatTesauroCompartida = new List<CatTesauroCompartida>();
            ListaCatTesauroPermiteTipoRec = new List<CatTesauroPermiteTipoRec>();
        }

        public override void Merge(DataWrapperBase pDataWrapper)
        {
            DataWrapperTesauro dataWrapperTesauro = (DataWrapperTesauro)pDataWrapper;

            this.ListaTesauro = this.ListaTesauro.Union(dataWrapperTesauro.ListaTesauro).ToList();
            this.ListaTesauroOrganizacion = this.ListaTesauroOrganizacion.Union(dataWrapperTesauro.ListaTesauroOrganizacion).ToList();
            this.ListaTesauroProyecto = this.ListaTesauroProyecto.Union(dataWrapperTesauro.ListaTesauroProyecto).ToList();
            this.ListaTesauroUsuario = this.ListaTesauroUsuario.Union(dataWrapperTesauro.ListaTesauroUsuario).ToList();
            this.ListaCategoriaTesauro = this.ListaCategoriaTesauro.Union(dataWrapperTesauro.ListaCategoriaTesauro).ToList();
            this.ListaCatTesauroAgCatTesauro = this.ListaCatTesauroAgCatTesauro.Union(dataWrapperTesauro.ListaCatTesauroAgCatTesauro).ToList();
            this.ListaCategoriaTesauroPropiedades = this.ListaCategoriaTesauroPropiedades.Union(dataWrapperTesauro.ListaCategoriaTesauroPropiedades).ToList();
            this.ListaCategoriaTesauroSugerencia = this.ListaCategoriaTesauroSugerencia.Union(dataWrapperTesauro.ListaCategoriaTesauroSugerencia).ToList();
            this.ListaCatTesauroCompartida = this.ListaCatTesauroCompartida.Union(dataWrapperTesauro.ListaCatTesauroCompartida).ToList();
            this.ListaCatTesauroPermiteTipoRec = this.ListaCatTesauroPermiteTipoRec.Union(dataWrapperTesauro.ListaCatTesauroPermiteTipoRec).ToList();

        }

        public void CargaRelacionesPerezosasCache()
        {
           foreach(TesauroOrganizacion tesuaroOrg in ListaTesauroOrganizacion)
            {
                tesuaroOrg.Tesauro = ListaTesauro.FirstOrDefault(tesauro => tesauro.TesauroID.Equals(tesuaroOrg.TesauroID));
            }

           foreach(TesauroUsuario tesUs in ListaTesauroUsuario)
            {
                tesUs.Tesauro = ListaTesauro.FirstOrDefault(tesauro => tesauro.TesauroID.Equals(tesUs.TesauroID));
            }

           foreach(CatTesauroAgCatTesauro agCat in ListaCatTesauroAgCatTesauro)
            {
                agCat.CategoriaTesauro = ListaCategoriaTesauro.FirstOrDefault(catTes => catTes.TesauroID.Equals(agCat.TesauroID) && catTes.CategoriaTesauroID.Equals(agCat.CategoriaInferiorID));
            }
        }
    }
}
