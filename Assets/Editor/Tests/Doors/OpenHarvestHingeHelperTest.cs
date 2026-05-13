using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class OpenHarvestHingeHelperTest
    {
        private GameObject _go;
        private Rigidbody _rigidbody;
        private OpenHarvestHingeHelper _helper;

        [SetUp]
        public void SetUp()
        {
            _go = new GameObject("HingeHelperTest");
            _rigidbody = _go.AddComponent<Rigidbody>();
            _rigidbody.isKinematic = true;
            _helper = _go.AddComponent<OpenHarvestHingeHelper>();
        }

        [Test]
        public void Update_DoesNotProcessWhenIdleAndNotTouched()
        {
            _go.transform.localEulerAngles = new Vector3(0f, 90f, 0f);
            InvokePrivateMethod(_helper, "Update");

            float lastDegrees = (float)GetPrivateField(_helper, "lastDegrees");
            Assert.AreEqual(0f, lastDegrees);
        }

        [Test]
        public void Update_ProcessesWhenTouched()
        {
            _go.transform.localEulerAngles = new Vector3(0f, 90f, 0f);
            SetPrivateField(_helper, "touchingCount", 1);
            InvokePrivateMethod(_helper, "Update");

            float lastDegrees = (float)GetPrivateField(_helper, "lastDegrees");
            Assert.AreEqual(90f, lastDegrees);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_go);
        }

        private static void InvokePrivateMethod(object target, string methodName)
        {
            MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(method, $"Missing method {methodName}");
            method.Invoke(target, null);
        }

        private static object GetPrivateField(object target, string fieldName)
        {
            FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(field, $"Missing field {fieldName}");
            return field.GetValue(target);
        }

        private static void SetPrivateField(object target, string fieldName, object value)
        {
            FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(field, $"Missing field {fieldName}");
            field.SetValue(target, value);
        }
    }
}
