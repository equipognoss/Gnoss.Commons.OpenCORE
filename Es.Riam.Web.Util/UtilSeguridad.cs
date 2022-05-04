namespace Es.Riam.Gnoss.Web.Util
{
    /// <summary>
    /// Métodos de utilidad para manejar controles
    /// </summary>
    public class UtilSeguridad
    {
        public static string AsegurarParametroSqlLike(string pInputSql)
        {
            string s = pInputSql;
            s = s.Replace("'", "''");
            s = s.Replace("\"\"", "");
            s = s.Replace("--", "");
            s = s.Replace("[", "[[]");
            s = s.Replace("%", "[%]");
            s = s.Replace("_", "[_]");

            return s;
        }

        public static string AsegurarParametroSql(string pInputSql)
        {
            string s = pInputSql;
            s = s.Replace("'", "''");
            s = s.Replace("\"\"", "");
            s = s.Replace("--", "");
            return s;
        }

    }
}
