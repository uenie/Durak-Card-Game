using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellBito : MonoBehaviour
{
    // Самая верхняя
    public bool isFirst = false;

    // Все карты в ячейке
    public List<GameObject> allCardsInCellBito = new List<GameObject>();

    // Количество карт в ячейке
    public int cardCount = 0;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddCardsFrom(List<GameObject> cardsGO)
    {
        if (cardsGO.Count > 0)
        {
            // Перенести
            foreach (GameObject cardGO in cardsGO)
            {
                allCardsInCellBito.Add(cardGO);

                cardGO.transform.parent = transform;

                cardGO.transform.position = transform.position;

                cardGO.transform.rotation = transform.rotation;

                cardGO.GetComponent<Card>().SetOpen(false);
            }

            // Убрать карты из массива карт
            cardsGO.Clear();
        }
    }
}
