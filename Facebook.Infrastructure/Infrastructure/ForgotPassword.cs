using System;
using System.Collections.Generic;

namespace Facebook.Infrastructure.Infrastructure;

public partial class ForgotPassword
{
    public long ForgotId { get; set; }

    public long UserId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
