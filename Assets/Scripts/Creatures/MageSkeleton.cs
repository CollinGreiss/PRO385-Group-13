using UnityEngine;

public class MageSkeleton : Creature
{
    public MageSkeleton(Card card) : base(card) { }

    public override void ApplyAreaEffect()
    {
        if (currentArea != null)
        {
            currentArea.SetAreaType(CardArea.forest);
        }
    }
}
