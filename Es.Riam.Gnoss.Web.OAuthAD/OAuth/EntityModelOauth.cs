namespace ConsoleApplication1.OAuth
{
    using Es.Riam.Gnoss.OAuthAD.OAuth;
    using Microsoft.EntityFrameworkCore;

    public partial class EntityModelOauth : DbContext
    {
        public EntityModelOauth()
            : base()
        {
        }

        public virtual DbSet<ConsumerData> ConsumerData { get; set; }
        public virtual DbSet<Nonce> Nonce { get; set; }
        public virtual DbSet<OAuthConsumer> OAuthConsumer { get; set; }
        public virtual DbSet<OAuthToken> OAuthToken { get; set; }
        public virtual DbSet<OAuthTokenExterno> OAuthTokenExterno { get; set; }
        public virtual DbSet<PinToken> PinToken { get; set; }
        public virtual DbSet<Usuario> Usuario { get; set; }
        public virtual DbSet<UsuarioConsumer> UsuarioConsumer { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Nonce>()
               .HasKey(c => new { c.Context, c.Code, c.Timestamp });

            modelBuilder.Entity<OAuthConsumer>()
                .HasOne(e => e.ConsumerData)
                .WithOne(e => e.OAuthConsumer).IsRequired();

            modelBuilder.Entity<OAuthConsumer>()
                .HasOne(e => e.UsuarioConsumer)
                .WithOne(e => e.OAuthConsumer).IsRequired();

            modelBuilder.Entity<OAuthConsumer>()
                .HasMany(e => e.OAuthToken)
                .WithOne(e => e.OAuthConsumer).IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OAuthToken>()
                .HasOne(e => e.OAuthTokenExterno)
                .WithOne(e => e.OAuthToken).IsRequired();

            modelBuilder.Entity<OAuthToken>()
                .HasOne(e => e.PinToken)
                .WithOne(e => e.OAuthToken).IsRequired();

            modelBuilder.Entity<Usuario>()
                .HasMany(e => e.OAuthTokenExterno)
                .WithOne(e => e.Usuario).IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Usuario>()
                .HasMany(e => e.PinToken)
                .WithOne(e => e.Usuario).IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Usuario>()
                .HasMany(e => e.UsuarioConsumer)
                .WithOne(e => e.Usuario).IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
