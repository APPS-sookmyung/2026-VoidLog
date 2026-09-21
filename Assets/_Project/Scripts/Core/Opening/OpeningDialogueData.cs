using System;

[Serializable]
public class OpeningDialogueData
{
    public string GroupID;
    public int Order;
    public string Speaker;
    public string Text;
    public string Portrait;
    public string SoundEffect;
    public string PanelType;

    public OpeningDialogueData(string groupId, int order, string speaker, string text, string portrait, string soundEffect, string panelType)
    {
        this.GroupID = groupId;
        this.Order = order;
        this.Speaker = speaker;
        this.Text = text;
        this.Portrait = portrait;
        this.SoundEffect = soundEffect;
        this.PanelType = panelType;
    }
}