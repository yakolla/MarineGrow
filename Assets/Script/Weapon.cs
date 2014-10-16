using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {

	protected GameObject	m_aimpoint = null;


	virtual public void Init(GameObject aimpoint)
	{
		m_aimpoint = aimpoint;
	}

}
