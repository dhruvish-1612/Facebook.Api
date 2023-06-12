// <copyright file="AccountController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Controllers
{
    using Facebook.CustomException;
    using Facebook.Interface;
    using Facebook.Model;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// the user account.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository accountRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController" /> class.
        /// </summary>
        /// <param name="accountRepository">The account repository.</param>
        /// <param name="encrptionAndDecryptionRepository">The encrption and decryption repository.</param>
        public AccountController(IAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }

        /// <summary>
        /// Users the login.
        /// </summary>
        /// <param name="loginParams">The login parameters.</param>
        /// <returns>user object if successfully login.</returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> UserLogin([FromForm] LoginParams loginParams)
        {
            try
            {
                return this.Ok(await this.accountRepository.UserLogin(loginParams));
            }
            catch (AggregateValidationException ex)
            {
                return this.BadRequest(ex.Validations);
            }
        }

        /// <summary>
        /// Forgots the password.
        /// </summary>
        /// <param name="emailId">The email identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns> UserId. </returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> SendTokenViaMailForForgotPassword(string emailId, long userId)
        {
            try
            {
                return this.Ok(await this.accountRepository.SendTokenViaMailForForgotPassword(emailId, userId));
            }
            catch (AggregateValidationException ex)
            {
                return this.BadRequest(ex.Validations);
            }
        }

        /// <summary>
        /// Updates the new password.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="token">The token.</param>
        /// <returns>return user object.</returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> VerifyToken(long userId, string token)
        {
            try
            {
                return this.Ok(await this.accountRepository.VerifyToken(userId, token));
            }
            catch (AggregateValidationException ex)
            {
                return this.BadRequest(ex.Validations);
            }
        }

        /// <summary>
        /// Updates the new password.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="oldPassword">The older password.</param>
        /// <param name="updatedPassword">The updated password.</param>
        /// <returns>return true if successfully update the password.</returns>
        [AllowAnonymous]
        [HttpPatch]
        public async Task<IActionResult> ResetPassword(long userId, string oldPassword, string updatedPassword)
        {
            try
            {
                return this.Ok(await this.accountRepository.ResetPassword(userId, oldPassword, updatedPassword));
            }
            catch (AggregateValidationException ex)
            {
                return this.BadRequest(ex.Validations);
            }
        }

        /// <summary>
        /// EncodePasswordToBase64.
        /// </summary>
        /// <param name="updatedPassword">The updated password.</param>
        /// <returns>
        /// encoded password.
        /// </returns>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult EncodePasswordToBase64(string updatedPassword)
        {
            var result = this.accountRepository.EncodePasswordToBase64(updatedPassword);
            return this.Ok(result);
        }

        /// <summary>
        /// Decodes the from64.
        /// </summary>
        /// <param name="updatedPassword">The updated password.</param>
        /// <returns>
        /// decoded password.
        /// </returns>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult DecodeFrom64(string updatedPassword)
        {
            var result = this.accountRepository.DecodeFrom64(updatedPassword);
            return this.Ok(result);
        }
    }
}
