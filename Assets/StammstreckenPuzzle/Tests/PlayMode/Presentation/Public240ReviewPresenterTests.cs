using System;
using NUnit.Framework;
using STP.Presentation.UI;
using UnityEngine;

namespace STP.Tests.Presentation.PlayMode
{
    /// <summary>Presentation-only contracts without an extra Application/Domain assembly edge.</summary>
    public sealed class Public240ReviewPresenterTests
    {
        [TestCase(4, 4)]
        [TestCase(10, 10)]
        public void Grid_ExposesEveryStableFocusableCell_AndKeepsOverlayInBoard(int width, int height)
        {
            var grid = new Public240ReviewGrid(width, height);
            Assert.That(grid.GridWidth, Is.EqualTo(width));
            Assert.That(grid.GridHeight, Is.EqualTo(height));
            for (var y = 0; y < height; y++)
                for (var x = 0; x < width; x++)
                {
                    var button = grid.CellButton(x, y);
                    Assert.That(button.name, Is.EqualTo("qa-cell-" + x + "-" + y));
                    Assert.That(button.focusable, Is.True);
                    Assert.That(grid.IsCellSelected(x, y), Is.False);
                    Assert.That(button.parent, Is.SameAs(grid.Board));
                }
            grid.SetTrainTexture(null);
            Assert.Throws<ArgumentOutOfRangeException>(() => grid.CellButton(width, 0));
            grid.Dispose();
            grid.Dispose();
        }

        [Test]
        public void Grid_RejectsInvalidDimensions_InsteadOfCreatingFallbackBoard()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Public240ReviewGrid(1, 4));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Public240ReviewGrid(4, 33));
        }

        [Test]
        public void OwnStringTable_UsesGermanFormatting_RejectsMissingKey_AndDisposesIdempotently()
        {
            var source = new TextAsset("{\"locale\":\"de-DE\",\"entries\":[{\"key\":\"qa.public240.number\",\"value\":\"{0:N1}\",\"argumentTypes\":[\"number\"]}]}");
            var locale = new Public240ReviewLocalization(source);
            try
            {
                Assert.That(locale.Resolve("qa.public240.number", 1234.5), Is.EqualTo("1.234,5"));
                Assert.Throws<InvalidOperationException>(() => locale.Resolve("qa.public240.missing"));
                Assert.Throws<ArgumentException>(() => locale.Resolve("qa.public240.number", "1234"));
                Assert.Throws<ArgumentException>(() => locale.Resolve("qa.public240.number"));
                locale.Dispose();
                locale.Dispose();
                Assert.Throws<ObjectDisposedException>(() => locale.Resolve("qa.public240.number", 1));
            }
            finally
            {
                locale.Dispose();
                UnityEngine.Object.DestroyImmediate(source);
            }
        }

        [TestCase("{\"locale\":\"en-US\",\"entries\":[{\"key\":\"qa.public240.a\",\"value\":\"a\",\"argumentTypes\":[]}]}")]
        [TestCase("{\"locale\":\"de-DE\",\"entries\":[{\"key\":\"qa.public240.a\",\"value\":\"a\",\"argumentTypes\":[]},{\"key\":\"qa.public240.a\",\"value\":\"b\",\"argumentTypes\":[]}]}")]
        [TestCase("{\"locale\":\"de-DE\",\"entries\":[{\"key\":\"product.a\",\"value\":\"a\",\"argumentTypes\":[]}]}")]
        public void OwnStringTable_RejectsWrongLocale_DuplicateKey_AndForeignNamespace(string json)
        {
            var source = new TextAsset(json);
            try { Assert.Throws<ArgumentException>(() => new Public240ReviewLocalization(source)); }
            finally { UnityEngine.Object.DestroyImmediate(source); }
        }

        [TestCase("{0}", "[]")]
        [TestCase("{1}", "[\"integer\"]")]
        [TestCase("{0} {", "[\"integer\"]")]
        [TestCase("{0:00}", "[\"string\"]")]
        [TestCase("constant", "[\"integer\"]")]
        [TestCase("{0}", "[\"unknown\"]")]
        public void OwnStringTable_RejectsMissingMalformedOrUntypedFormatContract(string value, string argumentTypes)
        {
            var source = new TextAsset("{\"locale\":\"de-DE\",\"entries\":[{\"key\":\"qa.public240.a\",\"value\":\""
                + value + "\",\"argumentTypes\":" + argumentTypes + "}]}");
            try { Assert.Throws<ArgumentException>(() => new Public240ReviewLocalization(source)); }
            finally { UnityEngine.Object.DestroyImmediate(source); }
        }
    }
}
