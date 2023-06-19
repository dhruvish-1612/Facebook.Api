// <copyright file="IStoryRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Interface
{
    using System.Collections.Generic;
    using Facebook.Model;

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
        /// Deltes the story.
        /// </summary>
        /// <param name="storyId">The story identifier.</param>
        /// <returns>true if story is delted.</returns>
        Task<bool> DeleteStory(long storyId);

        /// <summary>
        /// Gets all stories for user asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>get all the stories.</returns>
        Task<List<GetAllUserPostModel>> GetAllStoriesForUserAsync(long userId);
    }
}
