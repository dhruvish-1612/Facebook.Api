// <copyright file="IStoryRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Interface
{
    using Facebook.Model;
    using System.Collections.Generic;

    /// <summary>
    /// story interface.
    /// </summary>
    public interface IStoryRepository
    {
        /// <summary>
        /// Adds the story by user.
        /// </summary>
        /// <param name="story">The story.</param>
        /// <returns>added story.</returns>
        Task<StoryModel> AddStoryByUser(GetStoryModel story);

        /// <summary>
        /// Gets all stories for user asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>get all the stories.</returns>
        Task<List<GetAllStoriesForUserModel>> GetAllStoriesForUserAsync(long userId);
    }
}
