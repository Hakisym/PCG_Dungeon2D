using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorridorFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    [SerializeField]
    private int corridorLength = 14, corridorCount = 5;
    [SerializeField]
    [Range(0.1f, 1)]
    private float roomPercent = 0.8f;

    protected override void RunProceduralGeneration()
    {
        CorridorFirstGeneration();
    }

    private void CorridorFirstGeneration()
    {
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        HashSet<Vector2Int> potentialRoomPositions = new HashSet<Vector2Int>();

        List<List<Vector2Int>> corridors = CreateCorridors(floorPositions, potentialRoomPositions); // Create corridors

        HashSet<Vector2Int> roomPositions = CreateRooms(potentialRoomPositions); // Create rooms

        List<Vector2Int> deadEnds = FindAllDeadEnds(floorPositions); // Find all dead ends in the dungeon

        CreateRoomsAtDeadEnd(deadEnds, roomPositions); // Create rooms at dead ends

        floorPositions.UnionWith(roomPositions); // Combine corridors and rooms to get final floor positions

        //for (int i = 0; i < corridors.Count; i++)
        //{
        //    //corridors[i] = IncreaseCorridorSizeByOne(corridors[i]);
        //    corridors[i] = IncreaseCorridorBrush3By3(corridors[i]); // Increase corridor size by 3x3 brush
        //    floorPositions.UnionWith(corridors[i]);
        //}

        tilemapVisualizer.PaintFloorTiles(floorPositions); // Paint floor tiles
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer); // Create walls based on floor positions
    }


    // Method to increase the size of the corridor by a 3x3 brush
    private List<Vector2Int> IncreaseCorridorBrush3By3(List<Vector2Int> corridor)
    {
        List<Vector2Int> newCorridor = new List<Vector2Int>();
        for (int i = 0; i < corridor.Count - 1; i++)
        {
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    newCorridor.Add(corridor[i] + new Vector2Int(x, y));
                }
            }
        }
        return newCorridor;
    }

    // Method to increase the size of the corridor by a 2x2 brush
    private List<Vector2Int> IncreaseCorridorSizeByOne(List<Vector2Int> corridor)
    {
        List<Vector2Int> newCorridor = new List<Vector2Int>();
        Vector2Int directionFromCell = corridor[corridor.Count-1] - corridor[corridor.Count-2];
        for (int i = 0; i < corridor.Count; i++)
        {
            Vector2Int newCorridorTileOffset
                = GetDirection90From(directionFromCell);
            newCorridor.Add(corridor[i]);
            newCorridor.Add(corridor[i] + newCorridorTileOffset);
        }
        //patch corner
        newCorridor.Add(corridor[corridor.Count - 1] + new Vector2Int(1, 1));
        return newCorridor;
    }

    // Method to rotate the direction by 90 degrees
    private Vector2Int GetDirection90From(Vector2Int direction)
    {
        //hold horizontal and vertical direction
        if (direction == Vector2Int.up || direction == Vector2Int.down)
            return Vector2Int.right;
        if (direction == Vector2Int.right || direction == Vector2Int.left)
            return Vector2Int.up;
        return Vector2Int.zero;
    }

    // Method to create rooms using RandomWalk algorithms
    private void CreateRoomsAtDeadEnd(List<Vector2Int> deadEnds, HashSet<Vector2Int> roomFloors)
    {
        foreach (var position in deadEnds)
        {
            //remove dead ends that already have a room
            if (!roomFloors.Contains(position))
            {
                var room = RunRandomWalk(randomWalkParameters, position);
                roomFloors.UnionWith(room);
            }
        }
    }

    // Method to find all dead ends
    private List<Vector2Int> FindAllDeadEnds(HashSet<Vector2Int> floorPositions)
    {
        List<Vector2Int> deadEnds = new List<Vector2Int>();
        foreach (var position in floorPositions)
        {
            int neighborCount = 0;
            foreach (var direction in Direction2D.CardinalDirectionList)
            {
                if (floorPositions.Contains(position + direction))
                    neighborCount++;
            }
            //dead end has one neighbor block only
            if (neighborCount == 1)
                deadEnds.Add(position);
        }
        return deadEnds;
    }

    private HashSet<Vector2Int> CreateRooms(HashSet<Vector2Int> potentialRoomPositions)
    {
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        int roomToCreateCount = Mathf.RoundToInt(potentialRoomPositions.Count * roomPercent);

        // Randomly select positions from potentialRoomPositions to create rooms
        List<Vector2Int> roomToCreate = potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomToCreateCount).ToList();

        // Generate and add each room's floor positions to the roomPositions set
        foreach (var roomPosition in roomToCreate)
        {
            var roomFloor = RunRandomWalk(randomWalkParameters, roomPosition); // Generate room floor using random walk
            roomPositions.UnionWith(roomFloor); // Combine room floor with existing room positions
        }

        return roomPositions;
    }

    private List<List<Vector2Int>> CreateCorridors(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> potentialRoomPositions)
    {
        var currentPosition = startPosition;
        potentialRoomPositions.Add(currentPosition); // Add the starting position as a potential room position

        List<List<Vector2Int>> corridors = new List<List<Vector2Int>>();
        // Generate corridors
        for (int i = 0; i < corridorCount; i++)
        {
            var corridor = ProceduralGenerationAlgorithms.RandomWalkCorridor(currentPosition, corridorLength); // Generate a random walk corridor
            corridors.Add(corridor);

            currentPosition = corridor[corridor.Count - 1]; // Update the current position to the end of the corridor
            potentialRoomPositions.Add(currentPosition); // Add the end of the corridor as a potential room position
            floorPositions.UnionWith(corridor); // Combine the corridor with the existing floor positions
        }
        return corridors;
    }
}


