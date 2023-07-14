namespace FarmStation.Services;

public class TokenProvider
{
    //https://app.pluralsight.com/course-player?clipId=a95e47f6-2661-4e6d-8c36-0c65c07040f3
    public string XsrfToken { get; set; }
}

public class InitialApplicationState
{
    public string XsrfToken { get; set; }
}
