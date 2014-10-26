using UnityEngine;
using System.Collections;

public class DamageNumberGUI : MonoBehaviour {

	public float scroll = 0.08f;
	public float alpha = 1f;
	public float duration = 1.5f;
	float	m_lastTime = 0f;
	Vector3 targetPos;
	GameObject	target;
	float	posY = 0;
	// Use this for initialization
	void Start () {
		guiText.material.color = new Color(1f,1f,1f,1.0f);
	}

	public void Init(GameObject obj, string str)
	{
		target = obj;
		targetPos = obj.transform.position;
		guiText.text = str;
		m_lastTime = Time.time;

		Transform trans = transform.Find("DamageNumberGUI");
		if (trans != null)
		{
			trans.GetComponent<DamageNumberGUI>().Init(obj, str);
		}
	}

	// Update is called once per frame
	void Update () {


		if (m_lastTime+duration>Time.time){

			if (target)
			{
				targetPos = target.transform.position;
			}

			Vector3 pos = Camera.main.WorldToViewportPoint(targetPos);
			posY += scroll*Time.deltaTime;
			pos.y += posY;
			transform.position = pos;

			alpha -= Time.deltaTime/duration;
			Color color = guiText.material.color;
			color.a = alpha;  
			guiText.material.color = color;


		}
		else {
			Destroy(gameObject);
		}
	}
}
