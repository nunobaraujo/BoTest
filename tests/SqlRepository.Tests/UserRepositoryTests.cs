using NUnit.Framework;
using Repositories.Sql;
using System;
using System.Collections.Generic;
using System.Text;
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
            _userRepository = new UserRepository("Server=.\\sqlexpress1;Database=BoTest;User Id=sa;Password=na123456;");
        }


        [Test]
        [Category("SQL")]
        [Category("UserRepository")]
        public async Task UserRead()
        {   
            var user = await _userRepository.GetUser("1");
            Assert.AreEqual(user.UserName, "sa");
        }

        [Test]
        [Category("SQL")]
        [Category("UserRepository")]
        public async Task UserAuth()
        {
            var userId = await _userRepository.AuthUser("sa", "i28PqffNhA9YUqoTY2wGow==");
            Assert.AreEqual(userId, "1");
        }
        [Test]
        [Category("SQL")]
        [Category("UserRepository")]
        public async Task UserAuthFail()
        {
            var userId = await _userRepository.AuthUser("asd", "asd");
            Assert.IsNull(userId);
        }

        [Test]
        [Category("SQL")]
        [Category("UserRepository")]
        public async Task CompanyRead()
        {
            var company = await _userRepository.GetCompany("1");
            Assert.AreEqual(company.Id, "1");
        }
    }
}
