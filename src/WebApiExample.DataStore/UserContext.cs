using System;
using System.Linq;
using System.Threading.Tasks;
using WebApiExample.Common.DataAccess;
using WebApiExample.DataStore.Models;
#if !NETSTANDARD2_0
using Microsoft.EntityFrameworkCore;
#endif

namespace WebApiExample.DataStore
{
#if NETSTANDARD2_0
    /// <summary>
    /// Stub implementation for netstandard2.0 compatibility shim.
    /// EF Core is not available on netstandard2.0; use net10.0 for full functionality.
    /// .NET Framework callers should provide their own IUnitOfWork implementation.
    /// </summary>
    public class UserContext : IUnitOfWork
    {
        private static Exception NotSupported() =>
            new PlatformNotSupportedException("UserContext requires net10.0 or later. Provide a custom IUnitOfWork for .NET Framework.");

        public T Add<T>(T entity) where T : class => throw NotSupported();
        public IQueryable<T> GetAll<T>() where T : class => throw NotSupported();
        public Task<T?> GetAsync<T>(Guid id) where T : class, IRootEntity => throw NotSupported();
        public void Remove<T>(T entity) where T : class => throw NotSupported();
        public void Update<T>(T entity) where T : class => throw NotSupported();
        public Task CommitAsync() => throw NotSupported();
    }
#else
    public class UserContext : DbContext, IUnitOfWork
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {
        }

        #region DbSets

        public DbSet<User> Users { get; set; } = null!;

        #endregion

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>()
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

        public Task<T?> GetAsync<T>(Guid id) where T : class, IRootEntity
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
#endif
}
