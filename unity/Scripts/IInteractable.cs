namespace CampusNavigator
{
    public interface IInteractable
    {
        string GetPrompt();
        void Interact(PlayerInteraction player);
    }
}
