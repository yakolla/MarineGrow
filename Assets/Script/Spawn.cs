using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Enum = System.Enum;

public class Spawn : MonoBehaviour {


	[SerializeField]
	GameObject		m_champ = null;

	[SerializeField]
	Transform[]		m_areas = null;

	[SerializeField]
	GameObject		m_prefSpawnEffect = null;
	
	GameObject[]	m_prefItemBoxes = new GameObject[(int)ItemData.Type.Count];


	List<GameObject>	m_bosses = new List<GameObject>();

	RefWorldMap		m_refWorldMap;
	Dungeon			m_dungeon;
	int				m_wave = 0;
	int				m_repeatWave = 0;
	// Use this for initialization
	void Start () {

		m_dungeon = transform.parent.GetComponent<Dungeon>();
		int dungeonId = m_dungeon.DungeonId;
		m_refWorldMap = RefData.Instance.RefWorldMaps[dungeonId];

		string[] itemTypeNames = Enum.GetNames(typeof(ItemData.Type));
		for(int i = 0; i < itemTypeNames.Length-1; ++i)
		{
			m_prefItemBoxes[i] = Resources.Load<GameObject>("Pref/Item" + itemTypeNames[i] + "Box");
		}

		guiText.pixelOffset = new Vector2(Screen.width/2, -Screen.height/4);
		m_areas = transform.GetComponentsInChildren<Transform>();
		StartWave(0);
	}



	IEnumerator EffectWaveText(string msg, float alpha)
	{
		guiText.text = msg;
		Color color = guiText.color;
		color.a = alpha;
		guiText.color = color;
		yield return new WaitForSeconds (0.2f);

		if (alpha > 0)
		{
			StartCoroutine(EffectWaveText(msg, alpha-0.1f));
		}

	}

	IEnumerator EffectBulletTime(float t)
	{
		yield return new WaitForSeconds (0.01f);
		
		if (t > 0)
		{
			Time.timeScale = 1.1f-t;
			StartCoroutine(EffectBulletTime(t-0.01f));
		}
		else
		{
			m_champ.GetComponent<Creature>().SetFollowingCamera();
		}
		
	}

	void StartWave(int wave)
	{
		if (m_refWorldMap.waves.Length <= wave)
		{
			m_repeatWave++;
			wave = 0;
		}

		m_wave = wave;

		StartCoroutine(spawnMobPer(m_refWorldMap.waves[wave]));
	}

	IEnumerator checkBossAlive()
	{
		yield return new WaitForSeconds (3f);

		bool existBoss = false;
		foreach(GameObject boss in m_bosses)
		{
			if (boss != null)
			{
				existBoss = true;
				break;
			}
		}

		if (existBoss == true)
		{
			StartCoroutine(checkBossAlive());
		}
		else
		{
			m_bosses.Clear();
			StartWave(m_wave+1);
		}
	}



