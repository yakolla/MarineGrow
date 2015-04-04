using UnityEngine;
using System.Collections;

public class FloatingGuageBarGUI : MonoBehaviour {

	Vector3 targetPos;
	protected Creature	m_creature;

	[SerializeField]
	Texture		m_gague = null;

	[SerializeField]
	Texture		m_gagueEdge = null;

	// Use this for initialization
	void Start () {
		m_creature = transform.parent.gameObject.GetComponent<Creature>();

	}

	// Update is called once per frame
	void Update () {

		targetPos = m_creature.transform.position;
	}

	void OnGUI()
	{

		Vector2 pos = Camera.main.WorldToViewportPoint(targetPos);
		float startX = pos.x*Screen.width-Screen.width/25;
		float startY = (1-pos.y)*Screen.height-Screen.height/transform.transform.localPosition.y;
		int size = Screen.width/120;
		int maxSize = size*10;

		float ratio = guageRemainRatio();

		GUI.DrawTextureWithTexCoords(new Rect(startX, startY, (maxSize-size)*ratio, size), m_gague, new Rect(0f, 0f, ratio, 1f));
		GUI.DrawTextureWithTexCoords(new Rect(startX, startY, (maxSize-size), size), m_gagueEdge, new Rect(0f, 0f, 1f, 1f));

	}

	virtual protected float guageRemainRatio()
	{
		return 1f;
	}
}

