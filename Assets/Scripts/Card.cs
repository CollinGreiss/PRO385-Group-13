using UnityEngine;

public class Card
{

    public enum Type
    {

        Creature,
        Spell,
        Landscape

    }

    public enum Area
    {

        empty,
        forest,
        lava,
        water,
        desert

    }


    public string cardName;
    public string cardDescription;
    public Sprite cardImage;
    public int cardHealth;
    public int cardAttack;
    public int cardCost;
    public Type cardType;
    public Area cardArea;
    public bool activeOnPlay = false;

}