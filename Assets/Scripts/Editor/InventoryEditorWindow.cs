using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using InventoryNamespace;

public class InventoryEditorWindow : EditorWindow {

    private InventoryDatabase inventoryDatabase;

    #region HeaderVariables

    private bool createInventory, itemDatabase, craftSystem = false;

    #endregion

    #region CreateInventoryVariables

    private string itemListName;
    private string itemListDirectory;

    #endregion

    #region ItemDatabaseVariables

    private Item addedItem = new Item();
    private Item updatedItem = new Item();
    private bool addItem, updateItem, deleteItem = false;
    private int selectedItemIndex;

    #endregion

    #region CraftSystemVariables

    private bool addBlueprint, updateBlueprint, deleteBlueprint = false;
    private int resultItemIndex = 0;
    private int resultItemCount = 0;
    private int neededItemNumber = 0;

    private List<int> neededItemIndexes = new List<int>();
    private List<int> neededItemCounts = new List<int>();

    private Blueprint blueprint = new Blueprint();

    #endregion

    [MenuItem("Inventory/Inventory System")]
    public static void ShowItemEditorWindow()
    {
        var editorWindow = GetWindow<InventoryEditorWindow>();
        editorWindow.titleContent = new GUIContent("Inventory", "Tooltip");
        editorWindow.minSize = new Vector2(800f, 400f);
    }

    public void OnEnable()
    {
        if (EditorPrefs.HasKey("ObjectPath"))
        {
            string objectPath = EditorPrefs.GetString("ObjectPath");
            inventoryDatabase = AssetDatabase.LoadAssetAtPath(objectPath, typeof(InventoryDatabase)) as InventoryDatabase;
        }
    }

