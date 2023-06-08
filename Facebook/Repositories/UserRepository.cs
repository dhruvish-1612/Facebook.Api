// <copyright file="UserRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Repositories
{
    using System.Net;
    using System.Text.RegularExpressions;
    using AutoMapper;
    using Facebook.CustomException;
    using Facebook.Infrastructure.Infrastructure;
    using Facebook.Interface;
    using Facebook.Model;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// contains logic.
    /// </summary>
    /// <seealso cref="Facebook.Interface.IUserRepository"/>
    public class UserRepository : IUserRepository
    {
        private readonly FacebookContext db;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository" /> class.
        /// </summary>
        /// <param name="facebookContext">The facebook context.</param>
        /// <param name="mapper">The mapper.</param>
        public UserRepository(FacebookContext facebookContext, IMapper mapper)
        {
            this.db = facebookContext;
            this.mapper = mapper;
        }

        /// <summary>
        /// Gets the users.
        /// </summary>
        /// <returns>get all the users.</returns>
        public async Task<List<UserModel>> GetUsers()
        {
            List<User> userModels = await this.db.Users.ToListAsync();
            return this.mapper.Map<List<UserModel>>(userModels) ?? new List<UserModel>();
        }

        /// <summary>
        /// Gets the user by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns> get the user By Id .</returns>
        public async Task<UserModel> GetUserById(long id)
        {
            List<ValidationsModel> validationErrors = new();
            User? user = await this.db.Users.FirstOrDefaultAsync(user => user.UserId == id && user.DeletedAt == null);
            UserModel userModel = new();
            if (user == null)
            {
                validationErrors.Add(new ValidationsModel { StatusCode = (int)HttpStatusCode.NotFound, ErrorMessage = "User Not Found" });
                throw new AggregateValidationException { Validations = validationErrors };
            }

            return this.mapper.Map<UserModel>(user);
        }

        /// <summary>
        /// Gets the cities asynchronous.
        /// </summary>
        /// <returns>get all the cities.</returns>
        public async Task<List<City>> GetCitiesAsync()
        {
            List<City> cities = await this.db.Cities.ToListAsync() ?? new List<City>();
            return cities;
        }

        /// <summary>
        /// Gets the country asynchronous.
        /// </summary>
        /// <returns>get all the countries.</returns>
        public async Task<List<Country>> GetCountryAsync()
        {
            List<Country> countries = await this.db.Countries.ToListAsync() ?? new List<Country>();
            return countries;
        }

        /// <summary>
        /// Users the upsert.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>get updated and newly added user object.</returns>
        public async Task<UserModel> UserUpsert(UserModel user)
        {
            long userId;
            if (user.UserId == 0)
            {
                userId = await this.AddNewUser(user);
            }
            else
            {
                 userId = await this.UpdateUser(user);
            }

            return await this.GetUserById(userId);
        }

        /// <summary>
        /// Adds the new user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>return userid if successfully added otherwise throw exception.</returns>
        /// <exception cref="Facebook.CustomException.AggregateValidationException">custom exception.</exception>
        public async Task<long> AddNewUser(UserModel user)
        {
            List<ValidationsModel> errors = new();
            bool isUserExist = await this.db.Users.AnyAsync(userCheck => (userCheck.Email == user.Email.ToLower() || userCheck.PhoneNumber == user.PhoneNumber) && userCheck.DeletedAt == null);
            bool validEmail = await this.ValidateEmail(user.Email.ToLower());
            bool validPassword = await this.ValidatePassword(user.Password);
            if (!validEmail && !validPassword)
                errors.Add(new ValidationsModel { StatusCode = (int)HttpStatusCode.Unauthorized, ErrorMessage = "Not Valid Email. Please Enter Valid Email like dhruvish12@gmail.com" });

            if (isUserExist)
                errors.Add(new ValidationsModel { StatusCode = (int)HttpStatusCode.Unauthorized, ErrorMessage = "Email Or Phonenumber Already Exist." });

            if (errors.Any())
                throw new AggregateValidationException { Validations = errors };

            string fileName = string.Empty;
            if (user.FormFile != null)
               fileName = await this.GetAvtarName(user.FormFile);

            user.Avatar = fileName;
            user.Email = user.Email.ToLower();
            User newlyUser = this.mapper.Map<User>(user);
            this.db.Users.Add(newlyUser);
            await this.db.SaveChangesAsync();
            return newlyUser.UserId;
        }

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>Update new user it is valid otherwise throw the exception.</returns>
        /// <exception cref="AggregateValidationException">custom exception.</exception>
        public async Task<long> UpdateUser(UserModel user)
        {
            List<ValidationsModel> errors = new();
            User existingUser = await this.db.Users.FirstOrDefaultAsync(fetchUser => user.DeletedAt == null && fetchUser.UserId == user.UserId) ?? new User();

            if (existingUser == null)
            {
                errors.Add(new ValidationsModel() { StatusCode = (int)HttpStatusCode.NotFound, ErrorMessage = "User Not Found" });
                throw new AggregateValidationException { Validations = errors };
            }

            bool isValidPassword = await this.ValidatePassword(user.Password);
            if (!isValidPassword)
            {
                errors.Add(new ValidationsModel() { StatusCode = (int)HttpStatusCode.Unauthorized, ErrorMessage = "Incorrect Password And Phonenumber" });
            }

            if (existingUser.Email != user.Email.ToLower())
            {
                errors.Add(new ValidationsModel() { StatusCode = (int)HttpStatusCode.Conflict, ErrorMessage = "You Do Not Change Your Email." });
            }

            if (errors.Any())
                throw new AggregateValidationException { Validations = errors };

            string fileName = string.Empty;
            if (user.FormFile != null)
            {
                fileName = await this.GetAvtarName(user.FormFile);
            }

            user.Avatar = fileName;
            this.DeleteExistingMedia(existingUser.Avatar ?? string.Empty);
            user.UpdatedAt = DateTime.Now;
            User updatedUser = this.mapper.Map(user, existingUser);
            this.db.Users.Update(updatedUser);
            await this.db.SaveChangesAsync();
            return updatedUser.UserId;
        }

        /// <summary>
        /// Deletes the user.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>if successfully delete user then return deleted user objrct otherwise return empty model.</returns>
        public async Task<UserModel> DeleteUser(long id)
        {
            List<ValidationsModel> validationErrors = new List<ValidationsModel>();
            User? userToDelete = await this.db.Users.FirstOrDefaultAsync(user => user.UserId == id && user.DeletedAt == null);
            UserModel deleteUserObj = new();
            if (userToDelete == null)
            {
                validationErrors.Add(new ValidationsModel { StatusCode = (int)HttpStatusCode.NotFound, ErrorMessage = "User Not Found" });
                throw new AggregateValidationException { Validations = validationErrors };
            }

            deleteUserObj = this.mapper.Map<UserModel>(userToDelete);
            userToDelete.DeletedAt = DateTime.Now;
            this.db.Users.Update(userToDelete);
            await this.db.SaveChangesAsync();

            return deleteUserObj;
        }

        /// <summary>
        /// Gets the name of the avtar.
        /// </summary>
        /// <param name="formFile">The form file.</param>
        /// <returns>Get filename.</returns>
        public async Task<string> GetAvtarName(IFormFile formFile)
        {
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName);
            string uploadfile = Path.Combine(Directory.GetCurrentDirectory(), "Medias\\UserProfilePhoto", fileName);
            using var filestream = new FileStream(uploadfile, FileMode.Create);
            await formFile.CopyToAsync(filestream);
            return fileName;
        }

        /// <summary>
        /// Deltes the existing media.
        /// </summary>
        /// <param name="mediaName">Name of the media.</param>
        /// <returns>nothing.</returns>
        public async Task DeleteExistingMedia(string mediaName)
        {
            await Task.Run(() =>
            {
                string uploadfile = Path.Combine(Directory.GetCurrentDirectory(), "Medias\\UserProfilePhoto", mediaName ?? string.Empty);
                if (File.Exists(uploadfile))
                {
                    File.Delete(uploadfile);
                }
            });
        }

        /// <summary>
        /// Verifies the email.
        /// </summary>
        /// <param name="emailAddress">The email address.</param>
        /// <returns>True if email is correct otherwise false.</returns>
        public async Task<bool> ValidateEmail(string emailAddress)
        {
            string emailPattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
            Regex emailRegex = new(emailPattern);
            bool isValidEmail = await Task.Run(() => emailRegex.IsMatch(emailAddress));
            return isValidEmail;
        }

        /// <summary>
        /// Validates the password.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns>True if Password is correct otherwise false.</returns>
        public async Task<bool> ValidatePassword(string password)
        {
            string passwordPattern = @"^(?=.*[A-Z])(?=.*\d).{8,}$";
            Regex passwordRegex = new(passwordPattern);
            bool isValidEmail = await Task.Run(() => passwordRegex.IsMatch(password));
            return isValidEmail;
        }
    }
}
