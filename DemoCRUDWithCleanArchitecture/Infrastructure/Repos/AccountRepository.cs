using Azure;
using Domain.Entity.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

using Application.Contracts;
using Application.DTOs.Request.Account;
using Application.DTOs.Response;
using Application.DTOs.Response.Account;
using Domain.Entity.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net.Mail;
using Application.Extensions;
using Humanizer;
namespace Infrastructure.Repos
{

    public class AccountRepository
    (RoleManager<IdentityRole> roleManager,
    UserManager<ApplicationUser> userManager, IConfiguration config,
    SignInManager<ApplicationUser> signInManager) : IAccount
    {
        private async Task<ApplicationUser> FindUserByEmailAsync(string email)
            => await userManager.FindByEmailAsync(email);

        private async Task<IdentityRole> FindRoleByNameAsync(string roleName)
            => await roleManager.FindByNameAsync(roleName);

        private static string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        private async Task<string> GenerateToken(ApplicationUser user)
        {
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                var userClaims = new[]
                {
                new Claim (ClaimTypes.Name, user.Email),
                new Claim (ClaimTypes.Email, user.Email),
                new Claim (ClaimTypes.Role, (await userManager.GetRolesAsync(user)).FirstOrDefault().ToString()),
                new Claim("Fullname", user.Name)
                };
                var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: userClaims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
                );
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch { return null!; }
        }
        private async Task<GeneralResponse> AssignUserToRole(ApplicationUser user, IdentityRole role)
        {


            if (user is null || role is null) return new GeneralResponse(false, "Model state cannot be empty");
            if (await FindRoleByNameAsync(role.Name) == null)

                await CreateRoleAsync(role.Adapt(new CreateRoleDTO()));
            IdentityResult result = await userManager.AddToRoleAsync(user, role.Name);
            string error = CheckResponse(result);
            if (!string.IsNullOrEmpty(error))
                return new GeneralResponse(false, error);
            else
                return new GeneralResponse(true, $"{user.Name} assigned to {role.Name} role");
        }

        private static string CheckResponse(IdentityResult result)
        {
            if (!!result.Succeeded)
            {
                var errors = result.Errors.Select(_ => _.Description);
                return string.Join(Environment.NewLine, errors);
            }
            return null!;
        }
        public Task<GeneralResponse> ChangeUserRoleAsync(ChangeUserRoleRequestDTO model)
        {
            throw new NotImplementedException();
        }

        public async Task<GeneralResponse> CreateAccountAsync(CreateAccountDTO model)
        {
            try
            {
                if (await FindUserByEmailAsync(model.EmailAddress) != null)
                    return new GeneralResponse(false, "Sorry, user is already created");
                var user = new ApplicationUser()
                {
                    Name = model.Name,
                    UserName = model.EmailAddress,
                    Email = model.EmailAddress,
                    PasswordHash = model.Password
                };
                var result = await userManager.CreateAsync(user, model.Password);
                string error = CheckResponse(result);
                if (!string.IsNullOrEmpty(error))
                    return new GeneralResponse(false, error);
                var (flag, message) = await AssignUserToRole(user, new IdentityRole() { Name = model.Role });
                   return new GeneralResponse(flag, message);
            }
            catch (Exception ex)
            {
                return new GeneralResponse(false, ex.Message);
            }
        }

        public async Task CreateAdmin()
        {
            try
            {
                if ((await FindRoleByNameAsync(Constant.Role.Admin)) != null) return;
                var admin = new CreateAccountDTO()
                {
                    Name = "Admin",
                    Password = "Admin@123",
                    EmailAddress = "admin@admin.com",
                    Role = Constant.Role.Admin

                };
                await CreateAccountAsync(admin);
            }
            catch { }
        }

        public Task<GeneralResponse> CreateRoleAsync(CreateRoleDTO model)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<GetRoleDTO>> GetRolesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<GetUsersWithRolesResponseDTO>> GetUsersWithRolesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<LoginResponse> LoginAccountAsync(LoginDTO model)
        {
            throw new NotImplementedException();
        }


    }
}
