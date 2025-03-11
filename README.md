<div align="center">

# Browsers Manager 多账户管理套件

[![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.8-512BD4.svg)](https://dotnet.microsoft.com/download/dotnet-framework)
[![Windows](https://img.shields.io/badge/Windows-10%2B-0078D6.svg?logo=windows)](https://www.microsoft.com/windows)
[![License](https://img.shields.io/badge/License-GPL%20v3-blue.svg)](LICENSE)
[![NuGet](https://img.shields.io/badge/NuGet-Newtonsoft.Json_13.0.3-green.svg)](https://www.nuget.org/packages/newtonsoft.json/13.0.3)

  <strong>作者：crypto2uk</strong>：<span title="撸秃区块链"></span>
  [![X](https://img.shields.io/badge/X-1DA1F2.svg?style=flat&logo=x&logoColor=white)](https://x.com/crypto2ukX)

</div>

> [!IMPORTANT]
> ## ⚠️ 免责声明
> 
> 1. **本软件为开源项目，仅供学习交流使用，不得用于任何闭源商业用途**
> 2. **使用者应遵守当地法律法规，禁止用于任何非法用途**
> 3. **开发者不对因使用本软件导致的直接/间接损失承担任何责任**
> 4. **使用本软件即表示您已阅读并同意本免责声明**

> [!WARNING]
> ## ⚠️ 安全声明
> 
> 1. 本项目采用最小化依赖原则，唯一第三方组件为[Newtonsoft.Json](https://www.newtonsoft.com/json)
> 2. 建议用户自行从[官方仓库]()获取源码编译

## 核心功能

- 🚀 **配置管理**
  - 一键创建隔离的浏览器配置环境
  - 一键生成账号数字头像

- 🧩 **窗口控制**
  - 批量启动/关闭浏览器实例（支持账号范围选择）
  - 智能窗口布局系统（网格/自定义排列）
  - 基于任务的窗口分组管理

- 📚 **工作流增强**
  - 任务导向的书签管理系统
  - 批量任务同步执行（一键启动同任务组）
  - 操作审计日志

## 系统要求

| 组件 | 最低要求 |
|------|----------|
| 操作系统 | Windows 10/11 64-bit (21H2+) |
| 运行时 | .NET Framework 4.8 |
| 内存 | 4GB 可用内存 |
| 存储 | SSD 建议，100MB 可用空间 |

## 编译指南

### 环境准备
1. 安装 [Visual Studio Build Tools](https://visualstudio.microsoft.com/visual-cpp-build-tools/)
2. 安装 [.NET Framework 4.8 Developer Pack](https://dotnet.microsoft.com/download/dotnet-framework/net48)

### 编译步骤
```powershell
# 还原NuGet包
nuget restore BrowsersManager.sln

# 发布构建
msbuild /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=.\dist
```

### 依赖管理
```xml
<!-- 项目文件中的NuGet引用 -->
<PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
```


## 更新日志

### v1.0
- 首次发布


## 许可证

本项目采用 GPL-3.0 License，保留所有权利。使用本代码需明确标注来源，禁止闭源商业使用。
