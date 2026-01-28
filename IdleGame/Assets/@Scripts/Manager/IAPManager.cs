using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using Cysharp.Threading.Tasks;
public class IAPManager : IStoreListener
{
    public readonly string removeADS = "removeads";
    public readonly string dia01 = "dia300";

    private IStoreController storeController; // 구매 과정 제어
    private IExtensionProvider storeExtensionProvider; // 플랫폼 위한 확정 처리

    public void InitUnityIAP()
    {
        if (storeController != null) return;

        //초기화가 필요한데 초기화를 도와주는 코드임
        var module = StandardPurchasingModule.Instance();
        var builder = ConfigurationBuilder.Instance(module);

        builder.AddProduct(
            dia01,
            ProductType.Consumable,
            new StoreSpecificIds
        {
            {dia01, GooglePlay.Name },
            {dia01, AppleAppStore.Name }
        });

        builder.AddProduct(
            removeADS,
            ProductType.NonConsumable,
            new StoreSpecificIds
        {
            {removeADS, GooglePlay.Name },
            {removeADS, AppleAppStore.Name }
        });

        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("초기화 성공");

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
        Product product =  storeController.products.WithID(_productID);
        if (product != null && product.availableToPurchase)
        {
            storeController.InitiatePurchase(product);
        }
        else
            Debug.Log("상품이 없거나 현재 구매가 불가능합니다.");
    }
}
