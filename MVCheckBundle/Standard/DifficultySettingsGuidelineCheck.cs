using MapsetParser.objects;
using MapsetParser.settings;
using MapsetVerifierFramework.objects;
using MapsetVerifierFramework.objects.attributes;
using MapsetVerifierFramework.objects.metadata;
using System;
using System.Collections.Generic;

namespace MVCheckBundle.Standard {
    [Check]
    public class DifficultySettingsGuidelineCheck : BeatmapCheck {
        public override CheckMetadata GetMetadata() => new BeatmapCheckMetadata {
            Category = "Settings",
            Message = "Difficulty settings exceeding guideline boundaries.",
            Author = "enneya",
            Modes = new Beatmap.Mode[] { Beatmap.Mode.Standard },
            Difficulties = Constants.AllDifficulties,
            Documentation = new Dictionary<string, string> {
                { "Purpose", "Check if difficulty settings are conform to Ranking Criteria guidelines for their respective difficulty levels." },
                { "Reasoning", @"
                                In case the mapper (or you) forgot to change settings after copying another difficulty or accidentally touched a control slider. 
                                <note>Note that mappers can intentionally pick out of range values according to map design (especially
                                for Insane and above), make sure to use your own judgement before pointing any warnings out.</note>

		                        <h3>Guidelines overview (minimum - maximum)</h3>

                                <style type='text/css' scoped>
                                    table, th, td {
                                        border: 1px solid;
                                        border-collapse: collapse;
                                        padding: 5px;
                                    }
                                </style>

		                        <table>
                                    <tr><td>Difficulty</td><td>Circle Size</td><td>Approach Rate</td><td>Overall Difficulty</td><td>HP Drain</td>" 
                                + GenerateDocumentation() + "</table>"
			    }
            }
        };

        public override Dictionary<string, IssueTemplate> GetTemplates() => new Dictionary<string, IssueTemplate> {
            {
                "Warning Between",
                new IssueTemplate(Issue.Level.Warning,
                    "{0} is set to {1}. Guidelines recommend this value to be between {2} and {3}, ensure this makes sense.",
                    "difficulty setting", "value", "recommended minimum", "recommended maximum")
                .WithCause("Current value is outside the range recommended by the Ranking Criteria.")
            },
            {
                "Warning Less",
                new IssueTemplate(Issue.Level.Warning,
                    "{0} is set to {1}. Guidelines recommend this value to be less than {2}, ensure this makes sense.",
                    "difficulty setting", "value", "recommended maximum")
                .WithCause("Current value is above the maximum recommended by the Ranking Criteria.")
            },
            {
                "Warning More",
                new IssueTemplate(Issue.Level.Warning,
                    "{0} is set to {1}. Guidelines recommend this value to be more than {2}, ensure this makes sense.",
                    "difficulty setting", "value", "recommended minimum")
                .WithCause("Current value is below the minimum recommended by the Ranking Criteria.")
            }
        };
        
        public override IEnumerable<Issue> GetIssues(Beatmap beatmap) {
            foreach (Beatmap.Difficulty difficulty in Constants.AllDifficulties) {
                Issue csCheck = CheckSetting(beatmap, difficulty, "CS", beatmap.difficultySettings.circleSize, Constants.CsGuidelines);
                if (csCheck != null) yield return csCheck;
                
                Issue arCheck = CheckSetting(beatmap, difficulty, "AR", beatmap.difficultySettings.approachRate, Constants.ArGuidelines);
                if (arCheck != null) yield return arCheck;

                Issue odCheck = CheckSetting(beatmap, difficulty, "OD", beatmap.difficultySettings.overallDifficulty, Constants.OdGuidelines);
                if (odCheck != null) yield return odCheck;

                Issue hpCheck = CheckSetting(beatmap, difficulty, "HP", beatmap.difficultySettings.hpDrain, Constants.HpGuidelines);
                if (hpCheck != null) yield return hpCheck;
            }
        }

        private Issue CheckSetting(Beatmap beatmap, Beatmap.Difficulty interpretation, string label, float value, Dictionary<Beatmap.Difficulty, float[]> values) {
            float lowerBound = values[interpretation][0];
            float upperBound = values[interpretation][1];

            if (lowerBound == -1 && upperBound < value) {
                return new Issue(GetTemplate("Warning Less"), beatmap, label, value, upperBound).ForDifficulties(interpretation);
            }

            if (lowerBound > value && upperBound == -1) {
                return new Issue(GetTemplate("Warning More"), beatmap, label, value, lowerBound).ForDifficulties(interpretation);
            }

            if (lowerBound > value || upperBound < value) {
                return new Issue(GetTemplate("Warning Between"), beatmap, label, value, lowerBound, upperBound).ForDifficulties(interpretation);
            }

            return null;
        }

        private string GenerateDocumentation() {
            string result = "";

            foreach(Beatmap.Difficulty difficulty in Constants.AllDifficulties) {
                result += $"<tr><td>{Enum.GetName(typeof(Beatmap.Difficulty), difficulty)}</td>";
                result += $"<td>{ParseBoundary(Constants.CsGuidelines[difficulty][0])} - {ParseBoundary(Constants.CsGuidelines[difficulty][1])}</td>";
                result += $"<td>{ParseBoundary(Constants.ArGuidelines[difficulty][0])} - {ParseBoundary(Constants.ArGuidelines[difficulty][1])}</td>";
                result += $"<td>{ParseBoundary(Constants.OdGuidelines[difficulty][0])} - {ParseBoundary(Constants.OdGuidelines[difficulty][1])}</td>";
                result += $"<td>{ParseBoundary(Constants.HpGuidelines[difficulty][0])} - {ParseBoundary(Constants.HpGuidelines[difficulty][1])}</td></tr>";
            }

            return result;
        }

        private string ParseBoundary(float value) => (value == -1) ? "N/A" : value.ToString();
    }
}