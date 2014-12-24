using UnityEngine;
using System.Collections;

[System.Serializable]
public class ItemFollowerData : ItemData{

	GameObject 	m_prefFollower;
	string 	m_followerName;

	public ItemFollowerData(int refItemId) : base(refItemId, 1)
	{
		m_prefFollower = Resources.Load<GameObject>("Pref/" + RefItem.codeName);
		m_followerName = RefItem.codeName;
	}

	public GameObject PrefFollower
	{
		get {return m_prefFollower;}
	}

	public string FollowerName
	{
		get{return m_followerName;}
	}

	override public void Use(Creature obj)
	{
		GameObject.Instantiate(m_prefFollower, obj.transform.position, obj.transform.rotation);
		Warehouse.Instance.PullItem(RefItem.id, 1);
	}

	override public void NoUse(Creature obj)
	{
	}
	
	override public string Description()
	{
		return m_followerName + "\n" +  base.Description();
	}

	override public bool Compare(ItemData item)
	{
		if (item.RefItem.type != ItemData.Type.Weapon)
			return base.Compare(item);

		ItemWeaponData weapon = (ItemWeaponData)item;
		return FollowerName.CompareTo(weapon.WeaponName) == 0;
	}

}
