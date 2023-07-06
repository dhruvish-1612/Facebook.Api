// <copyright file="JwtTokenHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Auth
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using System.Text.Json;
    using Facebook.Infrastructure.Infrastructure;
    using Facebook.Model;
    using Microsoft.IdentityModel.Tokens;

    /// <summary>
    /// Jwt Token Helper Class.
    /// </summary>
    public class JwtTokenHelper
    {
        /// <summary>
        /// Generates the token.
        /// </summary>
        /// <param name="jwtSetting">The JWT setting.</param>
        /// <param name="user">The user.</param>
        /// <returns>Token Generation.</returns>
        public static string GenerateToken(JwtSetting jwtSetting, User user)
        {
            if (jwtSetting == null)
            {
                return string.Empty;
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Role, JsonSerializer.Serialize(user.Role)),
                new Claim("UserId", JsonSerializer.Serialize(user.UserId)),
            };

            var token = new JwtSecurityToken(
            jwtSetting.Issuer,
            jwtSetting.Audience,
            claims,
            expires: DateTime.UtcNow.AddDays(5), // Default 5 mins, max 1 day
            signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
