using System;
using NUnit.Framework;
using STP.Puzzle.Domain;

namespace STP.Tests.Domain.EditMode
{
    /// <summary>
    /// Konstruktionsinvarianten der PuzzleDefinition gemäß PUZZLE_ENGINE.md
    /// Abschnitt 3: Ungültige Definitionen können nicht konstruiert werden.
    /// </summary>
    public sealed class PuzzleDefinitionTests
    {
        private static readonly int[] OneZero = { 1, 0 };

        private static PuzzleDefinition SingleCellDefinition()
        {
            return new PuzzleDefinition(
                new GridSize(2, 2),
                new Endpoint(Direction.N, 0),
                new Endpoint(Direction.W, 0),
                OneZero,
                OneZero,
                PuzzleDefinition.RulesetVersionV1);
        }

        /// <summary>Die verankerte Ein-Zellen-Fixture (2×2, A=N0, B=W0) ist eine gültige Definition.</summary>
        [Test]
        public void SingleCellFixture_IsValid()
        {
            var definition = SingleCellDefinition();
            Assert.AreEqual(new CellCoordinate(0, 0), definition.A.AdjacentCell(definition.Grid));
            Assert.AreEqual(new CellCoordinate(0, 0), definition.B.AdjacentCell(definition.Grid));
        }

        /// <summary>A und B dürfen nicht derselbe Außenanschluss sein.</summary>
        [Test]
        public void IdenticalEndpoints_AreRejected()
        {
            Assert.Throws<ArgumentException>(() => new PuzzleDefinition(
                new GridSize(2, 2), new Endpoint(Direction.N, 0), new Endpoint(Direction.N, 0),
                OneZero, OneZero, PuzzleDefinition.RulesetVersionV1));
        }

        /// <summary>Endpointindizes müssen zur Achse passen.</summary>
        [Test]
        public void OutOfAxisEndpointIndex_IsRejected()
        {
            Assert.Throws<ArgumentException>(() => new PuzzleDefinition(
                new GridSize(2, 2), new Endpoint(Direction.N, 2), new Endpoint(Direction.W, 0),
                OneZero, OneZero, PuzzleDefinition.RulesetVersionV1));
            Assert.Throws<ArgumentException>(() => new PuzzleDefinition(
                new GridSize(2, 2), new Endpoint(Direction.E, -1), new Endpoint(Direction.W, 0),
                OneZero, OneZero, PuzzleDefinition.RulesetVersionV1));
        }

        /// <summary>Randzahlen benötigen passende Länge und Werte innerhalb der Rasterdimension.</summary>
        [Test]
        public void Counts_LengthAndRange_AreValidated()
        {
            Assert.Throws<ArgumentException>(() => new PuzzleDefinition(
                new GridSize(2, 2), new Endpoint(Direction.N, 0), new Endpoint(Direction.W, 0),
                new[] { 1 }, OneZero, PuzzleDefinition.RulesetVersionV1));
            Assert.Throws<ArgumentException>(() => new PuzzleDefinition(
                new GridSize(2, 2), new Endpoint(Direction.N, 0), new Endpoint(Direction.W, 0),
                new[] { 3, 0 }, OneZero, PuzzleDefinition.RulesetVersionV1));
            Assert.Throws<ArgumentException>(() => new PuzzleDefinition(
                new GridSize(2, 2), new Endpoint(Direction.N, 0), new Endpoint(Direction.W, 0),
                new[] { -1, 0 }, OneZero, PuzzleDefinition.RulesetVersionV1));
        }

        /// <summary>Zeilen- und Spaltensumme müssen gleich und mindestens 1 sein.</summary>
        [Test]
        public void CountSums_MustMatchAndBePositive()
        {
            // Isolierter Summenverstoß: Zeilen 1 vs. Spalten 2; alle übrigen
            // Invarianten (Indizes, Längen, Wertebereiche, Belegbarkeit der
            // Endpointzellen) sind erfüllt — die Ablehnung gilt allein der
            // Summengleichheit.
            Assert.Throws<ArgumentException>(() => new PuzzleDefinition(
                new GridSize(2, 2), new Endpoint(Direction.N, 0), new Endpoint(Direction.W, 0),
                new[] { 1, 0 }, new[] { 1, 1 }, PuzzleDefinition.RulesetVersionV1));
            // Gesamtsumme 0 verletzt die Mindestsumme 1.
            Assert.Throws<ArgumentException>(() => new PuzzleDefinition(
                new GridSize(2, 2), new Endpoint(Direction.N, 0), new Endpoint(Direction.W, 0),
                new[] { 0, 0 }, new[] { 0, 0 }, PuzzleDefinition.RulesetVersionV1));
        }

        /// <summary>Eine Endpointzelle, die laut Randzahlen nicht belegt sein kann, ist ungültig.</summary>
        [Test]
        public void EndpointCellWithZeroCount_IsRejected()
        {
            Assert.Throws<ArgumentException>(() => new PuzzleDefinition(
                new GridSize(2, 2), new Endpoint(Direction.N, 0), new Endpoint(Direction.W, 0),
                new[] { 0, 1 }, new[] { 0, 1 }, PuzzleDefinition.RulesetVersionV1));
        }

        /// <summary>Nicht registrierte Ruleset-Versionen werden abgelehnt.</summary>
        [Test]
        public void UnsupportedRuleset_IsRejected()
        {
            Assert.Throws<ArgumentException>(() => new PuzzleDefinition(
                new GridSize(2, 2), new Endpoint(Direction.N, 0), new Endpoint(Direction.W, 0),
                OneZero, OneZero, "train-track-v0"));
        }

        /// <summary>Rastergrenzen: Minimum 2 und Sicherheitsmaximum 32 je Achse.</summary>
        [Test]
        public void GridSize_EnforcesBounds()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new GridSize(1, 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => new GridSize(2, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new GridSize(33, 2));
            Assert.DoesNotThrow(() => new GridSize(2, 2));
            Assert.DoesNotThrow(() => new GridSize(32, 32));
        }
    }
}
