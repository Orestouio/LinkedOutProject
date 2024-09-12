using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendApp.auth;
using BackendApp.Model;
using Microsoft.EntityFrameworkCore;

namespace BackendApp.Data;

public class ApiContext
(DbContextOptions<ApiContext> options, IConfiguration configuration) 
: DbContext(options)
{

    private readonly IConfiguration configuration = configuration;

    public DbSet<AdminUser> AdminUsers {get; private set;} 
    public DbSet<RegularUser> RegularUsers {get; private set;}
    public DbSet<Post> Posts {get; private set;}
    public DbSet<JobPost> JobPosts {get; private set;}
    public DbSet<Notification> Notifications {get; private set;}
    public DbSet<Message> Messages {get; private set;}
    public DbSet<Connection> Connections {get; private set;}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseIdentityAlwaysColumns();
    }
}
