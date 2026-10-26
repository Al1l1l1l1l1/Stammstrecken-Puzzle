using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using STP.Application;

namespace STP.Infrastructure.Persistence
{
    /// <summary>ADR042/045 observation storage. One physical writer and unchanged record/index bytes, never a product profile.</summary>
    public sealed class HumanCalibrationStore : IHumanCalibrationRepository, IHumanRoutingRepository
    {
        private readonly string root;
        private readonly IHumanAssignmentCatalog assignments;
        private readonly ICanonicalJson json;
        private readonly Action<string>? fault;
        private readonly Dictionary<string, IHumanAssignmentCatalog> catalogs;
        private readonly string[] manifestHashes;
        private readonly bool routingEnabled;
        private readonly string? boundToken;

        private sealed class Index
        {
            internal Index(string manifest, string token, long generation, string hash) { Manifest = manifest; Token = token; Generation = generation; Hash = hash; }
            internal readonly string Manifest, Token, Hash;
            internal readonly long Generation;
            internal bool SameRun(Index other) => Manifest == other.Manifest && Token == other.Token;
        }
        private sealed class Inventory
        {
            internal readonly SortedDictionary<string, string> Files = new SortedDictionary<string, string>(StringComparer.Ordinal);
            internal readonly List<string> Directories = new List<string>();
            internal string Token = "";
        }
        private sealed class Slot
        {
            internal string Manifest = "", Token = "", Relative = "";
            internal Index? Completed;
        }
        private sealed class Observed
        {
            internal HumanStoreStatus Status;
            internal string Token = "", Error = "HUMAN_OBSERVATION_REQUIRED";
            internal HumanObservationSnapshot? Snapshot;
            internal IHumanAssignmentCatalog? Catalog;
            internal Index? Index;
            internal Inventory? Inputs;
            internal readonly List<Slot> Orphans = new List<Slot>();
            internal readonly List<string> HistoricalExtraRoles = new List<string>();
        }

        public HumanCalibrationStore(string privateRoot, IHumanAssignmentCatalog assignments, ICanonicalJson json, Action<string>? faultInjector = null)
            : this(privateRoot, assignments, json, faultInjector, null, null) { }

        public HumanCalibrationStore(string privateRoot, IHumanAssignmentCatalog assignments, IHumanManifestResolver resolver, ICanonicalJson json, Action<string>? faultInjector = null)
            : this(privateRoot, assignments, json, faultInjector, BuildSources(assignments, resolver), null) { }

