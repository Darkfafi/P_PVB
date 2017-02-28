using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SpawnPopUpByButtonPress : MonoBehaviour
{
    [SerializeField]
    private BasePopUp popUpToSpawn;

    [SerializeField]
    private bool coverPopUp = true;

    [SerializeField]
    private int layer = 0;

    private Button buttonListeningTo;

	protected void Awake()
    {
        buttonListeningTo = gameObject.GetComponent<Button>();
        buttonListeningTo.onClick.AddListener(OnClick);
    }

    protected void OnClick()
    {
        PopUpSystem.Instance.CreatePopUp(popUpToSpawn, coverPopUp, layer);
    }

    protected void OnDestroy()
    {
        buttonListeningTo.onClick.RemoveListener(OnClick);
    }
}
