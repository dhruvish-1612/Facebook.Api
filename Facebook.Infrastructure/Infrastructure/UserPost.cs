using System;
using System.Collections.Generic;

namespace Facebook.Infrastructure.Infrastructure;

public partial class UserPost
{
    public long UserPostId { get; set; }

    public long UserId { get; set; }

    public string? WrittenText { get; set; }

    public string MediaPath { get; set; } = null!;

    public string? MediaType { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<PostComment> PostComments { get; set; } = new List<PostComment>();

    public virtual ICollection<PostLike> PostLikes { get; set; } = new List<PostLike>();

    public virtual User User { get; set; } = null!;
}
