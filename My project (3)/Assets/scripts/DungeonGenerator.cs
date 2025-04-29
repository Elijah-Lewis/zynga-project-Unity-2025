using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

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
    public GameObject keyObject;
    public GameObject doorObject;
    public GameObject enemyObject;
    public Vector2 offset;
    public Vector3 SpawnPosition;
    public float roomScale = 1f;

    private List<Cell> board;
    private HashSet<Vector3> occupiedPositions = new HashSet<Vector3>();
    private List<GameObject> spawnedRooms = new List<GameObject>();
    private List<GameObject> currentEnemies = new List<GameObject>();  // Track enemies to delete later
    private GameObject previousKey; // Track previous key
    private GameObject previousDoor; // Track previous door

    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 1f;
    public GameObject player;
    public GameObject doorPrefab;
    public GameObject keyPrefab;
    public GameObject enemyPrefab;
    public Transform[] roomLocations;

    void Start()
    {
        StartCoroutine(FadeIn());
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
                        spawnedRooms.Add(newRoom);

                        if (Random.Range(0f, 1f) <= 0.33f)
                        {
                            SpawnEnemyInRoom(newRoom);
                        }
                    }
                }
            }
        }

        PlaceKeyInRandomRoom();
        PlaceDoorInRandomRoom();
        NavMeshSurface surface = FindFirstObjectByType<NavMeshSurface>();
        if (surface != null)
        {
            surface.BuildNavMesh();
        }
    }

    public void RegenerateDungeon()
    {
        // Destroy all previously spawned objects
        foreach (GameObject room in spawnedRooms)
        {
            Destroy(room);
        }
        spawnedRooms.Clear();
        occupiedPositions.Clear();
        
        // Destroy the previous key and door
        if (previousKey != null)
        {
            Destroy(previousKey);
        }
        if (previousDoor != null)
        {
            Destroy(previousDoor);
        }

        // Destroy previously spawned enemies
        foreach (GameObject enemy in currentEnemies)
        {
            Destroy(enemy);
        }
        currentEnemies.Clear();

        // Re-generate the maze and new dungeon
        board.Clear();
        MazeGenerator();
    }

    private void SpawnEnemyInRoom(GameObject room)
    {
        if (enemyPrefab == null || room == null) return;

        Vector3 spawnPosition = room.transform.position;

        // Add slight random offset so enemies don't stack perfectly
        float offsetX = Random.Range(-1.5f, 1.5f);
        float offsetZ = Random.Range(-1.5f, 1.5f);
        spawnPosition += new Vector3(offsetX, 0, offsetZ);

        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        enemy.transform.parent = room.transform;

        Debug.Log("Spawned enemy in room at " + spawnPosition);
    }

    void PlaceKeyInRandomRoom()
    {
        if (spawnedRooms.Count == 0 || keyObject == null) return;

        int randomIndex = Random.Range(0, spawnedRooms.Count);
        GameObject selectedRoom = spawnedRooms[randomIndex];
        Vector3 keyPosition = selectedRoom.transform.position + Vector3.up * 1.5f;

        previousKey = Instantiate(keyPrefab, keyPosition, Quaternion.identity);  // Store the new key object
    }

    void PlaceDoorInRandomRoom()
    {
        if (spawnedRooms.Count == 0 || doorObject == null) return;

        int randomIndex = Random.Range(0, spawnedRooms.Count);
        GameObject selectedRoom = spawnedRooms[randomIndex];
        Vector3 doorPosition = selectedRoom.transform.position + Vector3.up * 0.5f;

        previousDoor = Instantiate(doorPrefab, doorPosition, Quaternion.identity);  // Store the new door object
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

                if (dx == 1)
                {
                    board[currentCell].status[2] = true;
                    board[newCell].status[3] = true;
                }
                else if (dx == -1)
                {
                    board[currentCell].status[3] = true;
                    board[newCell].status[2] = true;
                }
                else if (dy == 1)
                {
                    board[currentCell].status[1] = true;
                    board[newCell].status[0] = true;
                }
                else if (dy == -1)
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

        if (y > 0 && !board[cell - (int)size.x].visited)
            neighbors.Add(cell - (int)size.x);

        if (y < size.y - 1 && !board[cell + (int)size.x].visited)
            neighbors.Add(cell + (int)size.x);

        if (x < size.x - 1 && !board[cell + 1].visited)
            neighbors.Add(cell + 1);

        if (x > 0 && !board[cell - 1].visited)
            neighbors.Add(cell - 1);

        return neighbors;
    }

    public IEnumerator FadeAndRegenerate()
    {
        yield return StartCoroutine(FadeOut());

        RegenerateDungeon();

        if (spawnedRooms.Count > 0 && player != null)
        {
            GameObject randomRoom = spawnedRooms[Random.Range(0, spawnedRooms.Count)];
            player.transform.position = randomRoom.transform.position + Vector3.up * 1.5f;
        }

        yield return StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        fadeCanvasGroup.alpha = 1;
        fadeCanvasGroup.blocksRaycasts = true;
        fadeCanvasGroup.interactable = true;

        while (fadeCanvasGroup.alpha > 0)
        {
            fadeCanvasGroup.alpha -= Time.deltaTime / fadeDuration;
            yield return null;
        }

        fadeCanvasGroup.alpha = 0;
        fadeCanvasGroup.blocksRaycasts = false;
        fadeCanvasGroup.interactable = false;
        fadeCanvasGroup.gameObject.SetActive(false);
    }

    private IEnumerator FadeOut()
    {
        fadeCanvasGroup.gameObject.SetActive(true);
        fadeCanvasGroup.blocksRaycasts = true;
        fadeCanvasGroup.interactable = true;
        fadeCanvasGroup.alpha = 0;

        while (fadeCanvasGroup.alpha < 1)
        {
            fadeCanvasGroup.alpha += Time.deltaTime / fadeDuration;
            yield return null;
        }

        fadeCanvasGroup.alpha = 1;
    }
}
