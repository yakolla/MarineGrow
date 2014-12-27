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

	RefWorldMap		m_dungeon;
	int				m_wave = 0;
	int				m_repeatWave = 0;
	// Use this for initialization
	void Start () {

		int dungeonId = transform.parent.GetComponent<Dungeon>().DungeonId;		
		m_dungeon = RefData.Instance.RefWorldMaps[dungeonId];

		m_areas = transform.GetComponentsInChildren<Transform>();

		StartWave(0);
	}

	void StartWave(int wave)
	{
		if (m_dungeon.waves.Length <= wave)
		{
			m_repeatWave++;
			wave = 0;
		}

		m_wave = wave;

		StartCoroutine(spawnEnemyPer(m_dungeon.waves[wave]));
	}

	IEnumerator spawnEnemyPer(RefWave refWave)
	{
		if (m_target == null)
		{
			yield return new WaitForSeconds (1f);

			StartCoroutine(spawnEnemyPer(refWave));
		}
		else
		{
			Transform area = m_areas[Random.Range(0,m_areas.Length)];
			float cx = area.position.x;
			float cz = area.position.z;
			float scale = area.localScale.x/2;

			foreach(RefMobSpawn mobSpawn in  refWave.mobSpawns)
			{
				for(int repeatNum = 0; repeatNum < mobSpawn.repeatCount; ++repeatNum)
				{

					foreach(KeyValuePair<int, RefMob> pair in mobSpawn.refMobs)
					{

						GameObject prefEnemy = Resources.Load<GameObject>("Pref/mon/" + pair.Value.prefEnemy);
						for(int i = 0; i < mobSpawn.mobCount; ++i)
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
							enemy.m_creatureProperty.Level = m_wave+1+m_repeatWave*m_dungeon.waves.Length;
						}	
					}

					yield return new WaitForSeconds (mobSpawn.interval);
				}
			}

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
