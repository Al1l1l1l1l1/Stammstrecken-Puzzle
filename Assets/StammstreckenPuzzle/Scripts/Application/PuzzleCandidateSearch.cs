using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using STP.Puzzle.Domain;
using STP.Puzzle.Solver;

namespace STP.Application
{
    /// <summary>Explicit bounded parameters. These are authoring filters, never human product approval.</summary>
    public sealed class PuzzleCandidateParameters
    {
        public PuzzleCandidateParameters(int minSize, int maxSize, int minDensityPercent, int maxDensityPercent,
            int minDeductionSteps, int minDeductionDepth, int maxAttempts, int pathStateBudget, int totalBudgetMs,
            int solverBudgetMs, int maxTrivialLines, int recentSimilarityPercent)
        {
            if (minSize < 4 || maxSize > 10 || minSize > maxSize || minDensityPercent < 10 ||
                maxDensityPercent > 90 || minDensityPercent > maxDensityPercent || minDeductionSteps < 0 ||
                minDeductionDepth < 0 || maxAttempts < 1 || maxAttempts > 10000 || pathStateBudget < 1 ||
                totalBudgetMs < 1 || totalBudgetMs > 60000 || solverBudgetMs < 1 || solverBudgetMs > totalBudgetMs ||
                maxTrivialLines < 0 || recentSimilarityPercent < 0 || recentSimilarityPercent > 100)
                throw new ArgumentOutOfRangeException(nameof(minSize));
            MinSize = minSize; MaxSize = maxSize; MinDensityPercent = minDensityPercent; MaxDensityPercent = maxDensityPercent;
            MinDeductionSteps = minDeductionSteps; MinDeductionDepth = minDeductionDepth; MaxAttempts = maxAttempts;
            PathStateBudget = pathStateBudget; TotalBudgetMs = totalBudgetMs; SolverBudgetMs = solverBudgetMs;
            MaxTrivialLines = maxTrivialLines; RecentSimilarityPercent = recentSimilarityPercent;
        }
        public int MinSize { get; }
        public int MaxSize { get; }
        public int MinDensityPercent { get; }
        public int MaxDensityPercent { get; }
        public int MinDeductionSteps { get; }
        public int MinDeductionDepth { get; }
        public int MaxAttempts { get; }
        public int PathStateBudget { get; }
        public int TotalBudgetMs { get; }
        public int SolverBudgetMs { get; }
        public int MaxTrivialLines { get; }
        public int RecentSimilarityPercent { get; }
    }

    /// <summary>A separately solved public candidate; not an issued endless identity, proof or release approval.</summary>
    public sealed class PuzzleCandidateResult
    {
        internal PuzzleCandidateResult(PuzzleDefinition? puzzle, SolverResult? verification, int attempts, string status)
        { Puzzle = puzzle; Verification = verification; Attempts = attempts; Status = status; }
        public PuzzleDefinition? Puzzle { get; }
        public SolverResult? Verification { get; }
        public int Attempts { get; }
        public string Status { get; }
        public bool Succeeded => Puzzle != null;
    }

    /// <summary>Versioned, provider-free background authoring/runtime search. Issuance remains the save authority.</summary>
    public static class PuzzleCandidateSearch
    {
        public const string GeneratorVersion = "path-search-v1";
        public const string NoveltyVersion = "public-d4-l1-v1";

        public static Task<PuzzleCandidateResult> GenerateAsync(ulong seed, PuzzleCandidateParameters parameters,
            IReadOnlyList<PuzzleDefinition> campaign, IReadOnlyList<PuzzleDefinition> recent, CancellationToken cancellation,
            Func<long>? monotonicNow = null, EndlessQualityBounds? quality = null)
        {
            if (parameters == null || campaign == null || recent == null) throw new ArgumentNullException();
            var reference = campaign.ToArray(); var history = recent.Skip(Math.Max(0, recent.Count - 50)).ToArray();
            if (reference.Any(p => p == null) || history.Any(p => p == null)) throw new ArgumentException("GEN_NULL_REFERENCE");
            return Task.Run(() => Search(seed, parameters, reference, history, cancellation, monotonicNow, quality), cancellation);
        }

