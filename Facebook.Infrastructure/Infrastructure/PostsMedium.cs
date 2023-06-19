using System;
using System.Collections.Generic;

namespace Facebook.Infrastructure.Infrastructure;

public partial class PostsMedium
{
    public long PostMediaId { get; set; }

    public long UserPostId { get; set; }

    public string? MediaPath { get; set; }

    public string? MediaType { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual UserPost UserPost { get; set; } = null!;
}
