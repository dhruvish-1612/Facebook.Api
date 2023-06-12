// <copyright file="StoryController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace Facebook.Controllers
{
    using Facebook.CustomException;
    using Facebook.Interface;
    using Facebook.Model;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Story Controller.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Route("api/[controller]")]
    [ApiController]
    public class StoryController : ControllerBase
    {
        private readonly IStoryRepository storyRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="StoryController"/> class.
        /// </summary>
        /// <param name="storyRepository">The story repository.</param>
        public StoryController(IStoryRepository storyRepository)
        {
            this.storyRepository = storyRepository;
        }

        /// <summary>
        /// Adds the story by user.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Add the story.</returns>
        [HttpPost("AddStory")]
        public async Task<IActionResult> AddStoryByUser([FromForm]GetStoryModel model)
        {
            try
            {
                return this.Ok(await this.storyRepository.AddStoryByUser(model));
            }
            catch (AggregateValidationException ex)
            {
                return this.BadRequest(ex.Validations);
            }
        }

        [HttpGet("getStoryForThatUser")]
        public async Task<IActionResult> GetAllStoriesForUserAsync(long userId)
        {
            try
            {
                return this.Ok(await this.storyRepository.GetAllStoriesForUserAsync(userId));
            }
            catch(AggregateValidationException ex)
            {
                return this.BadRequest(ex.Validations);
            }
        }
    }
}
