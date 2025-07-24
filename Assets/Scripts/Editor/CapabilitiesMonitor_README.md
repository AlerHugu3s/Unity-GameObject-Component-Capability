# Capabilities Monitor 📊 / 能力监控器

*[English](#english) | [中文](#chinese)*

---

## English

### Overview
**Capabilities Monitor** is a powerful Unity Editor window designed for debugging and monitoring the UnityGCC Capability system. It provides real-time visualization of capability states, timeline tracking, and comprehensive debugging tools.

### Features

#### 🎯 **Real-time State Monitoring**
- **Visual Status Indicators**: Green (●) for active, Red (●) for inactive capabilities
- **Auto-refresh**: Configurable refresh interval (0.05-1 seconds)
- **Manual Refresh**: Instant update with a single click
- **Hierarchical Display**: Organized by Owner → TickGroup → Capability

#### 📈 **Timeline Visualization**
- **Historical Tracking**: Visual timeline showing capability state changes over time
- **Frame-accurate**: Records exact frame numbers when state changes occur
- **Time Range Control**: Adjustable time window (5-60 seconds)
- **Event Tooltips**: Hover to see detailed event information (frame, time, state)
- **Color-coded Events**: Green dots for activation, red dots for deactivation

#### 📊 **Information Display**
- **Owner Information**: GameObject names and references
- **TickGroup Categorization**: Clear grouping by execution order
- **Duration Tracking**: Shows how long capabilities have been active/inactive
- **Event Statistics**: Current frame count and total recorded events

#### 🔧 **Control Options**
- **Timeline Toggle**: Show/hide timeline view
- **History Cleanup**: Clear all recorded events
- **Time Range Adjustment**: Slide to change monitoring period
- **Safe Initialization**: Handles startup scenarios gracefully

### Usage Instructions

#### 1. **Opening the Window**
```
Unity Menu Bar → UnityGCC → Capabilities Monitor
```

#### 2. **Basic Operation**
1. Enter **Play Mode** in Unity
2. Wait for the CapabilitiesController to initialize
3. Observe real-time capability states in the tree view
4. Enable Timeline to see historical state changes

#### 3. **Timeline Features**
- **Enable Timeline**: Check "显示Timeline" to activate timeline view
- **Adjust Time Range**: Use the slider to set monitoring period (5-60 seconds)
- **View Event Details**: Hover over timeline dots to see frame and time information
- **Clear History**: Click "清空历史" to reset all recorded events

#### 4. **Troubleshooting**
- **"CapabilitiesController正在初始化中"**: Wait for the system to fully start
- **"等待capability状态变化"**: Normal state when no events have been recorded yet
- **Empty Display**: Ensure your GameObjects have capabilities properly registered

### Technical Details

#### Requirements
- Unity Editor (2019.4+)
- UnityGCC Capability System
- Play Mode required for functionality

#### Architecture
- **State Tracking**: Monitors capability.bActive property changes
- **Event Recording**: Stores CapabilityStateEvent objects with timestamp and frame data
- **Memory Management**: Automatic cleanup of old events based on time window
- **Error Handling**: Comprehensive exception handling for startup scenarios

#### Performance
- **Minimal Overhead**: Only active during editor debugging
- **Efficient Updates**: Smart refresh system only updates when needed
- **Memory Optimized**: Automatic cleanup prevents memory leaks

### Best Practices

1. **Development Workflow**
   - Keep the window open during development for continuous monitoring
   - Use timeline to identify performance bottlenecks
   - Adjust refresh rate based on your needs (faster for debugging, slower for general monitoring)

2. **Debugging Tips**
   - Look for unexpected activation patterns in the timeline
   - Check if capabilities are activating too frequently
   - Verify proper Owner assignment for all capabilities

3. **Performance Optimization**
   - Reduce refresh interval for better performance
   - Clear history periodically during long debugging sessions
   - Close window when not needed to reduce editor overhead

### Known Limitations
- Requires Play Mode to function
- Timeline data is not persistent across play sessions
- Large numbers of capabilities may impact editor performance

---

## Chinese

### 概述
**Capabilities Monitor（能力监控器）** 是一个强大的Unity编辑器窗口，专为调试和监控UnityGCC能力系统而设计。它提供实时的能力状态可视化、时间轴跟踪和全面的调试工具。

### 功能特点

#### 🎯 **实时状态监控**
- **可视状态指示器**：绿色(●)表示激活，红色(●)表示未激活
- **自动刷新**：可配置刷新间隔（0.05-1秒）
- **手动刷新**：单击即可立即更新
- **分层显示**：按Owner → TickGroup → Capability的层次结构组织

#### 📈 **时间轴可视化**
- **历史追踪**：显示能力状态随时间变化的可视时间轴
- **帧精确度**：记录状态变化发生的确切帧数
- **时间范围控制**：可调节时间窗口（5-60秒）
- **事件提示**：悬停查看详细事件信息（帧数、时间、状态）
- **颜色编码事件**：绿点表示激活，红点表示停用

#### 📊 **信息显示**
- **Owner信息**：GameObject名称和引用
- **TickGroup分类**：按执行顺序清晰分组
- **持续时间跟踪**：显示能力激活/未激活的持续时间
- **事件统计**：当前帧数和记录的事件总数

#### 🔧 **控制选项**
- **时间轴开关**：显示/隐藏时间轴视图
- **历史清理**：清除所有记录的事件
- **时间范围调整**：滑动更改监控周期
- **安全初始化**：优雅处理启动场景

### 使用说明

#### 1. **打开窗口**
```
Unity菜单栏 → UnityGCC → Capabilities Monitor
```

#### 2. **基本操作**
1. 在Unity中进入**Play模式**
2. 等待CapabilitiesController初始化
3. 在树形视图中观察实时能力状态
4. 启用Timeline查看历史状态变化

#### 3. **时间轴功能**
- **启用时间轴**：勾选"显示Timeline"激活时间轴视图
- **调整时间范围**：使用滑动条设置监控周期（5-60秒）
- **查看事件详情**：悬停在时间轴点上查看帧和时间信息
- **清空历史**：点击"清空历史"重置所有记录的事件

#### 4. **故障排除**
- **"CapabilitiesController正在初始化中"**：等待系统完全启动
- **"等待capability状态变化"**：尚未记录任何事件时的正常状态
- **显示为空**：确保您的GameObject已正确注册了capabilities

### 技术详情

#### 系统要求
- Unity编辑器（2019.4+）
- UnityGCC能力系统
- 需要Play模式才能运行

#### 架构
- **状态跟踪**：监控capability.bActive属性变化
- **事件记录**：存储带有时间戳和帧数据的CapabilityStateEvent对象
- **内存管理**：基于时间窗口的旧事件自动清理
- **错误处理**：针对启动场景的全面异常处理

#### 性能
- **最小开销**：仅在编辑器调试期间活跃
- **高效更新**：智能刷新系统仅在需要时更新
- **内存优化**：自动清理防止内存泄漏

### 最佳实践

1. **开发工作流程**
   - 在开发期间保持窗口打开以进行持续监控
   - 使用时间轴识别性能瓶颈
   - 根据需要调整刷新率（调试时更快，一般监控时更慢）

2. **调试技巧**
   - 在时间轴中查找意外的激活模式
   - 检查能力是否激活过于频繁
   - 验证所有能力的正确Owner分配

3. **性能优化**
   - 减少刷新间隔以获得更好的性能
   - 在长时间调试会话期间定期清空历史
   - 不需要时关闭窗口以减少编辑器开销

### 已知限制
- 需要Play模式才能运行
- 时间轴数据在播放会话间不持久
- 大量能力可能影响编辑器性能

---

## Version History / 版本历史

### v1.0.0
- Initial release with basic monitoring functionality
- Real-time capability state display
- Owner and TickGroup categorization

### v1.1.0
- Added timeline visualization
- Event tracking and logging
- Mouse hover tooltips
- Configurable time range

### v1.2.0
- Enhanced error handling for startup scenarios
- Improved performance and memory management
- Better UI feedback and status messages
- Safe initialization checks

---

## Support / 支持

For issues, feature requests, or questions about the Capabilities Monitor, please refer to the UnityGCC project documentation or create an issue in the project repository.

如需技术支持、功能请求或关于能力监控器的问题，请参考UnityGCC项目文档或在项目仓库中创建issue。 