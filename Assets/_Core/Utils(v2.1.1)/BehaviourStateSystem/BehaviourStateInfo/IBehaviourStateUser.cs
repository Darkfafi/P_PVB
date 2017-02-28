// Created by | Ramses Di Perna | 02-10-2016

namespace Ramses.BehaviourStates
{
	/// <summary>
	/// This is a tag which StateUsers should inherit if they want to be stated as a BehaviourStateUser.
	/// </summary>
	public interface IBehaviourStateUser
	{
		IBehaviourStateHandler BehaviourStateHandler { get; }
    }
}
