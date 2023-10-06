using UnityEngine;

public class Pacdot : MonoBehaviour
{
    public bool isSuperPacdot = false; // 声明公共布尔变量 isSuperPacdot，并赋值为 false

    private void OnTriggerEnter2D(Collider2D collision) // 声明私有无返回值方法 OnTriggerEnter2D，传入 Collider2D 参数 collision
    {
        if (collision.gameObject.name == "Pacman") // 如果发生碰撞的游戏对象的名称是 "Pacman"
        {
            if (isSuperPacdot) // 如果 isSuperPacdot 为 true
            {
                LevelGenerator.Instance.OnEatPacdot(gameObject); // 调用 GameManager 实例的 OnEatPacdot 方法，传入当前游戏对象
                LevelGenerator.Instance.OnEatSuperPacdot(); // 调用 GameManager 实例的 OnEatSuperPacdot 方法

                Destroy(gameObject); // 销毁当前游戏对象
            }
            else // 否则
            {
                LevelGenerator.Instance.OnEatPacdot(gameObject); // 调用 GameManager 实例的 OnEatPacdot 方法，传入当前游戏对象
                Destroy(gameObject); // 销毁当前游戏对象
            }
        }
    }
}
