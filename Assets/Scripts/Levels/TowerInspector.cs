using UnityEngine;
using UnityEngine.UI;

public class TowerInspector : MonoBehaviour
{

    public IncomeTracker incomeTracker;
    public TowerPlacementManager placementManager;

    public GameObject towerInspectorPanel;
    private GameObject moneyLabel;
    private GameObject towerNameText;
    private GameObject towerDescriptionText;
    private GameObject targetingModeButton;
    private GameObject sellButton;
    private GameObject towerStatsText;
    private GameObject damageUpgradeButton;
    private GameObject attackSpeedUpgradeButton;
    private GameObject rangeUpgradeButton;
    private GameObject[] specialUnlockButtons;

    private Tower selectedTower;

    public Color defaultColor;


    private void Start()
    {
        towerInspectorPanel.SetActive(true);
        moneyLabel = towerInspectorPanel.transform.Find("MoneyLabel").gameObject;
        towerNameText = towerInspectorPanel.transform.Find("TowerName").gameObject;
        towerDescriptionText = towerInspectorPanel.transform.Find("TowerDescription").gameObject;
        targetingModeButton = towerInspectorPanel.transform.Find("ChangeTargeting").gameObject;
        sellButton = towerInspectorPanel.transform.Find("SellTower").gameObject;
        towerStatsText = towerInspectorPanel.transform.Find("TowerStats").gameObject;
        damageUpgradeButton = towerInspectorPanel.transform.Find("DamageUpgrade").gameObject;
        attackSpeedUpgradeButton = towerInspectorPanel.transform.Find("AttackSpeedUpgrade").gameObject;
        rangeUpgradeButton = towerInspectorPanel.transform.Find("RangeUpgrade").gameObject;
        specialUnlockButtons = new GameObject[3];
        for (int i = 0; i < 3; i++)
        {
            specialUnlockButtons[i] = towerInspectorPanel.transform.Find($"Special{i+1}Unlock").gameObject;
        }
        DeselectTower();
    }


