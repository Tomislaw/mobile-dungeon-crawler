using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeBarsUI : MonoBehaviour
{
    [SerializeField]
    private int numberOfActiveBars;

    [SerializeField]
    private int upgradeBarCount;

    [SerializeField]
    private GameObject upgradeBarOn;
    [SerializeField]
    private GameObject upgradeBarOff;

    private List<GameObject> _upgradeBars = new List<GameObject>();

    public void SetActiveBars(int numberOfActiveBars)
    {
        this.numberOfActiveBars = numberOfActiveBars;
        _upgradeBars.ForEach(upgradeBar => Destroy(upgradeBar));
        _upgradeBars.Clear();
        for (int i = 0; i < upgradeBarCount; i++)
        {
            var bar = i < numberOfActiveBars ? upgradeBarOn : upgradeBarOff;
            var newBar = Instantiate(bar);
            newBar.transform.SetParent(transform, false);
            newBar.name = "Bar_" + i;
            newBar.SetActive(true);
            _upgradeBars.Add(newBar);
        }
    }

    void Start()
    {
        if(upgradeBarOn.transform.parent == this.transform)
            upgradeBarOn.SetActive(false);

        if (upgradeBarOff.transform.parent == this.transform)
            upgradeBarOff.SetActive(false);

        SetActiveBars(numberOfActiveBars);
    }

}
