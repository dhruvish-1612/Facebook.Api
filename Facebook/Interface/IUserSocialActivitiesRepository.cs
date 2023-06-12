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
        /// Users the posts.
        /// </summary>
        /// <param name="posts">The posts.</param>
        /// <returns>return. </returns>
        Task<List<GetUserPostModel>> UserPosts(UserPostsModel posts);
    }
}
