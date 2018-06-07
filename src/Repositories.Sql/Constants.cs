namespace Repositories.Sql
{
    internal static class Constants
    {
        public const string GetLastInsertedId = "SELECT SCOPE_IDENTITY()";
    }
}
