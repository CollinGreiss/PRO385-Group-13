using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerSide
{

    public int health = 20;
    public int power = 0;
    public PlayerArea[] areas = new PlayerArea[4];

}


public class PlayerArea
{

    public List<Creature> creatures = new List<Creature>();
    private CardArea AreaType = CardArea.empty;

    public CardArea GetAreaType()
    {
        return AreaType;
    }

    public void SetAreaType(CardArea areaType)
    {
        AreaType = areaType;
        foreach (var creature in creatures) creature.currentAreaType = areaType;
    }

}