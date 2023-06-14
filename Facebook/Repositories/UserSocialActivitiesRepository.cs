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
                errors.Add(new ValidationsModel((int)HttpStatusCode.NotFound, "User Not Found"));

            if (posts.Posts.Count == 0)
                errors.Add(new ValidationsModel((int)HttpStatusCode.Unauthorized, "Atleast Select One Posts."));

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

        /// <summary>
        /// Gets all user post.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>return all the users posts.</returns>
        public async Task<List<GetAllUserPostModel>> GetAllUserPost(long userId)
        {
            List<ValidationsModel> errors = new();
            bool isUserExist = await this.iuserRequestRepository.ValidateUserById(userId);
            if (!isUserExist)
                errors.Add(new ValidationsModel((int)HttpStatusCode.NotFound, "User Not Found"));

            if (errors.Any())
                throw new AggregateValidationException { Validations = errors };

            IQueryable<UserPost> query;

            query = this.db.Friendships
                .Where(friend => (friend.ProfileAccept == userId || friend.ProfileRequest == userId) && friend.IsFriend == true && friend.DeletedAt == null)
                .SelectMany(userpost => userpost.ProfileRequestNavigation.UserPosts.Concat(userpost.ProfileAcceptNavigation.UserPosts))
                .Where(post => post.DeletedAt == null);

            if (!query.Any())
            {
                query = this.db.UserPosts.Where(post => post.UserId == userId && post.DeletedAt == null);
            }

            List<GetAllUserPostModel> getAllPosts = await query.Select(posts => new GetAllUserPostModel
            {
                Id = posts.UserPostId,
                UserId = posts.UserId,
                UserName = posts.User.FirstName + " " + posts.User.LastName,
                UserAvtar = posts.User.Avatar ?? string.Empty,
                MediaPath = posts.MediaPath,
                WrittenText = posts.WrittenText ?? string.Empty,
                CreatedAt = posts.CreatedAt,
            }).OrderByDescending(sequence => sequence.CreatedAt).ToListAsync();

            return getAllPosts;
        }

        /// <summary>
        /// Deletes the post.
        /// </summary>
        /// <param name="postId">The post identifier.</param>
        /// <returns>true if successfully deleted.</returns>
        /// <exception cref="Facebook.CustomException.AggregateValidationException">for validation.</exception>
        public async Task<bool> DeletePost(long postId)
        {
            List<ValidationsModel> errors = new();

            UserPost? userPost = await this.db.UserPosts.FindAsync(postId);
            if (userPost == null)
            {
                errors.Add(new ValidationsModel { StatusCode = (int)HttpStatusCode.NotFound, ErrorMessage = "Post Is Not Found." });
                throw new AggregateValidationException { Validations = errors };
            }

            userPost.DeletedAt = DateTime.Now;
            this.db.UserPosts.Update(userPost);
            await this.db.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Adds the comment.
        /// </summary>
        /// <param name="comment">The comment.</param>
        /// <returns>return recently added comment.</returns>
        public async Task<GetPostCommentModel> UpsertComment(AddCommentModel comment)
        {
            List<ValidationsModel> errors = new();

            bool isUserExist = await this.iuserRequestRepository.ValidateUserById(comment.UserId);
            if (!isUserExist)
                errors.Add(new ValidationsModel { StatusCode = (int)HttpStatusCode.NotFound, ErrorMessage = "User Not Found." });
            if (string.IsNullOrWhiteSpace(comment.CommentText))
                errors.Add(new ValidationsModel { StatusCode = (int)HttpStatusCode.NoContent, ErrorMessage = "Please add Comments." });
            if (errors.Any())
                throw new AggregateValidationException { Validations = errors };

            PostComment? postComment;
            if (comment.UserPostCommentId == 0)
            {
                postComment = this.mapper.Map<PostComment>(comment);
                this.db.PostComments.Add(postComment);
            }
            else
            {
                postComment = await this.db.PostComments.FirstOrDefaultAsync(fetchComment => fetchComment.UserPostCommentId == comment.UserPostCommentId
                              && fetchComment.DeletedAt == null);
                if (postComment == null)
                {
                    errors.Add(new ValidationsModel { StatusCode = (int)HttpStatusCode.NotFound, ErrorMessage = "this comment is not found." });
                    throw new AggregateValidationException { Validations = errors };
                }

                if (postComment.UserId != comment.UserId)
                    errors.Add(new ValidationsModel { StatusCode = (int)HttpStatusCode.Unauthorized, ErrorMessage = "Don't change User." });
                if (errors.Any())
                    throw new AggregateValidationException { Validations = errors };

                postComment.CommentText = comment.CommentText;
                postComment.UpdatedAt = DateTime.Now;
                this.db.PostComments.Update(postComment);
            }

            await this.db.SaveChangesAsync();
            return await this.GetPostCommetnById(postComment.UserPostCommentId);
        }

        /// <summary>
        /// Deletes the comment.
        /// </summary>
        /// <param name="userPostCommentId">The user post comment identifier.</param>
        /// <returns>true if successfully deleted.</returns>
        /// <exception cref="Facebook.CustomException.AggregateValidationException">for validations.</exception>
        public async Task<bool> DeleteComment(long userPostCommentId)
        {
            List<ValidationsModel> errors = new();
            PostComment? userPostComment = await this.db.PostComments.FindAsync(userPostCommentId);
            if (userPostComment == null)
            {
                errors.Add(new ValidationsModel((int)HttpStatusCode.NoContent, "Please add Comments."));
                throw new AggregateValidationException { Validations = errors };
            }

            userPostComment.DeletedAt = DateTime.Now;
            this.db.PostComments.Update(userPostComment);
            await this.db.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Gets the post comments.
        /// </summary>
        /// <param name="postId">The post identifier.</param>
        /// <returns>get all comment for that posts.</returns>
        /// <exception cref="Facebook.CustomException.AggregateValidationException">for validatoions.</exception>
        public async Task<List<GetPostCommentModel>> GetPostComments(long postId)
        {
            List<ValidationsModel> errors = new();
            bool isPostValid = await this.ValidatePost(postId);
            if (!isPostValid)
            {
                errors.Add(new ValidationsModel((int)HttpStatusCode.NotFound, "Post Is Not Found."));
                throw new AggregateValidationException { Validations = errors };
            }

            List<GetPostCommentModel> getPostComments = await this.db.PostComments.Where(comment => comment.UserPostId == postId && comment.DeletedAt == null)
                .Select(getComment => new GetPostCommentModel
                {
                    UserPostCommentId = getComment.UserPostCommentId,
                    UserPostId = getComment.UserPostId,
                    UserId = getComment.UserId,
                    CommentedUserName = getComment.User.FirstName + " " + getComment.User.LastName,
                    CommentedUserAvtar = getComment.User.Avatar,
                    CommentText = getComment.CommentText,
                    CreatedAt = getComment.CreatedAt,
                }).OrderBy(sequence => sequence.CreatedAt).ToListAsync();
            return getPostComments;
        }

        /// <summary>
        /// Validates the post.
        /// </summary>
        /// <param name="postId">The post identifier.</param>
        /// <returns>true if post is valid o/w false.</returns>
        public async Task<bool> ValidatePost(long postId)
        {
            bool isPostExist = await Task.Run(() => this.db.UserPosts.AnyAsync(checkPost => checkPost.UserPostId == postId));
            return isPostExist;
        }

        /// <summary>
        /// Gets the post commetn by identifier.
        /// </summary>
        /// <param name="postCommentId">The post comment identifier.</param>
        /// <returns>Get Comment By it's Id.</returns>
        public async Task<GetPostCommentModel> GetPostCommetnById(long postCommentId)
        {
            GetPostCommentModel getPostComments = await this.db.PostComments.Where(comment => comment.UserPostCommentId == postCommentId && comment.DeletedAt == null)
                .Select(getComment => new GetPostCommentModel
                {
                    UserPostCommentId = getComment.UserPostCommentId,
                    UserPostId = getComment.UserPostId,
                    UserId = getComment.UserId,
                    CommentedUserName = getComment.User.FirstName + " " + getComment.User.LastName,
                    CommentedUserAvtar = getComment.User.Avatar,
                    CommentText = getComment.CommentText,
                    CreatedAt = getComment.CreatedAt,
                }).FirstOrDefaultAsync() ?? new GetPostCommentModel();
            return getPostComments;
        }

        /// <summary>
        /// Likes the or dislike post.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="postId">The post identifier.</param>
        /// <returns>true if successfully like and dislike.</returns>
        /// <exception cref="Facebook.CustomException.AggregateValidationException">for validation.</exception>
        public async Task<bool> LikeOrDislikePost(long userId, long postId)
        {
            List<ValidationsModel> errors = new();

            bool isUserExist = await this.iuserRequestRepository.ValidateUserById(userId);
            if (!isUserExist)
                errors.Add(new ValidationsModel((int)HttpStatusCode.NotFound, "User Not Found."));
            bool isPostValid = await this.ValidatePost(postId);
            if (!isPostValid)
                errors.Add(new ValidationsModel((int)HttpStatusCode.NotFound, "Post Is Not Found."));
            if (errors.Any())
                throw new AggregateValidationException { Validations = errors };
            PostLike postLike = await this.db.PostLikes.FirstOrDefaultAsync(like => like.LikeUserId == userId && like.UserPostId == postId) ?? new PostLike();

            if (postLike.UserPostLikeId == 0)
            {
                postLike.LikeUserId = userId;
                postLike.LikeStatus = true;
                postLike.UserPostId = postId;
                postLike.LikeDate = DateTime.Now;
                this.db.PostLikes.Add(postLike);
            }
            else
            {
                postLike.LikeStatus = postLike.LikeStatus == false ? true : false;
                postLike.LikeDate = DateTime.Now;
                this.db.PostLikes.Update(postLike);
            }

            await this.db.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Gets the post likes.
        /// </summary>
        /// <param name="postId">The post identifier.</param>
        /// <returns>get likes of that post.</returns>
        public async Task<List<GetUserPostLikeModel>> GetPostLikes(long postId)
        {
            List<ValidationsModel> errors = new();

            bool isPostValid = await this.ValidatePost(postId);
            if (!isPostValid)
                errors.Add(new ValidationsModel((int)HttpStatusCode.NotFound, "Post Is Not Found."));
            if (errors.Any())
                throw new AggregateValidationException { Validations = errors };

            List<GetUserPostLikeModel> getUserPostLikes = await this.db.PostLikes.Where(like => like.UserPostId == postId && like.LikeStatus == true)
                .Select(getLike => new GetUserPostLikeModel
                {
                    UserPostLikeId = getLike.UserPostId,
                    UserPostId = getLike.UserPostId,
                    LikedUserId = getLike.LikeUserId,
                    LikedUserName = getLike.LikeUser.FirstName + " " + getLike.LikeUser.LastName,
                    LikedUserAvtar = getLike.LikeUser.Avatar,
                    CreatedAt = getLike.LikeDate,
                }).ToListAsync();
            return getUserPostLikes;
        }
    }
}
