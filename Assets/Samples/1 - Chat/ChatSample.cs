using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Gpt4All.Samples
{
    public class ChatSample : MonoBehaviour
    {
        public LlmManager manager;
    
        [Header("UI")]
        public InputField input;
        public Text output;
        public ScrollRect scroll;
        public Button submit;

        private string _previousText;

        private void Awake()
        {
            input.onEndEdit.AddListener(OnSubmit);
            submit.onClick.AddListener(OnSubmitPressed);
            manager.OnResponseUpdated += OnResponseHandler;
        }
    
        private void OnSubmit(string prompt)
        {
            if (!Input.GetKey(KeyCode.Return))
                return;
            SendToChat(input.text);
        }
    
        private void OnSubmitPressed()
        {
            SendToChat(input.text);
        }

        private async void SendToChat(string prompt)
        {
            if (string.IsNullOrEmpty(prompt))
                return;

            input.text = "";
            output.text += $"<b>User:</b> {prompt}\n<b>Answer</b>: ";
            _previousText = output.text;
            
            await manager.Prompt(prompt);
            output.text += "\n";
            ScrollDown();
        }
    
        private void OnResponseHandler(string response)
        {
            output.text = _previousText + response;
            ScrollDown();
        }

        private async void ScrollDown()
        {
            await Task.Yield();
            Canvas.ForceUpdateCanvases ();
            scroll.normalizedPosition = new Vector2(0, 0);
            Canvas.ForceUpdateCanvases ();
        }
    }
}

