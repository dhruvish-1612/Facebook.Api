// <copyright file="AccountRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Repositories
{
    using System.ComponentModel;
    using System.Net;
    /*using System.Net.Mail;*/
    using System.Text.RegularExpressions;
    using System.Web.Helpers;
    using AutoMapper;
    using Facebook.Auth;
    using Facebook.CustomException;
    using Facebook.Enums;
    using Facebook.Infrastructure.Infrastructure;
    using Facebook.Interface;
    using Facebook.Model;
    using MailKit.Net.Smtp;
    using MailKit.Security;
    using Microsoft.EntityFrameworkCore;
    using MimeKit;
    using MimeKit.Text;

    /// <summary>
    /// Account Repository.
    /// </summary>
    /// <seealso cref="Facebook.Interface.IAccountRepository" />
    public class AccountRepository : IAccountRepository
    {
        private readonly FacebookContext db;
        private readonly IMapper mapper;
        private readonly IUserRepository userRepository;
        private readonly IUserRequestRepository userRequest;
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountRepository" /> class.
        /// </summary>
        /// <param name="db">The database.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="userRequest">The user request.</param>
        /// <param name="configuration">The configuration.</param>
        public AccountRepository(FacebookContext db, IMapper mapper, IUserRepository userRepository, IUserRequestRepository userRequest, IConfiguration configuration)
        {
            this.db = db;
            this.mapper = mapper;
            this.userRepository = userRepository;
            this.userRequest = userRequest;
            this.configuration = configuration;
        }

        /// <summary>
        /// Users the login.
        /// </summary>
        /// <param name="loginParams">The login parameters.</param>
        /// <returns>If Successfully login then get user details.</returns>
        public async Task<string> UserLogin(LoginParams loginParams)
        {
            List<ValidationsModel> errors = new();
            bool isValidEmail = await this.userRepository.ValidateEmail(loginParams.Email);
            bool isVaildPassword = await this.userRepository.ValidatePassword(loginParams.Password);
            if (!isValidEmail || !isVaildPassword)
                errors.Add(new ValidationsModel() { StatusCode = (int)HttpStatusCode.Unauthorized, ErrorMessage = "Email Or Password Incorrect." });

            User existUser = await this.db.Users.FirstOrDefaultAsync(user => user.Email.Equals(loginParams.Email.ToLower())) ?? new User();

            if (existUser.UserId == 0 && !Crypto.VerifyHashedPassword(loginParams.Password, existUser.Password))
                errors.Add(new ValidationsModel { StatusCode = (int)HttpStatusCode.NotFound, ErrorMessage = "Email or Password Not Found." });

            if (errors.Any())
                throw new AggregateValidationException { Validations = errors };

            JwtSetting jwtSettings = this.configuration.GetSection(nameof(JwtSetting)).Get<JwtSetting>();
            string token = JwtTokenHelper.GenerateToken(jwtSettings, existUser);
            return token;
        }

        /// <summary>
        /// Forgots the password.
        /// </summary>
        /// <param name="emailId">The email identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>email for forgot password and return userId.</returns>
        public async Task<long> SendTokenViaMailForForgotPassword(string emailId, long userId)
        {
            List<ValidationsModel> errors = new();
            User user = await this.db.Users.FirstOrDefaultAsync(u => u.Email.Equals(emailId.ToLower()) && u.UserId == userId && u.DeletedAt == null) ?? new User();

            if (user.UserId == 0)
                errors.Add(new ValidationsModel { StatusCode = (int)HttpStatusCode.NotFound, ErrorMessage = "User Of This EmailAddress Not Exist" });

            if (errors.Any())
                throw new AggregateValidationException { Validations = errors };

            Random random = new();
            string finalString = random.Next(100000, 999999).ToString();
            this.SendTokenMail(emailId, (string)finalString);

            ForgotPassword entry = new();
            entry.UserId = userId;
            entry.Token = finalString;
            entry.CreatedAt = DateTime.Now;
            this.db.ForgotPasswords.Add(entry);
            await this.db.SaveChangesAsync();
            return user.UserId;
        }

        /// <summary>
        /// Validates the otp two minuets.
        /// </summary>
        /// <param name="forgotObject">The forgot object.</param>
        /// <returns>
        /// return false do not validate true otp enter by user otherwise true.
        /// </returns>
        public async Task<bool> ValidateOtpTwoMinuets(ForgotPassword forgotObject)
        {
            DateTime startTime = DateTime.Now;
            if (forgotObject.ForgotId != 0 && startTime.Subtract(forgotObject.CreatedAt) >= TimeSpan.FromMinutes(2))
            {
                forgotObject.DeletedAt = DateTime.Now;
                this.db.ForgotPasswords.Update(forgotObject);
                await this.db.SaveChangesAsync();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Forgots the password.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="token">The token.</param>
        /// <returns>
        /// updated password user object.
        /// </returns>
        public async Task<long> VerifyToken(long userId, string token)
        {
            List<ValidationsModel> errors = new();
            bool isUserExist = await this.userRequest.ValidateUserById(userId);
            if (!isUserExist)
                errors.Add(new ValidationsModel { StatusCode = (int)HttpStatusCode.NotFound, ErrorMessage = "User Not Found" });

            ForgotPassword validtoken = await this.db.ForgotPasswords.FirstOrDefaultAsync(validateToken => validateToken.Token == token && validateToken.UserId == userId && validateToken.DeletedAt == null) ?? new ForgotPassword();

            if (validtoken.ForgotId == 0)
                errors.Add(new ValidationsModel { StatusCode = (int)HttpStatusCode.Unauthorized, ErrorMessage = "Token Is Not Valid For This User" });

            bool isValidateOtpTwoMinuets = await this.ValidateOtpTwoMinuets(validtoken);
            if (!isValidateOtpTwoMinuets)
                errors.Add(new ValidationsModel { StatusCode = (int)HttpStatusCode.Unauthorized, ErrorMessage = "Token Is Expired. Please Generate New Otp." });

            if (errors.Any())
                throw new AggregateValidationException { Validations = errors };
            User user = new();
            validtoken.DeletedAt = DateTime.Now;
            this.db.ForgotPasswords.Update(validtoken);
            await this.db.SaveChangesAsync();
            return userId;
        }

        /// <summary>
        /// Updates the new password.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="oldPassword">The old password.</param>
        /// <param name="updatedPassword">The updated password.</param>
        /// <returns>
        /// return true if password successfully updated.
        /// </returns>
        /// <exception cref="Facebook.CustomException.AggregateValidationException">validations for update new password.</exception>
        public async Task<bool> ResetPassword(long userId, string oldPassword, string updatedPassword)
        {
            List<ValidationsModel> errors = new();
            User? user = await this.db.Users.FirstOrDefaultAsync(x => x.UserId == userId && x.DeletedAt == null);
            if (user == null)
                errors.Add(new ValidationsModel { StatusCode = (int)HttpStatusCode.NotFound, ErrorMessage = "User Not Found" });

            bool isValidPassword = await this.userRepository.ValidatePassword(updatedPassword);
            if (!isValidPassword)
                errors.Add(new ValidationsModel() { StatusCode = (int)HttpStatusCode.Unauthorized, ErrorMessage = "Password Incorrect." });

            if (!string.IsNullOrWhiteSpace(oldPassword))
            {
                if (!Crypto.VerifyHashedPassword(user.Password, oldPassword))
                    errors.Add(new ValidationsModel { StatusCode = (int)HttpStatusCode.Unauthorized, ErrorMessage = "Old Password Is Incorrect." });
            }

            if (errors.Any())
                throw new AggregateValidationException { Validations = errors };

            user.Password = Crypto.HashPassword(updatedPassword);
            this.db.Update(user);
            await this.db.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Sends the token mail.
        /// </summary>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="token">The token.</param>
        /// <returns>Sned Mail to User for Forgot Password.</returns>
        public async Task SendTokenMail(string emailAddress, string token)
        {
            await Task.Run(() =>
            {
                var mailBody = $"<h1>Otp For Reset password</h1><h2> {token} </h2>";

                // create email message
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse("devloper.testing2022@gmail.com"));
                email.To.Add(MailboxAddress.Parse(emailAddress));
                email.Subject = "Reset Your Password";
                email.Body = new TextPart(TextFormat.Html) { Text = mailBody };

                // send email
                using var smtp = new SmtpClient();
                smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                smtp.Authenticate("devloper.testing2022@gmail.com", "zryemtpwhipptczr");
                smtp.Send(email);
                smtp.Disconnect(true);
            });
        }

        /* public async Task SendTokenMail(string emailAddress, string token)
         {
             await Task.Run(() =>
             {
                 // Command-line argument must be the SMTP host.
                 SmtpClient client = new("smtp.gmail.com", 587);
                 MailAddress from = new("devloper.testing2022@gmail.com", "Dhruvish " + (char)0xD8 + " Patel", System.Text.Encoding.UTF8);
                 MailAddress to = new(emailAddress);
                 MailMessage message = new("devloper.testing2022@gmail.com", emailAddress);
                 message.Body = $"<h1>Click link to reset password</h1><br><h2> {token} </h2>";
                 message.BodyEncoding = System.Text.Encoding.UTF8;
                 message.Subject = "Reset Your Password";
                 message.SubjectEncoding = System.Text.Encoding.UTF8;
                 client.SendAsync(message, token);
                 message.Dispose();
             });
         }*/

        /// <summary>
        /// Encodes the password to base64.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns>encoded password.</returns>
        /// <exception cref="System.Exception">Error in base64Encode" + ex.Message.</exception>
        public string EncodePasswordToBase64(string password)
        {
            try
            {
                byte[] encData_byte = new byte[password.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(password);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode" + ex.Message);
            }
        }

        /// <summary>
        /// Decodes the from64.
        /// </summary>
        /// <param name="encodedData">The encoded data.</param>
        /// <returns>decoded password.</returns>
        public string DecodeFrom64(string encodedData)
        {
            System.Text.UTF8Encoding encoder = new();
            System.Text.Decoder utf8Decode = encoder.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encodedData);
            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            string result = new(decoded_char);
            return result;
        }
    }
}
