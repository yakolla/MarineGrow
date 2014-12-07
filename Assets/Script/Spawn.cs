using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawn : MonoBehaviour {


	[SerializeField]
	GameObject		m_target = null;


	int				m_wave = 0;
	int				m_spawnCount = 0;
	int				m_maxSpawnCountOfTheWave = 0;
	// Use this for initialization
	void Start () {
		StartWave(0);
	}

	void StartWave(int wave)
	{
		int dungeonId = transform.parent.GetComponent<Dungeon>().DungeonId;

		if (RefData.Instance.RefWorldMaps[dungeonId].waves.Length <= wave)
			return;

		m_wave = wave;
		m_spawnCount = 0;
		m_maxSpawnCountOfTheWave = 0;

		foreach(KeyValuePair<int, RefMobSpawn> pair in RefData.Instance.RefWorldMaps[dungeonId].waves[wave].refMobSpawns)
		{
			m_maxSpawnCountOfTheWave += pair.Value.repeatCount;
			StartCoroutine(spawnEnemyPer(pair.Value, 0));
		}
	}

	IEnumerator spawnEnemyPer(RefMobSpawn desc, int repeatNum)
	{
		if (m_target != null && repeatNum < desc.repeatCount)
		{
			float cx = transform.position.x;
			float cz = transform.position.z;
			
			GameObject prefEnemy = Resources.Load<GameObject>("Pref/mon/" + desc.prefEnemy);
			for(int i = 0; i < desc.mobCount; ++i)
			{
				Vector3 enemyPos = prefEnemy.transform.position;
				GameObject obj = Instantiate (prefEnemy, new Vector3(Random.Range(cx,cx+3f), enemyPos.y, Random.Range(cz,cz+3f)), Quaternion.Euler (0, 0, 0)) as GameObject;
				obj.GetComponent<Enemy>().SetTarget(m_target);
				obj.GetComponent<Enemy>().SetSpawnDesc(desc);
			}
		}
				
		yield return new WaitForSeconds (desc.interval);

		if (repeatNum < desc.repeatCount)
		{
			StartCoroutine(spawnEnemyPer(desc, repeatNum+1));
		}

		++m_spawnCount;

		if (m_spawnCount == m_maxSpawnCountOfTheWave)
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
