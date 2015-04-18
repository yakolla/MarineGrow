using UnityEngine;
using System.Collections;

public class MobBossDeathEffect : MonoBehaviour {

	Animator m_ani;

	void Start () {
		m_ani = GetComponent<Animator>();

	}

	void Update()
	{
		if (transform.position.y > 0f)
		{
			Vector3	pos = transform.position;
			pos.y -= Time.deltaTime*6f;
			pos.y = Mathf.Max(pos.y, 0);
			transform.position = pos;
		}

		if (m_ani.GetCurrentAnimatorStateInfo(0).IsName("Death Done") == true)
		{
			GameObjectPool.Instance.Free(gameObject);
		}
	}

}
