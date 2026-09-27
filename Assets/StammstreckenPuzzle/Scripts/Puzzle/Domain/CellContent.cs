namespace STP.Puzzle.Domain
{
    /// <summary>
    /// Geschlossener Inhalt einer Rasterzelle gemäß GAME_STATE_MODEL.md Abschnitt 3.
    /// Die Stringwerte sind in Level- und Saveformat identisch (`UNSET`,
    /// `MARK_EMPTY`, `MARK_OCCUPIED`, `TRACK_*`). Gelb ist kein Feldinhalt;
    /// Auswahl ist Präsentationszustand.
    /// </summary>
    public enum CellContent : byte
    {
        /// <summary>Unberührt oder vollständig geleert; zählt nicht als Belegung.</summary>
        Unset = 0,

        /// <summary>Rote optionale Leerannahme des Spielers; zählt nicht als Belegung.</summary>
        MarkEmpty = 1,

        /// <summary>Graue optionale Belegungsannahme; zählt für die sichtbare Mengenrückmeldung, nicht für Completion.</summary>
        MarkOccupied = 2,

        /// <summary>Braune senkrechte Gerade (N, S).</summary>
        TrackNS = 3,

        /// <summary>Braune waagerechte Gerade (E, W).</summary>
        TrackEW = 4,

        /// <summary>Braune Kurve oben–rechts (N, E).</summary>
        TrackNE = 5,

        /// <summary>Braune Kurve rechts–unten (E, S).</summary>
        TrackES = 6,

        /// <summary>Braune Kurve unten–links (S, W).</summary>
        TrackSW = 7,

        /// <summary>Braune Kurve links–oben (W, N).</summary>
        TrackWN = 8,
    }

    /// <summary>
    /// Klassifikation und Abbildung der Zellinhalte.
    /// </summary>
    public static class CellContentSemantics
    {
        /// <summary>Prüft, ob der Inhalt eine der sechs konkreten Gleisformen ist.</summary>
        public static bool IsConcreteTrack(CellContent content)
        {
            return content >= CellContent.TrackNS;
        }

        /// <summary>Prüft, ob der Inhalt für die sichtbare Mengenrückmeldung als Belegung zählt (konkrete Form oder graue Annahme).</summary>
        public static bool CountsAsVisibleOccupancy(CellContent content)
        {
            return content == CellContent.MarkOccupied || IsConcreteTrack(content);
        }

        /// <summary>Bildet eine konkrete Gleisform auf ihre TrackShape ab.</summary>
        public static TrackShape ToTrackShape(CellContent content)
        {
            return content switch
            {
                CellContent.TrackNS => TrackShape.NS,
                CellContent.TrackEW => TrackShape.EW,
                CellContent.TrackNE => TrackShape.NE,
                CellContent.TrackES => TrackShape.ES,
                CellContent.TrackSW => TrackShape.SW,
                CellContent.TrackWN => TrackShape.WN,
                _ => throw new System.ArgumentException(
                    $"{content} ist keine konkrete Gleisform.", nameof(content)),
            };
        }

        /// <summary>Bildet eine TrackShape auf den zugehörigen Zellinhalt ab.</summary>
        public static CellContent FromTrackShape(TrackShape shape)
        {
            return shape switch
            {
                TrackShape.NS => CellContent.TrackNS,
                TrackShape.EW => CellContent.TrackEW,
                TrackShape.NE => CellContent.TrackNE,
                TrackShape.ES => CellContent.TrackES,
                TrackShape.SW => CellContent.TrackSW,
                TrackShape.WN => CellContent.TrackWN,
                _ => throw new System.ArgumentOutOfRangeException(nameof(shape)),
            };
        }

        /// <summary>Prüft, ob eine konkrete Gleisform einen Anschluss in der gegebenen Richtung besitzt.</summary>
        public static bool HasPort(CellContent content, Direction direction)
        {
            return TrackShapeGeometry.HasPort(ToTrackShape(content), direction);
        }
    }
}
