using UnityEngine;

public class Board : MonoBehaviour
{
    
    public PhysicalArea[] player1Areas;
    public PhysicalArea[] player2Areas;

    public void UpdateBoard()
    {

        GameManager gameManager = GameManager.Instance;
        if (gameManager == null) return;

        PlayerSide player1 = gameManager.player1Side;
        PlayerSide player2 = gameManager.player2Side;

        for (int i = 0; i < player1.areas.Length; i++)
        {
            player1Areas[i].SetAreaType(player1.areas[i].GetAreaType());
            player1Areas[i].SetCreatures(player1.areas[i].creatures);
        }

        for (int i = 0; i < player2.areas.Length; i++)
        {
            player2Areas[i].SetAreaType(player2.areas[i].GetAreaType());
            player2Areas[i].SetCreatures(player2.areas[i].creatures);
        }

    }

}
