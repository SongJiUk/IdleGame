using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Memo
{

    /*

    스킬 사용시 무적되게 만들지 말지 => 무적 되게 하는게 맞는듯 => 사용시 무적으로 만듬
    던전 결과 팝업만들기 => 완료
    던전 시간체크해서 탈락 확인 => while문 끝나면 넣게 만듬
    몬스터가 왜 계속 생성되는가? => Boss로 바꿔줘야하는데 Play로 바꿔서그랬던거
    클리어하면 보상 어떻게 받을지 => 다이아 = 레벨 * 50 , 골드 = Money에 (레벨 *5) * 10 값 => 이렇게 했음 일단
    던전 입장재료 입장시 or 클리어시 차감시키기. => 완료
    던전 입장재료 없을시 Toast띄우기, 버튼에 text 빨간색으로 만들기 => 완료 
    던전팝업에 남은 시간 체크하기 => 완료
    난이도 선택버튼 활성화(Toast) => 완료
    UI_HeroStatPopup 완성하기
    제련 마무리하기 => 1~5개 나올 확률 지정해놓고, 50, 25, 15, 9, 1 해놓고 가져오게 만들기(글자 색으로 grade판별)
    버프 활성화시 적용 안되는거같음 => 이건 해결
    버프 사용후, 시간 저장은 완료=> 근데 이거 껏다가 팝업을 켜줘야하는거라 UI_GameScene으로 옮겨줘야할거같은데
    팝업에서 사용하던걸 클릭을 누르면 UI_GameScene에서 관리하는 식으로 해야될거같음 그래야 게임이 시작되자마자 활성화될듯 
    (버프 매니저를 만들지, GameScene에서 사용할지 고민해보자) => 버프 매니저 만들어놨으 이거 어떻게 사용할지 설정하기
    => 연동 완료
    (테스트만 하면됌) : 레벨 올려서 확인해보기 (UI도 아마 초기화 해야될듯) => 공격력 오름
    광고 제거시 상점 해당 버튼 비활성화 시키기(일단 비활성화 만들긴 했음, 초기화하고 다시 확인해보기) => 했음
    업적, 미션 옆에 스크롤 달기 -=> 완료
    업적도 저장해야됨 => 저장 완료
    업적 스탯들 적용 되는지 확인 => 적용시키긴함(
    데일리미션 어떻게 저장할지 생각해보기 => 해결
    +버튼 누르면 하단 안보이는문제 => 해결 
    던전 결과창 수정하기 => 일단 실패는 했음 
    구글 로그인 확인하기 => 성공
    가끔 보스 z값 이상해지는거같은데 확인하기 => z값 고정함
    safeArea 설정하고나서 화면 이상한거 확인하기, 골드, 다이아 획득 위치 이상함 => 팝업 등등(아이폰으로도 확인하기) => 확인됌
    배치할때, PlayerController꺼져있을수 있음 처음 시작할때 켜줘야할듯 => SetInfo에서 활성화 완료
    렌더 카메라 이상한 이유 => 고정하지않고 처음 시작할때 코드로 생성해주면 됌
    배치 캐릭터 저장하고 불러오기 => 해씅ㅁ
    하단 버튼들 한번만 클릭되게, 하나 켜지면 기존거 꺼지게 => 했음
    마스터리, 코스튬 뺄지말지 => 이건 빼는게 맞을거같음(마스터리만 넣던가) => 삭제
    캐릭터들 스킬 효과, 스킬 이미지 만들기  =>
    데미지 확인해보기 => 맞음
    렌더 카메라 되어있는것들 꾸며주기(다 하고 꾸미자) -> 던전, 영웅, 스탯등 => 함
    퀘스트 처음 0으로 시작 => 완료
    게임씬 -> 보스씬으로 넘어가는순간에 던전에 들어가면 어떻게할지? => State Changing으로 만들어놨음, 
    클레릭 스킬 안나가는 이유 => level을 high로 올려서 링커가 생성자를 삭제해서 그런거였음
    재화업데이트 => 했음 일일상점은 그냥 뺌
    골드던전 HP 테스트 => 10단계까지밖에없는데 너무 약한거같음( 그래서 * 20해줌)
    Atlas로 사용하는 이미지들 확실하게 => 일단 maxsize 올려놓긴했음 확인해봐야함(된듯)
    모든 광고 제거 안되는 이유 => 앱에서 넣었음 테스트 해보기(애플은 내부결제 하려면 결제해야해서 안함)
    결과창 성공만 테스트하면 됌 (테스트 완료)
    버프 레벨, fastmode image(완료))
    하단 스크롤도 에러있음 => 고쳤음
    퀘스트 테스트 하기 => 퀘스트 완료
    각인아이템 획득하는거 넣어서 테스트 해보기 => 각인 완료
    영웅, 유물에서 소환하기 누르면 안되는거 => 완료
    공격속도 적용되는지 확인(스탯에서도) => 일단 수정은 해놨는데 세부 수치 확인해보기 => 완료
    던전 둘다 비교(안꺼지는게 있는거같음) => 비교해본결과 됌.
    각종 반지 효과 만들고, 이미지도 만들어야됌. 유물팝업 양쪽으로 너무 넓어져있음 => 반지 효과 만들었음(경험치, 골드 획득)
    상점 팝업 처음에 영웅소환, 유물소환 text, image잘 안맞음 => 해결
    화면이 꺼져도 실행되나 확인해보기 => 백그라운드 실행은 안되는거임(저장 방법 변경함)
    유물에서 클릭해서 강화하기 누를때 UI_Toast밑으로 나옴 => 해결
    광고 시청시 사운드 일시정지 => 됌
    버튼 클릭 사운드 추가 => 함 
    보스몬스터 나올때 겹치더라도 밀릴수 있도록하기 => 일단은 해놨음
    골드던전 모두가 가운데에서 생성되게 하지 않기 => 랜덤값으로 변경
    설정에서 배경음, 효과음 처음에는 풀로 맞춰놓기 => 했음
    게임 효과음 추가 (다 추가는 해놨음, 이제 맞추기만 하면 될듯) => 효과음 추가 완료
    인앱 항목 찾을수 없음 => 일단 전 버전은 됐는데 이름 바꿔서 빌드해서 테스트해봐야함 => 성공
    로그아웃, 리셋버튼하면 게임 아예 재시작되게 => 바꿈
    로그인하고 ID 메인씬에 적용하기 => 했음
    로그아웃, 리셋 테스트 => 다 잘됌
    언어에서 일본어 빼던가 넣어주던가 하기 => 뺌
    구글 로그인을 해도 누르면 게스트로 로그인이 되는듯 이것도 확인해봐야함 => 수정 했음
    렐릭 인포에서 소환 누르면 하단 없어짐 => 해결
    상점 재화부분 애플에서는 안되니까 대충이라도 해놓기 => 해놨음
    애플 테스트할때 Restore기능 확인하기, 시작할때 ATT도 확인() => ReStore는 결제안해서 못하고, ATT는 처음에 나온거 확인하면 됌
    BackKey로 팝업닫으면 오류 생김 => 해결함
    아이폰 홀드시 pause왜 안되는지? => 시간찍어서 확인해보기(되는데, 시간 확인이안됌 => 수정해야될듯) => override 안해서 그런거같음
    사망했을때 던전 못들어가게하기(멈춰버림) => 이중으로 잠궈놓긴했음
    게스트로 했을때 => 구글로그인하기 or 게스트로 계속하기 => 처음에 로그인할때 => 해결

    TODO : 계정 연동 오류 팝업 크기 맞추기 => 해놨음 테스트 해보기
    TODO : 랭킹 시스템
    TODO : 아이템 , 이름 등등도 Localization해주자(시트 하나 더 만들어서 하는게 나을수도있을듯)
    TODO : Localization 추가하기
    TODO : 도중에 몬스터와 보스가 같이나오는 경우가 있음 => 아직 확인안됌
    TODO : 게임씬 나눠주기
    TODO : 갑자기 영웅 사라지는거 확인 => 아직 확인 못함

    [MonsterController] OnDisable()에서 Managers.StageM.deadEvent -= OnDead 구독 해제 누락 => 해결 (OnDisable에 null 체크 포함하여 구독 해제 추가)
    [SpawnManager] Init()에서 이벤트 9개 구독하지만 Clear/OnDestroy에서 해제 안 함 => 해결 (Init에 중복 구독 방지 패턴 적용, OnDestroy에서 Clear() 자동 호출 추가)
    [PlayerController] Damage 프로퍼티에서 Characters_Data[DATA.Name] 직접 접근 => 해결 (Damage, Defense 프로퍼티, SetStat() 3곳 모두 TryGetValue()로 교체, 미발견 시 경고 로그 출력)
    [UIManager] GetPopupUI()에서 SpawnUI() 반환값 null 체크 없이 바로 .gameObject 접근 => 해결 (GetPopupUI, ShowPopup 두 곳에 null 체크 및 에러 로그 추가)

    [UpdateManager] ExcuteWriteData()가 async void => 해결 (async UniTaskVoid로 교체, try-catch-finally 추가, isWriting 플래그 finally에서 해제)
    [GameManager] SaveGameOnPause()가 async void => 해결 (async UniTaskVoid로 교체, try-catch 추가, 호출부에 .Forget() 추가)
    [SpawnManager] OnDungeon()이 async void => 해결 (이벤트 핸들러 void 유지, 비동기 로직을 OnDungeonAsync()로 분리 후 .Forget() 호출)
    [FirebaseDB] WriteData() 실패 시 재시도 로직 없음 => 해결 (최대 3회, 2초 간격 재시도 추가. 3회 모두 실패 시 에러 로그 출력)
    [FirebaseLogin] catch { return false; } 빈 catch 블록 => 해결 (SignInWithCredentialOnly에 FirebaseException/Exception 구분 처리 추가, LinkGoogleToCurrentUser의 throw e → throw; 로 교체하여 스택 트레이스 보존)
    [PlayerController] FindClosetTarget() 매 프레임 실행 => 해결 (PlayerController, MonsterController에 0.3초 탐색 쿨다운 추가)
    [CreatureController] Vector3.Distance() => 해결 (FindClosetTarget, GoBackToSpawn, Tick의 거리 비교 모두 sqrMagnitude로 교체)

    [ADManager] 광고 보상 콜백(OnAdPaid)이 Android JNI 스레드에서 호출되어 UI 생성 시 "Graphics device is null" 에러 발생 => 해결 (MobileAds.Initialize() 전에 MobileAds.RaiseAdEventsOnUnityMainThread = true 설정 추가. Google Mobile Ads SDK 내부에서 모든 광고 이벤트 콜백을 Unity 메인 스레드에서 실행하도록 보장)

    [FirebaseDB] WriteData()에서 DATA/CHARACTER/ITEM/SMELT 4개 동시 저장 시 일부만 성공하면 데이터 불일치 => 해결 (4개를 Dictionary로 묶어 userRef 단일 경로에 SetRawJsonValueAsync 1회 호출로 원자적 저장)
    [FirebaseDB] IsNewDay()가 클라이언트 시간 기반 => 해결 (DateTime.MinValue/MaxValue 범위 체크 추가, 변환 실패 시 try-catch로 안전하게 처리)
    [BuffManager] SceneUI를 UI_GameScene으로 직접 캐스팅 => 해결 (GameScene 헬퍼 프로퍼티 추가, 3곳 모두 GameScene?. 로 교체하여 null 안전 접근)
    TODO : [UIManager] popup.Init().Forget() => 예외 처리 포함한 방식으로 교체 (UniTask.Void + try-catch)

    === 성능 최적화 TODO ===

    [높음] [UpdateManager] tickables.Contains() 매 프레임 O(n) 탐색 => List → HashSet으로 교체하여 O(1)로 개선
    [높음] [CharacterManager] AlivePlayers 프로퍼티가 매 호출마다 Where().ToList() 새 List 생성 => 캐싱 또는 스팬/열거자 방식으로 교체
    [높음] [CreatureController] FindClosetTarget(List<T>) 에서 .ToArray() 호출로 매번 배열 할당 => 오버로드 통합 or span 활용
    [높음] [PlayerController] Damage 프로퍼티 내부에서 Dictionary 탐색 + Manager 체인 계산이 Tick마다 발생 가능 => 캐싱 도입

    [중간] [PoolManager] ContainsKey() + [] 이중 탐색 => TryGetValue() 한 번으로 교체
    [중간] [SpawnManager] DeSpawnMonster()에서 mcList.ToList() 불필요한 복사 => 역순 for 루프로 교체
    [중간] [SpawnManager] AlivePlayers 반복 호출 (ApplyKnockBackWithDelay 등) => 지역 변수로 캐싱
    [중간] [UIManager] UI_Root를 매번 GameObject.Find()로 탐색 => 최초 1회 캐싱
    [중간] [GameManager] 재화 변경 시 OnGoodsChanged 매번 즉시 발생 => 일정 주기 배칭으로 UI 갱신 횟수 줄이기

    [낮음] [BuffController] AddBuff()에서 activeBuffs.Find() 선형 탐색 => Dictionary<BuffEffectType, IBuff> 구조로 교체
    [낮음] [GameData] GetGradeCharacter() 매 호출마다 임시 List 생성 후 랜덤 선택 => 사전에 등급별 분류 캐싱



    Sound Effect by <a href="https://pixabay.com/ko/users/freesound_community-46691455/?utm_source=link-attribution&utm_medium=referral&utm_campaign=music&utm_content=108175">freesound_community</a> from <a href="https://pixabay.com/sound-effects//?utm_source=link-attribution&utm_medium=referral&utm_campaign=music&utm_content=108175">Pixabay</a>
    */


}
