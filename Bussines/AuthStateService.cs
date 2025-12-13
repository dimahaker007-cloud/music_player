public class AuthStateService
{
    public bool IsAuthenticated { get; private set; }
    public event Action OnChange;

    public void Login()
    {
        IsAuthenticated = true;
        NotifyStateChanged();
    }

    public void Logout()
    {
        IsAuthenticated = false;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}