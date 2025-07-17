using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName = "Scriptables/ActionCard")]
public abstract class ActionCardData : ScriptableObject
{
	public string title;
	public string description;
	public Sprite background;

    public enum ActionCardType
	{
		Attack,
		Movement,
		Special
	}

	public abstract ActionCardType GetCardType ();

	public abstract bool CanUsePredicate ( EntityCard _user );

	public abstract void Use ( EntityCard _user, List<EntityCard> _target );

	public virtual int TargetNeededAmount ()
	{
		return 0;
	}

	public virtual bool TargetPredicate( EntityCard _user, EntityCard _target )
	{
		return _user != _target && _user.IsPlayer != _target.IsPlayer;
	}
}
