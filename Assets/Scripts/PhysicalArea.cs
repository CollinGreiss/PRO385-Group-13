using System.Collections.Generic;
using UnityEngine;

public class PhysicalArea : MonoBehaviour
{

    private List<Creature> creatures;
    private Card.Area areaType;

    public void SetAreaType(Card.Area newAreaType)
    {
        areaType = newAreaType;


    }
    
    public void SetCreatures(List<Creature> newCreatures)
    {
        creatures = newCreatures;
    }

}
