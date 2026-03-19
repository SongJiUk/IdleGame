using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class DropItemController : UIDirecting
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

    Define.Grade grade;
    Data.ItemData itemData;
    Camera cam;
    Tween timeTween;
    public override bool Init()
    {
        if (!base.Init()) return false;
        if (itemRect == null || text == null || grades == null || loot == null)
        {
            Debug.LogError("[DropItemController] 필요한 오브젝트가 없음");
            return false;
        }
        cam = Camera.main;

        for (int i = 0; i < grades.Count; i++) grades[i].SetActive(false);
        return true;
    }

    public void SetInfo(Vector3 _pos, Data.ItemData _itemData)
    {

        itemData = _itemData;
        grade = itemData.ItemGrade;
        transform.position = _pos;
        Vector3 targetPos = new Vector3(_pos.x + (Random.insideUnitSphere.x * 2f)
            , 0.5f, _pos.z + (Random.insideUnitSphere.z * 2f));

        if ((Managers.UIM.SceneUI as UI_GameScene).isSavingMode)
        {
            LootItem().Forget();
            return;
        }

        SimluateProjectileAsync(targetPos).Forget();
    }

    public async UniTask SimluateProjectileAsync(Vector3 _pos)
    {
        Vector3 displacementXZ = _pos - transform.position;
        displacementXZ.y = 0;
        float range = displacementXZ.magnitude;
        float angleRad = firingAngle * Mathf.Deg2Rad;

        float V0_squared = (range * gravity) / Mathf.Sin(2 * angleRad);
        if (V0_squared <= 0 || Mathf.Abs(Mathf.Sin(2 * angleRad)) < 0.001f)
        {
            Debug.LogError("�߻�ü ������ 0���ų� 90����");
            return;
        }

        float V0 = Mathf.Sqrt(V0_squared); //�ʱ�ӷ� 
        float Vx = V0 * Mathf.Cos(angleRad); //���� �ӵ�
        float Vy = V0 * Mathf.Sin(angleRad); // ���� �ʱ� �ӵ�
        float flightDuration = range / Vx; // ����ð� ( �ð� = �Ÿ� / �ӵ�)

        Vector3 horizontalDirection = displacementXZ.normalized;
        Vector3 startPos = transform.position;


        try
        {
            float currentTime = 0f;

            timeTween = DOTween.To(() => currentTime, x => currentTime = x, flightDuration, flightDuration)
                .OnUpdate(() =>
                {
                    if (transform == null)
                    {
                        timeTween?.Kill();
                        return;
                    }

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

            await timeTween.AsyncWaitForCompletion();
            if (this == null) return;
            transform.position = _pos;
            ItemCheck();

        }
        catch (System.OperationCanceledException) { }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
        finally
        {
            if (timeTween != null && timeTween.IsActive())
            {
                timeTween.Kill();
            }
        }
    }

    //�̰� �̰��̰�
    void ItemCheck()
    {
        transform.rotation = Quaternion.identity;

        grades[(int)grade].gameObject.SetActive(true);

        itemRect.gameObject.SetActive(true);
        itemRect.SetParent(Managers.UIM.SceneUI.WorldItemParent, false);
        text.text = Utils.StringToColorGrade(grade) + Managers.LocalizationM.Get(itemData.Name) + "</color>";

        if (cam != null) itemRect.position = cam.WorldToScreenPoint(transform.position);

        LootItem().Forget();
    }

    public async UniTask LootItem()
    {
        try
        {
            await UniTask.WaitForSeconds(Random.Range(1.0f, 1.5f));

            for (int i = 0; i < grades.Count; i++) grades[i].SetActive(false);

            (Managers.UIM.SceneUI as UI_GameScene).GetItem(itemData);
            Managers.InventoryM.GetItem(itemData);

            itemRect.transform.SetParent(this.transform, false);
            itemRect.gameObject.SetActive(false);
            loot.Play();

            if ((Managers.UIM.SceneUI as UI_GameScene).isSavingMode)
            {
                if ((Managers.UIM.SceneUI as UI_GameScene).savingModePopup != null)
                {
                    (Managers.UIM.SceneUI as UI_GameScene).savingModePopup.GetItem(itemData);
                }
            }

            await UniTask.WaitForSeconds(0.5f);

            Managers.ObjectM.DeSpawn<DropItemController>(this);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }

    }

    private void OnDisable()
    {
        if (timeTween != null && timeTween.IsActive())
        {
            timeTween.Kill();
            timeTween = null;
        }
    }

}
