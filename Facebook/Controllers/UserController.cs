// <copyright file="UserController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Controllers
{
    using Facebook.Constant;
    using Facebook.CustomException;
    using Facebook.Helpers;
    using Facebook.Interface;
    using Facebook.Model;
    using Facebook.ParameterModel;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// the user controller.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Authorize(Roles = AccessRoleConstant.UserRole)]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        /// <summary>
        /// The user repository.
        /// </summary>
        private readonly IUserRepository userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        public UserController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        /// <summary>
        /// Gets the users.
        /// </summary>
        /// <param name="paginationParams">The pagination parameters.</param>
        /// <returns>
        /// get all users.
        /// </returns>
        [HttpPost("GetUsers")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUsers([FromBody]PaginationParams paginationParams)
        {
            return this.Ok(await this.userRepository.GetUsers(paginationParams));
        }

        /// <summary>
        /// Gets the users by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>get User by id.</returns>
        [HttpGet("GetUserById/{id}")]
        public async Task<IActionResult> GetUsersById(long id)
        {
            try
            {
                UserModel result = await this.userRepository.GetUserById(id);
                return this.Ok(result);
            }
            catch (AggregateValidationException ex)
            {
                return this.BadRequest(ex);
            }
        }

        /// <summary>
        /// Gets the cities.
        /// </summary>
        /// <param name="paginationParams">The pagination parameters.</param>
        /// <returns>
        /// get all the cities.
        /// </returns>
        [HttpPost("GetCities")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCities([FromBody]PaginationParams paginationParams)
        {
            return this.Ok(await this.userRepository.GetCitiesAsync(paginationParams));
        }

        /// <summary>
        /// Gets the countries.
        /// </summary>
        /// <param name="paginationParams">The pagination parameters.</param>
        /// <returns>
        /// get all the countries.
        /// </returns>
        [HttpPost("GetCountries")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCountries([FromBody]PaginationParams paginationParams)
        {
            return this.Ok(await this.userRepository.GetCountryAsync(paginationParams));
        }

        /// <summary>
        /// Users the upsert.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>User Upsert UserModel.</returns>
        [HttpPost("Upsert")]
        [AllowAnonymous]
        public async Task<IActionResult> UserUpsert([FromForm] UserModel user)
        {
            try
            {
                UserModel result = await this.userRepository.UserUpsert(user);
                return this.Ok(result);
            }
            catch (AggregateValidationException ex) when (ex.Validations.Any())
            {
                return this.BadRequest(ex.Validations);
            }
            catch (Exception)
            {
                return this.BadRequest("Exception Occurred While Upserting User");
            }
        }

        /// <summary>
        /// Delete User.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// updated or added user object and if any exception occur while upserting then it is autometically throw the status code.
        /// </returns>
        [HttpDelete("DeleteUser/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteUser(long id)
        {
            try
            {
                UserModel result = await this.userRepository.DeleteUser(id);
                return this.Ok(result);
            }
            catch (AggregateValidationException ex)
            {
                return this.BadRequest(ex.Validations);
            }
        }
    }
}
