using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class AUIWindow : MonoBehaviour
{
	public Action onWindowOpened; //for you to use freely
	public Action onWindowClosed; //for you to use freely
	public Action onCanvasEnabled; 
	public Action onCanvasDisabled;

	protected WaitForSecondsRealtime m_hideDurationWFS;
	protected WaitForSecondsRealtime m_showDurationWFS;
	protected Coroutine m_showCoroutine;
	protected Coroutine m_hideCoroutine;

	[SerializeField] protected float m_modalFadeDuration = 0.2f;
	[SerializeField] protected GameObject m_blockClickGO;
	[SerializeField] protected Canvas m_canvas;
	public Canvas WindowCanvas => m_canvas;
	public CanvasGroup m_modalCanvasGroup;

	protected Tweener m_modalTween;
	protected Vector2 m_modalAlphaOffOn = new(0, 1);

	public virtual bool CanClick
	{
		get
		{
			return !m_blockClickGO.activeSelf;
		}
		set
		{
			m_blockClickGO.SetActive(!value);
			//m_canvasGroup.interactable = value; we want to keep the modal interactable so we use a BlockClick Object instead
			//m_canvasGroup.blocksRaycasts = value;
		}
	}


	public virtual void SetCanvasEnable ( bool _enable, bool _force = false )
	{
		if (m_canvas.enabled == _enable && !_force) return;

		m_canvas.enabled = _enable;

		if (_enable)
		{
			//only then Launch Awake 
			if (!gameObject.activeSelf)
				gameObject.SetActive(true);

			OnEnableCanvas();
		}
		else
		{
			OnDisableCanvas();
		}
	}

	protected virtual void OnEnableCanvas ()
	{
		onCanvasEnabled?.Invoke();
	}

	protected virtual void OnDisableCanvas ()
	{
		onCanvasDisabled?.Invoke();
	}

	public void OnLoad ()
	{

	}

	public void ShowWindow ( float _delay, bool _instant )
	{
		if (!UIManager.Instance.oppenedPanels.Contains(this))
			UIManager.Instance.oppenedPanels.Add(this);

		if (m_showCoroutine != null)
			StopCoroutine(m_showCoroutine);
		if (m_hideCoroutine != null)
			StopCoroutine(m_hideCoroutine);

		if (gameObject.activeSelf)
		{
			m_showCoroutine = StartCoroutine(ShowCR(_delay, _instant));
		}
	}

	public virtual void Close ( float _delay = 0f, bool _instant = false )
	{
		HideWindow(_delay, _instant);
	}

	public void HideWindow ( float _delay, bool _instant )
	{
		if (UIManager.Instance.oppenedPanels.Contains(this))
			UIManager.Instance.oppenedPanels.Remove(this);

		if (m_showCoroutine != null)
			StopCoroutine(m_showCoroutine);
		if (m_hideCoroutine != null)
			StopCoroutine(m_hideCoroutine);

		if (gameObject.activeSelf)
			m_hideCoroutine = StartCoroutine(HideCR(_delay, _instant));
	}

	protected virtual IEnumerator ShowCR ( float _delay, bool _instant )
	{
		CanClick = false;

		if (_delay != 0f)
			yield return new WaitForSecondsRealtime(_delay);

		OnShowStarted();
		SetModalVisible(true, _instant ? 0f : m_modalFadeDuration);
		//SetContainersVisible(true, _instant ? 0f : (m_overrideDurations ? m_showDuration : null));

		if (!_instant)
			yield return m_showDurationWFS;

		OnShowFinished();
	}

	protected virtual void OnShowStarted ()
	{

	}

	protected virtual void OnShowFinished ()
	{
		CanClick = true;
	}

	protected virtual IEnumerator HideCR ( float _delay, bool _instant )
	{
		CanClick = false;

		if (_delay != 0f)
			yield return new WaitForSecondsRealtime(_delay);

		OnHideStarted();
		SetModalVisible(false, _instant ? 0f : m_modalFadeDuration);
		//SetContainersVisible(false, _instant ? 0f : (m_overrideDurations ? m_hideDuration : null));

		if (!_instant)
			yield return m_hideDurationWFS;

		OnHideFinished();
	}

	protected virtual void OnHideStarted ()
	{
		/*if (m_freezeTimeOnShow)
			TimeManager.UnfreezeTime();

		if (m_disableCameraOnShow)
			CameraManager.Instance.SetActiveCam(true);*/
	}

	protected virtual void OnHideFinished ()
	{
		onWindowClosed?.Invoke();
		//onWindowClosed = null;
	}

	public void SetModalVisible ( bool _visible, float _duration )
	{
		if (m_modalCanvasGroup == null) return;

		m_modalTween?.Kill();

		if (_duration == 0f)
		{
			m_modalCanvasGroup.alpha = m_modalAlphaOffOn[_visible ? 1 : 0];
		}
		else
		{
			m_modalTween = m_modalCanvasGroup.DOFade(m_modalAlphaOffOn[_visible ? 1 : 0], _duration).SetUpdate(true);
		}
	}
}
