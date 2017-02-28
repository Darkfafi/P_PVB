using Ramses.Confactory;
using UnityEngine.Networking;

public class ConEntityIdHandler : NetworkBehaviour, IConfactory {

    private int entitySpawnCount = 0;

    public void ConClear() {

    }

    public void ResetSpawnCount() {
        entitySpawnCount = 0;
    }

    public int GetUniqueEntityId() {
        return ++entitySpawnCount;
    }
}
