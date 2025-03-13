using System.Data;
using System.Text.RegularExpressions;

namespace Isoqube.Orchestration.Core.Configurations.Utilities
{
    public class FormattedGuidGenerator
    {
        public static string Generate()
        {
            return Regex.Replace(Guid.NewGuid().ToString("N").Substring(0, 14).ToUpper(), @"(\w{4})(\w{4})(\w{6})", @"$1-$2-$3");
        }
    }

    public static class EnumUtil
    {
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }

    public static class PlatformDateTime
    {
        public static DateTime Datetime
        {
            get { return DateTime.UtcNow; }
        }
    }
}
