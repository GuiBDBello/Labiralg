using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour
{
    [System.Serializable]
    public class Cell
    {
        public bool visited;
        public GameObject north;
        public GameObject south;
        public GameObject east;
        public GameObject west;
    }

    public GameObject wall;
    public float wallLength;
    // Colunas
    public int xSize;
    // TODO: Altura
    public int ySize;
    // Linhas
    public int zSize;

    private Vector3 initialPosition;
    private GameObject wallHolder;
    // TODO: Tornar variável cells privada
    public Cell[] cells;
    // TODO: Tornar variável currentCell privada
    public int currentCell = 0;
    // TODO: Verificar a necessidade da variável totalCells (igual à cells)
    private int totalCells = 0;

    // Start is called before the first frame update
    void Start()
    {
        //wall.transform.localScale += new Vector3(0, 0, wallLength - wall.transform.localScale.z);
        CreateWalls();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateWalls()
    {
        wallHolder = new GameObject();
        wallHolder.name = "Maze";
        // Posição do canto esquerdo inferior da tela
        initialPosition = new Vector3((-xSize / 2) + (wallLength / 2), 0.0f, (-zSize / 2) + (wallLength / 2));
        Vector3 myPosition = initialPosition;
        // TODO: Remover variável temporária, má prática
        GameObject tempWall;

        // Colunas
        for (int i = 0; i < zSize; i++)
        {
            // Maior ou igual (<=) é necessário pois retorna uma parede a mais, a última coluna
            for (int j = 0; j <= xSize; j++)
            {
                myPosition = new Vector3(initialPosition.x + (j * wallLength) - (wallLength / 2), 0.0f,
                    initialPosition.z + (i * wallLength) - (wallLength / 2));
                tempWall = Instantiate(wall, myPosition, Quaternion.identity) as GameObject;
                tempWall.transform.parent = wallHolder.transform;
            }
        }

        // Linhas
        // Maior ou igual (<=) é necessário pois retorna uma parede a mais, a última linha
        for (int i = 0; i <= zSize; i++)
        {
            for (int j = 0; j < xSize; j++)
            {
                myPosition = new Vector3(initialPosition.x + (j * wallLength), 0.0f,
                    initialPosition.z + (i * wallLength) - wallLength);
                tempWall = Instantiate(wall, myPosition, Quaternion.Euler(0.0f, 90.0f, 0.0f)) as GameObject;
                tempWall.transform.parent = wallHolder.transform;
            }
        }
        // Após terminar a criação dos muros, cria-se as células
        CreateCells();
    }

    void CreateCells ()
    {
        int children = wallHolder.transform.childCount;
        GameObject[] allWalls = new GameObject[children];
        cells = new Cell[xSize * zSize];
        int westEastProcess = 0;
        int childProcess = 0;
        int termCount = 0;

        // Retorna todos os filhos
        for (int i = 0; i < children; i++)
        {
            allWalls[i] = wallHolder.transform.GetChild(i).gameObject;
        }

        // Vincula paredes às células
        for (int cellprocess = 0; cellprocess < cells.Length; cellprocess++)
        {
            cells[cellprocess] = new Cell();
            cells[cellprocess].west = allWalls[westEastProcess];
            cells[cellprocess].south = allWalls[childProcess + ((xSize + 1) * zSize)];

            // Verifica se é a última célula da linha
            if (termCount == xSize)
            {
                // Pula uma célula e zera a "coluna"
                westEastProcess += 2;
                termCount = 0;
            } else
            {
                westEastProcess++;
            }
            termCount++;
            childProcess++;


            cells[cellprocess].east = allWalls[westEastProcess];
            cells[cellprocess].north = allWalls[(childProcess + ((xSize + 1) * zSize)) + xSize - 1];
        }

        CreateMaze();
    }

    void CreateMaze()
    {
        GiveMeNeighbour();
    }

    void GiveMeNeighbour()
    {
        totalCells = xSize * zSize;
        int length = 0;
        int[] neighbours = new int[4];
        int check = 0;

        // Verifica se está na última célula da linha
        // TODO: Refatorar para método
        check = (currentCell + 1) / xSize;
        check -= 1;
        check *= xSize;
        check += xSize;

        // TODO: Refatorar para método
        // Sul
        if (currentCell - xSize >= 0)
        {
            if (cells[currentCell - xSize].visited == false)
            {
                neighbours[length] = currentCell - xSize;
                length++;
            }
        }
        // Oeste
        if (currentCell - 1 >= 0 && currentCell != check)
        {
            if (cells[currentCell - 1].visited == false)
            {
                neighbours[length] = currentCell - 1;
                length++;
            }
        }
        // Leste
        if (currentCell + 1 < totalCells && (currentCell + 1 ) != check)
        {
            if (cells[currentCell + 1].visited == false)
            {
                neighbours[length] = currentCell + 1;
                length++;
            } 
        }
        // Norte
        if (currentCell + xSize < totalCells)
        {
            if (cells[currentCell + xSize].visited == false)
            {
                neighbours[length] = currentCell + xSize;
                length++;
            }
        }

        

        for (int i = 0; i < length; i++)
        {
            Debug.Log(neighbours[i]);
        }
    }
}