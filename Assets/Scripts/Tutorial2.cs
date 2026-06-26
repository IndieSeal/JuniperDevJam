using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using static Tutorial1;

public class Tutorial2 : MonoBehaviour
{
    [SerializeField] private PathFollower pathFollower;

    #region Part1

    [FoldoutGroup("Part 1"), SerializeField, TextArea] private string part1TextDialogue1 = "Yes, we made it to Maria!\nLet's keep on going :D";
    [FoldoutGroup("Part 1"), SerializeField] private Transform mapCenter;
    [FoldoutGroup("Part 1"), SerializeField] private float zoomAmount = 50;

    private Vector3 vampirePosition => pathFollower.transform.position + (Vector3.up * 3);

    public void StartPart1()
    {
        StartCoroutine(StartPart1Coroutine());
    }

    public IEnumerator StartPart1Coroutine()
    {
        SpinnerCollider.IsGlobalInteractable = false;
        pathFollower.IsAllowedToMove = false;

        yield return new WaitForSeconds(0.5f);      

        yield return WaitForCamera(mapCenter.position, 1.3f, zoomAmount, 20);
        yield return WaitForDialogueWithClick(part1TextDialogue1);
        yield return WaitForCamera(vampirePosition, 1.3f, 10, 20);
        CameraMovement.Instance.UnforceCamera();

        pathFollower.IsAllowedToMove = true;
    }

    #endregion
    #region Part 2

    [FoldoutGroup("Part 2"), SerializeField, TextArea] private string part2TextDialogue1 = "Ah, crap! We woke the baby up D:\nTry calming them down!";
    [FoldoutGroup("Part 2"), SerializeField, TextArea] private string part2TextDialogue2 = "HURRY, CALM IT DOWN, GUARDS ARE GOING TO COME!!";
    [FoldoutGroup("Part 2"), SerializeField, TextArea] private string part2TextDialogue3 = "Phew, I got nervous there, glad you could calm the baby down!";

    [FoldoutGroup("Part 2"), SerializeField] private Baby part2Baby;
    private bool hasBabyAttacked;

    void OnEnable()
    {
        part2Baby.OnAttack += BabyAttacked;
    }

    void OnDisable()
    {
        part2Baby.OnAttack -= BabyAttacked;
    }

    private void BabyAttacked()
    {
        hasBabyAttacked = true;
    }

    public void ResetPart2()
    {
        hasBabyAttacked = false;
        part2Baby.SetBabyNormality(false);
        pathFollower.IsAllowedToMove = true;
    }

    public void StartPart2()
    {
        StartCoroutine(StartPart2Coroutine());
    }

    public IEnumerator StartPart2Coroutine()
    {
        SpinnerCollider.IsGlobalInteractable = false;
        pathFollower.IsAllowedToMove = false;

        part2Baby.InstructionAlert();

        yield return new WaitForSeconds(0.5f);      

        yield return WaitForCamera(part2Baby.transform.position);
        
        yield return WaitForDialogueWithClick(part2TextDialogue1);
        yield return WaitForCamera(pathFollower.transform.position, zoom: 13);
        CameraMovement.Instance.UnforceCamera();

        SpinnerCollider.IsGlobalInteractable = true;

        while(!part2Baby.IsSelected) yield return null;

        part2Baby.InstructionAttack();
        part2Baby.SetBabyNormality(true);

        StartCoroutine(WaitForDialogueToFinish(part2TextDialogue2));
        yield return WaitForSpinnerToFinish(part2Baby);
        yield return WaitForDialogueWithClick(part2TextDialogue3);

        SpinnerCollider.IsGlobalInteractable = false;
        pathFollower.IsAllowedToMove = true;
    }

    #endregion

    [FoldoutGroup("Part 3"), SerializeField, TextArea] private string part3TextDialogue1 = "OH DRACULA! Is that a priest?\nAND THEY HAVE HOLY WATER?!";
    [FoldoutGroup("Part 3"), SerializeField, TextArea] private string part3TextDialogue2 = "Could you try shaking their water away?";
    [FoldoutGroup("Part 3"), SerializeField, TextArea] private string part3TextDialogue3 = "But don't forget to be quick about it!";
    [FoldoutGroup("Part 3"), SerializeField] private Priest part3Priest;

    private bool hasGoneThroughPart3;

    public void PossibleWait(float duration)
    {
        if(!hasGoneThroughPart3) return;
        StartCoroutine(VampireWait(pathFollower, duration));
    }

    public void StartPart3()
    {
        part2Baby.SetBabyNormality(false);

        hasGoneThroughPart3 = true;
        StartCoroutine(StartPart3Coroutine());
    }

    private IEnumerator StartPart3Coroutine()
    {
        pathFollower.IsAllowedToMove = false;

        yield return WaitForCamera(part3Priest.transform.position + (Vector3.up * 3));
        
        yield return WaitForDialogueWithClick(part3TextDialogue1);
        yield return WaitForCamera(vampirePosition, zoom: 13);
        yield return WaitForDialogueWithClick(part3TextDialogue2);
        CameraMovement.Instance.UnforceCamera();
        SpinnerCollider.IsGlobalInteractable = true;

        yield return new WaitForSeconds(1);
        yield return WaitForDialogueToFinish(part3TextDialogue3);
        yield return new WaitForSeconds(4);

        pathFollower.IsAllowedToMove = true;
    }
}