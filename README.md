# AR Cooking Guide 프로젝트 실행 가이드

## 📚 목차
1. [프로젝트 개요](#1-프로젝트-개요)
2. [Unity 기본 개념](#2-unity-기본-개념)
3. [Mixed Reality Toolkit (MRTK)](#3-mixed-reality-toolkit-mrtk)
4. [프로젝트 구조](#4-프로젝트-구조)
5. [핵심 컴포넌트](#5-핵심-컴포넌트)
6. [개발 환경 설정](#6-개발-환경-설정)
7. [빌드 및 배포](#7-빌드-및-배포)

---

## 1. 프로젝트 개요

### 1.1 프로젝트 정보
- **프로젝트명**: AR Cooking Guide
- **대상 플랫폼**: Microsoft HoloLens 2
- **개발 환경**: Unity 2022.3.62f2 (LTS)
- **주요 프레임워크**: MRTK (Mixed Reality Toolkit)
- **목적**: Mixed Reality 환경에서 요리 단계별 안내를 제공하는 애플리케이션

### 1.2 기술 스택
```
Unity Engine 2022.3.62f2
├── MRTK 2.8.3 (Foundation, Standard Assets, Tools)
├── OpenXR 1.14.3
├── Universal Render Pipeline (URP) 14.0.12
├── TextMesh Pro 3.0.7
└── AR Foundation 5.2.0
```

---

## 2. Unity 기본 개념

### 2.1 Unity 프로젝트 구조

Unity 프로젝트는 다음 세 폴더로 구성됩니다:

```
ARCookingGuide/
├── Assets/           # 게임 오브젝트, 스크립트, 리소스
├── Packages/         # 의존성 패키지 및 라이브러리
└── ProjectSettings/  # 프로젝트 설정 파일
```

#### **Assets/** - 개발자가 작성하는 모든 콘텐츠
- **스크립트**: `CookingGuideController.cs` - 요리 가이드 로직
- **씬(Scene)**: `SampleScene.unity` - 메인 요리 가이드 씬
- **프리팹(Prefab)**: `Prefab.prefab` - 재사용 가능한 게임 오브젝트
- **리소스**: 폰트(`Fonts/`), 머티리얼(`GlassUI.mat`), 설정 파일
- **데이터베이스**: `RecipeDB/` - 레시피 이미지, `Resources/` - CSV 데이터

#### **Packages/** - Unity 패키지 관리
- `manifest.json`: 설치된 패키지 목록 및 버전 정의
- `packages-lock.json`: 의존성 잠금 파일
- `MixedReality/`: MRTK 패키지 파일 (.tgz)

#### **ProjectSettings/** - Unity 엔진 설정
- `ProjectVersion.txt`: Unity 에디터 버전 (2022.3.62f2)
- `ProjectSettings.asset`: 플랫폼, 빌드 설정
- `GraphicsSettings.asset`: 렌더링 파이프라인 (URP)
- `XRSettings.asset`: XR 플러그인 설정 (OpenXR)

### 2.2 Unity 생명주기 (Lifecycle)

Unity 스크립트는 특정 순서로 함수가 호출됩니다:

```csharp
// CookingGuideController.cs 예시

void Start()    // 게임 시작 시 1회 실행
{
    UpdateUI(); // 첫 번째 요리 단계 표시
}

void Update()   // 매 프레임마다 실행 (초당 60회)
{
    if (isTimerRunning && currentTimer > 0)
    {
        currentTimer -= Time.deltaTime; // 타이머 감소
        UpdateTimerText();
    }
}
```

**주요 생명주기 함수:**
- `Awake()`: 스크립트 인스턴스 로드 시
- `Start()`: 첫 프레임 전에 1회 실행
- `Update()`: 매 프레임마다 실행
- `FixedUpdate()`: 물리 연산용 (고정 시간 간격)
- `OnDestroy()`: 오브젝트 파괴 시

### 2.3 GameObject와 Component

Unity는 **GameObject**에 **Component**를 부착하는 구조입니다.

```
GameObject: "CookingGuidePanel"
├── Transform (위치, 회전, 크기)
├── CookingGuideController (C# 스크립트)
├── TextMeshPro (제목 텍스트)
├── VideoPlayer (요리 영상)
└── Canvas (UI 렌더링)
```

#### 본 프로젝트의 Component 사용 예시:

**CookingGuideController.cs**
```csharp
public class CookingGuideController : MonoBehaviour
{
    // Component 참조
    public TextMeshPro textTitle;      // TextMeshPro Component
    public VideoPlayer videoPlayer;     // VideoPlayer Component
    
    void UpdateUI()
    {
        textTitle.text = "떡 불리기";  // Component 속성 변경
        videoPlayer.Play();             // Component 메서드 호출
    }
}
```

### 2.4 Prefab (프리팹)

**재사용 가능한 GameObject 템플릿**입니다.

- **예시**: `Assets/Prefab.prefab` - AR 요리 카드 UI 템플릿
- **장점**: 동일한 오브젝트를 여러 씬에서 사용 가능
- **수정**: Prefab 원본을 수정하면 모든 인스턴스에 반영됨

```
Prefab.prefab
├── CookingGuideController
├── Title Text (TextMeshPro)
├── Ingredients Text
├── Timer Display
└── Video Panel
```

---

## 3. Mixed Reality Toolkit (MRTK)

### 3.1 MRTK란?

**Microsoft의 오픈소스 Mixed Reality 개발 프레임워크**로, HoloLens/VR 개발을 단순화합니다.

#### 본 프로젝트의 MRTK 버전:
```json
// Packages/manifest.json
{
  "dependencies": {
    "com.microsoft.mixedreality.toolkit.foundation": "2.8.3",
    "com.microsoft.mixedreality.toolkit.standardassets": "2.8.3",
    "com.microsoft.mixedreality.toolkit.tools": "2.8.3"
  }
}
```

### 3.2 MRTK 핵심 기능

#### 1. **Input System** - 손 제스처, 시선, 음성 입력
```csharp
// 예상 사용 시나리오 (본 프로젝트 확장 가능)
public void NextStep()  // 버튼 클릭 또는 음성 명령 "다음"
{
    currentStepIndex++;
    UpdateUI();
}
```

#### 2. **Spatial Awareness** - 공간 인식 및 메쉬 생성
- HoloLens가 주변 환경(벽, 테이블)을 인식
- AR 오브젝트를 실제 공간에 배치

#### 3. **Standard Assets** - UI 컴포넌트
- 버튼, 슬라이더, 다이얼로그 등 MR 전용 UI
- 본 프로젝트: `Assets/MixedRealityToolkit.Generated/CustomProfiles/`

### 3.3 MRTK Configuration Profile

MRTK는 **Configuration Profile**로 설정을 관리합니다.

```
Assets/MixedRealityToolkit.Generated/CustomProfiles/
├── New MixedRealityInputSystemProfile.asset      # 입력 설정
├── New MixedRealitySpeechCommandsProfile.asset   # 음성 명령
└── New MixedRealityToolkitConfigurationProfile.asset  # 메인 프로필
```

**주요 설정 항목:**
- Camera Settings: 카메라 클리어 플래그 (투명, 스카이박스)
- Input System: 핸드 트래킹, 시선 추적
- Spatial Awareness: 공간 메쉬 시각화

---

## 4. 프로젝트 구조

### 4.1 디렉토리 구조 상세

```
ARCookingGuide/
│
├── Assets/
│   ├── CookingGuideController.cs          # 메인 로직 스크립트
│   ├── Prefab.prefab                       # AR 요리 카드 UI
│   ├── GlassUI.mat                         # 반투명 유리 머티리얼
│   │
│   ├── Fonts/                              # 한글 폰트
│   │   ├── malgun.ttf                      # 맑은 고딕
│   │   ├── malgunbd.ttf                    # 맑은 고딕 Bold
│   │   └── KoreanFont_SDF.asset            # TextMeshPro SDF 폰트
│   │
│   ├── Scenes/                             # Unity 씬
│   │   └── SampleScene.unity               # 메인 요리 가이드 씬
│   │
│   ├── RecipeDB/                           # 레시피 데이터베이스
│   │   └── Images/                         # 요리 단계별 이미지
│   │       ├── step11.jpg ~ step16.jpg     # 계란볶음밥 (6단계)
│   │       ├── step21.jpg ~ step28.jpg     # 김치찌개 (6단계)
│   │       └── step31.jpg ~ step37.jpg     # 불고기 (7단계)
│   │
│   ├── Resources/                          # Unity 런타임 로드 리소스
│   │   └── recipeDB.csv                    # 레시피 CSV 데이터
│   │
│   ├── Settings/                           # URP 렌더링 설정
│   │   ├── URP-Balanced.asset              # 균형 품질
│   │   ├── URP-HighFidelity.asset          # 고품질
│   │   └── URP-Performant.asset            # 성능 우선
│   │
│   ├── MRTK/                               # MRTK 셰이더
│   │   └── Shaders/
│   │       ├── MixedRealityStandard.shader # MRTK 표준 셰이더
│   │       └── MixedRealityTextMeshPro.shader
│   │
│   ├── MixedRealityToolkit.Generated/      # MRTK 생성 파일
│   │   ├── CustomProfiles/                 # 커스텀 프로필
│   │   └── link.xml                        # IL2CPP 코드 보존
│   │
│   └── XR/                                 # XR 설정
│       ├── Loaders/                        # XR 플러그인 로더
│       └── Settings/                       # OpenXR 설정
│
├── Packages/
│   ├── manifest.json                       # 패키지 의존성
│   └── MixedReality/                       # MRTK 패키지 파일
│       ├── com.microsoft.mixedreality.openxr-1.11.2.tgz
│       ├── com.microsoft.mixedreality.toolkit.foundation-2.8.3.tgz
│       └── ...
│
└── ProjectSettings/
    ├── ProjectVersion.txt                  # Unity 2022.3.62f2
    ├── ProjectSettings.asset               # 플랫폼, 빌드 설정
    ├── GraphicsSettings.asset              # URP 설정
    └── XRSettings.asset                    # OpenXR 설정
```

### 4.2 파일 타입별 설명

| 확장자 | 설명 | 예시 |
|--------|------|------|
| `.cs` | C# 스크립트 | `CookingGuideController.cs` |
| `.unity` | Unity 씬 파일 | `SampleScene.unity` |
| `.prefab` | 프리팹 (재사용 GameObject) | `Prefab.prefab` |
| `.mat` | 머티리얼 (재질) | `GlassUI.mat` |
| `.asset` | Unity Asset 파일 | `URP-Balanced.asset` |
| `.shader` | 셰이더 코드 | `MixedRealityStandard.shader` |
| `.ttf` | 트루타입 폰트 | `malgun.ttf` |
| `.tgz` | 압축 패키지 | MRTK 패키지 파일 |
| `.csv` | 데이터 파일 | `recipeDB.csv` |
| `.jpg` | 이미지 파일 | `step11.jpg` 등 |

---

## 5. 핵심 컴포넌트

### 5.1 CookingGuideController.cs

**요리 단계 관리 및 UI 업데이트 로직**

#### 주요 기능:

**1. 데이터 구조 - CookingStep**
```csharp
[System.Serializable]  // Unity Inspector에 표시
public struct CookingStep
{
    public string title;            // "떡 불리기"
    [TextArea] 
    public string ingredients;      // "떡 200g, 물 500ml"
    public string heatLevel;        // "중불"
    public float timerSeconds;      // 300 (5분)
    public VideoClip videoClip;     // 요리 영상
}

public List<CookingStep> stepList;  // 전체 단계 목록
```

**2. UI Component 참조**
```csharp
public TextMeshPro textTitle;       // 제목 표시
public TextMeshPro textIngredients; // 재료 표시
public TextMeshPro textTimer;       // 타이머 표시
public VideoPlayer videoPlayer;     // 영상 재생
```

**3. 단계 전환 로직**
```csharp
public void NextStep()  // 다음 단계
{
    if (currentStepIndex < stepList.Count - 1)
    {
        currentStepIndex++;
        UpdateUI();  // UI 갱신
    }
}

public void PrevStep()  // 이전 단계
{
    if (currentStepIndex > 0)
    {
        currentStepIndex--;
        UpdateUI();
    }
}
```

**4. 타이머 시스템**
```csharp
void Update()
{
    if (isTimerRunning && currentTimer > 0)
    {
        currentTimer -= Time.deltaTime;  // 실시간 감소
        UpdateTimerText();
    }
}

void UpdateTimerText()
{
    int minutes = Mathf.FloorToInt(currentTimer / 60F);
    int seconds = Mathf.FloorToInt(currentTimer % 60F);
    textTimer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    // 출력 예: "05:30"
}
```

### 5.2 TextMeshPro (TMP)

**고품질 텍스트 렌더링 시스템**

#### 일반 Unity Text vs TextMeshPro

| 항목 | Unity Text | TextMeshPro |
|------|------------|-------------|
| 품질 | 저해상도에서 흐림 | 모든 해상도에서 선명 |
| 기술 | Bitmap | SDF (Signed Distance Field) |
| 효과 | 제한적 | 그림자, 외곽선, 그라데이션 |
| 성능 | 낮음 | 높음 (GPU 가속) |

#### 본 프로젝트 사용 예시:
```csharp
public TextMeshPro textTitle;

void UpdateUI()
{
    textTitle.text = "떡볶이 만들기";  // 텍스트 설정
    textTitle.color = Color.white;      // 색상 변경
    textTitle.fontSize = 48;            // 크기 변경
}
```

#### 한글 폰트 설정:
```
Assets/Fonts/
├── malgun.ttf              # 원본 폰트
└── KoreanFont_SDF.asset    # TextMeshPro용 변환 폰트
```

**폰트 생성 과정:**
1. `Window > TextMeshPro > Font Asset Creator`
2. `malgun.ttf` 선택
3. Character Set: `Unicode Range` 선택 (한글 포함)
4. Atlas Resolution: `4096x4096` (고품질)
5. Generate Font Atlas → Save

### 5.3 VideoPlayer Component

**Unity 비디오 재생 컴포넌트**

```csharp
public VideoPlayer videoPlayer;

void UpdateUI()
{
    CookingStep currentData = stepList[currentStepIndex];
    
    if (currentData.videoClip != null)
    {
        videoPlayer.clip = currentData.videoClip;  // 영상 설정
        videoPlayer.Play();                         // 재생 시작
    }
}
```

**VideoPlayer 설정:**
- **Source**: Video Clip (에셋으로 관리)
- **Render Mode**: Material Override (3D 공간에 렌더링)
- **Loop**: true (반복 재생)
- **Play On Awake**: false (자동 재생 안 함)

### 5.4 Universal Render Pipeline (URP)

**Unity의 경량 렌더링 파이프라인**

#### 본 프로젝트의 URP 구성:
```
Assets/Settings/
├── URP-Balanced.asset          # 메인 렌더 파이프라인
├── URP-Balanced-Renderer.asset # 렌더러 설정
├── URP-HighFidelity.asset      # 고품질 (PC/콘솔)
└── URP-Performant.asset        # 성능 우선 (모바일)
```

**URP 장점:**
- **경량화**: HoloLens 2 같은 모바일 디바이스에 최적화
- **배터리 효율**: 낮은 전력 소비
- **커스터마이징**: Quality 레벨별 설정 가능

**설정 확인:**
```
Edit > Project Settings > Graphics
└── Scriptable Render Pipeline Settings: URP-Balanced.asset
```

### 5.5 레시피 데이터베이스 구조

**CSV 기반 레시피 관리 시스템**

#### 데이터베이스 파일 위치:
```
Assets/
├── Resources/
│   └── recipeDB.csv        # 레시피 메타데이터 (런타임 로드)
└── RecipeDB/
    └── Images/             # 요리 단계별 이미지
        ├── step11.jpg      # 계란볶음밥 1단계
        ├── step12.jpg      # 계란볶음밥 2단계
        └── ...
```

#### CSV 파일 형식 (`recipeDB.csv`):
```csv
recipe_id,name,category,ingredients,steps,images
1,계란볶음밥,한식,"밥|계란|파|소금|식용유","밥 준비|계란 풀기|파 썰기|볶기|완성","step11.jpg;step12.jpg;step13.jpg;step14.jpg;step15.jpg;step16.jpg"
2,김치찌개,한식,"김치|두부|돼지고기|고춧가루|대파","재료 준비|육수 끓이기|김치 넣기|두부 넣기|끓이기|완성","step21.jpg;step22.jpg;step23.jpg;step24.jpg;step25.jpg;step28.jpg"
3,불고기,한식,"소고기|양파|배|간장|설탕|마늘","재료 준비|양념 만들기|재우기|굽기|완성","step31.jpg;step32.jpg;step33.jpg;step34.jpg;step35.jpg;step36.jpg;step37.jpg"
```

#### 필드 설명:
| 필드 | 설명 | 구분자 |
|------|------|--------|
| `recipe_id` | 레시피 고유 번호 | - |
| `name` | 레시피 이름 (한글) | - |
| `category` | 음식 분류 (한식/중식/양식 등) | - |
| `ingredients` | 필요한 재료 목록 | `|` (파이프) |
| `steps` | 요리 단계 설명 | `|` (파이프) |
| `images` | 단계별 이미지 파일명 | `;` (세미콜론) |

#### 이미지 명명 규칙:
```
step{recipe_id}{step_number}.jpg

예시:
- step11.jpg = 레시피1(계란볶음밥)의 1단계
- step23.jpg = 레시피2(김치찌개)의 3단계
- step37.jpg = 레시피3(불고기)의 7단계
```

#### Unity에서 데이터 로드:
```csharp
// Resources 폴더의 CSV 파일 로드
TextAsset csvFile = Resources.Load<TextAsset>("recipeDB");
string[] lines = csvFile.text.Split('\n');

// 이미지 로드 (RecipeDB/Images에서)
Sprite stepImage = Resources.Load<Sprite>("RecipeDB/Images/step11");
```

---

## 6. 개발 환경 설정

### 6.1 필수 소프트웨어

#### 1. **Unity Hub & Unity Editor**
```
Unity Hub (최신 버전)
└── Unity 2022.3.62f2 (LTS) ← 반드시 이 버전
```

**설치 시 포함 모듈:**
- ✅ Universal Windows Platform Build Support
- ✅ Windows Build Support (IL2CPP)
- ✅ Visual Studio Community 2022

#### 2. **Visual Studio 2022**
```
Visual Studio Community 2022
└── Workloads:
    ├── .NET desktop development
    ├── Universal Windows Platform development
    └── Game development with Unity
```

#### 3. **Git**
- Git Bash 또는 GitHub Desktop
- 버전 관리 및 협업용

### 6.2 프로젝트 열기

#### 단계별 실행:

**1. 저장소 복제**
```bash
git clone https://github.com/How0515/ARCookingGuide.git
cd ARCookingGuide
```

**2. Unity Hub에서 프로젝트 추가**
```
Unity Hub > Projects > Add
└── ARCookingGuide 폴더 선택
```

**3. Unity 버전 확인**
- 프로젝트 목록에서 Unity 버전이 `2022.3.62f2`인지 확인
- 다른 버전이면 설치 또는 변경

**4. 프로젝트 열기**
- 프로젝트 클릭
- Unity가 `Packages/manifest.json` 기반으로 패키지 자동 설치
- 진행 바: `Importing packages...` (3~5분 소요)

**5. 씬 열기**
```
Project 창 > Assets > Scenes > SampleScene.unity 더블클릭
```

**6. Play 버튼 클릭**
- Game 뷰에서 실행 확인
- Console 창에서 에러 확인

### 6.3 패키지 설치 확인

#### Package Manager 확인:
```
Window > Package Manager
```

**설치되어야 할 패키지:**
| 패키지 | 버전 | 상태 |
|--------|------|------|
| Mixed Reality Toolkit Foundation | 2.8.3 | ✅ In Project |
| OpenXR Plugin | 1.14.3 | ✅ In Project |
| TextMeshPro | 3.0.7 | ✅ In Project |
| Universal RP | 14.0.12 | ✅ In Project |

**누락 시 해결:**
```
Window > Package Manager > Packages: In Project
└── 누락된 패키지 우클릭 > Reimport
```

### 6.4 일반적인 오류 해결

#### 오류 1: "The type or namespace name 'TMPro' could not be found"
```
해결: Package Manager > TextMeshPro > Import TMP Essential Resources
```

#### 오류 2: "Assembly 'Assembly-CSharp' will not be loaded"
```
해결: Assets > Reimport All
```

#### 오류 3: MRTK 관련 오류
```
해결:
1. Edit > Project Settings > XR Plug-in Management
2. Universal Windows Platform 탭 선택
3. OpenXR 체크
```

#### 오류 4: Unity 버전 불일치
```
현상: "This project was last opened with..."
해결: Unity Hub에서 정확히 2022.3.62f2 설치 후 재열기
```

---

## 7. 빌드 및 배포

### 7.1 HoloLens 2 빌드 설정

#### 1. **Build Settings 열기**
```
File > Build Settings (Ctrl+Shift+B)
```

#### 2. **플랫폼 전환**
```
Platform: Universal Windows Platform (UWP)
└── Switch Platform 클릭 (처음 1회만)
```

#### 3. **UWP 설정**
```
Target Device: HoloLens
Architecture: ARM64
Build Type: D3D Project
Target SDK Version: Latest installed
Minimum Platform Version: 10.0.10240.0
Visual Studio Version: Latest installed
Build and Run on: USB Device
Build configuration: Release
```

#### 4. **씬 추가**
```
Scenes In Build:
└── [✓] Assets/Scenes/SampleScene.unity
```

### 7.2 Player Settings 설정

```
Edit > Project Settings > Player
```

#### **Publishing Settings**
```
Package name: com.yourcompany.arcookingguide
Publisher: CN=YourName
Signing: Create Test Certificate...
Capabilities:
  [✓] InternetClient
  [✓] Microphone (음성 명령용)
  [✓] SpatialPerception (공간 인식)
  [✓] WebCam (카메라 사용 시)
```

#### **XR Settings**
```
Project Settings > XR Plug-in Management
└── Universal Windows Platform:
    [✓] OpenXR
    [✓] Microsoft HoloLens feature group
```

### 7.3 빌드 실행

#### 1. **Build 폴더 생성**
```
프로젝트 루트에 _builds/ 폴더 생성 (이미 .gitignore에 포함)
```

#### 2. **Build**
```
Build Settings > Build
└── _builds/HoloLens 폴더 선택
    (3~10분 소요)
```

#### 3. **Visual Studio에서 솔루션 열기**
```
_builds/HoloLens/ARCookingGuide.sln 더블클릭
```

#### 4. **Visual Studio 빌드 설정**
```
Configuration: Release
Platform: ARM64
Target: Device (USB 연결) 또는 Remote Machine (Wi-Fi)
```

#### 5. **배포**
```
Debug > Start Without Debugging (Ctrl+F5)
```

### 7.4 HoloLens 2 디바이스 연결

#### USB 연결 방식:
```
1. HoloLens 2를 USB-C로 PC에 연결
2. HoloLens에서 "Trust this computer" 승인
3. Visual Studio에서 Device 선택 후 배포
```

#### Wi-Fi 연결 방식:
```
1. HoloLens 2 IP 주소 확인
   Settings > Network & Internet > Wi-Fi > Properties
   
2. Visual Studio에서 Remote Machine 선택
   
3. Address에 IP 입력 (예: 192.168.1.100)
   Authentication Mode: Universal (Unencrypted Protocol)
```

### 7.5 디버깅

#### Unity Remote 5 (에디터 테스트)
```
1. HoloLens 2에 Unity Remote 5 설치 (Microsoft Store)
2. PC와 같은 Wi-Fi 연결
3. Edit > Project Settings > Editor > Device: Any Android Device
4. Unity Play 버튼 클릭 → HoloLens에서 실시간 테스트
```

#### Windows Device Portal (디바이스 모니터링)
```
1. HoloLens 2에서 Device Portal 활성화
   Settings > Update & Security > For developers > Device Portal
   
2. 브라우저에서 접속
   https://<HoloLens IP>
   
3. 기능:
   - 앱 설치/제거
   - 파일 탐색기
   - 성능 모니터링
   - 스크린샷/비디오 캡처
```

---

## 8. Git 워크플로우

### 8.1 브랜치 전략

```
main (프로덕션)
 ↑ merge
dev (통합)
 ↑ merge
feature/ar (AR 기능)
feature/recipe-db (레시피 DB)
feature/voice-command (음성 명령)
```

### 8.2 개발 프로세스

#### 1. **feature 브랜치 생성**
```bash
git checkout dev
git pull origin dev
git checkout -b feature/my-feature
```

#### 2. **작업 및 커밋**
```bash
git add Assets/MyScript.cs
git commit -m "feat: add cooking timer feature"
```

#### 3. **Push 및 PR 생성**
```bash
git push origin feature/my-feature
# GitHub에서 Pull Request 생성
```

#### 4. **코드 리뷰 및 Merge**
```
PR 승인 후 dev로 merge
→ 충돌 해결 필요 시 로컬에서 merge 후 재푸시
```

### 8.3 .gitignore 주요 항목

```gitignore
# Unity 생성 파일 (버전 관리 제외)
/Library/
/Temp/
/Obj/
/Build/
/Builds/
/Logs/

# IDE 설정 (개인 환경)
.vscode/
.vs/
*.csproj
*.sln

# 빌드 출력
_builds/
*.appx
```

---

## 9. 추가 학습 리소스

### 9.1 Unity 공식 문서
- Unity Manual: https://docs.unity3d.com/Manual/
- Unity Scripting API: https://docs.unity3d.com/ScriptReference/

### 9.2 MRTK 문서
- MRTK 2 Documentation: https://learn.microsoft.com/mixed-reality/mrtk-unity/
- HoloLens 2 개발 가이드: https://learn.microsoft.com/hololens/

### 9.3 본 프로젝트 관련
- TextMeshPro: https://docs.unity3d.com/Packages/com.unity.textmeshpro@3.0/manual/
- URP: https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@14.0/
- OpenXR: https://www.khronos.org/openxr/

---

## 10. FAQ

### Q1: Unity 버전이 정확히 일치해야 하나요?
**A**: 네, `2022.3.62f2`를 사용해야 합니다. 마이너 버전 차이도 패키지 충돌, Library 재생성, 에셋 깨짐 등의 문제를 일으킬 수 있습니다.

### Q2: MRTK 버전은 무엇을 사용하나요?
**A**: 본 프로젝트는 **MRTK 2.8.3**을 사용합니다. MRTK 3.x와는 호환되지 않으므로, 반드시 2.x 버전을 유지해야 합니다.

### Q3: HoloLens 2 없이 개발할 수 있나요?
**A**: 네, Unity 에디터에서 기본 테스트가 가능합니다. 더 정확한 테스트를 위해서는:
- HoloLens 2 Emulator 사용
- Unity Play Mode에서 마우스/키보드로 시뮬레이션
- Unity Remote 5 + 실제 디바이스

### Q4: 빌드 크기를 줄이려면?
**A**: 
- 사용하지 않는 MRTK 패키지 제거
- URP 설정을 Performant로 변경
- 폰트 Atlas 해상도 낮추기 (4096 → 2048)
- 영상 압축 (H.264)

### Q5: 한글이 깨져서 나옵니다.
**A**: 
1. TextMeshPro SDF 폰트가 한글 범위를 포함하는지 확인
2. `Assets/Fonts/KoreanFont_SDF.asset` 사용
3. Font Asset Creator에서 `Unicode Range (Hex)` 선택:
   - `AC00-D7A3` (한글 음절)
   - `1100-11FF` (한글 자모)

---

## 요약 체크리스트

프로젝트 실행 전 확인:
- [ ] Unity 2022.3.62f2 설치
- [ ] Visual Studio 2022 설치 (UWP 모듈 포함)
- [ ] Git 저장소 클론
- [ ] Unity Hub에서 프로젝트 열기
- [ ] Package Manager에서 패키지 설치 확인
- [ ] `Assets/Scenes/SampleScene.unity` 열기
- [ ] Play 버튼으로 실행 테스트

HoloLens 2 배포 전 확인:
- [ ] Build Settings에서 UWP로 플랫폼 전환
- [ ] Architecture: ARM64 설정
- [ ] Publishing Settings에서 Capabilities 설정
- [ ] XR Plug-in Management에서 OpenXR 활성화
- [ ] Test Certificate 생성
- [ ] Visual Studio에서 Release/ARM64 빌드
- [ ] HoloLens 2 연결 (USB 또는 Wi-Fi)
- [ ] 배포 및 테스트

---

**문서 버전**: 1.0  
**최종 업데이트**: 2025년 12월 4일  
**작성자**: AR Cooking Guide Team
