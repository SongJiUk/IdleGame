// Assets/way2tushar/NativeAlerts/Samples~/AlertDemo.cs
using UnityEngine;

namespace way2tushar.NativeAlerts
{

    public class AlertDemo : MonoBehaviour
    {

        public async void OnClickSimpleOkAlertBtn()
        {
            await NativeAlert.ShowAsync(new AlertOptions
            {
                title = "Hello!",
                message = "Welcome to NativeAlerts!",
                theme = AlertTheme.Light,
                buttons = new() { new() { text = "OK" } }
            });

        }

        public async void OnClickConfirmationDialogBtn()
        {
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
        }

        public async void OnClickThemingBtn()
        {
            await NativeAlert.ShowAsync(new AlertOptions
            {
                title = "Theme Test",
                message = "This is the Dark theme preview.",
                theme = AlertTheme.Dark,
                buttons = new() { new() { text = "OK" } }
            });
        }

        public async void OnClickComplexWorkflowBtn()
        {
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
        }
    }
}