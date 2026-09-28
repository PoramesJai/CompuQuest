using System.Collections.Generic;
using UnityEngine;

namespace CompuQuest.LevelDesign
{
    /// <summary>
    /// สร้าง Grey-box (บล็อกกล่องหยาบ) ของด่านอัตโนมัติตามผัง Template ที่ใช้ร่วมกันทั้ง 3 ด่าน
    /// (จุดเริ่มต้น → โซนเก็บของ → โซนประกอบ/วาง → Boss Quiz → ทางออก)
    /// ใช้พิกัดเดียวกับผังพื้นและแบบจำลอง 3 มิติที่แสดงในแชท เพียงแปลงหน่วยจาก px เป็นเมตรจริง
    ///
    /// วิธีใช้: สร้าง Empty GameObject ชื่อ "LevelBlockout" ในแต่ละฉากด่าน (Stage1/Stage2/Stage3)
    /// เพิ่ม Component นี้ → เลือก Stage ที่ตรงกับฉากใน Inspector
    /// คลิกขวาที่หัว Component (หรือปุ่ม ⋮ มุมขวาบน) → เลือกเมนู "Build Blockout"
    /// เมื่อโมเดล 3D จริงจากทีม Blender พร้อมแล้ว ให้เรียก "Clear Blockout" แล้วนำโมเดลจริงมาวางแทน
    /// </summary>
    public class LevelBlockout : MonoBehaviour
    {
        public enum Stage { Stage1_CPU, Stage2_RepairShop, Stage3_IODevices }

        [Header("เลือกด่านที่ตรงกับฉากนี้")]
        public Stage stage = Stage.Stage1_CPU;

        [Header("รูปแบบการสร้าง")]
        [Tooltip("true = สร้างเป็นกล่องทึบแทนปริมาตรห้อง (ดูภาพรวมง่าย)\nfalse = สร้างเฉพาะพื้นเดินได้ (เหมาะกับตอนอยากเดินทดสอบจริงใน Play Mode)")]
        public bool solidVolumes = true;

        [Tooltip("ความหนาของพื้นเวลาเลือกโหมด 'เฉพาะพื้น' (เมตร)")]
        public float floorThickness = 0.2f;

        private const string ContainerName = "Blockout_Generated";

        // ---- นิยามห้องแบบ Template (ใช้ร่วมกันทั้ง 3 ด่าน ตำแหน่ง/ขนาดเหมือนกันทุกด่าน) ----
        private struct RoomDef
        {
            public string key;   // "start" "collect" "place" "quiz" "exit"
            public float x, z, w, d, h;
            public Color color;
            public RoomDef(string key, float x, float z, float w, float d, float h, Color color)
            { this.key = key; this.x = x; this.z = z; this.w = w; this.d = d; this.h = h; this.color = color; }
        }

        private struct BoxDef
        {
            public float x, z, w, d, h, y0;
            public Color color;
            public BoxDef(float x, float z, float w, float d, float h, Color color, float y0 = 0f)
            { this.x = x; this.z = z; this.w = w; this.d = d; this.h = h; this.color = color; this.y0 = y0; }
        }

        private static readonly Color ColGray = new Color(0.70f, 0.70f, 0.66f);
        private static readonly Color ColTeal = new Color(0.36f, 0.79f, 0.65f);
        private static readonly Color ColPurple = new Color(0.69f, 0.66f, 0.93f);
        private static readonly Color ColCoral = new Color(0.94f, 0.60f, 0.48f);
        private static readonly Color ColCorridor = new Color(0.83f, 0.82f, 0.78f);
        private static readonly Color ColMarkerCollect = new Color(0.06f, 0.43f, 0.34f);
        private static readonly Color ColMarkerPlace = new Color(0.33f, 0.29f, 0.72f);

        // ห้องหลัก 5 ห้อง เรียงแบบซิกแซกเหมือนผังที่ใช้ตกลงกันไว้: แถวบน 3 ห้อง แถวล่าง 2 ห้อง
        private List<RoomDef> GetRooms() => new List<RoomDef>
        {
            new RoomDef("start",   0f,   0f, 7f, 4.5f, 3f, ColGray),
            new RoomDef("collect", 9f,   0f, 7f, 4.5f, 4f, ColTeal),
            new RoomDef("place",   18f,  0f, 7f, 4.5f, 4f, ColPurple),
            new RoomDef("quiz",    18f,  6.5f, 7f, 4.5f, 5f, ColCoral),
            new RoomDef("exit",    0f,   6.5f, 7f, 4.5f, 3f, ColGray),
        };

        // ทางเดินเชื่อมห้อง (กว้าง 2 เมตร พอสำหรับ Character Controller เดินสบาย)
        private List<BoxDef> GetCorridors() => new List<BoxDef>
        {
            new BoxDef(7f,   1.25f, 2f, 2f, 0.4f, ColCorridor),   // start -> collect
            new BoxDef(16f,  1.25f, 2f, 2f, 0.4f, ColCorridor),   // collect -> place
            new BoxDef(20.5f, 4.5f, 2f, 2f, 0.4f, ColCorridor),   // place -> quiz (แนวตั้ง)
            new BoxDef(7f,   7.75f, 11f, 2f, 0.4f, ColCorridor),  // quiz -> exit
        };

