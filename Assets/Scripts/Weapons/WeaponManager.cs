﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponManager : MonoBehaviour , IWeaponManager
{
	public Transform weaponBone;

	static Dictionary<int, IWeapon> weapons = new Dictionary<int, IWeapon>();

	Dictionary<int, IWeapon> allWeapons = new Dictionary<int, IWeapon>();

	IWeapon curWeapon;

	public IWeapon currentWeapon
	{
		get
		{
			return curWeapon;
		}
		set
		{
			curWeapon = value;
		}
	}

	void Start()
	{
		if(weaponBone == null)
		{
			// TODO Don't know if it is the best way
			Debug.LogError("Weapon bone not found");
		}

		for(int i = 0; i < weaponBone.childCount; i++)
		{
			Transform child = weaponBone.GetChild(i);

			IWeapon weapon = child.GetComponent<IWeapon>();
			if(weapon != null)
			{
				allWeapons.Add(weapon.slot, weapon);
			}
		}
	}

	public void ChangeWeapon(int weaponNumber)
	{
		IWeapon selectedWeapon;

		if(weapons.TryGetValue(weaponNumber, out selectedWeapon))
		{
			currentWeapon.setActive(false);
			weapons[weaponNumber].setActive(true);
			currentWeapon = weapons[weaponNumber];
		}


	}
	// Doesn't work, I must first put all weapons on the prefab, map them and then use this to find the weapon for the slot and make it active
	public void AddWeapon(Transform weapon)
	{
		IWeapon weaponFound;
		IWeapon weaponReference;
		IWeapon pickedWeapon = weapon.GetComponent<IWeapon>();

		if(pickedWeapon == null)
		{
			return;
		}

		if(allWeapons.TryGetValue(pickedWeapon.slot, out weaponReference))
		{
			if(!weapons.TryGetValue(pickedWeapon.slot, out weaponFound))
			{
				weapons.Add(pickedWeapon.slot, weaponReference);
			}
			else
			{
				// Pick as Ammo
			}
		}

		if(curWeapon == null)
		{
			curWeapon = weaponReference;
			curWeapon.setActive(true);
		}
		else
		{
			weaponReference.setActive(false);
		}

	}
}