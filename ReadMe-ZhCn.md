# TDXL English Practice (英语学习练习平台)

TDXL English Practice 是一个一站式英语学习与模考练习平台，集成了模拟考试环境、智能语音朗读助手、情感词汇卡片、写作计划课程以及多模型 AI 生成范文库。

🌐 **语言版本 / Language Versions:**
* [English (英文版)](ReadMe.md)
* **简体中文 (Simplified Chinese)** (当前文档)

---

## 1. 项目介绍

**TDXL English Practice** 旨在为用户提供全方位的英语听说读写译系统化训练环境。项目在架构设计上采用“轻量级静态门户 + 数据库驱动的 Next.js 极客应用”双轨制架构：

1. **静态学习门户核心（根目录）**：一系列经过极致优化、支持离线使用的单页 Web 应用，基于 HTML5、CSS3 和 JavaScript 编写。利用浏览器原生 Web APIs（如 Web Speech Synthesis 语音合成、Range 选区等接口）实现了极具科技感的“单词级同步高亮朗读助手”、“发音情感词汇练习卡片”和“5天写作计划”等高频交互学习工具。
2. **高级模拟考场系统 (`Mock-exam-timer/GLM-Agent-timer`)**：一个基于 Next.js 16 (App Router) 的全栈式高级 Web 应用。该模块为英语模拟考试提供了沉浸式、精细化的计时体验：包括 7 个题型阶段进度追踪、自定义考题模板配置、动态 SVG 计时圆环、考试环境白噪音播放器、随堂草稿记事本、中断进度自动缓存恢复，以及基于 SQLite 数据库的本地考试历史记录和图表分析面板。

---

## 2. 技术栈清单

项目采用了先进、高响应性、组件化的技术选型，整体技术栈划分为以下四大核心层级：

### 2.1 前端技术栈 (Front-end)
| 技术组件 | 具体版本 | 项目中的核心作用说明 |
| :--- | :--- | :--- |
| **Next.js** | `^16.1.1` | 现代 React 服务端渲染及路由框架，采用 App Router 机制管理页面布局、API 路由及渲染优化。 |
| **React** | `^19.0.0` | 核心界面库，负责组件的生命周期、状态同步与模块化 UI 构建。 |
| **Tailwind CSS** | `^4.0.0` | 实用优先 CSS 框架（最新的 v4 编译引擎），实现整套暗黑/明亮自适应设计系统。 |
| **Framer Motion** | `^12.23.2` | 提供核心计时圆环动画、页面微交互、阶段切换渐变及粒子效果。 |
| **Zustand** | `^5.0.6` | 极简、高效的客户端状态管理库，集中式管理倒计时状态机、白噪音播放配置及临时会话。 |
| **TanStack React Query** | `^5.82.0` | 处理客户端与 API 接口间的数据同步、缓存管理和历史记录的自动刷新。 |
| **Recharts** | `^2.15.4` | 数据可视化图表库，用于直观展示历次模拟考试耗时分布与完成状态。 |
| **Radix UI Primitives** | 各种版本 | 无障碍 Headless UI 基类组件（对话框 Dialog、下拉 Select、折叠面板 Accordion、选项卡 Tabs、提示 Tooltip 等），构建了整套 UI 组件库。 |
| **MDX Editor & React Markdown**| 各种版本 | 解析与渲染带有富文本 Markdown 格式的模考题目、范文以及记事本。 |
| **Web Speech API** | *浏览器原生* | 静态模块中的核心语音引擎 (`window.speechSynthesis`)，实现离线/在线的短文朗读、划词翻译与单词发音。 |

### 2.2 后端技术栈 (Back-end)
| 技术组件 | 具体版本 | 项目中的核心作用说明 |
| :--- | :--- | :--- |
| **Node.js 运行环境** | `^18.18.0` / `^20.0.0` | 负责 Next.js 服务端运行、依赖构建和后台数据 API 管道的处理。 |
| **Next.js 路由处理程序**| `^16.1.1` | 提供 RESTful JSON 接口（如 `/api/exam-history`，`/api/exam-history/export`），实现考场数据的高效读写。 |
| **Prisma ORM** | `^6.11.1` | 对象关系映射工具，定义关系模型，自动生成 SQLite 迁移脚本，并提供严格类型安全的数据库查询。 |
| **Next-Auth** | `^4.24.11` | 管理用户身份验证与会话保存，预留用于多用户云端数据隔离与管理。 |

