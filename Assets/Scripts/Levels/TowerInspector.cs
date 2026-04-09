using UnityEngine;

public class TowerInspector : MonoBehaviour
{

    public IncomeTracker incomeTracker;
    public TowerPlacementManager placementManager;

    public GameObject towerInspectorPanel;
    private GameObject towerNameText;
    private GameObject towerDescriptionText;
    private GameObject towerStatsText;
    private GameObject damageUpgradeButton;
    private GameObject attackSpeedUpgradeButton;
    private GameObject rangeUpgradeButton;
    private GameObject specialUpgradeButton;
    private GameObject sellButton;


    private Tower selectedTower;

    private void Start()
    {
        towerInspectorPanel.SetActive(true);
        towerNameText = towerInspectorPanel.transform.Find("TowerName").gameObject;
        towerDescriptionText = towerInspectorPanel.transform.Find("TowerDescription").gameObject;
        towerStatsText = towerInspectorPanel.transform.Find("TowerStats").gameObject;
        damageUpgradeButton = towerInspectorPanel.transform.Find("DamageUpgrade").gameObject;
        attackSpeedUpgradeButton = towerInspectorPanel.transform.Find("AttackSpeedUpgrade").gameObject;
        rangeUpgradeButton = towerInspectorPanel.transform.Find("RangeUpgrade").gameObject;
        specialUpgradeButton = towerInspectorPanel.transform.Find("SpecialUnlock").gameObject;
        sellButton = towerInspectorPanel.transform.Find("SellTower").gameObject;
        DeselectTower();
    }
    
    public void SelectTower(Tower tower)
    {
        selectedTower = tower;
        RefreshText();
    }


    public void DeselectTower()
    {
        selectedTower = null;
        RefreshText();
    }


    private void RefreshText()
    {
        if (selectedTower != null)
        {
            towerNameText.GetComponent<TMPro.TextMeshProUGUI>().text = selectedTower.towerName;
            towerDescriptionText.GetComponent<TMPro.TextMeshProUGUI>().text = selectedTower.description;
            towerStatsText.GetComponent<TMPro.TextMeshProUGUI>().text = 
                    $"Damage: {selectedTower.damage}\nFire Rate: {selectedTower.attackSpeed}\nRange: {selectedTower.range}\nDamage Type: {selectedTower.damageType}\nProjectile Type: {selectedTower.projectileType}";
            if (selectedTower.damageUpgradeLevel >= selectedTower.damageUpgradeCosts.Length)
                damageUpgradeButton.SetActive(false);
            else
            {
                damageUpgradeButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = 
                        $"${selectedTower.damageUpgradeCosts[selectedTower.damageUpgradeLevel]} [{selectedTower.damageUpgradeLevel}/{selectedTower.damageUpgradeCosts.Length}]";
                damageUpgradeButton.SetActive(true);
            }
            if (selectedTower.attackSpeedUpgradeLevel >= selectedTower.attackSpeedUpgradeCosts.Length)
                attackSpeedUpgradeButton.SetActive(false);
            else
            {
                attackSpeedUpgradeButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = 
                        $"${selectedTower.attackSpeedUpgradeCosts[selectedTower.attackSpeedUpgradeLevel]} [{selectedTower.attackSpeedUpgradeLevel}/{selectedTower.attackSpeedUpgradeCosts.Length}]";
                attackSpeedUpgradeButton.SetActive(true);
            }
            if (selectedTower.rangeUpgradeLevel >= selectedTower.rangeUpgradeCosts.Length)
                rangeUpgradeButton.SetActive(false);
            else
            {
                rangeUpgradeButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = 
                        $"${selectedTower.rangeUpgradeCosts[selectedTower.rangeUpgradeLevel]} [{selectedTower.rangeUpgradeLevel}/{selectedTower.rangeUpgradeCosts.Length}]";
                rangeUpgradeButton.SetActive(true);
            }
            if (selectedTower.specialUnlocked || selectedTower.specialCost <= 0)
                specialUpgradeButton.SetActive(false);
            else
            {
                specialUpgradeButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = $"${selectedTower.specialCost} [Unlock Special]";
                specialUpgradeButton.SetActive(true);
            }

            sellButton.SetActive(true);
        }
        else
        {
            towerNameText.GetComponent<TMPro.TextMeshProUGUI>().text = "";
            towerDescriptionText.GetComponent<TMPro.TextMeshProUGUI>().text = "";
            towerStatsText.GetComponent<TMPro.TextMeshProUGUI>().text = "";
            damageUpgradeButton.SetActive(false);
            attackSpeedUpgradeButton.SetActive(false);
            rangeUpgradeButton.SetActive(false);
            specialUpgradeButton.SetActive(false);
            sellButton.SetActive(false);
        }
    }

    public void UpgradeDamage()
    {
        if (selectedTower != null)
        {
            if (incomeTracker.currentMoney < selectedTower.damageUpgradeCosts[selectedTower.damageUpgradeLevel])
            {
                return;
            }
            incomeTracker.currentMoney -= selectedTower.damageUpgradeCosts[selectedTower.damageUpgradeLevel];
            selectedTower.cost += selectedTower.damageUpgradeCosts[selectedTower.damageUpgradeLevel];
            selectedTower.damage *= 1.1f; // Replace with array in tower class later
            selectedTower.damageUpgradeLevel++;
            RefreshText();
        }
    }

    public void UpgradeAttackSpeed()
    {
        if (selectedTower != null)
        {
            if (incomeTracker.currentMoney < selectedTower.attackSpeedUpgradeCosts[selectedTower.attackSpeedUpgradeLevel])
            {
                return;
            }
            incomeTracker.currentMoney -= selectedTower.attackSpeedUpgradeCosts[selectedTower.attackSpeedUpgradeLevel];
            selectedTower.cost += selectedTower.attackSpeedUpgradeCosts[selectedTower.attackSpeedUpgradeLevel];
            selectedTower.attackSpeed *= 1.1f;
            selectedTower.attackSpeedUpgradeLevel++;
            RefreshText();
        }
    }

    public void UpgradeRange()
    {
        if (selectedTower != null)
        {
            if (incomeTracker.currentMoney < selectedTower.rangeUpgradeCosts[selectedTower.rangeUpgradeLevel])
            {
                return;
            }
            incomeTracker.currentMoney -= selectedTower.rangeUpgradeCosts[selectedTower.rangeUpgradeLevel];
            selectedTower.cost += selectedTower.rangeUpgradeCosts[selectedTower.rangeUpgradeLevel];
            selectedTower.range *= 1.1f;
            selectedTower.rangeUpgradeLevel++;
            RefreshText();
        }
    }

    public void UnlockSpecial()
    {
        if (selectedTower != null)
        {
            if (incomeTracker.currentMoney < selectedTower.specialCost)
            {
                return;
            }
            incomeTracker.currentMoney -= selectedTower.specialCost;
            selectedTower.cost += selectedTower.specialCost;
            selectedTower.specialUnlocked = true;
            RefreshText();
        }
    }

    public void SellTower()
    {
        if (selectedTower != null)
        {
            incomeTracker.currentMoney += (int)(selectedTower.cost * 0.7f);
            placementManager.RemoveTower(selectedTower.GetComponent<TowerInstance>().GridPosition);
            Destroy(selectedTower.gameObject);
            DeselectTower();
        }
    }
}
