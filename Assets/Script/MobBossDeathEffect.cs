using UnityEngine;
using System.Collections;

public class MobBossDeathEffect : MonoBehaviour {

	Animator m_ani;

	void Start () {
		m_ani = GetComponent<Animator>();

	}

	void Update()
	{
		if (m_ani.GetCurrentAnimatorStateInfo(0).IsName("Death Done") == true)
		{
			DestroyObject(gameObject);
		}
	}

}