        private HumanCalibrationStore(string privateRoot, IHumanAssignmentCatalog source, ICanonicalJson canonical, Action<string>? faultInjector,
            Dictionary<string, IHumanAssignmentCatalog>? sources, string? sessionToken)
        {
            if (privateRoot == null || !Path.IsPathRooted(privateRoot)) throw new ArgumentException("HUMAN_ROOT_NOT_ABSOLUTE");
            root = Path.Combine(Path.GetFullPath(privateRoot), "human-calibration-v1");
            assignments = source ?? throw new ArgumentNullException(nameof(source));
            LocalHash(assignments.ManifestBytesSha256);
            json = canonical ?? throw new ArgumentNullException(nameof(canonical)); fault = faultInjector;
            routingEnabled = sources != null;
            catalogs = sources ?? new Dictionary<string, IHumanAssignmentCatalog>(StringComparer.Ordinal) { { assignments.ManifestBytesSha256, assignments } };
            manifestHashes = catalogs.Keys.OrderBy(x => x, StringComparer.Ordinal).ToArray();
            boundToken = sessionToken; CheckPath(root);
        }
        private static Dictionary<string, IHumanAssignmentCatalog> BuildSources(IHumanAssignmentCatalog bound, IHumanManifestResolver resolver)
        {
            if (bound == null || resolver == null) throw new ArgumentNullException();
            var hashes = resolver.ManifestBytesSha256?.ToArray() ?? throw new ArgumentException("HUMAN_ROUTING_RESOLVER_REQUIRED");
            if (hashes.Length != 17 || hashes.Distinct(StringComparer.Ordinal).Count() != 17) throw new ArgumentException("HUMAN_ROUTING_CLOSED_REGISTRY_REQUIRED");
            var result = new Dictionary<string, IHumanAssignmentCatalog>(StringComparer.Ordinal);
            foreach (string hash in hashes)
            {
                LocalHash(hash); var resolved = resolver.Resolve(hash) ?? throw new ArgumentException("HUMAN_ROUTING_MANIFEST_MISSING");
                var catalog = resolved.Catalog;
                if (catalog.ManifestBytesSha256 != hash || LocalSaveHash.Bytes(resolved.GetOriginalBytes()) != hash ||
                    catalog.BuildId != bound.BuildId || catalog.UiSourceSha256 != bound.UiSourceSha256 || catalog.PublicCatalogHashSha256 != bound.PublicCatalogHashSha256 || catalog.Assignments.Count != 6)
                    throw new ArgumentException("HUMAN_ROUTING_MANIFEST_BINDING");
                result.Add(hash, catalog);
            }
            if (!result.ContainsKey(bound.ManifestBytesSha256)) throw new ArgumentException("HUMAN_ROUTING_BOUND_SOURCE_MISSING");
            return result;
        }
        private static void LocalHash(string value) { if (!System.Text.RegularExpressions.Regex.IsMatch(value ?? "", "^[a-f0-9]{64}$")) throw new ArgumentException("HUMAN_MANIFEST_HASH_INVALID"); }
        private IHumanAssignmentCatalog Catalog(string manifest) => catalogs.TryGetValue(manifest, out var source) ? source : throw new ArgumentException("HUMAN_INDEX_SOURCE_UNSUPPORTED");
        private HumanCalibrationCodec Codec(string manifest) => new HumanCalibrationCodec(json, Catalog(manifest));
        private string IndexPath => Path.Combine(root, "active-run.json");
        private string ActiveRecoveryIndex => Path.Combine(root, "active-recovery-index.json");
        private static string[] Roles(string path) => new[] { path, path + ".bak", path + ".tmp" };
        private string RecordPath(string manifest, string token)
        {
            LocalHash(manifest);
            if (!Guid.TryParseExact(token, "N", out var parsed) || parsed == Guid.Empty) throw new IOException("HUMAN_RUN_TOKEN_INVALID");
            return Path.Combine(root, manifest, token, "observations.json");
        }
        private string RecoveryAnchor(string manifest, string token) => Path.Combine(Path.GetDirectoryName(RecordPath(manifest, token))!, "recovery-anchor.json");
        private string ArchivedIndex(string manifest, string token) => Path.Combine(Path.GetDirectoryName(RecordPath(manifest, token))!, "completed-index.json");
        private static void CheckPath(string path)
        {
            for (string? part = Path.GetFullPath(path); part != null; part = Path.GetDirectoryName(part))
                if ((Directory.Exists(part) || File.Exists(part)) && (File.GetAttributes(part) & FileAttributes.ReparsePoint) != 0) throw new IOException("HUMAN_REPARSE_REJECTED");
        }
        private string Inside(string relative)
        {
            var full = Path.GetFullPath(Path.Combine(root, relative.Replace('/', Path.DirectorySeparatorChar)));
            var comparison = Path.DirectorySeparatorChar == '\\' ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            if (!full.StartsWith(root.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar, comparison)) throw new IOException("HUMAN_PATH_ESCAPE");
            CheckPath(full); return full;
        }
        private byte[]? Read(string path, int limit)
        {
            CheckPath(path); if (!File.Exists(path)) return null;
            if (new FileInfo(path).Length > limit) throw new IOException("HUMAN_FILE_SIZE_INVALID"); return File.ReadAllBytes(path);
        }
        private byte[] IndexBytes(Index index) => LocalSaveHash.Canonical(json, LocalSaveCodec.Object(("humanIndexVersion", "1"),
            ("manifestBytesSha256", LocalSaveHash.Quote(index.Manifest)), ("sessionToken", LocalSaveHash.Quote(index.Token)),
            ("generation", index.Generation.ToString(System.Globalization.CultureInfo.InvariantCulture)), ("recordBytesSha256", LocalSaveHash.Quote(index.Hash))));
        private Index DecodeIndex(byte[] source)
        {
            var parsed = json.Canonicalize(source, new JsonResourceLimits(4096, 4, 256, 32, 16)); if (!parsed.Succeeded) throw new ArgumentException("HUMAN_INDEX_INVALID");
            var cursor = CanonicalDtoCursor.FromCanonical(parsed.GetUtf8()!);
            var version = cursor.Members().Where(p => p.Key == "humanIndexVersion").ToArray();
            if (version.Length == 1 && version[0].Value.Integer() != 1) throw new ArgumentException("HUMAN_INDEX_VERSION_UNSUPPORTED");
            var f = cursor.Fields("humanIndexVersion", "manifestBytesSha256", "sessionToken", "generation", "recordBytesSha256"); f[0].RequireInteger(1);
            string manifest = f[1].String(), token = f[2].String(), hash = f[4].String(); LocalHash(manifest); LocalHash(hash);
            if (!Guid.TryParseExact(token, "N", out var id) || id == Guid.Empty || f[3].Integer() < 1 || f[3].Integer() > 9007199254740991) throw new ArgumentException("HUMAN_INDEX_INVALID");
            Catalog(manifest); return new Index(manifest, token, f[3].Integer(), hash);
        }
        private Inventory Capture(bool missingAtRead = false)
        {
            var result = new Inventory(); long total = 0;
            void Visit(string directory, int depth)
            {
                if (depth > 32) throw new IOException("HUMAN_GLOBAL_OBSERVATION_LIMIT");
                CheckPath(directory);
                foreach (var file in Directory.GetFiles(directory).OrderBy(x => x, StringComparer.Ordinal))
                {
                    CheckPath(file); long size = new FileInfo(file).Length; total = checked(total + size);
                    if (size > 8388608 || total > 536870912 || result.Files.Count >= 32768) throw new IOException("HUMAN_GLOBAL_OBSERVATION_LIMIT");
                    using var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                    using var sha = System.Security.Cryptography.SHA256.Create();
                    result.Files.Add(Path.GetRelativePath(root, file).Replace('\\', '/'), string.Concat(sha.ComputeHash(stream).Select(b => b.ToString("x2", System.Globalization.CultureInfo.InvariantCulture))));
                }
                foreach (var child in Directory.GetDirectories(directory).OrderBy(x => x, StringComparer.Ordinal))
                {
                    CheckPath(child); if (result.Directories.Count >= 32768) throw new IOException("HUMAN_GLOBAL_OBSERVATION_LIMIT");
                    result.Directories.Add(Path.GetRelativePath(root, child).Replace('\\', '/')); Visit(child, depth + 1);
                }
            }
            if (!missingAtRead && Directory.Exists(root)) Visit(root, 0);
            var fingerprint = "R" + (Path.DirectorySeparatorChar == '\\' ? root.ToUpperInvariant() : root) + string.Join("", manifestHashes.Select(h => "M" + h)) + string.Join("", result.Directories.OrderBy(x => x, StringComparer.Ordinal).Select(p => "D" + p.Length + ":" + p)) +
                string.Join("", result.Files.Select(p => "F" + p.Key.Length + ":" + p.Key + ":" + p.Value));
            result.Token = LocalSaveHash.Bytes(Encoding.UTF8.GetBytes(fingerprint)); return result;
        }
        private bool IsArchivedCompleted(Index expected)
        {
            var index = Read(ArchivedIndex(expected.Manifest, expected.Token), 4096); var record = Read(RecordPath(expected.Manifest, expected.Token), 8388608);
            if (index == null || record == null) return false;
            try
            {
                var archived = DecodeIndex(index); var snapshot = DecodeRecord(expected.Manifest, record);
                return archived.SameRun(expected) && archived.Generation == expected.Generation && archived.Hash == expected.Hash && LocalSaveHash.Bytes(record) == expected.Hash &&
                    snapshot.Generation == expected.Generation && snapshot.Run.SessionToken == expected.Token && snapshot.Run.NextAssignmentIndex == Catalog(expected.Manifest).Assignments.Count;
            }
            catch (Exception e) when (e is ArgumentException || e is SaveCodecFailure || e is InvalidOperationException) { if (IsUnsupported(e)) throw; return false; }
        }
        private static bool IsUnsupported(Exception error) => error is ArgumentException && (error.Message == "HUMAN_INDEX_SOURCE_UNSUPPORTED" || error.Message == "HUMAN_INDEX_VERSION_UNSUPPORTED" || error.Message == "HUMAN_RECORD_VERSION_UNSUPPORTED");
        private HumanObservationSnapshot DecodeRecord(string manifest, byte[] bytes)
        {
            var parsed = json.Canonicalize(bytes, new JsonResourceLimits(8388608, 64, 65536, 1048576, 256));
            if (!parsed.Succeeded) throw new ArgumentException("HUMAN_RECORD_JSON_INVALID");
            var version = CanonicalDtoCursor.FromCanonical(parsed.GetUtf8()!).Members().Where(p => p.Key == "humanRecordVersion").ToArray();
            if (version.Length == 1 && version[0].Value.Integer() != 1) throw new ArgumentException("HUMAN_RECORD_VERSION_UNSUPPORTED");
            return Codec(manifest).Decode(bytes);
        }
        private Observed Observe()
        {
            var inputs = Capture();
            try { return ObserveCore(inputs); }
            catch (Exception error) when (IsUnsupported(error)) { return State(new Observed { Inputs = inputs, Token = inputs.Token }, HumanStoreStatus.Unsupported, error.Message); }
        }
        private Observed ObserveCore(Inventory inputs)
        {
            var observed = new Observed { Inputs = inputs, Token = inputs.Token };
            bool invalid = false;
            foreach (string directory in inputs.Directories.Where(d => !d.Contains('/')))
                if (!catalogs.ContainsKey(directory)) return State(observed, HumanStoreStatus.Unsupported, "HUMAN_INDEX_SOURCE_UNSUPPORTED");
            var allowedRoot = Roles(IndexPath).Concat(new[] { ActiveRecoveryIndex }).Select(Path.GetFileName).ToArray();
            if (inputs.Files.Keys.Any(p => !p.Contains('/') && !allowedRoot.Contains(p))) invalid = true;
            var indices = new List<(int Role, Index Index)>(); var indexPaths = Roles(IndexPath).Concat(new[] { ActiveRecoveryIndex }).ToArray();
            for (int i = 0; i < indexPaths.Length; i++)
            {
                var bytes = Read(indexPaths[i], 4096); if (bytes == null) continue;
                try { indices.Add((i, DecodeIndex(bytes))); }
                catch (ArgumentException e) { if (IsUnsupported(e)) throw; invalid = true; }
                catch (SaveCodecFailure) { invalid = true; }
            }
            var slots = new List<Slot>();
            foreach (string directory in inputs.Directories)
            {
                var parts = directory.Split('/');
                if (parts.Length < 2 || parts[1] == "_recovery") continue;
                if (!Guid.TryParseExact(parts[1], "N", out var parsed) || parsed == Guid.Empty) { invalid = true; continue; }
                if (parts.Length > 2) { invalid = true; continue; }
                var slot = new Slot { Manifest = parts[0], Token = parts[1], Relative = directory }; slots.Add(slot);
                var archived = Read(ArchivedIndex(slot.Manifest, slot.Token), 4096);
                if (archived != null) try { var candidate = DecodeIndex(archived); if (candidate.Manifest == slot.Manifest && candidate.Token == slot.Token && IsArchivedCompleted(candidate)) slot.Completed = candidate; }
                    catch (Exception e) when (e is ArgumentException || e is SaveCodecFailure || e is InvalidOperationException) { if (IsUnsupported(e)) throw; invalid = true; }
            }
            var active = indices.Where(x => x.Role == 0).Select(x => x.Index).SingleOrDefault() ?? indices.Where(x => x.Role == 3).Select(x => x.Index).SingleOrDefault();
            if (active == null && indices.Count > 0)
            {
                if (indices.Select(x => x.Index.Manifest + ":" + x.Index.Token).Distinct().Count() != 1) return State(observed, HumanStoreStatus.Corrupt, "HUMAN_RUN_INDEX_AMBIGUOUS");
                active = indices.OrderByDescending(x => x.Index.Generation).First().Index;
            }
            if (active == null)
            {
                if (slots.Count == 1) active = new Index(slots[0].Manifest, slots[0].Token, 1, new string('0', 64));
                else if (slots.Count > 1) return State(observed, HumanStoreStatus.Corrupt, "HUMAN_RUN_INDEX_AMBIGUOUS");
            }
            if (active == null)
                return inputs.Files.Count == 0 && inputs.Directories.Count == 0 && !invalid ? State(observed, HumanStoreStatus.Missing, "HUMAN_RUN_MISSING") : State(observed, HumanStoreStatus.RecoveryRequired, "HUMAN_GLOBAL_ORPHANS_REQUIRE_RECOVERY");
            observed.Catalog = Catalog(active.Manifest);
            foreach (var other in indices.Where(x => !x.Index.SameRun(active))) if (other.Role != 1 || !IsArchivedCompleted(other.Index)) invalid = true;
            foreach (var slot in slots.Where(s => s.Manifest != active.Manifest || s.Token != active.Token))
            {
                if (slot.Completed == null) { observed.Orphans.Add(slot); invalid = true; continue; }
                string prefix = slot.Relative + "/";
                foreach (var extra in inputs.Files.Keys.Where(p => p.StartsWith(prefix, StringComparison.Ordinal) && p != prefix + "observations.json" && p != prefix + "completed-index.json"))
                {
                    bool acceptable = new[] { prefix + "observations.json.bak", prefix + "observations.json.tmp", prefix + "recovery-anchor.json" }.Contains(extra);
                    if (acceptable) try
                    {
                        var snapshot = DecodeRecord(slot.Manifest, Read(Inside(extra), 8388608)!);
                        acceptable = snapshot.Run.SessionToken == slot.Token && snapshot.Generation <= slot.Completed.Generation &&
                            (snapshot.Generation != slot.Completed.Generation || inputs.Files[extra] == slot.Completed.Hash);
                    }
                    catch (Exception error) when (error is ArgumentException || error is SaveCodecFailure || error is InvalidOperationException) { if (IsUnsupported(error)) throw; acceptable = false; }
                    if (!acceptable) { invalid = true; observed.HistoricalExtraRoles.Add(extra); }
                }
            }
            var records = new List<(int Role, HumanObservationSnapshot Snapshot, string Hash)>();
            var sources = Roles(RecordPath(active.Manifest, active.Token)).Concat(new[] { RecoveryAnchor(active.Manifest, active.Token) }).ToArray();
            var activePrefix = active.Manifest + "/" + active.Token + "/";
            var allowedRecords = sources.Select(p => Path.GetRelativePath(root, p).Replace('\\', '/')).Concat(new[] { activePrefix + "completed-index.json" }).ToArray();
            if (inputs.Files.Keys.Any(p => p.StartsWith(activePrefix, StringComparison.Ordinal) && !allowedRecords.Contains(p))) invalid = true;
            for (int i = 0; i < sources.Length; i++)
            {
                var bytes = Read(sources[i], 8388608); if (bytes == null) continue;
                try { var snapshot = DecodeRecord(active.Manifest, bytes); if (snapshot.Run.SessionToken != active.Token) throw new ArgumentException("HUMAN_RECORD_TOKEN_MISMATCH"); records.Add((i, snapshot, LocalSaveHash.Bytes(bytes))); }
                catch (Exception e) when (e is ArgumentException || e is SaveCodecFailure || e is InvalidOperationException) { if (IsUnsupported(e)) throw; invalid = true; }
            }
            if (records.Count == 0) return State(observed, HumanStoreStatus.RecoveryRequired, "HUMAN_RECORD_RECOVERY_REQUIRED");
            var highest = records.OrderByDescending(x => x.Snapshot.Generation).First();
            if (records.Any(x => x.Snapshot.Generation == highest.Snapshot.Generation && x.Hash != highest.Hash)) return State(observed, HumanStoreStatus.Corrupt, "HUMAN_SPLIT_BRAIN");
            observed.Snapshot = highest.Snapshot;
            observed.Index = new Index(active.Manifest, active.Token, highest.Snapshot.Generation, highest.Hash);
            var matching = indices.Where(x => x.Index.SameRun(active)).ToArray();
            bool ready = !invalid && highest.Role == 0 && matching.Any(x => x.Role == 0 && x.Index.Generation == highest.Snapshot.Generation && x.Index.Hash == highest.Hash) &&
                matching.All(x => x.Index.Generation <= highest.Snapshot.Generation && (x.Index.Generation != highest.Snapshot.Generation || x.Index.Hash == highest.Hash));
            return State(observed, ready ? HumanStoreStatus.Ready : HumanStoreStatus.RecoveryRequired, ready ? "NONE" : "HUMAN_EXPLICIT_RECOVERY_REQUIRED");
        }
        private static Observed State(Observed value, HumanStoreStatus status, string error) { value.Status = status; value.Error = error; return value; }
        private Observed ObserveBound(Observed? observed = null)
        {
            observed ??= Observe();
            if (observed.Catalog != null && observed.Catalog.ManifestBytesSha256 != assignments.ManifestBytesSha256)
                return new Observed { Status = HumanStoreStatus.Unsupported, Error = "HUMAN_BOUND_MANIFEST_CHANGED", Token = observed.Token };
            if (boundToken != null && (observed.Status == HumanStoreStatus.Missing || observed.Snapshot != null && observed.Snapshot.Run.SessionToken != boundToken))
                return new Observed { Status = HumanStoreStatus.Conflict, Error = "HUMAN_BOUND_SESSION_CHANGED", Token = observed.Token };
            return observed;
        }

