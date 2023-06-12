// <copyright file="StoryRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Repositories
{
    using AutoMapper;
    using Facebook.CustomException;
    using Facebook.Infrastructure.Infrastructure;
    using Facebook.Interface;
    using Facebook.Model;
    using Microsoft.EntityFrameworkCore;
    using System.Net;

    /// <summary>
    /// Story Repository.
    /// </summary>
    public class StoryRepository : IStoryRepository
    {
        private readonly FacebookContext db;
        private readonly IMapper mapper;
        private readonly IUserRequestRepository userRequestRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="StoryRepository" /> class.
        /// </summary>
        /// <param name="facebookContext">The facebook context.</param>
        /// <param name="mapper">The mapper.</param>
        public StoryRepository(FacebookContext facebookContext, IMapper mapper, IUserRequestRepository userRequestRepository)
        {
            this.db = facebookContext;
            this.mapper = mapper;
            this.userRequestRepository = userRequestRepository;
        }

        /// <summary>
        /// Adds the story by user.
        /// </summary>
        /// <param name="story">The story.</param>
        /// <returns>added story.</returns>
        public async Task<StoryModel> AddStoryByUser(GetStoryModel story)
        {
            List<ValidationsModel> errors = new();
            bool isUserExist = await this.userRequestRepository.ValidateUserById(story.UserId);
            if (!isUserExist)
                errors.Add(new ValidationsModel { StatusCode = (int)HttpStatusCode.NotFound, ErrorMessage = "User Not Found" });

            if (story.Media == null)
                errors.Add(new ValidationsModel { StatusCode = (int)HttpStatusCode.Unauthorized, ErrorMessage = "Atleast Select One Posts." });

            if (errors.Any())
                throw new AggregateValidationException { Validations = errors };

            StoryModel storyModel = await this.SaveStoryIntoFolder(story.Media);

            StoryModel addedStory = new()
            {
                UserId = story.UserId,
                MediaPath = storyModel.MediaPath,
                MediaType = storyModel.MediaType,
                WrittenText = story.Text,
            };
            Story? saveStory = this.mapper.Map<Story>(addedStory);
            this.db.Stories.Add(saveStory);
            await this.db.SaveChangesAsync();
            return addedStory;
        }

        /// <summary>
        /// Saves the story into folder.
        /// </summary>
        /// <param name="formFile">The form file.</param>
        /// <returns>return media path and type.</returns>
        public async Task<StoryModel> SaveStoryIntoFolder(IFormFile formFile)
        {
            string type = Path.GetExtension(formFile.FileName);
            string fileName = Guid.NewGuid().ToString() + type;
            string uploadFile = Path.Combine(Directory.GetCurrentDirectory(), "Medias\\UserStories", fileName);

            using (var fileStream = new FileStream(uploadFile, FileMode.Create))
                await formFile.CopyToAsync(fileStream);

            return new StoryModel { MediaPath = fileName, MediaType = type };
        }

        /// <summary>
        /// Gets all stories for user asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>get all the stories for that user.</returns>
        public async Task<List<GetAllStoriesForUserModel>> GetAllStoriesForUserAsync(long userId)
        {
            List<GetAllStoriesForUserModel> getAllStoriesForUserModels = await this.db.Friendships
                .Where(friend => (friend.ProfileAccept == userId || friend.ProfileRequest == userId) && friend.DeletedAt == null && friend.IsFriend == true)
                .SelectMany(friend => friend.ProfileAcceptNavigation.Stories.Concat(friend.ProfileRequestNavigation.Stories))
                .Select(story => new GetAllStoriesForUserModel
                {
                    StoryId = story.StoryId,
                    UserId = story.UserId,
                    UserName = story.User.FirstName + " " + story.User.LastName,
                    UserAvtar = story.User.Avatar ?? string.Empty,
                    MediaPath = story.MediaPath,
                    WrittenText = story.WrittenText,
                    CreatedAt = story.CreatedAt,
                }).ToListAsync();

            return getAllStoriesForUserModels;
        }
    }
}
