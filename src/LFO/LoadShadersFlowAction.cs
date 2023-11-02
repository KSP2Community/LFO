﻿using UnityEngine;
using KSP.Game.Flow;
using Shapes;
using SpaceWarp.API.Assets;

namespace LFO;

public class LoadShadersFlowAction : FlowAction
{
    private readonly List<string> _requestedShaders;

    public LoadShadersFlowAction(List<string> requestedShaders) : base("Loading LFO Shaders...")
    {
        _requestedShaders = requestedShaders;
    }

    public override void DoAction(Action resolve, Action<string> reject)
    {
        LFOPlugin.Log($"Loading LFO Shaders. " + string.Join(", ", _requestedShaders));
        foreach (string toLoad in _requestedShaders)
        {
            if (Shared.LFO.Instance.LoadedShaders.ContainsKey(toLoad))
            {
                continue;
            }

            string path = Shared.LFO.ShadersPath + toLoad.Replace('/', '-') + ".mat";
            Material material;
            try
            {
                material = AssetManager.GetAsset<Material>(path);
            }
            catch (IndexOutOfRangeException)
            {
                LFOPlugin.LogError(
                    $"Error loading {toLoad}. Shader material doesn't exist or can't be found.\n Key: {path}"
                );
                continue;
            }

            if (material == null)
            {
                LFOPlugin.LogError($"Error loading {toLoad}. Loaded object at {path} is not a material!");
                continue;
            }

            if (material.shader == null)
            {
                LFOPlugin.LogError(
                    $"Error loading {toLoad}. Loaded object at {path}'s material doesn't have a shader!");
                continue;
            }

            if (material.shader.name.ToLower() != toLoad.ToLower())
            {
                LFOPlugin.LogWarning(
                    $"Shader name '{material.shader.name.ToLower()}' is different from key '{toLoad.ToLower()}'!"
                );
            }

            Shared.LFO.Instance.LoadedShaders.Add(toLoad, material.shader);
        }

        Shared.LFO.Instance.LoadedShaders.Keys.ForEach(a => _requestedShaders.Remove(a));

        LFOPlugin.Log($"LFO Shaders loaded. " + string.Join(", ", Shared.LFO.Instance.LoadedShaders.Keys));

        resolve();
    }
}