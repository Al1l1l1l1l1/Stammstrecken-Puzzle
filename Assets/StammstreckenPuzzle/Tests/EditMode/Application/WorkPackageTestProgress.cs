using System;
using System.IO;
using NUnit.Framework.Interfaces;
using UnityEngine.TestRunner;

[assembly: TestRunCallback(typeof(STP.Tests.Application.EditMode.WorkPackageTestProgress))]

namespace STP.Tests.Application.EditMode
{
    /// <summary>Opt-in observation of unfinished tests. Never changes scheduling, assertions or test results.</summary>
    public sealed class WorkPackageTestProgress : ITestRunCallback
    {
        private static void Write(string status, string name)
        {
            string? path = Environment.GetEnvironmentVariable("STP_TEST_PROGRESS_FILE");
            if (string.IsNullOrEmpty(path)) return;
            File.AppendAllText(path, DateTime.UtcNow.ToString("O") + " " + status + " " + name + Environment.NewLine);
        }
        public void RunStarted(ITest testsToRun) => Write("RUN_START", testsToRun.FullName);
        public void RunFinished(ITestResult testResults) => Write("RUN_FINISH", testResults.ResultState.ToString());
        public void TestStarted(ITest test) { if (!test.IsSuite) Write("START", test.FullName); }
        public void TestFinished(ITestResult result) { if (!result.Test.IsSuite) Write(result.ResultState.ToString(), result.Test.FullName); }
    }
}
