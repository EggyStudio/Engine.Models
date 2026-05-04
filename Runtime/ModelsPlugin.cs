namespace Engine;

/// <summary>
/// Aggregator plugin for the model-import backends. Brings up the two third-party
/// importers the engine ships with so that any of the ~45 file formats they cover can
/// be loaded as a <see cref="SceneAsset"/> via the <see cref="AssetServer"/>:
/// </summary>
/// <remarks>
/// <para>
/// <b>Module split (matches the project pattern of e.g. <c>Engine.Scenes</c> +
/// <c>Engine.Scenes.Usd</c>):</b>
/// </para>
/// <list type="bullet">
///   <item><description>
///     <c>Engine.Models</c> (this module) - aggregator plugin and any
///     model-specific payload extensions (skinning, skeletons, animation curves)
///     that go beyond the generic geometry / material / camera / light vocabulary
///     already in <c>Engine.Scenes</c>.
///   </description></item>
///   <item><description>
///     <c>Engine.Models.Assimp</c> - Open Asset Import Library backend
///     (<c>AssimpModelReader</c> + <c>AssimpModelLoader</c>). Covers ~40 long-tail
///     formats: FBX, OBJ, COLLADA (.dae), 3DS, BLEND, PLY, STL, X, MD2/MD3/MD5, IFC,
///     LWO, etc. Lossy on PBR material graphs but unbeatable for breadth.
///   </description></item>
///   <item><description>
///     <c>Engine.Models.Gltf</c> - SharpGLTF backend
///     (<c>GltfModelReader</c> + <c>GltfModelLoader</c>). Covers <c>.gltf</c> /
///     <c>.glb</c> with full PBR, KHR_* extensions, skinning and morph targets.
///     Registered <i>after</i> Assimp so glTF files prefer the higher-fidelity path
///     (last-registration wins in <see cref="AssetServer.RegisterLoader{T}"/>).
///   </description></item>
/// </list>
/// <para>
/// <b>Scene format (USD):</b> <c>.usd / .usda / .usdc / .usdz</c> stay owned by
/// <see cref="UsdScenesPlugin"/> in <c>Engine.Scenes.Usd</c>. The backends here do not
/// register for those extensions even though Assimp can technically parse some of them.
/// </para>
/// <para>
/// <b>Order:</b> add <i>after</i> <see cref="ScenesPlugin"/> and <see cref="AssetPlugin"/>;
/// <see cref="DefaultPlugins"/> wires this up. Standalone consumers can opt in:
/// <code>
/// app.AddPlugin(new ScenesPlugin())
///    .AddPlugin(new ModelsPlugin());
/// </code>
/// </para>
/// </remarks>
/// <seealso cref="ScenesPlugin"/>
/// <seealso cref="AssimpModelPlugin"/>
/// <seealso cref="GltfModelPlugin"/>
public sealed class ModelsPlugin : IPlugin
{
    private static readonly ILogger Logger = Log.Category("Engine.Models");

    /// <inheritdoc />
    public void Build(App app)
    {
        Logger.Info("ModelsPlugin: Wiring model-import backends...");

        // Order matters: Assimp registers .gltf/.glb too, but the glTF-specific backend
        // is registered second so it wins last-write semantics in AssetServer.RegisterLoader.
        app.AddPlugin(new AssimpModelPlugin());
        app.AddPlugin(new GltfModelPlugin());

        Logger.Info("ModelsPlugin: Model backends ready.");
    }
}