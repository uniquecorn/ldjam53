using Sirenix.OdinInspector.Editor;
using UnityEditor;

public class DataWindow : OdinEditorWindow
{
    protected override object GetTarget() => ItemData.Instance;
    [MenuItem("Window/Data Window")]
    static void Init() => GetWindow<DataWindow>().Show();
}