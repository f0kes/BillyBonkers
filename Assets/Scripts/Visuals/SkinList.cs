using System;
using System.Collections.Generic;
using UnityEngine;

namespace Visuals
{
	public class SkinList : MonoBehaviour
	{
		public static SkinList I;
		public  List<Skin> Skins;

		private void Awake()
		{
			if(I!=null)
				Destroy(this);
			else
				I = this;
		}

		private void Start()
		{
				
		}

		public  Skin GetSkin(int index)
		{
			return Skins[index];
		}
	}
}