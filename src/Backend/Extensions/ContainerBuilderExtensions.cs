using Autofac;
using Contracts.Models;
using Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Extensions
{
    public static class ContainerBuilderExtensions
    {
        public static async Task InitUsersRepository(this IContainer src)
        {
            var usersRepo = src.Resolve<IUserRepository>();
            var user = await usersRepo.User.Get(Constants.AdminUserName);
            if (user == null)
            {                
                var newUser = new User
                {
                    UserName = Constants.AdminUserName,
                    CreationDate = DateTime.UtcNow,
                    FirstName = "admin",
                    LastName = "sysadmin",
                    Country = "PT",
                    Email = "geral@nbsoft.pt",
                    Language="pt-PT",
                    Pin = "1234",
                    Theme = "dark",
                    Accentcolor = -16741888
                };
                var adminUser = usersRepo.User.GeneratePassword(newUser, "sa123456");
                await usersRepo.User.Add(adminUser);
            }
        }
    }
}
