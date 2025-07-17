using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptables/Cards/Movement/Simple")]
public class SimpleMovementCardData : AMovementCardData
{
	public override void Use (EntityCard _user, List<EntityCard> _target)
	{
		ARoom targetRoom = GetRoomInDirection(_user);
		_user.MoveTo(targetRoom);
	}

	public override bool CanUsePredicate (EntityCard _user)
	{
		return GetRoomInDirection(_user) != null;
	}

	public ARoom GetRoomInDirection (EntityCard _user)
	{
		if (_user == null || _user.CurrentRoom == null)
			return null;

		Vector2Int finalPosition = _user.CurrentRoom.Position + direction;
		ARoom target = GameManager.Instance.MapManager.GetRoom(finalPosition.x, finalPosition.y);

		return target;
	}

	public override int TargetNeededAmount ()
	{
		return 0;
	}
}
