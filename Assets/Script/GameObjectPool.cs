using UnityEngine;
using System.Collections.Generic;

public class GameObjectPool {

	Dictionary< GameObject, List<GameObject> > m_freeGameObject = new Dictionary< GameObject, List<GameObject> >();
	Dictionary< GameObject, GameObject > m_allocGameObject = new Dictionary< GameObject, GameObject >();

	static GameObjectPool m_ins = null;
	static public GameObjectPool Instance
	{
		get {
			if (m_ins == null)
			{
				m_ins = new GameObjectPool();
			}
			
			return m_ins;
		}
	}

	public GameObject	Alloc(GameObject pref, Vector3 pos, Quaternion rotation)
	{
		GameObject obj = null;
		if (m_freeGameObject.ContainsKey(pref))
		{
			int count = m_freeGameObject[pref].Count;

			if (count > 0)
			{
				obj = m_freeGameObject[pref][count-1];
				m_freeGameObject[pref].RemoveAt(count-1);
			}
		}

		if (obj == null)
		{
			obj = GameObject.Instantiate (pref, pos, rotation) as GameObject;
		}

		m_allocGameObject.Add(obj, pref);

		obj.transform.position = pos;
		obj.transform.rotation = rotation;
		obj.SetActive(true);
		return obj;
	}

	public void		Free(GameObject obj)
	{
		if (m_allocGameObject.ContainsKey(obj))
		{
			GameObject pref = m_allocGameObject[obj];
			if (false == m_freeGameObject.ContainsKey(pref))
			{
				m_freeGameObject.Add(pref, new List<GameObject>());
			}

			m_freeGameObject[pref].Add(obj);

			obj.SetActive(false);

			m_allocGameObject.Remove(obj);
		}
	}
}