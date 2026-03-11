#import <UIKit/UIKit.h>
#import "NativeAlerts.h"
extern void UnitySendMessage(const char* obj, const char* method, const char* msg);
static UIViewController* TopVC(void){
    UIWindow *keyWin=nil;
    for(UIWindow *w in UIApplication.sharedApplication.windows){
        if(w.isKeyWindow){keyWin=w;break;}
    }
    if(!keyWin)keyWin=UIApplication.sharedApplication.keyWindow;
    UIViewController *vc=keyWin.rootViewController;
    while(vc.presentedViewController)vc=vc.presentedViewController;
    return vc;
}
static void ResolveBack(int callbackId,NSInteger index){
    NSString *payload=[NSString stringWithFormat:@"%d|%ld",callbackId,(long)index];
    UnitySendMessage("NativeAlertBridge_GO","OnAlertResult",payload.UTF8String);
}
static NSString* StrOrNil(id v){return [v isKindOfClass:[NSString class]]?(NSString*)v:nil;}
static NSString* ButtonText(id btn){if(![btn isKindOfClass:[NSDictionary class]])return @"OK";
    NSString *t=StrOrNil(((NSDictionary*)btn)[@"text"]);return t.length?t:@"OK";}
static UIAlertActionStyle ButtonStyle(id btn){
    if(![btn isKindOfClass:[NSDictionary class]])return UIAlertActionStyleDefault;
    id raw=((NSDictionary*)btn)[@"style"];
    if([raw isKindOfClass:[NSString class]]){
        NSString *s=(NSString*)raw;
        if([s isEqualToString:@"Cancel"])return UIAlertActionStyleCancel;
        if([s isEqualToString:@"Destructive"])return UIAlertActionStyleDestructive;
        return UIAlertActionStyleDefault;
    }else if([raw isKindOfClass:[NSNumber class]]){
        switch([(NSNumber*)raw integerValue]){case 1:return UIAlertActionStyleCancel;
            case 2:return UIAlertActionStyleDestructive;default:return UIAlertActionStyleDefault;}
    }return UIAlertActionStyleDefault;
}
void _na_showAlert(const char* json,int callbackId){
    if(!json){ResolveBack(callbackId,0);return;}
    NSString *jsonStr=[NSString stringWithUTF8String:json];
    NSData *data=[jsonStr dataUsingEncoding:NSUTF8StringEncoding];
    NSError *err=nil;id root=[NSJSONSerialization JSONObjectWithData:data options:0 error:&err];
    if(err||![root isKindOfClass:[NSDictionary class]]){ResolveBack(callbackId,0);return;}
    NSDictionary *obj=(NSDictionary*)root;
    NSString *title=StrOrNil(obj[@"title"]);
    NSString *message=StrOrNil(obj[@"message"]);
    NSString *theme=StrOrNil(obj[@"theme"]);
    NSArray *buttons=obj[@"buttons"];
    if(![buttons isKindOfClass:[NSArray class]]||buttons.count==0)
        buttons=@[@{@"text":@"OK",@"style":@"Default"}];
    if(buttons.count>3)buttons=[buttons subarrayWithRange:NSMakeRange(0,3)];
    dispatch_async(dispatch_get_main_queue(),^{
        UIAlertController *ac=[UIAlertController alertControllerWithTitle:title.length?title:nil
            message:message.length?message:nil preferredStyle:UIAlertControllerStyleAlert];
        if(@available(iOS 13.0,*)){
            if([theme isEqualToString:@"Light"])ac.overrideUserInterfaceStyle=UIUserInterfaceStyleLight;
            else if([theme isEqualToString:@"Dark"])ac.overrideUserInterfaceStyle=UIUserInterfaceStyleDark;
        }
        [buttons enumerateObjectsUsingBlock:^(id btn,NSUInteger idx,BOOL *stop){
            NSString *txt=ButtonText(btn);
            UIAlertActionStyle st=ButtonStyle(btn);
            UIAlertAction *act=[UIAlertAction actionWithTitle:txt style:st handler:^(UIAlertAction *a){
                ResolveBack(callbackId,(NSInteger)idx);}];
            [ac addAction:act];
        }];
        UIViewController *presenter=TopVC();
        if(presenter)[presenter presentViewController:ac animated:YES completion:nil];
        else ResolveBack(callbackId,0);
    });
}