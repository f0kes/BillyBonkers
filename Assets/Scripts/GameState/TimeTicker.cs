using System;
using System.Threading.Tasks;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameState
{
	public class TimeTicker : NetworkBehaviour
	{
		public static TimeTicker I;

		public class OnTickEventArgs : EventArgs
		{
			public int Tick;
		}

		public class OnUpdateEventArgs : EventArgs
		{
			public float DeltaTime;
		}

		[SerializeField] private static float _tickRate = 60f;
		public static float TickInterval => 1f / _tickRate;
		private float _currentTickTime = 0f;

		private int _currentTick;
		
		private bool _isPaused = false;
		public int CurrentTick
		{
			get => _currentTick;
			set => _currentTick = value;
		}

		public static event Action<OnTickEventArgs> OnTick;
		public static event Action<OnTickEventArgs> OnTickEnd;
		public static event EventHandler<OnUpdateEventArgs> OnUpdate;

		public async void InvokeInTime(Action toInvoke, float time)
		{
			float timePassed = 0;
			OnUpdate += delegate(object sender, OnUpdateEventArgs args) { timePassed += args.DeltaTime; };
			while (timePassed < time)
			{
				await Task.Yield();
			}

			toInvoke.Invoke();
		}


		private void Awake()
		{
			_currentTick = 0;
			if (I == null)
			{
				I = this;
				DontDestroyOnLoad(this);
				
			}
			else
			{
				Destroy(gameObject);
			}
		}

		public override void OnStartClient()
		{
			base.OnStartClient();
			_currentTick = 0;
			Unpause();
		}

		private void Update()
		{
			if(_isPaused) return;
			_currentTickTime += Time.deltaTime;
			while (_currentTickTime >= TickInterval)
			{
				_currentTickTime -= TickInterval;
				Tick();
			}

			OnUpdate?.Invoke(this, new OnUpdateEventArgs {DeltaTime = Time.deltaTime});
		}

		public void Reset()
		{
			_currentTick = 0;
		}

		public void Pause()
		{
			_isPaused = true;
		}
		public void Unpause()
		{
			_isPaused = false;
		}

		public void Tick(bool simulating = false)
		{
			_currentTick++;
			OnTick?.Invoke(new OnTickEventArgs {Tick = _currentTick});
			OnTickEnd?.Invoke(new OnTickEventArgs {Tick = _currentTick});
			Physics.Simulate(TickInterval);
			if(simulating) return;
			InputSystem.Update();
		}
		public static float TicksToSeconds(int ticks)
		{
			return ticks * TickInterval;
		}
		public static int SecondsToTicks(float seconds)
		{
			return (int) (seconds / TickInterval);
		}
	}
}