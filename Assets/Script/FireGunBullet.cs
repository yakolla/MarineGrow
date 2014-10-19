using UnityEngine;
using System.Collections;

public class FireGunBullet : Bullet {

	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
	}
	override public void Init(GameObject aimpoint)
	{
		base.Init(aimpoint);
		this.transform.parent = m_aimpoint.transform;
		GameObject pref = Resources.Load<GameObject>("Pref/FireGunBullet");
		this.transform.localPosition = pref.transform.localPosition;
		this.transform.localScale = pref.transform.localScale;
	}

	override public void StopFiring()
	{
		base.StopFiring();
		DestroyObject(this.gameObject);
	}

	void OnTriggerEnter(Collider other) 
	{
		if (other.tag.CompareTo("Enemy") == 0)
		{
			DestroyObject(other.gameObject);
		}
	}
}
