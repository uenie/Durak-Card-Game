using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CardGenerator : MonoBehaviour
{
    // Префаб ячейки для игроков
    public GameObject cellPlayerPrefab;
    public float cellPlayerXOffset = 1f;
    public float cellPlayerYOffset = 1f;
    public float cellPlayerYOffsetForClosedCards = 0.2f;
    public float cellPlayerYOffsetForOpenCards = 0.5f;
    public float cellPlayerXOffsetForClosedCards = 0.2f;
    public float cellPlayerXOffsetForOpenCards = 0.5f;
    public CellPlayer[] allCellPlayers = new CellPlayer[4];

    // Префаб ячейки для отбоя
    //public GameObject cellBitoPrefab;
    public float cellBitoXOffset = 3f;
    public GameObject cellBito;

    // Колода
    public CellKoloda cellKoloda;
    public CellKozyr cellKozyr = null;

    // Все 36/52/24 карты
    public GameObject[] allCards = new GameObject[36];

    // Коэффициенты расстояния между картами
    public float offsetX = 3;
    public float offsetY = 4;

    // Префаб карты
    public GameObject cardPrefab;

    // Масть: Червы (♥), Бубны (♦), Трефы (♣), Пики (♠)
    public Sprite[] mastSprites = new Sprite[4];

    // Значение - величина
    // 13 величин по возрастанию: 1(Туз), 2...10, 11(Валет), 12(Дама), 13(Король)
    // 52 вида - 13 величин * 4 масти = 52 варианта карт
    public Sprite[] cardValueTexturesBlack = new Sprite[13];
    public Sprite[] cardValueTexturesRed = new Sprite[13];

    // Use this for initialization
    void Awake()
    {
        // Найти из сцены ячейку колоды
        cellKoloda = GameObject.FindObjectOfType<CellKoloda>();

        // Найти из сцены ячейку козыря
        cellKozyr = GameObject.FindObjectOfType<CellKozyr>();

        SpawnAllCards();
        Swap();

        // Назначить индексы игрокам
        for (int iCellPlayer = 0; iCellPlayer < allCellPlayers.Length; iCellPlayer++)
        {
            // Ячейка игрока
            CellPlayer cellPlayer = allCellPlayers[iCellPlayer].GetComponent<CellPlayer>();

            cellPlayer.iCellPlayer = iCellPlayer;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SpawnAllCards()
    {
        // Индекс для массива карт
        int allCardsIndex = 0;

        // Создать все карты
        for (int iMast = 0; iMast <= 3; iMast++)
        {
            for (int iValue = 4; iValue <= 12; iValue++)
            {
                // Создать карту в колоде
                Vector3 newPosition = new Vector3(cellKoloda.transform.position.x, cellKoloda.transform.position.y, -cellKoloda.cardCount - 1);
                GameObject cardGO = Instantiate(cardPrefab, newPosition, Quaternion.identity);
                Card card = cardGO.GetComponent<Card>();
                //cellKoloda.allCardsInKoloda[cellKoloda.cardCount] = cardGO;
                cardGO.transform.parent = cellKoloda.transform;

                // Применить текстуру масти
                card.mast = iMast;
                card.mastSR.sprite = mastSprites[iMast];

                // Установить цвет
                if (card.mast < 2)
                {
                    card.isCardColorRed = true;
                }
                else
                {
                    card.isCardColorRed = false;
                }

                // Применить текстуру значения
                card.value = iValue + 2;
                if (card.isCardColorRed)
                {
                    card.valueSR.sprite = cardValueTexturesRed[iValue];
                }
                else
                {
                    card.valueSR.sprite = cardValueTexturesBlack[iValue];
                }

                // Раскрыть карту
                card.SetOpen(false);

                // Переименовать карту
                card.name = "Card (" + card.value + ", " + (card.isCardColorRed ? "Red" : "Black") + " (" + card.mastSR.sprite.name + "))";

                // Добавить карту в массив карт колоды
                allCards[allCardsIndex] = card.gameObject;
                allCardsIndex++;
                cellKoloda.cardCount++;
            }
        }
    }

    void Swap()
    {
        // Перемешать карты
        int[] allCardsIndexes = new int[36];
        for (int i = 0; i < allCardsIndexes.Length; i++)
        {
            allCardsIndexes[i] = i;
            //print(allCardsIndexes[i]);
        }
        for (int i = 0; i < allCardsIndexes.Length; i++)
        {
            int index = allCardsIndexes[i];
            int randomIndex = UnityEngine.Random.Range(0, allCardsIndexes.Length - 1);
            allCardsIndexes[i] = allCardsIndexes[randomIndex];
            allCardsIndexes[randomIndex] = index;
            // print(allCardsIndexes[i]);
        }

        // Заполнение колоды картами
        cellKoloda.cardCount = 0;
        for (int i = allCards.Length - 1; i >= 0; i--)
        {
            // Текущая карта
            GameObject cardGO = allCards[allCardsIndexes[i]];
            Card card = cardGO.GetComponent<Card>();
            //cellKoloda.allCardsInKoloda[i] = cardGO;

            // Поместить карту поверх другой
            Vector3 newPosition = new Vector3(cellKoloda.transform.position.x, cellKoloda.transform.position.y, -cellKoloda.cardCount - 1);
            cardGO.transform.position = newPosition;
            card.backgroundSR.sortingOrder = cellKoloda.cardCount;
            card.mastSR.sortingOrder = cellKoloda.cardCount + 1;
            card.valueSR.sortingOrder = cellKoloda.cardCount + 1;

            // Увеличить счетчик колоды - количество карт
            cellKoloda.cardCount++;

            // Первую, самую нижнюю карту раскрыть и сделать козырем
            if (i == allCards.Length - 1)
            {
                card.SetOpen(true);

                cellKozyr.cardKozyr = card;
                card.transform.parent = cellKozyr.transform;

                card.transform.position = cellKozyr.transform.position;
                card.transform.rotation = cellKozyr.transform.rotation;
                //print("Козырная масть - " + cellKozyr.cardKozyr.mast);
            }
        }
    }
}



//// Создавать все карты, кроме 2-5
//if (iValue == 13)
//{
//    iValue = 0;
//}

//card.isOpen = true;

//// Создавать все карты, кроме 2-5
//if (iValue == 0)
//{
//    iValue = 13;
//}




//// Поместить карту поверх другой
//card.backgroundSR.sortingOrder = cellPlayer.cardCount;
//card.mastSR.sortingOrder = cellPlayer.cardCount + 1;
//card.valueSR.sortingOrder = cellPlayer.cardCount + 1;
//cellPlayer.cardCount++;
//card.inCellPlayer = true;

//// Уже не в колоде
//card.vKolode = false;

// cellPlayerPrefab
//// Instantiating
//Vector3 newPositionForCellPlayer = new Vector3(cellPlayerPrefab.transform.position.x + iCellPlayer * cellPlayerXOffset, cellPlayerPrefab.transform.position.y, cellPlayerPrefab.transform.position.z);
//GameObject cellPlayerGO = Instantiate(cellPlayerPrefab, newPositionForCellPlayer, Quaternion.identity);
//allCellPlayer[iCellPlayer] = cellPlayerGO;
//cellPlayer.allCardsInPlayerCell[iCellPlayerCards] = card.gameObject;

//// Переместить карту
//Vector3 newPositionForCard;
//if (iCellPlayer == 0)
//{
//    // Раскрыть карту игрока
//    card.isOpen = true;
//    //card.isFirst = true;
//    card.ApplySettings();
//    newPositionForCard = new Vector3(cellPlayer.transform.position.x + iCellPlayerCards * cellPlayerXOffsetForOpenCards, cellPlayer.transform.position.y, -cellPlayer.cardCount);
//    //newPositionForCard = new Vector3(cellPlayer.transform.position.x, cellPlayer.transform.position.y - iCellPlayerCards * cellPlayerYOffsetForOpenCards, -cellPlayer.cardCount);
//}
//else if (iCellPlayer == 2)
//{
//    newPositionForCard = new Vector3(cellPlayer.transform.position.x + iCellPlayerCards * cellPlayerXOffsetForOpenCards, cellPlayer.transform.position.y, -cellPlayer.cardCount);
//}
//else
//{
//    newPositionForCard = new Vector3(cellPlayer.transform.position.x, cellPlayer.transform.position.y - iCellPlayerCards * cellPlayerYOffsetForClosedCards, -cellPlayer.cardCount);
//}

//card.transform.position = newPositionForCard;



//// Создание 4 ячеек
//for (int iCell4 = 0; iCell4 < 4; iCell4++)
//{
//    Vector3 newPositionForCell4 = new Vector3(cell4Prefab.transform.position.x + iCell4 * cell4XOffset, cell4Prefab.transform.position.y, cell4Prefab.transform.position.z);
//    GameObject cell4GO = Instantiate(cell4Prefab, newPositionForCell4, Quaternion.identity);
//    allCell4[iCell4] = cell4GO;
//}

//// Проверка на победу
//bool win = true;
//foreach (GameObject cardGO in allCards)
//{
//    if (!cardGO.GetComponent<Card>().isOpen)
//    {
//        win = false;
//    }
//}
//if (win)
//{
//    int winW = 250, winH = 50;
//    int x = Screen.width / 2 - winW / 2, y = Screen.height / 2 - winH / 2;
//    GUI.Box(new Rect(x, y, winW, winH), "Win!");
//    // print("win - " + Screen.width + " - " + Screen.height);
//    // print("win - " + x + " - " + y);
//}

//Razdat();




//// Найти из сцены все ячейки игроков
//allCellPlayers = FindObjectsOfType<CellPlayer>();


