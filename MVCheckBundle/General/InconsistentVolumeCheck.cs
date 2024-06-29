using MapsetParser.objects;
using MapsetParser.statics;
using MapsetVerifierFramework.objects;
using MapsetVerifierFramework.objects.attributes;
using MapsetVerifierFramework.objects.metadata;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace MVCheckBundle.General {

    [Check]
    public class InconsistentVolumeCheck : GeneralCheck {
        public override CheckMetadata GetMetadata() => new CheckMetadata() {
            Category = "Hit Sounds",
            Message = "Inconsistent volume changing timing points.",
            Author = "enneya",
            Documentation = new Dictionary<string, string> {
                { "Purpose", "" },
                { "Reasoning", "" }
            }
        };

        public override Dictionary<string, IssueTemplate> GetTemplates() => new Dictionary<string, IssueTemplate> {
            { 
                "Difference",
                new IssueTemplate(Issue.Level.Minor,
                    "{0} different volume across difficulties: {1}.",
                    "timestamp", "difficulty - percent")
                .WithCause("Volumes are not consistent across difficulties.")
            },
            {
                "Not Exist",
                new IssueTemplate(Issue.Level.Minor,
                    "{0} {1}% volume change does not exist on {2}.",
                    "timestamp", "percent", "difficulties")
                .WithCause("Volumes are not consistent across difficulties.")
            }, 
            { 
                "Exist",
                new IssueTemplate(Issue.Level.Minor,
                    "{0} {1}% volume change only exists on {2}.",
                    "timestamp", "percent", "difficulties")
                .WithCause("Volumes are not consistent across difficulties.")
            }, 
        };

        public override IEnumerable<Issue> GetIssues(BeatmapSet beatmapSet) {
            Dictionary<Beatmap, List<TimingLine>> allLines = new Dictionary<Beatmap, List<TimingLine>>();

            foreach (Beatmap beatmap in beatmapSet.beatmaps) {
                List<TimingLine> timingLines = new List<TimingLine> {
                    beatmap.timingLines[0]
                };

                foreach (TimingLine timingLine in beatmap.timingLines) {
                    if (timingLines.Last().volume != timingLine.volume) {
                        timingLines.Add(timingLine);
                    }
                }
                allLines.Add(beatmap, timingLines);
            }

            List<TimingLine> uniqueLines = new List<TimingLine>();
            TimingLineVolumeComparer comparer = new TimingLineVolumeComparer();

            foreach (var diffLines in allLines) {
                foreach (TimingLine timingLine in diffLines.Value) {
                    if (!uniqueLines.Contains(timingLine, comparer)) uniqueLines.Add(timingLine);
                }
            }

            IEnumerable<double> timestamps = uniqueLines.Select(line => line.offset).Distinct();

            foreach (double timestamp in timestamps) {
                IEnumerable<TimingLine> lines = uniqueLines.Where(line => line.offset == timestamp);

                if (lines.Count() > 1) {
                    List<string> output = new List<string>();

                    foreach (var diffLines in allLines) {
                        IEnumerable<TimingLine> sortedLines = diffLines.Value.Where(line => lines.Contains(line, comparer));
                        if (sortedLines.Count() != 0) output.Add($"\"{diffLines.Key.metadataSettings.version}\" - {sortedLines.First().volume}%");
                    }

                    yield return new Issue(
                        GetTemplate("Difference"), null,
                        Timestamp.Get(timestamp), string.Join(", ", output)
                    );
                } else if (lines.Count() == 1) {
                    TimingLine line = lines.First();
                    List<string> diffsWithLine = new List<string>();
                    List<string> diffsWithoutLine = new List<string>();

                    foreach (var diffLines in allLines) {

                        if (diffLines.Value.Contains(line, comparer)) {
                            diffsWithLine.Add($"\"{diffLines.Key.metadataSettings.version}\"");
                        } else {
                            diffsWithoutLine.Add($"\"{diffLines.Key.metadataSettings.version}\"");
                        }

                    }

                    if (diffsWithLine.Count != beatmapSet.beatmaps.Count) {
                        if (diffsWithoutLine.Count > diffsWithLine.Count) {
                            yield return new Issue(GetTemplate("Exist"), null, Timestamp.Get(line.offset), line.volume, string.Join(", ", diffsWithLine));
                        } else {
                            yield return new Issue(GetTemplate("Not Exist"), null, Timestamp.Get(line.offset), line.volume, string.Join(", ", diffsWithoutLine));
                        }
                    }
                };
            }
        }
    }

    public class TimingLineVolumeComparer : IEqualityComparer<TimingLine> {
        public bool Equals(TimingLine x, TimingLine y) {
            if (ReferenceEquals(x, y)) return true;
            if (x == null || y == null) return false;
            return x.offset == y.offset && x.volume == y.volume;
        }

        public int GetHashCode(TimingLine obj) {
            return obj.GetHashCode();
        }
    }
}
