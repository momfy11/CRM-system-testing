using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SwineSync.Tests;

public class TestSession : ISession
{
  private readonly Dictionary<string, byte[]> _storage = new();

  public IEnumerable<string> Keys => _storage.Keys; // id, company, role

  public string Id => Guid.NewGuid().ToString(); // session id

  public bool IsAvailable => true; // session aktiv forever

  public void Clear() => _storage.Clear();

  public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask; // fake sparar session

  public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask; // fake  session load data

  public void Remove(string key) => _storage.Remove(key);

  public void Set(string key, byte[] value) => _storage[key] = value; // eco enterprises

  public bool TryGetValue(string key, out byte[] value) => _storage.TryGetValue(key, out value);
}
