using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCreationGui : MonoBehaviour
{
    public GameObject unitDefinitionGuiPrefab;
    public RectTransform scrollViewContent;

    private readonly List<UnitDefinitionGui> unitDefinitionGuis = new List<UnitDefinitionGui>();

    // Start is called before the first frame update
    void Start()
    {
        Hide();
    }

    public void Display(UnitDefinitionGui.UnitDefinition[] unitDefinitions)
    {
        foreach (var unitDefGui in unitDefinitionGuis)
        {
            Destroy(unitDefGui.gameObject);
        }
        unitDefinitionGuis.Clear();

        for (int i = 0; i < unitDefinitions.Length; i++)
        {
            GameObject unitDefGuiGo = Instantiate<GameObject>(unitDefinitionGuiPrefab, scrollViewContent);
            RectTransform rectTransform = unitDefGuiGo.GetComponent<RectTransform>();
            UnitDefinitionGui unitDefGui = unitDefGuiGo.GetComponent<UnitDefinitionGui>();

            //Positioning
            rectTransform.anchoredPosition = new Vector2(rectTransform.sizeDelta.x * i, 0);
            unitDefGui.SetUnitDefinition(unitDefinitions[i]);

            unitDefinitionGuis.Add(unitDefGui);
        }

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    
}
