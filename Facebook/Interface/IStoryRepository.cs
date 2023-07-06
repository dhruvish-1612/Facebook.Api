// <copyright file="IStoryRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Interface
{
    using System.Collections.Generic;
    using Facebook.Model;
    using Facebook.ParameterModel;

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
        Task<GetAllUserPostModel> AddStoryByUser(long userId, GetStoryModel story);

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
        /// <param name="paginationParams">The pagination parameters.</param>
        /// <returns>
        /// get all the stories.
        /// </returns>
        Task<Pagination<GetAllUserPostModel>> GetAllStoriesForUserAsync(long userId, PaginationParams paginationParams);

        /// <summary>
        /// Gets the story by identifier.
        /// </summary>
        /// <param name="storyId">The story identifier.</param>
        /// <returns>get Story By Id.</returns>
        Task<GetAllUserPostModel> GetStoryById(long storyId);
    }
}
