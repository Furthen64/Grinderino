using System;
using Microsoft.Xna.Framework;

namespace Grinderino.Models;

public class World
{
    public const int BlockSize  = 32;
    public const int WorldWidth = 40;
    public const int WorldHeight = 80;

    private readonly BlockType[,] _blocks = new BlockType[WorldWidth, WorldHeight];
    private readonly Random _rng;

    public World(int seed = 0)
    {
        _rng = seed == 0 ? new Random() : new Random(seed);
        Generate();
    }

    private void Generate()
    {
        for (int x = 0; x < WorldWidth; x++)
        {
            for (int y = 0; y < WorldHeight; y++)
            {
                if (y == 0)
                {
                    _blocks[x, y] = BlockType.Air;
                    continue;
                }

                // Bottom row is bedrock
                if (y == WorldHeight - 1)
                {
                    _blocks[x, y] = BlockType.Bedrock;
                    continue;
                }

                // Left/right walls are bedrock
                if (x == 0 || x == WorldWidth - 1)
                {
                    _blocks[x, y] = BlockType.Bedrock;
                    continue;
                }

                // Surface layer (rows 1-3) is dirt
                if (y <= 3)
                {
                    _blocks[x, y] = BlockType.Dirt;
                    continue;
                }

                // Deep bedrock veins
                if (y >= WorldHeight - 5)
                {
                    _blocks[x, y] = BlockType.Bedrock;
                    continue;
                }

                _blocks[x, y] = GenerateBlock(x, y);
            }
        }

        // Open starting area (3x2 above spawn)
        int spawnX = WorldWidth / 2;
        for (int dx = -1; dx <= 1; dx++)
        {
            _blocks[spawnX + dx, 1] = BlockType.Air;
            _blocks[spawnX + dx, 2] = BlockType.Air;
        }
    }

    private BlockType GenerateBlock(int x, int y)
    {
        float depth = (float)y / WorldHeight;

        // Scatter unpickable bedrock pockets throughout the deep
        if (depth > 0.4f && _rng.NextDouble() < 0.04)
            return BlockType.Bedrock;

        int roll = _rng.Next(100);

        if (depth < 0.2f)
        {
            // Shallow - mostly dirt, some coal
            if (roll < 5)  return BlockType.CoalOre;
            return BlockType.Dirt;
        }
        if (depth < 0.4f)
        {
            // Mid-shallow - dirt/stone/coal/iron
            if (roll < 3)  return BlockType.IronOre;
            if (roll < 10) return BlockType.CoalOre;
            if (roll < 40) return BlockType.Stone;
            return BlockType.Dirt;
        }
        if (depth < 0.6f)
        {
            // Mid - stone dominant, ores
            if (roll < 2)  return BlockType.GoldOre;
            if (roll < 6)  return BlockType.IronOre;
            if (roll < 10) return BlockType.CoalOre;
            return BlockType.Stone;
        }
        if (depth < 0.8f)
        {
            // Deep - stone, gold, some diamond
            if (roll < 1)  return BlockType.DiamondOre;
            if (roll < 4)  return BlockType.GoldOre;
            if (roll < 7)  return BlockType.IronOre;
            return BlockType.Stone;
        }
        // Very deep - diamond, treasure possible
        if (roll < 2)  return BlockType.Treasure;
        if (roll < 5)  return BlockType.DiamondOre;
        if (roll < 8)  return BlockType.GoldOre;
        return BlockType.Stone;
    }

    public BlockType GetBlock(int x, int y)
    {
        if (x < 0 || x >= WorldWidth || y < 0 || y >= WorldHeight)
            return BlockType.Bedrock;
        return _blocks[x, y];
    }

    public void SetBlock(int x, int y, BlockType type)
    {
        if (x < 0 || x >= WorldWidth || y < 0 || y >= WorldHeight) return;
        _blocks[x, y] = type;
    }

    public bool IsInBounds(int x, int y) =>
        x >= 0 && x < WorldWidth && y >= 0 && y < WorldHeight;

    public static Rectangle BlockRect(int bx, int by) =>
        new Rectangle(bx * BlockSize, by * BlockSize, BlockSize, BlockSize);
}
