namespace BackEndProject.Contexts
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Setting> Settings { get; set; } = null!;

        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<CourseCategory> CourseCategories { get; set; } = null!;

        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<Speaker> Speakers { get; set; } = null!;
        public DbSet<EventSpeaker> EventSpeakers { get; set; } = null!;

        public DbSet<Teacher> Teachers { get; set; } = null!;
        public DbSet<Skill> Skills { get; set; } = null!;
        public DbSet<SocialMedia> SocialMedias { get; set; } = null!;

        public DbSet<Blog> Blogs { get; set; } = null!;
        public DbSet<Subscribe> Subscribes { get; set; } = null!;

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<BaseEntity>();
            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        entry.Entity.ModifiedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.ModifiedAt = DateTime.UtcNow;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<Category>().HasQueryFilter(c => !c.IsDeleted);

            modelBuilder.Entity<Event>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Speaker>().HasQueryFilter(s => !s.IsDeleted);

            modelBuilder.Entity<Teacher>().HasQueryFilter(t => !t.IsDeleted);
            modelBuilder.Entity<Skill>().HasQueryFilter(s => !s.IsDeleted);
            modelBuilder.Entity<SocialMedia>().HasQueryFilter(sm => !sm.IsDeleted);

            modelBuilder.Entity<Blog>().HasQueryFilter(b => !b.IsDeleted);

            base.OnModelCreating(modelBuilder);
        }
    }

}
