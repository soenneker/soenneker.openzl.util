using System;
namespace Soenneker.OpenZl.Util.Native;

// One retained context bounds idle native memory. Concurrent callers get independent contexts.
internal sealed class NativeContextPool<T>(Func<T> factory) : IDisposable where T : class, IDisposable
{
    private readonly object _gate = new();
    private T? _idle;
    private bool _disposed;

    internal T Rent()
    {
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (_idle is { } value) { _idle = null; return value; }
        }
        return factory();
    }
    internal void Return(T context)
    {
        lock (_gate)
        {
            if (!_disposed && _idle == null) { _idle = context; return; }
        }
        context.Dispose();
    }
    internal void ThrowIfDisposed() { lock (_gate) ObjectDisposedException.ThrowIf(_disposed, this); }
    public void Dispose()
    {
        T? context;
        lock (_gate) { _disposed = true; context = _idle; _idle = null; }
        context?.Dispose();
    }
}
