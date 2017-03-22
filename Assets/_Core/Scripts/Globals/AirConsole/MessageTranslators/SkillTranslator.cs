using Newtonsoft.Json.Linq;
using UnityEngine;
using Ramses.Confactory;
using NDream.AirConsole;

public delegate void DeviceSkillHandler(int deviceId, Skill skill);

public class SkillTranslator : BaseACMessageTranslator
{
    public event DeviceSkillHandler SkillPickRequestEvent;
    public event DeviceSkillHandler SkillUseRequestEvent;

    public void UpdateSkillsAvailable(int deviceId, params Skill[] skillsAvailable)
    {
        int[] skillIndexList = new int[skillsAvailable.Length];

        for (int i = 0; i < skillIndexList.Length; i++)
        {
            skillIndexList[i] = ConfactoryFinder.Instance.Get<ConSkills>().GetIndexValueOfSkill(skillsAvailable[i]);
        }

        var message = new
        {
            action = "UpdateSkillsAvailable",
            info = new { skillIndexes = CreateParsableString(skillIndexList) }
        };

        //RegisteredPlayer[] registeredPlayers = ConfactoryFinder.Instance.Get<ConPlayers>().GetCurrentlyRegisteredPlayers(true);
        //for(int i = 0; i < registeredPlayers.Length; i++)
        //{
        //    if (registeredPlayers[i].IsConnected)
        //        AirConsole.instance.Message(registeredPlayers[i].DeviceID, message);
        //}
        AirConsole.instance.Message(deviceId, message);
    }

    protected override void MessageReceived(int from, JToken data)
    {
        if (SendEventIfSkillUseMessage(from, data)) { return; }
        if (SendEventIfSkillPickMessage(from, data)) { return; }
    }

    private bool SendEventIfSkillUseMessage(int from, JToken data)
    {
        if(data["skillUseMessage"] != null)
        {
            if (data["skillUseMessage"]["skillIndex"] != null)
            {
                if (SkillUseRequestEvent != null)
                    SkillUseRequestEvent(from, ConfactoryFinder.Instance.Get<ConSkills>().SkillsInOrder[(int)data["skillUseMessage"]["skillIndex"]]);
                return true;
            }
            else
            {
                Debug.LogError("No info given for the 'skillUseMessage'");   
            }
        }
        return false;
    }

    private bool SendEventIfSkillPickMessage(int from, JToken data)
    {
        if (data["skillPickMessage"] != null)
        {
            if (data["skillPickMessage"]["skillIndex"] != null)
            { 
                if (SkillPickRequestEvent != null)
                    SkillPickRequestEvent(from, ConfactoryFinder.Instance.Get<ConSkills>().SkillsInOrder[(int)data["skillPickMessage"]["skillIndex"]]);
                return true;
            }
            else
            {
                Debug.LogError("No info given for the 'skillPickMessage'");
            }
        }
        return false;
    }
}
