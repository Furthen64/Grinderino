using System.Collections.Generic;

namespace Grinderino.Models;

public class Inventory
{
    private readonly Dictionary<BlockType, int> _items = new();

    public void Add(BlockType type, int count = 1)
    {
        if (!_items.ContainsKey(type)) _items[type] = 0;
        _items[type] += count;
    }

    public int Count(BlockType type) => _items.TryGetValue(type, out int v) ? v : 0;

    public bool Remove(BlockType type, int count = 1)
    {
        if (Count(type) < count) return false;
        _items[type] -= count;
        if (_items[type] <= 0) _items.Remove(type);
        return true;
    }

    public IReadOnlyDictionary<BlockType, int> Items => _items;

    public int TotalSellValue()
    {
        int total = 0;
        foreach (var kv in _items)
            total += BlockData.GetValue(kv.Key) * kv.Value;
        return total;
    }

    public int SellAll()
    {
        int value = TotalSellValue();
        _items.Clear();
        return value;
    }

    public int SellItem(BlockType type, int count)
    {
        int actual = System.Math.Min(count, Count(type));
        if (actual <= 0) return 0;
        Remove(type, actual);
        return BlockData.GetValue(type) * actual;
    }
}
