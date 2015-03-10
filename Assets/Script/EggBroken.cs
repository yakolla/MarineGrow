using UnityEngine;
using System.Collections;

public class EggBroken : MonoBehaviour {


	Spawn		m_spawn;
	RefMob		m_refMob;
	RefItemSpawn[]	m_refDropItems;
	uint			m_mobLevel;

	public void Init(Spawn spawn, RefMob refMob, RefItemSpawn[] refDropItems, uint mobLevel)
	{
		m_spawn = spawn;
		m_refMob = refMob;
		m_refDropItems = refDropItems;
		m_mobLevel = mobLevel;

	}

	void Start()
	{
		StartCoroutine(EffectEgg(m_refMob, m_refDropItems));
	}

	IEnumerator EffectEgg(RefMob refMob, RefItemSpawn[] refDropItems)
	{
		audio.Play();
		yield return new WaitForSeconds (0.5f);
		
		m_spawn.SpawnMob(m_refMob, m_refDropItems, transform.position, m_mobLevel, 0.5f, false);

		while(transform.position.y > -1f)
		{
			Vector3 eggPos = transform.position;
			eggPos.y -= 1f*Time.deltaTime;
			transform.position = eggPos;
			yield return null;
		}
		
		DestroyObject(gameObject);
	}
}
