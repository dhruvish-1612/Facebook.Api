using System;
using System.Collections.Generic;

namespace Facebook.Infrastructure.Infrastructure;

public partial class PostComment
{
    public long UserPostCommentId { get; set; }

    public long UserPostId { get; set; }

    public long UserId { get; set; }

    public string? CommentText { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual UserPost UserPost { get; set; } = null!;
}
