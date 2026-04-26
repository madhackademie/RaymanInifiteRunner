using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Tests EditMode de diagnostic pour comprendre les casses quand un PlantDefinition "Laitue" est supprimé.
/// </summary>
public class LettuceAssetReferencesTests
{
    private const string ActiveLettuceAssetPath = "Assets/Data/Ferme/Laitue.asset";
    private const string LegacyLettuceAssetPath = "Assets/Scripts/Data/Laitue.asset";
    private const string LettucePrefabPath = "Assets/Prefabs/World/Plantes/LaitueObj.prefab";
    private const string SeedSelectionPrefabPath = "Assets/Prefabs/Ui/SeedSelectionUI.prefab";
    private const string LettucePlantId = "lettuce";

    [Test]
    public void LettuceWorldPrefab_HasBothDefinitionReferencesAssigned()
    {
        GameObject lettucePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(LettucePrefabPath);
        Assert.That(lettucePrefab, Is.Not.Null, $"Prefab introuvable: {LettucePrefabPath}");

        PlantGrow plantGrow = lettucePrefab.GetComponent<PlantGrow>();
        Assert.That(plantGrow, Is.Not.Null, "Le prefab laitue doit avoir un composant PlantGrow.");

        PlantDefinitionHolder holder = lettucePrefab.GetComponent<PlantDefinitionHolder>();
        Assert.That(holder, Is.Not.Null, "Le prefab laitue doit avoir un composant PlantDefinitionHolder.");

        Object growDefinition = new SerializedObject(plantGrow).FindProperty("plantDefinition")?.objectReferenceValue;
        Object holderDefinition = new SerializedObject(holder).FindProperty("definition")?.objectReferenceValue;

        Assert.That(growDefinition, Is.Not.Null, "PlantGrow.plantDefinition est null (référence cassée).");
        Assert.That(holderDefinition, Is.Not.Null, "PlantDefinitionHolder.definition est null (référence cassée).");
    }

    [Test]
    public void SeedSelectionPrefab_HasLettuceEntryWithValidReferences()
    {
        GameObject seedSelectionPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(SeedSelectionPrefabPath);
        Assert.That(seedSelectionPrefab, Is.Not.Null, $"Prefab introuvable: {SeedSelectionPrefabPath}");

        SeedSelectionUI seedSelectionUI = seedSelectionPrefab.GetComponent<SeedSelectionUI>();
        Assert.That(seedSelectionUI, Is.Not.Null, "Le prefab SeedSelectionUI doit avoir un composant SeedSelectionUI.");

        SerializedProperty availableSeeds = new SerializedObject(seedSelectionUI).FindProperty("availableSeeds");
        Assert.That(availableSeeds, Is.Not.Null, "La propriété serialized 'availableSeeds' est introuvable.");

        bool found = false;
        for (int i = 0; i < availableSeeds.arraySize; i++)
        {
            SerializedProperty element = availableSeeds.GetArrayElementAtIndex(i);
            Object definitionRef = element.FindPropertyRelative("plantDefinition")?.objectReferenceValue;
            Object prefabRef = element.FindPropertyRelative("plantPrefab")?.objectReferenceValue;
            if (definitionRef == null || prefabRef == null)
                continue;

            if (definitionRef is not PlantDefinition definition || definition.plantId != LettucePlantId)
                continue;

            found = true;
            break;
        }

        Assert.That(
            found,
            Is.True,
            "Aucune entrée SeedSelectionUI valide n'a été trouvée pour plantId='lettuce'."
        );
    }

    [Test]
    public void LettuceReferences_AcrossPrefabs_PointToSamePlantDefinitionAsset()
    {
        Object worldDefinition = GetWorldPrefabPlantGrowDefinition();
        Object seedDefinition = GetSeedSelectionLettuceDefinition();

        Assert.That(worldDefinition, Is.Not.Null, "Référence PlantGrow manquante dans le prefab monde.");
        Assert.That(seedDefinition, Is.Not.Null, "Référence laitue manquante dans SeedSelectionUI.");

        string worldPath = AssetDatabase.GetAssetPath(worldDefinition);
        string seedPath = AssetDatabase.GetAssetPath(seedDefinition);
        Assert.That(
            worldPath,
            Is.EqualTo(seedPath),
            $"Références incohérentes: world='{worldPath}' vs seedSelection='{seedPath}'."
        );
    }

