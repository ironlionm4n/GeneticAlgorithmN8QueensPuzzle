using Unity.Mathematics;
using UnityEngine;

public class GenerateBoard : MonoBehaviour
{
    [SerializeField] private GameObject whiteSquare;
    [SerializeField] private GameObject blackSquare;
    public static GameObject[,] boardState = new GameObject[8,8];

private void Awake()
    {
        RenderBoard();
    }

private void RenderBoard()
    {
        var counter = 0;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if ((i + j) % 2 == 0)
                {
                    var whiteSqr = InstantiateSquare(whiteSquare, i, j);
                    whiteSqr.name = "whiteSquare " + counter;
                    GeneticAlgorithm.squares.Add(whiteSqr);
                    boardState[i, j] = whiteSqr;
                }
                else
                {
                    var blackSqr = InstantiateSquare(blackSquare, i, j);
                    blackSqr.name = "blackSquare " + counter;
                    GeneticAlgorithm.squares.Add(blackSqr);
                    boardState[i, j] = blackSqr;
                }
                counter++;
            }
        }
    }

    private GameObject InstantiateSquare(GameObject o, int j, int i)
    {
        return Instantiate(o, new Vector3(j, -i, 0), quaternion.identity);
    }
}
