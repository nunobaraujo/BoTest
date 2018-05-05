using System.Linq;

namespace Repositories.Common
{
    internal static class Common
    {
        public static string GetColumnNames<T>()
        {
            string result = string.Join(",", typeof(T).GetProperties().Select(x => x.Name));
            return result;
        }
        public static string GetFieldNames<T>()
        {
            string result = string.Join(",", typeof(T).GetProperties().Select(x => "@" + x.Name));
            return result;
        }
    }
}
