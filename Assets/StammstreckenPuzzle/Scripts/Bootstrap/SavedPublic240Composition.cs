using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using STP.Application;
using STP.Infrastructure.Content;
using STP.Infrastructure.Persistence;

namespace STP.Bootstrap
{
    /// <summary>The explicit worker-created, isolated ADR036 wiring of the unchanged public catalog and S save authority.</summary>
    public sealed class SavedPublic240Composition
    {
        private SavedPublic240Composition(Public240ReviewData catalog, ReviewProgressCatalog progress,
            SaveProfileIdentity profile, SaveFileSystem files, AtomicSaveRepository repository,
            PersistedPuzzleSession persisted, AutosaveCoordinator autosave, string privateRoot)
        {
            Catalog = catalog; ProgressCatalog = progress; Profile = profile; Files = files;
            Repository = repository; Persisted = persisted; Autosave = autosave; PrivateRoot = privateRoot;
            WorkerThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        /// <summary>Exactly the existing all240 public-only catalog, never an authoring or production source.</summary>
        public IPublicPuzzleCatalog Catalog { get; }
        /// <summary>The actual S QA campaign/completion/aggregate binding.</summary>
        public ReviewProgressCatalog ProgressCatalog { get; }
        /// <summary>The sole physical review namespace and profile r.</summary>
        public SaveProfileIdentity Profile { get; }
        /// <summary>The actual infrastructure adapter; no second writer or path authority is composed.</summary>
        public SaveFileSystem Files { get; }
        /// <summary>The actual validated atomic repository.</summary>
        public AtomicSaveRepository Repository { get; }
        /// <summary>The unchanged S session use case, whose Domain property is not consumed by Bootstrap.</summary>
        public PersistedPuzzleSession Persisted { get; }
        /// <summary>The single existing S autosave coordinator.</summary>
        public AutosaveCoordinator Autosave { get; }
        /// <summary>Captured actual application root, before Persistence appends the profile namespace.</summary>
        public string PrivateRoot { get; }
        /// <summary>Actual worker identity for the concrete runtime composition boundary.</summary>
        public int WorkerThreadId { get; }

        /// <summary>Copies source arrays and performs every catalog/hash/path/IO construction on a worker; creates no profile or save.</summary>
        public static Task<SavedPublic240Composition> CreateAsync(byte[] publicBytes, byte[] campaignBytes,
            byte[] completionBytes, string privateRoot, ILocalSessionClock clock, ILocalSaveDispatcher dispatcher,
            string appVersion, string buildId, CancellationToken cancellationToken)
        {
            if (publicBytes == null || campaignBytes == null || completionBytes == null || clock == null || dispatcher == null)
                throw new ArgumentNullException();
            if (string.IsNullOrWhiteSpace(appVersion) || string.IsNullOrWhiteSpace(buildId))
                throw new ArgumentException("SAVED_BUILD_DIAGNOSIS_REQUIRED");
            var p = (byte[])publicBytes.Clone(); var c = (byte[])campaignBytes.Clone(); var v = (byte[])completionBytes.Clone();
            return Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                Public240ReviewData.VerifyAssetBytes(p);
                var catalog = new Public240ReviewData(p);
                ICanonicalJson json = new CanonicalJsonAdapter();
                var progress = new ReviewProgressCatalog(catalog, c, v, json);
                var profile = new SaveProfileIdentity(SaveProfileIdentity.Public240ReviewNamespace, "r", CatalogUse.IsolatedReview);
                var codec = new LocalSaveCodec(json, profile, progress.CatalogSet, catalog, progress.Campaign, progress.Completion);
                var policy = new LocalProgressPolicy(catalog, progress.Campaign, progress.Completion, json, new SaveLedgerCodec(json));
                cancellationToken.ThrowIfCancellationRequested();
                SaveFileSystem.ProvisionPrivateRoot(privateRoot);
                var files = new SaveFileSystem(privateRoot, profile, progress.CatalogSet.AggregateCatalog);
                ValidateStoragePathLengths(files);
                var repository = new AtomicSaveRepository(files, codec, clock, appVersion, buildId);
                var persisted = new PersistedPuzzleSession(repository, catalog, policy, profile, progress.CatalogSet, clock, dispatcher);
                return new SavedPublic240Composition(catalog, progress, profile, files, repository,
                    persisted, new AutosaveCoordinator(persisted), privateRoot);
            }, cancellationToken);
        }

        /// <summary>Read-only pure catalog preflight for the explicitly serialized editor/build inputs; no filesystem or save operation.</summary>
        public static void ValidateCatalogBytes(byte[] publicBytes, byte[] campaignBytes, byte[] completionBytes)
        {
            Public240ReviewData.VerifyAssetBytes(publicBytes);
            var catalog = new Public240ReviewData(publicBytes);
            ICanonicalJson json = new CanonicalJsonAdapter();
            _ = new ReviewProgressCatalog(catalog, campaignBytes, completionBytes, json);
        }

        /// <summary>Checks every actual Windows candidate/archive/staging filename before a profile read or write; Unix keeps its native limits.</summary>
        public static void ValidateStoragePathLengths(SaveFileSystem files)
        {
            if (files == null) throw new ArgumentNullException(nameof(files));
            ValidateStoragePathLengthsForProfileDirectory(files.ProfileDirectory);
        }

        /// <summary>Pure length preflight for an already supplied physical profile directory; it neither constructs a save path nor touches files.</summary>
        public static void ValidateStoragePathLengthsForProfileDirectory(string profileDirectory)
        {
            if (profileDirectory == null || !Path.IsPathRooted(profileDirectory)) throw new ArgumentException("SAVED_PROFILE_DIRECTORY_NOT_ABSOLUTE");
            if (Path.DirectorySeparatorChar != '\\') return;
            string directory = profileDirectory;
            foreach (string area in new[] { "", "recovery", "migration-source" })
            {
                string subdirectory = area.Length == 0 ? directory : Path.Combine(directory, area);
                if (subdirectory.Length >= 248) throw new IOException("SAVED_WINDOWS_DIRECTORY_TOO_LONG");
                if (area.Length == 0)
                {
                    foreach (string file in new[] { "save.json", "save.backup.json", "save.pending.json", "save.migration-source.json" })
                        CheckWindowsFile(Path.Combine(subdirectory, file));
                }
                else
                {
                    foreach (SaveCandidateKind kind in new[] { SaveCandidateKind.Main, SaveCandidateKind.Backup, SaveCandidateKind.Pending })
                        CheckWindowsFile(Path.Combine(subdirectory, kind + "-" + new string('0', 64) + ".json"));
                }
                CheckWindowsFile(Path.Combine(subdirectory, "t-" + new string('0', 32) + ".tmp"));
            }
        }

        private static void CheckWindowsFile(string path)
        {
            if (path.Length >= 260) throw new IOException("SAVED_WINDOWS_FILE_TOO_LONG");
        }
    }
}
