using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DungeonGenerator : MonoBehaviour
{
    public class Cell{
        public bool visited = false;
        public bool[] status = new bool[4];

    }

    public Vector2 size;
    public int startPos = 0;
    public GameObject room;
    public Vector2 offset; // this is the distance btwn each room
    public Vector3 SpawnPosition;

    List<Cell> board;

    void Start(){
        MazeGenerator();
    }

    void Update(){}

    void GenerateDungeon(){
        for (int i = 0; i < size.x; i++){
            for (int j = 0; j < size.y; j++){
                Cell currentCell = board[Mathf.FloorToInt(i + j * size.x)];
                if (currentCell.visited){
                    var newRoom = Instantiate(room, new Vector3(i*offset.x, 0, -j*offset.y), Quaternion.identity, transform).GetComponent<RoomBehaviour>();
                    newRoom.UpdateRoom(currentCell.status);
                
                    newRoom.name += " " + i + "-" + j;
                }
            }
        }
    }

    void setSpawnPosition()
    {
        SpawnPosition = new Vector3(0, 0, 0);
    }

    void MazeGenerator(){
        board = new List<Cell>();

        for (int i = 0; i < size.x; i++){
            for (int j = 0; j < size.y; j++){
                board.Add(new Cell());
            }
        }

        int currentCell = startPos;

        Stack<int> path = new Stack<int>();

        int  k = 0;

        while (k < 1000){
            k++;

            board[currentCell].visited = true;

            if (currentCell == board.Count - 1){
                break;
            }

            //check the cell's neighbors
            List<int> neighbors = CheckNeighbors(currentCell);

            if (neighbors.Count == 0){
                if(path.Count == 0){
                    break;
                }
                else{
                    currentCell = path.Pop();
                }
            }
            else{
                path.Push(currentCell);

                int newCell = neighbors[Random.Range(0, neighbors.Count)];

                //which way is the new cell going
                if (newCell > currentCell){
                    //down or right
                    if (newCell - 1 == currentCell){
                        //going right
                        board[currentCell].status[2] = true;
                        currentCell = newCell;
                        board[currentCell].status[3] = true;
                    }
                    else{
                        //going down
                        //going right
                        board[currentCell].status[1] = true;
                        currentCell = newCell;
                        board[currentCell].status[0] = true;
                    }
                }
                else{
                    //up or left
                    if (newCell + 1 == currentCell){
                        //going up
                        board[currentCell].status[3] = true;
                        currentCell = newCell;
                        board[currentCell].status[2] = true;
                    }
                    else{
                        //going left
                        board[currentCell].status[0] = true;
                        currentCell = newCell;
                        board[currentCell].status[1] = true;
                    }
                }
            }
        }
        GenerateDungeon();
    }  

    List<int>  CheckNeighbors(int cell){
        List<int> neighbors = new List<int>();
        
        //check the up neighbor
        if ((cell - size.x >= 0) && !board[Mathf.FloorToInt(cell - size.x)].visited){
            neighbors.Add(Mathf.FloorToInt(cell - size.x));
        }

        //check the down neighbor
        if ((cell + size.x < board.Count) && !board[Mathf.FloorToInt(cell + size.x)].visited){
            neighbors.Add(Mathf.FloorToInt(cell + size.x));
        }

        //check the right neighbor
        if ((cell + 1) % size.x != 0 && !board[Mathf.FloorToInt(cell + 1)].visited){
            neighbors.Add(Mathf.FloorToInt(cell + 1));
        }

        //check the left neighbor
        if (cell % size.x != 0 && !board[Mathf.FloorToInt(cell - 1)].visited){
            neighbors.Add(Mathf.FloorToInt(cell - 1));
        }

        return neighbors;
    }
}
