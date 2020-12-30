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

    public void Initialize(int numOfItem)
    {
        foreach (var unitDefGui in unitDefinitionGuis)
        {
            Destroy(unitDefGui.gameObject);
        }
        unitDefinitionGuis.Clear();

        for (int i = 0; i < numOfItem; i++)
        {
            GameObject unitDefGuiGo = Instantiate<GameObject>(unitDefinitionGuiPrefab, scrollViewContent);
            RectTransform rectTransform = unitDefGuiGo.GetComponent<RectTransform>();
            UnitDefinitionGui unitDefGui = unitDefGuiGo.GetComponent<UnitDefinitionGui>();

            //Positioning
            rectTransform.anchoredPosition = new Vector2(rectTransform.sizeDelta.x * i, 0);

            unitDefinitionGuis.Add(unitDefGui);
        }

        gameObject.SetActive(true);
    }

    public IReadOnlyList<UnitDefinitionGui> GetUnitDefinitionGuis()
    {
        return unitDefinitionGuis;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    
}
