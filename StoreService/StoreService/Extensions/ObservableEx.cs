using System.Reactive.Linq;
using System.Reactive;

namespace StoreService.Extensions
{
  public static class ObservableEx
  {
    public static IObservable<Unit> SelectAsync<TSrc>(this IObservable<TSrc> _this, Func<TSrc, CancellationToken, Task> _selector)
    {
      Func<TSrc, CancellationToken, Task> _selector2 = _selector;
      return _this.Select((TSrc _x) => Observable.FromAsync((CancellationToken _cancellation) => _selector2(_x, _cancellation))).Concat();
    }
  }
}
