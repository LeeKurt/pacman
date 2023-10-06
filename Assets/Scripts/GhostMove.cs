using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GhostMove : MonoBehaviour // 定义 GhostMove 类，继承自 MonoBehaviour
{
    public GameObject[] wayPointsGos; // 声明公共 GameObject 数组 wayPointsGos
    public float speed = 0.2f; // 声明公共浮点数 speed，并赋值为0.2f
    private List<Vector3> wayPoints = new List<Vector3>(); // 声明私有 Vector3 列表 wayPoints
    private int index = 0; // 声明私有整数 index，并赋值为0
    private Vector3 startPos; // 声明私有 Vector3 startPos

    private void Start() // 声明私有无返回值方法 Start()
    {
        startPos = transform.position + new Vector3(0, 3, 0); // 将 startPos 设置为当前位置加上向量 (0, 3, 0)
        LoadAPath(wayPointsGos[LevelGenerator.Instance.usingIndex[GetComponent<SpriteRenderer>().sortingOrder - 2]]); // 调用 LoadAPath 方法，传入 wayPointsGos 数组中的指定游戏对象
    }

    private void FixedUpdate() // 声明私有无返回值方法 FixedUpdate()
    {
        if (transform.position != wayPoints[index]) // 如果当前位置不等于 wayPoints[index]
        {
            Vector2 temp = Vector2.MoveTowards(transform.position, wayPoints[index], speed); // 将 temp 设置为当前位置向 wayPoints[index] 移动一定距离的向量
            GetComponent<Rigidbody2D>().MovePosition(temp); // 通过获取 Rigidbody2D 组件，并调用 MovePosition 方法移动到 temp 的位置
        }
        else // 否则
        {
            index++; // index 自增
            if (index >= wayPoints.Count) // 如果 index 大于等于 wayPoints 列表的数量
            {
                index = 0; // index 设为0
                LoadAPath(wayPointsGos[Random.Range(0, wayPointsGos.Length)]); // 调用 LoadAPath 方法，传入 wayPointsGos 数组中的随机游戏对象
            }
        }
        Vector2 dir = wayPoints[index] - transform.position; // 计算方向向量 dir，即 wayPoints[index] 减去当前位置
        GetComponent<Animator>().SetFloat("DirX", dir.x); // 获取 Animator 组件，并设置 "DirX" 参数为 dir.x
        GetComponent<Animator>().SetFloat("DirY", dir.y); // 获取 Animator 组件，并设置 "DirY" 参数为 dir.y
    }

    private void LoadAPath(GameObject go) // 声明私有无返回值方法 LoadAPath，传入 GameObject 参数 go
    {
        wayPoints.Clear(); // 清空 wayPoints 列表
        foreach (Transform t in go.transform) // 遍历 go 游戏对象的子物体
        {
            wayPoints.Add(t.position); // 将子物体的位置添加到 wayPoints 列表中
        }
        wayPoints.Insert(0, startPos); // 在 wayPoints 列表的开头插入 startPos
        wayPoints.Add(startPos); // 在 wayPoints 列表的末尾添加 startPos
    }

    private void OnTriggerEnter2D(Collider2D collision) // 声明私有无返回值方法 OnTriggerEnter2D，传入 Collider2D 参数 collision
    {
        if (collision.gameObject.name == "Pacman") // 如果发生碰撞的游戏对象的名称是 "Pacman"
        {
            if (LevelGenerator.Instance.isSuperPacman) // 如果 GameManager 实例的 isSuperPacman 为 true
            {
                transform.position = startPos - new Vector3(0, 3, 0); // 将当前位置设置为 startPos 减去向量 (0, 3, 0)
                index = 0; // index 设为0
                LevelGenerator.Instance.score += 500; // GameManager 实例的 score 属性增加500
            }
            else // 否则
            {
                collision.gameObject.SetActive(false); // 将碰撞的游戏对象设置为非激活状态（即隐藏）
                LevelGenerator.Instance.game.SetActive(false); // 将 GameManager 实例的 game 游戏对象设置为非激活状态
                Instantiate(LevelGenerator.Instance.GameOver); // 在场景中实例化 GameManager 实例的 GameOver 游戏对象
                Invoke("ReStart", 3f); // 延迟3秒后调用 ReStart 方法
            }
        }
    }

    private void ReStart() // 声明私有无返回值方法 ReStart()
    {
        SceneManager.LoadScene(0); // 加载场景0
    }
}