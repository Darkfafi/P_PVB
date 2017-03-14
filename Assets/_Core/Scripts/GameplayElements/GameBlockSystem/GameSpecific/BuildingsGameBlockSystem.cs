
public class BuildingsGameBlockSystem : BaseGameBlockSystem<BuildingsGame>
{
    public BuildingsGameBlockSystem(BuildingsGame gameInstance, ScriptableGameBlock[] buildingBlocks) : base(gameInstance, buildingBlocks)
    {

    }

    public BuildingsGameBlockSystem(BuildingsGame gameInstance, IGameBlock<BuildingsGame>[] buildingBlocks) : base(gameInstance, buildingBlocks)
    {

    }
}
