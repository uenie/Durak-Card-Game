using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CellPlayer : MonoBehaviour
{
    //// Стиль отображения статуса
    //public GUIStyle statusStyle;

    // Отбой
    public bool otboy = false;
    public bool beru = false;

    // Перемещается ли одна из карт
    public bool mouseDragging = false;

    // Нажатие на пробел
    public bool spaceKeyDown = false;

    // Генератор карт
    CardGenerator cardGenerator;

    // Игровое поле
    CellGamePole cellGamePole;

    // Которая ячека
    public int iCellPlayer = -1;

    //// Ячейка игрока или компьютера
    //public bool isPlayer = false;

    // Менеджер очередей
    public TurnManager turnManager;

    // Соседи слева и справа - ходить левому соседу и отбиваться\подкидывать правому
    public CellPlayer prevCellPlayer;
    public CellPlayer nextCellPlayer;

    // Все карты в ячейке
    //public GameObject[] allCardsInPlayerCell = new GameObject[52];

    public List<GameObject> allCardsInPlayerCell0 = new List<GameObject>();
    public List<GameObject> allCardsInPlayerCell1 = new List<GameObject>();
    public List<GameObject> allCardsInPlayerCell2 = new List<GameObject>();
    public List<GameObject> allCardsInPlayerCell3 = new List<GameObject>();

    //public  Card[] allCardsInPlayerCell = null;

    // Количество карт в ячейке - вспомогательная переменная
    public int cardCount = 0;

    // Use this for initialization
    void Start()
    {
        // Назначить генератор
        cardGenerator = FindObjectOfType<CardGenerator>();

        // Назначить игровое поле
        cellGamePole = FindObjectOfType<CellGamePole>();

        // Назначить менеджер очередей
        turnManager = FindObjectOfType<TurnManager>();

        // Вывести статус
        //PrintStatus();
    }

    // Update is called once per frame
    void Update()
    {
        // Нажатие на пробел
        spaceKeyDown = Input.GetKeyDown(KeyCode.Space);

        // Поднятие 
        if (beru)
        {
            PickUp();
            beru = false;
        }

        //if (!turnManager.isFirstRound)
        if (turnManager.playersTurn)
        {
            // Неотбитые карты
            List<GameObject> allNotBeatedCardsGO = cellGamePole.GetNotBeatedCards();

            // Когда отбивающийся игрок отбивает все карты
            // Проверить на отбой у двух соседов отбивающегося
            if (allNotBeatedCardsGO.Count == 0 && prevCellPlayer.otboy && nextCellPlayer.otboy)
            {
                // Убрать карты в игровом поле в отбой
                turnManager.MoveGamePoleCardsToOtboy();

                // Назначить очередь отбившемуся
                turnManager.SetTurn(this);

                //print("otboy");
            }

            // Обновление компьютера
            if (tag == "AI")
            {
                // Отбивающийся комп
                if (this == turnManager.playersTurn.nextCellPlayer && cellGamePole.allDownCards.Count > 0)
                {
                    // Список карт игрока, которыми можно отбиваться
                    List<GameObject> allBeatingCardsGO = new List<GameObject>();

                    foreach (GameObject cardGO in allCardsInPlayerCell0)
                    {
                        if (cardGO.GetComponent<Card>().AllowedToBeatCount() > 0)
                        {
                            allBeatingCardsGO.Add(cardGO);
                        }
                    }
                    foreach (GameObject cardGO in allCardsInPlayerCell1)
                    {
                        if (cardGO.GetComponent<Card>().AllowedToBeatCount() > 0)
                        {
                            allBeatingCardsGO.Add(cardGO);
                        }
                    }
                    foreach (GameObject cardGO in allCardsInPlayerCell2)
                    {
                        if (cardGO.GetComponent<Card>().AllowedToBeatCount() > 0)
                        {
                            allBeatingCardsGO.Add(cardGO);
                        }
                    }
                    foreach (GameObject cardGO in allCardsInPlayerCell3)
                    {
                        if (cardGO.GetComponent<Card>().AllowedToBeatCount() > 0)
                        {
                            allBeatingCardsGO.Add(cardGO);
                        }
                    }

                    //print(name + ": allBeatingCardsGO.Count - " + allBeatingCardsGO.Count);

                    // Может ли отбиться
                    if (allBeatingCardsGO.Count > 0) // Есть чем отбиваться
                    {
                        // Отбиваться как можно слабыми картами
                        GameObject beatingCardGO = GetSmallestCardIn(allBeatingCardsGO);
                        //print(name + ": beatingCardGO - " + beatingCardGO.name);

                        // TODO: Отбиваться от сильных карт
                        foreach (GameObject cardGO in allNotBeatedCardsGO)
                        {
                            if (beatingCardGO.GetComponent<Card>().CanBeat(cardGO.GetComponent<Card>()))
                            {
                                beatingCardGO.GetComponent<Card>().Beat(cardGO.GetComponent<Card>());
                                break;
                            }
                        }
                    }
                    else if (prevCellPlayer.otboy && nextCellPlayer.otboy)
                    {
                        // Комп мгновенно поднимает, если не сможет отбиться
                        // TODO: Поднимать карты только с согласия соседей, после того, когда они подкинут все нужные карты
                        //print("Компьютер поднимает карты - " + name);
                        beru = true;
                    }
                }

                // Умный ход для искусственного интеллекта
                // Подкидывающий комп
                if (this == turnManager.playersTurn || prevCellPlayer.prevCellPlayer == turnManager.playersTurn)
                {
                    //print(name + ": CanGoToGamePole");

                    // Ходить как можно слабыми картами
                    GameObject cardToGoGO = GetSmallestCardToGo();

                    // Если нашли карту, которой можно ходить
                    if (cardToGoGO && CanGoToGamePole())
                    {
                        //print(name + ": cardToGoGO - " + cardToGoGO.name);

                        // Ходить можно столько же, сколько карт имеет отбивающийся игрок
                        Card cardToGo = cardToGoGO.GetComponent<Card>();
                        if (cardToGo)
                        {
                            //print(name + ": cardToGo");

                            // Раскрыть карту
                            cardToGo.SetOpen(true);

                            // Ход или подкидывание
                            cardToGo.AddToGamePole();
                        }
                        //if ()
                        //}
                        //{
                    }
                    // Если не нашли карту, которой можно ходить
                    // Если все карты в игровом поле отбиты 
                    //cellGamePole.allDownCards.Count == cellGamePole.allUpCards.Count || 
                    // или отбивающийся игрок берет карты
                    else if (cellGamePole.allDownCards.Count > 0 || turnManager.playersTurn.nextCellPlayer.beru)
                    {
                        // Отбой
                        otboy = true;
                    }
                    //else
                    //{
                    //    print("ожидание");
                    //}
                } // Подкидывающий комп
            } //if (tag == "AI") 

            // Обновление игрока
            if (tag == "Player")
            {
                // Отсортировать карты игрока
                SortPlayerCards();

                // Перемещается ли одна из карт
                bool cardIsDragging = false;
                foreach (GameObject cardGO in allCardsInPlayerCell0)
                {
                    if (cardGO.GetComponent<Card>().mouseDragging)
                    {
                        cardIsDragging = true;
                    }
                }
                foreach (GameObject cardGO in allCardsInPlayerCell1)
                {
                    if (cardGO.GetComponent<Card>().mouseDragging)
                    {
                        cardIsDragging = true;
                    }
                }
                foreach (GameObject cardGO in allCardsInPlayerCell2)
                {
                    if (cardGO.GetComponent<Card>().mouseDragging)
                    {
                        cardIsDragging = true;
                    }
                }
                foreach (GameObject cardGO in allCardsInPlayerCell3)
                {
                    if (cardGO.GetComponent<Card>().mouseDragging)
                    {
                        cardIsDragging = true;
                    }
                }
                if (cardIsDragging)
                {
                    mouseDragging = true;
                }
                else
                {
                    mouseDragging = false;
                }
            } //if (tag == "Player")

            // Отображение карт игрока
            cardCount = 0;
            // Отображение карт обычных мастей
            for (int i = 0; i < 4; i++)
            {
                if (i != turnManager.cellKozyr.cardKozyr.mast)
                {
                    if (i == 0)
                    {
                        ShowCards(allCardsInPlayerCell0);
                    }
                    if (i == 1)
                    {
                        ShowCards(allCardsInPlayerCell1);
                    }
                    if (i == 2)
                    {
                        ShowCards(allCardsInPlayerCell2);
                    }
                    if (i == 3)
                    {
                        ShowCards(allCardsInPlayerCell3);
                    }
                }
            }
            // Козыри всегда в крайне правой стороне
            if (turnManager.cellKozyr.cardKozyr.mast == 0)
            {
                ShowCards(allCardsInPlayerCell0);
            }
            if (turnManager.cellKozyr.cardKozyr.mast == 1)
            {
                ShowCards(allCardsInPlayerCell1);
            }
            if (turnManager.cellKozyr.cardKozyr.mast == 2)
            {
                ShowCards(allCardsInPlayerCell2);
            }
            if (turnManager.cellKozyr.cardKozyr.mast == 3)
            {
                ShowCards(allCardsInPlayerCell3);
            }
            //print(cardCount);
        }

        // Проверка на присутствие карт у игрока и в колоде
        if ((GetComponentInChildren<Card>() == null) &&
            (turnManager.cellKoloda.GetComponentInChildren<Card>() == null) &&
            (turnManager.cellKozyr.GetComponentInChildren<Card>() == null))
        {
            print("Выбывание игрока " + name);
            // Выбывание игрока при отсутствии карт
            turnManager.playersTurn = prevCellPlayer;
            turnManager.allCellPlayers.Remove(this);
            nextCellPlayer.prevCellPlayer = prevCellPlayer;
            prevCellPlayer.nextCellPlayer = nextCellPlayer;
            gameObject.SetActive(false);
        }
    }

    void FixedUpdate()
    {

    }

    void OnGUI()
    {
        if (tag == "Player")
        {
            // Статус
            GUI.Box(new Rect(10 + 110, 10, 300, 30), GetStatus());

            if (turnManager.playersTurn)
            {
                //print("GetNotBeatedCards().Count = " + cellGamePole.GetNotBeatedCards().Count);

                Rect buttonRectBito = new Rect(10, 10 + 35 + 35, 100, 30);
                Rect buttonRectBeru = new Rect(10, 10 + 35 + 35 + 35, 100, 30);
                string otboyNoCardsText = "";

                // Кнопка "Бери"
                if (cellGamePole.GetNotBeatedCards().Count > 0)
                {
                    otboyNoCardsText = "Бери";
                }
                // Кнопка "Бито"
                else if (cellGamePole.allDownCards.Count > 0 && (turnManager.playersTurn == this || turnManager.playersTurn == prevCellPlayer.prevCellPlayer))
                {
                    otboyNoCardsText = "Бито";
                }
                if (otboyNoCardsText != "" && (this == turnManager.playersTurn || turnManager.playersTurn == prevCellPlayer.prevCellPlayer) && (GUI.Button(buttonRectBito, otboyNoCardsText) || spaceKeyDown))
                {
                    otboy = true;
                }
                // Кнопка "Беру"
                else if (turnManager.playersTurn == prevCellPlayer && (GUI.Button(buttonRectBeru, "Беру") || spaceKeyDown))
                {
                    beru = true;
                }
            }
        }
    }

    //// Количество всех карт игрока
    //public int AllCardsCount()
    //{
    //    return allCardsInPlayerCell0.Count + allCardsInPlayerCell1.Count + allCardsInPlayerCell2.Count + allCardsInPlayerCell3.Count;
    //}

    // Поднятие всех карт игрового поля отбивающимся игроком
    void PickUp()
    {
        // Поднятие
        AddAllCardsFrom(cellGamePole.allUpCards);
        AddAllCardsFrom(cellGamePole.allDownCards);

        // Назначить очередь левому соседу отбивающегося игрока
        turnManager.SetTurn(nextCellPlayer);
    }

    // Подсчет всех карт игрока
    public int CardCountInCellPlayer(CellPlayer cellPlayer)
    {
        int allCardsCount =
            cellPlayer.allCardsInPlayerCell0.Count +
            cellPlayer.allCardsInPlayerCell1.Count +
            cellPlayer.allCardsInPlayerCell2.Count +
            cellPlayer.allCardsInPlayerCell3.Count;

        return allCardsCount;
    }

    // Можно ли ходить игроку
    public bool CanGoToGamePole()
    {
        // Количество не отбитых карт на игровом поле
        //List<GameObject> allDownCardsNotBeated = new List<GameObject>();
        int allDownCardsNotBeatedCount = 0;
        foreach (GameObject cardGO in cellGamePole.allDownCards)
        {
            if (!cardGO.GetComponent<Card>().isBeated)
            {
                allDownCardsNotBeatedCount++;
            }
        }

        //print(name + ": allDownCardsNotBeatedCount - " + allDownCardsNotBeatedCount);
        //print(name + ": CardCountInCellPlayer(turnManager.playersTurn.nextCellPlayer) - " + CardCountInCellPlayer(turnManager.playersTurn.nextCellPlayer));

        if ((turnManager.playersTurn == this || turnManager.playersTurn == prevCellPlayer.prevCellPlayer) // Если игрок ходит или подкидывает
            && allDownCardsNotBeatedCount < CardCountInCellPlayer(turnManager.playersTurn.nextCellPlayer) // Если у отбивающегося игрока есть чем отбиваться
            )//&& !turnManager.playersTurn.nextCellPlayer.beru) // Если отбивающийся игрок не берет
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Отбор самой маленькой карты из всех карт игрока для хода или подкидывания
    GameObject GetSmallestCardToGo()
    {
        GameObject smallestCardToGoGO = null;
        List<GameObject> smallestCardsToGo = new List<GameObject>
            {
                GetSmallestCardToGoIn(allCardsInPlayerCell0),
                GetSmallestCardToGoIn(allCardsInPlayerCell1),
                GetSmallestCardToGoIn(allCardsInPlayerCell2),
                GetSmallestCardToGoIn(allCardsInPlayerCell3)
            };
        smallestCardToGoGO = GetSmallestCardToGoIn(smallestCardsToGo);
        return smallestCardToGoGO;
    }

    GameObject GetSmallestCardToGoIn(List<GameObject> otherCardsGO)
    {
        List<GameObject> cardsAllowedToGo = new List<GameObject>();
        GameObject smallestCardAllowedToGo = null;

        foreach (GameObject otherCardGO in otherCardsGO)
        {
            if (otherCardGO)
            {
                Card otherCard = otherCardGO.GetComponent<Card>();

                // Если можно ходить заданной картой
                if (otherCard.AllowedToGo())
                {
                    cardsAllowedToGo.Add(otherCardGO);
                }
            }
        }

        if (cardsAllowedToGo.Count > 0)
        {
            smallestCardAllowedToGo = GetSmallestCardIn(cardsAllowedToGo);
            //print();

        }

        return smallestCardAllowedToGo;
    }

    // Отбор самой маленькой карты из заданных карт
    GameObject GetSmallestCardIn(List<GameObject> otherCardsGO)
    {
        GameObject smallestCardGO = null;

        // Отобрать самую маленькую карту из заданных карт
        foreach (GameObject otherCardGO in otherCardsGO)
        {
            if (otherCardGO)
            {
                Card otherCard = otherCardGO.GetComponent<Card>();

                // Если одна карта уже выбрана
                if (smallestCardGO)
                {
                    Card cardToGo = smallestCardGO.GetComponent<Card>();

                    // Козырная масть
                    int mastKozyr = turnManager.cellKozyr.cardKozyr.mast;

                    // Если заданная карта - козырь
                    if (otherCard.mast == mastKozyr)
                    {
                        // Если выбранная карта - козырь
                        if (cardToGo.mast == mastKozyr)
                        {
                            // Если заданная карта меньше выбранной по величине
                            if (otherCard.value < cardToGo.value)
                            {
                                // Назначить карту для хода
                                smallestCardGO = otherCardGO;
                            }
                        }

                        // Если выбранная карта - не козырь
                        if (cardToGo.mast != mastKozyr) { }
                    }

                    // Если заданная карта - не козырь
                    if (otherCard.mast != mastKozyr)
                    {
                        // Если выбранная карта - козырь
                        if (cardToGo.mast == mastKozyr)
                        {
                            // Назначить карту для хода
                            smallestCardGO = otherCardGO;
                        }

                        // Если выбранная карта - не козырь
                        if (cardToGo.mast != mastKozyr)
                        {
                            // Если заданная карта меньше выбранной по величине
                            if (otherCard.value < cardToGo.value)
                            {
                                // Назначить карту для хода
                                smallestCardGO = otherCardGO;
                            }
                        }
                    }
                }
                // Если ни одна карта еще не выбрана, т.е. первый цикл
                else
                {
                    // Назначить карту для хода
                    smallestCardGO = otherCardGO;
                }
            }
        }

        return smallestCardGO;
    }

    // Отображение карт игрока
    void ShowCards(List<GameObject> cards)
    {
        foreach (GameObject cardGO in cards)
        {
            Card card = cardGO.GetComponent<Card>();
            if (!card.mouseDragging)
            {
                //Card card = cardGO.GetComponent<Card>();
                // Поместить карту поверх другой
                card.backgroundSR.sortingOrder = cardCount;
                card.mastSR.sortingOrder = cardCount + 1;
                card.valueSR.sortingOrder = cardCount + 1;

                // Переместить карту
                CellPlayer cellPlayer = this;
                int iCellPlayerCards = cardCount;
                float cellPlayerXOffsetForOpenCards = cardGenerator.cellPlayerXOffsetForOpenCards;
                float cellPlayerXOffsetForClosedCards = cardGenerator.cellPlayerYOffsetForClosedCards;
                float cellPlayerYOffsetForClosedCards = cardGenerator.cellPlayerYOffsetForClosedCards;

                Vector3 newPositionForCard = transform.position;
                if (iCellPlayer == 0) // Отображение карт игрока
                {
                    card.SetOpen(true);
                    newPositionForCard = new Vector3(cellPlayer.transform.position.x + iCellPlayerCards * cellPlayerXOffsetForOpenCards, cellPlayer.transform.position.y, -cellPlayer.cardCount);
                    //newPositionForCard = new Vector3(cellPlayer.transform.position.x, cellPlayer.transform.position.y - iCellPlayerCards * cellPlayerYOffsetForOpenCards, -cellPlayer.cardCount);

                    if (card.stickOut)
                    {
                        newPositionForCard.y += card.yOffsetMouseOver;// = new Vector3(newPositionForCard.transform.position.x, transform.parent.position.y + yOffsetMouseOver, transform.position.z);
                        //transform.position = new Vector3(transform.position.x, transform.parent.position.y + yOffsetMouseOver, transform.position.z);

                    }
                }
                else if (iCellPlayer == 1) // Отображение карт 1-компьютера
                {
                    card.SetOpen(false);
                    newPositionForCard = new Vector3(cellPlayer.transform.position.x + iCellPlayerCards * cellPlayerXOffsetForClosedCards, cellPlayer.transform.position.y, -cellPlayer.cardCount);
                }
                else if (iCellPlayer == 2) // Отображение карт 2-компьютера
                {
                    card.SetOpen(false);
                    newPositionForCard = new Vector3(cellPlayer.transform.position.x, cellPlayer.transform.position.y - iCellPlayerCards * cellPlayerYOffsetForClosedCards, -cellPlayer.cardCount);
                }

                card.transform.position = newPositionForCard;

                //card.inCellPlayer = true;
            }
            cardCount++;
        }
    }

    // Сортировка карт игрока
    void SortPlayerCards()
    {
        allCardsInPlayerCell0.Sort(SortCardByValue);
        allCardsInPlayerCell1.Sort(SortCardByValue);
        allCardsInPlayerCell2.Sort(SortCardByValue);
        allCardsInPlayerCell3.Sort(SortCardByValue);
    }

    void AddAllCardsFrom(List<GameObject> cardsGO)
    {
        // Перенести
        foreach (GameObject cardGO in cardsGO)
        {
            // Не бито
            cardGO.GetComponent<Card>().isBeated = false;

            // Добавить карту в массив
            AddCardGO(cardGO);

            // Назначить новый родительский Transform
            cardGO.transform.parent = transform;
        }

        // Убрать карты из массива карт
        cardsGO.Clear();
    }

    // Добавление карты в массива
    public void AddCardGO(GameObject cardGO)
    {
        if (cardGO.GetComponent<Card>().mast == 0)
        {
            allCardsInPlayerCell0.Add(cardGO);
            return;
        }
        if (cardGO.GetComponent<Card>().mast == 1)
        {
            allCardsInPlayerCell1.Add(cardGO);
            return;
        }
        if (cardGO.GetComponent<Card>().mast == 2)
        {
            allCardsInPlayerCell2.Add(cardGO);
            return;
        }
        if (cardGO.GetComponent<Card>().mast == 3)
        {
            allCardsInPlayerCell3.Add(cardGO);
            return;
        }
    }

    // Удаление карты из массива
    public void RemoveCardGO(GameObject cardGO)
    {
        if (cardGO.GetComponent<Card>().mast == 0)
        {
            allCardsInPlayerCell0.Remove(cardGO);
            return;
        }
        else if (cardGO.GetComponent<Card>().mast == 1)
        {
            allCardsInPlayerCell1.Remove(cardGO);
            return;
        }
        else if (cardGO.GetComponent<Card>().mast == 2)
        {
            allCardsInPlayerCell2.Remove(cardGO);
            return;
        }
        else if (cardGO.GetComponent<Card>().mast == 3)
        {
            allCardsInPlayerCell3.Remove(cardGO);
            return;
        }
    }

    // Метод, меняющий местами две карты
    void SwapCards(Card[] cards, int indexA, int indexB)
    {
        Card tmp = cards[indexA];
        cards[indexA] = cards[indexB];
        cards[indexB] = tmp;
    }

    // Метод для сортировки карт по значению номинала
    int SortCardByValue(GameObject card1, GameObject card2)
    {
        // Отсортировать по величине
        // Начиная с меньших по возрастанию, слева направо
        return card1.GetComponent<Card>().value.CompareTo(card2.GetComponent<Card>().value);
    }

    // Метод для сортировки карт по масти
    int SortCardByMast(GameObject card1, GameObject card2)//Card card1, Card card2
    {
        return card1.GetComponent<Card>().mast.CompareTo(card2.GetComponent<Card>().mast);
    }

    // Метод для сортировки карт по масти для козырей
    int SortCardByMastKozyr(GameObject card1, GameObject card2)//Card card1, Card card2
    {
        if (card1.GetComponent<Card>().mast == turnManager.cellKozyr.cardKozyr.mast && card2.GetComponent<Card>().mast != turnManager.cellKozyr.cardKozyr.mast)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }

    // Получение статуса игрока
    public string GetStatus()
    {
        // Сообщения
        //string allowedActionsString = "Вообще, могу ходить левому соседу - " + nextCellPlayer.name + ", \nподкидывать/отбиваться от правого соседа - " + prevCellPlayer.name;
        string currrentActionString = "Сейчас ";

        // Состояние игрока - ходит, отбивается, ничего не делает или может только подкидывать
        if (turnManager.playersTurn == this)
        {
            currrentActionString += "хожу";
        }
        else if (turnManager.playersTurn == prevCellPlayer)
        {
            currrentActionString += "отбиваюсь";
        }
        else if (turnManager.playersTurn == prevCellPlayer.prevCellPlayer)
        {
            currrentActionString += "могу только подкидывать";
        }
        else if (turnManager.playersTurn == nextCellPlayer)
        {
            currrentActionString += "ничего не делаю";
        }

        //// Вывести сообщение
        //print(name + ": " + currrentActionString + "\n" + allowedActionsString);

        //return (name + ": " + currrentActionString);
        return (currrentActionString);
    }
}



//// Кнопка "Я отбился"
//if (tag == "Player" && prevCellPlayer == turnManager.playersTurn && nextCellPlayer.otboy && prevCellPlayer.otboy && GUI.Button(new Rect(10, 10 + 35 + 35 + 35 + 35, 100, 30), "Я отбился"))
//{
//    print("Я отбился");
//}

//return card1.GetComponent<Card>().mast.CompareTo(card2.GetComponent<Card>().mast);

// Отсортировать по масти
//cards.Sort(SortCardByMast);

//foreach (List<GameObject> cards in allCardsInPlayerCell0)
//{
//    cards.Sort(SortCardByValue);
//}

//allCardsInPlayerCell.Sort(SortCardByMastKozyr);
//print("cardCount: " + cardCount);


//void Sorting()
//{
//    //if (mastCurrent == turnManager.cellKozyr.cardKozyr.mast)
//    //{
//    //    if (card1.mast == mastCurrent && card2.mast == mastCurrent)
//    //    {
//    //        //print(card1.value.CompareTo(card2.value));
//    //        return card1.value.CompareTo(card2.value);
//    //    }
//    //    else
//    //    {
//    //        //return -1;
//    //        return card1.mast.CompareTo(card2.mast);
//    //    }
//    //}
//    //else
//    //{
//    //    return card1.value.CompareTo(card2.value);
//    //}

//    //for (int i = 0; i < allCardsInPlayerCell.Count; i++)
//    //{
//    //    Card card = allCardsInPlayerCell[i];
//    //    if (card.mast == iMast)
//    //    {
//    //        Card[] cardsArray = allCardsInPlayerCell.ToArray();
//    //        SwapCards(cardsArray, i, 0);

//    //        List<Card> newList = new List<Card>();
//    //        foreach (Card cardInArray in cardsArray)
//    //        {
//    //            newList.Add(cardInArray);
//    //        }

//    //        allCardsInPlayerCell = newList;
//    //    }
//    //}

//    //foreach (Card card in allCardsInPlayerCell)
//    //{
//    //    if (card.mast == mastCurrent)
//    //    {
//    //        Card cardTempLast = allCardsInPlayerCell[allCardsInPlayerCell.Count-1];
//    //        Card cardTempCur = card;

//    //        allCardsInPlayerCell[allCardsInPlayerCell.Count - 1] = card;
//    //        cardTempCur = cardTempLast;

//    //        //cardTemp = card;
//    //        //allCardsInPlayerCell[0] = card;

//    //        //allCardsInPlayerCell.Remove(card);
//    //        //allCardsInPlayerCell.Add(cardTemp);
//    //    }
//    //}


//    //return card1.mast.CompareTo(card2.mast);

//    //if (card1.mast == mastCurrent && card2.mast == mastCurrent)
//    //{
//    //    //print(card1.value.CompareTo(card2.value));
//    //    return card1.value.CompareTo(card2.value);
//    //}
//    //else
//    //{
//    //    //return -1;
//    //    return card1.mast.CompareTo(card2.mast);
//    //}

//    //if (card2.mast == turnManager.cellKozyr.cardKozyr.mast)
//    //{
//    //    return -1;
//    //}
//    //else
//    //{
//    //    return card1.mast.CompareTo(card2.mast);
//    //} 
//    //allCardsInPlayerCell.Sort(SortCardByMast);
//    //mastCurrent = i;
//    //mastCurrent = 0;


//List<GameObject> newList = new List<GameObject>();

//// Отсортировать по масти
//for (int iMast = 0; iMast < 4; iMast++)
//{
//    // Сортировать по масти
//    for (int i = 0; i < allCardsInPlayerCell.Count; i++)
//    {
//        GameObject card = allCardsInPlayerCell[i];
//        if (card.GetComponent<Card>().mast == iMast)
//        {
//            newList.Add(card);
//        }
//    }
//}

//allCardsInPlayerCell.Clear();

//for (int i = 0; i < newList.Count; i++)
//{
//    newList.Add(allCardsInPlayerCell[i]);
//    //GameObject card = allCardsInPlayerCell[i];
//    //if (card.GetComponent<Card>().mast == iMast)
//    //{
//    //}
//}
//}


//IComparer cardByValueComparer = new CardByValueComparer();


//Array.Sort(cards, cardByValueComparer);



//foreach (List<GameObject> list in allCardsInPlayerCell)
//{
//    list.Remove(targetCardGO);

//    //foreach (GameObject cardGO in list)
//    //{
//    //    if (cardGO == targetCardGO)
//    //    {

//    //    }
//    //}
//}

//foreach (List<GameObject> list in allCardsInPlayerCell)
//{
//    list.Add(cardGO);
//}
//allCardsInPlayerCell[cardGO.GetComponent<Card>().mast][allCardsInPlayerCell[cardGO.GetComponent<Card>().mast].Length] = cardGO;


//public class CardByValueComparer : IComparer
//{
//    // Call CaseInsensitiveComparer.Compare with the parameters reversed.
//    public int Compare(GameObject card1, GameObject card2)
//    {
//        return card1.GetComponent<Card>().value.CompareTo(card2.GetComponent<Card>().value);
//        //return Compare(y, x);
//    }
//}


//// Отбивающийся игрок
//CellPlayer beatingPlayer = turnManager.playersTurn.nextCellPlayer;



//foreach (GameObject cardGO in cardsGO)
//{
//    cardsGO.Remove(cardGO);
//}
//print("Ход или подкидывание - " + name);


// Тест отбора маленькой карты
//print(name + " ходит " + cardToGoGO.name);


//print("cellGamePole.allDownCards.Count=" + cellGamePole.allDownCards.Count);
//print("cellGamePole.allUpCards.Count=" + cellGamePole.allUpCards.Count);

//print(name + ": otboy = true");

//print(name + ": cardToGoGO.name = " + cardToGoGO.name);
