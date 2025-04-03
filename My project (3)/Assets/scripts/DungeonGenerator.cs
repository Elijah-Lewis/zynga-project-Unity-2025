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
    public GameObject keyObject; // Reference to the key GameObject
    public GameObject doorObject; // Reference to the door GameObject
    public GameObject enemyObject; // Reference to the enemy GameObject
    public Vector2 offset; // Distance between rooms
    public Vector3 SpawnPosition;
    public float roomScale = 1f; // Scale factor for rooms

    private List<Cell> board;
    private HashSet<Vector3> occupiedPositions = new HashSet<Vector3>(); // Prevent overlap
    private List<GameObject> spawnedRooms = new List<GameObject>(); // Store references to rooms

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
                    Vector3 roomPosition = new Vector3(i * offset.x * roomScale, 0, -j * offset.y * roomScale) + SpawnPosition;

                    if (!occupiedPositions.Contains(roomPosition))
                    {
                        var newRoom = Instantiate(room, roomPosition, Quaternion.identity, transform);
                        newRoom.transform.localScale = new Vector3(roomScale, roomScale, roomScale);

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
                        spawnedRooms.Add(newRoom); // Store reference to spawned room

                        // Randomly spawn enemies in ~1/3 of the rooms
                        if (Random.Range(0f, 1f) <= 0.33f) // 33% chance
                        {
                            SpawnEnemyInRoom(newRoom);
                        }
                    }
                }
            }
        }
        PlaceKeyInRandomRoom(); // Call after rooms are generated
        PlaceDoorInRandomRoom(); // Call after rooms are generated
    }

    public void RegenerateDungeon()
    {
        // Clear all previously spawned rooms, enemies, key, and door
        foreach (GameObject room in spawnedRooms)
        {
            Destroy(room);
        }
        spawnedRooms.Clear();
        occupiedPositions.Clear();

        if (keyObject != null)
            keyObject.transform.position = new Vector3(1000, 1000, 1000); // Move it far away before regenerating

        if (doorObject != null)
            doorObject.transform.position = new Vector3(1000, 1000, 1000); // Move it far away before regenerating

        // Reset the board for a fresh maze generation
        board.Clear();

        // Generate a new dungeon layout
        MazeGenerator();
    }

    void SpawnEnemyInRoom(GameObject room)
    {
        // Find the floor object within the room by name
        Transform floor = room.transform.Find("Floor");

        if (floor != null)
        {
            // Get the position of the floor and adjust the Y position for correct height
            Vector3 floorPosition = floor.position;

            // Place the enemy on top of the floor (adjust the Y position)
            Vector3 enemyPosition = new Vector3(floorPosition.x, floorPosition.y + 1f, floorPosition.z);

            // Instantiate the enemy at the position on top of the floor
            Instantiate(enemyObject, enemyPosition, Quaternion.identity, room.transform);
        }
        else
        {
            Debug.LogWarning("Floor object not found in room!");
        }
    }

    void PlaceKeyInRandomRoom()
    {
        if (spawnedRooms.Count == 0 || keyObject == null) return; // Safety check

        int randomIndex = Random.Range(0, spawnedRooms.Count);
        GameObject selectedRoom = spawnedRooms[randomIndex];

        // Determine key position (center of the room)
        Vector3 keyPosition = selectedRoom.transform.position + Vector3.up * 1.5f; // Adjust Y for visibility

        keyObject.transform.position = keyPosition;
    }

    void PlaceDoorInRandomRoom()
    {
        if (spawnedRooms.Count == 0 || doorObject == null) return; // Safety check

        int randomIndex = Random.Range(0, spawnedRooms.Count);
        GameObject selectedRoom = spawnedRooms[randomIndex];

        // Determine door position (center of the room but lower for placement)
        Vector3 doorPosition = selectedRoom.transform.position + Vector3.up * 0.5f; // Adjust Y for proper placement

        doorObject.transform.position = doorPosition;
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

    //add a fade transition before changfing scenes
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 1f;

    private IEnumerator FadeAndRegenerate()
    {
        yield return StartCoroutine(FadeOut()); // Fade to black

        // Clear previous dungeon
        foreach (GameObject room in spawnedRooms)
        {
            Destroy(room);
        }
        spawnedRooms.Clear();
        occupiedPositions.Clear();

        if (keyObject != null) keyObject.transform.position = new Vector3(1000, 1000, 1000);
        if (doorObject != null) doorObject.transform.position = new Vector3(1000, 1000, 1000);

        board.Clear();

        // Generate a new dungeon
        MazeGenerator();

        yield return StartCoroutine(FadeIn()); // Fade back in
    }

    private IEnumerator FadeIn()
    {
        fadeCanvasGroup.alpha = 1;
        while (fadeCanvasGroup.alpha > 0)
        {
            fadeCanvasGroup.alpha -= Time.deltaTime / fadeDuration;
            yield return null;
        }
    }

    private IEnumerator FadeOut()
    {
        fadeCanvasGroup.alpha = 0;
        while (fadeCanvasGroup.alpha < 1)
        {
            fadeCanvasGroup.alpha += Time.deltaTime / fadeDuration;
            yield return null;
        }
    }
}