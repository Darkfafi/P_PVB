// Created by | Ramses Di Perna | 08-10-2016

namespace Ramses.StateMachine
{
	public abstract class StateSwitcher<User> where User : class, IStateMachineUser
	{
		protected User effected { get; private set; }
		protected IStateMachine<User> stateMachine { get; private set; }

		protected abstract void SwitcherActivate();
		protected abstract void SwitcherUpdate();
		protected abstract void SwitcherDeactivate();

		public void ActivateSwitcher(User effected, IStateMachine<User> stateMachine)
		{
			this.effected = effected;
			this.stateMachine = stateMachine;
			SwitcherActivate();
		}

		public void UpdateSwitcher()
		{
			SwitcherUpdate();
		}

		public void DeactivateSwitcher()
		{
			SwitcherDeactivate();
			this.effected = null;
			this.stateMachine = null;
		}
	}
}
