using System;
using UnityEngine;

namespace Utils
{
	public class ConsoleToGUI : MonoBehaviour
	{
		//#if !UNITY_EDITOR
		static string m_myLog = "";
		private string _output;
		private string _stack;
		private bool _show = false;

		void OnEnable()
		{
			Application.logMessageReceived += Log;
		}

		void OnDisable()
		{
			Application.logMessageReceived -= Log;
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.F1))
			{
				_show = !_show;
			}
		}

		private void Log(string logString, string stackTrace, LogType type)
		{
			_output = logString;
			_stack = stackTrace;
			m_myLog = _output + "\n" + m_myLog;
			if (m_myLog.Length > 5000)
			{
				m_myLog = m_myLog.Substring(0, 4000);
			}
		}

		private void OnGUI()
		{
			if (_show)
			{
				m_myLog = GUI.TextArea(new Rect(10, 10, Screen.width - 10, Screen.height - 10), m_myLog);
			}
		}
		//#endif
	}
}