using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using STP.Application;

namespace STP.Infrastructure.Persistence
{
    public sealed partial class SaveFileSystem
    {
        internal const string LifecycleMain="profile-lifecycle-v1.json",LifecycleStage="profile-lifecycle-v1.pending.json",LifecyclePrevious="profile-lifecycle-v1.previous.json";
        private static readonly string[] LifecycleNames={LifecycleMain,LifecycleStage,LifecyclePrevious};
        internal sealed class DeletionPlan
        {
            internal readonly List<string> Files=new List<string>(),Directories=new List<string>();
        }
        private void CheckDeletionPath(string path)
        {
            RequirePrivateRoot(root);EnsureInside(directory);CheckReparse(directory);EnsureInside(path);CheckReparse(path);
            string prefix=directory+Path.DirectorySeparatorChar;var comparison=Path.DirectorySeparatorChar=='\\'?StringComparison.OrdinalIgnoreCase:StringComparison.Ordinal;
            if(!string.Equals(path,directory,comparison)&&!Path.GetFullPath(path).StartsWith(prefix,comparison))throw new IOException("SAVE_DELETE_PATH_ESCAPE");
        }
        internal byte[]? ReadLifecycle(string name)
        {
            if(!LifecycleNames.Contains(name,StringComparer.Ordinal))throw new ArgumentException("SAVE_LIFECYCLE_PATH_INVALID");string path=CheckedPath(name);CheckDeletionPath(path);
            try{return Read(path,1024);}catch(IOException e)when(e.Message=="SAVE_FILE_SIZE_LIMIT"){return new byte[]{0};} // Invalid bounded record; the separate observation hashes every actual byte.
        }
        private bool LifecycleExists(string path)
        {
            CheckDeletionPath(path);try{if((File.GetAttributes(path)&FileAttributes.Directory)!=0)throw new IOException("SAVE_LIFECYCLE_NOT_FILE");return true;}
            catch(FileNotFoundException){return false;}catch(DirectoryNotFoundException){return false;}
        }
        internal DeletionPlan PlanDeletion(CancellationToken ct)
        {
            CheckDeletionPath(directory);var plan=new DeletionPlan();
            FileAttributes attributes;try{attributes=File.GetAttributes(directory);}catch(FileNotFoundException){return plan;}catch(DirectoryNotFoundException){return plan;}
            if((attributes&FileAttributes.Directory)==0)throw new IOException("SAVE_DELETE_PROFILE_NOT_DIRECTORY");
            void Walk(string dir,int depth)
            {
                if(depth>64||plan.Files.Count+plan.Directories.Count>100000)throw new IOException("SAVE_DELETE_SCOPE_LIMIT");
                CheckDeletionPath(dir);
                foreach(string path in Directory.GetFileSystemEntries(dir).OrderBy(x=>x,StringComparer.Ordinal))
                {
                    ct.ThrowIfCancellationRequested();CheckDeletionPath(path);var a=File.GetAttributes(path);
                    if((a&FileAttributes.Directory)!=0){Walk(path,depth+1);plan.Directories.Add(path);}
                    else plan.Files.Add(path);
                }
            }
            Walk(directory,0);return plan;
        }
        internal string DeletionObservationToken(DeletionPlan plan,CancellationToken ct,bool metadataOnly=false)
        {
            using(var bytes=new MemoryStream())using(var writer=new BinaryWriter(bytes,Encoding.UTF8,true))
            {
            writer.Write(Profile.Namespace);writer.Write(Profile.ProfileId);writer.Write((int)Profile.Use);writer.Write(Catalog.CatalogId);writer.Write(Catalog.Revision);writer.Write(Catalog.HashSha256);writer.Write((int)Catalog.ApprovalStatus);
            writer.Write(metadataOnly);
            if(!metadataOnly)foreach(string path in plan.Directories.OrderBy(x=>x,StringComparer.Ordinal)){writer.Write((byte)1);writer.Write(path.Substring(directory.Length+1));}
            foreach(string path in plan.Files.Where(p=>!metadataOnly||IsLifecycle(p)).OrderBy(x=>x,StringComparer.Ordinal))
            {
                ct.ThrowIfCancellationRequested();CheckDeletionPath(path);
                using(var stream=new FileStream(path,FileMode.Open,FileAccess.Read,FileShare.Read))using(var hash=SHA256.Create())
                {
                    if(stream.Length>67108864)throw new IOException("SAVE_DELETE_SOURCE_SIZE_LIMIT");
                    byte[] digest=hash.ComputeHash(stream);writer.Write((byte)2);writer.Write(path.Substring(directory.Length+1));writer.Write(stream.Length);writer.Write(digest);
                }
            }
            writer.Flush();return LocalSaveHash.Bytes(bytes.ToArray());
            }
        }
        private bool IsLifecycle(string path)
        {
            var comparer=Path.DirectorySeparatorChar=='\\'?StringComparer.OrdinalIgnoreCase:StringComparer.Ordinal;
            return comparer.Equals(Path.GetDirectoryName(path),directory)&&LifecycleNames.Contains(Path.GetFileName(path),comparer);
        }
        internal void DeleteProfileData(DeletionPlan plan,CancellationToken ct)
        {
            foreach(string path in plan.Files.Where(p=>!IsLifecycle(p)))
            {
                ct.ThrowIfCancellationRequested();CheckDeletionPath(path);var attributes=File.GetAttributes(path);
                if((attributes&(FileAttributes.Directory|FileAttributes.ReparsePoint))!=0)throw new IOException("SAVE_DELETE_SOURCE_CHANGED");
                Checkpoint("delete.file.before");CheckDeletionPath(path);File.Delete(path);Checkpoint("delete.file.after");
                try{File.GetAttributes(path);throw new IOException("SAVE_DELETE_REREAD_FAILED");}catch(FileNotFoundException){}catch(DirectoryNotFoundException){}
            }
            foreach(string path in plan.Directories)
            {ct.ThrowIfCancellationRequested();CheckDeletionPath(path);Checkpoint("delete.directory.before");Directory.Delete(path,false);Checkpoint("delete.directory.after");}
            var verify=PlanDeletion(ct);if(verify.Directories.Count!=0||verify.Files.Any(p=>!IsLifecycle(p)))throw new IOException("SAVE_DELETE_REREAD_FAILED");
            Checkpoint("delete.data.verified");
        }
        internal void ClearLifecycleStages()
        {
            foreach(string name in new[]{LifecycleStage,LifecyclePrevious})
            {
                string path=CheckedPath(name);CheckDeletionPath(path);if(!LifecycleExists(path))continue;
                Checkpoint("lifecycle.cleanup.before");File.Delete(path);Checkpoint("lifecycle.cleanup.after");
                if(LifecycleExists(path))throw new IOException("SAVE_LIFECYCLE_STAGE_REMAINS");
            }
        }
        internal void WriteLifecycle(byte[] bytes,string operation)
        {
            EnsureWriteDirectory();string stage=CheckedPath(LifecycleStage),main=CheckedPath(LifecycleMain),previous=CheckedPath(LifecyclePrevious);
            CheckDeletionPath(stage);Write(stage,bytes,"lifecycle."+operation,FileMode.Create);
            if(!Read(stage,1024)!.SequenceEqual(bytes))throw new IOException("SAVE_LIFECYCLE_REREAD_FAILED");Checkpoint("lifecycle."+operation+".stage-verified");
            // The verified stage stays an active fail-closed marker before any prior exchange backup is removed.
            CheckDeletionPath(previous);if(LifecycleExists(previous))File.Delete(previous);
            Checkpoint("lifecycle."+operation+".replace.before");CheckDeletionPath(main);CheckDeletionPath(stage);
            if(!LifecycleExists(main))File.Move(stage,main);else File.Replace(stage,main,previous);
            Checkpoint("lifecycle."+operation+".replace.after");
            if(Read(main,1024)==null||!Read(main,1024)!.SequenceEqual(bytes))throw new IOException("SAVE_LIFECYCLE_REREAD_FAILED");
            Checkpoint("lifecycle."+operation+".main-verified");ClearLifecycleStages();
        }
    }
}
