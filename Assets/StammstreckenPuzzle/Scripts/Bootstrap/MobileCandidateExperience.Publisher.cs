using System;
using System.Threading.Tasks;

namespace STP.Bootstrap
{
    public sealed partial class MobileCandidateExperience
    {
        private PublisherContactConfiguration? publisherContacts;
        private Action<string>? openPublisherContact;
        /// <summary>Explicit inert setup; final contact details and qualified review remain publisher inputs.</summary>
        public void ConfigurePublisherContacts(PublisherContactConfiguration contacts,Action<string> open)
        {
            if(disposed||publisherContacts!=null)throw new InvalidOperationException("PUBLISHER_CONTACT_CONFIGURATION_CLOSED");
            publisherContacts=contacts??throw new ArgumentNullException(nameof(contacts));
            openPublisherContact=open??throw new ArgumentNullException(nameof(open));
        }
        private bool CanOpenPublisherContact=>!disposed&&!externalInputSuspended&&State!=null&&saved.CanRunProfileAction&&publisherContacts!=null&&openPublisherContact!=null;
        private Task OpenPublisherContactAsync(bool support)
        {
            if(!CanOpenPublisherContact)throw new InvalidOperationException("PUBLISHER_CONTACT_UNAVAILABLE");
            return publisherContacts!.RequestOpenAsync(support,openPublisherContact!);
        }
    }
}
