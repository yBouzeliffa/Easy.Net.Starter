using System.ComponentModel;

namespace Easy.Net.Starter.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription<T>(this T value) where T : IConvertible
        {
            var fi = value.GetType().GetField(value.ToString());
            var attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
            return attributes != null && attributes.Any()
                ? attributes.First().Description
                : value.ToString();
        }

        //public static RequireService ToEnum(this string enumString)
        //{
        //    return (RequireService)Enum.Parse(typeof(RequireService), enumString);
        //}
    }
}
