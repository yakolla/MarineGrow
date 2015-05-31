using UnityEngine;
using System.Collections;
using Newtonsoft.Json;

public class ItemFollowerData : ItemData{

	public string 	m_followerName;
	public int		m_refMobId;
	public RefCreatureBaseProperty	m_baseProperty;
	public MobAIType	m_aiType;

	[JsonConstructor]
	private ItemFollowerData()
	{
	}

	public ItemFollowerData(int refMobId) : base(refMobId-30001+1001, 1)
	{
		RefMob refMob = RefData.Instance.RefMobs[refMobId];
		m_followerName = refMob.prefBody;
		m_refMobId = refMobId;
		m_baseProperty = refMob.baseCreatureProperty;
		m_aiType = refMob.mobAI;
	}

	public string FollowerName
	{
		get{return m_followerName;}
	}

	override public void Pickup(Creature obj)
	{
		base.Pickup(obj);
		Use (obj);
	}

	override public void Use(Creature obj)
	{
		Vector3 enemyPos = obj.transform.position;
		float angle = Random.Range(-3.14f, 3.14f);
		enemyPos.x += Mathf.Cos(angle) * 1f;
		enemyPos.z += Mathf.Sin(angle) * 1f;
		GameObject followerObj = (GameObject)GameObject.Instantiate(Resources.Load<GameObject>("Pref/Follower"), enemyPos, obj.transform.rotation);
		GameObject prefEnemyBody = Resources.Load<GameObject>("Pref/mon_skin/" + m_followerName);
		
		GameObject enemyBody = GameObject.Instantiate (prefEnemyBody, Vector3.zero, Quaternion.Euler (0, 0, 0)) as GameObject;
		enemyBody.name = "Body";
		enemyBody.transform.parent = followerObj.transform;
		enemyBody.transform.localPosition = Vector3.zero;
		enemyBody.transform.localRotation = prefEnemyBody.transform.rotation;
		enemyBody.transform.localScale = prefEnemyBody.transform.localScale;

		Follower follower = (Follower)followerObj.GetComponent<Follower>();

		follower.Init(obj, m_aiType, m_baseProperty, Level);

		RefMob refMob = RefData.Instance.RefMobs[m_refMobId];
		foreach(RefMob.WeaponDesc weaponDesc in refMob.refWeaponItems)
		{
			ItemWeaponData itemWeaponData = new ItemWeaponData(weaponDesc.refItemId, weaponDesc.weaponStat);
			itemWeaponData.Level = Level;
			follower.EquipWeapon(itemWeaponData);
		}

		Const.GetSpawn().AddFollower(follower);
	}

	override public void NoUse(Creature obj)
	{
	}
	
	override public string Description()
	{
		return "<color=white>" + m_followerName + "</color>" + "\n" +  base.Description();
	}

	override public bool Compare(ItemData item)
	{
		if (item.RefItem.type != ItemData.Type.Follower)
			return false;

		ItemFollowerData itemFollowerData = item as ItemFollowerData;
		return m_refMobId == itemFollowerData.m_refMobId;
	}

}
