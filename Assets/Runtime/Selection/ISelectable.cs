
public interface ISelectable
{
    void Select(bool i_animated);

    void Unselect(bool i_animated);

    bool IsSelected { get; }

    //void MarkForSelection(bool i_animated);

    //void UnmarkForSelection(bool i_animated);
}
