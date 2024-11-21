using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Net.Cache;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Tile_Map_Create : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Text debugText;
    public List<GameObject> monster;
    public List<Spawn_Ctrl> spawn_list;
    public GameObject portal;
    // public static Tile_Map_Create instance = null;
    int position_count = 0;
    public Tilemap Tilemap;
    public TileBase wall;
    public TileBase Floor,
                    Side,
                    Corner,
                    Pillar,
                    Ground,
                    Clear_Portal;
    public TileBase road;
    public int horizontal = 80, vertical = 80; //변경
    public bool is_spawn = false;
    public bool is_exit = false;
    [SerializeField] float minimumDevideRate = 0.4f; //공간이 나눠지는 최소 비율
    [SerializeField] float maximumDivideRate = 0.6f; //공간이 나눠지는 최대 비율
    int Max_Depth = 2;
    GameObject newPortal;
    int map_count = 0;
    int temp_spawn = 0;
    int spawn_count = 0;
    public enum Player_Type
    {
        Parry,
        Dodge,
        Run
    }


    void Awake()
    {
        
    }
    void Start()
    {
        Tilemap.ClearAllTiles();
        player = GameManager.PlayerManager.Player;
    }
    public void Tile_Node(Map_Node root)
    {

        root.node = new TileNode(0, 0);
        Divide_Tile(root, root.node, 0);
        ConnectRooms(root, root.node.leftNode);
        ConnectRooms(root, root.node.rightNode);
        //Make_Tile(root);
    }

    public void Divide_Tile(Map_Node parent, TileNode root, int n)
    {

        if (n == Max_Depth)
        {
            GenerateRoom(parent, root);
            return; //내가 원하는 높이에 도달하면 더 나눠주지 않는다.
        }
        //그 외의 경우에는

        int maxLength = Mathf.Max(root.height, root.width);
        //가로와 세로중 더 긴것을 구한후, 가로가 길다면 위 좌, 우로 세로가 더 길다면 위, 아래로 나눠주게 될 것이다.
        int split = Mathf.RoundToInt(Random.Range(maxLength * minimumDevideRate, maxLength * maximumDivideRate));
        //나올 수 있는 최대 길이와 최소 길이중에서 랜덤으로 한 값을 선택
        if (root.width >= root.height) //가로가 더 길었던 경우에는 좌 우로 나누게 될 것이며, 이 경우에는 세로 길이는 변하지 않는다.
        {

            root.leftNode = new TileNode(root.x, root.y, split, root.height);
            root.rightNode = new TileNode(root.x + split, root.y, root.width - split, root.height);
            // for(int i = 0 ; i < root.height;i++) parent.tile[root.x+split,root.y+i] = 1;
        }
        else
        {

            root.leftNode = new TileNode(root.x, root.y, root.width, split);
            root.rightNode = new TileNode(root.x, root.y + split, root.width, root.height - split);
            // for(int i = 0 ; i < root.width;i++) parent.tile[root.x+i,root.y+split] = 1;
        }

        root.leftNode.parNode = root;
        root.rightNode.parNode = root;
        Divide_Tile(parent, root.leftNode, n + 1);
        Divide_Tile(parent, root.rightNode, n + 1);
    }

    public void Make_Tile(Map_Node parent)
    {

        for (int i = 0; i < vertical; i++)
        {
            for (int j = 0; j < horizontal; j++)
            {

                Vector3Int tilePosition = new Vector3Int(parent.nodeRect.x + j, parent.nodeRect.y - i, 1);
                TileBase tile = null;
                Matrix4x4 transformMatrix = Matrix4x4.identity; // Default to no rotation

                switch (parent.tile[j, i])
                {
                    case 0:
                        tile = wall;
                        break;
                    case 1: // 0 degrees
                        tile = Corner;
                        transformMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 270), Vector3.one);
                        break;
                    case 2: // 270 degrees
                        tile = Corner;
                        transformMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 0), Vector3.one);
                        break;
                    case 3: // 90 degrees
                        tile = Corner;
                        transformMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 180), Vector3.one);
                        break;
                    case 4: // 180 degrees
                        tile = Corner;
                        transformMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 90), Vector3.one);
                        break;
                    case 5:
                        tile = Floor;
                        transformMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 180), Vector3.one);
                        break;
                    case 6:
                        tile = Side;
                        transformMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 180), Vector3.one);
                        break;
                    case 7:
                        tile = Side;
                        break;
                    case 8:
                        tile = Floor;
                        break;
                    case 9:
                        tile = road;
                        break;
                    case 10:
                        tile = Ground;
                        break;
                    case 98:
                        tile = Clear_Portal;
                        break;
                    case 99:
                        tile = Pillar;
                        break;
                    default:
                        break;
                }

                if (tile != null)
                {
                    Tilemap.SetTile(tilePosition, tile);
                    Tilemap.SetTransformMatrix(tilePosition, transformMatrix);
                }
            }
        }
    }
    private void GenerateRoom(Map_Node parent, TileNode tree)
    {
        int width = Random.Range(tree.width / 2, tree.width - 1);
        //방의 가로 최소 크기는 노드의 가로길이의 절반, 최대 크기는 가로길이보다 1 작게 설정한 후 그 사이 값중 랜덤한 값을 구해준다.
        int height = Random.Range(tree.height / 2, tree.height - 1);
        //높이도 위와 같다.
        tree.x = tree.x + Random.Range(1, tree.width - width);
        //방의 x좌표이다. 만약 0이 된다면 붙어 있는 방과 합쳐지기 때문에,최솟값은 1로 해주고, 최댓값은 기존 노드의 가로에서 방의 가로길이를 빼 준 값이다.
        tree.y = tree.y + Random.Range(1, tree.height - height);
        if (parent.Tile_left == null || parent.Tile_left.x > tree.x) parent.Tile_left = tree;
        if (parent.Tile_Right == null || parent.Tile_Right.x < tree.x) parent.Tile_Right = tree;
        if (parent.Tile_Up == null || parent.Tile_Up.y > tree.y) parent.Tile_Up = tree;
        if (parent.Tile_Down == null || parent.Tile_Down.y < tree.y) parent.Tile_Down = tree;
        //y좌표도 위와 같다.
        tree.width = width;
        tree.height = height;
        FillRoom(parent, tree.x, tree.y, tree.width, tree.height);        
        ChangeRoom(parent, tree.x, tree.y, tree.width, tree.height, PlayerDataAnalyze.Instance.playerType);
        
    }
    private void FillRoom(Map_Node parent, int x, int y, int width, int height)
    { //room의 rect정보를 받아서 tile을 set해주는 함수

        for (int i = x; i < x + width; i++)
        {
            for (int j = y; j < y + height; j++)
            {
                if (i == x)
                {
                    if (j == y) parent.tile[i, j] = 1;
                    else if (j == y + height - 1) parent.tile[i, j] = 2;
                    else parent.tile[i, j] = 6;
                }
                else if (i == x + width - 1)
                {
                    if (j == y) parent.tile[i, j] = 3;
                    else if (j == y + height - 1) parent.tile[i, j] = 4;
                    else parent.tile[i, j] = 7;
                }
                else if (j == y) parent.tile[i, j] = 5;
                else if (j == y + height - 1) parent.tile[i, j] = 8;
                else parent.tile[i, j] = 11;

            }
        }
    }
    private void ChangeRoom(Map_Node parent, int x, int y, int width, int height, string playStyle = null)
    {
        if(map_count %4 ==0) temp_spawn = spawn_count;
        int floor_count = height / 5;
        
        // j 먼저 계산 (바깥쪽 루프)
        for (int j = 1; j <= floor_count; j++)
        {
            bool monster_spawn = false;
            
            int altitude3 = y + (j * 5);
            int startPoint = UnityEngine.Random.Range(4, width / 2);
            int rand = UnityEngine.Random.Range(width / 4, width / 2);
            bool is_wall = false;
            int wall_position = -1;

            // i 계산 (안쪽 루프)
            for (int i = x+startPoint; i < x+startPoint + rand; i++)
            {
                if (altitude3 < y + height -2 && i < x + width - 3)
                {
                    if(altitude3 == y + height -2) altitude3 -=1;
                    if (parent.map_type == Map_Node.Map_type.Enterance && !is_spawn)
                    {
                        is_spawn = !is_spawn;
                        monster_spawn =true;
                        if (player == null)
                        {
                            player = GameManager.PlayerManager.Player;
                        }
                        player.transform.position = new Vector3(80 * (position_count / 4) + i + 4, -altitude3+3, 1);
                    }
                    else if (parent.map_type == Map_Node.Map_type.Exit && !is_exit)
                    {
                        is_exit = !is_exit;
                        newPortal = Instantiate(portal);
                        newPortal.transform.position = new Vector3(80 * ((position_count - 48) / 4) + i + 4, -240 - altitude3+2, 1);
                    }
                    if(!monster_spawn)
                    {
                        spawn_count++;
                        monster_spawn =true;
                        Vector3 pos = new Vector3(80 * ((position_count % 16) / 4) + i + (rand/2) , 2-altitude3- (position_count / 16) * 80, 1);
                        ObjectPool.instance.GetObjectFromPool(pos);
                        // GameObject spawn_monster = Instantiate(monster[a]);
                        // spawn_monster.transform.position = new Vector3(80 * ((position_count % 16) / 4) + i + (rand/2) , 2-altitude3- (position_count / 16) * 80, 1);
                    }
                    parent.tile[i, altitude3] = 10;
                    if(altitude3 == y+5)
                    {

                    }
                    else
                    {
                        if(playStyle == "dash" &&!is_wall)
                        {
                            if(Random.Range(0.0f,10.0f) < 3.0f)
                            {
                                wall_position = Random.Range(1,rand-1);
                                parent.tile[i + wall_position,altitude3-1] = 99;
                            }
                            is_wall = true;
                        }
                        else if(playStyle == "High_dash" &&!is_wall )
                        {
                            
                            wall_position = Random.Range(1,rand-1);
                            parent.tile[i + wall_position, altitude3 -1] = 99;
                            is_wall = true;
                        }
                    }
                }
            }
            monster_spawn =false;
        }
        if(map_count%4 == 3) 
        {
            // Debug.Log(map_count);
            spawn_list[map_count/4].start = temp_spawn;
            spawn_list[map_count/4].end = spawn_count;
            // Debug.Log(temp_spawn+ " " + spawn_count + " " + spawn_list[map_count/4].start + " " + spawn_list[map_count/4].end);
        }
        map_count++;
        position_count++;

    }



    private void ConnectRooms(Map_Node parent, TileNode root)
    {
        if (root.leftNode == null || root.rightNode == null)
        {
            return;
        }

        ConnectRooms(parent, root.leftNode);
        ConnectRooms(parent, root.rightNode);

        TileNode left = root.leftNode;
        TileNode right = root.rightNode;

        int leftCenterX = left.x + left.width / 2;
        int leftCenterY = left.y + left.height / 2;
        int rightCenterX = right.x + right.width / 2;
        int rightCenterY = right.y + right.height / 2;

        if (leftCenterY < rightCenterY)
        {
            for (int j = leftCenterY; j <= rightCenterY; j++)
            {
                if (parent.tile[rightCenterX, j] != 10)
                    parent.tile[rightCenterX, j] = 11;
            }
        }
        else
        {
            for (int j = rightCenterY; j <= leftCenterY; j++)
            {
                if (parent.tile[leftCenterX, j] != 10)
                    parent.tile[leftCenterX, j] = 11;
            }
        }
        if (leftCenterX < rightCenterX)
        {
            for (int i = leftCenterX; i <= rightCenterX; i++)
            {
                if (parent.tile[i, leftCenterY] != 10)
                    parent.tile[i, leftCenterY] = 11;
            }
        }
        else
        {
            for (int i = rightCenterX; i <= leftCenterX; i++)
            {
                if (parent.tile[i, rightCenterY] != 10)
                    parent.tile[i, rightCenterY] = 11;
            }
        }


    }


    public void MakeRoad(Map_Node parent, TileNode left, TileNode right)
    {
        int a = Random.Range(0, 4);
        int leftCenterX, leftCenterY, rightCenterX, rightCenterY;

        switch (a)
        {
            case 0:
                left = left.leftNode;
                right = right.leftNode;
                break;
            case 1:
                left = left.leftNode;
                right = right.rightNode;
                break;
            case 2:
                left = left.rightNode;
                right = right.leftNode;
                break;
            case 3:
                left = left.rightNode;
                right = right.rightNode;
                break;
            default:
                return;
        }
        leftCenterX = left.x + left.width / 2;
        leftCenterY = left.y + left.height / 2;
        rightCenterX = right.x + right.width / 2;
        rightCenterY = right.y + right.height / 2;
        for (int i = leftCenterX; i <= rightCenterX; i++)
        {
            if (parent.tile[i, leftCenterY] != 10) parent.tile[i, leftCenterY] = 11;
        }

        if (leftCenterY < rightCenterY)
        {
            for (int j = leftCenterY; j <= rightCenterY; j++)
            {
                if (parent.tile[rightCenterX, j] == 0) parent.tile[rightCenterX, j] = 11;
            }
        }
        else
        {
            for (int j = rightCenterY; j <= leftCenterY; j++)
            {
                if (parent.tile[leftCenterX, j] == 0) parent.tile[leftCenterX, j] = 11;
            }
        }

    }
    private void AddTilesInRange(int[,] tileArray, int startX, int endX, int startY, int endY, int value)
    {
        for (int i = startX; i <= endX; i++)
        {
            for (int j = startY; j <= endY; j++)
            {
                if (tileArray[i, j] != 10) tileArray[i, j] = value;
            }
        }
    }
    public void Horiontal_add(Map_Node parent_Left, Map_Node parent_Right, TileNode left, TileNode right)
    {
        int a = Random.Range(0, 4);
        int leftCenterX, leftCenterY, rightCenterX, rightCenterY;

        switch (a)
        {
            case 0:
                left = left.leftNode;
                right = right.leftNode;
                break;
            case 1:
                left = left.leftNode;
                right = right.rightNode;
                break;
            case 2:
                left = left.rightNode;
                right = right.leftNode;
                break;
            case 3:
                left = left.rightNode;
                right = right.rightNode;
                break;
            default:
                return;
        }
        leftCenterX = left.x + left.width / 2;
        leftCenterY = left.y + left.height / 2;
        rightCenterX = right.x + right.width / 2;
        rightCenterY = right.y + right.height / 2;
        for (int x = leftCenterX; x < 80; x++)
        {
            if (parent_Left.tile[x, leftCenterY] != 10)
            {
                parent_Left.tile[x, leftCenterY] = 11;
                parent_Left.tile[x, leftCenterY+1] = 11;
            }
                
        }
        if (leftCenterY > rightCenterY)
        {
            for (int y = rightCenterY; y <= leftCenterY; y++)
                parent_Left.tile[79, y] = 11;
        }
        else
        {
            for (int y = leftCenterY; y <= rightCenterY; y++)
            {
                if(y == rightCenterY)
                {
                    parent_Left.tile[79, y+1] = 11;
                }
                parent_Left.tile[79, y] = 11;
            }
                
        }
        for (int x = 0; x <= rightCenterX; x++)
            if (parent_Right.tile[x, rightCenterY] != 10)
            {
                parent_Right.tile[x, rightCenterY] = 11;
                parent_Right.tile[x, rightCenterY+1] = 11;
            } 
    }

    public void Vertical_add(Map_Node parent_Up, Map_Node parent_Down, TileNode up, TileNode down)
    {
        int a = Random.Range(0, 4);
        int UpCenterX, UpCenterY, DownCenterX, DownCenterY;
        switch (a)
        {
            case 0:
                up = up.leftNode.rightNode;
                down = down.leftNode.leftNode;
                break;
            case 1:
                up = up.leftNode.rightNode;
                down = down.rightNode.leftNode;
                break;
            case 2:
                up = up.rightNode.rightNode;
                down = down.leftNode.leftNode;
                break;
            case 3:
                up = up.rightNode.rightNode;
                down = down.rightNode.leftNode;
                break;
            default:
                return;
        }
        UpCenterX = up.x + up.width / 2;
        UpCenterY = up.y + up.height / 2;
        DownCenterX = down.x + down.width / 2;
        DownCenterY = down.y + down.height / 2;
        for (int y = UpCenterY; y < 80; y++)
            if (parent_Up.tile[UpCenterX, y] != 10) parent_Up.tile[UpCenterX, y] = 11;
        if (UpCenterX < DownCenterX)
        {
            for (int x = UpCenterX; x <= DownCenterX; x++)
                parent_Up.tile[x, 79] = 11;
        }
        else
        {
            for (int x = DownCenterX; x <= UpCenterX; x++)
                parent_Up.tile[x, 79] = 11;
        }
        for (int y = 0; y <= DownCenterY; y++)
            if (parent_Down.tile[DownCenterX, y] != 10) parent_Down.tile[DownCenterX, y] = 11;
    }
    public void Reset_value()
    {
        ObjectPool.instance.Reset_Monster();
        Destroy(newPortal);
        is_exit = false;
        is_spawn=false;
        spawn_count = 0;
        position_count = 0;
        map_count = 0;
        temp_spawn = 0;
    }
}