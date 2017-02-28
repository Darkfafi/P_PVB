// Created by | Ramses Di Perna | 02-10-2016
using System;

namespace Ramses.BehaviourStates
{
	/// <summary>
	/// The SelfEndingBehaviourState is a state which is ended by itself, not specificly by an outside source.
	/// It may be ended by an outside source as the normal BehaviourState.
	/// </summary>
	public abstract class SelfEndingBehaviourState<T> : BehaviourState<T>, ISelfEndingBehaviourState where T : IBehaviourStateUser
	{
		private Type typeToSwitchTo;

		/// <summary>
		/// Used to set the state which this state will switch to after it is finished
		/// </summary>
		public void SetSelfEndingState(Type behaviourType)
		{
			typeToSwitchTo = behaviourType;
		}

		/// <summary>
		///  Called To end itself and switch to the correct behaviourState
		/// </summary>
		protected void SelfEnd()
		{
			user.BehaviourStateHandler.SetState(typeToSwitchTo);
		}
	}

	public interface ISelfEndingBehaviourState
	{
		void SetSelfEndingState(Type behaviourType);
	}
}