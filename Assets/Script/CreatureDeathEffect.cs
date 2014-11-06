using UnityEngine;
using System.Collections;

public class CreatureDeathEffect : MonoBehaviour {

	ParticleSystem m_ps;

	void Start () {
		m_ps = transform.Find("ef death").GetComponent<ParticleSystem>();
	}

	void Update()
	{
		if (m_ps.IsAlive() == false)
		{
			DestroyObject(this.gameObject);
		}
	}
}
