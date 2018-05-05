using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Middleware.Validator
{
    public interface IApiKeyValidator
    {
        bool Validate(string apiKey);
    }
}
