using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion;
using Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.OrganizacionDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.Pais;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.Tesauro;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Es.Riam.Gnoss.AD.ServiciosGenerales
{

    public class JoinOrganizacionOrganizacionParticipaProy
    {
        public Organizacion Organizacion { get; set; }
        public OrganizacionParticipaProy OrganizacionParticipaProy { get; set; }
    }

    public class JoinOrganizacionOrganizacionParticipaProyProyecto
    {
        public Organizacion Organizacion { get; set; }
        public OrganizacionParticipaProy OrganizacionParticipaProy { get; set; }
        public Proyecto Proyecto { get; set; }
    }

    public class JoinOrganizacionOrganizacionParticipaProyProyectoPerfil
    {
        public Organizacion Organizacion { get; set; }
        public OrganizacionParticipaProy OrganizacionParticipaProy { get; set; }
        public Proyecto Proyecto { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class JoinOrganizacionOrganizacionParticipaProyProyectoPerfilIdentidad
    {
        public Organizacion Organizacion { get; set; }
        public OrganizacionParticipaProy OrganizacionParticipaProy { get; set; }
        public Proyecto Proyecto { get; set; }
        public Perfil Perfil { get; set; }
        public AD.EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinOrganizacionOrganizacionParticipaProyProyectoPerfilIdentidadPais
    {
        public Organizacion Organizacion { get; set; }
        public OrganizacionParticipaProy OrganizacionParticipaProy { get; set; }
        public Proyecto Proyecto { get; set; }
        public Perfil Perfil { get; set; }
        public AD.EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Pais Pais { get; set; }
    }

    public class JoinOrganizacionPerfil
    {
        public Organizacion Organizacion { get; set; }
        public Perfil Perfil { get; set; }
    }
    public class JoinOrganizacionPerfilIdentidad
    {
        public Organizacion Organizacion { get; set; }
        public Perfil Perfil { get; set; }
        public AD.EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinOrganizacionPerfilIdentidadMyGnoss
    {
        public Organizacion Organizacion { get; set; }
        public Perfil Perfil { get; set; }
        public AD.EntityModel.Models.IdentidadDS.Identidad IdentidadMyGnoss { get; set; }
    }

    public class JoinOrganizacionPerfilIdentidadMyGnossIdentidad
    {
        public Organizacion Organizacion { get; set; }
        public Perfil Perfil { get; set; }
        public AD.EntityModel.Models.IdentidadDS.Identidad IdentidadMyGnoss { get; set; }
        public AD.EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinOrganizacionPerfilIdentidadPais
    {
        public Organizacion Organizacion { get; set; }
        public Perfil Perfil { get; set; }
        public AD.EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Pais Pais { get; set; }
    }

    public class JoinOrganizacionPerfilIdentidadProvincia
    {
        public Organizacion Organizacion { get; set; }
        public Perfil Perfil { get; set; }
        public AD.EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Provincia Provincia { get; set; }
    }

    public class JoinPersonaVinculoOrganizacionPerfil
    {
        public PersonaVinculoOrganizacion PersonaVinculoOrganizacion { get; set; }
        public Perfil Perfil { get; set; }
    }
    public class JoinPersonaVinculoOrganizacionPerfilIdentidad
    {
        public PersonaVinculoOrganizacion PersonaVinculoOrganizacion { get; set; }
        public Perfil Perfil { get; set; }
        public AD.EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }
    public class JoinOrganizacionPersonaVinculoOrganizacion
    {
        public Organizacion Organizacion { get; set; }
        public PersonaVinculoOrganizacion PersonaVinculoOrganizacion { get; set; }
    }
    public class JoinOrganizacionPerfilPersonaOrg
    {
        public Organizacion Organizacion { get; set; }
        public PerfilPersonaOrg PerfilPersonaOrg { get; set; }
    }
    public class JoinOrganizacionperfPersOrgIdentidad
    {
        public Organizacion Organizacion { get; set; }
        public PerfilPersonaOrg PerfilPersonaOrg { get; set; }
        public AD.EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }
    public class JoinOrganizacionPerfilOrganizacion
    {
        public Organizacion Organizacion { get; set; }
        public PerfilOrganizacion PerfilOrganizacion { get; set; }
    }
    public class JoinOrganizacionPerfilOrganizacionIdentidad
    {
        public Organizacion Organizacion { get; set; }
        public PerfilOrganizacion PerfilOrganizacion { get; set; }
        public AD.EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }
    public class JoinOrganizacionPersonaVinculoOrganizacionPersona
    {
        public Organizacion Organizacion { get; set; }
        public PersonaVinculoOrganizacion PersonaVinculoOrganizacion { get; set; }
        public AD.EntityModel.Models.PersonaDS.Persona Persona { get; set; }
    }
    public class JoinOrganizacionSolicitudOrganizacion
    {
        public Organizacion Organizacion { get; set; }
        public AD.EntityModel.Models.Solicitud.SolicitudOrganizacion SolicitudOrganizacion { get; set; }
    }
    public class JoinOrganizacionSolicitudOrganizacionSolicitud
    {
        public Organizacion Organizacion { get; set; }
        public AD.EntityModel.Models.Solicitud.SolicitudOrganizacion SolicitudOrganizacion { get; set; }
        public AD.EntityModel.Models.Solicitud.Solicitud Solicitud { get; set; }
    }
    public class JoinOrganizacionProyectoRolUsuario
    {
        public Organizacion Organizacion { get; set; }
        public AD.EntityModel.Models.UsuarioDS.ProyectoRolUsuario ProyectoRolUsuario { get; set; }
    }
    public class JoinOrganizacionPersonaOcupacionFigura
    {
        public Organizacion Organizacion { get; set; }
        public PersonaOcupacionFigura PersonaOcupacionFigura { get; set; }
    }
    public class JoinOrganizacionPersonaOcupacionFormaSec
    {
        public Organizacion Organizacion { get; set; }
        public PersonaOcupacionFormaSec PersonaOcupacionFormaSec { get; set; }
    }
    public class JoinPersonaVinculoOrganizacionPersonaOcupacionFigura
    {
        public PersonaVinculoOrganizacion PersonaVinculoOrganizacion { get; set; }
        public PersonaOcupacionFigura PersonaOcupacionFigura { get; set; }
    }
    public class JoinPersonaVinculoOrganizacionPersonaOcupacionFormaSec
    {
        public PersonaVinculoOrganizacion PersonaVinculoOrganizacion { get; set; }
        public PersonaOcupacionFormaSec PersonaOcupacionFormaSec { get; set; }
    }

    public class JoinOrganizacionGnossPersonaOcupacionFigura
    {
        public OrganizacionGnoss OrganizacionGnoss { get; set; }
        public PersonaOcupacionFigura PersonaOcupacionFigura { get; set; }
    }
    public class JoinOrganizacionGnossPersonaOcupacionFormaSec
    {
        public OrganizacionGnoss OrganizacionGnoss { get; set; }
        public PersonaOcupacionFormaSec PersonaOcupacionFormaSec { get; set; }
    }
    public class JoinTesauroOrganizacionCategoriaTesauro
    {
        public TesauroOrganizacion TesauroOrganizacion { get; set; }
        public CategoriaTesauro CategoriaTesauro { get; set; }
    }

    public class JoinOrganizacionPerfilIdentidadCurriculum
    {
        public Organizacion Organizacion { get; set; }
        public Perfil Perfil { get; set; }
        public AD.EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public AD.EntityModel.Models.PersonaDS.Curriculum Curriculum { get; set; }
    }

    public class JoinOrganizacionPerfilIdentidadOrganizacionClase
    {
        public Organizacion Organizacion { get; set; }
        public Perfil Perfil { get; set; }
        public AD.EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public OrganizacionClase OrganizacionClase { get; set; }
    }

    public class JoinOrganizacionPerfilIdentidadCurriculumDocumento
    {
        public Organizacion Organizacion { get; set; }
        public Perfil Perfil { get; set; }
        public AD.EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public AD.EntityModel.Models.PersonaDS.Curriculum Curriculum { get; set; }
        public Documento Documento { get; set; }
    }

    //Creacion de JOINS
    public static class Joins
    {
        public static IQueryable<JoinProyectoOrganizacionParticipaProy> JoinOrganizacionParticipaProy(this IQueryable<Proyecto> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.OrganizacionParticipaProy, proyecto => proyecto.ProyectoID, organizacionParticipaProy => organizacionParticipaProy.ProyectoID, (proyecto, organizacionParticipaProy) => new JoinProyectoOrganizacionParticipaProy
            {
                Proyecto = proyecto,
                OrganizacionParticipaProy = organizacionParticipaProy
            });
        }

        public static IQueryable<JoinProyectoAdministradorProyecto> JoinAdministradorProyecto(this IQueryable<Proyecto> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.AdministradorProyecto, proyecto => proyecto.ProyectoID, administradorProyecto => administradorProyecto.ProyectoID, (proyecto, administradorProyecto) => new JoinProyectoAdministradorProyecto
            {
                Proyecto = proyecto,
                AdministradorProyecto = administradorProyecto
            });
        }

        public static IQueryable<JoinProyectoAdministradorProyectoPersona> JoinPersona(this IQueryable<JoinProyectoAdministradorProyecto> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Persona, item => item.AdministradorProyecto.UsuarioID, persona => persona.UsuarioID.Value, (item, persona) => new JoinProyectoAdministradorProyectoPersona
            {
                Proyecto = item.Proyecto,
                AdministradorProyecto = item.AdministradorProyecto,
                Persona = persona
            });
        }

        public static IQueryable<JoinProyectoAdministradorProyectoPersonaPerfil> JoinPerfil(this IQueryable<JoinProyectoAdministradorProyectoPersona> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.Persona.PersonaID, perfil => perfil.PersonaID.Value, (item, perfil) => new JoinProyectoAdministradorProyectoPersonaPerfil
            {
                Proyecto = item.Proyecto,
                AdministradorProyecto = item.AdministradorProyecto,
                Persona = item.Persona,
                Perfil = perfil
            });
        }

        public static IQueryable<JoinProyectoAdministradorProyectoPersonaPerfilIdentidad> JoinIdentidad(this IQueryable<JoinProyectoAdministradorProyectoPersonaPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => new { item.AdministradorProyecto.ProyectoID, item.Perfil.PerfilID }, identidad => new { identidad.ProyectoID, identidad.PerfilID }, (item, identidad) => new JoinProyectoAdministradorProyectoPersonaPerfilIdentidad
            {
                Proyecto = item.Proyecto,
                AdministradorProyecto = item.AdministradorProyecto,
                Persona = item.Persona,
                Perfil = item.Perfil,
                Identidad = identidad
            });
        }

        public static IQueryable<JoinProyectoOrganizacionParticipaProyOrganizacion> JoinOrganizacion(this IQueryable<JoinProyectoOrganizacionParticipaProy> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Organizacion, item => item.OrganizacionParticipaProy.OrganizacionID, organizacion => organizacion.OrganizacionID, (item, organizacion) => new JoinProyectoOrganizacionParticipaProyOrganizacion
            {
                Proyecto = item.Proyecto,
                OrganizacionParticipaProy = item.OrganizacionParticipaProy,
                Organizacion = organizacion
            });
        }

        public static IQueryable<JoinProyectoOrganizacionParticipaProyOrganizacionPais> JoinPais(this IQueryable<JoinProyectoOrganizacionParticipaProyOrganizacion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Pais, item => item.Organizacion.PaisID.Value, pais => pais.PaisID, (item, pais) => new JoinProyectoOrganizacionParticipaProyOrganizacionPais
            {
                Proyecto = item.Proyecto,
                OrganizacionParticipaProy = item.OrganizacionParticipaProy,
                Organizacion = item.Organizacion,
                Pais = pais
            });
        }

        public static IQueryable<JoinProyectoOrganizacionParticipaProyOrganizacionPaisPerfil> JoinPerfil(this IQueryable<JoinProyectoOrganizacionParticipaProyOrganizacionPais> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.Organizacion.OrganizacionID, perfil => perfil.OrganizacionID, (item, perfil) => new JoinProyectoOrganizacionParticipaProyOrganizacionPaisPerfil
            {
                Proyecto = item.Proyecto,
                OrganizacionParticipaProy = item.OrganizacionParticipaProy,
                Organizacion = item.Organizacion,
                Pais = item.Pais,
                Perfil = perfil
            });
        }

        public static IQueryable<JoinProyectoOrganizacionParticipaProyOrganizacionPaisPerfilIdentidad> JoinIdentidad(this IQueryable<JoinProyectoOrganizacionParticipaProyOrganizacionPaisPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.Perfil.PerfilID, identidad => identidad.IdentidadID, (item, identidad) => new JoinProyectoOrganizacionParticipaProyOrganizacionPaisPerfilIdentidad
            {
                Proyecto = item.Proyecto,
                OrganizacionParticipaProy = item.OrganizacionParticipaProy,
                Organizacion = item.Organizacion,
                Pais = item.Pais,
                Perfil = item.Perfil,
                Identidad = identidad
            });
        }

        public static IQueryable<JoinProyectoOrganizacionParticipaProyPerfil> JoinPerfil(this IQueryable<JoinProyectoOrganizacionParticipaProy> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.OrganizacionParticipaProy.OrganizacionID, perfil => perfil.OrganizacionID, (item, perfil) => new JoinProyectoOrganizacionParticipaProyPerfil
            {
                Proyecto = item.Proyecto,
                OrganizacionParticipaProy = item.OrganizacionParticipaProy,
                Perfil = perfil
            });
        }

        public static IQueryable<JoinProyectoOrganizacionParticipaProyPerfilIdentidad> LeftJoinIdentidad(this IQueryable<JoinProyectoOrganizacionParticipaProyPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.GroupJoin(entityContext.Identidad, item => new { PerfilID = item.Perfil.PerfilID, ProyectoID = item.Proyecto.ProyectoID }, identidad => new { PerfilID = identidad.PerfilID, ProyectoID = identidad.ProyectoID }, (item, identidad) => new
            {
                Proyecto = item.Proyecto,
                OrganizacionParticipaProy = item.OrganizacionParticipaProy,
                Perfil = item.Perfil,
                Identidad = identidad
            }).SelectMany(x => x.Identidad.DefaultIfEmpty(), (x, y) => new JoinProyectoOrganizacionParticipaProyPerfilIdentidad
            {
                Proyecto = x.Proyecto,
                OrganizacionParticipaProy = x.OrganizacionParticipaProy,
                Perfil = x.Perfil,
                Identidad = y
            });
        }

        public static IQueryable<JoinProyectoOrganizacionParticipaProyPerfilIdentidadPerfil> LeftJoinPerfil(this IQueryable<JoinProyectoOrganizacionParticipaProyPerfilIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.GroupJoin(entityContext.Perfil, item => item.Identidad.PerfilID, perfil => perfil.PerfilID, (item, perfil) => new
            {
                Proyecto = item.Proyecto,
                OrganizacionParticipaProy = item.OrganizacionParticipaProy,
                Perfil = item.Perfil,
                Identidad = item.Identidad,
                Perfil2 = perfil
            }).SelectMany(x => x.Perfil2.DefaultIfEmpty(), (x, y) => new JoinProyectoOrganizacionParticipaProyPerfilIdentidadPerfil
            {
                Proyecto = x.Proyecto,
                OrganizacionParticipaProy = x.OrganizacionParticipaProy,
                Perfil = x.Perfil,
                Identidad = x.Identidad,
                Perfil2 = y
            });
        }

        public static IQueryable<JoinProyectoOrganizacionParticipaProyPerfilIdentidadPerfilPersona> LeftJoinPersona(this IQueryable<JoinProyectoOrganizacionParticipaProyPerfilIdentidadPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.GroupJoin(entityContext.Persona, item => item.Perfil2.PersonaID.Value, persona => persona.PersonaID, (item, persona) => new
            {
                Proyecto = item.Proyecto,
                OrganizacionParticipaProy = item.OrganizacionParticipaProy,
                Perfil = item.Perfil,
                Identidad = item.Identidad,
                Perfil2 = item.Perfil2,
                Persona = persona
            }).SelectMany(x => x.Persona.DefaultIfEmpty(), (x, y) => new JoinProyectoOrganizacionParticipaProyPerfilIdentidadPerfilPersona
            {
                Proyecto = x.Proyecto,
                OrganizacionParticipaProy = x.OrganizacionParticipaProy,
                Perfil = x.Perfil,
                Identidad = x.Identidad,
                Perfil2 = x.Perfil2,
                Persona = y
            });
        }

        public static IQueryable<JoinProyectoOrganizacionParticipaProyPerfilIdentidadPerfilPersonaAdministradorProyecto> LeftJoinAdministradorProyecto(this IQueryable<JoinProyectoOrganizacionParticipaProyPerfilIdentidadPerfilPersona> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.GroupJoin(entityContext.AdministradorProyecto, item => new { UsuarioID = item.Persona.UsuarioID.Value, ProyectoID = item.OrganizacionParticipaProy.ProyectoID }, administradorProyecto => new { UsuarioID = administradorProyecto.UsuarioID, ProyectoID = administradorProyecto.ProyectoID }, (item, administradorProyecto) => new
            {
                Proyecto = item.Proyecto,
                OrganizacionParticipaProy = item.OrganizacionParticipaProy,
                Perfil = item.Perfil,
                Identidad = item.Identidad,
                Perfil2 = item.Perfil2,
                Persona = item.Persona,
                AdministradorProyecto = administradorProyecto
            }).SelectMany(x => x.AdministradorProyecto.DefaultIfEmpty(), (x, y) => new JoinProyectoOrganizacionParticipaProyPerfilIdentidadPerfilPersonaAdministradorProyecto
            {
                Proyecto = x.Proyecto,
                OrganizacionParticipaProy = x.OrganizacionParticipaProy,
                Perfil = x.Perfil,
                Identidad = x.Identidad,
                Perfil2 = x.Perfil2,
                Persona = x.Persona,
                AdministradorProyecto = y
            });
        }


        public static IQueryable<JoinOrganizacionPerfilIdentidadCurriculumDocumento> JoinDocumento(this IQueryable<JoinOrganizacionPerfilIdentidadCurriculum> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Documento, join => join.Organizacion.OrganizacionID, doc => doc.DocumentoID, (join, doc) =>
            new JoinOrganizacionPerfilIdentidadCurriculumDocumento
            {
                Organizacion = join.Organizacion,
                Perfil = join.Perfil,
                Identidad = join.Identidad,
                Curriculum = join.Curriculum,
                Documento = doc
            });
        }

        public static IQueryable<JoinOrganizacionPerfilIdentidadCurriculum> JoinCurriculum(this IQueryable<JoinOrganizacionPerfilIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Curriculum, join => join.Identidad.CurriculumID, curr => curr.CurriculumID, (join, curr) =>
            new JoinOrganizacionPerfilIdentidadCurriculum
            {
                Organizacion = join.Organizacion,
                Perfil = join.Perfil,
                Identidad = join.Identidad,
                Curriculum = curr
            });
        }

        public static IQueryable<JoinOrganizacionPerfilIdentidadOrganizacionClase> JoinOrganizacionClase(this IQueryable<JoinOrganizacionPerfilIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.OrganizacionClase, item => item.Organizacion.OrganizacionID, organizacionClase => organizacionClase.OrganizacionID, (item, organizacionClase) =>
            new JoinOrganizacionPerfilIdentidadOrganizacionClase
            {
                Organizacion = item.Organizacion,
                Perfil = item.Perfil,
                Identidad = item.Identidad,
                OrganizacionClase = organizacionClase
            });
        }

        public static IQueryable<JoinTesauroOrganizacionCategoriaTesauro> JoinCategoriaTesauro(this IQueryable<TesauroOrganizacion> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.CategoriaTesauro, join => join.TesauroID, persOcForSec => persOcForSec.TesauroID, (join, persOcForSec) =>
            new JoinTesauroOrganizacionCategoriaTesauro
            {
                TesauroOrganizacion = join,
                CategoriaTesauro = persOcForSec
            });
        }


        public static IQueryable<JoinOrganizacionGnossPersonaOcupacionFormaSec> JoinPersonaOcupacionFormaSec(this IQueryable<OrganizacionGnoss> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.PersonaOcupacionFormaSec, join => join.OrganizacionID, persOcForSec => persOcForSec.OrganizacionPersonalID, (join, persOcForSec) =>
            new JoinOrganizacionGnossPersonaOcupacionFormaSec
            {
                OrganizacionGnoss = join,
                PersonaOcupacionFormaSec = persOcForSec
            });
        }

        public static IQueryable<JoinOrganizacionGnossPersonaOcupacionFigura> JoinPersonaOcupacionFigura(this IQueryable<OrganizacionGnoss> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.PersonaOcupacionFigura, join => join.OrganizacionID, persOcFig => persOcFig.OrganizacionPersonalID, (join, persOcFig) =>
            new JoinOrganizacionGnossPersonaOcupacionFigura
            {
                OrganizacionGnoss = join,
                PersonaOcupacionFigura = persOcFig
            });
        }

        public static IQueryable<JoinPersonaVinculoOrganizacionPersonaOcupacionFormaSec> JoinPersonaOcupacionFormaSec(this IQueryable<PersonaVinculoOrganizacion> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.PersonaOcupacionFormaSec, join => join.OrganizacionID, persOcForSec => persOcForSec.OrganizacionID, (join, persOcForSec) =>
            new JoinPersonaVinculoOrganizacionPersonaOcupacionFormaSec
            {
                PersonaVinculoOrganizacion = join,
                PersonaOcupacionFormaSec = persOcForSec
            });
        }
        public static IQueryable<JoinPersonaVinculoOrganizacionPersonaOcupacionFigura> JoinPersonaOcupacionFigura(this IQueryable<PersonaVinculoOrganizacion> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.PersonaOcupacionFigura, join => join.OrganizacionID, persOcFig => persOcFig.OrganizacionID, (join, persOcFig) =>
            new JoinPersonaVinculoOrganizacionPersonaOcupacionFigura
            {
                PersonaVinculoOrganizacion = join,
                PersonaOcupacionFigura = persOcFig
            });
        }
        public static IQueryable<JoinOrganizacionPersonaOcupacionFormaSec> JoinPersonaOcupacionFormaSec(this IQueryable<Organizacion> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.PersonaOcupacionFormaSec, join => join.OrganizacionID, persOcForSec => persOcForSec.OrganizacionPersonalID, (join, persOcForSec) =>
            new JoinOrganizacionPersonaOcupacionFormaSec
            {
                Organizacion = join,
                PersonaOcupacionFormaSec = persOcForSec
            });
        }
        public static IQueryable<JoinOrganizacionPersonaOcupacionFigura> JoinPersonaOcupacionFigura(this IQueryable<Organizacion> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.PersonaOcupacionFigura, join => join.OrganizacionID, persOcFig => persOcFig.OrganizacionPersonalID, (join, persOcFig) =>
            new JoinOrganizacionPersonaOcupacionFigura
            {
                Organizacion = join,
                PersonaOcupacionFigura = persOcFig
            });
        }

        public static IQueryable<JoinOrganizacionProyectoRolUsuario> JoinProyectoRolUsuario(this IQueryable<Organizacion> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.ProyectoRolUsuario, join => join.OrganizacionID, proyRolUsu => proyRolUsu.OrganizacionGnossID, (join, proyRolUsu) =>
            new JoinOrganizacionProyectoRolUsuario
            {
                Organizacion = join,
                ProyectoRolUsuario = proyRolUsu
            });
        }
        public static IQueryable<JoinOrganizacionSolicitudOrganizacionSolicitud> JoinSolicitud(this IQueryable<JoinOrganizacionSolicitudOrganizacion> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Solicitud, join => join.SolicitudOrganizacion.SolicitudID, solic => solic.SolicitudID, (join, solic) =>
            new JoinOrganizacionSolicitudOrganizacionSolicitud
            {
                Organizacion = join.Organizacion,
                SolicitudOrganizacion = join.SolicitudOrganizacion,
                Solicitud = solic
            });
        }
        public static IQueryable<JoinOrganizacionSolicitudOrganizacion> JoinSolicitudOrganizacion(this IQueryable<Organizacion> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.SolicitudOrganizacion, join => join.OrganizacionID, solOrg => solOrg.OrganizacionID, (join, solOrg) =>
            new JoinOrganizacionSolicitudOrganizacion
            {
                Organizacion = join,
                SolicitudOrganizacion = solOrg
            });
        }

        public static IQueryable<JoinOrganizacionPersonaVinculoOrganizacionPersona> JoinPersona(this IQueryable<JoinOrganizacionPersonaVinculoOrganizacion> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Persona, join => join.PersonaVinculoOrganizacion.PersonaID, persona => persona.PersonaID, (join, persona) =>
            new JoinOrganizacionPersonaVinculoOrganizacionPersona
            {
                Organizacion = join.Organizacion,
                PersonaVinculoOrganizacion = join.PersonaVinculoOrganizacion,
                Persona = persona
            });
        }
        public static IQueryable<JoinOrganizacionPerfilOrganizacionIdentidad> JoinIdentidad(this IQueryable<JoinOrganizacionPerfilOrganizacion> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, join => join.PerfilOrganizacion.OrganizacionID, ident => ident.OrganizacionID, (join, ident) =>
            new JoinOrganizacionPerfilOrganizacionIdentidad
            {
                Organizacion = join.Organizacion,
                PerfilOrganizacion = join.PerfilOrganizacion,
                Identidad = ident
            });
        }
        public static IQueryable<JoinOrganizacionPerfilOrganizacion> JoinPerfilOrganizacion(this IQueryable<Organizacion> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.PerfilOrganizacion, join => join.OrganizacionID, perfOrg => perfOrg.OrganizacionID, (join, perfOrg) =>
            new JoinOrganizacionPerfilOrganizacion
            {
                Organizacion = join,
                PerfilOrganizacion = perfOrg
            });
        }

        public static IQueryable<JoinOrganizacionperfPersOrgIdentidad> JoinIdentidad(this IQueryable<JoinOrganizacionPerfilPersonaOrg> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, join => join.PerfilPersonaOrg.PerfilID, ident => ident.PerfilID, (join, ident) =>
            new JoinOrganizacionperfPersOrgIdentidad
            {
                Organizacion = join.Organizacion,
                PerfilPersonaOrg = join.PerfilPersonaOrg,
                Identidad = ident
            });
        }

        public static IQueryable<JoinOrganizacionPerfilPersonaOrg> JoinPerfilPersonaOrg(this IQueryable<Organizacion> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.PerfilPersonaOrg, join => join.OrganizacionID, perfPersOrg => perfPersOrg.OrganizacionID, (join, perfPersOrg) =>
            new JoinOrganizacionPerfilPersonaOrg
            {
                Organizacion = join,
                PerfilPersonaOrg = perfPersOrg
            });
        }

        public static IQueryable<JoinOrganizacionPersonaVinculoOrganizacion> JoinPersonaVinculoOrganizacion(this IQueryable<Organizacion> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.PersonaVinculoOrganizacion, join => join.OrganizacionID, persVincOrg => persVincOrg.OrganizacionID, (join, persVincOrg) =>
            new JoinOrganizacionPersonaVinculoOrganizacion
            {
                Organizacion = join,
                PersonaVinculoOrganizacion = persVincOrg
            });
        }
        public static IQueryable<JoinPersonaVinculoOrganizacionPerfilIdentidad> JoinIdentidad(this IQueryable<JoinPersonaVinculoOrganizacionPerfil> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, join => join.Perfil.PerfilID, ident => ident.PerfilID, (join, ident) =>
            new JoinPersonaVinculoOrganizacionPerfilIdentidad
            {
                PersonaVinculoOrganizacion = join.PersonaVinculoOrganizacion,
                Perfil = join.Perfil,
                Identidad = ident
            });
        }
        public static IQueryable<JoinPersonaVinculoOrganizacionPerfil> JoinPerfil(this IQueryable<PersonaVinculoOrganizacion> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Perfil, join => new { OrganizacionID = join.OrganizacionID, PersonaID = join.PersonaID }, perf => new { OrganizacionID = perf.OrganizacionID.Value, PersonaID = perf.PersonaID.Value }, (join, perf) =>
            new JoinPersonaVinculoOrganizacionPerfil
            {
                PersonaVinculoOrganizacion = join,
                Perfil = perf
            });
        }

        public static IQueryable<JoinOrganizacionPerfilIdentidad> JoinIdentidad(this IQueryable<JoinOrganizacionPerfil> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, join => join.Perfil.PerfilID, ident => ident.PerfilID, (join, ident) =>
            new JoinOrganizacionPerfilIdentidad
            {
                Organizacion = join.Organizacion,
                Perfil = join.Perfil,
                Identidad = ident
            });
        }

        public static IQueryable<JoinOrganizacionPerfilIdentidadMyGnoss> JoinIdentidadMyGnoss(this IQueryable<JoinOrganizacionPerfil> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, join => join.Perfil.PerfilID, identidadMyGnoss => identidadMyGnoss.PerfilID, (join, identidadMyGnoss) =>
            new JoinOrganizacionPerfilIdentidadMyGnoss
            {
                Organizacion = join.Organizacion,
                Perfil = join.Perfil,
                IdentidadMyGnoss = identidadMyGnoss
            });
        }

        public static IQueryable<JoinOrganizacionPerfilIdentidadMyGnossIdentidad> JoinIdentidad(this IQueryable<JoinOrganizacionPerfilIdentidadMyGnoss> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, join => join.Perfil.PerfilID, identidad => identidad.PerfilID, (join, identidad) =>
            new JoinOrganizacionPerfilIdentidadMyGnossIdentidad
            {
                Organizacion = join.Organizacion,
                Perfil = join.Perfil,
                IdentidadMyGnoss = join.IdentidadMyGnoss,
                Identidad = identidad
            });
        }

        public static IQueryable<JoinOrganizacionPerfilIdentidadPais> JoinPais(this IQueryable<JoinOrganizacionPerfilIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Pais, item => item.Organizacion.PaisID.Value, pais => pais.PaisID, (item, pais) =>
            new JoinOrganizacionPerfilIdentidadPais
            {
                Organizacion = item.Organizacion,
                Perfil = item.Perfil,
                Identidad = item.Identidad,
                Pais = pais
            });
        }

        public static IQueryable<JoinOrganizacionPerfilIdentidadProvincia> JoinProvincia(this IQueryable<JoinOrganizacionPerfilIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Provincia, item => item.Organizacion.ProvinciaID.Value, provincia => provincia.ProvinciaID, (item, provincia) =>
            new JoinOrganizacionPerfilIdentidadProvincia
            {
                Organizacion = item.Organizacion,
                Perfil = item.Perfil,
                Identidad = item.Identidad,
                Provincia = provincia
            });
        }

        public static IQueryable<JoinOrganizacionPerfil> JoinPerfil(this IQueryable<Organizacion> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Perfil, join => join.OrganizacionID, perf => perf.OrganizacionID, (join, perf) =>
            new JoinOrganizacionPerfil
            {
                Organizacion = join,
                Perfil = perf
            });
        }

        public static IQueryable<JoinOrganizacionOrganizacionParticipaProy> JoinOrganizacionParticipaProy(this IQueryable<Organizacion> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.OrganizacionParticipaProy, join => join.OrganizacionID, orgPartProy => orgPartProy.OrganizacionID, (join, orgPartProy) =>
            new JoinOrganizacionOrganizacionParticipaProy
            {
                Organizacion = join,
                OrganizacionParticipaProy = orgPartProy
            });
        }

        public static IQueryable<JoinOrganizacionOrganizacionParticipaProyProyecto> JoinProyecto(this IQueryable<JoinOrganizacionOrganizacionParticipaProy> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Proyecto, join => join.OrganizacionParticipaProy.ProyectoID, proyecto => proyecto.ProyectoID, (join, proyecto) =>
            new JoinOrganizacionOrganizacionParticipaProyProyecto
            {
                Organizacion = join.Organizacion,
                OrganizacionParticipaProy = join.OrganizacionParticipaProy,
                Proyecto = proyecto
            });
        }

        public static IQueryable<JoinOrganizacionOrganizacionParticipaProyProyectoPerfil> JoinPerfil(this IQueryable<JoinOrganizacionOrganizacionParticipaProyProyecto> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Perfil, join => join.Organizacion.OrganizacionID, perfil => perfil.OrganizacionID, (join, perfil) =>
            new JoinOrganizacionOrganizacionParticipaProyProyectoPerfil
            {
                Organizacion = join.Organizacion,
                OrganizacionParticipaProy = join.OrganizacionParticipaProy,
                Proyecto = join.Proyecto,
                Perfil = perfil
            });
        }

        public static IQueryable<JoinOrganizacionOrganizacionParticipaProyProyectoPerfilIdentidad> JoinIdentidad(this IQueryable<JoinOrganizacionOrganizacionParticipaProyProyectoPerfil> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, join => join.Perfil.PerfilID, identidad => identidad.PerfilID, (join, identidad) =>
            new JoinOrganizacionOrganizacionParticipaProyProyectoPerfilIdentidad
            {
                Organizacion = join.Organizacion,
                OrganizacionParticipaProy = join.OrganizacionParticipaProy,
                Proyecto = join.Proyecto,
                Perfil = join.Perfil,
                Identidad = identidad
            });
        }

        public static IQueryable<JoinOrganizacionOrganizacionParticipaProyProyectoPerfilIdentidadPais> JoinPais(this IQueryable<JoinOrganizacionOrganizacionParticipaProyProyectoPerfilIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Pais, join => join.Organizacion.PaisID.Value, pais => pais.PaisID, (join, pais) =>
            new JoinOrganizacionOrganizacionParticipaProyProyectoPerfilIdentidadPais
            {
                Organizacion = join.Organizacion,
                OrganizacionParticipaProy = join.OrganizacionParticipaProy,
                Proyecto = join.Proyecto,
                Perfil = join.Perfil,
                Identidad = join.Identidad,
                Pais = pais
            });
        }
    }

    #region Enumeraciones

    /// <summary>
    /// Sectores definidos para las organizaciones
    /// </summary>
    public enum SectoresOrganizacion
    {
        /// <summary>
        /// Tipo sin definir o por defecto
        /// </summary>
        Otros = 0,
        /// <summary>
        /// Consultoría
        /// </summary>
        Consultoria = 1,
        /// <summary>
        /// Agricultura, pesca y minería
        /// </summary>
        Agricultura_Pesca_Mineria = 2,
        /// <summary>
        /// Arte, cultura y sociedad
        /// </summary>
        Arte_Cultura_Sociedad = 3,
        /// <summary>
        /// Bioquíomica y farmacia
        /// </summary>
        Bioquimica_Farmacia = 4,
        /// <summary>
        /// Periodismo
        /// </summary>
        Periodismo = 5,
        /// <summary>
        /// Dirección y gerencia
        /// </summary>
        Direccion_Gerencia = 6,
        /// <summary>
        /// Seguros
        /// </summary>
        Seguros = 7,
        /// <summary>
        /// Arquitectura
        /// </summary>
        Arquitectura = 8,
        /// <summary>
        /// Hostelería y turismo
        /// </summary>
        Hosteleria_Turismo = 9,
        /// <summary>
        /// Limpieza y servicios urbanos
        /// </summary>
        Limpieza_ServiciosUrbanos = 10,
        /// <summary>
        /// Mantenimiento de instalaciones
        /// </summary>
        Mantenimiento_de_Instalaciones = 11,
        /// <summary>
        /// Medio ambiente
        /// </summary>
        MedioAmbiente = 12,
        /// <summary>
        /// Secretariado administrativo
        /// </summary>
        Secretariado_Administrativo = 13,
        /// <summary>
        /// Seguridad y defensa
        /// </summary>
        Seguridad_Defensa = 14,
        /// <summary>
        /// Administración de empresas
        /// </summary>
        Administracion_de_empresas = 15,
        /// <summary>
        /// Administración pública
        /// </summary>
        Administracion_publica = 16,
        /// <summary>
        /// Atención al cliente
        /// </summary>
        Atencion_al_cliente = 17,
        /// <summary>
        /// Calidad
        /// </summary>
        Calidad = 18,
        /// <summary>
        /// Comercial y ventas
        /// </summary>
        Comercial_Ventas = 19,
        /// <summary>
        /// Compras
        /// </summary>
        Compras = 20,
        /// <summary>
        /// Diseño y artes gráficas
        /// </summary>
        Disenio_ArtesGraficas = 21,
        /// <summary>
        /// Educación y formación
        /// </summary>
        Educacion_Formacion = 22,
        /// <summary>
        /// Finanzas y banca
        /// </summary>
        Finanzas_Banca = 23,
        /// <summary>
        /// Informática y telecomunicaciones
        /// </summary>
        Informatica_Telecomunicaciones = 24,
        /// <summary>
        /// Ingenieros y técnicos
        /// </summary>
        Ingenieros_Tecnicos = 25,
        /// <summary>
        /// Inmobiliario
        /// </summary>
        Inmobiliario = 26,
        /// <summary>
        /// Construcción
        /// </summary>
        Construccion = 27,
        /// <summary>
        /// Logística y almacenaje
        /// </summary>
        Logistica_Almacenaje = 28,
        /// <summary>
        /// Legal
        /// </summary>
        Legal = 29,
        /// <summary>
        /// Investigación e I+D
        /// </summary>
        Investigacion_ID = 30,
        /// <summary>
        /// Marketing y comunicación
        /// </summary>
        Marketing_Comunicacion = 31,
        /// <summary>
        /// Artes y oficios
        /// </summary>
        Artes_Oficios = 32,
        /// <summary>
        /// Recursos humanos
        /// </summary>
        Recursos_Humanos = 33,
        /// <summary>
        /// Sanidad y salud
        /// </summary>
        Sanidad_Salud = 34,
        /// <summary>
        /// Turismo y restauración
        /// </summary>
        Turismo_Restauracion = 35,
        /// <summary>
        /// Productos alimenticios y bebidas
        /// </summary>
        Productos_alimenticios_bebidas = 36
    }

    /// <summary>
    /// Tipos definidos para las organizaciones
    /// </summary>
    public enum TiposOrganizacion
    {
        /// <summary>
        /// Tipo sin definir o por defecto
        /// </summary>
        Otros = 0,
        /// <summary>
        /// Empresario individual
        /// </summary>
        EmpresarioIndividual = 1,
        /// <summary>
        /// Comunidad de bienes
        /// </summary>
        ComunidadBienes = 2,
        /// <summary>
        /// Sociedades civiles
        /// </summary>
        SociedadesCiviles = 3,
        /// <summary>
        /// Sociedad colectiva
        /// </summary>
        SociedadColectiva = 4,
        /// <summary>
        /// Sociedad limitada
        /// </summary>
        SociedadLimitada = 5,
        /// <summary>
        /// Sociedad limitada unipersonal
        /// </summary>
        SociedadLimitadaUnipersonal = 6,
        /// <summary>
        /// Sociedad limitada, nueva empresa
        /// </summary>
        SociedadLimitadaNuevaEmpresa = 7,
        /// <summary>
        /// Sociedad anónima
        /// </summary>
        SociedadAnonima = 8,
        /// <summary>
        /// Sociedad comanditaria
        /// </summary>
        SociedadComanditaria = 9,
        /// <summary>
        /// Sociedad laboral
        /// </summary>
        SociedadLaboral = 10,
        /// <summary>
        /// Sociedad de garantía recíproca
        /// </summary>
        SociedadGarantiaReciproca = 11,
        /// <summary>
        /// Sociedad de capital riesgo
        /// </summary>
        SociedadCapitalRiesgo = 12,
        /// <summary>
        /// Agrupación de interés económico
        /// </summary>
        AgrupacionInteresEconomico = 13,
        /// <summary>
        /// Sociedad de inversión mobiliaria
        /// </summary>
        SociedadInversionMobiliaria = 14,
        /// <summary>
        /// Asociaciones
        /// </summary>
        Asociaciones = 15,
        /// <summary>
        /// Fundaciones
        /// </summary>
        Fundaciones = 16,
        /// <summary>
        /// Comunidades de propietarios
        /// </summary>
        ComunidadPropietarios = 17,
        /// <summary>
        /// Administración pública
        /// </summary>
        AdministracionPublica = 18,
        /// <summary>
        /// Corporaciones
        /// </summary>
        Corporaciones = 19,
        /// <summary>
        /// Unión temporal de empresas
        /// </summary>
        UnionTemporalEmpresas = 20,
        /// <summary>
        /// Grupo de investigación
        /// </summary>
        GrupoInvestigacion = 21,
        /// <summary>
        /// Empresa
        /// </summary>
        Empresa = 22
    }

    /// <summary>
    /// Enumeración para diferenciar los tipos de clase
    /// </summary>
    public enum TipoClase
    {
        /// <summary>
        /// Clase para Universidad 2.0
        /// </summary>
        Universidad20 = 0,
        /// <summary>
        /// Clase para Educación Expandida
        /// </summary>
        EducacionExpandida = 1,
        /// <summary>
        /// Clase para Educación primaria
        /// </summary>
        EducacionPrimaria = 2
    }

    /// <summary>
    /// Enumeración para diferenciar los diferentes tipos de visibilidad de los contactos de una organizacion
    /// </summary>
    public enum TipoVisibilidadContactosOrganizacion
    {
        /// <summary>
        /// Nadie
        /// </summary>
        Nadie = 0,
        /// <summary>
        /// Contactos
        /// </summary>
        Contactos = 1,
        /// <summary>
        /// Contactos de contactos
        /// </summary>
        ContactosDeContactos = 2
    }

    #endregion

    /// <summary>
    /// DataAdapter para organizaciones
    /// </summary>
    public class OrganizacionAD : BaseAD
    {
        private EntityContext mEntityContext;
        private LoggingService mLoggingService;
        private ConfigService mConfigService;
        private ILogger mlogger;
        private ILoggerFactory mloggerFactory;

        #region Constructores

        /// <summary>
        /// El por defecto, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        public OrganizacionAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<OrganizacionAD> logger, ILoggerFactory loggerFactory)
            : base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
        {
            mEntityContext = entityContext;
            mConfigService = configService;
            mLoggingService = loggingService;
            mlogger = logger;
            mloggerFactory = loggerFactory;
            this.CargarConsultasYDataAdapters();
        }

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuracion de conexion a la Base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD"></param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public OrganizacionAD(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<OrganizacionAD> logger, ILoggerFactory loggerFactory)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
        {
            mEntityContext = entityContext;
            mConfigService = configService;
            mLoggingService = loggingService;
            mlogger = logger;
            mloggerFactory = loggerFactory;
            this.CargarConsultasYDataAdapters(IBD);
        }

        #endregion

        #region Consultas

        #region Sólo la parte del Select

        private string SelectOrganizacionClase;

        #endregion
                
        private string sqlSelectOrganizacionClaseClasesAdministraUsuario;

        #endregion

        #region Métodos generales

        #region Públicos


        /// <summary>
        /// Actualiza los cambios de privacidad en el perfil de una organizacion
        /// </summary>
        /// <param name="pFilaOrganizacion">datos de la organizacion</param>
        /// <param name="pFilaConfigOrg">datos de la configuracion de la organizacion</param>
        /// <returns>true si se han realizado cambios</returns>
        public bool GuardarCambiosPrivacidadOrganizacion(Organizacion pFilaOrganizacion, ConfiguracionGnossOrg pFilaConfigOrg)
        {
            bool cambiadaPrivacidadRecursos = false;
            if (mEntityContext.Entry(pFilaOrganizacion).State.Equals(EntityState.Modified) || mEntityContext.Entry(pFilaConfigOrg).State.Equals(EntityState.Modified))
            {
                cambiadaPrivacidadRecursos = true;
            }
            mEntityContext.SaveChanges();

            return cambiadaPrivacidadRecursos;
        }

        /// <summary>
        /// Obtiene si esta activada el registro automático en la comunidad
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="pOrganizacionID">Id de la organización</param>
        /// <returns>Devuelve cierto en caso de que este activado el registro automático y falso en otro caso</returns>
        public Dictionary<Guid, bool> ObtenerParametroRegistroautomatico(List<Guid> listaComunidades, Guid pOrganizacionID)
        {
            return mEntityContext.OrganizacionParticipaProy.Where(item => listaComunidades.Contains(item.ProyectoID) && item.OrganizacionID.Equals(pOrganizacionID)).Select(item => new { item.ProyectoID, item.RegistroAutomatico }).ToDictionary(item => item.ProyectoID, item => item.RegistroAutomatico.Equals(1));
        }

        /// <summary>
        /// Lista con los usuarios que pertenecen a la organizacion
        /// </summary>
        /// <param name="pOrganizacionID">Id de la organización</param>
        /// <returns>Lista con los usuarios que pertenecen a la organizacion</returns>
        public List<Guid> ObetenerPersonasDeLaOrganizacion(Guid pOrganizacionID)
        {
            return mEntityContext.PersonaVinculoOrganizacion.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).Select(item => item.PersonaID).ToList();
        }

        /// <summary>
        /// Actualiza el valor del registro automático en una comunidad
        /// </summary>
        /// <param name="pOrganizacionID">Id de la organizacion</param>
        /// <param name="pProyectoID">Id de la comunidad</param>
        public void ActualizarRegAuto(Guid pOrganizacionID, Guid pProyectoID)
        {
            var resultado = mEntityContext.OrganizacionParticipaProy.Where(item => item.OrganizacionID.Equals(pOrganizacionID) && item.ProyectoID.Equals(pProyectoID)).FirstOrDefault();
            if (resultado != null)
            {
                if (resultado.RegistroAutomatico.Equals(1))
                {
                    resultado.RegistroAutomatico = 0;
                }
                else if (resultado.RegistroAutomatico.Equals(0))
                {
                    resultado.RegistroAutomatico = 1;
                }
            }
            mEntityContext.SaveChanges();
        }

        /// <summary>
        /// Comprueba si el email pasado por parámetro ya existe en la tabla de personas vinculadas con organización
        /// </summary>
        /// <param name="pEmail">Email que se quiere comprobar</param>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <returns>TRUE si el email ya existe, FALSE en caso contrario</returns>
        public bool ExisteEmailPersonaVinculoOrganizacion(string pEmail, Guid pPersonaID)
        {
            return mEntityContext.PersonaVinculoOrganizacion.Any(item => item.EmailTrabajo.ToUpper().Equals(pEmail.ToUpper()) && !item.PersonaID.Equals(pPersonaID));
        }

		/// <summary>
		/// Comprueba si la organización pasada por parámetro ya existe en el sistema
		/// </summary>
		/// <param name="pOrganizacionID">Identificador de la organización</param>
		/// <returns>TRUE si la organización ya existe, FALSE en caso contrario</returns>
		public bool ExisteOrganizacionPorOrganizacionID(string pOrganizacionID)
        {   
            return mEntityContext.Organizacion.Any(item => item.OrganizacionID.Equals(new Guid(pOrganizacionID)));
        }

        /// <summary>
        /// Comprueba si la organización pasada por parámetro ya existe en el sistema
        /// </summary>
        /// <param name="pNombreOrganizacion">Nombre de organización</param>
        /// <returns>TRUE si la organización ya existe, FALSE en caso contrario</returns>
        public bool ExisteOrganizacion(string pNombreOrganizacion)
        {
            return mEntityContext.Organizacion.Any(item => item.Nombre.ToUpper().Equals(pNombreOrganizacion.ToUpper()));
        }

        /// <summary>
        /// Obtiene (carga ligera) las tablas "Organizacion", "PersonaVinculoOrganizacion" y "OrganizacionGnoss"
        /// de todas las (PersonaOcupacionFigura/PersonaOcupacionFormaSec)de la estructura orgánica/secundaria de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionesDePersonasDeEstructuraDeProyectoCargaLigera(Guid pProyectoID)
        {
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            dataWrapperOrganizacion.ListaOrganizacion = mEntityContext.Organizacion.JoinPersonaOcupacionFigura().Where(item => item.PersonaOcupacionFigura.ProyectoID.Equals(pProyectoID)).Select(item => item.Organizacion)
                .Concat(mEntityContext.Organizacion.JoinPersonaOcupacionFormaSec().Where(item => item.PersonaOcupacionFormaSec.ProyectoID.Equals(pProyectoID)).Select(item => item.Organizacion))
                .ToList().Distinct().ToList();

            dataWrapperOrganizacion.ListaPersonaVinculoOrganizacion = mEntityContext.PersonaVinculoOrganizacion.JoinPersonaOcupacionFigura().Where(item => item.PersonaOcupacionFigura.ProyectoID.Equals(pProyectoID)).Select(item => item.PersonaVinculoOrganizacion)
                .Concat(mEntityContext.PersonaVinculoOrganizacion.JoinPersonaOcupacionFormaSec().Where(item => item.PersonaOcupacionFormaSec.ProyectoID.Equals(pProyectoID)).Select(item => item.PersonaVinculoOrganizacion)).ToList().Distinct().ToList();

            dataWrapperOrganizacion.ListaOrganizacionGnoss = mEntityContext.OrganizacionGnoss.JoinPersonaOcupacionFigura().Where(item => item.PersonaOcupacionFigura.ProyectoID.Equals(pProyectoID)).Select(item => item.OrganizacionGnoss).
                Union(mEntityContext.OrganizacionGnoss.JoinPersonaOcupacionFormaSec().Where(item => item.PersonaOcupacionFormaSec.ProyectoID.Equals(pProyectoID)).Select(item => item.OrganizacionGnoss)).ToList();

            return dataWrapperOrganizacion;
        }


        /// <summary>
        /// CARGA LIGERA. Obtiene las tablas "Organizacion" y "PersonaVinculoOrganizacion" de las organizaciones de una lista de identidades (se entiende que para identidades de tipo 1,2 0 3)  NO SE OBTIENEN LOS CAMPOS DE ORGANIZACION.LOGOTIPO NI DE PERSONAVINCULOORGANIZACION.FOTO
        /// </summary>
        /// <param name="pIdentidades">Identificadores de las identidades</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionesDeIdentidades(List<Guid> pIdentidades)
        {
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            if (pIdentidades.Count > 0)
            {
                dataWrapperOrganizacion.ListaOrganizacion = mEntityContext.Organizacion.JoinPerfil().JoinIdentidad().Where(item => pIdentidades.Contains(item.Identidad.IdentidadID)).Select(item => item.Organizacion).ToList().Distinct().ToList();

                dataWrapperOrganizacion.ListaPersonaVinculoOrganizacion = mEntityContext.PersonaVinculoOrganizacion.JoinPerfil().JoinIdentidad().Where(item => pIdentidades.Contains(item.Identidad.IdentidadID)).Select(item => item.PersonaVinculoOrganizacion).ToList().Distinct().ToList();
            }

            return dataWrapperOrganizacion;
        }

        /// <summary>
        /// Obtiene las tablas "Organizacion" y "PersonaVinculoOrganizacion" de las organizaciones en las que participa una persona
        /// </summary>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionesDePersona(Guid pPersonaID)
        {
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

			dataWrapperOrganizacion.ListaOrganizacion = mEntityContext.Organizacion.JoinPerfil().Where(item => !item.Perfil.Eliminado && item.Perfil.PersonaID.Value.Equals(pPersonaID)).Select(item => item.Organizacion).Distinct().ToList();             
            
            dataWrapperOrganizacion.ListaOrganizacionEmpresa = mEntityContext.OrganizacionEmpresa.Join(mEntityContext.PersonaVinculoOrganizacion, orgEmpresa => orgEmpresa.OrganizacionID, persVinOrg => persVinOrg.OrganizacionID, (orgEmpresa, persVinOrg) => new
            {
                OrganizacionEmpresa = orgEmpresa,
                PersonaVinculoOrganizacion = persVinOrg
            }).Select(item => item.OrganizacionEmpresa).ToList();

            dataWrapperOrganizacion.ListaPersonaVinculoOrganizacion = mEntityContext.PersonaVinculoOrganizacion.Where(item => item.PersonaID.Equals(pPersonaID)).ToList();

            return dataWrapperOrganizacion;
        }

        /// <summary>
        /// Obtiene las tablas "Organizacion" y "PersonaVinculoOrganizacion" de las organizaciones en las que participan las personas pasadas por parámetro
        /// </summary>
        /// <param name="pListaPersonaID">Lista de identificadores de personas</param>
        /// <returns>Dataset de organizaciones con las organizaciones y sus vínculos con personas</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionesDeListaPersona(List<Guid> pListaPersonaID)
        {
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            if (pListaPersonaID.Count > 0)
            {
                dataWrapperOrganizacion.ListaOrganizacion = mEntityContext.Organizacion.JoinPersonaVinculoOrganizacion().Where(item => pListaPersonaID.Contains(item.PersonaVinculoOrganizacion.PersonaID)).Select(item => item.Organizacion).Distinct().ToList();

                dataWrapperOrganizacion.ListaPersonaVinculoOrganizacion = mEntityContext.Organizacion.JoinPersonaVinculoOrganizacion().Where(item => pListaPersonaID.Contains(item.PersonaVinculoOrganizacion.PersonaID)).Select(item => item.PersonaVinculoOrganizacion).ToList();
            }

            return dataWrapperOrganizacion;
        }

        /// <summary>
        /// Obtiene el identificador de una organización a partir de su nombre corto
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto de la organización</param>
        /// <returns>Identificador de Organizacion</returns>
        public Guid ObtenerOrganizacionesIDPorNombre(string pNombreCorto)
        {
            return mEntityContext.Organizacion.Where(item => item.NombreCorto.Equals(pNombreCorto)).Select(item => item.OrganizacionID).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene organizaciones a partir de sus identificadores. Carga las tablas "Organizacion"
        /// </summary>
        /// <param name="pListaOrganizacionID">Lista de identificadores de organizaciones</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionesPorID(List<Guid> pListaOrganizacionID)
        {
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            if (pListaOrganizacionID.Count > 0)
            {
                dataWrapperOrganizacion.ListaOrganizacion = mEntityContext.Organizacion.Where(item => pListaOrganizacionID.Contains(item.OrganizacionID)).ToList();
            }

            return dataWrapperOrganizacion;
        }

        /// <summary>
        /// Obtiene organizaciones a partir de sus identificadores. Carga las tablas "Organizacion"
        /// </summary>
        /// <param name="pListaOrganizacionID">Lista de identificadores de organizaciones</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionesPorIDCargaLigera(List<Guid> pListaOrganizacionID)
        {
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            if (pListaOrganizacionID.Count > 0)
            {
                dataWrapperOrganizacion.ListaOrganizacion = mEntityContext.Organizacion.Where(item => pListaOrganizacionID.Contains(item.OrganizacionID)).ToList();
            }

            return dataWrapperOrganizacion;
        }

        /// <summary>
        /// Obtiene las organizaciones a partir de identidades cargardas en un dataSet.
        /// </summary>
        /// <param name="pDataWrapperIdentidad">DataSet con las identidades de organizaciones</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionesDeIdentidadesCargadas(DataWrapperIdentidad pDataWrapperIdentidad)
        {
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            List<Guid> listaOrganizacionID = new List<Guid>();

            foreach (var item in pDataWrapperIdentidad.ListaIdentidad)
            {
                listaOrganizacionID.Add(item.OrganizacionID);
            }

            dataWrapperOrganizacion.ListaOrganizacion = mEntityContext.Organizacion.Where(item => listaOrganizacionID.Contains(item.OrganizacionID)).ToList();

            dataWrapperOrganizacion.ListaPersonaVinculoOrganizacion = mEntityContext.PersonaVinculoOrganizacion.Where(item => listaOrganizacionID.Contains(item.OrganizacionID)).ToList();

            dataWrapperOrganizacion.ListaConfiguracionGnossOrg = mEntityContext.ConfiguracionGnossOrg.Where(item => listaOrganizacionID.Contains(item.OrganizacionID)).ToList();

            dataWrapperOrganizacion.ListaOrganizacionParticipaProy = mEntityContext.OrganizacionParticipaProy.Where(item => listaOrganizacionID.Contains(item.OrganizacionID)).ToList();

            dataWrapperOrganizacion.ListaPersonaVisibleEnOrg = mEntityContext.PersonaVisibleEnOrg.Where(item => listaOrganizacionID.Contains(item.OrganizacionID)).ToList();

            return dataWrapperOrganizacion;
        }

        /// <summary>
        /// Obtiene las organizaciones a partir de una lista de identidades
        /// </summary>
        /// <param name="pListaIdentidades">Lista de identidades de organizaciones</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionesPorIdentidad(List<Guid> pListaIdentidades)
        {
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();
            if (pListaIdentidades.Count > 0)
            {
                List<Guid> listaOrganizacionID = mEntityContext.Perfil.JoinIdentidad().Where(item => pListaIdentidades.Contains(item.Identidad.IdentidadID) && item.Perfil.OrganizacionID != null).Select(item => item.Perfil.OrganizacionID.Value).ToList();

                dataWrapperOrganizacion.ListaOrganizacion = mEntityContext.Organizacion.Where(item => listaOrganizacionID.Contains(item.OrganizacionID)).ToList();

                dataWrapperOrganizacion.ListaPersonaVinculoOrganizacion = mEntityContext.PersonaVinculoOrganizacion.Where(item => listaOrganizacionID.Contains(item.OrganizacionID)).ToList();

                dataWrapperOrganizacion.ListaOrganizacionParticipaProy = mEntityContext.OrganizacionParticipaProy.Where(item => listaOrganizacionID.Contains(item.OrganizacionID)).ToList();

                dataWrapperOrganizacion.ListaConfiguracionGnossOrg = mEntityContext.ConfiguracionGnossOrg.Where(item => listaOrganizacionID.Contains(item.OrganizacionID)).ToList();

                dataWrapperOrganizacion.ListaPersonaVisibleEnOrg = mEntityContext.PersonaVisibleEnOrg.Where(item => listaOrganizacionID.Contains(item.OrganizacionID)).ToList();
            }

            return dataWrapperOrganizacion;
        }

        /// <summary>
        /// Obtiene las organizaciones a partir de una identidad
        /// </summary>
        /// <param name="pIdentidad">Identificador de identidad</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionesPorIdentidad(Guid pIdentidad)
        {
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            List<Guid> listaOrganizacionID = mEntityContext.Perfil.JoinIdentidad().Where(item => item.Identidad.IdentidadID.Equals(pIdentidad) && item.Perfil.OrganizacionID != null).Select(item => item.Perfil.OrganizacionID.Value).ToList();

            dataWrapperOrganizacion.ListaOrganizacion = mEntityContext.Organizacion.Where(item => listaOrganizacionID.Contains(item.OrganizacionID)).ToList();

            dataWrapperOrganizacion.ListaPersonaVinculoOrganizacion = mEntityContext.PersonaVinculoOrganizacion.Where(item => listaOrganizacionID.Contains(item.OrganizacionID)).ToList();

            dataWrapperOrganizacion.ListaConfiguracionGnossOrg = mEntityContext.ConfiguracionGnossOrg.Where(item => listaOrganizacionID.Contains(item.OrganizacionID)).ToList();

            dataWrapperOrganizacion.ListaPersonaVisibleEnOrg = mEntityContext.PersonaVisibleEnOrg.Where(item => listaOrganizacionID.Contains(item.OrganizacionID)).ToList();

            return dataWrapperOrganizacion;
        }

        /// <summary>
        /// Obtiene el id autonumérico que se le asigna a cada organización para crear la tabla BASE
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <returns></returns>
        public int ObtenerTablaBaseOrganizacionIDOrganizacionPorID(Guid pOrganizacionID)
        {
            if (pOrganizacionID != Guid.Empty)
            {
                return mEntityContext.Organizacion.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).Select(item => item.TablaBaseOrganizacionID).FirstOrDefault();
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Obtiene el ID de una organizacion a partir de su id de tesauro (null si el usuario no existe)
        /// </summary>
        /// <param name="pPerfilID">Id del perfil</param>
        /// <returns></returns>
        public Guid? ObtenerOrganizacionIDPorIDTesauro(Guid pTesauroID)
        {
            return mEntityContext.TesauroOrganizacion.JoinCategoriaTesauro().Where(item => item.CategoriaTesauro.CategoriaTesauroID.Equals(pTesauroID)).Select(item => item.TesauroOrganizacion.OrganizacionID).FirstOrDefault();
        }


        /// <summary>
        /// Obtiene una organización a partir de su identificador
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionPorID(Guid pOrganizacionID)
        {
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            dataWrapperOrganizacion.ListaOrganizacion = mEntityContext.Organizacion.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).ToList();
            dataWrapperOrganizacion.ListaOrganizacionParticipaProy = mEntityContext.OrganizacionParticipaProy.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).ToList();
            dataWrapperOrganizacion.ListaAdministradorOrganizacion = mEntityContext.AdministradorOrganizacion.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).ToList();
            dataWrapperOrganizacion.ListaPersonaVinculoOrganizacion = mEntityContext.PersonaVinculoOrganizacion.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).ToList();
            dataWrapperOrganizacion.ListaOrganizacionGnoss = mEntityContext.OrganizacionGnoss.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).ToList();
            dataWrapperOrganizacion.ListaConfiguracionGnossOrg = mEntityContext.ConfiguracionGnossOrg.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).ToList();
            dataWrapperOrganizacion.ListaPersonaVisibleEnOrg = mEntityContext.PersonaVisibleEnOrg.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).ToList();

            return dataWrapperOrganizacion;
        }

        /// <summary>
        /// Obtiene una Organización a partir de su Identificador CARGA LIGERA. "Organizacion"
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionPorIDCargaLigera(Guid pOrganizacionID)
        {
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            dataWrapperOrganizacion.ListaOrganizacion = mEntityContext.Organizacion.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).ToList();

            return dataWrapperOrganizacion;
        }

        /// <summary>
        /// Obtiene el nombre de una organización a partir de su identificador
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <returns>Dataset de organizaciones</returns>
        public Organizacion ObtenerNombreOrganizacionPorID(Guid pOrganizacionID)
        {
            return mEntityContext.Organizacion.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene el nombre de una Organización a partir del identificador de su identidad
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la Identidad de la organizacion</param>
        /// <returns>Dataset de organización</returns>
        public DataWrapperOrganizacion ObtenerNombreOrganizacionPorIdentidad(Guid pIdentidadID)
        {
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            dataWrapperOrganizacion.ListaOrganizacion = mEntityContext.Organizacion.JoinPerfil().JoinIdentidad().Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID)).Select(item => item.Organizacion).ToList();

            return dataWrapperOrganizacion;
        }

        /// <summary>
        /// Obtiene los nombres de las organizaciones a partir de sus identificadores
        /// </summary>
        /// <param name="pOrganizacionesIDs">Lista de identificadores de organizaciones</param>
        /// <returns>Nombres de las organizaciones</returns>
        public Dictionary<Guid, KeyValuePair<string, string>> ObtenerNombreOrganizacionesPorIDs(List<Guid> pOrganizacionesIDs)
        {
            Dictionary<Guid, KeyValuePair<string, string>> listaNombres = new Dictionary<Guid, KeyValuePair<string, string>>();
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();
            if (pOrganizacionesIDs.Count > 0)
            {
                Dictionary<Guid, string> listaParmetros = new Dictionary<Guid, string>();
                dataWrapperOrganizacion.ListaOrganizacion = mEntityContext.Organizacion.Where(item => pOrganizacionesIDs.Contains(item.OrganizacionID)).ToList();

                foreach (var filaOrg in dataWrapperOrganizacion.ListaOrganizacion)
                {
                    listaNombres.Add(filaOrg.OrganizacionID, new KeyValuePair<string, string>(filaOrg.Nombre, filaOrg.NombreCorto));
                }
            }

            return listaNombres;
        }

        /// <summary>
        /// Obtiene una organización a partir de su identidad
        /// </summary>
        /// <param name="pIdentidadID">Identificador de identidad</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionDeIdentidad(Guid pIdentidadID)
        {
            return ObtenerOrganizacionDeIdentidad(pIdentidadID, true);
        }

        /// <summary>
        /// Obtiene una organización a partir de su identidad
        /// </summary>
        /// <param name="pIdentidadID">Identificador de identidad</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionDeIdentidad(Guid pIdentidadID, bool pCargaLigera)
        {
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            var resultado = mEntityContext.Organizacion.JoinPerfilPersonaOrg().JoinIdentidad().Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID)).Select(item => item.Organizacion.OrganizacionID)
                 .Union(mEntityContext.Organizacion.JoinPerfilOrganizacion().JoinIdentidad().Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID)).Select(item => item.Organizacion.OrganizacionID));

            dataWrapperOrganizacion.ListaOrganizacion = mEntityContext.Organizacion.Join(resultado, org => org.OrganizacionID, res => res, (org, res) =>
             new
             {
                 Organizacion = org,
                 resultado = res
             }).OrderBy(item => item.Organizacion.Nombre).Select(item => item.Organizacion).ToList();

            var resultado2 = mEntityContext.Organizacion.JoinPerfilPersonaOrg().JoinIdentidad().Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID)).Select(item => item.Organizacion.OrganizacionID)
                .Union(mEntityContext.Organizacion.JoinPerfilOrganizacion().JoinIdentidad().Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID)).Select(item => item.Organizacion.OrganizacionID));

            if (pCargaLigera)
            {
                dataWrapperOrganizacion.ListaPersonaVinculoOrganizacion = mEntityContext.PersonaVinculoOrganizacion.JoinPerfil().JoinIdentidad().Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID)).Select(item => item.PersonaVinculoOrganizacion).ToList();
            }
            else
            {
                dataWrapperOrganizacion.ListaPersonaVinculoOrganizacion = mEntityContext.PersonaVinculoOrganizacion.Join(resultado2, persVincOrg => persVincOrg.OrganizacionID, res2 => res2, (persVincOrg, res2) =>
                 new
                 {
                     PersonaVinculoOrganizacion = persVincOrg,
                     resultado2 = res2
                 }).Select(item => item.PersonaVinculoOrganizacion).ToList();
            }
            return dataWrapperOrganizacion;
        }

        /// <summary>
        /// Obtiene la organización a partir de su perfil
        /// </summary>
        /// <param name="pIdentidadID">Identificador de identidad</param>
        /// <returns>Dataset de organización</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionPorPerfil(Guid pPerfil)
        {
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            dataWrapperOrganizacion.ListaOrganizacion = mEntityContext.Organizacion.JoinPerfil().Where(item => item.Perfil.PerfilID.Equals(pPerfil)).Select(item => item.Organizacion).ToList().Distinct().ToList();

            return dataWrapperOrganizacion;
        }

        /// <summary>
        /// Obtiene organizaciones (carga ligera) a partir de la lista de identificadores de identidades pasada por parámetro
        /// Carga las tablas Organizacion, OrganizacionEmpresa y OrganizacionClase
        /// </summary>
        /// <param name="pListaIdentidades">Lista de identificadores de identidad</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionesPorIdentidadesCargaLigera(List<Guid> pListaIdentidades)
        {
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            dataWrapperOrganizacion.ListaOrganizacion = mEntityContext.Organizacion.JoinPerfilOrganizacion().JoinIdentidad().Where(item => pListaIdentidades.Contains(item.Identidad.IdentidadID)).Select(item => item.Organizacion).ToList().Distinct().ToList();

            return dataWrapperOrganizacion;
        }

        /// <summary>
        /// Obtiene las organizaciones en las que participa un usuario 
        /// Carga las tablas Organizacion, OrganizacionEmpresa y OrganizacionClase
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionesParticipaUsuario(Guid pUsuarioID)
        {
            DataWrapperOrganizacion dataWrapperOrganzacion = new DataWrapperOrganizacion();

            dataWrapperOrganzacion.ListaOrganizacion = mEntityContext.Organizacion.JoinPersonaVinculoOrganizacion().JoinPersona().Where(item => item.Persona.UsuarioID.Value.Equals(pUsuarioID)).OrderBy(item => item.Organizacion.Nombre).Select(item => item.Organizacion).ToList();

            return dataWrapperOrganzacion;
        }



        /// <summary>
        /// Obtiene el número de alumnos de una clase
        /// </summary>
        /// <param name="pOrganizacionID">GUID de la clase</param>
        /// <returns>Número de alumnos de la clase</returns>
        public int ObtenerNumeroAlumnosDeClase(Guid pOrganizacionID)
        {
            string consulta = "SELECT count(*) FROM PerfilPersonaOrg INNER JOIN Perfil ON PerfilPersonaOrg.PerfilID = Perfil.PerfilID WHERE Perfil.OrganizacionID = " + IBD.GuidParamValor("OrganizacionID") + " AND Perfil.Eliminado = 0";
            DbCommand commandsqlNumeroAlumnos = ObtenerComando(consulta);
            AgregarParametro(commandsqlNumeroAlumnos, IBD.GuidParamValor("OrganizacionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pOrganizacionID));

            return (int)EjecutarEscalar(commandsqlNumeroAlumnos);
        }

        /// <summary>
        /// Obtiene el número de miembros de una organización en un proyecto
        /// </summary>
        /// <param name="pOrganizacionID">GUID de la organización</param>
        /// <param name="pProyectoID">GUID del proyecto</param>
        /// <param name="pVisibles">True para obtener solo los visibles, False para obtener todos</param>
        /// <returns>Número de miembros</returns>
        public int ObtenerNumeroMiembrosDeOrganizacionEnProyecto(Guid pOrganizacionID, Guid pProyectoID, bool pVisibles)
        {
            string consulta = "SELECT count(*) FROM PerfilPersonaOrg INNER JOIN Perfil ON PerfilPersonaOrg.PerfilID = Perfil.PerfilID INNER JOIN Identidad ON PerfilPersonaOrg.PerfilID = Identidad.PerfilID WHERE Perfil.OrganizacionID = " + IBD.GuidParamValor("OrganizacionID") + " AND Identidad.ProyectoID = " + IBD.GuidParamValor("ProyectoID") + " AND Perfil.Eliminado = 0";
            if (pVisibles)
            {
                consulta = "SELECT Count(*) FROM (SELECT Perfil.* FROM PerfilPersonaOrg INNER JOIN Perfil ON PerfilPersonaOrg.PerfilID = Perfil.PerfilID INNER JOIN Identidad ON PerfilPersonaOrg.PerfilID = Identidad.PerfilID INNER JOIN PersonaVisibleEnOrg ON PerfilPersonaOrg.PersonaID = PersonaVisibleEnOrg.PersonaID WHERE Perfil.OrganizacionID = " + IBD.GuidParamValor("OrganizacionID") + " AND Identidad.ProyectoID = " + IBD.GuidParamValor("ProyectoID") + " AND Perfil.Eliminado = 0 UNION SELECT Perfil.* FROM PerfilPersonaOrg INNER JOIN Perfil ON PerfilPersonaOrg.PerfilID = Perfil.PerfilID INNER JOIN Identidad ON PerfilPersonaOrg.PerfilID = Identidad.PerfilID WHERE Perfil.OrganizacionID = " + IBD.GuidParamValor("OrganizacionID") + " AND Identidad.ProyectoID = " + IBD.GuidParamValor("ProyectoID") + " AND Identidad.Tipo = 1 AND Perfil.Eliminado = 0) Subconsulta";
            }
            DbCommand commandsqlNumeroMiembros = ObtenerComando(consulta);
            AgregarParametro(commandsqlNumeroMiembros, IBD.GuidParamValor("OrganizacionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pOrganizacionID));
            AgregarParametro(commandsqlNumeroMiembros, IBD.GuidParamValor("ProyectoID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pProyectoID));

            return (int)EjecutarEscalar(commandsqlNumeroMiembros);
        }

        /// <summary>
        /// Verdad si una organización es una clase
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizacion</param>
        /// <returns></returns>
        public bool ComprobarOrganizacionEsClase(Guid pOrganizacionID)
        {
            string consulta = "SELECT 1 FROM OrganizacionClase WHERE OrganizacionClase.OrganizacionID = " + IBD.ToParam("orgID");

            DbCommand comando = ObtenerComando(consulta);
            AgregarParametro(comando, IBD.ToParam("orgID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pOrganizacionID));
            object resultado = EjecutarEscalar(comando);

            return (resultado != null) && resultado.Equals(1);
        }

        /// <summary>
        /// Verdad si una organización es una clase
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizacion</param>
        /// <returns></returns>
        public bool ComprobarOrganizacionEsClasePrimaria(Guid pOrganizacionID)
        {
            string consulta = "SELECT 1 FROM OrganizacionClase WHERE OrganizacionClase.OrganizacionID = " + IBD.ToParam("orgID") + " AND TipoClase= " + (short)TipoClase.EducacionPrimaria;

            DbCommand comando = ObtenerComando(consulta);
            AgregarParametro(comando, IBD.ToParam("orgID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pOrganizacionID));
            object resultado = EjecutarEscalar(comando);

            return (resultado != null) && resultado.Equals(1);
        }


        /// <summary>
        /// Verdad si el usuario administra alguna clase
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns></returns>
        public bool ComprobarUsuarioAdministraAlgunaClase(Guid pUsuarioID)
        {
            int empieza = sqlSelectOrganizacionClaseClasesAdministraUsuario.IndexOf("SELECT");
            int acaba = sqlSelectOrganizacionClaseClasesAdministraUsuario.IndexOf("FROM");

            string consulta = "SELECT 1 " + sqlSelectOrganizacionClaseClasesAdministraUsuario.Remove(empieza, acaba);

            DbCommand comando = ObtenerComando(consulta);
            AgregarParametro(comando, IBD.ToParam("usuarioID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pUsuarioID));
            object resultado = EjecutarEscalar(comando);

            return (resultado != null) && resultado.Equals(1);
        }


        /// <summary>
        /// Comprueba si una persona es visible en una organización
        /// </summary>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <returns>True si es visible, false si no lo es</returns>
        public bool ComprobarPersonaEsVisibleEnOrg(Guid pPersonaID, Guid pOrganizacionID)
        {
            return mEntityContext.PersonaVisibleEnOrg.Any(item => item.PersonaID.Equals(pPersonaID) && item.OrganizacionID.Equals(pOrganizacionID));
        }

        /// <summary>
        /// Obtiene las organizaciones que solicitan acceso a un proyecto concreto
        /// Carga las tablas Organizacion, OrganizacionEmpresa y OrganizacionClase
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionesSolicitanAccesoProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            dataWrapperOrganizacion.ListaOrganizacion = mEntityContext.Organizacion.JoinSolicitudOrganizacion().JoinSolicitud().Where(item => item.Solicitud.OrganizacionID.Equals(pOrganizacionID) && item.Solicitud.ProyectoID.Equals(pProyectoID) && item.Solicitud.Estado.Equals((short)EstadoSolicitud.Espera)).Select(item => item.Organizacion).ToList();

            return dataWrapperOrganizacion;
        }

        /// <summary>
        /// Obtiene las organizaciones Gnoss en las que participa un usuario
        /// Carga las tablas Organizacion, OrganizacionEmpresa y OrganizacionClase
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionesGnossAccedeUsuario(Guid pUsuarioID)
        {
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            dataWrapperOrganizacion.ListaOrganizacion = mEntityContext.Organizacion.JoinProyectoRolUsuario().Where(item => item.ProyectoRolUsuario.UsuarioID.Equals(pUsuarioID)).Select(item => item.Organizacion).ToList().Distinct().ToList();

            return dataWrapperOrganizacion;
        }



        /// <summary>
        /// Actualiza los cambios de organizaciones
        /// </summary>
        /// <param name="pOrganizacionDS">Dataset de organizaciones</param>
        public void ActualizarOrganizaciones()
        {
            try
            {
                mEntityContext.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Obtiene las organizaciones vinculadas a la persona pasada por parámetro 
        /// Carga las tablas Organizacion, OrganizacionEmpresa, OrganizacionClase, PersonaVinculoOrganizacion
        /// </summary>
        /// <param name="pPersonaID">Identificador de persona</param>
        /// <param name="pOrganizacionID">TRUE si debe hacerse una carga pesada, FALSE si debe ser ligera</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionVinculadaAPersona(Guid pPersonaID, Guid pOrganizacionID)
        {
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            dataWrapperOrganizacion.ListaOrganizacion = mEntityContext.Organizacion.JoinPersonaVinculoOrganizacion().Where(item => item.PersonaVinculoOrganizacion.PersonaID.Equals(pPersonaID) && item.PersonaVinculoOrganizacion.OrganizacionID.Equals(pOrganizacionID)).Select(item => item.Organizacion).ToList();
            dataWrapperOrganizacion.ListaPersonaVinculoOrganizacion = mEntityContext.PersonaVinculoOrganizacion.Where(item => item.PersonaID.Equals(pPersonaID) && item.OrganizacionID.Equals(pOrganizacionID)).ToList();

            return dataWrapperOrganizacion;
        }

        /// <summary>
        /// Obtiene las organizaciones vinculadas a la persona pasada por parámetro 
        /// Carga las tablas Organizacion, OrganizacionEmpresa, OrganizacionClase, PersonaVinculoOrganizacion
        /// </summary>
        /// <param name="pPersonaID">Identificador de persona</param>
        /// <param name="pCargaPesada">TRUE si debe hacerse una carga pesada, FALSE si debe ser ligera</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionesVinculadasAPersona(Guid pPersonaID, bool pCargaPesada)
        {
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            dataWrapperOrganizacion.ListaOrganizacion = mEntityContext.Organizacion.JoinPersonaVinculoOrganizacion().Where(item => item.PersonaVinculoOrganizacion.PersonaID.Equals(pPersonaID)).Select(item => item.Organizacion).ToList();

            dataWrapperOrganizacion.ListaOrganizacionEmpresa = mEntityContext.OrganizacionEmpresa.Join(mEntityContext.PersonaVinculoOrganizacion, orgEmp => orgEmp.OrganizacionID, persVincOrg => persVincOrg.OrganizacionID, (orgEmp, persVincOrg) => new
            {
                OrganizacionEmpresa = orgEmp,
                PersonaVinculoOrganizacion = persVincOrg
            }).Where(item => item.PersonaVinculoOrganizacion.PersonaID.Equals(pPersonaID)).Select(item => item.OrganizacionEmpresa).ToList();

            dataWrapperOrganizacion.ListaPersonaVinculoOrganizacion = mEntityContext.PersonaVinculoOrganizacion.Where(item => item.PersonaID.Equals(pPersonaID)).ToList();

            return dataWrapperOrganizacion;
        }

        /// <summary>
        /// Obtiene la fila de ConfiguraciónGnossOrg de una organización
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion ObtenerConfiguracionGnossOrg(Guid pOrganizacionID)
        {
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            dataWrapperOrganizacion.ListaConfiguracionGnossOrg = mEntityContext.ConfiguracionGnossOrg.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).ToList();

            return dataWrapperOrganizacion;
        }



        /// <summary>
        /// Obtiene las tablas Organizacion, OrganizacionEmpresa, OrganizacionClase y OrganizacionParticipaProy del proyecto pasado por parámetro
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionesPorSusIdentidadesDeProyecto(Guid pProyectoID)
        {
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            dataWrapperOrganizacion.ListaOrganizacion = mEntityContext.Organizacion.JoinOrganizacionParticipaProy().Where(item => item.OrganizacionParticipaProy.ProyectoID.Equals(pProyectoID)).Select(item => item.Organizacion).ToList();

            dataWrapperOrganizacion.ListaOrganizacionParticipaProy = mEntityContext.OrganizacionParticipaProy.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();

            return dataWrapperOrganizacion;
        }

        /// <summary>
        /// Carga las tablas Organizacion, OrganizacionEmpresa, OrganizacionClase y AdministradorOrganizacion para la organización pasada por parámetro
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion CargarAdministradoresdeOrganizacion(Guid pOrganizacionID)
        {
            DataWrapperOrganizacion dataWrapeprOrganizacion = new DataWrapperOrganizacion();

            dataWrapeprOrganizacion.ListaOrganizacion = mEntityContext.Organizacion.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).ToList();

            dataWrapeprOrganizacion.ListaAdministradorOrganizacion = mEntityContext.AdministradorOrganizacion.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).ToList();

            return dataWrapeprOrganizacion;
        }

        /// <summary>
        /// Carga SOLO la tabla  "PersonaVinculoOrganizacion" carga ligera  para la organización pasada por parámetro
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion CargarPersonasVinculoOrgDeOrganizacion(Guid pOrganizacionID)
        {
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            mEntityContext.PersonaVinculoOrganizacion.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).Select(item => new { item.PersonaID, item.OrganizacionID, item.FechaVinculacion, item.Cargo, item.EmailTrabajo, item.UsarFotoPersonal });

            return dataWrapperOrganizacion;
        }

        /// <summary>
        /// Carga SOLO la tabla "PersonasVisiblesDeOrg" para la organización pasada por parámetro
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion CargarPersonasVisiblesDeOrganizacion(Guid pOrganizacionID)
        {
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            dataWrapperOrganizacion.ListaPersonaVisibleEnOrg = mEntityContext.PersonaVisibleEnOrg.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).OrderBy(item => item.Orden).ToList();

            return dataWrapperOrganizacion;
        }

        /// <summary>
        /// Indica si la persona es visible en la organizacion
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <returns>Booleano que indica si la persona es visible en la organizacion</returns>
        public bool EsPersonaVisibleEnOrganizacion(Guid pOrganizacionID, Guid pPersonaID)
        {
            return mEntityContext.PersonaVisibleEnOrg.Any(item => item.OrganizacionID.Equals(pOrganizacionID) && item.PersonaID.Equals(pPersonaID));
        }

        /// <summary>
        /// Carga SOLO la tabla "PersonasVisiblesDeOrg" para las organizaciones en las que una persona es visible
        /// </summary>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion CargarOrganizacionesDePersonaVisible(Guid pPersonaID)
        {
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            dataWrapperOrganizacion.ListaPersonaVisibleEnOrg = mEntityContext.PersonaVisibleEnOrg.Where(item => item.PersonaID.Equals(pPersonaID)).ToList();

            return dataWrapperOrganizacion;
        }

        /// <summary>
        /// Obtiene las organizaciones que administra un usuario pasado por parámetro
        /// Carga la tabla AdministradorOrganizacion
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion CargarOrganizacionesAdministraUsuario(Guid pUsuarioID)
        {
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            dataWrapperOrganizacion.ListaAdministradorOrganizacion = mEntityContext.AdministradorOrganizacion.Where(item => item.UsuarioID.Equals(pUsuarioID)).ToList();

            return dataWrapperOrganizacion;
        }

        /// <summary>
        /// Indica si el usuario ers el único administrador de una organización
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>True si es el único administrador de alguna organización</returns>
        public bool EsUsuarioAdministradorUnicoDeOrganizacion(Guid pUsuarioID)
        {
            return mEntityContext.AdministradorOrganizacion.Where(item2 => item2.UsuarioID.Equals(pUsuarioID) && item2.Tipo.Equals((short)TipoAdministradoresOrganizacion.Administrador)).GroupBy(item => item.OrganizacionID).Where(agrupacion => agrupacion.Count() == 1).Any();
        }


        /// <summary>
        /// Comprueba si una organización tiene en base de datos creado un Tesauro propio
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        /// <returns>TRUE si tiene tesauro, FALSE en caso contrario</returns>
        public bool TieneTesauroPropio(Guid pOrganizacionID)
        {
            return mEntityContext.TesauroOrganizacion.Any(item => item.OrganizacionID.Equals(pOrganizacionID));
        }

        /// <summary>
        /// Comprueba si una organización tiene en base de datos creado una base de recursos propia
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        /// <returns>TRUE si tiene base de recursos, FALSE en caso contrario</returns>
        public bool TieneBaseDeRecursos(Guid pOrganizacionID)
        {
            return mEntityContext.BaseRecursosOrganizacion.Any(item => item.OrganizacionID.Equals(pOrganizacionID));
        }

        /// <summary>
        /// Comprueba si una organización tiene en base de datos creado un perfil de organización
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        /// <returns>TRUE si tiene perfil de organización, FALSE en caso contrario</returns>
        public bool TienePerfil(Guid pOrganizacionID)
        {
            return mEntityContext.PerfilOrganizacion.Any(item => item.OrganizacionID.Equals(pOrganizacionID));
        }


        /// <summary>
        /// Obtiene la fila de la vinculación de una persona a una organización.
        /// </summary>
        /// <param name="pOrganizacionID">ID de la org</param>
        /// <param name="pPersonaID">ID de la persona</param>
        /// <returns>Fila de la vinculación de una persona a una organización</returns>
        public string ObtenerCargoPersonaVinculoOrganizacion(Guid pOrganizacionID, Guid pPersonaID)
        {
            return mEntityContext.PersonaVinculoOrganizacion.Where(item => item.PersonaID.Equals(pPersonaID) && item.OrganizacionID.Equals(pOrganizacionID)).Select(item => item.Cargo).FirstOrDefault();
        }

        /// <summary>
        /// Comprueba si existe una organización con el nombre corto pasado por parámetro
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto</param>
        /// <returns>TRUE si existe, FALSE en caso contrario</returns>
        public bool ExisteNombreCortoEnBD(string pNombreCorto)
        {
            return mEntityContext.Organizacion.Any(item => item.NombreCorto.ToUpper().Equals(pNombreCorto.ToUpper()));
        }

        /// <summary>
        /// Obtiene los nombres cortos que empizan con los caracteres introducidos.
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto</param>
        /// <returns>nombres cortos que empizan con los caracteres introducidos</returns>
        public List<string> ObtenerNombresCortosEmpiezanPor(string pNombreCorto)
        {
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            dataWrapperOrganizacion.ListaOrganizacion = mEntityContext.Organizacion.Where(item => item.NombreCorto.StartsWith(pNombreCorto)).ToList();

            List<string> listaNombres = new List<string>();

            foreach (var filaOrg in dataWrapperOrganizacion.ListaOrganizacion)
            {
                listaNombres.Add(filaOrg.NombreCorto);
            }

            return listaNombres;
        }

        /// <summary>
        /// Comprueba si el usuario pasado como parámetro es administrador de la clase indicada
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de organización de tipo clase</param>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>TRUE si el usuario administra la clase, FALSE en caso contrario</returns>
        public bool EsUsuarioAdministradorClase(Guid pOrganizacionID, Guid pUsuarioID)
        {
            return mEntityContext.AdministradorOrganizacion.Any(item => item.OrganizacionID.Equals(pOrganizacionID) && item.UsuarioID.Equals(pUsuarioID));
        }

        /// <summary>
        /// Comprueba si hay alguna persona visible en la organización
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <returns>TRUE si hay alguna persona visible, FALSE en caso contrario</returns>
        public bool HayPersonasVisibles(Guid pOrganizacionID)
        {
            return mEntityContext.PersonaVisibleEnOrg.Any(item => item.OrganizacionID.Equals(pOrganizacionID));
        }

        /// <summary>
        /// Obtiene los tags de varias organizaciones en un proyecto
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad de la organización en el proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public Dictionary<Guid, string> ObtenerTagsDeOrganizacionesEnProyecto(List<Guid> pListaIdentidadID, Guid pProyectoID)
        {
            Dictionary<Guid, string> tags = new Dictionary<Guid, string>();

            foreach (Guid id in pListaIdentidadID)
            {
                tags.Add(id, "");
            }

            if (pListaIdentidadID.Count > 0)
            {
                var consultaIdentidades = mEntityContext.Organizacion.JoinPerfil().JoinIdentidad().JoinCurriculum().JoinDocumento().Where(item => pListaIdentidadID.Contains(item.Identidad.IdentidadID) && item.Identidad.ProyectoID.Equals(pProyectoID) && item.Identidad.FechaBaja == null && item.Identidad.FechaExpulsion == null && !item.Perfil.Eliminado && !item.Organizacion.Eliminada && item.Identidad.Tipo.Equals(3)).Select(item => new { item.Identidad.IdentidadID, item.Identidad.CurriculumID, item.Curriculum.Tags }).ToList().Distinct().ToList();

                foreach (var fila in consultaIdentidades.Where(item => item.Tags != null))
                {
                    Guid idDoc = (Guid)fila.IdentidadID;
                    string tagss = (string)fila.Tags;
                    tags[idDoc] = tagss;
                }
            }
            return tags;
        }

        /// <summary>
        /// Obtiene los tags de una organización en un proyecto
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad de la organización en el proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public string ObtenerTagsDeOrganizacionEnProyecto(Guid pIdentidadID, Guid pProyectoID)
        {
            List<Guid> lista = new List<Guid>();
            lista.Add(pIdentidadID);
            return ObtenerTagsDeOrganizacionesEnProyecto(lista, pProyectoID)[pIdentidadID];
        }

        /// <summary>
        /// Obtiene las organizaciones que participan en un proyecto (OrganizacionID, Nombre, NombreCorto y Alias)
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public DataWrapperOrganizacion ObtenerOrganizacionesParticipanEnProyecto(Guid pProyectoID)
        {
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            dataWrapperOrganizacion.ListaOrganizacion = mEntityContext.Organizacion.JoinOrganizacionParticipaProy().Where(item => !item.OrganizacionParticipaProy.EstaBloqueada && item.OrganizacionParticipaProy.ProyectoID.Equals(pProyectoID)).Select(item => item.Organizacion).ToList();

            return dataWrapperOrganizacion;
        }

        /// <summary>
        /// Comprueba si la organización participa en el proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pOrganizacionID">Identificador de la organizacion</param>
        /// <returns>true si la organización participa en el proyecto</returns>
        public bool ParticipaOrganizacionEnProyecto(Guid pProyectoID, Guid pOrganizacionID)
        {
            DataWrapperOrganizacion dataWrapperOrganizacion = ObtenerOrganizacionesParticipanEnProyecto(pProyectoID);
            return dataWrapperOrganizacion.ListaOrganizacion.Where(item => item.OrganizacionID.Equals(pOrganizacionID)) != null;
        }

        /// <summary>
        /// Obtiene las filas de la tabla PersonaVinculoOrganizacion cuya organizacion comienza por pInicio
        /// </summary>
        /// <param name="pInicio">caracteres por los que comienza la organizacion</param>
        /// <returns></returns>
        public List<PersonaVinculoOrganizacion> ObtenerFilasPersonaVincOrganizacion(string pInicio)
        {
            var query = mEntityContext.PersonaVinculoOrganizacion;

            if (!string.IsNullOrEmpty(pInicio))
            {
                return query.Where(item => item.OrganizacionID.ToString().StartsWith(pInicio)).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        /// <summary>
        /// Obtiene las filas de la tabla Organizacion cuya organizacion comienzapor pInicio
        /// </summary>
        /// <param name="pInicio">caracteres por los que comienza la organizacion</param>
        /// <returns></returns>
        public List<Organizacion> ObtenerFilasOrganizaciones(string pInicio)
        {
            var query = mEntityContext.Organizacion;

            if (!string.IsNullOrEmpty(pInicio))
            {
                return query.Where(item => item.OrganizacionID.ToString().StartsWith(pInicio)).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        /// <summary>
        /// Actualiza las coordenadas de la organizacion indicada
        /// </summary>
        /// <param name="pOrganizacionID">ID de la Organizacion</param>
        /// <param name="pCoordenadas">Coordenadas del Logo</param>
        public void ActualizarCoordenadasOrganizacion(Guid pOrganizacionID, string pCoordenadas)
        {
            var resultado = mEntityContext.Organizacion.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).FirstOrDefault();
            if (resultado != null)
            {
                resultado.CoordenadasLogo = pCoordenadas;
            }
            mEntityContext.SaveChanges();
        }

        /// <summary>
        /// Actualiza las coordenadas de la persona en la organizacion indicada
        /// </summary>
        /// <param name="pOrganizacionID">ID de la Organizacion</param>
        /// <param name="pPersonaID">ID de la Persona</param>
        /// <param name="pCoordenadas">Coordenadas de la foto</param>
        public void ActualizarCoordenadasPersonaVincOrganizacion(Guid pOrganizacionID, Guid pPersonaID, string pCoordenadas)
        {
            var resultado = mEntityContext.PersonaVinculoOrganizacion.Where(item => item.OrganizacionID.Equals(pOrganizacionID) && item.PersonaID.Equals(pPersonaID)).FirstOrDefault();
            if (resultado != null)
            {
                resultado.CoordenadasFoto = pCoordenadas;
                resultado.FechaAnadidaFoto = DateTime.Now;
            }
            mEntityContext.SaveChanges();
        }


        /// <summary>
        /// Actualiza el número de la versión de la foto de la organización
        /// </summary>
        /// <param name="pOrganizacionID">ID de la Orgnizacion</param>
        public void ActualizarVersionFotoOrganizacion(Guid pOrganizacionID)
        {
            var resultado = mEntityContext.Organizacion.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).FirstOrDefault();
            if (resultado != null)
            {
                if (!resultado.VersionLogo.HasValue)
                {
                    resultado.VersionLogo = 1;
                }
                else
                {
                    resultado.VersionLogo++;
                }
            }
            mEntityContext.SaveChanges();
        }

        /// <summary>
        /// Actualiza el número de la versión de la foto de la persona en la organización
        /// </summary>
        /// <param name="pOrganizacionID">ID de la Orgnizacion</param>
        /// <param name="pPersonaID">ID de la Persona</param>
        public void ActualizarVersionFotoPersonaVincOrganizacion(Guid pOrganizacionID, Guid pPersonaID)
        {
            var resultado = mEntityContext.PersonaVinculoOrganizacion.Where(item => item.OrganizacionID.Equals(pOrganizacionID) && item.PersonaID.Equals(pPersonaID)).FirstOrDefault();
            if (resultado != null)
            {
                if (!resultado.VersionFoto.HasValue)
                {
                    resultado.VersionFoto = 1;
                }
                else
                {
                    resultado.VersionFoto++;
                }
            }
            mEntityContext.SaveChanges();
        }


        #endregion

        #region Privados
        
        /// <summary>
        /// En caso de que se utilice el GnossConfig.xml por defecto se sigue utilizando el IBD estático
        /// </summary>
        private void CargarConsultasYDataAdapters()
        {
            this.CargarConsultasYDataAdapters(IBD);
        }

        /// <summary>
        /// En caso de que se utilice un GnossConfig.xml que no es el de por defecto se pasa un objeto IBaseDatos creado con respecto
        /// al fichero de configuracion que se ha apsado como parámetro
        /// </summary>
        /// <param name="IBD">Objecto IBaseDatos para el archivo pasado al s del AD</param>
        private void CargarConsultasYDataAdapters(IBaseDatos IBD)
        {
            #region Consultas

            #region Sólo la parte del Select

            this.SelectOrganizacionClase = "SELECT " + IBD.CargarGuid("OrganizacionClase.OrganizacionID") + ", OrganizacionClase.Centro, OrganizacionClase.Asignatura, OrganizacionClase.Curso, OrganizacionClase.Grupo, OrganizacionClase.CursoAcademico, OrganizacionClase.NombreCortoCentro, OrganizacionClase.NombreCortoAsig, OrganizacionClase.TipoClase FROM OrganizacionClase";

            #endregion

            this.sqlSelectOrganizacionClaseClasesAdministraUsuario = SelectOrganizacionClase.Replace("FROM", ", Organizacion.Nombre FROM") + " INNER JOIN AdministradorOrganizacion ON AdministradorOrganizacion.OrganizacionID = OrganizacionClase.OrganizacionID INNER JOIN Organizacion ON Organizacion.OrganizacionID = AdministradorOrganizacion.OrganizacionID WHERE (AdministradorOrganizacion.UsuarioID = " + IBD.GuidParamValor("usuarioID") + ") AND AdministradorOrganizacion.Tipo = " + (short)TipoAdministradoresOrganizacion.Administrador + " AND Organizacion.OrganizacionID NOT IN (SELECT PersonaVinculoOrganizacion.OrganizacionID FROM PersonaVinculoOrganizacion INNER JOIN Persona ON Persona.PersonaID = PersonaVinculoOrganizacion.PersonaID WHERE Persona.UsuarioID = " + IBD.GuidParamValor("usuarioID") + ")";

            #endregion
        }

        #endregion

        #endregion
    }
}