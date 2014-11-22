using UnityEngine;

public class Parabola {

	float 			m_startTime;
	float			m_finishTime;
	Vector3			m_vel;
	float			m_height;
	float			m_gravity = 10f;
	Vector3			m_oriPos;

	GameObject		m_obj;

	public Parabola(GameObject obj, float hspeed, float vspeed, float vang)
	{
		m_obj = obj;
		m_oriPos = obj.transform.position;

		vang = vang * Mathf.Deg2Rad;
		float hang = -m_obj.transform.eulerAngles.y * Mathf.Deg2Rad;

		m_vel.x = hspeed * Mathf.Cos(hang);
		m_vel.y = vspeed * Mathf.Sin(vang);
		m_vel.z = hspeed * Mathf.Sin(hang);

		m_height = (m_vel.y*m_vel.y)/(2*m_gravity);

		m_startTime = Time.time;
		m_finishTime = Mathf.Abs((m_vel.y/m_gravity)*2)+m_startTime;
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
		if (Time.time <= m_finishTime)
		{
			float elapse = Time.time - m_startTime;
			float x = m_vel.x*elapse;
			float z = m_vel.z*elapse;
			float y = m_vel.y*elapse -0.5f*m_gravity*(elapse*elapse);
			m_obj.transform.position = new Vector3(m_oriPos.x+x, y, m_oriPos.z+z);
			return true;
		}

		return false;
	}

}
