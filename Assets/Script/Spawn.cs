using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawn : MonoBehaviour {


	[SerializeField]
	GameObject		m_target = null;

	Transform[]		m_areas = null;


	int				m_wave = 0;
	int				m_maxRepeatCount = 0;
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
		m_maxRepeatCount = dungeon.waves[wave].repeatCount;

		StartCoroutine(spawnEnemyPer(dungeon.waves[wave], 0));
	}

	IEnumerator spawnEnemyPer(RefWave wave, int repeatNum)
	{
		if (m_target != null)
		{
			Transform area = m_areas[Random.Range(0,m_areas.Length)];
			float cx = area.position.x;
			float cz = area.position.z;
			float scale = area.localScale.x;

			foreach(KeyValuePair<int, RefMob> pair in wave.refMobSpawns)
			{
				GameObject prefEnemy = Resources.Load<GameObject>("Pref/mon/" + pair.Value.prefEnemy);
				for(int i = 0; i < wave.mobCount; ++i)
				{
					Vector3 enemyPos = prefEnemy.transform.position;
					GameObject obj = Instantiate (prefEnemy, new Vector3(Random.Range(cx-scale,cx+scale), enemyPos.y, Random.Range(cz-scale,cz+scale)), Quaternion.Euler (0, 0, 0)) as GameObject;
					Enemy enemy = obj.GetComponent<Enemy>();
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
			StartCoroutine(spawnEnemyPer(wave, repeatNum));
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
