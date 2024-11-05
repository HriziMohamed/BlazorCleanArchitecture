using Application.DTOs.Request.Account;
using Application.DTOs.Response;
using Application.DTOs.Response.Account;
using Application.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.Services
{
    public class AccountService(HttpClientService httpClientService): IAccountService
    {
        public async Task<LoginResponse> LoginAccountAsync(LoginDTO model)
        {
            try
            {
                var publicClient = httpClientService.GetPublicClient();
                var response = await publicClient.PostAsJsonAsync(Constant.LoginRoute, model);
                string error = CheckResponseStatus(response);
                if (!string.IsNullOrEmpty(error))
                    return new LoginResponse(Flag: false, Message: error);

                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                return result!;
            }
            catch (Exception ex)
            {
                return new LoginResponse(Flag: false, Message: ex.Message);
            }
        }

        public async Task<GeneralResponse> RegisterAccountAsync(CreateAccountDTO model)
        {
            try
            {
                var publicClient = httpClientService.GetPublicClient();
                var response = await publicClient.PostAsJsonAsync(Constant.RegisterRoute, model);
                string error = CheckResponseStatus(response);
                if (!string.IsNullOrEmpty(error))
                    return new GeneralResponse(Flag: false, Message: error);

                var result = await response.Content.ReadFromJsonAsync<GeneralResponse>();
                return result!;
            }
            catch (Exception ex)
            {
                return new GeneralResponse(Flag: false, Message: ex.Message);
            }
        }

        private static string CheckResponseStatus(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
                return $"Sorry unknown error occured. {Environment.NewLine} Error Description: {Environment.NewLine} Status Code: {response.StatusCode} {Environment.NewLine} Reason Phrase: {response.ReasonPhrase} ";
            else
                return null;
        }

        public async Task CreateAdminAtFirstStart()
        {
            try
            {
                var client = httpClientService.GetPublicClient();
                await client.PostAsync(Constant.CreateAdminRoute, null);
            }
            catch { }
        }
        public Task<GeneralResponse> ChangeUserRoleAsync(ChangeUserRoleRequestDTO model)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> CreateAccountAsync(CreateAccountDTO model)
        {
            throw new NotImplementedException();
        }

        public Task CreateAdmin()
        {
            throw new NotImplementedException();
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

        public Task<LoginResponse> RefreshTokenAsync(RefreshTokenDTO model)
        {
            throw new NotImplementedException();
        }
    }
}
