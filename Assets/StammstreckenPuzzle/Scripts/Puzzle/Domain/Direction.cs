namespace STP.Puzzle.Domain
{
    /// <summary>
    /// Geschlossene Himmelsrichtung einer Rasterkante gemäß
    /// LEVEL_DATA_FORMAT.md Abschnitt 2. Die Achsen wachsen mit x nach rechts
    /// und y nach unten.
    /// </summary>
    public enum Direction : byte
    {
        /// <summary>Oben (Delta y = -1).</summary>
        N = 0,

        /// <summary>Rechts (Delta x = +1).</summary>
        E = 1,

        /// <summary>Unten (Delta y = +1).</summary>
        S = 2,

        /// <summary>Links (Delta x = -1).</summary>
        W = 3,
    }

    /// <summary>
    /// Deterministische Richtungsoperationen des Domainkerns.
    /// </summary>
    public static class DirectionGeometry
    {
        /// <summary>Liefert die Gegenrichtung (N↔S, E↔W).</summary>
        public static Direction Opposite(Direction direction)
        {
            return direction switch
            {
                Direction.N => Direction.S,
                Direction.E => Direction.W,
                Direction.S => Direction.N,
                Direction.W => Direction.E,
                _ => throw new System.ArgumentOutOfRangeException(nameof(direction)),
            };
        }

        /// <summary>Liefert das x-Delta der Richtung (-1, 0 oder +1).</summary>
        public static int DeltaX(Direction direction)
        {
            return direction switch
            {
                Direction.E => 1,
                Direction.W => -1,
                _ => 0,
            };
        }

        /// <summary>Liefert das y-Delta der Richtung (-1, 0 oder +1).</summary>
        public static int DeltaY(Direction direction)
        {
            return direction switch
            {
                Direction.S => 1,
                Direction.N => -1,
                _ => 0,
            };
        }
    }
}
