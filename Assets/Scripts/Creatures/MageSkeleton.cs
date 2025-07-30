using UnityEngine;

public class MageSkeleton : Creature
{
    public MageSkeleton(Card card, int id) : base(card, id) { }

    public override void ApplyAreaEffect()
    {
        if (currentArea != null)
        {
            currentArea.SetAreaType(CardArea.forest);
        }
    }
}
