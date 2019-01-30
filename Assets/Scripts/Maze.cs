using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour
{
    public GameObject wall;
    public float wallLength;
    // Colunas
    public int xSize;
    // Altura
    public int ySize;
    // Linhas
    public int zSize;

    private Vector3 initialPosition;
    private GameObject wallHolder;

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
    }
}