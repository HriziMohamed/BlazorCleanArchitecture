﻿using Application.DTOs.Request.Account;
using Application.DTOs.Response.Account;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
namespace Application.Extensions
{

    public class CustomAuthenticationStateProvider(LocalStorageService localStorageService) : AuthenticationStateProvider
    {
        private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var tokenModel = await localStorageService.GetModelFromToken();
            if (string.IsNullOrEmpty(tokenModel.Token)) return await Task.FromResult(new AuthenticationState(_anonymous));

            var getUserClaims = DecryptToken(tokenModel.Token!);
            if (getUserClaims == null) return await Task.FromResult(new AuthenticationState(_anonymous));

            var claimsPrincipal = SetClaimPrincipal(getUserClaims);
            return await Task.FromResult(new AuthenticationState(claimsPrincipal));
        }


        public async Task UpdateAuthenticationState(LocalStorageDTO localStorageDTO)
        {
            var claimsPrincipal = new ClaimsPrincipal();
            if (localStorageDTO.Token != null || localStorageDTO.Refresh != null)
            {
                await localStorageService.SetBrowserLocalStorage(localStorageDTO);
                var getUserClaims = DecryptToken(localStorageDTO.Token!);
                claimsPrincipal = SetClaimPrincipal(getUserClaims);
            }
            else
            {
                await localStorageService.RemoveTokenFromBrowserLocalStorage();
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
            }
        }

        public static ClaimsPrincipal SetClaimPrincipal(UserClaimsDTO claims)
        {
            if (claims.Email is null) return new ClaimsPrincipal();
            return new ClaimsPrincipal(new ClaimsIdentity(
                        [
                        new(ClaimTypes.Name , claims.UserName!),
                        new(ClaimTypes.Email , claims.Email!),
                        new(ClaimTypes.Role , claims.Role!),
                        new("Fullname" , claims.Fullname!)
                        ], Constant.AuthenticationType));
        }

        private static UserClaimsDTO DecryptToken(string jwtToken)
        {           
            try
            {
                if (string.IsNullOrEmpty(jwtToken)) return new UserClaimsDTO();

                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwtToken);

                var name = token.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.Name)!.Value;
                var email = token.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.Email)!.Value;
                var role = token.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.Role)!.Value;
                var fullname = token.Claims.FirstOrDefault(_ => _.Type == "Fullname")!.Value;
                return new UserClaimsDTO(fullname, name, email, role);
            }
            catch
            {
                return null!;
            }
        }

    }
}
