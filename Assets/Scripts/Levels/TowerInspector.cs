using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class TowerInspector : MonoBehaviour
{

    public IncomeTracker incomeTracker;
    public TowerPlacementManager placementManager;

    public GameObject towerInspectorPanel;
    private TextMeshProUGUI moneyLabel;
    private TextMeshProUGUI towerNameText;
    private TextMeshProUGUI towerDescriptionText;
    private GameObject targetingModeButton;
    private GameObject sellButton;
    private GameObject damageUpgradeButton;
    private TextMeshProUGUI damageInfoText;
    private TextMeshProUGUI damageCostText;
    private GameObject attackSpeedUpgradeButton;
    private TextMeshProUGUI attackSpeedInfoText;
    private TextMeshProUGUI attackSpeedCostText;
    private GameObject rangeUpgradeButton;
    private TextMeshProUGUI rangeInfoText;
    private TextMeshProUGUI rangeCostText;
    private GameObject[] specialUnlockButtons;

    private Tower selectedTower;

    public Color defaultColour;
    public Color towerRangeColour;

    public Dictionary<System.Type, bool[]> upgradesAvailable;


    private void Start()
    {
        towerInspectorPanel.SetActive(true);

        moneyLabel = towerInspectorPanel.transform.Find("MoneyLabel").gameObject.GetComponent<TMPro.TextMeshProUGUI>();

        towerNameText = towerInspectorPanel.transform.Find("TowerName").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
        towerDescriptionText = towerInspectorPanel.transform.Find("TowerDescription").gameObject.GetComponent<TMPro.TextMeshProUGUI>();

        targetingModeButton = towerInspectorPanel.transform.Find("ChangeTargeting").gameObject;
        sellButton = towerInspectorPanel.transform.Find("SellTower").gameObject;
        
        damageUpgradeButton = towerInspectorPanel.transform.Find("DamageUpgrade").gameObject;
        damageInfoText = damageUpgradeButton.transform.Find("StatText").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
        damageCostText = damageUpgradeButton.transform.Find("UpgradeText").gameObject.GetComponent<TMPro.TextMeshProUGUI>();

        attackSpeedUpgradeButton = towerInspectorPanel.transform.Find("AttackSpeedUpgrade").gameObject;
        attackSpeedInfoText = attackSpeedUpgradeButton.transform.Find("StatText").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
        attackSpeedCostText = attackSpeedUpgradeButton.transform.Find("UpgradeText").gameObject.GetComponent<TMPro.TextMeshProUGUI>();

        rangeUpgradeButton = towerInspectorPanel.transform.Find("RangeUpgrade").gameObject;
        rangeInfoText = rangeUpgradeButton.transform.Find("StatText").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
        rangeCostText = rangeUpgradeButton.transform.Find("UpgradeText").gameObject.GetComponent<TMPro.TextMeshProUGUI>();

        specialUnlockButtons = new GameObject[3];
        for (int i = 0; i < 3; i++)
        {
            specialUnlockButtons[i] = towerInspectorPanel.transform.Find($"Special{i+1}Unlock").gameObject;
        }

        upgradesAvailable = new Dictionary<System.Type, bool[]>();

        DeselectTower();
    }


    private void Update()
    {
        moneyLabel.text = $"Money: ${incomeTracker.currentMoney}";
    }
    

    public void SelectTower(Tower tower)
    {
        if (tower == selectedTower) {
            DeselectTower();
            return;
        }

        if (selectedTower != null)
        {
            selectedTower.attackRange.SetIndicatorVisibility(false);
        }
        selectedTower = tower;
        selectedTower.attackRange.SetIndicatorColour(towerRangeColour);
        selectedTower.attackRange.SetIndicatorVisibility(true);
        RefreshText();
    }


    public void DeselectTower()
    {
        if (selectedTower != null)
        {
            selectedTower.attackRange.SetIndicatorVisibility(false);
        }
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
            towerNameText.GetComponent<TMPro.TextMeshProUGUI>().text = selectedTower.towerName;

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
            // towerStatsText.GetComponent<TMPro.TextMeshProUGUI>().text = 
            //     $"Damage: {selectedTower.damage.ToString("0.00")}\n\nFire Rate: {selectedTower.attackSpeed.ToString("0.00")}\n\nRange: {selectedTower.range.ToString("0.00")}";


            // Damage Upgrade Button
            if (selectedTower.damageUpgradeLevel >= selectedTower.damageUpgrades.Length)
                damageUpgradeButton.SetActive(false);
            else
            {
                damageInfoText.text = $"Damage: {selectedTower.damage.ToString("0.00")}";
                damageCostText.text = 
                    $"${selectedTower.damageUpgrades[selectedTower.damageUpgradeLevel].cost} [{selectedTower.damageUpgradeLevel+1}/{selectedTower.damageUpgrades.Length}]";
                damageUpgradeButton.SetActive(true);
            }

            // Attack Speed Upgrade Button
            if (selectedTower.attackSpeedUpgradeLevel >= selectedTower.attackSpeedUpgrades.Length)
                attackSpeedUpgradeButton.SetActive(false);
            else
            {
                attackSpeedInfoText.text = $"Attack Speed: {selectedTower.attackSpeed.ToString("0.00")}";
                attackSpeedCostText.text = 
                    $"${selectedTower.attackSpeedUpgrades[selectedTower.attackSpeedUpgradeLevel].cost} [{selectedTower.attackSpeedUpgradeLevel+1}/{selectedTower.attackSpeedUpgrades.Length}]";
                attackSpeedUpgradeButton.SetActive(true);
            }

            // Range Upgrade Button
            if (selectedTower.rangeUpgradeLevel >= selectedTower.rangeUpgrades.Length)
                rangeUpgradeButton.SetActive(false);
            else
            {
                rangeInfoText.text = $"Range: {selectedTower.range.ToString("0.00")}";
                rangeCostText.text = 
                    $"${selectedTower.rangeUpgrades[selectedTower.rangeUpgradeLevel].cost} [{selectedTower.rangeUpgradeLevel+1}/{selectedTower.rangeUpgrades.Length}]";
                rangeUpgradeButton.SetActive(true);
            }

            // Special Unlock Buttons
            if (!upgradesAvailable.ContainsKey(selectedTower.GetType()))
            {
                upgradesAvailable[selectedTower.GetType()] = new bool[] /*{false,false,false}*/ {true,true,true};
            }

            for (int i = 0; i < specialUnlockButtons.Length; i++)
            {
                if (selectedTower.specialUnlocked != 0 || !upgradesAvailable[selectedTower.GetType()][i])
                {
                    specialUnlockButtons[i].SetActive(false);
                }
                else
                {
                    specialUnlockButtons[i].GetComponentInChildren<TMPro.TextMeshProUGUI>().text = 
                        $"[${selectedTower.specialUpgrades[i].cost}] - {selectedTower.specialUpgrades[i].name}:\n{selectedTower.specialUpgrades[i].description}";
                    specialUnlockButtons[i].SetActive(true);
                }
            }
            
        }
        else
        {
            towerInspectorPanel.GetComponent<RawImage>().color = defaultColour;

            towerNameText.GetComponent<TMPro.TextMeshProUGUI>().text = "";
            towerDescriptionText.GetComponent<TMPro.TextMeshProUGUI>().text = "";
            targetingModeButton.SetActive(false);
            sellButton.SetActive(false);
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
