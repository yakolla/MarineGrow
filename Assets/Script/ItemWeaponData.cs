using UnityEngine;
using System.Collections;

[System.Serializable]
public class ItemWeaponData : ItemData{

	GameObject 	m_prefWeapon;
	string 	m_weaponName;

	public ItemWeaponData(int refItemId) : base(refItemId, 1)
	{
		m_prefWeapon = Resources.Load<GameObject>("Pref/Weapon/" + RefItem.codeName);
		m_weaponName = RefItem.codeName;
		Lock = true;
	}

	public GameObject PrefWeapon
	{
		get {return m_prefWeapon;}
	}

	public string WeaponName
	{
		get{return m_weaponName;}
	}

	override public void Use(Creature obj)
	{
		obj.ChangeWeapon(this);

		if (RefItem.evolutionFiring == null)
		{
			obj.m_firingDescs = new Weapon.FiringDesc[1];
			obj.m_firingDescs[0].angle = 0;
			obj.m_firingDescs[0].delayTime = 0;
		}
		else
		{
			obj.m_firingDescs = new Weapon.FiringDesc[this.Evolution*2+1];
			for(int i = 0; i < obj.m_firingDescs.Length; ++i)
			{
				obj.m_firingDescs[i].angle = RefItem.evolutionFiring.angle*((i+1)/2);
				if (i % 2 == 1)
				{
					obj.m_firingDescs[i].angle *= -1;
				}


				obj.m_firingDescs[i].delayTime = RefItem.evolutionFiring.delay*i;

			}
		}

		ApplyOptions(obj);
	}

	override public void NoUse(Creature obj)
	{
		NoApplyOptions(obj);
	}
	
	override public string Description()
	{
		return "<color=white>" + m_weaponName + "</color>" + "\n" +  base.Description();
	}

	override public bool Compare(ItemData item)
	{
		if (item.RefItem.type != ItemData.Type.Weapon)
			return base.Compare(item);

		ItemWeaponData weapon = (ItemWeaponData)item;
		return WeaponName.CompareTo(weapon.WeaponName) == 0;
	}

}
