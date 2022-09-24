using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Script
{
    /// <summary>
    /// 移动控制器
    /// </summary>
    public class MoveControl
    {
        #region 属性
        //玩家
        public Transform playerTrans { get; set; }
        //所有敌人
        public List<Transform> enemiesTrans { get; private set; }
        //移动状态
        public bool _isMoving { get; set; }
        //移动时间
        private float _moveTime = 0.2f;
        //路径
        public Stack<Node> _path;
        public Dictionary<GameObject ,Stack<Node>> paths = new Dictionary<GameObject ,Stack<Node>>();

        #endregion

        //单例对象
        private static MoveControl _moveController = null;
        private AStarPathFinding _pathFinding = AStarPathFinding.GetInstance();
        
        private MoveControl(){}

        /// <summary>
        /// 创建并获取移动控制器的单例
        /// </summary>
        /// <returns></returns>
        public static MoveControl GetInstance()
        {
            if (_moveController == null)
            {
                _moveController = new MoveControl();
                _moveController.enemiesTrans = new List<Transform>();
                _moveController._isMoving = false;
            }

            return _moveController;
        }

        /// <summary>
        /// 角色与敌人的移动
        /// </summary>
        /// <param name="playerDir">玩家移动方向</param>
        public void Moving(Node playerPosition)
        {
            Node[,] graph = _pathFinding.GraphNodes;
            if (!_isMoving)
            {
            
                //玩家移动
                playerTrans.DOMove(playerPosition.position, _moveTime).OnComplete(Reset);
    
                //敌人移动
                Node nextPos;
                foreach (Transform enemy in enemiesTrans)
                {
                    //移动方向判定
                    /*if (enemy.position.y < final.y)
                        dir = (int)enemy.position.x != (int)final.x
                            ? (enemy.position.x < final.x ? Vector3.right : Vector3.left)
                            : Vector3.up;
                    else if (enemy.position.y > final.y) dir = Vector3.down;
                    else dir = enemy.position.x < final.x ? Vector3.right : Vector3.left;*/
                    //开始移动
                    nextPos = paths[enemy.gameObject].Pop();
                    enemy.DOMove(nextPos.position, _moveTime).OnComplete(Reset);
                    paths[enemy.gameObject] = _pathFinding.FindPath(nextPos, playerPosition);
                }
                //锁定移动状态
                _isMoving = true;
            }
        }

        /// <summary>
        /// 重置移动条件
        /// </summary>
        private void Reset()
        {
            _isMoving = false;
        }
    }
}
