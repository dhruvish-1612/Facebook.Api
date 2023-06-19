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

        /// <summary>
        /// Gets all user post.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>getting all users posts.</returns>
        [HttpGet("GetAllPosts")]
        public async Task<IActionResult> GetAllUserPost(long userId)
        {
            return this.Ok(await this.userSocialActivitiesRepository.GetAllUserPost(userId));
        }

        /// <summary>
        /// Deletes the comment.
        /// </summary>
        /// <param name="postId">The post identifier.</param>
        /// <returns>
        /// true if successfully post deleted.
        /// </returns>
        [HttpDelete("DeletePost")]
        public async Task<IActionResult> DeletePost(long postId)
        {
            try
            {
                return this.Ok(await this.userSocialActivitiesRepository.DeletePost(postId));
            }
            catch (AggregateValidationException ex)
            {
                return this.BadRequest(ex.Validations);
            }
        }

        /// <summary>
        /// Upserts the comment.
        /// </summary>
        /// <param name="comment">The comment.</param>
        /// <returns>return added comment.</returns>
        [HttpPost("UpsertComment")]

        public async Task<IActionResult> UpsertComment([FromForm] AddCommentModel comment)
        {
            try
            {
                return this.Ok(await this.userSocialActivitiesRepository.UpsertComment(comment));
            }
            catch (AggregateValidationException ex)
            {
                return this.BadRequest(ex.Validations);
            }
        }

        /// <summary>
        /// Deletes the comment.
        /// </summary>
        /// <param name="userPostCommentId">The user post comment identifier.</param>
        /// <returns>true if successfully comment deleted.</returns>
        [HttpDelete("DeleteComment")]
        public async Task<IActionResult> DeleteComment(long userPostCommentId)
        {
            try
            {
                return this.Ok(await this.userSocialActivitiesRepository.DeleteComment(userPostCommentId));
            }
            catch (AggregateValidationException ex)
            {
                return this.BadRequest(ex.Validations);
            }
        }

        /// <summary>
        /// Gets the post comments.
        /// </summary>
        /// <param name="postId">The post identifier.</param>
        /// <returns>get all comment for that posts.</returns>
        [HttpGet("GetPostComments")]
        public async Task<IActionResult> GetPostComments(long postId)
        {
            try
            {
                return this.Ok(await this.userSocialActivitiesRepository.GetPostComments(postId));
            }
            catch (AggregateValidationException ex)
            {
                return this.BadRequest(ex.Validations);
            }
        }

        /// <summary>
        /// Likes the or dislike post.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="postId">The post identifier.</param>
        /// <returns>true if successfully like and dislike.</returns>
        [HttpPost("LikeOrDislikePost")]
        public async Task<IActionResult> LikeOrDislikePost(long userId, long postId)
        {
            try
            {
                return this.Ok(await this.userSocialActivitiesRepository.LikeOrDislikePost(userId, postId));
            }
            catch (AggregateValidationException ex)
            {
                return this.BadRequest(ex.Validations);
            }
        }

        /// <summary>
        /// Gets the post likes.
        /// </summary>
        /// <param name="postId">The post identifier.</param>
        /// <returns>Get Post Likes.</returns>
        [HttpGet("GetPostLikes")]
        public async Task<IActionResult> GetPostLikes(long postId)
        {
            try
            {
                return this.Ok(await this.userSocialActivitiesRepository.GetPostLikes(postId));
            }
            catch (AggregateValidationException ex)
            {
                return this.BadRequest(ex.Validations);
            }
        }
    }
}
    