internal static class TestHelpers
{
    public static void SetPrivateProperty<T>(this T obj, string propertyName, object value)
    {
        var property = typeof(T).GetProperty(propertyName,
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.NonPublic);

        if (property != null && property.CanWrite)
        {
            property.SetValue(obj, value);
        }
    }
}