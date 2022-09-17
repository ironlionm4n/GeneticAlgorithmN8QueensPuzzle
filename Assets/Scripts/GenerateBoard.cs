using Unity.Mathematics;
using UnityEngine;

public class GenerateBoard : MonoBehaviour
{
    [SerializeField] private GameObject whiteSquare;
    [SerializeField] private GameObject blackSquare;
    
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
                    var whiteSqr = InstantiateSquare(whiteSquare, j, i);
                    whiteSqr.name = "whiteSquare " + counter;
                    GeneticAlgorithm.squares.Add(whiteSqr);
                }
                else
                {
                    var blackSqr = InstantiateSquare(blackSquare, j, i);
                    blackSqr.name = "blackSquare " + counter;
                    GeneticAlgorithm.squares.Add(blackSqr);
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
