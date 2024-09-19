using StoreService.Models.WebSocketModels;

namespace StoreService.ImitationCarTravel
{
  public static class Travel
  {
    private static int _initialSpeed = 0;
    public static async Task<WsMessageSpeedUpdate> StartTravel()
    {
      _initialSpeed = Random.Shared.Next(60, 180);
      return new WsMessageSpeedUpdate(_initialSpeed);
    }
  }
}
