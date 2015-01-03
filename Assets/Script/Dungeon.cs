using UnityEngine;
using System.Collections;
using Enum = System.Enum;

public class Dungeon : MonoBehaviour {

	[SerializeField]
	int				m_dungeonId;

	RefWorldMap		m_refWorldMap;

	[SerializeField]
	GameObject		m_prefSpawnEffect = null;

	GameObject[]	m_prefItemBoxes = new GameObject[(int)ItemData.Type.Count];

	// Use this for initialization
	void Start () {		

		
		m_refWorldMap = RefData.Instance.RefWorldMaps[m_dungeonId];

		string[] itemTypeNames = Enum.GetNames(typeof(ItemData.Type));
		for(int i = 0; i < itemTypeNames.Length-1; ++i)
		{
			m_prefItemBoxes[i] = Resources.Load<GameObject>("Pref/Item" + itemTypeNames[i] + "Box");
		}
	}

	// Update is called once per frame
	void Update () {

	}

	IEnumerator delayLoadLevel(float delay)
	{
		yield return new WaitForSeconds(delay);
		Application.LoadLevel("Worldmap");
	}
	
	public void DelayLoadLevel(float delay)
	{
		StartCoroutine(delayLoadLevel(delay));
	}

	void bindItemOption(ItemData item, RefItemOptionSpawn[] descs)
	{
		foreach(RefItemOptionSpawn desc in descs)
		{
			float ratio = Random.Range(0f, 1f);
			if (ratio <= desc.ratio)
			{
				item.OptionDescs.Add(new ItemOptionDesc(desc.type, Random.Range(desc.minValue, desc.maxValue)));
			}
		}
	}

	IEnumerator EffectBulletTime(float t)
	{
		yield return new WaitForSeconds (0.002f);
		
		if (t > 0)
		{
			Time.timeScale = 1.1f-t;
			StartCoroutine(EffectBulletTime(t-0.002f));
		}
		else
		{

		}
		
	}

	IEnumerator EffectSpawnBossBaby(Parabola parabola, RefMob refMob, RefMobSpawn refMobSpawn, int mobLevel, GameObject champ)
	{
		yield return new WaitForSeconds (0.002f);
		
		if (parabola.Update() == true)
		{
			StartCoroutine(EffectSpawnBossBaby(parabola, refMob, refMobSpawn, mobLevel, champ));
		}
		else
		{
			SpawnMob(refMob, refMobSpawn, parabola.Position, mobLevel, champ);
			parabola.Destroy();
		}
		
	}

	public void OnKillMob(Mob mob)
	{
		StartCoroutine(SpawnItemBox(mob, mob.transform.position));

		if (mob.Boss == true)
		{
			StartCoroutine(EffectBulletTime(1));
			for(int i = 0; i < 4; ++i)
			{
				GameObject spawnEffect = Instantiate (m_prefSpawnEffect, mob.transform.position, m_prefSpawnEffect.transform.rotation) as GameObject;
				Parabola parabola = new Parabola(spawnEffect, Random.Range(-5.5f, 5.5f), Random.Range(5, 7), Random.Range(60, 90), 1);
				StartCoroutine(EffectSpawnBossBaby(parabola, mob.RefMob, mob.RefMobSpawn, mob.m_creatureProperty.Level, mob.m_targeting));
			}
		}
	}

	IEnumerator SpawnEffectDestroy(GameObject obj, float delay)
	{
		
		yield return new WaitForSeconds (delay);
		
		DestroyObject(obj);
		
	}

	public Mob SpawnMob(RefMob refMob, RefMobSpawn refMobSpawn, Vector3 pos, int mobLevel, GameObject champ)
	{
		GameObject prefEnemy = Resources.Load<GameObject>("Pref/mon/" + refMob.prefEnemy);
		GameObject prefEnemyBody = Resources.Load<GameObject>("Pref/" + refMob.prefBody);
		
		Vector3 enemyPos = pos;
		enemyPos.y = m_prefSpawnEffect.transform.position.y;

		GameObject spawnEffect = Instantiate (m_prefSpawnEffect, enemyPos, m_prefSpawnEffect.transform.rotation) as GameObject;
		ParticleSystem particle = spawnEffect.GetComponentInChildren<ParticleSystem>();
		
		StartCoroutine(SpawnEffectDestroy(spawnEffect, particle.duration));
		
		GameObject enemyObj = Instantiate (prefEnemy, enemyPos, Quaternion.Euler (0, 0, 0)) as GameObject;
		GameObject enemyBody = Instantiate (prefEnemyBody, enemyPos, Quaternion.Euler (0, 0, 0)) as GameObject;
		enemyBody.name = "Body";
		enemyBody.transform.parent = enemyObj.transform;
		enemyBody.transform.localPosition = Vector3.zero;
		enemyBody.transform.localRotation = prefEnemyBody.transform.rotation;
		

		Mob enemy = enemyObj.GetComponent<Mob>();
		ItemObject weapon = new ItemObject(new ItemWeaponData(refMob.refWeaponItem));
		weapon.Item.Use(enemy);
		
		enemy.SetTarget(champ);
		enemy.RefMob = refMob;
		enemy.Dungeon = this;
		enemy.RefMobSpawn = refMobSpawn;
		enemy.m_creatureProperty.Level = mobLevel;
		
		return enemy;
		
	}

	IEnumerator SpawnItemBox(Mob mob, Vector3 pos)
	{

		if (mob != null)
		{
			foreach(RefItemSpawn desc in mob.RefMobSpawn.refDropItems)
			{
				float ratio = Random.Range(0f, 1f);
				if (ratio <= desc.ratio)
				{
					GameObject itemBoxObj = (GameObject)Instantiate(m_prefItemBoxes[(int)desc.refItem.type], pos, Quaternion.Euler(0f, 0f, 0f));
					itemBoxObj.SetActive(false);
					ItemData item = null;
					switch(desc.refItem.type)
					{
					case ItemData.Type.Gold:
						item = new ItemGoldData(Random.Range(desc.minValue, desc.maxValue));
						break;
					case ItemData.Type.HealPosion:
						item = new ItemHealPosionData(Random.Range(desc.minValue, desc.maxValue));
						break;
					case ItemData.Type.Weapon:
						item = new ItemWeaponData(desc.refItem.id);
						bindItemOption(item, desc.itemOptionSpawns);
						
						break;
					case ItemData.Type.WeaponUpgradeFragment:
						item = new ItemWeaponUpgradeFragmentData();					
						break;
					case ItemData.Type.Follower:
						item = new ItemFollowerData(desc.refItemId);					
						break;
					case ItemData.Type.WeaponEvolutionFragment:
						item = new ItemWeaponEvolutionFragmentData();					
						break;
					}
					
					if (item != null)
					{
						ItemBox itemBox = itemBoxObj.GetComponent<ItemBox>();
						itemBox.Item = item;
						itemBoxObj.SetActive(true);
						yield return new WaitForSeconds (0.2f);
					}
					
				}
			}
		}


	}

	public int DungeonId
	{
		get{return m_dungeonId;}
	}
}
