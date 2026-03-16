# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## 언어 규칙

모든 답변과 코드 주석은 반드시 한국어로 작성할 것.

## Project Overview

Mobile idle RPG built with Unity 2022.3.62f3 (C#), targeting Android and iOS. Uses UniTask for async operations, Firebase for cloud save/auth, and Google Play Games Services.

## Build & Development

- **Engine:** Unity 2022.3.62f3 — open `IdleGame/` as the Unity project
- **Build:** Unity Editor → File > Build Settings → Android or iOS
- **Play testing:** Unity Editor Play Mode
- **Editor shortcuts (in-game only):**
  - `F` — TestManualSave
  - `R` — TestManualLoad

There are no CLI build scripts or test runners. All testing is done in the Unity Editor.

## Architecture

### Manager Singleton Hub

`Assets/3. Scripts/Manager/Managers.cs` is the central entry point. All managers are accessed as static properties:

```csharp
Managers.GameM    // GameManager — currency, progression, game state
Managers.UIM      // UIManager — UI lifecycle
Managers.PoolM    // PoolManager — object pooling
Managers.ResourceM // ResourceManager — asset loading/caching
Managers.DataM    // DataManager — loads Datas.json
```

### Custom Update Loop

`UpdateManager.cs` replaces Unity's `Update()`. Anything that needs per-frame updates implements `ITickable` (scaled) or `IUnScaledTickable` (unscaled) and registers with `UpdateManager`.

### Entity Hierarchy

```
BaseController
└── CreatureController (HP, damage, buffs, state machine)
    ├── PlayerController
    └── MonsterController
```

Creatures use an enum state machine: `Idle → Move → Attack → Hit → Dead → Skill`.

### UI System

- `UI_Base.cs` — base class with enum-driven reflection binding:
  ```csharp
  Bind<Button>(typeof(Buttons));   // auto-finds children matching enum names
  Get<Button>(Buttons.Play);
  ```
- `UI_Popup.cs` — modal popup base
- `UI_Scene.cs` — scene-level UI base
- Popup classes follow `UI_*Popup.cs` naming

### Data Layer

- All game data loaded from a single `Datas.json` via `DataManager.Init()`
- Uses generic `ILoader<K,V>` for type-safe deserialization (Newtonsoft.Json)
- Player state lives in `GameData.cs` — serialized to JSON and synced to Firebase Realtime Database
- `LevelDesign.cs` (ScriptableObject) holds stat scaling formulas for players and monsters

**Data ID ranges:**
| Range | Type |
|-------|------|
| 1–10 | Player |
| 100+ | Buff types |
| 10000+ | Monsters |
| 20000+ | Projectiles |
| 30000+ | Items |
| 40000+ | Skills |
| 50000+ | Buffs |
| 60000+ | VFX |
| 70000+ | Dungeons |
| 80000+ | Missions |
| 90000+ | Achievements/Quests |

### Firebase Integration

`Assets/3. Scripts/Manager/Firebase/`
- `FirebaseManager.cs` — initialization
- `FirebaseLogin.cs` — email, Google Sign-In, Apple Sign-In
- `FirebaseDB.cs` — cloud save/load with conflict resolution

### Scenes

1. `TitleScene` — login and initial load
2. `GameScene` — main gameplay

### Key Packages

- **UniTask** (Cysharp) — all async code uses `UniTask` / `async UniTask<T>`
- **Addressables 1.22.3** — runtime asset loading
- **Unity Localization 1.5.9** — multi-language (Korean default, Google Sheets source)
- **Unity Purchasing 5.0.4** — IAP
- **Google Mobile Ads** — AdMob
- **Google Play Games SDK** — achievements, leaderboards

## 학습 원칙

- 코드를 바로 짜주지 말고, 접근 방법과 힌트를 먼저 제시할 것
- 사용자가 직접 시도한 코드가 있으면 그걸 기반으로 개선 방향만 알려줄 것
- 왜 이렇게 구현하는지 이유를 항상 설명할 것
- 모르는 개념이 나오면 관련 Unity 공식 문서 링크 함께 제공할 것

## 코드 요청 시 대응 방식

- 1단계: 문제 접근 방법 설명
- 2단계: 사용자가 직접 구현 시도
- 3단계: 요청 시에만 전체 코드 제공
- 단, 반복 작업/자동화/데이터 변환은 바로 코드 제공 가능

## 코드 리뷰 방식

- 잘된 부분 먼저 언급
- 문제점은 왜 문제인지 이유 설명
- 개선 방법은 힌트로 먼저 제시, 직접 수정은 요청 시에만

## Git 컨벤션

- 커밋 메시지는 한국어로 작성
- 형식: `[타입] 내용` (예: `[feat] 골드 자동수집 기능 추가`)
- 타입: `feat` / `fix` / `refactor` / `chore` / `docs`

## Firebase 주의사항

- Firebase 호출은 반드시 try-catch 처리
- 클라이언트에서 직접 DB 쓰기 최소화, 보안 규칙 고려
- 오프라인 대응 로직 항상 포함할 것
- API 키, 인증 정보 절대 코드에 하드코딩 금지

## 코드 리뷰 스타일

- 변경 사항 요약을 먼저 설명
- 성능 영향도 반드시 언급
- 모바일 메모리/배터리 영향 고려해서 리뷰

## 테스트 코드

현재 프로젝트(KnightOrderGrow)는 테스트 코드를 사용하지 않음.
이유: Managers 싱글톤 의존성이 깊어 자동화 비용 대비 실익이 없음. 손 테스트 + Firebase Crashlytics로 대체.

**다음 프로젝트 시작 시 반드시 세팅할 것:**
- `Assets/Tests/EditMode/` — EditMode 단위 테스트 (순수 로직)
- `Assets/Tests/PlayMode/` — PlayMode 통합 테스트 (런타임 필요 로직)
- 각 폴더에 `.asmdef` 생성 (UnityEngine.TestRunner 참조)
- `run_tests.sh` — Unity CLI 기반 자동 실행 + 한국어 리포트
- 테스트 대상: 핵심 수식(데미지, 버프 배율, 날짜 계산), 재화 증감 로직
- 테스트 메서드명은 한국어로 작성 가능
- 싱글톤 의존성 최소화 설계로 테스트 가능한 구조 유지
