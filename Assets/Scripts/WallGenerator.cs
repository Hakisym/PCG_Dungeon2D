using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WallGenerator
{
    // Create walls based on the given floor positions
    public static void CreateWalls(HashSet<Vector2Int> floorPositions, TilemapVisualizer tilemapVisualizer)
    {
        // Find positions for basic walls and corner walls
        var basicWallPositions = FindWallInDirections(floorPositions, Direction2D.CardinalDirectionList);
        var cornerWallPositions = FindWallInDirections(floorPositions, Direction2D.DiagonalDirectionList);

        // Create basic walls and corner walls
        CreateBasicWall(tilemapVisualizer, basicWallPositions, floorPositions);
        CreateCornerWall(tilemapVisualizer, cornerWallPositions, floorPositions);
    }

    // Create corner walls at specified positions
    private static void CreateCornerWall(TilemapVisualizer tilemapVisualizer, HashSet<Vector2Int> cornerWallPositions, HashSet<Vector2Int> floorPositions)
    {
        foreach (var position in cornerWallPositions)
        {
            // Get binary representation of neighboring floor positions
            string neighboursBinaryType = "";
            foreach (var direction in Direction2D.EightDirectionList)
            {
                var neighbourPosition = position + direction;
                if (floorPositions.Contains(neighbourPosition))
                {
                    neighboursBinaryType += "1";
                }
                else
                {
                    neighboursBinaryType += "0";
                }
            }


            // Paint a corner wall tile at the specified position with neighbors info
            tilemapVisualizer.PaintSingleCornerWall(position, neighboursBinaryType);
        }
    }

    private static void CreateBasicWall(TilemapVisualizer tilemapVisualizer, HashSet<Vector2Int> basicWallPositions, HashSet<Vector2Int> floorPositions)
    {
        foreach (var position in basicWallPositions)
        {
            string neighboursBinaryType = "";
            foreach (var direction in Direction2D.CardinalDirectionList)
            {
                var neighbourPosition = position + direction;
                if (floorPositions.Contains(neighbourPosition))
                {
                    neighboursBinaryType += "1";
                }
                else
                {
                    neighboursBinaryType += "0";
                }
            }

            tilemapVisualizer.PaintSingleBasicWall(position,neighboursBinaryType); // Paint a basic wall tile at the specified position
        }
    }

    // Find wall positions in the specified directions surrounding the floor positions
    private static HashSet<Vector2Int> FindWallInDirections(HashSet<Vector2Int> floorPositions, List<Vector2Int> directionList)
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();

        foreach (var position in floorPositions)
        {
            foreach (var direction in directionList)
            {
                var neighbourPosition = position + direction;

                if (!floorPositions.Contains(neighbourPosition))
                {
                    wallPositions.Add(neighbourPosition); // Add the wall position if it is not a floor position
                }
            }
        }

        return wallPositions;
    }
}

