using Godot;
using static Godot.GD; // for print
using System;

public partial class GameManager : Node
{
    /****************************
        GAMEMANAGER VARIABLES
    ****************************/
    
    /*
     * Node declarations
     */
    
    private Player _player;         // player node
    private CanvasLayer _hud;       // HUD node
    private Node _arrowInstance;    // arrow node (for later use in instantiation)

    /*
     * Scene preloads
     */
    
    private SceneTree _tree;            // scene tree
    private PackedScene _arrowPreload;  // arrow preload
    
    
    /***************************
        GAMEMANAGER METHODS
    ***************************/
    
    /*
     * Signal Handlers
     */
    
    // Handle a new node entering the scene tree
    private void OnNodeEnterSceneTreeSignal(Node node)
    {
        string nodeName = node.Name.ToString(); // New node's name (as C# string)
        
        // If node is a new arrow
        if (nodeName.Contains("Arrow"))
        {
            Print("new arrow assigned: " + node.Name);
        }
    }
    
    // Handle player shooting an arrow
    private void OnPlayerShootSignal(int shootDirection, Vector2 spawnLocation)
    {
        Print(shootDirection + " " + (_player.Position + spawnLocation));
        _arrowInstance = _arrowPreload.Instantiate();
        AddSibling(_arrowInstance, true);
    }
    
    
    /*
     * Arrow methods
     */

    private void SpawnArrow()
    {
        // TODO: change arrow behavior such that it spawns at a given point instead player's point automatically
    }
    
    
    /*******************
        GODOT METHODS
    *******************/
    
    public override void _EnterTree()
    {
        /*
         * Node assignments
         */
        
        _tree = GetTree();                                      // Assign Local Scene Tree
        _player = GetNode<Player>("/root/Game/Player"); // Assign player node
        _hud = GetNode<CanvasLayer>("/root/Game/HUD");  // Assign HUD node
        
        /*
         * Arrow preload assignment
         */

        _arrowPreload = Load<PackedScene>("res://scenes/arrow.tscn"); // Assign arrow scene to preload
    }

    public override void _Ready()
    {
        /*
         * Signals
         */
        
        _tree.NodeAdded += OnNodeEnterSceneTreeSignal;  // Assign Node Enter signal
        _player.Shoot += OnPlayerShootSignal;           // Assign Shoot signal
    }
}
