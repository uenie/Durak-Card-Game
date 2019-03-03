using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    // Тестовый режим
    public bool testingMode = false;
    public int testingModePlayerTurn = 0;

    // Чья очередь ходить
    public CellPlayer playersTurn;

    // CardGenerator
    public CardGenerator cardGenerator;

    // Колода
    public CellKoloda cellKoloda;

    // Ячейка козыря
    public CellKozyr cellKozyr;

    // Ячейка игрового поля
    CellGamePole cellGamePole;

    // Ячейка отбоя
    CellBito cellBito;

    // Все ячейки игроков
    public List<CellPlayer> allCellPlayers = new List<CellPlayer>();

    // Назначена ли очередь какому-либо игроку в начале игры
    public bool finalTurnAtBeginning = false;

    // Первый ли раунд
    public bool isFirstRound = true;

    // Первый ли раунд
    public bool isFirstGO = true;

    // Use this for initialization
    void Awake()
    {
        // Найти из сцены CardGenerator
        cardGenerator = GameObject.FindObjectOfType<CardGenerator>();

        // Найти из сцены ячейку колоды
        cellKoloda = GameObject.FindObjectOfType<CellKoloda>();

        // Найти из сцены ячейку козыря
        cellKozyr = GameObject.FindObjectOfType<CellKozyr>();

        // Найти из сцены игровое поле
        cellGamePole = FindObjectOfType<CellGamePole>();

        // Найти из сцены отбой
        cellBito = FindObjectOfType<CellBito>();
    }

    // Update is called once per frame
    void Update()
    {
        // Первый раунд
        if (isFirstRound)
        {
            // Раздача
            foreach (CellPlayer cellPlayer in allCellPlayers)
            {
                FillCellPlayer(cellPlayer);
            }

            if (testingMode)
            {
                // TODO: Пока для тестирования ставлю очередь четвертому игроку, т.е. компу - после теста отключу testingMode
                playersTurn = allCellPlayers[testingModePlayerTurn];
            }
            else
            {
                // Первый раунд, ходит тот игрок, у кого наименьший козырь
                // Определить, кто будет ходить первым
                // Найден ли козырь у кого-нибудь
                bool kozyrFound = false;

                // Минимальный козырь
                int minKozyr = 15;

                // Перебор всех игроков
                foreach (CellPlayer player in allCellPlayers)
                {
                    // Перебор всех карт игрока
                    List<GameObject> cards = new List<GameObject>();

                    if (cellKozyr.cardKozyr.mast == 0)
                    {
                        cards = player.allCardsInPlayerCell0;
                    }
                    else if (cellKozyr.cardKozyr.mast == 1)
                    {
                        cards = player.allCardsInPlayerCell1;
                    }
                    else if (cellKozyr.cardKozyr.mast == 2)
                    {
                        cards = player.allCardsInPlayerCell2;
                    }
                    else if (cellKozyr.cardKozyr.mast == 3)
                    {
                        cards = player.allCardsInPlayerCell3;
                    }

                    // Установка очереди игрока
                    //void SetTurnByMinMast(List<GameObject> cards, bool kozyrFound, int minKozyr, CellPlayer player)
                    //{
                    //}
                    if (cards.Count > 0)
                    {
                        // Выбрать того игрока, у которого наименьший козырь
                        foreach (GameObject cardGO in cards)
                        {
                            Card card = cardGO.GetComponent<Card>();

                            // Проверка масти карты
                            if (card.mast == cellKozyr.cardKozyr.mast)
                            {
                                // Нашли козырь
                                kozyrFound = true;

                                // Если нашли минимальный козырь у игрока
                                if (card.value < minKozyr)
                                {
                                    // Назначим очередь игроку и изменим минимальный козырь
                                    playersTurn = player;
                                    minKozyr = card.value;
                                }
                            }
                        }
                    }
                }

                // Если нашли козырь
                if (kozyrFound)
                {
                    print("Найден минимальный козырь у игрока " + playersTurn.name);
                    //FindObjectOfType<Menu>().RestartGame();
                }
                // Если не нашли козырь ни у кого
                else
                {
                    // Назначим очередь первому игроку
                    playersTurn = allCellPlayers[0];

                    print("Не найден козырь ни у кого, очередь назначен первому игроку - " + playersTurn.name);
                }
            }

            finalTurnAtBeginning = true;

            // Уже не первый раунд
            isFirstRound = false;
        }
    }

    // Убрать карты в игровом поле в отбой
    public void MoveGamePoleCardsToOtboy()
    {
        cellBito.AddCardsFrom(cellGamePole.allUpCards);
        cellBito.AddCardsFrom(cellGamePole.allDownCards);
    }

    // Изменение очереди
    public void SetTurn(CellPlayer cellPlayer)
    {
        // Сбросить отбой у игроков
        foreach (CellPlayer player in allCellPlayers)
        {
            player.otboy = false;
        }

        // Прежняя очередь
        CellPlayer oldTurn = playersTurn;

        // Конец раунда
        // Заполнить карты игроков до шести карт
        //  Сначала заполнить до шести карт карты игрока, который ходил, и потом следующего игрока и т.д.
        //   Или поочередно, каждому игроку по одной карте по кругу - дополнительный параметр
        // TODO: Сделать универсальное заполнение карт, для разного количества игроков
        FillCellPlayer(oldTurn);
        FillCellPlayer(oldTurn.nextCellPlayer);
        FillCellPlayer(oldTurn.nextCellPlayer.nextCellPlayer);
        //FillCellPlayer(oldTurn.nextCellPlayer.nextCellPlayer.nextCellPlayer);

        // Назначим очередь заданному игроку
        playersTurn = cellPlayer;

        print("Очередь изменена. Теперь ходит " + playersTurn.name);
    }

    // Заполнение картами ячейки заданного игрока до шести карт
    void FillCellPlayer(CellPlayer cellPlayer)
    {
        while (cellPlayer.CardCountInCellPlayer(cellPlayer) < 6)
        {
            // Отобрать случайную карту из колоды
            Card[] allCards = FindObjectsOfType<Card>();
            Card[] cardsInKoloda = new Card[36];
            //List<Card> cardsInKoloda = new List<Card>();
            int index = 0;
            foreach (Card cardInKoloda in allCards)
            {
                if (cardInKoloda.transform.parent == cellKoloda.transform)
                {
                    cardsInKoloda[index] = cardInKoloda;
                    index++;
                }
            }

            if (index > 0) // Взять карту из колоды
            {
                // Переместить карту из колоды в ячейку
                Card card = cardsInKoloda[Random.Range(0, index)];

                // Назначить карту игроку
                cellPlayer.AddCardGO(card.gameObject);

                // Сделать родителем карты ячейку соответствующего игрока
                card.gameObject.transform.parent = cellPlayer.transform;
            }
            else
            {
                // Взять карту из ячейки козыря
                Card cardKozyr = cellKozyr.GetComponentInChildren<Card>();
                if (cardKozyr)
                {
                    // Поменять вращение карты
                    cardKozyr.gameObject.transform.rotation = cellKoloda.transform.rotation;

                    // Закрыть карту
                    cardKozyr.SetOpen(false);

                    // Назначить карту-козырь игроку
                    cellPlayer.AddCardGO(cardKozyr.gameObject);

                    // Сделать родителем карты ячейку соответствующего игрока
                    cardKozyr.gameObject.transform.parent = cellPlayer.transform;
                }
                else
                {
                    break;
                }
            }
        }
    }
}

