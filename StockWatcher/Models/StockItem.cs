﻿namespace StockWatcher.Models;

public class StockItem
{
    public string SecId { get; set; }
    public string Name { get; set; }
    public int LotSize { get; set; }
    public byte Level { get; set; }
}