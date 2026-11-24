# 像素跑酷 · Unity 2D 小游戏

一个用 **Unity 2022.3 LTS + C#** 从零实现的 2D 无尽跑酷小游戏。

角色自动向前奔跑，玩家通过跳跃躲避迎面而来的障碍，跑得越远得分越高，速度与难度随进度动态提升。

> 演示视频：https://t.bilibili.com/1274268181992570896
>


---

## 玩法与操作

| 操作 | 说明 |
| --- | --- |
| 空格 / 鼠标左键 / 触屏点击 | 开始游戏、跳跃、结束后重开 |
| 撞到红色障碍 | 游戏结束 |

- 每移动 1 个世界单位得 10 分
- 移动速度随时间线性增长（6 → 14），障碍间距按当前速度等比放大
- 最高分通过 `PlayerPrefs` 本地持久化

---

## 功能特性

- **完整游戏循环**：准备（Ready）→ 进行（Running）→ 结束（GameOver）三态状态机
- **物理跳跃**：`Rigidbody2D` + `BoxCollider2D`，落地检测通过碰撞回调实现，帧率无关
- **动态难度**：速度随时间提升，障碍间距 = 基础值 + 速度系数，保证任意速度下均可跳过
- **视差滚动**：远景建筑 0.35 倍速、地面接缝 1.0 倍速循环滚动
- **零素材依赖**：角色、障碍、地面等精灵由代码通过纹理绘制（带抗锯齿的圆角矩形），音效由正弦扫频实时合成
- **自动搭场景**：通过 `RuntimeInitializeOnLoadMethod` 在进入 Play 时构建全部对象，**无需手动拖拽任何组件**

---

## 技术栈

- Unity 2022.3 LTS（兼容 Unity 6，代码内已做 `UNITY_6000_0_OR_NEWER` 版本分支）
- C#
- Rigidbody2D / BoxCollider2D / Trigger 碰撞
- SpriteRenderer + 程序化纹理
- IMGUI（OnGUI）绘制 HUD 与界面
- 新 / 旧输入系统兼容层

---

## 目录结构

```
Assets/
└── Scripts/
    ├── GameBootstrap.cs      # 启动器：Play 时自动搭建场景
    ├── GameManager.cs        # 状态机、分数、速度、最高分
    ├── PlayerController.cs   # 跳跃、落地检测、挤压动画
    ├── ObstacleSpawner.cs    # 按移动距离生成障碍
    ├── Obstacle.cs           # 障碍移动与碰撞判负
    ├── ScrollingObject.cs    # 通用滚动 + 循环回收（视差）
    ├── SpriteFactory.cs      # 程序化生成精灵（SDF 圆角矩形）
    ├── SoundFX.cs            # 程序化合成音效
    ├── InputHelper.cs        # 新旧输入系统兼容
    ├── GameConfig.cs         # 全局可调参数
    ├── ViewBounds.cs         # 相机可视范围
    ├── GameUI.cs             # HUD 与界面
    └── GroundMarker.cs       # 地面标记组件
```

---

## 如何运行

1. 使用 **Unity 2022.3 LTS**（或 Unity 6）打开本项目
2. 等待脚本编译完成（Console 无报错）
3. 打开 `Assets/Scenes/SampleScene.unity`，直接点击 **Play**

> 场景中不需要挂任何脚本：`GameBootstrap` 会在进入 Play 后自动创建相机、地面、玩家与各类管理器。

## 如何构建

`File → Build Settings` → 平台选择 **Windows, Mac, Linux** → **Build**，选择一个空文件夹作为输出目录即可得到可执行程序。

---

## 实现要点

**难度曲线为什么不会出现"必死局"**
跳跃的滞空时间由重力与初速度决定，是固定值；因此障碍间距按 `基础间距 + 速度 × 系数` 随速度增长，保证高速下玩家仍有足够的落地与起跳距离。

**碰撞为什么用 Trigger**
障碍只需"撞到即判负"，不需要物理反弹。`BoxCollider2D.isTrigger = true` 只触发 `OnTriggerEnter2D` 事件，不产生推力，手感更干净。

**为什么全部用代码生成美术资源**
避免引入外部素材与授权问题，同时让项目的全部逻辑与表现都可版本管理；圆角矩形通过有符号距离场（SDF）逐像素计算并做边缘抗锯齿。

---

## 开发说明

本项目在 AI 编码工具辅助下完成初版实现，作者逐文件阅读、调试并调整了跳跃力度、障碍间距、难度曲线等参数，修复了输入系统兼容与碰撞判定等问题。

## License

MIT
