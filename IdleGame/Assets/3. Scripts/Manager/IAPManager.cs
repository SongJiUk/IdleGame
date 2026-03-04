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


    public Action OnPurchaseSuccess;
    public Action OnPurchaseFail;
    public void InitUnityIAP()
    {

        if (Managers.IAPM == null) return;
#if UNITY_IOS
        Debug.Log("IAP disabled on IOS)");
        return;
#else
        if (storeController != null) return;

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
        Debug.Log("--- 초기화 성공: 등록된 상품 목록 ---");
        foreach (var product in controller.products.all)
        {
            Debug.Log($"ID: {product.definition.id} | 제목: {product.metadata.localizedTitle}");
        }

        storeController = controller;
        storeExtensionProvider = extensions;
    }


    public void OnInitializeFailed(InitializationFailureReason error, string message = null)
    {
        Debug.LogError("초기화 실패");
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        throw new System.NotImplementedException();
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.LogError("구매 실패");
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        Debug.Log($"구매 성공: {args.purchasedProduct.definition.id}");
        Debug.Log($"TransactionID: {args.purchasedProduct.transactionID}");

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
        if (storeController == null)
        {
            Debug.LogError("🚨 IAP ERROR: storeController가 아직 null입니다. 초기화가 안 끝났어요!");
            return;
        }

        // 현재 등록된 모든 상품 확인
        Debug.Log($"🔍 현재 상점에 등록된 상품 개수: {storeController.products.all.Length}");
        foreach (var p in storeController.products.all)
        {
            Debug.Log($"상품 목록: {p.definition.id} | 사용가능: {p.availableToPurchase}");
        }

        Product product = storeController.products.WithID(_productID);

        if (product != null && product.availableToPurchase)
        {
            storeController.InitiatePurchase(product);
        }
        else
        {
            // 여기서 정확히 뭐가 문제인지 로그를 쪼개서 봅니다.
            if (product == null)
                Debug.LogError($"🚨 상품 ID가 일치하는 게 없습니다: {_productID}");
            else if (!product.availableToPurchase)
                Debug.LogError($"🚨 상품은 찾았는데 구매가 불가능한 상태입니다: {product.definition.id}");
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