### 2.3 基础设施与存储 (Infrastructure & Storage)
| 技术组件 | 具体版本 | 项目中的核心作用说明 |
| :--- | :--- | :--- |
| **SQLite 数据库** | `^3.0.0` (内嵌) | 关系型轻量级数据库，以本地单文件形式（`custom.db`）存储，免去部署独立数据库服务器的繁琐过程。 |
| **本地 Web 存储 API** | *浏览器原生* | 使用客户端 `localStorage` 缓存机制，实现浏览器刷新/断电时考场状态的秒级自动恢复。 |
| **AI 范文对比库** | *数据层* | 集成了主流先进大模型（DeepSeek、Gemini、Qwen、GLM 5.1/Agent、豆包、小米 AI）针对特定英语写作命题生成的比对范文。 |

### 2.4 工具链与开发辅助 (Toolchain)
| 技术组件 | 具体版本 | 项目中的核心作用说明 |
| :--- | :--- | :--- |
| **TypeScript** | `^5.0.0` | 静态类型编译器，提供全栈类型安全保障，避免运行期隐式错误，并加速代码自动补全。 |
| **ESLint** | `^9.0.0` | 代码静态扫描与风格规范检查工具，强制执行统一的代码风格。 |
| **PostCSS** | `^8.0.0` / `@tailwindcss/postcss ^4` | 处理 Tailwind CSS 指令，自动处理浏览器兼容性前缀并进行 CSS 构建优化。 |
| **cross-env** | `^10.1.0` | 跨平台环境变量配置工具，确保在 Windows、macOS 和 Linux 环境中一致的环境参数传递。 |
| **shx** | `^0.4.0` | 跨平台 Shell 脚本工具，使得项目中的目录复制、清理等脚本在 Unix 和 Windows 系统下表现完全一致。 |

---

## 3. 环境依赖要求

为保障项目顺利启动与运行，请确保本地开发机满足以下最低配置要求：

### 3.1 基础软件要求
* **Node.js**：**最低版本兼容 `>= v18.18.0`**（推荐使用 **`v20.11.0 LTS`** 或更高版本）。
* **npm 包管理器**：**最低版本兼容 `>= v9.0.0`**（推荐使用 **`v10.2.0`** 或更高版本）。
* **SQLite 运行环境**：Prisma 会自动下载适配当前系统的原生二进制引擎，无需在操作系统层额外安装 SQLite 可执行文件。

### 3.2 客户端浏览器要求
* **现代浏览器**：Google Chrome (>= 110)、Microsoft Edge (>= 110)、Safari (>= 16)、Firefox (>= 110)。
* **Web Speech API 支持**：必须启用以确保朗读功能发音。*如需获得精准的单词级同步高亮效果，必须在操作系统中安装本地/系统自带的 TTS 英文语音包。*

---

## 4. 本地部署与启动步骤

通过以下命令可以快速部署并运行静态门户或高级 Next.js 模拟考场应用：

