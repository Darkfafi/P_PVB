// Created by | Ramses Di Perna | 02-10-2016

namespace Ramses.BehaviourStates
{
	/// <summary>
	/// This class should be inheritted if the inteded class should be a state in the state system.
	/// This state is allowed to manipulate the user given.
	/// If you want to end the state in the state itself, then please use the SelfEndingBehaviourState instead.
	/// </summary>
	public abstract class BehaviourState<T> : IBehaviourState where T : IBehaviourStateUser
	{
		protected T user { get; private set; }

		/// <summary>
		/// This will be called before activated. If this returns false the user will stay in his current state 
		/// </summary>
		public virtual bool IsAbleToActivate(T user)
		{
			return true;
		}
		/// <summary>
		/// Called When the state is activated
		/// </summary>
		public virtual void OnStateStart(T user, BehaviourStateInfo info)
		{
			this.user = user;
		}
		/// <summary>
		/// Called every frame while the state is active
		/// </summary>
		public virtual void OnStateUpdate()
		{

		}
		/// <summary>
		/// Called when the state is deactivated
		/// </summary>
		public virtual void OnStateEnded()
		{

		}
		/// <summary>
		/// This will be called before activated. If this returns false the user will stay in his current state 
		/// </summary>
		public bool IsAbleToActivate(IBehaviourStateUser user)
		{
			return IsAbleToActivate((T)user);
		}
		/// <summary>
		/// Called When the state is activated
		/// </summary>
		public void OnStateStart(IBehaviourStateUser user, BehaviourStateInfo info)
		{
			OnStateStart((T)user, info);
		}
	}

	public interface IBehaviourState
	{
		bool IsAbleToActivate(IBehaviourStateUser user);
		void OnStateStart(IBehaviourStateUser user, BehaviourStateInfo info);
		void OnStateUpdate();
		void OnStateEnded();
	}
}