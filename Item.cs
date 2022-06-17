using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    /*
     * Класс, описывающий элемент игры (например, человек, знания, электричество..)
     * Каждый элемент имеет уникальный ID, его описание и название.
     * Кроме того, в объекте класса хранится информация о том, открыт ли уже этот элемент игроком,
     * какая иконка должна быть у элемента после его открытия (так как изначально элементы скрыты),
     * список компонентов, необходимых для создания этого элемента, и список элементов, в создании
     * которых он участвует.
     */
    public int id;
    public string description;
    public string webLink;
    public bool isOpened;
    public GameObject scriptLoader;
    public Sprite icon;
    public Sprite endGameImage;
    public Sprite achievementImage;
    public GameObject[] components;
    public GameObject[] furtherItems;
    private GameState gameState;

    // Выполняется при инициализации элемента
    void Start()
    {
        gameState = scriptLoader.GetComponent<GameState>();
        GetComponent<Button>().onClick.AddListener(Click);
    }

    // Метод, открывающий данный элемент в случае верной комбинации
    public void Unlock(bool isNewItem)
    {
        GetComponent<Image>().sprite = GetComponent<Item>().icon;
        isOpened = true;
        components = Array.Empty<GameObject>();
        if (isNewItem)
        {
            gameState.openedElements.Add(id);
            gameState.ShowWindow(gameObject);
            PlayerPrefs.SetString("isNewGame", "false");
            PlayerPrefs.SetString("openedElements", string.Join(" ", gameState.openedElements));
            PlayerPrefs.Save();
        }
    }

    // Метод-обработчик события клика по элементу
    private void Click()
    {
        if (!isOpened)
            return;
        if (gameState.descriptionModeIsActive)
            gameState.ShowWindow(gameObject);
        if (!gameState.selectedItems.Contains(GetComponent<Item>()))
        {
            gameState.selectedItems.Add(this);
            GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f);
            gameState.elementNameText.GetComponent<Text>().text = name;
        }
        else
        {
            gameState.selectedItems.Remove(GetComponent<Item>());
            GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            gameState.elementNameText.GetComponent<Text>().text = "";
        }
    }
}
