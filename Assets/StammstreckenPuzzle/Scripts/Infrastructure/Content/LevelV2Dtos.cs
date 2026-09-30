using System.Collections.Generic;
using STP.Puzzle.Domain;

namespace STP.Infrastructure.Content
{
    /// <summary>
    /// Typisierte DTO-Schicht des level-v2-Vertrags gemäß
    /// ARCHITECTURE/schemas/level-v2.schema.json. Die DTOs entstehen
    /// ausschließlich aus der strukturellen Prüfung
    /// (<see cref="LevelV2SchemaValidator"/>) und tragen die geparste
    /// Dokumentwurzel für die Hashprojektionen.
    /// </summary>
    public sealed class LevelV2Document
    {
        /// <summary>Dokumentschemaversion (Vertrag: konstant 2).</summary>
        public long DocumentSchemaVersion { get; }

        /// <summary>Rätselregelvertrag (Vertrag: konstant <c>train-track-v1</c>).</summary>
        public string RulesetVersion { get; }

        /// <summary>Fachliche Puzzleidentität.</summary>
        public string PuzzleId { get; }

        /// <summary>Redaktionelle Revision (mindestens 1).</summary>
        public long ContentRevision { get; }

        /// <summary>Rastergröße.</summary>
        public LevelV2Grid Grid { get; }

        /// <summary>Außenanschluss A.</summary>
        public LevelV2Endpoint EndpointA { get; }

        /// <summary>Außenanschluss B.</summary>
        public LevelV2Endpoint EndpointB { get; }

        /// <summary>Zielzahlen je Zeile.</summary>
        public IReadOnlyList<int> RowCounts { get; }

        /// <summary>Zielzahlen je Spalte.</summary>
        public IReadOnlyList<int> ColumnCounts { get; }

        /// <summary>Hierarchiegebundene Metadaten und Lokalisierung.</summary>
        public LevelV2Content Content { get; }

        /// <summary>Didaktik-, Qualitäts- und Zeitkalibrierungsdaten.</summary>
        public LevelV2Production Production { get; }

        /// <summary>Karten-, Zug- und Ergebnisreferenzen.</summary>
        public LevelV2Completion Completion { get; }

        /// <summary>Kanonische Authoringlösung (authoringseitiges Artefakt; keine Auslieferung an die Puzzle-UI).</summary>
        public LevelV2Solution Solution { get; }

        /// <summary>Bindung an das Proofartefakt (Artefakt-ID, Format, Proofhash).</summary>
        public LevelV2ProofRef ProofRef { get; }

        /// <summary>Geparste Dokumentwurzel (Quelle der Hashprojektionen).</summary>
        public JsonValue.Object Source { get; }

        /// <summary>Erstellt das DTO.</summary>
        public LevelV2Document(
            long documentSchemaVersion,
            string rulesetVersion,
            string puzzleId,
            long contentRevision,
            LevelV2Grid grid,
            LevelV2Endpoint endpointA,
            LevelV2Endpoint endpointB,
            IReadOnlyList<int> rowCounts,
            IReadOnlyList<int> columnCounts,
            LevelV2Content content,
            LevelV2Production production,
            LevelV2Completion completion,
            LevelV2Solution solution,
            LevelV2ProofRef proofRef,
            JsonValue.Object source)
        {
            DocumentSchemaVersion = documentSchemaVersion;
            RulesetVersion = rulesetVersion;
            PuzzleId = puzzleId;
            ContentRevision = contentRevision;
            Grid = grid;
            EndpointA = endpointA;
            EndpointB = endpointB;
            RowCounts = rowCounts;
            ColumnCounts = columnCounts;
            Content = content;
            Production = production;
            Completion = completion;
            Solution = solution;
            ProofRef = proofRef;
            Source = source;
        }
    }

    /// <summary>Rastergröße (Breite/Höhe jeweils 2..10).</summary>
    public sealed class LevelV2Grid
    {
        /// <summary>Breite.</summary>
        public int Width { get; }

        /// <summary>Höhe.</summary>
        public int Height { get; }

        /// <summary>Erstellt das Raster.</summary>
        public LevelV2Grid(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }

    /// <summary>Außenanschluss (Seite N/E/S/W plus Index 0..9).</summary>
    public sealed class LevelV2Endpoint
    {
        /// <summary>Rasterkante.</summary>
        public Direction Side { get; }

        /// <summary>Spalten- (N/S) oder Zeilenindex (E/W).</summary>
        public int Index { get; }

        /// <summary>Erstellt den Anschluss.</summary>
        public LevelV2Endpoint(Direction side, int index)
        {
            Side = side;
            Index = index;
        }
    }

    /// <summary>Hierarchie- und Lokalisierungsmetadaten.</summary>
    public sealed class LevelV2Content
    {
        /// <summary>Season.</summary>
        public long Season { get; }

        /// <summary>Netzabschnitt.</summary>
        public long NetworkSection { get; }

        /// <summary>Route.</summary>
        public long Route { get; }

        /// <summary>Position innerhalb der Route.</summary>
        public long Position { get; }

        /// <summary>Lokalisierungsschlüssel des Titels.</summary>
        public string TitleKey { get; }

        /// <summary>Lokalisierungsschlüssel des Status.</summary>
        public string StatusKey { get; }

