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
        public static string GetColumnNamesWithTable<T>(string tablename)
        {
            string result = string.Join(",", typeof(T).GetProperties().Select(x => $"[{tablename}].{x.Name}"));
            return result;
        }

        
        public static string GetColumnNamesExceptId<T>(string idColumnName)
        {
            string result = string.Join(",", typeof(T).GetProperties().Select(x => x.Name))
                .Replace($"{idColumnName},", "");
            return result;
        }
        public static string GetFieldNames<T>()
        {
            string result = string.Join(",", typeof(T).GetProperties().Select(x => "@" + x.Name));
            return result;
        }
        public static string GetFieldNamesExceptId<T>(string idColumnName)
        {
            string result = string.Join(",", typeof(T).GetProperties().Select(x => "@" + x.Name))
                .Replace($"@{idColumnName},", "");
            return result;
        }
        public static string GetUpdateQueryFields<T>(string idColumnName)
        {
            string result = string.Join(",", typeof(T).GetProperties()
               .Select(x => x.Name + "=@" + x.Name))
               .Replace($"{idColumnName}=@{idColumnName},", "");
            return result;
        }
    }
}
