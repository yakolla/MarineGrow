using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawn : MonoBehaviour {


	[SerializeField]
	GameObject		m_target = null;

	[SerializeField]
	AudioClip		m_sfxSpawnEffect;

	[SerializeField]
	Transform[]		m_areas = null;

	[SerializeField]
	GameObject		m_prefSpawnEffect = null;

	List<GameObject>	m_bosses = new List<GameObject>();

	RefWorldMap		m_dungeon;
	int				m_wave = 0;
	int				m_repeatWave = 0;
	// Use this for initialization
	void Start () {

		int dungeonId = transform.parent.GetComponent<Dungeon>().DungeonId;		
		m_dungeon = RefData.Instance.RefWorldMaps[dungeonId];
		guiText.pixelOffset = new Vector2(Screen.width/2, -Screen.height/4);
		m_areas = transform.GetComponentsInChildren<Transform>();
		StartWave(0);
	}

	IEnumerator SpawnEffectDestroy(GameObject obj, float delay)
	{

		yield return new WaitForSeconds (delay);
		
		DestroyObject(obj);
		
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
			m_target.GetComponent<Creature>().SetFollowingCamera();
		}
		
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

	IEnumerator checkBossAlive()
	{
		yield return new WaitForSeconds (1f);

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

	IEnumerator spawnEnemyPer(RefWave refWave)
	{
		if (m_target == null)
		{
			yield return new WaitForSeconds (1f);

			StartCoroutine(spawnEnemyPer(refWave));
		}
		else
		{
			int totalWave = m_wave + 1 + m_repeatWave*m_dungeon.waves.Length;
			bool isBossWave = m_dungeon.waves.Length-1 == m_wave;
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

						GameObject prefEnemy = Resources.Load<GameObject>("Pref/mon/" + pair.Value.prefEnemy);
						for(int i = 0; i < mobSpawn.mobCount; ++i)
						{
							Vector3 enemyPos = prefEnemy.transform.position;
							enemyPos.x = Random.Range(cx-scale,cx+scale);
							enemyPos.y = m_prefSpawnEffect.transform.position.y;
							enemyPos.z = Random.Range(cz-scale,cz+scale);

							audio.clip = m_sfxSpawnEffect;
							audio.Play();

							GameObject spawnEffect = Instantiate (m_prefSpawnEffect, enemyPos, m_prefSpawnEffect.transform.rotation) as GameObject;
							ParticleSystem particle = spawnEffect.GetComponentInChildren<ParticleSystem>();

							StartCoroutine(SpawnEffectDestroy(spawnEffect, particle.duration));
							yield return new WaitForSeconds (1);

							GameObject obj = Instantiate (prefEnemy, enemyPos, Quaternion.Euler (0, 0, 0)) as GameObject;
							if (isBossWave == true)
							{

								m_bosses.Add(obj);
								StartCoroutine(EffectBulletTime(1));
								obj.GetComponent<Creature>().SetFollowingCamera();
							}

							Mob enemy = obj.GetComponent<Mob>();
							ItemObject weapon = new ItemObject(new ItemWeaponData(pair.Value.refWeaponItem));
							weapon.Item.Use(enemy);
							
							enemy.SetTarget(m_target);
							enemy.SetSpawnDesc(pair.Value);

							enemy.m_creatureProperty.Level = totalWave;
						}	
					}

					yield return new WaitForSeconds (mobSpawn.interval);
				}
			}

			StartCoroutine(checkBossAlive());

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
