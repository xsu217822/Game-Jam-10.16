# Credits 面板设置指南

## 问题分析

Credits 面板不在单独的场景中，而是在 MainMenu 场景中作为一个隐藏的 GameObject，初始时处于禁用状态。

## Hierarchy 结构（应该的样子）

```
Canvas
├── MainMenuUI
│   ├── PlayButton
│   ├── SettingsButton
│   └── ExitButton
├── SettingsPanel
│   ├── ResolutionDropdown
│   ├── FullscreenToggle
│   ├── VolumeSlider
│   └── BackButton
└── CreditsPanel  ← 这是关键对象
    ├── CreditsText
    ├── ExitButton  ← 或者名称可能不同
    └── ... (其他 UI 元素)
```

## 设置步骤

### 步骤 1: 确认 CreditsPanel 的位置
1. 在 Hierarchy 中找到 CreditsPanel
2. 确保它在 Canvas 的直接子对象下
3. 记下它的准确路径（可能是 Canvas/CreditsPanel 或其他）

### 步骤 2: 确认 ExitButton 的位置
1. CreditsPanel 内应该有一个退出按钮
2. 按钮的名称应该包含 "Exit"（不区分大小写）
3. 例如：ExitButton、ExitCreditsButton、BackButton 等

### 步骤 3: 确保 CreditsUIManager 脚本配置正确
1. 在 Hierarchy 中找到 OptionManager（或有 CreditsUIManager 脚本的对象）
2. 在 Inspector 中查看 CreditsUIManager 脚本
3. **方案 A**（推荐）: 让脚本自动查找
   - 保持 Credits Panel 和 Exit Button 字段为空（或未赋值）
   - 脚本会在 Start() 时自动查找
4. **方案 B**（备选）: 手动赋值
   - 将 CreditsPanel GameObject 拖放到 "Credits Panel" 字段
   - 将 ExitButton Button 组件拖放到 "Exit Button" 字段

## 脚本的自动查找机制

CreditsUIManager 现在使用**多层次查找策略**：

### 查找优先级

1. **GameObject.Find()** - 按路径查找
   ```csharp
   GameObject.Find("Canvas/CreditsPanel")
   GameObject.Find("CreditsPanel")
   ```

2. **GetComponentsInChildren(true)** - 从 Canvas 查找所有子对象（包括禁用的）
   ```csharp
   // 查找名为 "CreditsPanel" 的对象
   // 查找名称包含 "Exit" 的 Button
   ```

3. **全局搜索** - 最后手段
   ```csharp
   // 按名称或类型进行全局搜索
   ```

## 常见问题排查

### Q1: CreditsPanel 找不到
**症状**: Console 显示 "Panel:✗"

**排查步骤**:
1. 在 Hierarchy 中是否存在 CreditsPanel？
2. 是否被标记为 "(Inactive)"？（这没关系，脚本可以找到禁用的对象）
3. 它的确切名称是什么？（必须包含 "CreditsPanel" 或通过 Find() 找到）
4. 它是否在 Canvas 下？

**解决方案**:
- 如果名称不同，将对象重命名为包含 "CreditsPanel" 的名称
- 或者在 Inspector 中手动拖放 GameObject 到 Credits Panel 字段

---

### Q2: ExitButton 找不到
**症状**: Console 显示 "Button:✗"

**排查步骤**:
1. CreditsPanel 内是否有 Button 组件？
2. 按钮的名称是什么？
3. 按钮是否被标记为 "(Inactive)"？
4. Button 脚本是否正确挂载？

**解决方案**:
- 确保按钮名称包含 "Exit"（不区分大小写）
- 或者在 Inspector 中手动拖放 Button 到 Exit Button 字段

---

### Q3: Credits 面板显示后立即隐藏
**症状**: 点击 Credits 按钮，面板闪现后消失

**排查步骤**:
1. 是否调用了 HideCreditsPanel()？
2. OnEnable() 是否在不应该的时候被调用？
3. CreditsUIManager 脚本在哪个对象上？

**解决方案**:
- 如果 CreditsUIManager 在 CreditsPanel 上，会导致问题
- 应该在其他对象上（如 OptionManager 或 UIManager）

---

### Q4: Console 中没有看到任何日志
**症状**: 点击按钮，Console 没有反应

**排查步骤**:
1. CreditsUIManager 脚本是否正确挂载？
2. 脚本是否在启用状态？
3. 是否有编译错误？
4. Console 是否打开？

**解决方案**:
- 打开 Window → General → Console
- 检查是否有 ERROR 日志（红色）
- 确认脚本在正确的对象上

## 检查清单

### 初始设置
- [ ] CreditsPanel 在 Canvas 的子对象下
- [ ] CreditsPanel 包含一个 Button 组件或子对象
- [ ] Button 的名称包含 "Exit" 或 "exit"
- [ ] CreditsUIManager 脚本挂在 OptionManager（或类似的管理对象）

### 脚本配置
- [ ] CreditsUIManager 脚本的引用为空（让它自动查找）
  或
- [ ] CreditsUIManager 脚本的引用已手动赋值

### 测试
- [ ] 启动游戏
- [ ] 进入 Settings 菜单
- [ ] 检查 Console 中是否显示：
  ```
  [CreditsUIManager] Start() - 初始化 UI 组件
  [CreditsUIManager] ✓ UI 查找完成 - Panel:✓, Button:✓
  [CreditsUIManager] ✓ 退出按钮已连接
  [CreditsUIManager] ✓ Credits 面板已隐藏
  [CreditsUIManager] ✓ 初始化完成
  ```

## 调试技巧

### 打印 Hierarchy 结构
在 Console 中输入以下代码来打印 Canvas 下的所有对象：

```csharp
Canvas canvas = FindObjectOfType<Canvas>();
foreach (Transform child in canvas.GetComponentsInChildren<Transform>(true))
{
    Debug.Log(child.name);
}
```

### 验证对象查找
在 Start() 或 OnEnable() 中手动验证：

```csharp
GameObject panel = GameObject.Find("Canvas/CreditsPanel");
Debug.Log("Panel found: " + (panel != null));

Button[] allButtons = FindObjectOfType<Canvas>().GetComponentsInChildren<Button>(true);
foreach (Button btn in allButtons)
{
    Debug.Log("Button: " + btn.name);
}
```

## 性能考虑

- **GetComponentsInChildren(true)** 包含禁用的对象，会多扫描一次
- **GameObject.Find()** 通过字符串查找，性能较低
- 一旦找到对象，脚本会保存引用，不会重复查找

## 总结

新的 CreditsUIManager 具有以下特性：
1. ✓ 自动查找 CreditsPanel 和 ExitButton
2. ✓ 支持隐藏/禁用的对象
3. ✓ 详细的调试日志
4. ✓ 多层次查找策略
5. ✓ 支持灵活的命名约定

**关键**: Credits 面板必须始终存在于 MainMenu 场景中（即使禁用），名称应包含 "CreditsPanel" 或 ExitButton 名称应包含 "Exit"。

