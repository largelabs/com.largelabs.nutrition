
public interface IAppear
{
    bool IsAppearInit { get; }
    void Appear(bool i_animated);

    void Disappear(bool i_animated);

    bool IsAppearing { get; }

    bool IsDisappearing { get; }
}