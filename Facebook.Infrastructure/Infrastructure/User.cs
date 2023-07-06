using System;
using System.Collections.Generic;

namespace Facebook.Infrastructure.Infrastructure;

public partial class User
{
    public long UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public int Gender { get; set; }

    public int? RelationStatus { get; set; }

    public string? Avatar { get; set; }

    public int Role { get; set; }

    public int? CityId { get; set; }

    public int? CountryId { get; set; }

    public string? Address { get; set; }

    public string? Bio { get; set; }

    public string? Hobbies { get; set; }

    public DateTime? BirthDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual City? City { get; set; }

    public virtual Country? Country { get; set; }

    public virtual ICollection<ForgotPassword> ForgotPasswords { get; set; } = new List<ForgotPassword>();

    public virtual ICollection<Friendship> FriendshipProfileAcceptNavigations { get; set; } = new List<Friendship>();

    public virtual ICollection<Friendship> FriendshipProfileRequestNavigations { get; set; } = new List<Friendship>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<PostComment> PostComments { get; set; } = new List<PostComment>();

    public virtual ICollection<PostLike> PostLikes { get; set; } = new List<PostLike>();

    public virtual ICollection<Story> Stories { get; set; } = new List<Story>();

    public virtual ICollection<UserPost> UserPosts { get; set; } = new List<UserPost>();
}