        // เสาหมุดจุดวางไอเทมเก็บได้ 4 จุด ในห้อง collect (ตำแหน่งสัมพัทธ์ในห้อง)
        private List<BoxDef> GetCollectMarkers(RoomDef room)
        {
            float[] localX = { 1f, 2.8f, 4.6f, 6.4f };
            var list = new List<BoxDef>();
            foreach (var lx in localX)
                list.Add(new BoxDef(room.x + lx, room.z + 2.25f, 0.3f, 0.3f, 1.2f, ColMarkerCollect));
            return list;
        }

        // เสาหมุดตำแหน่งช่องเสียบ 4 ช่อง ในห้อง place
        private List<BoxDef> GetPlaceMarkers(RoomDef room)
        {
            float[] localX = { 1f, 2.8f, 4.6f, 6.4f };
            var list = new List<BoxDef>();
            foreach (var lx in localX)
                list.Add(new BoxDef(room.x + lx, room.z + 2.25f, 0.3f, 0.3f, 1.2f, ColMarkerPlace));
            return list;
        }

        // ---- ป้ายชื่อโซนของแต่ละด่าน (เนื้อหาต่างกัน แต่ตำแหน่ง/รูปทรงเหมือนกันทุกด่าน) ----
        private Dictionary<string, string> GetLabels()
        {
            switch (stage)
            {
                case Stage.Stage1_CPU:
                    return new Dictionary<string, string> {
                        {"start","จุดเริ่มต้น (ถูกดูดเข้ามาใน CPU)"},
                        {"collect","โซนเก็บชิ้นส่วน (ALU, CU, Register, Cache)"},
                        {"place","แท่นวางชิ้นส่วน"},
                        {"quiz","Boss Quiz"},
                        {"exit","ทางออก (ประตูออกจาก CPU)"}
                    };
                case Stage.Stage2_RepairShop:
                    return new Dictionary<string, string> {
                        {"start","จุดเริ่มต้น (สำรวจคอมที่พัง)"},
                        {"collect","ร้านอุปกรณ์ (SSD, GPU, Mainboard, PSU)"},
                        {"place","ประกอบเคสคอม"},
                        {"quiz","Boss Quiz"},
                        {"exit","ทางออก (ไฟในเคสสว่างขึ้น)"}
                    };
                default: // Stage3
                    return new Dictionary<string, string> {
                        {"start","จุดเริ่มต้น (หาอุปกรณ์ I/O ที่ขาด)"},
                        {"collect","ร้านอุปกรณ์ I/O (คีย์บอร์ด เมาส์ จอ ลำโพง)"},
                        {"place","จุดเสียบพอร์ต"},
                        {"quiz","เปิดเครื่อง (คำถามรวม 5 ข้อ)"},
                        {"exit","จบเกม (ผ่าน 60% ขึ้นไป)"}
                    };
            }
        }

        [ContextMenu("Build Blockout")]
        public void BuildBlockout()
        {
            ClearBlockout();

            Transform container = new GameObject(ContainerName).transform;
            container.SetParent(transform, false);

            var labels = GetLabels();
            foreach (var room in GetRooms())
            {
                string label = labels.TryGetValue(room.key, out var l) ? l : room.key;
                GameObject go = solidVolumes
                    ? MakeBox(container, room.x, 0f, room.z, room.w, room.h, room.d, room.color, $"Room_{room.key}")
                    : MakeBox(container, room.x, 0f, room.z, room.w, floorThickness, room.d, room.color, $"Room_{room.key}");
                go.name += $" — {label}";

                if (room.key == "collect")
                    foreach (var m in GetCollectMarkers(room))
                        MakeBox(container, m.x, m.y0, m.z, m.w, m.h, m.d, m.color, "ItemSpawnPoint");

                if (room.key == "place")
                    foreach (var m in GetPlaceMarkers(room))
                        MakeBox(container, m.x, m.y0, m.z, m.w, m.h, m.d, m.color, "SlotPoint");
            }

            foreach (var c in GetCorridors())
                MakeBox(container, c.x, 0f, c.z, c.w, floorThickness, c.d, c.color, "Corridor");

            Debug.Log($"[LevelBlockout] สร้าง Grey-box สำหรับ {stage} เรียบร้อย ({(solidVolumes ? "ปริมาตรทึบ" : "เฉพาะพื้น")})");
        }

        [ContextMenu("Clear Blockout")]
        public void ClearBlockout()
        {
            Transform old = transform.Find(ContainerName);
            if (old != null)
            {
                if (Application.isPlaying) Destroy(old.gameObject);
                else DestroyImmediate(old.gameObject);
            }
        }

        private GameObject MakeBox(Transform parent, float x, float y0, float z, float w, float h, float d, Color color, string name)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = new Vector3(x + w / 2f, y0 + h / 2f, z + d / 2f);
            go.transform.localScale = new Vector3(w, h, d);

            var renderer = go.GetComponent<Renderer>();
            var mat = new Material(Shader.Find("Standard")) { color = color };
            renderer.sharedMaterial = mat;

            return go;
        }
    }
}
