using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyScript : MonoBehaviour
{
    /// <summary>
    /// 障礙物(敵人)狀態列舉
    /// </summary>
    public enum State
    {
        Running,//移動中
        InZone,//已通過判斷區
        Defeat,//已擊敗
        Follow//跟隨玩家
    }

    /// <summary>
    /// 目前此敵人狀態
    /// </summary>
    [SerializeField]
    public State EnemyState;

    public float MoveSpeed = 2f;

    /// <summary>
    /// 是否轉動
    /// </summary>
    public bool IsRotate;

    /// <summary>
    /// 是否會漸漸消失
    /// </summary>
    public bool fadeOut;

    /// <summary>
    /// 是否開始消失
    /// </summary>
    public bool StartFade;

    /// <summary>
    /// 消失速率
    /// </summary>
    public float fadeValue;

    /// <summary>
    /// 旋轉速度
    /// </summary>
    public float RotateSpeed;

    public Rigidbody2D rb;

    /// <summary>
    /// 顯示的指令圖片
    /// </summary>
    public SpriteRenderer CodeSprite;

    /// <summary>
    /// 顯示的敵人圖片
    /// </summary>
    public SpriteRenderer EnemySprite;

    /// <summary>
    /// 要集合的位置
    /// </summary>
    Vector3 GroupPosition;

    /// <summary>
    /// 欲移動方向
    /// </summary>
    public Vector2 Movement;

    /// <summary>
    /// twitch ID(如果有的話)
    /// </summary>
    public Text TwitchID;

    /// <summary>
    /// 敵人的碰撞判斷範圍
    /// </summary>
    public Collider2D touchCollider;

    /// <summary>
    /// 需要輸入的指令 0:上 1:下 2:左 3:右
    /// </summary>
    public int InputCode;

    // Start is called before the first frame update
    void Start()
    {
        //隨機設定轉動速度，如果有觸發需要轉動的話，則可以這麼處理
        RotateSpeed = Random.Range(-2, 2);
        //找到碰到玩家後的集合點
        this.GroupPosition = GameObject.Find("GroupPoint").transform.position;
        //預設為移動中
        this.EnemyState = State.Running;
        this.fadeValue = 0;
        StartFade = false;
    }

    private void FixedUpdate()
    {
        //如果現在可以遊玩的狀態(非暫停，且此敵人狀態在移動中與已經過判斷區的時候
        if (GameManager.GameMainManager.GameState == GameManager.GameProgress.Playing && (int)this.EnemyState<2)
        {
            //如果有需要旋轉，在這邊加入
            if (IsRotate) rb.rotation += RotateSpeed;
            
            //指令圖片消失功能
            if (this.StartFade) {
                this.fadeValue += 0.025f;
                float FadeValue = (float)System.Math.Cos(this.fadeValue);
                if (FadeValue < 0) FadeValue *= -1;
                CodeSprite.color = new Color(1, 1, 1, FadeValue);
            }
            rb.MovePosition(rb.position + Movement * MoveSpeed * Time.fixedDeltaTime);
        }

        if (GameManager.GameMainManager.GameState == GameManager.GameProgress.Playing && EnemyState==State.Follow)
        {
            Vector2 direction = (GroupPosition - transform.position).normalized;
            rb.MovePosition(rb.position + direction * MoveSpeed * Time.fixedDeltaTime);
        }

    }

    public void SetSprite(Sprite SetSprite)
    {
        if (fadeOut) this.StartFade = true;
        CodeSprite.sprite = SetSprite;
        this.EnemyState = State.InZone;
    }

    public void CreateSet(bool IsLeft)
    {
        if (IsLeft)
        {
            Movement = Vector2.left;
        }
        else
        {
            Movement = Vector2.right;
        }
    }

    /// <summary>
    /// 設定來自Twitch的善意
    /// </summary>
    /// <param name="ID"></param>
    public void SetNameText(string ID)
    {
        if (this.TwitchID)
        {
            this.TwitchID.text = ID+"的善意~";
        }
        
    }

    /// <summary>
    /// 被消除
    /// </summary>
    /// <param name="UserInputCode"></param>
    public void DestroyEnemy(int UserInputCode)
    {
        if (UserInputCode == this.InputCode)
        {
            CodeSprite.enabled = false;
            //狀態切換為已擊敗
            this.EnemyState = State.Defeat;
            //取消碰撞
            touchCollider.enabled = false;
            //rigidbody相關設定
            rb.gravityScale = 1f;
            rb.mass = 1f;
            //旋轉
            rb.AddTorque(800f);
            //停止移動
            rb.MovePosition(rb.position);
            //設定拋出力量與方向
            Vector2 force = new Vector2(Random.Range(-5, 5), Random.Range(10, 15));
            Debug.Log(force);
            //依上面設定拋出
            rb.AddForce(force, ForceMode2D.Impulse);
            //五秒後刪除此物件
            Destroy(this.gameObject,5f);
        }
    }

}
