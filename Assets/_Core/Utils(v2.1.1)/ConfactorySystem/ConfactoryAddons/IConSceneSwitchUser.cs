// Created by | Ramses Di Perna | 30-09-2016

namespace Ramses.Confactory.Addons
{
	public interface IConSceneSwitchUser
	{
		/// <summary>
		/// WARNING: Don't call this method!
		/// This Method will be called when the Scene is switched.
		/// If you want to listen to the switching of scenes by the ConSceneSwitcher, then listen to its events instead.
		/// </summary>
		void OnConSceneSwitched(string oldScene, string newScene);
	}
}