using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Facebook.Infrastructure.Infrastructure;

public partial class FacebookContext : DbContext
{
    public FacebookContext()
    {
    }

    public FacebookContext(DbContextOptions<FacebookContext> options)
        : base(options)
    {
    }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<ForgotPassword> ForgotPasswords { get; set; }

    public virtual DbSet<Friendship> Friendships { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<PostComment> PostComments { get; set; }

    public virtual DbSet<PostLike> PostLikes { get; set; }

    public virtual DbSet<PostsMedium> PostsMedia { get; set; }

    public virtual DbSet<Story> Stories { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserPost> UserPosts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=PCT71\\SQL2017;Initial Catalog=Facebook;Persist Security Info=False;User ID=sa;Password=Tatva@123;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=False;Connection Timeout=30;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.CityId).HasName("PK__city__031491A871F50431");

            entity.ToTable("city");

            entity.Property(e => e.CityId).HasColumnName("city_id");
            entity.Property(e => e.CityName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("city_name");
            entity.Property(e => e.CountryId).HasColumnName("country_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deleted_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Country).WithMany(p => p.Cities)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_city_country_id");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.CountryId).HasName("PK__country__7E8CD0552EDB174F");

            entity.ToTable("country");

            entity.Property(e => e.CountryId).HasColumnName("country_id");
            entity.Property(e => e.CountryCode).HasColumnName("country_code");
            entity.Property(e => e.CountryName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("country_name");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Iso)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("iso");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<ForgotPassword>(entity =>
        {
            entity.HasKey(e => e.ForgotId).HasName("PK__forgot_p__7DDF1F1BD3F21DD5");

            entity.ToTable("forgot_password");

            entity.Property(e => e.ForgotId).HasColumnName("forgot_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Token)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("token");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.ForgotPasswords)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_forgot_password_user_id");
        });

        modelBuilder.Entity<Friendship>(entity =>
        {
            entity.HasKey(e => e.FriendshipId).HasName("pk_friendship");

            entity.ToTable("friendship");

            entity.Property(e => e.FriendshipId).HasColumnName("friendship_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deleted_at");
            entity.Property(e => e.IsFriend)
                .HasDefaultValueSql("((0))")
                .HasColumnName("is_friend");
            entity.Property(e => e.ProfileAccept).HasColumnName("profile_accept");
            entity.Property(e => e.ProfileRequest).HasColumnName("profile_request");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("((0))")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.ProfileAcceptNavigation).WithMany(p => p.FriendshipProfileAcceptNavigations)
                .HasForeignKey(d => d.ProfileAccept)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_friendship_profile_accept");

            entity.HasOne(d => d.ProfileRequestNavigation).WithMany(p => p.FriendshipProfileRequestNavigations)
                .HasForeignKey(d => d.ProfileRequest)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_friendship_profile_request");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("notification");

            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.ActivityId).HasColumnName("activity_id");
            entity.Property(e => e.ActivityType).HasColumnName("activity_type");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deleted_at");
            entity.Property(e => e.IsRead).HasColumnName("is_read");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__notificat__user___1C873BEC");
        });

        modelBuilder.Entity<PostComment>(entity =>
        {
            entity.HasKey(e => e.UserPostCommentId).HasName("PK__post_com__4A3CA5BEAB57D446");

            entity.ToTable("post_comment");

            entity.Property(e => e.UserPostCommentId).HasColumnName("user_post_comment_id");
            entity.Property(e => e.CommentText)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("comment_text");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deleted_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.UserPostId).HasColumnName("user_post_id");

            entity.HasOne(d => d.User).WithMany(p => p.PostComments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_post_comment_user_id");

            entity.HasOne(d => d.UserPost).WithMany(p => p.PostComments)
                .HasForeignKey(d => d.UserPostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_post_comment_user_post_id");
        });

        modelBuilder.Entity<PostLike>(entity =>
        {
            entity.HasKey(e => e.UserPostLikeId).HasName("PK__post_lik__40F3585AB6838B5C");

            entity.ToTable("post_like");

            entity.Property(e => e.UserPostLikeId).HasColumnName("user_post_like_id");
            entity.Property(e => e.LikeDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("like_date");
            entity.Property(e => e.LikeStatus)
                .HasDefaultValueSql("((0))")
                .HasColumnName("like_status");
            entity.Property(e => e.LikeUserId).HasColumnName("like_user_id");
            entity.Property(e => e.UserPostId).HasColumnName("user_post_id");

            entity.HasOne(d => d.LikeUser).WithMany(p => p.PostLikes)
                .HasForeignKey(d => d.LikeUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_post_like_like_user_id");

            entity.HasOne(d => d.UserPost).WithMany(p => p.PostLikes)
                .HasForeignKey(d => d.UserPostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_post_like_user_post_id");
        });

        modelBuilder.Entity<PostsMedium>(entity =>
        {
            entity.HasKey(e => e.PostMediaId).HasName("PK__posts_me__02CDD6180A0FB8DF");

            entity.ToTable("posts_media");

            entity.Property(e => e.PostMediaId).HasColumnName("post_media_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.MediaPath)
                .HasMaxLength(2048)
                .IsUnicode(false)
                .HasColumnName("media_path");
            entity.Property(e => e.MediaType)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("media_type");
            entity.Property(e => e.UserPostId).HasColumnName("user_post_id");

            entity.HasOne(d => d.UserPost).WithMany(p => p.PostsMedia)
                .HasForeignKey(d => d.UserPostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_posts_media_user_post_id");
        });

        modelBuilder.Entity<Story>(entity =>
        {
            entity.HasKey(e => e.StoryId).HasName("PK__story__66339C56741B0803");

            entity.ToTable("story");

            entity.Property(e => e.StoryId).HasColumnName("story_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deleted_at");
            entity.Property(e => e.MediaPath)
                .HasMaxLength(2048)
                .IsUnicode(false)
                .HasColumnName("media_path");
            entity.Property(e => e.MediaType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("media_type");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.WrittenText)
                .HasColumnType("text")
                .HasColumnName("written_text");

            entity.HasOne(d => d.User).WithMany(p => p.Stories)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_story_user_id");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__users__B9BE370F5E73E1D3");

            entity.ToTable("users");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Address)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("address");
            entity.Property(e => e.Avatar)
                .HasMaxLength(2048)
                .IsUnicode(false)
                .HasColumnName("avatar");
            entity.Property(e => e.Bio)
                .HasColumnType("text")
                .HasColumnName("bio");
            entity.Property(e => e.BirthDate)
                .HasColumnType("datetime")
                .HasColumnName("birth_date");
            entity.Property(e => e.CityId).HasColumnName("city_id");
            entity.Property(e => e.CountryId).HasColumnName("country_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Email)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("first_name");
            entity.Property(e => e.Gender).HasColumnName("gender");
            entity.Property(e => e.Hobbies)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("hobbies");
            entity.Property(e => e.LastName)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("last_name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("phone_number");
            entity.Property(e => e.RelationStatus).HasColumnName("relation_status");
            entity.Property(e => e.Role).HasColumnName("role");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.City).WithMany(p => p.Users)
                .HasForeignKey(d => d.CityId)
                .HasConstraintName("fk_users_city");

            entity.HasOne(d => d.Country).WithMany(p => p.Users)
                .HasForeignKey(d => d.CountryId)
                .HasConstraintName("fk_users_country");
        });

        modelBuilder.Entity<UserPost>(entity =>
        {
            entity.HasKey(e => e.UserPostId).HasName("PK__user_pos__A8CCE37BBABCF6A8");

            entity.ToTable("user_post");

            entity.Property(e => e.UserPostId).HasColumnName("user_post_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserPosts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_user_post_user_id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
