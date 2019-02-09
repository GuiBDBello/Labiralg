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

    public GameObject wall;
    public float wallLength;
    // Colunas
    public int xSize;
    // Altura
    public int ySize;
    // Linhas
    public int zSize;

    private GameObject WallHolder { get; set; }
    private Vector3 InitialPosition { get; set; }
    private Cell[] Cells { get; set; }
    private List<int> LastCells { get; set; }
    private int VisitedCells { get; set; }
    private int TotalCells { get; set; } = 0;
    private int CurrentCell { get; set; } = 0;
    // TODO: Verificar a necessidade da variável totalCells (igual à cells)
    private int CurrentNeighbour { get; set; } = 0;
    private int WallToBreak { get; set; } = 0;
    private int BackingUp { get; set; } = 0;
    private bool StartedBuilding { get; set; } = false;

    // Start is called before the first frame update
    void Start() {
        //wall.transform.localScale += new Vector3(0, 0, wallLength - wall.transform.localScale.z);
        this.CreateWalls();

        // Após terminar a criação dos muros, cria-se as células
        this.CreateCells();

        this.CreateMaze();
    }

    // Update is called once per frame
    void Update() { }

    void CreateWalls() {
        this.WallHolder = new GameObject();
        this.WallHolder.name = "Maze";
        // Posição do canto esquerdo inferior da tela
        this.InitialPosition = new Vector3((-xSize / 2) + (wallLength / 2), 0.0f, (-zSize / 2) + (wallLength / 2));
        Vector3 myPosition = this.InitialPosition;
        // TODO: Remover variável temporária, má prática
        GameObject tempWall;

        // Colunas
        for (int i = 0; i < zSize; i++)
        {
            // Maior ou igual (<=) é necessário pois retorna uma parede a mais, a última coluna
            for (int j = 0; j <= xSize; j++)
            {
                myPosition = new Vector3(this.InitialPosition.x + (j * wallLength) - (wallLength / 2), 0.0f,
                    this.InitialPosition.z + (i * wallLength) - (wallLength / 2));
                tempWall = Instantiate(wall, myPosition, Quaternion.identity) as GameObject;
                tempWall.transform.parent = this.WallHolder.transform;
            }
        }

        // Linhas
        // Maior ou igual (<=) é necessário pois retorna uma parede a mais, a última linha
        for (int i = 0; i <= zSize; i++)
        {
            for (int j = 0; j < xSize; j++)
            {
                myPosition = new Vector3(this.InitialPosition.x + (j * wallLength), 0.0f,
                    this.InitialPosition.z + (i * wallLength) - wallLength);
                tempWall = Instantiate(wall, myPosition, Quaternion.Euler(0.0f, 90.0f, 0.0f)) as GameObject;
                tempWall.transform.parent = this.WallHolder.transform;
            }
        }
    }

    void CreateCells () {
        this.LastCells = new List<int>();
        this.LastCells.Clear();
        this.TotalCells = xSize * zSize;
        int children = this.WallHolder.transform.childCount;
        GameObject[] allWalls = new GameObject[children];
        this.Cells = new Cell[xSize * zSize];
        int westEastProcess = 0;
        int childProcess = 0;
        int termCount = 0;

        // Retorna todos os filhos
        for (int i = 0; i < children; i++)
        {
            allWalls[i] = this.WallHolder.transform.GetChild(i).gameObject;
        }

        // Vincula paredes às células
        for (int cellprocess = 0; cellprocess < this.Cells.Length; cellprocess++)
        {
            // Verifica se é a última célula da linha
            if (termCount == xSize)
            {
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
        //if (visitedCells < totalCells)
        while (this.VisitedCells < this.TotalCells) {
            if (this.StartedBuilding) {
                this.GiveMeNeighbour();
                // Se a célula vizinha não foi visitada, e a célula atual já foi, quebra o muro entre elas
                if (this.Cells[this.CurrentNeighbour].visited == false && this.Cells[this.CurrentCell].visited == true)
                {
                    this.BreakWall();
                    this.Cells[this.CurrentNeighbour].visited = true;
                    this.VisitedCells++;
                    this.LastCells.Add(this.CurrentCell);
                    this.CurrentCell = this.CurrentNeighbour;
                    // Restaura o valor de retorno se não encontra nenhum vizinho
                    if (this.LastCells.Count > 0)
                    {
                        this.BackingUp = this.LastCells.Count - 1;
                    }
                }
            } else
            {
                // Escolhe uma célula aleatória para iniciar a construção do Labirinto
                this.CurrentCell = Random.Range(0, this.TotalCells);
                this.Cells[this.CurrentCell].visited = true;
                this.VisitedCells++;
                this.StartedBuilding = true;
            }

            // Chamada recursiva
            //Invoke("CreateMaze", 0.0f);
            Debug.Log("Finished");
        }
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
        if (this.CurrentCell - xSize >= 0)
        {
            if (this.Cells[this.CurrentCell - xSize].visited == false)
            {
                neighbours[length] = this.CurrentCell - xSize;
                connectingWall[length] = 1;
                length++;
            }
        }
        // Oeste
        if (this.CurrentCell - 1 >= 0 && this.CurrentCell != check)
        {
            if (this.Cells[this.CurrentCell - 1].visited == false)
            {
                neighbours[length] = this.CurrentCell - 1;
                connectingWall[length] = 2;
                length++;
            }
        }
        // Leste
        if (this.CurrentCell + 1 < this.TotalCells && (this.CurrentCell + 1 ) != check)
        {
            if (this.Cells[this.CurrentCell + 1].visited == false)
            {
                neighbours[length] = this.CurrentCell + 1;
                connectingWall[length] = 3;
                length++;
            } 
        }
        // Norte
        if (this.CurrentCell + xSize < this.TotalCells)
        {
            if (this.Cells[this.CurrentCell + xSize].visited == false)
            {
                neighbours[length] = this.CurrentCell + xSize;
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