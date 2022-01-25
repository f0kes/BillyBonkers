using System;
using System.Threading.Tasks;
using UnityEngine;

namespace GameState
{
	public class TimeTicker : MonoBehaviour
	{
		public class OnTickEventArgs : EventArgs
		{
			public int Tick;
		}

		public class OnUpdateEventArgs : EventArgs
		{
			public float DeltaTime;
		}


		public static TimeTicker I;
		private int _tick;

		public static event EventHandler<OnTickEventArgs> OnTick;
		public static event EventHandler<OnTickEventArgs> OnTickEnd;
		public static event EventHandler<OnUpdateEventArgs> OnUpdate;

		public async void InvokeInTime(Action toInvoke, float time)
		{
			float timePassed = 0;
			OnUpdate += delegate(object sender, OnUpdateEventArgs args) { timePassed += args.DeltaTime; };
			while (timePassed<time)
			{
				await Task.Yield();
			}
			toInvoke.Invoke();
		}

       
		private void Awake()
		{
			_tick = 0;
			if (I == null)
			{
				I = this;
			}
			else
			{
				Destroy(gameObject);
			}
		}

		private void Update()
		{
			_tick++;
			OnTick?.Invoke(this, new OnTickEventArgs {Tick = _tick});
			OnTickEnd?.Invoke(this, new OnTickEventArgs {Tick = _tick});
			OnUpdate?.Invoke(this, new OnUpdateEventArgs {DeltaTime = Time.deltaTime});
		}
	}
}