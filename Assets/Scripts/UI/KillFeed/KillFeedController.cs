using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KillFeedController : MonoBehaviour
{
    [SerializeField] private GameObject killFeedItemPrefab;
    [SerializeField] private float timeShowPerItem;

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
            NetworkUIManager.Singleton.OnPlayerKilledPlayer += ShowKillFeedItem;
        }
        else
        {
            NetworkUIManager.Singleton.OnPlayerKilledPlayer -= ShowKillFeedItem;
        }
    }

    private void ShowKillFeedItem(Player killer, Player killed)
    {
        GameObject item = Instantiate(killFeedItemPrefab, this.transform);
        TMP_Text itemText = item.GetComponentInChildren<TMP_Text>();
        itemText.SetText(StringUti.Format(GetKillFeedString(killer.Team, killed.Team), killer.Name, killed.Name));
        StartCoroutine(DestroyItemAfter(timeShowPerItem, item.GetComponent<Animator>()));
    }

    private string GetKillFeedString(Team killerTeam, Team killedTeam)
    {
        bool isSameTeam = killerTeam == killedTeam;
        if (isSameTeam)
        {
            return killerTeam == Team.Blue ? StringUti.Singleton.BlueSameTeamKill : StringUti.Singleton.RedSameTeamKill;
        }
        else
        {
            return killerTeam == Team.Blue ? StringUti.Singleton.BlueTeamKill : StringUti.Singleton.RedTeamKill;
        }
    }

    private IEnumerator DestroyItemAfter(float duration, Animator itemAnimator)
    {
        yield return new WaitForSeconds(duration);
        itemAnimator.SetTrigger(itemAnimator.GetParameter(0).name);

        Destroy(itemAnimator.gameObject, 0.5f);
    }
}
