using System;
using System.Collections.Generic;
using System.Linq;
using STP.Puzzle.Solver;

namespace STP.Application
{
    /// <summary>Exact integer conversion for already sourced thresholds; arithmetic creates no calibration approval.</summary>
    public sealed class CalibratedStarTimes
    {
        public const long MaxSeconds = 9007199254740991L / 1000;
        public CalibratedStarTimes(string calibrationVersion,long twoStarsSeconds,long threeStarsSeconds)
        {
            CalibrationVersion=LocalModel.Text(calibrationVersion);
            LocalModel.Require(twoStarsSeconds>0&&threeStarsSeconds>0&&twoStarsSeconds<=MaxSeconds&&threeStarsSeconds<twoStarsSeconds,"STAR_SECONDS_INVALID");
            TwoStarsSeconds=twoStarsSeconds;ThreeStarsSeconds=threeStarsSeconds;
            TwoStarMs=checked(twoStarsSeconds*1000);ThreeStarMs=checked(threeStarsSeconds*1000);
        }
        public string CalibrationVersion{get;}
        public long TwoStarsSeconds{get;}
        public long ThreeStarsSeconds{get;}
        public long TwoStarMs{get;}
        public long ThreeStarMs{get;}
        public int Stars(long activeElapsedMs){LocalModel.Safe(activeElapsedMs);return activeElapsedMs<=ThreeStarMs?3:activeElapsedMs<=TwoStarMs?2:1;}
    }

    /// <summary>Full source references pinned in search parameter v2 before any generation.</summary>
    public sealed class EndlessStarCalibration
    {
        public EndlessStarCalibration(CatalogIdentity completionCatalog,CatalogIdentity calibrationCatalog,string calibrationVersion,long twoStarMs,long threeStarMs)
        {
            CompletionCatalog=completionCatalog??throw new ArgumentNullException(nameof(completionCatalog));CalibrationCatalog=calibrationCatalog??throw new ArgumentNullException(nameof(calibrationCatalog));CalibrationVersion=LocalModel.Text(calibrationVersion);
            LocalModel.Require(completionCatalog.Use==calibrationCatalog.Use&&completionCatalog.ApprovalStatus==calibrationCatalog.ApprovalStatus&&completionCatalog.Use!=CatalogUse.IsolatedReview,"STAR_CATALOG_USE_MISMATCH");
            LocalModel.Positive(twoStarMs);LocalModel.Positive(threeStarMs);LocalModel.Require(threeStarMs<twoStarMs&&twoStarMs%1000==0&&threeStarMs%1000==0,"STAR_MILLISECONDS_INVALID");TwoStarMs=twoStarMs;ThreeStarMs=threeStarMs;
        }
        public CatalogIdentity CompletionCatalog{get;}
        public CatalogIdentity CalibrationCatalog{get;}
        public string CalibrationVersion{get;}
        public long TwoStarMs{get;}
        public long ThreeStarMs{get;}
        public int Stars(long elapsed){LocalModel.Safe(elapsed);return elapsed<=ThreeStarMs?3:elapsed<=TwoStarMs?2:1;}
        public bool EqualsBinding(EndlessStarCalibration other)=>other!=null&&CompletionCatalog.Equals(other.CompletionCatalog)&&CalibrationCatalog.Equals(other.CalibrationCatalog)&&CalibrationVersion==other.CalibrationVersion&&TwoStarMs==other.TwoStarMs&&ThreeStarMs==other.ThreeStarMs;
        public string Projection()=>"{\"completionCatalog\":"+LocalSaveHash.CatalogProjection(CompletionCatalog)+",\"calibrationCatalog\":"+LocalSaveHash.CatalogProjection(CalibrationCatalog)+",\"calibrationVersion\":"+LocalSaveHash.Quote(CalibrationVersion)+",\"twoStarMs\":"+TwoStarMs.ToString(System.Globalization.CultureInfo.InvariantCulture)+",\"threeStarMs\":"+ThreeStarMs.ToString(System.Globalization.CultureInfo.InvariantCulture)+"}";
    }

    /// <summary>Closed immutable metric interval; cannot relax a generator's mandatory Unique/NoGuess gate.</summary>
    public sealed class QualityInterval
    {
        public QualityInterval(long min,long max){LocalModel.Safe(min);LocalModel.Safe(max);LocalModel.Require(min<=max,"GEN_QUALITY_INTERVAL_INVALID");Min=min;Max=max;}
        public long Min{get;}
        public long Max{get;}
        public bool Contains(long value)=>value>=Min&&value<=Max;
    }

    /// <summary>All supported metric ranges are checked on the actual verified candidate before promotion.</summary>
    public sealed class EndlessQualityBounds
    {
        public EndlessQualityBounds(QualityInterval searchNodes,QualityInterval deductionSteps,QualityInterval maxDeductionDepth,QualityInterval requiredGuessDepth,QualityInterval trivialLines)
        {SearchNodes=searchNodes??throw new ArgumentNullException(nameof(searchNodes));DeductionSteps=deductionSteps??throw new ArgumentNullException(nameof(deductionSteps));MaxDeductionDepth=maxDeductionDepth??throw new ArgumentNullException(nameof(maxDeductionDepth));RequiredGuessDepth=requiredGuessDepth??throw new ArgumentNullException(nameof(requiredGuessDepth));TrivialLines=trivialLines??throw new ArgumentNullException(nameof(trivialLines));LocalModel.Require(requiredGuessDepth.Min==0&&requiredGuessDepth.Max==0,"GEN_QUALITY_GUESS_REQUIRED");}
        public QualityInterval SearchNodes{get;}
        public QualityInterval DeductionSteps{get;}
        public QualityInterval MaxDeductionDepth{get;}
        public QualityInterval RequiredGuessDepth{get;}
        public QualityInterval TrivialLines{get;}
        public bool Accepts(SolverMetrics metrics,int trivialLines)=>metrics!=null&&SearchNodes.Contains(metrics.SearchNodes)&&DeductionSteps.Contains(metrics.DeductionSteps)&&MaxDeductionDepth.Contains(metrics.MaxDeductionDepth)&&RequiredGuessDepth.Contains(metrics.RequiredGuessDepth)&&TrivialLines.Contains(trivialLines);
    }

