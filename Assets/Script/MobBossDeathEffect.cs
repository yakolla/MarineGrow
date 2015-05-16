using UnityEngine;
using System.Collections;

public class MobBossDeathEffect : MonoBehaviour {

	Animator m_ani;
	float	m_time = 0;

	void Start () {
		m_ani = GetComponent<Animator>();
		m_time = Time.time + 5f;
	}

	void Update()
	{
		if (transform.position.y > 0f)
		{
			Vector3	pos = transform.position;
			pos.y -= Time.deltaTime*6f;
			pos.y = Mathf.Max(pos.y, 0);
			transform.position = pos;

			return;
		}

		if (m_time < Time.time || m_ani.GetCurrentAnimatorStateInfo(0).IsName("Death Done") == true)
		{
			GameObject.DestroyObject(gameObject);
		}
	}

}
