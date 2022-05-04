using Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.AD.EncapsuladoDatos
{
    [Serializable]
    public class DataWrapperDocumentacion : DataWrapperBase
    {
        public List<BaseRecursos> ListaBaseRecursos;
        public List<BaseRecursosProyecto> ListaBaseRecursosProyecto;
        public List<ColaDocumento> ListaColaDocumento;
        public List<Documento> ListaDocumento;
        public List<DocumentoRolGrupoIdentidades> ListaDocumentoRolGrupoIdentidades;
        public List<DocumentoRolIdentidad> ListaDocumentoRolIdentidad;
        public List<DocumentoWebAgCatTesauro> ListaDocumentoWebAgCatTesauro;
        public List<DocumentoWebVinBaseRecursos> ListaDocumentoWebVinBaseRecursos;
        public List<DocumentoWebVinBaseRecursosExtra> ListaDocumentoWebVinBaseRecursosExtra;
        public List<HistorialDocumento> ListaHistorialDocumento;
        public List<VersionDocumento> ListaVersionDocumento;
        public List<DocumentoVincDoc> ListaDocumentoVincDoc;
        public List<BaseRecursosUsuario> ListaBaseRecursosUsuario;
        public List<BaseRecursosOrganizacion> ListaBaseRecursosOrganizacion;
        public List<VotoDocumento> ListaVotoDocumento;
        public List<DocumentoConProyNombreCortoProyRelacionadoID> ListaDocumentoConProyNombreCorto;
        public List<NombrePerfil> ListaNombrePerfil;
        public List<NombreGrupo> ListaNombreGrupo;
        public List<NombreGrupoOrg> ListaNombreGrupoOrg;
        public List<DocumentoAtributoBiblio> ListaDocumentoAtributoBiblio;
        public List<DocumentoComentario> ListaDocumentoComentario;
        public List<DocumentoTokenBrightcove> ListaDocumentoTokenBrightcove;
        public List<DocumentoTokenTOP> ListaDocumentoTokenTOP;
        public List<DocumentoEnvioNewsLetter> ListaDocumentoEnvioNewsLetter;
        public List<DocumentoNewsletter> ListaDocumentoNewsLetter;
        public List<DocumentoGrupoUsuario> ListaDocumentoGrupoUsuario;
        public List<DocumentoTipologia> ListaDocumentoTipologia;
        public List<DocumentoRespuesta> ListaDocumentoRespuesta;
        public List<DocumentoRespuestaVoto> ListaDocumentoRespuestaVoto;
        public List<FichaBibliografica> ListaFichaBibliografica;
        public List<AtributoFichaBibliografica> ListaAtributoFichaBibliografica;
        public List<DocumentoEnEdicion> ListaDocumentoEnEdicion;
        public List<ColaCargaRecursos> ListaColaCargarRecursos;
        public List<DocumentoUrlCanonica> ListaDocumentoUrlCanonica;

        public DataWrapperDocumentacion()
        {
            ListaBaseRecursos = new List<BaseRecursos>();
            ListaBaseRecursosProyecto = new List<BaseRecursosProyecto>();
            ListaColaDocumento = new List<ColaDocumento>();
            ListaDocumento = new List<Documento>();
            ListaDocumentoRolGrupoIdentidades = new List<DocumentoRolGrupoIdentidades>();
            ListaDocumentoRolIdentidad = new List<DocumentoRolIdentidad>();
            ListaDocumentoWebAgCatTesauro = new List<DocumentoWebAgCatTesauro>();
            ListaDocumentoWebVinBaseRecursos = new List<DocumentoWebVinBaseRecursos>();
            ListaDocumentoWebVinBaseRecursosExtra = new List<DocumentoWebVinBaseRecursosExtra>();
            ListaHistorialDocumento = new List<HistorialDocumento>();
            ListaVersionDocumento = new List<VersionDocumento>();
            ListaDocumentoVincDoc = new List<DocumentoVincDoc>();
            ListaBaseRecursosUsuario = new List<BaseRecursosUsuario>();
            ListaBaseRecursosOrganizacion = new List<BaseRecursosOrganizacion>();
            ListaVotoDocumento = new List<VotoDocumento>();
            ListaDocumentoConProyNombreCorto = new List<DocumentoConProyNombreCortoProyRelacionadoID>();
            ListaNombrePerfil = new List<NombrePerfil>();
            ListaNombreGrupo = new List<NombreGrupo>();
            ListaNombreGrupoOrg = new List<NombreGrupoOrg>();
            ListaDocumentoAtributoBiblio = new List<DocumentoAtributoBiblio>();
            ListaDocumentoComentario = new List<DocumentoComentario>();
            ListaDocumentoTokenBrightcove = new List<DocumentoTokenBrightcove>();
            ListaDocumentoTokenTOP = new List<DocumentoTokenTOP>();
            ListaDocumentoEnvioNewsLetter = new List<DocumentoEnvioNewsLetter>();
            ListaDocumentoNewsLetter = new List<DocumentoNewsletter>();
            ListaDocumentoGrupoUsuario = new List<DocumentoGrupoUsuario>();
            ListaDocumentoTipologia = new List<DocumentoTipologia>();
            ListaDocumentoRespuesta = new List<DocumentoRespuesta>();
            ListaDocumentoRespuestaVoto = new List<DocumentoRespuestaVoto>();
            ListaFichaBibliografica = new List<FichaBibliografica>();
            ListaAtributoFichaBibliografica = new List<AtributoFichaBibliografica>();
            ListaDocumentoEnEdicion = new List<DocumentoEnEdicion>();
            ListaColaCargarRecursos = new List<ColaCargaRecursos>();
            ListaDocumentoUrlCanonica = new List<DocumentoUrlCanonica>();
        }

        public override void Merge(DataWrapperBase pDataWrapper)
        {
            DataWrapperDocumentacion dataWrapperDocumentacion = (DataWrapperDocumentacion)pDataWrapper;
            ListaBaseRecursos = ListaBaseRecursos.Union(dataWrapperDocumentacion.ListaBaseRecursos).ToList();
            ListaBaseRecursosProyecto = ListaBaseRecursosProyecto.Union(dataWrapperDocumentacion.ListaBaseRecursosProyecto).ToList();
            ListaColaDocumento = ListaColaDocumento.Union(dataWrapperDocumentacion.ListaColaDocumento).ToList();
            ListaDocumento = ListaDocumento.Union(dataWrapperDocumentacion.ListaDocumento).ToList();
            ListaDocumentoRolGrupoIdentidades = ListaDocumentoRolGrupoIdentidades.Union(dataWrapperDocumentacion.ListaDocumentoRolGrupoIdentidades).ToList();
            ListaDocumentoRolIdentidad = ListaDocumentoRolIdentidad.Union(dataWrapperDocumentacion.ListaDocumentoRolIdentidad).ToList();
            ListaDocumentoWebAgCatTesauro = ListaDocumentoWebAgCatTesauro.Union(dataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro).ToList();
            ListaDocumentoWebVinBaseRecursos = ListaDocumentoWebVinBaseRecursos.Union(dataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos).ToList();
            ListaDocumentoWebVinBaseRecursosExtra = ListaDocumentoWebVinBaseRecursosExtra.Union(dataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursosExtra).ToList();
            ListaHistorialDocumento = ListaHistorialDocumento.Union(dataWrapperDocumentacion.ListaHistorialDocumento).ToList();
            ListaVersionDocumento = ListaVersionDocumento.Union(dataWrapperDocumentacion.ListaVersionDocumento).ToList();
            ListaDocumentoVincDoc = ListaDocumentoVincDoc.Union(dataWrapperDocumentacion.ListaDocumentoVincDoc).ToList();
            ListaBaseRecursosUsuario = ListaBaseRecursosUsuario.Union(dataWrapperDocumentacion.ListaBaseRecursosUsuario).ToList();
            ListaBaseRecursosOrganizacion = ListaBaseRecursosOrganizacion.Union(dataWrapperDocumentacion.ListaBaseRecursosOrganizacion).ToList();
            ListaVotoDocumento = ListaVotoDocumento.Union(dataWrapperDocumentacion.ListaVotoDocumento).ToList();
            ListaDocumentoConProyNombreCorto = ListaDocumentoConProyNombreCorto.Union(dataWrapperDocumentacion.ListaDocumentoConProyNombreCorto).ToList();
            ListaNombrePerfil = ListaNombrePerfil.Union(dataWrapperDocumentacion.ListaNombrePerfil).ToList();
            ListaNombreGrupo = ListaNombreGrupo.Union(dataWrapperDocumentacion.ListaNombreGrupo).ToList();
            ListaNombreGrupoOrg = ListaNombreGrupoOrg.Union(dataWrapperDocumentacion.ListaNombreGrupoOrg).ToList();
            ListaDocumentoAtributoBiblio = ListaDocumentoAtributoBiblio.Union(dataWrapperDocumentacion.ListaDocumentoAtributoBiblio).ToList();
            ListaDocumentoComentario = ListaDocumentoComentario.Union(dataWrapperDocumentacion.ListaDocumentoComentario).ToList();
            ListaDocumentoTokenBrightcove = ListaDocumentoTokenBrightcove.Union(dataWrapperDocumentacion.ListaDocumentoTokenBrightcove).ToList();
            ListaDocumentoTokenTOP = ListaDocumentoTokenTOP.Union(dataWrapperDocumentacion.ListaDocumentoTokenTOP).ToList();
            ListaDocumentoEnvioNewsLetter = ListaDocumentoEnvioNewsLetter.Union(dataWrapperDocumentacion.ListaDocumentoEnvioNewsLetter).ToList();
            ListaDocumentoNewsLetter = ListaDocumentoNewsLetter.Union(dataWrapperDocumentacion.ListaDocumentoNewsLetter).ToList();
            ListaDocumentoGrupoUsuario = ListaDocumentoGrupoUsuario.Union(dataWrapperDocumentacion.ListaDocumentoGrupoUsuario).ToList();
            ListaDocumentoTipologia = ListaDocumentoTipologia.Union(dataWrapperDocumentacion.ListaDocumentoTipologia).ToList();
            ListaDocumentoRespuesta = ListaDocumentoRespuesta.Union(dataWrapperDocumentacion.ListaDocumentoRespuesta).ToList();
            ListaDocumentoRespuestaVoto = ListaDocumentoRespuestaVoto.Union(dataWrapperDocumentacion.ListaDocumentoRespuestaVoto).ToList();
            ListaFichaBibliografica = ListaFichaBibliografica.Union(dataWrapperDocumentacion.ListaFichaBibliografica).ToList();
            ListaAtributoFichaBibliografica = ListaAtributoFichaBibliografica.Union(dataWrapperDocumentacion.ListaAtributoFichaBibliografica).ToList();
            ListaDocumentoEnEdicion = ListaDocumentoEnEdicion.Union(dataWrapperDocumentacion.ListaDocumentoEnEdicion).ToList();
            ListaColaCargarRecursos = ListaColaCargarRecursos.Union(dataWrapperDocumentacion.ListaColaCargarRecursos).ToList();
            ListaDocumentoUrlCanonica = ListaDocumentoUrlCanonica.Union(dataWrapperDocumentacion.ListaDocumentoUrlCanonica).ToList();
        }

        public void CargaRelacionesPerezosasCache()
        {
            foreach(BaseRecursosOrganizacion baseRecursosOrganizacion in ListaBaseRecursosOrganizacion)
            {
                baseRecursosOrganizacion.BaseRecursos = ListaBaseRecursos.FirstOrDefault(baseRec => baseRec.BaseRecursosID.Equals(baseRecursosOrganizacion.BaseRecursosID));
            }

            foreach(BaseRecursosProyecto baseRecursosProyecto in ListaBaseRecursosProyecto)
            {
                baseRecursosProyecto.BaseRecursos = ListaBaseRecursos.FirstOrDefault(baseRec => baseRec.BaseRecursosID.Equals(baseRecursosProyecto.BaseRecursosID));
            }

            foreach (BaseRecursosUsuario baseRecursosUsuario in ListaBaseRecursosUsuario)
            {
                baseRecursosUsuario.BaseRecursos = ListaBaseRecursos.FirstOrDefault(baseRec => baseRec.BaseRecursosID.Equals(baseRecursosUsuario.BaseRecursosID));
            }

            foreach(Documento documento in ListaDocumento)
            {
                documento.VersionDocumento = ListaVersionDocumento.FirstOrDefault(version => version.DocumentoID.Equals(documento.DocumentoID));
                documento.DocumentoWebVinBaseRecursos = ListaDocumentoWebVinBaseRecursos.Where(docWebVin => docWebVin.DocumentoID.Equals(documento.DocumentoID)).ToList();
                documento.VotoDocumento = ListaVotoDocumento.Where(voto => voto.DocumentoID.Equals(documento.DocumentoID)).ToList();
                documento.DocumentoRespuestaVoto = ListaDocumentoRespuestaVoto.Where(respuesta => respuesta.DocumentoID.Equals(documento.DocumentoID)).ToList();
            }

            foreach (DocumentoWebVinBaseRecursos documentoWebVinBaseRecursos in ListaDocumentoWebVinBaseRecursos)
            {
                documentoWebVinBaseRecursos.BaseRecursos = ListaBaseRecursos.FirstOrDefault(baseRec => baseRec.BaseRecursosID.Equals(documentoWebVinBaseRecursos.BaseRecursosID));
                documentoWebVinBaseRecursos.Documento = ListaDocumento.FirstOrDefault(doc => doc.DocumentoID.Equals(documentoWebVinBaseRecursos.DocumentoID));
            }

            foreach(VersionDocumento versionDoc in ListaVersionDocumento)
            {
                versionDoc.Documento = ListaDocumento.FirstOrDefault(doc => doc.DocumentoID.Equals(doc.DocumentoID));
            }

        }
    }
}
