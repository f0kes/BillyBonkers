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
			public bool Simulating;
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

		public static event Action<OnTickEventArgs> OnTickStart;
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
			SyncTick();
			Unpause();
		}

		public override void OnStopServer()
		{
			base.OnStopServer();
			SyncTick();
		}

		private void Update()
		{
			if (_isPaused) return;
			_currentTickTime += Time.deltaTime;
			while (_currentTickTime >= TickInterval)
			{
				_currentTickTime -= TickInterval;
				Tick();
			}

			OnUpdate?.Invoke(this, new OnUpdateEventArgs {DeltaTime = Time.deltaTime});
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
			OnTickStart?.Invoke(new OnTickEventArgs {Tick = _currentTick, Simulating = simulating});
			OnTick?.Invoke(new OnTickEventArgs {Tick = _currentTick, Simulating = simulating});

			Physics.Simulate(TickInterval);
			if (!simulating)
			{
				InputSystem.Update();
			}

			OnTickEnd?.Invoke(new OnTickEventArgs {Tick = _currentTick, Simulating = simulating});
		}
		
		public void SyncTick()
		{
			double now = NetworkTime.time;
			int tick = (int)Math.Floor((now / TickInterval));
			_currentTick = tick;
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