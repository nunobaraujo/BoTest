using Autofac;
using Contracts.Models;
using Core.Extensions;
using Core.Repositories;
using NBsoft.Logs.Interfaces;
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

            // Encrypt unencrypted users
            var allUsers = (await usersRepo.User.List())
                .Where(u => u.Encrypted == false)
                .Select(u => u.ToDto())
                .ToList();

            if (allUsers.Count > 0)
            {
                var UpdatedUsers = new List<User>();
                foreach (var item in allUsers)
                {
                    var editable = item.ToDto();
                    editable.Encrypted = true;
                    editable.Country = string.IsNullOrEmpty(item.Country) ? "PT" : item.Country;
                    editable.FirstName = string.IsNullOrEmpty(item.FirstName) ? item.UserName : item.FirstName;
                    editable.LastName = string.IsNullOrEmpty(item.LastName) ? item.UserName : item.LastName;
                    editable.Email = string.IsNullOrEmpty(item.Email) ? $"{item.UserName}@mail.com" : item.Email;
                    UpdatedUsers.Add(editable);
                }
                try
                {
                    var userResult = await usersRepo.User.UpdateBatch(UpdatedUsers);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }
            }

            // Encrypt unencrypted companies
            var allCompanies = (await usersRepo.Company.List())
                .Where(u => u.Encrypted == false)
                .Select(u => u.ToDto())
                .ToList();
            if (allCompanies.Count > 0)
            {
                var UpdateCompanies = new List<Company>();
                foreach (var item in allCompanies)
                {
                    var editable = item.ToDto();
                    editable.Encrypted = true;
                    UpdateCompanies.Add(editable);
                }
                await usersRepo.Company.UpdateBatch(UpdateCompanies);
            }
        }
    }
}
