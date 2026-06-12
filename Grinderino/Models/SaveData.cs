using System.Collections.Generic;

namespace Grinderino.Models;

public class RunStats
{
    public int RunNumber { get; set; }
    public int BlocksMined { get; set; }
    public int OresFound { get; set; }
    public int ArtifactsFound { get; set; }
    public int MoneyEarned { get; set; }
    public int MaxDepthReached { get; set; }
    public System.DateTime Date { get; set; } = System.DateTime.Now;
}

public class SaveData
{
    public int Money { get; set; } = 0;
    public Tool CurrentTool { get; set; } = ToolCatalogue.BasicPickaxe();
    public bool HasMetalDetector { get; set; } = false;
    public Inventory Inventory { get; set; } = new Inventory();
    public List<RunStats> RunHistory { get; set; } = new();
    public bool HasSave { get; set; } = false;

    public static SaveData New()
    {
        return new SaveData
        {
            Money = 0,
            CurrentTool = ToolCatalogue.BasicPickaxe(),
            HasMetalDetector = false,
            Inventory = new Inventory(),
            RunHistory = new List<RunStats>(),
            HasSave = true
        };
    }
}
