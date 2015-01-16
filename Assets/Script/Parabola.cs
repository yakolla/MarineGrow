using UnityEngine;

public class Parabola {

	float 			m_startTime;
	float			m_finishTime;
	Vector3			m_vel;
	float			m_height;
	float			m_gravity = 10f;
	Vector3			m_oriPos;
	int				m_maxBouncing = 1;
	int				m_bouncing = 0;
	bool			m_finish = false;
	float			m_groundY = 0f;
	GameObject		m_obj;

	public Parabola(GameObject obj, float hspeed, float vspeed, float hRadian, float vRadian, int bouncing)
	{
		m_obj = obj;
		m_oriPos = obj.transform.position;
		m_maxBouncing = bouncing;

		m_vel.x = hspeed * Mathf.Cos(hRadian);
		m_vel.y = vspeed * Mathf.Sin(vRadian);
		m_vel.z = hspeed * Mathf.Sin(hRadian);
		m_height = (m_vel.y*m_vel.y)/(2*m_gravity);

		m_startTime = Time.time;
		m_finishTime = Mathf.Abs((m_vel.y/m_gravity)*2)+m_startTime;

	}

	public float GroundY
	{
		set {m_groundY = value;}
		get {return m_groundY;}
	}

	public float MaxHeight
	{
		get { return m_height;}
	}

	public Vector3 Position
	{
		get {return m_obj.transform.position;}
	}

	public void Destroy()
	{
		MonoBehaviour.DestroyObject(m_obj);
	}	


	public bool Update()
	{
		if (m_finish == true)
			return false;

		float elapse = Time.time - m_startTime;
		float x = m_vel.x*elapse;
		float z = m_vel.z*elapse;
		float y = m_vel.y*elapse -0.5f*m_gravity*(elapse*elapse);
		m_obj.transform.position = new Vector3(m_oriPos.x+x, Mathf.Max(y, m_groundY), m_oriPos.z+z);

		if (Time.time > m_finishTime && y <= m_groundY)
		{
			++m_bouncing;
			if (m_maxBouncing <= m_bouncing)
			{
				m_finish = true;
				return false;
			}

			// more bounce			
			m_oriPos = m_obj.transform.position;
			m_vel *= 1-(float)m_bouncing/m_maxBouncing;
			m_startTime = Time.time;
			m_finishTime = Mathf.Abs((m_vel.y/m_gravity)*2)+m_startTime;
		}

		return true;
	}

}
