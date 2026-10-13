using System.Runtime.CompilerServices;

// Result construction belongs to the common editor pipeline, not arbitrary cache consumers.
[assembly: InternalsVisibleTo("STP.Editor.Content")]
