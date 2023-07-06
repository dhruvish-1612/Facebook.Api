using System;
using System.Collections.Generic;

namespace Facebook.Infrastructure.Infrastructure;

public partial class Notification
{
    public long NotificationId { get; set; }

    public long UserId { get; set; }

    public int ActivityType { get; set; }

    public long ActivityId { get; set; }

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
