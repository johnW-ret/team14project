using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using TeamFourteen;

namespace Tests
{
    public class Example
    {
        // A Test behaves as an ordinary method
        [Test]
        public void ExampleSimplePasses()
        {
            // Use the Assert class to test conditions
            Assert.AreEqual(ExampleScript.AddIntegers(5, 6), 11);
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator ExampleWithEnumeratorPasses()
        {
            ExampleScript exampleScript = new GameObject().AddComponent<ExampleScript>();

            Assert.AreEqual(exampleScript.PrintHelloWorld(), "Hello World!");

            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
