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

	float[]		m_hSpeed = {1f, 3f};
	float[]		m_vSpeed = {5f, 5f};
	float[]		m_vRadian = {-3.14f, 3.14f};
	float[]		m_hRadian = {1.3f, 1.5f};

	bool		m_ingDeathEffect;


	public void Init(Spawn spawn, RefMob refMob, RefItemSpawn[] refDropItems, int mobLevel)
	{
		m_spawn = spawn;
		m_refMob = refMob;
		m_refDropItems = refDropItems;
		m_mobLevel = mobLevel;

	}

	public float[]	HSpeed
	{
		get{return m_hSpeed;}
	}

	public float[]	VSpeed
	{
		get{return m_vSpeed;}
	}

	public float[] HRadian
	{
		get{return m_hRadian;}
	}

	public float[] VRadian
	{
		get{return m_vRadian;}
	}

	void Start()
	{
		m_parabola = new Parabola(gameObject, 
		                          Random.Range(m_hSpeed[0], m_hSpeed[1]), 
		                          Random.Range(m_vSpeed[0], m_vSpeed[1]), 
		                          Random.Range(m_vRadian[0], m_vRadian[1]), 
		                          Random.Range(m_hRadian[0], m_hRadian[1]), 
		                          1);
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
