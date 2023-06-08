using System;
using System.Collections.Generic;

namespace Facebook.Infrastructure.Infrastructure;

public partial class Friendship
{
    public long FriendshipId { get; set; }

    public long ProfileRequest { get; set; }

    public long ProfileAccept { get; set; }

    public int? Status { get; set; }

    public bool? IsFriend { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User ProfileAcceptNavigation { get; set; } = null!;

    public virtual User ProfileRequestNavigation { get; set; } = null!;
}