        private static PuzzleCandidateResult Search(ulong seed, PuzzleCandidateParameters p, PuzzleDefinition[] campaign,
            PuzzleDefinition[] recent, CancellationToken cancellation, Func<long>? monotonicNow, EndlessQualityBounds? quality)
        {
            var time = Stopwatch.StartNew();
            Func<long> now=monotonicNow??(()=>time.ElapsedMilliseconds);
            long start=now(), last=start;
            long Elapsed()
            {
                long current=now();
                if(start<0||current<last)return long.MaxValue;
                last=current;return current-start;
            }
            var random = new RandomSequence(seed);
            var seen = new HashSet<string>(campaign.Concat(recent).Select(CanonicalSignature), StringComparer.Ordinal);
            var templates=campaign.Where(q=>q.Grid.Width>=p.MinSize&&q.Grid.Width<=p.MaxSize&&q.Grid.Height>=p.MinSize&&q.Grid.Height<=p.MaxSize).ToArray();
            var templatePaths=new Dictionary<PuzzleDefinition,int[]>();
            int attempts = 0;
            while (attempts < p.MaxAttempts && Elapsed() < p.TotalBudgetMs)
            {
                cancellation.ThrowIfCancellationRequested(); attempts++;
                GridSize grid;Endpoint a,b;List<int>? path;
                if(p.MinSize>=8&&(attempts&1)==1&&templates.Length>0)
                {
                    var source=templates[random.Next(0,templates.Length)];grid=source.Grid;a=source.A;b=source.B;
                    if(!templatePaths.TryGetValue(source,out var original))
                    {
                        long remaining=p.TotalBudgetMs-Elapsed();if(remaining<=0)break;
                        var sourceProof=PuzzleSolver.Solve(source,new SolverLimits(1000000,Math.Min(p.SolverBudgetMs,remaining),cancellation,now));
                        cancellation.ThrowIfCancellationRequested();
                        if(sourceProof.Classification==SolverClassification.INDETERMINATE)return new PuzzleCandidateResult(null,null,attempts,"GEN_VERIFICATION_INDETERMINATE");
                        if(sourceProof.Classification!=SolverClassification.UNIQUE||sourceProof.Metrics!.RequiredGuessDepth!=0)continue;
                        original=sourceProof.Path.Select(c=>c.Y*grid.Width+c.X).ToArray();templatePaths.Add(source,original);
                    }
                    path=MutatePublicPath(original,grid,ref random);
                    if(path.Count<(grid.CellCount*p.MinDensityPercent+99)/100||path.Count>grid.CellCount*p.MaxDensityPercent/100)continue;
                }
                else
                {
                    int chosenWidth=random.Next(p.MinSize,p.MaxSize+1),chosenHeight=random.Next(p.MinSize,p.MaxSize+1);
                    grid=GridSize.Create(chosenWidth,chosenHeight).Value!;
                    int aSide=random.Next(0,4),aIndex=random.Next(0,(aSide%2==0?chosenWidth:chosenHeight));
                    int bSide=random.Next(0,4),bIndex=random.Next(0,(bSide%2==0?chosenWidth:chosenHeight));
                    a=Endpoint.Create(grid,(Direction)aSide,aIndex).Value!;b=Endpoint.Create(grid,(Direction)bSide,bIndex).Value!;
                    if(a.Cell.X==b.Cell.X&&a.Cell.Y==b.Cell.Y)continue;
                    int minimum=Math.Max(4,(grid.CellCount*p.MinDensityPercent+99)/100),maximum=grid.CellCount*p.MaxDensityPercent/100;
                    int desired=random.Next(minimum,maximum+1);
                    path=TryPath(grid,a.Cell,b.Cell,desired,maximum,p.PathStateBudget,ref random,cancellation,()=>Elapsed()>=p.TotalBudgetMs);
                }
                int width=grid.Width,height=grid.Height;
                if (path == null) continue;
                var rows = new int[height]; var columns = new int[width];
                foreach (int cell in path) { rows[cell / width]++; columns[cell % width]++; }
                int trivial = rows.Count(n => n <= 1 || n == width) + columns.Count(n => n <= 1 || n == height);
                if (trivial > p.MaxTrivialLines) continue;
                var puzzle = PuzzleDefinition.Create(grid, a, b, rows, columns, "train-track-v1").Value;
                if (puzzle == null || seen.Contains(CanonicalSignature(puzzle))) continue;
                if (recent.Any(old => SimilarityDistancePercent(puzzle, old) <= p.RecentSimilarityPercent)) continue;
                long left = p.TotalBudgetMs - Elapsed();
                if (left <= 0) break;
                var screen=PuzzleSolver.ScreenForRootSolution(puzzle,new SolverLimits(1000000,Math.Min(p.SolverBudgetMs,left),cancellation,now));
                cancellation.ThrowIfCancellationRequested();
                if(screen==RootSolutionScreen.Interrupted)return new PuzzleCandidateResult(null,null,attempts,"GEN_VERIFICATION_INDETERMINATE");
                if(screen==RootSolutionScreen.Rejected)continue;
                left=p.TotalBudgetMs-Elapsed();if(left<=0)break;
                // Verification receives only public inputs. The sampled path is neither supplied nor trusted.
                var verified = PuzzleSolver.Solve(puzzle, new SolverLimits(1000000, Math.Min(p.SolverBudgetMs, left), cancellation, now));
                cancellation.ThrowIfCancellationRequested();
                // A device-dependent timeout must never skip to a later candidate for this descriptor.
                // Stop the whole reservation attempt; a retry starts from the identical seed and references.
                if(verified.Classification==SolverClassification.INDETERMINATE)
                    return new PuzzleCandidateResult(null,null,attempts,"GEN_VERIFICATION_INDETERMINATE");
                if(Elapsed()>=p.TotalBudgetMs)
                    return new PuzzleCandidateResult(null,null,attempts,"GEN_BUDGET_EXHAUSTED");
                if (verified.Classification != SolverClassification.UNIQUE || verified.Metrics == null ||
                    verified.Metrics.RequiredGuessDepth != 0 || verified.Metrics.DeductionSteps < p.MinDeductionSteps ||
                    verified.Metrics.MaxDeductionDepth < p.MinDeductionDepth) continue;
                if (quality != null)
                {
                    int occupied = puzzle.RowCounts.Sum();
                    if (occupied * 100 < grid.CellCount * p.MinDensityPercent || occupied * 100 > grid.CellCount * p.MaxDensityPercent ||
                        !quality.Accepts(verified.Metrics, trivial)) continue;
                }
                return new PuzzleCandidateResult(puzzle, verified, attempts, "DRAFT_CANDIDATE_VERIFIED");
            }
            cancellation.ThrowIfCancellationRequested();
            return new PuzzleCandidateResult(null, null, attempts,
                Elapsed() >= p.TotalBudgetMs ? "GEN_BUDGET_EXHAUSTED" : "GEN_NO_ACCEPTABLE_CANDIDATE");
        }

