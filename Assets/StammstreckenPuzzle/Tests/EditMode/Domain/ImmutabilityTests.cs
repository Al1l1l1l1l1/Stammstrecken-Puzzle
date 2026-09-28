using System;
using System.Collections.Generic;
using NUnit.Framework;
using STP.Puzzle.Domain;

namespace STP.Tests.Domain.EditMode
{
    /// <summary>
    /// Regressionstests zur Unveränderlichkeit: Öffentlich exponierte
    /// Collections von Definition und Session-Snapshot dürfen weder per
    /// Rückcast noch per IList-Zugriff extern mutierbar sein. Eine Mutation am
    /// Command-Fluss vorbei (Revision, Timer, Ereignisse) ist nicht möglich.
    /// </summary>
    public sealed class ImmutabilityTests
    {
        private static PuzzleDefinition SingleCellDefinition()
        {
            return new PuzzleDefinition(
                new GridSize(2, 2),
                new Endpoint(Direction.N, 0),
                new Endpoint(Direction.W, 0),
                new[] { 1, 0 },
                new[] { 1, 0 },
                PuzzleDefinition.RulesetVersionV1);
        }

        /// <summary>Der Zellstand kann nicht auf das zugrunde liegende Array zurückgecastet werden.</summary>
        [Test]
        public void Cells_CannotBeCastBackToMutableArray()
        {
            var state = PuzzleSessionState.StartNew(SingleCellDefinition(), "p", "a");

            Assert.Throws<InvalidCastException>(() => _ = (CellContent[])state.Cells);
            Assert.Throws<NotSupportedException>(() =>
                ((IList<CellContent>)state.Cells)[0] = CellContent.TrackWN);

            Assert.AreEqual(CellContent.Unset, state.Cells[0]);
            Assert.AreEqual(0, state.Revision);
        }

        /// <summary>Validierte Randzahlen sind nach der Konstruktion nicht extern mutierbar.</summary>
        [Test]
        public void Counts_CannotBeMutatedAfterConstruction()
        {
            var definition = SingleCellDefinition();

            Assert.Throws<InvalidCastException>(() => _ = (int[])definition.RowCounts);
            Assert.Throws<NotSupportedException>(() => ((IList<int>)definition.RowCounts)[0] = 5);
            Assert.Throws<NotSupportedException>(() => ((IList<int>)definition.ColumnCounts)[0] = 5);

            Assert.AreEqual(1, definition.RowCounts[0]);
            Assert.AreEqual(0, definition.RowCounts[1]);
        }

        /// <summary>Der Undo-Verlauf und seine Diff-Einträge sind nicht extern mutierbar.</summary>
        [Test]
        public void UndoStackAndDiffEntries_CannotBeMutated()
        {
            var state = PuzzleSessionState.StartNew(SingleCellDefinition(), "p", "a");
            var applied = PuzzleSession.Handle(
                state,
                new ApplyCellContentCommand("c1", state.Revision, new CellCoordinate(0, 0), CellContent.TrackWN),
                100).State;

            Assert.Throws<NotSupportedException>(() =>
                ((IList<CellDiff>)applied.UndoStack).Add(new CellDiff(
                    new[] { new CellDiffEntry(new CellCoordinate(1, 1), CellContent.Unset, CellContent.TrackNS) })));
            Assert.Throws<NotSupportedException>(() =>
                ((IList<CellDiffEntry>)applied.UndoStack[0].Entries)[0] =
                    new CellDiffEntry(new CellCoordinate(1, 1), CellContent.Unset, CellContent.TrackNS));

            Assert.AreEqual(1, applied.UndoStack.Count);
            Assert.AreEqual(new CellCoordinate(0, 0), applied.UndoStack[0].Entries[0].Coordinate);
        }

        /// <summary>Nach fehlgeschlagenen Mutationsversuchen bleibt der Snapshot unverändert gültig.</summary>
        [Test]
        public void State_RemainsIntactAfterFailedMutationAttempts()
        {
            var state = PuzzleSessionState.StartNew(SingleCellDefinition(), "p", "a");
            try
            {
                ((IList<CellContent>)state.Cells)[0] = CellContent.TrackWN;
            }
            catch (NotSupportedException)
            {
                // Erwarteter Schutz: keine Mutation, keine Nebenwirkung.
            }

            Assert.AreEqual(CellContent.Unset, state.Cells[0]);
            Assert.AreEqual(0, state.Revision);
            Assert.IsFalse(state.TimerStarted);
            Assert.IsEmpty(state.UndoStack);
        }
    }
}
