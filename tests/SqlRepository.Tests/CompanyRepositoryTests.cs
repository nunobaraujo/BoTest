using Contracts.Models;
using NUnit.Framework;
using Repositories.Sql;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SqlRepository.Tests
{
    [TestFixture]
    public class CompanyRepositoryTests
    {
        private CompanyRepository _companyRepository;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _companyRepository = new CompanyRepository("Server=.\\sqlexpress1;Database=botest_company;User Id=sa;Password=na123456;", null);
        }


        [Test]
        [Category("SQL")]
        [Category("CompanyRepository")]
        public async Task CompanyOptionsCRUD()
        {
            var obj = new CompanyOptions
            {
               Id = Guid.NewGuid().ToString().Replace("-","").ToUpper(),
               CloseJobReport = "CloseJobReport",
               IntegrationReturnDocument = "IntegrationReturnDocument",
               IntegrationShippingDocument = "IntegrationShippingDocument",
               IntegrationType = "IntegrationType",
               JobFileReport = "JobFileRepor",
               JobTicketReport = "JobTicketReport",
               ReturnDocument = "ReturnDocument",
               ShippingDocument = "ShippingDocument"               
            };

            // Create
            await _companyRepository.CompanyOptions.Set(obj);
            // Read
            var read = await _companyRepository.CompanyOptions.Get();
            // Update
            await _companyRepository.CompanyOptions.Set(obj);

            Assert.AreEqual("1", read.Id);
        }
    }
}
