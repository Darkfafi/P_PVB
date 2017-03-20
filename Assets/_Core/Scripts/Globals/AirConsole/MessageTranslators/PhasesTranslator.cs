using Newtonsoft.Json.Linq;
using NDream.AirConsole;

public class PhasesTranslator : BaseACMessageTranslator
{
    public void UpdateOnCurrentPhase(GamePhase gamePhase, int deviceId)
    {
        var message = new
        {
            action = "gamePhaseUpdate",
            gamePhase = gamePhase.ToString()
        };

        AirConsole.instance.Message(deviceId, message);   
    }

    protected override void MessageReceived(int from, JToken data)
    {
        
    }
}
