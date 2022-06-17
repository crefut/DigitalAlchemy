using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameState : MonoBehaviour
{
    /*
     * Класс, хранящий информацию о текущем состоянии игры. Отвечает за работу с сохранениями и за поведение
     * пользовательского интерфейса при различных действиях пользователя.
     */
    public List<Item> selectedItems;
    public List<int> openedElements;
    public GameObject numOfUnlockedElements;
    public GameObject blackoutWindow;
    public GameObject descriptionWindow;
    public GameObject descriptionWindowText;
    public GameObject scrollView;
    public GameObject showDescriptionButton;
    public GameObject elementNameText;
    public GameObject elementIcon;
    public GameObject elementWebInfo;
    public GameObject endGameImage;
    public GameObject endGameFillingImage;
    public GameObject achievementImage;
    public bool descriptionModeIsActive;
    public bool gameIsOver;
    
    // Проверяет наличие сохранений при запуске игры и восстанавливает игровое состояние 
    void Start()
    {
        if (PlayerPrefs.HasKey("isNewGame") && PlayerPrefs.GetString("isNewGame") == "false")
            foreach (var id in PlayerPrefs.GetString("openedElements").Split())
                openedElements.Add(int.Parse(id));
        else
            openedElements = new List<int> {1, 2, 3, 4};

        var items = new Dictionary<int, GameObject>();
        foreach (var item in GameObject.FindGameObjectsWithTag("Item"))
            items.Add(item.GetComponent<Item>().id, item);
        foreach (var elementId in new List<int>(openedElements))
        {
            var item = items[elementId];
            item.GetComponent<Item>().Unlock(false);
        }
        UpdateNumOfUnlockedElements();
    }
    
    // Обновляет надпись, показывающую количество открытых элементов
    public void UpdateNumOfUnlockedElements()
    {
        numOfUnlockedElements.GetComponent<Text>().text = String.Format("Открыто элементов: {0}/{1}",
            openedElements.Count, GameObject.FindGameObjectsWithTag("Item").Length);
    }

    // Функция сочетания элементов
    public void CombineItems()
    {
        var intersection = selectedItems[0].furtherItems;
        for (var i = 1; i < selectedItems.Count; i++)
            intersection = intersection.Intersect(selectedItems[i].furtherItems).ToArray();

        foreach (var nextItem in intersection)
        {
            if (nextItem.GetComponent<Item>().components.Length != selectedItems.Count)
                continue;
            nextItem.GetComponent<Item>().Unlock(true);
            ClearSelection();
            UpdateNumOfUnlockedElements();
            break;
        }
        ClearSelection();
    }

    // Отменяет выбор ранее выбранных элементов
    public void ClearSelection()
    {
        foreach (var item in selectedItems)
            item.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        selectedItems = new List<Item>();
    }

    // Включает режим показа описаний элементов
    public void ShowItemDescription()
    {
        showDescriptionButton.GetComponent<Image>().color = descriptionModeIsActive ?
            new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0.5f);
        descriptionModeIsActive = !descriptionModeIsActive;
    }

    // Показывает окно с описанием элемента
    public void ShowWindow(GameObject item)
    {
        scrollView.GetComponent<ScrollRect>().horizontal = false;
        descriptionModeIsActive = true;
        if (item.GetComponent<Item>().endGameImage != null)
        {
            gameIsOver = true;
            endGameImage.GetComponent<Image>().sprite = item.GetComponent<Item>().endGameImage;
            achievementImage.GetComponent<Image>().sprite = item.GetComponent<Item>().achievementImage;
        }
        ChangeDescriptionWindowTextAndIcons(item.name, item.GetComponent<Item>().description,
            item.GetComponent<Item>().webLink, item.GetComponent<Item>().icon);
        StartCoroutine(FadeIn());
    }

    // Скрывает окно с описанием элемента
    public void CloseWindow()
    {
        StartCoroutine(FadeOut());
    }

    // Анимация всплывания окна с описание элемента
    public IEnumerator FadeIn()
    {
        blackoutWindow.SetActive(true);
        descriptionWindow.SetActive(true);
        for (var transparency = 0f; transparency <= 1; transparency += Time.deltaTime * 2f)
        {
            descriptionWindow.GetComponent<CanvasGroup>().alpha = transparency;
            yield return null;
        }
    }

    // Анимация ухода окна с описание элемента
    public IEnumerator FadeOut()
    {
        scrollView.GetComponent<ScrollRect>().horizontal = true;
        descriptionModeIsActive = false;
        showDescriptionButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        for (var transparency = 1f; transparency >= 0; transparency -= Time.deltaTime * 2f)
        {
            descriptionWindow.GetComponent<CanvasGroup>().alpha = transparency;
            yield return null;
        }
        var scrollbarPos = descriptionWindowText.transform.position;
        descriptionWindowText.transform.position = new Vector3(scrollbarPos.x, 0, scrollbarPos.z);
        blackoutWindow.SetActive(false);
        descriptionWindow.SetActive(false);
        if (gameIsOver)
            StartCoroutine(FinishGame());
    }

    // Отвечает за анимацию конца игры
    private IEnumerator FinishGame()
    {
        endGameImage.SetActive(true);
        endGameFillingImage.SetActive(true);

        for (var transparency = 0f; transparency <= 1; transparency += Time.deltaTime * 2f)
        {
            endGameFillingImage.GetComponent<Image>().color = new Color(0, 0, 0, transparency);
            endGameImage.GetComponent<Image>().color = new Color(1, 1, 1, transparency);
            yield return null;
        }

        var rt = achievementImage.GetComponent<RectTransform>();
        var currentPos = rt.anchoredPosition;
        var finalPosY = currentPos.y;
        
        rt.anchoredPosition = new Vector2(currentPos.x, rt.rect.height * rt.localScale.y + 5);
        achievementImage.SetActive(true);

        for (var posY = rt.anchoredPosition.y; posY >= finalPosY; posY -= Time.deltaTime * 600f)
        {
            rt.anchoredPosition = new Vector2(currentPos.x, posY);
            yield return null;
        }
    }

    // Изменяет текст окна с описанием элемента, ссылку на веб-ресурс и иконку элемента
    public void ChangeDescriptionWindowTextAndIcons(string title, string description, string webLink, Sprite icon)
    {
        foreach (var text in descriptionWindow.GetComponentsInChildren<Text>())
            text.text = text.gameObject.name == "Name" ? title : description + "\n\n\n\n";

        elementIcon.GetComponent<Image>().sprite = icon;
        elementWebInfo.GetComponent<WebLink>().url = webLink;
    }

    // Загружает главное меню
    public void LoadMainMenu()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }
}
