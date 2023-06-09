// <copyright file="IUserSocialActivitiesRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Facebook.Model;

namespace Facebook.Interface
{
    /// <summary>
    /// UserSocialActivitiesRepository Interface.
    /// </summary>
    public interface IUserSocialActivitiesRepository
    {
        Task UserPosts(UserPostsModel posts);
    }
}
