using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LogConsoleUIController : MonoBehaviour
{
    [SerializeField] private Text logText;
    [SerializeField] private ScrollRect logScrollRect;
    [SerializeField] private int maxEntries = 200;
    [SerializeField] private bool autoScrollToBottom = true;

    private readonly Queue<string> _entries = new Queue<string>();
    private readonly StringBuilder _builder = new StringBuilder(4096);
    private bool _isDirty;

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLogMessageReceived;
        _isDirty = true;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLogMessageReceived;
    }

    private void Update()
    {
        if (!_isDirty)
        {
            return;
        }

        RebuildLogText();
        _isDirty = false;

        if (autoScrollToBottom && logScrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            logScrollRect.verticalNormalizedPosition = 0f;
        }
    }

    public void ClearLogs()
    {
        _entries.Clear();
        _isDirty = true;
    }

    private void HandleLogMessageReceived(string condition, string stackTrace, LogType type)
    {
        string prefix = type == LogType.Error || type == LogType.Assert || type == LogType.Exception
            ? "[ERR]"
            : type == LogType.Warning
                ? "[WRN]"
                : "[LOG]";

        _entries.Enqueue(prefix + " " + condition);
        while (_entries.Count > maxEntries)
        {
            _entries.Dequeue();
        }

        _isDirty = true;
    }

    private void RebuildLogText()
    {
        if (logText == null)
        {
            return;
        }

        _builder.Clear();
        foreach (string entry in _entries)
        {
            _builder.AppendLine(entry);
        }

        logText.text = _builder.ToString();
    }
}
