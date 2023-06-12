// <copyright file="UserSocialActivitiesRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Repositories
{
    using System.Net;
    using AutoMapper;
    using Facebook.CustomException;
    using Facebook.Infrastructure.Infrastructure;
    using Facebook.Interface;
    using Facebook.Model;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// UserSocialActivitiesRepository.
    /// </summary>
    public class UserSocialActivitiesRepository : IUserSocialActivitiesRepository
    {
        private readonly FacebookContext db;
        private readonly IUserRequestRepository iuserRequestRepository;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserSocialActivitiesRepository" /> class.
        /// </summary>
        /// <param name="facebookContext">The facebook context.</param>
        /// <param name="userRequestRepository">The user request repository.</param>
        /// <param name="mapper">The mapper.</param>
        public UserSocialActivitiesRepository(FacebookContext facebookContext, IUserRequestRepository userRequestRepository, IMapper mapper)
        {
            this.db = facebookContext;
            this.iuserRequestRepository = userRequestRepository;
            this.mapper = mapper;
        }

        /// <summary>
        /// Users the posts.
        /// </summary>
        /// <param name="posts">The posts.</param>
        /// <returns>all users Uploaded posts.</returns>
        /// <exception cref="Facebook.CustomException.AggregateValidationException">exception for valid user and atleast seletc one posts.</exception>
        public async Task<List<GetUserPostModel>> UserPosts(UserPostsModel posts)
        {
            List<ValidationsModel> errors = new();
            bool isUserExist = await this.iuserRequestRepository.ValidateUserById(posts.UserId);
            if (!isUserExist)
                errors.Add(new ValidationsModel { StatusCode = (int)HttpStatusCode.NotFound, ErrorMessage = "User Not Found" });

            if (posts.Posts.Count == 0)
                errors.Add(new ValidationsModel { StatusCode = (int)HttpStatusCode.Unauthorized, ErrorMessage = "Atleast Select One Posts." });

            if (errors.Any())
                throw new AggregateValidationException { Validations = errors };

            List<PostsWithTypes> postsWithTypes = await this.SavePostsToFolder(posts.Posts);
            List<GetUserPostModel> userPosts = postsWithTypes.Select(postsWithTypes => new GetUserPostModel
            {
                UserId = posts.UserId,
                MediaPath = postsWithTypes.MediaName,
                WrittenText = posts.PostText,
            }).ToList();

            List<UserPost> saveUserPost = this.mapper.Map<List<UserPost>>(userPosts);
            await this.db.UserPosts.AddRangeAsync(saveUserPost);
            await this.db.SaveChangesAsync();
            return userPosts;
        }

        /// <summary>
        /// Saves the posts to folder.
        /// </summary>
        /// <param name="files">The files.</param>
        /// <returns>posts and it's type.</returns>
        public async Task<List<PostsWithTypes>> SavePostsToFolder(List<IFormFile> files)
        {
            List<PostsWithTypes> postsWithTypes = new();

            List<Task> savingTasks = files.Select(async formFile =>
            {
                string type = Path.GetExtension(formFile.FileName);
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName);
                string uploadFile = Path.Combine(Directory.GetCurrentDirectory(), "Medias\\UsersPosts", fileName);

                using (var fileStream = new FileStream(uploadFile, FileMode.Create))
                    await formFile.CopyToAsync(fileStream);

                postsWithTypes.Add(new PostsWithTypes { MediaName = fileName, MediaType = type });
            }).ToList();

            await Task.WhenAll(savingTasks);
            return postsWithTypes;
        }

        /// <summary>
        /// Sends the user post.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>all the users posts.</returns>
        public async Task<List<GetUserPostModel>> SendUserPost(long userId)
        {
            List<GetUserPostModel> getUserPostModels = await this.db.UserPosts
                .Select(userPost => new GetUserPostModel
                {
                    UserId = userId,
                    WrittenText = userPost.WrittenText ?? string.Empty,
                    MediaPath = userPost.MediaPath,
                }).ToListAsync();
            return getUserPostModels;
        }
    }
}
