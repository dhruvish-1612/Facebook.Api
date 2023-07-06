// <copyright file="UserSocialActivitiesController.cs" company="PlaceholderCompany">
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
    /// UserSocialActivitiesRepository.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Authorize(Roles = AccessRoleConstant.UserRole)]
    [Route("[controller]")]
    public class UserSocialActivitiesController : ControllerBase
    {
        private readonly IUserSocialActivitiesRepository userSocialActivitiesRepository;
        private readonly GetUserId getUserId;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserSocialActivitiesController" /> class.
        /// </summary>
        /// <param name="userSocialActivitiesRepository">The user social activities repository.</param>
        /// <param name="getUserId">The get user identifier.</param>
        public UserSocialActivitiesController(IUserSocialActivitiesRepository userSocialActivitiesRepository, GetUserId getUserId)
        {
            this.userSocialActivitiesRepository = userSocialActivitiesRepository;
            this.getUserId = getUserId;
        }

        /// <summary>
        /// Adds the posts.
        /// </summary>
        /// <param name="post">The post.</param>
        /// <returns>
        /// Added Posts of that users.
        /// </returns>
        [HttpPost("AddPosts")]
        public async Task<IActionResult> AddPosts([FromForm] UserPostsModel post)
        {
            try
            {
                long userId = this.getUserId.GetLoginUserId();
                return this.Ok(await this.userSocialActivitiesRepository.UserPosts(userId, post));
            }
            catch (AggregateValidationException ex)
            {
                return this.BadRequest(ex.Validations);
            }
        }

        /// <summary>
        /// Gets all user post.
        /// </summary>
        /// <param name="postParams">The post parameters.</param>
        /// <returns>
        /// getting all users posts.
        /// </returns>
        [HttpPost("GetAllPosts")]
        public async Task<IActionResult> GetAllUserPost([FromBody] PostParams postParams)
        {
            try
            {
                long userId = this.getUserId.GetLoginUserId();
                return this.Ok(await this.userSocialActivitiesRepository.GetAllUserPost(userId, postParams));
            }
            catch (AggregateValidationException ex)
            {
                return this.BadRequest(ex.Validations);
            }
        }

        /// <summary>
        /// Gets the post by identifier.
        /// </summary>
        /// <param name="postId">The post identifier.</param>
        /// <returns>Get Post By Id.</returns>
        [HttpGet("GetPostById/{postId}")]
        public async Task<IActionResult> GetPostById(long postId)
        {
            try
            {
                return this.Ok(await this.userSocialActivitiesRepository.GetPostById(postId));
            }
            catch (AggregateValidationException ex)
            {
                return this.BadRequest(ex.Validations);
            }
        }

        /// <summary>
        /// Deletes the comment.
        /// </summary>
        /// <param name="postId">The post identifier.</param>
        /// <returns>
        /// true if successfully post deleted.
        /// </returns>
        [HttpDelete("DeletePost/{postId}")]
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

        public async Task<IActionResult> UpsertComment([FromBody] AddCommentModel comment)
        {
            try
            {
                long userId = this.getUserId.GetLoginUserId();
                return this.Ok(await this.userSocialActivitiesRepository.UpsertComment(userId, comment));
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
        [HttpDelete("DeleteComment/{userPostCommentId}")]
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
        /// <param name="getCommentsParam">The get comments parameter.</param>
        /// <returns>
        /// get all comment for that posts.
        /// </returns>
        [HttpPost("GetPostComments")]
        public async Task<IActionResult> GetPostComments([FromBody] GetCommentsParam getCommentsParam)
        {
            try
            {
                return this.Ok(await this.userSocialActivitiesRepository.GetPostComments(getCommentsParam));
            }
            catch (AggregateValidationException ex)
            {
                return this.BadRequest(ex.Validations);
            }
        }

        /// <summary>
        /// Likes the or dislike post.
        /// </summary>
        /// <param name="postId">The post identifier.</param>
        /// <returns>
        /// true if successfully like and dislike.
        /// </returns>
        [HttpPost("LikeOrDislikePost")]
        public async Task<IActionResult> LikeOrDislikePost(long postId)
        {
            try
            {
                long userId = this.getUserId.GetLoginUserId();
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
        [HttpGet("GetPostLikes/{postId}")]
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

        /// <summary>
        /// Gets the like by identifier.
        /// </summary>
        /// <param name="likeId">The like identifier.</param>
        /// <returns>Get Like By Id.</returns>
        [HttpGet("GetLikeById/{likeId}")]
        public async Task<IActionResult> GetLikeById(long likeId)
        {
            try
            {
                return this.Ok(await this.userSocialActivitiesRepository.GetLikeById(likeId));
            }
            catch (AggregateValidationException ex)
            {
                return this.BadRequest(ex.Validations);
            }
        }

        /// <summary>
        /// Gets the comment by identifier.
        /// </summary>
        /// <param name="commentId">The comment identifier.</param>
        /// <returns>
        /// Get Comment By it's Id.
        /// </returns>
        [HttpGet("GetCommentById/{commentId}")]
        public async Task<IActionResult> GetCommentById(long commentId)
        {
            try
            {
                return this.Ok(await this.userSocialActivitiesRepository.GetPostCommetnById(commentId));
            }
            catch (AggregateValidationException ex)
            {
                return this.BadRequest(ex.Validations);
            }
        }
    }
}
