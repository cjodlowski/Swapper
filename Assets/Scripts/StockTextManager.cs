using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StockTextManager : MonoBehaviour
{
    public GameObject StockTextPrefab;
    private List<TextMeshProUGUI> texts = new List<TextMeshProUGUI>();
    private Dictionary<int, int> stocks = new Dictionary<int, int>();
    private string stockFormat = "P{0} Lives: {1} |";

    [SerializeField]
    public int maxStocks = 5;

    // returns the id for the new StockText to modify its properties
    public int CreateStockText(Color color)
    {
        var stockText = Instantiate(StockTextPrefab, Vector3.zero, Quaternion.identity);
        stockText.transform.SetParent(transform, false);
        var text = stockText.GetComponent<TextMeshProUGUI>();
        texts.Add(text);
        var id = texts.Count - 1;
        stocks.Add(id, maxStocks);
        texts[id].text = string.Format(stockFormat, id+1, stocks[id]);
        texts[id].color = color;
        return id;
    }

    public int CheckStock(int id)
    {
        return stocks[id];
    }

    // returns was successful in having stock to remove available
    public bool AttemptRemoveStock(int id)
    {
        if (stocks[id] == 1)
        {
            stocks[id] -= 1;
            texts[id].text = string.Format(stockFormat, id + 1, stocks[id]);
            return false;
        }
        stocks[id] -= 1;
        texts[id].text = string.Format(stockFormat, id+1, stocks[id]);

        return true;
    }
    public void ResetStock(int id)
    {
        stocks[id] = 3;
        texts[id].text = string.Format(stockFormat, id+1, stocks[id]);
    }
}
