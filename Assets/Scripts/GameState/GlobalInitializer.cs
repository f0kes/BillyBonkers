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
		public static IEnumerable<MethodInfo> OnTickMethods { get; private set; }

		private void Awake()
		{
			DontDestroyOnLoad(this);
			OnTickMethods = AppDomain.CurrentDomain.GetAssemblies() // Returns all currenlty loaded assemblies
				.SelectMany(x => x.GetTypes()) // returns all types defined in this assemblies
				.Where(x => x.IsClass)
				.SelectMany(x => x.GetMethods()) // returns all methods defined in those classes
				.Where(x => x.GetCustomAttributes(typeof(OnTickAttribute), false).FirstOrDefault() != null);
		}
	}
}