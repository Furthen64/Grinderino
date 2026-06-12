namespace Grinderino.Models;

public enum ToolTier
{
    Wooden = 1,
    Stone  = 2,
    Iron   = 3,
    Gold   = 4,
    Diamond = 5
}

public class Tool
{
    public string Name { get; set; }
    public ToolTier Tier { get; set; }
    public int Power { get; set; }
    public int SharpnessLevel { get; set; }
    public int SteelLevel { get; set; }

    public Tool(string name, ToolTier tier, int basePower)
    {
        Name = name;
        Tier = tier;
        Power = basePower;
        SharpnessLevel = 0;
        SteelLevel = 0;
    }

    public int EffectivePower => Power + SharpnessLevel * 2 + SteelLevel;

    public float MineSpeed => 1f + SharpnessLevel * 0.25f;

    public int UpgradeCostSharpness => (SharpnessLevel + 1) * 30;
    public int UpgradeCostSteel     => (SteelLevel + 1) * 50;
    public int MaxSharpnessLevel    => 5;
    public int MaxSteelLevel        => 5;
}

public static class ToolCatalogue
{
    public static Tool BasicPickaxe()   => new Tool("Wooden Pickaxe", ToolTier.Wooden,  2);
    public static Tool StonePickaxe()   => new Tool("Stone Pickaxe",  ToolTier.Stone,   4);
    public static Tool IronPickaxe()    => new Tool("Iron Pickaxe",   ToolTier.Iron,    6);
    public static Tool GoldPickaxe()    => new Tool("Gold Pickaxe",   ToolTier.Gold,    8);
    public static Tool DiamondPickaxe() => new Tool("Diamond Pickaxe",ToolTier.Diamond, 12);

    public static int BuyCost(ToolTier tier) => tier switch
    {
        ToolTier.Stone   => 100,
        ToolTier.Iron    => 300,
        ToolTier.Gold    => 800,
        ToolTier.Diamond => 2000,
        _                => 0
    };
}
