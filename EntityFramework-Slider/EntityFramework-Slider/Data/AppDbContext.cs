using EntityFramework_Slider.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework_Slider.Data
{
    public class AppDbContext: IdentityDbContext<AppUser>  //IdentityDbContext-yaziriqki migration edende identity tablarida gelsin,icinde AppUser-elave model gonderik yaratsin deye

    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<SliderInfo> SliderInfos { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<About> Abouts { get; set; }

        public DbSet<Adventage> Adventages { get; set; }

        public DbSet<Experts> Experts { get; set; }

        public DbSet<ExpertsHeader> ExpertsHeaders { get; set; }

        public DbSet<Subscribe> Subscribs { get; set; }

        public DbSet<BlogHeader> BlogHeaders { get; set; }

        public DbSet<Say> Says { get; set; }

        public DbSet<Instagram> Instagrams { get; set; }

        public DbSet<Setting> Settings { get; set; }
        public DbSet<Social> Socials { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);  //idenity tablari saxlasin sqlde(IdenityRole,IdentityUser) 
            modelBuilder.Entity<Product>().HasQueryFilter(m => !m.SoftDelete);
            modelBuilder.Entity<Blog>().HasQueryFilter(m => !m.SoftDelete);
            modelBuilder.Entity<BlogHeader>().HasQueryFilter(m => !m.SoftDelete);
            modelBuilder.Entity<ExpertsHeader>().HasQueryFilter(m => !m.SoftDelete);
            modelBuilder.Entity<Setting>()

                .HasData(    //tabli hazir dolmus sekilde yaratmaq ucun
                      new Setting
                      {
                          Id = 1,

                          Key = "HeaderLogo",

                          Value = "logo.png"
                      },

                       new Setting
                        {
                            Id = 2,

                            Key = "Phone",

                            Value = "123245"
                        },

                        new Setting
                        {
                            Id = 3,

                            Key = "Email",

                            Value = "p135@code.edu.az"
                         }


                );

              

            
        }

    }
}
