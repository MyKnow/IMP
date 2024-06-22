using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;

using System.Runtime.InteropServices;


public class _CardGameManager : MonoBehaviour
{
    public static _CardGameManager Instance;
    public static int gameSize = 4;
    // gameobject instance
    [SerializeField]
    private GameObject prefab;
    // parent object of cards
    [SerializeField]
    private GameObject cardList;
    // sprite for card back
    [SerializeField]
    private Sprite cardBack;
    // all possible sprite for card front
    [SerializeField]
    private Sprite[] sprites;
    // list of card
    private _Card[] cards;

    //we place card on this panel
    [SerializeField]
    private GameObject panel;
    [SerializeField]
    private GameObject info;
    // for preloading
    [SerializeField]
    private _Card spritePreload;
    // other UI
    [SerializeField]
    private Text sizeLabel;
    [SerializeField]
    private Slider sizeSlider;
    [SerializeField]
    private Text timeLabel;
    private float time;

    private int spriteSelected;
    private int cardSelected;
    private int cardLeft;
    private bool gameStart;

    // 맞춘 페어의 수를 추적하는 변수
    private int matchedPairs;

    // 키와 카드 인덱스를 매핑하는 딕셔너리 선언
    private Dictionary<KeyCode, int> keyToCardIndex;


    private AndroidNativeWrapper androidNativeWrapper;

    private bool initialized = false; // 추가

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        gameStart = false;
        panel.SetActive(false);

