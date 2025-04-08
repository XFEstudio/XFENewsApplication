namespace XFENewsApplication.Utilities;

public class HistoryManager<T, R>(int capacity = 100) where T : notnull
{
    private readonly LinkedList<T> _historyList = new();
    private readonly Dictionary<T, R> _historyMap = [];

    public void Visit(T visitValue, R content)
    {
        if (_historyMap.ContainsKey(visitValue))
        {
            _historyList.Remove(visitValue);
        }
        else if (_historyList.Count >= capacity)
        {
            var lastNode = _historyList.Last;
            if (lastNode is not null)
            {
                _historyMap.Remove(lastNode.Value);
                _historyList.RemoveLast();
            }
        }
        _historyList.AddFirst(visitValue);
        _historyMap[visitValue] = content;
    }

    public List<R> Export() => [.. _historyList.Select(history => _historyMap[history])];
}
