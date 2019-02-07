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
    private Cell[] cells;
    private int currentCell = 0;
    // TODO: Verificar a necessidade da variável totalCells (igual à cells)
    private int totalCells = 0;
    private int visitedCells;
    private bool startedBuilding = false;
    private int currentNeighbour = 0;
    private List<int> lastCells;
    private int backingUp = 0;
    private int wallToBreak = 0;

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
        lastCells = new List<int>();
        lastCells.Clear();
        totalCells = xSize * zSize;
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
            // Verifica se é a última célula da linha
            if (termCount == xSize)
            {
                // Pula uma célula e zera a "coluna" (pula para a próxima coluna)
                westEastProcess++;
                termCount = 0;
            }

            cells[cellprocess] = new Cell();
            cells[cellprocess].west = allWalls[westEastProcess];
            cells[cellprocess].south = allWalls[childProcess + ((xSize + 1) * zSize)];

            westEastProcess++;
            termCount++;
            childProcess++;


            cells[cellprocess].east = allWalls[westEastProcess];
            cells[cellprocess].north = allWalls[(childProcess + ((xSize + 1) * zSize)) + xSize - 1];
        }

        CreateMaze();
    }

    void CreateMaze()
    {
        //if (visitedCells < totalCells)
        while (visitedCells < totalCells)
        {
            if (startedBuilding)
            {
                GiveMeNeighbour();
                // Se a célula vizinha não foi visitada, e a célula atual já foi, quebra o muro entre elas
                if (cells[currentNeighbour].visited == false && cells[currentCell].visited == true)
                {
                    BreakWall();
                    cells[currentNeighbour].visited = true;
                    visitedCells++;
                    lastCells.Add(currentCell);
                    currentCell = currentNeighbour;
                    // Restaura o valor de retorno se não encontra nenhum vizinho
                    if (lastCells.Count > 0)
                    {
                        backingUp = lastCells.Count - 1;
                    }
                }
            } else
            {
                // Escolhe uma célula aleatória para iniciar a construção do Labirinto
                currentCell = Random.Range(0, totalCells);
                cells[currentCell].visited = true;
                visitedCells++;
                startedBuilding = true;
            }

            // Chamada recursiva
            //Invoke("CreateMaze", 0.0f);
            Debug.Log("Finished");
        }
    }

    void BreakWall()
    {
        switch (wallToBreak)
        {
            case 1:
                Destroy(cells[currentCell].south);
                break;
            case 2:
                Destroy(cells[currentCell].west);
                break;
            case 3:
                Destroy(cells[currentCell].east);
                break;
            case 4:
                Destroy(cells[currentCell].north);
                break;
        }
    }

    void GiveMeNeighbour()
    {
        int length = 0;
        int[] neighbours = new int[4];
        int[] connectingWall = new int[4];
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
                connectingWall[length] = 1;
                length++;
            }
        }
        // Oeste
        if (currentCell - 1 >= 0 && currentCell != check)
        {
            if (cells[currentCell - 1].visited == false)
            {
                neighbours[length] = currentCell - 1;
                connectingWall[length] = 2;
                length++;
            }
        }
        // Leste
        if (currentCell + 1 < totalCells && (currentCell + 1 ) != check)
        {
            if (cells[currentCell + 1].visited == false)
            {
                neighbours[length] = currentCell + 1;
                connectingWall[length] = 3;
                length++;
            } 
        }
        // Norte
        if (currentCell + xSize < totalCells)
        {
            if (cells[currentCell + xSize].visited == false)
            {
                neighbours[length] = currentCell + xSize;
                connectingWall[length] = 4;
                length++;
            }
        }
        
        /*
        for (int i = 0; i < length; i++)
        {
            Debug.Log(neighbours[i]);
        }
        */

        // Busca um vizinho aleatório
        if (length != 0)
        {
            int theChosenOne = Random.Range(0, length);
            currentNeighbour = neighbours[theChosenOne];
            wallToBreak = connectingWall[theChosenOne];
        } else
        {
            // Ao encontrar um caminho sem-saída, faz o caminho inverso até encontrar uma saída
            if (backingUp > 0)
            {
                currentCell = lastCells[backingUp];
                backingUp--;
            }
        }
    }
}