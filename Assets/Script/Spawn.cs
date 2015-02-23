using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Enum = System.Enum;

public class Spawn : MonoBehaviour {

	public enum SpawnMobType
	{
		Normal,
		Boss,
		Egg,
	}

	[SerializeField]
	GameObject		m_champ = null;

	[SerializeField]
	Transform[]		m_areas = null;

	[SerializeField]
	GameObject		m_prefSpawnEffect = null;
	
	GameObject[]	m_prefItemBoxSkins = new GameObject[(int)ItemData.Type.Count];
	GameObject		m_prefItemBox;

	[SerializeField]
	GameObject		m_prefEgg;
	float			m_effectBulletTime = 0f;

	FollowingCamera	m_followingCamera = null;

	List<GameObject>	m_bosses = new List<GameObject>();

	RefWorldMap		m_refWorldMap;
	Dungeon			m_dungeon;
	int				m_wave = 0;

	[SerializeField]
	int				m_spawningPool = 0;
	// Use this for initialization
	void Start () {

		m_followingCamera = Camera.main.GetComponent<FollowingCamera>();

		m_dungeon = transform.parent.GetComponent<Dungeon>();
		int dungeonId = m_dungeon.DungeonId;
		m_refWorldMap = RefData.Instance.RefWorldMaps[dungeonId];

		m_prefItemBox = Resources.Load<GameObject>("Pref/ItemBox/ItemBox");
		string[] itemTypeNames = Enum.GetNames(typeof(ItemData.Type));
		for(int i = 0; i < itemTypeNames.Length-1; ++i)
		{
			m_prefItemBoxSkins[i] = Resources.Load<GameObject>("Pref/ItemBox/item_" + itemTypeNames[i] + "_skin");
			if (m_prefItemBoxSkins[i] == null)
				Debug.Log("Pref/ItemBox/item_" + itemTypeNames[i] + "_skin");
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

	RefWave	GetCurrentWave()
	{
		return m_refWorldMap.waves[m_wave%m_refWorldMap.waves.Length];
	}

	void StartWave(int wave)
	{
		m_wave = wave;
		StartCoroutine(checkBossAlive());
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

			StartCoroutine(spawnMobPer(GetCurrentWave().mobSpawns[m_spawningPool%GetCurrentWave().mobSpawns.Length]));


		}
	}


	Transform	getSpawnArea(bool champAreaExcept)
	{
		if (m_champ != null && champAreaExcept == true)
		{
			List<Transform> areas = new List<Transform>();

			for(int i = 1; i < m_areas.Length; ++i)
			{
				Transform area = m_areas[i];
				float dist = Vector3.Distance(area.position, m_champ.transform.position);
				if (dist > 5f)
				{
					areas.Add(area);

				}
			}
			return areas[Random.Range(1,areas.Count)];
		}

		return m_areas[Random.Range(1,m_areas.Length)];
	}

	void	buildSpawnMob(List<RefMob> buildMobs, List<int> buildMobCount, float progress, RefMobSpawnRatio.Desc spawnRatioDesc, RefMob[] mobs)
	{
		if (spawnRatioDesc == null)
			return;

		int minIndex = (int)(spawnRatioDesc.ratio[0] * mobs.Length);
		int maxIndex = (int)((spawnRatioDesc.ratio[0] * (1f-progress) + spawnRatioDesc.ratio[1] * progress) * mobs.Length);

		minIndex = Mathf.Clamp(minIndex, 0, mobs.Length-1);

		buildMobs.Add(mobs[Random.Range(minIndex, maxIndex)]);

		minIndex = (int)(spawnRatioDesc.count[0]);
		maxIndex = (int)(spawnRatioDesc.count[0] * (1f-progress) + spawnRatioDesc.count[1] * progress);
		buildMobCount.Add(Random.Range(minIndex, maxIndex));
	}

	IEnumerator spawnMobPer(RefMobSpawn mobSpawn)
	{
		if (m_champ == null)
		{
			yield return new WaitForSeconds (1f);

			StartCoroutine(spawnMobPer(mobSpawn));
		}
		else
		{

			StartCoroutine(EffectWaveText("Wave " + (m_spawningPool + 1), 1));

			//foreach(RefMobSpawn mobSpawn in  mobSpawns)
			{
				SpawnMobType spawnMobType = SpawnMobType.Normal;
				if (mobSpawn.boss == true)
				{
					guiText.text = "Boss";
					Color color = guiText.color;
					color.a = 1;
					guiText.color = color;
					spawnMobType = SpawnMobType.Boss;
				}

				float waveProgress = Mathf.Min(1f, m_spawningPool / GetCurrentWave().mobSpawns.Length * 0.1f);
				//Debug.Log(waveProgress + "," + m_spawningPool);

				int spawnCount = 0;
				int mobSpawnRepeatCount = (int)(mobSpawn.repeatCount[0] * (1f-waveProgress) + mobSpawn.repeatCount[1] * waveProgress);
				for(int r = 0; r < mobSpawnRepeatCount; ++r)
				{
					List<RefMob>	spawnMobs = new List<RefMob>();
					List<int> 		spawnMobCount = new List<int>();
					buildSpawnMob(spawnMobs, spawnMobCount, waveProgress, mobSpawn.refMobIds.melee, RefData.Instance.RefMeleeMobs);
					buildSpawnMob(spawnMobs, spawnMobCount, waveProgress, mobSpawn.refMobIds.range, RefData.Instance.RefRangeMobs);
					buildSpawnMob(spawnMobs, spawnMobCount, waveProgress, mobSpawn.refMobIds.boss, RefData.Instance.RefBossMobs);
					buildSpawnMob(spawnMobs, spawnMobCount, waveProgress, mobSpawn.refMobIds.shuttle, RefData.Instance.RefShuttleMobs);

					for(int ii = 0;  ii < spawnMobs.Count; ++ii)
					{
						Transform area = getSpawnArea(true);
						Vector3 cp = area.position;
						Vector3 scale = area.localScale*0.5f;

						RefMob refMob = spawnMobs[ii];
						if (refMob.nearByChampOnSpawn == true)
						{
							if (m_champ)
							{
								cp = m_champ.transform.position;
							}
							
						}
						else
						{
							cp = area.position;
						}

						for(int i = 0; i < spawnMobCount[ii]; ++i)
						{
							Vector3 enemyPos = cp;
							enemyPos.x += Random.Range(-scale.x,scale.x);
							enemyPos.z += Random.Range(-scale.z,scale.z);
							
							++spawnCount;

							RefItemSpawn[] dropItems = mobSpawn.refDropItems;
							if (dropItems == null)
							{
								if (GetCurrentWave().itemSpawn.mapMobItems.ContainsKey(refMob.id))
								{
									dropItems = GetCurrentWave().itemSpawn.mapMobItems[refMob.id].refDropItems;
								}

							}
							StartCoroutine(  EffectSpawnMob(refMob
							                                , dropItems
							                                , enemyPos
							                                , spawnMobLevel()
							                                , spawnMobType
							                                , false)
							               );
							
							
							yield return new WaitForSeconds (0.5f);
						}
					}
					yield return new WaitForSeconds (mobSpawn.interval);
				}
				StartCoroutine(checkBossAlive());
			}

			m_spawningPool++;

		}
	}

	void bindItemOption(ItemData item, RefItemOptionSpawn[] descs)
	{
		foreach(RefItemOptionSpawn desc in descs)
		{
			float ratio = Random.Range(0f, 1f);
			if (ratio <= desc.ratio)
			{
				item.OptionDescs.Add(new ItemMagicOption(desc.type, Random.Range(desc.minValue, desc.maxValue)));
			}
		}
	}

	int spawnMobLevel()
	{
		return 1+m_spawningPool / GetCurrentWave().mobSpawns.Length;
	}

	IEnumerator EffectSpawnItemPandora(RefMob refMob, RefItemSpawn[] refDropItems, Vector3 pos)
	{
		GameObject eggObj = Instantiate (Resources.Load<GameObject>("Pref/mon_skin/item_supplybox_skin"), pos, Quaternion.Euler(Vector3.zero)) as GameObject;

		Parabola parabola = new Parabola(eggObj, Random.Range(1f, 3f), 5f, Random.Range(-3.14f, 3.14f), Random.Range(-1.5f, 1.5f), 3);
		while(parabola.Update())
		{
			yield return null;
		}	

		SpawnMob(refMob, refDropItems, parabola.Position, 1, SpawnMobType.Normal, false);
		DestroyObject(eggObj);
	}

	public RefItemSpawn[] GetMobItemDrops(RefMob refMob)
	{
		return GetCurrentWave().itemSpawn.mapMobItems.ContainsKey(refMob.id) ? GetCurrentWave().itemSpawn.mapMobItems[refMob.id].refDropItems : null;
	}
	
	public Egg spawnMobEgg(RefMob refMob, Vector3 pos, int mobLevel)
	{
		GameObject eggObj = Instantiate(m_prefEgg, pos, m_prefEgg.transform.rotation) as GameObject;
		Egg egg = eggObj.GetComponent<Egg>();
		egg.Init(this, refMob, GetMobItemDrops(refMob), mobLevel);

		return egg;
	}
	
	public void OnKillMob(Mob mob)
	{

		SpawnItemBox(mob.RefDropItems, mob.transform.position);
		
		if (mob.Boss == true)
		{
			m_effectBulletTime = 1f;
		}
		if (mob.RefMob.eggMob != null)
		{
			for(int i = 0; i < mob.RefMob.eggMob.count; ++i)
			{
				spawnMobEgg(mob.RefMob.eggMob.refMob, mob.transform.position, mob.m_creatureProperty.Level);
			}
		}


	}

	IEnumerator SpawnEffectDestroy(GameObject obj, float delay)
	{		
		yield return new WaitForSeconds (delay);
		
		DestroyObject(obj);		
	}


	IEnumerator EffectSpawnMob(RefMob refMob, RefItemSpawn[] refDropItems, Vector3 pos, int mobLevel, SpawnMobType spawnMobType, bool followingCamera)
	{		

		Vector3 enemyPos = pos;
		enemyPos.y = m_prefSpawnEffect.transform.position.y;

		if (m_prefSpawnEffect != null)
		{
			GameObject spawnEffect = Instantiate (m_prefSpawnEffect, enemyPos, m_prefSpawnEffect.transform.rotation) as GameObject;
			ParticleSystem particle = spawnEffect.GetComponentInChildren<ParticleSystem>();
			
			StartCoroutine(SpawnEffectDestroy(spawnEffect, particle.duration));
		}

		yield return new WaitForSeconds (1f);
		
		SpawnMob(refMob, refDropItems, enemyPos, mobLevel, spawnMobType, followingCamera);

	}
	
	public Mob SpawnMob(RefMob refMob, RefItemSpawn[] refDropItems, Vector3 pos, int mobLevel, SpawnMobType spawnMobType, bool followingCamera)
	{
		GameObject prefEnemy = Resources.Load<GameObject>("Pref/mon/mob");
		GameObject prefEnemyBody = Resources.Load<GameObject>("Pref/mon_skin/" + refMob.prefBody);
		if (prefEnemyBody == null)
		{
			Debug.Log(refMob.prefBody);
			return null;
		}
		Vector3 enemyPos = pos;

		GameObject enemyObj = Instantiate (prefEnemy, enemyPos, Quaternion.Euler (0, 0, 0)) as GameObject;
		GameObject enemyBody = Instantiate (prefEnemyBody, Vector3.zero, Quaternion.Euler (0, 0, 0)) as GameObject;
		enemyBody.name = "Body";
		enemyBody.transform.parent = enemyObj.transform;
		enemyBody.transform.localPosition = Vector3.zero;
		enemyBody.transform.localRotation = prefEnemyBody.transform.rotation;
		switch(spawnMobType)
		{
		case SpawnMobType.Normal:
			break;
		case SpawnMobType.Boss:
			break;
		case SpawnMobType.Egg:
			enemyObj.transform.localScale *= 0.5f;
			break;
		}
		
		bool boss = spawnMobType == SpawnMobType.Boss;
		
		Mob enemy = enemyObj.GetComponent<Mob>();
		enemy.Init(refMob, mobLevel, this, refDropItems, boss);
		
		enemy.SetTarget(m_champ);
		Debug.Log(refMob.prefBody + ", Lv : " + mobLevel + ", HP: " + enemy.m_creatureProperty.HP + ", PA:" + enemy.m_creatureProperty.PhysicalAttackDamage + ", PD:" + enemy.m_creatureProperty.PhysicalDefencePoint);
		
		if (boss == true)	
		{
			m_bosses.Add(enemy.gameObject);
		}

		
		if (followingCamera == true)
		{
			m_effectBulletTime = 1f;
			enemy.SetFollowingCamera(m_champ);
			TimeEffector.Instance.StopTime();
		}

		return enemy;
	}
	
	public void SpawnItemBox(RefItemSpawn[] refDropItems, Vector3 pos)
	{		
		if (refDropItems == null)
		{
			refDropItems = GetCurrentWave().itemSpawn.defaultItem;
		}

		foreach(RefItemSpawn desc in refDropItems)
		{
			for(int i = 0; i < desc.count; ++i)
			{
				float ratio = Random.Range(0f, 1f);
				if (ratio <= desc.ratio)
				{
					float scale = 1f;
					ItemData item = null;
					switch(desc.refItem.type)
					{
					case ItemData.Type.Gold:
						item = new ItemGoldData(Random.Range(desc.minValue, desc.maxValue));
						switch(item.Count/3)
						{
						case 0:
							scale = 0.3f;
							break;
						case 1:
							scale = 0.5f;
							break;
						case 2:
							scale = 0.7f;
							break;
						}
						break;
					case ItemData.Type.HealPosion:
						item = new ItemHealPosionData(Random.Range(desc.minValue, desc.maxValue));
						switch(item.Count/300)
						{
						case 0:
							scale = 0.3f;
							break;
						case 1:
							scale = 0.5f;
							break;
						case 2:
							scale = 0.7f;
							break;
						}
						break;
					case ItemData.Type.Weapon:
						item = new ItemWeaponData(desc.refItem.id, null);
						bindItemOption(item, desc.itemOptionSpawns);
						
						break;
					case ItemData.Type.WeaponParts:
						item = new ItemWeaponUpgradeFragmentData();					
						break;
					case ItemData.Type.Follower:
						//item = new ItemFollowerData(RefData.Instance.RefMobs[desc.maxValue]);					
						break;
					case ItemData.Type.WeaponDNA:
						item = new ItemWeaponEvolutionFragmentData();					
						break;
					case ItemData.Type.GoldMedal:
						item = new ItemGoldMedalData();					
						break;
					case ItemData.Type.SilverMedal:
						item = new ItemSilverMedalData();					
						break;
					case ItemData.Type.MobEgg:
						spawnMobEgg(RefData.Instance.RefMeleeMobs[Random.Range(0, RefData.Instance.RefMeleeMobs.Length)], pos, spawnMobLevel());
						break;
					case ItemData.Type.ItemPandora:
						StartCoroutine(EffectSpawnItemPandora(RefData.Instance.RefItemPandoraMobs[desc.minValue], GetMobItemDrops(RefData.Instance.RefItemPandoraMobs[desc.minValue]), pos));
						break;
					}
					
					if (item != null)
					{
						GameObject itemBoxObj = (GameObject)Instantiate(m_prefItemBox, pos, Quaternion.Euler(0f, 0f, 0f));
						GameObject itemSkinObj = (GameObject)Instantiate(m_prefItemBoxSkins[(int)desc.refItem.type], pos, Quaternion.Euler(0f, 0f, 0f));
						itemSkinObj.transform.parent = itemBoxObj.transform;
						itemSkinObj.transform.localPosition = Vector3.zero;
						itemSkinObj.transform.localRotation = m_prefItemBoxSkins[(int)desc.refItem.type].transform.rotation;
						itemBoxObj.transform.localScale = Vector3.one * scale;
						itemBoxObj.SetActive(false);
						
						ItemBox itemBox = itemBoxObj.GetComponent<ItemBox>();
						itemBox.Item = item;
						itemBoxObj.SetActive(true);
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
