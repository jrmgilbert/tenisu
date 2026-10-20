using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Tenisu.Core.Players;

namespace Tenisu.Infrastructure.Persistence.Configuration;

internal sealed class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
    public void Configure(EntityTypeBuilder<Player> b)
    {
        b.HasKey(p => p.Id);
        b.HasIndex(p => p.ShortName).IsUnique();
        b.Property(p => p.Sex).HasConversion<string>();

        b.ComplexProperty(p => p.Country, c =>
        {
            c.Property(x => x.Code).HasColumnName("CountryCode");
            c.Property(x => x.Picture).HasColumnName("CountryPicture");
        });

        b.ComplexProperty(p => p.Data, s =>
        {
            s.Property(x => x.Last)
             .HasConversion(v => v.ToString(), v => LastResults.Parse(v));
        });
    }
}