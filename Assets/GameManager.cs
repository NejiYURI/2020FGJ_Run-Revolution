using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class Difficulty
    {
        public float TimeToStart;
        public float Freq;
        public float Speed;
        public bool Rotate;
        public bool fadeOut;
    }

    /// <summary>
    /// 遊戲狀態
    /// </summary>
    public enum GameProgress
    {
        Pause,
        Playing,
        GameOver,
        GameClear
    }

    /// <summary>
    /// 畫面震動事件
    /// </summary>
    public event Action ScreenShake;
    public void ScreenShakeTrigger()
    {
        if (ScreenShake != null)
        {
            ScreenShake();
        }
    }



    /// <summary>
    /// 遊戲狀態
    /// </summary>
    [SerializeField]
    public GameProgress GameState;

    /// <summary>
    /// 難度設定，可設定啟動時間、是否翻轉、是否消失
    /// </summary>
    [SerializeField]
    public List<Difficulty> difficultSetup = new List<Difficulty>();


    /// <summary>
    /// 目前難度
    /// </summary>
    public int NowDifficult;


    /// <summary>
    /// 敵人生成速率
    /// </summary>
    public float SpawnFreq;

    /// <summary>
    /// 愈生成的敵人Prefab(之後可考慮更改為Object Pool)
    /// </summary>
    public GameObject EnemyObj;

    public GameObject EnemyObj_Twitch;

    /// <summary>
    /// 已進入判斷區域的敵人 
    /// </summary>
    [SerializeField]
    private List<EnemyScript> EnemyInZone;

    /// <summary>
    /// 要顯示的輸入圖片
    /// </summary>
    public List<Sprite> CodeSprite;

    /// <summary>
    /// 已經碰到玩家的敵人
    /// </summary>
    [SerializeField]
    private List<EnemyScript> EnemyInStack;

    /// <summary>
    /// 生成障礙物(敵人)的位置
    /// </summary>
    public Transform EnemySpawnPoint;

    /// <summary>
    /// 敵人是否為由左往右
    /// </summary>
    public bool Isleft;

    /// <summary>
    /// 敵人移動速度
    /// </summary>
    public float EnemySpeed;

    /// <summary>
    /// 鏡頭搖晃時間
    /// </summary>
    public float ShakeDuration;

    /// <summary>
    /// 鏡頭搖晃程度
    /// </summary>
    public float ShakeMagnitude;

    /// <summary>
    /// 宣告此物件可以由此專案中的所有Script讀取到
    /// </summary>
    public static GameManager GameMainManager;

    /// <summary>
    /// 顯示在上面的所有要求輸入指令
    /// </summary>
    public List<Image> InputCodeShowImageList;

    public List<Sprite> EnemySprite;

    /// <summary>
    /// 遊戲結束文字
    /// </summary>
    public GameObject GameOverText;

    /// <summary>
    /// 現在是否無法輸入(玩家按錯鍵時觸發)
    /// </summary>
    public bool IsInputDisable;

    /// <summary>
    /// 無法輸入時間長度
    /// </summary>
    public float DisableDuration;

    /// <summary>
    /// 使用者禁止輸入狀態回復條
    /// </summary>
    public Image DisableBar;

    /// <summary>
    /// 背景(有可能會換，不過目前以這個方式捲動)
    /// </summary>
    public Renderer Background;

    /// <summary>
    /// 此關卡長度(秒)
    /// </summary>
    public float GameTotalTime;

    /// <summary>
    /// 目前遊玩時間
    /// </summary>
    public float GameCurrentTime;

    /// <summary>
    /// 進度條UI
    /// </summary>
    public Image ProgressBar;

    /// <summary>
    /// 過關文字
    /// </summary>
    public GameObject GameClearText;

    /// <summary>
    /// 聲音控制器
    /// </summary>
    public SoundController soundController;

    public float MusicVolume;

    public AudioClip BackgroundMusic;

    public AudioClip CorrectSound;

    public AudioClip WrongSound;

    /// <summary>
    /// 初始化，將GameMainManager指向自己
    /// </summary>
    private void Awake()
    {
        GameMainManager = this;
    }

    private void Start()
    {
        //初始化，先清除所有的List
        this.EnemyInZone = new List<EnemyScript>();
        this.EnemyInStack = new List<EnemyScript>();
        GameOverText.SetActive(false);
        StartCoroutine(SpawnController());
        SetInputCodeImage();
        this.IsInputDisable = false;
        this.DisableBar.enabled = false;
        this.GameClearText.SetActive(false);
        StartCoroutine(ProgressBarController());
        this.NowDifficult = -1;
        if (soundController && BackgroundMusic)
        {
            soundController.PlayMusic(BackgroundMusic, MusicVolume);
        }
    }

    private void Update()
    {

        if (GameState == GameProgress.Playing && !IsInputDisable)
        {
            //案件事件按下"上"的時候觸發訂閱事件
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Debug.Log("UpPress");

                //ButtonPressTrigger(0);
                BtnPressFunc(0);
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Debug.Log("DownPress");
                //ButtonPressTrigger(1);
                BtnPressFunc(1);
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Debug.Log("LeftPress");
                //ButtonPressTrigger(2);
                BtnPressFunc(2);
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Debug.Log("RightPress");
                //ButtonPressTrigger(3);
                BtnPressFunc(3);
            }
        }

    }

    private void FixedUpdate()
    {
        if (GameState == GameProgress.Playing)
        {
            if (Isleft)
            {
                Background.material.mainTextureOffset += new Vector2(EnemySpeed * 0.03f * Time.deltaTime, 0);
            }
            else
            {
                Background.material.mainTextureOffset -= new Vector2(EnemySpeed * 0.03f * Time.deltaTime, 0);
            }

        }
    }

    /// <summary>
    /// 按下按鈕之後的判斷
    /// </summary>
    /// <param name="InputCode"></param>
    private void BtnPressFunc(int InputCode)
    {
        //先檢查已經碰到玩家的敵人列表中有沒有資料(優先處理)，沒有的話檢查已進入判斷區的敵人列表
        if (this.EnemyInStack.Count > 0)
        {
            EnemyScript es = this.EnemyInStack[0];
            if (es.InputCode == InputCode)
            {
                this.EnemyInStack.Remove(es);
                es.DestroyEnemy(InputCode);
                //播放正確的聲音
                if(soundController && CorrectSound)
                {
                    soundController.PlaySE(CorrectSound);
                }
            }
            else
            {
                ScreenShakeTrigger();
                StartCoroutine(DisableCountDown(DisableDuration));
                //播放錯誤的聲音
                if (soundController && WrongSound)
                {
                    soundController.PlaySE(WrongSound);
                }
            }
            SetInputCodeImage();
        }
        else if (this.EnemyInZone.Count > 0)
        {
            //如果進入判斷區的列表中有值，檢查第一筆資料且判斷輸入的指令是否對應   
            EnemyScript es = this.EnemyInZone[0];
            if (es.InputCode == InputCode)
            {
                this.EnemyInZone.Remove(es);
                es.DestroyEnemy(InputCode);

                //播放正確的聲音
                if (soundController && CorrectSound)
                {
                    soundController.PlaySE(CorrectSound);
                }
            }
            else
            {
                ScreenShakeTrigger();
                StartCoroutine(DisableCountDown(DisableDuration));

                //播放錯誤的聲音
                if (soundController && WrongSound)
                {
                    soundController.PlaySE(WrongSound);
                }
            }


        }
    }

    /// <summary>
    /// 新增進入判斷區的敵人
    /// </summary>
    /// <param name="obj"></param>
    public void NewEnemyInZone(EnemyScript obj)
    {
        this.EnemyInZone.Add(obj);
    }

    /// <summary>
    /// 設定上方顯示的優先處理敵人UI
    /// </summary>
    public void SetInputCodeImage()
    {
        if (this.EnemyInStack.Count > 0)
        {
            for (int i = 0; i < InputCodeShowImageList.Count; i++)
            {
                if (this.EnemyInStack.Count < i + 1)
                {
                    InputCodeShowImageList[i].enabled = false;
                }
                else
                {
                    InputCodeShowImageList[i].enabled = true;
                    InputCodeShowImageList[i].sprite = CodeSprite[this.EnemyInStack[i].InputCode];
                }
            }

        }
        else
        {
            foreach (var item in InputCodeShowImageList)
            {
                item.enabled = false;
            }

        }
    }

    /// <summary>
    /// 如果敵人觸碰到玩家，則會呼叫此功能
    /// </summary>
    /// <param name="obj"></param>
    public void EnemyTouchPlayer(EnemyScript obj)
    {
        //設定攝影機搖晃功能
        ScreenShakeTrigger();
        EnemyScript es = this.EnemyInZone.Find(x => x == obj);
        if (es != null)
        {
            Debug.Log("Object Match");
            this.EnemyInZone.Remove(obj);
        }
        this.EnemyInStack.Add(obj);
        if (this.EnemyInStack.Count > 4)
        {
            GameOver();
            return;
        }
        //播放錯誤的聲音
        if (soundController && WrongSound)
        {
            soundController.PlaySE(WrongSound);
        }
        SetInputCodeImage();
    }

    private void GameOver()
    {
        GameState = GameProgress.GameOver;
        GameOverText.SetActive(true);
    }

    private void GameClear()
    {
        GameState = GameProgress.GameClear;
    }

    public void CreateEnemyFromTwitch(string ID)
    {
        Debug.Log("TwitchGo");
        //先定義新出現的敵人要輸入的代碼是甚麼
        int code = UnityEngine.Random.Range(0, 4);
        //生成物件
        GameObject SpawnObj = Instantiate(EnemyObj_Twitch, EnemySpawnPoint.position, Quaternion.identity);
        //取得物件Script作設定用
        EnemyScript es = SpawnObj.GetComponent<EnemyScript>();
        if (es != null)
        {
            es.CreateSet(Isleft);
            es.MoveSpeed = EnemySpeed;

            if (this.NowDifficult >= 0)
            {
                if (difficultSetup[this.NowDifficult].Rotate)
                {
                    int RotateRnd = UnityEngine.Random.Range(0, 4);
                    if (RotateRnd == 0)
                    {
                        es.IsRotate = true;
                    }
                }
                if (difficultSetup[this.NowDifficult].fadeOut)
                {

                    int FadeRnd = UnityEngine.Random.Range(0, 4);
                    if (FadeRnd == 0)
                    {
                        es.fadeOut = true;
                    }
                }
            }
            es.SetNameText(ID);
            es.InputCode = code;
        }
    }

    public void ToScene(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }

    /// <summary>
    /// 生成敵人的功能，觸發一次後會依生成速率觸發下一次生成
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnController()
    {

        //先定義新出現的敵人要輸入的代碼是甚麼
        int code = UnityEngine.Random.Range(0, 4);
        //生成物件
        GameObject SpawnObj = Instantiate(EnemyObj, EnemySpawnPoint.position, Quaternion.identity);
        //取得物件Script作設定用
        EnemyScript es = SpawnObj.GetComponent<EnemyScript>();
        if (es != null)
        {
            es.CreateSet(Isleft);
            es.MoveSpeed = EnemySpeed;

            if (this.NowDifficult >= 0)
            {
                if (difficultSetup[this.NowDifficult].Rotate)
                {
                    int RotateRnd = UnityEngine.Random.Range(0, 4);
                    if (RotateRnd == 0)
                    {
                        es.IsRotate = true;
                    }
                }
                if (difficultSetup[this.NowDifficult].fadeOut)
                {

                    int FadeRnd = UnityEngine.Random.Range(0, 4);
                    if (FadeRnd == 0)
                    {
                        es.fadeOut = true;
                    }
                }
            }

            es.InputCode = code;
            es.EnemySprite.sprite = EnemySprite[code];
        }
        //等待下一次的生成時間
        yield return new WaitForSeconds(SpawnFreq);
        if (GameState == GameProgress.Playing)
            StartCoroutine(SpawnController());

    }

    /// <summary>
    /// 進度條推進功能
    /// </summary>
    /// <returns></returns>
    IEnumerator ProgressBarController()
    {
        Debug.Log(GameState);
        if (GameState == GameProgress.GameOver) yield break;
        this.ProgressBar.fillAmount = this.GameCurrentTime / this.GameTotalTime;
        yield return new WaitForSeconds(0.01f);
        this.GameCurrentTime += 0.01f;

        float tmpFreq = this.SpawnFreq;
        float tmpSpeed = this.EnemySpeed;
        //foreach (var item in difficultSetup)
        //{
        //    if (this.GameCurrentTime >= item.TimeToStart)
        //    {
        //        tmpFreq = item.Freq;
        //        tmpSpeed = item.Speed;  
        //    }
        //}
        if (difficultSetup.Count > 0)
        {
            for (int i = 0; i < difficultSetup.Count; i++)
            {
                if (this.GameCurrentTime >= difficultSetup[i].TimeToStart)
                {
                    tmpFreq = difficultSetup[i].Freq;
                    tmpSpeed = difficultSetup[i].Speed;
                    this.NowDifficult = i;
                }
            }
        }
        this.SpawnFreq = tmpFreq;
        this.EnemySpeed = tmpSpeed;

        if (this.GameCurrentTime >= GameTotalTime)
        {
            this.GameClearText.SetActive(true);
            GameState = GameProgress.GameClear;
            yield break;
        }
        else
        {
            StartCoroutine(ProgressBarController());
        }

    }

    /// <summary>
    /// 禁止輸入跑進度條
    /// </summary>
    /// <param name="DurationNow"></param>
    /// <returns></returns>
    IEnumerator DisableCountDown(float DurationNow)
    {
        this.IsInputDisable = true;
        this.DisableBar.enabled = true;
        this.DisableBar.fillAmount = DurationNow / this.DisableDuration;
        yield return new WaitForSeconds(0.01f);
        DurationNow -= 0.01f;

        if (DurationNow <= 0)
        {
            this.IsInputDisable = false;
            this.DisableBar.enabled = false;
            yield return null;
        }
        else
        {
            StartCoroutine(DisableCountDown(DurationNow));
        }

    }
}
