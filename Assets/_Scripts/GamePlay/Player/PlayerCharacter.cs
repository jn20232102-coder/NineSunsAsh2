public class PlayerCharacter : CharacterBase
{
    // 玩家专属逻辑（复活、回营地、背包、广播到 UI 等）放这里
    protected override void OnDeathInternal()
    {
        base.OnDeathInternal();
        // TODO: 通知 RunFlow/打开失败UI/复活倒计时等
    }
}
