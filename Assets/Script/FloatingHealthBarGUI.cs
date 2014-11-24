using UnityEngine;
using System.Collections;

public class FloatingHealthBarGUI : MonoBehaviour {

	Vector3 targetPos;
	GameObject	m_owner;
	Creature	m_creature;

	[SerializeField]
	Texture		m_gague = null;

	// Use this for initialization
	void Start () {
		m_owner = transform.parent.gameObject;
		m_creature = m_owner.GetComponent<Creature>();

	}

	// Update is called once per frame
	void Update () {

		targetPos = m_owner.transform.position;
	}

	void OnGUI()
	{

		Vector2 pos = Camera.main.WorldToViewportPoint(targetPos);
		float startX = pos.x*Screen.width-25;
		float startY = (1-pos.y)*Screen.height-40;
		int size = 5;

		float ratio = m_creature.m_creatureProperty.getHPRemainRatio();

		GUI.DrawTextureWithTexCoords(new Rect(startX, startY, (50-size)*ratio, size), m_gague, new Rect(0f, 0f, ratio, 1f));

	}
}
