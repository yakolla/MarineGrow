using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SpinButtonGUI : MonoBehaviour {


	[SerializeField]
	UnityEngine.Events.UnityEvent	m_onStop;


	public void StopSpin()
	{
		m_onStop.Invoke();
		
		this.gameObject.GetComponent<Animator>().SetBool("Spin", false);
	}

	public bool IsSpining()
	{
		return gameObject.GetComponent<Animator>().GetBool("Spin");
	}
}
