using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using System;
using DG.Tweening;

public class Gold_Dice : MonoBehaviour
{
    public TextMeshPro coinText;
    public Transform diceImage;
    public GameObject particle;
    private void OnEnable()
    {
        Init();
        AsyncDiceRoll().Forget();
    }

    void Init()
    {
        transform.localScale = Vector3.zero;
        if (coinText != null) coinText.text = "";
        if (particle != null) particle.SetActive(false);
    }
    async UniTaskVoid AsyncDiceRoll()
    {
        try
        {
            transform.DOScale(0.5f, 0.2f).SetEase(Ease.OutBack);

            for (int i = 0; i < 10; i++)
            {
                int randnum = UnityEngine.Random.Range(1, 7);
                coinText.text = randnum.ToString();

                diceImage.DOPunchRotation(new Vector3(0, 0, 90f), 0.1f);
                diceImage.DOPunchScale(Vector3.one * 0.2f, 0.1f);

                await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: this.GetCancellationTokenOnDestroy());
            }

            diceImage.DORotate(new Vector3(45f, 0, 0), 0.2f);
            transform.DOScale(0.6f, 0.2f).SetEase(Ease.OutQuad);
            if (particle != null)
            {
                particle.SetActive(true);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(1.0f), cancellationToken: this.GetCancellationTokenOnDestroy());

            await transform.DOScale(0f, 0.5f).SetEase(Ease.InBack).ToUniTask();
        }
        catch (OperationCanceledException) { }
        catch (Exception e) { }
        finally
        {
            transform.DOKill();
            diceImage.DOKill();
            Managers.ResourceM.Destroy(this.gameObject);
        }
        
    }
 
}
