using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Tests
{
    public class NpcAudioImportSettingsTest
    {
        const string NpcComponentsRoot = "Assets/Components/NPCs";
        const int ExpectedSampleRateHz = 22050;

        [Test]
        public void AllNpcAudioClipsUseCompressedInMemoryAdpcmMonoAt22050Hz()
        {
            var audioPaths = FindNpcAudioAssetPaths().ToList();
            if (audioPaths.Count == 0)
            {
                Assert.Fail($"No audio clips found under '{NpcComponentsRoot}' in folders named 'Audio'.");
            }

            var fixedPaths = new List<string>();
            foreach (var path in audioPaths)
            {
                if (TryApplyExpectedImportSettings(path))
                {
                    fixedPaths.Add(path);
                }
            }

            if (fixedPaths.Count > 0)
            {
                AssetDatabase.SaveAssets();
            }

            var failures = new List<string>();
            foreach (var path in audioPaths)
            {
                CollectImportSettingFailures(path, failures);
            }

            if (failures.Count == 0)
            {
                return;
            }

            var message = new StringBuilder();
            message.AppendLine(
                $"Expected NPC audio import settings: Compressed In Memory, Force To Mono, ADPCM, {ExpectedSampleRateHz} Hz.");
            message.AppendLine($"Checked {audioPaths.Count} clip(s) under '{NpcComponentsRoot}'.");
            if (fixedPaths.Count > 0)
            {
                message.AppendLine($"Reimported {fixedPaths.Count} clip(s) with corrected settings:");
                foreach (var path in fixedPaths)
                {
                    message.AppendLine($"  {path}");
                }
            }

            foreach (var failure in failures)
            {
                message.AppendLine(failure);
            }

            Assert.Fail(message.ToString());
        }

        static IEnumerable<string> FindNpcAudioAssetPaths()
        {
            return AssetDatabase.FindAssets("t:AudioClip", new[] { NpcComponentsRoot })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => path.Contains("/Audio/"))
                .OrderBy(path => path);
        }

        static bool TryApplyExpectedImportSettings(string path)
        {
            var importer = AssetImporter.GetAtPath(path) as AudioImporter;
            if (importer == null)
            {
                return false;
            }

            if (!NeedsImportSettingsFix(importer))
            {
                return false;
            }

            importer.forceToMono = true;

            var settings = importer.defaultSampleSettings;
            settings.loadType = AudioClipLoadType.CompressedInMemory;
            settings.compressionFormat = AudioCompressionFormat.ADPCM;
            settings.sampleRateSetting = AudioSampleRateSetting.OverrideSampleRate;
            settings.sampleRateOverride = ExpectedSampleRateHz;
            importer.defaultSampleSettings = settings;

            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();
            return true;
        }

        static bool NeedsImportSettingsFix(AudioImporter importer)
        {
            var settings = importer.defaultSampleSettings;

            if (!importer.forceToMono)
            {
                return true;
            }

            if (settings.loadType != AudioClipLoadType.CompressedInMemory)
            {
                return true;
            }

            if (settings.compressionFormat != AudioCompressionFormat.ADPCM)
            {
                return true;
            }

            if (settings.sampleRateSetting != AudioSampleRateSetting.OverrideSampleRate)
            {
                return true;
            }

            if (settings.sampleRateOverride != ExpectedSampleRateHz)
            {
                return true;
            }

            return false;
        }

        static void CollectImportSettingFailures(string path, ICollection<string> failures)
        {
            var importer = AssetImporter.GetAtPath(path) as AudioImporter;
            if (importer == null)
            {
                failures.Add($"{path}: expected an AudioImporter.");
                return;
            }

            var settings = importer.defaultSampleSettings;

            if (!importer.forceToMono)
            {
                failures.Add($"{path}: Force To Mono must be enabled.");
            }

            if (settings.loadType != AudioClipLoadType.CompressedInMemory)
            {
                failures.Add(
                    $"{path}: Load Type must be Compressed In Memory (was {settings.loadType}).");
            }

            if (settings.compressionFormat != AudioCompressionFormat.ADPCM)
            {
                failures.Add(
                    $"{path}: Compression Format must be ADPCM (was {settings.compressionFormat}).");
            }

            if (settings.sampleRateSetting != AudioSampleRateSetting.OverrideSampleRate)
            {
                failures.Add(
                    $"{path}: Sample Rate Setting must override to {ExpectedSampleRateHz} Hz (was {settings.sampleRateSetting}).");
            }

            if (settings.sampleRateOverride != ExpectedSampleRateHz)
            {
                failures.Add(
                    $"{path}: Sample Rate Override must be {ExpectedSampleRateHz} Hz (was {settings.sampleRateOverride}).");
            }
        }
    }
}
