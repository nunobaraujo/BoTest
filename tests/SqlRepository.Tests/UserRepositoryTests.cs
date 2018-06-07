using Backend;
using Contracts.Models;
using Core.Extensions;
using NUnit.Framework;
using Repositories.Sql;
using System;
using System.Threading.Tasks;

namespace SqlRepository.Tests
{
    [TestFixture]
    public class UserRepositoryTests
    {
        private UserRepository _userRepository;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _userRepository = new UserRepository("Server=.\\sqlexpress1;Database=BoTest;User Id=sa;Password=na123456;", null);
        }


        [Test]
        [Category("SQL")]
        [Category("UserRepository")]
        public async Task UserCRUD()
        {
            var newUser = new User
            {
                FirstName ="john",
                LastName = "Doe",
                CreationDate = DateTime.UtcNow,
                Country ="PT",
                Theme = "dark",
                Accentcolor = 1,
                Language = "pt-PT",
                Pin = "1234",
                Email = "someMail@mail.com",
                UserName = "TestUser01",
                PasswordHash = "i28PqffNhA9YUqoTY2wGow==",
                Salt = "E66CD3E5B1CF"
            };

            // CREATE
            var user1 = await _userRepository.User.Add(newUser);
            Assert.IsNotNull(user1);
            // READ
            var createdUser = await _userRepository.User.Get(user1.UserName);
            Assert.AreEqual(createdUser.UserName, user1);
            // UPDATE
            var updatedUser = createdUser.ToDto();
            updatedUser.Pin = "9876";
            await _userRepository.User.Update(updatedUser);
            createdUser = await _userRepository.User.Get(user1.UserName);
            Assert.AreEqual(createdUser.Pin, updatedUser.Pin);
            // DELETE
            await _userRepository.User.Delete(updatedUser.UserName);


            var user = await _userRepository.User.Get(Constants.AdminUserName);
            Assert.AreEqual(user.UserName, Constants.AdminUserName);
        }

        [Test]
        [Category("SQL")]
        [Category("UserRepository")]
        public async Task UserAuth()
        {
            var userId = await _userRepository.User.Auth(Constants.AdminUserName, "#Na123");
            Assert.NotNull(userId);
        }
        [Test]
        [Category("SQL")]
        [Category("UserRepository")]
        public async Task UserAuthFail()
        {
            var userId = await _userRepository.User.Auth("asd", "asd");
            Assert.IsNull(userId);
        }

        [Test]
        [Category("SQL")]
        [Category("UserRepository")]
        public async Task CompanyCRUD()
        {
            var newCompany = new Company
            {
                CreationDate = DateTime.UtcNow,
                Country = "PT",                
                EMail = "someMail@mail.com",
                Address = "Some Address",
                CAE = "Some CAE",
                City = "Some City",
                Fax = "Some Fax",
                IBAN = "Some Iban",
                MobilePhone = "Mobile Phone",
                Name = "some name",
                PostalCode = "some PostalCode",
                Reference = "SomeRef",
                TaxIdNumber = "SomeTaxIdNumber",
                Telephone = "Some Telephone",
                URL = "Some URL",
            };

            // CREATE
            var companyId = await _userRepository.Company.Add(newCompany);
            Assert.IsNotNull(companyId);
            // READ
            var createdCompany = await _userRepository.Company.Get(companyId);
            Assert.AreEqual(createdCompany.Id, companyId);
            // UPDATE
            var updatedCompany = createdCompany.ToDto();
            updatedCompany.IBAN= "123456789";
            await _userRepository.Company.Update(updatedCompany);
            createdCompany= await _userRepository.Company.Get(companyId);
            Assert.AreEqual(createdCompany.IBAN, updatedCompany.IBAN);
            // DELETE
            await _userRepository.Company.Delete(updatedCompany.Id);

        }

        [Test]
        [Category("SQL")]
        [Category("UserRepository")]
        public async Task CompanyList()
        {   
            var companyId = await _userRepository.Company.List();
            Assert.IsNotNull(companyId);
        }
    }
}
