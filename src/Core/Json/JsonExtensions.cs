namespace Core.Json
{
    public static class JsonExtensions
    {
        public static string ToJson(this object value)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(value);
        }
    }
}
