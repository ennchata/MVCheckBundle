using MapsetParser.objects;
using MapsetParser.settings;
using MapsetVerifierFramework.objects;
using MapsetVerifierFramework.objects.attributes;
using MapsetVerifierFramework.objects.metadata;
using System.Collections.Generic;

namespace MVCheckBundle {
    public static class Constants {
        public static string FileExceptionErrorMessage = "\"{0}\" couldn't be checked, so you'll need to do that manually."; // {1}"; // thanks naxess

        public static Beatmap.Difficulty[] AllDifficulties = new Beatmap.Difficulty[] {
            Beatmap.Difficulty.Easy,
            Beatmap.Difficulty.Normal,
            Beatmap.Difficulty.Hard,
            Beatmap.Difficulty.Insane,
            Beatmap.Difficulty.Expert,
            // Beatmap.Difficulty.Ultra,
        };

        #region Guidelines
        public static Dictionary<Beatmap.Difficulty, float[]> CsGuidelines = new Dictionary<Beatmap.Difficulty, float[]> {
            { Beatmap.Difficulty.Easy,      new float[] {-1, 4} },
            { Beatmap.Difficulty.Normal,    new float[] {-1, 5} },
            { Beatmap.Difficulty.Hard,      new float[] {-1, 6} },
            { Beatmap.Difficulty.Insane,    new float[] {-1, 7} },
            { Beatmap.Difficulty.Expert,    new float[] {-1, 7} },
            // { Beatmap.Difficulty.Ultra,     new float[] {-1, 7} },
        };

        public static Dictionary<Beatmap.Difficulty, float[]> ArGuidelines = new Dictionary<Beatmap.Difficulty, float[]> {
            { Beatmap.Difficulty.Easy,      new float[] {-1, 5} },
            { Beatmap.Difficulty.Normal,    new float[] {4, 6} },
            { Beatmap.Difficulty.Hard,      new float[] {6, 8} },
            { Beatmap.Difficulty.Insane,    new float[] {7, 9.3f} },
            { Beatmap.Difficulty.Expert,    new float[] {8, -1} },
            // { Beatmap.Difficulty.Ultra,     new float[] {8, -1} },
        };

        public static Dictionary<Beatmap.Difficulty, float[]> OdGuidelines = new Dictionary<Beatmap.Difficulty, float[]> {
            { Beatmap.Difficulty.Easy,      new float[] {1, 3} },
            { Beatmap.Difficulty.Normal,    new float[] {3, 5} },
            { Beatmap.Difficulty.Hard,      new float[] {5, 7} },
            { Beatmap.Difficulty.Insane,    new float[] {7, 9} },
            { Beatmap.Difficulty.Expert,    new float[] {8, -1} },
            // { Beatmap.Difficulty.Ultra,     new float[] {8, -1} },
        };

        public static Dictionary<Beatmap.Difficulty, float[]> HpGuidelines = new Dictionary<Beatmap.Difficulty, float[]> {
            { Beatmap.Difficulty.Easy,      new float[] {1, 3} },
            { Beatmap.Difficulty.Normal,    new float[] {3, 5} },
            { Beatmap.Difficulty.Hard,      new float[] {4, 6} },
            { Beatmap.Difficulty.Insane,    new float[] {5, 8} },
            { Beatmap.Difficulty.Expert,    new float[] {5, -1} },
            // { Beatmap.Difficulty.Ultra,     new float[] {5, -1} },
        };
        #endregion
    }
}