        // Optional large-grid proposal uses only a freshly solved public reference, never an authoring solution.
        private static List<int> MutatePublicPath(int[] original,GridSize grid,ref RandomSequence random)
        {
            var path=new List<int>(original);int count=random.Next(2,21);
            for(int mutation=0;mutation<count;mutation++)
            {
                int i=random.Next(0,path.Count-1);int ax=path[i]%grid.Width,ay=path[i]/grid.Width;
                int bx=path[i+1]%grid.Width,by=path[i+1]/grid.Width;
                int dx=-(by-ay),dy=bx-ax;if(random.Next(0,2)==0){dx=-dx;dy=-dy;}
                int cx=ax+dx,cy=ay+dy,ex=bx+dx,ey=by+dy;
                if(cx<0||cx>=grid.Width||ex<0||ex>=grid.Width||cy<0||cy>=grid.Height||ey<0||ey>=grid.Height)continue;
                int c=cy*grid.Width+cx,e=ey*grid.Width+ex;if(path.Contains(c)||path.Contains(e))continue;
                path.Insert(i+1,c);path.Insert(i+2,e);
            }
            return path;
        }

        private static List<int>? TryPath(GridSize grid, CellCoordinate start, CellCoordinate end, int desired,
            int maximum, int stateBudget, ref RandomSequence random, CancellationToken cancellation, Func<bool> expired)
        {
            int source = start.Y * grid.Width + start.X, target = end.Y * grid.Width + end.X;
            var occupied = new bool[grid.CellCount]; var path = new List<int> { source };
            var choices = new Stack<int[]>(); var positions = new Stack<int>();
            occupied[source] = true; choices.Push(Shuffle(ref random)); positions.Push(0);
            int states = 0;
            while (path.Count > 0 && states++ < stateBudget)
            {
                if ((states & 63) == 0) { cancellation.ThrowIfCancellationRequested(); if (expired()) return null; }
                int current = path[path.Count - 1];
                if (current == target && path.Count >= desired) return path;
                int option = positions.Pop();
                if (option == 4 || path.Count >= maximum || current == target)
                { choices.Pop(); occupied[current] = false; path.RemoveAt(path.Count - 1); continue; }
                positions.Push(option + 1);
                int direction = choices.Peek()[option];
                int x = current % grid.Width + TrackGeometry.DeltaX((Direction)direction);
                int y = current / grid.Width + TrackGeometry.DeltaY((Direction)direction);
                if (!grid.Contains(x, y)) continue;
                int next = y * grid.Width + x;
                if (occupied[next] || (next == target && path.Count + 1 < desired)) continue;
                int distance = Math.Abs(x - end.X) + Math.Abs(y - end.Y);
                if (path.Count + 1 + distance > maximum) continue;
                occupied[next] = true; path.Add(next); choices.Push(Shuffle(ref random)); positions.Push(0);
            }
            return null;
        }
        private static int[] Shuffle(ref RandomSequence random)
        { var values = new[] { 0, 1, 2, 3 }; for (int i = 3; i > 0; i--) { int j = random.Next(0, i + 1); int t = values[i]; values[i] = values[j]; values[j] = t; } return values; }

