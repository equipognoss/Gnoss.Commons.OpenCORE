using Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.AD.EncapsuladoDatos
{
    [Serializable]
    public class DataWrapperIdentidad : DataWrapperBase
    {
        public List<PerfilGadget> ListaPerfilGadget { get; set; }
        public List<GrupoIdentidadesAutocompletado> ListagrupoIdentidadesAutocompletado { get; set; }

        public List<Perfil> ListaPerfil;

        public List<UsuarioPerfilIdentidad> ListaUsuarioPerfilIdentidad { get; set; }

        public List<IdentidadPerfil> ListaIdentidadPerfil;

        public List<PerfilPersonaOrg> ListaPerfilPersonaOrg;

        public List<EntityModel.Models.IdentidadDS.Identidad> ListaIdentidad;

        public List<PerfilRedesSociales> ListaPerfilRedesSociales;
        public List<PerfilRedesSociales> ListaPerfilRedesSocialesOrganizacion;
        public List<PerfilPersona> ListaPerfilPersona;

        public List<PerfilOrganizacion> ListaPerfilOrganizacion;

        public List<Profesor> ListaProfesor;

        public List<DatoExtraProyectoOpcionIdentidad> ListaDatoExtraProyectoOpcionIdentidad;

        public List<DatoExtraProyectoVirtuosoIdentidad> ListaDatoExtraProyectoVirtuosoIdentidad;

        public List<DatoExtraEcosistemaOpcionPerfil> ListaDatoExtraEcosistemaOpcionPerfil;

        public List<DatoExtraEcosistemaVirtuosoPerfil> ListaDatoExtraEcosistemaVirtuosoPerfil;

        public List<GrupoIdentidades> ListaGrupoIdentidades;
        public List<GrupoIdentidadesEnvio> ListaGrupoIdentidadesEnvio;

        public List<GrupoIdentidadesOrganizacion> ListaGrupoIdentidadesOrganizacion;

        public List<GrupoIdentidadesParticipacion> ListaGrupoIdentidadesParticipacion;

        public List<GrupoIdentidadesProyecto> ListaGrupoIdentidadesProyecto;

        public List<IdentidadContadores> ListaIdentidadContadores;

        public override void Merge(DataWrapperBase pDataWrapper)
        {
            DataWrapperIdentidad dataWrapperIdentidad = (DataWrapperIdentidad)pDataWrapper;

            ListaPerfilGadget = ListaPerfilGadget.Union(dataWrapperIdentidad.ListaPerfilGadget).ToList();
            ListaDatoExtraEcosistemaOpcionPerfil = ListaDatoExtraEcosistemaOpcionPerfil.Union(dataWrapperIdentidad.ListaDatoExtraEcosistemaOpcionPerfil).ToList();
            ListaDatoExtraEcosistemaVirtuosoPerfil = ListaDatoExtraEcosistemaVirtuosoPerfil.Union(dataWrapperIdentidad.ListaDatoExtraEcosistemaVirtuosoPerfil).ToList();
            ListaUsuarioPerfilIdentidad = ListaUsuarioPerfilIdentidad.Union(dataWrapperIdentidad.ListaUsuarioPerfilIdentidad).ToList();
            ListaDatoExtraProyectoOpcionIdentidad = ListaDatoExtraProyectoOpcionIdentidad.Union(dataWrapperIdentidad.ListaDatoExtraProyectoOpcionIdentidad).ToList();
            ListaDatoExtraProyectoVirtuosoIdentidad = ListaDatoExtraProyectoVirtuosoIdentidad.Union(dataWrapperIdentidad.ListaDatoExtraProyectoVirtuosoIdentidad).ToList();
            ListaGrupoIdentidades = ListaGrupoIdentidades.Union(dataWrapperIdentidad.ListaGrupoIdentidades).ToList();
            ListaGrupoIdentidadesEnvio = ListaGrupoIdentidadesEnvio.Union(dataWrapperIdentidad.ListaGrupoIdentidadesEnvio).ToList();
            ListaGrupoIdentidadesOrganizacion = ListaGrupoIdentidadesOrganizacion.Union(dataWrapperIdentidad.ListaGrupoIdentidadesOrganizacion).ToList();
            ListaGrupoIdentidadesParticipacion = ListaGrupoIdentidadesParticipacion.Union(dataWrapperIdentidad.ListaGrupoIdentidadesParticipacion).ToList();
            ListaIdentidad = ListaIdentidad.Union(dataWrapperIdentidad.ListaIdentidad).ToList();
            ListaPerfil = ListaPerfil.Union(dataWrapperIdentidad.ListaPerfil).ToList();
            ListaPerfilOrganizacion = ListaPerfilOrganizacion.Union(dataWrapperIdentidad.ListaPerfilOrganizacion).ToList();
            ListaPerfilPersona = ListaPerfilPersona.Union(dataWrapperIdentidad.ListaPerfilPersona).ToList();
            ListaPerfilPersonaOrg = ListaPerfilPersonaOrg.Union(dataWrapperIdentidad.ListaPerfilPersonaOrg).ToList();
            ListaPerfilRedesSociales = ListaPerfilRedesSociales.Union(dataWrapperIdentidad.ListaPerfilRedesSociales).ToList();
            ListaProfesor = ListaProfesor.Union(dataWrapperIdentidad.ListaProfesor).ToList();
            ListaGrupoIdentidadesProyecto = ListaGrupoIdentidadesProyecto.Union(dataWrapperIdentidad.ListaGrupoIdentidadesProyecto).ToList();
            ListaIdentidadContadores = ListaIdentidadContadores.Union(dataWrapperIdentidad.ListaIdentidadContadores).ToList();
            ListaIdentidadPerfil = ListaIdentidadPerfil.Union(dataWrapperIdentidad.ListaIdentidadPerfil).ToList();
        }
        public DataWrapperIdentidad()
        {
            ListaPerfil = new List<Perfil>();
            ListaPerfilGadget = new List<PerfilGadget>();
            ListaPerfilPersonaOrg = new List<PerfilPersonaOrg>();
            ListaIdentidad = new List<EntityModel.Models.IdentidadDS.Identidad>();
            ListaUsuarioPerfilIdentidad = new List<UsuarioPerfilIdentidad>();
            ListaPerfilRedesSociales = new List<PerfilRedesSociales>();
            ListaPerfilRedesSocialesOrganizacion = new List<PerfilRedesSociales>();
            ListaPerfilPersona = new List<PerfilPersona>();
            ListaPerfilOrganizacion = new List<PerfilOrganizacion>();
            ListaProfesor = new List<Profesor>();
            ListaDatoExtraProyectoOpcionIdentidad = new List<DatoExtraProyectoOpcionIdentidad>();
            ListaDatoExtraProyectoVirtuosoIdentidad = new List<DatoExtraProyectoVirtuosoIdentidad>();
            ListaDatoExtraEcosistemaOpcionPerfil = new List<DatoExtraEcosistemaOpcionPerfil>();
            ListaDatoExtraEcosistemaVirtuosoPerfil = new List<DatoExtraEcosistemaVirtuosoPerfil>();
            ListaGrupoIdentidades = new List<GrupoIdentidades>();
            ListaGrupoIdentidadesEnvio = new List<GrupoIdentidadesEnvio>();
            ListaGrupoIdentidadesOrganizacion = new List<GrupoIdentidadesOrganizacion>();
            ListaGrupoIdentidadesParticipacion = new List<GrupoIdentidadesParticipacion>();
            ListaGrupoIdentidadesProyecto = new List<GrupoIdentidadesProyecto>();
            ListaIdentidadContadores = new List<IdentidadContadores>();
            ListaIdentidadPerfil = new List<IdentidadPerfil>();
        }

        public void CargaRelacionesPerezosasCache()
        {
            foreach(EntityModel.Models.IdentidadDS.Identidad identidad in ListaIdentidad)
            {
                identidad.Perfil = ListaPerfil.FirstOrDefault(perfil => perfil.PerfilID.Equals(identidad.PerfilID));
                identidad.GrupoIdentidadesParticipacion = ListaGrupoIdentidadesParticipacion.Where(grupoIden => grupoIden.Identidad != null && grupoIden.Identidad.Equals(identidad.IdentidadID)).ToList();
                identidad.IdentidadContadores = ListaIdentidadContadores.FirstOrDefault(idenCont => idenCont.IdentidadID.Equals(identidad.IdentidadID));
            }

            foreach(IdentidadContadores identidadContadores in ListaIdentidadContadores)
            {
                identidadContadores.Identidad = ListaIdentidad.FirstOrDefault(identidad => identidad.IdentidadID.Equals(identidadContadores.IdentidadID));
            }

            foreach(Perfil perfil in ListaPerfil)
            {
                perfil.Identidad = ListaIdentidad.Where(identidad => identidad.PerfilID.Equals(perfil.PerfilID)).ToList();
                perfil.Profesor = ListaProfesor.Where(profesor => profesor.PerfilID.Equals(perfil.PerfilID)).ToList();
            }

            foreach(PerfilPersonaOrg perfilPersonaOrg in ListaPerfilPersonaOrg)
            {
                perfilPersonaOrg.Perfil = ListaPerfil.FirstOrDefault(perf => perf.PerfilID.Equals(perfilPersonaOrg.PerfilID));
            }
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~DataWrapperIdentidad()
        {
            //Libero los recursos
            Dispose(false);
        }

        /// <summary>
        /// Determina si está disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Libera los recursos
        /// </summary>
        /// <param name="disposing">Determina si se está llamando desde el Dispose()</param>
        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                disposed = true;
                try
                {
                    if (disposing)
                    {
                        if (ListaDatoExtraEcosistemaOpcionPerfil != null)
                            ListaDatoExtraEcosistemaOpcionPerfil.Clear();

                        if (ListaDatoExtraEcosistemaVirtuosoPerfil != null)
                            ListaDatoExtraEcosistemaVirtuosoPerfil.Clear();

                        if (ListaDatoExtraProyectoOpcionIdentidad != null)
                            ListaDatoExtraProyectoOpcionIdentidad.Clear();

                        if (ListaDatoExtraProyectoVirtuosoIdentidad != null)
                            ListaDatoExtraProyectoVirtuosoIdentidad.Clear();

                        if (ListaGrupoIdentidades != null)
                            ListaGrupoIdentidades.Clear();

                        if (ListaGrupoIdentidadesOrganizacion != null)
                            ListaGrupoIdentidadesOrganizacion.Clear();

                        if (ListaGrupoIdentidadesParticipacion != null)
                            ListaGrupoIdentidadesParticipacion.Clear();

                        if (ListaGrupoIdentidadesProyecto != null)
                            ListaGrupoIdentidadesProyecto.Clear();

                        if (ListaIdentidad != null)
                            ListaIdentidad.Clear();

                        if (ListaIdentidadContadores != null)
                            ListaIdentidadContadores.Clear();

                        if (ListaIdentidadPerfil != null)
                            ListaIdentidadPerfil.Clear();

                        if (ListaPerfil != null)
                            ListaPerfil.Clear();

                        if (ListaPerfilGadget != null)
                            ListaPerfilGadget.Clear();

                        if (ListaPerfilOrganizacion != null)
                            ListaPerfilOrganizacion.Clear();

                        if (ListaPerfilPersona != null)
                            ListaPerfilPersona.Clear();

                        if (ListaPerfilPersonaOrg != null)
                            ListaPerfilPersonaOrg.Clear();

                        if (ListaPerfilRedesSociales != null)
                            ListaPerfilRedesSociales.Clear();

                        if (ListaPerfilRedesSocialesOrganizacion != null)
                            ListaPerfilRedesSocialesOrganizacion.Clear();

                        if (ListaProfesor != null)
                            ListaProfesor.Clear();

                        if (ListaUsuarioPerfilIdentidad != null)
                            ListaUsuarioPerfilIdentidad.Clear();
                    }
                }
                finally
                {
                    ListaDatoExtraEcosistemaOpcionPerfil = null;
                    ListaDatoExtraEcosistemaVirtuosoPerfil = null;
                    ListaDatoExtraProyectoOpcionIdentidad = null;
                    ListaDatoExtraProyectoVirtuosoIdentidad = null;
                    ListaGrupoIdentidades = null;
                    ListaGrupoIdentidadesOrganizacion = null;
                    ListaGrupoIdentidadesParticipacion = null;
                    ListaGrupoIdentidadesProyecto = null;
                    ListaIdentidad = null;
                    ListaIdentidadContadores = null;
                    ListaIdentidadPerfil = null;
                    ListaPerfil = null;
                    ListaPerfilGadget = null;
                    ListaPerfilOrganizacion = null;
                    ListaPerfilPersona = null;
                    ListaPerfilPersonaOrg = null;
                    ListaPerfilRedesSociales = null;
                    ListaPerfilRedesSocialesOrganizacion = null;
                    ListaProfesor = null;
                    ListaUsuarioPerfilIdentidad = null;

                    // Llamo al dispose de la clase base
                    base.Dispose(disposing);
                }
            }
        }

    }

}
