using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    // Ячейка козыря
    CellKozyr cellKozyr;

    // Игровое поле
    CellGamePole cellGamePole;

    // Раскрыта ли карта (изначально не раскрыта)
    [SerializeField]
    private bool isOpen = false;

    // Перемещается ли карта
    public bool mouseDragging = false;

    // Выделять ли карту
    public bool stickOut = false;

    // Насколько выделять карту
    public float yOffsetMouseOver = 0.5f;

    // Отбита ли карта
    public bool isBeated = false;

    // Насколько добавлять sortingOrder при перемещении карты
    public int sortingOrderAdd = 100;
    public int sortingOrderAddZ = 100;

    // Прежняя позиция карты, до перемещения
    public Vector3 oldPositionInCell;

    // SortingOrder спрайтов
    public int backgroundSRSortingOrder;
    public int mastSRSortingOrder;
    public int valueSRSortingOrder;

    // Текстуры фона карты
    public SpriteRenderer backgroundSR;
    public Sprite openedCardTexture;
    public Sprite closedCardTexture;

    // Масть: цвет - красный: Червы (♥), Бубны (♦); цвет - черный: Трефы (♣), Пики (♠)
    public int mast = 0;
    public SpriteRenderer mastSR;

    // Цвет: true - красный, false - черный
    public bool isCardColorRed = false;

    // Значение, величина
    // 9 величин по возрастанию: 6...10, 11 (Валет), 12 (Дама), 13 (Король), 14 (Туз)
    // 9 величин * 4 масти = 36 карт
    public int value = 0;
    public SpriteRenderer valueSR;

    // Старое расположение карты
    Vector3 oldCardPosition;

    CellPlayer cellPlayer;
    CellPlayer playersTurn;

    // Use this for initialization
    void Start()
    {
        // Назначить ячейку козыря
        cellKozyr = FindObjectOfType<CellKozyr>();

        // Назначить игровое поле
        cellGamePole = FindObjectOfType<CellGamePole>();
    }

    // Update is called once per frame
    void Update()
    {
        // Обновление для карты игрока
        if (transform.parent && transform.parent.tag == "Player")
        {
            cellPlayer = transform.parent.GetComponent<CellPlayer>();
            playersTurn = cellPlayer.turnManager.playersTurn;

            // Когда игрок ходит или подкидываает
            stickOut = false;
            if (playersTurn == cellPlayer || playersTurn == cellPlayer.prevCellPlayer.prevCellPlayer)
            {
                // Выделить карты игрока, которыми можно ходить или подкидывать
                if (cellPlayer.CanGoToGamePole())
                {
                    foreach (GameObject cardGO in cellGamePole.GetAllCards())
                    {
                        if (value == cardGO.GetComponent<Card>().value)
                        {
                            stickOut = true;
                            break;
                        }
                    }
                }
                if (cellGamePole.allDownCards.Count == 0)
                {
                    stickOut = true;
                }
            }
            // Когда игрок отбивается
            else if (playersTurn == cellPlayer.prevCellPlayer)
            {
                // Выделить карты игрока, которыми можно отбиваться
                if (AllowedToBeatCount() > 0)
                {
                    stickOut = true;
                }
                else
                {
                    stickOut = false;
                }
            }
        }
    }

    // Установить состояние карты, раскрыта или не раскрыта
    public void SetOpen(bool open)
    {
        isOpen = open;

        // Применить текстуру фона карты
        if (isOpen)
        {
            backgroundSR.sprite = openedCardTexture;
            mastSR.gameObject.SetActive(true);
            valueSR.gameObject.SetActive(true);
        }
        else
        {
            backgroundSR.sprite = closedCardTexture;
            mastSR.gameObject.SetActive(false);
            valueSR.gameObject.SetActive(false);
        }
    }

    // Проверка на допустимость хода этой картой в самом начале
    public bool AllowedToGo()
    {
        // Возвращаемый результат
        bool isAllowed = false;

        CellPlayer player = transform.parent.GetComponent<CellPlayer>();

        // Принадлежит ли карта какому-либо игроку
        if (player)
        {
            // Можно ли вообще ходить или подкидывать владельцу этой карты
            if (player.turnManager.playersTurn == player || player.turnManager.playersTurn == player.prevCellPlayer.prevCellPlayer && player.CanGoToGamePole())
            {
                // Если только ходить, то можно ходить в самом начале, когда ничего в игровом поле пока нет
                if (player.turnManager.playersTurn == player)
                {
                    // Если игровое поле пусто, то можно ходить какой угодно картой
                    if (cellGamePole.allDownCards.Count == 0)
                    {
                        return true;
                    }
                }

                // Перебрать все карты игрового поля
                // Нижние карты
                foreach (GameObject cardGO in cellGamePole.allDownCards)
                {
                    // Проверить на совпадение значения карты
                    Card card = cardGO.GetComponent<Card>();
                    if (value == card.value)
                    {
                        return true;
                        //isAllowed = true;
                        //break;
                    }
                }
                // Верхние карты
                foreach (GameObject cardGO in cellGamePole.allUpCards)
                {
                    // Проверить на совпадение значения карты
                    Card card = cardGO.GetComponent<Card>();
                    if (value == card.value)
                    {
                        return true;
                        //isAllowed = true;
                        //break;
                    }
                }

                // Игрок или комп
                if (player.tag == "Player") // Игрок
                {

                }
                else if (player.tag == "AI") // Компьютер
                {

                }
            }
        }

        // Возвратить значение
        return isAllowed;
    }

    // Проверка на превосходство этой карты над другой
    public void Beat(Card otherCard)
    {
        // Убрать карту из массива карт игрока и добавить ее в игровое поле
        CellPlayer cellPlayer = transform.parent.GetComponent<CellPlayer>();
        cellPlayer.RemoveCardGO(gameObject);
        transform.parent = otherCard.transform;
        transform.position = new Vector3(transform.parent.position.x + cellGamePole.xOffset,
                                         transform.parent.position.y + cellGamePole.yOffset, transform.parent.position.z);
        cellGamePole.allUpCards.Add(gameObject);

        // sortingOrder
        backgroundSR.sortingOrder = otherCard.backgroundSR.sortingOrder;
        mastSR.sortingOrder = otherCard.mastSR.sortingOrder + 1;
        valueSR.sortingOrder = otherCard.valueSR.sortingOrder + 1;

        // Возвратить карты в игровом поле обратно
        StickOutCardsInCellGamePole(false);

        // Карта отбита
        otherCard.GetComponent<Card>().isBeated = true;
    }

    // Проверка на превосходство этой карты над другой
    public bool CanBeat(Card otherCard)
    {
        // Проверка свободности другой карты, не отбита ли она раньше
        if (!otherCard.isBeated) // Не отбита ли
        {
            if (mast == cellKozyr.cardKozyr.mast && otherCard.mast == cellKozyr.cardKozyr.mast)  // Эта карта - козырь и другая карта - козырь
            {
                if (value > otherCard.value) // Значение больше
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (mast == cellKozyr.cardKozyr.mast) // Только эта карта - козырь
            {
                return true;
            }
            else if (otherCard.mast == cellKozyr.cardKozyr.mast) // Только другая карта - козырь
            {
                return false;
            }
            else if (mast == otherCard.mast) // Совпадение обычных мастей
            {
                if (value > otherCard.value) // Значение больше
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    // Проверка на допустимость отбивания этой картой
    public int AllowedToBeatCount()
    {
        // Возвращаемый результат
        int allowedToBeatCount = 0;

        // Принадлежит ли карта какому-либо игроку
        if (transform.parent)
        {
            // Можно ли вообще отбиваться владельцу этой карты
            CellPlayer player = transform.parent.GetComponent<CellPlayer>();
            if (player && player.turnManager.playersTurn == player.prevCellPlayer) // Правый сосед должен ходить
            {
                // Перебрать все карты игрового поля
                // Нижние карты
                foreach (GameObject otherCardGO in cellGamePole.allDownCards)
                {
                    // Проверить на совпадение масти и превосходство этой карты над другой
                    // Проверка свободности другой карты, не отбита ли она раньше
                    Card otherCard = otherCardGO.GetComponent<Card>();
                    if (CanBeat(otherCard))
                    {
                        allowedToBeatCount++;
                    }
                }

                // Игрок или комп
                if (player.tag == "Player") // Игрок
                {

                }
                else if (player.tag == "AI") // Компьютер
                {

                }
            }
        }
        else
        {
            Debug.LogError("Карта не принадлежит никакому игроку.");
        }

        // Возвратить значение
        return allowedToBeatCount;
    }

    // Выделить или вернуть карты в игровом поле
    void StickOutCardsInCellGamePole(bool stickOut)
    {
        // Перебрать все нижние карты игрового поля
        foreach (GameObject otherCardGO in cellGamePole.allDownCards)
        {
            // Проверить на совпадение масти и превосходство этой карты над другой
            // Проверка свободности другой карты, не отбита ли она раньше
            Card otherCard = otherCardGO.GetComponent<Card>();
            if (stickOut)
            {
                if (CanBeat(otherCard))
                {
                    //print("CanBeat");
                    // Выделить карту в игровом поле
                    otherCard.stickOut = true;
                }
            }
            else
            {
                // Вернуть в обратное место карту в игровом поле
                otherCard.stickOut = false;
            }
        }
    }

    // Ход или подкидывание картой
    public void AddToGamePole()
    {
        // Убрать карту из массива карт игрока
        CellPlayer cellPlayer = transform.parent.GetComponent<CellPlayer>();
        cellPlayer.RemoveCardGO(gameObject);

        // Перенести в игровое поле
        transform.parent = cellGamePole.transform;
        //transform.position = new Vector3(transform.parent.position.x + cellGamePole.xOffsetForNextCards * cellGamePole.allDownCards.Count,
        //                                 transform.parent.position.y, transform.parent.position.z);
        cellGamePole.allDownCards.Add(gameObject);
        stickOut = false;
    }

    void OnMouseEnter()
    {
        // При наведении курсора в карту игрока
        if (transform.parent && transform.parent.tag == "Player")
        {
            // Когда игрок ходит или подкидываает
            if (playersTurn == cellPlayer || playersTurn == cellPlayer.prevCellPlayer.prevCellPlayer && !cellPlayer.mouseDragging)
            {
                // Выделить карты в игровом поле
                //if (AllowedToBeatCount() > 0)
                //{
                //    stickOut = true;
                //}
                //else
                //{
                //    stickOut = false;
                //}
            }
            // Когда игрок отбивается
            else if (playersTurn == cellPlayer.prevCellPlayer)
            {
                // Выделить карты в игровом поле, которых можно отбить этой картой
                StickOutCardsInCellGamePole(true);
            }
        }
    }

    void OnMouseOver()
    {
    }

    void OnMouseExit()
    {
        if (transform.parent && transform.parent.tag == "Player" && !transform.parent.GetComponent<CellPlayer>().mouseDragging)
        {
            // Возвратить обратно карты в игровом поле
            StickOutCardsInCellGamePole(false);
        }
    }

    // Зажатие ЛКМ игроком 
    void OnMouseDown()
    {
        if (transform.parent && transform.parent.tag == "Player" && !transform.parent.GetComponent<CellPlayer>().mouseDragging)
        {
            if (AllowedToGo() || (AllowedToBeatCount() > 0))
            {
                // Сохранить расположение карты
                oldCardPosition = transform.position - Camera.main.ScreenPointToRay(Input.mousePosition).origin;

                mouseDragging = true;

            }
        }
    }

    void OnMouseDrag()
    {
        if (transform.parent && transform.parent.tag == "Player")
        {
            // Перемещение карты с зажатой ЛКМ
            Vector3 newPosition = Camera.main.ScreenPointToRay(Input.mousePosition).origin + oldCardPosition;
            // print(newPosition.z);
            transform.position = new Vector3(newPosition.x, newPosition.y, sortingOrderAddZ);


            backgroundSR.sortingOrder = sortingOrderAdd;
            mastSR.sortingOrder = sortingOrderAdd + 1;
            valueSR.sortingOrder = sortingOrderAdd + 1;
        }
    }

    // Опускание ЛКМ
    void OnMouseUp()
    {
        if (transform.parent && transform.parent.tag == "Player")
        {
            if (mouseDragging)
            {
                mouseDragging = false;

                // Ход картой игрока
                //if (AllowedToGo() && CanGoToGamePole())
                if (AllowedToGo() && stickOut)
                {
                    //transform.position = new Vector3(transform.position.x, transform.parent.position.y + yOffsetMouseOver, transform.position.z);
                    AddToGamePole();
                }

                // Отбивание
                int allowedToBeatCount = AllowedToBeatCount();
                if (allowedToBeatCount > 0)
                {
                    // Когда можно отбить лишь одну карту
                    if (allowedToBeatCount == 1)
                    {
                        // Неотбитые карты
                        List<GameObject> allNotBeatedCardsGO = cellGamePole.GetNotBeatedCards();

                        // Отбить карту
                        foreach (GameObject cardGO in allNotBeatedCardsGO)
                        {
                            if (CanBeat(cardGO.GetComponent<Card>()))
                            {
                                Beat(cardGO.GetComponent<Card>());
                                break;
                            }
                        }
                    }
                    // Когда можно отбить несколько карт
                    else
                    {
                        // Временно отключить коллайдер карты
                        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
                        boxCollider.enabled = false;

                        // После опускания ЛКМ над картой, прикрепить карту к карте под ней
                        RaycastHit2D hit;
                        hit = Physics2D.Raycast(transform.position + (-oldCardPosition), Vector3.forward);

                        Card otherCard = null;

                        if (hit.collider != null)
                        {
                            otherCard = hit.collider.GetComponent<Card>();

                            if (otherCard != null && otherCard.transform.parent.name == "CellGamePole")
                            {
                                // Проверить на допустимость хода
                                if (CanBeat(otherCard))
                                {
                                    // Отбить карту
                                    Beat(otherCard);
                                }
                                // Вернуть карту на прежнее место 
                                else
                                {
                                    // print("transform.childCount: "+transform.childCount);
                                    transform.position = oldPositionInCell;
                                    //transform.SetParent(oldParent);

                                    backgroundSR.sortingOrder = backgroundSRSortingOrder;
                                    mastSR.sortingOrder = mastSRSortingOrder;
                                    valueSR.sortingOrder = valueSRSortingOrder;

                                    // Обнулить луч попадания
                                    hit = new RaycastHit2D();
                                }
                            }
                        }

                        // Включить коллайдер карты
                        boxCollider.enabled = true;
                    }
                }
            }
        }
    }
}

// ---------------------------------------------------------------------------------------------

// Выбор карты, которой нужно отбиться

//// Отбивание
//if (AllowedToBeatCount() > 0)
//{
//    // Выбор карты, которой нужно отбиться
//    // Отобрать карты игрового поля, которых можно отбить этой картой
//    //ArrayList allowedToBeatCardsInCellGamePole = new ArrayList();
//    List<Card> allowedToBeatCardsInCellGamePole = new List<Card>();

//    foreach (GameObject otherCardGO in cellGamePole.allDownCards)
//    {
//        // Проверить на совпадение масти и превосходство этой карты над другой
//        // Проверка свободности другой карты, не отбита ли она раньше
//        Card otherCard = otherCardGO.GetComponent<Card>();
//        if (CanBeat(Card otherCard))
//        {
//            allowedToBeatCardsInCellGamePole.Add(otherCard);
//        }
//    }


//print("allowedToBeatCardsInCellGamePole.Count == " + allowedToBeatCardsInCellGamePole.Count);

//// Если одна карта - то сразу отбить
//if (allowedToBeatCardsInCellGamePole.Count == 1)
//{

//}
//// Если карт, которых можно отбить выбранной картой, несколько
//else if (AllowedToBeatCount() > 1)
//{

//}


//if (vKolode)
//{
//    // Уже не в колоде
//    vKolode = false;
//    inCellKozyr = true;
//}




//// Отрисовка карты
//if (!mouseDragging && transform.parent != null && transform.parent.GetComponent<Card>() != null)
//{
//    Card parentCard = transform.parent.GetComponent<Card>();
//    SpriteRenderer parentCardSR = parentCard.GetComponent<SpriteRenderer>();
//    backgroundSR.sortingOrder = parentCardSR.sortingOrder + 1;
//    mastSR.sortingOrder = backgroundSR.sortingOrder + 1;
//    valueSR.sortingOrder = backgroundSR.sortingOrder + 1;
//    parentCard.isFirst = false;
//    // print("+1");
//}


//// В одной из ячеек игроков
//public bool inCellPlayer = false;

//// В ячейке отбоя
//public bool inCellBito = false;

//// Самая верхняя
//public bool isFirst = false;

//// В колоде
//public bool vKolode = true;

//// Под колодой
//public bool inCellKozyr = false;

//private Card card;

//Transform oldParent;

// Отобрать карты игрового поля, которых можно отбить этой картой

//cellPlayer.allCardsInPlayerCell.Remove(gameObject);
//print(cellGamePole.allDownCards.Count);

//if (name == "Card (9, Black (Trefy))")
//{
//    print("AllowedToBeatCount() = " + AllowedToBeatCount());
//}


//print(player.name);

//else
//{
//    Debug.LogError("Карта не принадлежит на к какому игроку.");
//}

//if ()
//{
//    print("AllowedToGo");
//    stickOut = true;
//}
//if (transform.parent && transform.parent.tag == "Player" && !transform.parent.GetComponent<CellPlayer>().mouseDragging)
//{}

