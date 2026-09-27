namespace STP.Puzzle.Domain
{
    /// <summary>
    /// Genau eine der sechs konkreten Gleisformen gemäß Konzeptbaustein 03
    /// Abschnitt 3 und GAME_STATE_MODEL.md Abschnitt 3. Jede Form besitzt exakt
    /// zwei unterschiedliche Anschlüsse; Kreuzungen und T-Knoten sind damit
    /// konstruktiv ausgeschlossen.
    /// </summary>
    public enum TrackShape : byte
    {
        /// <summary>Senkrechte Gerade: oben ↔ unten (N, S).</summary>
        NS = 0,

        /// <summary>Waagerechte Gerade: links ↔ rechts (E, W).</summary>
        EW = 1,

        /// <summary>Kurve oben–rechts (N, E).</summary>
        NE = 2,

        /// <summary>Kurve rechts–unten (E, S).</summary>
        ES = 3,

        /// <summary>Kurve unten–links (S, W).</summary>
        SW = 4,

        /// <summary>Kurve links–oben (W, N).</summary>
        WN = 5,
    }

    /// <summary>
    /// Anschlussgeometrie der sechs Gleisformen.
    /// </summary>
    public static class TrackShapeGeometry
    {
        /// <summary>Alle sechs Formen in fester Enum-Reihenfolge.</summary>
        public static readonly TrackShape[] All =
        {
            TrackShape.NS, TrackShape.EW, TrackShape.NE, TrackShape.ES, TrackShape.SW, TrackShape.WN,
        };

        /// <summary>Liefert die beiden Anschlüsse einer Form (erster, zweiter Port).</summary>
        public static (Direction First, Direction Second) Ports(TrackShape shape)
        {
            return shape switch
            {
                TrackShape.NS => (Direction.N, Direction.S),
                TrackShape.EW => (Direction.E, Direction.W),
                TrackShape.NE => (Direction.N, Direction.E),
                TrackShape.ES => (Direction.E, Direction.S),
                TrackShape.SW => (Direction.S, Direction.W),
                TrackShape.WN => (Direction.W, Direction.N),
                _ => throw new System.ArgumentOutOfRangeException(nameof(shape)),
            };
        }

        /// <summary>Prüft, ob die Form einen Anschluss in der gegebenen Richtung besitzt.</summary>
        public static bool HasPort(TrackShape shape, Direction direction)
        {
            var (first, second) = Ports(shape);
            return first == direction || second == direction;
        }

        /// <summary>Liefert die Form, die genau zwei unterschiedliche Richtungen verbindet.</summary>
        public static TrackShape FromPorts(Direction first, Direction second)
        {
            foreach (var shape in All)
            {
                if (HasPort(shape, first) && HasPort(shape, second))
                {
                    return shape;
                }
            }
            throw new System.ArgumentException(
                $"Keine Gleisform verbindet {first} mit {second}.", nameof(first));
        }
    }
}
