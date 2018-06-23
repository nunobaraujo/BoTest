using Contracts.Api;
using Contracts.Models;
using Contracts.Requests;
using Refit;
using SocketClient.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocketClient.Api
{
    class BusinessApi : IBusinessApi
    {
        private readonly Client _client;

        public BusinessApi(Client client)
        {
            _client = client;
        }
                
        public async Task<Document> CreateDocument([Body] CreateDocumentRequest request)
        {
            _client.Connect();
            return await _client.BusinessApi.CreateDocument(request);
        }
    }
}
