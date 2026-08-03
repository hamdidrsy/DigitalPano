namespace DigitalPano.Web.Models.Admin;

public sealed record DashboardViewModel(
    int ActiveAnnouncementCount,
    int ScheduledAnnouncementCount,
    int ExpiredAnnouncementCount,
    int ActiveScreenCount,
    int OnlineScreenCount,
    int EmergencyAnnouncementCount);
