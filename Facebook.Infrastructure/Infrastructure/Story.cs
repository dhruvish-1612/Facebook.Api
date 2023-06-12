using System;
using System.Collections.Generic;

namespace Facebook.Infrastructure.Infrastructure;

public partial class Story
{
    public long StoryId { get; set; }

    public long UserId { get; set; }

    public string MediaPath { get; set; } = null!;

    public string MediaType { get; set; } = null!;

    public string? WrittenText { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
