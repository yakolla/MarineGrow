using UnityEngine;
using System.Collections;

public class Egg : MonoBehaviour {

	[SerializeField]
	GameObject	m_prefEggDeath;

	Spawn		m_spawn;
	RefMob		m_refMob;
	RefItemSpawn[]	m_refDropItems;
	int			m_mobLevel;
	Parabola 	m_parabola;


	bool		m_ingDeathEffect;


	public void Init(Spawn spawn, RefMob refMob, RefItemSpawn[] refDropItems, int mobLevel)
	{
		m_spawn = spawn;
		m_refMob = refMob;
		m_refDropItems = refDropItems;
		m_mobLevel = mobLevel;

	}

	void Start()
	{
		m_parabola = new Parabola(gameObject, Random.Range(1f, 3f), 5f, Random.Range(-3.14f, 3.14f), Random.Range(-1.5f, 1.5f), 1);
	}

	IEnumerator EffectEgg()
	{
		yield return new WaitForSeconds (3f);		
		
		GameObject eggObj = Instantiate (m_prefEggDeath, m_parabola.Position, transform.rotation) as GameObject;
		EggBroken	eggBroken = eggObj.GetComponent<EggBroken>();
		eggBroken.Init(m_spawn, m_refMob, m_refDropItems, m_mobLevel);		
		
		DestroyObject(this.gameObject);
	}

	void Update()
	{
		if(m_parabola.Update())
		{
			return;
		}

		if (m_ingDeathEffect == false)
		{
			m_ingDeathEffect = true;
			StartCoroutine(EffectEgg());
		}


	}
}
