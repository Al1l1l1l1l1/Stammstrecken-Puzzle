using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using STP.Infrastructure.Persistence;

namespace STP.Tests.Persistence.EditMode
{
    public sealed class SaveMigrationTests
    {
        public static IEnumerable<string> FixedNegativeNames()=>LocalSaveContractFixtures.Bundle.Get("negativeCases").Items.Select(c=>c.Get("name").Text).ToArray();
        [TestCase("V1_FULL_FIRST_CLEAR")]
        [TestCase("V1_FULL_RETAINED_LEGACY")]
        public void LegacyWithoutWagonsMigratesWithExactInventorySelectionAndBalance(string name)
        {
            var fixture = LocalSaveContractFixtures.Case(name);
            byte[] source = Encoding.UTF8.GetBytes(fixture.Get("sourceEnvelopeUtf8").Text); var original = (byte[])source.Clone();
            var codec = LocalSaveContractFixtures.CodecFor(fixture); var decoded = codec.Decode(source);
            Assert.That(decoded.Status, Is.EqualTo(LocalSaveDecodeStatus.Valid), decoded.ErrorCode);
            Assert.That(decoded.SourceSchemaVersion, Is.EqualTo(1));
            var snapshot = LocalSaveContractFixtures.Property(decoded, "Snapshot")!;
            var state = LocalSaveContractFixtures.Property(snapshot, "Payload")!;
            var inventory = LocalSaveContractFixtures.Property(state, "Inventory")!;
            var records = ((System.Collections.IEnumerable)LocalSaveContractFixtures.Property(inventory, "Ownership")!).Cast<object>().ToArray();
            Assert.That(records.All(o => (string)LocalSaveContractFixtures.Property(o, "ItemType")! != "WAGON"), Is.True);
            byte[] encoded = LocalSaveContractFixtures.Encode(codec, snapshot);
            var target = FixtureJson.Parse(Encoding.UTF8.GetString(encoded)).Get("payload");
            var old = FixtureJson.Parse(fixture.Get("sourcePayloadUtf8").Text);
            Assert.That(target.Get("inventory").Canonical(), Is.EqualTo(old.Get("inventory").Canonical()));
            Assert.That(target.Get("ledgerCheckpoint").Get("balancePatience").Integer, Is.EqualTo(old.Get("ledgerCheckpoint").Get("balancePatience").Integer));
            Assert.That(source, Is.EqualTo(original));
        }
        [Test]
        public void SchemaOneLegacyReaderDoesNotInventHistoricalWagonSupport()
        {
            Assert.DoesNotThrow(() => new RetainedLegacyOwnershipReader("legacy-test", 1, new[] { "OPERATING_OBJECT", "LIVERY", "TRAIN_VARIANT" }));
            Assert.That(Assert.Throws<ArgumentException>(() => new RetainedLegacyOwnershipReader("legacy-test", 1, new[] { "WAGON" }))!.Message, Is.EqualTo("SAVE_LEGACY_READER_UNSUPPORTED"));
        }
        [TestCaseSource(nameof(FixedNegativeNames))]
        public void EveryFixedNegative_StopsAtItsRealReaderOrBindingBoundaryWithoutAReplacement(string name)
        {
            var c=LocalSaveContractFixtures.Bundle.Get("negativeCases").Items.Single(x=>x.Get("name").Text==name);var source=Encoding.UTF8.GetBytes((c.Maybe("sourceEnvelopeUtf8")??c.Get("sourceFixtureUtf8")).Text);var original=(byte[])source.Clone();string status;string code;object? replacement=null;
            try{var codec=c.Maybe("sourceFixtureUtf8")!=null?LocalSaveContractFixtures.Codec():LocalSaveContractFixtures.CodecFor(c);var result=codec.Decode(source);status=result.Status.ToString();code=result.ErrorCode??"NONE";replacement=LocalSaveContractFixtures.Property(result,"Snapshot");}
            catch(Exception e)
            {
                while(e is TargetInvocationException&&e.InnerException!=null)e=e.InnerException;
                // Real immutable source-binding factories fail before codec registration. This boundary is distinct from a payload decode.
                if(e.GetType().FullName=="STP.Infrastructure.Persistence.SaveCodecFailure"){status=(bool)e.GetType().GetProperty("Unsupported",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(e)!?"Unsupported":"Corrupt";code=e.Message;}
                else if(e is ArgumentException){status="Corrupt";code=e.Message.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)[0];}
                else throw;
            }
            Assert.That(status,Is.EqualTo(c.Get("expectedStatus").Text),name+":"+code);Assert.That(code,Is.EqualTo(c.Get("expectedErrorCode").Text),name);Assert.That(replacement,Is.Null);Assert.That(source,Is.EqualTo(original));
        }
    }
}
