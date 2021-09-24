using UnityEngine;

namespace TeamFourteen
{
    public class ExampleScript : MonoBehaviour
    {
        private readonly string helloWorldText = "Hello World!";
        [ContextMenu("Print Hello World")]
        public string PrintHelloWorld()
        {
            Debug.Log(helloWorldText);
            return helloWorldText;
        }

        public static int AddIntegers(int a, int b) => a + b;
    }
}
