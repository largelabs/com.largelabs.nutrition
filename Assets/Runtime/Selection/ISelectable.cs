
public interface ISelectable
{
    void MarkForSelection(bool i_animated);

    void UnmarkForSelection(bool i_animated);

    void Select(bool i_animated);

    void Unselect(bool i_animated);

    bool IsSelected { get; }
}
