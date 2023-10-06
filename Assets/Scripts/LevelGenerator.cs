using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelGenerator : MonoBehaviour
{
    private static LevelGenerator _instance;  // 游戏管理器的实例
    public static LevelGenerator Instance  // 游戏管理器的单例
    {
        get
        {
            return _instance;
        }
    }

    public GameObject pacman;  // Pacman 游戏对象
    public GameObject pink;  // 粉色鬼魂游戏对象
    public GameObject yello;  // 黄色鬼魂游戏对象
    public GameObject blue;  // 蓝色鬼魂游戏对象
    public GameObject red;  // 红色鬼魂游戏对象
    public GameObject Starts;  // 游戏开始界面
    public GameObject game;  // 游戏界面
    public GameObject startcountDown;  // 游戏开始倒计时界面
    public GameObject GameOver;  // 游戏结束界面
    public GameObject Win;  // 游戏胜利界面
  //  public AudioClip startClip;  // 游戏开始音效
    public Text remainText;  // 剩余点数的文本
    public Text nowText;  // 当前吃掉的点数的文本
    public Text scoreText;  // 分数的文本

    public bool isSuperPacman = false;  // 是否处于超级Pacman状态
    public List<int> usingIndex = new List<int>();  // 使用的索引列表
    public List<int> rawIndex = new List<int> { 0, 1, 2, 3 };  // 原始索引列表
    private List<GameObject> pacdotGos = new List<GameObject>();  // Pacdot游戏对象列表
    private int pacdotNum = 0;  // Pacdot数量
    private int nowEat = 0;  // 当前吃掉的点数
    public int score = 0;  // 分数

    private void Awake()
    {
        _instance = this;  // 设置游戏管理器的实例为当前实例
        Screen.SetResolution(1024, 768, false);  // 设置屏幕分辨率
        int tempCount = rawIndex.Count;
        for (int i = 0; i < tempCount; i++)  // 随机排序使用的索引列表
        {
            int tempIndex = Random.Range(0, rawIndex.Count);
            usingIndex.Add(rawIndex[tempIndex]);
            rawIndex.RemoveAt(tempIndex);
        }
        foreach (Transform t in GameObject.Find("Maze").transform)  // 获取迷宫中的所有Pacdot游戏对象
        {
            pacdotGos.Add(t.gameObject);
        }
        pacdotNum = GameObject.Find("Maze").transform.childCount;  // 获取迷宫中Pacdot的数量
    }

    private void Start()
    {
        SetGameState(false);  // 设置游戏状态为暂停
        SetGameState(true);
        // 延迟10秒后调用CreateSuperPacdot方法
        Invoke("CreateSuperPacdot", 10f);
        // 激活game游戏对象
        game.SetActive(true);
        // 播放音频
        GetComponent<AudioSource>().Play();
    }

    private void Update()
    {
        if (nowEat == pacdotNum && pacman.GetComponent<PacmanMove>().enabled != false)  // 当吃掉的点数等于总点数且Pacman可以移动时
        {
            game.SetActive(false);  // 隐藏游戏界面
            Instantiate(Win);  // 实例化胜利界面
            StopAllCoroutines();  // 停止所有协程
            SetGameState(false);  // 设置游戏状态为暂停
        }
        if (nowEat == pacdotNum)  // 当吃掉的点数等于总点数
        {
            if (Input.anyKeyDown)  // 如果有按键输入
            {
                SceneManager.LoadScene(0);  // 重新加载场景
            }
        }
        if (game.activeInHierarchy)  // 如果游戏界面是激活状态
        {
            remainText.text = "Remain:\n\n" + (pacdotNum - nowEat);  // 更新剩余点数的文本
            nowText.text = "Eaten:\n\n" + nowEat;  // 更新当前吃掉的点数的文本
            scoreText.text = "Score:\n\n" + score;  // 更新分数的文本
        }
    }
    public void OnStartButton()
    {
        // 启动协程，播放开始倒计时
        StartCoroutine(PlayStartCountDown());
        // 在指定位置播放音频剪辑
     //   AudioSource.PlayClipAtPoint(startClip, new Vector3(0, 0, -5));
        // 禁用Starts游戏对象
        Starts.SetActive(false);
    }

    public void OnExitButton()
    {
        // 退出应用程序
        Application.Quit();
    }

    IEnumerator PlayStartCountDown()
    {
        // 实例化startcountDown游戏对象
        GameObject go = Instantiate(startcountDown);
        // 等待指定时间（4秒）
        yield return new WaitForSeconds(4f);
        // 销毁startcountDown游戏对象
        Destroy(go);
        // 设置游戏状态为true
        SetGameState(true);
        // 延迟10秒后调用CreateSuperPacdot方法
        Invoke("CreateSuperPacdot", 10f);
        // 激活game游戏对象
        game.SetActive(true);
        // 播放音频
        GetComponent<AudioSource>().Play();
    }

    public void OnEatPacdot(GameObject go)
    {
        auds.Play();
        // 增加已吃的数量
        nowEat++;
        // 增加分数
        score += 100;
        // 从pacdotGos列表中移除指定的游戏对象
        pacdotGos.Remove(go);
    }
    public AudioSource auds;
    public void OnEatSuperPacdot()
    {

        // 增加分数
        score += 200;
        // 延迟10秒后调用CreateSuperPacdot方法
        Invoke("CreateSuperPacdot", 10f);
        // 设置isSuperPacman为true
        isSuperPacman = true;
        // 冻结敌人
        FreezeEnemy();
        // 启动协程，恢复敌人
        StartCoroutine(RecoveryEnemy());
    }

    IEnumerator RecoveryEnemy()
    {
        // 等待指定时间（3秒）
        yield return new WaitForSeconds(3f);
        // 取消冻结敌人
        DisFreezeEnemy();
        // 设置isSuperPacman为false
        isSuperPacman = false;
    }

    private void CreateSuperPacdot()
    {
        // 如果pacdotGos列表中的元素数量小于5，则返回
        if (pacdotGos.Count < 5)
        {
            return;
        }
        // 生成随机索引
        int tempIndex = Random.Range(0, pacdotGos.Count);
        // 设置指定游戏对象的缩放
        pacdotGos[tempIndex].transform.localScale = new Vector3(3, 3, 3);
        // 设置指定游戏对象的isSuperPacdot属性为true
        pacdotGos[tempIndex].GetComponent<Pacdot>().isSuperPacdot = true;
    }

    private void FreezeEnemy()
    {
        // 禁用敌人的移动组件
        pink.GetComponent<GhostMove>().enabled = false;
        yello.GetComponent<GhostMove>().enabled = false;
        blue.GetComponent<GhostMove>().enabled = false;
        red.GetComponent<GhostMove>().enabled = false;
        // 设置敌人的精灵渲染器颜色为灰色
        pink.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, 0.7f);
        yello.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, 0.7f);
        blue.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, 0.7f);
        red.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, 0.7f);
    }

    private void DisFreezeEnemy()
    {
        // 启用敌人的移动组件
        pink.GetComponent<GhostMove>().enabled = true;
        yello.GetComponent<GhostMove>().enabled = true;
        blue.GetComponent<GhostMove>().enabled = true;
        red.GetComponent<GhostMove>().enabled = true;
        // 设置敌人的精灵渲染器颜色为白色
        pink.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        yello.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f,1f);
        blue.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        red.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
    }

    private void SetGameState(bool state)
    {
        // 根据给定的状态启用或禁用组件
        pacman.GetComponent<PacmanMove>().enabled = state;
        pink.GetComponent<GhostMove>().enabled = state;
        yello.GetComponent<GhostMove>().enabled = state;
        blue.GetComponent<GhostMove>().enabled = state;
        red.GetComponent<GhostMove>().enabled = state;
    }
}
