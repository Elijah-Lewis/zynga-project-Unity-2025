using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public class Cell
    {
        public bool visited = false;
        public bool[] status = new bool[4]; // Order: Up, Down, Right, Left
    }

    public Vector2 size;
    public int startPos = 0;
    public GameObject room;
    public Vector2 offset; // Distance between rooms
    public Vector3 SpawnPosition;
    public float roomScale = 1f; // Scale factor for rooms

    private List<Cell> board;
    private HashSet<Vector3> occupiedPositions = new HashSet<Vector3>(); // Prevent overlap

    void Start()
    {
        MazeGenerator();
    }

    void GenerateDungeon()
    {
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                Cell currentCell = board[Mathf.FloorToInt(i + j * size.x)];
                if (currentCell.visited)
                {
                    Vector3 roomPosition = new Vector3(i * offset.x * roomScale, 0, -j * offset.y * roomScale) + SpawnPosition; // Apply scale and SpawnPosition

                    if (!occupiedPositions.Contains(roomPosition))
                    {
                        var newRoom = Instantiate(room, roomPosition, Quaternion.identity, transform);

                        // Scale the room
                        newRoom.transform.localScale = new Vector3(roomScale, roomScale, roomScale);

                        // Ensure Rigidbody is set to Kinematic to prevent flying
                        Rigidbody rb = newRoom.GetComponent<Rigidbody>();
                        if (rb != null)
                        {
                            rb.isKinematic = true;
                            rb.linearVelocity = Vector3.zero;
                            rb.angularVelocity = Vector3.zero;
                        }

                        newRoom.GetComponent<RoomBehaviour>().UpdateRoom(currentCell.status);
                        newRoom.name = $"Room {i}-{j}";
                        occupiedPositions.Add(roomPosition);
                    }
                }
            }
        }
    }

    void MazeGenerator()
    {
        board = new List<Cell>();

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                board.Add(new Cell());
            }
        }

        int currentCell = startPos;
        Stack<int> path = new Stack<int>();
        int k = 0;

        while (k < 1000)
        {
            k++;
            board[currentCell].visited = true;

            if (currentCell == board.Count - 1)
                break;

            List<int> neighbors = CheckNeighbors(currentCell);

            if (neighbors.Count == 0)
            {
                if (path.Count == 0) break;
                currentCell = path.Pop();
            }
            else
            {
                path.Push(currentCell);
                int newCell = neighbors[Random.Range(0, neighbors.Count)];

                // Determine direction and connect rooms correctly
                int dx = newCell % (int)size.x - currentCell % (int)size.x;
                int dy = (newCell / (int)size.x) - (currentCell / (int)size.x);

                if (dx == 1)  // Right
                {
                    board[currentCell].status[2] = true;
                    board[newCell].status[3] = true;
                }
                else if (dx == -1)  // Left
                {
                    board[currentCell].status[3] = true;
                    board[newCell].status[2] = true;
                }
                else if (dy == 1)  // Down
                {
                    board[currentCell].status[1] = true;
                    board[newCell].status[0] = true;
                }
                else if (dy == -1)  // Up
                {
                    board[currentCell].status[0] = true;
                    board[newCell].status[1] = true;
                }

                currentCell = newCell;
            }
        }
        GenerateDungeon();
    }

    List<int> CheckNeighbors(int cell)
    {
        List<int> neighbors = new List<int>();

        int x = cell % (int)size.x;
        int y = cell / (int)size.x;

        // Up
        if (y > 0 && !board[cell - (int)size.x].visited)
            neighbors.Add(cell - (int)size.x);

        // Down
        if (y < size.y - 1 && !board[cell + (int)size.x].visited)
            neighbors.Add(cell + (int)size.x);

        // Right
        if (x < size.x - 1 && !board[cell + 1].visited)
            neighbors.Add(cell + 1);

        // Left
        if (x > 0 && !board[cell - 1].visited)
            neighbors.Add(cell - 1);

        return neighbors;
    }
}
