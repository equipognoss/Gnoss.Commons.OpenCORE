namespace Es.Riam.Gnoss.Util.General
{
    public class DatosError
    {
        public string NameSpace { get; set; }
        public string Name { get; set; }
        public string Source { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public DatosError InnerException { get; set; }
    }
}
