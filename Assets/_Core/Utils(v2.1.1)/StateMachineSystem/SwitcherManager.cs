using UnityEngine;
using System.Collections.Generic;
using Ramses.StateMachine;
using System;

public class SwitcherManager<User> where User : class, IStateMachineUser
{
	public bool Active { get; protected set; }
	protected User effected { get; private set; }
	protected IStateMachine<User> stateMachine { get; private set; }

	private Dictionary<Type, StateSwitcher<User>> stateSwitchers = new Dictionary<Type, StateSwitcher<User>>();

	public void ActivateSwitcher(User effected, IStateMachine<User> stateMachine)
	{
		Active = true;
		this.effected = effected;
		this.stateMachine = stateMachine;
		foreach (KeyValuePair<Type, StateSwitcher<User>> switcherPair in stateSwitchers)
		{
			switcherPair.Value.ActivateSwitcher(effected, stateMachine);
		}
	}

	public void DeactivateSwitcher()
	{
		Active = false;
		foreach (KeyValuePair<Type, StateSwitcher<User>> switcherPair in stateSwitchers)
		{
			switcherPair.Value.DeactivateSwitcher();
		}
		effected = null;
		stateMachine = null;
	}

	public void SwitcherUpdate()
	{
		if (Active)
		{
			foreach (KeyValuePair<Type, StateSwitcher<User>> switcherPair in stateSwitchers)
			{
				switcherPair.Value.UpdateSwitcher();
			}
		}
	}

	public void AddSwitchers(params Type[] switcherTypes)
	{
		for(int i = 0; i < switcherTypes.Length; i++)
		{
			AddSwitcher(switcherTypes[i]);
		}
	}

	public void AddSwitcher(Type switcherType)
	{
		if (!HasSwitcher(switcherType))
		{
			StateSwitcher<User> s = (StateSwitcher<User>)Activator.CreateInstance(switcherType);
			stateSwitchers.Add(switcherType, s);
			if (Active)
				s.ActivateSwitcher(effected, stateMachine);
		}
	}

	public void AddSwitcher<Switcher>() where Switcher : StateSwitcher<User>, new()
	{
		AddSwitcher(typeof(Switcher));
	}

	public void ClearSwitcher()
	{
		foreach (KeyValuePair<Type, StateSwitcher<User>> pair in stateSwitchers)
		{
			if (Active)
				pair.Value.DeactivateSwitcher();
		}
		stateSwitchers.Clear();
	}

	public void RemoveSwitcher<Switcher>() where Switcher : StateSwitcher<User>, new()
	{
		if (HasSwitcher<Switcher>())
		{
			if (Active)
				stateSwitchers[typeof(Switcher)].DeactivateSwitcher();

			stateSwitchers.Remove(typeof(Switcher));
		}
	}

	public bool HasSwitcher<Switcher>() where Switcher : StateSwitcher<User>, new()
	{
		return HasSwitcher(typeof(Switcher));
	}

	public bool HasSwitcher(Type switcherType)
	{
		return stateSwitchers.ContainsKey(switcherType);
	}
}
