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

        private void Awake()
        {
            input.onSubmit.AddListener(OnSubmit);
            submit.onClick.AddListener(OnSubmitPressed);
            manager.OnResponse += OnResponseHandler;
        }
    
        private async void OnSubmit(string prompt)
        {
            if (string.IsNullOrEmpty(prompt))
                return;

            input.text = "";
            output.text += $"<b>User:</b> {prompt}\n<b>Answer</b>: ";
            await manager.Prompt(prompt);
            output.text += "\n";
            ScrollDown();
        }
    
        private void OnSubmitPressed()
        {
            OnSubmit(input.text);
        }
    
        private void OnResponseHandler(int tokenId, string response)
        {
            output.text += response;
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

