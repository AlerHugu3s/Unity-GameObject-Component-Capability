#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityGCC.Capabilities;

namespace UnityGCC.Editor
{
    public class CapabilitiesMonitorWindow : EditorWindow
    {
        private Vector2 scrollPosition;
        private Vector2 timelineScrollPosition;
        private Dictionary<GameObject, Dictionary<TickGroup, List<BaseCapabilities>>> groupedCapabilities;
        private bool autoRefresh = true;
        private float refreshInterval = 0.1f;
        private double lastRefreshTime;

        // Timeline功能
        private bool showTimeline = true;
        private float timelineSeconds = 10f;
        private List<CapabilityStateEvent> stateEvents = new List<CapabilityStateEvent>();
        private Dictionary<BaseCapabilities, bool> lastKnownStates = new Dictionary<BaseCapabilities, bool>();
        private int currentFrame = 0;

        // Timeline视觉设置
        private float timelineHeight = 200f;
        private Color activeColor = Color.green;
        private Color inactiveColor = Color.red;
        private Color backgroundTimelineColor = new Color(0.2f, 0.2f, 0.2f);

        [System.Serializable]
        public class CapabilityStateEvent
        {
            public BaseCapabilities capability;
            public bool isActive;
            public float time;
            public int frame;
            public string capabilityName;
            public string ownerName;

            public CapabilityStateEvent(BaseCapabilities cap, bool active, float timestamp, int frameNumber)
            {
                capability = cap;
                isActive = active;
                time = timestamp;
                frame = frameNumber;
                capabilityName = cap.GetType().Name;
                ownerName = cap.Owner?.name ?? "Unknown";
            }
        }

        [MenuItem("UnityGCC/Capabilities Monitor")]
        public static void ShowWindow()
        {
            GetWindow<CapabilitiesMonitorWindow>("Capabilities Monitor");
        }

        private void OnEnable()
        {
            EditorApplication.update += OnEditorUpdate;
        }

        private void OnDisable()
        {
            EditorApplication.update -= OnEditorUpdate;
        }

        private void OnEditorUpdate()
        {
            if (autoRefresh && EditorApplication.timeSinceStartup - lastRefreshTime > refreshInterval)
            {
                Repaint();
                lastRefreshTime = EditorApplication.timeSinceStartup;
            }
        }

        private void OnGUI()
        {
            DrawHeader();

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("需要在Play Mode下才能查看Capabilities状态", MessageType.Info);
                return;
            }

            if (CapabilitiesController.Instance == null)
            {
                EditorGUILayout.HelpBox("未找到CapabilitiesController实例", MessageType.Warning);
                return;
            }

            RefreshGroupedCapabilities();

            // Timeline视图
            if (showTimeline)
            {
                DrawTimelineHeader();
                DrawTimeline();
                EditorGUILayout.Space();
            }

            DrawCapabilitiesTree();
        }

        private void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Capabilities Monitor", EditorStyles.boldLabel);

            GUILayout.FlexibleSpace();

            autoRefresh = EditorGUILayout.Toggle("自动刷新", autoRefresh, GUILayout.Width(80));

