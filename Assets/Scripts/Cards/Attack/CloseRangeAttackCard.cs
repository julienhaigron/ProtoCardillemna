using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Scriptables/Cards/Attack/CloseRange")]
public class CloseRangeAttackCard : AAttackCardData
{
	public int damageAmount = 1;

	public override int TargetNeededAmount ()
	{
		return 1;
	}

	public override bool TargetPredicate ( EntityCard _user, EntityCard _target )
	{
		return GameManager.Instance.MapManager.GetDistance(_user.CurrentRoom, _target.CurrentRoom) == 1;
	}

	public override void Use ( EntityCard _user, List<EntityCard> _targets )
	{
		foreach(EntityCard target in _targets)
			target.TakeDamage(damageAmount);
	}

	public override bool CanUsePredicate ( EntityCard _user )
	{
		return true;
	}

}
