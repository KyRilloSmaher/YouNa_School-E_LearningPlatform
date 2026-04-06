using Microsoft.EntityFrameworkCore;
using SharedKernel.Domain.CoreAbstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SharedKernel.Infrastructure.Persistence
{
    /// <summary>
    /// Base implementation of <see cref="IRepository{T}"/> using Entity Framework Core.
    /// </summary>
    /// <typeparam name="T">The type of entity this repository manages.</typeparam>
    /// <remarks>
    /// This abstract class provides a default EF Core implementation of the repository pattern.
    /// Derived repositories can override methods to add domain-specific query logic.
    /// The class supports both tracked and untracked queries for performance optimization.
    /// </remarks>
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        /// <summary>
        /// Gets the database context used by this repository.
        /// </summary>
        protected readonly DbContext Context;

        /// <summary>
        /// Gets the <see cref="DbSet{TEntity}"/> for the entity type <typeparamref name="T"/>.
        /// </summary>
        protected readonly DbSet<T> DbSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{T}"/> class.
        /// </summary>
        /// <param name="context">The database context to be used for data access operations.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="context"/> is null.
        /// </exception>
        protected Repository(DbContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            DbSet = context.Set<T>();
        }

        /// <inheritdoc/>
        public virtual async Task<T?> GetByIdAsync(Guid id,bool asTracked = true,CancellationToken cancellationToken = default)
        {
            if (asTracked)
            {
                // Use FindAsync for tracked entities (checks change tracker first)
                return await DbSet.FindAsync(new object[] { id }, cancellationToken);
            }

            // For untracked entities, we need to query with AsNoTracking
            // This assumes entities have a Guid property named "Id"
            var parameter = Expression.Parameter(typeof(T), "e");
            var property = Expression.Property(parameter, "Id");
            var constant = Expression.Constant(id);
            var equals = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<T, bool>>(equals, parameter);

            return await DbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(lambda, cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<T>> GetAllAsync(bool asTracked = false,CancellationToken cancellationToken = default)
        {
            var query = GetQuery(asTracked);
            return await query.ToListAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async Task AddAsync(T entity,CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await DbSet.AddAsync(entity, cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async Task AddRangeAsync(IEnumerable<T> entities,CancellationToken cancellationToken = default)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            await DbSet.AddRangeAsync(entities, cancellationToken);
        }

        /// <inheritdoc/>
        public virtual void Remove(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            DbSet.Remove(entity);
        }

        /// <inheritdoc/>
        public virtual void RemoveRange(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            DbSet.RemoveRange(entities);
        }

        /// <inheritdoc/>
        public virtual async Task<bool> ExistsAsync(Guid id,CancellationToken cancellationToken = default)
        {
            // Build expression tree for Id == id
            var parameter = Expression.Parameter(typeof(T), "e");
            var property = Expression.Property(parameter, "Id");
            var constant = Expression.Constant(id);
            var equals = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<T, bool>>(equals, parameter);

            return await DbSet
                .AsNoTracking()
                .AnyAsync(lambda, cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return await DbSet.AsNoTracking().CountAsync(cancellationToken);
        }

        /// <summary>
        /// Gets a queryable source for entities with optional tracking control.
        /// </summary>
        /// <param name="asTracked">
        /// A value indicating whether the entities should be tracked.
        /// Default is false for better performance in read scenarios.
        /// </param>
        /// <returns>
        /// An <see cref="IQueryable{T}"/> that can be used to build complex queries.
        /// </returns>
        /// <remarks>
        /// Derived repositories should use this method as the starting point for all queries
        /// to ensure consistent tracking behavior across the application.
        /// </remarks>
        protected virtual IQueryable<T> GetQuery(bool asTracked = false)
        {
            return asTracked ? DbSet : DbSet.AsNoTracking();
        }

        /// <summary>
        /// Gets a queryable source that includes specified related entities.
        /// </summary>
        /// <param name="asTracked">
        /// A value indicating whether the entities should be tracked.
        /// </param>
        /// <param name="includes">
        /// The related entities to include in the query.
        /// </param>
        /// <returns>
        /// An <see cref="IQueryable{T}"/> with the specified includes applied.
        /// </returns>
        /// <remarks>
        /// Use this method for eager loading of related entities. Be cautious of
        /// performance implications when including multiple or large related collections.
        /// </remarks>
        protected virtual IQueryable<T> GetQueryWithIncludes(bool asTracked = false,params Expression<Func<T, object>>[] includes)
        {
            var query = GetQuery(asTracked);

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return query;
        }

        /// <summary>
        /// Finds entities matching the specified predicate.
        /// </summary>
        /// <param name="predicate">The condition to match.</param>
        /// <param name="asTracked">Whether to track the entities.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Matching entities.</returns>
        /// <remarks>
        /// This method provides a generic way to filter entities without creating
        /// domain-specific repository methods for simple queries.
        /// </remarks>
        protected virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate,bool asTracked = false,CancellationToken cancellationToken = default)
        {
            var query = GetQuery(asTracked).Where(predicate);
            return await query.ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Finds the first entity matching the specified predicate.
        /// </summary>
        /// <param name="predicate">The condition to match.</param>
        /// <param name="asTracked">Whether to track the entity.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The first matching entity, or null if none found.</returns>
        protected virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate,bool asTracked = false,CancellationToken cancellationToken = default)
        {
            var query = GetQuery(asTracked).Where(predicate);
            return await query.FirstOrDefaultAsync(cancellationToken);
        }
        /// <inheritdoc/>
        public virtual async Task<IQueryable<T>> GetAllAsQueryableAsync(bool asTracked = false, CancellationToken cancellationToken = default)
        {
            var query = GetQuery(asTracked);
            // Ensure the query is executed asynchronously to avoid blocking calls
            await Task.Yield();
            return query;
        }
    }
}