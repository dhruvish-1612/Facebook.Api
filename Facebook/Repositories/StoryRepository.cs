// <copyright file="StoryRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Repositories
{
    using System.Net;
    using AutoMapper;
    using Facebook.CustomException;
    using Facebook.Enums;
    using Facebook.Hubs;
    using Facebook.Infrastructure.Infrastructure;
    using Facebook.Interface;
    using Facebook.Model;
    using Facebook.ParameterModel;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Story Repository.
    /// </summary>
    public class StoryRepository : IStoryRepository
    {
        private readonly FacebookContext db;
        private readonly IMapper mapper;
        private readonly IUserRequestRepository userRequestRepository;
        private readonly INotificationRepository notificationRepository;
        private readonly IHubContext<FacebookHub> facebookHub;

        /// <summary>
        /// Initializes a new instance of the <see cref="StoryRepository" /> class.
        /// </summary>
        /// <param name="facebookContext">The facebook context.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="userRequestRepository">The user request repository.</param>
        /// <param name="notificationRepository">The notification repository.</param>
        public StoryRepository(FacebookContext facebookContext, IMapper mapper, IUserRequestRepository userRequestRepository, INotificationRepository notificationRepository, IHubContext<FacebookHub> facebookHub)
        {
            this.db = facebookContext;
            this.mapper = mapper;
            this.userRequestRepository = userRequestRepository;
            this.notificationRepository = notificationRepository;
            this.facebookHub = facebookHub;
        }

        /// <summary>
        /// Adds the story by user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="story">The story.</param>
        /// <returns>
        /// added story.
        /// </returns>
        public async Task<GetAllUserPostModel> AddStoryByUser(long userId, GetStoryModel story)
        {
            List<ValidationsModel> errors = new();
            User? user = await this.db.Users.FirstOrDefaultAsync(checkUser => checkUser.UserId == userId && checkUser.DeletedAt == null) ?? new User();
            if (user.UserId == 0)
                errors.Add(new ValidationsModel((int)HttpStatusCode.NotFound, "User Not Found"));

            if (story.Media == null)
                errors.Add(new ValidationsModel((int)HttpStatusCode.Unauthorized, "Atleast Select One Posts."));

            if (errors.Any())
                throw new AggregateValidationException { Validations = errors };

            StoryModel storyModel = await this.SaveStoryIntoFolder(story.Media);

            StoryModel addedStory = new()
            {
                UserId = userId,
                MediaPath = storyModel.MediaPath,
                MediaType = storyModel.MediaType,
                WrittenText = story.Text,
            };
            Story? saveStory = this.mapper.Map<Story>(addedStory);
            this.db.Stories.Add(saveStory);
            await this.db.SaveChangesAsync();

            GetAllUserPostModel returnStory = new()
            {
                Id = saveStory.StoryId,
                UserId = userId,
                UserName = user.FirstName + " " + user.LastName,
                UserAvtar = user.Avatar ?? string.Empty,
                PostMediaWithTypes = new List<PostsWithTypes> { new PostsWithTypes { MediaName = storyModel.MediaPath, MediaType = storyModel.MediaType } },
                WrittenText = story.Text,
                CreatedAt = DateTime.Now,
            };
            List<string> toUserIds = await this.notificationRepository.AddStoryNotification(saveStory.UserId, saveStory.StoryId);
            await this.facebookHub.Clients.Groups(toUserIds).SendAsync("SendNotification", returnStory);
            return returnStory;
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
        /// <param name="paginationParam">The get notification parameter.</param>
        /// <returns>
        /// get all the stories for that user.
        /// </returns>
        public async Task<Pagination<GetAllUserPostModel>> GetAllStoriesForUserAsync(long userId, PaginationParams paginationParam)
        {
            List<ValidationsModel> errors = new();
            bool isUserExist = await this.userRequestRepository.ValidateUserById(userId);
            if (!isUserExist)
                errors.Add(new ValidationsModel((int)HttpStatusCode.NotFound, "User Not Found"));

            if (errors.Any())
                throw new AggregateValidationException { Validations = errors };

            DateTime thresholdTime = DateTime.Now.AddDays(-1);
            IQueryable<Story> query;
            query = this.db.Friendships.Where(friend => (friend.ProfileAccept == userId || friend.ProfileRequest == userId)
                    && friend.DeletedAt == null && friend.IsFriend == true)
                   .SelectMany(friend => friend.ProfileAcceptNavigation.Stories.Concat(friend.ProfileRequestNavigation.Stories))
                   .Where(checkStoryTime => checkStoryTime.CreatedAt >= thresholdTime);

            if (!query.Any())
            {
                query = this.db.Stories.Where(story => story.UserId == userId && story.CreatedAt >= thresholdTime);
            }

            List<GetAllUserPostModel> getAllStoriesForUser = query.Select(story => new GetAllUserPostModel
            {
                Id = story.StoryId,
                UserId = story.UserId,
                UserName = story.User.FirstName + " " + story.User.LastName,
                UserAvtar = story.User.Avatar ?? string.Empty,
                PostMediaWithTypes = new List<PostsWithTypes> { new PostsWithTypes { MediaName = story.MediaPath, MediaType = story.MediaType } },
                WrittenText = story.WrittenText,
                CreatedAt = story.CreatedAt,
            }).OrderByDescending(user => user.UserId == userId).ToList();

            if (getAllStoriesForUser.Any())
                getAllStoriesForUser = getAllStoriesForUser.DistinctBy(story => story.Id).ToList();

            List<GetAllUserPostModel> getPaginatedAllStoriesForUser = getAllStoriesForUser.Skip((paginationParam.PageNumber - 1) * paginationParam.PageSize)
                                                                      .Take(paginationParam.PageSize).ToList();
            return new Pagination<GetAllUserPostModel>(getPaginatedAllStoriesForUser, getPaginatedAllStoriesForUser.Count, getAllStoriesForUser.Count);
        }

        /*public async Task<Pagination<GetAllUserPostModel>> GetAllStoriesForUserAsync(long userId, PaginationParams paginationParam)
        {
            List<ValidationsModel> errors = new();
            bool isUserExist = await this.userRequestRepository.ValidateUserById(userId);
            if (!isUserExist)
                errors.Add(new ValidationsModel((int)HttpStatusCode.NotFound, "User Not Found"));

            if (errors.Any())
                throw new AggregateValidationException { Validations = errors };

            DateTime thresholdTime = DateTime.Now.AddDays(-1);
            IQueryable<Story> query;
            query = this.db.Friendships.Where(friend => (friend.ProfileAccept == userId || friend.ProfileRequest == userId)
                    && friend.DeletedAt == null && friend.IsFriend == true)
                   .SelectMany(friend => friend.ProfileAcceptNavigation.Stories.Concat(friend.ProfileRequestNavigation.Stories));
                   // .Where(checkStoryTime => checkStoryTime.CreatedAt >= thresholdTime);

            if (!query.Any())
            {
                query = this.db.Stories.Where(story => story.UserId == userId && story.CreatedAt >= thresholdTime);
            }

            List<GetAllUserPostModel> getAllStoriesForUser = query.Select(story => new GetAllUserPostModel
            {
                Id = story.StoryId,
                UserId = story.UserId,
                UserName = story.User.FirstName + " " + story.User.LastName,
                UserAvtar = story.User.Avatar ?? string.Empty,
                PostMediaWithTypes = new List<PostsWithTypes> { new PostsWithTypes { MediaName = story.MediaPath, MediaType = story.MediaType } },
                WrittenText = story.WrittenText,
                CreatedAt = story.CreatedAt,
            }).OrderByDescending(user => user.UserId == userId).ToList();

            if (getAllStoriesForUser.Any())
                getAllStoriesForUser = getAllStoriesForUser.DistinctBy(story => story.Id).ToList();

            List<GetAllUserPostModel> getPaginatedAllStoriesForUser = getAllStoriesForUser.Skip((paginationParam.PageNumber - 1) * paginationParam.PageSize)
                                                                      .Take(paginationParam.PageSize).ToList();
            return new Pagination<GetAllUserPostModel>(getPaginatedAllStoriesForUser, getPaginatedAllStoriesForUser.Count, getAllStoriesForUser.Count);
        }*/

        /// <summary>
        /// Gets the story by identifier.
        /// </summary>
        /// <param name="storyId">The story identifier.</param>
        /// <returns>
        /// get Story By Id.
        /// </returns>
        public async Task<GetAllUserPostModel> GetStoryById(long storyId)
        {
            GetAllUserPostModel? getStory = await this.db.Stories.Where(x => x.StoryId == storyId && x.DeletedAt == null).Select(story => new GetAllUserPostModel
            {
                Id = story.StoryId,
                UserId = story.UserId,
                UserName = $"{story.User.FirstName} {story.User.LastName}",
                UserAvtar = story.User.Avatar ?? string.Empty,
                PostMediaWithTypes = new List<PostsWithTypes> { new PostsWithTypes { MediaName = story.MediaPath ?? string.Empty, MediaType = story.MediaType ?? string.Empty } },
                WrittenText = story.WrittenText ?? string.Empty,
                CreatedAt = story.CreatedAt,
            }).FirstOrDefaultAsync();

            if (getStory == null)
                throw new AggregateValidationException { Validations = new List<ValidationsModel> { new ValidationsModel((int)HttpStatusCode.Unauthorized, "StoryId Is Invalid") } };

            return getStory;
        }

        /// <summary>
        /// Deltes the story.
        /// </summary>
        /// <param name="storyId">The story identifier.</param>
        /// <returns>true if story is delted successfully.</returns>
        /// <exception cref="Facebook.CustomException.AggregateValidationException">for validations.</exception>
        public async Task<bool> DeleteStory(long storyId)
        {
            List<ValidationsModel> errors = new();

            Story? story = await this.db.Stories.FindAsync(storyId);
            if (story == null)
            {
                errors.Add(new ValidationsModel((int)HttpStatusCode.NotFound, "Story Is Not Found."));
                throw new AggregateValidationException { Validations = errors };
            }

            string uploadfile = Path.Combine(Directory.GetCurrentDirectory(), "Medias\\UserStories", story.MediaPath);
            if (File.Exists(uploadfile))
            {
                await Task.Run(() => File.Delete(uploadfile));
            }

            story.DeletedAt = DateTime.Now;
            this.db.Stories.Update(story);
            await this.db.SaveChangesAsync();
            await this.notificationRepository.DeletePostOrStory(story.StoryId, (int)NotificationTypeEnum.AddStory);
            return true;
        }
    }
}
