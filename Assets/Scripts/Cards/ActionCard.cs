using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;

public class ActionCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_titleTMP;
    [SerializeField] private TextMeshProUGUI m_descriptionTMP;
    [SerializeField] private Image m_image;
    [SerializeField] private SerializableDictionary<ActionCardData.ActionCardType, GameObject> m_cardIcons;

    private Tween m_movementTween;
    private Tween m_rotationTween;

    private ActionCardData m_data;
    public ActionCardData Data => m_data;
    private CardStack m_stack;
    public CardStack Stack => m_stack;

    public void Init(ActionCardData _data )
	{
        m_data = _data;
        m_titleTMP.text = _data.title;
        m_descriptionTMP.text = _data.description;
        m_image.sprite = _data.background;
        m_cardIcons[_data.GetCardType()].SetActive(true);
    }

    public void GoToPosition(Vector3 _position, Quaternion _rotation, float _duration )
	{
        if (m_movementTween.IsActive())
            m_movementTween.Kill();
        if (m_rotationTween.IsActive())
            m_rotationTween.Kill();

        m_movementTween = transform.DOMove(_position, _duration);
        m_rotationTween = transform.DORotateQuaternion(_rotation, _duration);
	}

    public void Use (EntityCard _user, List<EntityCard> _target)
	{
        m_data.Use(_user, _target);
    }

    public void SetStack(CardStack _stack )
	{
        m_stack = _stack;
	}

    public bool CanUse ()
	{
        return m_stack.type == CardStack.StackType.Hand;
	}

    public void OnHoverExit ()
	{

	}

    public void OnHoverEnter ()
	{

	}

}
