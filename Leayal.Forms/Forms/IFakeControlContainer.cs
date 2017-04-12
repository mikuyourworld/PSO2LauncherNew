namespace Leayal.Forms
{
    interface IFakeControlContainer
    {
        FakeControlCollection Controls { get; }

        event FakeControlEventHandler ControlAdded;
        event FakeControlEventHandler ControlRemoved;
    }
}
