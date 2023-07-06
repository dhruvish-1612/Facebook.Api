// <copyright file="IUserRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Interface
{
    using Facebook.Infrastructure.Infrastructure;
    using Facebook.Model;
    using Facebook.ParameterModel;

    /// <summary>
    /// User Interface only method is defined.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Gets the users.
        /// </summary>
        /// <param name="paginationParams">The pagination parameters.</param>
        /// <returns>
        /// get all the users.
        /// </returns>
        Task<Pagination<UserModel>> GetUsers(PaginationParams paginationParams);

        /// <summary>
        /// Gets the user by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>get the user By Id .</returns>
        Task<UserModel> GetUserById(long id);

        /// <summary>
        /// Users the upsert.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>get updated and newly added user object.</returns>
        Task<UserModel> UserUpsert(UserModel user);

        /// <summary>
        /// Deletes the user.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns> if successfully delete user then return deleted user objrct otherwise return empty model.</returns>
        Task<UserModel> DeleteUser(long id);

        /// <summary>
        /// Gets the country asynchronous.
        /// </summary>
        /// <param name="paginationParams">The pagination parameters.</param>
        /// <returns>
        /// get all the countries.
        /// </returns>
        Task<Pagination<Country>> GetCountryAsync(PaginationParams paginationParams);

        /// <summary>
        /// Gets the cities asynchronous.
        /// </summary>
        /// <param name="paginationDParams">The pagination d parameters.</param>
        /// <returns>
        /// get all all the cities.
        /// </returns>
        Task<Pagination<City>> GetCitiesAsync(PaginationParams paginationDParams);

        /// <summary>
        /// Validates the password.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns>true if valid password otherwise false.</returns>
        Task<bool> ValidatePassword(string password);

        /// <summary>
        /// Validates the email.
        /// </summary>
        /// <param name="emailAddress">The email address.</param>
        /// <returns>true if valid email otherwise false.</returns>
        Task<bool> ValidateEmail(string emailAddress);
    }
}
