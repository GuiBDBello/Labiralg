using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour {
    [System.Serializable]
    public class Cell {
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
    public float wallThickness; // X
    public float wallHeight;    // Y
    public float wallLength;    // Z
    public int xSize;   // Colunas
    public int zSize;   // Linhas
    public bool generateInstantly;
    public float generationSpeed;

    private GameObject WallHolder { get; set; }
    private GameObject PickUpHolder { get; set; }
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

    // Start é chamado antes do primeiro frame
    void Start() {
        // Inicializa variáveis e propriedades
        this.Init();

        // Cria um grid com os muros
        this.CreateWalls();

        // Após terminar a criação dos muros, cria-se as células
        this.CreateCells();
        
        // Após criar os muros, gera o Labirinto
        if (generateInstantly) {
            this.InstantlyCreateMaze();
        } else {
            this.SlowlyCreateMaze();
        }

        if (this.SpawnPickUp != null) {
            StopCoroutine(this.SpawnPickUp);
        }
        this.SpawnPickUp = StartCoroutine(SpawnPickUpInGameArea(pickUpSpawnTime));
    }

    // Update é chamado uma vez por frame
    void Update() { }

    void Init() {
        Destroy(PickUpHolder);

        this.VisitedCells = 0;
        this.CurrentCell = 0;
        this.CurrentNeighbour = 0;
        this.WallToBreak = 0;
        this.BackingUp = 0;
        this.StartedBuilding = false;

        // Labirinto
        this.WallHolder = new GameObject();
        this.WallHolder.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        this.WallHolder.name = "Maze";

        // Pick Ups
        this.PickUpHolder = new GameObject();
        this.PickUpHolder.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        this.PickUpHolder.name = "Pick Ups";
    }

    public IEnumerator SpawnPickUpInGameArea(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        GameObject tempPickUp;
        Vector3 spawnArea = new Vector3(Random.Range((-xSize * wallLength) / 2, (xSize * wallLength) / 2), 1.0f, Random.Range((-zSize * wallLength) / 2, (zSize * wallLength) / 2));
        tempPickUp = Instantiate(pickUp, spawnArea, Quaternion.identity) as GameObject;
        tempPickUp.transform.parent = this.PickUpHolder.transform;
        // Chamada recursiva...
        this.SpawnPickUp = StartCoroutine(SpawnPickUpInGameArea(pickUpSpawnTime));
    }

    void CreateWalls() {
        // Posição do canto esquerdo inferior da tela
        //this.InitialPosition = new Vector3(0.0f, 0.0f, 0.0f);
        // Posição centralizada
        this.InitialPosition = new Vector3((-xSize * wallLength / 2) + (wallLength / 2), 0.0f, (-zSize * wallLength / 2) + (wallLength));
        Vector3 myPosition = this.InitialPosition;
        // TODO: Remover variável temporária, má prática
        GameObject tempWall;
        GameObject tempColumn;
        GameObject tempGround;

        // Colunas (muros)
        for (int i = 0; i < zSize; i++) {
            // Maior ou igual (<=) é necessário pois retorna um muro a mais, a última coluna
            for (int j = 0; j <= xSize; j++) {
                myPosition = new Vector3(this.InitialPosition.x + (j * wallLength) - (wallLength / 2), wallHeight / 2, this.InitialPosition.z + (i * wallLength) - (wallLength / 2));
                tempWall = Instantiate(wall, myPosition, Quaternion.identity) as GameObject;
                tempWall.transform.parent = this.WallHolder.transform;
                // Tamanho do muro
                wall.transform.localScale = new Vector3(wallThickness, wallHeight, wallLength - wallThickness);
            }
        }

        // Linhas (muros)
        // Maior ou igual (<=) é necessário pois retorna um muro a mais, a última linha
        for (int i = 0; i <= zSize; i++) {
            for (int j = 0; j < xSize; j++) {
                myPosition = new Vector3(this.InitialPosition.x + (j * wallLength), wallHeight / 2, this.InitialPosition.z + (i * wallLength) - wallLength);
                tempWall = Instantiate(wall, myPosition, Quaternion.Euler(0.0f, 90.0f, 0.0f)) as GameObject;
                tempWall.transform.parent = this.WallHolder.transform;

                // Tamanho do muro
                wall.transform.localScale = new Vector3(wallThickness, wallHeight, wallLength - wallThickness);
            }
        }

        // Colunas
        for (int i = 0; i <= zSize; i++) {
            for (int j = 0; j <= xSize; j++) {
                myPosition = new Vector3(this.InitialPosition.x + (j * wallLength) - (wallLength / 2), wallHeight / 2, this.InitialPosition.z + (i * wallLength) - (wallLength));
                tempColumn = Instantiate(column, myPosition, Quaternion.identity) as GameObject;
                tempColumn.transform.parent = this.WallHolder.transform;
                // Tamanho das colunas
                column.transform.localScale = new Vector3(wallThickness, wallHeight, wallThickness);
            }
        }
        //ground.transform.localScale = new Vector3(xSize / 2, 1.0f, zSize / 2);
        //ground.transform.position = new Vector3(0.0f, 0.0f, 0.0f);

        // Tamanho do chão
        ground.transform.localScale = new Vector3(((wallLength / 10) * xSize) + (wallThickness / 10), 1.0f, ((wallLength / 10) * zSize) + (wallThickness / 10));

        myPosition = new Vector3(this.InitialPosition.x + ((xSize * wallLength) / 2) - (wallLength / 2), 0.0f, this.InitialPosition.z + ((zSize * wallLength) / 2) - wallLength);
        tempGround = Instantiate(ground, myPosition, Quaternion.Euler(0.0f, 0.0f, 0.0f)) as GameObject;
        tempGround.transform.parent = this.WallHolder.transform;
    }

    void CreateCells () {
        this.Cells = new Cell[xSize * zSize];
        this.LastCells = new List<int>();
        this.LastCells.Clear();

        int children = this.WallHolder.transform.childCount;
        int westEastProcess = 0;
        int childProcess = 0;
        int termCount = 0;

        GameObject[] allWalls = new GameObject[children];

        // Retorna todos os filhos
        for (int i = 0; i < children; i++) {
            allWalls[i] = this.WallHolder.transform.GetChild(i).gameObject;
        }

        // Vincula muros às células
        for (int cellprocess = 0; cellprocess < this.Cells.Length; cellprocess++) {
            // Verifica se é a última célula da linha
            if (termCount == xSize) {
                // Pula uma célula e zera a "coluna" (pula para a próxima coluna)
                westEastProcess++;
                termCount = 0;
            }

            this.Cells[cellprocess] = new Cell();
            this.Cells[cellprocess].west = allWalls[westEastProcess];
            this.Cells[cellprocess].south = allWalls[childProcess + ((xSize + 1) * zSize)];

            westEastProcess++;
            termCount++;
            childProcess++;

            this.Cells[cellprocess].east = allWalls[westEastProcess];
            this.Cells[cellprocess].north = allWalls[(childProcess + ((xSize + 1) * zSize)) + xSize - 1];
        }
    }

    void CreateMaze() {
        if (this.StartedBuilding) {
            this.GiveMeNeighbour();
            // Se a célula vizinha não foi visitada, e a célula atual já foi, quebra o muro entre elas
            if (this.Cells[this.CurrentNeighbour].visited == false && this.Cells[this.CurrentCell].visited == true) {
                this.BreakWall();
                this.Cells[this.CurrentNeighbour].visited = true;
                this.LastCells.Add(this.CurrentCell);
                this.CurrentCell = this.CurrentNeighbour;
                this.VisitedCells++;
                // Restaura o valor de retorno se não encontra nenhum vizinho
                if (this.LastCells.Count > 0) {
                    this.BackingUp = this.LastCells.Count - 1;
                }
            }
        } else {
            // Escolhe uma célula aleatória para iniciar a construção do Labirinto
            this.CurrentCell = Random.Range(0, this.Cells.Length);
            this.Cells[this.CurrentCell].visited = true;
            this.VisitedCells++;
            this.StartedBuilding = true;
        }
    }

    void InstantlyCreateMaze() {
        while (this.VisitedCells < this.Cells.Length) {
            this.CreateMaze();
        }
    }

    void SlowlyCreateMaze() {
        if (this.VisitedCells < this.Cells.Length) {
            this.CreateMaze();
            // Chamada recursiva
            Invoke("SlowlyCreateMaze", generationSpeed);
        } else {
            StartCoroutine(RecreateMaze(3.0f));
        }
    }

    IEnumerator RecreateMaze(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        Destroy(WallHolder);
        this.Start();
    }

    void GiveMeNeighbour() {
        int length = 0;
        int[] neighbours = new int[4];
        int[] connectingWall = new int[4];
        int check = 0;

        // Verifica se está na última célula da linha
        // TODO: Refatorar para método
        check = (this.CurrentCell + 1) / xSize;
        check -= 1;
        check *= xSize;
        check += xSize;

        // TODO: Refatorar para método
        // Sul
        if (this.CurrentCell - xSize >= 0) {
            if (this.Cells[this.CurrentCell - xSize].visited == false) {
                neighbours[length] = this.CurrentCell - xSize;
                connectingWall[length] = 1;
                length++;
            }
        }
        // Oeste
        if (this.CurrentCell - 1 >= 0 && this.CurrentCell != check) {
            if (this.Cells[this.CurrentCell - 1].visited == false) {
                neighbours[length] = this.CurrentCell - 1;
                connectingWall[length] = 2;
                length++;
            }
        }
        // Leste
        if (this.CurrentCell + 1 < this.Cells.Length && (this.CurrentCell + 1 ) != check) {
            if (this.Cells[this.CurrentCell + 1].visited == false) {
                neighbours[length] = this.CurrentCell + 1;
                connectingWall[length] = 3;
                length++;
            } 
        }
        // Norte
        if (this.CurrentCell + xSize < this.Cells.Length) {
            if (this.Cells[this.CurrentCell + xSize].visited == false) {
                neighbours[length] = this.CurrentCell + xSize;
                connectingWall[length] = 4;
                length++;
            }
        }

        // Busca um vizinho aleatório
        if (length != 0) {
            int theChosenOne = Random.Range(0, length);
            this.CurrentNeighbour = neighbours[theChosenOne];
            this.WallToBreak = connectingWall[theChosenOne];
        } else {
            // Ao encontrar um caminho sem-saída, faz o caminho inverso até encontrar uma saída
            if (this.BackingUp > 0) {
                this.CurrentCell = this.LastCells[this.BackingUp];
                this.BackingUp--;
            }
        }
    }

    void BreakWall() {
        switch (this.WallToBreak) {
            case 1:
                Destroy(this.Cells[this.CurrentCell].south);
                break;
            case 2:
                Destroy(this.Cells[this.CurrentCell].west);
                break;
            case 3:
                Destroy(this.Cells[this.CurrentCell].east);
                break;
            case 4:
                Destroy(this.Cells[this.CurrentCell].north);
                break;
        }
    }
}