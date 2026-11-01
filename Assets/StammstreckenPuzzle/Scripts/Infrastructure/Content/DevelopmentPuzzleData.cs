using STP.Application;

namespace STP.Infrastructure.Content
{
    /// <summary>Two independent public-only QA inputs; no campaign, authoring solution or catalog import.</summary>
    public static class DevelopmentPuzzleData
    {
        /// <summary>Creates an empty in-memory QA attempt from dimensions, ports and visible counts only.</summary>
        public static DevelopmentPuzzleSession CreateSession(bool large)
        {
            var size = large ? 10 : 4;
            var rows = new int[size];
            var columns = new int[size];
            for (var index = 0; index < size; index++)
            {
                rows[index] = index == 0 ? size : 1;
                columns[index] = index == size - 1 ? size : 1;
            }
            return new DevelopmentPuzzleSession(large ? "qa.wp038.grid10" : "qa.wp038.grid4", size, size,
                DevelopmentEdge.W, 0, DevelopmentEdge.S, size - 1, rows, columns);
        }
    }
}
