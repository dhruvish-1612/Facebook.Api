// <copyright file="UserSocialActivitiesController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace Facebook.Controllers
{
    using Facebook.CustomException;
    using Facebook.Interface;
    using Facebook.Model;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// UserSocialActivitiesRepository.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Route("[controller]")]
    [ApiController]
    public class UserSocialActivitiesController : ControllerBase
    {
        private readonly IUserSocialActivitiesRepository userSocialActivitiesRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserSocialActivitiesController"/> class.
        /// </summary>
        /// <param name="userSocialActivitiesRepository">The user social activities repository.</param>
        public UserSocialActivitiesController(IUserSocialActivitiesRepository userSocialActivitiesRepository) => this.userSocialActivitiesRepository = userSocialActivitiesRepository;

        /// <summary>
        /// Adds the posts.
        /// </summary>
        /// <param name="posts">The posts.</param>
        /// <returns>Added Posts of that users.</returns>
        [HttpPost("AddPosts")]
        public async Task<IActionResult> AddPosts([FromForm] UserPostsModel posts)
        {
            try
            {
                return this.Ok(await this.userSocialActivitiesRepository.UserPosts(posts));
            }
            catch (AggregateValidationException ex)
            {
                return this.BadRequest(ex.Validations);
            }
        }
    }
}
