// Assets/way2tushar/NativeAlerts/Runtime/NativeAlert.cs
// Java-free Android + iOS bridge (namespace: way2tushar.NativeAlerts)
// - Max 3 buttons (no list mode)
// - Theme serialized as STRING ("System" | "Light" | "Dark")
// - ANDROID: Material/DeviceDefault *_Dialog_Alert styles; "System" follows device night mode
// - iOS: extern call (implemented in NativeAlerts.mm/.h)

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

namespace way2tushar.NativeAlerts
{
    public enum AlertTheme { System, Light, Dark }
    public enum AlertButtonStyle { Default, Cancel, Destructive }

    [Serializable]
    public class AlertButton
    {
        public string text;
        public AlertButtonStyle style = AlertButtonStyle.Default;
    }

    [Serializable]
    public class AlertOptions
    {
        public string title;
        public string message;
        public List<AlertButton> buttons = new() { new AlertButton { text = "OK" } };
        public AlertTheme theme = AlertTheme.System;
    }

    // Payload to serialize theme as STRING for native JSON
    [Serializable]
    class AlertOptionsPayload
    {
        public string title;
        public string message;
        public List<AlertButton> buttons;
        public string theme; // "System" | "Light" | "Dark"
    }

    public static class NativeAlert
    {
        static int _nextId = 1;
        static readonly Dictionary<int, TaskCompletionSource<int>> _pending = new();

#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")] private static extern void _na_showAlert(string json, int id);
#endif

        /// <summary>Show a native popup. Returns pressed button index (0..n-1).</summary>
        public static Task<int> ShowAsync(AlertOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (options.buttons == null || options.buttons.Count == 0)
                options.buttons = new() { new AlertButton { text = "OK" } };

            // Enforce max 3 across platforms
            if (options.buttons.Count > 3)
                options.buttons = options.buttons.GetRange(0, 3);

            // Serialize THEME AS STRING
            var payload = new AlertOptionsPayload
            {
                title = options.title,
                message = options.message,
                buttons = options.buttons,
                theme = options.theme.ToString()
            };
            var json = JsonUtility.ToJson(payload);

            var id = _nextId++;
            var tcs = new TaskCompletionSource<int>();
            _pending[id] = tcs;

#if UNITY_EDITOR
            Debug.Log($"[NativeAlert] Editor stub: {json}");
            tcs.SetResult(0);
#elif UNITY_IOS
            _na_showAlert(json, id);
#elif UNITY_ANDROID
            ShowAndroid_NoGradle(json, id);
#else
            tcs.SetResult(0);
#endif
            return tcs.Task;
        }

        /// <summary>iOS native calls this via UnitySendMessage with payload "&lt;id&gt;|&lt;index&gt;".</summary>
        public static void Resolve(string payload)
        {
            var parts = payload.Split('|');
            if (parts.Length != 2) return;
            if (!int.TryParse(parts[0], out var id)) return;
            if (!int.TryParse(parts[1], out var index)) return;
            TryResolve(id, index);
        }

