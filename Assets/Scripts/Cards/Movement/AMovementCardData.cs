using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AMovementCardData : ActionCardData
{
	public Vector2Int direction;

	public override ActionCardType GetCardType ()
	{
		return ActionCardType.Movement;
	}
}
