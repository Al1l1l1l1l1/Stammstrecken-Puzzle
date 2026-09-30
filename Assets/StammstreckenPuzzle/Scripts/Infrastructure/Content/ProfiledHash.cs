using System;

namespace STP.Infrastructure.Content
{
    /// <summary>
    /// Profilierter Hash gemäß LEVEL_DATA_FORMAT.md Abschnitt 6: Jeder Hash
    /// wird als <c>{profile, sha256}</c> gespeichert. Kein Hash wird anhand
    /// seines Wertes heuristisch gedeutet.
    /// </summary>
    public readonly struct ProfiledHash : IEquatable<ProfiledHash>
    {
        /// <summary>Registriertes Hashprofil.</summary>
        public string Profile { get; }

        /// <summary>Kleingeschriebener SHA-256-Hexwert (64 Zeichen).</summary>
        public string Sha256 { get; }

        /// <summary>Erstellt einen profilierten Hash.</summary>
        public ProfiledHash(string profile, string sha256)
        {
            Profile = profile ?? throw new ArgumentNullException(nameof(profile));
            Sha256 = sha256 ?? throw new ArgumentNullException(nameof(sha256));
        }

        /// <inheritdoc/>
        public bool Equals(ProfiledHash other)
        {
            return Profile == other.Profile && Sha256 == other.Sha256;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is ProfiledHash other && Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return Profile.GetHashCode() ^ Sha256.GetHashCode();
        }

        /// <summary>Gleichheit von Profil und Wert.</summary>
        public static bool operator ==(ProfiledHash left, ProfiledHash right)
        {
            return left.Equals(right);
        }

        /// <summary>Ungleichheit von Profil oder Wert.</summary>
        public static bool operator !=(ProfiledHash left, ProfiledHash right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{Profile}:{Sha256}";
        }
    }
}
