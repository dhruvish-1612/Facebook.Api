﻿// <copyright file="IAccountRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Interface
{
    using Facebook.Model;

    /// <summary>
    /// Account Interface.
    /// </summary>
    public interface IAccountRepository
    {
        /// <summary>
        /// Forgots the password.
        /// </summary>
        /// <param name="emailId">The email identifier.</param>
        /// <returns>UserId.</returns>
        Task<long> SendTokenViaMailForForgotPassword(string emailId);

        /// <summary>
        /// Sends the token mail.
        /// </summary>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="token">The token.</param>
        /// <returns>nothing.</returns>
        Task SendTokenMail(string emailAddress, string token);

        /// <summary>
        /// Updates the new password.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="token">The token.</param>
        /// <returns>
        /// updated password user object.
        /// </returns>
        Task<long> VerifyToken(long userId, string token);

        /// <summary>
        /// Users the login.
        /// </summary>
        /// <param name="loginParams">The login parameters.</param>
        /// <returns>If Successfully login then get user details.</returns>
        Task<string> UserLogin(LoginParams loginParams);

        /// <summary>
        /// Updates the new password.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="oldPassword">The old password.</param>
        /// <param name="updatedPassword">The updated password.</param>
        /// <returns>
        /// return true if successfully password changed.
        /// </returns>
        Task<bool> ResetPassword(long userId, string oldPassword, string updatedPassword);

        /// <summary>
        /// Decodes the from64.
        /// </summary>
        /// <param name="encodedData">The encoded data.</param>
        /// <returns>Decoded password.</returns>
        string DecodeFrom64(string encodedData);

        /// <summary>
        /// Encodes the password to base64.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns>Encoded password.</returns>
        string EncodePasswordToBase64(string password);
    }
}
