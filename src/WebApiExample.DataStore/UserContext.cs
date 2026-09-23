using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApiExample.Common.DataAccess;
using WebApiExample.DataStore.Models;

namespace WebApiExample.DataStore
{
    public class UserContext : DbContext, IUnitOfWork
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd();
        }

        public new T Add<T>(T entity) where T : class
        {
            return base.Set<T>().Add(entity).Entity;
        }

        public Task CommitAsync()
        {
            return base.SaveChangesAsync();
        }

        public IQueryable<T> GetAll<T>() where T : class
        {
            return base.Set<T>();
        }

        public Task<T> GetAsync<T>(Guid id) where T : class, IRootEntity
        {
            return this.GetAll<T>().FirstOrDefaultAsync(e => e.Id == id);
        }

        public new void Remove<T>(T entity) where T : class
        {
            base.Set<T>().Remove(entity);
        }

        public new void Update<T>(T entity) where T : class
        {
            base.Entry(entity).State = EntityState.Modified;
        }
    }
}
