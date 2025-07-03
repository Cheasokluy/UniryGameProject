using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PicturManager :MonoBehaviour 
{
    public Picture PicturePrefab;
    [Space]
    [Header("End Game nScreen")]
    public GameObject EndGamePanel;
    public GameObject NewBestScoreText;
    public GameObject YourScoreText;
    public GameObject EndTimeText;

    public Transform PicSpawnPosition;
    public enum GameState { NoAction, MovingOnPosition, DeletingPuzzles, FlipBack, Checkign, GameEnd };
    public enum PuzzleState
    {
        PuzzleRotating,
        CanRotate
    };
    public enum RevealedState
    {
        NoRevealed,
        OneRevealed,
        TwoRevealed
    };
    [HideInInspector]
    public GameState CurrentGameState;
    [HideInInspector]
    public PuzzleState CurrentPuzzleState;
    [HideInInspector]
    public RevealedState PuzzleRevealedNumber;

    [HideInInspector]
    public List<Picture> PictureList;
    public Vector2 StartPosition = new Vector2(-2.15f, 3.62f);
    
    private Vector2 _offset = new Vector2(1.5f, 1.52f);
    private Vector2 _offsetFor15Pairs = new Vector2(1.08f, 1.22f);
    private Vector2 _offSetFor20Pairs = new Vector2(1.08f, 1.0f);
    private Vector3 _newScaleDown = new Vector3(0.9f, 0.9f, 0.001f);

    private List<Material> _materialList = new List<Material>();
    private List<string> _texturePathList = new List<string>();
    private Material _firstMaterial;
    private string _firstTexturePath;

    private int _firstRevealedPic;
    private int _secondRevealedPic;
    private int _revealedPicNumber = 0;
    private int _picToDestroy1;
    private int _picToDestroy2;

    private bool _corutineStarted = false;

    private int _pairNumbers;
    private int _removedPairs;
    private Timer _gameTimer;
    


    private void Start()
    {

        CurrentGameState = GameState.NoAction;
        CurrentPuzzleState = PuzzleState.CanRotate;
        PuzzleRevealedNumber = RevealedState.NoRevealed;
        _revealedPicNumber = 0;
        _firstRevealedPic = -1;
        _secondRevealedPic = -1;

        _removedPairs = 0;
        _pairNumbers = (int)GameSettings.Instance.GetPairNumber();

        _gameTimer = GameObject.Find("Main Camera").GetComponent<Timer>();

        LoadMaterials();

        if (GameSettings.Instance.GetPairNumber()==GameSettings.EPairNumber.E10Pairs)
        {
            CurrentGameState = GameState.MovingOnPosition;
            SpwanPictureMesh(4, 5, StartPosition, _offset, false);
            MovePicture(4, 5, StartPosition, _offset);
        }
        else if (GameSettings.Instance.GetPairNumber() == GameSettings.EPairNumber.E15Pairs)
        {
            CurrentGameState = GameState.MovingOnPosition;
            SpwanPictureMesh(5, 6, StartPosition, _offset, false);
            MovePicture(5, 6, StartPosition, _offsetFor15Pairs);
        }
        else if (GameSettings.Instance.GetPairNumber() == GameSettings.EPairNumber.E20Pairs)
        {
            CurrentGameState = GameState.MovingOnPosition;
            SpwanPictureMesh(5, 8, StartPosition, _offset, true);
            MovePicture(5, 8, StartPosition, _offSetFor20Pairs);
        }

    }
    private void Update()
    {
        if(CurrentGameState == GameState.DeletingPuzzles)
        {
            if(CurrentPuzzleState == PuzzleState.CanRotate)
            {
                DestroyPicture();
                CheckGameEnd();
            }
        }
        if(CurrentGameState == GameState.FlipBack)
        {
            if(CurrentPuzzleState == PuzzleState.CanRotate && _corutineStarted == false)
            {
                StartCoroutine(FlipBack());
            }
        }

        if(CurrentGameState == GameState.GameEnd)
        {
            if(PictureList[_firstRevealedPic].gameObject.activeSelf == false &&
                PictureList[_secondRevealedPic].gameObject.activeSelf == false &&
                EndGamePanel.activeSelf == false)
            {
                ShowEndGameInformation();
            }
        }
    }

    private void ShowEndGameInformation()
    {
        EndGamePanel.SetActive(true);
        YourScoreText.SetActive(true);

        var timer = _gameTimer.GetCurrentTime();
        var minutes = Mathf.Floor(timer / 60);
        var seconds = Mathf.RoundToInt(timer % 60);
        var newText = minutes.ToString("00") + ":" + seconds.ToString("00");
        EndTimeText.GetComponent<Text>().text = newText;
    }

    private bool CheckGameEnd()
    {
        if(_removedPairs == _pairNumbers && CurrentGameState != GameState.GameEnd)
        {
            CurrentGameState = GameState.GameEnd;
            _gameTimer.StopTimer();
        }
        return (CurrentGameState == GameState.GameEnd);
    }

    public void CheckPicture()
    {
        CurrentGameState = GameState.Checkign;
        _revealedPicNumber = 0;
        for(int id = 0; id < PictureList.Count; id++)
        {
            if(PictureList[id].Revealed && _revealedPicNumber < 2)
            {
                if(_revealedPicNumber == 0)
                {
                    _firstRevealedPic = id;
                    _revealedPicNumber++;
                }else if (_revealedPicNumber == 1)
                {
                    _secondRevealedPic = id;
                    _revealedPicNumber++;
                }
            }
        }

        if(_revealedPicNumber == 2)
        {
            if(PictureList[_firstRevealedPic].GetIndex() == PictureList[_secondRevealedPic].GetIndex()&& _firstRevealedPic != _secondRevealedPic)
            {
                CurrentGameState = GameState.DeletingPuzzles;
                _picToDestroy1 = _firstRevealedPic;
                _picToDestroy2 = _secondRevealedPic;
            }
            else
            {
                CurrentGameState = GameState.FlipBack;
            }
            
        }
        CurrentPuzzleState = PicturManager.PuzzleState.CanRotate;
        if(CurrentGameState == GameState.Checkign)
        {
            CurrentGameState = GameState.NoAction;
        }
    }

    private void DestroyPicture()
    {
        PuzzleRevealedNumber = RevealedState.NoRevealed;
        PictureList[_picToDestroy1].Deactivate();
        PictureList[_picToDestroy2].Deactivate();
        _revealedPicNumber = 0;
        _removedPairs++;
        CurrentGameState = GameState.NoAction;
        CurrentPuzzleState = PuzzleState.CanRotate;
    }

    private IEnumerator FlipBack()
    {
        _corutineStarted = true;
        yield return new WaitForSeconds(0.5f);

        PictureList[_firstRevealedPic].FlipBack();
        PictureList[_secondRevealedPic].FlipBack();

        PictureList[_firstRevealedPic].Revealed = false;
        PictureList[_secondRevealedPic].Revealed = false;

        PuzzleRevealedNumber = RevealedState.NoRevealed;
        CurrentGameState = GameState.NoAction;

        _corutineStarted = false;
    }

    private void LoadMaterials()
    {
        var materialFilePath = GameSettings.Instance.GetMaterialDirectoryName();
        var textureFilePath = GameSettings.Instance.GetPuzzzleCategoryTextureDirectoryName();
        var pairNumber = (int)GameSettings.Instance.GetPairNumber();
        const string matBaseName = "Pic";
        var firstMaterialName = "Back";
        for(var index =1; index <= pairNumber; index++)
        {
            var currentFilePath = materialFilePath + matBaseName + index;
            Material mat = Resources.Load(currentFilePath, typeof(Material)) as Material;
            _materialList.Add(mat);

            var currenTextureFilePath = textureFilePath + matBaseName + index;
            _texturePathList.Add(currenTextureFilePath);
        }

        _firstTexturePath = textureFilePath + firstMaterialName;
        _firstMaterial = Resources.Load(materialFilePath + firstMaterialName, typeof(Material)) as Material;
    }


    private void SpwanPictureMesh(int rows,int colums, Vector2 Pos,Vector2 offset,bool scaleDown)
    {
        for(int col=0;col< colums; col++)
        {
            for(int row = 0; row < rows; row++)
            {
                var tempPicture = (Picture)Instantiate(PicturePrefab,PicSpawnPosition.position, PicturePrefab.transform.rotation);
                if (scaleDown)
                {
                    tempPicture.transform.localScale = _newScaleDown;
                }
                tempPicture.name = tempPicture.name + 'c' + col + 'r' + row;
                PictureList.Add(tempPicture);
            }
        }

        ApplyTexture();
    }

    public void ApplyTexture()
    {
        var rndMatIndex = Random.Range(0, _materialList.Count);
        var AppliedTimes = new int[_materialList.Count];

        for(int i=0; i < _materialList.Count; i++)
        {
            AppliedTimes[i] = 0;
        }
         foreach(var o in PictureList)
        {
            var randPrevious = rndMatIndex;
            var counter = 0;
            var forceMat = false;

            while(AppliedTimes[rndMatIndex]>= 2 || ((randPrevious == rndMatIndex)&& !forceMat))
            {
                rndMatIndex = Random.Range(0, _materialList.Count);
                counter++;
                if(counter > 100)
                {
                    for(var j =0; j< _materialList.Count; j++)
                    {
                        if (AppliedTimes[j] < 2)
                        {
                            rndMatIndex = j;
                            forceMat = true;

                        }

                    }
                    if(forceMat == false)
                    {
                        return;
                    }
                }
            }

            // Set back material (what shows initially)
            o.SetFirstMaterial(_firstMaterial, _firstTexturePath);

            // Set the actual picture material (what shows when flipped)
            o.SetSecondMaterial(_materialList[rndMatIndex], _texturePathList[rndMatIndex]);

            // Apply the back material initially
            o.ApplyFirstMaterial();

            //o.SetFirstMaterial(_firstMaterial, _firstTexturePath);
            //o.ApplyFirstMaterial();
            //o.SetFirstMaterial(_materialList[rndMatIndex], _texturePathList[rndMatIndex]);
            ////o.ApplyFirstMaterial(); //test
            ///
            o.SetIndex(rndMatIndex);
            o.Revealed = false;
            AppliedTimes[rndMatIndex] += 1;

            forceMat = false;

        }
    }

    private void MovePicture(int rows, int columns,Vector2 pos,Vector2 offset)
    {
        var index = 0;
        for (var col=0; col< columns; col++)
        {
            for (int row=0; row< rows; row++)
            {
                var targetPosition = new Vector3((pos.x + (offset.x * row)), (pos.y - (offset.y * col)), 0.0f);
                StartCoroutine(MoveToPosition(targetPosition, PictureList[index]));
                index++;
            }
        }
    }
    private IEnumerator MoveToPosition(Vector3 target,Picture obj)
    {
        var randomDis = 7;
        while( obj.transform.position != target)
        {
            obj.transform.position = Vector3.MoveTowards(obj.transform.position, target, randomDis * Time.deltaTime);
            yield return 0;
        }
    }
}
