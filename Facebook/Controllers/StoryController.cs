// <copyright file="StoryController.cs" company="PlaceholderCompany">
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
    /// Story Controller.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Route("[controller]")]

    [Authorize(Roles = AccessRoleConstant.UserRole)]
    public class StoryController : ControllerBase
    {
        private readonly IStoryRepository storyRepository;
        private readonly GetUserId getUserId;

        /// <summary>
        /// Initializes a new instance of the <see cref="StoryController" /> class.
        /// </summary>
        /// <param name="storyRepository">The story repository.</param>
        /// <param name="getUserId">The get user identifier.</param>
        public StoryController(IStoryRepository storyRepository, GetUserId getUserId)
        {
            this.storyRepository = storyRepository;
            this.getUserId = getUserId;
        }

        /// <summary>
        /// Adds the story by user.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Add the story.</returns>
        [HttpPost("AddStory")]
        public async Task<IActionResult> AddStoryByUser([FromForm] GetStoryModel model)
        {
            try
            {
                long userId = this.getUserId.GetLoginUserId();
                return this.Ok(await this.storyRepository.AddStoryByUser(userId, model));
            }
            catch (AggregateValidationException ex)
            {
                return this.BadRequest(ex.Validations);
            }
        }

        /// <summary>
        /// Gets all stories for user asynchronous.
        /// </summary>
        /// <param name="paginationParams">The pagination parameters.</param>
        /// <returns>
        /// get all stories for that users.
        /// </returns>
        [HttpPost("GetStoriesForUser")]
        public async Task<IActionResult> GetAllStoriesForUserAsync([FromBody] PaginationParams paginationParams)
        {
            try
            {
                long userId = this.getUserId.GetLoginUserId();
                return this.Ok(await this.storyRepository.GetAllStoriesForUserAsync(userId, paginationParams));
            }
            catch (AggregateValidationException ex)
            {
                return this.BadRequest(ex.Validations);
            }
        }

        /// <summary>
        /// Gets the story by identifier.
        /// </summary>
        /// <param name="storyId">The story identifier.</param>
        /// <returns>get Story By Id.</returns>
        [HttpGet("GetStoryById/{storyId}")]
        public async Task<IActionResult> GetStoryById(long storyId)
        {
            try
            {
                return this.Ok(await this.storyRepository.GetStoryById(storyId));
            }
            catch (AggregateValidationException ex)
            {
                return this.BadRequest(ex.Validations);
            }
        }

        /// <summary>
        /// Deltes the story.
        /// </summary>
        /// <param name="storyId">The story identifier.</param>
        /// <returns>true if story is deleted.</returns>
        [HttpDelete("DeleteStory/{storyId}")]
        public async Task<IActionResult> DeleteStory(long storyId)
        {
            try
            {
                return this.Ok(await this.storyRepository.DeleteStory(storyId));
            }
            catch (AggregateValidationException ex)
            {
                return this.BadRequest(ex.Validations);
            }
        }
    }
}
