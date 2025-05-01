namespace DatingApp.Server.Data;

public class DataContext : IdentityDbContext<UserModel, AppRoleModel, int, 
    IdentityUserClaim<int>, AppUserRoleModel, IdentityUserLogin<int>, 
    IdentityRoleClaim<int>, IdentityUserToken<int>>
{
    public DataContext(DbContextOptions options):base(options){ }

    public DbSet<UserLikeModel> Likes { get; set; }
    public DbSet<MessageModel> Messages { get; set; }
    public DbSet<GroupModel> Groups {get; set;}
    public DbSet<ConnectionModel> Connections {get; set;}

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // configure identity
        builder.Entity<UserModel>().HasMany(ur => ur.UserRoles).WithOne(u => u.User).HasForeignKey(ur => ur.UserId).IsRequired();
        builder.Entity<AppRoleModel>().HasMany(ur => ur.UserRoles).WithOne(u => u.AppRole).HasForeignKey(ur => ur.RoleId).IsRequired();
        builder.Entity<UserModel>().HasMany(u => u.Photos).WithOne(u => u.User).HasForeignKey(u => u.UserId);

        // to implement M:N relationship, one side followed by the other side
        builder.Entity<UserLikeModel>().HasKey(k => new {k.SourceUserId, k.LikedUserId});
        builder.Entity<UserLikeModel>().HasOne(s => s.SourceUser).WithMany(l => l.LikedUsers).HasForeignKey(s => s.SourceUserId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<UserLikeModel>().HasOne(s => s.LikedUser).WithMany(l => l.LikedByUsers).HasForeignKey(s => s.LikedUserId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<MessageModel>().HasOne(u => u.Recipient).WithMany(s => s.MessagesReceived).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<MessageModel>().HasOne(u => u.Sender).WithMany(s => s.MessagesSent).OnDelete(DeleteBehavior.Restrict);
    }
}