	IEnumerator spawnMobPer(RefWave refWave)
	{
		if (m_champ == null)
		{
			yield return new WaitForSeconds (1f);

			StartCoroutine(spawnMobPer(refWave));
		}
		else
		{
			int totalWave = m_wave + 1 + m_repeatWave*m_refWorldMap.waves.Length;
			bool isBossWave = m_refWorldMap.waves.Length-1 == m_wave;
			if (isBossWave == true)
			{
				StartCoroutine(EffectWaveText("Boss", 1));
			}
			else
			{
				StartCoroutine(EffectWaveText("Wave " + totalWave, 1));
			}

			Transform area = m_areas[Random.Range(1,m_areas.Length)];
			float cx = area.position.x;
			float cz = area.position.z;
			float scale = area.localScale.x/2;

			foreach(RefMobSpawn mobSpawn in  refWave.mobSpawns)
			{
				for(int repeatNum = 0; repeatNum < mobSpawn.repeatCount; ++repeatNum)
				{

					foreach(KeyValuePair<int, RefMob> pair in mobSpawn.refMobs)
					{
						if (pair.Value.nearByChampOnSpawn == true)
						{
							cx = m_champ.transform.position.x;
							cz = m_champ.transform.position.z;
						}
						else
						{
							cx = area.position.x;
							cz = area.position.z;
						}

						GameObject prefEnemy = Resources.Load<GameObject>("Pref/mon/" + pair.Value.prefEnemy);
						GameObject prefEnemyBody = Resources.Load<GameObject>("Pref/" + pair.Value.prefBody);
						for(int i = 0; i < mobSpawn.mobCount; ++i)
						{
							Vector3 enemyPos = Vector3.zero;
							enemyPos.x = Random.Range(cx-scale,cx+scale);
							enemyPos.z = Random.Range(cz-scale,cz+scale);

							SpawnMob(pair.Value, mobSpawn, enemyPos, totalWave, isBossWave);


							yield return new WaitForSeconds (0.5f);
						}	
					}

					yield return new WaitForSeconds (mobSpawn.interval);
				}
			}

			StartCoroutine(checkBossAlive());

		}
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

	
	IEnumerator EffectSpawnBossBaby(Parabola parabola, RefMob refMob, RefMobSpawn refMobSpawn, int mobLevel)
	{
		yield return new WaitForSeconds (0.002f);
		
		if (parabola.Update() == true)
		{
			StartCoroutine(EffectSpawnBossBaby(parabola, refMob, refMobSpawn, mobLevel));
		}
		else
		{
			SpawnMob(refMob, refMobSpawn, parabola.Position, mobLevel, false);
			parabola.Destroy();
		}
		
	}
	
	public void OnKillMob(Mob mob)
	{
		StartCoroutine(SpawnItemBox(mob, mob.transform.position));
		
		if (mob.Boss == true)
		{
			StartCoroutine(EffectBulletTime(1));
		}
		if (mob.RefMob.eggMob != null)
		{
			for(int i = 0; i < mob.RefMob.eggMob.count; ++i)
			{
				GameObject spawnEffect = Instantiate (m_prefSpawnEffect, mob.transform.position, m_prefSpawnEffect.transform.rotation) as GameObject;
				Parabola parabola = new Parabola(spawnEffect, Random.Range(-2.5f, 2.5f), Random.Range(5, 7), Random.Range(60, 90), 1);
				StartCoroutine(EffectSpawnBossBaby(parabola, mob.RefMob.eggMob.refMob, mob.RefMobSpawn, mob.m_creatureProperty.Level));
			}
		}


	}
	
	IEnumerator SpawnEffectDestroy(GameObject obj, float delay)
	{		
		yield return new WaitForSeconds (delay);
		
		DestroyObject(obj);		
	}


	IEnumerator EffectSpawnMob(RefMob refMob, RefMobSpawn refMobSpawn, Vector3 pos, int mobLevel, bool boss)
	{		
		GameObject prefEnemy = Resources.Load<GameObject>("Pref/mon/" + refMob.prefEnemy);
		GameObject prefEnemyBody = Resources.Load<GameObject>("Pref/" + refMob.prefBody);
		
		Vector3 enemyPos = pos;
		enemyPos.y = m_prefSpawnEffect.transform.position.y;

		GameObject spawnEffect = Instantiate (m_prefSpawnEffect, enemyPos, m_prefSpawnEffect.transform.rotation) as GameObject;
		ParticleSystem particle = spawnEffect.GetComponentInChildren<ParticleSystem>();
		
		StartCoroutine(SpawnEffectDestroy(spawnEffect, particle.duration));

		yield return new WaitForSeconds (1f);
		
		GameObject enemyObj = Instantiate (prefEnemy, enemyPos, Quaternion.Euler (0, 0, 0)) as GameObject;
		GameObject enemyBody = Instantiate (prefEnemyBody, Vector3.zero, Quaternion.Euler (0, 0, 0)) as GameObject;
		enemyBody.name = "Body";
		enemyBody.transform.parent = enemyObj.transform;
		enemyBody.transform.localPosition = Vector3.zero;
		enemyBody.transform.localRotation = prefEnemyBody.transform.rotation;
		enemyBody.transform.localScale *= prefEnemy.transform.localScale.x;
		
		Mob enemy = enemyObj.GetComponent<Mob>();
		enemy.Init(refMob, this, refMobSpawn, boss);
		ItemObject weapon = new ItemObject(new ItemWeaponData(refMob.refWeaponItem));
		weapon.Item.Use(enemy);
		
		enemy.SetTarget(m_champ);
		enemy.m_creatureProperty.Level = mobLevel;
		
		if (boss == true)
		{			
			m_bosses.Add(enemy.gameObject);
			StartCoroutine(EffectBulletTime(1));
			enemy.SetFollowingCamera();
		}

	}
	
	public void SpawnMob(RefMob refMob, RefMobSpawn refMobSpawn, Vector3 pos, int mobLevel, bool boss)
	{
		StartCoroutine(EffectSpawnMob(refMob, refMobSpawn, pos, mobLevel, boss));
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

	// Update is called once per frame
	void Update () {
		if (m_champ == null)
		{
			m_champ = GameObject.Find("Champ(Clone)");
		}
	}

}
