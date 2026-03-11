# 🧩 way2tushar.NativeAlerts — Documentation

**NativeAlerts** is a Unity plugin that brings **native iOS and Android popup alerts** (using system `UIAlertController` and `AlertDialog`) directly into your Unity games and apps.  
It supports:
- ✅ Multiple buttons (any number)
- ✅ Dark, Light, and System theme modes
- ✅ Asynchronous API (`await`-ready)
- ✅ Full IL2CPP and Mono support
- ✅ No Gradle or Android Studio dependency (Android implemented via Unity’s `AndroidJavaObject` bridge)
- ✅ iOS native implementation with Light/Dark overrides
- ✅ Auto handles app switching and scene reloads

---

## 📂 Folder Structure

```
Assets/
└── way2tushar/
    └── NativeAlerts/
        ├── Runtime/
        │   ├── NativeAlert.cs
        │   ├── NativeAlertBridge.cs
        │   └── way2tushar.NativeAlerts.asmdef
        ├── iOS/
        │   ├── NativeAlerts.h
        │   └── NativeAlerts.mm
        ├── Samples/
        │   ├── Demo.unity
        │   └── AlertDemo.cs
        └── README.md
```

---

## 🧠 Overview

`way2tushar.NativeAlerts` provides a **cross-platform alert system** with the same Unity API that adapts natively per platform:

| Platform | Backend            | Features                                |
|-----------|-------------------|------------------------------------------|
| **Android** | `android.app.AlertDialog` | Dark/Light dialogs, 3 buttons, or list view |
| **iOS** | `UIAlertController` | Unlimited buttons, Dark/Light overrides  |
| **Editor** | Simulated Stub    | Logs JSON, auto-resolves index 0         |

---

## ⚙ Installation

1. Download and Import from AssetStore.  
2. Done — no extra steps, no Gradle setup, no manifest editing required.

---

## 🚀 Quick Start

```csharp
using UnityEngine;
using way2tushar.NativeAlerts;

public class Example : MonoBehaviour
{
    async void Start()
    {
        var index = await NativeAlert.ShowAsync(new AlertOptions {
            title = "Hello!",
            message = "This is a native popup test.",
            theme = AlertTheme.System,
            buttons = new() {
                new() { text = "OK" },
            }
        });

        Debug.Log($"Button pressed index: {index}");
    }
}
```

---

## 📚 API Reference

### 🔹 **`NativeAlert.ShowAsync(AlertOptions options)`**
Shows a native popup asynchronously and returns the index of the button pressed.

**Returns:** `Task<int>` → the button index.

---

### 🔹 **`AlertOptions`**

| Property | Type | Description | Default |
|-----------|------|-------------|----------|
| `title` | `string` | The alert title. | `""` |
| `message` | `string` | The alert message text. | `""` |
| `buttons` | `List<AlertButton>` | Buttons shown in the alert. | `[OK]` |
| `theme` | `AlertTheme` | Color mode: `System`, `Light`, or `Dark`. | `System` |

---

### 🔹 **`AlertButton`**

| Property | Type | Description |
|-----------|------|-------------|
| `text` | `string` | The text of the button. |
| `style` | `AlertButtonStyle` | The visual style: `Default`, `Cancel`, `Destructive`. |

---

### 🔹 **Enums**

```csharp
public enum AlertTheme { System, Light, Dark }
public enum AlertButtonStyle { Default, Cancel, Destructive }
```

---

## 🧩 Activity Examples

### 🟢 **Activity 1: Simple OK Alert**

```csharp
await NativeAlert.ShowAsync(new AlertOptions {
    title = "Hello!",
    message = "Welcome to NativeAlerts!"
});
```

---

### 🔵 **Activity 2: Confirmation Dialog**

```csharp
int result = await NativeAlert.ShowAsync(new AlertOptions
{
    title = "Delete file?",
    message = "This action cannot be undone.",
    theme = AlertTheme.System,
    buttons = new() {
        new() { text = "Cancel" },
        new() { text = "Delete" }
    }
});
```

---



### 🟣 **Activity 3: Theming (Light/Dark/System)**

```csharp
await NativeAlert.ShowAsync(new AlertOptions
{
    title = "Theme Test",
    message = "This is the Dark theme preview.",
    theme = AlertTheme.Dark,
    buttons = new() { new() { text = "OK" } }
});
```

---

### 🔴 **Activity 4: Complex Workflow**

```csharp
int langIndex = await NativeAlert.ShowAsync(new AlertOptions
{
    title = "Language",
    message = "Select your language",
    buttons = new() {
        new() { text = "English" },
        new() { text = "Bangla" },
        new() { text = "Hindi" }
    }
});

string lang = langIndex switch
{
    0 => "English",
    1 => "Bangla",
    2 => "Hindi",
    _ => "Unknown"
};

int confirm = await NativeAlert.ShowAsync(new AlertOptions
{
    title = "Confirm",
    message = $"Set language to {lang}?",
    buttons = new() {
        new() { text = "Cancel"},
        new() { text = "Yes" }
    }
});
```

---

## ⚡ Behavior Notes

| Feature | Android | iOS |
|----------|----------|-----|
| Multiple buttons | Up to 3 (else list view) | Unlimited |
| Orientation changes | Safe | Safe |
| Async await support | ✅ | ✅ |

---

## 🧰 Integration Tips

- Safe to call from any thread.
- `NativeAlertBridge` persists across scenes.
- Use `await` or `.ContinueWith(...)`.
- Minimum: Android 5.0+, iOS 12+.

---

## 🧑‍💻 Troubleshooting

| Issue | Cause | Fix |
|--------|--------|-----|
| Alert doesn’t appear | Background thread | Already handled internally |
| Build fails (iOS) | Wrong file type | Ensure `.mm` not `.m` |
| Theme not applied | OEM override | Use explicit theme |

---


## 🧪 Sample Scene

Open `Samples/Demo` — shows a dark-mode alert automatically.

---

## 🏁 License

Created by **way2tushar** — free for personal and commercial use.

---

## 💬 Support

Need help or want a new feature (like text input or sheets)?  
Open an issue or contact the author.
If you like the asset, please give it a better rating, as that feedback is a great source of inspiration.

---

## ✅ Summary

| Feature | Status |
|----------|---------|
| Android AlertDialog | ✅ |
| iOS UIAlertController | ✅ |
| Dark / Light / System | ✅ |
| Async API | ✅ |
| IL2CPP Safe | ✅ |
| Multi Button | ✅ |
| Editor Safe | ✅ |
| Thread Safe | ✅ |