        private bool MissingDirectoryForRead(CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested(); bool missing = false;
            for (string? part = root; part != null; part = Path.GetDirectoryName(part))
            {
                try
                {
                    var attributes = File.GetAttributes(part);
                    if ((attributes & FileAttributes.ReparsePoint) != 0) throw new IOException("HUMAN_REPARSE_REJECTED");
                    if ((attributes & FileAttributes.Directory) == 0) throw new IOException("HUMAN_ROOT_NOT_DIRECTORY");
                }
                catch (FileNotFoundException) { if (part == root) missing = true; }
                catch (DirectoryNotFoundException) { if (part == root) missing = true; }
            }
            return missing;
        }
        private T ReadOnly<T>(CancellationToken cancellation, bool bound, Func<Observed,T> projection)
        {
            if (MissingDirectoryForRead(cancellation))
            {
                // Linearize at the observed absent directory. Never read later writer output without its lock.
                var inputs = Capture(true);
                var absent = State(new Observed { Inputs = inputs, Token = inputs.Token }, HumanStoreStatus.Missing, "HUMAN_RUN_MISSING");
                return projection(bound ? ObserveBound(absent) : absent);
            }
            return Exclusive(cancellation, () => projection(bound ? ObserveBound() : Observe()), false);
        }
        private T Exclusive<T>(CancellationToken ct,Func<T> action,bool createDirectory=true)
        {
            CheckPath(root);if(Path.DirectorySeparatorChar=='/')return ExclusiveDirectory(ct,action,createDirectory);
            using var mutex=new Mutex(false,"STP.HumanObservation."+LocalSaveHash.Bytes(Encoding.UTF8.GetBytes(root.ToUpperInvariant())));bool entered=false;var elapsed=System.Diagnostics.Stopwatch.StartNew();try{while(!entered){ct.ThrowIfCancellationRequested();if(elapsed.ElapsedMilliseconds>=30000)throw new IOException("HUMAN_WRITER_BUSY");try{entered=mutex.WaitOne(100);}catch(AbandonedMutexException){entered=true;}}return action();}finally{if(entered)mutex.ReleaseMutex();}
        }
        private T ExclusiveDirectory<T>(CancellationToken ct,Func<T> action,bool createDirectory)
        {
            int descriptor=-1;try
            {
                if(createDirectory)Directory.CreateDirectory(root);CheckPath(root);descriptor=NativeDirectory.Open(root,NativeDirectory.Flags());if(descriptor<0)throw new IOException("HUMAN_NATIVE_LOCK_OPEN_FAILED");var elapsed=System.Diagnostics.Stopwatch.StartNew();
                while(true){ct.ThrowIfCancellationRequested();if(NativeDirectory.Flock(descriptor,2|4)==0)break;int error=Marshal.GetLastWin32Error();if(error!=11&&error!=35&&error!=4)throw new IOException("HUMAN_NATIVE_LOCK_UNSUPPORTED_"+error);if(elapsed.ElapsedMilliseconds>=30000)throw new IOException("HUMAN_WRITER_BUSY");Thread.Sleep(10);}ct.ThrowIfCancellationRequested();return action();
            }
            catch(DllNotFoundException e){throw new IOException("HUMAN_NATIVE_LOCK_UNAVAILABLE",e);}catch(EntryPointNotFoundException e){throw new IOException("HUMAN_NATIVE_LOCK_UNAVAILABLE",e);}finally{if(descriptor>=0)NativeDirectory.Close(descriptor);}
        }
        private static class NativeDirectory
        {
            internal static int Flags()
            {
#if UNITY_IOS && !UNITY_EDITOR
                return 0x01000000;
#elif UNITY_ANDROID && !UNITY_EDITOR
                return 1<<19;
#else
                if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX))return 0x01000000;if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))return 1<<19;throw new IOException("HUMAN_NATIVE_LOCK_PLATFORM_UNSUPPORTED");
