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
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models;
using Microsoft.EntityFrameworkCore;
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

        #region Constructores

        /// <summary>
        /// El por defecto, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        public OrganizacionAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            mEntityContext = entityContext;
            mConfigService = configService;
            mLoggingService = loggingService;
            
            this.CargarConsultasYDataAdapters();
        }

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuracion de conexion a la Base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD"></param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public OrganizacionAD(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            mEntityContext = entityContext;
            mConfigService = configService;
            mLoggingService = loggingService;
            
            this.CargarConsultasYDataAdapters(IBD);
        }

        #endregion

        #region Consultas

        #region Sólo la parte del Select

        private string SelectPesadoOrganizacion;
        private string SelectOrganizacionEmpresa;
        private string SelectLigeroOrganizacion;
        private string SelectOrganizacionClase;
        private string SelectOrganizacionSinLogotipo;
        private string SelectOrganizacionParticipaProy;
        private string SelectTagOrganizacion;
        private string SelectAdministradorOrganizacion;
        private string SelectPesadoPersonaVinculoOrganizacion;
        private string SelectLigeroPersonaVinculoOrganizacion;
        private string SelectPersonaVisibleEnOrg;
        private string SelectEmpleadoHistoricoEstadoLaboral;
        private string SelectHistoricoOrganizacionParticipaProy;
        private string SelectIdentidadOrganizacion;
        private string SelectOrganizacionGnoss;
        private string SelectPesadoSede;
        private string SelectLigeroSede;
        private string SelectCnaeAgregacionOrganizacion;
        private string SelectConfiguracionGnossOrg;

        #endregion

        private string sqlSelectObtenerOrganizacionesSolicitanAccesoAProyecto;
        private string sqlSelectObtenerOrganizacionesEmpresaSolicitanAccesoAProyecto;
        private string sqlSelectObtenerOrganizacionesClaseSolicitanAccesoAProyecto;
        private string sqlSelectExisteOrganizacionEnBD;
        private string sqlSelectObtenerOrganizacionesDePersonasDeEstructuraDeProyectoCargaLigera;
        private string sqlSelectObtenerPersonaVinculoOrganizacionDeEstructuraDeProyectoCargaLigera;
        private string sqlSelectOrganizacionPorID;
        private string sqlSelectExisteOrganizacionEnBDPorIDOrg;
        private string sqlSelectOrganizacionClasePorID;
        private string sqlSelectOrganizacionPorIDLigero;
        private string sqlSelectOrganizacionEmpresaPorID;
        private string sqlSelectNombreOrganizacionPorID;
        private string sqlSelectOrganizacionesParticipaUsuario;
        private string sqlSelectOrganizacionesEmpresaParticipaUsuario;
        private string sqlSelectOrganizacionesClaseParticipaUsuario;
        private string sqlSelectOrganizacionClaseClasesAdministraUsuario;
        private string sqlSelectOrganizacionesDeIdentidad;
        private string sqlSelectOrganizacionesEmpresaDeIdentidad;
        private string sqlSelectOrganizacionesClaseDeIdentidad;
        private string sqlSelectOrganizacionesClaseDePersonaSinPersonaVinculadaOrganizacion;
        private string sqlSelectPersonaVinculoOrganizacionDeIdentidad;
        private string sqlSelectOrganizacionesGnossAccedeUsuario;
        private string sqlSelectOrganizacionesEmpresaAccedeUsuario;
        private string sqlSelectOrganizacionesClaseAccedeUsuario;
        private string sqlSelectTodasOrgPesada;
        private string sqlSelectTodasOrgLigera;
        //private string sqlSelectTodasOrgEmpresa;
        //private string sqlSelectTodasOrgClase;
        private string sqlSelectOrganizacionesVinculadasAPersona;

        private string sqlSelectOrganizacionDePersYOrg;
        private string sqlSelectOrgEmpresaDePersYOrg;
        private string sqlSelectOrganizacionClaseDePersYOrg;
        private string sqlSelectPersonaVinculoOrganizacionDePersYOrg;

        private string sqlSelectOrganizacionesDePersona;
        private string sqlSelectOrganizacionesDePersonaPesada;
        private string sqlSelectOrganizacionesEmpresaDePersona;
        private string sqlSelectOrganizacionesEmpresaDePersonaPesada;
        private string sqlSelectOrganizacionesClaseDePersona;
        private string sqlSelectTagOrganizacionesDePersona;
        private string sqlSelectOrganizacionesVinculadasAPersonaPesada;
        private string sqlSelectTodasOrgGnossCargaLigera;
        private string sqlSelectTodasOrgGnossCargaPesada;
        private string sqlSelectTodasOrgEmpresaGnossCargaLigera;
        private string sqlSelectTodasOrgEmpresaGnossCargaPesada;
        private string sqlSelectTodasOrgClaseGnoss;
        private string sqlSelectOrgGnoss;
        private string sqlSelectTodasCnaeAgregacionOrganizacionOrgGnoss;
        private string sqlSelectCnaeAgregacionDeOrganizacion;
        private string sqlSelectTodosOrgGnossAdministradorOrganizacion;
        private string sqlSelectObtenerOrganizacionesGnossDePersonasDeEstructuraDeProyectoCargaLigera;
        private string sqlSelectOrgGnossPorID;
        private string sqlSelectTodasCnaeAgregacionOrganizacionOrgGnossPorID;
        private string sqlSelectTodosOrgGnossAdministradorOrganizacionPorID;
        private string sqlSelectLigeroPersonaVinculoOrgPorID;
        private string sqlSelectPesadoPersonaVinculoOrgPorID;
        private string sqlSelectObtenerOrganizacionesPorSusIdentidadesDeProyecto;
        private string sqlSelectObtenerOrganizacionesEmpresaPorSusIdentidadesDeProyecto;
        private string sqlSelectObtenerOrganizacionesClasePorSusIdentidadesDeProyecto;
        private string sqlSelectObtenerOrganizacionParticipaProyPorSusIdentidadesDeProyecto;
        private string sqlSelectTodasOrgLigeraCorporativo;
        private string sqlSelectTodasOrgEmpresaLigeraCorporativo;
        private string sqlSelectTodasOrgClaseCorporativo;
        private string sqlSelectTagsOrganizacionGnoss;
        private string sqlSelectEmpleadoHistoricoEstadoLaboralDePersona;
        private string sqlSelectEmpleadoHistoricoEstadoLaboralDeOrganizacion;
        private string sqlSelectOcupacionesParticipaOrganizacion;
        private string sqlSelectExisteNombreCortoEnBD;
        private string sqlSelectOrganizacionIDPorNombreOrg;
        private string sqlSelectObtenerOrganizacionParticipaProyDeProyecto;
        private string sqlSelectTablaBaseOrganizacionIDDeOrganizacionPorID;
        private string sqlSelectPersonasVisiblesDeOrg;
        private string sqlSelectOrganizacionesVisiblesDePers;
        private string sqlSelectEsUsuarioAdministradorOrganizacion;

        #endregion

        #region DataAdapter

        #region Organizacion

        string sqlOrganizacionInsert;
        string sqlOrganizacionDelete;
        string sqlOrganizacionModify;

        #endregion

        #region OrganizacionEmpresa

        string sqlOrganizacionEmpresaInsert;
        string sqlOrganizacionEmpresaDelete;
        string sqlOrganizacionEmpresaModify;

        #endregion

        #region OrganizacionClase

        string sqlOrganizacionClaseInsert;
        string sqlOrganizacionClaseDelete;
        string sqlOrganizacionClaseModify;

        #endregion

        #region OrganizacionParticipaProy

        string sqlOrganizacionParticipaProyInsert;
        string sqlOrganizacionParticipaProyDelete;
        string sqlOrganizacionParticipaProyModify;

        #endregion

        #region TagOrganizacion

        string sqlTagOrganizacionInsert;
        string sqlTagOrganizacionDelete;
        string sqlTagOrganizacionModify;

        #endregion

        #region AdministradorOrganizacion

        string sqlAdministradorOrganizacionInsert;
        string sqlAdministradorOrganizacionDelete;
        string sqlAdministradorOrganizacionModify;

        #endregion

        #region PersonaVinculoOrganizacion

        string sqlPersonaVinculoOrganizacionInsert;
        string sqlPersonaVinculoOrganizacionDelete;
        string sqlPersonaVinculoOrganizacionModify;

        #endregion

        #region PersonaVisibleEnOrg

        string sqlPersonaVisibleEnOrgInsert;
        string sqlPersonaVisibleEnOrgDelete;
        string sqlPersonaVisibleEnOrgModify;

        #endregion

        #region EmpleadoHistoricoEstadoLaboral

        string sqlEmpleadoHistoricoEstadoLaboralInsert;
        string sqlEmpleadoHistoricoEstadoLaboralDelete;
        string sqlEmpleadoHistoricoEstadoLaboralModify;

        #endregion

        #region HistoricoOrgParticipaProy

        string sqlHistoricoOrganizacionParticipaProyInsert;
        string sqlHistoricoOrganizacionParticipaProyDelete;
        string sqlHistoricoOrganizacionParticipaProyModify;

        #endregion

        #region OrganizacionGnoss

        string sqlOrganizacionGnossInsert;
        string sqlOrganizacionGnossDelete;
        string sqlOrganizacionGnossModify;

        #endregion

        #region Sede

        string sqlSedeInsert;
        string sqlSedeDelete;
        string sqlSedeModify;

        #endregion

        #region CnaeAgregacionOrganizacion

        string sqlCnaeAgregacionOrganizacionInsert;
        string sqlCnaeAgregacionOrganizacionDelete;
        string sqlCnaeAgregacionOrganizacionModify;

        #endregion

        #region ConfiguracionGnossOrg

        string sqlConfiguracionGnossOrgInsert;
        string sqlConfiguracionGnossOrgDelete;
        string sqlConfiguracionGnossOrgModify;

        #endregion

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
            //TODO: hecho
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            if (pListaOrganizacionID.Count > 0)
            {
                dataWrapperOrganizacion.ListaOrganizacion = mEntityContext.Organizacion.Where(item => pListaOrganizacionID.Contains(item.OrganizacionID)).ToList();
            }

            return dataWrapperOrganizacion;

            //    OrganizacionDS organizacionDS = new OrganizacionDS();

            //if (pListaOrganizacionID.Count > 0)
            //{
            //    // Organizacion
            //    DbCommand commandsqlSelectOrganizacionPorID = ObtenerComando(this.SelectPesadoOrganizacion);

            //    //OrganizacionEmpresa
            //    DbCommand commandsqlSelectOrganizacionEmpresaPorID = ObtenerComando(this.SelectOrganizacionEmpresa);

            //    //OrganizacionClase
            //    DbCommand commandsqlSelectOrganizacionClasePorID = ObtenerComando(this.SelectOrganizacionClase);

            //    string concatenador = " WHERE ";
            //    int numeroParametro = 0;

            //    foreach (Guid id in pListaOrganizacionID)
            //    {
            //        commandsqlSelectOrganizacionPorID.CommandText += concatenador + "(OrganizacionID = " + IBD.GuidParamValor("organizacionID" + numeroParametro.ToString()) + ")";

            //        AgregarParametro(commandsqlSelectOrganizacionPorID, IBD.ToParam("organizacionID" + numeroParametro.ToString()), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(id));

            //        concatenador = " OR ";
            //        numeroParametro++;
            //    }
            //    CargarDataSet(commandsqlSelectOrganizacionPorID, organizacionDS, "Organizacion");
            //    CargarDataSet(commandsqlSelectOrganizacionEmpresaPorID, organizacionDS, "OrganizacionEmpresa");
            //    CargarDataSet(commandsqlSelectOrganizacionClasePorID, organizacionDS, "OrganizacionClase");
            //}
            //return (organizacionDS);
        }

        /// <summary>
        /// Obtiene organizaciones a partir de sus identificadores. Carga las tablas "Organizacion"
        /// </summary>
        /// <param name="pListaOrganizacionID">Lista de identificadores de organizaciones</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionesPorIDCargaLigera(List<Guid> pListaOrganizacionID)
        {
            //TODO: hecho 
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            if (pListaOrganizacionID.Count > 0)
            {
                dataWrapperOrganizacion.ListaOrganizacion = mEntityContext.Organizacion.Where(item => pListaOrganizacionID.Contains(item.OrganizacionID)).ToList();
            }

            //    OrganizacionDS organizacionDS = new OrganizacionDS();

            //if (pListaOrganizacionID.Count > 0)
            //{
            //    // Organizacion
            //    DbCommand commandsqlSelectOrganizacionPorID = ObtenerComando(this.SelectLigeroOrganizacion);

            //    //OrganizacionEmpresa
            //    DbCommand commandsqlSelectOrganizacionEmpresaPorID = ObtenerComando(this.SelectOrganizacionEmpresa);

            //    //OrganizacionClase
            //    DbCommand commandsqlSelectOrganizacionClasePorID = ObtenerComando(this.SelectOrganizacionClase);

            //    string concatenador = " WHERE ";
            //    int numeroParametro = 0;

            //    foreach (Guid id in pListaOrganizacionID)
            //    {
            //        //OrganizacionPorID
            //        commandsqlSelectOrganizacionPorID.CommandText += concatenador + "(OrganizacionID = " + IBD.GuidParamValor("organizacionID" + numeroParametro.ToString()) + ")";

            //        AgregarParametro(commandsqlSelectOrganizacionPorID, IBD.ToParam("organizacionID" + numeroParametro.ToString()), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(id));

            //        //OrganizacionEmpresaPorID
            //        commandsqlSelectOrganizacionEmpresaPorID.CommandText += concatenador + "(OrganizacionID = " + IBD.GuidParamValor("organizacionID" + numeroParametro.ToString()) + ")";

            //        AgregarParametro(commandsqlSelectOrganizacionEmpresaPorID, IBD.ToParam("organizacionID" + numeroParametro.ToString()), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(id));

            //        //OrganizacionClasePorID
            //        commandsqlSelectOrganizacionClasePorID.CommandText += concatenador + "(OrganizacionID = " + IBD.GuidParamValor("organizacionID" + numeroParametro.ToString()) + ")";

            //        AgregarParametro(commandsqlSelectOrganizacionClasePorID, IBD.ToParam("organizacionID" + numeroParametro.ToString()), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(id));

            //        concatenador = " OR ";
            //        numeroParametro++;
            //    }
            //    CargarDataSet(commandsqlSelectOrganizacionPorID, organizacionDS, "Organizacion");
            //    CargarDataSet(commandsqlSelectOrganizacionEmpresaPorID, organizacionDS, "OrganizacionEmpresa");
            //    CargarDataSet(commandsqlSelectOrganizacionClasePorID, organizacionDS, "OrganizacionClase");
            //}
            //return (organizacionDS);
            return dataWrapperOrganizacion;
        }

        /// <summary>
        /// Obtiene las organizaciones a partir de identidades cargardas en un dataSet.
        /// </summary>
        /// <param name="pDataWrapperIdentidad">DataSet con las identidades de organizaciones</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionesDeIdentidadesCargadas(DataWrapperIdentidad pDataWrapperIdentidad)
        {
            //TODO: hecho
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

            //OrganizacionDS organizacionDS = new OrganizacionDS();
            //string whereOrg = "";

            //foreach (Perfil filaPerfil in pDataWrapperIdentidad.ListaPerfil)
            //{
            //    if (filaPerfil.OrganizacionID.HasValue)
            //    {
            //        whereOrg += IBD.GuidValor(filaPerfil.OrganizacionID.Value) + ",";
            //    }
            //}

            //if (whereOrg != "")
            //{
            //    whereOrg = " WHERE OrganizacionID IN (" + whereOrg.Substring(0, whereOrg.Length - 1) + ")";

            //    DbCommand commandSelectOrgIdentidades = ObtenerComando(SelectPesadoOrganizacion);

            //    //Organización
            //    commandSelectOrgIdentidades.CommandText = "SELECT " + IBD.CargarGuid("Organizacion.OrganizacionID") + ", Organizacion.Nombre, Organizacion.Telefono, Organizacion.Email, Organizacion.Fax, Organizacion.Web, Organizacion.Logotipo, " + IBD.CargarGuid("Organizacion.PaisID") + ", " + IBD.CargarGuid("Organizacion.ProvinciaID") + ", Organizacion.Provincia, " + IBD.CargarGuid("Organizacion.OrganizacionPadreID") + ", Organizacion.Direccion, Organizacion.CP, Organizacion.Localidad, Organizacion.EsBuscable, Organizacion.EsBuscableExternos, Organizacion.ModoPersonal, Organizacion.Eliminada, Organizacion.NombreCorto, Organizacion.CoordenadasLogo,Organizacion.VersionLogo, Organizacion.Alias FROM Organizacion" + whereOrg;
            //    CargarDataSet(commandSelectOrgIdentidades, organizacionDS, "Organizacion");

            //    //PersonaVinculoOrganizacion
            //    commandSelectOrgIdentidades.CommandText = SelectLigeroPersonaVinculoOrganizacion + whereOrg;
            //    CargarDataSet(commandSelectOrgIdentidades, organizacionDS, "PersonaVinculoOrganizacion");

            //    //ConfiguracionGnossOrg
            //    commandSelectOrgIdentidades.CommandText = SelectConfiguracionGnossOrg + " FROM ConfiguracionGnossOrg " + whereOrg;
            //    CargarDataSet(commandSelectOrgIdentidades, organizacionDS, "ConfiguracionGnossOrg");

            //    //OrganizacionParticipaProy
            //    commandSelectOrgIdentidades.CommandText = SelectOrganizacionParticipaProy + " FROM OrganizacionParticipaProy " + whereOrg;
            //    CargarDataSet(commandSelectOrgIdentidades, organizacionDS, "OrganizacionParticipaProy");

            //    //PersonaVisibleEnOrg
            //    commandSelectOrgIdentidades.CommandText = SelectPersonaVisibleEnOrg + whereOrg;
            //    CargarDataSet(commandSelectOrgIdentidades, organizacionDS, "PersonaVisibleEnOrg");

            //}
            //return organizacionDS;
            return dataWrapperOrganizacion;
        }

        /// <summary>
        /// Obtiene las organizaciones a partir de una lista de identidades
        /// </summary>
        /// <param name="pListaIdentidades">Lista de identidades de organizaciones</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionesPorIdentidad(List<Guid> pListaIdentidades)
        {
            //TODO: hecho
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

            //OrganizacionDS organizacionDS = new OrganizacionDS();

            //if (pListaIdentidades.Count > 0)
            //{
            //    string select = "";

            //    string where = " WHERE IdentidadID IN (";
            //    DbCommand commandsqlSelectOrganizacionesPorIdentidad = ObtenerComando(SelectPesadoOrganizacion);

            //    foreach (Guid id in pListaIdentidades)
            //    {
            //        where += " " + IBD.GuidValor(id) + ", ";

            //    }
            //    where = where.Substring(0, where.Length - 2) + " ) ";

            //    select += " WHERE OrganizacionID IN ( " +
            //    " SELECT Perfil.OrganizacionID FROM Perfil INNER JOIN Identidad ON Perfil.PerfilID = Identidad.PerfilID " + where +
            //    " AND Perfil.OrganizacionID is not null )";

            //    //Organización
            //    commandsqlSelectOrganizacionesPorIdentidad.CommandText = SelectPesadoOrganizacion + select;
            //    CargarDataSet(commandsqlSelectOrganizacionesPorIdentidad, organizacionDS, "Organizacion");

            //    //PersonaVinculoOrganizacion
            //    commandsqlSelectOrganizacionesPorIdentidad.CommandText = SelectLigeroPersonaVinculoOrganizacion + select;
            //    CargarDataSet(commandsqlSelectOrganizacionesPorIdentidad, organizacionDS, "PersonaVinculoOrganizacion");

            //    //OrganizacionParticipaProy
            //    commandsqlSelectOrganizacionesPorIdentidad.CommandText = SelectOrganizacionParticipaProy + " FROM OrganizacionParticipaProy " + select;
            //    CargarDataSet(commandsqlSelectOrganizacionesPorIdentidad, organizacionDS, "OrganizacionParticipaProy");

            //    //OrganizacionEmpresa
            //    commandsqlSelectOrganizacionesPorIdentidad.CommandText = SelectOrganizacionEmpresa + select;
            //    CargarDataSet(commandsqlSelectOrganizacionesPorIdentidad, organizacionDS, "OrganizacionEmpresa");

            //    //OrganizacionClase
            //    commandsqlSelectOrganizacionesPorIdentidad.CommandText = SelectOrganizacionClase + select;
            //    CargarDataSet(commandsqlSelectOrganizacionesPorIdentidad, organizacionDS, "OrganizacionClase");

            //    //ConfiguracionGnossOrg
            //    commandsqlSelectOrganizacionesPorIdentidad.CommandText = SelectConfiguracionGnossOrg + " FROM ConfiguracionGnossOrg " + select;
            //    CargarDataSet(commandsqlSelectOrganizacionesPorIdentidad, organizacionDS, "ConfiguracionGnossOrg");

            //    //PersonaVisibleEnOrg
            //    commandsqlSelectOrganizacionesPorIdentidad.CommandText = SelectPersonaVisibleEnOrg + select;
            //    CargarDataSet(commandsqlSelectOrganizacionesPorIdentidad, organizacionDS, "PersonaVisibleEnOrg");

            //}
            //return (organizacionDS);
            return dataWrapperOrganizacion;
        }

        /// <summary>
        /// Obtiene las organizaciones a partir de una identidad
        /// </summary>
        /// <param name="pIdentidad">Identificador de identidad</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionesPorIdentidad(Guid pIdentidad)
        {
            //TODO: hecho
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            List<Guid> listaOrganizacionID = mEntityContext.Perfil.JoinIdentidad().Where(item => item.Identidad.IdentidadID.Equals(pIdentidad) && item.Perfil.OrganizacionID != null).Select(item => item.Perfil.OrganizacionID.Value).ToList();

            dataWrapperOrganizacion.ListaOrganizacion = mEntityContext.Organizacion.Where(item => listaOrganizacionID.Contains(item.OrganizacionID)).ToList();

            dataWrapperOrganizacion.ListaPersonaVinculoOrganizacion = mEntityContext.PersonaVinculoOrganizacion.Where(item => listaOrganizacionID.Contains(item.OrganizacionID)).ToList();

            dataWrapperOrganizacion.ListaConfiguracionGnossOrg = mEntityContext.ConfiguracionGnossOrg.Where(item => listaOrganizacionID.Contains(item.OrganizacionID)).ToList();

            dataWrapperOrganizacion.ListaPersonaVisibleEnOrg = mEntityContext.PersonaVisibleEnOrg.Where(item => listaOrganizacionID.Contains(item.OrganizacionID)).ToList();


            //OrganizacionDS organizacionDS = new OrganizacionDS();

            //string select = " WHERE OrganizacionID IN ( " +
            //    " SELECT Perfil.OrganizacionID FROM Perfil INNER JOIN Identidad ON Identidad.PerfilID = Perfil.PerfilID WHERE IdentidadID = " + IBD.GuidParamValor("IdentidadID") +
            //    " AND Perfil.OrganizacionID is not null )";

            //DbCommand commandsqlSelectOrganizacionesPorIdentidad = ObtenerComando(SelectPesadoOrganizacion + select);

            //AgregarParametro(commandsqlSelectOrganizacionesPorIdentidad, IBD.ToParam("IdentidadID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pIdentidad));

            //CargarDataSet(commandsqlSelectOrganizacionesPorIdentidad, organizacionDS, "Organizacion");

            ////PersonaVinculoOrganizacion
            //commandsqlSelectOrganizacionesPorIdentidad.CommandText = SelectLigeroPersonaVinculoOrganizacion + select;
            //CargarDataSet(commandsqlSelectOrganizacionesPorIdentidad, organizacionDS, "PersonaVinculoOrganizacion");

            ////OrganizacionEmpresa
            //DbCommand commandsqlSelectOrganizacionesEmpresaPorIdentidad = ObtenerComando(SelectOrganizacionEmpresa + select);
            //AgregarParametro(commandsqlSelectOrganizacionesEmpresaPorIdentidad, IBD.ToParam("IdentidadID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pIdentidad));            

            //CargarDataSet(commandsqlSelectOrganizacionesEmpresaPorIdentidad, organizacionDS, "OrganizacionEmpresa");

            ////OrganizacionClase
            //DbCommand commandsqlSelectOrganizacionesClasePorIdentidad = ObtenerComando(SelectOrganizacionClase + select);
            //AgregarParametro(commandsqlSelectOrganizacionesClasePorIdentidad, IBD.ToParam("IdentidadID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pIdentidad));            

            //CargarDataSet(commandsqlSelectOrganizacionesClasePorIdentidad, organizacionDS, "OrganizacionClase");

            ////ConfiguracionGnossOrg
            //commandsqlSelectOrganizacionesPorIdentidad.CommandText = SelectConfiguracionGnossOrg + " FROM ConfiguracionGnossOrg " + select;

            //CargarDataSet(commandsqlSelectOrganizacionesPorIdentidad, organizacionDS, "ConfiguracionGnossOrg");

            ////PersonaVisibleEnOrg
            //commandsqlSelectOrganizacionesPorIdentidad.CommandText = SelectPersonaVisibleEnOrg + select;
            //CargarDataSet(commandsqlSelectOrganizacionesPorIdentidad, organizacionDS, "PersonaVisibleEnOrg");

            //return (organizacionDS);
            return dataWrapperOrganizacion;
        }

        /// <summary>
        /// Obtiene el id autonumérico que se le asigna a cada organización para crear la tabla BASE
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <returns></returns>
        public int ObtenerTablaBaseOrganizacionIDOrganizacionPorID(Guid pOrganizacionID)
        {
            //TODO: hecho 
            if (pOrganizacionID != Guid.Empty)
            {
                return mEntityContext.Organizacion.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).Select(item => item.TablaBaseOrganizacionID).FirstOrDefault();
            }
            else
            {
                return -1;
            }

            //int resultado = -1;

            //if (pOrganizacionID != Guid.Empty)
            //{
            //    DbCommand selectTablaBaseOrganizacionID = ObtenerComando(this.sqlSelectTablaBaseOrganizacionIDDeOrganizacionPorID = "SELECT TablaBaseOrganizacionID FROM Organizacion WHERE OrganizacionID = " + IBD.GuidParamValor("organizacionID"));
            //    AgregarParametro(selectTablaBaseOrganizacionID, IBD.ToParam("organizacionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pOrganizacionID));

            //    object id = EjecutarEscalar(selectTablaBaseOrganizacionID);

            //    if ((id is int) && (((int)id) > 0))
            //    {
            //        resultado = (int)id;
            //    }
            //}
            //return resultado;
        }

        /// <summary>
        /// Obtiene el ID de una organizacion a partir de su id de tesauro (null si el usuario no existe)
        /// </summary>
        /// <param name="pPerfilID">Id del perfil</param>
        /// <returns></returns>
        public Guid? ObtenerOrganizacionIDPorIDTesauro(Guid pTesauroID)
        {
            //TODO: hecho
            return mEntityContext.TesauroOrganizacion.JoinCategoriaTesauro().Where(item => item.CategoriaTesauro.CategoriaTesauroID.Equals(pTesauroID)).Select(item => item.TesauroOrganizacion.OrganizacionID).FirstOrDefault();

            //DbCommand commandsqlSelectOrganizacionIDPorTesauroID = ObtenerComando(selectOrganizacionIDPorTesauroID = "SELECT TesauroOrganizacion.OrganizacionID from TesauroOrganizacion inner join CategoriaTesauro on TesauroOrganizacion.TesauroID=CategoriaTesauro.TesauroID WHERE CategoriaTesauro.CategoriaTesauroID = " + IBD.ToParam("TesauroID"));
            //AgregarParametro(commandsqlSelectOrganizacionIDPorTesauroID, IBD.ToParam("TesauroID"), DbType.Guid, pTesauroID);

            //try
            //{
            //    object id = EjecutarEscalar(commandsqlSelectOrganizacionIDPorTesauroID);

            //    if ((id != null) && (id is Guid) && (!id.Equals(Guid.Empty)))
            //    {
            //        return (Guid)id;
            //    }
            //}
            //catch (Exception e) 
            //{
            //    Error.GuardarLogError(e);
            //}

            //return null;
        }


        /// <summary>
        /// Obtiene una organización a partir de su identificador
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionPorID(Guid pOrganizacionID)
        {
            //TODO: hecho
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            dataWrapperOrganizacion.ListaOrganizacion = mEntityContext.Organizacion.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).ToList();

            //OrganizacionDS organizacionDS = new OrganizacionDS();

            ////Organizacion
            //DbCommand commandsqlSelectOrganizacionPorID = ObtenerComando(sqlSelectOrganizacionPorID);
            //AgregarParametro(commandsqlSelectOrganizacionPorID, IBD.ToParam("organizacionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pOrganizacionID));
            //CargarDataSet(commandsqlSelectOrganizacionPorID, organizacionDS, "Organizacion");

            dataWrapperOrganizacion.ListaOrganizacionParticipaProy = mEntityContext.OrganizacionParticipaProy.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).ToList();

            ////OrganizacionParticipaProy
            //DbCommand commandsqlSelectObtenerOrganizacionParticipaProy = ObtenerComando(sqlSelectObtenerOrganizacionParticipaProyDeProyecto);
            //AgregarParametro(commandsqlSelectObtenerOrganizacionParticipaProy, IBD.ToParam("organizacionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pOrganizacionID));
            //CargarDataSet(commandsqlSelectObtenerOrganizacionParticipaProy, organizacionDS, "OrganizacionParticipaProy");

            dataWrapperOrganizacion.ListaAdministradorOrganizacion = mEntityContext.AdministradorOrganizacion.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).ToList();

            ////AdministradorOrganizacion
            //DbCommand commandsqlsqlSelectTodosOrgGnossAdministradorOrganizacionPorID = ObtenerComando(sqlSelectTodosOrgGnossAdministradorOrganizacionPorID);
            //AgregarParametro(commandsqlsqlSelectTodosOrgGnossAdministradorOrganizacionPorID, IBD.ToParam("organizacionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pOrganizacionID));
            //CargarDataSet(commandsqlsqlSelectTodosOrgGnossAdministradorOrganizacionPorID, organizacionDS, "AdministradorOrganizacion");

            dataWrapperOrganizacion.ListaPersonaVinculoOrganizacion = mEntityContext.PersonaVinculoOrganizacion.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).ToList();

            ////PersonaVinculoOrganizacion
            //DbCommand commandsqlsqlSelectPersonaVinculoOrgPorID = ObtenerComando(sqlSelectPesadoPersonaVinculoOrgPorID);
            //AgregarParametro(commandsqlsqlSelectPersonaVinculoOrgPorID, IBD.ToParam("organizacionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pOrganizacionID));
            //CargarDataSet(commandsqlsqlSelectPersonaVinculoOrgPorID, organizacionDS, "PersonaVinculoOrganizacion");

            ////OrganizacionEmpresa
            //DbCommand commandsqlSelectOrganizacionEmpresaPorID = ObtenerComando(sqlSelectOrganizacionEmpresaPorID);
            //AgregarParametro(commandsqlSelectOrganizacionEmpresaPorID, IBD.ToParam("organizacionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pOrganizacionID));
            //CargarDataSet(commandsqlSelectOrganizacionEmpresaPorID, organizacionDS, "OrganizacionEmpresa");

            ////OrganizacionClase
            //DbCommand commandsqlSelectOrganizacionClasePorID = ObtenerComando(sqlSelectOrganizacionClasePorID);
            //AgregarParametro(commandsqlSelectOrganizacionClasePorID, IBD.ToParam("organizacionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pOrganizacionID));
            //CargarDataSet(commandsqlSelectOrganizacionClasePorID, organizacionDS, "OrganizacionClase");

            dataWrapperOrganizacion.ListaOrganizacionGnoss = mEntityContext.OrganizacionGnoss.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).ToList();

            ////OrganizacionGnoss
            //DbCommand commandcommandsqlSelectOrganizacionPorID = ObtenerComando(sqlSelectOrgGnossPorID);
            //AgregarParametro(commandcommandsqlSelectOrganizacionPorID, IBD.ToParam("organizacionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pOrganizacionID));
            //CargarDataSet(commandcommandsqlSelectOrganizacionPorID, organizacionDS, "OrganizacionGnoss");

            ////CnaeAgregacionOrganizacion
            //DbCommand commandssqlSelectTodasCnaeAgregacionOrganizacionOrgGnossPorID = ObtenerComando(sqlSelectTodasCnaeAgregacionOrganizacionOrgGnossPorID);
            //AgregarParametro(commandssqlSelectTodasCnaeAgregacionOrganizacionOrgGnossPorID, IBD.ToParam("organizacionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pOrganizacionID));
            //CargarDataSet(commandssqlSelectTodasCnaeAgregacionOrganizacionOrgGnossPorID, organizacionDS, "CnaeAgregacionOrganizacion");

            dataWrapperOrganizacion.ListaConfiguracionGnossOrg = mEntityContext.ConfiguracionGnossOrg.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).ToList();

            ////ConfiguracionGnossOrg
            //DbCommand commandsqlSelectOrganizacionesPorID = ObtenerComando(SelectConfiguracionGnossOrg + " FROM ConfiguracionGnossOrg Where OrganizacionID =" + IBD.GuidParamValor("organizacionID"));
            //AgregarParametro(commandsqlSelectOrganizacionesPorID, IBD.ToParam("organizacionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pOrganizacionID));
            //CargarDataSet(commandsqlSelectOrganizacionesPorID, organizacionDS, "ConfiguracionGnossOrg");

            dataWrapperOrganizacion.ListaPersonaVisibleEnOrg = mEntityContext.PersonaVisibleEnOrg.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).ToList();

            ////PersonaVisibleEnOrg
            //DbCommand commandsqlSelectPersonaVisibleEnOrgPorID = ObtenerComando(SelectPersonaVisibleEnOrg + " WHERE OrganizacionID =" + IBD.GuidParamValor("organizacionID"));
            //AgregarParametro(commandsqlSelectPersonaVisibleEnOrgPorID, IBD.ToParam("organizacionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pOrganizacionID));
            //CargarDataSet(commandsqlSelectPersonaVisibleEnOrgPorID, organizacionDS, "PersonaVisibleEnOrg");

            //return organizacionDS;
            return dataWrapperOrganizacion;

        }

        /// <summary>
        /// Obtiene una Organización a partir de su Identificador CARGA LIGERA. "Organizacion"
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionPorIDCargaLigera(Guid pOrganizacionID)
        {
            //TODO: hecho
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            dataWrapperOrganizacion.ListaOrganizacion = mEntityContext.Organizacion.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).ToList();

            //OrganizacionDS organizacionDS = new OrganizacionDS();

            ////Organizacion
            //DbCommand commandsqlSelectOrganizacionPorID = ObtenerComando(sqlSelectOrganizacionPorID);
            //AgregarParametro(commandsqlSelectOrganizacionPorID, IBD.ToParam("organizacionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pOrganizacionID));
            //CargarDataSet(commandsqlSelectOrganizacionPorID, organizacionDS, "Organizacion");

            ////OrganizacionEmpresa
            //DbCommand commandsqlSelectOrganizacionEmpresaPorID = ObtenerComando(sqlSelectOrganizacionEmpresaPorID);
            //AgregarParametro(commandsqlSelectOrganizacionEmpresaPorID, IBD.ToParam("organizacionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pOrganizacionID));
            //CargarDataSet(commandsqlSelectOrganizacionEmpresaPorID, organizacionDS, "OrganizacionEmpresa");

            ////OrganizacionClase
            //DbCommand commandsqlSelectOrganizacionClasePorID = ObtenerComando(sqlSelectOrganizacionClasePorID);
            //AgregarParametro(commandsqlSelectOrganizacionClasePorID, IBD.ToParam("organizacionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pOrganizacionID));
            //CargarDataSet(commandsqlSelectOrganizacionClasePorID, organizacionDS, "OrganizacionClase");

            //return organizacionDS;
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
            //TODO: hecho
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            dataWrapperOrganizacion.ListaOrganizacion = mEntityContext.Organizacion.JoinPerfil().JoinIdentidad().Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID)).Select(item => item.Organizacion).ToList();

            return dataWrapperOrganizacion;

            //OrganizacionDS organizacionDS = new OrganizacionDS();

            //DbCommand commandsqlSelectOrganizacionPorID = ObtenerComando(SelectLigeroOrganizacion + " INNER JOIN Perfil ON (Organizacion.OrganizacionID = Perfil.OrganizacionID) INNER JOIN Identidad ON Identidad.PerfilID = Perfil.PerfilID WHERE Identidad.IdentidadID = " + IBD.GuidValor(pIdentidadID));
            //CargarDataSet(commandsqlSelectOrganizacionPorID, organizacionDS, "Organizacion");

            //return (organizacionDS);
        }

        /// <summary>
        /// Obtiene los nombres de las organizaciones a partir de sus identificadores
        /// </summary>
        /// <param name="pOrganizacionesIDs">Lista de identificadores de organizaciones</param>
        /// <returns>Nombres de las organizaciones</returns>
        public Dictionary<Guid, KeyValuePair<string, string>> ObtenerNombreOrganizacionesPorIDs(List<Guid> pOrganizacionesIDs)
        {
            //TODO: hecho
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

            //    Dictionary<Guid, KeyValuePair<string, string>> listaNombres = new Dictionary<Guid, KeyValuePair<string, string>>();

            //if (pOrganizacionesIDs.Count > 0)
            //{
            //    string sqlSelectNombre = SelectLigeroOrganizacion + " WHERE ";
            //    int cont = 0;
            //    Dictionary<Guid, string> listaParmetros = new Dictionary<Guid, string>();

            //    foreach (Guid organizacionID in pOrganizacionesIDs)
            //    {
            //        string parametro = "organizacionID" + cont.ToString();
            //        sqlSelectNombre += " (OrganizacionID = " + IBD.GuidParamValor(parametro) + ") OR";
            //        listaParmetros.Add(organizacionID, parametro);
            //        cont++;
            //    }
            //    sqlSelectNombre = sqlSelectNombre.Substring(0, sqlSelectNombre.Length - 2);
            //    DbCommand comandoSelectNombres = ObtenerComando(sqlSelectNombre);

            //    foreach (Guid organizacionID in listaParmetros.Keys)
            //    {
            //        AgregarParametro(comandoSelectNombres, listaParmetros[organizacionID], IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(organizacionID));
            //    }
            //    OrganizacionDS organizacionDS = new OrganizacionDS();
            //    CargarDataSet(comandoSelectNombres, organizacionDS, "organizacion");

            //    foreach (OrganizacionDS.OrganizacionRow filaOrg in organizacionDS.Organizacion)
            //    {
            //        listaNombres.Add(filaOrg.OrganizacionID, new KeyValuePair<string, string>(filaOrg.Nombre, filaOrg.NombreCorto));
            //    }
            //    organizacionDS.Dispose();
            //}
            //return listaNombres;
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
            //TODO: hecho
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            var resultado = mEntityContext.Organizacion.JoinPerfilPersonaOrg().JoinIdentidad().Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID)).Select(item => item.Organizacion.OrganizacionID)
                 .Union(mEntityContext.Organizacion.JoinPerfilOrganizacion().JoinIdentidad().Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID)).Select(item => item.Organizacion.OrganizacionID));

            dataWrapperOrganizacion.ListaOrganizacion = mEntityContext.Organizacion.Join(resultado, org => org.OrganizacionID, res => res, (org, res) =>
             new
             {
                 Organizacion = org,
                 resultado = res
             }).OrderBy(item => item.Organizacion.Nombre).Select(item => item.Organizacion).ToList();

            //OrganizacionDS organizacionDS = new OrganizacionDS();

            ////Organizacion
            //DbCommand commandsqlSelectOrganizacionesDeIdentidad = ObtenerComando(this.sqlSelectOrganizacionesDeIdentidad = SelectPesadoOrganizacion + " INNER JOIN 
            //(SELECT Organizacion.OrganizacionID FROM Organizacion 
            //INNER JOIN PerfilPersonaOrg ON PerfilPersonaOrg.OrganizacionID = Organizacion.OrganizacionID 
            //INNER JOIN Identidad ON Identidad.PerfilID = PerfilPersonaOrg.PerfilID 
            //WHERE (Identidad.IdentidadID = " + IBD.GuidParamValor("identidadID") + ")" + " 
            //UNION SELECT Organizacion.OrganizacionID FROM Organizacion 
            //INNER JOIN PerfilOrganizacion ON PerfilOrganizacion.OrganizacionID = Organizacion.OrganizacionID 
            //INNER JOIN Identidad ON Identidad.PerfilID = PerfilOrganizacion.PerfilID 
            //WHERE (Identidad.IdentidadID = " + IBD.GuidParamValor("identidadID") + ") ) 
            //Resultado ON Resultado.OrganizacionID = Organizacion.OrganizacionID ORDER BY Organizacion.Nombre"
            //AgregarParametro(commandsqlSelectOrganizacionesDeIdentidad, IBD.ToParam("identidadID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pIdentidadID));
            //CargarDataSet(commandsqlSelectOrganizacionesDeIdentidad, organizacionDS, "Organizacion");

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

            ////PersonaVinculoOrganizacion
            //string sqlPersonaVincOrg = this.sqlSelectPersonaVinculoOrganizacionDeIdentidad = SelectPesadoPersonaVinculoOrganizacion + " INNER JOIN 
            //(SELECT Organizacion.OrganizacionID FROM Organizacion 
            //INNER JOIN PerfilPersonaOrg ON PerfilPersonaOrg.OrganizacionID = Organizacion.OrganizacionID 
            //INNER JOIN Identidad ON Identidad.PerfilID = PerfilPersonaOrg.PerfilID 
            //WHERE (Identidad.IdentidadID = " + IBD.GuidParamValor("identidadID") + ")" + " 
            //UNION SELECT Organizacion.OrganizacionID FROM Organizacion 
            //INNER JOIN PerfilOrganizacion ON PerfilOrganizacion.OrganizacionID = Organizacion.OrganizacionID 
            //INNER JOIN Identidad ON Identidad.PerfilID = PerfilOrganizacion.PerfilID 
            //WHERE (Identidad.IdentidadID = " + IBD.GuidParamValor("identidadID") + ") ) Resultado ON Resultado.OrganizacionID = 

            //if (pCargaLigera)
            //{
            //    sqlPersonaVincOrg = SelectLigeroPersonaVinculoOrganizacion + " INNER JOIN Perfil ON Perfil.PersonaID = PersonaVinculoOrganizacion.PersonaID INNER JOIN Identidad ON Identidad.PerfilID = Perfil.PerfilID WHERE (Identidad.IdentidadID = " + IBD.GuidParamValor("identidadID") + ")";
            //}
            //DbCommand commandsqlSelectPersonaVinculoOrganizacionDeIdentidad = ObtenerComando(sqlPersonaVincOrg);
            //AgregarParametro(commandsqlSelectPersonaVinculoOrganizacionDeIdentidad, IBD.ToParam("identidadID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pIdentidadID));
            //CargarDataSet(commandsqlSelectPersonaVinculoOrganizacionDeIdentidad, organizacionDS, "PersonaVinculoOrganizacion");


            ////OrganizacionEmpresa
            //DbCommand commandsqlSelectOrganizacionesEmpresaDeIdentidad = ObtenerComando(sqlSelectOrganizacionesEmpresaDeIdentidad);
            //AgregarParametro(commandsqlSelectOrganizacionesEmpresaDeIdentidad, IBD.ToParam("identidadID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pIdentidadID));
            //CargarDataSet(commandsqlSelectOrganizacionesEmpresaDeIdentidad, organizacionDS, "OrganizacionEmpresa");

            ////OrganizacionClase
            //DbCommand commandsqlSelectOrganizacionesClaseDeIdentidad = ObtenerComando(sqlSelectOrganizacionesClaseDeIdentidad);
            //AgregarParametro(commandsqlSelectOrganizacionesClaseDeIdentidad, IBD.ToParam("identidadID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pIdentidadID));
            //CargarDataSet(commandsqlSelectOrganizacionesClaseDeIdentidad, organizacionDS, "OrganizacionClase");

            //return (organizacionDS);
            return dataWrapperOrganizacion;
        }

        /// <summary>
        /// Obtiene la organización a partir de su perfil
        /// </summary>
        /// <param name="pIdentidadID">Identificador de identidad</param>
        /// <returns>Dataset de organización</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionPorPerfil(Guid pPerfil)
        {
            //TODO: hecho
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            dataWrapperOrganizacion.ListaOrganizacion = mEntityContext.Organizacion.JoinPerfil().Where(item => item.Perfil.PerfilID.Equals(pPerfil)).Select(item => item.Organizacion).ToList().Distinct().ToList();

            //OrganizacionDS organizacionDS = new OrganizacionDS();

            ////Organizacion
            //string consulta = SelectLigeroOrganizacion.Replace("SELECT", "SELECT DISTINCT ") + " INNER JOIN Perfil ON Perfil.OrganizacionID = Organizacion.OrganizacionID WHERE Perfil.PerfilID = " + IBD.GuidValor(pPerfil);

            //DbCommand commandsqlSelectOrganizacionesDePerfil = ObtenerComando(consulta);
            //CargarDataSet(commandsqlSelectOrganizacionesDePerfil, organizacionDS, "Organizacion");

            ////OrganizacionEmpresa
            //consulta = SelectOrganizacionEmpresa.Replace("SELECT", "SELECT DISTINCT ") + " INNER JOIN Perfil ON Perfil.OrganizacionID = OrganizacionEmpresa.OrganizacionID WHERE Perfil.PerfilID = " + IBD.GuidValor(pPerfil);

            //DbCommand commandsqlSelectOrganizacionesEmpresaDePerfil = ObtenerComando(consulta);
            //CargarDataSet(commandsqlSelectOrganizacionesEmpresaDePerfil, organizacionDS, "OrganizacionEmpresa");

            ////OrganizacionClase
            //consulta = SelectOrganizacionClase.Replace("SELECT", "SELECT DISTINCT ") + " INNER JOIN Perfil ON Perfil.OrganizacionID = OrganizacionClase.OrganizacionID WHERE Perfil.PerfilID = " + IBD.GuidValor(pPerfil);

            //DbCommand commandsqlSelectOrganizacionesClaseDePerfil = ObtenerComando(consulta);
            //CargarDataSet(commandsqlSelectOrganizacionesClaseDePerfil, organizacionDS, "OrganizacionClase");

            //return organizacionDS;

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
            //TODO: hecho
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            dataWrapperOrganizacion.ListaOrganizacion = mEntityContext.Organizacion.JoinPerfilOrganizacion().JoinIdentidad().Where(item => pListaIdentidades.Contains(item.Identidad.IdentidadID)).Select(item => item.Organizacion).ToList().Distinct().ToList();

            //OrganizacionDS organizacionDS = new OrganizacionDS();

            //if (pListaIdentidades.Count == 0)
            //{
            //    return organizacionDS;
            //}
            ////Organizacion
            //string consulta = SelectLigeroOrganizacion.Replace("SELECT", "SELECT DISTINCT ") + " INNER JOIN PerfilOrganizacion ON PerfilOrganizacion.OrganizacionID = Organizacion.OrganizacionID INNER JOIN Identidad ON Identidad.PerfilID = PerfilOrganizacion.PerfilID WHERE Identidad.IdentidadID IN (";

            //for (int i = 0; i < pListaIdentidades.Count; i++)
            //{
            //    consulta += IBD.GuidValor(pListaIdentidades[i]) + ", ";
            //}
            //consulta = consulta.Substring(0, consulta.Length - 2) + ")";

            //DbCommand commandsqlSelectOrganizacionesDeIdentidad = ObtenerComando(consulta);
            //CargarDataSet(commandsqlSelectOrganizacionesDeIdentidad, organizacionDS, "Organizacion");

            ////OrganizacionEmpresa
            //consulta = SelectOrganizacionEmpresa.Replace("SELECT", "SELECT DISTINCT ") + " INNER JOIN PerfilOrganizacion ON PerfilOrganizacion.OrganizacionID = OrganizacionEmpresa.OrganizacionID INNER JOIN Identidad ON Identidad.PerfilID = PerfilOrganizacion.PerfilID WHERE Identidad.IdentidadID IN (";

            //for (int i = 0; i < pListaIdentidades.Count; i++)
            //{
            //    consulta += IBD.GuidValor(pListaIdentidades[i]) + ", ";
            //}
            //consulta = consulta.Substring(0, consulta.Length - 2) + ")";

            //DbCommand commandsqlSelectOrganizacionesEmpresaDeIdentidad = ObtenerComando(consulta);
            //CargarDataSet(commandsqlSelectOrganizacionesEmpresaDeIdentidad, organizacionDS, "OrganizacionEmpresa");

            ////OrganizacionClase
            //consulta = SelectOrganizacionClase.Replace("SELECT", "SELECT DISTINCT ") + " INNER JOIN PerfilOrganizacion ON PerfilOrganizacion.OrganizacionID = OrganizacionClase.OrganizacionID INNER JOIN Identidad ON Identidad.PerfilID = PerfilOrganizacion.PerfilID WHERE Identidad.IdentidadID IN (";

            //for (int i = 0; i < pListaIdentidades.Count; i++)
            //{
            //    consulta += IBD.GuidValor(pListaIdentidades[i]) + ", ";
            //}
            //consulta = consulta.Substring(0, consulta.Length - 2) + ")";

            //DbCommand commandsqlSelectOrganizacionesClaseDeIdentidad = ObtenerComando(consulta);
            //CargarDataSet(commandsqlSelectOrganizacionesClaseDeIdentidad, organizacionDS, "OrganizacionClase");

            //return organizacionDS;
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
            //TODO: hecho
            DataWrapperOrganizacion dataWrapperOrganzacion = new DataWrapperOrganizacion();

            dataWrapperOrganzacion.ListaOrganizacion = mEntityContext.Organizacion.JoinPersonaVinculoOrganizacion().JoinPersona().Where(item => item.Persona.UsuarioID.Value.Equals(pUsuarioID)).OrderBy(item => item.Organizacion.Nombre).Select(item => item.Organizacion).ToList();

            //OrganizacionDS organizacionDS = new OrganizacionDS();

            ////Organizacion
            //DbCommand commandsqlSelectOrganizacionesParticipaUsuario = ObtenerComando(this.sqlSelectOrganizacionesParticipaUsuario = SelectPesadoOrganizacion + " INNER JOIN PersonaVinculoOrganizacion ON PersonaVinculoOrganizacion.OrganizacionID = Organizacion.OrganizacionID INNER JOIN Persona ON PersonaVinculoOrganizacion.PersonaID = Persona.PersonaID WHERE (Persona.UsuarioID = " + IBD.GuidParamValor("usuarioID") + ") ORDER BY Organizacion.Nombre");
            //AgregarParametro(commandsqlSelectOrganizacionesParticipaUsuario, IBD.ToParam("usuarioID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pUsuarioID));

            //CargarDataSet(commandsqlSelectOrganizacionesParticipaUsuario, organizacionDS, "Organizacion");

            ////OrganizacionEmpresa
            //DbCommand commandsqlSelectOrganizacionesEmpresaParticipaUsuario = ObtenerComando(sqlSelectOrganizacionesEmpresaParticipaUsuario);
            //AgregarParametro(commandsqlSelectOrganizacionesEmpresaParticipaUsuario, IBD.ToParam("usuarioID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pUsuarioID));

            //CargarDataSet(commandsqlSelectOrganizacionesEmpresaParticipaUsuario, organizacionDS, "OrganizacionEmpresa");

            ////OrganizacionClase
            //DbCommand commandsqlSelectOrganizacionesClaseParticipaUsuario = ObtenerComando(sqlSelectOrganizacionesClaseParticipaUsuario);
            //AgregarParametro(commandsqlSelectOrganizacionesClaseParticipaUsuario, IBD.ToParam("usuarioID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pUsuarioID));

            //CargarDataSet(commandsqlSelectOrganizacionesClaseParticipaUsuario, organizacionDS, "OrganizacionClase");

            //return (organizacionDS);
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
            //TODO: hecho 

            return mEntityContext.PersonaVisibleEnOrg.Any(item => item.PersonaID.Equals(pPersonaID) && item.OrganizacionID.Equals(pOrganizacionID));

            //string select = "SELECT 1 FROM PersonaVisibleEnOrg WHERE PersonaID = " + IBD.GuidValor(pPersonaID) + " AND OrganizacionID = " + IBD.GuidValor(pOrganizacionID);

            //DbCommand commandsqlSelect = ObtenerComando(select);

            //return EjecutarEscalar(commandsqlSelect) != null;
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
            //TODO: hecho
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            dataWrapperOrganizacion.ListaOrganizacion = mEntityContext.Organizacion.JoinSolicitudOrganizacion().JoinSolicitud().Where(item => item.Solicitud.OrganizacionID.Equals(pOrganizacionID) && item.Solicitud.ProyectoID.Equals(pProyectoID) && item.Solicitud.Estado.Equals((short)EstadoSolicitud.Espera)).Select(item => item.Organizacion).ToList();

            //OrganizacionDS organizacionDS = new OrganizacionDS();

            ////Organizacion
            //DbCommand commandsqlSelectOrganizacionesSolicitanAccesoProyecto = ObtenerComando(this.sqlSelectObtenerOrganizacionesSolicitanAccesoAProyecto = SelectLigeroOrganizacion + " 
            //INNER JOIN SolicitudOrganizacion ON Organizacion.OrganizacionID = SolicitudOrganizacion.OrganizacionID 
            //INNER JOIN Solicitud ON Solicitud.SolicitudID = SolicitudOrganizacion.SolicitudID 
            //WHERE Solicitud.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + " 
            //AND Solicitud.ProyectoID = " + IBD.GuidParamValor("proyectoID") + " 
            //AND Solicitud.Estado = " + ((short)EstadoSolicitud.Espera).ToString());
            //AgregarParametro(commandsqlSelectOrganizacionesSolicitanAccesoProyecto, IBD.ToParam("organizacionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pOrganizacionID));
            //AgregarParametro(commandsqlSelectOrganizacionesSolicitanAccesoProyecto, IBD.ToParam("proyectoID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pProyectoID));

            //CargarDataSet(commandsqlSelectOrganizacionesSolicitanAccesoProyecto, organizacionDS, "Organizacion");

            ////OrganizacionEmpresa
            //DbCommand commandsqlSelectOrganizacionesEmpresaSolicitanAccesoProyecto = ObtenerComando(sqlSelectObtenerOrganizacionesEmpresaSolicitanAccesoAProyecto);
            //AgregarParametro(commandsqlSelectOrganizacionesEmpresaSolicitanAccesoProyecto, IBD.ToParam("organizacionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pOrganizacionID));
            //AgregarParametro(commandsqlSelectOrganizacionesEmpresaSolicitanAccesoProyecto, IBD.ToParam("proyectoID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pProyectoID));

            //CargarDataSet(commandsqlSelectOrganizacionesEmpresaSolicitanAccesoProyecto, organizacionDS, "OrganizacionEmpresa");

            ////OrganizacionClase
            //DbCommand commandsqlSelectOrganizacionesClaseSolicitanAccesoProyecto = ObtenerComando(sqlSelectObtenerOrganizacionesClaseSolicitanAccesoAProyecto);
            //AgregarParametro(commandsqlSelectOrganizacionesClaseSolicitanAccesoProyecto, IBD.ToParam("organizacionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pOrganizacionID));
            //AgregarParametro(commandsqlSelectOrganizacionesClaseSolicitanAccesoProyecto, IBD.ToParam("proyectoID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pProyectoID));

            //CargarDataSet(commandsqlSelectOrganizacionesClaseSolicitanAccesoProyecto, organizacionDS, "OrganizacionClase");

            //return (organizacionDS);
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
            //TODO: hecho
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            dataWrapperOrganizacion.ListaOrganizacion = mEntityContext.Organizacion.JoinProyectoRolUsuario().Where(item => item.ProyectoRolUsuario.UsuarioID.Equals(pUsuarioID)).Select(item => item.Organizacion).ToList().Distinct().ToList();

            //OrganizacionDS organizacionDS = new OrganizacionDS();

            ////Organizacion
            //DbCommand commandsqlSelectOrganizacionesAccedeUsuario = ObtenerComando(this.sqlSelectOrganizacionesGnossAccedeUsuario = 
            //SelectLigeroOrganizacion.Replace("SELECT", "SELECT DISTINCT ") + " 
            //INNER JOIN ProyectoRolUsuario ON Organizacion.OrganizacionID = ProyectoRolUsuario.OrganizacionGnossID 
            //WHERE (ProyectoRolUsuario.UsuarioID = " + IBD.GuidParamValor("usuarioID") + ")");

            //AgregarParametro(commandsqlSelectOrganizacionesAccedeUsuario, IBD.ToParam("usuarioID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pUsuarioID));

            //CargarDataSet(commandsqlSelectOrganizacionesAccedeUsuario, organizacionDS, "Organizacion");

            ////OrganizacionEmpresa
            //DbCommand commandsqlSelectOrganizacionesEmpresaAccedeUsuario = ObtenerComando(sqlSelectOrganizacionesEmpresaAccedeUsuario);
            //AgregarParametro(commandsqlSelectOrganizacionesEmpresaAccedeUsuario, IBD.ToParam("usuarioID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pUsuarioID));

            //CargarDataSet(commandsqlSelectOrganizacionesEmpresaAccedeUsuario, organizacionDS, "OrganizacionEmpresa");

            ////OrganizacionClase
            //DbCommand commandsqlSelectOrganizacionesClaseAccedeUsuario = ObtenerComando(sqlSelectOrganizacionesClaseAccedeUsuario);
            //AgregarParametro(commandsqlSelectOrganizacionesClaseAccedeUsuario, IBD.ToParam("usuarioID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pUsuarioID));

            //CargarDataSet(commandsqlSelectOrganizacionesClaseAccedeUsuario, organizacionDS, "OrganizacionClase");

            //return (organizacionDS);
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
            //TODO: hecho
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            dataWrapperOrganizacion.ListaOrganizacion = mEntityContext.Organizacion.JoinPersonaVinculoOrganizacion().Where(item => item.PersonaVinculoOrganizacion.PersonaID.Equals(pPersonaID) && item.PersonaVinculoOrganizacion.OrganizacionID.Equals(pOrganizacionID)).Select(item => item.Organizacion).ToList();

            //OrganizacionDS dataset = new OrganizacionDS();

            ////Organizacion
            //DbCommand commandsqlSelectOrganizaciones = ObtenerComando(this.sqlSelectOrganizacionDePersYOrg = SelectPesadoOrganizacion + " INNER JOIN PersonaVinculoOrganizacion ON Organizacion.OrganizacionID = PersonaVinculoOrganizacion.OrganizacionID WHERE (PersonaVinculoOrganizacion.PersonaID = " + IBD.GuidParamValor("personaID") + ") AND (PersonaVinculoOrganizacion.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + ")");
            //AgregarParametro(commandsqlSelectOrganizaciones, IBD.ToParam("personaID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pPersonaID));
            //AgregarParametro(commandsqlSelectOrganizaciones, IBD.ToParam("organizacionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pOrganizacionID));
            //CargarDataSet(commandsqlSelectOrganizaciones, dataset, "Organizacion");

            dataWrapperOrganizacion.ListaPersonaVinculoOrganizacion = mEntityContext.PersonaVinculoOrganizacion.Where(item => item.PersonaID.Equals(pPersonaID) && item.OrganizacionID.Equals(pOrganizacionID)).ToList();

            ////PersonaVinculoOrganizacion
            //DbCommand commandsqlSelectOrganizacionesVinculadasAPersona = ObtenerComando(this.sqlSelectPersonaVinculoOrganizacionDePersYOrg = SelectPesadoPersonaVinculoOrganizacion + " WHERE (PersonaVinculoOrganizacion.PersonaID = " + IBD.GuidParamValor("personaID") + ") AND (PersonaVinculoOrganizacion.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + ")");
            //AgregarParametro(commandsqlSelectOrganizacionesVinculadasAPersona, IBD.ToParam("personaID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pPersonaID));
            //AgregarParametro(commandsqlSelectOrganizacionesVinculadasAPersona, IBD.ToParam("organizacionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pOrganizacionID));
            //CargarDataSet(commandsqlSelectOrganizacionesVinculadasAPersona, dataset, "PersonaVinculoOrganizacion");

            ////OrganizacionEmpresa
            //DbCommand commandsqlSelectOrganizacionesEmpresas = ObtenerComando(sqlSelectOrgEmpresaDePersYOrg);
            //AgregarParametro(commandsqlSelectOrganizacionesEmpresas, IBD.ToParam("personaID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pPersonaID));
            //AgregarParametro(commandsqlSelectOrganizacionesEmpresas, IBD.ToParam("organizacionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pOrganizacionID));
            //CargarDataSet(commandsqlSelectOrganizacionesEmpresas, dataset, "OrganizacionEmpresa");

            ////OrganizacionClase
            //DbCommand commandsqlSelectOrganizacionesClase = ObtenerComando(sqlSelectOrganizacionClaseDePersYOrg);
            //AgregarParametro(commandsqlSelectOrganizacionesClase, IBD.ToParam("personaID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pPersonaID));
            //AgregarParametro(commandsqlSelectOrganizacionesClase, IBD.ToParam("organizacionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pOrganizacionID));
            //CargarDataSet(commandsqlSelectOrganizacionesClase, dataset, "OrganizacionClase");

            //return dataset;
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
            //TODO: hecho
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            dataWrapperOrganizacion.ListaConfiguracionGnossOrg = mEntityContext.ConfiguracionGnossOrg.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).ToList();

            return dataWrapperOrganizacion;

            //OrganizacionDS dataset = new OrganizacionDS();

            //DbCommand commandsqlSelectConfiguracionGnossOrg = ObtenerComando(this.SelectConfiguracionGnossOrg = "SELECT " + IBD.CargarGuid("ConfiguracionGnossOrg.OrganizacionID") + ", ConfiguracionGnossOrg.VerRecursos, ConfiguracionGnossOrg.VerRecursosExterno, ConfiguracionGnossOrg.VisibilidadContactos " + " FROM ConfiguracionGnossOrg WHERE OrganizacionID = " + IBD.ToParam("organizacionID"));
            //AgregarParametro(commandsqlSelectConfiguracionGnossOrg, IBD.ToParam("organizacionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pOrganizacionID));
            //CargarDataSet(commandsqlSelectConfiguracionGnossOrg, dataset, "ConfiguracionGnossOrg");

            //return (dataset);
        }



        /// <summary>
        /// Obtiene las tablas Organizacion, OrganizacionEmpresa, OrganizacionClase y OrganizacionParticipaProy del proyecto pasado por parámetro
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionesPorSusIdentidadesDeProyecto(Guid pProyectoID)
        {
            //TODO: hecho
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            dataWrapperOrganizacion.ListaOrganizacion = mEntityContext.Organizacion.JoinOrganizacionParticipaProy().Where(item => item.OrganizacionParticipaProy.ProyectoID.Equals(pProyectoID)).Select(item => item.Organizacion).ToList();

            //OrganizacionDS organizacionDS = new OrganizacionDS();

            ////Organizacion
            //DbCommand commandsqlSelectObtenerOrganizacionesPorSusIdentidadesDeProyecto = ObtenerComando(this.sqlSelectObtenerOrganizacionesPorSusIdentidadesDeProyecto = SelectLigeroOrganizacion.Replace("FROM Organizacion", ", TablaBaseOrganizacionID FROM Organizacion") + " INNER JOIN OrganizacionParticipaProy ON Organizacion.OrganizacionID = OrganizacionParticipaProy.OrganizacionID  WHERE (ProyectoID = " + IBD.GuidParamValor("proyectoID") + ")");
            //AgregarParametro(commandsqlSelectObtenerOrganizacionesPorSusIdentidadesDeProyecto, IBD.ToParam("proyectoID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pProyectoID));
            //CargarDataSet(commandsqlSelectObtenerOrganizacionesPorSusIdentidadesDeProyecto, organizacionDS, "Organizacion");

            dataWrapperOrganizacion.ListaOrganizacionParticipaProy = mEntityContext.OrganizacionParticipaProy.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();

            ////OrganizacionParticipaProy
            //DbCommand commandsqlSelectObtenerOrganizacionParticipaProyPorSusIdentidadesDeProyecto = ObtenerComando(this.sqlSelectObtenerOrganizacionParticipaProyPorSusIdentidadesDeProyecto = SelectOrganizacionParticipaProy + " FROM OrganizacionParticipaProy WHERE (ProyectoID = " + IBD.GuidParamValor("proyectoID") + ")");
            //AgregarParametro(commandsqlSelectObtenerOrganizacionParticipaProyPorSusIdentidadesDeProyecto, IBD.ToParam("proyectoID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pProyectoID));
            //CargarDataSet(commandsqlSelectObtenerOrganizacionParticipaProyPorSusIdentidadesDeProyecto, organizacionDS, "OrganizacionParticipaProy");

            ////OrganizacionEmpresa
            //DbCommand commandsqlSelectObtenerOrganizacionesEmpresaPorSusIdentidadesDeProyecto = ObtenerComando(sqlSelectObtenerOrganizacionesEmpresaPorSusIdentidadesDeProyecto);
            //AgregarParametro(commandsqlSelectObtenerOrganizacionesEmpresaPorSusIdentidadesDeProyecto, IBD.ToParam("proyectoID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pProyectoID));
            //CargarDataSet(commandsqlSelectObtenerOrganizacionesEmpresaPorSusIdentidadesDeProyecto, organizacionDS, "OrganizacionEmpresa");

            ////OrganizacionClase
            //DbCommand commandsqlSelectObtenerOrganizacionesClasePorSusIdentidadesDeProyecto = ObtenerComando(sqlSelectObtenerOrganizacionesClasePorSusIdentidadesDeProyecto);
            //AgregarParametro(commandsqlSelectObtenerOrganizacionesClasePorSusIdentidadesDeProyecto, IBD.ToParam("proyectoID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pProyectoID));
            //CargarDataSet(commandsqlSelectObtenerOrganizacionesClasePorSusIdentidadesDeProyecto, organizacionDS, "OrganizacionClase");

            //return organizacionDS;
            return dataWrapperOrganizacion;
        }

        /// <summary>
        /// Carga las tablas Organizacion, OrganizacionEmpresa, OrganizacionClase y AdministradorOrganizacion para la organización pasada por parámetro
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion CargarAdministradoresdeOrganizacion(Guid pOrganizacionID)
        {
            //TODO: hecho
            DataWrapperOrganizacion dataWrapeprOrganizacion = new DataWrapperOrganizacion();

            dataWrapeprOrganizacion.ListaOrganizacion = mEntityContext.Organizacion.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).ToList();

            //OrganizacionDS organizacionDS = new OrganizacionDS();

            ////Organizacion
            //DbCommand commandsqlSelectOrganizacionPorID = ObtenerComando(sqlSelectOrganizacionPorID);
            //AgregarParametro(commandsqlSelectOrganizacionPorID, IBD.ToParam("organizacionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pOrganizacionID));
            //CargarDataSet(commandsqlSelectOrganizacionPorID, organizacionDS, "Organizacion");

            dataWrapeprOrganizacion.ListaAdministradorOrganizacion = mEntityContext.AdministradorOrganizacion.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).ToList();

            ////AdministradorOrganizacion
            //DbCommand commandsqlsqlSelectTodosOrgGnossAdministradorOrganizacionPorID = ObtenerComando(sqlSelectTodosOrgGnossAdministradorOrganizacionPorID);
            //AgregarParametro(commandsqlsqlSelectTodosOrgGnossAdministradorOrganizacionPorID, IBD.ToParam("organizacionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pOrganizacionID));
            //CargarDataSet(commandsqlsqlSelectTodosOrgGnossAdministradorOrganizacionPorID, organizacionDS, "AdministradorOrganizacion");

            ////OrganizacionEmpresa
            //DbCommand commandsqlSelectOrganizacionEmpresaPorID = ObtenerComando(sqlSelectOrganizacionEmpresaPorID);
            //AgregarParametro(commandsqlSelectOrganizacionEmpresaPorID, IBD.ToParam("organizacionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pOrganizacionID));
            //CargarDataSet(commandsqlSelectOrganizacionEmpresaPorID, organizacionDS, "OrganizacionEmpresa");

            ////OrganizacionClase
            //DbCommand commandsqlSelectOrganizacionClasePorID = ObtenerComando(sqlSelectOrganizacionClasePorID);
            //AgregarParametro(commandsqlSelectOrganizacionClasePorID, IBD.ToParam("organizacionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pOrganizacionID));
            //CargarDataSet(commandsqlSelectOrganizacionClasePorID, organizacionDS, "OrganizacionClase");

            //return organizacionDS;
            return dataWrapeprOrganizacion;
        }

        /// <summary>
        /// Carga SOLO la tabla  "PersonaVinculoOrganizacion" carga ligera  para la organización pasada por parámetro
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion CargarPersonasVinculoOrgDeOrganizacion(Guid pOrganizacionID)
        {
            //TODO: hecho
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            mEntityContext.PersonaVinculoOrganizacion.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).Select(item => new { item.PersonaID, item.OrganizacionID, item.FechaVinculacion, item.Cargo, item.EmailTrabajo, item.UsarFotoPersonal });

            return dataWrapperOrganizacion;

            //OrganizacionDS organizacionDS = new OrganizacionDS();
            //organizacionDS.EnforceConstraints = false;

            ////PersonaVinculoOrganizacion
            //DbCommand commandsqlsqlSelectPersonaVinculoOrgPorID = ObtenerComando(sqlSelectLigeroPersonaVinculoOrgPorID = "SELECT " + IBD.CargarGuid("PersonaVinculoOrganizacion.PersonaID") + ", " + IBD.CargarGuid("PersonaVinculoOrganizacion.OrganizacionID") + ", FechaVinculacion, Cargo, EmailTrabajo, UsarFotoPersonal FROM PersonaVinculoOrganizacion "; " WHERE (OrganizacionID = " + IBD.GuidParamValor("organizacionID") + ")");
            //AgregarParametro(commandsqlsqlSelectPersonaVinculoOrgPorID, IBD.ToParam("organizacionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pOrganizacionID));
            //CargarDataSet(commandsqlsqlSelectPersonaVinculoOrgPorID, organizacionDS, "PersonaVinculoOrganizacion");

            //return organizacionDS;
        }

        /// <summary>
        /// Carga SOLO la tabla "PersonasVisiblesDeOrg" para la organización pasada por parámetro
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion CargarPersonasVisiblesDeOrganizacion(Guid pOrganizacionID)
        {
            //TODO: revisar
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            dataWrapperOrganizacion.ListaPersonaVisibleEnOrg = mEntityContext.PersonaVisibleEnOrg.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).OrderBy(item => item.Orden).ToList();

            return dataWrapperOrganizacion;

            //OrganizacionDS organizacionDS = new OrganizacionDS();
            //organizacionDS.EnforceConstraints = false;

            ////PersonasVisiblesDeOrg
            //DbCommand commandsPersonasVisiblesDeOrg = ObtenerComando(this.sqlSelectPersonasVisiblesDeOrg = SelectPersonaVisibleEnOrg + " WHERE OrganizacionID = " + IBD.GuidParamValor("organizacionID") + " ORDER BY PersonaVisibleEnOrg.Orden");
            //AgregarParametro(commandsPersonasVisiblesDeOrg, IBD.ToParam("organizacionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pOrganizacionID));
            //CargarDataSet(commandsPersonasVisiblesDeOrg, organizacionDS, "PersonaVisibleEnOrg");

            //return organizacionDS;
        }

        /// <summary>
        /// Indica si la persona es visible en la organizacion
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <returns>Booleano que indica si la persona es visible en la organizacion</returns>
        public bool EsPersonaVisibleEnOrganizacion(Guid pOrganizacionID, Guid pPersonaID)
        {
            //TODO: hecho
            return mEntityContext.PersonaVisibleEnOrg.Any(item => item.OrganizacionID.Equals(pOrganizacionID) && item.PersonaID.Equals(pPersonaID));

            //string sqlEsVisible = "SELECT 1 FROM PersonaVisibleEnOrg WHERE OrganizacionID = " + IBD.GuidValor(pOrganizacionID) + " AND PersonaID = " + IBD.GuidValor(pPersonaID);

            //DbCommand commandEsVisible = ObtenerComando(sqlEsVisible);

            //object resultado = EjecutarEscalar(commandEsVisible);

            //return (resultado != null);
        }

        /// <summary>
        /// Carga SOLO la tabla "PersonasVisiblesDeOrg" para las organizaciones en las que una persona es visible
        /// </summary>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion CargarOrganizacionesDePersonaVisible(Guid pPersonaID)
        {
            //TODO: hecho
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            dataWrapperOrganizacion.ListaPersonaVisibleEnOrg = mEntityContext.PersonaVisibleEnOrg.Where(item => item.PersonaID.Equals(pPersonaID)).ToList();

            return dataWrapperOrganizacion;

            //OrganizacionDS organizacionDS = new OrganizacionDS();
            //organizacionDS.EnforceConstraints = false;

            ////PersonasVisiblesDeOrg
            //DbCommand commandsPersonasVisiblesDeOrg = ObtenerComando(this.sqlSelectOrganizacionesVisiblesDePers = SelectPersonaVisibleEnOrg + " WHERE PersonaID = " + IBD.GuidParamValor("personaID"));
            //AgregarParametro(commandsPersonasVisiblesDeOrg, IBD.ToParam("personaID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pPersonaID));
            //CargarDataSet(commandsPersonasVisiblesDeOrg, organizacionDS, "PersonaVisibleEnOrg");

            //return organizacionDS;
        }

        /// <summary>
        /// Obtiene las organizaciones que administra un usuario pasado por parámetro
        /// Carga la tabla AdministradorOrganizacion
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion CargarOrganizacionesAdministraUsuario(Guid pUsuarioID)
        {
            //TODO: hecho
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            dataWrapperOrganizacion.ListaAdministradorOrganizacion = mEntityContext.AdministradorOrganizacion.Where(item => item.UsuarioID.Equals(pUsuarioID)).ToList();

            //OrganizacionDS organizacionDS = new OrganizacionDS();

            ////AdministradorOrganizacion
            //DbCommand commandsql = ObtenerComando(SelectAdministradorOrganizacion + " FROM AdministradorOrganizacion WHERE (UsuarioID = " + IBD.GuidParamValor("UsuarioID") + ")");
            //AgregarParametro(commandsql, IBD.ToParam("UsuarioID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pUsuarioID));
            //CargarDataSet(commandsql, organizacionDS, "AdministradorOrganizacion");

            //return organizacionDS;
            return dataWrapperOrganizacion;
        }

        /// <summary>
        /// Indica si el usuario ers el único administrador de una organización
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>True si es el único administrador de alguna organización</returns>
        public bool EsUsuarioAdministradorUnicoDeOrganizacion(Guid pUsuarioID)
        {
            //TODO: hecho
            return mEntityContext.AdministradorOrganizacion.Where(item2 => item2.UsuarioID.Equals(pUsuarioID) && item2.Tipo.Equals((short)TipoAdministradoresOrganizacion.Administrador)).GroupBy(item => item.OrganizacionID).Where(agrupacion => agrupacion.Count() == 1).Any();

            //return mEntityContext.AdministradorOrganizacion.GroupBy(item => item, item => item.OrganizacionID, (objeto, agrupacion) => new
            //{
            //    AdministradorOrganizacion = objeto,
            //    agrupacion = agrupacion
            //}).Where(item => item.AdministradorOrganizacion.UsuarioID.Equals(pUsuarioID) && item.AdministradorOrganizacion.Tipo.Equals((short)TipoAdministradoresOrganizacion.Administrador) && item.agrupacion.Count() == 1).Any();

            //obtenemos una tabla con las organizaciones de la q el usuario es admin y el numero de admin de las organizaciones
            //string sql = "SELECT 1 ";
            //sql+=" FROM AdministradorOrganizacion ";
            //sql += " WHERE UsuarioID = " + IBD.GuidParamValor("usuarioID") + " AND Tipo=" + (short)TipoAdministradoresOrganizacion.Administrador + " ";
            //sql += " group by OrganizacionID having count(OrganizacionID) = 1";

            //DbCommand comando = ObtenerComando(sql);
            //AgregarParametro(comando, IBD.ToParam("usuarioID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pUsuarioID));

            //object resultado = EjecutarEscalar(comando);

            //return (resultado != null);
        }


        /// <summary>
        /// Comprueba si una organización tiene en base de datos creado un Tesauro propio
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        /// <returns>TRUE si tiene tesauro, FALSE en caso contrario</returns>
        public bool TieneTesauroPropio(Guid pOrganizacionID)
        {
            //TODO: hecho
            return mEntityContext.TesauroOrganizacion.Any(item => item.OrganizacionID.Equals(pOrganizacionID));

            //Object organizacionID;

            //string sqlSelectTieneTesauroPropio = "SELECT TesauroID,OrganizacionID FROM TesauroOrganizacion WHERE (TesauroOrganizacion.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + ")";
            //DbCommand commandsqlSelectTieneTesauroPropio = ObtenerComando(sqlSelectTieneTesauroPropio);
            //AgregarParametro(commandsqlSelectTieneTesauroPropio, IBD.ToParam("organizacionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pOrganizacionID));

            //try
            //{
            //    organizacionID = EjecutarEscalar(commandsqlSelectTieneTesauroPropio);
            //}
            //finally
            //{
            //}
            //return (organizacionID != null);
        }

        /// <summary>
        /// Comprueba si una organización tiene en base de datos creado una base de recursos propia
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        /// <returns>TRUE si tiene base de recursos, FALSE en caso contrario</returns>
        public bool TieneBaseDeRecursos(Guid pOrganizacionID)
        {
            //TODO: hecho
            return mEntityContext.BaseRecursosOrganizacion.Any(item => item.OrganizacionID.Equals(pOrganizacionID));

            //Object organizacionID;

            //string sqlSelectTieneBaseDeRecursos = "SELECT BaseRecursosID,OrganizacionID FROM BaseRecursosOrganizacion WHERE (BaseRecursosOrganizacion.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + ")";
            //DbCommand commandsqlSelectTieneBaseDeRecursos = ObtenerComando(sqlSelectTieneBaseDeRecursos);
            //AgregarParametro(commandsqlSelectTieneBaseDeRecursos, IBD.ToParam("organizacionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pOrganizacionID));

            //try
            //{
            //    organizacionID = EjecutarEscalar(commandsqlSelectTieneBaseDeRecursos);
            //}
            //finally
            //{
            //}
            //return (organizacionID != null);
        }

        /// <summary>
        /// Comprueba si una organización tiene en base de datos creado un perfil de organización
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        /// <returns>TRUE si tiene perfil de organización, FALSE en caso contrario</returns>
        public bool TienePerfil(Guid pOrganizacionID)
        {
            //TODO: hecho
            return mEntityContext.PerfilOrganizacion.Any(item => item.OrganizacionID.Equals(pOrganizacionID));

            //Object organizacionID;

            //string sqlSelectTienePerfil = "SELECT PerfilID, OrganizacionID FROM PerfilOrganizacion WHERE (PerfilOrganizacion.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + ")";
            //DbCommand commandsqlSelectTienePerfil = ObtenerComando(sqlSelectTienePerfil);
            //AgregarParametro(commandsqlSelectTienePerfil, IBD.ToParam("organizacionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pOrganizacionID));

            //try
            //{
            //    organizacionID = EjecutarEscalar(commandsqlSelectTienePerfil);
            //}
            //finally
            //{
            //}
            //return (organizacionID != null);
        }


        /// <summary>
        /// Obtiene la fila de la vinculación de una persona a una organización.
        /// </summary>
        /// <param name="pOrganizacionID">ID de la org</param>
        /// <param name="pPersonaID">ID de la persona</param>
        /// <returns>Fila de la vinculación de una persona a una organización</returns>
        public string ObtenerCargoPersonaVinculoOrganizacion(Guid pOrganizacionID, Guid pPersonaID)
        {
            //TODO: hecho
            return mEntityContext.PersonaVinculoOrganizacion.Where(item => item.PersonaID.Equals(pPersonaID) && item.OrganizacionID.Equals(pOrganizacionID)).Select(item => item.Cargo).FirstOrDefault();

            //string cargo = null;
            //DataSet orgDS = new DataSet();

            //DbCommand commandsql = ObtenerComando("SELECT Cargo FROM PersonaVinculoOrganizacion WHERE PersonaID=" + IBD.GuidValor(pPersonaID) + " AND OrganizacionID=" + IBD.GuidValor(pOrganizacionID));
            //CargarDataSet(commandsql, orgDS, "PersonaVinculoOrganizacion");

            //foreach (DataRow fila in orgDS.Tables[0].Rows)
            //{
            //    if (!fila.IsNull("Cargo"))
            //    {
            //        cargo = (string)fila["Cargo"];
            //    }
            //    break;
            //}

            //orgDS.Dispose();
            //return cargo;
        }

        /// <summary>
        /// Comprueba si existe una organización con el nombre corto pasado por parámetro
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto</param>
        /// <returns>TRUE si existe, FALSE en caso contrario</returns>
        public bool ExisteNombreCortoEnBD(string pNombreCorto)
        {
            //TODO: hecho
            return mEntityContext.Organizacion.Any(item => item.NombreCorto.ToUpper().Equals(pNombreCorto.ToUpper()));

            //Object encontrado;
            //DbCommand commandsqlSelectExisteNombreCortoEnBD = ObtenerComando(this.sqlSelectExisteNombreCortoEnBD = "SELECT " + IBD.CargarGuid("OrganizacionID") + " FROM Organizacion WHERE (UPPER(NombreCorto) = UPPER(" + IBD.ToParam("nombreCorto") + "))");
            //AgregarParametro(commandsqlSelectExisteNombreCortoEnBD, IBD.ToParam("nombreCorto"), DbType.String, pNombreCorto);

            //try
            //{
            //    encontrado = EjecutarEscalar(commandsqlSelectExisteNombreCortoEnBD);
            //}
            //finally
            //{
            //}
            //return (encontrado != null);
        }

        /// <summary>
        /// Obtiene los nombres cortos que empizan con los caracteres introducidos.
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto</param>
        /// <returns>nombres cortos que empizan con los caracteres introducidos</returns>
        public List<string> ObtenerNombresCortosEmpiezanPor(string pNombreCorto)
        {
            //TODO: hecho
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            dataWrapperOrganizacion.ListaOrganizacion = mEntityContext.Organizacion.Where(item => item.NombreCorto.StartsWith(pNombreCorto)).ToList();

            List<string> listaNombres = new List<string>();

            foreach (var filaOrg in dataWrapperOrganizacion.ListaOrganizacion)
            {
                listaNombres.Add(filaOrg.NombreCorto);
            }

            return listaNombres;

            //DataSet orgDS = new DataSet();
            //string select = "SELECT Organizacion.NombreCorto FROM Organizacion Where NombreCorto like " + IBD.ToParam("NombreCorto");
            //DbCommand comandoSelect = ObtenerComando(select);
            //AgregarParametro(comandoSelect, IBD.ToParam("NombreCorto"), DbType.String, pNombreCorto + "%");
            //CargarDataSet(comandoSelect, orgDS, "Organizacion");

            //List<string> listaNombres = new List<string>();
            //foreach (DataRow filaOrg in orgDS.Tables["Organizacion"].Rows)
            //{
            //    listaNombres.Add(UtilCadenas.RemoveAccentsWithRegEx((string)filaOrg["NombreCorto"]).ToLower());
            //}
            //orgDS.Dispose();
            //return listaNombres;
        }

        /// <summary>
        /// Comprueba si el usuario pasado como parámetro es administrador de la clase indicada
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de organización de tipo clase</param>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>TRUE si el usuario administra la clase, FALSE en caso contrario</returns>
        public bool EsUsuarioAdministradorClase(Guid pOrganizacionID, Guid pUsuarioID)
        {
            //TODO: hecho
            return mEntityContext.AdministradorOrganizacion.Any(item => item.OrganizacionID.Equals(pOrganizacionID) && item.UsuarioID.Equals(pUsuarioID));

            //Object encontrado;
            //DbCommand commandsqlSelectEsUsuarioAdministradorClase = ObtenerComando(this.sqlSelectEsUsuarioAdministradorOrganizacion = SelectAdministradorOrganizacion + " FROM AdministradorOrganizacion WHERE (OrganizacionID = " + IBD.GuidParamValor("organizacionID") + ") AND (UsuarioID = " + IBD.GuidParamValor("usuarioID") + ")");
            //AgregarParametro(commandsqlSelectEsUsuarioAdministradorClase, IBD.ToParam("organizacionID"), DbType.Guid, pOrganizacionID);
            //AgregarParametro(commandsqlSelectEsUsuarioAdministradorClase, IBD.ToParam("usuarioID"), DbType.Guid, pUsuarioID);

            //try
            //{
            //    encontrado = EjecutarEscalar(commandsqlSelectEsUsuarioAdministradorClase);
            //}
            //finally
            //{
            //}
            //return (encontrado != null);
        }

        /// <summary>
        /// Comprueba si hay alguna persona visible en la organización
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <returns>TRUE si hay alguna persona visible, FALSE en caso contrario</returns>
        public bool HayPersonasVisibles(Guid pOrganizacionID)
        {
            //TODO: hecho
            return mEntityContext.PersonaVisibleEnOrg.Any(item => item.OrganizacionID.Equals(pOrganizacionID));

            //string consulta = "SELECT 1 FROM PersonaVisibleEnOrg WHERE OrganizacionID = " + IBD.GuidValor(pOrganizacionID);
            //DbCommand comando = ObtenerComando(consulta);
            //return EjecutarEscalar(comando) != null;
        }

        /// <summary>
        /// Obtiene los tags de varias organizaciones en un proyecto
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad de la organización en el proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public Dictionary<Guid, string> ObtenerTagsDeOrganizacionesEnProyecto(List<Guid> pListaIdentidadID, Guid pProyectoID)
        {
            //TODO: hecho
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


            //Dictionary<Guid, string> tags = new Dictionary<Guid, string>();

            //foreach (Guid id in pListaIdentidadID)
            //{
            //    tags.Add(id, "");
            //}

            //if (pListaIdentidadID.Count > 0)
            //{
            //    string where = "";
            //    if (pListaIdentidadID.Count == 1)
            //    {
            //        where = " = '" + IBD.ValorDeGuid(pListaIdentidadID[0]) + "'";
            //    }
            //    else
            //    {
            //        where = " IN (";
            //        string coma = "";
            //        foreach (Guid id in pListaIdentidadID)
            //        {
            //            where += coma + "'" + IBD.ValorDeGuid(id) + "'";
            //            coma = ",";
            //        }
            //        where += ")";
            //    }

            //    string select = "SELECT distinct Identidad.IdentidadID,Curriculum.Tags FROM Organizacion" 
            //        + "INNER JOIN Perfil ON Perfil.OrganizacionID = Organizacion.OrganizacionID" 
            //        + "INNER JOIN Identidad ON Perfil.PerfilID = Identidad.PerfilID "
            //        + "INNER JOIN Curriculum ON Curriculum.CurriculumID = Identidad.CurriculumID "
            //        + "WHERE Identidad.IdentidadID " + where + " AND Identidad.ProyectoID = " + IBD.ToParam("ProyectoID")  
            //        + "AND Identidad.FechaBaja IS NULL AND Identidad.FechaExpulsion IS NULL AND Perfil.Eliminado = 0 AND Organizacion.Eliminada = 0 AND Identidad.Tipo = 3";

            //    DbCommand commandsql = ObtenerComando(select);
            //    AgregarParametro(commandsql, IBD.ToParam("ProyectoID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pProyectoID));

            //    DataSet docDS = new DataSet();
            //    CargarDataSet(commandsql, docDS, "Documento");

            //    foreach (DataRow fila in docDS.Tables["Documento"].Rows)
            //    {
            //        if (!(fila["Tags"] is DBNull))
            //        {
            //            Guid idDoc = (Guid)fila["IdentidadID"];
            //            string tagss = (string)fila["Tags"];
            //            tags[idDoc] = tagss;
            //        }
            //    }

            //}
            //return tags;
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
            //TODO: hecho
            DataWrapperOrganizacion dataWrapperOrganizacion = new DataWrapperOrganizacion();

            dataWrapperOrganizacion.ListaOrganizacion = mEntityContext.Organizacion.JoinOrganizacionParticipaProy().Where(item => !item.OrganizacionParticipaProy.EstaBloqueada && item.OrganizacionParticipaProy.ProyectoID.Equals(pProyectoID)).Select(item => item.Organizacion).ToList();

            //OrganizacionDS orgDS = new OrganizacionDS();
            //orgDS.EnforceConstraints = false;

            //string select = @"SELECT Organizacion.OrganizacionID, Organizacion.Nombre, Organizacion.NombreCorto, Organizacion.Alias 
            //FROM Organizacion 
            //INNER JOIN OrganizacionParticipaProy ON Organizacion.OrganizacionID = OrganizacionParticipaProy.OrganizacionID
            //WHERE OrganizacionParticipaProy.EstaBloqueada = 0 AND OrganizacionParticipaProy.ProyectoID = " + IBD.ToParam("ProyectoID");

            //DbCommand commandsql = ObtenerComando(select);
            //AgregarParametro(commandsql, IBD.ToParam("ProyectoID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pProyectoID));
            //CargarDataSet(commandsql, orgDS, "Organizacion");

            //return orgDS;
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
            //TODO: hecho

            var query = mEntityContext.PersonaVinculoOrganizacion;

            if (!string.IsNullOrEmpty(pInicio))
            {
                return query.Where(item => item.OrganizacionID.ToString().StartsWith(pInicio)).ToList();
            }
            else
            {
                return query.ToList();
            }


            //OrganizacionDS organizacionDS = new OrganizacionDS();

            ////Identidad
            //string selectPersonaEnOrgConFoto = this.SelectLigeroPersonaVinculoOrganizacion = "SELECT " + IBD.CargarGuid("PersonaVinculoOrganizacion.PersonaID") + ", " + IBD.CargarGuid("PersonaVinculoOrganizacion.OrganizacionID") + ", FechaVinculacion, Cargo, EmailTrabajo, UsarFotoPersonal FROM PersonaVinculoOrganizacion ";
            //if (!string.IsNullOrEmpty(pInicio))
            //{
            //    selectPersonaEnOrgConFoto += "  where OrganizacionID like'" + pInicio + "%'";
            //}
            //DbCommand commandsqlPersonaEnOrgConFoto = ObtenerComando(selectPersonaEnOrgConFoto);

            //CargarDataSet(commandsqlPersonaEnOrgConFoto, organizacionDS, "PersonaVinculoOrganizacion");

            //return organizacionDS; 
        }

        /// <summary>
        /// Obtiene las filas de la tabla Organizacion cuya organizacion comienzapor pInicio
        /// </summary>
        /// <param name="pInicio">caracteres por los que comienza la organizacion</param>
        /// <returns></returns>
        public List<Organizacion> ObtenerFilasOrganizaciones(string pInicio)
        {
            //TODO: hecho 

            var query = mEntityContext.Organizacion;

            if (!string.IsNullOrEmpty(pInicio))
            {
                return query.Where(item => item.OrganizacionID.ToString().StartsWith(pInicio)).ToList();
            }
            else
            {
                return query.ToList();
            }


            //OrganizacionDS organizacionDS = new OrganizacionDS();

            ////Identidad
            //string selectOrganizacionConFoto = this.SelectLigeroOrganizacion = "SELECT " + IBD.CargarGuid("Organizacion.OrganizacionID") + ", Organizacion.Nombre, Organizacion.EsBuscable, Organizacion.EsBuscableExternos, Organizacion.ModoPersonal, Organizacion.Eliminada, Organizacion.NombreCorto, Organizacion.CoordenadasLogo,Organizacion.VersionLogo, Organizacion.Alias FROM Organizacion"; 
            //if (!string.IsNullOrEmpty(pInicio))
            //{
            //    selectOrganizacionConFoto += " where OrganizacionID like'" + pInicio + "%'";
            //}
            //DbCommand commandsqlOrganizacionConFoto = ObtenerComando(selectOrganizacionConFoto);
            //CargarDataSet(commandsqlOrganizacionConFoto, organizacionDS, "Organizacion");

            //return organizacionDS; 
        }

        /// <summary>
        /// Actualiza las coordenadas de la organizacion indicada
        /// </summary>
        /// <param name="pOrganizacionID">ID de la Organizacion</param>
        /// <param name="pCoordenadas">Coordenadas del Logo</param>
        public void ActualizarCoordenadasOrganizacion(Guid pOrganizacionID, string pCoordenadas)
        {
            //TODO: hecho
            var resultado = mEntityContext.Organizacion.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).FirstOrDefault();
            if (resultado != null)
            {
                resultado.CoordenadasLogo = pCoordenadas;
            }
            mEntityContext.SaveChanges();

            //string comandoSql = "UPDATE Organizacion SET CoordenadasLogo = " + IBD.ToParam("CoordenadasLogo") + " WHERE OrganizacionID = " + IBD.GuidValor(pOrganizacionID);

            //DbCommand commandcomandoSql = ObtenerComando(comandoSql);
            //AgregarParametro(commandcomandoSql, IBD.ToParam("CoordenadasLogo"), DbType.String, pCoordenadas);

            //ActualizarBaseDeDatos(commandcomandoSql);
        }

        /// <summary>
        /// Actualiza las coordenadas de la persona en la organizacion indicada
        /// </summary>
        /// <param name="pOrganizacionID">ID de la Organizacion</param>
        /// <param name="pPersonaID">ID de la Persona</param>
        /// <param name="pCoordenadas">Coordenadas de la foto</param>
        public void ActualizarCoordenadasPersonaVincOrganizacion(Guid pOrganizacionID, Guid pPersonaID, string pCoordenadas)
        {
            //TODO: hecho
            var resultado = mEntityContext.PersonaVinculoOrganizacion.Where(item => item.OrganizacionID.Equals(pOrganizacionID) && item.PersonaID.Equals(pPersonaID)).FirstOrDefault();
            if (resultado != null)
            {
                resultado.CoordenadasFoto = pCoordenadas;
                resultado.FechaAnadidaFoto = DateTime.Now;
            }
            mEntityContext.SaveChanges();


            //string comandoSql = "UPDATE PersonaVinculoOrganizacion SET CoordenadasFoto = " + IBD.ToParam("CoordenadasFoto") + ", FechaAnadidaFoto = " + IBD.ToParam("FechaAnadidaFoto") + " WHERE OrganizacionID = " + IBD.GuidValor(pOrganizacionID) + " AND PersonaID = " + IBD.GuidValor(pPersonaID);

            //DbCommand commandcomandoSql = ObtenerComando(comandoSql);
            //AgregarParametro(commandcomandoSql, IBD.ToParam("CoordenadasFoto"), DbType.String, pCoordenadas);
            //AgregarParametro(commandcomandoSql, IBD.ToParam("FechaAnadidaFoto"), DbType.DateTime, DateTime.Now);

            //ActualizarBaseDeDatos(commandcomandoSql);
        }


        /// <summary>
        /// Actualiza el número de la versión de la foto de la organización
        /// </summary>
        /// <param name="pOrganizacionID">ID de la Orgnizacion</param>
        public void ActualizarVersionFotoOrganizacion(Guid pOrganizacionID)
        {
            //TODO: hecho
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

            //string comandoSql = "UPDATE Organizacion SET VersionLogo = case when VersionLogo is null then 1 else VersionLogo +1 end where OrganizacionID= " + IBD.GuidValor(pOrganizacionID);

            //DbCommand commandcomandoSql = ObtenerComando(comandoSql);
            //ActualizarBaseDeDatos(commandcomandoSql);
        }

        /// <summary>
        /// Actualiza el número de la versión de la foto de la persona en la organización
        /// </summary>
        /// <param name="pOrganizacionID">ID de la Orgnizacion</param>
        /// <param name="pPersonaID">ID de la Persona</param>
        public void ActualizarVersionFotoPersonaVincOrganizacion(Guid pOrganizacionID, Guid pPersonaID)
        {
            //TODO: hecho
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

            //string comandoSql = "UPDATE PersonaVinculoOrganizacion SET VersionFoto = case when VersionFoto is null then 1 else VersionFoto +1 end where OrganizacionID= " + IBD.GuidValor(pOrganizacionID)+" AND PersonaID= " + IBD.GuidValor(pPersonaID);

            //DbCommand commandcomandoSql = ObtenerComando(comandoSql);
            //ActualizarBaseDeDatos(commandcomandoSql);
        }


        #endregion

        #region Privados

        /// <summary>
        /// Disminuye el número de organizaciones que participan en un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        private void DisminuirNumeroOrParticipanEnProyecto(Guid pProyectoID)
        {
            //Tengo que actualizar en ProyectoDS / Proyecto / "NumeroOrgRegistradas"
            ProyectoAD proyectoAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            proyectoAD.DisminuirNumeroOrParticipanEnProyecto(pProyectoID);
            proyectoAD.Dispose();
        }

        /// <summary>
        /// Aumenta el número de organizaciones que participan en un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        private void AumentarNumeroOrgParticipanEnProyecto(Guid pProyectoID)
        {
            //Tengo que actualizar en ProyectoDS / Proyecto / "NumeroOrgRegistradas"
            ProyectoAD proyectoAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            proyectoAD.AumentarNumeroOrgParticipanEnProyecto(pProyectoID);
            proyectoAD.Dispose();
        }

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

            this.SelectPesadoOrganizacion = "SELECT " + IBD.CargarGuid("Organizacion.OrganizacionID") + ", Organizacion.Nombre, Organizacion.Telefono, Organizacion.Email, Organizacion.Fax, Organizacion.Web, Organizacion.Logotipo, " + IBD.CargarGuid("Organizacion.PaisID") + ", " + IBD.CargarGuid("Organizacion.ProvinciaID") + ", Organizacion.Provincia, " + IBD.CargarGuid("Organizacion.OrganizacionPadreID") + ", Organizacion.Direccion, Organizacion.CP, Organizacion.Localidad, Organizacion.EsBuscable, Organizacion.EsBuscableExternos, Organizacion.ModoPersonal, Organizacion.Eliminada, Organizacion.NombreCorto, Organizacion.CoordenadasLogo,Organizacion.VersionLogo, Organizacion.Alias FROM Organizacion";

            this.SelectOrganizacionEmpresa = "SELECT " + IBD.CargarGuid("OrganizacionEmpresa.OrganizacionID") + ", OrganizacionEmpresa.CIF, OrganizacionEmpresa.FechaCreacion, OrganizacionEmpresa.Empleados, OrganizacionEmpresa.TipoOrganizacion, OrganizacionEmpresa.SectorOrganizacion FROM OrganizacionEmpresa";

            this.SelectLigeroOrganizacion = "SELECT " + IBD.CargarGuid("Organizacion.OrganizacionID") + ", Organizacion.Nombre, Organizacion.EsBuscable, Organizacion.EsBuscableExternos, Organizacion.ModoPersonal, Organizacion.Eliminada, Organizacion.NombreCorto, Organizacion.CoordenadasLogo,Organizacion.VersionLogo, Organizacion.Alias FROM Organizacion";

            this.SelectOrganizacionClase = "SELECT " + IBD.CargarGuid("OrganizacionClase.OrganizacionID") + ", OrganizacionClase.Centro, OrganizacionClase.Asignatura, OrganizacionClase.Curso, OrganizacionClase.Grupo, OrganizacionClase.CursoAcademico, OrganizacionClase.NombreCortoCentro, OrganizacionClase.NombreCortoAsig, OrganizacionClase.TipoClase FROM OrganizacionClase";

            this.SelectOrganizacionSinLogotipo = "SELECT " + IBD.CargarGuid("Organizacion.OrganizacionID") + ", Organizacion.Nombre, Organizacion.Telefono, Organizacion.Email, Organizacion.Fax, Organizacion.Web, " + IBD.CargarGuid("Organizacion.PaisID") + ", " + IBD.CargarGuid("Organizacion.ProvinciaID") + ", Organizacion.Provincia, " + IBD.CargarGuid("Organizacion.OrganizacionPadreID") + ", Organizacion.Direccion, Organizacion.CP, Organizacion.Localidad, Organizacion.EsBuscable, Organizacion.EsBuscableExternos, Organizacion.ModoPersonal, Organizacion.Eliminada, Organizacion.NombreCorto, Organizacion.CoordenadasLogo, Organizacion.Alias FROM Organizacion";

            this.SelectOrganizacionParticipaProy = "SELECT " + IBD.CargarGuid("OrganizacionParticipaProy.OrganizacionID") + ", " + IBD.CargarGuid("OrganizacionParticipaProy.OrganizacionProyectoID") + ", " + IBD.CargarGuid("OrganizacionParticipaProy.ProyectoID") + ", OrganizacionParticipaProy.FechaInicio, " + IBD.CargarGuid("OrganizacionParticipaProy.IdentidadID") + " , OrganizacionParticipaProy.EstaBloqueada ,OrganizacionParticipaProy.RegistroAutomatico";

            this.SelectTagOrganizacion = "SELECT TagOrganizacion.TagID, TagOrganizacion.Tipo, " + IBD.CargarGuid("TagOrganizacion.OrganizacionID") + " FROM TagOrganizacion";

            this.SelectAdministradorOrganizacion = "SELECT " + IBD.CargarGuid("AdministradorOrganizacion.UsuarioID") + ", " + IBD.CargarGuid("AdministradorOrganizacion.OrganizacionID") + " , AdministradorOrganizacion.Tipo ";

            this.SelectPesadoPersonaVinculoOrganizacion = "SELECT " + IBD.CargarGuid("PersonaVinculoOrganizacion.PersonaID") + ", " + IBD.CargarGuid("PersonaVinculoOrganizacion.OrganizacionID") + ", PersonaVinculoOrganizacion.Cargo, " + IBD.CargarGuid("PersonaVinculoOrganizacion.PaisTrabajoID") + ", " + IBD.CargarGuid("PersonaVinculoOrganizacion.ProvinciaTrabajoID") + ", PersonaVinculoOrganizacion.ProvinciaTrabajo, PersonaVinculoOrganizacion.CPTrabajo, PersonaVinculoOrganizacion.DireccionTrabajo, PersonaVinculoOrganizacion.LocalidadTrabajo, PersonaVinculoOrganizacion.TelefonoTrabajo, PersonaVinculoOrganizacion.Extension, PersonaVinculoOrganizacion.TelefonoMovilTrabajo, PersonaVinculoOrganizacion.EmailTrabajo, " + IBD.CargarGuid("PersonaVinculoOrganizacion.CategoriaProfesionalID") + ", " + IBD.CargarGuid("PersonaVinculoOrganizacion.TipoContratoID") + ",PersonaVinculoOrganizacion.FechaVinculacion, PersonaVinculoOrganizacion.Foto, PersonaVinculoOrganizacion.CoordenadasFoto, PersonaVinculoOrganizacion.UsarFotoPersonal, PersonaVinculoOrganizacion.VersionFoto, PersonaVinculoOrganizacion.FechaAnadidaFoto FROM PersonaVinculoOrganizacion";

            this.SelectPersonaVisibleEnOrg = "SELECT " + IBD.CargarGuid("PersonaVisibleEnOrg.PersonaID") + ", " + IBD.CargarGuid("PersonaVisibleEnOrg.OrganizacionID") + ", PersonaVisibleEnOrg.Orden FROM PersonaVisibleEnOrg ";


            this.SelectLigeroPersonaVinculoOrganizacion = "SELECT " + IBD.CargarGuid("PersonaVinculoOrganizacion.PersonaID") + ", " + IBD.CargarGuid("PersonaVinculoOrganizacion.OrganizacionID") + ", FechaVinculacion, Cargo, EmailTrabajo, UsarFotoPersonal FROM PersonaVinculoOrganizacion ";

            this.SelectEmpleadoHistoricoEstadoLaboral = "SELECT " + IBD.CargarGuid("EmpleadoHistoricoEstadoLaboral.PersonaID") + ", " + IBD.CargarGuid("EmpleadoHistoricoEstadoLaboral.OrganizacionID") + ", " + IBD.CargarGuid("EmpleadoHistoricoEstadoLaboral.EstadoLaboralID") + ", EmpleadoHistoricoEstadoLaboral.Fecha, EmpleadoHistoricoEstadoLaboral.Causa ";

            this.SelectHistoricoOrganizacionParticipaProy = "SELECT " + IBD.CargarGuid("HistoricoOrgParticipaProy.OrganizacionID") + ", " + IBD.CargarGuid("HistoricoOrgParticipaProy.OrganizacionProyectoID") + ", " + IBD.CargarGuid("HistoricoOrgParticipaProy.ProyectoID") + " FROM HistoricoOrgParticipaProy";

            this.SelectIdentidadOrganizacion = "SELECT " + IBD.CargarGuid("IdentidadOrganizacio.IdentidadID") + ", " + IBD.CargarGuid("IdentidadOrganizacio.OrganizacionID") + " ";

            this.SelectOrganizacionGnoss = "SELECT " + IBD.CargarGuid("OrganizacionGnoss.OrganizacionID");

            this.SelectPesadoSede = "SELECT " + IBD.CargarGuid("Sede.SedeID") + ", " + IBD.CargarGuid("Sede.OrganizacionID") + ", Sede.Telefono, Sede.Fax, " + IBD.CargarGuid("Sede.PaisID") + ", " + IBD.CargarGuid("Sede.ProvinciaID") + ", Sede.Provincia, Sede.Localidad, Sede.CP, Sede.Direccion ";

            this.SelectLigeroSede = "SELECT " + IBD.CargarGuid("SedeID") + ", " + IBD.CargarGuid("OrganizacionID") + " ";

            this.SelectCnaeAgregacionOrganizacion = "SELECT " + IBD.CargarGuid("CnaeAgregacionOrganizacion.OrganizacionID") + ", CnaeAgregacionOrganizacion.CnaeID ";

            this.SelectConfiguracionGnossOrg = "SELECT " + IBD.CargarGuid("ConfiguracionGnossOrg.OrganizacionID") + ", ConfiguracionGnossOrg.VerRecursos, ConfiguracionGnossOrg.VerRecursosExterno, ConfiguracionGnossOrg.VisibilidadContactos ";

            #endregion

            this.sqlSelectObtenerOrganizacionesSolicitanAccesoAProyecto = SelectLigeroOrganizacion + " INNER JOIN SolicitudOrganizacion ON Organizacion.OrganizacionID = SolicitudOrganizacion.OrganizacionID INNER JOIN Solicitud ON Solicitud.SolicitudID = SolicitudOrganizacion.SolicitudID WHERE Solicitud.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + " AND Solicitud.ProyectoID = " + IBD.GuidParamValor("proyectoID") + " AND Solicitud.Estado = " + ((short)EstadoSolicitud.Espera).ToString();

            this.sqlSelectObtenerOrganizacionesEmpresaSolicitanAccesoAProyecto = SelectOrganizacionEmpresa + " INNER JOIN SolicitudOrganizacion ON OrganizacionEmpresa.OrganizacionID = SolicitudOrganizacion.OrganizacionID INNER JOIN Solicitud ON Solicitud.SolicitudID = SolicitudOrganizacion.SolicitudID WHERE Solicitud.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + " AND Solicitud.ProyectoID = " + IBD.GuidParamValor("proyectoID") + " AND Solicitud.Estado = " + ((short)EstadoSolicitud.Espera).ToString();

            this.sqlSelectObtenerOrganizacionesClaseSolicitanAccesoAProyecto = SelectOrganizacionClase + " INNER JOIN SolicitudOrganizacion ON OrganizacionClase.OrganizacionID = SolicitudOrganizacion.OrganizacionID INNER JOIN Solicitud ON Solicitud.SolicitudID = SolicitudOrganizacion.SolicitudID WHERE Solicitud.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + " AND Solicitud.ProyectoID = " + IBD.GuidParamValor("proyectoID") + " AND Solicitud.Estado = " + ((short)EstadoSolicitud.Espera).ToString();

            this.sqlSelectExisteOrganizacionEnBD = "SELECT " + IBD.CargarGuid("OrganizacionID") + " FROM Organizacion WHERE (UPPER(Nombre) = UPPER(" + IBD.ToParam("nombreOrganizacion") + "))";

            this.sqlSelectExisteOrganizacionEnBDPorIDOrg = "SELECT " + IBD.CargarGuid("OrganizacionID") + " FROM Organizacion WHERE OrganizacionID=" + IBD.ToParam("OrganizacionID") + " ";

            this.sqlSelectObtenerOrganizacionesDePersonasDeEstructuraDeProyectoCargaLigera = SelectLigeroOrganizacion.Replace("SELECT", "SELECT DISTINCT ") + " INNER JOIN PersonaOcupacionFigura  ON Organizacion.OrganizacionID = PersonaOcupacionFigura.OrganizacionPersonalID WHERE (PersonaOcupacionFigura.ProyectoID = " + IBD.GuidParamValor("ProyectoID") + ") UNION " + SelectLigeroOrganizacion.Replace("SELECT", "SELECT DISTINCT ") + " INNER JOIN PersonaOcupacionFormaSec  ON Organizacion.OrganizacionID = PersonaOcupacionFormaSec.OrganizacionPersonalID WHERE  (PersonaOcupacionFormaSec.ProyectoID = " + IBD.GuidParamValor("ProyectoID") + ")";

            this.sqlSelectObtenerOrganizacionesGnossDePersonasDeEstructuraDeProyectoCargaLigera = SelectOrganizacionGnoss.Replace("SELECT", "SELECT DISTINCT ") + " FROM OrganizacionGnoss INNER JOIN PersonaOcupacionFigura  ON OrganizacionGnoss.OrganizacionID = PersonaOcupacionFigura.OrganizacionPersonalID WHERE (PersonaOcupacionFigura.ProyectoID = " + IBD.GuidParamValor("ProyectoID") + ") UNION " + SelectOrganizacionGnoss.Replace("SELECT", "SELECT DISTINCT ") + " FROM OrganizacionGnoss INNER JOIN PersonaOcupacionFormaSec  ON OrganizacionGnoss.OrganizacionID = PersonaOcupacionFormaSec.OrganizacionPersonalID WHERE  (PersonaOcupacionFormaSec.ProyectoID = " + IBD.GuidParamValor("ProyectoID") + ")";

            this.sqlSelectObtenerPersonaVinculoOrganizacionDeEstructuraDeProyectoCargaLigera = "SELECT DISTINCT " + IBD.CargarGuid("PersonaVinculoOrganizacion.OrganizacionID") + ", " + IBD.CargarGuid("PersonaVinculoOrganizacion.PersonaID") + " , PersonaVinculoOrganizacion.FechaVinculacion, PersonaVinculoOrganizacion.Cargo, PersonaVinculoOrganizacion.TelefonoTrabajo, PersonaVinculoOrganizacion.EmailTrabajo, PersonaVinculoOrganizacion.UsarFotoPersonal FROM PersonaVinculoOrganizacion INNER JOIN PersonaOcupacionFigura ON PersonaVinculoOrganizacion.OrganizacionID = PersonaOcupacionFigura.OrganizacionPersonalID WHERE (PersonaOcupacionFigura.ProyectoID = " + IBD.GuidParamValor("ProyectoID") + ") UNION SELECT DISTINCT " + IBD.CargarGuid("PersonaVinculoOrganizacion.OrganizacionID") + ", " + IBD.CargarGuid("PersonaVinculoOrganizacion.PersonaID") + " , PersonaVinculoOrganizacion.FechaVinculacion, PersonaVinculoOrganizacion.Cargo, PersonaVinculoOrganizacion.TelefonoTrabajo, PersonaVinculoOrganizacion.EmailTrabajo, PersonaVinculoOrganizacion.UsarFotoPersonal FROM PersonaVinculoOrganizacion INNER JOIN PersonaOcupacionFormaSec ON PersonaVinculoOrganizacion.OrganizacionID = PersonaOcupacionFormaSec.OrganizacionPersonalID WHERE (PersonaOcupacionFormaSec.ProyectoID = " + IBD.GuidParamValor("ProyectoID") + ")";

            this.sqlSelectOrganizacionPorID = SelectPesadoOrganizacion + " WHERE (OrganizacionID = " + IBD.GuidParamValor("organizacionID") + ")";

            this.sqlSelectOrganizacionEmpresaPorID = SelectOrganizacionEmpresa + " WHERE (OrganizacionID = " + IBD.GuidParamValor("organizacionID") + ")";

            this.sqlSelectOrganizacionClasePorID = SelectOrganizacionClase + " WHERE (OrganizacionID = " + IBD.GuidParamValor("organizacionID") + ")";

            this.sqlSelectOrganizacionPorIDLigero = SelectLigeroOrganizacion + " WHERE (OrganizacionID = " + IBD.GuidParamValor("organizacionID") + ")";

            this.sqlSelectNombreOrganizacionPorID = SelectLigeroOrganizacion + " WHERE (OrganizacionID = " + IBD.GuidParamValor("organizacionID") + ")";

            this.sqlSelectOrganizacionesParticipaUsuario = SelectPesadoOrganizacion + " INNER JOIN PersonaVinculoOrganizacion ON PersonaVinculoOrganizacion.OrganizacionID = Organizacion.OrganizacionID INNER JOIN Persona ON PersonaVinculoOrganizacion.PersonaID = Persona.PersonaID WHERE (Persona.UsuarioID = " + IBD.GuidParamValor("usuarioID") + ") ORDER BY Organizacion.Nombre";

            this.sqlSelectOrganizacionesEmpresaParticipaUsuario = SelectOrganizacionEmpresa + " INNER JOIN PersonaVinculoOrganizacion ON PersonaVinculoOrganizacion.OrganizacionID = OrganizacionEmpresa.OrganizacionID INNER JOIN Persona ON PersonaVinculoOrganizacion.PersonaID = Persona.PersonaID WHERE (Persona.UsuarioID = " + IBD.GuidParamValor("usuarioID") + ")";

            this.sqlSelectOrganizacionesClaseParticipaUsuario = SelectOrganizacionClase + " INNER JOIN PersonaVinculoOrganizacion ON PersonaVinculoOrganizacion.OrganizacionID = OrganizacionClase.OrganizacionID INNER JOIN Persona ON PersonaVinculoOrganizacion.PersonaID = Persona.PersonaID WHERE (Persona.UsuarioID = " + IBD.GuidParamValor("usuarioID") + ")";

            this.sqlSelectOrganizacionClaseClasesAdministraUsuario = SelectOrganizacionClase.Replace("FROM", ", Organizacion.Nombre FROM") + " INNER JOIN AdministradorOrganizacion ON AdministradorOrganizacion.OrganizacionID = OrganizacionClase.OrganizacionID INNER JOIN Organizacion ON Organizacion.OrganizacionID = AdministradorOrganizacion.OrganizacionID WHERE (AdministradorOrganizacion.UsuarioID = " + IBD.GuidParamValor("usuarioID") + ") AND AdministradorOrganizacion.Tipo = " + (short)TipoAdministradoresOrganizacion.Administrador + " AND Organizacion.OrganizacionID NOT IN (SELECT PersonaVinculoOrganizacion.OrganizacionID FROM PersonaVinculoOrganizacion INNER JOIN Persona ON Persona.PersonaID = PersonaVinculoOrganizacion.PersonaID WHERE Persona.UsuarioID = " + IBD.GuidParamValor("usuarioID") + ")";

            this.sqlSelectOrganizacionesDeIdentidad = SelectPesadoOrganizacion + " INNER JOIN (SELECT Organizacion.OrganizacionID FROM Organizacion INNER JOIN PerfilPersonaOrg ON PerfilPersonaOrg.OrganizacionID = Organizacion.OrganizacionID INNER JOIN Identidad ON Identidad.PerfilID = PerfilPersonaOrg.PerfilID WHERE (Identidad.IdentidadID = " + IBD.GuidParamValor("identidadID") + ")" + " UNION SELECT Organizacion.OrganizacionID FROM Organizacion INNER JOIN PerfilOrganizacion ON PerfilOrganizacion.OrganizacionID = Organizacion.OrganizacionID INNER JOIN Identidad ON Identidad.PerfilID = PerfilOrganizacion.PerfilID WHERE (Identidad.IdentidadID = " + IBD.GuidParamValor("identidadID") + ") ) Resultado ON Resultado.OrganizacionID = Organizacion.OrganizacionID ORDER BY Organizacion.Nombre";

            this.sqlSelectOrganizacionesEmpresaDeIdentidad = SelectOrganizacionEmpresa + " INNER JOIN (SELECT Organizacion.OrganizacionID FROM Organizacion INNER JOIN PerfilPersonaOrg ON PerfilPersonaOrg.OrganizacionID = Organizacion.OrganizacionID INNER JOIN Identidad ON Identidad.PerfilID = PerfilPersonaOrg.PerfilID WHERE (Identidad.IdentidadID = " + IBD.GuidParamValor("identidadID") + ")" + " UNION SELECT Organizacion.OrganizacionID FROM Organizacion INNER JOIN PerfilOrganizacion ON PerfilOrganizacion.OrganizacionID = Organizacion.OrganizacionID INNER JOIN Identidad ON Identidad.PerfilID = PerfilOrganizacion.PerfilID WHERE (Identidad.IdentidadID = " + IBD.GuidParamValor("identidadID") + ") ) Resultado ON Resultado.OrganizacionID = OrganizacionEmpresa.OrganizacionID";

            this.sqlSelectOrganizacionesClaseDeIdentidad = SelectOrganizacionClase + " INNER JOIN (SELECT Organizacion.OrganizacionID FROM Organizacion INNER JOIN PerfilPersonaOrg ON PerfilPersonaOrg.OrganizacionID = Organizacion.OrganizacionID INNER JOIN Identidad ON Identidad.PerfilID = PerfilPersonaOrg.PerfilID WHERE (Identidad.IdentidadID = " + IBD.GuidParamValor("identidadID") + ")" + " UNION SELECT Organizacion.OrganizacionID FROM Organizacion INNER JOIN PerfilOrganizacion ON PerfilOrganizacion.OrganizacionID = Organizacion.OrganizacionID INNER JOIN Identidad ON Identidad.PerfilID = PerfilOrganizacion.PerfilID WHERE (Identidad.IdentidadID = " + IBD.GuidParamValor("identidadID") + ") ) Resultado ON Resultado.OrganizacionID = OrganizacionClase.OrganizacionID";

            this.sqlSelectOrganizacionesClaseDePersonaSinPersonaVinculadaOrganizacion = SelectOrganizacionClase + " INNER JOIN (SELECT Organizacion.OrganizacionID FROM Organizacion INNER JOIN PerfilPersonaOrg ON PerfilPersonaOrg.OrganizacionID = Organizacion.OrganizacionID WHERE (PerfilPersonaOrg.PersonaID = " + IBD.GuidParamValor("PersonaID") + ")" + " UNION SELECT Organizacion.OrganizacionID FROM Organizacion INNER JOIN PerfilOrganizacion ON PerfilOrganizacion.OrganizacionID = Organizacion.OrganizacionID INNER JOIN Perfil ON PerfilOrganizacion.PerfilID = Perfil.PerfilID WHERE (Perfil.PersonaID = " + IBD.GuidParamValor("PersonaID") + ") ) Resultado ON Resultado.OrganizacionID = OrganizacionClase.OrganizacionID";

            this.sqlSelectPersonaVinculoOrganizacionDeIdentidad = SelectPesadoPersonaVinculoOrganizacion + " INNER JOIN (SELECT Organizacion.OrganizacionID FROM Organizacion INNER JOIN PerfilPersonaOrg ON PerfilPersonaOrg.OrganizacionID = Organizacion.OrganizacionID INNER JOIN Identidad ON Identidad.PerfilID = PerfilPersonaOrg.PerfilID WHERE (Identidad.IdentidadID = " + IBD.GuidParamValor("identidadID") + ")" + " UNION SELECT Organizacion.OrganizacionID FROM Organizacion INNER JOIN PerfilOrganizacion ON PerfilOrganizacion.OrganizacionID = Organizacion.OrganizacionID INNER JOIN Identidad ON Identidad.PerfilID = PerfilOrganizacion.PerfilID WHERE (Identidad.IdentidadID = " + IBD.GuidParamValor("identidadID") + ") ) Resultado ON Resultado.OrganizacionID = PersonaVinculoOrganizacion.OrganizacionID";

            this.sqlSelectOrganizacionesGnossAccedeUsuario = SelectLigeroOrganizacion.Replace("SELECT", "SELECT DISTINCT ") + " INNER JOIN ProyectoRolUsuario ON Organizacion.OrganizacionID = ProyectoRolUsuario.OrganizacionGnossID WHERE (ProyectoRolUsuario.UsuarioID = " + IBD.GuidParamValor("usuarioID") + ")";

            this.sqlSelectOrganizacionesEmpresaAccedeUsuario = SelectOrganizacionEmpresa.Replace("SELECT", "SELECT DISTINCT ") + " INNER JOIN ProyectoRolUsuario ON OrganizacionEmpresa.OrganizacionID = ProyectoRolUsuario.OrganizacionGnossID WHERE (ProyectoRolUsuario.UsuarioID = " + IBD.GuidParamValor("usuarioID") + ")";

            this.sqlSelectOrganizacionesClaseAccedeUsuario = SelectOrganizacionClase.Replace("SELECT", "SELECT DISTINCT ") + " INNER JOIN ProyectoRolUsuario ON OrganizacionClase.OrganizacionID = ProyectoRolUsuario.OrganizacionGnossID WHERE (ProyectoRolUsuario.UsuarioID = " + IBD.GuidParamValor("usuarioID") + ")";

            this.sqlSelectTodasOrgPesada = SelectPesadoOrganizacion;

            this.sqlSelectTodasOrgLigera = SelectLigeroOrganizacion;

            this.sqlSelectOrganizacionesVinculadasAPersona = SelectLigeroPersonaVinculoOrganizacion + " WHERE (PersonaID = " + IBD.GuidParamValor("personaID") + ")";

            this.sqlSelectOrganizacionesVinculadasAPersonaPesada = SelectPesadoPersonaVinculoOrganizacion + " WHERE (PersonaID = " + IBD.GuidParamValor("personaID") + ")";

            this.sqlSelectOrganizacionDePersYOrg = SelectPesadoOrganizacion + " INNER JOIN PersonaVinculoOrganizacion ON Organizacion.OrganizacionID = PersonaVinculoOrganizacion.OrganizacionID WHERE (PersonaVinculoOrganizacion.PersonaID = " + IBD.GuidParamValor("personaID") + ") AND (PersonaVinculoOrganizacion.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + ")";

            this.sqlSelectOrgEmpresaDePersYOrg = SelectOrganizacionEmpresa + " INNER JOIN PersonaVinculoOrganizacion ON OrganizacionEmpresa.OrganizacionID = PersonaVinculoOrganizacion.OrganizacionID WHERE (PersonaVinculoOrganizacion.PersonaID = " + IBD.GuidParamValor("personaID") + ") AND (PersonaVinculoOrganizacion.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + ")";

            this.sqlSelectOrganizacionClaseDePersYOrg = SelectOrganizacionClase + " INNER JOIN PersonaVinculoOrganizacion ON OrganizacionClase.OrganizacionID = PersonaVinculoOrganizacion.OrganizacionID WHERE (PersonaVinculoOrganizacion.PersonaID = " + IBD.GuidParamValor("personaID") + ") AND (PersonaVinculoOrganizacion.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + ")";

            this.sqlSelectPersonaVinculoOrganizacionDePersYOrg = SelectPesadoPersonaVinculoOrganizacion + " WHERE (PersonaVinculoOrganizacion.PersonaID = " + IBD.GuidParamValor("personaID") + ") AND (PersonaVinculoOrganizacion.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + ")";


            this.sqlSelectOrganizacionesDePersona = SelectLigeroOrganizacion + " INNER JOIN PersonaVinculoOrganizacion ON Organizacion.OrganizacionID = PersonaVinculoOrganizacion.OrganizacionID WHERE (PersonaVinculoOrganizacion.PersonaID = " + IBD.GuidParamValor("personaID") + ")";

            this.sqlSelectOrganizacionesDePersonaPesada = SelectPesadoOrganizacion + " INNER JOIN PersonaVinculoOrganizacion ON Organizacion.OrganizacionID = PersonaVinculoOrganizacion.OrganizacionID WHERE (PersonaVinculoOrganizacion.PersonaID = " + IBD.GuidParamValor("personaID") + ")";
            this.sqlSelectOrganizacionesEmpresaDePersona = SelectOrganizacionEmpresa + " INNER JOIN PersonaVinculoOrganizacion ON OrganizacionEmpresa.OrganizacionID = PersonaVinculoOrganizacion.OrganizacionID WHERE (PersonaVinculoOrganizacion.PersonaID = " + IBD.GuidParamValor("personaID") + ")";

            this.sqlSelectOrganizacionesEmpresaDePersonaPesada = SelectOrganizacionEmpresa + " INNER JOIN PersonaVinculoOrganizacion ON OrganizacionEmpresa.OrganizacionID = PersonaVinculoOrganizacion.OrganizacionID WHERE (PersonaVinculoOrganizacion.PersonaID = " + IBD.GuidParamValor("personaID") + ")";

            this.sqlSelectOrganizacionesClaseDePersona = SelectOrganizacionClase + " INNER JOIN PersonaVinculoOrganizacion ON OrganizacionClase.OrganizacionID = PersonaVinculoOrganizacion.OrganizacionID WHERE (PersonaVinculoOrganizacion.PersonaID = " + IBD.GuidParamValor("personaID") + ")";

            this.sqlSelectTagOrganizacionesDePersona = SelectTagOrganizacion + " INNER JOIN PersonaVinculoOrganizacion ON TagOrganizacion.OrganizacionID = PersonaVinculoOrganizacion.OrganizacionID WHERE (PersonaVinculoOrganizacion.PersonaID = " + IBD.GuidParamValor("personaID") + ")";

            this.sqlSelectTodasOrgGnossCargaLigera = SelectLigeroOrganizacion + " INNER JOIN OrganizacionGnoss ON Organizacion.OrganizacionID = OrganizacionGnoss.OrganizacionID";

            this.sqlSelectTodasOrgGnossCargaPesada = SelectPesadoOrganizacion + " INNER JOIN OrganizacionGnoss ON Organizacion.OrganizacionID = OrganizacionGnoss.OrganizacionID";

            this.sqlSelectTodasOrgEmpresaGnossCargaLigera = SelectOrganizacionEmpresa + " INNER JOIN OrganizacionGnoss ON OrganizacionEmpresa.OrganizacionID = OrganizacionGnoss.OrganizacionID";

            this.sqlSelectTodasOrgEmpresaGnossCargaPesada = SelectOrganizacionEmpresa + " INNER JOIN OrganizacionGnoss ON OrganizacionEmpresa.OrganizacionID = OrganizacionGnoss.OrganizacionID";

            this.sqlSelectTodasOrgClaseGnoss = SelectOrganizacionClase + " INNER JOIN OrganizacionGnoss ON OrganizacionClase.OrganizacionID = OrganizacionGnoss.OrganizacionID";

            this.sqlSelectTagsOrganizacionGnoss = SelectTagOrganizacion + " INNER JOIN OrganizacionGnoss ON TagOrganizacion.OrganizacionID = OrganizacionGnoss.OrganizacionID";

            this.sqlSelectOrgGnoss = SelectOrganizacionGnoss + " FROM OrganizacionGnoss";

            this.sqlSelectTodasCnaeAgregacionOrganizacionOrgGnoss = SelectCnaeAgregacionOrganizacion + " FROM CnaeAgregacionOrganizacion INNER JOIN OrganizacionGnoss ON CnaeAgregacionOrganizacion.OrganizacionID = OrganizacionGnoss.OrganizacionID";

            this.sqlSelectCnaeAgregacionDeOrganizacion = SelectCnaeAgregacionOrganizacion + " FROM CnaeAgregacionOrganizacion WHERE OrganizacionID = " + IBD.GuidParamValor("organizacionID");

            this.sqlSelectTodosOrgGnossAdministradorOrganizacion = SelectAdministradorOrganizacion + " FROM AdministradorOrganizacion INNER JOIN OrganizacionGnoss ON OrganizacionGnoss.OrganizacionID = AdministradorOrganizacion.OrganizacionID";

            this.sqlSelectOrgGnossPorID = SelectOrganizacionGnoss + " FROM OrganizacionGnoss WHERE (OrganizacionID = " + IBD.GuidParamValor("organizacionID") + ")";

            this.sqlSelectTodasCnaeAgregacionOrganizacionOrgGnossPorID = SelectCnaeAgregacionOrganizacion + " FROM CnaeAgregacionOrganizacion WHERE (OrganizacionID = " + IBD.GuidParamValor("organizacionID") + ")";

            this.sqlSelectTodosOrgGnossAdministradorOrganizacionPorID = SelectAdministradorOrganizacion + " FROM AdministradorOrganizacion WHERE (OrganizacionID = " + IBD.GuidParamValor("organizacionID") + ")";

            this.sqlSelectLigeroPersonaVinculoOrgPorID = SelectLigeroPersonaVinculoOrganizacion + " WHERE (OrganizacionID = " + IBD.GuidParamValor("organizacionID") + ")";

            this.sqlSelectPesadoPersonaVinculoOrgPorID = SelectPesadoPersonaVinculoOrganizacion + " WHERE (OrganizacionID = " + IBD.GuidParamValor("organizacionID") + ")";

            this.sqlSelectObtenerOrganizacionParticipaProyPorSusIdentidadesDeProyecto = SelectOrganizacionParticipaProy + " FROM OrganizacionParticipaProy WHERE (ProyectoID = " + IBD.GuidParamValor("proyectoID") + ")";

            this.sqlSelectObtenerOrganizacionesPorSusIdentidadesDeProyecto = SelectLigeroOrganizacion.Replace("FROM Organizacion", ", TablaBaseOrganizacionID FROM Organizacion") + " INNER JOIN OrganizacionParticipaProy ON Organizacion.OrganizacionID = OrganizacionParticipaProy.OrganizacionID  WHERE (ProyectoID = " + IBD.GuidParamValor("proyectoID") + ")";

            this.sqlSelectObtenerOrganizacionesEmpresaPorSusIdentidadesDeProyecto = SelectOrganizacionEmpresa + " INNER JOIN OrganizacionParticipaProy ON OrganizacionEmpresa.OrganizacionID = OrganizacionParticipaProy.OrganizacionID WHERE (ProyectoID = " + IBD.GuidParamValor("proyectoID") + ")";

            this.sqlSelectObtenerOrganizacionesClasePorSusIdentidadesDeProyecto = SelectOrganizacionClase + " INNER JOIN OrganizacionParticipaProy ON OrganizacionClase.OrganizacionID = OrganizacionParticipaProy.OrganizacionID WHERE (ProyectoID = " + IBD.GuidParamValor("proyectoID") + ")";

            this.sqlSelectTodasOrgLigeraCorporativo = SelectLigeroOrganizacion + " WHERE Organizacion.ModoPersonal = 0";

            this.sqlSelectTodasOrgEmpresaLigeraCorporativo = SelectOrganizacionEmpresa + " INNER JOIN Organizacion ON OrganizacionEmpresa.OrganizacionID = Organizacion.OrganizacionID WHERE Organizacion.ModoPersonal = 0";

            this.sqlSelectTodasOrgClaseCorporativo = SelectOrganizacionClase + " INNER JOIN Organizacion ON OrganizacionClase.OrganizacionID = Organizacion.OrganizacionID WHERE Organizacion.ModoPersonal = 0";

            this.sqlSelectEmpleadoHistoricoEstadoLaboralDePersona = SelectEmpleadoHistoricoEstadoLaboral + " FROM EmpleadoHistoricoEstadoLaboral WHERE (PersonaID = " + IBD.GuidParamValor("personaID") + ")";

            this.sqlSelectEmpleadoHistoricoEstadoLaboralDeOrganizacion = SelectEmpleadoHistoricoEstadoLaboral + " FROM EmpleadoHistoricoEstadoLaboral WHERE (OrganizacionID = " + IBD.GuidParamValor("organizacionID") + ")";

            // David: Traer los nombres de organización, nombre de proyecto, nombre y apellidos de administrador de los mismos y su email, de
            //        los proyectos en los que una persona se encuentra en una ocupación. Se tiene en cuenta que el
            //        administrador puede tener el perfil como persona libre, como vinculada con una organización y que la ocupación
            //        puede ser de una figura o una forma secundaria
            this.sqlSelectOcupacionesParticipaOrganizacion = IBD.ReplaceParam("(SELECT Organizacion.Nombre as OrganizacionNombre, Proyecto.Nombre as ProyectoNombre, Persona.Nombre as AdministradorNombre, Persona.Apellidos as AdministradorApellidos, PersonaVinculoOrganizacion.EmailTrabajo as AdministradorEmail FROM PersonaOcupacionFigura INNER JOIN ((Organizacion INNER JOIN (PerfilPersonaOrg INNER JOIN ((ProyectoUsuarioIdentidad INNER JOIN Identidad ON ProyectoUsuarioIdentidad.IdentidadID = Identidad.IdentidadID) INNER JOIN (PersonaVinculoOrganizacion INNER JOIN (Persona INNER JOIN AdministradorProyecto ON Persona.UsuarioID = AdministradorProyecto.UsuarioID) ON PersonaVinculoOrganizacion.PersonaID = Persona.PersonaID) ON ProyectoUsuarioIdentidad.UsuarioID = AdministradorProyecto.UsuarioID) ON (PerfilPersonaOrg.PersonaID = Persona.PersonaID) AND (PerfilPersonaOrg.PerfilID = Identidad.PerfilID)) ON (Organizacion.OrganizacionID = ProyectoUsuarioIdentidad.OrganizacionGnossID) AND (Organizacion.OrganizacionID = PersonaVinculoOrganizacion.OrganizacionID) AND (Organizacion.OrganizacionID = PerfilPersonaOrg.OrganizacionID) AND (Organizacion.OrganizacionID = AdministradorProyecto.OrganizacionID) AND (Organizacion.OrganizacionID = PersonaVinculoOrganizacion.OrganizacionID) AND (Organizacion.OrganizacionID = PersonaVinculoOrganizacion.OrganizacionID)) INNER JOIN Proyecto ON (Organizacion.OrganizacionID = Proyecto.OrganizacionID) AND (AdministradorProyecto.ProyectoID = Proyecto.ProyectoID) AND (ProyectoUsuarioIdentidad.ProyectoID = Proyecto.ProyectoID)) ON (PersonaOcupacionFigura.ProyectoID = Proyecto.ProyectoID) AND (PersonaOcupacionFigura.OrganizacionID = Organizacion.OrganizacionID) WHERE (((PersonaOcupacionFigura.OrganizacionPersonalID)=" + IBD.GuidParamValor("organizacionID") + ") AND ((AdministradorProyecto.Tipo)=@tipo))) UNION " +
                "(SELECT Organizacion.Nombre as OrganizacionNombre, Proyecto.Nombre as ProyectoNombre, Persona.Nombre as AdministradorNombre, Persona.Apellidos as AdministradorApellidos, DatosTrabajoPersonaLibre.EmailTrabajo as AdministradorEmail FROM PersonaOcupacionFigura INNER JOIN (DatosTrabajoPersonaLibre INNER JOIN (((Organizacion INNER JOIN ((ProyectoUsuarioIdentidad INNER JOIN Identidad ON ProyectoUsuarioIdentidad.IdentidadID = Identidad.IdentidadID) INNER JOIN (Persona INNER JOIN AdministradorProyecto ON Persona.UsuarioID = AdministradorProyecto.UsuarioID) ON ProyectoUsuarioIdentidad.UsuarioID = AdministradorProyecto.UsuarioID) ON (Organizacion.OrganizacionID = ProyectoUsuarioIdentidad.OrganizacionGnossID) AND (Organizacion.OrganizacionID = AdministradorProyecto.OrganizacionID)) INNER JOIN Proyecto ON (Organizacion.OrganizacionID = Proyecto.OrganizacionID) AND (AdministradorProyecto.ProyectoID = Proyecto.ProyectoID) AND (ProyectoUsuarioIdentidad.ProyectoID = Proyecto.ProyectoID)) INNER JOIN PerfilPersona ON (Persona.PersonaID = PerfilPersona.PersonaID) AND (Identidad.PerfilID = PerfilPersona.PerfilID)) ON DatosTrabajoPersonaLibre.PersonaID = Persona.PersonaID) ON (PersonaOcupacionFigura.ProyectoID = Proyecto.ProyectoID) AND (PersonaOcupacionFigura.OrganizacionID = Organizacion.OrganizacionID) WHERE (((PersonaOcupacionFigura.OrganizacionPersonalID)=" + IBD.GuidParamValor("organizacionID") + ") AND ((AdministradorProyecto.Tipo)=@tipo))) UNION " +
                "(SELECT Organizacion.Nombre as OrganizacionNombre, Proyecto.Nombre as ProyectoNombre, Persona.Nombre as AdministradorNombre, Persona.Apellidos as AdministradorApellidos, PersonaVinculoOrganizacion.EmailTrabajo as AdministradorEmail FROM PersonaOcupacionFormaSec INNER JOIN ((Organizacion INNER JOIN (PerfilPersonaOrg INNER JOIN ((ProyectoUsuarioIdentidad INNER JOIN Identidad ON ProyectoUsuarioIdentidad.IdentidadID = Identidad.IdentidadID) INNER JOIN (PersonaVinculoOrganizacion INNER JOIN (Persona INNER JOIN AdministradorProyecto ON Persona.UsuarioID = AdministradorProyecto.UsuarioID) ON PersonaVinculoOrganizacion.PersonaID = Persona.PersonaID) ON ProyectoUsuarioIdentidad.UsuarioID = AdministradorProyecto.UsuarioID) ON (PerfilPersonaOrg.PersonaID = Persona.PersonaID) AND (PerfilPersonaOrg.PerfilID = Identidad.PerfilID)) ON (Organizacion.OrganizacionID = ProyectoUsuarioIdentidad.OrganizacionGnossID) AND (Organizacion.OrganizacionID = PersonaVinculoOrganizacion.OrganizacionID) AND (Organizacion.OrganizacionID = PerfilPersonaOrg.OrganizacionID) AND (Organizacion.OrganizacionID = AdministradorProyecto.OrganizacionID) AND (Organizacion.OrganizacionID = PersonaVinculoOrganizacion.OrganizacionID) AND (Organizacion.OrganizacionID = PersonaVinculoOrganizacion.OrganizacionID)) INNER JOIN Proyecto ON (Organizacion.OrganizacionID = Proyecto.OrganizacionID) AND (AdministradorProyecto.ProyectoID = Proyecto.ProyectoID) AND (ProyectoUsuarioIdentidad.ProyectoID = Proyecto.ProyectoID)) ON (PersonaOcupacionFormaSec.ProyectoID = Proyecto.ProyectoID) AND (PersonaOcupacionFormaSec.OrganizacionID = Organizacion.OrganizacionID) WHERE (((PersonaOcupacionFormaSec.OrganizacionPersonalID)=" + IBD.GuidParamValor("organizacionID") + ") AND ((AdministradorProyecto.Tipo)=@tipo))) UNION " +
                "(SELECT Organizacion.Nombre as OrganizacionNombre, Proyecto.Nombre as ProyectoNombre, Persona.Nombre as AdministradorNombre, Persona.Apellidos as AdministradorApellidos, DatosTrabajoPersonaLibre.EmailTrabajo as AdministradorEmail FROM PersonaOcupacionFormaSec INNER JOIN (DatosTrabajoPersonaLibre INNER JOIN (((Organizacion INNER JOIN ((ProyectoUsuarioIdentidad INNER JOIN Identidad ON ProyectoUsuarioIdentidad.IdentidadID = Identidad.IdentidadID) INNER JOIN (Persona INNER JOIN AdministradorProyecto ON Persona.UsuarioID = AdministradorProyecto.UsuarioID) ON ProyectoUsuarioIdentidad.UsuarioID = AdministradorProyecto.UsuarioID) ON (Organizacion.OrganizacionID = ProyectoUsuarioIdentidad.OrganizacionGnossID) AND (Organizacion.OrganizacionID = AdministradorProyecto.OrganizacionID)) INNER JOIN Proyecto ON (Organizacion.OrganizacionID = Proyecto.OrganizacionID) AND (AdministradorProyecto.ProyectoID = Proyecto.ProyectoID) AND (ProyectoUsuarioIdentidad.ProyectoID = Proyecto.ProyectoID)) INNER JOIN PerfilPersona ON (Persona.PersonaID = PerfilPersona.PersonaID) AND (Identidad.PerfilID = PerfilPersona.PerfilID)) ON DatosTrabajoPersonaLibre.PersonaID = Persona.PersonaID) ON (PersonaOcupacionFormaSec.ProyectoID = Proyecto.ProyectoID) AND (PersonaOcupacionFormaSec.OrganizacionID = Organizacion.OrganizacionID) WHERE (((PersonaOcupacionFormaSec.OrganizacionPersonalID)=" + IBD.GuidParamValor("organizacionID") + ") AND ((AdministradorProyecto.Tipo)=@tipo)))");

            this.sqlSelectExisteNombreCortoEnBD = "SELECT " + IBD.CargarGuid("OrganizacionID") + " FROM Organizacion WHERE (UPPER(NombreCorto) = UPPER(" + IBD.ToParam("nombreCorto") + "))";

            this.sqlSelectOrganizacionIDPorNombreOrg = "SELECT Organizacion.OrganizacionID FROM Organizacion WHERE UPPER(Organizacion.NombreCorto) = UPPER(" + IBD.ToParam("nombreCorto") + ")";

            this.sqlSelectObtenerOrganizacionParticipaProyDeProyecto = SelectOrganizacionParticipaProy + " FROM OrganizacionParticipaProy WHERE (OrganizacionParticipaProy.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + " )";

            this.sqlSelectTablaBaseOrganizacionIDDeOrganizacionPorID = "SELECT TablaBaseOrganizacionID FROM Organizacion WHERE OrganizacionID = " + IBD.GuidParamValor("organizacionID");

            this.sqlSelectPersonasVisiblesDeOrg = SelectPersonaVisibleEnOrg + " WHERE OrganizacionID = " + IBD.GuidParamValor("organizacionID") + " ORDER BY PersonaVisibleEnOrg.Orden";

            this.sqlSelectOrganizacionesVisiblesDePers = SelectPersonaVisibleEnOrg + " WHERE PersonaID = " + IBD.GuidParamValor("personaID");

            this.sqlSelectEsUsuarioAdministradorOrganizacion = SelectAdministradorOrganizacion + " FROM AdministradorOrganizacion WHERE (OrganizacionID = " + IBD.GuidParamValor("organizacionID") + ") AND (UsuarioID = " + IBD.GuidParamValor("usuarioID") + ")";

            #endregion

            #region DataAdapter

            #region Organizacion

            this.sqlOrganizacionInsert = IBD.ReplaceParam("INSERT INTO Organizacion (OrganizacionID, Nombre, Telefono, Email, Fax, Web, Logotipo, PaisID, ProvinciaID, Provincia, OrganizacionPadreID, Direccion, CP, Localidad, EsBuscable, EsBuscableExternos, ModoPersonal, Eliminada, NombreCorto, CoordenadasLogo,VersionLogo, Alias) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", @Nombre, @Telefono, @Email, @Fax, @Web, @Logotipo, " + IBD.GuidParamColumnaTabla("PaisID") + ", " + IBD.GuidParamColumnaTabla("ProvinciaID") + ", @Provincia, " + IBD.GuidParamColumnaTabla("OrganizacionPadreID") + ", @Direccion, @CP, @Localidad, @EsBuscable, @EsBuscableExternos, @ModoPersonal, @Eliminada, @NombreCorto, @CoordenadasLogo,@VersionLogo, @Alias)");

            this.sqlOrganizacionDelete = IBD.ReplaceParam("DELETE FROM Organizacion WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (Nombre = @O_Nombre) AND (Telefono = @O_Telefono OR @O_Telefono IS NULL AND Telefono IS NULL) AND (Email = @O_Email OR @O_Email IS NULL AND Email IS NULL) AND (Fax = @O_Fax OR @O_Fax IS NULL AND Fax IS NULL) AND (Web = @O_Web OR @O_Web IS NULL AND Web IS NULL) AND " + IBD.ComparacionCamposImagenesConOriginal("Logotipo", true) + " AND (PaisID = " + IBD.GuidParamColumnaTabla("O_PaisID") + " OR " + IBD.GuidParamColumnaTabla("O_PaisID") + " IS NULL AND PaisID IS NULL) AND (ProvinciaID = " + IBD.GuidParamColumnaTabla("O_ProvinciaID") + " OR " + IBD.GuidParamColumnaTabla("O_ProvinciaID") + " IS NULL AND ProvinciaID IS NULL) AND (Provincia = @O_Provincia OR @O_Provincia IS NULL AND Provincia IS NULL) AND (OrganizacionPadreID = " + IBD.GuidParamColumnaTabla("O_OrganizacionPadreID") + " OR " + IBD.GuidParamColumnaTabla("O_OrganizacionPadreID") + " IS NULL AND OrganizacionPadreID IS NULL) AND (Direccion = @O_Direccion OR @O_Direccion IS NULL AND Direccion IS NULL) AND (CP = @O_CP OR @O_CP IS NULL AND CP IS NULL) AND (Localidad = @O_Localidad OR @O_Localidad IS NULL AND Localidad IS NULL) AND (EsBuscable = @O_EsBuscable) AND (EsBuscableExternos = @O_EsBuscableExternos) AND (ModoPersonal = @O_ModoPersonal) AND (Eliminada = @O_Eliminada) AND (NombreCorto = @O_NombreCorto) AND (CoordenadasLogo = @O_CoordenadasLogo OR @O_CoordenadasLogo IS NULL AND CoordenadasLogo IS NULL) AND (Alias = @O_Alias OR @O_Alias IS NULL AND Alias IS NULL)");

            this.sqlOrganizacionModify = IBD.ReplaceParam("UPDATE Organizacion SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", Nombre = @Nombre, Telefono = @Telefono, Email = @Email, Fax = @Fax, Web = @Web, Logotipo = @Logotipo, PaisID = " + IBD.GuidParamColumnaTabla("PaisID") + ", ProvinciaID = " + IBD.GuidParamColumnaTabla("ProvinciaID") + ", Provincia = @Provincia, OrganizacionPadreID = " + IBD.GuidParamColumnaTabla("OrganizacionPadreID") + ", Direccion = @Direccion, CP = @CP, Localidad = @Localidad, EsBuscable = @EsBuscable, EsBuscableExternos = @EsBuscableExternos, ModoPersonal = @ModoPersonal, Eliminada = @Eliminada, NombreCorto = @NombreCorto, CoordenadasLogo = @CoordenadasLogo,VersionLogo = @VersionLogo, Alias = @Alias WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (Nombre = @O_Nombre) AND (Telefono = @O_Telefono OR @O_Telefono IS NULL AND Telefono IS NULL) AND (Email = @O_Email OR @O_Email IS NULL AND Email IS NULL) AND (Fax = @O_Fax OR @O_Fax IS NULL AND Fax IS NULL) AND (Web = @O_Web OR @O_Web IS NULL AND Web IS NULL) AND " + IBD.ComparacionCamposImagenesConOriginal("Logotipo", true) + " AND (PaisID = " + IBD.GuidParamColumnaTabla("O_PaisID") + " OR " + IBD.GuidParamColumnaTabla("O_PaisID") + " IS NULL AND PaisID IS NULL) AND (ProvinciaID = " + IBD.GuidParamColumnaTabla("O_ProvinciaID") + " OR " + IBD.GuidParamColumnaTabla("O_ProvinciaID") + " IS NULL AND ProvinciaID IS NULL) AND (Provincia = @O_Provincia OR @O_Provincia IS NULL AND Provincia IS NULL) AND (OrganizacionPadreID = " + IBD.GuidParamColumnaTabla("O_OrganizacionPadreID") + " OR " + IBD.GuidParamColumnaTabla("O_OrganizacionPadreID") + " IS NULL AND OrganizacionPadreID IS NULL) AND (Direccion = @O_Direccion OR @O_Direccion IS NULL AND Direccion IS NULL) AND (CP = @O_CP OR @O_CP IS NULL AND CP IS NULL) AND (Localidad = @O_Localidad OR @O_Localidad IS NULL AND Localidad IS NULL) AND (EsBuscable = @O_EsBuscable) AND (EsBuscableExternos = @O_EsBuscableExternos) AND (ModoPersonal = @O_ModoPersonal) AND (Eliminada = @O_Eliminada) AND (NombreCorto = @O_NombreCorto) AND (CoordenadasLogo = @O_CoordenadasLogo OR @O_CoordenadasLogo IS NULL AND CoordenadasLogo IS NULL) AND (Alias = @O_Alias OR @O_Alias IS NULL AND Alias IS NULL)");

            #endregion

            #region OrganizacionEmpresa

            this.sqlOrganizacionEmpresaInsert = IBD.ReplaceParam("INSERT INTO OrganizacionEmpresa (OrganizacionID, CIF, FechaCreacion, Empleados, TipoOrganizacion, SectorOrganizacion) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", @CIF, @FechaCreacion, @Empleados, @TipoOrganizacion, @SectorOrganizacion)");

            this.sqlOrganizacionEmpresaDelete = IBD.ReplaceParam("DELETE FROM OrganizacionEmpresa WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (CIF = @O_CIF OR @O_CIF IS NULL AND CIF IS NULL) AND (FechaCreacion = @O_FechaCreacion OR @O_FechaCreacion IS NULL AND FechaCreacion IS NULL) AND (Empleados = @O_Empleados OR @O_Empleados IS NULL AND Empleados IS NULL) AND (TipoOrganizacion = @O_TipoOrganizacion) AND (SectorOrganizacion = @O_SectorOrganizacion)");

            this.sqlOrganizacionEmpresaModify = IBD.ReplaceParam("UPDATE OrganizacionEmpresa SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", CIF = @CIF, FechaCreacion = @FechaCreacion, Empleados = @Empleados, TipoOrganizacion = @TipoOrganizacion, SectorOrganizacion = @SectorOrganizacion WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ")");

            #endregion

            #region OrganizacionClase

            this.sqlOrganizacionClaseInsert = IBD.ReplaceParam("INSERT INTO OrganizacionClase (OrganizacionID, Centro, Asignatura, Curso, Grupo, CursoAcademico, NombreCortoCentro, NombreCortoAsig, TipoClase) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", @Centro, @Asignatura, @Curso, @Grupo, @CursoAcademico, @NombreCortoCentro, @NombreCortoAsig, @TipoClase)");

            this.sqlOrganizacionClaseDelete = IBD.ReplaceParam("DELETE FROM OrganizacionClase WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (Centro = @O_Centro) AND (Asignatura = @O_Asignatura) AND (Curso = @O_Curso) AND (Grupo = @O_Grupo OR Grupo IS NULL AND @O_Grupo IS NULL) AND (CursoAcademico = @O_CursoAcademico) AND (NombreCortoCentro = @O_NombreCortoCentro) AND (NombreCortoAsig = @O_NombreCortoAsig) AND (TipoClase = @O_TipoClase)");

            this.sqlOrganizacionClaseModify = IBD.ReplaceParam("UPDATE OrganizacionClase SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", Centro = @Centro, Asignatura = @Asignatura, Curso = @Curso, Grupo = @Grupo, CursoAcademico = @CursoAcademico, NombreCortoCentro = @NombreCortoCentro, NombreCortoAsig = @NombreCortoAsig, TipoClase = @TipoClase WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (Centro = @O_Centro OR @O_Centro IS NULL AND Centro IS NULL) AND (Asignatura = @O_Asignatura) AND (Curso = @O_Curso) AND (Grupo = @O_Grupo OR Grupo IS NULL AND @O_Grupo IS NULL) AND (CursoAcademico = @O_CursoAcademico) AND (NombreCortoCentro = @O_NombreCortoCentro) AND (NombreCortoAsig = @O_NombreCortoAsig) AND (TipoClase = @O_TipoClase)");

            #endregion

            #region OrganizacionParticipaProy

            this.sqlOrganizacionParticipaProyInsert = IBD.ReplaceParam("INSERT INTO OrganizacionParticipaProy (OrganizacionID, OrganizacionProyectoID, ProyectoID, FechaInicio, IdentidadID, EstaBloqueada, RegistroAutomatico) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("OrganizacionProyectoID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", @FechaInicio, " + IBD.GuidParamColumnaTabla("IdentidadID") + " , @EstaBloqueada, @RegistroAutomatico)");

            this.sqlOrganizacionParticipaProyDelete = IBD.ReplaceParam("DELETE FROM OrganizacionParticipaProy WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (OrganizacionProyectoID = " + IBD.GuidParamColumnaTabla("O_OrganizacionProyectoID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ") AND (FechaInicio = @O_FechaInicio) AND (IdentidadID = " + IBD.GuidParamColumnaTabla("O_IdentidadID") + ") AND (EstaBloqueada = @O_EstaBloqueada) AND (RegistroAutomatico = @O_RegistroAutomatico)");

            this.sqlOrganizacionParticipaProyModify = IBD.ReplaceParam("UPDATE OrganizacionParticipaProy SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", OrganizacionProyectoID = " + IBD.GuidParamColumnaTabla("OrganizacionProyectoID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", FechaInicio = @FechaInicio, IdentidadID = " + IBD.GuidParamColumnaTabla("IdentidadID") + ", EstaBloqueada = @EstaBloqueada, RegistroAutomatico = @RegistroAutomatico WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (OrganizacionProyectoID = " + IBD.GuidParamColumnaTabla("O_OrganizacionProyectoID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ") AND (FechaInicio = @O_FechaInicio) AND (IdentidadID = " + IBD.GuidParamColumnaTabla("O_IdentidadID") + ") AND (EstaBloqueada = @O_EstaBloqueada) AND (RegistroAutomatico = @O_RegistroAutomatico)");
            #endregion

            #region TagOrganizacion

            this.sqlTagOrganizacionInsert = IBD.ReplaceParam("INSERT INTO TagOrganizacion (TagID, Tipo, OrganizacionID) VALUES (@TagID, @Tipo, " + IBD.GuidParamColumnaTabla("OrganizacionID") + ")");

            this.sqlTagOrganizacionDelete = IBD.ReplaceParam("DELETE FROM TagOrganizacion WHERE (TagID = @O_TagID) AND (Tipo = @O_Tipo) AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ")");

            this.sqlTagOrganizacionModify = IBD.ReplaceParam("UPDATE TagOrganizacion SET TagID = @TagID, Tipo = @Tipo, OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + " WHERE (TagID = @O_TagID) AND (Tipo = @O_Tipo) AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ")");

            #endregion

            #region AdministradorOrganizacion

            this.sqlAdministradorOrganizacionInsert = IBD.ReplaceParam("INSERT INTO AdministradorOrganizacion (UsuarioID, OrganizacionID,Tipo) VALUES (" + IBD.GuidParamColumnaTabla("UsuarioID") + ", " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", @Tipo)");

            this.sqlAdministradorOrganizacionDelete = IBD.ReplaceParam("DELETE FROM AdministradorOrganizacion WHERE (UsuarioID = " + IBD.GuidParamColumnaTabla("O_UsuarioID") + ") AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND Tipo = @O_Tipo");

            this.sqlAdministradorOrganizacionModify = IBD.ReplaceParam("UPDATE AdministradorOrganizacion SET UsuarioID = " + IBD.GuidParamColumnaTabla("UsuarioID") + ", OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", Tipo = @Tipo WHERE (UsuarioID = " + IBD.GuidParamColumnaTabla("O_UsuarioID") + ") AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND Tipo = @O_Tipo");

            #endregion

            #region PersonaVinculoOrganizacion

            this.sqlPersonaVinculoOrganizacionInsert = IBD.ReplaceParam("INSERT INTO PersonaVinculoOrganizacion (PersonaID, OrganizacionID, Cargo, PaisTrabajoID, ProvinciaTrabajoID, ProvinciaTrabajo, CPTrabajo, DireccionTrabajo, LocalidadTrabajo, TelefonoTrabajo, Extension, TelefonoMovilTrabajo, EmailTrabajo, CategoriaProfesionalID, TipoContratoID, FechaVinculacion , Foto, CoordenadasFoto, UsarFotoPersonal, VersionFoto, FechaAnadidaFoto) VALUES (" + IBD.GuidParamColumnaTabla("PersonaID") + ", " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", @Cargo, " + IBD.GuidParamColumnaTabla("PaisTrabajoID") + ", " + IBD.GuidParamColumnaTabla("ProvinciaTrabajoID") + ", @ProvinciaTrabajo, @CPTrabajo, @DireccionTrabajo, @LocalidadTrabajo, @TelefonoTrabajo, @Extension , @TelefonoMovilTrabajo, @EmailTrabajo, " + IBD.GuidParamColumnaTabla("CategoriaProfesionalID") + ", " + IBD.GuidParamColumnaTabla("TipoContratoID") + ", @FechaVinculacion, @Foto, @CoordenadasFoto, @UsarFotoPersonal, @VersionFoto, @FechaAnadidaFoto)");

            this.sqlPersonaVinculoOrganizacionDelete = IBD.ReplaceParam("DELETE FROM PersonaVinculoOrganizacion WHERE (PersonaID = " + IBD.GuidParamColumnaTabla("O_PersonaID") + ") AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID")) + ")";// AND (Cargo = @O_Cargo OR @O_Cargo IS NULL AND Cargo IS NULL) AND (PaisTrabajoID = " + IBD.GuidParamColumnaTabla("O_PaisTrabajoID") + " OR " + IBD.GuidParamColumnaTabla("O_PaisTrabajoID") + " IS NULL AND PaisTrabajoID IS NULL) AND (ProvinciaTrabajoID = " + IBD.GuidParamColumnaTabla("O_ProvinciaTrabajoID") + " OR " + IBD.GuidParamColumnaTabla("O_ProvinciaTrabajoID") + " IS NULL AND ProvinciaTrabajoID IS NULL) AND (ProvinciaTrabajo = @O_ProvinciaTrabajo OR @O_ProvinciaTrabajo IS NULL AND ProvinciaTrabajo IS NULL) AND (CPTrabajo = @O_CPTrabajo OR @O_CPTrabajo IS NULL AND CPTrabajo IS NULL) AND (DireccionTrabajo = @O_DireccionTrabajo OR @O_DireccionTrabajo IS NULL AND DireccionTrabajo IS NULL) AND (LocalidadTrabajo = @O_LocalidadTrabajo OR @O_LocalidadTrabajo IS NULL AND LocalidadTrabajo IS NULL) AND (TelefonoTrabajo = @O_TelefonoTrabajo OR @O_TelefonoTrabajo IS NULL AND TelefonoTrabajo IS NULL) AND (Extension = @O_Extension OR @O_Extension IS NULL AND Extension IS NULL) AND (TelefonoMovilTrabajo = @O_TelefonoMovilTrabajo OR @O_TelefonoMovilTrabajo IS NULL AND TelefonoMovilTrabajo IS NULL) AND (EmailTrabajo = @O_EmailTrabajo OR @O_EmailTrabajo IS NULL AND EmailTrabajo IS NULL) AND (CategoriaProfesionalID = " + IBD.GuidParamColumnaTabla("O_CategoriaProfesionalID") + " OR " + IBD.GuidParamColumnaTabla("O_CategoriaProfesionalID") + " IS NULL AND CategoriaProfesionalID IS NULL) AND (TipoContratoID = " + IBD.GuidParamColumnaTabla("O_TipoContratoID") + " OR " + IBD.GuidParamColumnaTabla("O_TipoContratoID") + " IS NULL AND TipoContratoID IS NULL) AND (FechaVinculacion = @O_FechaVinculacion) AND " + IBD.ComparacionCamposImagenesConOriginal("Foto", true) + " AND (CoordenadasFoto = @O_CoordenadasFoto OR @O_CoordenadasFoto IS NULL AND CoordenadasFoto IS NULL) AND (UsarFotoPersonal = @O_UsarFotoPersonal)");

            this.sqlPersonaVinculoOrganizacionModify = IBD.ReplaceParam("UPDATE PersonaVinculoOrganizacion SET PersonaID = " + IBD.GuidParamColumnaTabla("PersonaID") + ", OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", Cargo = @Cargo, PaisTrabajoID = " + IBD.GuidParamColumnaTabla("PaisTrabajoID") + ", ProvinciaTrabajoID = " + IBD.GuidParamColumnaTabla("ProvinciaTrabajoID") + ", ProvinciaTrabajo = @ProvinciaTrabajo, CPTrabajo = @CPTrabajo, DireccionTrabajo = @DireccionTrabajo, LocalidadTrabajo = @LocalidadTrabajo, TelefonoTrabajo = @TelefonoTrabajo, Extension = @Extension, TelefonoMovilTrabajo = @TelefonoMovilTrabajo, EmailTrabajo = @EmailTrabajo, CategoriaProfesionalID = " + IBD.GuidParamColumnaTabla("CategoriaProfesionalID") + ", TipoContratoID = " + IBD.GuidParamColumnaTabla("TipoContratoID") + ", FechaVinculacion = @FechaVinculacion, Foto = @Foto, CoordenadasFoto = @CoordenadasFoto, UsarFotoPersonal = @UsarFotoPersonal, VersionFoto = @VersionFoto, FechaAnadidaFoto = @FechaAnadidaFoto WHERE (PersonaID = " + IBD.GuidParamColumnaTabla("O_PersonaID") + ") AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ")");

            #endregion

            #region PersonaVisibleEnOrg

            this.sqlPersonaVisibleEnOrgInsert = IBD.ReplaceParam("INSERT INTO PersonaVisibleEnOrg (PersonaID, OrganizacionID, Orden) VALUES (" + IBD.GuidParamColumnaTabla("PersonaID") + ", " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", @Orden)");

            this.sqlPersonaVisibleEnOrgDelete = IBD.ReplaceParam("DELETE FROM PersonaVisibleEnOrg WHERE (PersonaID = " + IBD.GuidParamColumnaTabla("O_PersonaID") + ") AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (Orden = @O_Orden) ");
            this.sqlPersonaVisibleEnOrgModify = IBD.ReplaceParam("UPDATE PersonaVisibleEnOrg SET PersonaID = " + IBD.GuidParamColumnaTabla("PersonaID") + ", OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", Orden = @Orden WHERE (PersonaID = " + IBD.GuidParamColumnaTabla("O_PersonaID") + ") AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (Orden = @O_Orden) ");

            #endregion

            #region EmpleadoHistoricoEstadoLaboral

            this.sqlEmpleadoHistoricoEstadoLaboralInsert = IBD.ReplaceParam("INSERT INTO EmpleadoHistoricoEstadoLaboral (PersonaID, OrganizacionID, EstadoLaboralID, Fecha, Causa) VALUES (" + IBD.GuidParamColumnaTabla("PersonaID") + ", " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("EstadoLaboralID") + ", @Fecha, @Causa)");

            this.sqlEmpleadoHistoricoEstadoLaboralDelete = IBD.ReplaceParam("DELETE FROM EmpleadoHistoricoEstadoLaboral WHERE (PersonaID = " + IBD.GuidParamColumnaTabla("O_PersonaID") + ") AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (EstadoLaboralID = " + IBD.GuidParamColumnaTabla("O_EstadoLaboralID") + ") AND (Fecha = @O_Fecha) AND (Causa = @O_Causa OR @O_Causa IS NULL AND Causa IS NULL)");

            this.sqlEmpleadoHistoricoEstadoLaboralModify = IBD.ReplaceParam("UPDATE EmpleadoHistoricoEstadoLaboral SET PersonaID = " + IBD.GuidParamColumnaTabla("PersonaID") + ", OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", EstadoLaboralID = " + IBD.GuidParamColumnaTabla("EstadoLaboralID") + ", Fecha = @Fecha, Causa = @Causa WHERE (PersonaID = " + IBD.GuidParamColumnaTabla("O_PersonaID") + ") AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (EstadoLaboralID = " + IBD.GuidParamColumnaTabla("O_EstadoLaboralID") + ") AND (Fecha = @O_Fecha) AND (Causa = @O_Causa OR @O_Causa IS NULL AND Causa IS NULL)");

            #endregion

            #region HistoricoOrgParticipaProy

            this.sqlHistoricoOrganizacionParticipaProyInsert = IBD.ReplaceParam("INSERT INTO HistoricoOrgParticipaProy (OrganizacionID, OrganizacionProyectoID, ProyectoID) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("OrganizacionProyectoID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ")");

            this.sqlHistoricoOrganizacionParticipaProyDelete = IBD.ReplaceParam("DELETE FROM HistoricoOrgParticipaProy WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (OrganizacionProyectoID = " + IBD.GuidParamColumnaTabla("O_OrganizacionProyectoID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ")");

            this.sqlHistoricoOrganizacionParticipaProyModify = IBD.ReplaceParam("UPDATE HistoricoOrgParticipaProy SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", OrganizacionProyectoID = " + IBD.GuidParamColumnaTabla("OrganizacionProyectoID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + " WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (OrganizacionProyectoID = " + IBD.GuidParamColumnaTabla("O_OrganizacionProyectoID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ")");

            #endregion

            #region OrganizacionGnoss

            this.sqlOrganizacionGnossInsert = IBD.ReplaceParam("INSERT INTO OrganizacionGnoss (OrganizacionID) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ")");

            this.sqlOrganizacionGnossDelete = IBD.ReplaceParam("DELETE FROM OrganizacionGnoss WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ")");

            this.sqlOrganizacionGnossModify = IBD.ReplaceParam("UPDATE OrganizacionGnoss SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + " WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ")");

            #endregion

            #region Sede

            this.sqlSedeInsert = IBD.ReplaceParam("INSERT INTO Sede (SedeID, OrganizacionID, Telefono, Fax, PaisID, ProvinciaID, Provincia, Localidad, CP, Direccion) VALUES (" + IBD.GuidParamColumnaTabla("SedeID") + ", " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", @Telefono, @Fax, " + IBD.GuidParamColumnaTabla("PaisID") + ", " + IBD.GuidParamColumnaTabla("ProvinciaID") + ", @Provincia, @Localidad, @CP, @Direccion)");

            this.sqlSedeDelete = IBD.ReplaceParam("DELETE FROM Sede WHERE (SedeID = " + IBD.GuidParamColumnaTabla("O_SedeID") + ") AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (Telefono = @O_Telefono OR @O_Telefono IS NULL AND Telefono IS NULL) AND (Fax = @O_Fax OR @O_Fax IS NULL AND Fax IS NULL) AND (PaisID = " + IBD.GuidParamColumnaTabla("O_PaisID") + " OR " + IBD.GuidParamColumnaTabla("O_PaisID") + " IS NULL AND PaisID IS NULL) AND (ProvinciaID = " + IBD.GuidParamColumnaTabla("O_ProvinciaID") + " OR " + IBD.GuidParamColumnaTabla("O_ProvinciaID") + " IS NULL AND ProvinciaID IS NULL) AND (Provincia = @O_Provincia OR @O_Provincia IS NULL AND Provincia IS NULL) AND (Localidad = @O_Localidad OR @O_Localidad IS NULL AND Localidad IS NULL) AND (CP = @O_CP OR @O_CP IS NULL AND CP IS NULL) AND (Direccion = @O_Direccion OR @O_Direccion IS NULL AND Direccion IS NULL)");

            this.sqlSedeModify = IBD.ReplaceParam("UPDATE Sede SET SedeID = " + IBD.GuidParamColumnaTabla("SedeID") + ", OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", Telefono = @Telefono, Fax = @Fax, PaisID = " + IBD.GuidParamColumnaTabla("PaisID") + ", ProvinciaID = " + IBD.GuidParamColumnaTabla("ProvinciaID") + ", Provincia = @Provincia, Localidad = @Localidad, CP = @CP, Direccion = @Direccion WHERE (SedeID = " + IBD.GuidParamColumnaTabla("O_SedeID") + ") AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (Telefono = @O_Telefono OR @O_Telefono IS NULL AND Telefono IS NULL) AND (Fax = @O_Fax OR @O_Fax IS NULL AND Fax IS NULL) AND (PaisID = " + IBD.GuidParamColumnaTabla("O_PaisID") + " OR " + IBD.GuidParamColumnaTabla("O_PaisID") + " IS NULL AND PaisID IS NULL) AND (ProvinciaID = " + IBD.GuidParamColumnaTabla("O_ProvinciaID") + " OR " + IBD.GuidParamColumnaTabla("O_ProvinciaID") + " IS NULL AND ProvinciaID IS NULL) AND (Provincia = @O_Provincia OR @O_Provincia IS NULL AND Provincia IS NULL) AND (Localidad = @O_Localidad OR @O_Localidad IS NULL AND Localidad IS NULL) AND (CP = @O_CP OR @O_CP IS NULL AND CP IS NULL) AND (Direccion = @O_Direccion OR @O_Direccion IS NULL AND Direccion IS NULL)");

            #endregion

            #region CnaeAgregacionOrganizacion

            this.sqlCnaeAgregacionOrganizacionInsert = IBD.ReplaceParam("INSERT INTO CnaeAgregacionOrganizacion (OrganizacionID, CnaeID) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", @CnaeID)");

            this.sqlCnaeAgregacionOrganizacionDelete = IBD.ReplaceParam("DELETE FROM CnaeAgregacionOrganizacion WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (CnaeID = @O_CnaeID)");

            this.sqlCnaeAgregacionOrganizacionModify = IBD.ReplaceParam("UPDATE CnaeAgregacionOrganizacion SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", CnaeID = @CnaeID WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (CnaeID = @O_CnaeID)");

            #endregion

            #region ConfiguracionGnossOrg

            this.sqlConfiguracionGnossOrgInsert = IBD.ReplaceParam("INSERT INTO ConfiguracionGnossOrg (OrganizacionID, VerRecursos, VerRecursosExterno, VisibilidadContactos) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", @VerRecursos, @VerRecursosExterno ,@VisibilidadContactos )");

            this.sqlConfiguracionGnossOrgDelete = IBD.ReplaceParam("DELETE FROM ConfiguracionGnossOrg WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + " AND (VerRecursos = @O_VerRecursos) AND (VerRecursosExterno = @O_VerRecursosExterno)AND (VisibilidadContactos = @O_VisibilidadContactos))");

            this.sqlConfiguracionGnossOrgModify = IBD.ReplaceParam("UPDATE ConfiguracionGnossOrg SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", VerRecursosExterno = @VerRecursosExterno, VerRecursos = @VerRecursos, VisibilidadContactos = @VisibilidadContactos WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (VerRecursos = @O_VerRecursos) AND (VerRecursosExterno = @O_VerRecursosExterno)AND (VisibilidadContactos = @O_VisibilidadContactos)");

            #endregion

            #endregion
        }

        /// <summary>
        /// Actualiza el nombre cambiado de una organización en los perfiles de sus usuarios
        /// </summary>
        /// <param name="pFilaOrganizacion">Fila de la organización de la cual se ha cambiado el nombre</param>
        //private void ActualizarNombreOrganizacionCambiado(OrganizacionDS.OrganizacionRow pFilaOrganizacion)
        //{
        //    //TODO: Migrar a EF
        //    //Tengo que actualizar en IdentidadDS / Perfil / "NombreOrganizacion"
        //    IdentidadAD identidadAD = new IdentidadAD();
        //    identidadAD.ActualizarCambioNombreOrganizacion(pFilaOrganizacion.OrganizacionID, pFilaOrganizacion.Nombre,pFilaOrganizacion.Alias);
        //    identidadAD.Dispose();
        //}

        #endregion

        #endregion
    }
}
