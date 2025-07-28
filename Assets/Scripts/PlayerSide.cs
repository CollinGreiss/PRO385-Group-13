using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerSide
{

    public int health = 20;
    public int power = 0;
    public PlayerArea[] areas;

}


public class PlayerArea
{

    public List<Creature> creatures = new List<Creature>();
    private Card.Area AreaType = Card.Area.empty;

    public Card.Area GetAreaType()
    {
        return AreaType;
    }

    public void SetAreaType(Card.Area areaType)
    {
        AreaType = areaType;
        foreach (var creature in creatures) creature.currentAreaType = areaType;
    }

}