public static class Input
{
    public static InputActions Actions { get; }

    static Input()
    {
        Actions = new InputActions();
        Actions.Enable();
    }
}