        androidNativeWrapper = new AndroidNativeWrapper();
        InitializeKeyToCardIndex();
    }

    private void InitializeKeyToCardIndex()
    {
        keyToCardIndex = new Dictionary<KeyCode, int>
        {
            { KeyCode.Escape, 12 },   // 첫 번째 버튼 (Escape 키)
            { KeyCode.Alpha1, 13},   // 두 번째 버튼 (Alpha1 키)
            { KeyCode.Alpha2, 14 },   // 세 번째 버튼 (Alpha2 키)
            { KeyCode.Alpha3, 15 },   // 네 번째 버튼 (Alpha3 키)
            { KeyCode.Alpha4, 8 },   // 다섯 번째 버튼 (Alpha4 키)
            { KeyCode.Alpha5, 9 },   // 여섯 번째 버튼 (Alpha6 키)
            { KeyCode.Alpha6, 10 },   // 일곱 번째 버튼 (Alpha5 키)
            { KeyCode.Alpha7, 11 },   // 여덟 번째 버튼 (Alpha7 키)
            { KeyCode.Alpha8, 4 },   // 아홉 번째 버튼 (Alpha8 키)
            { KeyCode.Alpha9, 5 },   // 열 번째 버튼 (Alpha9 키)
            { KeyCode.Alpha0, 6 },  // 열한 번째 버튼 (Alpha0 키)
            { KeyCode.Minus, 7 },   // 열두 번째 버튼 (Minus 키)
            { KeyCode.Equals, 0 },  // 열세 번째 버튼 (Equals 키)
            { KeyCode.Backspace, 1 }, // 열네 번째 버튼 (Backspace 키)
            { KeyCode.Tab, 2 },     // 열다섯 번째 버튼 (Tab 키)
            { KeyCode.Q, 3 }        // 열여섯 번째 버튼 (Q 키)
        };
    }

    private void Update()
    {
        if (gameStart)
        {
            time += Time.deltaTime;
            timeLabel.text = "남은 시간: " + time.ToString("F2") + "s";
            
            // androidNativeWrapper.NativeSegmentControl((int)time);

            foreach (var entry in keyToCardIndex)
            {
                if (Input.GetKeyDown(entry.Key))
                {
                    SelectCardByIndex(entry.Value);
                }
            }

                // 모든 키 코드를 순회하며 체크
            foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                {
                    Debug.Log("Key pressed: " + keyCode);
                }
            }
        } else {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StartCardGame(0.5f);
            } else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                StartCardGame(1.0f);
            } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
                StartCardGame(1.5f);
            } else if (Input.GetKeyDown(KeyCode.Alpha3)) {
                StartCardGame(2.0f);
            }

            
            if (!initialized) // 조건 추가
            {
                androidNativeWrapper.NativeIOCtlClear();
                androidNativeWrapper.NativeIOCtlReturnHome();
                androidNativeWrapper.NativeIOCtlDisplay(true);
                androidNativeWrapper.NativeIOCtlCursor(false);
                androidNativeWrapper.NativeIOCtlBlink(false);
                androidNativeWrapper.NativeTextLCDControl("IMP MEMORY GAME ", "                ");
                // androidNativeWrapper.NativeDotMatrixControl("Made by Minho and Namhun");
                // androidNativeWrapper.CustomFullColorRandom();
                androidNativeWrapper.NativeLEDControl(0);
                initialized = true; // 상태 변경
            }
        }
    }

    private void SelectCardByIndex(int index)
    {
        if (index >= 0 && index < cards.Length)
        {
            cards[index].CardBtn();
        }
    }

    // Start a game
    public void StartCardGame(float delay = 1.0f)
    {
        if (gameStart) return; // return if game already running
        gameStart = true;
        // toggle UI
        panel.SetActive(true);
        info.SetActive(false);
        // set cards, size, position
        SetGamePanel();
        // renew gameplay variables
        cardSelected = spriteSelected = -1;
        cardLeft = cards.Length;
        matchedPairs = 0; // 초기화
        androidNativeWrapper.NativeLEDControl(matchedPairs);
        androidNativeWrapper.CustomFullColorLEDOff();
        // androidNativeWrapper.NativeSegmentIOControl(1);
        // allocate sprite to card
        SpriteCardAllocation();
        StartCoroutine(HideFace(delay));
        time = 0;
    }

    // Initialize cards, size, and position based on size of game
    private void SetGamePanel(){
        // if game is odd, we should have 1 card less
        int isOdd = gameSize % 2 ;

        cards = new _Card[gameSize * gameSize - isOdd];
        // remove all gameobject from parent
        foreach (Transform child in cardList.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        // calculate position between each card & start position of each card based on the Panel
        RectTransform panelsize = panel.transform.GetComponent(typeof(RectTransform)) as RectTransform;
        float row_size = panelsize.sizeDelta.x;
        float col_size = panelsize.sizeDelta.y;
        float scale = 1.0f/gameSize;
        float xInc = row_size/gameSize;
        float yInc = col_size/gameSize;
        float curX = -xInc * (float)(gameSize / 2);
        float curY = -yInc * (float)(gameSize / 2);

        if(isOdd == 0) {
            curX += xInc / 2;
            curY += yInc / 2;
        }
        float initialX = curX;
        // for each in y-axis
        for (int i = 0; i < gameSize; i++)
        {
            curX = initialX;
            // for each in x-axis
            for (int j = 0; j < gameSize; j++)
            {
                GameObject c;
                // if is the last card and game is odd, we instead move the middle card on the panel to last spot
                if (isOdd == 1 && i == (gameSize - 1) && j == (gameSize - 1))
                {
                    int index = gameSize / 2 * gameSize + gameSize / 2;
                    c = cards[index].gameObject;
                }
                else
                {
                    // create card prefab
                    c = Instantiate(prefab);
                    // assign parent
                    c.transform.parent = cardList.transform;

                    int index = i * gameSize + j;
                    cards[index] = c.GetComponent<_Card>();
                    cards[index].ID = index;
                    // modify its size
                    c.transform.localScale = new Vector3(scale, scale);
                }
                // assign location
                c.transform.localPosition = new Vector3(curX, curY, 0);
                curX += xInc;

            }
            curY += yInc;
        }

    }
    // reset face-down rotation of all cards
    void ResetFace()
    {
        for (int i = 0; i < gameSize; i++)
            cards[i].ResetRotation();
    }
    // Flip all cards after a short period
    IEnumerator HideFace(float delay = 0.5f)
    {
        //display for a short moment before flipping
        yield return new WaitForSeconds(delay);
        for (int i = 0; i < cards.Length; i++)
            cards[i].Flip();
        yield return new WaitForSeconds(delay);
    }
    // Allocate pairs of sprite to card instances
    private void SpriteCardAllocation()
    {
        int i, j;
        int[] selectedID = new int[cards.Length / 2];
        // sprite selection
        for (i = 0; i < cards.Length/2; i++)
        {
            // get a random sprite
            int value = UnityEngine.Random.Range(0, sprites.Length - 1);
            // check previous number has not been selection
            // if the number of cards is larger than number of sprites, it will reuse some sprites
            for (j = i; j > 0; j--)
            {
                if (selectedID[j - 1] == value)
                    value = (value + 1) % sprites.Length;
            }
            selectedID[i] = value;
        }

        // card sprite deallocation
        for (i = 0; i < cards.Length; i++)
        {
            cards[i].Active();
            cards[i].SpriteID = -1;
            cards[i].ResetRotation();
        }
        // card sprite pairing allocation
        for (i = 0; i < cards.Length / 2; i++)
            for (j = 0; j < 2; j++)
            {
                int value = UnityEngine.Random.Range(0, cards.Length - 1);
                while (cards[value].SpriteID != -1)
                    value = (value + 1) % cards.Length;

                cards[value].SpriteID = selectedID[i];
            }

    }
    // Slider update gameSize
    public void SetGameSize() {
        gameSize = (int)sizeSlider.value;
        sizeLabel.text = gameSize + " X " + gameSize;
    }
    // return Sprite based on its id
    public Sprite GetSprite(int spriteId)
    {
        return sprites[spriteId];
    }
    // return card back Sprite
    public Sprite CardBack()
    {
        return cardBack;
    }
    // check if clickable
    public bool canClick()
    {
        if (!gameStart)
            return false;
        return true;
    }
    // card onclick event
    public void cardClicked(int spriteId, int cardId)
    {
        // first card selected
        if (spriteSelected == -1)
        {
            spriteSelected = spriteId;
            cardSelected = cardId;
            androidNativeWrapper.NativePiezoControl(6);
        }
        else
        { // second card selected
            if (spriteSelected == spriteId)
            {
                //correctly matched
                cards[cardSelected].Inactive();
                cards[cardId].Inactive();
                cardLeft -= 2;
                matchedPairs++;
                Debug.Log("지금까지 맞춘 갯수 : " + matchedPairs);
                androidNativeWrapper.NativeLEDControl(matchedPairs);
                androidNativeWrapper.NativePiezoControlSuccess();
                androidNativeWrapper.CustomFullColorLEDSuccess();
                CheckGameWin();
            }
            else
            {
                // incorrectly matched
                androidNativeWrapper.NativePiezoControlFail();
                androidNativeWrapper.CustomFullColorLEDFail();
                cards[cardSelected].Flip();
                cards[cardId].Flip();
            }
            cardSelected = spriteSelected = -1;
        }
    }
    // check if game is completed
    private void CheckGameWin()
    {
        // win game
        if (cardLeft == 0)
        {
            EndGame();
            AudioPlayer.Instance.PlayAudio(1);
        }
    }
    // stop game
    private void EndGame()
    {
        gameStart = false;
        panel.SetActive(false);
    }
    public void GiveUp()
    {
        EndGame();
    }
    public void DisplayInfo(bool i)
    {
        info.SetActive(i);
    }
}