    /// <summary>Bound source values for one calibrated profile. A DTO alone is not a production import decision.</summary>
    public sealed class CalibratedEndlessProfile
    {
        public CalibratedEndlessProfile(EndlessSearchProfile profile,int version,EndlessQualityBounds quality,EndlessStarCalibration calibration)
        {Profile=profile??throw new ArgumentNullException(nameof(profile));Quality=quality??throw new ArgumentNullException(nameof(quality));StarCalibration=calibration??throw new ArgumentNullException(nameof(calibration));LocalModel.Require(version>0,"GEN_PROFILE_VERSION_INVALID");Version=version;}
        public EndlessSearchProfile Profile{get;}
        public string Id=>Profile.Id;
        public string HashSha256=>Profile.HashSha256;
        public int Version{get;}
        public PuzzleCandidateParameters Search=>Profile.Search;
        public EndlessQualityBounds Quality{get;}
        public EndlessStarCalibration StarCalibration{get;}
    }

    /// <summary>Existing four exact ENDLESS_INSTANCE reward values from a bound runtime completion source.</summary>
    public sealed class EndlessCompletionValues
    {
        public EndlessCompletionValues(CatalogIdentity catalog,int firstClear,int secondStar,int thirdStar,int postClear)
        {Catalog=catalog??throw new ArgumentNullException(nameof(catalog));LocalModel.Require(firstClear==15&&secondStar==5&&thirdStar==10&&postClear==10,"ENDLESS_COMPLETION_VALUES_INVALID");FirstClear=firstClear;SecondStar=secondStar;ThirdStar=thirdStar;PostClear=postClear;}
        public CatalogIdentity Catalog{get;}
        public int FirstClear{get;}
        public int SecondStar{get;}
        public int ThirdStar{get;}
        public int PostClear{get;}
    }

    /// <summary>Bounded open-completion result facts; never used to resurrect terminal membership or a grant.</summary>
    public sealed class LocalEndlessResult
    {
        public LocalEndlessResult(PublicPuzzleIdentity identity,Guid attemptId,long activeElapsedMs,int stars,EconomyTerminal firstClearTerminal,EconomyTerminal? secondStarPaid,EconomyTerminal? thirdStarPaid)
        {
            Identity=identity??throw new ArgumentNullException(nameof(identity));FirstClearTerminal=firstClearTerminal??throw new ArgumentNullException(nameof(firstClearTerminal));AttemptId=attemptId;ActiveElapsedMs=LocalModel.Safe(activeElapsedMs);Stars=stars;SecondStarPaid=secondStarPaid;ThirdStarPaid=thirdStarPaid;
            LocalModel.Require(identity.PuzzleId.StartsWith("E1-",StringComparison.Ordinal)&&attemptId!=Guid.Empty&&stars>=1&&stars<=3&&(secondStarPaid!=null)==(stars>=2)&&(thirdStarPaid!=null)==(stars==3),"ENDLESS_RESULT_INVALID");
            Check(firstClearTerminal,"LEVEL_FIRST_CLEAR","first-clear",15);if(secondStarPaid!=null)Check(secondStarPaid,"LEVEL_SECOND_STAR","second-star",5);if(thirdStarPaid!=null)Check(thirdStarPaid,"LEVEL_THIRD_STAR","third-star",10);
        }
        private void Check(EconomyTerminal terminal,string reason,string suffix,int amount)
        {LocalModel.Require(terminal.SubjectId==Identity.PuzzleId&&terminal.ReasonCode==reason&&terminal.TransactionId=="level:"+Identity.PuzzleId+":"+suffix&&terminal.ClaimId=="reward:"+reason+":"+Identity.PuzzleId&&terminal.Amount==amount&&terminal.Catalog.Equals(FirstClearTerminal.Catalog)&&terminal.CommittedGeneration>0&&terminal.CommittedGeneration==FirstClearTerminal.CommittedGeneration&&terminal.CreatedAtUtc==FirstClearTerminal.CreatedAtUtc,"ENDLESS_RESULT_TERMINAL_INVALID");}
        public PublicPuzzleIdentity Identity{get;}
        public Guid AttemptId{get;}
        public long ActiveElapsedMs{get;}
        public int Stars{get;}
        public EconomyTerminal FirstClearTerminal{get;}
        public EconomyTerminal? SecondStarPaid{get;}
        public EconomyTerminal? ThirdStarPaid{get;}
        public IReadOnlyList<EconomyTerminal> Terminals=>Array.AsReadOnly(new[]{FirstClearTerminal,SecondStarPaid,ThirdStarPaid}.Where(t=>t!=null).Cast<EconomyTerminal>().ToArray());
    }
}
