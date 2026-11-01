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
