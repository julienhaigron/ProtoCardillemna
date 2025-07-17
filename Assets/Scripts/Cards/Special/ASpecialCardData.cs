using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ASpecialCardData : ActionCardData
{
    public override ActionCardType GetCardType ()
	{
		return ActionCardType.Special;
	}
}
