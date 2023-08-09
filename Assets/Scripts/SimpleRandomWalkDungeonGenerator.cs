using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;

public class SimpleRandomWalkDungeonGenerator : AbstractDungeonGenerator
{
    [SerializeField]
    protected SimpleRandomWalkSO randomWalkParameters;

    // Override the method from the base class to run the procedural generation algorithm
    protected override void RunProceduralGeneration()
    {
        HashSet<Vector2Int> floorPositions = RunRandomWalk(randomWalkParameters,startPosition); // Generate the dungeon map using random walks
        tilemapVisualizer.PaintFloorTiles(floorPositions); // Paint the floor tiles on the tilemap
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
    }

    // Generate the dungeon map using random walks
    protected HashSet<Vector2Int> RunRandomWalk(SimpleRandomWalkSO parameters, Vector2Int position)
    {
        var currentPosition = position;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();

        for (int i = 0; i < parameters.iterations; i++)
        {
            var path = ProceduralGenerationAlgorithms.SimpleRandomWalk(currentPosition, parameters.walkLength); // Generate a random walk path
            floorPositions.UnionWith(path); // Combine the generated path with the existing floor positions

            if (parameters.startRandomlyEachIteration)
                currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count)); // Randomly choose a new starting position each iteration
        }

        return floorPositions;
    }
}


