// <copyright file="UserSocialActivitiesRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Repositories
{
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

        /// <summary>
        /// Initializes a new instance of the <see cref="UserSocialActivitiesRepository"/> class.
        /// </summary>
        /// <param name="facebookContext">The facebook context.</param>
        public UserSocialActivitiesRepository(FacebookContext facebookContext)
        {
            this.db = facebookContext;
        }

        /// <summary>
        /// Users the posts.
        /// </summary>
        /// <param name="posts">The posts.</param>
        public async Task UserPosts(UserPostsModel posts)
        {
            List<PostsWithTypes> postsWithTypes = await this.SavePostsToFolder(posts.Posts);
            List<UserPost> userPosts = postsWithTypes.Select(postsWithTypes => new UserPost
            {
                UserId = posts.UserId,
                MediaPath = postsWithTypes.MediaName,
                WrittenText = posts.PostText,
            }).ToList();

            await this.db.UserPosts.AddRangeAsync(userPosts);
            await this.db.SaveChangesAsync();
        }

        /// <summary>
        /// Saves the posts to folder.
        /// </summary>
        /// <param name="files">The files.</param>
        /// <returns></returns>
        public async Task<List<PostsWithTypes>> SavePostsToFolder(List<IFormFile> files)
        {
            List<PostsWithTypes> postsWithTypes = new();

            List<Task> savingTasks = files.Select(async formFile =>
            {
                string type = Path.GetExtension(formFile.FileName);
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName);
                string uploadFile = Path.Combine(Directory.GetCurrentDirectory(), "Medias\\UsersPosts", fileName);

                using var fileStream = new FileStream(uploadFile, FileMode.Create);
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
        /// <returns></returns>
        public async Task<List<GetUserPostModel>> SendUserPost(long userId)
        {
            List<GetUserPostModel> getUserPostModels = await this.db.UserPosts
                .Select(userPost => new GetUserPostModel
                {
                    UserId = userId,
                    PostText = userPost.WrittenText ?? string.Empty,
                    MediaPath = userPost.MediaPath,
                }).ToListAsync();
            return getUserPostModels;
        }
    }
}
