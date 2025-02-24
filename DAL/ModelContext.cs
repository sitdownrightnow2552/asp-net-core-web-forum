﻿using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DAL.Database;
using DAL.Models;
using DAL.Models.Auth;
using DAL.Models.Forum;
using DAL.Models.Topic;
using DAL.Validation;
using Humanizer;
using SlugityLib;
using Thread = DAL.Models.Forum.Thread;

namespace DAL
{
    // I use EF6 instead of EFCore because i need the TPC model (which is not supported by EFCore yet)
    public class ModelContext : DbContext
    {
        private static readonly Slugity Slugity = new();

        public ModelContext(string connection, bool fakeData) : base(connection)
        {
            if (fakeData)
            {
                System.Data.Entity.Database.SetInitializer(new FakeDataDatabaseInitializer());
                return;
            }

            System.Data.Entity.Database.SetInitializer(new DatabaseInitializer());
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Experience> Experiences { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<UserInfo> UserInfos { get; set; }

        public DbSet<Forum> Forums { get; set; }
        public DbSet<Thread> Threads { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Vote> Votes { get; set; }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<Language> Languages { get; set; }

        public DbSet<Image> Images { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Image>().Map(m => m.MapInheritedProperties());
            modelBuilder.Entity<Role>().Map(m => m.MapInheritedProperties());
            modelBuilder.Entity<User>().Map(m => m.MapInheritedProperties());
            modelBuilder.Entity<Country>().Map(m => m.MapInheritedProperties());
            modelBuilder.Entity<City>().Map(m => m.MapInheritedProperties());
            modelBuilder.Entity<Experience>().Map(m => m.MapInheritedProperties());
            modelBuilder.Entity<Position>().Map(m => m.MapInheritedProperties());

            modelBuilder.Entity<Vote>().Map(m => m.MapInheritedProperties());
            modelBuilder.Entity<Forum>().Map(m => m.MapInheritedProperties());
            modelBuilder.Entity<Post>().Map(m => m.MapInheritedProperties());
            modelBuilder.Entity<Thread>().Map(m => m.MapInheritedProperties());

            modelBuilder.Entity<Category>().Map(m => m.MapInheritedProperties());
            modelBuilder.Entity<Tag>().Map(m => m.MapInheritedProperties());
            modelBuilder.Entity<Specialty>().Map(m => m.MapInheritedProperties());
            modelBuilder.Entity<Language>().Map(m => m.MapInheritedProperties());

            modelBuilder.Entity<UserInfo>().Map(m => m.MapInheritedProperties());
            modelBuilder.Entity<UserInfo>().HasRequired(m => m.User)
                .WithOptional(x => x.UserInfo)
                .WillCascadeOnDelete(true);
            modelBuilder.Entity<UserInfo>().HasMany(m => m.WorkSpecialities)
                .WithMany(x => x.UserInfos);
            modelBuilder.Entity<UserInfo>().HasOptional(m => m.WorkExperience)
                .WithMany(x => x.UserInfos)
                .HasForeignKey(m => m.WorkExperienceId)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<UserInfo>().HasOptional(m => m.WorkPosition)
                .WithMany(x => x.UserInfos)
                .HasForeignKey(m => m.WorkPositionId)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<UserInfo>().HasOptional(m => m.WorkCity)
                .WithMany(x => x.UserInfos)
                .HasForeignKey(m => m.WorkCityId)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<UserInfo>().HasOptional(m => m.WorkCountry)
                .WithMany(x => x.UserInfos)
                .HasForeignKey(m => m.WorkCountryId)
                .WillCascadeOnDelete(false);
        }

        public override int SaveChanges()
        {
            SetProperties();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            SetProperties();
            return base.SaveChangesAsync(cancellationToken);
        }

        // Audit date and slug generation handle
        private void SetProperties()
        {
            foreach (var entry in ChangeTracker.Entries()
                .Where(p => p.State is EntityState.Modified or EntityState.Added))
            {
                var properties = entry.Entity
                    .GetType()
                    .GetProperties()
                    .Where(prop =>
                        Attribute.IsDefined(prop, typeof(Standardized)) && prop.PropertyType == typeof(string));

                foreach (var property in properties)
                {
                    var value = entry.CurrentValues[property.Name]?.ToString();
                    if (string.IsNullOrWhiteSpace(value)) return;
                    if (property.IsDefined(typeof(CodeId), true))
                    {
                        entry.CurrentValues[property.Name] = value.Transform(To.LowerCase);
                    }
                    else
                    {
                        entry.CurrentValues[property.Name] = value.Transform(To.TitleCase);
                    }
                }
            }

            foreach (var entity in ChangeTracker.Entries().Where(p => p.State == EntityState.Added))
            {
                if (entity.Entity is IAuditable created)
                {
                    created.CreatedAt = DateTime.Now;
                    created.UpdatedAt = DateTime.Now;
                }


                if (entity.Entity is ITracked tracked)
                {
                    tracked.LastActivityAt = DateTime.Now;
                }

                if (entity.Entity is ISlugged slugged)
                {
                    slugged.Slug = Slugity.GenerateSlug(slugged.RawSlug);
                }
            }

            foreach (var entity in ChangeTracker.Entries().Where(p => p.State == EntityState.Modified))
            {
                var isNotTracking = true;

                if (entity.Entity is ITracked)
                {
                    var trackingProp = entity.Property("LastActivityAt");
                    isNotTracking = trackingProp.CurrentValue.Equals(trackingProp.OriginalValue);
                }

                // not update audit if last active at changed
                if (entity.Entity is IAuditable updated && isNotTracking)
                {
                    updated.UpdatedAt = DateTime.Now;
                }


                if (entity.Entity is ISlugged slugged)
                {
                    slugged.Slug = Slugity.GenerateSlug(slugged.RawSlug);
                }
            }
        }
    }
}