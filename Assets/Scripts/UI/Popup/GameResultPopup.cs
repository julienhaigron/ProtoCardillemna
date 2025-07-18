using UnityEngine;
using TMPro;

public class GameResultPopup : AUIPopup
{
	[SerializeField] private TextMeshProUGUI m_resultTMP;
    
    public void Init(bool _didPlayerWin )
	{
		m_resultTMP.text = _didPlayerWin ? "Player wins" : "You Lose";
	}
}
