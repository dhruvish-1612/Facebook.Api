// <copyright file="GetUserPostLikeWithCountModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Model
{
    /// <summary>
    /// GetUserPostLikeWithCountModel.
    /// </summary>
    public class GetUserPostLikeWithCountModel
    {
        /// <summary>
        ///  Gets or sets the user post likes.
        /// </summary>
        public List<GetUserPostLikeModel> GetUserPostLikes { get; set; } = new();

        /// <summary>
        /// Gets or sets the likes count.
        /// </summary>
        /// <value>
        /// The likes count.
        /// </value>
        public long LikesCount { get; set; } = 0;
    }
}
