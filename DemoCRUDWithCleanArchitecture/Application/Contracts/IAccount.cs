using Application.DTOs.Response.Account;
using Application.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Request.Account;

namespace Application.Contracts
{
    public interface IAccount
    {

        Task CreateAdmin();

        Task<GeneralResponse> CreateAccountAsync(CreateAccountDTO model);

        Task<LoginResponse> LoginAccountAsync(LoginDTO model);

        Task<GeneralResponse> CreateRoleAsync(CreateRoleDTO model);

        Task<IEnumerable<GetRoleDTO>> GetRolesAsync();

        Task<IEnumerable<GetUsersWithRolesResponseDTO>> GetUsersWithRolesAsync();

        Task<GeneralResponse> ChangeUserRoleAsync(ChangeUserRoleRequestDTO model);
    }
}
