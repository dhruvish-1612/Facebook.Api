// <copyright file="UserSocialActivitiesRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Repositories
{
    using System.Net;
    using AutoMapper;
    using Facebook.CustomException;
    using Facebook.Hubs;
    using Facebook.Infrastructure.Infrastructure;
    using Facebook.Interface;
    using Facebook.Model;
    using Facebook.ParameterModel;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// UserSocialActivitiesRepository.
    /// </summary>
    public class UserSocialActivitiesRepository : IUserSocialActivitiesRepository
    {
        private readonly FacebookContext db;
        private readonly IUserRequestRepository iuserRequestRepository;
        private readonly IMapper mapper;
        private readonly INotificationRepository notificationRepository;
        private readonly IHubContext<FacebookHub> HubContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserSocialActivitiesRepository" /> class.
        /// </summary>
        /// <param name="facebookContext">The facebook context.</param>
        /// <param name="userRequestRepository">The user request repository.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="notificationRepository">The notification repository.</param>
        /// <param name="facebookHub">The facebook hub.</param>
        public UserSocialActivitiesRepository(FacebookContext facebookContext, IUserRequestRepository userRequestRepository, IMapper mapper, INotificationRepository notificationRepository, IHubContext<FacebookHub> facebookHub)
        {
            this.db = facebookContext;
            this.iuserRequestRepository = userRequestRepository;
            this.mapper = mapper;
            this.notificationRepository = notificationRepository;
            this.HubContext = facebookHub;
        }

        /// <summary>
        /// Users the posts.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="posts">The posts.</param>
        /// <returns>
        /// all users Uploaded posts.
        /// </returns>
        /// <exception cref="Facebook.CustomException.AggregateValidationException">exception for valid user and atleast seletc one posts.</exception>
        public async Task<GetAllUserPostModel> UserPosts(long userId, UserPostsModel posts)
        {
            List<ValidationsModel> errors = new();
            User? user = await this.db.Users.FirstOrDefaultAsync(checkUser => checkUser.UserId == userId) ?? new User();
            if (user.UserId == 0)
                errors.Add(new ValidationsModel((int)HttpStatusCode.NotFound, "User Not Found"));

            if (!posts.Posts.Any() && string.IsNullOrWhiteSpace(posts.PostText))
                errors.Add(new ValidationsModel((int)HttpStatusCode.Unauthorized, "No Posts Or Description Provided."));

            if (errors.Any())
                throw new AggregateValidationException { Validations = errors };

            UserPost userPost = new()
            {
                UserId = userId,
                Description = posts.PostText,
            };
            await this.db.UserPosts.AddAsync(userPost);
            await this.db.SaveChangesAsync();

            List<PostMediaModel> userMediaPosts = new();
            List<PostsWithTypes> postsWithTypes = new();
            if (posts.Posts.Any())
            {
                postsWithTypes = await this.SavePostsToFolder(posts.Posts);
                userMediaPosts = postsWithTypes.Select(postsWithTypes => new PostMediaModel
                {
                    UserPostId = userPost.UserPostId,
                    MediaPath = postsWithTypes.MediaName,
                    MediaType = postsWithTypes.MediaType,
                }).ToList();

                List<PostsMedium> postsMedium = this.mapper.Map<List<PostsMedium>>(userMediaPosts);
                await this.db.PostsMedia.AddRangeAsync(postsMedium);
                await this.db.SaveChangesAsync();
            }

            GetAllUserPostModel getAllUserPost = new()
            {
                Id = userPost.UserPostId,
                UserId = userId,
                UserName = $"{user.FirstName} {user.LastName}",
                UserAvtar = user.Avatar ?? string.Empty,
                WrittenText = posts.PostText,
                PostMediaWithTypes = postsWithTypes,
                CreatedAt = DateTime.Now,
            };
            List<string> touserIds = await this.notificationRepository.AddPostNotification(userPost.UserId, userPost.UserPostId);
            await this.HubContext.Clients.Groups(touserIds).SendAsync("SendNotification", getAllUserPost);
            return getAllUserPost;
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
            List<GetUserPostModel> getUserPostModels = await this.db.UserPosts.Where(media => media.DeletedAt == null)
                .Select(userPost => new GetUserPostModel
                {
                    UserId = userId,
                    Description = userPost.Description ?? string.Empty,
                    PostMediaWithTypes = userPost.PostsMedia.Select(media => new PostsWithTypes
                    {
                        MediaName = media.MediaPath ?? string.Empty,
                        MediaType = media.MediaType ?? string.Empty,
                    }).ToList(),
                }).ToListAsync();
            return getUserPostModels;
        }

        /// <summary>
        /// Gets all user post.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="postParams">The post parameters.</param>
        /// <returns>
        /// get all user posts.
        /// </returns>
        public async Task<Pagination<GetAllUserPostModel>> GetAllUserPost(long userId, PostParams postParams)
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
                .Where(post => post.DeletedAt == null).Include(user => user.User);

            if (!await query.AnyAsync() || postParams.IsUserPosts)
            {
                query = this.db.UserPosts.Where(post => post.UserId == userId && post.DeletedAt == null).Include(user => user.User);
            }

            List<UserPost> userPosts = await query.ToListAsync();
            List<GetAllUserPostModel> getAllPosts = userPosts.Select(posts => new GetAllUserPostModel
            {
                Id = posts.UserPostId,
                UserId = posts.UserId,
                UserName = $"{posts.User.FirstName} {posts.User.LastName}",
                UserAvtar = posts.User.Avatar ?? string.Empty,
                PostMediaWithTypes = this.db.PostsMedia.Where(x => x.UserPostId == posts.UserPostId).Select(media => new PostsWithTypes
                {
                    MediaName = media.MediaPath ?? string.Empty,
                    MediaType = media.MediaType ?? string.Empty,
                }).ToList(),
                WrittenText = posts.Description ?? string.Empty,
                CreatedAt = posts.CreatedAt,
            }).OrderByDescending(sequence => sequence.CreatedAt).ToList();

            if (getAllPosts.Any())
                getAllPosts = getAllPosts.DistinctBy(posts => posts.Id).ToList();

            List<GetAllUserPostModel> getPaginatedAllPosts = getAllPosts.Skip((postParams.PaginationParams.PageNumber - 1) * postParams.PaginationParams.PageSize).Take(postParams.PaginationParams.PageSize).ToList();

            return new Pagination<GetAllUserPostModel>(
                getPaginatedAllPosts,
                getPaginatedAllPosts.Count,
                getAllPosts.Count);
        }

        /*public async Task<Pagination<GetAllUserPostModel>> GetAllUserPost(long userId, PostParams postParams)
        {
            List<ValidationsModel> errors = new();
            bool isUserExist = await this.iuserRequestRepository.ValidateUserById(userId);
            if (!isUserExist)
                errors.Add(new ValidationsModel((int)HttpStatusCode.NotFound, "User Not Found"));

            if (errors.Any())
                throw new AggregateValidationException { Validations = errors };
            var query = from friend in this.db.Friendships
                        where (friend.ProfileAccept == userId || friend.ProfileRequest == userId) && friend.IsFriend == true && friend.DeletedAt == null
                        select friend;

            var friendPostsQuery = from friend in query
                                   from userPost in friend.ProfileRequest == userId ? friend.ProfileAcceptNavigation.UserPosts : friend.ProfileRequestNavigation.UserPosts
                                   select userPost;

            var userPostsQuery = from post in query
                                 where post.ProfileAccept == userId || post.ProfileRequest == userId
                                 select post.ProfileRequest.

            var userPosts = friendPostsQuery.AsEnumerable();
            var getAllPosts = userPosts.Select(posts => new GetAllUserPostModel
            {
                Id = posts.UserPostId,
                UserId = posts.UserId,
                UserName = $"{posts.User.FirstName} {posts.User.LastName}",
                UserAvtar = posts.User.Avatar ?? string.Empty,
                PostMediaWithTypes = this.db.PostsMedia.Where(x => x.UserPostId == posts.UserPostId).Select(media => new PostsWithTypes
                {
                    MediaName = media.MediaPath ?? string.Empty,
                    MediaType = media.MediaType ?? string.Empty,
                }).ToList(),
                WrittenText = posts.Description ?? string.Empty,
                CreatedAt = posts.CreatedAt,
            }).OrderByDescending(sequence => sequence.CreatedAt).ToList();

            if (getAllPosts.Any())
                getAllPosts = getAllPosts.DistinctBy(posts => posts.Id).ToList();

            List<GetAllUserPostModel> getPaginatedAllPosts = getAllPosts.Skip((postParams.PaginationParams.PageNumber - 1) * postParams.PaginationParams.PageSize).Take(postParams.PaginationParams.PageSize).ToList();

            return new Pagination<GetAllUserPostModel>(
                getPaginatedAllPosts,
                getPaginatedAllPosts.Count,
                getAllPosts.Count);
        }*/

        /// <summary>
        /// Gets the post by identifier.
        /// </summary>
        /// <param name="postId">The post identifier.</param>
        /// <returns>get post by Id.</returns>
        /// <exception cref="AggregateValidationException">for validations.</exception>
        public async Task<GetAllUserPostModel> GetPostById(long postId)
        {
            GetAllUserPostModel? getPost = await this.db.UserPosts.Where(x => x.UserPostId == postId && x.DeletedAt == null).Select(post => new GetAllUserPostModel
            {
                Id = post.UserPostId,
                UserId = post.UserId,
                UserName = $"{post.User.FirstName} {post.User.LastName}",
                UserAvtar = post.User.Avatar ?? string.Empty,
                PostMediaWithTypes = post.PostsMedia.Select(post => new PostsWithTypes { MediaName = post.MediaPath ?? string.Empty, MediaType = post.MediaType ?? string.Empty }).ToList(),
                WrittenText = post.Description ?? string.Empty,
                CreatedAt = post.CreatedAt,
            }).FirstOrDefaultAsync();

            if (getPost == null)
                throw new AggregateValidationException { Validations = new List<ValidationsModel> { new ValidationsModel((int)HttpStatusCode.Unauthorized, "PostId Is Invalid") } };

            return getPost;
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

            UserPost? userPost = await this.db.UserPosts.Include(post => post.PostsMedia).FirstOrDefaultAsync(post => post.UserPostId == postId);

            if (userPost == null)
            {
                errors.Add(new ValidationsModel { StatusCode = (int)HttpStatusCode.NotFound, ErrorMessage = "Post Is Not Found." });
                throw new AggregateValidationException { Validations = errors };
            }

            this.DeleteExistingPost(userPost.PostsMedia.Select(post => post.MediaPath).ToList());
            userPost.DeletedAt = DateTime.Now;
            this.db.UserPosts.Update(userPost);
            await this.db.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Deletes the existing post.
        /// </summary>
        /// <param name="postName">Name of the post.</param>
        /// <returns>nothing.</returns>
        public async Task DeleteExistingPost(List<string?> postName)
        {
            if (postName != null)
            {
                List<Task> deletePost = postName.Select(async post =>
                {
                    string uploadfile = Path.Combine(Directory.GetCurrentDirectory(), "Medias\\UsersPosts", post);
                    if (File.Exists(uploadfile))
                    {
                        await Task.Run(() => File.Delete(uploadfile));
                    }
                }).ToList();
                await Task.WhenAll(deletePost);
            }
        }

        /// <summary>
        /// Adds the comment.
        /// </summary>
        /// <param name="userId">userid.</param>
        /// <param name="comment">The comment.</param>
        /// <returns>
        /// return recently added comment.
        /// </returns>
        public async Task<GetPostCommentModel> UpsertComment(long userId, AddCommentModel comment)
        {
            List<ValidationsModel> errors = new();

            User? user = await this.db.Users.FirstOrDefaultAsync(checkUser => checkUser.UserId == userId) ?? new User();
            if (user.UserId == 0)
                errors.Add(new ValidationsModel { StatusCode = (int)HttpStatusCode.NotFound, ErrorMessage = "User Not Found." });
            if (string.IsNullOrWhiteSpace(comment.CommentText))
                errors.Add(new ValidationsModel { StatusCode = (int)HttpStatusCode.NoContent, ErrorMessage = "Please add Comments." });
            if (errors.Any())
                throw new AggregateValidationException { Validations = errors };

            PostComment? postComment = new();
            string toUserId = string.Empty;
            if (comment.UserPostCommentId == 0)
            {
                postComment.UserPostId = comment.UserPostId;
                postComment.CommentText = comment.CommentText;
                postComment.UserId = userId;
                this.db.PostComments.Add(postComment);
                await this.db.SaveChangesAsync();
                toUserId = await this.notificationRepository.AddCommentNotification(postComment.UserPostCommentId);
            }
            else
            {
                postComment = await this.db.PostComments.FirstOrDefaultAsync(fetchComment => fetchComment.UserPostCommentId == comment.UserPostCommentId
                              && fetchComment.DeletedAt == null);

                if (postComment == null)
                    throw new AggregateValidationException { Validations = new List<ValidationsModel> { new ValidationsModel((int)HttpStatusCode.NotFound, "this comment is not found.") } };

                if (postComment.UserId != userId)
                    throw new AggregateValidationException { Validations = new List<ValidationsModel> { new ValidationsModel((int)HttpStatusCode.Unauthorized, "Comment can not be edited by another user.") } };

                postComment.CommentText = comment.CommentText;
                postComment.UpdatedAt = DateTime.Now;
                this.db.PostComments.Update(postComment);
                await this.db.SaveChangesAsync();
            }

            GetPostCommentModel getPostComment = new()
            {
                UserPostCommentId = postComment.UserPostCommentId,
                UserPostId = postComment.UserPostId,
                UserId = user.UserId,
                CommentedUserName = $"{user.FirstName} {user.LastName}",
                CommentedUserAvtar = $"{user.Avatar}",
                CommentText = postComment.CommentText,
                CreatedAt = postComment.CreatedAt,
            };
            await this.HubContext.Clients.Group(toUserId).SendAsync("SendNotification", getPostComment);
            return getPostComment;
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
                errors.Add(new ValidationsModel((int)HttpStatusCode.NoContent, "This Type Of Comment Is Not Found."));
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
        /// <param name="getCommentsParam">The get comments parameter.</param>
        /// <returns>
        /// get all comment for that posts.
        /// </returns>
        /// <exception cref="Facebook.CustomException.AggregateValidationException">for validatoions.</exception>
        public async Task<Pagination<GetPostCommentModel>> GetPostComments(GetCommentsParam getCommentsParam)
        {
            bool isPostValid = await this.ValidatePost(getCommentsParam.PostId);
            if (!isPostValid)
            {
                throw new AggregateValidationException { Validations = new List<ValidationsModel> { new ValidationsModel((int)HttpStatusCode.NotFound, "Post Is Not Found.") } };
            }

            List<GetPostCommentModel> getPostComments = await this.db.PostComments.Where(comment => comment.UserPostId == getCommentsParam.PostId && comment.DeletedAt == null)
                .Select(getComment => new GetPostCommentModel
                {
                    UserPostCommentId = getComment.UserPostCommentId,
                    UserPostId = getComment.UserPostId,
                    UserId = getComment.UserId,
                    CommentedUserName = getComment.User.FirstName + " " + getComment.User.LastName,
                    CommentedUserAvtar = getComment.User.Avatar,
                    CommentText = getComment.CommentText,
                    CreatedAt = getComment.CreatedAt,
                }).OrderByDescending(sequence => sequence.CreatedAt).ToListAsync();

            List<GetPostCommentModel> getPaginatedComments = getPostComments.Skip((getCommentsParam.PaginationParams.PageNumber - 1) * getCommentsParam.PaginationParams.PageSize)
                                                            .Take(getCommentsParam.PaginationParams.PageSize).ToList();

            return new Pagination<GetPostCommentModel>(getPaginatedComments, getPaginatedComments.Count, getPostComments.Count);
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
            GetPostCommentModel? getPostComments = await this.db.PostComments.Where(comment => comment.UserPostCommentId == postCommentId && comment.DeletedAt == null)
                .Select(getComment => new GetPostCommentModel
                {
                    UserPostCommentId = getComment.UserPostCommentId,
                    UserPostId = getComment.UserPostId,
                    UserId = getComment.UserId,
                    CommentedUserName = getComment.User.FirstName + " " + getComment.User.LastName,
                    CommentedUserAvtar = getComment.User.Avatar,
                    CommentText = getComment.CommentText,
                    CreatedAt = getComment.CreatedAt,
                }).FirstOrDefaultAsync();

            if (getPostComments == null)
                throw new AggregateValidationException { Validations = new List<ValidationsModel> { new ValidationsModel((int)HttpStatusCode.Unauthorized, "CommentId Is Invalid") } };

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

            PostLike? postLike = await this.db.PostLikes.FirstOrDefaultAsync(like => like.LikeUserId == userId && like.UserPostId == postId) ?? new PostLike();

            if (postLike.UserPostLikeId == 0)
            {
                postLike.LikeUserId = userId;
                postLike.LikeStatus = true;
                postLike.UserPostId = postId;
                postLike.LikeDate = DateTime.Now;
                this.db.PostLikes.Add(postLike);
                await this.db.SaveChangesAsync();
                string postUserId = await this.notificationRepository.AddLikeNotification(postLike.UserPostLikeId);
                GetUserPostLikeModel getUserPostLike = await this.GetLikeById(postLike.UserPostLikeId);
                await this.HubContext.Clients.Group(postUserId).SendAsync("SendNotification", getUserPostLike);
                return true;
            }
            else
            {
                postLike.LikeStatus = !postLike.LikeStatus;
                postLike.LikeDate = DateTime.Now;
                this.db.PostLikes.Update(postLike);
                await this.db.SaveChangesAsync();
                string postUserId = await this.notificationRepository.AddLikeNotification(postLike.UserPostLikeId);
                GetUserPostLikeModel getUserPostLike = await this.GetLikeById(postLike.UserPostLikeId);
                await this.HubContext.Clients.Group(postUserId).SendAsync("SendNotification", getUserPostLike);
                return postLike.LikeStatus == false ? false : true;
            }
        }

        /// <summary>
        /// Gets the post likes.
        /// </summary>
        /// <param name="postId">The post identifier.</param>
        /// <returns>get likes of that post.</returns>
        public async Task<GetUserPostLikeWithCountModel> GetPostLikes(long postId)
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
                    LikeStatus = getLike.LikeStatus,
                    CreatedAt = getLike.LikeDate,
                }).ToListAsync();

            GetUserPostLikeWithCountModel getPostLikesWithCount = new()
            {
                GetUserPostLikes = getUserPostLikes,
                LikesCount = getUserPostLikes.Count,
            };
            return getPostLikesWithCount;
        }

        /// <summary>
        /// Gets the like by identifier.
        /// </summary>
        /// <param name="likeId">The like identifier.</param>
        /// <returns>get like By Id.</returns>
        /// <exception cref="Facebook.Model.ValidationsModel">CommentId Is Invalid.</exception>
        public async Task<GetUserPostLikeModel> GetLikeById(long likeId)
        {
            GetUserPostLikeModel? getUserPostLikes = await this.db.PostLikes.Where(like => like.UserPostLikeId == likeId)
               .Select(getLike => new GetUserPostLikeModel
               {
                   UserPostLikeId = getLike.UserPostId,
                   UserPostId = getLike.UserPostId,
                   LikedUserId = getLike.LikeUserId,
                   LikedUserName = getLike.LikeUser.FirstName + " " + getLike.LikeUser.LastName,
                   LikedUserAvtar = getLike.LikeUser.Avatar,
                   LikeStatus = getLike.LikeStatus,
                   CreatedAt = getLike.LikeDate,
               }).FirstOrDefaultAsync();

            if (getUserPostLikes == null)
                throw new AggregateValidationException { Validations = new List<ValidationsModel> { new ValidationsModel((int)HttpStatusCode.Unauthorized, "CommentId Is Invalid") } };

            return getUserPostLikes;
        }
    }
}