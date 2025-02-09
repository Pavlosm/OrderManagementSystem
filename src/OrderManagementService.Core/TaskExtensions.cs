using System.Diagnostics.CodeAnalysis;

namespace OrderManagementService.Core;

public static class TaskExtensions
{
    /// <summary>
    /// Observes and ignores a potential exception on a given Task.
    /// If a Task fails and throws an exception which is never observed, it will be caught by the .NET finalizer thread.
    /// This function awaits the given task and if the exception is thrown, it observes this exception and simply ignores it.
    /// This will prevent the escalation of this exception to the .NET finalizer thread.
    /// </summary>
    /// <param name="task">The task to be ignored.</param>
    [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "ignored")]
    public static void Ignore(this Task task)
    {
        if (task.IsCompleted)
        {
            // ReSharper disable once UnusedVariable
            _ = task.Exception;
        }
        else
        {
            task.ContinueWith(
                // ReSharper disable once UnusedVariable
                t => { var ignored = t.Exception; },
                CancellationToken.None,
                TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);
        }
    }
}