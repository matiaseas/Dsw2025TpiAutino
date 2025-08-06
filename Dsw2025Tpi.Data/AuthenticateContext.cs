using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Data;

public class AuthenticateContext : IdentityDbContext
{
    public AuthenticateContext(DbContextOptions<AuthenticateContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<IdentityUser>(e => { e.ToTable("usuarios"); });
        builder.Entity<IdentityRole>(e => { e.ToTable("roles"); });
        builder.Entity<IdentityUserRole<string>>(e => { e.ToTable("usuariosRoles"); });
        builder.Entity<IdentityUserClaim<string>>(e => { e.ToTable("usuariosClaims"); });
        builder.Entity<IdentityUserLogin<string>>(e => { e.ToTable("usuariosLogins"); });
        builder.Entity<IdentityRoleClaim<string>>(e => { e.ToTable("rolesClaims"); });
        builder.Entity<IdentityUserToken<string>>(e => { e.ToTable("usuariosTokens"); });

    }
}
