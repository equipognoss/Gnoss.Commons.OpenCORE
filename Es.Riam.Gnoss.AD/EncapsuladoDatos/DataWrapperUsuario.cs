using System;
using System.Collections.Generic;
using System.Linq;
using Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS;
using Es.Riam.Gnoss.AD.Usuarios.Model;

namespace Es.Riam.Gnoss.AD.EncapsuladoDatos
{
    [Serializable]
    public class DataWrapperUsuario: DataWrapperBase
    {
        public List<EntityModel.Models.UsuarioDS.Usuario> ListaUsuario;
        public List<ProyectoRolUsuario> ListaProyectoRolUsuario;
        public List<ProyectoUsuarioIdentidad> ListaProyectoUsuarioIdentidad;
        public List<GeneralRolUsuario> ListaGeneralRolUsuario;
        public List<AdministradorGeneral> ListaAdministradorGeneral;
        public List<InicioSesion> ListaInicioSesion;
        public List<ClausulaRegistro> ListaClausulaRegistro;
        public List<ProyRolUsuClausulaReg> ListaProyRolUsuClausulaReg;
        public List<ProyectoRolGrupoUsuario> ListaProyectoRolGrupoUsuario;
        //Agregadas para solucionar errores
        public List<UsuarioIdentidadPersona> ListaUsuarioIdentidadPersona;
        public List<OrganizacionRolUsuario> ListaOrganizacionRolUsuario;
        public List<GrupoUsuarioUsuario> ListaGrupoUsuarioUsuario;
        public List<UsuarioVinculadoLoginRedesSociales> ListaUsuarioVinculadoLoginRedesSociales;
        public List<HistoricoProyectoUsuario> ListaHistoricoProyectoUsuario;
        public List<UsuarioContadores> ListaUsuarioContadores;
        public DataWrapperUsuario()
        {
            ListaUsuario = new List<EntityModel.Models.UsuarioDS.Usuario>();
            ListaProyRolUsuClausulaReg = new List<ProyRolUsuClausulaReg>();
            ListaClausulaRegistro = new List<ClausulaRegistro>();
            ListaProyectoRolUsuario = new List<ProyectoRolUsuario>();
            ListaProyectoUsuarioIdentidad = new List<ProyectoUsuarioIdentidad>();
            ListaGeneralRolUsuario = new List<GeneralRolUsuario>();
            ListaAdministradorGeneral = new List<AdministradorGeneral>();
            ListaInicioSesion = new List<InicioSesion>();
            ListaUsuarioIdentidadPersona = new List<UsuarioIdentidadPersona>();
            ListaOrganizacionRolUsuario = new List<OrganizacionRolUsuario>();
            ListaGrupoUsuarioUsuario = new List<GrupoUsuarioUsuario>();
            ListaUsuarioVinculadoLoginRedesSociales = new List<UsuarioVinculadoLoginRedesSociales>();
            ListaHistoricoProyectoUsuario = new List<HistoricoProyectoUsuario>();
            ListaUsuarioContadores = new List<UsuarioContadores>();
            ListaProyectoRolGrupoUsuario = new List<ProyectoRolGrupoUsuario>();
        }

        public override void Merge(DataWrapperBase pDataWrapper)
        {
            DataWrapperUsuario dataWrapperUsuario = (DataWrapperUsuario)pDataWrapper;
            ListaUsuario = ListaUsuario.Union(dataWrapperUsuario.ListaUsuario).ToList();
            ListaProyRolUsuClausulaReg = ListaProyRolUsuClausulaReg.Union(dataWrapperUsuario.ListaProyRolUsuClausulaReg).ToList();
            ListaClausulaRegistro = ListaClausulaRegistro.Union(dataWrapperUsuario.ListaClausulaRegistro).ToList();
            ListaProyectoRolUsuario = ListaProyectoRolUsuario.Union(dataWrapperUsuario.ListaProyectoRolUsuario).ToList();
            ListaProyectoUsuarioIdentidad = ListaProyectoUsuarioIdentidad.Union(dataWrapperUsuario.ListaProyectoUsuarioIdentidad).ToList();
            ListaGeneralRolUsuario = ListaGeneralRolUsuario.Union(dataWrapperUsuario.ListaGeneralRolUsuario).ToList();
            ListaAdministradorGeneral = ListaAdministradorGeneral.Union(dataWrapperUsuario.ListaAdministradorGeneral).ToList();
            ListaInicioSesion = ListaInicioSesion.Union(dataWrapperUsuario.ListaInicioSesion).ToList();
            ListaUsuarioIdentidadPersona = ListaUsuarioIdentidadPersona.Union(dataWrapperUsuario.ListaUsuarioIdentidadPersona).ToList();
            ListaOrganizacionRolUsuario = ListaOrganizacionRolUsuario.Union(dataWrapperUsuario.ListaOrganizacionRolUsuario).ToList();
            ListaGrupoUsuarioUsuario = ListaGrupoUsuarioUsuario.Union(dataWrapperUsuario.ListaGrupoUsuarioUsuario).ToList();
            ListaUsuarioVinculadoLoginRedesSociales = ListaUsuarioVinculadoLoginRedesSociales.Union(dataWrapperUsuario.ListaUsuarioVinculadoLoginRedesSociales).ToList();
            ListaHistoricoProyectoUsuario = ListaHistoricoProyectoUsuario.Union(dataWrapperUsuario.ListaHistoricoProyectoUsuario).ToList();
            ListaUsuarioContadores = ListaUsuarioContadores.Union(dataWrapperUsuario.ListaUsuarioContadores).ToList();
            ListaProyectoRolGrupoUsuario = ListaProyectoRolGrupoUsuario.Union(dataWrapperUsuario.ListaProyectoRolGrupoUsuario).ToList();
        }

        public void CargaRelacionesPerezosasCache()
        {
            foreach(EntityModel.Models.UsuarioDS.Usuario usuario in ListaUsuario)
            {
                usuario.GrupoUsuarioUsuario = ListaGrupoUsuarioUsuario.Where(item => item.UsuarioID.Equals(usuario.UsuarioID)).ToList();
                usuario.ProyectoUsuarioIdentidad = ListaProyectoUsuarioIdentidad.Where(item => item.UsuarioID.Equals(usuario.UsuarioID)).ToList();
                usuario.ProyectoRolUsuario = ListaProyectoRolUsuario.Where(item => item.UsuarioID.Equals(usuario.UsuarioID)).ToList();
                usuario.OrganizacionRolUsuario = ListaOrganizacionRolUsuario.Where(item => item.UsuarioID.Equals(usuario.UsuarioID)).ToList();
                usuario.UsuarioVinculadoLoginRedesSociales = ListaUsuarioVinculadoLoginRedesSociales.Where(item => item.UsuarioID.Equals(usuario.UsuarioID)).ToList();
                
            }
        }
    }
}
