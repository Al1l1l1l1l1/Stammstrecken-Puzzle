using UnityEngine;

namespace STP.Presentation.UI
{
    public sealed class StationBulletins : ScriptableObject
    {
        public Shader railwayShader = null!;
        public TextAsset labels = null!;
        public TextAsset mails = null!;
        public TextAsset voices = null!;
        public TextAsset gags = null!;
    }
}
