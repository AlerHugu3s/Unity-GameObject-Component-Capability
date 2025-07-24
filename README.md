# UnityGCC

![Unity](https://img.shields.io/badge/Unity-2022.3+-black.svg?style=flat&logo=unity) 
![License](https://img.shields.io/badge/License-MIT-blue.svg) 
![Language](https://img.shields.io/badge/Language-CSharp-purple.svg)
![Platform](https://img.shields.io/badge/Platform-Unity%20Editor-red.svg)

*[English](#english) | [中文](#chinese)*

---

## English

A Unity-based game development framework implementing the Capabilities programming design pattern introduced in the GDC 2025 talk "Capabilities: Coding ALL the Gameplay for 'Split Fiction'".

### Table of Contents
- [🎯 Design Philosophy](#-design-philosophy)
- [🏗️ Core Architecture](#️-core-architecture)
- [🚀 Quick Start](#-quick-start)
- [📋 Usage Workflow](#-usage-workflow)
- [🔧 Advanced Features](#-advanced-features)
- [🛠️ Developer Tools](#️-developer-tools)
- [📁 Project Structure](#-project-structure)
- [📄 License](#-license--许可证)

## 🎯 Design Philosophy

This project adopts the Capabilities architecture pattern, a modular approach to organizing game logic:

- **Capabilities**: Independent game logic modules, such as movement, combat, interaction, etc.
- **Components**: Data containers and state managers
- **Unified Lifecycle**: All Capabilities execute in predefined TickGroup order
- **Data-Driven**: Configure game objects through GCCSheet resource files

---

## 🏗️ Core Architecture

### Core Class Structure

```
CapabilitiesController (Singleton)
├── Manages all Capabilities lifecycle
├── Executes in TickGroup order
└── Handles Capability registration and unregistration

GCCSheetLoader
├── Loads configuration from GCCSheet resources
├── Automatically creates Components
└── Automatically instantiates Capabilities

BaseCapabilities (Abstract Base Class)
├── ShouldActivated() - Activation conditions
├── ShouldDeActivated() - Deactivation conditions
├── OnActivated() - Execute on activation
├── OnDeActivated() - Execute on deactivation
└── TickActive() - Per-frame update

BaseComponent (Component Base Class)
├── Data storage and state management
├── Tag blocking system
└── Interaction with Capabilities
```

### TickGroup Execution Order

```
1. SeparatedTickOrder   - Independent execution order
2. Input               - Input processing
3. BeforeMovement      - Pre-movement processing
4. InfluenceMovement   - Movement influence
5. ActionMovement      - Action movement
6. Movement            - Movement execution
7. LastMovement        - Post-movement processing
8. BeforeGameplay      - Pre-gameplay logic
9. Gameplay            - Gameplay logic
10. AfterGameplay      - Post-gameplay logic
11. AfterPhysics       - Post-physics processing
12. Audio              - Audio processing
13. PostWork           - Post work
```

## 🚀 Quick Start

### 1. Environment Requirements

- Unity 2022.3 LTS or higher
- Visual Studio 2022 or Visual Studio Code

### 2. Basic Setup

#### Step 1: Add CapabilitiesController to Scene

```csharp
// Create an empty GameObject in the scene
// Add CapabilitiesController component (automatically becomes singleton)
```

#### Step 2: Create Game Object

```csharp
// Create GameObject
// Add GCCSheetLoader component
// Create and configure GCCSheet resource
```

#### Step 3: Create Custom Capability

```csharp
using UnityGCC.Capabilities;

public class PlayerMoveCapability : BaseCapabilities
{
    public override TickGroup TickGroup => TickGroup.Movement;
    
    public override bool ShouldActivated()
    {
        // Define activation conditions
        return Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
    }
    
    public override void OnActivated()
    {
        base.OnActivated();
        Debug.Log("Start moving");
    }
    
    public override void TickActive(float deltaTime)
    {
        base.TickActive(deltaTime);
        
        if (bActive)
        {
            // Execute movement logic
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
        Debug.Log("Stop moving");
    }
}
```

#### Step 4: Create Custom Component

```csharp
using UnityGCC.Components;

public class PlayerComponent : BaseComponent
{
    [Header("Player Properties")]
    public float moveSpeed = 5f;
    public float jumpHeight = 2f;
    public int health = 100;
    
    protected override void Awake()
    {
        base.Awake();
        // Initialize player-specific data
    }
    
    // Player-specific methods
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            // Handle death logic
        }
    }
}
```

## 📋 Usage Workflow

### 1. Create GCCSheet Resource

1. Right-click in Project window → Create → GCC Sheet
2. Configure Components list (add required component scripts)
3. Configure Capabilities list (add required capability scripts)
4. Set property values for each component

### 2. Configure Game Object

1. Create GameObject
2. Add `GCCSheetLoader` component
3. Drag the created GCCSheet to the Sheet field
4. Run the game, the system will automatically:
   - Add configured Components
   - Create configured Capabilities
   - Execute in lifecycle order

### 3. Tag Blocking System

```csharp
// Block specific tags in Component
playerComponent.BlockTags(new[] { TagEnum.Movement }, instigator);

// Unblock tags
playerComponent.UnBlockTags(new[] { TagEnum.Movement }, instigator);

// Check if tag is blocked
bool isBlocked = playerComponent.IsTagBlocked(TagEnum.Movement, instigator);
```

## 🔧 Advanced Features

### Instigator System

```csharp
// Each Capability has its own Instigator
public override void SetUp()
{
    base.SetUp();
    // Instigator automatically created for source identification
}
```

### Dynamic Capability Management

```csharp
// Register new Capability at runtime
BaseCapabilities newCapability = new CustomCapability();
newCapability.Owner = gameObject;
newCapability.SetUp();

// Capability removal is automatically handled when Owner is destroyed
```

## 🛠️ Developer Tools

### Capabilities Monitor 📊

A powerful Unity Editor window for debugging and monitoring the Capability system:

- **Real-time Status Visualization**: Green/red indicators for active/inactive capabilities
- **Timeline Tracking**: Visual timeline showing capability state changes over time  
- **Frame-accurate Logging**: Records exact frame numbers when state changes occur
- **Multi-language Support**: Chinese and English interface
- **Performance Monitoring**: Identify bottlenecks and optimization opportunities

📖 **[View Detailed Documentation](Assets/Scripts/Editor/CapabilitiesMonitor_README.md)**

**Access**: Unity Menu Bar → UnityGCC → Capabilities Monitor

## 📁 Project Structure

```
UnityGCC/
├── Assets/
│   ├── Scenes/
│   │   └── SampleScene.unity              # Sample scene
│   └── Scripts/
│       ├── Capabilities/                  # Capability implementations
│       │   └── BaseCapabilities.cs        # Capability base class
│       ├── Components/                    # Component implementations
│       │   └── BaseComponent.cs           # Component base class
│       ├── Editor/                        # Editor extensions
│       │   ├── GCCSheetEditor.cs          # GCCSheet editor
│       │   ├── CapabilitiesMonitorWindow.cs # Debug monitor window
│       │   └── CapabilitiesMonitor_README.md # Monitor documentation
│       ├── CapabilitiesController.cs      # Core controller
│       ├── GCCSheet.cs                   # Configuration resource class
│       ├── GCCSheetLoader.cs             # Loader
│       └── Instigator.cs                 # Source identifier
└── README.md
```

---

## Chinese

这是一个基于Unity的游戏开发框架，实现了GDC 2025演讲"Capabilities: Coding ALL the Gameplay for 'Split Fiction'"中介绍的Capabilities编程设计模式。

## 🎯 设计理念

### 目录
- [🎯 设计理念](#-设计理念)
- [🏗️ 核心架构](#️-核心架构)
- [🚀 快速开始](#-快速开始)
- [📋 使用流程](#-使用流程)
- [🔧 高级特性](#-高级特性)
- [🛠️ 开发者工具](#️-开发者工具)
- [📁 项目结构](#-项目结构)
- [📄 许可证](#-license--许可证)

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

## 🛠️ 开发者工具

### Capabilities Monitor 📊

强大的Unity编辑器窗口，用于调试和监控Capability系统：

- **实时状态可视化**：用绿色/红色指示器显示激活/未激活的capabilities
- **时间轴跟踪**：显示capability状态随时间变化的可视时间轴
- **帧精确记录**：记录状态变化发生的确切帧数
- **多语言支持**：中英文界面切换
- **性能监控**：识别瓶颈和优化机会

📖 **[查看详细文档](Assets/Scripts/Editor/CapabilitiesMonitor_README.md)**

**访问方式**: Unity菜单栏 → UnityGCC → Capabilities Monitor

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
│       │   ├── GCCSheetEditor.cs      # GCCSheet编辑器
│       │   ├── CapabilitiesMonitorWindow.cs # 调试监控窗口
│       │   └── CapabilitiesMonitor_README.md # 监控器文档
│       ├── CapabilitiesController.cs  # 核心控制器
│       ├── GCCSheet.cs               # 配置资源类
│       ├── GCCSheetLoader.cs         # 加载器
│       └── Instigator.cs             # 来源标识
└── README.md
```

## 📄 License / 许可证

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

本项目采用MIT许可证 - 查看[LICENSE](LICENSE)文件了解详情

## 🙏 Acknowledgments / 致谢

- Thanks to the GDC 2025 talk "Capabilities: Coding ALL the Gameplay for 'Split Fiction'" for design inspiration
- Thanks to the Unity community for support and contributions

- 感谢GDC 2025演讲"Capabilities: Coding ALL the Gameplay for 'Split Fiction'"提供的设计灵感
- 感谢Unity社区的支持和贡献

## 📞 Contact / 联系方式

For questions or suggestions, please contact us through:

如有问题或建议，请通过以下方式联系：

- Submit Issues / 提交Issue
- Start Discussions / 发起讨论
- Create Pull Requests / 创建Pull Request

---

⭐ If this project helps you, please consider giving it a Star! / 如果这个项目对您有帮助，请考虑给它一个Star！ 
