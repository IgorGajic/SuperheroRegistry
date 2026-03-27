namespace SuperheroRegistry.Application.Interfaces;

/// <summary>
/// Manages database transactions for maintaining data consistency.
/// Wraps DbContext transactions in a simple, reusable interface.
/// </summary>
public interface ITransactionManager : IAsyncDisposable
{
    /// <summary>
    /// Begins a new transaction.
    /// </summary>
    Task BeginAsync();

    /// <summary>
    /// Commits the current transaction.
    /// </summary>
    Task CommitAsync();

    /// <summary>
    /// Rolls back the current transaction.
    /// </summary>
    Task RollbackAsync();

    /// <summary>
    /// Executes an operation within a transaction.
    /// Automatically commits on success, rolls back on exception.
    /// </summary>
    /// <typeparam name="T">The return type of the operation.</typeparam>
    /// <param name="operation">The async operation to execute.</param>
    /// <returns>The result of the operation.</returns>
    Task<T> ExecuteAsync<T>(Func<Task<T>> operation);

    /// <summary>
    /// Executes an operation within a transaction without returning a value.
    /// Automatically commits on success, rolls back on exception.
    /// </summary>
    /// <param name="operation">The async operation to execute.</param>
    Task ExecuteAsync(Func<Task> operation);
}
