using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class TimeEffector {

	enum Type
	{
		Start,
		Stop,
		BulletTime,
	}

	Type	m_type = Type.Start;
	float	m_bulletTime = 0f;

	static TimeEffector m_ins = null;
	static public TimeEffector Instance
	{
		get {
			if (m_ins == null)
			{
				m_ins = new TimeEffector();
			}

			return m_ins;
		}
	}

	public void StartTime()
	{
		m_type = Type.Start;
	}

	public void StopTime()
	{
		m_type = Type.Stop;
	}

	public void BulletTime()
	{
		if (m_type == Type.Stop)
			return;

		m_type = Type.BulletTime;
		m_bulletTime = 1f;
	}

	public bool IsStop()
	{
		return m_type == Type.Stop;
	}

	public void Update()
	{
		switch(m_type)
		{
		case Type.Start:
			Time.timeScale = 1f;
			break;
		case Type.Stop:
			Time.timeScale = 0f;
			break;
		case Type.BulletTime:
			float t = m_bulletTime;
			if (t > 0)
			{
				Time.timeScale = 1.1f-t;
				m_bulletTime -= 0.01f;
			}	

			break;
		}
	}
}
