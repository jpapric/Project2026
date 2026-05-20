using Server.Domain;

public class PlcDataCache
{
    private EAF? _latestData;
    private readonly object _lock = new();

    public void Update(EAF data)
    {
        lock (_lock) _latestData = data;
    }

    public EAF? Get()
    {
        lock (_lock) return _latestData;
    }
}