using UnityEngine;
using UnityEngine.SceneManagement;

public class SetGameButton : MonoBehaviour
{
    public enum EButtonType
    {
        NotSet,
        PairNumberBtn,
        PussleCategoryBtn,
    };
    [SerializeField] public EButtonType ButtonType = EButtonType.NotSet;
    [HideInInspector] public GameSettings.EPairNumber PairNumber = GameSettings.EPairNumber.NotSet;
    [HideInInspector] public GameSettings.EPuzzleCategories PiuzzlCategories = GameSettings.EPuzzleCategories.NotSet;

    public void SetGameOption(string GameSeceneName)
    {
        var comp = gameObject.GetComponent<SetGameButton>();
        switch (comp.ButtonType)
        {
            case SetGameButton.EButtonType.PairNumberBtn:
                GameSettings.Instance.SetPairNumber(comp.PairNumber);
                break;

            case EButtonType.PussleCategoryBtn:
                GameSettings.Instance.SettPuzzleCategories(comp.PiuzzlCategories);
                break;
        }
        if (GameSettings.Instance.AllSettingsReady())
        {
            SceneManager.LoadScene(GameSeceneName);
        }
    }

}
