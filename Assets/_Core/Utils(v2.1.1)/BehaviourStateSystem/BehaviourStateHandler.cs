// Created by | Ramses Di Perna | 02-10-2016
using System.Collections.Generic;
using System;
using System.Collections;

namespace Ramses.BehaviourStates
{
	/// <summary>
	/// The BehaviourStateHandler is the system which switches the IBehaviourStateUser from one state to another.
	/// The full system is based uppon the fact that other states get the IBehaviourStateUser as parameter to manipulate when active.
	/// IMPORTANT:
	/// The CleanStateHandler() should be called when the class is no longer used.
	/// </summary>
	public class BehaviourStateHandler<S> : IBehaviourStateHandler where S : class, System.IStateSwitcher
	{
		public delegate void BehaviourStateHandlerStateHandler(IBehaviourStateUser User, Type oldStateType, Type newStateType);
		/// <summary>
		/// Called when the user switches from one state to another.
		/// Note: This wont be called when the user switches from a 'null' state.
		/// </summary>
		public event BehaviourStateHandlerStateHandler StateSwitchedEvent;

		/// <summary>
		/// The User linked to the BehaviourStateHandler
		/// </summary>
		public IBehaviourStateUser User { get; private set; }
		private S switcher;

		private IBehaviourState currentState = null;
		private Dictionary<Type, IBehaviourState> previousActivatedStates = new Dictionary<Type, IBehaviourState>();

		/// <summary>
		/// The User is the object which will be effected by the states.
		/// The Switcher is the class which decides which state will be set when.
		/// </summary>
		public BehaviourStateHandler(IBehaviourStateUser user)
		{
			User = user;
			this.switcher = Activator.CreateInstance<S>();
			object context = new object();
			Confactory.ConfactoryFinder.Instance.Get<ConCoroutines>().StartCoroutine(SwitcherAwakeCall(context), context);
		}

		~BehaviourStateHandler()
		{
			switcher = null;
		}

		/// <summary>
		/// This method sets a new state for the BehaviourStateUser.
		/// The parameters given are info components which will be catched by the given state. This state can use them if it so desires.
		/// If the method returns 'true', the state has been set to the current state.
		/// It will always keep a reference to the state and reuse it when called again until the CleanStateHandler() is called.
		/// </summary>
		public bool SetState<T>(params IStateInfoPart[] stateInfoParts) where T : IBehaviourState
		{
			return SetState(typeof(T), stateInfoParts);
		}

		/// <summary>
		/// This method sets a new state for the BehaviourStateUser.
		/// The parameters given are info components which will be catched by the given state. This state can use them if it so desires.
		/// If the method returns 'true', the state has been set to the current state.
		/// It will always keep a reference to the state and reuse it when called again until the CleanStateHandler() is called.
		/// </summary>
		public bool SetState(Type stateType, params IStateInfoPart[] stateInfoParts)
		{
			ISelfEndingBehaviourState selfEndingState;
			IBehaviourState previousState = currentState;

			stateType = switcher.GetConvertedState(stateType);

			if(!switcher.GetPermissionForState(stateType))
			{
				return false;
			}

            if (!previousActivatedStates.ContainsKey(stateType))
			{
				previousActivatedStates.Add(stateType, (IBehaviourState)Activator.CreateInstance(stateType));
			}

			if (!previousActivatedStates[stateType].IsAbleToActivate(User))
			{
				return false;
			}
			
			selfEndingState = previousActivatedStates[stateType] as ISelfEndingBehaviourState;
			
			if (selfEndingState != null)
			{
				selfEndingState.SetSelfEndingState(switcher.GetReturnState(stateType));
			}

			RemoveCurrentState();
			currentState = previousActivatedStates[stateType];
			currentState.OnStateStart(User, new BehaviourStateInfo(stateInfoParts));

			if(previousState != null && StateSwitchedEvent != null)
			{
				StateSwitchedEvent(User, previousState.GetType(), currentState.GetType());
            }

			return true;
		}

		/// <summary>
		/// This method will return true if the current state is equal to the given state type, else false.
		/// </summary>
		public bool IsInState<T>() where T : IBehaviourState
		{
			return IsInState(typeof(T));
        }

		/// <summary>
		/// This method will return true if the current state is equal to the given state type, else false.
		/// </summary>
		public bool IsInState(Type behaviourStateType)
		{
			if (currentState.GetType() == behaviourStateType)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Returns the type of the current state.
		/// If there is no current state, this method returns null.
		/// </summary>
		/// <returns></returns>
		public Type GetCurrentStateType()
		{
			if(currentState != null)
			{
				return currentState.GetType();
			}
			return null;
		}

		/// <summary>
		/// This method updates the current state and the IStateSwitcher
		/// </summary>
		public void UpdateState()
		{
			if (switcher != null)
			{
				switcher.GlobalStateSwitching(User);
			}

			if (currentState != null)
			{
				currentState.OnStateUpdate();
			}
		}

		/// <summary>
		/// This will call the IStateSwitcher.DestroyStateSwitching(); and will remove the current state.
		/// Also this will clean the history of all activated states.
		/// </summary>
		public void CleanStateHandler()
		{
			RemoveCurrentState();
			switcher.DestroyStateSwitching(User);
			previousActivatedStates.Clear();
			currentState = null;
		}

		private void RemoveCurrentState()
		{
			if (currentState != null)
			{
				currentState.OnStateEnded();
				currentState = null;
			}
		}

		private IEnumerator SwitcherAwakeCall(object context)
		{
			yield return null;
			switcher.AwakeStateSwitching(User);
			Confactory.ConfactoryFinder.Instance.Get<ConCoroutines>().StopContext(context);
		}
	}

	public interface IBehaviourStateHandler
	{
		bool SetState<T>(params IStateInfoPart[] stateInfoParts) where T : IBehaviourState;
		bool SetState(Type stateType, params IStateInfoPart[] stateInfoParts);
		void UpdateState();
		bool IsInState(Type behaviourStateType);
		bool IsInState<T>() where T : IBehaviourState;
		Type GetCurrentStateType();
		void CleanStateHandler();
	}
}