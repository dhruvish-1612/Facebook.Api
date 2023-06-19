// <copyright file="IUserSocialActivitiesRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Interface
{
    using Facebook.Model;

    /// <summary>
    /// UserSocialActivitiesRepository Interface.
    /// </summary>
    public interface IUserSocialActivitiesRepository
    {
        /// <summary>
        /// Adds the comment.
        /// </summary>
        /// <param name="comment">The comment.</param>
        /// <returns>return added comment.</returns>
        Task<GetPostCommentModel> UpsertComment(AddCommentModel comment);

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
        /// <returns>get all user posts.</returns>
        Task<List<GetAllUserPostModel>> GetAllUserPost(long userId);

        /// <summary>
        /// Users the posts.
        /// </summary>
        /// <param name="posts">The posts.</param>
        /// <returns>return. </returns>
        Task<List<GetUserPostModel>> UserPosts(UserPostsModel posts);

        /// <summary>
        /// Gets the post comments.
        /// </summary>
        /// <param name="postId">The post identifier.</param>
        /// <returns>get all comments for that post.</returns>
        Task<List<GetPostCommentModel>> GetPostComments(long postId);

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
    }
}
