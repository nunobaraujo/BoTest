using Contracts;
using Contracts.Api;
using Contracts.Models;
using Contracts.Requests;
using Core.Managers;
using Refit;
using Services.Comms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Controllers
{
    class BusinessController : IBusinessApi
    {
        private readonly IUserManager _userManager;
        private readonly IClientConnection _clientConnection;
        private readonly IBusinessManager _businessManager;

        public BusinessController(IClientConnection clientConnection, IUserManager userManager, IBusinessManager businessManager)
        {
            _userManager = userManager;
            _clientConnection = clientConnection;
            _businessManager = businessManager;
        }

        public async Task<Document> CreateDocument([Body] CreateDocumentRequest request)
        {
            var companyId = (await _userManager.GetActiveCompany(request.Token)).Id;
            var documentId = await _businessManager.CreateDocument(companyId, request.Document, request.DocumentLines.Cast<IDocumentLine>().ToList());
            request.Document.Id = documentId;
            return request.Document;
        }

    }
}