    public void OnGUI()
    {
        Header();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(inventoryDatabase);
        }
    }

    private void Header() {
        var pressedStyle = new GUIStyle(GUI.skin.button);
        pressedStyle.fontStyle = FontStyle.Bold;
        pressedStyle.fixedHeight = 26.5f;

        GUILayout.Space(7.5f);
        GUILayout.BeginHorizontal();

        var createInventoryButton = createInventory ? GUILayout.Button("Create Inventory", pressedStyle) : GUILayout.Button("Create Inventory", GUILayout.Height(24f));
        if (createInventoryButton) {
            EnableAllMainWindows(false);
            createInventory = !createInventory;
        }

        var itemDatabaseButton = itemDatabase ? GUILayout.Button("Item Database", pressedStyle) : GUILayout.Button("Item Database", GUILayout.Height(24f));
        if (itemDatabaseButton) {
            EnableAllMainWindows(false);
            itemDatabase = !itemDatabase;
        }

        var craftSystemButton = craftSystem ? GUILayout.Button("Craft System", pressedStyle) : GUILayout.Button("Craft System", GUILayout.Height(24f));
        if (craftSystemButton)
        {
            EnableAllMainWindows(false);
            craftSystem = !craftSystem;
        }

        GUILayout.EndHorizontal();

        if (createInventory) { ShowCreateInventoryWindow(); }
        if (itemDatabase) { ShowItemDatabaseWindow(); }
        if (craftSystem) { ShowCraftSystemWindow(); }

    }

    #region CreateInventoryFunctions

    private void ShowCreateInventoryWindow()
    {
        EditorGUILayout.BeginVertical("Box");
        GUILayout.Label("Create Inventory", EditorStyles.boldLabel);

        var pressedStyle = new GUIStyle(GUI.skin.button);
        pressedStyle.alignment = TextAnchor.MiddleCenter;
        pressedStyle.fontStyle = FontStyle.Normal;
        pressedStyle.fontSize = 12;
        pressedStyle.margin = new RectOffset(100, 100, 0, 0);
        pressedStyle.fixedHeight = 40f;

        GUILayout.BeginHorizontal();
        itemListDirectory = EditorGUILayout.TextField("Inventory Folder", itemListDirectory);

        var selectDirectoryButton = GUILayout.Button("Browse...", GUILayout.Width(75f), GUILayout.Height(13.5f));
        if (selectDirectoryButton)
        {
            itemListDirectory = EditorUtility.OpenFolderPanel("Select Directory", "", "");
        }
        GUILayout.EndHorizontal();

        itemListName = EditorGUILayout.TextField("Inventory Name", itemListName);

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        var createInventoryButton = GUILayout.Button("Create Inventory", pressedStyle);
        GUILayout.EndHorizontal();

        if (createInventoryButton)
        {
            inventoryDatabase = CreateItemListData(itemListDirectory, (itemListName));
            if (inventoryDatabase)
            {
                inventoryDatabase.itemList = new List<Item>();
                string relPath = AssetDatabase.GetAssetPath(inventoryDatabase);
                EditorPrefs.SetString("ObjectPath", relPath);
            }
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    #endregion

    #region ItemDatabaseFunctions

    private void ShowItemDatabaseWindow()
    {
        var pressedStyle = new GUIStyle(GUI.skin.button);
        pressedStyle.fontStyle = FontStyle.Bold;
        pressedStyle.fixedHeight = 20f;

        EditorGUILayout.BeginVertical("Box");
        GUILayout.Label("Item Database", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        EditorPrefs.SetString("ObjectPath", EditorGUILayout.TextField("Database Location", EditorPrefs.GetString("ObjectPath")));
        var selectDirectoryButton = GUILayout.Button("Browse...", GUILayout.Width(75f), GUILayout.Height(13.5f));
        if (selectDirectoryButton)
        {
            OpenItemListData();
        }
        GUILayout.EndHorizontal();
 
        GUILayout.BeginHorizontal();
        var addItemButton = addItem ? GUILayout.Button("Add Item", pressedStyle) : GUILayout.Button("Add Item");
        if (addItemButton)
        {
            EnableAllItemDatabaseWindows(false);
            addItem = true;
        }
        var updateItemButton = updateItem ? GUILayout.Button("Update Item", pressedStyle) : GUILayout.Button("Update Item");
        if (updateItemButton)
        {
            EnableAllItemDatabaseWindows(false);
            updateItem = true;
        }
        var deleteItemButton = deleteItem ? GUILayout.Button("Delete Item", pressedStyle) : GUILayout.Button("Delete Item");
        if (deleteItemButton)
        {
            EnableAllItemDatabaseWindows(false);
            deleteItem = true;
        }
        GUILayout.EndHorizontal();

        if (addItem) OpenAddItemWindow();
        else if (updateItem) OpenUpdateItemWindow();
        else if (deleteItem) OpenDeleteItemWindow();

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }

    private void OpenAddItemWindow()
    {
        GUIStyle boxStyle = new GUIStyle("Box");
        boxStyle.fontSize = 20;
        GUILayout.BeginVertical(boxStyle, GUILayout.Width(position.width - 23));

        CreateGUIForItem(ref addedItem);

        GUILayout.EndVertical();

        if (GUILayout.Button("ADD")) AddItem();

    }

    private void OpenUpdateItemWindow()
    {
        GUIStyle boxStyle = new GUIStyle("Box");
        boxStyle.fontSize = 20;
        GUILayout.BeginVertical(boxStyle, GUILayout.Width(position.width - 23));

        if (GetItemNames() != null)
        {
            selectedItemIndex = EditorGUILayout.Popup("Select Item", selectedItemIndex, GetItemNames());
            updatedItem = inventoryDatabase.itemList[selectedItemIndex];
            CreateGUIForItem(ref updatedItem);
        }
        else
            selectedItemIndex = EditorGUILayout.Popup("Select Item", selectedItemIndex, new string[0]);

        GUILayout.EndVertical();
    }

    private void OpenDeleteItemWindow()
    {
        GUIStyle boxStyle = new GUIStyle("Box");
        boxStyle.fontSize = 20;
        GUILayout.BeginVertical(boxStyle, GUILayout.Width(position.width - 23));

        if (GetItemNames() != null)
        {
            selectedItemIndex = EditorGUILayout.Popup("Select Item", selectedItemIndex, GetItemNames());
            if (GUILayout.Button("Delete")) DeleteItem(selectedItemIndex);
        }
        else
            selectedItemIndex = EditorGUILayout.Popup("Select Item", selectedItemIndex, new string[0]);

        GUILayout.EndVertical();
    }

    private void AddItem()
    {
        Item item = new Item(addedItem);
        inventoryDatabase.itemList.Add(item);
    }

    private void UpdateItem()
    {
        Item item = new Item(addedItem);
        inventoryDatabase.itemList.Add(item);
    }

    private void DeleteItem(int index)
    {
        inventoryDatabase.itemList.RemoveAt(index);
    }

    private void CreateGUIForItem(ref Item item)
    {
        item.itemName = EditorGUILayout.TextField("Item Name", item.itemName, GUILayout.Width(position.width - 30));

        GUILayout.BeginHorizontal();
        GUILayout.Label("Item Description");
        GUILayout.Space(47);
        item.itemDescription = EditorGUILayout.TextArea(item.itemDescription, GUILayout.Width(position.width - 180), GUILayout.Height(70));
        GUILayout.EndHorizontal();

        item.itemIcon = (Sprite)EditorGUILayout.ObjectField("Item Icon", item.itemIcon, typeof(Sprite), false);

        item.itemType = (ItemType)EditorGUILayout.EnumPopup("Item Type", item.itemType, GUILayout.Width(position.width - 33));

        GUILayout.BeginHorizontal();
        item.slotProportions.x = EditorGUILayout.IntField("Item Slot Width", (int)item.slotProportions.x);
        item.slotProportions.y = EditorGUILayout.IntField("Item Slot Height", (int)item.slotProportions.y);
        GUILayout.EndHorizontal();

        item.slotMaxSize = EditorGUILayout.IntField("Max Slot Stack", item.slotMaxSize);

        GUILayout.BeginHorizontal();
        item.minAmountInMap = EditorGUILayout.IntField("Map Info: Min Amount", item.minAmountInMap);
        item.maxAmountInMap = EditorGUILayout.IntField("Max Amount", item.maxAmountInMap);
        item.percentageInMap = EditorGUILayout.FloatField("Percentage", item.percentageInMap);
        GUILayout.EndHorizontal();
    }

    #endregion

    #region CraftSystemFunctions

    private void ShowCraftSystemWindow()
    {
        var pressedStyle = new GUIStyle(GUI.skin.button);
        pressedStyle.fontStyle = FontStyle.Bold;
        pressedStyle.fixedHeight = 20f;

        EditorGUILayout.BeginVertical("Box");
        GUILayout.Label("Craft System", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        EditorPrefs.SetString("ObjectPath", EditorGUILayout.TextField("Database Location", EditorPrefs.GetString("ObjectPath")));
        var selectDirectoryButton = GUILayout.Button("Browse...", GUILayout.Width(75f), GUILayout.Height(13.5f));
        if (selectDirectoryButton)
        {
            OpenItemListData();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        var addBlueprintButton = addBlueprint ? GUILayout.Button("Add Blueprint", pressedStyle) : GUILayout.Button("Add Blueprint");
        if (addBlueprintButton)
        {
            EnableAllCraftSystemWindows(false);
            addBlueprint = true;
        }
        var updateBlueprintButton = updateBlueprint ? GUILayout.Button("Update Blueprint", pressedStyle) : GUILayout.Button("Update Blueprint");
        if (updateBlueprintButton)
        {
            EnableAllCraftSystemWindows(false);
            updateBlueprint = true;
        }
        var deleteBlueprintButton = deleteBlueprint ? GUILayout.Button("Delete Blueprint", pressedStyle) : GUILayout.Button("Delete Blueprint");
        if (deleteBlueprintButton)
        {
            EnableAllCraftSystemWindows(false);
            deleteBlueprint = true;
        }
        GUILayout.EndHorizontal();

        if (addBlueprint) OpenAddBlueprintWindow();
        else if (updateBlueprint) OpenUpdateBlueprintWindow();
        else if (deleteBlueprint) OpenDeleteBlueprintWindow();

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    private void OpenAddBlueprintWindow()
    {
        EditorGUILayout.BeginVertical("Box");

        GUILayout.BeginHorizontal();
        if (GetItemNames() != null)
            resultItemIndex = EditorGUILayout.Popup("Result Item", resultItemIndex, GetItemNames());
        else
            resultItemIndex = EditorGUILayout.Popup("Result Item", resultItemIndex, new string[0]);

        resultItemCount = EditorGUILayout.IntField("Count", resultItemCount);
        GUILayout.EndHorizontal();

        neededItemNumber = EditorGUILayout.IntField("Needed Item Count", neededItemNumber);
        if (neededItemNumber != 0)
        {
            CreateBlueprntUI(ref neededItemIndexes, ref neededItemCounts, neededItemNumber);
            if (GUILayout.Button("ADD")){ AddBlueprint(); }
        }
        GUILayout.EndHorizontal();
    }

    private void OpenUpdateBlueprintWindow()
    {
        EditorGUILayout.BeginVertical("Box");

        GUILayout.BeginHorizontal();
        if (GetBlueprintNames() != null)
            resultItemIndex = EditorGUILayout.Popup("Result Item", resultItemIndex, GetBlueprintNames());
        else
            resultItemIndex = EditorGUILayout.Popup("Result Item", resultItemIndex, new string[0]);

        GUILayout.EndHorizontal();
        neededItemIndexes = GetNeededItemCountsOfBlueprint(resultItemIndex);
        neededItemCounts = GetNeededItemCountsOfBlueprint(resultItemIndex);
        CreateBlueprntUI(ref neededItemIndexes, ref neededItemCounts, resultItemIndex);
        GUILayout.EndHorizontal();

    }

    private void OpenDeleteBlueprintWindow()
    {

    }

    private void AddBlueprint()
    {
        blueprint.resultItem = new CraftItem();
        blueprint.resultItem.item = inventoryDatabase.itemList[resultItemIndex];
        blueprint.resultItem.count = resultItemCount;

        for (int i=0; i<neededItemIndexes.Count; i++)
        {
            CraftItem neededItem = new CraftItem();
            neededItem.item = inventoryDatabase.itemList[neededItemIndexes[i]];
            neededItem.count = neededItemCounts[i];
            blueprint.neededItems.Add(neededItem);
        }
        inventoryDatabase.craftBlueprints.Add(new Blueprint(blueprint.resultItem, blueprint.neededItems));

        blueprint.Clear();
        neededItemNumber = 0;
        neededItemIndexes.Clear();
        neededItemCounts.Clear();
    }

    private void CreateBlueprntUI(ref List<int> itemIndexes, ref List<int> itemCounts, int itemNumber)
    {
        if (itemNumber < itemIndexes.Count)
        {
            for (int i= itemNumber; i<itemIndexes.Count; i++) {
                itemIndexes.RemoveAt(i);
                itemCounts.RemoveAt(i);
            }
        }
        else if (itemNumber > itemIndexes.Count)
        {
            int arraySize = itemIndexes.Count;
            for (int i = arraySize; i < itemNumber; i++)
            {
                itemIndexes.Add(0);
                itemCounts.Add(0);
            }
        }
        for (int i=0; i< itemIndexes.Count; i++)
        {
            GUILayout.BeginHorizontal();
            itemIndexes[i] = EditorGUILayout.Popup("Needed Item", itemIndexes[i], GetItemNames());
            itemCounts[i] = EditorGUILayout.IntField("Count", itemCounts[i]);
            GUILayout.EndHorizontal();
        }
    }

    #endregion

    public static InventoryDatabase CreateItemListData(string directory, string name)
    {
        string relPath = directory.Substring(Application.dataPath.Length - "Assets".Length);
        string createAssetString = relPath + "/" + name + ".asset";

        InventoryDatabase asset = ScriptableObject.CreateInstance<InventoryDatabase>();
        AssetDatabase.CreateAsset(asset, createAssetString);
        AssetDatabase.SaveAssets();

        return asset;
    }

    private void OpenItemListData()
    {
        string absPath = EditorUtility.OpenFilePanel("Select Inventory Item List", "", "");
        if (absPath.StartsWith(Application.dataPath))
        {
            itemDatabase = (InventoryDatabase)Resources.Load(EditorPrefs.GetString("ObjectPath"));

            string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
            inventoryDatabase = AssetDatabase.LoadAssetAtPath(relPath, typeof(InventoryDatabase)) as InventoryDatabase;
            if (inventoryDatabase.itemList == null)
                inventoryDatabase.itemList = new List<Item>();
            if (inventoryDatabase)
            {
                EditorPrefs.SetString("ObjectPath", relPath);
            }
        }
    }

    private string[] GetItemNames()
    {
        if (inventoryDatabase == null) return null;
        else if (inventoryDatabase.itemList.Count == 0) return null;

        string[] itemNames = new string[inventoryDatabase.itemList.Count];
        for (int i = 0; i < itemNames.Length; i++)
            itemNames[i] = inventoryDatabase.itemList[i].itemName;
        return itemNames;
    }

    private string[] GetBlueprintNames()
    {
        if (inventoryDatabase == null) return null;
        else if (inventoryDatabase.craftBlueprints.Count == 0) return null;

        string[] blueprintNames = new string[inventoryDatabase.craftBlueprints.Count];
        for (int i = 0; i < blueprintNames.Length; i++)
        {
            CraftItem resultItem = inventoryDatabase.craftBlueprints[i].resultItem;
            blueprintNames[i] = resultItem.item.itemName + ", Count:" + resultItem.count;
        }
        return blueprintNames;
    }

    private List<int> GetNeededItemIndexesOfBlueprint(int blueprintIndex)
    {
        List<int> indexes = new List<int>();
        for (int i=0; i< inventoryDatabase.craftBlueprints[blueprintIndex].neededItems.Count; i++)
        {
            for (int j=0; j< inventoryDatabase.itemList.Count; j++)
            {
                if (inventoryDatabase.craftBlueprints[blueprintIndex].neededItems[i].item.itemName ==
                   inventoryDatabase.itemList[j].itemName)
                {
                    indexes.Add(j);
                    break;
                }
            }
        }
        return indexes;
    }

    private List<int> GetNeededItemCountsOfBlueprint(int blueprintIndex)
    {
        List<int> counts = new List<int>();
        for (int i = 0; i < inventoryDatabase.craftBlueprints[blueprintIndex].neededItems.Count; i++)
        {
            counts.Add(inventoryDatabase.craftBlueprints[blueprintIndex].neededItems[i].count);
        }
        return counts;
    }

    private void EnableAllMainWindows(bool active)
    {
        createInventory = active;
        itemDatabase = active;
        craftSystem = active;
    }

    private void EnableAllItemDatabaseWindows(bool active)
    {
        addItem = active;
        updateItem = active;
        deleteItem = active;
    }

    private void EnableAllCraftSystemWindows(bool active)
    {
        addBlueprint = active;
        updateBlueprint = active;
        deleteBlueprint = active;
    }

}