using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;

namespace ViggosScraper.Model;

public interface ICache<in TId, T>
    where TId : notnull
    where T : notnull
{
    public void SetName(string name);
    public bool Contains(TId key, out T? value);
    public Task<T> Get(TId key, Func<Task<T>> supplier, TimeSpan cacheTime);
    public bool TryGet(TId key, out T? result);
    public T Set(TId id, T item, TimeSpan cacheTime);
    public void Remove(TId key);
    public void Destroy();
}

internal class Cache<TId, T> : ICache<TId, T> where TId : notnull where T : notnull
{
    private readonly MemoryCache _cache;
    private readonly ConcurrentDictionary<string, Task<T>> _loading = new();
    private string? _name;

    public Cache(MemoryCache cache)
    {
        _cache = cache;
    }

    #region ICache<TId,T> Members

    public void SetName(string name)
    {
        if (_name is not null) throw new System.Exception("Cache has already be named");

        _name = name;
    }

    public bool Contains(TId id, out T? value)
    {
        var found = _cache.TryGetValue(GetKey(id), out var result);
        if (found) value = (T) result!;
        else value = default;
        return found;
    }

    public async Task<T> Get(TId id, Func<Task<T>> supplier, TimeSpan cacheTime)
    {
        var key = GetKey(id);
        if (_cache.TryGetValue(key, out var result)) return (T) result!;

        if (_loading.TryGetValue(key, out var task)) return await task;

        try
        {
            return await _loading.GetOrAdd(key, Get(key, supplier, cacheTime));
        }
        finally
        {
            _loading.TryRemove(key, out _);
        }
    }

    public bool TryGet(TId key, out T? result)
    {
        return _cache.TryGetValue(GetKey(key), out result);
    }

    public T Set(TId id, T item, TimeSpan cacheTime)
    {
        var key = GetKey(id);
        return _cache.Set(key, item, cacheTime);
    }

    public void Remove(TId key)
    {
        _cache.Remove(GetKey(key));
    }

    public void Destroy()
    {
        _cache.Clear();
    }

    #endregion

    private async Task<T> Get(string key, Func<Task<T>> supplier, TimeSpan cacheTime)
    {
        var response = await supplier.Invoke();
        _cache.Set(key, response, cacheTime);
        return response;
    }

    /// <summary>
    ///     Creates a unique key for the cache index.
    ///     This ensures that different caches with same key does not collide
    /// </summary>
    /// <param name="key">The key for the cache object</param>
    /// <returns>The name of the key, formatted as ${name--keyType-valueType-key}</returns>
    private string GetKey(TId key)
    {
        var keyName = "";
        if (_name is not null) keyName += $"{_name}--";

        var keyType = typeof(TId).FullName;
        if (keyType is not null) keyName += $"{keyType}-";

        var valueType = typeof(T).FullName;
        if (valueType is not null) keyName += $"{valueType}-";

        return keyName + key;
    }
}