using UnityEngine;

public class Parabola {

	float 			m_speed = 10f;
	float 			m_startTime;
	float			m_ang = 45f;
	Vector2			m_vel;
	float			m_height;
	float			m_gravity = 10f;

	GameObject		m_obj;

	public Parabola(GameObject obj, float speed, float ang)
	{
		m_obj = obj;
		m_speed = speed;
		m_ang = ang;

		m_vel.x = m_speed * Mathf.Cos(m_ang);
		m_vel.y = m_speed * Mathf.Sin(m_ang);
		m_height = (m_vel.y*m_vel.y)/(2*m_gravity);
		m_startTime = Time.time;
	}

	public float MaxHeight
	{
		get { return m_height;}
	}

	public void Destroy()
	{
		MonoBehaviour.DestroyObject(m_obj);
	}	


	public bool Update()
	{
		if (m_obj.transform.position.y >= 0)
		{
			float elapse = Time.time - m_startTime;
			float y = m_vel.y*elapse -0.5f*m_gravity*(elapse*elapse);
			m_obj.transform.position = new Vector3(m_obj.transform.position.x, y, m_obj.transform.position.z);
			m_obj.transform.Translate(m_vel.x*Time.deltaTime, 0, 0, m_obj.transform);
			return true;
		}

		return false;
	}

}
