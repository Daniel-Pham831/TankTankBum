using UnityEngine;

public enum OnOffState
{
    Inactive = 0,
    Active = 1
}

[CreateAssetMenu(fileName = "UIAnimatorsController", menuName = "TankTankBum/UIAnimatorsController", order = 0)]
public class UIAnimatorsController : SingletonScriptableObject<UIAnimatorsController>
{

}
