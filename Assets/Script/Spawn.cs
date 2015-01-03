using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawn : MonoBehaviour {


	[SerializeField]
	GameObject		m_champ = null;

	[SerializeField]
	Transform[]		m_areas = null;



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

						GameObject prefEnemy = Resources.Load<GameObject>("Pref/mon/" + pair.Value.prefEnemy);
						GameObject prefEnemyBody = Resources.Load<GameObject>("Pref/" + pair.Value.prefBody);
						for(int i = 0; i < mobSpawn.mobCount; ++i)
						{
							Vector3 enemyPos = Vector3.zero;
							enemyPos.x = Random.Range(cx-scale,cx+scale);
							enemyPos.z = Random.Range(cz-scale,cz+scale);
							Mob mob = m_dungeon.SpawnMob(pair.Value, mobSpawn, enemyPos, totalWave, m_champ);
							if (isBossWave == true)
							{			
								m_bosses.Add(mob.gameObject);
								StartCoroutine(EffectBulletTime(1));
								mob.Boss = true;
								mob.SetFollowingCamera();
							}

							yield return new WaitForSeconds (1);
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
		if (m_champ == null)
		{
			m_champ = GameObject.Find("Champ(Clone)");
		}
	}

}
