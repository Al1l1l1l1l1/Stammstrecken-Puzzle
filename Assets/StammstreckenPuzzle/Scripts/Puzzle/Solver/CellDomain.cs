using System;
using STP.Puzzle.Domain;

namespace STP.Puzzle.Solver
{
    /// <summary>
    /// Kompakte Zell-Domäne des Solvers: eine Bitmaske aus `EMPTY` und den
    /// sechs Gleisformen gemäß SOLVER_ARCHITECTURE.md Abschnitt 2. Die
    /// festgelegte Wertreihenfolge der Suche ist `EMPTY`, `TRACK_NS`,
    /// `TRACK_EW`, `TRACK_NE`, `TRACK_ES`, `TRACK_SW`, `TRACK_WN`.
    /// </summary>
    internal static class CellDomain
    {
        /// <summary>Bit der Leerbelegung.</summary>
        internal const byte EmptyBit = 1;

        /// <summary>Vollständige Domäne aus allen sieben Werten.</summary>
        internal const byte AllBits = 0x7F;

        /// <summary>Liefert das Bit einer Gleisform.</summary>
        internal static byte ShapeBit(TrackShape shape)
        {
            return (byte)(1 << (1 + (int)shape));
        }

        /// <summary>Prüft, ob die Domäne leer ist (Widerspruch).</summary>
        internal static bool IsEmpty(byte domain) => domain == 0;

        /// <summary>Prüft, ob die Domäne genau einen Wert enthält (Zuweisung).</summary>
        internal static bool IsSingleton(byte domain)
        {
            return domain != 0 && (domain & (byte)(domain - 1)) == 0;
        }

        /// <summary>Zählt die verbliebenen Werte der Domäne.</summary>
        internal static int Count(byte domain)
        {
            var count = 0;
            var value = domain;
            while (value != 0)
            {
                count += value & 1;
                value >>= 1;
            }
            return count;
        }

        /// <summary>Prüft, ob die Domäne eine Leerbelegung noch zulässt.</summary>
        internal static bool AllowsEmpty(byte domain) => (domain & EmptyBit) != 0;

        /// <summary>Prüft, ob die Domäne mindestens eine Gleisform zulässt.</summary>
        internal static bool AllowsAnyTrack(byte domain) => (domain & (byte)(AllBits ^ EmptyBit)) != 0;

        /// <summary>Prüft, ob die Domäne eine Gleisform mit dem gegebenen Port zulässt.</summary>
        internal static bool AllowsTrackWithPort(byte domain, Direction port)
        {
            foreach (var shape in TrackShapeGeometry.All)
            {
                if ((domain & ShapeBit(shape)) != 0 && TrackShapeGeometry.HasPort(shape, port))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>Prüft, ob jede zugelassene Gleisform den gegebenen Port besitzt (und die Domäne kein EMPTY zulässt).</summary>
        internal static bool AllTracksHavePort(byte domain, Direction port)
        {
            if (AllowsEmpty(domain) || !AllowsAnyTrack(domain))
            {
                return false;
            }
            foreach (var shape in TrackShapeGeometry.All)
            {
                if ((domain & ShapeBit(shape)) != 0 && !TrackShapeGeometry.HasPort(shape, port))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>Entfernt alle Gleisformen mit dem gegebenen Port; liefert die neue Maske.</summary>
        internal static byte RemoveTracksWithPort(byte domain, Direction port)
        {
            var result = domain;
            foreach (var shape in TrackShapeGeometry.All)
            {
                if (TrackShapeGeometry.HasPort(shape, port))
                {
                    result = (byte)(result & ~ShapeBit(shape));
                }
            }
            return result;
        }

        /// <summary>Entfernt alle Gleisformen ohne den gegebenen Port sowie EMPTY; liefert die neue Maske.</summary>
        internal static byte KeepOnlyTracksWithPort(byte domain, Direction port)
        {
            var result = (byte)(domain & ~EmptyBit);
            foreach (var shape in TrackShapeGeometry.All)
            {
                if (!TrackShapeGeometry.HasPort(shape, port))
                {
                    result = (byte)(result & ~ShapeBit(shape));
                }
            }
            return result;
        }

        /// <summary>Entfernt das EMPTY-Bit; liefert die neue Maske.</summary>
        internal static byte RemoveEmpty(byte domain) => (byte)(domain & ~EmptyBit);

        /// <summary>Begrenzt die Domäne auf EMPTY; liefert die neue Maske.</summary>
        internal static byte OnlyEmpty(byte domain) => (byte)(domain & EmptyBit);

        /// <summary>Begrenzt die Domäne auf genau eine Gleisform; liefert die neue Maske.</summary>
        internal static byte OnlyShape(TrackShape shape) => ShapeBit(shape);

        /// <summary>Liefert die in der Domäne verbliebene Gleisform einer Singleton-Domäne.</summary>
        internal static TrackShape SingletonShape(byte domain)
        {
            foreach (var shape in TrackShapeGeometry.All)
            {
                if (domain == ShapeBit(shape))
                {
                    return shape;
                }
            }
            throw new ArgumentException("Domäne ist keine zugewiesene Gleisform.", nameof(domain));
        }

        /// <summary>
        /// Liefert die Werte der Domäne in fester Suchreihenfolge: zuerst EMPTY
        /// (als <c>null</c>), dann die sechs Formen in Enum-Reihenfolge.
        /// </summary>
        internal static TrackShape?[] OrderedValues(byte domain)
        {
            var values = new System.Collections.Generic.List<TrackShape?>(7);
            if (AllowsEmpty(domain))
            {
                values.Add(null);
            }
            foreach (var shape in TrackShapeGeometry.All)
            {
                if ((domain & ShapeBit(shape)) != 0)
                {
                    values.Add(shape);
                }
            }
            return values.ToArray();
        }
    }
}