        /// <summary>ID-independent A/B-symmetric public signature, normalized over all eight dihedral transforms.</summary>
        public static string CanonicalSignature(PuzzleDefinition puzzle)
        {
            if (puzzle == null) throw new ArgumentNullException(nameof(puzzle));
            return Variants(puzzle).Select(v => v.Signature).OrderBy(s => s, StringComparer.Ordinal).First();
        }
        /// <summary>Minimum normalized counts/endpoints L1 distance; 0 includes D4 clones and reversed endpoints.</summary>
        public static int SimilarityDistancePercent(PuzzleDefinition first, PuzzleDefinition second)
        {
            if (first == null || second == null) throw new ArgumentNullException();
            var reference = new PublicPattern(first); int best = 100;
            foreach (var candidate in Variants(second))
            {
                if (candidate.Width != reference.Width || candidate.Height != reference.Height) continue;
                int countDistance = reference.Rows.Zip(candidate.Rows, (a, b) => Math.Abs(a - b)).Sum() +
                    reference.Columns.Zip(candidate.Columns, (a, b) => Math.Abs(a - b)).Sum();
                int endpointDistance = Math.Min(EndpointDistance(reference.A, candidate.A) + EndpointDistance(reference.B, candidate.B),
                    EndpointDistance(reference.A, candidate.B) + EndpointDistance(reference.B, candidate.A));
                int scale = 2 * reference.Width * reference.Height + 2 * (reference.Width + reference.Height + 4);
                best = Math.Min(best, (100 * (countDistance + endpointDistance) + scale / 2) / scale);
            }
            return best;
        }
        private static int EndpointDistance(int[] a, int[] b) => Math.Abs(a[0] - b[0]) + Math.Abs(a[1] - b[1]) + (a[2] == b[2] ? 0 : 2);
        private static IEnumerable<PublicPattern> Variants(PuzzleDefinition puzzle)
        {
            var current = new PublicPattern(puzzle);
            for (int i = 0; i < 4; i++) { yield return current; yield return current.Mirror(); current = current.Rotate(); }
        }
        private sealed class PublicPattern
        {
            internal readonly int Width, Height;
            internal readonly int[] Rows, Columns, A, B;
            internal PublicPattern(PuzzleDefinition p) : this(p.Grid.Width, p.Grid.Height, p.RowCounts.ToArray(), p.ColumnCounts.ToArray(),
                new[] { p.A.Cell.X, p.A.Cell.Y, (int)p.A.Side }, new[] { p.B.Cell.X, p.B.Cell.Y, (int)p.B.Side }) { }
            private PublicPattern(int width, int height, int[] rows, int[] columns, int[] a, int[] b)
            { Width = width; Height = height; Rows = rows; Columns = columns; A = a; B = b; }
            internal string Signature
            {
                get
                {
                    string a = string.Join(",", A), b = string.Join(",", B);
                    if (string.CompareOrdinal(a, b) > 0) { string swap = a; a = b; b = swap; }
                    return Width + "x" + Height + ":" + string.Join(",", Rows) + ":" + string.Join(",", Columns) + ":" + a + ":" + b;
                }
            }
            internal PublicPattern Rotate()
            {
                int[] Point(int[] p) => new[] { Height - 1 - p[1], p[0], (p[2] + 1) & 3 };
                return new PublicPattern(Height, Width, Columns.ToArray(), Rows.Reverse().ToArray(), Point(A), Point(B));
            }
            internal PublicPattern Mirror()
            {
                int[] Point(int[] p) => new[] { Width - 1 - p[0], p[1], p[2] == 1 ? 3 : p[2] == 3 ? 1 : p[2] };
                return new PublicPattern(Width, Height, Rows.ToArray(), Columns.Reverse().ToArray(), Point(A), Point(B));
            }
        }
        private struct RandomSequence
        {
            private ulong state;
            internal RandomSequence(ulong seed) { state = seed; }
            internal int Next(int minimum, int maximum)
            {
                if (minimum >= maximum) return minimum;
                ulong range = (ulong)(maximum - minimum), threshold = unchecked(0UL - range) % range;
                ulong value; do { value = Next64(); } while (value < threshold);
                return minimum + (int)(value % range);
            }
            private ulong Next64()
            {
                state += 0x9E3779B97F4A7C15UL; ulong z = state;
                z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
                z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL; return z ^ (z >> 31);
            }
        }
    }
}
