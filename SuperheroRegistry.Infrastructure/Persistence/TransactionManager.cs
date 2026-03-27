using Microsoft.EntityFrameworkCore.Storage;
using SuperheroRegistry.Application.Interfaces;

namespace SuperheroRegistry.Infrastructure.Persistence;

/// <summary>
/// Manages database transactions for maintaining data consistency across operations.
/// </summary>
public class TransactionManager : ITransactionManager
{
    private readonly AppDbContext _dbContext;
    private IDbContextTransaction? _currentTransaction;

    public TransactionManager(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Begins a new transaction.
    /// </summary>
    public async Task BeginAsync()
    {
        if (_currentTransaction != null)
        {
            throw new InvalidOperationException("A database transaction is already in progress. Nested transactions are not supported by TransactionManager.");
        }
        _currentTransaction = await _dbContext.Database.BeginTransactionAsync();
    }

    /// <summary>
    /// Commits the current transaction.
    /// </summary>
    public async Task CommitAsync()
    {
        try
        {
            await _currentTransaction?.CommitAsync()!;
        }
        catch
        {
            await RollbackAsync();
            throw;
        }
        finally
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }
    }

    /// <summary>
    /// Rolls back the current transaction.
    /// </summary>
    public async Task RollbackAsync()
    {
        try
        {
            await _currentTransaction?.RollbackAsync()!;
        }
        finally
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }
    }

    /// <summary>
    /// Executes an operation within an automatic transaction.
    /// Commits on success, rolls back on exception.
    /// </summary>
    public async Task<T> ExecuteAsync<T>(Func<Task<T>> operation)
    {
        await BeginAsync();
        try
        {
            var result = await operation();
            await _dbContext.SaveChangesAsync();
            await _currentTransaction?.CommitAsync()!;
            return result;
        }
        catch
        {
            await RollbackAsync();
            throw;
        }
        finally
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }
    }

    /// <summary>
    /// Executes an operation within an automatic transaction without returning a value.
    /// </summary>
    public async Task ExecuteAsync(Func<Task> operation)
    {
        await BeginAsync();
        try
        {
            await operation();
            await _dbContext.SaveChangesAsync();
            await _currentTransaction?.CommitAsync()!;
        }
        catch
        {
            await RollbackAsync();
            throw;
        }
        finally
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }
    }

    /// <summary>
    /// Disposes the current transaction.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_currentTransaction != null)
        {
            await RollbackAsync();
        }
    }
}
