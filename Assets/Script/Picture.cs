using System.Collections;
using UnityEngine;

public class Picture :MonoBehaviour
{
    private Material _firstMaterial;
    private Material _secondMaterial;
    private Quaternion _currentRoation;

    [HideInInspector] public bool Revealed = false;
    private PicturManager _pictureManager;
    private bool _clicked = false;

    private void Start()
    {
        Revealed = false;
        _clicked = false;
        _pictureManager = GameObject.Find("PictureManager").GetComponent<PicturManager>();
        _currentRoation = gameObject.transform.rotation;
    }

    private void OnMouseDown()
    {
        if(_clicked == false)
        {
            _pictureManager.CurrentPuzzleState = PicturManager.PuzzleState.PuzzleRotating;
            StartCoroutine(LoopRotation(45, false));
            _clicked = true;

        }

        
        
    }

    public void FlipBack()
    {
        if (gameObject.activeSelf)
        {
            _pictureManager.CurrentPuzzleState = PicturManager.PuzzleState.PuzzleRotating;
            Revealed = false;
            StartCoroutine(LoopRotation(45, true));
        }
    }

    IEnumerator LoopRotation(float angle, bool FirstMat)
    {
        var rot = 0f;
        const float dir = 1f;
        const float rotSpeed = 180.0f;
        const float rotSpeed1 = 90.0f;
        var startAngle = angle;
        var assigned = false;

        if (FirstMat)
        {
            while (rot < angle)
            {
                var step = Time.deltaTime * rotSpeed1;
                gameObject.GetComponent<Transform>().Rotate(new Vector3(0, 2, 0) * step * dir);
                if (rot >= (startAngle - 2) && assigned == false)
                {
                    ApplyFirstMaterial();
                    assigned = true;
                }
                rot += (1 * step * dir);
                yield return null;
            }
        }
        else
        {
            while (angle > 0) //angle > 0
            {
                float step = Time.deltaTime * rotSpeed;
                gameObject.GetComponent<Transform>().Rotate(new Vector3(0, 2, 0) * step * dir);
                angle -= (1 * step * dir);
                yield return null;


            }
        }

        gameObject.GetComponent<Transform>().rotation = _currentRoation;

        if (!FirstMat)
        {
            Revealed = true;
            ApplySecondMaterial();
            _pictureManager.CheckPicture();
        }
        else
        {
            _pictureManager.PuzzleRevealedNumber = PicturManager.RevealedState.NoRevealed;
            _pictureManager.CurrentPuzzleState = PicturManager.PuzzleState.CanRotate;
        }
        _clicked = false;
    }

    public void SetFirstMaterial(Material mat,string textturePath)
    {
        _firstMaterial = mat;
        _firstMaterial.mainTexture = Resources.Load(textturePath, typeof(Texture2D)) as Texture2D;
    }
    public void SetSecondMaterial(Material mat, string textturePath)
    {
        _secondMaterial = mat;
        _secondMaterial.mainTexture = Resources.Load(textturePath, typeof(Texture2D)) as Texture2D;
    }
    public void ApplyFirstMaterial()
    {
        gameObject.GetComponent<Renderer>().material = _firstMaterial;
    }
    public void ApplySecondMaterial()
    {
        gameObject.GetComponent<Renderer>().material = _secondMaterial;
    }
}
