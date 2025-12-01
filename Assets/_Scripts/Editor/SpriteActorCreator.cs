using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering; // 为了使用 ShadowCastingMode 枚举

namespace NineSunsAsh.Editor
{
    public static class SpriteActorCreator
    {
        [MenuItem("GameObject/2.5DTool/Create Sprite Actor", false, 0)]
        private static void CreateSpriteActor(MenuCommand menuCommand)
        {
            // 1.创建父节点（空物体）
            GameObject root = new GameObject("SpriteActorRoot");
            Undo.RegisterCreatedObjectUndo(root, "Create Sprite Actor");
            // 如果有选中物体，就将新物体放在根节点的下面，方便在层级视图中管理
            GameObject parent = menuCommand.context as GameObject;
            if (parent != null)
            {
                GameObjectUtility.SetParentAndAlign(root, parent);
            }

            // 2. 创建子节点 + SpriteRender组件
            var spriteChild = new GameObject("SpriteRender");
            Undo.RegisterCreatedObjectUndo(spriteChild, "Create Sprite Child");
            spriteChild.transform.SetParent(root.transform, false);
            spriteChild.transform.rotation = Quaternion.Euler(45f, 0f, 0f);

            SpriteRenderer spriteRenderer = spriteChild.AddComponent<SpriteRenderer>();

            // 3. 给 SpriteRenderer 指定一个默认的阴影材质
            const string materialPath = "Assets/Shaders/M_ShadowLit.mat";
            var defaultMat = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
            if (defaultMat != null)
            {
                // 使用 sharedMaterial 避免在编辑器里无意中实例化材质
                spriteRenderer.sharedMaterial = defaultMat;
            }
            else
            {
                Debug.LogWarning($"SpriteActorCreator: 没找到默认材质：{materialPath}，请检查路径。");
            }

            // 4. 打开投射和接收阴影
            Renderer renderer = spriteRenderer; // SpriteRenderer 继承自 Renderer
            renderer.shadowCastingMode = ShadowCastingMode.On;
            renderer.receiveShadows = true;
            

            // 5. 选中创建出来的根节点，方便你继续调整
            Selection.activeGameObject = root;
        }
    }
}