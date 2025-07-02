using UnityEngine;
using System.Collections.Generic;

public class GameSettings:MonoBehaviour
{
    private readonly Dictionary<EPuzzleCategories, string> _puzzleCatDirectory =
        new Dictionary<EPuzzleCategories, string>();
    private int _settings;
    private const int SettingsNumbers = 2;
    public enum EPairNumber
    {
        NotSet =0,
        E10Pairs=10,
        E15Pairs = 15,
        E20Pairs = 20,
    }
    public enum EPuzzleCategories
    {
        NotSet,
        Fruits,
        Vegetables
    }
    public struct Settings
    {
        public EPairNumber PairNumber;
        public EPuzzleCategories PuzzleCategory;
    }

    private Settings _gameSetting;
    public static GameSettings Instance;
    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(this);
            Instance = this;

        }
        else
        {
            Destroy(this);
        }
        
    }
    private void Start()
    {
        SetPuzzleCategory();
        _gameSetting = new Settings();
        ResetGameSettings();
    }

    private void SetPuzzleCategory()
    {
        _puzzleCatDirectory.Add(EPuzzleCategories.Fruits, "Fruits");
        _puzzleCatDirectory.Add(EPuzzleCategories.Vegetables, "Vegetables");
    }

    public void SetPairNumber(EPairNumber Number)
    {
        if(_gameSetting.PairNumber == EPairNumber.NotSet)
        {
            _settings++;
            _gameSetting.PairNumber = Number;
           
        }
    }

    public void SettPuzzleCategories(EPuzzleCategories cat)
    {
        if(_gameSetting.PuzzleCategory == EPuzzleCategories.NotSet)
        {
            _settings++;
            _gameSetting.PuzzleCategory = cat;
        }
    }
     public EPairNumber GetPairNumber()
    {
        return _gameSetting.PairNumber;
    }
    public EPuzzleCategories GetPuzzleCategory()
    {
        return _gameSetting.PuzzleCategory;
    }

    public void ResetGameSettings()
    {
        _settings = 0;
        _gameSetting.PuzzleCategory = EPuzzleCategories.NotSet;
        _gameSetting.PairNumber = EPairNumber.NotSet;
    }

    public bool AllSettingsReady()
    {
        return _settings == SettingsNumbers;
    }

    public string GetMaterialDirectoryName()
    {
        return "Materials/";
    }
    public string GetPuzzzleCategoryTextureDirectoryName()
    {
        if (_puzzleCatDirectory.ContainsKey(_gameSetting.PuzzleCategory))
        {
            return "Graphics/PuzzleCat/" + _puzzleCatDirectory[_gameSetting.PuzzleCategory] + "/";
        }
        else
        {
            Debug.LogError("Error: cannt get directory name ");
            return "";
        }
    }
}