#endif
            }
#if UNITY_IOS && !UNITY_EDITOR
            private const string Library="__Internal";
#else
            private const string Library="libc";
#endif
#if UNITY_IOS && !UNITY_EDITOR
            [DllImport(Library,EntryPoint="stp_posix_open",SetLastError=true)]internal static extern int Open([MarshalAs(UnmanagedType.LPUTF8Str)]string path,int flags);
            [DllImport(Library,EntryPoint="stp_posix_flock",SetLastError=true)]internal static extern int Flock(int descriptor,int operation);
            [DllImport(Library,EntryPoint="stp_posix_close",SetLastError=true)]internal static extern int Close(int descriptor);
#else
            [DllImport(Library,EntryPoint="open",SetLastError=true)]internal static extern int Open([MarshalAs(UnmanagedType.LPUTF8Str)]string path,int flags);
            [DllImport(Library,EntryPoint="flock",SetLastError=true)]internal static extern int Flock(int descriptor,int operation);
            [DllImport(Library,EntryPoint="close",SetLastError=true)]internal static extern int Close(int descriptor);
#endif
        }

        private HumanStoreResult LoadResult(Observed observed) => new HumanStoreResult(observed.Status, observed.Error,
            observed.Status == HumanStoreStatus.Ready ? observed.Snapshot : null, observed.Token);
        private string FailureToken(string code) => LocalSaveHash.Bytes(Encoding.UTF8.GetBytes(root + ":" + code));
        private HumanStoreResult Safe(Func<HumanStoreResult> action)
        {
            try { return action(); }
            catch (OperationCanceledException) { return new HumanStoreResult(HumanStoreStatus.Cancelled, "HUMAN_CANCELLED"); }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException || e is ArgumentException || e is SaveCodecFailure)
            { return new HumanStoreResult(HumanStoreStatus.RecoveryRequired, "HUMAN_STORAGE_OBSERVATION_REQUIRED"); }
        }
        private HumanRoutingInspection RoutingResult(Observed observed, HumanStoreStatus? overrideStatus = null)
        {
            var status = overrideStatus ?? observed.Status; HumanRoutingAuthority? authority = null;
            if (status == HumanStoreStatus.Missing) authority = HumanRoutingAuthority.Missing(observed.Token);
            else if ((status == HumanStoreStatus.Ready || status == HumanStoreStatus.Committed) && observed.Index != null)
                authority = HumanRoutingAuthority.Confirmed(observed.Index.Manifest, observed.Index.Token, observed.Index.Generation, observed.Index.Hash, observed.Token);
            return new HumanRoutingInspection(status, observed.Error, observed.Token, observed.Catalog, observed.Snapshot, authority);
        }
        private HumanRoutingInspection SafeRouting(Func<HumanRoutingInspection> action)
        {
            try { return action(); }
            catch (OperationCanceledException) { return new HumanRoutingInspection(HumanStoreStatus.Cancelled, "HUMAN_CANCELLED", FailureToken("cancelled")); }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException || e is ArgumentException || e is SaveCodecFailure)
            { return new HumanRoutingInspection(HumanStoreStatus.RecoveryRequired, "HUMAN_STORAGE_OBSERVATION_REQUIRED", FailureToken("unknown")); }
        }
        public Task<HumanStoreResult> LoadAsync(CancellationToken cancellation) => Task.Run(() => Safe(() => ReadOnly(cancellation, true, LoadResult)), cancellation);
        public Task<HumanRoutingInspection> InspectAsync(CancellationToken cancellation) => Task.Run(() => SafeRouting(() => ReadOnly(cancellation, false, observed =>
            routingEnabled ? RoutingResult(observed) : new HumanRoutingInspection(HumanStoreStatus.Unsupported, "HUMAN_ROUTING_RESOLVER_REQUIRED", FailureToken("resolver")))), cancellation);
        public IHumanCalibrationRepository OpenConfirmedRepository(HumanRoutingAuthority confirmedAuthority)
        {
            if (!routingEnabled || confirmedAuthority == null || confirmedAuthority.IsMissing) throw new ArgumentException("HUMAN_CONFIRMED_ROUTING_REQUIRED");
            var catalog = Catalog(confirmedAuthority.ManifestBytesSha256!);
            return new HumanCalibrationStore(Path.GetDirectoryName(root)!, catalog, json, fault, catalogs, confirmedAuthority.SessionToken);
        }
        private void AtomicWrite(string target, byte[] bytes, string point)
        {
            CheckPath(target); Directory.CreateDirectory(Path.GetDirectoryName(target)!); CheckPath(target); string pending = target + ".tmp";
            using (var stream = new FileStream(pending, FileMode.Create, FileAccess.Write, FileShare.None)) { stream.Write(bytes, 0, bytes.Length); stream.Flush(true); }
            fault?.Invoke(point + "_PENDING_FLUSHED");
            if (File.Exists(target)) File.Replace(pending, target, target + ".bak"); else File.Move(pending, target); fault?.Invoke(point + "_MAIN_REPLACED");
            var actual = Read(target, 8388608); if (actual == null || !actual.SequenceEqual(bytes)) throw new IOException("HUMAN_REREAD_BYTES_MISMATCH");
        }
        private HumanStoreResult Write(HumanObservationSnapshot snapshot, CancellationToken cancellation)
        {
            string manifest = snapshot.Run.ManifestBytesSha256; var codec = Codec(manifest); var bytes = codec.Encode(snapshot); cancellation.ThrowIfCancellationRequested();
            try
            {
                AtomicWrite(RecordPath(manifest, snapshot.Run.SessionToken), bytes, "RECORD");
                var reread = codec.Decode(Read(RecordPath(manifest, snapshot.Run.SessionToken), 8388608)!);
                if (reread.Generation != snapshot.Generation) throw new IOException("HUMAN_REREAD_VALIDATION_FAILED");
                var index = new Index(manifest, snapshot.Run.SessionToken, snapshot.Generation, LocalSaveHash.Bytes(bytes)); AtomicWrite(IndexPath, IndexBytes(index), "INDEX");
                var observed = Observe();
                if (observed.Status != HumanStoreStatus.Ready || observed.Snapshot?.Generation != snapshot.Generation || observed.Index?.Manifest != manifest || observed.Index?.Token != snapshot.Run.SessionToken) throw new IOException("HUMAN_INDEX_REREAD_FAILED");
                if (File.Exists(ActiveRecoveryIndex))
                {
                    CheckPath(ActiveRecoveryIndex); File.Delete(ActiveRecoveryIndex); observed = Observe();
                    if (observed.Status != HumanStoreStatus.Ready || observed.Snapshot?.Generation != snapshot.Generation || observed.Index?.Manifest != manifest || observed.Index?.Token != snapshot.Run.SessionToken) throw new IOException("HUMAN_RECOVERY_INDEX_REREAD_FAILED");
                }
                return new HumanStoreResult(HumanStoreStatus.Committed, "NONE", snapshot, observed.Token);
            }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException || e is OperationCanceledException)
            { string? token = null; try { token = Observe().Token; } catch (Exception failure) when (failure is IOException || failure is UnauthorizedAccessException) { } return new HumanStoreResult(HumanStoreStatus.RecoveryRequired, "HUMAN_WRITE_OUTCOME_UNKNOWN", observationToken: token); }
        }
        private HumanStoreResult Start(Observed observed, HumanCalibrationRun run, CancellationToken cancellation)
        {
            if (run.NextAssignmentIndex != 0 || run.Trials.Count != 0 || Directory.Exists(Path.GetDirectoryName(RecordPath(run.ManifestBytesSha256, run.SessionToken))!))
                return new HumanStoreResult(HumanStoreStatus.Conflict, "HUMAN_NEW_PARTICIPANT_MUST_BE_EMPTY", observationToken: observed.Token);
            var start = new HumanObservationSnapshot(1, run); Codec(run.ManifestBytesSha256).Encode(start);
            if (observed.Status == HumanStoreStatus.Ready)
            {
                if (observed.Index == null || observed.Snapshot == null || observed.Snapshot.Run.NextAssignmentIndex != observed.Catalog!.Assignments.Count || observed.Snapshot.Run.SessionToken == run.SessionToken)
                    return new HumanStoreResult(HumanStoreStatus.Conflict, "HUMAN_NEXT_PARTICIPANT_NOT_ALLOWED", observationToken: observed.Token);
                AtomicWrite(ArchivedIndex(observed.Index.Manifest, observed.Index.Token), IndexBytes(observed.Index), "PARTICIPANT_ARCHIVE");
                if (!IsArchivedCompleted(observed.Index)) throw new IOException("HUMAN_COMPLETED_ARCHIVE_REREAD_FAILED");
            }
            else if (observed.Status != HumanStoreStatus.Missing) return LoadResult(observed);
            return Write(start, cancellation);
        }
        public Task<HumanRoutingInspection> StartNextParticipantAsync(HumanRoutingAuthority expectedAuthority, string targetManifestBytesSha256,
            HumanRunOrigin origin, string createdAtUtc, CancellationToken cancellation) => Task.Run(() => SafeRouting(() => Exclusive(cancellation, () =>
        {
            if (!routingEnabled || boundToken != null) throw new ArgumentException("HUMAN_ROUTING_RESOLVER_REQUIRED");
            var observed = Observe(); var current = RoutingResult(observed);
            if (expectedAuthority == null || current.Authority == null || !current.CanStartParticipant || !current.Authority.Matches(expectedAuthority))
                return new HumanRoutingInspection(HumanStoreStatus.Conflict, "HUMAN_ROUTING_AUTHORITY_CHANGED", observed.Token);
            var target = Catalog(targetManifestBytesSha256);
            var run = new HumanCalibrationRun(target.ManifestBytesSha256, Guid.NewGuid().ToString("N"), origin, createdAtUtc, 0, Array.Empty<HumanTrialRecord>());
            var result = Start(observed, run, cancellation); var fresh = Observe();
            return result.Succeeded ? RoutingResult(fresh, HumanStoreStatus.Committed) : new HumanRoutingInspection(result.Status, result.ErrorCode, fresh.Token);
        })), cancellation);
        public Task<HumanStoreResult> StartRunAsync(HumanCalibrationRun run, string? expectedPriorToken, CancellationToken cancellation) => Task.Run(() => Safe(() => Exclusive(cancellation, () =>
        {
            var observed = ObserveBound();
            if (routingEnabled || boundToken != null || run.ManifestBytesSha256 != assignments.ManifestBytesSha256) return new HumanStoreResult(HumanStoreStatus.Conflict, "HUMAN_ROUTING_USE_GLOBAL_START", observationToken: observed.Token);
            if (expectedPriorToken == null ? observed.Status != HumanStoreStatus.Missing : observed.Status != HumanStoreStatus.Ready || observed.Snapshot?.Run.SessionToken != expectedPriorToken)
                return new HumanStoreResult(HumanStoreStatus.Conflict, "HUMAN_EXISTING_RUN_REQUIRES_EXPLICIT_ARCHIVE", observationToken: observed.Token);
            return Start(observed, run, cancellation);
        })), cancellation);
        public Task<HumanStoreResult> CommitAsync(HumanObservationSnapshot snapshot, long expectedGeneration, CancellationToken cancellation) => Task.Run(() => Safe(() => Exclusive(cancellation, () =>
        {
            var observed = ObserveBound(); if (observed.Status != HumanStoreStatus.Ready) return LoadResult(observed);
            if (snapshot.Run.ManifestBytesSha256 != assignments.ManifestBytesSha256 || observed.Snapshot!.Generation != expectedGeneration || snapshot.Generation != expectedGeneration + 1 || snapshot.Run.SessionToken != observed.Snapshot.Run.SessionToken)
                return new HumanStoreResult(HumanStoreStatus.Conflict, "HUMAN_OBSERVATION_CONFLICT", observationToken: observed.Token);
            return Write(snapshot, cancellation);
        })), cancellation);
        private void ArchiveRecoveryRoles(Observed observed, CancellationToken cancellation)
        {
            var snapshot = observed.Snapshot ?? throw new IOException("HUMAN_RECOVERY_AUTHORITY_REQUIRED"); var manifest = snapshot.Run.ManifestBytesSha256; var token = snapshot.Run.SessionToken;
            var source = observed.Inputs ?? throw new IOException("HUMAN_RECOVERY_INPUTS_REQUIRED");
            string activePrefix = manifest + "/" + token + "/";
            var paths = source.Files.Keys.Where(p => !p.Contains("/_recovery/", StringComparison.Ordinal) &&
                (!p.Contains('/') || p.StartsWith(activePrefix, StringComparison.Ordinal) || observed.Orphans.Any(s => p.StartsWith(s.Relative + "/", StringComparison.Ordinal)) || observed.HistoricalExtraRoles.Contains(p))).ToArray();
            string archive;
            do { archive = Path.Combine(root, manifest, "_recovery", Guid.NewGuid().ToString("N").Substring(0, 16)); } while (Directory.Exists(archive));
            CheckPath(archive); Directory.CreateDirectory(archive); int ordinal = 0;
            foreach (string path in paths)
            {
                cancellation.ThrowIfCancellationRequested(); var original = Read(Inside(path), 8388608)!;
                if (LocalSaveHash.Bytes(original) != source.Files[path]) throw new IOException("HUMAN_RECOVERY_SOURCE_CHANGED");
                // Flatten archived copies to stay within Unity/Mono Windows path limits. The sidecar is diagnostic provenance, never read as authority.
                var prefix = (ordinal++).ToString("D5", System.Globalization.CultureInfo.InvariantCulture);
                string name = Path.GetFileName(path); if (name.Length > 24) name = name.Substring(0, 24);
                var destination = Path.Combine(archive, prefix + "-" + name); CheckPath(destination);
                var pathBytes = Encoding.UTF8.GetBytes(path + "\n"); var pathCopy = Path.Combine(archive, prefix + ".source-path.txt");
                using (var stream = new FileStream(pathCopy, FileMode.CreateNew, FileAccess.Write, FileShare.None)) { stream.Write(pathBytes, 0, pathBytes.Length); stream.Flush(true); }
                if (!Read(pathCopy, 8388608)!.SequenceEqual(pathBytes)) throw new IOException("HUMAN_RECOVERY_ARCHIVE_REREAD_FAILED");
                using (var stream = new FileStream(destination, FileMode.CreateNew, FileAccess.Write, FileShare.None)) { stream.Write(original, 0, original.Length); stream.Flush(true); }
                if (!Read(destination, 8388608)!.SequenceEqual(original)) throw new IOException("HUMAN_RECOVERY_ARCHIVE_REREAD_FAILED");
            }
            foreach (var slot in observed.Orphans)
            {
                var bytesOfDirectory = Encoding.UTF8.GetBytes(slot.Relative + "/\n"); var directoryCopy = Path.Combine(archive, (ordinal++).ToString("D5", System.Globalization.CultureInfo.InvariantCulture) + ".source-directory.txt");
                using var stream = new FileStream(directoryCopy, FileMode.CreateNew, FileAccess.Write, FileShare.None); stream.Write(bytesOfDirectory, 0, bytesOfDirectory.Length); stream.Flush(true);
            }
            cancellation.ThrowIfCancellationRequested(); string anchor = RecoveryAnchor(manifest, token); string pending = anchor + "." + Guid.NewGuid().ToString("N") + ".tmp"; var bytes = Codec(manifest).Encode(snapshot);
            using (var stream = new FileStream(pending, FileMode.CreateNew, FileAccess.Write, FileShare.None)) { stream.Write(bytes, 0, bytes.Length); stream.Flush(true); }
            if (!Read(pending, 8388608)!.SequenceEqual(bytes)) throw new IOException("HUMAN_RECOVERY_ANCHOR_REREAD_FAILED");
            if (File.Exists(anchor)) File.Replace(pending, anchor, Path.Combine(archive, "replaced-record-anchor.json")); else File.Move(pending, anchor); fault?.Invoke("RECOVERY_ANCHOR_COMMITTED");
            var indexBytes = IndexBytes(new Index(manifest, token, snapshot.Generation, LocalSaveHash.Bytes(bytes))); pending = ActiveRecoveryIndex + "." + Guid.NewGuid().ToString("N") + ".tmp";
            using (var stream = new FileStream(pending, FileMode.CreateNew, FileAccess.Write, FileShare.None)) { stream.Write(indexBytes, 0, indexBytes.Length); stream.Flush(true); }
            if (!Read(pending, 4096)!.SequenceEqual(indexBytes)) throw new IOException("HUMAN_RECOVERY_INDEX_ANCHOR_REREAD_FAILED");
            if (File.Exists(ActiveRecoveryIndex)) File.Replace(pending, ActiveRecoveryIndex, Path.Combine(archive, "replaced-active-recovery-index.json")); else File.Move(pending, ActiveRecoveryIndex); fault?.Invoke("RECOVERY_INDEX_ANCHOR_COMMITTED");
            foreach (var path in paths.Select(Inside).Where(p => p != anchor && p != ActiveRecoveryIndex))
            { CheckPath(path); File.Delete(path); if (path == IndexPath) fault?.Invoke("RECOVERY_ROOT_MAIN_DELETED"); }
            foreach (var slot in observed.Orphans)
            {
                var directory = Inside(slot.Relative);
                foreach (var child in source.Directories.Where(p => p.StartsWith(slot.Relative + "/", StringComparison.Ordinal)).OrderByDescending(p => p.Length)) if (Directory.Exists(Inside(child)) && !Directory.EnumerateFileSystemEntries(Inside(child)).Any()) Directory.Delete(Inside(child));
                if (Directory.Exists(directory) && !Directory.EnumerateFileSystemEntries(directory).Any()) Directory.Delete(directory);
            }
            fault?.Invoke("RECOVERY_ROLES_CLEARED");
        }
        public Task<HumanStoreResult> RecoverAsync(string observationToken, CancellationToken cancellation) => Task.Run(() => Safe(() => Exclusive(cancellation, () =>
        {
            var observed = ObserveBound(); if (observed.Token != observationToken) return new HumanStoreResult(HumanStoreStatus.Conflict, "HUMAN_OBSERVATION_CHANGED", observationToken: observed.Token);
            if (observed.Status == HumanStoreStatus.Ready || observed.Status != HumanStoreStatus.RecoveryRequired || observed.Snapshot == null) return LoadResult(observed);
            ArchiveRecoveryRoles(observed, cancellation); return Write(observed.Snapshot, cancellation);
        })), cancellation);
        public Task<HumanRoutingInspection> RecoverRoutingAsync(string globalObservationToken, CancellationToken cancellation) => Task.Run(() => SafeRouting(() => Exclusive(cancellation, () =>
        {
            if (!routingEnabled || boundToken != null) throw new ArgumentException("HUMAN_ROUTING_RESOLVER_REQUIRED");
            var observed = Observe(); if (observed.Token != globalObservationToken) return new HumanRoutingInspection(HumanStoreStatus.Conflict, "HUMAN_OBSERVATION_CHANGED", observed.Token);
            if (observed.Status != HumanStoreStatus.RecoveryRequired || observed.Snapshot == null) return RoutingResult(observed);
            ArchiveRecoveryRoles(observed, cancellation); var result = Write(observed.Snapshot, cancellation); var fresh = Observe();
            return result.Succeeded ? RoutingResult(fresh) : new HumanRoutingInspection(result.Status, result.ErrorCode, fresh.Token);
        })), cancellation);
        public Task<byte[]> ExportAsync(CancellationToken cancellation) => Task.Run(() => ReadOnly(cancellation, true, observed =>
        {
            if (observed.Status != HumanStoreStatus.Ready || observed.Snapshot == null) throw new IOException("HUMAN_EXPORT_REQUIRES_CONFIRMED_RECORD");
            return Read(RecordPath(observed.Snapshot.Run.ManifestBytesSha256, observed.Snapshot.Run.SessionToken), 8388608)!;
        }), cancellation);
    }
}
