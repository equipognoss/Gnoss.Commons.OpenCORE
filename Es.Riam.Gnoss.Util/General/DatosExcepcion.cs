namespace Es.Riam.Gnoss.Util.General
{
    public class DatosExcepcion
    {
        public DatosExcepcion()
        {
            TipoExcepcion = "";
            MensajeExcepcion = "";
            StackExcepcion = "";
            DatosEx = null;
        }
        public string TipoExcepcion { get; set; }
        public string MensajeExcepcion { get; set; }
        public string StackExcepcion { get; set; }
        public DatosExcepcion DatosEx { get; set; }
    }
}
