using UnityEngine;
using System.Collections;

public class DamageNumberSprite : MonoBehaviour {

	public enum MovementType
	{
		Parabola,
		RisingUp,
		FloatingUp,
	}

	[SerializeField]
	string	m_damageNumber;

	MovementType	m_movementType = MovementType.Parabola;

	Parabola	m_parabola;

	Creature	m_target;
	Vector3		m_targetPos;

	[SerializeField]
	float		m_duration = 1.5f;
	float		m_startTime = 0f;

	[SerializeField]
	Vector3		m_offset;
	float		m_posY = 0f;
	// Use this for initialization
	void Start () {

	}
	
	public void Init(Creature obj, string damage, Color color, MovementType movementType)
	{
		m_movementType = movementType;
		m_target = obj;
		m_targetPos = obj.transform.position;
		m_startTime = Time.time;

		TypogenicText	text = GetComponent<TypogenicText>();
		text.Text = damage;
		text.ColorTopLeft = color;

		if (movementType == MovementType.Parabola)
			m_parabola = new Parabola(gameObject, 5f, 0f, 1.3f, 1);
	}
	
	// Update is called once per frame
	void LateUpdate () {		

		switch(m_movementType)
		{
		case MovementType.Parabola:
			if (false == m_parabola.Update())
			{
				GameObjectPool.Instance.Free(gameObject);
			}
			break;
		case MovementType.RisingUp:
			if (m_target)
			{
				m_targetPos = m_target.transform.position+m_target.AimpointLocalPos;
			}
			m_posY += 1.5f*Time.deltaTime;

			m_targetPos.y += m_posY;
			transform.position = m_targetPos;

			if (m_startTime+m_duration < Time.time)
			{
				GameObjectPool.Instance.Free(gameObject);
			}
			break;
		case MovementType.FloatingUp:
			if (m_target)
			{
				m_targetPos = m_target.transform.position+m_target.AimpointLocalPos;
				m_targetPos.y += 1f;
			}

			Debug.Log(m_targetPos);
			transform.position = m_targetPos;
			
			if (m_startTime+m_duration < Time.time)
			{
				GameObjectPool.Instance.Free(gameObject);
			}
			break;
		}

	}
}

