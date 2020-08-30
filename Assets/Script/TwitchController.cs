using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.IO;

public class TwitchController : MonoBehaviour
{
    /// <summary>
    /// twitch連線相關功能
    /// </summary>
    private TcpClient twitchClinet;
    private StreamReader reader;
    private StreamWriter writer;

    [SerializeField]
    private UnityEngine.UI.Text debugText;

    //oauth要去https://twitchapps.com/申請 userName與chanelName輸入想要連線的聊天室與名稱
    public string userName, oAuth, chanelName;

    void Start()
    {
        //連線
        Connect();
    }
    
    void Update()
    {
        //如果未連線，重新連線
        if (!twitchClinet.Connected)
        {
            Connect();
        }
        //讀取聊天室
        ReadChat();
    }

    /// <summary>
    /// 連線至Twitch聊天室
    /// </summary>
    private void Connect()
    {
        Debug.Log("StartConnectTwitch");
        twitchClinet = new TcpClient("irc.chat.twitch.tv", 6667);
        reader = new StreamReader(twitchClinet.GetStream());
        writer = new StreamWriter(twitchClinet.GetStream());

        writer.WriteLine("PASS " + oAuth);
        writer.WriteLine("NICK " + userName);
        writer.WriteLine("USER " + userName + " 8 * :" + userName);
        writer.WriteLine("JOIN #" + chanelName);
        writer.Flush();
    }

    /// <summary>
    /// 讀取文字內容
    /// </summary>
    private void ReadChat()
    {
        if(twitchClinet.Available > 0)
        {
            Debug.Log("Available");
            var message = reader.ReadLine();
            Debug.Log(message);
            if (!message.Contains("PRIVMSG")) { return; }
            //分割字串Index
            var splitPoint = message.IndexOf("!", 1);
            //取得使用者ID
            var chatName = message.Substring(0, splitPoint);
            chatName = chatName.Substring(1);

            //取得使用者輸入內容
            splitPoint = message.IndexOf(":", 1);
            message = message.Substring(splitPoint + 1);

            //如果有DebugUI，可以顯示
            if (debugText)
            {
                debugText.text = message;
            }
            //檢查是否有指令
            commandMatcher(message, chatName);
        }
    }

    /// <summary>
    /// 檢查是否有指定指令
    /// </summary>
    /// <param name="message"></param>
    /// <param name="ID"></param>
    private void commandMatcher(string message,string ID)
    {
        if(message == "!spawn")
        {
            Debug.Log(ID);
            GameManager.GameMainManager.CreateEnemyFromTwitch(ID);
        }
    }

   

}
