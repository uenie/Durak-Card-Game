using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    // Состояние меню
    bool menuOn = false;

    TurnManager turnManager;

    // Use this for initialization
    void Start()
    {
        turnManager = FindObjectOfType<TurnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // Вызов меню с помощью Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menuOn = !menuOn;
        }

        // Перезапустить игру при нажатии на клавишу F2
        if (Input.GetKeyDown(KeyCode.F2))
        {
            RestartGame();
        }
    }

    void OnGUI()
    {
        // Показ меню
        if (menuOn)
        {
            if (GUI.Button(new Rect(10, 10, 100, 30), "Restart"))
            {
                RestartGame();
            }
            if (GUI.Button(new Rect(10, 10 + 35, 100, 30), "Exit"))
            {
                Application.Quit();
            }
        }

        // Показывать масть козыря
        string kozyrMast = "";
        switch (turnManager.cellKozyr.cardKozyr.mast)
        {
            case 0:
                kozyrMast = "Червы (♥)";
                break;
            case 1:
                kozyrMast = "Бубны (♦)";
                break;
            case 2:
                kozyrMast = "Трефы (♣)";
                break;
            case 3:
                kozyrMast = "Пики (♠)";
                break;
        }
        GUI.Box(new Rect(10 + 110, 10 + 35, 300, 30), "Козырная масть: " + kozyrMast);

        // Кто проиграл или ничья
        string whoLose = "";
        if (turnManager.allCellPlayers.Count == 1)
        {
            whoLose = turnManager.allCellPlayers[0].name;

            turnManager.allCellPlayers[0].gameObject.SetActive(false);

            //print("turnManager.allCellPlayers.Count == 1");

        }
        else if (turnManager.allCellPlayers.Count == 0)
        {
            whoLose = "Ничья";
            //print("turnManager.allCellPlayers.Count == 0");
        }
        //else
        //{
        //    whoLose = "";
        //}
        if (whoLose != "")
        {
            GUI.Box(new Rect(10 + 110, 10 + 35 + 35, 300, 30), "whoLose = " + whoLose);
        }
    }

    // Перезапуск игры
    void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
}
