using System;

[Serializable]
public class DialogueData
{
    public string GroupID;  // 대사 묶음 키값
    public int Order;       // 출력 순서
    public string Speaker;  // 화자 이름
    public string Text; // 대사 본문
    public string Portrait;  // 표정 이미지
    public string SoundEffect; // 효과음

    public DialogueData(string groupId, int order, string speaker, string text, string portrait, string soundEffect)
    {
        this.GroupID = groupId;
        this.Order = order;
        this.Speaker = speaker;
        this.Text = text;
        this.Portrait = portrait;
        this.SoundEffect = soundEffect;

    }
}