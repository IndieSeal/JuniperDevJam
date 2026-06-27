using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using static Tutorial1;

public class MeetTheKing : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cinCamera;
    [SerializeField] private PathFollower pathFollower;
    [SerializeField] private PathFollower kingPath;
    [SerializeField] private Transform cameraFollow;
    [SerializeField] private Sprite kingSprite;
    
    [SerializeField, TextArea] private string part0Dialogue0 = "I'm getting kinda nervous, it's been a while"; //Vamp
    [SerializeField, TextArea] private string part1Dialogue1 = "We're almost there, thanks to you I haven't been seen."; //Vamp
    [SerializeField, TextArea] private string part2Dialogue1 = "<shake>KING!<waitfor=0.5> I'M HERE FOR YOU!<waitfor=0.5> PREPARE FOR MY BITE</shake>"; //Vamp
    [SerializeField, TextArea] private string part2Dialogue2 = "Hahaha, why do you always act like this everytime we meet?"; //King
    [SerializeField, TextArea] private string part2Dialogue3 = "<speed s=0.5><shake>GET CLOSER</shake></speed>"; //King

    [SerializeField] private Transform position;
    [SerializeField] private float camVelocity = 2;
    [SerializeField] private float zoomAmount = 5;
    [SerializeField] private float zoomSpeed = 0.5f;

    [SerializeField] private GameObject friendlyBiteObj;

    void Awake()
    {
        kingPath.IsAllowedToMove = false;
    }

    public void ExecutePart0()
    {
        StartCoroutine(Part0Coroutine());
    }

    private IEnumerator Part0Coroutine()
    {
        yield return new WaitForSeconds(1.5f);
        yield return WaitForDialogueToFinish(part0Dialogue0);
    }
    
    public void ExecutePart1()
    {
        StartCoroutine(Part1Coroutine());
    }

    private IEnumerator Part1Coroutine()
    {
        pathFollower.IsAllowedToMove = false;
        yield return WaitForDialogueWithClick(part1Dialogue1);
        pathFollower.IsAllowedToMove = true;
    }

    public void ExecutePart2()
    {
        StartCoroutine(Part2Coroutine());
    }

    private IEnumerator Part2Coroutine()
    {
        pathFollower.IsAllowedToMove = false;
        yield return WaitForDialogueWithClick(part2Dialogue1);
        cinCamera.Follow = cameraFollow.transform;
        yield return WaitForDialogueWithClick(part2Dialogue2);
        yield return WaitForDialogueWithClick(part2Dialogue3);
        pathFollower.IsAllowedToMove = true;
        kingPath.IsAllowedToMove = true;

        while(kingPath.Index < kingPath.Path.PointAmount) yield return null;

        Vector2 closenessStartPosition = cameraFollow.transform.position;
        Vector2 closenessTargetPosition = position.position;

        float closenessTimer = 0;
        while (true)
        {
            closenessTimer += Time.deltaTime;
            MoveWithoutZ(Vector2.Lerp(closenessStartPosition, closenessTargetPosition, closenessTimer / camVelocity));
            cinCamera.Lens.OrthographicSize = Mathf.Lerp(cinCamera.Lens.OrthographicSize, zoomAmount, zoomSpeed * Time.deltaTime);

            if(Vector2.Distance(cameraFollow.position, closenessTargetPosition) < 0.01f && (cinCamera.Lens.OrthographicSize - zoomAmount) < 0.01f)
            {
                yield return null;
                break;
            }
        }

        yield return new WaitForSeconds(0.7f);
        
        pathFollower.gameObject.SetActive(false);
        kingPath.gameObject.SetActive(false);
        friendlyBiteObj.SetActive(true);
        
        yield return new WaitForSeconds(4);
        StartCoroutine(TransitionManager.Instance.TransitionToNewScene("LevelSelect"));
    }

    private void MoveWithoutZ(Vector2 newPosition) => cameraFollow.position = new Vector3(newPosition.x, newPosition.y, cinCamera.transform.position.z);
}