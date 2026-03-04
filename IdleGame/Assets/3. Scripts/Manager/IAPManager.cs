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

        //builder.AddProduct(
        //    dia300,
        //    ProductType.Consumable,
        //    new StoreSpecificIds
        //{
        //    {dia300, GooglePlay.Name },
        //    {dia300, AppleAppStore.Name }
        //});

        //builder.AddProduct(
        //    dia550,
        //    ProductType.Consumable,
        //    new StoreSpecificIds
        //{
        //    {dia550, GooglePlay.Name },
        //    {dia550, AppleAppStore.Name }
        //});

        //builder.AddProduct(
        //    dia1200,
        //    ProductType.Consumable,
        //    new StoreSpecificIds
        //{
        //    {dia1200, GooglePlay.Name },
        //    {dia1200, AppleAppStore.Name }
        //});

        //builder.AddProduct(
        //    dia4000,
        //    ProductType.Consumable,
        //    new StoreSpecificIds
        //{
        //    {dia4000, GooglePlay.Name },
        //    {dia4000, AppleAppStore.Name }
        //});

        //builder.AddProduct(
        //    dia7000,
        //    ProductType.Consumable,
        //    new StoreSpecificIds
        //{
        //    {dia7000, GooglePlay.Name },
        //    {dia7000, AppleAppStore.Name }
        //});

        //builder.AddProduct(
        //    dia13000,
        //    ProductType.Consumable,
        //    new StoreSpecificIds
        //{
        //    {dia13000, GooglePlay.Name },
        //    {dia300, AppleAppStore.Name }
        //});

        //builder.AddProduct(
        //    removeADS,
        //    ProductType.NonConsumable,
        //    new StoreSpecificIds
        //{
        //    {removeADS, GooglePlay.Name },
        //    {removeADS, AppleAppStore.Name }
        //});

        UnityPurchasing.Initialize(this, builder);
#endif
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("### [IAP] 초기화 성공! 등록된 상품: " + controller.products.all.Length + "개 ###");
        storeController = controller;
        storeExtensionProvider = extensions;
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
        Debug.Log($"### [IAP] 구매 성공 이벤트 수신: {args.purchasedProduct.definition.id} ###");

        string purchaseName = args.purchasedProduct.definition.id;

        Define.IAP iap = (Define.IAP)Enum.Parse(typeof(Define.IAP), purchaseName);
        HandlePurchaseAsync(iap).Forget();

        return PurchaseProcessingResult.Complete;
    }

    public async UniTask HandlePurchaseAsync(Define.IAP _iap)
    {
        var rewardPopup = await Managers.UIM.ShowPopup<UI_Reward>();

        rewardPopup.GetIAPReward(_iap);
    }

    public void Purchase(string _productID)
    {
#if UNITY_IOS
        Debug.Log("IOS에서는 인앱결제 사용안함 ");
        return;
#else
        //Product product = storeController.products.WithID(_productID);
        //if (product != null && product.availableToPurchase)
        //{
        //    storeController.InitiatePurchase(product);
        //}
        //else
        //    Debug.Log("상품이 없거나 현재 구매가 불가능합니다.");
        if (IsInitialized)
        {
            Debug.LogError("### [IAP] 결제 실패: IAP 시스템이 아직 초기화되지 않았습니다. ###");
            return;
        }
        
        Product product = storeController.products.WithID(_productID);
        if (product == null)
        {
            Debug.LogError($"### [IAP] 결제 실패: {_productID} ID를 가진 상품을 찾을 수 없습니다. (ID 오타 확인!) ###");
            return;
        }

        // 3. 구매 가능 여부 체크
        if (!product.availableToPurchase)
        {
            Debug.LogError($"### [IAP] 결제 실패: {product.metadata.localizedTitle} 상품은 현재 구매 불가능 상태입니다. ###");
            return;
        }

        Debug.Log($"### [IAP] 구매 시도: {product.metadata.localizedTitle} ###");
        storeController.InitiatePurchase(product);
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

    public void NotifyPurchaseSucces()
    {
        OnPurchaseSuccess?.Invoke();
    }

}