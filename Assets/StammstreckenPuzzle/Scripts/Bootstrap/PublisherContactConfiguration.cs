using System;
using System.Threading.Tasks;

namespace STP.Bootstrap
{
    /// <summary>Explicit public publisher destinations. Presence is configuration, never legal approval.</summary>
    public sealed class PublisherContactConfiguration
    {
        /// <summary>Validates fixed public HTTPS destinations without starting a browser or service.</summary>
        public PublisherContactConfiguration(string legalUrl,string supportUrl)
        {LegalUrl=RequirePublicHttps(legalUrl);SupportUrl=RequirePublicHttps(supportUrl);}
        /// <summary>Configured publisher legal and privacy page.</summary>
        public string LegalUrl{get;}
        /// <summary>Configured publisher support page.</summary>
        public string SupportUrl{get;}
        private static string RequirePublicHttps(string value)
        {
            if(value==null||value.Length>2048||value!=value.Trim()||
                !Uri.TryCreate(value,UriKind.Absolute,out var uri)||!uri.IsWellFormedOriginalString()||
                uri.Scheme!=Uri.UriSchemeHttps||uri.HostNameType!=UriHostNameType.Dns||uri.IsLoopback||
                uri.Host.IndexOf('.')<0||!string.IsNullOrEmpty(uri.UserInfo)||!uri.IsDefaultPort||
                !string.IsNullOrEmpty(uri.Query)||Array.Exists(value.ToCharArray(),char.IsControl))
                throw new ArgumentException("PUBLISHER_PUBLIC_HTTPS_URL_REQUIRED");
            return uri.AbsoluteUri;
        }
        /// <summary>Dispatches only the requested validated address; no background launch, identifiers or page-success claim.</summary>
        public Task RequestOpenAsync(bool support,Action<string> open)
        {if(open==null)throw new ArgumentNullException(nameof(open));open(support?SupportUrl:LegalUrl);return Task.CompletedTask;}
    }
}
