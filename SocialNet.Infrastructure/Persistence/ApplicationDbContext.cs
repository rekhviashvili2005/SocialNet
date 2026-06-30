using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SocialNet.Domain.Entities;
using SocialNet.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNet.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Post> Posts => Set<Post>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<Like> Likes => Set<Like>();

        public DbSet<Follow> Follows => Set<Follow>();



        public DbSet<PostHashtag> PostHashtags => Set<PostHashtag>();


        public DbSet<PostImage> PostImages => Set<PostImage>();


        public DbSet<Notification> Notifications => Set<Notification>();


        public DbSet<CommentImage> CommentImages => Set<CommentImage>();

        public DbSet<BlacklistedToken> BlacklistedTokens => Set<BlacklistedToken>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);



            builder.Entity<PostHashtag>()
                .HasOne(h => h.Post)
                .WithMany(p => p.Hashtags)
                .HasForeignKey(h => h.PostId)
                .OnDelete(DeleteBehavior.Cascade);



            builder.Entity<Post>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);




            builder.Entity<Comment>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);


            builder.Entity<Like>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.NoAction);


            builder.Entity<Like>()
                .HasIndex(l => new { l.UserId, l.PostId })
                .IsUnique();



            /// ამეებს სხვა გზირ ვაკეთებბბ

            builder.Entity<Follow>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Follow>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(f => f.FollowingId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Follow>()
                .HasIndex(f => new { f.FollowerId, f.FollowingId })
                .IsUnique();

            //bervi suratis atvirtva ro shevdzlo
            builder.Entity<PostImage>()
                .HasOne(i => i.Post)
                .WithMany(p => p.Images)
                .HasForeignKey(i => i.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            //Notifications

            builder.Entity<Notification>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Notification>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(n => n.ActorId)
                .OnDelete(DeleteBehavior.NoAction);




            builder.Entity<CommentImage>()
                .HasOne(i => i.Comment)
                .WithMany(c => c.Images)
                .HasForeignKey(i => i.CommentId)
                .OnDelete(DeleteBehavior.Cascade);
        }




    }
}
