using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using static Tutorial1;

public class SmallTown : MonoBehaviour
{
    [FoldoutGroup("Part 1"), SerializeField, TextArea] private string part1TextDialogue1 = "We're officially on Rose!\nLet's keep moving forward!";

    public void StartPart1()
    {
        StartCoroutine(StartPart1Coroutine());
    }

    public IEnumerator StartPart1Coroutine()
    {
        yield return new WaitForSeconds(0.5f);      
        yield return WaitForDialogueToFinish(part1TextDialogue1);
    }

    [FoldoutGroup("Part 2"), SerializeField, TextArea] private string part2TextDialogue1 = "This is Sina, the last barrier!\nAGH, THE GUARDS ARE RIGHT BEHIND ME!";

    public void StartPart2()
    {
        StartCoroutine(StartPart2Coroutine());
    }

    public IEnumerator StartPart2Coroutine()
    {
        yield return new WaitForSeconds(0.5f);      
        yield return WaitForDialogueToFinish(part2TextDialogue1);
    }

    [FoldoutGroup("Part 1.5"), SerializeField, TextArea] private string part9TextDialogue1 = "THAT'S TOO MANY GUARDS, BREAK THE LEGS OF THAT ROCK HOLDER!";

    public void StartPart9()
    {
        StartCoroutine(StartPart9Coroutine());
    }

    public IEnumerator StartPart9Coroutine()
    {
        yield return WaitForDialogueToFinish(part9TextDialogue1);
    }
}