//print("Менеджер очередей нашел у игрока " + player.name + " наименьший козырь: " + cardGO.name);

//print("Менеджер очередей перебирает карты игрока " + player.name);

//foreach (List<GameObject> list in player.allCardsInPlayerCell)
//{
//}
//print("Менеджер очередей назначил очередь игроку " + player.name);
//}
//print("Менеджер очередей назначил очередь игроку " + playersTurn.name);
//print("Менеджер очередей перебирает карту " + cardGO.name);

// Назначим очередь первому игроку
//playersTurn = players[0];
// Назначим очередь четвертому игроку, т.е. компу
//print("Менеджер очередей нашел из сцены ячейку козыря. Козырь - " + cellKozyr.cardKozyr.name);
//SetTurnByMinMast();

//print("Ходит игрок " + playersTurn.name);



//// Заполнение карт у игроков до 6
//void Razdat(CellPlayer oldTurn)
//{
//    // Индекс для массива карт
//    int allCardsIndex = 0;

//    // Заполнение картами ячеек игроков
//    for (int iCellPlayer = 0; iCellPlayer < allCellPlayer.Length; iCellPlayer++)
//    {
//        // Раздать игрокам по шесть карт
//        for (int iCellPlayerCards = 0; iCellPlayerCards < 6; iCellPlayerCards++)
//        {
//        }
//    }
//}
//// Ячейка игрока
//CellPlayer cellPlayer = allCellPlayer[iCellPlayer].GetComponent<CellPlayer>();



// Раскрыть карту
//card.SetOpen(true);

//allCardsIndex++;

//Card card = cellKoloda.allCardsInKoloda[allCardsIndex].GetComponent<Card>();




//// Найти из сцены все ячейки игроков
//allCellPlayers = FindObjectsOfType<CellPlayer>();
//// Ячейки игроков
//public CellPlayer[] players = new CellPlayer[4];

//// Карта - не козырь
//if (cellKozyr.cardKozyr != card)
//{
//    // Карта - козырь
//}
//else
//{

//}
//Card[] allCards = FindObjectsOfType<Card>();
//List<Card> allCardsInKoloda = new List<Card>();

//foreach (Card cardInAllCards in allCards)
//{
//    if (cardInAllCards.transform.parent == cellKoloda.transform)
//    {
//        allCardsInKoloda.Add(cardInAllCards);
//    }
//}

//bool added = false;

//foreach (Card cardInAllCards in allCardsInKoloda)
//{
//    if (cardInAllCards)
//    {
//        // Назначить карту игроку
//        cellPlayer.AddCardGO(card.gameObject);

//        // Сделать родителем карты ячейку соответствующего игрока
//        card.gameObject.transform.parent = cellPlayer.transform;

//        added = true;
//    }
//}

//if (!added)
//{
//    // Назначить карту игроку
//    cellPlayer.AddCardGO(card.gameObject);

//    // Сделать родителем карты ячейку соответствующего игрока
//    card.gameObject.transform.parent = cellPlayer.transform;
//}

//if (card)
//{
//}
//cellKozyr.cardKozyr.transform.position = cellKoloda.transform.position;


