// <copyright file="IUserRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Interface
{
    using Facebook.Infrastructure.Infrastructure;
    using Facebook.Model;

    /// <summary>
    /// User Interface only method is defined.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Gets the users.
        /// </summary>
        /// <returns>get all the users.</returns>
        Task<List<UserModel>> GetUsers();

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
        /// <returns>get all the countries.</returns>
        Task<List<Country>> GetCountryAsync();

        /// <summary>
        /// Gets the cities asynchronous.
        /// </summary>
        /// <returns>get all all the cities.</returns>
        Task<List<City>> GetCitiesAsync();
        Task<bool> ValidatePassword(string password);
        Task<bool> ValidateEmail(string emailAddress);
    }
}
