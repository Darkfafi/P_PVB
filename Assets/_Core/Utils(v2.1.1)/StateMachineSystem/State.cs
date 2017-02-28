// Created by | Ramses Di Perna | 08-10-2016

using System;
using System.Collections.Generic;
namespace Ramses.StateMachine
{
	public abstract class State<User> where User : class, IStateMachineUser
	{
		public bool Active { get; private set; }
		protected User effected { get; private set; }
		protected IStateMachine<User> stateMachine { get; private set; }

		private SwitcherManager<User> stateSwitcherManager = new SwitcherManager<User>(); 

		private StateInfo lastStateInfo = new StateInfo();

		public virtual bool IsAbleToActivate(User user)
		{
			return true;
		}

		protected abstract void StateStart(StateInfo info);
		protected abstract void StateUpdate();
		protected abstract void StateEnd();

		public State()
		{
			stateSwitcherManager = new SwitcherManager<User>();
		}

		public void ActivateState(User effected, IStateMachine<User> stateMachine)
		{
			ActivateState(effected, stateMachine, lastStateInfo);
        }

		public void ActivateState(User effected, IStateMachine<User> stateMachine, StateInfo info)
		{
			Active = true;
			lastStateInfo = info;
			this.effected = effected;
			this.stateMachine = stateMachine;
			stateSwitcherManager.ActivateSwitcher(effected, stateMachine);
			StateStart(info);
		}

		public void UpdateState()
		{
			StateUpdate();
			stateSwitcherManager.SwitcherUpdate();
		}

		public void DeactivateState()
		{
			Active = false;
			StateEnd();
			stateSwitcherManager.DeactivateSwitcher();
			effected = null;
			stateMachine = null;
		}

		public void AddStateSwitcher<Switcher>() where Switcher : StateSwitcher<User>, new()
		{
			if (!stateMachine.HasAnyStateSwitcher<Switcher>())
			{
				stateSwitcherManager.AddSwitcher<Switcher>();
			}
			else
			{
				stateSwitcherManager.RemoveSwitcher<Switcher>();
				UnityEngine.Debug.LogError("Switcher of type: '" + typeof(Switcher).ToString() + "' Already set as ANY state switcher. Current State: '" + this.GetType() + "'. {Please remove the switcher at one of the 2 locations}");
			}
		}

		public void RemoveStateSwitcher<Switcher>() where Switcher : StateSwitcher<User>, new()
		{
			stateSwitcherManager.RemoveSwitcher<Switcher>();
		}

		public bool HasStateSwitcher(Type switcherType)
		{
			return stateSwitcherManager.HasSwitcher(switcherType);
		}

		public bool HasStateSwitcher<Switcher>() where Switcher : StateSwitcher<User>, new()
		{
			return HasStateSwitcher(typeof(Switcher));
		}
	}
}