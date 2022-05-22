using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class LocalPlayerDeadUI : MonoBehaviour
{
    public TMP_Text text;

    private void Start()
    {
        registerToEvent(true);
    }
    private void OnDestroy()
    {
        registerToEvent(false);
    }

    private void registerToEvent(bool confirm)
    {
        if (confirm)
        {
            NetworkUIManager.Singleton.OnLocalPlayerDeadUI += OnLocalPlayerDeadUI;

        }
        else
        {
            NetworkUIManager.Singleton.OnLocalPlayerDeadUI -= OnLocalPlayerDeadUI;

        }
    }

    private void OnLocalPlayerDeadUI(byte id, float countDuration)
    {
        if (PlayerManager.Singleton.MyPlayer.ID == id) // the death player screen will show you are dead ui
        {
            if (TryGetComponent<Animator>(out Animator animator))
                StartCoroutine(StartShowDeathScreen(countDuration, animator));
        }
        else // the others player will show next spawn time of the death player
        {

        }

    }

    private IEnumerator StartShowDeathScreen(float countDuration, Animator animator)
    {
        string animatorParam = animator.parameters[0].name;
        animator.SetInteger(animatorParam, (int)OnOffState.Active);
        while (countDuration > 0)
        {
            text.text = StringUti.Format(StringUti.Singleton.YouAreDead, Math.Round(countDuration, 1));
            countDuration -= Time.deltaTime;
            yield return null;
        }

        animator.SetInteger(animatorParam, (int)OnOffState.Inactive);
    }
}
