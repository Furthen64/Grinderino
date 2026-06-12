using Microsoft.Xna.Framework;

namespace Grinderino.Models;

public enum BlockType
{
    Air,
    Dirt,
    Stone,
    Bedrock,
    CoalOre,
    IronOre,
    GoldOre,
    DiamondOre,
    Treasure
}

public static class BlockData
{
    public static int GetHardness(BlockType type) => type switch
    {
        BlockType.Air      => 0,
        BlockType.Dirt     => 1,
        BlockType.Stone    => 3,
        BlockType.Bedrock  => int.MaxValue,
        BlockType.CoalOre  => 2,
        BlockType.IronOre  => 4,
        BlockType.GoldOre  => 5,
        BlockType.DiamondOre => 8,
        BlockType.Treasure => 3,
        _                  => 1
    };

    public static Color GetColor(BlockType type) => type switch
    {
        BlockType.Air        => Color.Transparent,
        BlockType.Dirt       => new Color(139, 90, 43),
        BlockType.Stone      => new Color(128, 128, 128),
        BlockType.Bedrock    => new Color(40, 40, 40),
        BlockType.CoalOre    => new Color(60, 60, 60),
        BlockType.IronOre    => new Color(200, 160, 120),
        BlockType.GoldOre    => new Color(255, 215, 0),
        BlockType.DiamondOre => new Color(100, 220, 255),
        BlockType.Treasure   => new Color(255, 140, 0),
        _                    => Color.Purple
    };

    public static string GetName(BlockType type) => type switch
    {
        BlockType.CoalOre    => "Coal",
        BlockType.IronOre    => "Iron Ore",
        BlockType.GoldOre    => "Gold Ore",
        BlockType.DiamondOre => "Diamond",
        BlockType.Treasure   => "Artifact",
        _                    => type.ToString()
    };

    public static int GetValue(BlockType type) => type switch
    {
        BlockType.CoalOre    => 5,
        BlockType.IronOre    => 15,
        BlockType.GoldOre    => 50,
        BlockType.DiamondOre => 200,
        BlockType.Treasure   => 500,
        _                    => 0
    };

    public static bool IsMineable(BlockType type) => type != BlockType.Air && type != BlockType.Bedrock;
}
