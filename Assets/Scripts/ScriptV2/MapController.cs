using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Text;

public class MapController : MonoBehaviour
{
    public int[,] map;
    public Tilemap tilemap;
    public Tilemap pelletTileMap;
    public List<TileBase> wallTiles;
    public TileBase pelletTile;
    public TileBase powerPelletTile;


    void Start()
    {
        map = new int[31, 28]; // Khởi tạo mảng 2 chiều 31 dòng x 28 cột
        LoadWallMap();
        LoadPelletMap();

        SaveMapAsJson(map);
        SaveMapAsTxt(map);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadWallMap()
    {
        // Lấy bounds thật sự của tilemap
        BoundsInt boundsTileMap = tilemap.cellBounds;

        // Tính offset giữa tọa độ trong tilemap và chỉ số mảng
        int offsetX = boundsTileMap.xMin; // thường là -14
        int offsetY = boundsTileMap.yMin; // thường là -15

        // Duyệt theo ma trận 31 dòng x 28 cột
        for (int row = 0; row < 31; row++)
        {
            for (int col = 0; col < 28; col++)
            {
                // Dịch sang tọa độ tilemap thực
                int tileX = col + offsetX + 7;
                int tileY = (30 - row) + offsetY; // Đảo row để hàng trên cùng là hàng 0

                Vector3Int cellPos = new Vector3Int(tileX, tileY, 0);
                TileBase tile = tilemap.GetTile(cellPos);

                if (tile != null && wallTiles.Contains(tile))
                {
                    map[row, col] = 1; // Tường
                    Debug.Log($"Tường tại ô ({row}, {col}) + ({tileX}, {tileY}) + tile: {tile.name}");
                }
                else
                {
                    map[row, col] = 0; // Mặc định
                }
            }
        }

        Debug.Log("LoadMap hoàn tất.");
    }

    public void LoadPelletMap()
    {
        BoundsInt boundsPelletMap = pelletTileMap.cellBounds;

        // Tính offset giữa tọa độ trong tilemap và chỉ số mảng
        int offsetX = boundsPelletMap.xMin; // thường là -14
        int offsetY = boundsPelletMap.yMin; // thường là -15
        Debug.Log($"offsetX: {offsetX}, offsetY: {offsetY}");

        // Duyệt theo ma trận 31 dòng x 28 cột
        for (int row = 0; row < 31; row++)
        {
            for (int col = 0; col < 28; col++)
            {
                // Dịch sang tọa độ tilemap thực
                int tileX = col + offsetX - 1;
                int tileY = (30 - row) + offsetY - 1; // Đảo row để hàng trên cùng là hàng 0

                Vector3Int cellPos = new Vector3Int(tileX, tileY, 0);
                TileBase tile = pelletTileMap.GetTile(cellPos);

                if (tile != null && tile == pelletTile)
                {
                    map[row, col] = 2; // Viên bi
                    Debug.Log($"Viên bi tại ô ({row}, {col}) + ({tileX}, {tileY}) + tile: {tile.name}");
                }
                else if (tile != null && tile == powerPelletTile)
                {
                    map[row, col] = 3; // Viên bi lớn
                    Debug.Log($"Viên bi lớn tại ô ({row}, {col}) + ({tileX}, {tileY}) + tile: {tile.name}");
                }
            }
        }

        Debug.Log("LoadPelletMap hoàn tất.");
    }

    public void SaveMapAsJson(int[,] map)
    {
        Debug.Log("Bắt đầu ghi map ra file JSON...");
        int rows = map.GetLength(0);
        int cols = map.GetLength(1);

        MapData mapData = new MapData();
        mapData.map = new List<RowData>();

        for (int i = 0; i < rows; i++)
        {
            RowData rowData = new RowData();
            rowData.row = new List<int>();
            for (int j = 0; j < cols; j++)
            {
                rowData.row.Add(map[i, j]);
            }
            mapData.map.Add(rowData);
        }

        Debug.Log("MapData: " + mapData.map.Count + " rows, " + mapData.map[0].row.Count + " cols");

        string json = JsonUtility.ToJson(mapData, true);
        string path = Path.Combine(Application.dataPath, "map.json");
        File.WriteAllText(path, json);
        Debug.Log("Đã ghi map ra file JSON: " + path);
    }


    [System.Serializable]
    public class MapData
    {
        public List<RowData> map;
    }

    [System.Serializable]
    public class RowData
    {
        public List<int> row;
    }
    
    public void SaveMapAsTxt(int[,] map)
    {
        StringBuilder sb = new StringBuilder();

        int rows = map.GetLength(0);
        int cols = map.GetLength(1);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                sb.Append(map[row, col]);
                if (col < cols - 1)
                    sb.Append(" ");
            }
            sb.AppendLine(); // Xuống dòng sau mỗi hàng
        }

        string path = Path.Combine(Application.dataPath, "map.txt");
        File.WriteAllText(path, sb.ToString());
        Debug.Log("Đã ghi map ra file TXT: " + path);
    }
}
