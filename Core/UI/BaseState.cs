using Terraria.UI;

namespace ChatPlus.Core.UI;

public class BaseState<TData> : UIState
{
    public BasePanel<TData> Panel { get; }
    public DescriptionPanel<TData> Description { get; }

    public BaseState(BasePanel<TData> panel, DescriptionPanel<TData> description)
    {
        Panel = panel;
        Append(panel);

        Description = description;
        Append(description);

        panel.ConnectedPanel = description;
        description.ConnectedPanel = panel;
    }

    public override void OnActivate()
    {
        base.OnActivate();
        Panel.ResetInit();
    }
}

