// <copyright file="IUserSocialActivitiesRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Interface
{
    using Facebook.Model;
    using Facebook.ParameterModel;

    /// <summary>
    /// UserSocialActivitiesRepository Interface.
    /// </summary>
    public interface IUserSocialActivitiesRepository
    {
        /// <summary>
        /// Adds the comment.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="comment">The comment.</param>
        /// <returns>
        /// return added comment.
        /// </returns>
        Task<GetPostCommentModel> UpsertComment(long userId, AddCommentModel comment);

        /// <summary>
        /// Deletes the comment.
        /// </summary>
        /// <param name="userPostCommentId">The user post comment identifier.</param>
        /// <returns>true if suceessfully deleted.</returns>
        Task<bool> DeleteComment(long userPostCommentId);

        /// <summary>
        /// Gets all user post.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="postParams">The post parameters.</param>
        /// <returns>
        /// get all user posts.
        /// </returns>
        Task<Pagination<GetAllUserPostModel>> GetAllUserPost(long userId, PostParams postParams);

        /// <summary>
        /// Users the posts.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="posts">The posts.</param>
        /// <returns>
        /// return.
        /// </returns>
        Task<GetAllUserPostModel> UserPosts(long userId, UserPostsModel posts);

        /// <summary>
        /// Gets the post comments.
        /// </summary>
        /// <param name="getCommentsParam">The get comments parameter.</param>
        /// <returns>
        /// get all comments for that post.
        /// </returns>
        Task<Pagination<GetPostCommentModel>> GetPostComments(GetCommentsParam getCommentsParam);

        /// <summary>
        /// Likes the or dislike post.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="postId">The post identifier.</param>
        /// <returns>true if successfully like and dislike.</returns>
        Task<bool> LikeOrDislikePost(long userId, long postId);

        /// <summary>
        /// Deletes the post.
        /// </summary>
        /// <param name="postId">The post identifier.</param>
        /// <returns>true if successfully delete post.</returns>
        Task<bool> DeletePost(long postId);

        /// <summary>
        /// Gets the post likes.
        /// </summary>
        /// <param name="postId">The post identifier.</param>
        /// <returns>get post likes.</returns>
        Task<GetUserPostLikeWithCountModel> GetPostLikes(long postId);

        /// <summary>
        /// Gets the post by identifier.
        /// </summary>
        /// <param name="postId">The post identifier.</param>
        /// <returns>get post by Id.</returns>
        Task<GetAllUserPostModel> GetPostById(long postId);

        /// <summary>
        /// Gets the like by identifier.
        /// </summary>
        /// <param name="likeId">The like identifier.</param>
        /// <returns>get like by id.</returns>
        Task<GetUserPostLikeModel> GetLikeById(long likeId);

        /// <summary>
        /// Gets the post commetn by identifier.
        /// </summary>
        /// <param name="postCommentId">The post comment identifier.</param>
        /// <returns>Get Comment By it's Id.</returns>
        Task<GetPostCommentModel> GetPostCommetnById(long postCommentId);
    }
}
