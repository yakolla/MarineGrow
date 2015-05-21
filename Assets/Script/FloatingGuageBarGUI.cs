using UnityEngine;
using System.Collections;

public class FloatingGuageBarGUI : MonoBehaviour {

	protected Creature	m_creature;


	YGUISystem.GUIGuage m_guage;

	// Use this for initialization
	void Start () {
		m_creature = transform.parent.gameObject.GetComponent<Creature>();
		m_guage = new YGUISystem.GUIGuage(transform.Find("Canvas/HP").gameObject, 
		                                   ()=>{return guageRemainRatio();}, 
		()=>{return ""; }
		);

		Vector3 pos = m_creature.AimpointLocalPos;
		pos.y += 1;
		m_guage.RectTransform.transform.localPosition = pos;
	}

	// Update is called once per frame
	void Update () {
		transform.rotation = Quaternion.Euler(Vector3.zero);
		m_guage.Update();
	}

	virtual protected float guageRemainRatio()
	{
		return 1f;
	}
}

