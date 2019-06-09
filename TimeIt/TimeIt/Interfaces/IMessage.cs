namespace TimeIt.Interfaces
{
    //TODO: I SHOULD RENAME THIS TO INAPPNOTIFICATION
    public interface ISimpleMessage
    {
        void ShowMessage(string message, bool longDelay = false);
    }
}
