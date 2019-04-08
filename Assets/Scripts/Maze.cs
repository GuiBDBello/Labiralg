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

    public GameObject pickUp;
    public float pickUpSpawnTime;
    public GameObject wall;
    public GameObject column;
    public GameObject ground;
    public GameObject portal;
    public float wallThickness; // X
    public float wallHeight;    // Y
    public float wallLength;    // Z
    public int xSize;   // Colunas
    public int zSize;   // Linhas
    public bool generateInstantly;
    public float generationSpeed;

    private Zoom zoom;

    // Propriedades
    private GameObject WallHolder { get; set; }
    private GameObject PickUpHolder { get; set; }
    private GameObject PortalHolder { get; set; }
    private Vector3 InitialPosition { get; set; }
    private Cell[] Cells { get; set; }
    private List<int> LastCells { get; set; }
    private int VisitedCells { get; set; }
    private int CurrentCell { get; set; }
    private int CurrentNeighbour { get; set; }
    private int WallToBreak { get; set; }
    private int BackingUp { get; set; }
    private bool StartedBuilding { get; set; }
    private Coroutine SpawnPickUp { get; set; }

    private void Init()
    {
        //Destroy(WallHolder);
        Destroy(PickUpHolder);
        Destroy(PortalHolder);

        VisitedCells = 0;
        CurrentCell = 0;
        CurrentNeighbour = 0;
        WallToBreak = 0;
        BackingUp = 0;
        StartedBuilding = false;

        // Labirinto
        WallHolder = new GameObject();
        WallHolder.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        WallHolder.name = "Maze";

        // Pick Ups
        PickUpHolder = new GameObject();
        PickUpHolder.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        PickUpHolder.name = "Pick Ups";

        // Portals
        PortalHolder = new GameObject();
        PortalHolder.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        PortalHolder.name = "Portals";

        zoom = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Zoom>();
    }

    // Start é chamado antes do primeiro frame
    private void Start ()
    {
        // Inicializa variáveis e propriedades
        Init();

        CreateWalls();
        CreateCells();

        if (generateInstantly)
        {
            InstantlyCreateMaze();
        }
        else
        {
            SlowlyCreateMaze();
        }

        // Cria uma co-rotina para "Spawnar" os 'Pick Ups' pelo Labirinto
        if (SpawnPickUp != null)
        {
            StopCoroutine(SpawnPickUp);
        }
        SpawnPickUp = StartCoroutine(SpawnPickUpInGameArea(pickUpSpawnTime));

        SpawnPortals();
    }

    private Vector3 GetMazePositionRandom ()
    {
        return new Vector3(
            Random.Range((-xSize * wallLength) / 2, (xSize * wallLength) / 2),  // X
            1.0f,                                                               // Y
            Random.Range((-zSize * wallLength) / 2, (zSize * wallLength) / 2)   // Z
        );
    }

    private Vector3 GetMazePositionBottomLeft ()
    {
        return new Vector3(
            ((-xSize * wallLength) / 2) + (wallLength / 2), // X
            1.0f,                                           // Y
            ((-zSize * wallLength) / 2) + (wallLength / 2)  // Z
        );
    }

    private Vector3 GetMazePositionBottomRight ()
    {
        return new Vector3(
            ((xSize * wallLength) / 2) - (wallLength / 2),  // X
            1.0f,                                           // Y
            ((-zSize * wallLength) / 2) + (wallLength / 2)  // Z
        );
    }

    private Vector3 GetMazePositionUpperLeft ()
    {
        return new Vector3(
            ((-xSize * wallLength) / 2) + (wallLength / 2), // X
            1.0f,                                           // Y
            ((zSize * wallLength) / 2) - (wallLength / 2)   // Z
        );
    }

    private Vector3 GetMazePositionUpperRight ()
    {
        return new Vector3(
            ((xSize * wallLength) / 2) - (wallLength / 2),  // X
            1.0f,                                           // Y
            ((zSize * wallLength) / 2) - (wallLength / 2)   // Z
        );
    }

    // Cria um grid com os muros
    private void CreateWalls ()
    {
        // Posição do canto esquerdo inferior da tela
        //this.InitialPosition = new Vector3(0.0f, 0.0f, 0.0f);
        // Posição centralizada
        InitialPosition = new Vector3((-xSize * wallLength / 2) + (wallLength / 2), 0.0f, (-zSize * wallLength / 2) + (wallLength));
        Vector3 myPosition = InitialPosition;
        // TODO: Remover variável temporária, má prática
        GameObject tempWall;
        GameObject tempColumn;
        GameObject tempGround;

        // Colunas (muros)
        for (int i = 0; i < zSize; i++)
        {
            // Maior ou igual (<=) é necessário pois retorna um muro a mais, a última coluna
            for (int j = 0; j <= xSize; j++)
            {
                myPosition = new Vector3(InitialPosition.x + (j * wallLength) - (wallLength / 2), wallHeight / 2, InitialPosition.z + (i * wallLength) - (wallLength / 2));
                tempWall = Instantiate(wall, myPosition, Quaternion.identity) as GameObject;
                tempWall.transform.parent = WallHolder.transform;
                // Tamanho do muro
                wall.transform.localScale = new Vector3(wallThickness, wallHeight, wallLength - wallThickness);
            }
        }

        // Linhas (muros)
        // Maior ou igual (<=) é necessário pois retorna um muro a mais, a última linha
        for (int i = 0; i <= zSize; i++)
        {
            for (int j = 0; j < xSize; j++)
            {
                myPosition = new Vector3(InitialPosition.x + (j * wallLength), wallHeight / 2, InitialPosition.z + (i * wallLength) - wallLength);
                tempWall = Instantiate(wall, myPosition, Quaternion.Euler(0.0f, 90.0f, 0.0f)) as GameObject;
                tempWall.transform.parent = WallHolder.transform;

                // Tamanho do muro
                wall.transform.localScale = new Vector3(wallThickness, wallHeight, wallLength - wallThickness);
            }
        }

        // Colunas (pilares)
        for (int i = 0; i <= zSize; i++)
        {
            for (int j = 0; j <= xSize; j++)
            {
                myPosition = new Vector3(InitialPosition.x + (j * wallLength) - (wallLength / 2), wallHeight / 2, InitialPosition.z + (i * wallLength) - (wallLength));
                tempColumn = Instantiate(column, myPosition, Quaternion.identity) as GameObject;
                tempColumn.transform.parent = WallHolder.transform;
                // Tamanho das colunas
                column.transform.localScale = new Vector3(wallThickness, wallHeight, wallThickness);
            }
        }
        //ground.transform.localScale = new Vector3(xSize / 2, 1.0f, zSize / 2);
        //ground.transform.position = new Vector3(0.0f, 0.0f, 0.0f);

        // Tamanho do chão
        ground.transform.localScale = new Vector3(((wallLength / 10) * xSize) + (wallThickness / 10), 1.0f, ((wallLength / 10) * zSize) + (wallThickness / 10));

        myPosition = new Vector3(InitialPosition.x + ((xSize * wallLength) / 2) - (wallLength / 2), 0.0f, InitialPosition.z + ((zSize * wallLength) / 2) - wallLength);
        tempGround = Instantiate(ground, myPosition, Quaternion.Euler(0.0f, 0.0f, 0.0f)) as GameObject;
        tempGround.transform.parent = WallHolder.transform;
    }
    
        // Após terminar a criação dos muros, cria-se as células
    private void CreateCells ()
    {
        Cells = new Cell[xSize * zSize];
        LastCells = new List<int>();
        LastCells.Clear();

        int children = WallHolder.transform.childCount;
        int westEastProcess = 0;
        int childProcess = 0;
        int termCount = 0;

        GameObject[] allWalls = new GameObject[children];

        // Retorna todos os filhos
        for (int i = 0; i < children; i++)
        {
            allWalls[i] = WallHolder.transform.GetChild(i).gameObject;
        }

        // Vincula muros às células
        for (int cellprocess = 0; cellprocess < Cells.Length; cellprocess++)
        {
            // Verifica se é a última célula da linha
            if (termCount == xSize)
            {
                // Pula uma célula e zera a "coluna" (pula para a próxima coluna)
                westEastProcess++;
                termCount = 0;
            }

            Cells[cellprocess] = new Cell();
            Cells[cellprocess].west = allWalls[westEastProcess];
            Cells[cellprocess].south = allWalls[childProcess + ((xSize + 1) * zSize)];

            westEastProcess++;
            termCount++;
            childProcess++;

            Cells[cellprocess].east = allWalls[westEastProcess];
            Cells[cellprocess].north = allWalls[(childProcess + ((xSize + 1) * zSize)) + xSize - 1];
        }
    }

    // Após criar as células, gera o Labirinto
    private void CreateMaze ()
    {
        if (StartedBuilding)
        {
            GiveMeNeighbour();
            // Se a célula vizinha não foi visitada, e a célula atual já foi, quebra o muro entre elas
            if (Cells[CurrentNeighbour].visited == false && Cells[CurrentCell].visited == true)
            {
                BreakWall();
                Cells[CurrentNeighbour].visited = true;
                LastCells.Add(CurrentCell);
                CurrentCell = CurrentNeighbour;
                VisitedCells++;
                // Restaura o valor de retorno se não encontra nenhum vizinho
                if (LastCells.Count > 0)
                {
                    BackingUp = LastCells.Count - 1;
                }
            }
        }
        else
        {
            // Escolhe uma célula aleatória para iniciar a construção do Labirinto
            CurrentCell = Random.Range(0, Cells.Length);
            Cells[CurrentCell].visited = true;
            VisitedCells++;
            StartedBuilding = true;
        }
    }

    private void InstantlyCreateMaze ()
    {
        while (VisitedCells < Cells.Length)
        {
            CreateMaze ();
        }
    }

    private void SlowlyCreateMaze ()
    {
        if (VisitedCells < Cells.Length)
        {
            CreateMaze ();
            // Chamada recursiva
            Invoke("SlowlyCreateMaze", generationSpeed);
        }
        else
        {
            StartCoroutine (RecreateMaze (3.0f));
        }
    }

    // Busca as células vizinhas da célula atual
    private void GiveMeNeighbour ()
    {
        int length = 0;
        int[] neighbours = new int[4];
        int[] connectingWall = new int[4];
        int check = 0;

        // Verifica se está na última célula da linha
        // TODO: Refatorar para método
        check = (CurrentCell + 1) / xSize;
        check -= 1;
        check *= xSize;
        check += xSize;

        // TODO: Refatorar para método
        // Sul
        if (CurrentCell - xSize >= 0)
        {
            if (Cells[CurrentCell - xSize].visited == false)
            {
                neighbours[length] = CurrentCell - xSize;
                connectingWall[length] = 1;
                length++;
            }
        }
        // Oeste
        if (CurrentCell - 1 >= 0 && CurrentCell != check)
        {
            if (Cells[CurrentCell - 1].visited == false)
            {
                neighbours[length] = CurrentCell - 1;
                connectingWall[length] = 2;
                length++;
            }
        }
        // Leste
        if (CurrentCell + 1 < Cells.Length && (CurrentCell + 1) != check)
        {
            if (Cells[CurrentCell + 1].visited == false)
            {
                neighbours[length] = CurrentCell + 1;
                connectingWall[length] = 3;
                length++;
            }
        }
        // Norte
        if (CurrentCell + xSize < Cells.Length)
        {
            if (Cells[CurrentCell + xSize].visited == false)
            {
                neighbours[length] = CurrentCell + xSize;
                connectingWall[length] = 4;
                length++;
            }
        }

        // Busca um vizinho aleatório
        if (length != 0)
        {
            int theChosenOne = Random.Range(0, length);
            CurrentNeighbour = neighbours[theChosenOne];
            WallToBreak = connectingWall[theChosenOne];
        }
        else
        {
            // Ao encontrar um caminho sem-saída, faz o caminho inverso até encontrar uma saída
            if (BackingUp > 0)
            {
                CurrentCell = LastCells[BackingUp];
                BackingUp--;
            }
        }
    }

    private void BreakWall ()
    {
        switch (WallToBreak)
        {
            case 1:
                Destroy (Cells[CurrentCell].south);
                break;
            case 2:
                Destroy (Cells[CurrentCell].west);
                break;
            case 3:
                Destroy (Cells[CurrentCell].east);
                break;
            case 4:
                Destroy (Cells[CurrentCell].north);
                break;
        }
    }

    // TODO: Otimizar esse método
    private void SpawnPortals ()
    {
        GameObject tempPortal;

        tempPortal = Instantiate (portal, GetMazePositionBottomLeft(), Quaternion.identity) as GameObject;
        tempPortal.transform.parent = PortalHolder.transform;
        
        tempPortal = Instantiate (portal, GetMazePositionBottomRight(), Quaternion.identity) as GameObject;
        tempPortal.transform.parent = PortalHolder.transform;
        
        tempPortal = Instantiate (portal, GetMazePositionUpperLeft(), Quaternion.identity) as GameObject;
        tempPortal.transform.parent = PortalHolder.transform;

        tempPortal = Instantiate (portal, GetMazePositionUpperRight(), Quaternion.identity) as GameObject;
        tempPortal.transform.parent = PortalHolder.transform;
    }

    private IEnumerator SpawnPickUpInGameArea(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        GameObject tempPickUp;
        Vector3 spawnArea = GetMazePositionRandom();
        tempPickUp = Instantiate(pickUp, spawnArea, Quaternion.identity) as GameObject;
        tempPickUp.transform.parent = PickUpHolder.transform;
        // Chamada recursiva...
        SpawnPickUp = StartCoroutine(SpawnPickUpInGameArea(pickUpSpawnTime));
    }

    public IEnumerator RecreateMaze(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Destroy(WallHolder);
        Start();
    }

    public void NextMaze ()
    {
        xSize++;
        zSize++;

        zoom.ChangeZoom();

        //GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().

        StartCoroutine(RecreateMaze(0.1f));

        zoom.ChangeZoom();
    }
}