            if (GUILayout.Button("手动刷新", GUILayout.Width(80)))
            {
                Repaint();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            refreshInterval = EditorGUILayout.Slider("刷新间隔(秒)", refreshInterval, 0.05f, 1f);
            EditorGUILayout.EndHorizontal();

            // Timeline控制
            EditorGUILayout.BeginHorizontal();
            showTimeline = EditorGUILayout.Toggle("显示Timeline", showTimeline, GUILayout.Width(100));

            if (showTimeline)
            {
                EditorGUILayout.LabelField("时间范围:", GUILayout.Width(60));
                timelineSeconds = EditorGUILayout.Slider(timelineSeconds, 5f, 60f, GUILayout.Width(150));
                EditorGUILayout.LabelField("秒", GUILayout.Width(20));

                if (GUILayout.Button("清空历史", GUILayout.Width(80)))
                {
                    stateEvents.Clear();
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
        }

        private void RefreshGroupedCapabilities()
        {
            groupedCapabilities = new Dictionary<GameObject, Dictionary<TickGroup, List<BaseCapabilities>>>();
            currentFrame = Time.frameCount;
            float currentTime = Time.time;

            if (CapabilitiesController.Instance?.Capabilities == null) return;

            foreach (var tickGroupPair in CapabilitiesController.Instance.Capabilities)
            {
                var tickGroup = tickGroupPair.Key;
                var capabilities = tickGroupPair.Value;

                foreach (var capability in capabilities)
                {
                    if (capability?.Owner == null) continue;

                    var owner = capability.Owner;

                    if (!groupedCapabilities.ContainsKey(owner))
                    {
                        groupedCapabilities[owner] = new Dictionary<TickGroup, List<BaseCapabilities>>();
                    }

                    if (!groupedCapabilities[owner].ContainsKey(tickGroup))
                    {
                        groupedCapabilities[owner][tickGroup] = new List<BaseCapabilities>();
                    }

                    groupedCapabilities[owner][tickGroup].Add(capability);

                    // 检测状态变化并记录事件
                    if (showTimeline)
                    {
                        CheckAndRecordStateChange(capability, currentTime, currentFrame);
                    }
                }
            }

            // 清理过期的事件
            if (showTimeline)
            {
                CleanupOldEvents(currentTime);
            }
        }

        private void CheckAndRecordStateChange(BaseCapabilities capability, float currentTime, int frame)
        {
            bool currentState = capability.bActive;

            if (lastKnownStates.ContainsKey(capability))
            {
                bool lastState = lastKnownStates[capability];
                if (lastState != currentState)
                {
                    // 状态发生变化，记录事件
                    stateEvents.Add(new CapabilityStateEvent(capability, currentState, currentTime, frame));
                    lastKnownStates[capability] = currentState;
                }
            }
            else
            {
                // 首次记录
                lastKnownStates[capability] = currentState;
                stateEvents.Add(new CapabilityStateEvent(capability, currentState, currentTime, frame));
            }
        }

        private void CleanupOldEvents(float currentTime)
        {
            float cutoffTime = currentTime - timelineSeconds;
            stateEvents.RemoveAll(e => e.time < cutoffTime);
        }

        private void DrawCapabilitiesTree()
        {
            if (groupedCapabilities == null || groupedCapabilities.Count == 0)
            {
                EditorGUILayout.HelpBox("当前没有任何Capabilities", MessageType.Info);
                return;
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            foreach (var ownerPair in groupedCapabilities.OrderBy(x => x.Key.name))
            {
                var owner = ownerPair.Key;
                var tickGroups = ownerPair.Value;

                DrawOwnerHeader(owner);

                EditorGUI.indentLevel++;

                foreach (var tickGroupPair in tickGroups.OrderBy(x => (int)x.Key))
                {
                    var tickGroup = tickGroupPair.Key;
                    var capabilities = tickGroupPair.Value;

                    DrawTickGroupHeader(tickGroup);

                    EditorGUI.indentLevel++;

                    foreach (var capability in capabilities.OrderBy(x => x.GetType().Name))
                    {
                        DrawCapability(capability);
                    }

                    EditorGUI.indentLevel--;
                }

                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawOwnerHeader(GameObject owner)
        {
            EditorGUILayout.BeginHorizontal();

            // Owner名称
            EditorGUILayout.LabelField($"🎮 Owner: {owner.name}", EditorStyles.boldLabel);

            // 显示GameObject引用
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(owner, typeof(GameObject), true, GUILayout.Width(150));
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawTickGroupHeader(TickGroup tickGroup)
        {
            var style = new GUIStyle(EditorStyles.label)
            {
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.3f, 0.5f, 0.8f) }
            };

            EditorGUILayout.LabelField($"📋 TickGroup: {tickGroup}", style);
        }

        private void DrawCapability(BaseCapabilities capability)
        {
            EditorGUILayout.BeginHorizontal();

            // 状态指示器
            var statusColor = capability.bActive ? Color.green : Color.red;
            var statusText = capability.bActive ? "●" : "●";

            var statusStyle = new GUIStyle(EditorStyles.label)
            {
                normal = { textColor = statusColor },
                fontStyle = FontStyle.Bold,
                fontSize = 14
            };

            EditorGUILayout.LabelField(statusText, statusStyle, GUILayout.Width(20));

            var nameStyle = new GUIStyle(EditorStyles.label)
            {
                normal = { textColor = statusColor },
            };

            // Capability名称
            EditorGUILayout.LabelField(capability.GetType().Name, nameStyle);

            GUILayout.FlexibleSpace();

            // 状态信息
            var statusInfo = capability.bActive ? "Active" : "Inactive";
            var statusInfoColor = capability.bActive ? Color.green : Color.gray;

            var infoStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                normal = { textColor = statusInfoColor }
            };

            EditorGUILayout.LabelField(statusInfo, infoStyle, GUILayout.Width(60));

            // 持续时间
            if (capability.bActive && capability.ActiveDuration > 0)
            {
                EditorGUILayout.LabelField($"{capability.ActiveDuration:F1}s", EditorStyles.miniLabel, GUILayout.Width(40));
            }
            else if (!capability.bActive && capability.DeActiveDuration > 0)
            {
                EditorGUILayout.LabelField($"{capability.DeActiveDuration:F1}s", EditorStyles.miniLabel, GUILayout.Width(40));
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawTimelineHeader()
        {
            EditorGUILayout.LabelField("Timeline Log", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"当前帧: {currentFrame}", GUILayout.Width(100));
            EditorGUILayout.LabelField($"事件数量: {stateEvents.Count}", GUILayout.Width(100));
            EditorGUILayout.LabelField($"时间范围: {timelineSeconds:F1}秒", GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();
        }

        private void DrawTimeline()
        {
            if (stateEvents.Count == 0)
            {
                EditorGUILayout.HelpBox("暂无Timeline数据", MessageType.Info);
                return;
            }

            Rect timelineRect = GUILayoutUtility.GetRect(0, timelineHeight, GUILayout.ExpandWidth(true));

            // 绘制背景
            EditorGUI.DrawRect(timelineRect, backgroundTimelineColor);

            float currentTime = Time.time;
            float startTime = currentTime - timelineSeconds;

            // 绘制时间刻度
            DrawTimeScale(timelineRect, startTime, currentTime);

            // 按capability分组事件
            var groupedEvents = stateEvents
                .Where(e => e.time >= startTime)
                .GroupBy(e => e.capability)
                .ToList();

            if (groupedEvents.Count == 0) return;

            float rowHeight = (timelineRect.height - 30) / groupedEvents.Count; // 30是时间刻度的空间

            for (int i = 0; i < groupedEvents.Count; i++)
            {
                var group = groupedEvents[i];
                float rowY = timelineRect.y + 20 + i * rowHeight; // 20是顶部时间刻度空间

                DrawCapabilityTimeline(timelineRect, group.Key, group.ToList(), rowY, rowHeight, startTime, currentTime);
            }

            // 绘制当前时间线
            DrawCurrentTimeLine(timelineRect, startTime, currentTime);
        }

        private void DrawTimeScale(Rect timelineRect, float startTime, float currentTime)
        {
            // 绘制时间刻度背景
            Rect scaleRect = new Rect(timelineRect.x, timelineRect.y, timelineRect.width, 20);
            EditorGUI.DrawRect(scaleRect, new Color(0.3f, 0.3f, 0.3f));

            // 绘制时间标记
            int tickCount = 5;
            for (int i = 0; i <= tickCount; i++)
            {
                float t = (float)i / tickCount;
                float time = Mathf.Lerp(startTime, currentTime, t);
                float x = Mathf.Lerp(timelineRect.x, timelineRect.x + timelineRect.width, t);

                // 绘制刻度线
                Vector2 tickStart = new Vector2(x, timelineRect.y);
                Vector2 tickEnd = new Vector2(x, timelineRect.y + 15);
                Handles.color = Color.white;
                Handles.DrawLine(tickStart, tickEnd);

                // 绘制时间标签
                string timeLabel = (currentTime - time).ToString("F1") + "s";
                GUI.Label(new Rect(x - 15, timelineRect.y + 2, 30, 15), timeLabel,
                    new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleCenter, normal = { textColor = Color.white } });
            }
        }

        private void DrawCapabilityTimeline(Rect timelineRect, BaseCapabilities capability, List<CapabilityStateEvent> events,
            float rowY, float rowHeight, float startTime, float currentTime)
        {
            // 绘制capability名称
            string capabilityLabel = $"{capability.Owner?.name}/{capability.GetType().Name}";
            Rect labelRect = new Rect(timelineRect.x + 5, rowY, 200, rowHeight);
            GUI.Label(labelRect, capabilityLabel, new GUIStyle(EditorStyles.miniLabel)
            {
                normal = { textColor = Color.white },
                alignment = TextAnchor.MiddleLeft
            });

            // 绘制事件点
            foreach (var evt in events.OrderBy(e => e.time))
            {
                float normalizedTime = (evt.time - startTime) / (currentTime - startTime);
                float x = Mathf.Lerp(timelineRect.x + 200, timelineRect.x + timelineRect.width - 10, normalizedTime);

                Color eventColor = evt.isActive ? activeColor : inactiveColor;

                // 绘制事件点
                Rect eventRect = new Rect(x - 3, rowY + rowHeight * 0.3f, 6, rowHeight * 0.4f);
                EditorGUI.DrawRect(eventRect, eventColor);

                // 鼠标悬停显示详情
                if (eventRect.Contains(Event.current.mousePosition))
                {
                    string tooltip = $"Frame: {evt.frame}\nTime: {evt.time:F2}s\nState: {(evt.isActive ? "Active" : "Inactive")}";
                    GUI.Label(new Rect(Event.current.mousePosition.x + 10, Event.current.mousePosition.y - 10, 150, 60),
                        tooltip, EditorStyles.helpBox);
                }
            }

            // 绘制分隔线
            if (rowY + rowHeight < timelineRect.y + timelineRect.height)
            {
                Vector2 lineStart = new Vector2(timelineRect.x, rowY + rowHeight);
                Vector2 lineEnd = new Vector2(timelineRect.x + timelineRect.width, rowY + rowHeight);
                Handles.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                Handles.DrawLine(lineStart, lineEnd);
            }
        }

        private void DrawCurrentTimeLine(Rect timelineRect, float startTime, float currentTime)
        {
            // 绘制当前时间的垂直线
            float x = timelineRect.x + timelineRect.width - 10;
            Vector2 lineStart = new Vector2(x, timelineRect.y + 20);
            Vector2 lineEnd = new Vector2(x, timelineRect.y + timelineRect.height);
            Handles.color = Color.yellow;
            Handles.DrawLine(lineStart, lineEnd);

            // 添加"NOW"标签
            GUI.Label(new Rect(x - 15, timelineRect.y + timelineRect.height - 15, 30, 15), "NOW",
                new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleCenter, normal = { textColor = Color.yellow } });
        }
    }
}
#endif