using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using STP.Application;

namespace STP.Infrastructure.Persistence
{
    /// <summary>The only physical path, stream, persistent-flush and replacement boundary for local saves.</summary>
    public sealed class SaveFileSystem
    {
        private readonly string root;
        private readonly string directory;
        private readonly Action<string>? fault;
        private readonly string mutexName;
        /// <summary>Binds a private absolute root to one exact isolated profile/catalog, without creating files.</summary>
        public SaveFileSystem(string privateRoot, SaveProfileIdentity profile, CatalogIdentity catalog, Action<string>? faultInjector = null)
        {
            if (privateRoot == null || !Path.IsPathRooted(privateRoot)) throw new ArgumentException("SAVE_ROOT_NOT_ABSOLUTE");
            Profile = profile ?? throw new ArgumentNullException(nameof(profile));
            Catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
            if (profile.Use != catalog.Use) throw new ArgumentException("SAVE_PROFILE_CATALOG_MISMATCH");
            root = Path.GetFullPath(privateRoot).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            if (root.Length == 0) root = Path.GetPathRoot(Path.GetFullPath(privateRoot))!;
            string use = profile.Use == CatalogUse.IsolatedReview ? SaveProfileIdentity.Public240ReviewNamespace : profile.Use == CatalogUse.InternalPreview ? SaveProfileIdentity.MobileCandidateNamespace :
                profile.Use == CatalogUse.Production ? "production-v1" : profile.Use == CatalogUse.ContractTest ? "contract-test-v1" : throw new ArgumentException("SAVE_CATALOG_USE_MISMATCH");
            var parts = profile.Use == CatalogUse.IsolatedReview || profile.Use == CatalogUse.InternalPreview ? new[] { use, catalog.HashSha256, Segment(profile.ProfileId) } :
                new[] { use, Segment(profile.Namespace), catalog.HashSha256, Segment(profile.ProfileId) };
            Segment(profile.Namespace);
            directory = parts.Aggregate(root, Path.Combine);
            EnsureInside(directory);
            RequirePrivateRoot(root);
            CheckReparse(directory);
            string lockIdentity = Path.DirectorySeparatorChar == '\\' ? directory.ToUpperInvariant() : directory;
            mutexName = "STP.LocalSave." + LocalSaveHash.Bytes(System.Text.Encoding.UTF8.GetBytes(lockIdentity));
            fault = faultInjector;
        }
        /// <summary>Exact logical identity of this physical namespace.</summary>
        public SaveProfileIdentity Profile { get; }
        /// <summary>Exact aggregate catalog identity of this physical namespace.</summary>
        public CatalogIdentity Catalog { get; }
        /// <summary>Infrastructure-only physical namespace, also useful for isolated filesystem contract tests.</summary>
        public string ProfileDirectory => directory;
        /// <summary>Explicit infrastructure provisioning on the caller's worker; creates only the private root, never a profile or save file.</summary>
        public static void ProvisionPrivateRoot(string privateRoot)
        {
            if (privateRoot == null || !Path.IsPathRooted(privateRoot)) throw new ArgumentException("SAVE_ROOT_NOT_ABSOLUTE");
            string full = Path.GetFullPath(privateRoot); CheckReparse(full);
            Directory.CreateDirectory(full); CheckReparse(full); RequirePrivateRoot(full);
        }
        private static void RequirePrivateRoot(string path)
        {
            try
            {
                FileAttributes attributes = File.GetAttributes(path);
                if ((attributes & FileAttributes.Directory) == 0) throw new IOException("SAVE_PRIVATE_ROOT_NOT_DIRECTORY");
                if ((attributes & FileAttributes.ReparsePoint) != 0) throw new IOException("SAVE_REPARSE_PATH_REJECTED");
            }
            catch (FileNotFoundException e) { throw new IOException("SAVE_PRIVATE_ROOT_MISSING", e); }
            catch (DirectoryNotFoundException e) { throw new IOException("SAVE_PRIVATE_ROOT_MISSING", e); }
        }
        /// <summary>Calls an injected deterministic crash/error boundary; no default operation is injected.</summary>
        public void Checkpoint(string point) => fault?.Invoke(point);
        private static string Segment(string value)
        {
            if (value == null || !Regex.IsMatch(value, @"^[A-Za-z0-9][A-Za-z0-9._-]{0,127}$") || value.EndsWith(".", StringComparison.Ordinal) ||
                value == "." || value == ".." || Regex.IsMatch(value.Split('.')[0], @"^(CON|PRN|AUX|NUL|COM[1-9]|LPT[1-9])$", RegexOptions.IgnoreCase))
                throw new ArgumentException("SAVE_PATH_SEGMENT_INVALID");
            return value;
        }
        private void EnsureInside(string path)
        {
            string prefix = root.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
            var comparison = Path.DirectorySeparatorChar == '\\' ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            if (!Path.GetFullPath(path).StartsWith(prefix, comparison)) throw new IOException("SAVE_PATH_ESCAPE");
        }
        private static void CheckReparse(string path)
        {
            // Inspect every existing ancestor, including the private root itself. No followed link is trusted.
            for (string? current = Path.GetFullPath(path); current != null; current = Path.GetDirectoryName(current))
            {
                try { if ((File.GetAttributes(current) & FileAttributes.ReparsePoint) != 0) throw new IOException("SAVE_REPARSE_PATH_REJECTED"); }
                catch (FileNotFoundException) { }
                catch (DirectoryNotFoundException) { }
                string? parent = Path.GetDirectoryName(current);
                if (parent == current) break;
            }
        }
        private string CandidatePath(SaveCandidateKind kind)
        {
            string file = kind == SaveCandidateKind.Main ? "save.json" : kind == SaveCandidateKind.Backup ? "save.backup.json" :
                kind == SaveCandidateKind.Pending ? "save.pending.json" : throw new ArgumentOutOfRangeException(nameof(kind));
            return CheckedPath(file);
        }
        private string CheckedPath(string relative)
        {
            string full = Path.GetFullPath(Path.Combine(directory, relative));
            EnsureInside(full); CheckReparse(full); return full;
        }
        /// <summary>An OS named mutex serializes whole transactions across repository instances and processes without a lockfile.</summary>
        public IDisposable AcquireExclusive(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            CheckReparse(directory);
            if (Path.DirectorySeparatorChar == '/') return AcquireDirectoryLock(cancellationToken);
            var mutex = new Mutex(false, mutexName);
            bool held = false;
            try
            {
                try
                {
                    int winner = WaitHandle.WaitAny(new WaitHandle[] { mutex, cancellationToken.WaitHandle }, 30000);
                    if (winner == WaitHandle.WaitTimeout) throw new TimeoutException("SAVE_WRITER_BUSY");
                    held = winner == 0;
                }
                catch (AbandonedMutexException) { held = true; }
                if (!held) cancellationToken.ThrowIfCancellationRequested();
                cancellationToken.ThrowIfCancellationRequested();
                return new HeldMutex(mutex);
            }
            catch { if (held) mutex.ReleaseMutex(); mutex.Dispose(); throw; }
        }
        private IDisposable AcquireDirectoryLock(CancellationToken ct)
        {
            int descriptor = -1;
            try
            {
                RequirePrivateRoot(root); CheckReparse(root);
                descriptor = NativeDirectory.Open(root, NativeDirectory.ReadOnlyCloseOnExecFlags());
                if (descriptor < 0) throw NativeLockError("OPEN");
                var elapsed = System.Diagnostics.Stopwatch.StartNew();
                while (true)
                {
                    ct.ThrowIfCancellationRequested();
                    if (NativeDirectory.Flock(descriptor, 2 | 4) == 0) break; // LOCK_EX | LOCK_NB.
                    int error = Marshal.GetLastWin32Error();
                    if (error != 11 && error != 35 && error != 4) throw new IOException("SAVE_NATIVE_LOCK_UNSUPPORTED_" + error);
                    if (elapsed.ElapsedMilliseconds >= 30000) throw new TimeoutException("SAVE_WRITER_BUSY");
                    Thread.Sleep(10);
                }
                ct.ThrowIfCancellationRequested();
                var held = new HeldDirectory(descriptor); descriptor = -1; return held;
            }
            catch (DllNotFoundException e) { throw new IOException("SAVE_NATIVE_LOCK_UNAVAILABLE", e); }
            catch (EntryPointNotFoundException e) { throw new IOException("SAVE_NATIVE_LOCK_UNAVAILABLE", e); }
            finally { if (descriptor >= 0) NativeDirectory.Close(descriptor); }
        }
        private static IOException NativeLockError(string operation) => new IOException("SAVE_NATIVE_LOCK_" + operation + "_" + Marshal.GetLastWin32Error());
        private sealed class HeldDirectory : IDisposable
        {
            private int descriptor;
            internal HeldDirectory(int value) { descriptor = value; }
            public void Dispose()
            {
                int value = Interlocked.Exchange(ref descriptor, -1);
                // Closing this independently opened descriptor releases the kernel lock even after process death.
                if (value >= 0) NativeDirectory.Close(value);
            }
        }
        private static class NativeDirectory
        {
            // Set close-on-exec at open, without a fixed-arity PInvoke to variadic fcntl on Apple ARM64.
            internal static int ReadOnlyCloseOnExecFlags()
            {
#if UNITY_IOS && !UNITY_EDITOR
                return 0x01000000;
#elif UNITY_ANDROID && !UNITY_EDITOR
                return 1 << 19;
#else
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return 0x01000000;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return 1 << 19;
                throw new IOException("SAVE_NATIVE_LOCK_PLATFORM_UNSUPPORTED");
#endif
            }
#if UNITY_IOS && !UNITY_EDITOR
            private const string Library = "__Internal";
#else
            private const string Library = "libc";
#endif
            [DllImport(Library, EntryPoint = "open", SetLastError = true)]
            internal static extern int Open([MarshalAs(UnmanagedType.LPUTF8Str)] string path, int flags);
            [DllImport(Library, EntryPoint = "flock", SetLastError = true)]
            internal static extern int Flock(int descriptor, int operation);
            [DllImport(Library, EntryPoint = "close", SetLastError = true)]
            internal static extern int Close(int descriptor);
        }
        private sealed class HeldMutex : IDisposable
        {
            private Mutex? mutex;
            internal HeldMutex(Mutex value) { mutex = value; }
            public void Dispose() { var value = mutex; mutex = null; if (value != null) { value.ReleaseMutex(); value.Dispose(); } }
        }
        /// <summary>Reads every byte under a bounded stream. Only actual not-found exceptions mean Missing.</summary>
        public byte[]? ReadCandidate(SaveCandidateKind kind, int maxBytes)
        {
            Checkpoint("read." + kind + ".before");
            byte[]? result = Read(CandidatePath(kind), maxBytes);
            Checkpoint("read." + kind + ".after"); return result;
        }
        private static byte[]? Read(string path, int maxBytes)
        {
            try
            {
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    if (stream.Length > maxBytes) throw new IOException("SAVE_FILE_SIZE_LIMIT");
                    int length = checked((int)stream.Length); var result = new byte[length]; int offset = 0;
                    while (offset < length) { int n = stream.Read(result, offset, length - offset); if (n == 0) throw new IOException("SAVE_FILE_READ_INCOMPLETE"); offset += n; }
                    if (stream.ReadByte() != -1) throw new IOException("SAVE_FILE_CHANGED_DURING_READ");
                    return result;
                }
            }
            catch (FileNotFoundException) { return null; }
            catch (DirectoryNotFoundException) { return null; }
        }
        /// <summary>Creates only the exact private profile namespace. Called only inside an explicitly authorized write.</summary>
        public void EnsureWriteDirectory()
        {
            CheckReparse(directory); Checkpoint("directory.before"); Directory.CreateDirectory(directory);
            CheckReparse(directory); Checkpoint("directory.after");
        }
        /// <summary>Writes a full candidate with separate injectable open/write/flush windows; always requests Flush(true).</summary>
        public void WriteCandidate(SaveCandidateKind kind, byte[] bytes, string operation)
        {
            Write(CandidatePath(kind), bytes, operation, FileMode.Create);
        }
        private void Write(string path, byte[] bytes, string operation, FileMode mode)
        {
            CheckReparse(path); Checkpoint(operation + ".open.before");
            using (var stream = new FileStream(path, mode, FileAccess.Write, FileShare.None))
            {
                Checkpoint(operation + ".open.after");
                // A distinct partial-write window proves that truncation or a short write is never acknowledged.
                int first = bytes.Length / 2; stream.Write(bytes, 0, first); Checkpoint(operation + ".partial");
                stream.Write(bytes, first, bytes.Length - first); Checkpoint(operation + ".written");
                if (operation == "pending") Checkpoint("step.3.pending-written");
                Checkpoint(operation + ".flush.before"); stream.Flush(true); Checkpoint(operation + ".flush.after");
                if (operation == "pending") Checkpoint("step.4.pending-flushed");
            }
            Checkpoint(operation + ".closed");
        }
        /// <summary>Preserves immutable exact sources under content-addressed names; existing unlike bytes are never overwritten.</summary>
        public void PreserveSource(string area, SaveCandidateKind kind, byte[] bytes)
        {
            if (area != "recovery" && area != "migration-source") throw new ArgumentException("SAVE_ARCHIVE_AREA_INVALID");
            string name = area + "/" + kind + "-" + LocalSaveHash.Bytes(bytes) + ".json";
            string path = CheckedPath(name); Checkpoint(area + ".directory.before");
            Directory.CreateDirectory(Path.GetDirectoryName(path)!); CheckReparse(path); Checkpoint(area + ".directory.after");
            byte[]? existing = Read(path, LocalSaveHash.Limits.MaxBytes);
            if (existing == null)
            {
                FinalizeArchive(path, bytes, area + "." + kind);
                existing = Read(path, LocalSaveHash.Limits.MaxBytes);
            }
            if (existing == null || !existing.SequenceEqual(bytes)) throw new IOException("SAVE_SOURCE_ARCHIVE_MISMATCH");
            Checkpoint(area + "." + kind + ".verified");
            if (area == "migration-source")
            {
                // Keep the contract's first migration-source file intact, with additional sources individually content-addressed above.
                string primary = CheckedPath("save.migration-source.json");
                byte[]? prior = Read(primary, LocalSaveHash.Limits.MaxBytes);
                if (prior == null)
                {
                    FinalizeArchive(primary, bytes, "migration-source.primary");
                    var reread = Read(primary, LocalSaveHash.Limits.MaxBytes);
                    if (reread == null || !reread.SequenceEqual(bytes)) throw new IOException("SAVE_MIGRATION_SOURCE_MISMATCH");
                }
                Checkpoint("migration-source.primary.preserved");
            }
        }
        private void FinalizeArchive(string finalPath, byte[] bytes, string operation)
        {
            // A partial archive must never acquire an immutable final hash name and permanently block recovery retries.
            string stage = Path.Combine(Path.GetDirectoryName(finalPath)!, "t-" + Guid.NewGuid().ToString("N") + ".tmp");
            EnsureInside(stage); CheckReparse(stage);
            Write(stage, bytes, operation, FileMode.CreateNew);
            var verified = Read(stage, LocalSaveHash.Limits.MaxBytes);
            if (verified == null || !verified.SequenceEqual(bytes)) throw new IOException("SAVE_SOURCE_ARCHIVE_MISMATCH");
            Checkpoint(operation + ".staging-verified");
            Checkpoint(operation + ".finalize.before");
            // Same-directory rename; no existing final file is overwritten. A retry uses a fresh staging file.
            File.Move(stage, finalPath);
            Checkpoint(operation + ".finalize.after");
            verified = Read(finalPath, LocalSaveHash.Limits.MaxBytes);
            if (verified == null || !verified.SequenceEqual(bytes)) throw new IOException("SAVE_SOURCE_ARCHIVE_MISMATCH");
        }
        /// <summary>Promotes without losing the verified pending file if atomic Replace is unavailable.</summary>
        public void PromotePending(byte[] verifiedBytes)
        {
            string pending = CandidatePath(SaveCandidateKind.Pending), main = CandidatePath(SaveCandidateKind.Main);
            Checkpoint("promote.replace.before");
            bool replaced = false;
            try { Checkpoint("promote.replace.operation"); File.Replace(pending, main, null); replaced = true; }
            catch (PlatformNotSupportedException) { }
            catch (NotSupportedException) { }
            catch (FileNotFoundException) { }
            catch (IOException)
            {
                // No success is inferred. Even partial platform effects are reobserved, preserving the prepared fallback.
            }
            if (replaced) { Checkpoint("promote.replace.after"); return; }
            Checkpoint("promote.fallback.before");
            Write(main, verifiedBytes, "promote.fallback", FileMode.Create);
            Checkpoint("promote.fallback.after");
        }
    }
}
