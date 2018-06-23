using Contracts.Models;
using NBsoft.Logs;
using NBsoft.Logs.Interfaces;
using NUnit.Framework;
using Repositories.Sql;
using System;
using System.Linq;
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
            var connstring = "Server=.\\sqlexpress1;Database=botest_company;User Id=sa;Password=na123456;";
            _companyRepository = new CompanyRepository(connstring, null);
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

        [Test]
        [Category("SQL")]
        [Category("CompanyRepository")]
        public async Task JobCRUD()
        {            
            var obj = new Job
            {
                Id = Guid.NewGuid().ToString().Replace("-", "").ToUpper(),
                CustomerId = null,
                CustomerRouteId = null,
                UserId ="sa",
                ProductId = "1",
                BeginDate = DateTime.UtcNow,
                CreationDate = DateTime.UtcNow,
                Description = "job description",
                JobReference = "Job Reference",
                Notes ="Job comments",
                Option1 = true,
                ProductHeight = 1.5m,
                ProductWidth = 1.3m,
                ProductLength = 2.5m,
                ProductUnitType = 1,
                TotalValue = 12.55m
            };

            // Create
            var oId = await _companyRepository.Job.Add(obj);
            Assert.AreEqual(oId , obj.Id);
            // Read
            var read = await _companyRepository.Job.Get(obj.Id);
            Assert.AreEqual(false, read.Option6);
            // Update
            obj.Option6 = true;
            await _companyRepository.Job.Update(obj);
            read = await _companyRepository.Job.Get(obj.Id);
            Assert.AreEqual(true, read.Option6);
            //Delete
            await _companyRepository.Job.Delete(obj.Id);
            read = await _companyRepository.Job.Get(obj.Id);
            Assert.IsNull(read);
        }

        [Test]
        [Category("SQL")]
        [Category("CompanyRepository")]
        public async Task CustomerCRUD()
        {
            var obj = new Customer
            {
                Id = Guid.NewGuid().ToString().Replace("-", "").ToUpper(),
                CreationDate = DateTime.UtcNow,
                Comments = "old Comments",
                TaxIdNumber = Guid.NewGuid().ToString().Replace("-","").ToLower(),
                Name = "Name",
                IsClient = true,
                IsSupplier = true
            };

            // List
            var clientList = await _companyRepository.Customer.List();

            // Create
            var oId = await _companyRepository.Customer.Add(obj);
            Assert.AreEqual(oId, obj.Id);
            // Read
            var read = await _companyRepository.Customer.Get(obj.Id);
            Assert.AreEqual(read.Comments, "old Comments");
            // Update
            obj.Comments = "new comments";
            await _companyRepository.Customer.Update(obj);
            read = await _companyRepository.Customer.Get(obj.Id);
            Assert.AreEqual(obj.Comments, read.Comments);
            //Delete
            await _companyRepository.Customer.Delete(obj.Id);
            read = await _companyRepository.Customer.Get(obj.Id);
            Assert.IsNull(read);
        }

        [Test]
        [Category("SQL")]
        [Category("CompanyRepository")]
        public async Task CustomerRouteCRUD()
        {
            var customerid = (await _companyRepository.Customer.List())
                .Last()
                .Id;

            var routeId = (await _companyRepository.Route.List())
                .Last()
                .Id;

            var obj = new CustomerRoute
            {
                Id = Guid.NewGuid().ToString().Replace("-", "").ToUpper(),
                CustomerId = customerid,
                RouteId = routeId,
                CreationDate = DateTime.UtcNow,
                Comments = "old Comments",
                Name = "Name",
                Address = "Address",
                City = "City",
                Country = "Country",
                PostalCode = "PostalCode"
            };

            
            // Create
            var oId = await _companyRepository.CustomerRoute.Add(obj);
            Assert.AreEqual(oId, obj.Id);
            // Read
            var read = await _companyRepository.CustomerRoute.Get(obj.Id);
            Assert.AreEqual(read.Comments, "old Comments");
            // Update
            obj.Comments = "new comments";
            await _companyRepository.CustomerRoute.Update(obj);
            read = await _companyRepository.CustomerRoute.Get(obj.Id);
            Assert.AreEqual(obj.Comments, read.Comments);
            //Delete
            await _companyRepository.CustomerRoute.Delete(obj.Id);
            read = await _companyRepository.CustomerRoute.Get(obj.Id);
            Assert.IsNull(read);
        }

        //[Test]
        //[Category("SQL")]
        //[Category("CompanyRepository")]
        //public async Task DocumentCRUD()
        //{
        //    var customerid = (await _companyRepository.Document.List())
        //        .Last()
        //        .Id;

        //    var routeId = (await _companyRepository.Route.List())
        //        .Last()
        //        .Id;

        //    var obj = new Document
        //    {
        //        Id = Guid.NewGuid().ToString().Replace("-", "").ToUpper(),
        //        CustomerId = customerid,                
        //        CreationDate = DateTime.UtcNow,                
        //        Coin= "EUR",
        //        DocumentDate = DateTime.UtcNow

        //    };


        //    // Create
        //    var oId = await _companyRepository.Document.Add(obj);
        //    Assert.AreEqual(oId, obj.Id);
        //    // Read
        //    var read = await _companyRepository.Document.Get(obj.Id);
        //    Assert.AreEqual(read.Comments, "old Comments");
        //    // Update
        //    obj.Comments = "new comments";
        //    await _companyRepository.Document.Update(obj);
        //    read = await _companyRepository.Document.Get(obj.Id);
        //    Assert.AreEqual(obj.Comments, read.Comments);
        //    //Delete
        //    await _companyRepository.Document.Delete(obj.Id);
        //    read = await _companyRepository.Document.Get(obj.Id);
        //    Assert.IsNull(read);
        //}


        [Test]
        [Category("SQL")]
        [Category("CompanyRepository")]
        public async Task JobServiceProductsByJob()
        {
            var rs = await _companyRepository.JobServiceProducts.GetByJob("13331"); ;
            Assert.IsNotNull(rs);
        }

        [Test]
        [Category("SQL")]
        [Category("CompanyRepository")]
        public async Task RouteCRUD()
        {
            var obj = new Route
            {
                Id = Guid.NewGuid().ToString().Replace("-", "").ToUpper(),
                CreationDate = DateTime.UtcNow,
                Comments = "old Comments",
                Name = "Name",
                DayOfWeek = 1
            };

            // List
            var routeList = await _companyRepository.Route.List();

            // Create
            var oId = await _companyRepository.Route.Add(obj);
            Assert.AreEqual(oId, obj.Id);
            // Read
            var read = await _companyRepository.Route.Get(obj.Id);
            Assert.AreEqual(read.Comments, "old Comments");
            // Update
            obj.Comments = "new comments";
            await _companyRepository.Route.Update(obj);
            read = await _companyRepository.Route.Get(obj.Id);
            Assert.AreEqual(obj.Comments, read.Comments);
            //Delete
            await _companyRepository.Route.Delete(obj.Id);
            read = await _companyRepository.Route.Get(obj.Id);
            Assert.IsNull(read);
        }

        [Test]
        [Category("SQL")]
        [Category("CompanyRepository")]
        public async Task DocumentName()
        {
            var doc = (await _companyRepository.Document.GetByDate(new DateTime(2016, 1, 1), DateTime.Now))
                .OrderBy(x => x.DocumentDate)
                .Last();
            var docName = await _companyRepository.Document.GetName(doc.Id);
            Assert.AreEqual(docName, doc.Name);
        }
        [Test]
        [Category("SQL")]
        [Category("CompanyRepository")]
        public async Task DocumentDescription()
        {
            var doc = (await _companyRepository.Document.GetByDate(new DateTime(2016, 1, 1), DateTime.Now))
                .OrderBy(x => x.DocumentDate)
                .Last();
            var docDescription = await _companyRepository.Document.GetDescription(doc.Id);
            Assert.AreEqual(docDescription, doc.Description);
        }
    }
}