        static void TryResolve(int id, int index)
        {
            if (_pending.TryGetValue(id, out var tcs))
            {
                _pending.Remove(id);
                tcs.TrySetResult(index);
            }
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        // ---------------- ANDROID (no Gradle/aar) ----------------

        class OnClickProxy : AndroidJavaProxy
        {
            readonly int _id;
            readonly int _index;
            public OnClickProxy(int id, int index)
                : base("android.content.DialogInterface$OnClickListener")
            { _id = id; _index = index; }

            void onClick(AndroidJavaObject dialog, int which)
            {
                TryResolve(_id, _index);
                try { dialog?.Call("dismiss"); } catch { }
            }
        }

        static void ShowAndroid_NoGradle(string json, int id)
        {
            var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            if (activity == null) { TryResolve(id, 0); return; }

            activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                try
                {
                    // Parse JSON (theme is a STRING)
                    var obj = new AndroidJavaObject("org.json.JSONObject", json);
                    string title    = obj.Call<string>("optString", "title", "");
                    string message  = obj.Call<string>("optString", "message", "");
                    string themeStr = obj.Call<string>("optString", "theme", "System"); // "System"|"Light"|"Dark"
                    var buttons     = obj.Call<AndroidJavaObject>("optJSONArray", "buttons");
                    if (buttons == null) buttons = new AndroidJavaObject("org.json.JSONArray");

                    int count = Math.Min(buttons.Call<int>("length"), 3);

                    // Resolve theme id:
                    // - System: detect device night mode and pick Light/Dark theme accordingly
                    // - Light/Dark: explicit mapping
                    int themeId = themeStr == "System"
                        ? ResolveSystemDialogThemeId(activity)
                        : ResolveDialogThemeId(themeStr);

                    AndroidJavaObject builder =
                        (themeId != 0)
                        ? new AndroidJavaObject("android.app.AlertDialog$Builder", activity, themeId)
                        : new AndroidJavaObject("android.app.AlertDialog$Builder", activity);

                    if (!string.IsNullOrEmpty(title))   builder.Call<AndroidJavaObject>("setTitle", title);
                    if (!string.IsNullOrEmpty(message)) builder.Call<AndroidJavaObject>("setMessage", message);
                    builder.Call<AndroidJavaObject>("setCancelable", false);

                    // Map 1–3 buttons
                    if (count == 0)
                    {
                        builder.Call<AndroidJavaObject>("setPositiveButton", "OK", new OnClickProxy(id, 0));
                    }
                    else if (count == 1)
                    {
                        builder.Call<AndroidJavaObject>("setPositiveButton", BtnText(buttons, 0), new OnClickProxy(id, 0));
                    }
                    else if (count == 2)
                    {
                        builder.Call<AndroidJavaObject>("setPositiveButton", BtnText(buttons, 1), new OnClickProxy(id, 1));
                        builder.Call<AndroidJavaObject>("setNegativeButton", BtnText(buttons, 0), new OnClickProxy(id, 0));
                    }
                    else // 3
                    {
                        builder.Call<AndroidJavaObject>("setPositiveButton", BtnText(buttons, 0), new OnClickProxy(id, 0));
                        builder.Call<AndroidJavaObject>("setNegativeButton", BtnText(buttons, 1), new OnClickProxy(id, 1));
                        builder.Call<AndroidJavaObject>("setNeutralButton",  BtnText(buttons, 2), new OnClickProxy(id, 2));
                    }

                    var dialog = builder.Call<AndroidJavaObject>("create");
                    // Keep stock system look: no extra styling
                    dialog.Call("setCanceledOnTouchOutside", false);
                    dialog.Call("show");
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    TryResolve(id, 0);
                }
            }));
        }

        /// <summary>
        /// Map explicit "Light"/"Dark" to Material/DeviceDefault alert styles (single-card native look).
        /// "System" should not call this (use ResolveSystemDialogThemeId).
        /// </summary>
        static int ResolveDialogThemeId(string themeStr)
        {
            try
            {
                using var style = new AndroidJavaClass("android.R$style");
                if (themeStr == "Light")
                {
                    int id = 0;
                    try { id = style.GetStatic<int>("Theme_Material_Light_Dialog_Alert"); } catch { }
                    if (id == 0) { try { id = style.GetStatic<int>("Theme_DeviceDefault_Light_Dialog_Alert"); } catch { } }
                    if (id == 0) { try { id = style.GetStatic<int>("Theme_Holo_Light_Dialog"); } catch { } } // last resort
                    return id;
                }
                else if (themeStr == "Dark")
                {
                    int id = 0;
                    try { id = style.GetStatic<int>("Theme_Material_Dialog_Alert"); } catch { }
                    if (id == 0) { try { id = style.GetStatic<int>("Theme_DeviceDefault_Dialog_Alert"); } catch { } }
                    if (id == 0) { try { id = style.GetStatic<int>("Theme_Holo_Dialog"); } catch { } } // last resort
                    return id;
                }
            }
            catch { }
            return 0; // System handled elsewhere
        }

        /// <summary>
        /// Decide Light/Dark from the device's current system setting (night mode) and reuse the explicit resolver.
        /// </summary>
        static int ResolveSystemDialogThemeId(AndroidJavaObject activity)
        {
            try
            {
                var res    = activity.Call<AndroidJavaObject>("getResources");
                var config = res.Call<AndroidJavaObject>("getConfiguration");
                int uiMode = config.Get<int>("uiMode");

                var Conf       = new AndroidJavaClass("android.content.res.Configuration");
                int NIGHT_MASK = Conf.GetStatic<int>("UI_MODE_NIGHT_MASK");
                int NIGHT_YES  = Conf.GetStatic<int>("UI_MODE_NIGHT_YES");

                bool isDark = (uiMode & NIGHT_MASK) == NIGHT_YES;
                return ResolveDialogThemeId(isDark ? "Dark" : "Light");
            }
            catch
            {
                return 0; // fallback: use Activity theme
            }
        }

        static string BtnText(AndroidJavaObject arr, int i)
        {
            var obj = arr.Call<AndroidJavaObject>("getJSONObject", i);
            return obj.Call<string>("optString", "text", "OK");
        }
#endif // UNITY_ANDROID && !UNITY_EDITOR
    }
}
