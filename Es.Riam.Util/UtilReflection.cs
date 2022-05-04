namespace Es.Riam.Util
{
    public class UtilReflection
    {
        public static object GetValueReflection(object src, string propName)
        {
            return src.GetType().GetProperty(propName)?.GetValue(src, null);
        }

        public static void SetValueReflection(object src, string propName, object newValue)
        {
            src.GetType().GetProperty(propName)?.SetValue(src, newValue, null);
        }

        public static bool ContainsValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName) != null;
        }
    }
}
