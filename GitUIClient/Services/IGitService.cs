namespace GitUIClient.Services;

public interface IGitService
{
    string GetStatus();
    string GetLog(int count = 5);
    void Add(string path = ".");
    string Commit(string message);
}