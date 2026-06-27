using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tutorial1 : MonoBehaviour
{
    [SerializeField] private PathFollower pathFollower;

    #region Part1

    [FoldoutGroup("Part 1"), SerializeField, TextArea] private string part1TextDialogue1;
    [FoldoutGroup("Part 1"), SerializeField, TextArea] private string part1TextDialogue2;
    [FoldoutGroup("Part 1"), SerializeField, TextArea] private string part1TextDialogue3;

    public void InitiatePart1()
    {
        StartCoroutine(Part1Coroutine());
    }

    private IEnumerator Part1Coroutine()
    {
        SpinnerCollider.IsGlobalInteractable = false;
        pathFollower.IsAllowedToMove = false;

        yield return new WaitForSeconds(0.5f);      

        yield return WaitForCamera(pathFollower.transform.position + (Vector3.up * 3));
        
        yield return WaitForDialogueWithClick(part1TextDialogue1);
        yield return null;
        yield return WaitForDialogueWithClick(part1TextDialogue2);

        Vector2 startPosition = CameraMovement.Instance.transform.position;
        CameraMovement.Instance.UnforceCamera();

        float minMoveRequirement = 20;
        while(Vector2.Distance(startPosition, CameraMovement.Instance.transform.position) < minMoveRequirement) yield return null;

        StartCoroutine(WaitForCamera(pathFollower.transform.position + (Vector3.up * 3), zoom: 15));
        yield return WaitForDialogueWithClick(part1TextDialogue3);
        CameraMovement.Instance.UnforceCamera();
        
        pathFollower.IsAllowedToMove = true;
    }

    #endregion
    #region Part 2

    [FoldoutGroup("Part 2"), SerializeField] private Spinner part2Spinner;
    [FoldoutGroup("Part 2"), SerializeField] private float part2DeathDelay = 3;
    [Space]
    [FoldoutGroup("Part 2"), SerializeField, TextArea] private string part2TextDialogue1;
    [FoldoutGroup("Part 2"), SerializeField, TextArea] private string part2TextDialogue2;
    [FoldoutGroup("Part 2"), SerializeField, TextArea] private string part2TextDialogue3 = "I get impatient, so try to do it as fast as you can!";
    private bool reachedPointOfPart2;

    public void ReachedPointOfPart2()
    {
        GameManager.OnResetLevel += VampireDiedPart2;
    }

    private void VampireDiedPart2()
    {        
        GameManager.OnResetLevel -= VampireDiedPart2;
        StartCoroutine(Part2Coroutine());
    }

    private IEnumerator Part2Coroutine()
    {
        pathFollower.IsAllowedToMove = false;
        reachedPointOfPart2 = true;

        yield return new WaitForSeconds(0.5f);      

        yield return WaitForCamera(pathFollower.transform.position + (Vector3.up * 6), zoom: 15);
        
        yield return WaitForDialogueWithClick(part2TextDialogue1);
        yield return WaitForDialogueWithClick(part2TextDialogue2);

        yield return WaitForCamera(part2Spinner.transform.position);
        yield return new WaitForSeconds(1f);
        yield return WaitForCamera(pathFollower.transform.position + (Vector3.up * 15), zoom: 20);
        yield return WaitForDialogueWithClick(part2TextDialogue3);

        CameraMovement.Instance.UnforceCamera();

        SpinnerCollider.IsGlobalInteractable = true;
        pathFollower.IsAllowedToMove = true;
        Part2PotentialWait();
    }

    public void Part2PotentialWait()
    {
        if(!reachedPointOfPart2) return;

        StartCoroutine(VampireWait(pathFollower, part2DeathDelay));
    }
    
    public static IEnumerator VampireWait(PathFollower pathFollower, float seconds)
    {
        pathFollower.IsAllowedToMove = false;
        yield return new WaitForSeconds(seconds);
        pathFollower.IsAllowedToMove = true;
    }

    #endregion
    #region Part 3

    [FoldoutGroup("Part 3"), SerializeField] private Spinner part3Spinner;
    [FoldoutGroup("Part 3"), SerializeField, TextArea] private string part3TextDialogue1 = "See those ropes?\nTry cutting them down!";
    [FoldoutGroup("Part 3"), SerializeField, TextArea] private string part3TextDialogue2 = "Click on them and then do a circular motion around the red point!";
    [Space]
    [FoldoutGroup("Part 3"), SerializeField] private Transform part3Water;
    [FoldoutGroup("Part 3"), SerializeField, TextArea] private string part3TextDialogue3 = "Please don't let me fall into the holy water!";

    public void ReachedPointOfPart3() => StartCoroutine(Part3Coroutine());

    private IEnumerator Part3Coroutine()
    {
        SpinnerCollider.IsGlobalInteractable = false;
        pathFollower.IsAllowedToMove = false;
        
        yield return WaitForCamera(part3Spinner.transform.position);
        yield return new WaitForSeconds(0.3f);
        yield return WaitForDialogueWithClick(part3TextDialogue1);
        yield return WaitForDialogueWithClick(part3TextDialogue2);

        StartCoroutine(WaitForCamera(part3Water.position));
        yield return WaitForDialogueWithClick(part3TextDialogue3);

        yield return WaitForCamera(pathFollower.transform.position + (Vector3.up * 8), zoom: 20);
        CameraMovement.Instance.UnforceCamera();

        SpinnerCollider.IsGlobalInteractable = true;
        pathFollower.IsAllowedToMove = true;
    }

    #endregion

    public static IEnumerator WaitForCamera(Vector3 position, float moveSpeed = 1, float zoom = 7, float zoomSpeed = 6)
    {
        bool reachedDestination = false;
        CameraMovement.CameraReachedDeathDestination += () => reachedDestination = true;
        CameraMovement.Instance.ForceCamera(position, moveSpeed, zoom, zoomSpeed);

        while(!reachedDestination) yield return null;
    }

    public static IEnumerator WaitForDialogueWithClick(string dialogue, Sprite sprite = null)
    {
        bool completedDialogue = false;
        DialogueManager.Instance.InitiateDialogue(dialogue, () => Mouse.current.leftButton.wasPressedThisFrame, onDialogueOver: () => completedDialogue = true, sprite: sprite, showClickToContinue: true);

        while(!completedDialogue) yield return null;
    }

    public static IEnumerator WaitForDialogueToFinish(string dialogue)
    {
        bool completedDialogue = false;
        DialogueManager.Instance.InitiateDialogue(dialogue, () => true, onDialogueOver: () => completedDialogue = true);

        while(!completedDialogue) yield return null;
    }

    public static IEnumerator WaitForSpinnerToFinish(Spinner spinner)
    {
        while(spinner.GetProgress() != 1) yield return null;
    }
}