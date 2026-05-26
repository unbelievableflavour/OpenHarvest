using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XNode;

public class NpcChatTreePresenter : MonoBehaviour
{
    [SerializeField] private Text bodyText;
    [SerializeField] private RectTransform choiceRoot;
    [SerializeField] private Button choiceButtonTemplate;
    [SerializeField] private Button closeButton;

    private readonly List<GameObject> _spawnedChoiceRows = new List<GameObject>();
    private NpcChatSession _session;
    private NPCController _npc;
    private bool _singleLinePending;
    private string _singleLineBody;
    private bool _singleLineShowContinue;
    private Func<bool> _singleLineContinueOverride;

    private void Awake()
    {
        HideChoiceTemplate();

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(CloseChat);
        }
    }

    public void Begin(NpcChatNode startNode, NPCController npc)
    {
        HideChoiceTemplate();

        _npc = npc;
        _singleLinePending = false;
        _singleLineBody = string.Empty;
        _singleLineShowContinue = false;
        _singleLineContinueOverride = null;
        _session = new NpcChatSession(startNode);
        gameObject.SetActive(true);
        RefreshView();
    }

    public void BeginSingleLine(
        string body,
        NPCController npc,
        bool showContinue = true,
        Func<bool> onContinueOverride = null)
    {
        HideChoiceTemplate();

        _npc = npc;
        _session = null;
        _singleLinePending = true;
        _singleLineBody = body ?? string.Empty;
        _singleLineShowContinue = showContinue;
        _singleLineContinueOverride = onContinueOverride;
        gameObject.SetActive(true);
        RefreshView();
    }

    public void CloseChat()
    {
        EventManager.Emit("interactionUIToMain");
    }

    private void HideChoiceTemplate()
    {
        if (choiceButtonTemplate != null)
        {
            choiceButtonTemplate.gameObject.SetActive(false);
        }
    }

    private void RefreshView()
    {
        ClearChoiceRows();

        if (_singleLinePending)
        {
            ApplyBodyText(_singleLineBody);

            SpeakLine(_singleLineBody);
            if (_singleLineShowContinue)
            {
                AddChoiceRow("Continue", OnSingleLineContinue);
            }
            RebuildChatLayout();
            return;
        }

        if (_session == null || _session.IsFinished)
        {
            CloseChat();
            return;
        }

        if (!_session.TryGetCurrentNode(out NpcChatNode node))
        {
            CloseChat();
            return;
        }

        ApplyBodyText(node.body);

        SpeakLine(node.body);

        if (node.ChoiceCount > 0)
        {
            for (int i = 0; i < node.ChoiceCount; i++)
            {
                int index = i;
                string label = node.GetChoiceLabel(i);
                AddChoiceRow(label, () => OnChose(index));
            }
        }

        RebuildChatLayout();
    }

    private void ApplyBodyText(string content)
    {
        if (bodyText == null)
        {
            return;
        }

        AutoType autoType = bodyText.GetComponent<AutoType>();
        if (autoType == null)
        {
            bodyText.text = content;
            return;
        }

        autoType.StopAllCoroutines();
        autoType.ResetText(content);
        autoType.Refresh();
    }

    private void RebuildChatLayout()
    {
        if (choiceRoot != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(choiceRoot);
        }

        RectTransform panelRect = transform as RectTransform;
        if (panelRect != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(panelRect);
        }
    }

    private void OnSingleLineContinue()
    {
        _singleLinePending = false;
        _singleLineBody = string.Empty;
        _singleLineShowContinue = false;

        Func<bool> continueOverride = _singleLineContinueOverride;
        _singleLineContinueOverride = null;
        if (continueOverride != null && continueOverride.Invoke())
        {
            return;
        }

        CloseChat();
    }

    private void OnChose(int choiceIndex)
    {
        if (_session == null)
        {
            return;
        }

        if (!_session.TryChoose(choiceIndex, out bool ended))
        {
            return;
        }

        if (ended || _session.IsFinished)
        {
            CloseChat();
            return;
        }

        RefreshView();
    }

    private void AddChoiceRow(string label, UnityEngine.Events.UnityAction onClick)
    {
        if (choiceButtonTemplate == null || choiceRoot == null)
        {
            return;
        }

        Button row = Instantiate(choiceButtonTemplate, choiceRoot);
        row.gameObject.SetActive(true);
        row.onClick.RemoveAllListeners();
        row.onClick.AddListener(onClick);

        Text text = row.GetComponentInChildren<Text>(true);
        if (text != null)
        {
            text.text = label;
        }

        _spawnedChoiceRows.Add(row.gameObject);
    }

    private void ClearChoiceRows()
    {
        for (int i = 0; i < _spawnedChoiceRows.Count; i++)
        {
            if (_spawnedChoiceRows[i] != null)
            {
                Destroy(_spawnedChoiceRows[i]);
            }
        }

        _spawnedChoiceRows.Clear();
    }

    private void SpeakLine(string line)
    {
        if (string.IsNullOrEmpty(line) || _npc == null)
        {
            return;
        }

        var voice = _npc.GetComponent<NPCVoice>();
        if (voice == null)
        {
            return;
        }

        voice.Speak(line);
    }

}
