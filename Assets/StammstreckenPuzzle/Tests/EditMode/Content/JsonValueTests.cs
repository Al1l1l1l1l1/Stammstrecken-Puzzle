using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using STP.Infrastructure.Content;

namespace STP.Tests.Content.EditMode
{
    /// <summary>
    /// The JSON DOM is immutable after construction: no public view may hand out the internal storage, because a mutated
    /// tree could carry duplicate member names (which the parser and <see cref="JsonValue.CreateObject"/> forbid) into
    /// canonicalisation and hashing.
    /// </summary>
    public sealed class JsonValueTests
    {
        private static JsonValue Obj() => StrictJsonParser.ParseText("{\"a\":1,\"b\":[1,2,3],\"c\":{\"x\":true}}").Value!;

        /// <summary>The view must not be the backing array/list and must reject every mutating operation.</summary>
        internal static void AssertReadOnlyView<T>(IReadOnlyList<T> view, T replacement, string context)
        {
            Assert.That(view, Is.Not.InstanceOf<Array>(), context + ": the backing array must not be exposed");
            Assert.That(view.GetType().IsGenericType && view.GetType().GetGenericTypeDefinition() == typeof(List<>), Is.False, context + ": the backing list must not be exposed");
            if (view is IList<T> list)
            {
                if (list.Count > 0)
                {
                    Assert.Throws<NotSupportedException>(() => list[0] = replacement, context + ": indexer set");
                    Assert.Throws<NotSupportedException>(() => list.RemoveAt(0), context + ": RemoveAt");
                    Assert.Throws<NotSupportedException>(() => list.Clear(), context + ": Clear");
                }
                Assert.Throws<NotSupportedException>(() => list.Add(replacement), context + ": Add");
                Assert.Throws<NotSupportedException>(() => list.Insert(0, replacement), context + ": Insert");
            }
        }

        [Test]
        public void Items_AreAReadOnlyView_NotTheInternalArray()
        {
            var array = Obj().Get("b")!;
            AssertReadOnlyView<JsonValue>(array.Items, JsonValue.Null, "Items");
            Assert.That(array.Items.Select(i => i.IntegerValue).ToArray(), Is.EqualTo(new long[] { 1, 2, 3 }));
        }

        [Test]
        public void Members_AreAReadOnlyView_NotTheInternalArray()
        {
            var obj = Obj();
            AssertReadOnlyView<JsonMember>(obj.Members, new JsonMember("a", JsonValue.Null), "Members");
            Assert.That(obj.Members.Select(m => m.Name).ToArray(), Is.EqualTo(new[] { "a", "b", "c" }));
        }

        [Test]
        public void TheEmptyViews_AreReadOnlyToo_ForScalarsAndEmptyContainers()
        {
            AssertReadOnlyView<JsonValue>(JsonValue.CreateInteger(1).Items, JsonValue.Null, "scalar Items");
            AssertReadOnlyView<JsonMember>(JsonValue.CreateString("x").Members, new JsonMember("a", JsonValue.Null), "scalar Members");
            AssertReadOnlyView<JsonValue>(JsonValue.CreateArray(new JsonValue[0]).Items, JsonValue.Null, "empty array Items");
            AssertReadOnlyView<JsonMember>(JsonValue.CreateObject(new JsonMember[0]).Members, new JsonMember("a", JsonValue.Null), "empty object Members");
        }

        [Test]
        public void ADowncastAndMutation_CannotCreateDuplicateMemberNames_AndCannotChangeTheCanonicalForm()
        {
            var obj = JsonValue.CreateObject(new[] { new JsonMember("a", JsonValue.CreateInteger(1)), new JsonMember("b", JsonValue.CreateInteger(2)) });
            string before = JcsSerializer.Serialize(obj).Text!;
            Assert.That(before, Is.EqualTo("{\"a\":1,\"b\":2}"));

            // The original defect: (JsonMember[])obj.Members followed by an element write produced {"a":1,"a":2}.
            var asArray = obj.Members as JsonMember[];
            Assert.That(asArray, Is.Null, "Members must not be castable to the internal array");
            if (obj.Members is IList<JsonMember> list)
                Assert.Throws<NotSupportedException>(() => list[1] = new JsonMember("a", JsonValue.CreateInteger(99)));

            var items = JsonValue.CreateArray(new[] { JsonValue.CreateInteger(1), JsonValue.CreateInteger(2) });
            Assert.That(items.Items as JsonValue[], Is.Null, "Items must not be castable to the internal array");
            if (items.Items is IList<JsonValue> itemList)
                Assert.Throws<NotSupportedException>(() => itemList[0] = JsonValue.CreateInteger(99));

            Assert.That(JcsSerializer.Serialize(obj).Text, Is.EqualTo(before));
            Assert.That(JcsSerializer.Serialize(items).Text, Is.EqualTo("[1,2]"));
            Assert.That(obj.Members.Select(m => m.Name).Distinct().Count(), Is.EqualTo(obj.Members.Count), "member names stay unique");
        }

        [Test]
        public void TheFactories_CopyTheirInput_SoLaterChangesToTheSourceHaveNoEffect()
        {
            var source = new List<JsonValue> { JsonValue.CreateInteger(1) };
            var array = JsonValue.CreateArray(source);
            source.Add(JsonValue.CreateInteger(2));
            source[0] = JsonValue.CreateInteger(9);
            Assert.That(JcsSerializer.Serialize(array).Text, Is.EqualTo("[1]"));

            var members = new List<JsonMember> { new JsonMember("a", JsonValue.Null) };
            var obj = JsonValue.CreateObject(members);
            members.Add(new JsonMember("a", JsonValue.Null));
            Assert.That(JcsSerializer.Serialize(obj).Text, Is.EqualTo("{\"a\":null}"));
        }

        [Test]
        public void ReadOnlyViews_KeepIndexingEnumerationAndCountSemantics()
        {
            var array = Obj().Get("b")!;
            Assert.That(array.Items.Count, Is.EqualTo(3));
            Assert.That(array.Items[2].IntegerValue, Is.EqualTo(3));
            int seen = 0;
            foreach (var item in array.Items) seen += (int)item.IntegerValue;
            Assert.That(seen, Is.EqualTo(6));
            Assert.That(ReferenceEquals(array.Items, array.Items), Is.True, "the view is stable (no per-call allocation)");
        }
    }
}