### 4.1 运行静态学习门户
静态门户由纯 HTML/CSS/JS 组成，可以直接打开或通过轻量级服务器预览：
* **方法 A（直接打开）**：使用任意浏览器直接双击打开项目根目录下的 [index.html](file:///g:/Projects/TDXL-English-Practice/index.html)。
* **方法 B（本地轻量服务器）**：
  ```bash
  # 使用 npm 的 serve 工具（推荐，发音及高亮事件在 HTTP 环境下更稳定）
  npx serve -p 8080 .
  ```
  然后在浏览器中访问 `http://localhost:8080` 即可。

### 4.2 启动 Next.js 智能模拟考场系统
在终端中执行以下命令，完成数据库初始化与系统构建：

#### 第一步：进入 Next.js 子项目目录
```bash
cd Mock-exam-timer/GLM-Agent-timer
```

#### 第二步：安装项目依赖依赖包
```bash
npm install
```

#### 第三步：配置文件环境变量 `.env`
在 `Mock-exam-timer/GLM-Agent-timer` 目录下新建 `.env` 文件并配置数据库路径：
* **Windows (PowerShell 环境)**：
  ```powershell
  New-Item -Path .env -ItemType File -Value "DATABASE_URL=file:./dev.db" -Force
  ```
* **macOS / Linux (Terminal 环境)**：
  ```bash
  echo "DATABASE_URL=file:./dev.db" > .env
  ```

#### 第四步：初始化本地 SQLite 数据库（Prisma 初始化）
使用 Prisma 将模型 Schema 推送到 SQLite 并编译类型安全客户端：
```bash
# 推送数据库结构并创建 SQLite 本地数据表
npm run db:push

# 编译并生成 Prisma 专属的类型安全客户端代码
npm run db:generate
```

#### 第五步：启动本地开发服务器
```bash
npm run dev
```
打开浏览器并访问 `http://localhost:3000`。

#### 第六步：生产环境打包与本地运行（可选，用于验证发布状态）
```bash
# 编译打包 React 组件、TypeScript 代码与静态优化资源
npm run build

# 启动优化后的 Node.js 生产环境独立服务
npm run start
```
此时项目将运行在高效的 standalone 模式下，侦听 `3000` 端口，通过编译后的 `.next/standalone/server.js` 启动。

---

## 5. 项目结构说明

以下为 TDXL-English-Practice 项目的完整目录结构树状图及关键模块功能说明：

```text
TDXL-English-Practice/
├── Mock-exam-timer/                  # 模拟考试倒计时系统总目录
│   └── GLM-Agent-timer/              # Next.js 考场系统的工程根目录
│       ├── prisma/                   # Prisma ORM 相关目录
│       │   └── schema.prisma         # 数据库 Schema 定义文件，包含 User, Post, ExamHistory 模型
│       ├── public/                   # 存放 Next.js 服务端公共静态资源（图标、音频等）
│       ├── src/                      # 应用程序源代码目录
│       │   ├── app/                  # Next.js App Router 页面和 API 接口
│       │   │   ├── api/              # RESTful API 接口
│       │   │   │   ├── exam-history/ # 处理历次考试历史数据 CURD 的路由处理程序
│       │   │   │   └── route.ts      # 顶级 API 统一分发
│       │   │   ├── globals.css       # 全局样式，包含 Tailwind 核心指令与 HSL 主题变量
│       │   │   ├── layout.tsx        # 顶级 HTML Document 布局与多语言上下文包装器
│       │   │   └── page.tsx          # 模考系统的主面板控制大屏（包含倒计时核心组件）
│       │   ├── components/           # UI 及领域 React 组件
│       │   │   ├── timer/            # 模考专属交互组件（白噪音、圆环计时、设置、历史图表等）
│       │   │   └── ui/               # 基础原子级 UI 组件（弹窗、按钮、选择器、进度条等）
│       │   ├── hooks/                # 自定义 React hooks
│       │   └── lib/                  # 辅助工具类库与常量配置
│       │       ├── db.ts             # PrismaClient 实例化单例配置文件
│       │       ├── exam-data.ts      # 英语考试 7 大阶段时长、分值、题型配置常量
│       │       └── i18n.ts           # 多语言（中文/英文）本地化翻译字典
│       ├── .env                      # 本地环境变量配置（指定本地 SQLite 数据库路径）
│       ├── eslint.config.mjs         # 扁平化 ESLint 语法校验配置文件
│       ├── package.json              # 依赖包声明、编译脚本及项目元数据
│       └── tsconfig.json             # TypeScript 编译器指令配置文件
├── spec/                             # 规范文档目录
│   └── exam-mock-timer.md            # 智能考试倒计时系统核心需求与设计文档 (中文)
├── writing/                          # AI 写作范文库 (静态资源)
│   ├── artifical-intelligence-...   # "AI 就业机遇"比对范文 (DeepSeek、GLM、Qwen、豆包、小米)
│   ├── The Lying Flat Phenomenon-... # "躺平现象"比对范文 (DeepSeek、Gemini、小米)
│   └── The-artifical-intelligence-...# "AI 应用"比对范文 (小米)
├── index.html                        # 英语学习练习平台主导航页面 (现代磨砂玻璃质感 UI)
├── WritingReader.html                # 带有同步单词高亮的短文语音智能朗读助手 (静态单页)
├── english_writing_plan.html         # 5天写作计划系统化教学训练大纲 (静态单页)
├── exam-mock-timerV2.html            # 简易版考试倒计时工具 (纯静态，作离线备份使用)
└── 情感词汇-code-practice.html          # 情感词汇互动学习卡片 (静态单页，带 TTS 朗读发音)
```

---

## 6. 开发规范

为了保证项目的代码质量与后期维护性，所有开发人员需严格遵守以下约定：

### 6.1 代码风格与校验
* **TypeScript 强类型**：必须声明明确的类型或接口定义，禁止在非必要情况下滥用 `any` 隐式类型。
* **组件拆分原则**：页面逻辑应尽可能抽离。庞大复杂的组件需按功能划分子模块并存入 `components/timer/` 中，以保证主页 `page.tsx` 的简洁易读。
* **代码校验**：提交或打包前，必须执行 `npm run lint`。任何 ESLint 警告或错误均应优先修复，以防止持续集成构建失败。

### 6.2 样式与响应式布局规范
* **Tailwind v4 标准**：除特制动画或图表样式外，其余布局均应使用 Tailwind 实用工具类构建，避免书写重复的内联样式。
* **响应式自适应**：坚持“移动端优先”的流式布局设计原则。所有卡片、计时圆环、弹窗均需适配多种移动端屏幕宽度，充分利用 `sm:`、`md:` 和 `lg:` 断点来自动调整内边距及字体大小。

### 6.3 状态管理与 API 规范
* **普通 UI 状态**：使用 React 的 `useState` 或 `useReducer` 进行局部状态控制。
* **核心业务数据**：模考运行状态机、设置参数及白噪音播放状态应集中托管在 Zustand 状态树中（位于 `components/timer`）。
* **异步数据读写**：数据库的 CRUD 读写必须通过 TanStack React Query (`useQuery` / `useMutation`) 进行，确保客户端图表、列表的数据同步与更新效率。

---

## 7. 常见问题排查 (FAQ)

### Q1: 在执行项目构建或打包时，提示 `shx` 拷贝命令执行错误。
* **原因**：当打包路径被其他开发工具锁定，或者权限不足时，`shx` 文件复制命令可能会报错。
* **解决方法**：请关闭后台运行的 Next.js 服务进程，并确认 `shx` 已作为本地依赖正确安装。可尝试执行 `npm install` 重新建立跨平台二进制软链接。

### Q2: 语音朗读助手 (`WritingReader.html`) 中“单词高亮同步”出现滞后或错位。
* **原因**：部分浏览器（如 Google Chrome）的云端 TTS 引擎发音在分发流式语音块时，其 `onboundary` 事件触发的 `charIndex` 不够精准或存在偏移。
* **解决方法**：请在语音下拉菜单中，优先选择带有 **(本地 - 支持高亮)** 标识的系统原生发音（例如 Microsoft OS 本地英文语音包）。本地引擎能完美反馈每一个字符级发音边界。

### Q3: 运行开发服务器时，Prisma 报错 `Prisma Client has not yet been generated` 或 SQLite 数据库锁死。
* **原因**：尚未编译 Prisma 专属依赖包，或本地 SQLite 数据库文件正被另一个外部 IDE 数据库工具独占锁定。
* **解决方法**：
  1. 确认已关闭所有连接该 SQLite 的第三方数据库查看工具；
  2. 在工程根目录下执行 `npm run db:generate` 以重新编译生成类型定义；
  3. 执行 `npm run db:push` 以强制同步本地模型结构。

### Q4: 提示 3000 端口被本地其他开发工程占用。
* **原因**：Next.js 开发服务器默认侦听 `3000` 端口。
* **解决方法**：可在启动时通过 `-p` 命令行参数指定其他可用端口，例如：
  ```bash
  npx next dev -p 3005
  ```

---

## 8. 开源协议

本项目采用 MIT 开源许可协议。详情请参阅 [LICENSE](file:///g:/Projects/TDXL-English-Practice/LICENSE) 文件。
