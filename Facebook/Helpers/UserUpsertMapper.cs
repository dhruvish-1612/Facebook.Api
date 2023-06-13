// <copyright file="UserUpsertMapper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Helpers
{
    using AutoMapper;
    using Facebook.Infrastructure.Infrastructure;
    using Facebook.Model;

    /// <summary>
    /// UserUpsertMapper.
    /// </summary>
    public class UserUpsertMapper : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserUpsertMapper"/> class.
        /// </summary>
        public UserUpsertMapper()
        {
            this.CreateMap<User, UserModel>().ReverseMap();
            this.CreateMap<UserPost, GetUserPostModel>().ReverseMap();
            this.CreateMap<StoryModel, Story>().ReverseMap();
            this.CreateMap<AddCommentModel, PostComment>().ReverseMap();
        }
    }
}