    private void Update()
    {
        moneyLabel.GetComponent<TMPro.TextMeshProUGUI>().text = $"Money: ${incomeTracker.currentMoney}";
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
            // Background colour
            towerInspectorPanel.GetComponent<RawImage>().color = selectedTower.towerColor;

            // Tower Name
            towerNameText.GetComponent<TMPro.TextMeshProUGUI>().text = 
                selectedTower.towerName + (selectedTower.specialUnlocked != 0 ? $" (Upgraded)" : "");

            // Tower Description
            towerDescriptionText.GetComponent<TMPro.TextMeshProUGUI>().text = 
                $"{selectedTower.description}\nDamage Type: {selectedTower.damageType}\nProjectile Type: {selectedTower.projectileType}";
            
            // Targeting Mode
            targetingModeButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = $"Target {selectedTower.targetPriority}";
            targetingModeButton.SetActive(true);

            // Sell Button
            sellButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = $"Sell for ${Mathf.RoundToInt(selectedTower.cost * 0.7f)}";
            sellButton.SetActive(true);

            // Tower Stats
            towerStatsText.GetComponent<TMPro.TextMeshProUGUI>().text = 
                $"Damage: {selectedTower.damage.ToString("#.00")}\n\nFire Rate: {selectedTower.attackSpeed.ToString("#.00")}\n\nRange: {selectedTower.range.ToString("#.00")}";

            // Damage Upgrade Button
            if (selectedTower.damageUpgradeLevel >= selectedTower.damageUpgrades.Length)
                damageUpgradeButton.SetActive(false);
            else
            {
                damageUpgradeButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = 
                    $"${selectedTower.damageUpgrades[selectedTower.damageUpgradeLevel].cost} [{selectedTower.damageUpgradeLevel+1}/{selectedTower.damageUpgrades.Length}]";
                damageUpgradeButton.SetActive(true);
            }

            // Attack Speed Upgrade Button
            if (selectedTower.attackSpeedUpgradeLevel >= selectedTower.attackSpeedUpgrades.Length)
                attackSpeedUpgradeButton.SetActive(false);
            else
            {
                attackSpeedUpgradeButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = 
                    $"${selectedTower.attackSpeedUpgrades[selectedTower.attackSpeedUpgradeLevel].cost} [{selectedTower.attackSpeedUpgradeLevel+1}/{selectedTower.attackSpeedUpgrades.Length}]";
                attackSpeedUpgradeButton.SetActive(true);
            }

            // Range Upgrade Button
            if (selectedTower.rangeUpgradeLevel >= selectedTower.rangeUpgrades.Length)
                rangeUpgradeButton.SetActive(false);
            else
            {
                rangeUpgradeButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = 
                    $"${selectedTower.rangeUpgrades[selectedTower.rangeUpgradeLevel].cost} [{selectedTower.rangeUpgradeLevel+1}/{selectedTower.rangeUpgrades.Length}]";
                rangeUpgradeButton.SetActive(true);
            }

            // Special Unlock Buttons
            for (int i = 0; i < specialUnlockButtons.Length; i++)
            {
                if (selectedTower.specialUnlocked != 0)
                {
                    specialUnlockButtons[i].SetActive(false);
                }
                else
                {
                    specialUnlockButtons[i].GetComponentInChildren<TMPro.TextMeshProUGUI>().text = 
                        $"- ${selectedTower.specialUpgrades[i].cost} -\n{selectedTower.specialUpgrades[i].name}:\n{selectedTower.specialUpgrades[i].description}";
                    specialUnlockButtons[i].SetActive(true);
                }
            }
            
        }
        else
        {
            towerInspectorPanel.GetComponent<RawImage>().color = defaultColor;

            towerNameText.GetComponent<TMPro.TextMeshProUGUI>().text = "";
            towerDescriptionText.GetComponent<TMPro.TextMeshProUGUI>().text = "";
            targetingModeButton.SetActive(false);
            sellButton.SetActive(false);
            towerStatsText.GetComponent<TMPro.TextMeshProUGUI>().text = "";
            damageUpgradeButton.SetActive(false);
            attackSpeedUpgradeButton.SetActive(false);
            rangeUpgradeButton.SetActive(false);
            for (int i = 0; i < specialUnlockButtons.Length; i++)
            {
                specialUnlockButtons[i].SetActive(false);
            }
        }
    }


    public void UpgradeDamage()
    {
        if (selectedTower != null)
        {
            if (incomeTracker.currentMoney < selectedTower.damageUpgrades[selectedTower.damageUpgradeLevel].cost)
            {
                return;
            }
            incomeTracker.currentMoney -= selectedTower.damageUpgrades[selectedTower.damageUpgradeLevel].cost;
            selectedTower.cost += selectedTower.damageUpgrades[selectedTower.damageUpgradeLevel].cost;
            selectedTower.damage *= selectedTower.damageUpgrades[selectedTower.damageUpgradeLevel].effectAmount;
            selectedTower.damageUpgradeLevel++;
            RefreshText();
        }
    }


    public void UpgradeAttackSpeed()
    {
        if (selectedTower != null)
        {
            if (incomeTracker.currentMoney < selectedTower.attackSpeedUpgrades[selectedTower.attackSpeedUpgradeLevel].cost)
            {
                return;
            }
            incomeTracker.currentMoney -= selectedTower.attackSpeedUpgrades[selectedTower.attackSpeedUpgradeLevel].cost;
            selectedTower.cost += selectedTower.attackSpeedUpgrades[selectedTower.attackSpeedUpgradeLevel].cost;
            selectedTower.attackSpeed *= selectedTower.attackSpeedUpgrades[selectedTower.attackSpeedUpgradeLevel].effectAmount;
            selectedTower.attackSpeedUpgradeLevel++;
            RefreshText();
        }
    }


    public void UpgradeRange()
    {
        if (selectedTower != null)
        {
            if (incomeTracker.currentMoney < selectedTower.rangeUpgrades[selectedTower.rangeUpgradeLevel].cost)
            {
                return;
            }
            incomeTracker.currentMoney -= selectedTower.rangeUpgrades[selectedTower.rangeUpgradeLevel].cost;
            selectedTower.cost += selectedTower.rangeUpgrades[selectedTower.rangeUpgradeLevel].cost;
            selectedTower.UpdateRange(selectedTower.range * selectedTower.rangeUpgrades[selectedTower.rangeUpgradeLevel].effectAmount);
            selectedTower.rangeUpgradeLevel++;
            RefreshText();
        }
    }


    public void UnlockSpecial(int specialIndex)
    {
        if (selectedTower != null)
        {
            if (incomeTracker.currentMoney < selectedTower.specialUpgrades[specialIndex].cost)
            {
                return;
            }
            incomeTracker.currentMoney -= selectedTower.specialUpgrades[specialIndex].cost;
            selectedTower.cost += selectedTower.specialUpgrades[specialIndex].cost;
            selectedTower.BuySpecial(specialIndex);
            RefreshText();
        }
    }


    public void ChangeTargetingMode()
    {
        if (selectedTower != null)
        {
            selectedTower.ChangePriority();
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
