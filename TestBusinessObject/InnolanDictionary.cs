using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestBusinessObject
{
    public class InnolanDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        private void SetValue(TKey key, TValue value)
        {
            Add(key, value);
        }

        protected TValue GetValue(TKey key)
        {
            var value = default(TValue);
            return ContainsKey(key) ? TryGetValue(key, out value) ? value : value : value;
        }

        public new TValue this[TKey key]
        {
            get { return GetValue(key); }
            set { SetValue(key, value); }
        }

        public new void Add(TKey key, TValue value)
        {
            if (ContainsKey(key))
                base[key] = value;
            else
                base.Add(key, value);
        }
    }


    public class InnolanCollection<T> : Collection<T>
    {
        public Action<T> OnValueAddedToCollection;

        public new void Add(T item)
        {
            base.Add(item);
            if (OnValueAddedToCollection != null)
                OnValueAddedToCollection.Invoke(item);
        }
    }

    [TestClass]
    public class TestInnolanCollection
    {
        [TestMethod]
        public void AddToCollection_TriggerAction_AreEqual()
        {
            var collection = new InnolanCollection<string>();
            var result = string.Empty;
            collection.OnValueAddedToCollection = delegate(string s) { result = s; };
            collection.Add("New string");
            Assert.AreEqual("New string", result);
            Assert.AreEqual(1, collection.Count);
        }
    }

    [TestClass]
    public class TestInnolanDictionary
    {
        [TestMethod]
        public void GetIndexValue_NewDictionary_IsNull()
        {
            var alureDictionary = new InnolanDictionary<string, object>();
            var @object = alureDictionary["test"];
            Assert.IsNull(@object);
        }

        [TestMethod]
        public void SetIndexValue_NewDictionary_AreEqual()
        {
            var alureDictionary = new InnolanDictionary<string, object>();
            alureDictionary["test"] = 1;
            Assert.AreEqual(1, alureDictionary["test"]);
        }

        [TestMethod]
        public void SetIndexValue_ExistingValue_AreEqual()
        {
            var alureDictionary = new InnolanDictionary<string, object> { { "test", 1 } };
            alureDictionary["test"] = 2;
            Assert.AreEqual(2, alureDictionary["test"]);
        }

        [TestMethod]
        public void AddValue_NewDictionary_AreEqual()
        {
            var alureDictionary = new InnolanDictionary<string, object>();
            alureDictionary.Add("test", 1);
            Assert.AreEqual(1, alureDictionary["test"]);
        }

        [TestMethod]
        public void AddValue_ExistingValue_AreEqual()
        {
            var alureDictionary = new InnolanDictionary<string, object> {{"test", 1}, {"test", 2}};
            Assert.AreEqual(2, alureDictionary["test"]);
        }
    }
}