    [Test]
    public void ActiveLettuceAsset_HasKnownDependents()
    {
        string[] dependentAssets = FindDependents(ActiveLettuceAssetPath);
        Assert.That(
            dependentAssets,
            Does.Contain(LettucePrefabPath),
            $"Le prefab monde attendu n'est pas dépendant de {ActiveLettuceAssetPath}."
        );
        Assert.That(
            dependentAssets,
            Does.Contain(SeedSelectionPrefabPath),
            $"Le prefab SeedSelection attendu n'est pas dépendant de {ActiveLettuceAssetPath}."
        );
    }

    [Test]
    public void LegacyLettuceAsset_HasBeenRemoved()
    {
        Assert.That(
            AssetDatabase.LoadAssetAtPath<PlantDefinition>(LegacyLettuceAssetPath),
            Is.Null,
            $"L'asset legacy doit être supprimé: {LegacyLettuceAssetPath}"
        );
    }

    [Test, Explicit("Diagnostic manuel: détecte les plantId dupliqués qui causent des ambiguïtés à la suppression.")]
    public void PlantDefinitions_Diagnostic_HasNoDuplicatePlantIds()
    {
        string[] guids = AssetDatabase.FindAssets("t:PlantDefinition");
        var seenByPlantId = new Dictionary<string, string>();
        var duplicateMessages = new List<string>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            PlantDefinition definition = AssetDatabase.LoadAssetAtPath<PlantDefinition>(path);
            if (definition == null || string.IsNullOrWhiteSpace(definition.plantId))
                continue;

            if (seenByPlantId.TryGetValue(definition.plantId, out string firstPath))
            {
                duplicateMessages.Add($"plantId '{definition.plantId}' dupliqué: '{firstPath}' et '{path}'.");
                continue;
            }

            seenByPlantId.Add(definition.plantId, path);
        }

        Assert.That(
            duplicateMessages,
            Is.Empty,
            "Des PlantDefinition partagent le même plantId. " +
            "Cause probable de comportements instables après suppression:\n" +
            string.Join("\n", duplicateMessages)
        );
    }

    private static Object GetWorldPrefabPlantGrowDefinition()
    {
        GameObject lettucePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(LettucePrefabPath);
        if (lettucePrefab == null)
            return null;

        PlantGrow grow = lettucePrefab.GetComponent<PlantGrow>();
        if (grow == null)
            return null;

        return new SerializedObject(grow).FindProperty("plantDefinition")?.objectReferenceValue;
    }

    private static Object GetSeedSelectionLettuceDefinition()
    {
        GameObject seedSelectionPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(SeedSelectionPrefabPath);
        if (seedSelectionPrefab == null)
            return null;

        SeedSelectionUI seedSelectionUI = seedSelectionPrefab.GetComponent<SeedSelectionUI>();
        if (seedSelectionUI == null)
            return null;

        SerializedProperty availableSeeds = new SerializedObject(seedSelectionUI).FindProperty("availableSeeds");
        if (availableSeeds == null)
            return null;

        for (int i = 0; i < availableSeeds.arraySize; i++)
        {
            SerializedProperty element = availableSeeds.GetArrayElementAtIndex(i);
            Object definitionRef = element.FindPropertyRelative("plantDefinition")?.objectReferenceValue;
            if (definitionRef is PlantDefinition definition && definition.plantId == LettucePlantId)
                return definitionRef;
        }

        return null;
    }

    private static string[] FindDependents(string targetAssetPath)
    {
        var dependents = new List<string>();
        if (string.IsNullOrEmpty(targetAssetPath))
            return dependents.ToArray();

        foreach (string path in AssetDatabase.GetAllAssetPaths())
        {
            if (!path.StartsWith("Assets/") || path == targetAssetPath)
                continue;

            string[] dependencies = AssetDatabase.GetDependencies(path, recursive: false);
            foreach (string dependency in dependencies)
            {
                if (dependency == targetAssetPath)
                {
                    dependents.Add(path);
                    break;
                }
            }
        }

        return dependents.ToArray();
    }
}
