namespace STP.Application
{
    public sealed partial class SavedPublic240ReviewSession
    {
        /// <summary>Actual bound domain attempt mode for display, with no presentation-to-Domain assembly edge.</summary>
        public string? CurrentModeName
        {get{RequireOwner();var current=persisted.Session;return !disposed&&view!=null&&current!=null&&current.AttemptId==view.AttemptId?current.Mode.ToString():null;}}
    }
}
