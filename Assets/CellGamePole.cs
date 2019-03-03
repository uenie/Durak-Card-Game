using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellGamePole : MonoBehaviour
{
    // Менеджер очередей
    public TurnManager turnManager;

    // Пары карт: карты первого массива - те, от которых нужно отбиваться, карты второго массива - те, которыми отбились
    public List<GameObject> allDownCards;
    public List<GameObject> allUpCards;

    // Отступ для следующей пары карт
    public float xOffsetForNextCards = 3;

    // Отступы для карты, которая отбивает нижнюю
    public float xOffset = 0.3f;
    public float yOffset = -0.3f;

    // Количество карт в ячейке
    public int cardCount = 0;

    // Use this for initialization
    void Start()
    {
        // Назначить менеджер очередей
        turnManager = FindObjectOfType<TurnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // Отображение нижних карт игрового поля
        cardCount = 0;
        foreach (GameObject cardGO in allDownCards)
        {
            Card card = cardGO.GetComponent<Card>();
            CellGamePole cellPlayer = this;
            Vector3 newPositionForCard = transform.position;
            //card.SetOpen(true);
            card.backgroundSR.sortingOrder = 0;
            card.mastSR.sortingOrder = 1;
            card.valueSR.sortingOrder = 1;
            newPositionForCard = new Vector3(cellPlayer.transform.position.x + cardCount * xOffsetForNextCards, cellPlayer.transform.position.y, 0);
            if (card.stickOut)
            {
                newPositionForCard.y -= card.yOffsetMouseOver;
            }
            card.transform.position = newPositionForCard;
            cardCount++;
        }

        // Отображение верхних карт игрового поля
        cardCount = 0;
        foreach (GameObject cardGO in allUpCards)
        {
            // Поместить карту поверх другой
            Card card = cardGO.GetComponent<Card>();
            //CellGamePole cellPlayer = this;
            Vector3 newPositionForCard = transform.position;
            card.SetOpen(true);
            card.backgroundSR.sortingOrder = 2;
            card.mastSR.sortingOrder = 3;
            card.valueSR.sortingOrder = 3;
            newPositionForCard = new Vector3(card.transform.parent.position.x + xOffset, card.transform.parent.position.y + yOffset, -1);
            card.transform.position = newPositionForCard;
            cardCount++;
        }
    }

    // Подбор всех не отбитых карт
    public List<GameObject> GetNotBeatedCards()
    {
        List<GameObject> allNotBeatedCardsGO = new List<GameObject>();
        foreach (GameObject cardGO in allDownCards)
        {
            if (!cardGO.GetComponent<Card>().isBeated)
            {
                allNotBeatedCardsGO.Add(cardGO);
            }
        }

        return allNotBeatedCardsGO;
    }

    // Подбор всех не отбитых карт
    public List<GameObject> GetAllCards()
    {
        List<GameObject> allCardsGO = new List<GameObject>();

        foreach (GameObject cardGO in allUpCards)
        {
            allCardsGO.Add(cardGO);
        }
        foreach (GameObject cardGO in allDownCards)
        {
            allCardsGO.Add(cardGO);
        }

        //print("allCardsGO.Count = " + allCardsGO.Count);

        return allCardsGO;
    }
}


//card.inCellPlayer = true;
//float cellPlayerXOffsetForOpenCards = cardGenerator.cellPlayerXOffsetForOpenCards;
//float cellPlayerYOffsetForClosedCards = cardGenerator.cellPlayerYOffsetForClosedCards;
//if (iCellPlayer == 0)
//{//newPositionForCard = new Vector3(cellPlayer.transform.position.x, cellPlayer.transform.position.y - iCellPlayerCards * cellPlayerYOffsetForOpenCards, -cellPlayer.cardCount);
// = new Vector3(newPositionForCard.transform.position.x, transform.parent.position.y + yOffsetMouseOver, transform.position.z);
//transform.position = new Vector3(transform.position.x, transform.parent.position.y + yOffsetMouseOver, transform.position.z);
//}
//else if (iCellPlayer == 2)
//{
//    newPositionForCard = new Vector3(cellPlayer.transform.position.x + iCellPlayerCards * cellPlayerXOffsetForOpenCards, cellPlayer.transform.position.y, -cellPlayer.cardCount);
//}
//else if (iCellPlayer == 1 || iCellPlayer == 3)
//{
//    newPositionForCard = new Vector3(cellPlayer.transform.position.x, cellPlayer.transform.position.y - iCellPlayerCards * cellPlayerYOffsetForClosedCards, -cellPlayer.cardCount);
//}
//// Назначить генератор
//cardGenerator = FindObjectOfType<CardGenerator>();
// Пары карт
//// Генератор карт
//CardGenerator cardGenerator;
//public GameObject[] allDownCards = new GameObject[52];
//public GameObject[] allUpCards = new GameObject[52];
