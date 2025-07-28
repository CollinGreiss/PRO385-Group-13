using UnityEngine;

public class Card
{


    public string cardName;
    public string cardDescription;
    public Sprite cardImage;
    public int cardHealth;
    public int cardAttack;
    public int cardCost;
    public CardType cardType;
    public CardArea cardArea;
    public bool activeOnPlay = false;

}



public enum CardType
{

    Creature,
    Spell,
    Landscape

}

public enum CardArea
{

    empty,
    forest,
    lava,
    water,
    desert

}