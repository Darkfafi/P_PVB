using System.Collections.Generic;
using UnityEngine;
using Ramses.Confactory;
using Ramses.Confactory.Addons;
using System;

namespace Team.CodeConventions
{
    /// <summary>
    /// This class is a template example of the code conventions. 
    /// The order of the different parts and naming rules can be found here.
    /// </summary>
    public class CodeConventions : MonoBehaviour, IMyInterface
    {
        public enum Enum
        {
            FirstEnumItem,
            SecondEnumItem
        }

        // RULE: End delegates always with 'Handler' and events always with 'Event'
        public delegate void MyClassHandler(int firstParam, float secondParam);
        public event MyClassHandler ActionEvent;

        public const string CONSTANT_STRING = "ConstantString";

        public static string PublicStaticString = "StaticString";

        // RULE: Booleans always true false answerable
        public bool IsPublicBool;
        public bool HasPublicBool;

        public float PublicFloat;

        protected List<float> protectedListOfFloats;
        protected float[] protectedFloatArray;
        protected bool isProtectedBool;

        [SerializeField]
        private int _mySerializedFieldInt;
        private bool _isPrivateBool;

        /// <summary>
        /// This is my public static void method summery
        /// </summary>
        /// <param name="firstParameter"> secondParameter does that..</param>
        /// <param name="secondParameter"> secondParameter does that..</param>
        public static void PublicStaticVoid(string firstParameter, int secondParameter)
        {

        }

        public void FirstVoidMethod()
        {

        }

        public float SecondFloatMethod(int firstParameter)
        {
            return (float)firstParameter;
        }

        /// <summary>
        /// This is my public generic float method summery
        /// </summary>
        /// <typeparam name="T"> A class of the type 'IMyInterface' </typeparam>
        /// <param name="classFromInterface">The parameter represents</param>
        /// <returns>the method returns the float of.</returns>
        public float PublicGenericFloatMethod<T>(T classFromInterface) where T : class, IMyInterface
        {
            IMyInterface myClassInterace = classFromInterface;
            return myClassInterace.SecondFloatMethod(5);
        }

        public void PublicMethodVoid(int firstParameter, bool secondParameter)
        {

        }

        protected static void ProtectedStaticMethodVoid()
        {

        }

        protected void ProtectedMethodVoid(int firstParameter, params float[] secondParamsParameter)
        {

        }

        // RULE: Make Unity methods always protected. 
        protected void Awake()
        {
            ActionEvent += OnActionEvent;
        }

        protected void OnDestroy()
        {
            ActionEvent -= OnActionEvent;
        }


        private static void PrivateStaticMethodVoid()
        {

        }

        // RULE: Methods triggered on events are named 'On[eventName]' as seen below
        private void OnActionEvent(int firstParam, float secondParam)
        {

        }

        private void PrivateMethodVoid()
        {

        }
    }

    // OTHER EXAMPLES AND NON CLASS EXAMPLES

    public struct MyStruct
    {
        public float FloatInStruct;
    }

    // RULE: Interface names always start with 'I'
    public interface IMyInterface
    {
        void FirstVoidMethod();
        float SecondFloatMethod(int firstParameter);
    }

    // RULE: Abstract classes always start with the term 'Base'
    public abstract class BaseClass : IMyInterface
    {
        protected float protectedFloat;
        private float _privateFloat;

        public abstract void FirstVoidMethod();
        public abstract float SecondFloatMethod(int firstParameter);
    }

    // << UTILS Concentions && Small Examples >>



    // RULE: Pop-ups always end with the terms 'PopUp'
    public class MyPopUp : BasePopUp
    {
        protected override void Open()
        {

        }
    }

    // RULE: Confactory classes always start with the term 'Con'
    public class ConClass : IConfactory
    {
        public void ConClear()
        {

        }
    }

    public class ConClassSecond : MonoBehaviour, IConfactory
    {
        public void ConClear()
        {

        }
    }

    public class ConClassWithAddons : IConfactory, IConStructUser, IConSceneSwitchUser
    {
        public void ConClear()
        {

        }

        public IConfactory ConStruct(IConfactoryFinder confactoryFinder)
        {
#if IOS
            // return   [IOS version of class].
#elif ANDROID
            // return   [Android version of class].
#else
            return null; // return this version of class.
#endif
        }

        public void OnConSceneSwitched(string oldScene, string newScene)
        {
            
        }
    }
}