        /// <summary>Erstellt die Metadaten.</summary>
        public LevelV2Content(long season, long networkSection, long route, long position, string titleKey, string statusKey)
        {
            Season = season;
            NetworkSection = networkSection;
            Route = route;
            Position = position;
            TitleKey = titleKey;
            StatusKey = statusKey;
        }
    }

    /// <summary>Produktions- und Kalibrierungsdaten.</summary>
    public sealed class LevelV2Production
    {
        /// <summary>Lokalisierungsschlüssel des Einstiegsschlusses.</summary>
        public string EntryDeductionKey { get; }

        /// <summary>Authoring-Fokuswerte (mindestens einer, eindeutig, aus dem Vertrags-Enum).</summary>
        public IReadOnlyList<string> Focus { get; }

        /// <summary>Schlusskettentiefe (mindestens 1).</summary>
        public long ChainDepth { get; }

        /// <summary>Qualitätsnotiz.</summary>
        public string QualityNote { get; }

        /// <summary>Zeitklasse (reines Datenfeld, ohne produktseitige Kalibrierung).</summary>
        public string TimeClass { get; }

        /// <summary>Sternschwellen in Sekunden oder <c>null</c> (unkalibriert; reines Datenfeld).</summary>
        public LevelV2StarThresholds? StarThresholdsSeconds { get; }

        /// <summary>Erstellt die Produktionsdaten.</summary>
        public LevelV2Production(
            string entryDeductionKey,
            IReadOnlyList<string> focus,
            long chainDepth,
            string qualityNote,
            string timeClass,
            LevelV2StarThresholds? starThresholdsSeconds)
        {
            EntryDeductionKey = entryDeductionKey;
            Focus = focus;
            ChainDepth = chainDepth;
            QualityNote = qualityNote;
            TimeClass = timeClass;
            StarThresholdsSeconds = starThresholdsSeconds;
        }
    }

    /// <summary>Sternschwellen (positive Ganzzahlen plus Kalibrierungsversion).</summary>
    public sealed class LevelV2StarThresholds
    {
        /// <summary>Zwei-Sterne-Schwelle.</summary>
        public long TwoStars { get; }

        /// <summary>Drei-Sterne-Schwelle.</summary>
        public long ThreeStars { get; }

        /// <summary>Kalibrierungsversion.</summary>
        public string CalibrationVersion { get; }

        /// <summary>Erstellt die Schwellen.</summary>
        public LevelV2StarThresholds(long twoStars, long threeStars, string calibrationVersion)
        {
            TwoStars = twoStars;
            ThreeStars = threeStars;
            CalibrationVersion = calibrationVersion;
        }
    }

    /// <summary>Abschlussreferenzen.</summary>
    public sealed class LevelV2Completion
    {
        /// <summary>Kartenabschnittsreferenz.</summary>
        public string MapSegmentId { get; }

        /// <summary>Zugmomentreferenz.</summary>
        public string TrainMomentId { get; }

        /// <summary>Lokalisierungsschlüssel des Ergebnistexts.</summary>
        public string ResultTextKey { get; }

        /// <summary>Erstellt die Abschlussreferenzen.</summary>
        public LevelV2Completion(string mapSegmentId, string trainMomentId, string resultTextKey)
        {
            MapSegmentId = mapSegmentId;
            TrainMomentId = trainMomentId;
            ResultTextKey = resultTextKey;
        }
    }

    /// <summary>Kanonische Authoringlösung.</summary>
    public sealed class LevelV2Solution
    {
        /// <summary>Pfad von A nach B (mindestens eine Zelle, ohne Koordinatenwiederholung).</summary>
        public IReadOnlyList<LevelV2PathCell> Path { get; }

        /// <summary>Erstellt die Lösung.</summary>
        public LevelV2Solution(IReadOnlyList<LevelV2PathCell> path)
        {
            Path = path;
        }
    }

    /// <summary>Eine Pfadzelle der Authoringlösung.</summary>
    public sealed class LevelV2PathCell
    {
        /// <summary>Spaltenindex (0..9).</summary>
        public int X { get; }

        /// <summary>Zeilenindex (0..9).</summary>
        public int Y { get; }

        /// <summary>Konkrete Gleisform.</summary>
        public TrackShape Track { get; }

        /// <summary>Erstellt die Pfadzelle.</summary>
        public LevelV2PathCell(int x, int y, TrackShape track)
        {
            X = x;
            Y = y;
            Track = track;
        }
    }

    /// <summary>Proofbindung des Leveldokuments.</summary>
    public sealed class LevelV2ProofRef
    {
        /// <summary>Artefakt-ID des Proofs.</summary>
        public string ArtifactId { get; }

        /// <summary>Proofformatversion (Vertrag: konstant 1).</summary>
        public long ProofFormatVersion { get; }

        /// <summary>Profilierter Proofhash.</summary>
        public ProfiledHash ProofHash { get; }

        /// <summary>Erstellt die Proofbindung.</summary>
        public LevelV2ProofRef(string artifactId, long proofFormatVersion, ProfiledHash proofHash)
        {
            ArtifactId = artifactId;
            ProofFormatVersion = proofFormatVersion;
            ProofHash = proofHash;
        }
    }
}
