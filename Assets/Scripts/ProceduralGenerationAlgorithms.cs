using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProceduralGenerationAlgorithms
{
    // Generates a simple random walk path starting from the given position
    // Parameters:
    // - startPosition: The starting position of the walk
    // - walkLength: The number of steps in the walk
    // Returns:
    // - A HashSet containing the positions visited during the walk
    public static HashSet<Vector2Int> SimpleRandomWalk(Vector2Int startPosition, int walkLength)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();

        path.Add(startPosition);
        var previousPosition = startPosition;

        for (int i = 0; i < walkLength; i++)
        {
            var newPosition = previousPosition + Direction2D.GetRandomCardinalDirection();
            path.Add(newPosition);
            previousPosition = newPosition;
        }

        return path;
    }

    public static List<Vector2Int> RandomWalkCorridor(Vector2Int startPosition, int corridorLength)
    {
        List<Vector2Int> corridor = new List<Vector2Int>(); // Create a new list to store the corridor positions
        var direction = Direction2D.GetRandomCardinalDirection(); // Get a random cardinal direction
        var currentPosition = startPosition; // Set the current position to the starting position
        corridor.Add(currentPosition); // Add the starting position to the corridor

        for (int i = 0; i < corridorLength; i++)
        {
            currentPosition += direction; // Move in the chosen direction
            corridor.Add(currentPosition); // Add the new position to the corridor
        }

        return corridor; // Return the generated corridor
    }

    // Binary Space Partitioning (BSP) algorithm
    public static List<BoundsInt> BinarySpacePartitioning(BoundsInt spaceToSplit, int minWidth, int minHeight)
    {
        Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>();
        List<BoundsInt> roomsList = new List<BoundsInt>();
        roomsQueue.Enqueue(spaceToSplit);
        while (roomsQueue.Count > 0)
        {
            var room = roomsQueue.Dequeue();
            // Check if the room size is big enough to be split
            if (room.size.y >= minHeight && room.size.x >= minWidth)
            { 
                // Randomly choose the type of split
                if (Random.value > 0.5f)
                {
                    if (room.size.y >= minHeight * 2)
                    {
                        // Split horizontally if the room height is large enough
                        SplitHorizontally(minHeight, roomsQueue, room);
                    }else if (room.size.x >= minWidth * 2)
                    {
                        // Split vertically if the room width is large enough
                        SplitVertically(minWidth, roomsQueue, room);
                    }else
                    {
                        // If the room cannot be split further, add it to the list of rooms
                        roomsList.Add(room);
                    }
                }
                else
                {
                    if (room.size.x >= minWidth * 2)
                    {
                        SplitVertically(minWidth, roomsQueue, room);
                    }else if (room.size.y >= minHeight * 2)
                    {
                        SplitHorizontally(minHeight, roomsQueue, room);
                    }else
                    {
                        roomsList.Add(room);
                    }
                }
            }
        }
        return roomsList;
    }

    // Split the given room vertically
    private static void SplitVertically(int minWidth, Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var xSplit = Random.Range(1, room.size.x);
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(xSplit, room.size.y, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x+xSplit,room.min.y,room.min.z),
            new Vector3Int(room.size.x-xSplit, room.size.y, room.size.z));
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    // Split the given room horizontally
    private static void SplitHorizontally(int minHeight, Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var ySplit = Random.Range(1, room.size.y);
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(room.size.x, ySplit, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x, room.min.y+ySplit, room.min.z),
            new Vector3Int(room.size.x, room.size.y-ySplit, room.size.z));
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }
}

public static class Direction2D
{
    // List of cardinal directions in 2D
    public static List<Vector2Int> CardinalDirectionList = new List<Vector2Int>
    {
        new Vector2Int(0, 1),  // UP
        new Vector2Int(1, 0),  // RIGHT
        new Vector2Int(0, -1),  // DOWN
        new Vector2Int(-1, 0) // LEFT
    };

    public static List<Vector2Int> DiagonalDirectionList = new List<Vector2Int>
    {
        new Vector2Int(1, 1),  // UP-RIGHT
        new Vector2Int(1, -1),  // RIGHT-DOWN
        new Vector2Int(-1, -1),  // DOWN-LEFT
        new Vector2Int(-1, 1) // LEFT-UP
    };

    public static List<Vector2Int> EightDirectionList = new List<Vector2Int>
    {
        new Vector2Int(0, 1),  // UP
        new Vector2Int(1, 1),  // UP-RIGHT
        new Vector2Int(1, 0),  // RIGHT
        new Vector2Int(1, -1),  // RIGHT-DOWN
        new Vector2Int(0, -1),  // DOWN
        new Vector2Int(-1, -1),  // DOWN-LEFT
        new Vector2Int(-1, 0), // LEFT
        new Vector2Int(-1, 1) // LEFT-UP
    };

    // Returns a random cardinal direction from the CardinalDirectionList
    // Returns:
    // - A random Vector2Int representing a cardinal direction
    public static Vector2Int GetRandomCardinalDirection()
    {
        return CardinalDirectionList[Random.Range(0, CardinalDirectionList.Count)];
    }
}

