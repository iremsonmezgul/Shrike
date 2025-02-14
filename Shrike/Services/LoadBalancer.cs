using System.Linq;
using System.Threading.Tasks;
using Shrike.Models;
using Shrike.Services;

public class LoadBalancer
{
    private readonly RedisCacheService _cacheService;
    private readonly Dictionary<string, int> _tempCallCount = new();

    public LoadBalancer(RedisCacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task<Hummingbird?> GetRedirectHummingbird()
    {
        var hummingbirds = await _cacheService.GetAllHummingbirds();
        var sortedHummingbirds = hummingbirds.OrderBy(h => h.ActiveCallCount).ToList();

        if (sortedHummingbirds.Count == 0)
            return null;

        var selected = sortedHummingbirds.First();
        _tempCallCount[selected.Id] = _tempCallCount.GetValueOrDefault(selected.Id, 0) + 1;

        return selected;
    }

    public async Task RefreshHummingbirds()
    {
        var hummingbirds = await _cacheService.GetAllHummingbirds();

        foreach (var hummingbird in hummingbirds)
        {
            var updatedHummingbird = await HummingbirdService.FetchFromSystemInfo(hummingbird.Address);
            if (updatedHummingbird != null)
            {
                await _cacheService.UpdateHummingbird(updatedHummingbird);
            }
        }
    }
}
