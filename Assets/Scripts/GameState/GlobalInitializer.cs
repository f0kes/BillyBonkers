using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Attributes;
using UnityEngine;

namespace GameState
{
	public class GlobalInitializer : MonoBehaviour
	{
		public static IEnumerable<Type> IDTypes { get; private set; }

		private void Awake()
		{
			DontDestroyOnLoad(this);
			IDTypes = AppDomain.CurrentDomain.GetAssemblies() 
				.SelectMany(x => x.GetTypes()) 
				.Where(x => x.IsClass)
				.Where(x => x.GetCustomAttributes(typeof(IDAttribute), true).FirstOrDefault() != null);
			
		}
	}
}