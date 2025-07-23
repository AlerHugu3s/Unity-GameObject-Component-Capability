# UnityGCC

这是一个基于Unity的游戏开发框架，实现了GDC 2025演讲"Capabilities: Coding ALL the Gameplay for 'Split Fiction'"中介绍的Capabilities编程设计模式。

## 🎯 设计理念

本项目采用了Capabilities架构模式，这是一种模块化的游戏逻辑组织方式：

- **Capabilities（能力）**: 独立的游戏逻辑模块，如移动、攻击、交互等
- **Components（组件）**: 数据容器和状态管理器
- **统一生命周期**: 所有Capabilities按照预定义的TickGroup顺序执行
- **数据驱动**: 通过GCCSheet资源文件配置游戏对象

## 🏗️ 核心架构

### 核心类结构

```
CapabilitiesController (单例)
├── 管理所有Capabilities的生命周期
├── 按TickGroup顺序执行
└── 处理Capability的注册和注销

GCCSheetLoader
├── 从GCCSheet资源加载配置
├── 自动创建Components
└── 自动实例化Capabilities

BaseCapabilities (抽象基类)
├── ShouldActivated() - 激活条件
├── ShouldDeActivated() - 停用条件
├── OnActivated() - 激活时执行
├── OnDeActivated() - 停用时执行
└── TickActive() - 每帧更新

BaseComponent (组件基类)
├── 数据存储和状态管理
├── Tag阻塞系统
└── 与Capabilities交互
```

### TickGroup执行顺序

```
1. SeparatedTickOrder   - 独立执行顺序
2. Input               - 输入处理
3. BeforeMovement      - 移动前处理
4. InfluenceMovement   - 移动影响
5. ActionMovement      - 动作移动
6. Movement            - 移动执行
7. LastMovement        - 移动后处理
8. BeforeGameplay      - 游戏逻辑前
9. Gameplay            - 游戏逻辑
10. AfterGameplay      - 游戏逻辑后
11. AfterPhysics       - 物理后处理
12. Audio              - 音频处理
13. PostWork           - 后续工作
```

## 🚀 快速开始

### 1. 环境要求

- Unity 2022.3 LTS 或更高版本
- Visual Studio 2022 或 Visual Studio Code

### 2. 基础设置

#### 步骤1: 添加CapabilitiesController到场景

```csharp
// 在场景中创建一个空的GameObject
// 添加CapabilitiesController组件（自动成为单例）
```

#### 步骤2: 创建游戏对象

```csharp
// 创建GameObject
// 添加GCCSheetLoader组件
// 创建并配置GCCSheet资源
```

#### 步骤3: 创建自定义Capability

```csharp
using UnityGCC.Capabilities;

public class PlayerMoveCapability : BaseCapabilities
{
    public override TickGroup TickGroup => TickGroup.Movement;
    
    public override bool ShouldActivated()
    {
        // 定义激活条件
        return Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
    }
    
    public override void OnActivated()
    {
        base.OnActivated();
        Debug.Log("开始移动");
    }
    
    public override void TickActive(float deltaTime)
    {
        base.TickActive(deltaTime);
        
        if (bActive)
        {
            // 执行移动逻辑
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            
            Vector3 movement = new Vector3(horizontal, 0, vertical) * 5f * deltaTime;
            Owner.transform.Translate(movement);
        }
    }
    
    public override bool ShouldDeActivated()
    {
        return Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0;
    }
    
    public override void OnDeActivated()
    {
        base.OnDeActivated();
        Debug.Log("停止移动");
    }
}
```

#### 步骤4: 创建自定义Component

```csharp
using UnityGCC.Components;

public class PlayerComponent : BaseComponent
{
    [Header("玩家属性")]
    public float moveSpeed = 5f;
    public float jumpHeight = 2f;
    public int health = 100;
    
    protected override void Awake()
    {
        base.Awake();
        // 初始化玩家特定的数据
    }
    
    // 玩家特定的方法
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            // 处理死亡逻辑
        }
    }
}
```

## 📋 使用流程

### 1. 创建GCCSheet资源

1. 在Project窗口右键 → Create → GCC Sheet
2. 配置Components列表（添加需要的组件脚本）
3. 配置Capabilities列表（添加需要的能力脚本）
4. 设置各个组件的属性值

### 2. 配置游戏对象

1. 创建GameObject
2. 添加`GCCSheetLoader`组件
3. 将创建的GCCSheet拖拽到Sheet字段
4. 运行游戏，系统将自动：
   - 添加配置的Components
   - 创建配置的Capabilities
   - 按生命周期顺序执行

### 3. Tag阻塞系统

```csharp
// 在Component中阻塞特定标签
playerComponent.BlockTags(new[] { TagEnum.Movement }, instigator);

// 解除阻塞
playerComponent.UnBlockTags(new[] { TagEnum.Movement }, instigator);

// 检查标签是否被阻塞
bool isBlocked = playerComponent.IsTagBlocked(TagEnum.Movement, instigator);
```

## 🔧 高级特性

### Instigator系统

```csharp
// 每个Capability都有自己的Instigator
public override void SetUp()
{
    base.SetUp();
    // Instigator自动创建，用于标识来源
}
```

### 动态Capability管理

```csharp
// 运行时注册新的Capability
BaseCapabilities newCapability = new CustomCapability();
newCapability.Owner = gameObject;
newCapability.SetUp();

// 移除Capability会在Owner销毁时自动处理
```

## 📁 项目结构

```
UnityGCC/
├── Assets/
│   ├── Scenes/
│   │   └── SampleScene.unity          # 示例场景
│   └── Scripts/
│       ├── Capabilities/              # Capability实现
│       │   └── BaseCapabilities.cs    # Capability基类
│       ├── Components/                # Component实现
│       │   └── BaseComponent.cs       # Component基类
│       ├── Editor/                    # 编辑器扩展
│       │   └── GCCSheetEditor.cs      # GCCSheet编辑器
│       ├── CapabilitiesController.cs  # 核心控制器
│       ├── GCCSheet.cs               # 配置资源类
│       ├── GCCSheetLoader.cs         # 加载器
│       └── Instigator.cs             # 来源标识
└── README.md
```

## 📄 许可证

本项目采用MIT许可证 - 查看[LICENSE](LICENSE)文件了解详情

## 🙏 致谢

- 感谢GDC 2025演讲"Capabilities: Coding ALL the Gameplay for 'Split Fiction'"提供的设计灵感
- 感谢Unity社区的支持和贡献

## 📞 联系方式

如有问题或建议，请通过以下方式联系：

- 提交Issue
- 发起讨论
- 创建Pull Request

---

⭐ 如果这个项目对您有帮助，请考虑给它一个Star！ 