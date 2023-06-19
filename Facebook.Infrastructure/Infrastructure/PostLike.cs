using System;
using System.Collections.Generic;

namespace Facebook.Infrastructure.Infrastructure;

public partial class PostLike
{
    public long UserPostLikeId { get; set; }

    public long UserPostId { get; set; }

    public long LikeUserId { get; set; }

    public bool? LikeStatus { get; set; }

    public DateTime LikeDate { get; set; }

    public virtual User LikeUser { get; set; } = null!;

    public virtual UserPost UserPost { get; set; } = null!;
}
