using System.Collections.Generic;
using UnityEngine;

public class PhysicalArea : MonoBehaviour
{

    private List<Creature> creatures;
    private CardArea areaType;

    public void SetAreaType(CardArea newAreaType)
    {
        areaType = newAreaType;


    }
    
    public void SetCreatures(List<Creature> newCreatures)
    {
        creatures = newCreatures;
    }

}
