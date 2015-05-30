using UnityEngine;
using System.Collections;

public class ExplosionBullet : Bullet {

	[SerializeField]
	protected GameObject		m_prefBombEffect = null;

	[SerializeField]
	protected float				m_bombRange = 5f;

	// Use this for initialization
	void Start () {
	}

	void OnEnable()
	{
		m_isDestroying = false;
	}

	// Update is called once per frame
	protected void Update () {

		if (m_isDestroying == true)
			return;

		bomb(m_bombRange, m_prefBombEffect);
	}

}