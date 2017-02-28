// Created by | Ramses Di Perna | 08-10-2016

using System;
using System.Collections.Generic;

namespace Ramses.StateMachine
{
	public class StateMachine<User, DefaultState> : IStateMachine<User> where User : class, IStateMachineUser where DefaultState : State<User>
	{
		private User effected = null;

		private Type defaultStateType = null;
		private Type previousStateType = null;
		private State<User> currentState = null;

		private StateInfo currentStateInfo = new StateInfo();
		private StateInfo previousStateInfo = new StateInfo();

		private Dictionary<Type, State<User>> statePool = new Dictionary<Type, State<User>>();

		private SwitcherManager<User> anyStateSwitcherManager = new SwitcherManager<User>();
		private List<Type> failedAnyStateSwitcherPool = new List<Type>();

		public StateMachine(User effected)
		{
			this.effected = effected;
			anyStateSwitcherManager.ActivateSwitcher(effected, this);
			SetNewDefaultState<DefaultState>(true);
		}

		public bool SwitchToState<State>(params StateInfoParameter[] parameters) where State : State<User>
		{
			return SwitchToState<State>(new StateInfo(parameters));
		}

		public bool SwitchToState<State>(StateInfo info) where State : State<User>
		{
			return SwitchToState(typeof(State), info);
		}

		public bool SwitchToDefaultState(params StateInfoParameter[] parameters)
		{
			if (defaultStateType != null)
			{
				return SwitchToState(defaultStateType, new StateInfo(parameters));
			}
			UnityEngine.Debug.LogError("Default State == Null");
			return false;
		}

		public bool SwitchToPreviousState(bool whenUnableSwitchToDefault = true, bool allowSameState = false)
		{
			bool able = previousStateType != null;
			if (able)
			{
				if (previousStateType == currentState.GetType())
				{
					if (!allowSameState)
					{
						able = false;
						if(!whenUnableSwitchToDefault)
							UnityEngine.Debug.LogWarning("Previous State == Current State & No permission to switch to default state!");
					}
				}
			}

			if(!able)
			{
				if(whenUnableSwitchToDefault)
					return SwitchToDefaultState();

				return false;
			}
			if(previousStateType != null)
				return SwitchToState(previousStateType);

			UnityEngine.Debug.LogError("Previous State == Null!");
			return false;
		}

		public void Update()
		{
			if (currentState != null && currentState.Active)
			{
				currentState.UpdateState();
				anyStateSwitcherManager.SwitcherUpdate();
			}
		}

		public StateInfo GetCurrentStateInfo()
		{
			return currentStateInfo;
		}

		public StateInfo GetPreviousStateInfo()
		{
			return previousStateInfo;
		}

		public bool IsInState<State>() where State : State<User>
		{
			return currentState.GetType() == typeof(State);
		}

		public bool WasInState<State>() where State : State<User>
		{
			if(previousStateType == null)
				return false;
			
			return previousStateType == typeof(State);
		}

		public void SetNewDefaultState<State>(bool switchToState = false) where State : State<User>
		{
			defaultStateType = typeof(State);
			if (switchToState)
				SwitchToDefaultState();
		}

		public void DestroyStateMachine()
		{
			currentStateInfo = new StateInfo();
			previousStateInfo = new StateInfo();
			currentState = null;
			previousStateType = defaultStateType = null;
			anyStateSwitcherManager.DeactivateSwitcher();
			anyStateSwitcherManager = null;
			CleanStateMachinePool();
			statePool = null;
			this.effected = null;
		}

		public void AddAnyStateSwitcher<Switcher>() where Switcher : StateSwitcher<User>, new()
		{
			AddAnyStateSwitcher(typeof(Switcher));
		}

		public void RemoveAnyStateSwitcher<Switcher>() where Switcher : StateSwitcher<User>, new()
		{
			anyStateSwitcherManager.RemoveSwitcher<Switcher>();
		}

		public bool HasAnyStateSwitcher<Switcher>() where Switcher : StateSwitcher<User>, new()
		{
			return anyStateSwitcherManager.HasSwitcher<Switcher>();
		}

		public void CleanStateMachinePool()
		{
			statePool.Clear();
		}

		private bool SwitchToState(Type stateType, StateInfo? info = null)
		{
			if (!statePool.ContainsKey(stateType))
			{
				statePool.Add(stateType, (State<User>)Activator.CreateInstance(stateType));
			}

			if (!statePool[stateType].IsAbleToActivate(effected))
			{
				return false;
			}

			if (currentState != null)
			{
				previousStateType = currentState.GetType();
				previousStateInfo = currentStateInfo;
			}

			DeactivateCurrentState();
			currentState = statePool[stateType];

			if (info.HasValue)
			{
				currentStateInfo = info.Value;
				currentState.ActivateState(effected, this, info.Value);
			}
			else
			{
				currentState.ActivateState(effected, this);
			}
			AddFailedSwitcher();
			return true;
		}

		private void DeactivateCurrentState()
		{
			if (currentState != null)
			{
				currentState.DeactivateState();
				currentState = null;
			}
		}

		private void AddFailedSwitcher()
		{
			Type[] types = failedAnyStateSwitcherPool.ToArray();
			failedAnyStateSwitcherPool.Clear();
			for(int i = 0; i < types.Length; i++)
			{
				AddAnyStateSwitcher(types[i]);
			}
		}

		private void AddAnyStateSwitcher(Type switcherType)
		{
			if (currentState == null || !currentState.HasStateSwitcher(switcherType))
			{
				anyStateSwitcherManager.AddSwitcher(switcherType);
			}
			else
			{
				failedAnyStateSwitcherPool.Add(switcherType);
				UnityEngine.Debug.LogError("Switcher of type: '" + switcherType.ToString() + "' Already set as CURRENT state switcher. Current State: '" + currentState.ToString() + "'. {Please remove the switcher at one of the 2 locations}");
			}
		}
	}

	public interface IStateMachine<User> where User : class, IStateMachineUser
	{
		bool SwitchToState<State>(params StateInfoParameter[] parameters) where State : State<User>;
		bool SwitchToState<State>(StateInfo info) where State : State<User>;
		bool SwitchToDefaultState(params StateInfoParameter[] parameters);
		bool SwitchToPreviousState(bool whenUnableSwitchToDefault = true, bool allowSameState = false);
		bool IsInState<State>()		where State : State<User>;
		bool WasInState<State>()	where State : State<User>;
		void Update();
		void SetNewDefaultState<State>(bool switchToState = false) where State : State<User>;
		void DestroyStateMachine();
		void CleanStateMachinePool();
		void AddAnyStateSwitcher<Switcher>()	where Switcher : StateSwitcher<User>, new();
		void RemoveAnyStateSwitcher<Switcher>() where Switcher : StateSwitcher<User>, new();
		bool HasAnyStateSwitcher<Switcher>()	where Switcher : StateSwitcher<User>, new();
		StateInfo GetCurrentStateInfo();
		StateInfo GetPreviousStateInfo();
	}
}