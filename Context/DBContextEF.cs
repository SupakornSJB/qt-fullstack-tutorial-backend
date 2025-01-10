using Microsoft.EntityFrameworkCore;

namespace FullStackTutorialBackend.Context;

public class DBContextEF : DbContext
{
	private IConfiguration _config;

	public DbSet<VoteOption> VoteOptions { get; init; }
	public DbSet<VoteTopic> VoteTopics { get; init; }

	public DBContextEF(IConfiguration config)
	{
		_config = config;
	}

	protected override void OnConfiguring(DbContextOptionsBuilder builder)
	{
		base.OnConfiguring(builder);
		builder.UseSqlServer(_config.GetConnectionString("DefaultConnection"), option => {
			option.EnableRetryOnFailure();
		});
	}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
		modelBuilder.Entity<VoteTopic>();
		modelBuilder.Entity<VoteOption>()
			.HasKey(v => new { v.VoteTopicId, v.Content });
		modelBuilder.Entity<VoteOption>()
			.HasOne<VoteTopic>()
			.WithMany()
			.HasForeignKey(t => t.VoteTopicId);
    }
}