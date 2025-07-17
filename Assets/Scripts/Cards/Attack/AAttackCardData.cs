using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AAttackCardData : ActionCardData
{
    public override ActionCardType GetCardType ( )
	{
		return ActionCardType.Attack;
	}
}
