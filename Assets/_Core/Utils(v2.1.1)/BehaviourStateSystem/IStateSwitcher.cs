// Created by | Ramses Di Perna | 02-10-2016
using System;

namespace Ramses.BehaviourStates
{
	/// <summary>
	/// A StateSwitcher is the class which controlls in which state the user should and could switch to his next state.
	/// For Example: If the player uses an attack key and he is in a movement state then he should switch to the attacking state.
	/// </summary>
	public abstract class StateSwitcher<T> : System.IStateSwitcher where T : class, IBehaviourStateUser
	{
		/// <summary>
		/// This will be called when the object is created. So here the start default state must be set.
		/// </summary>
		protected abstract void AwakeStateSwitching(T user);

		/// <summary>
		/// This will be called when BehaviourStateHandler.CleanStateHandler() is called. Here everything must be cleaned.
		/// </summary>
		protected abstract void DestroyStateSwitching(T user);

		/// <summary>
		/// WARNING: This method is only supposed to be called by the BehaviourStateHandler
		/// Here you are supposed to write in when to globally switch to which state. 
		/// (when attack key is pressed attack state etc)
		/// </summary>
		protected abstract void GlobalStateSwitching(T user);

		/// <summary>
		/// This method is called to request the state after the given state type. (For SelfEndingBehaviourStates)
		/// Tip: Always give a default state. All requests desire an answer.
		/// </summary>
		public abstract Type GetReturnState(Type behaviourStateType);

		/// <summary>
		/// This method is called by the BehaviourStateHandler to ask permission to go to the state given in the parameter.
		/// </summary>
		public virtual bool GetPermissionForState(Type behaviourStateType)
		{
			return true;
		}

		/// <summary>
		/// This method is called by the BehaviourStateHandler to ask if there is a different State intended for the state it wants to switch to.
		/// </summary>
		public virtual Type GetConvertedState(Type behaviourStateType)
		{
			return behaviourStateType;
		}

		public void AwakeStateSwitching(IBehaviourStateUser user)
		{
			AwakeStateSwitching((T)user);
        }

		public void DestroyStateSwitching(IBehaviourStateUser user)
		{
			DestroyStateSwitching((T)user);
        }

		public void GlobalStateSwitching(IBehaviourStateUser user)
		{
			GlobalStateSwitching((T)user);
        }
	}
}
namespace Ramses.BehaviourStates.System
{
	/// <summary>
	/// WARNING: Inherit from the Class 'StateSwitcher' in the BehaviourState system
	/// A StateSwitcher is the class which controlls in which state the user should and could switch to his next state.
	/// For Example: If the player uses an attack key and he is in a movement state then he should switch to the attacking state.
	/// </summary>
	public interface IStateSwitcher
	{
		/// <summary>
		/// This will be called when the object is created. So here the start default state must be set.
		/// </summary>
		void AwakeStateSwitching(IBehaviourStateUser user);

		/// <summary>
		/// This will be called when BehaviourStateHandler.CleanStateHandler() is called. Here everything must be cleaned.
		/// </summary>
		void DestroyStateSwitching(IBehaviourStateUser user);

		/// <summary>
		/// WARNING: This method is only supposed to be called by the BehaviourStateHandler
		/// Here you are supposed to write in when to globally switch to which state. 
		/// (when attack key is pressed attack state etc)
		/// </summary>
		void GlobalStateSwitching(IBehaviourStateUser user);

		/// <summary>
		/// This method is called to request the state after the given state type. (For SelfEndingBehaviourStates)
		/// Tip: Always give a default state. All requests desire an answer.
		/// </summary>
		Type GetReturnState(Type behaviourStateType);

		/// <summary>
		/// This method is called by the BehaviourStateHandler to ask permission to go to the state given in the parameter.
		/// </summary>
		bool GetPermissionForState(Type behaviourStateType);

		/// <summary>
		/// This method is called by the BehaviourStateHandler to ask if there is a different State intended for the state it wants to switch to.
		/// </summary>
		Type GetConvertedState(Type behaviourStateType);
	}
}
