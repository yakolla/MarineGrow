using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawn : MonoBehaviour {


	[SerializeField]
	GameObject		m_target = null;

	[SerializeField]
	AudioClip		m_sfxSpawnEffect;

	Transform[]		m_areas = null;

	[SerializeField]
	GameObject		m_prefSpawnEffect = null;

	int				m_wave = 0;
	int				m_spawnCount = 0;
	// Use this for initialization
	void Start () {

		m_areas = transform.GetComponentsInChildren<Transform>();
		StartWave(0);
	}

	void StartWave(int wave)
	{
		int dungeonId = transform.parent.GetComponent<Dungeon>().DungeonId;

		RefWorldMap dungeon = RefData.Instance.RefWorldMaps[dungeonId];

		if (dungeon.waves.Length <= wave)
			return;

		m_wave = wave;

		StartCoroutine(spawnEnemyPer(dungeon.waves[wave], m_spawnCount));
	}

	IEnumerator spawnEnemyPer(RefWave wave, int repeatNum)
	{
		if (m_target != null)
		{
			Transform area = m_areas[Random.Range(0,m_areas.Length)];
			float cx = area.position.x;
			float cz = area.position.z;
			float scale = area.localScale.x/2;

			foreach(KeyValuePair<int, RefMob> pair in wave.refMobSpawns)
			{
				GameObject prefEnemy = Resources.Load<GameObject>("Pref/mon/" + pair.Value.prefEnemy);
				for(int i = 0; i < wave.mobCount; ++i)
				{
					Vector3 enemyPos = prefEnemy.transform.position;
					enemyPos.x = Random.Range(cx-scale,cx+scale);
					enemyPos.z = Random.Range(cz-scale,cz+scale);

					audio.clip = m_sfxSpawnEffect;
					audio.Play();
					GameObject spawnEffect = Instantiate (m_prefSpawnEffect, enemyPos, Quaternion.Euler (0, 0, 0)) as GameObject;
					yield return new WaitForSeconds (spawnEffect.particleSystem.duration);
					DestroyObject(spawnEffect);

					GameObject obj = Instantiate (prefEnemy, enemyPos, Quaternion.Euler (0, 0, 0)) as GameObject;
					Mob enemy = obj.GetComponent<Mob>();
					ItemObject weapon = new ItemObject(new ItemWeaponData(pair.Value.refWeaponItem));
					weapon.Item.Use(enemy);
					
					enemy.SetTarget(m_target);
					enemy.SetSpawnDesc(pair.Value);

					//enemy.m_creatureProperty.AlphaMaxHP+=repeatNum/2;
					enemy.m_creatureProperty.AlphaPhysicalAttackDamage+=repeatNum;
					enemy.m_creatureProperty.AlphaPhysicalDefencePoint+=repeatNum;
				}	
			}

			repeatNum++;
		}
				
		yield return new WaitForSeconds (wave.interval);


		if (repeatNum < wave.repeatCount)
		{
			++m_spawnCount;
			StartCoroutine(spawnEnemyPer(wave, m_spawnCount));
		}
		else
		{
			StartWave(m_wave+1);
		}
	}

	// Update is called once per frame
	void Update () {
		if (m_target == null)
		{
			m_target = GameObject.Find("Champ(Clone)");
		}
	}

}
