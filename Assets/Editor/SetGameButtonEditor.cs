using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SetGameButton))]
[CanEditMultipleObjects]
[System.Serializable]
public class SetGameButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        SetGameButton myScript = target as SetGameButton;

        switch (myScript.ButtonType)
        {
            case SetGameButton.EButtonType.PairNumberBtn:
                myScript.PairNumber = (GameSettings.EPairNumber)
                    EditorGUILayout.EnumPopup("Pair Numbers", myScript.PairNumber);
                break;
            case SetGameButton.EButtonType.PussleCategoryBtn:
                myScript.PiuzzlCategories = (GameSettings.EPuzzleCategories)
                    EditorGUILayout.EnumPopup("Puzlle Category", myScript.PiuzzlCategories);
                break;


        }
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
