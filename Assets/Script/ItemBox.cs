using UnityEngine;
using System.Collections;

public class ItemBox : MonoBehaviour {

	public enum Type
	{
		Gold,
		HPPosion,
		Ring,
	}

	[SerializeField]
	Type m_itemType = Type.Gold;

	[SerializeField]
	int		m_itemValue = 0;

	float m_speed = 10f;
	float m_startTime;
	float			m_ang = 45f;
	Vector2			m_vel;
	float			m_height;
	float			m_gravity = 10f;

	Bezier			m_bezier;

	GameObject	m_target;
	void Start () {
		m_speed = Random.Range(7, 10);
		m_ang = Random.Range(-8.5f, 8.5f)*10f;
		m_vel.x = m_speed * Mathf.Cos(m_ang);
		m_vel.y = m_speed * Mathf.Sin(m_ang);
		m_height = (m_vel.y*m_vel.y)/(2*m_gravity);
		m_startTime = Time.time;
	}

	public void SetTarget(GameObject target)
	{
		m_target = target;
		m_bezier = new Bezier(gameObject, target.transform.position, transform.position, target.transform.position);
	}

	void Update()
	{
		if (m_target == null)
		{
			if (transform.position.y >= 0)
			{
				float elapse = Time.time - m_startTime;
				float y = m_vel.y*elapse -0.5f*m_gravity*(elapse*elapse);
				transform.position = new Vector3(transform.position.x, y, transform.position.z);
				transform.Translate(m_vel.x*Time.deltaTime, 0, 0, transform);
			}
		}
		else
		{
			if (m_bezier.Update() == false)
			{
				Death();
			}
		}

	}

	public void Death()
	{
		DestroyObject(this.gameObject);
	}

	public Type ItemType
	{
		get { return m_itemType; }
	}

	public int ItemValue
	{
		get { return m_itemValue; }
	}
}
