using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class DropItemController : BaseController
{
    float firingAngle = 45.0f;
    float gravity = 9.8f;

    [SerializeField]
    Transform itemRect;
    [SerializeField]
    TextMeshProUGUI text;
    [SerializeField]
    List<GameObject> grades;
    [SerializeField]
    ParticleSystem loot;

    Define.ItemGrade grade;
    public override bool Init()
    {
        if (!base.Init()) return false;
        if (itemRect == null  || text ==null || grades == null || loot == null)
        {
            Debug.LogError("[DropItemController] 컴포넌트가 없습니다. 인스펙터 확인하기");
            return false;
        }

        for (int i = 0; i < grades.Count; i++) grades[i].SetActive(false);
        return true;
    }

    public void SetInfo(Vector3 _pos)
    {

        grade = (Define.ItemGrade)Random.Range(0, 5);
        transform.position = _pos;
        Vector3 targetPos = new Vector3(_pos.x + (Random.insideUnitSphere.x * 2f)
            , 0.5f, _pos.z + (Random.insideUnitSphere.z * 2f));

        SimluateProjectileAsync(targetPos).Forget();
    }

    public async UniTask SimluateProjectileAsync(Vector3 _pos)
    {
        Vector3 displacementXZ = _pos - transform.position;
        displacementXZ.y = 0;
        float range = displacementXZ.magnitude;
        float angleRad = firingAngle * Mathf.Deg2Rad;

        float V0_squared = (range * gravity) / Mathf.Sin(2 * angleRad);
        if (V0_squared <= 0 || Mathf.Abs(Mathf.Sin(2* angleRad)) < 0.001f)
        {
            Debug.LogError("발사체 각도가 0도거나 90도임");
            return;
        }

        float V0 = Mathf.Sqrt(V0_squared); //초기속력 
        float Vx = V0 * Mathf.Cos(angleRad); //수평 속도
        float Vy = V0 * Mathf.Sin(angleRad); // 수직 초기 속도
        float flightDuration = range / Vx; // 비행시간 ( 시간 = 거리 / 속도)

        Vector3 horizontalDirection = displacementXZ.normalized;
        Vector3 startPos = transform.position;

        Tween timeTween = null;

        try
        {
            float currentTime = 0f;

            timeTween = DOTween.To(() => currentTime, x => currentTime = x, flightDuration, flightDuration)
                .OnUpdate(() =>
                {
                    float t = currentTime;
                    Vector3 nextPosXZ = startPos + horizontalDirection * (Vx * t);

                    float nextPosY = startPos.y + (Vy * t) - (0.5f * gravity * t * t);
                    transform.position = new Vector3(nextPosXZ.x, nextPosY, nextPosXZ.z);

                    if (t > 0.001f)
                    {
                        Vector3 velocity = (transform.position - startPos) / t;
                        transform.rotation = Quaternion.LookRotation(velocity);
                    }

                })
                .SetLink(gameObject);

            await timeTween.ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());
            transform.position = _pos;
            ItemCheck();


            
        }
        catch (System.OperationCanceledException) { }
        catch (System.Exception e)
        {
            Debug.LogError("드랍아이템 시뮬레이션 중 오류 발생 : " + e.Message);
        }
        finally
        {
            if (timeTween != null && timeTween.IsActive())
            {
                timeTween.Kill();
            }
        }
    }

    //이거 이거이거
    void ItemCheck()
    {
        transform.rotation = Quaternion.identity;

        grades[(int)grade].gameObject.SetActive(true);

        itemRect.gameObject.SetActive(true);
        itemRect.parent = Managers.UIM.SceneUI.WorldItemParent;
        //TODO : 아이템 정보 받아와서 작성
        text.text = Utils.StringToColorGrade(grade) + "TESET ITEM" + "</color>";
         

        itemRect.position = Camera.main.WorldToScreenPoint(transform.position);

        LootItem().Forget();
    }

    public async UniTask LootItem()
    {
        try
        {
            await UniTask.WaitForSeconds(Random.Range(1.0f, 1.5f));

            for (int i = 0; i < grades.Count; i++) grades[i].SetActive(false);

            itemRect.transform.parent = this.transform;
            itemRect.gameObject.SetActive(false);
            loot.Play();
            await UniTask.WaitForSeconds(0.5f);

            Managers.ResourceM.Destory(gameObject);
        }
        catch(System.Exception e)
        {
            Debug.LogError("아이템 획득 에러 : " + e.Message);
        }

    }

}
