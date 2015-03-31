using UnityEngine;
using System.Collections;

public class BombFramgment : MonoBehaviour {

	Parabola	m_parabola;
	Rigidbody	m_rigidbody;

	void Start()
	{
		m_parabola = new Parabola(gameObject, Random.Range(0f, -4f), Random.Range(5f, 7f), Random.Range(-3.14f, 3.14f), Random.Range(1.3f, 1.57f), 3);
		m_rigidbody = GetComponent<Rigidbody>();
		m_rigidbody.AddTorque(Random.Range(0f, 2f), Random.Range(0f, 2f), Random.Range(0f, 2f), ForceMode.Impulse);
	}

	void Update()
	{
		if (false == m_parabola.Update())
		{
			DestroyObject(gameObject);
		}
	}

}
