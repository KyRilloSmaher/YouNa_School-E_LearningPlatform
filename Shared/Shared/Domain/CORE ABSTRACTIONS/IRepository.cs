using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SharedKernel.Domain.CoreAbstractions
{
    /// <summary>
    /// Defines the contract for a repository that provides data access operations for entities.
    /// </summary>
    /// <typeparam name="T">The type of entity this repository manages.</typeparam>
    /// <remarks>
    /// This interface defines the fundamental CRUD operations that all repositories should implement.
    /// It follows the Repository pattern to abstract data access details from domain logic.
    /// Implementations can use any persistence technology (EF Core, Dapper, Cosmos DB, etc.).
    /// </remarks>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Retrieves an entity by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to retrieve.</param>
        /// <param name="asTracked">
        /// A value indicating whether the returned entity should be tracked by the context.
        /// If true, changes to the entity will be persisted when SaveChanges is called.
        /// If false, the entity will be detached from the context (read-only mode).
        /// Default is true.
        /// </param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the
        /// entity if found; otherwise, null.
        /// </returns>
        /// <remarks>
        /// This method should handle both tracked and untracked entity retrieval.
        /// When <paramref name="asTracked"/> is false, implementations should ensure
        /// the entity is not attached to any change tracking mechanism.
        /// </remarks>
        Task<T?> GetByIdAsync(Guid id,bool asTracked = true,CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all entities from the repository.
        /// </summary>
        /// <param name="asTracked">
        /// A value indicating whether the returned entities should be tracked.
        /// Default is false for performance reasons.
        /// </param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains
        /// a collection of all entities.
        /// </returns>
        /// <remarks>
        /// Use with caution for large datasets. Consider using paging or specific queries
        /// instead of retrieving all entities at once.
        /// </remarks>
        Task<IEnumerable<T>> GetAllAsync(bool asTracked = false,CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all entities as an <see cref="IQueryable{T}"/> for further querying.
        /// </summary>
        /// <param name="asTracked">
        /// A value indicating whether the returned entities should be tracked.
        /// Default is false for performance reasons.
        /// </param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        ///  A task that represents the asynchronous operation. The task result contains
        /// a collection of all entities As Quarable List.
        /// </returns>
        Task<IQueryable<T>> GetAllAsQueryableAsync(bool asTracked = false,CancellationToken cancellationToken = default);
        /// <summary>
        /// Adds a new entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests.
        /// </param>
        /// <returns>A task that represents the asynchronous add operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="entity"/> is null.
        /// </exception>
        /// <remarks>
        /// The entity will be persisted when <c>SaveChanges</c> or equivalent is called
        /// on the underlying data context.
        /// </remarks>
        Task AddAsync(T entity,CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds multiple entities to the repository in a single operation.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests.
        /// </param>
        /// <returns>A task that represents the asynchronous bulk add operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="entities"/> is null.
        /// </exception>
        /// <remarks>
        /// This method is more efficient than multiple <see cref="AddAsync"/> calls
        /// for bulk insert scenarios.
        /// </remarks>
        Task AddRangeAsync(IEnumerable<T> entities,CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes an entity from the repository.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="entity"/> is null.
        /// </exception>
        /// <remarks>
        /// The removal will be persisted when <c>SaveChanges</c> or equivalent is called
        /// on the underlying data context.
        /// </remarks>
        void Remove(T entity);

        /// <summary>
        /// Removes multiple entities from the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to remove.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="entities"/> is null.
        /// </exception>
        void RemoveRange(IEnumerable<T> entities);

        /// <summary>
        /// Checks if an entity with the specified identifier exists in the repository.
        /// </summary>
        /// <param name="id">The unique identifier to check.</param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result is true
        /// if an entity with the specified identifier exists; otherwise, false.
        /// </returns>
        Task<bool> ExistsAsync(Guid id,CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the number of entities in the repository.
        /// </summary>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains
        /// the number of entities in the repository.
        /// </returns>
        Task<int> CountAsync(CancellationToken cancellationToken = default);
    }
}