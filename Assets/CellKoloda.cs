using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellKoloda : MonoBehaviour
{
    // Количество карт в ячейке
    public int cardCount = 0;

    void OnMouseDown()
    {
        // print("CellKoloda - OnMouseDown()");

    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}


// Все карты в колоде
//public GameObject[] allCardsInKoloda = new GameObject[52];
//public ArrayList allCardsInKoloda ;

//// Спрайт для закончившихся возможностей перебора колоды
//public Sprite endSprite;

//// Количество попыток
//public int trys = 2;
//public CellKozyr cellKozyr;


//if (trys > 0)
//{
//    // Пересобрать колоду
//    Transform cellKozyrTransform = cellKozyr.transform;
//    cardCount = 0;

//    while (true)
//    {
//        RaycastHit2D hit;
//        hit = Physics2D.Raycast(cellKozyrTransform.position, Vector3.back);

//        if (hit)
//        {
//            GameObject cardGO = hit.collider.gameObject;
//            Card card = cardGO.GetComponent<Card>();

//            // Поместить карту поверх другой
//            Vector3 newPosition = new Vector3(transform.position.x, transform.position.y, -cardCount - 1);
//            cardGO.transform.position = newPosition;
//            card.backgroundSR.sortingOrder = cardCount;
//            card.mastSR.sortingOrder = cardCount + 1;
//            card.valueSR.sortingOrder = cardCount + 1;
//            card.SetOpen(false);
//            //card.vKolode = true;
//            //card.isFirst = false;
//            //card.inCellKozyr = false;

//            cardCount++;
//            cellKozyr.cardCount--;
//        }
//        else
//        {
//            break;
//        }
//    }
//    trys--;
//    if (trys == 0)
//    {
//        GetComponent<SpriteRenderer>().sprite = endSprite;
//    }
//}