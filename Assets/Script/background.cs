using UnityEngine;
using System.Collections;

public class background : MonoBehaviour {


	GameObject		m_prefEnemy;
	GameObject		m_champ;

	// Use this for initialization
	void Start () {
		m_champ = GameObject.Find("/Champ") as GameObject;

		m_prefEnemy = Resources.Load<GameObject>("Pref/Enemy");

		for(int i = 0; i < 10; ++i)
		{
			Vector3 enemyPos = m_prefEnemy.transform.position;
			GameObject obj = Instantiate (m_prefEnemy, new Vector3(Random.Range(0f,10f), enemyPos.y, Random.Range(0f,10f)), Quaternion.Euler (0, 0, 0)) as GameObject;
			obj.GetComponent<Enemy>().SetChamp(m_champ);
		}
	}
	
	// Update is called once per frame
	void Update () {



	}


}
