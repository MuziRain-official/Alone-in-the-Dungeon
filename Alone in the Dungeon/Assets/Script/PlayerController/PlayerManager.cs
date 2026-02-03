/* PlayerManager.cs
* 负责管理玩家各个组件的单例脚本
*/

using UnityEngine;

namespace PlayerController
{
    public class PlayerManager : MonoBehaviour
    {
        // 单例实例
        public static PlayerManager Instance { get; private set; }
        
        // 对各个功能脚本的引用
        public OrientationController orientationController { get; private set; }
        public MovementController movementController { get; private set; }
        public InputHandler inputHandler { get; private set; }
        public AnimationController animationController { get; private set; }
        
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                InitializeComponents();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        void InitializeComponents()
        {
            // 获取同一个GameObject上的各个组件
            orientationController = GetComponent<OrientationController>();
            movementController = GetComponent<MovementController>();
            inputHandler = GetComponent<InputHandler>();
            animationController = GetComponent<AnimationController>();
        }
        
        // 玩家Transform的便捷访问属性
        public Transform PlayerTransform => transform;
        
        // 玩家GameObject的便捷访问属性
        public GameObject PlayerGameObject => gameObject;
    }
}