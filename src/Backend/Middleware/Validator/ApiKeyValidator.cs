namespace Backend.Middleware.Validator
{
    public class ApiKeyValidator : IApiKeyValidator
    {
        private readonly string _apiKey;

        public ApiKeyValidator(string apiKey)
        {
            _apiKey = apiKey;
        }

        public bool Validate(string apiKey)
        {
            return apiKey == _apiKey;
        }
    }
}
