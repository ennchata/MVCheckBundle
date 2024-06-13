using MapsetParser.objects;
using MapsetParser.settings;
using MapsetVerifierFramework.objects;
using MapsetVerifierFramework.objects.metadata;
using MapsetVerifierFramework.objects.attributes;
using System;
using System.Collections.Generic;
using System.IO;
using MapsetVerifierFramework.objects.resources;
using System.Linq;

namespace MVCheckBundle.General {

    [Check]
    public class SilentHitsoundFilesizeCheck : GeneralCheck {
        public override CheckMetadata GetMetadata() => new CheckMetadata() {
            Category = "Audio",
            Message = "Silent hitsounds more than 44 bytes in size.",
            Author = "enneya",
            Documentation = new Dictionary<string, string> {
                { "Purpose", "Ensuring silent hitsounds use the 44-byte sample prescribed by the Ranking Criteria." },
                { "Reasoning", @"
                                The Ranking Criteria dictates usage of <a href=""https://up.ppy.sh/files/blank.wav"">this blank sample</a> for silent hitsounds.
                                <note>Other files have unnecessarily large file sizes, and 0-byte files do not function.</note>
                                This check reports any blank hitsound files that are not 44 bytes in file size.
                " }
            }
        };

        public override Dictionary<string, IssueTemplate> GetTemplates() => new Dictionary<string, IssueTemplate>() {
            {
                "Problem",
                new IssueTemplate(Issue.Level.Problem,
                    "\"{0}\" is silent while being {1} bytes in file size.", "file name", "file size"
                ).WithCause("A blank hitsound file is unnecessarily large.")
            },
            {
                "Error",
                new IssueTemplate(Issue.Level.Error,
                    Constants.FileExceptionErrorMessage, "file name"
                ).WithCause("An error occoured while processing this hitsound file.")
            }
        };

        public override IEnumerable<Issue> GetIssues(BeatmapSet aBeatmapSet) {
            foreach (string hitsoundFile in aBeatmapSet.hitSoundFiles) {
                string fullPath = Path.Combine(aBeatmapSet.songPath, hitsoundFile);

                List<float[]> peaks = null;
                FileInfo fileInfo = null;
                Issue exceptionIssue = null;

                try {
                    peaks = AudioBASS.GetPeaks(fullPath);
                    fileInfo = new FileInfo(fullPath);
                } catch (Exception) {
                    exceptionIssue = new Issue(GetTemplate("Error"), null, hitsoundFile);
                }

                if (exceptionIssue != null) {
                    yield return exceptionIssue;
                } else if (fileInfo.Length != 44 && (!(peaks.Count > 0) || !(peaks.Sum(peak => peak.Sum()) > 0))) { // thanks naxess
                    yield return new Issue(GetTemplate("Problem"), null, hitsoundFile, fileInfo.Length);
                }
            }
        }
    }
}
