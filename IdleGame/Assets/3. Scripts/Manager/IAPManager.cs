using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using Cysharp.Threading.Tasks;
public class IAPManager : IStoreListener
{
    public readonly string removeADS = "removeads";
    public readonly string dia300 = "dia300";
    public readonly string dia550 = "dia550";
    public readonly string dia1200 = "dia1200";
    public readonly string dia4000 = "dia4000";
    public readonly string dia7000 = "dia7000";
    public readonly string dia13000 = "dia13000";

    private IStoreController storeController; // 구매 과정 제어
    private IExtensionProvider storeExtensionProvider; // 플랫폼 위한 확정 처리
    private bool isUserRequestedPurchase = false;

    public bool IsInitialized => storeController != null && storeExtensionProvider != null;
    public Action OnPurchaseSuccess;
    public Action OnPurchaseFail;

    public void InitUnityIAP()
    {


#if UNITY_IOS
        Debug.LogWarning("### [IAP] iOS 플랫폼 감지: IAP 기능을 비활성화합니다. (포트폴리오용 스킵) ###");
        return;
#else

        if(IsInitialized) return;

        //초기화가 필요한데 초기화를 도와주는 코드임
        var module = StandardPurchasingModule.Instance();
        var builder = ConfigurationBuilder.Instance(module);


        builder.AddProduct(dia300, ProductType.Consumable);
        builder.AddProduct(dia550, ProductType.Consumable);
        builder.AddProduct(dia1200, ProductType.Consumable);
        builder.AddProduct(dia4000, ProductType.Consumable);
        builder.AddProduct(dia7000, ProductType.Consumable);
        builder.AddProduct(dia13000, ProductType.Consumable);
        builder.AddProduct(removeADS, ProductType.NonConsumable);

        UnityPurchasing.Initialize(this, builder);
#endif
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("### [IAP] 초기화 성공! 등록된 상품: " + controller.products.all.Length + "개 ###");
        storeController = controller;
        storeExtensionProvider = extensions;

        //앱 시작시 구매 이력 복구 광고제거상태 동기화
        CheckPurchasedProducts();
    }


    public void OnInitializeFailed(InitializationFailureReason error, string message = null)
    {
        Debug.LogError($"### [IAP] 초기화 실패: {error} ###");
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        throw new System.NotImplementedException();
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.LogError($"### [IAP] 구매 최종 실패: {product.definition.id} | 사유: {failureReason} ###");
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        string productId = args.purchasedProduct.definition.id;
        bool isNewPurchase = isUserRequestedPurchase;
        isUserRequestedPurchase = false;
        Debug.Log($"### [IAP] 구매 처리 시작: {productId} ###");

        if (!Enum.TryParse(productId, out Define.IAP iapType))
        {
            return PurchaseProcessingResult.Complete;
        }

        if (productId == removeADS)
        {
            ApplyRemoveAds();
            NotifyPurchaseSucces();
        }

        bool isShowPopup = isNewPurchase || args.purchasedProduct.definition.type == ProductType.Consumable;
        ApplyPurchaseReward(iapType, isShowPopup);

        return PurchaseProcessingResult.Complete;
    }

    private void ApplyPurchaseReward(Define.IAP _type, bool _showPopup)
    {
        if (_showPopup)
        {
            HandlePurchaseAsync(_type).Forget();
        }
        else
        {
            Debug.Log($"### [IAP] {_type} 자동 복구 완료 (팝업 없음) ###");
        }
    }

    public async UniTask HandlePurchaseAsync(Define.IAP _iap)
    {
        var rewardPopup = await Managers.UIM.ShowPopup<UI_Reward>();

        rewardPopup.GetIAPReward(_iap);
    }

    public void Purchase(string _productID)
    {

#if UNITY_IOS
        Managers.UIM.ShowToast(Managers.LocalizationM.Get("UIToastIOSNoIAP"));
        Debug.Log("IOS에서는 인앱결제 사용안함 ");
        return;
#else
        if (!IsInitialized)
        {
            Debug.LogError("### [IAP] 결제 실패: IAP 시스템이 아직 초기화되지 않았습니다. ###");
            return;
        }
        
        Product product = storeController.products.WithID(_productID);
        if (product != null && product.availableToPurchase)
        {
            Debug.Log($"### [IAP] 구매 시도: {product.metadata.localizedTitle} ###");
            Managers.GameM.isInternalPause = true;
            isUserRequestedPurchase = true;
            storeController.InitiatePurchase(product);
        }

        
#endif

    }

    public Product GetProduct(string _prouductID)
    {
        return storeController.products.WithID(_prouductID);
    }

    //Restore버튼
    public void RestorePurchase()
    {
#if UNITY_IOS
        Debug.Log("IOS에서는 인앱결제 사용 안함");
        return;
#else

        if (storeController == null)
        {
            Debug.Log("IAP is not init");
            return;
        }

        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {

            var apple = storeExtensionProvider.GetExtension<IAppleExtensions>();
            apple.RestoreTransactions((bool succes, string message) =>
            {
                Debug.Log($"Restore Purchase Completed :  succes={succes}, message = {message}");

                if (succes)
                {
                    Debug.Log("Restore Succes");
                    CheckPurchasedProducts();
                }
                else
                {
                    Debug.Log("Restore Fail" + message);
                }
            });
        }
        else
        {
            Debug.Log("Restore Purchase is not support");
        }
#endif
    }
    private void CheckPurchasedProducts()
    {
        if (storeController == null) return;
        foreach (var product in storeController.products.all)
        {
            if (product.hasReceipt && product.definition.id == removeADS)
            {
                ApplyRemoveAds();
                break;
            }
        }
    }

    public void ApplyRemoveAds()
    {
        PlayerPrefs.SetInt("IsRemoveAds", 1);
        PlayerPrefs.Save();

        Managers.GameM.gameData.ADS_Remove = true;
    }

    public void NotifyPurchaseSucces()
    {
        OnPurchaseSuccess?.Invoke();
    }


    public void Clear()
    {
        OnPurchaseSuccess = null;
        OnPurchaseFail = null;

        isUserRequestedPurchase = false;

        Debug.Log("[IAPManager] 유저 구매 상태 및 이벤트 초기화 완료");
    }
}