using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ARoom : MonoBehaviour
{
    public int x;
    public int y;
    public Vector2Int Position => new Vector2Int(x, y);
    public RoomType roomType;

    protected EntityCard m_entityInCard;

    public virtual void Init ( int x, int y, RoomType roomType )
    {
        this.x = x;
        this.y = y;
        this.roomType = roomType;
    }

    public virtual void OnEnter ( EntityCard entity )
	{
        m_entityInCard = entity;
    }

    public void Spawn(EntityData _entityData )
	{
        EntityCard entityCard = Instantiate(GameManager.Instance.entityCard, transform.position, Quaternion.identity, transform);
        entityCard.Spawn(_entityData, this);
        entityCard.transform.position = transform.position + entityCard.EntityPositionOffset;
        OnEnter(entityCard);
    }

    public virtual bool EnterPredicate ()
	{
        return m_entityInCard == null;
	}
}
