# Sprite Pivot Batch Editor

[中文](#中文) | [English](#english)

---

## 中文

一个用于在 Unity 编辑器中**批量修改图集（Sprite Atlas）内所有精灵（Sprites）轴心（Pivot）** 的工具窗口。无需手动逐个修改，极大提升序列帧动画等工作的处理效率。

### 功能特点

*   **批量处理**: 一键修改图集中所有切片的轴心。
*   **灵活设置**: 提供多种预设轴心位置（中心、底部中心、左上角等）和精确的自定义轴心输入。
*   **实时预览**: 在工具窗口内显示图集当前所有切片的名称和轴心值。
*   **无损操作**: 直接修改 Unity 的精灵导入设置（meta 文件）。

### 安装方法

1.  在您的 Unity 项目 Assets 文件夹下，创建名为 `Editor` 的文件夹（如果尚未存在）。
2.  将 `SpritePivotEditorWindow.cs` 脚本文件放入 `Editor` 文件夹中。
3.  等待 Unity 重新编译脚本。

### 使用方法

1.  在 Unity 编辑器顶部菜单栏中，点击 **Tools > 图集轴心修改工具** 打开工具窗口。
2.  在 Project 窗口中选择一个**已切片（Sprite Mode 为 Multiple）** 的图集。
3.  将图集拖拽至工具窗口的 "图集" 字段，或通过对象选择器选择。
4.  选择轴心设置方式：
    *   **使用自定义轴心**: 勾选后，可在下方输入框直接输入归一化（0-1）的轴心坐标，或使用预设按钮快速选择。
    *   **使用预设对齐**: 不勾选“使用自定义轴心”，然后从右侧下拉菜单选择标准的对齐方式（如 Center, TopLeft 等）。
5.  点击 **应用轴心到所有切片** 按钮。
6.  等待 Unity 重新导入图集，操作即完成。底部列表会更新显示新的轴心值。

### 重要说明

*   **备份建议**: 此操作会直接修改资产的导入设置。**强烈建议在执行批量操作前，对项目或相关图集进行备份**。
*   **图集要求**: 工具仅对 Sprite Mode 设置为 `Multiple` 的纹理有效。
*   **撤销支持**: 此操作**无法**通过 Unity 的 `Edit -> Undo` 撤销，请谨慎操作。

### 兼容性

*   使用现代 `SpriteEditorDataProvider` API 编写。
*   应兼容 Unity 2019.4 LTS 及更高版本。

### 开源许可

本项目基于 **MIT License** 开源。这意味着您可以自由地使用、复制、修改、合并、发布、分发、再许可和/或销售本软件的副本。详见 `LICENSE` 文件。

---

## English

A Unity Editor tool window for **batch editing the pivot points** of all sprites inside a **Sprite Atlas**. Eliminates the need for manual, one-by-one adjustment, significantly improving workflow for sequence frame animations and more.

### Features

*   **Batch Processing**: Modify the pivot of all sprites in an atlas with a single click.
*   **Flexible Settings**: Offers multiple preset pivot positions (Center, Bottom Center, Top Left, etc.) and precise custom pivot input.
*   **Live Preview**: Displays the names and current pivot values of all sprites in the selected atlas within the window.
*   **Non-Destructive**: Works by modifying Unity's sprite import settings (meta files).

### Installation

1.  Inside your Unity project's `Assets` folder, create a folder named `Editor` (if it doesn't already exist).
2.  Place the `SpritePivotEditorWindow.cs` script file inside the `Editor` folder.
3.  Wait for Unity to recompile the scripts.

### Usage

1.  Open the tool window from the top menu: **Tools > Sprite Pivot Batch Editor** (or the translated Chinese menu item).
2.  Select a **sliced (Sprite Mode set to Multiple)** texture atlas in the Project window.
3.  Drag the atlas to the "Atlas" field in the tool window or use the object picker.
4.  Choose how to set the pivot:
    *   **Use Custom Pivot**: Check this box to directly enter normalized (0-1) pivot coordinates in the field below, or use the preset buttons for quick selection.
    *   **Use Preset Alignment**: Uncheck "Use Custom Pivot" and select a standard alignment (e.g., Center, TopLeft) from the dropdown menu on the right.
5.  Click the **Apply Pivot to All Sprites** button.
6.  Wait for Unity to reimport the atlas. The operation is complete when the import finishes. The list at the bottom will update showing the new pivot values.

### Important Notes

*   **Backup Recommended**: This operation directly modifies asset import settings. **It is highly recommended to backup your project or the relevant atlas before performing batch operations.**
*   **Atlas Requirement**: The tool only works on textures with their Sprite Mode set to `Multiple`.
*   **Undo Support**: This operation **cannot** be undone using Unity's `Edit -> Undo`. Please use with caution.

### Compatibility

*   Written using the modern `SpriteEditorDataProvider` API.
*   Should be compatible with Unity 2019.4 LTS and later versions.

### License

This project is open source under the **MIT License**. This means you are free to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the software. See the `LICENSE` file for details.
